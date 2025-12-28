using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using CaesarAbstraction;
using McdAbstraction;
using Microsoft.VisualBasic.FileIO;

namespace SapiLayer1;

public sealed class ParameterCollection : LateLoadReadOnlyCollection<Parameter>, IDisposable
{
	private Dictionary<string, Parameter> cache;

	private Dictionary<string, Parameter> combinedCache;

	private Channel channel;

	private float progress;

	private VcpHelper vcpHelper;

	private bool disposed;

	private bool haveBeenReadFromEcu;

	private StringCollection accumulatorPrefixes;

	private StringDictionary groupCodingStrings;

	private StringDictionary originalGroupCodingStrings;

	private Dictionary<string, CodingStringState> groupCodingStringsState = new Dictionary<string, CodingStringState>();

	private object groupCodingStringsStateLock = new object();

	private bool verifyAfterWrite;

	private bool verifyAfterCommit;

	private bool serializeGroupNames;

	public bool AutoReadSummaryParameters { get; set; }

	public Channel Channel => channel;

	public Parameter this[string qualifier]
	{
		get
		{
			if (cache == null)
			{
				cache = this.DistinctBy((Parameter p) => p.Qualifier.ToUpperInvariant()).ToDictionary((Parameter p) => p.Qualifier, StringComparer.OrdinalIgnoreCase);
			}
			if (combinedCache == null)
			{
				combinedCache = this.DistinctBy((Parameter p) => p.CombinedQualifier.ToUpperInvariant()).ToDictionary((Parameter p) => p.CombinedQualifier, StringComparer.OrdinalIgnoreCase);
			}
			string value;
			bool flag = channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out value);
			Dictionary<string, Parameter> dictionary = (qualifier.Contains('.') ? combinedCache : cache);
			if (dictionary.TryGetValue(qualifier, out var value2) || (flag && dictionary.TryGetValue(value, out value2)))
			{
				return value2;
			}
			return null;
		}
	}

	public float Progress => progress;

	public bool HaveBeenReadFromEcu => haveBeenReadFromEcu;

	public bool ValuesLoadedFromLog { get; internal set; }

	public VcpComponentError VcpComponentError => vcpHelper.ComponentError;

	public bool VcpHadParameterError => vcpHelper.HadParameterError;

	public StringDictionary GroupCodingStrings
	{
		get
		{
			bool flag;
			lock (groupCodingStringsStateLock)
			{
				flag = groupCodingStringsState.Any((KeyValuePair<string, CodingStringState> kv) => (kv.Value & CodingStringState.NeedsUpdate) != 0);
			}
			if (flag)
			{
				UpdateCodingStrings();
			}
			return groupCodingStrings;
		}
	}

	public StringDictionary OriginalGroupCodingStrings => originalGroupCodingStrings;

	public bool VerifyAfterWrite
	{
		get
		{
			return verifyAfterWrite;
		}
		set
		{
			verifyAfterWrite = value;
		}
	}

	public bool VerifyAfterCommit => verifyAfterCommit;

	public bool SerializeGroupNames => serializeGroupNames;

	public event ParameterUpdateEventHandler ParameterUpdateEvent;

	public event ParametersReadCompleteEventHandler ParametersReadCompleteEvent;

	public event ParametersWriteCompleteEventHandler ParametersWriteCompleteEvent;

	public event ParametersProcessVcpCompleteEventHandler ParametersProcessVcpCompleteEvent;

	internal ParameterCollection(Channel c)
	{
		channel = c;
		verifyAfterWrite = channel.Ecu.Properties.GetValue("VerifyAfterWrite", defaultIfNotSet: true);
		verifyAfterCommit = channel.Ecu.Properties.GetValue("VerifyAfterCommit", defaultIfNotSet: false);
		serializeGroupNames = channel.Ecu.Properties.GetValue("SerializeParameterGroupNames", defaultIfNotSet: false);
		AutoReadSummaryParameters = true;
		vcpHelper = new VcpHelper(this);
		accumulatorPrefixes = new StringCollection();
		if (c.Ecu.Xml != null)
		{
			XmlNodeList xmlNodeList = c.Ecu.Xml.SelectNodes("Ecu/AccumulatorGroups/AccumulatorGroup");
			if (xmlNodeList != null)
			{
				for (int i = 0; i < xmlNodeList.Count; i++)
				{
					XmlNode xmlNode = xmlNodeList.Item(i);
					if (xmlNode != null)
					{
						XmlNode namedItem = xmlNode.Attributes.GetNamedItem("Qualifier");
						accumulatorPrefixes.Add(namedItem.InnerText);
					}
				}
			}
		}
		groupCodingStrings = new StringDictionary();
		originalGroupCodingStrings = new StringDictionary();
	}

	protected override void AcquireList()
	{
		if (channel.McdEcuHandle != null)
		{
			uint nParameterIndex = 0u;
			Dictionary<string, List<McdDBService>> dictionary = (from s in channel.McdEcuHandle.DBServices.Where((McdDBService s) => s.Semantic == "VARIANTCODINGREAD" || s.Semantic == "VARIANTCODINGWRITE").ToList()
				group s by s.Semantic).ToDictionary((IGrouping<string, McdDBService> g) => g.Key, (IGrouping<string, McdDBService> g) => g.ToList());
			if (!dictionary.ContainsKey("VARIANTCODINGREAD"))
			{
				return;
			}
			{
				foreach (McdDBService readPrimitive in dictionary["VARIANTCODINGREAD"])
				{
					McdDBService mcdDBService = dictionary["VARIANTCODINGWRITE"].FirstOrDefault((McdDBService s) => s.DefaultPdu.Take(readPrimitive.DefaultPdu.Count()).Skip(1).SequenceEqual(readPrimitive.DefaultPdu.Skip(1)));
					string domainQualifier = McdCaesarEquivalence.GetDomainQualifier(readPrimitive.Name);
					if (readPrimitive != null && mcdDBService != null)
					{
						groupCodingStrings.Add(domainQualifier, null);
						groupCodingStringsState.Add(domainQualifier, CodingStringState.None);
						CheckAndAddParameterGroup(domainQualifier, ref nParameterIndex, readPrimitive, mcdDBService);
					}
					else
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to locate all necessary services for varcoding group " + channel.Ecu.Name + "." + domainQualifier);
					}
				}
				return;
			}
		}
		if (channel.EcuHandle != null)
		{
			ServiceCollection serviceCollection = new ServiceCollection(channel, ServiceTypes.ReadVarCode | ServiceTypes.WriteVarCode);
			uint nParameterIndex2 = 0u;
			lock (channel.OfflineVarcodingHandleLock)
			{
				Varcode offlineVarcodingHandle = channel.OfflineVarcodingHandle;
				if (offlineVarcodingHandle != null)
				{
					StringCollection varCodeDomains = channel.EcuHandle.VarCodeDomains;
					for (int num = 0; num < varCodeDomains.Count; num++)
					{
						string text = varCodeDomains[num];
						groupCodingStrings.Add(text, null);
						groupCodingStringsState.Add(text, CodingStringState.None);
						CaesarDIVarCodeDom val = channel.EcuHandle.OpenVarCodeDomain(text);
						try
						{
							if (val != null)
							{
								Service readService = serviceCollection[val.ReadDefaultStringService];
								Service writeService = serviceCollection[val.WriteCodingStringService];
								CheckAndAddParameterGroup(offlineVarcodingHandle, val, text, ref nParameterIndex2, readService, writeService);
							}
						}
						finally
						{
							((IDisposable)val)?.Dispose();
						}
					}
				}
			}
			XmlNode xml = channel.Ecu.Xml;
			if (xml == null)
			{
				return;
			}
			XmlNodeList xmlNodeList = xml.SelectNodes("Ecu/ServicesAsParameters/ServiceAsParameter");
			if (xmlNodeList == null)
			{
				return;
			}
			Parameter parameter = null;
			for (int num2 = 0; num2 < xmlNodeList.Count; num2++)
			{
				XmlNode xmlNode = xmlNodeList.Item(num2);
				string innerText = xmlNode.Attributes.GetNamedItem("Qualifier").InnerText;
				string innerText2 = xmlNode.Attributes.GetNamedItem("Name").InnerText;
				XmlNode namedItem = xmlNode.Attributes.GetNamedItem("WriteReferenceQualifier");
				XmlNode namedItem2 = xmlNode.Attributes.GetNamedItem("ReadReferenceQualifier");
				XmlNode namedItem3 = xmlNode.Attributes.GetNamedItem("WriteReferenceIndex");
				XmlNode namedItem4 = xmlNode.Attributes.GetNamedItem("Hide");
				XmlNode namedItem5 = xmlNode.Attributes.GetNamedItem("Accumulator");
				string text2 = string.Empty;
				string text3 = string.Empty;
				int result = -1;
				bool flag = false;
				Service service = null;
				Service service2 = null;
				bool hide = false;
				bool persistable = true;
				if (namedItem != null)
				{
					text2 = namedItem.InnerText;
					service = channel.Services[text2];
					if (service != null)
					{
						if (namedItem3 != null)
						{
							if (int.TryParse(namedItem3.InnerText, out result))
							{
								if (result < 0 || result > service.InputValues.Count - 1)
								{
									flag = true;
									Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Service as VCP parameter {0} definition error - write reference index out of range {1}", innerText2, result));
								}
							}
							else
							{
								flag = true;
								Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Service as VCP parameter {0} definition error - write reference index could not be parsed.", innerText2));
							}
						}
						else
						{
							flag = true;
							Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Service as VCP parameter {0} definition error - write reference index does not exist, when write service specified", innerText2));
						}
					}
					else
					{
						flag = true;
						Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Service as VCP parameter definition error - write service {0} does not exist", text2));
					}
				}
				if (namedItem2 != null)
				{
					text3 = namedItem2.InnerText;
					service2 = channel.Services[text3];
					if (service2 == null)
					{
						EcuInfo ecuInfo = channel.EcuInfos[text3];
						if (ecuInfo == null)
						{
							ecuInfo = channel.EcuInfos.GetItemContaining(text3);
						}
						if (ecuInfo != null && ecuInfo.Services.Count > 0)
						{
							service2 = ecuInfo.Services[0];
						}
					}
					if (service2 == null)
					{
						flag = true;
						Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Service as VCP parameter definition error - read service {0} does not exist", text3));
					}
				}
				if (namedItem4 != null)
				{
					hide = Convert.ToBoolean(namedItem4.InnerText, CultureInfo.InvariantCulture);
				}
				if (namedItem5 != null)
				{
					persistable = !Convert.ToBoolean(namedItem5.InnerText, CultureInfo.InvariantCulture);
				}
				if (!flag)
				{
					parameter = new Parameter(channel, nParameterIndex2++, "SVC_ServicesAsParameters", "Services", persistable, base.Count);
					parameter.Acquire(innerText, innerText2, service, text2, result, service2, text3, hide);
					base.Items.Add(parameter);
				}
			}
			if (parameter != null)
			{
				parameter.LastInGroup = true;
			}
		}
		else
		{
			if (!vcpHelper.HasExternalVcp)
			{
				return;
			}
			TextFieldParser defParser = channel.Ecu.GetDefParser();
			if (defParser == null)
			{
				return;
			}
			string value = channel.Ecu.Properties.GetValue("VcpGroupName", "VCP");
			string value2 = channel.Ecu.Properties.GetValue("VcpGroupQualifier", "VCP");
			uint num3 = 0u;
			while (!defParser.EndOfData)
			{
				string[] array = defParser.ReadFields();
				if (array[0] == "P" && array.Length >= 10)
				{
					Parameter parameter2 = new Parameter(channel, num3++, value2, value, persistable: true, base.Count);
					parameter2.Acquire(array);
					base.Items.Add(parameter2);
				}
			}
		}
	}

	private void InternalRead(Varcode varcode, ref CaesarException ce)
	{
		string text = string.Empty;
		int startIndex = -1;
		int num = 0;
		for (int i = 0; i < base.Count; i++)
		{
			if (ce != null)
			{
				break;
			}
			if (!channel.ChannelRunning)
			{
				break;
			}
			if (channel.Closing)
			{
				break;
			}
			Parameter parameter = base[i];
			if (!string.Equals(parameter.GroupQualifier, text, StringComparison.Ordinal))
			{
				text = parameter.GroupQualifier;
				startIndex = i;
				num = 0;
			}
			if (!parameter.HasBeenReadFromEcu && parameter.Marked)
			{
				num++;
			}
			if (num <= 0 || !parameter.LastInGroup)
			{
				continue;
			}
			UpdateProgress(Convert.ToSingle(i, CultureInfo.InvariantCulture), Convert.ToSingle(base.Count, CultureInfo.InvariantCulture));
			try
			{
				if (InternalReadGroup(text, startIndex, i, ignoreMarkedFlag: false, varcode))
				{
					channel.CodingParameterGroups[text]?.AcquireDefaultStringandFragmentChoicesForCoding(groupCodingStrings[text]);
				}
			}
			catch (CaesarException ex)
			{
				ce = ex;
			}
		}
		UpdateHaveBeenReadFromEcuFlag();
	}

	internal void InternalRead()
	{
		CaesarException ce = null;
		if (channel.ChannelHandle != null || channel.McdChannelHandle != null)
		{
			using Varcode varcode = channel.VCInit();
			InternalRead(varcode, ref ce);
		}
		else if (vcpHelper.HasExternalVcp && base.Count > 0)
		{
			try
			{
				vcpHelper.ProcessExternalRead();
			}
			catch (CaesarException ex)
			{
				ce = ex;
			}
			UpdateHaveBeenReadFromEcuFlag();
		}
		channel.SetCommunicationsState(CommunicationsState.Online);
		RaiseParameterCommunicationCompleteEvent(ce, write: false);
	}

	internal void InternalReadGroupVcp(string groupQualifier)
	{
		Parameter itemFirstInGroup = GetItemFirstInGroup(groupQualifier);
		Parameter itemLastInGroup = GetItemLastInGroup(groupQualifier);
		if (itemFirstInGroup != null && itemLastInGroup != null)
		{
			InternalReadGroup(groupQualifier, itemFirstInGroup.Index, itemLastInGroup.Index, ignoreMarkedFlag: false);
		}
	}

	internal void InternalReadGroup(Parameter parameter, bool explicitRead)
	{
		CaesarException e = null;
		Parameter itemLastInGroup = GetItemLastInGroup(parameter.GroupQualifier);
		try
		{
			InternalReadGroup(parameter.GroupQualifier, parameter.Index, itemLastInGroup.Index, ignoreMarkedFlag: true);
		}
		catch (CaesarException ex)
		{
			e = ex;
		}
		UpdateHaveBeenReadFromEcuFlag();
		if (explicitRead)
		{
			channel.SetCommunicationsState(CommunicationsState.Online);
			RaiseParameterCommunicationCompleteEvent(e, write: false);
		}
	}

	internal void InternalWriteVcp()
	{
		Varcode varcode = null;
		CaesarException ex = null;
		List<Tuple<string, int, int>> list = new List<Tuple<string, int, int>>();
		if (channel.ChannelHandle != null || channel.McdChannelHandle != null)
		{
			int item = -1;
			List<Parameter> list2 = new List<Parameter>();
			string text = string.Empty;
			bool flag = false;
			int num = 0;
			for (int i = 0; i < base.Count; i++)
			{
				Parameter parameter = base[i];
				if ((!parameter.ServiceAsParameter && IsCodingStringAssignedByClient(parameter.GroupQualifier)) || (parameter.Marked && !object.Equals(parameter.OriginalValue, parameter.Value)))
				{
					num++;
				}
			}
			int num2 = 0;
			for (int j = 0; j < base.Count; j++)
			{
				if (ex != null)
				{
					break;
				}
				if (!channel.ChannelRunning)
				{
					break;
				}
				if (channel.Closing)
				{
					break;
				}
				Parameter parameter2 = base[j];
				if (!string.Equals(parameter2.GroupQualifier, text, StringComparison.Ordinal))
				{
					text = parameter2.GroupQualifier;
					item = j;
					list2.Clear();
					flag = parameter2.ServiceAsParameter;
				}
				if ((!flag && IsCodingStringAssignedByClient(text)) || (parameter2.Marked && !object.Equals(parameter2.OriginalValue, parameter2.Value)))
				{
					if (list2.Count == 0 && !flag)
					{
						if (channel.IntendedSession.HasValue && channel.GetActiveDiagnosticInformation(out var responseSession, out var _) && !responseSession.Equals(channel.IntendedSession))
						{
							throw new CaesarException(SapiError.EcuFailedToRemainInDiagnosticSession);
						}
						if (varcode == null)
						{
							varcode = channel.VCInit();
						}
						bool communicatedViaJob = channel.ParameterGroups[text].CommunicatedViaJob;
						string text2 = ((!IsCodingStringState(text, CodingStringState.Incomplete) && !communicatedViaJob) ? groupCodingStrings[text] : null);
						if (!string.IsNullOrEmpty(text2))
						{
							varcode.EnableReadCodingStringFromEcu(enableReadCodingStringFromEcu: false);
							varcode.SetCurrentCodingString(parameter2.GroupQualifier, new Dump(text2).Data.ToArray());
						}
						else
						{
							varcode.EnableReadCodingStringFromEcu(enableReadCodingStringFromEcu: true);
						}
					}
					num2++;
					UpdateProgress(Convert.ToSingle(num2, CultureInfo.InvariantCulture), Convert.ToSingle(num, CultureInfo.InvariantCulture));
					if (varcode != null || flag)
					{
						parameter2.InternalWrite(varcode);
						if (parameter2.Exception == null)
						{
							RaiseParameterUpdateEvent(parameter2, null);
						}
					}
					list2.Add(parameter2);
				}
				if (list2.Count <= 0 || !parameter2.LastInGroup)
				{
					continue;
				}
				if (varcode != null)
				{
					CaesarException ex2 = null;
					try
					{
						varcode.DoCoding();
					}
					catch (NullReferenceException)
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(this, "Intentional catch of exception from VCDoCoding. Comms failure?");
					}
					if (varcode.IsErrorSet)
					{
						ex2 = varcode.Exception;
						varcode.Dispose();
						varcode = null;
						if (channel.ChannelRunning)
						{
							foreach (Parameter item2 in list2)
							{
								item2.Exception = ex2;
							}
						}
						else
						{
							ex = ex2;
						}
					}
				}
				else if (channel.IsChannelErrorSet)
				{
					ex = new CaesarException(channel.ChannelHandle);
				}
				if (channel.ChannelRunning)
				{
					list.Add(new Tuple<string, int, int>(text, item, j));
				}
			}
			if (varcode != null)
			{
				varcode.Dispose();
				varcode = null;
			}
			if (channel.ChannelRunning)
			{
				if (num > 0)
				{
					if ((channel.ChannelOptions & ChannelOptions.ExecuteParameterWriteInitializeCommitServices) != ChannelOptions.None && verifyAfterCommit)
					{
						channel.Services.InternalDereferencedExecute("CommitToPermanentMemoryService");
					}
					if (channel.ChannelRunning && (verifyAfterWrite || verifyAfterCommit))
					{
						using Varcode vh = channel.VCInit();
						foreach (Tuple<string, int, int> item3 in list)
						{
							if (!channel.ChannelRunning || channel.Closing)
							{
								continue;
							}
							try
							{
								InternalReadGroup(item3.Item1, item3.Item2, item3.Item3, ignoreMarkedFlag: false, vh);
							}
							catch (CaesarException ex4)
							{
								if (ex == null)
								{
									ex = ex4;
								}
							}
						}
					}
					if ((channel.ChannelOptions & ChannelOptions.ExecuteParameterWriteInitializeCommitServices) != ChannelOptions.None && verifyAfterWrite)
					{
						channel.Services.InternalDereferencedExecute("CommitToPermanentMemoryService");
					}
				}
			}
			else if (ex == null)
			{
				ex = new CaesarException(SapiError.CommunicationsCeasedDuringVarcoding);
			}
		}
		else if (vcpHelper.HasExternalVcp && base.Count > 0)
		{
			try
			{
				vcpHelper.ProcessExternalWrite();
			}
			catch (CaesarException ex5)
			{
				ex = ex5;
			}
		}
		if (ex != null)
		{
			throw ex;
		}
	}

	internal void InternalWrite()
	{
		CaesarException e = null;
		if ((channel.ChannelOptions & ChannelOptions.ExecuteParameterWriteInitializeCommitServices) != ChannelOptions.None)
		{
			Channel.Services.InternalDereferencedExecute("ParameterWriteInitializeService");
		}
		try
		{
			InternalWriteVcp();
		}
		catch (CaesarException ex)
		{
			e = ex;
		}
		channel.SetCommunicationsState(CommunicationsState.Online);
		RaiseParameterCommunicationCompleteEvent(e, write: true);
	}

	internal void RaiseParameterUpdateEvent(Parameter p, Exception e)
	{
		FireAndForget.Invoke(this.ParameterUpdateEvent, p, new ResultEventArgs(e));
	}

	internal void RaiseParameterCommunicationCompleteEvent(Exception e, bool write)
	{
		if (write)
		{
			FireAndForget.Invoke(this.ParametersWriteCompleteEvent, this, new ResultEventArgs(e));
		}
		else
		{
			FireAndForget.Invoke(this.ParametersReadCompleteEvent, this, new ResultEventArgs(e));
		}
		channel.SyncDone(e);
	}

	internal Parameter GetParameter(string parameterName, int hintIndex)
	{
		string alternateName;
		bool alternateAvailable = channel.Ecu.AlternateQualifiers.TryGetValue(parameterName, out alternateName);
		parameterName = StripUnderscores(parameterName);
		if (alternateAvailable)
		{
			alternateName = StripUnderscores(alternateName);
		}
		bool parameterNameIsCombined = parameterName.Contains('.');
		return (from item in this.Skip(hintIndex).Concat(this.Take(hintIndex))
			let itemName = StripUnderscores(parameterNameIsCombined ? item.CombinedQualifier : item.Qualifier)
			where itemName.CompareNoCase(parameterName) || (alternateAvailable && itemName.CompareNoCase(alternateName))
			select item).FirstOrDefault();
	}

	internal void UpdateProgress(float nominator, float denominator)
	{
		progress = nominator / denominator * 100f;
	}

	internal void ResetExceptions()
	{
		using IEnumerator<Parameter> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.ResetException();
		}
	}

	internal void ResetEcuReadFlags()
	{
		using (IEnumerator<Parameter> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.ResetHasBeenReadFromEcu();
			}
		}
		haveBeenReadFromEcu = false;
		originalGroupCodingStrings.Clear();
	}

	internal void InternalProcessVcp()
	{
		vcpHelper.Process();
		channel.SetCommunicationsState(CommunicationsState.Online);
		FireAndForget.Invoke(this.ParametersProcessVcpCompleteEvent, this, new ResultEventArgs(null));
		channel.SyncDone(null);
	}

	internal void SynchronousCheckFailure(object sender, CaesarException exception)
	{
		FireAndForget.Invoke(this.ParametersWriteCompleteEvent, this, new ResultEventArgs(exception));
	}

	internal void ResetGroupCodingString(string groupQualifier)
	{
		lock (groupCodingStringsStateLock)
		{
			if (groupCodingStringsState.ContainsKey(groupQualifier))
			{
				groupCodingStringsState[groupQualifier] |= CodingStringState.NeedsUpdate;
			}
		}
	}

	internal void SetGroupCodingString(string groupQualifier, string newValue)
	{
		lock (groupCodingStringsStateLock)
		{
			groupCodingStringsState[groupQualifier] |= CodingStringState.AssignedByClient;
			groupCodingStringsState[groupQualifier] &= ~CodingStringState.Incomplete;
		}
		SetGroupCodingString(groupQualifier, newValue, this.Where((Parameter p) => string.Equals(p.GroupQualifier, groupQualifier, StringComparison.OrdinalIgnoreCase)));
	}

	internal void SetGroupCodingString(string groupQualifier, string newValue, IEnumerable<Parameter> parametersToRead)
	{
		CaesarException ex = null;
		byte[] content = new Dump(newValue).Data.ToArray();
		lock (Channel.OfflineVarcodingHandleLock)
		{
			Varcode offlineVarcodingHandle = Channel.OfflineVarcodingHandle;
			if (offlineVarcodingHandle != null)
			{
				offlineVarcodingHandle.SetCurrentCodingString(groupQualifier, content);
				if (offlineVarcodingHandle.IsErrorSet)
				{
					ex = offlineVarcodingHandle.Exception;
				}
				else
				{
					groupCodingStrings[groupQualifier] = newValue;
					foreach (Parameter item in parametersToRead)
					{
						item.InternalRead(offlineVarcodingHandle, fromDevice: false);
					}
				}
			}
		}
		if (ex != null)
		{
			throw ex;
		}
	}

	public void Read(bool synchronous)
	{
		ResetExceptions();
		channel.QueueAction(CommunicationsState.ReadParameters, synchronous);
	}

	public void ReadAll(bool synchronous)
	{
		ResetEcuReadFlags();
		Read(synchronous);
	}

	public void ReadGroup(string groupQualifier, bool fromCache, bool synchronous)
	{
		ResetExceptions();
		Parameter itemFirstInGroup = GetItemFirstInGroup(groupQualifier);
		if (itemFirstInGroup != null)
		{
			if (!fromCache)
			{
				for (int i = itemFirstInGroup.Index; i < base.Count; i++)
				{
					Parameter parameter = base[i];
					if (!string.Equals(groupQualifier, parameter.GroupQualifier, StringComparison.Ordinal))
					{
						break;
					}
					parameter.ResetHasBeenReadFromEcu();
				}
			}
			channel.QueueAction(itemFirstInGroup, synchronous);
			return;
		}
		throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Parameter group {0} not valid", groupQualifier), "groupQualifier");
	}

	public void Write(bool synchronous)
	{
		ResetExceptions();
		channel.QueueAction(CommunicationsState.WriteParameters, synchronous, SynchronousCheckFailure);
	}

	public void Load(string path, ParameterFileFormat parameterFileFormat, Collection<string> unknownList)
	{
		using StreamReader inputStream = new StreamReader(path);
		Load(inputStream, parameterFileFormat, unknownList, respectAccessLevels: true);
	}

	public void Load(string path, ParameterFileFormat parameterFileFormat, StringDictionary unknownList)
	{
		StreamReader streamReader = new StreamReader(path);
		try
		{
			Load(streamReader, parameterFileFormat, unknownList, respectAccessLevels: true);
		}
		finally
		{
			streamReader.Close();
		}
	}

	public void Save(string path, ParameterFileFormat parameterFileFormat)
	{
		StreamWriter streamWriter = new StreamWriter(path);
		Save(streamWriter, parameterFileFormat, respectAccessLevels: true);
		streamWriter.Close();
	}

	public void SaveAccumulator(string path, ParameterFileFormat parameterFileFormat)
	{
		StreamWriter streamWriter = new StreamWriter(path);
		SaveAccumulator(streamWriter, parameterFileFormat, respectAccessLevels: true);
		streamWriter.Close();
	}

	public void Load(StreamReader inputStream, ParameterFileFormat parameterFileFormat, Collection<string> unknownList, bool respectAccessLevels)
	{
		StringDictionary stringDictionary = new StringDictionary();
		Load(inputStream, parameterFileFormat, stringDictionary, respectAccessLevels);
		if (unknownList == null)
		{
			return;
		}
		foreach (DictionaryEntry item in stringDictionary)
		{
			unknownList.Add(item.Key.ToString());
		}
	}

	public void Load(StreamReader inputStream, ParameterFileFormat parameterFileFormat, StringDictionary unknownList, bool respectAccessLevels)
	{
		ResetExceptions();
		vcpHelper.LoadFromStream(inputStream, parameterFileFormat, unknownList, respectAccessLevels);
	}

	public void Save(StreamWriter outputStream, ParameterFileFormat parameterFileFormat, bool respectAccessLevels)
	{
		if (parameterFileFormat == ParameterFileFormat.ParFile || parameterFileFormat == ParameterFileFormat.VerFile)
		{
			vcpHelper.SaveToStream(outputStream, parameterFileFormat, respectAccessLevels, saveAccumulator: false);
			return;
		}
		throw new ArgumentException("File format not supported");
	}

	public void SaveAccumulator(StreamWriter outputStream, ParameterFileFormat parameterFileFormat, bool respectAccessLevels)
	{
		if (parameterFileFormat == ParameterFileFormat.ParFile || parameterFileFormat == ParameterFileFormat.VerFile)
		{
			vcpHelper.SaveToStream(outputStream, parameterFileFormat, respectAccessLevels, saveAccumulator: true);
			return;
		}
		throw new ArgumentException("File format not supported");
	}

	public void ProcessVcp(string inputFile, string outputFile, bool synchronous)
	{
		if (channel.Extension != null)
		{
			channel.Extension.PrepareVcp();
		}
		StreamReader input = new StreamReader(inputFile);
		StreamWriter output = new StreamWriter(outputFile);
		vcpHelper.SetVcpStreams(input, output);
		channel.QueueAction(CommunicationsState.ProcessVcp, synchronous);
	}

	public void ProcessVcp(Stream inputStream, Stream outputStream, bool synchronous)
	{
		StreamReader input = new StreamReader(inputStream);
		StreamWriter output = new StreamWriter(outputStream);
		vcpHelper.SetVcpStreams(input, output);
		channel.QueueAction(CommunicationsState.ProcessVcp, synchronous);
	}

	public Parameter GetItemByName(string index)
	{
		return this.Where((Parameter item) => string.Equals(item.Name, index, StringComparison.Ordinal)).FirstOrDefault();
	}

	public Parameter GetItemFirstInGroup(string groupQualifier)
	{
		return this.Where((Parameter parameter) => string.Equals(groupQualifier, parameter.GroupQualifier, StringComparison.Ordinal)).FirstOrDefault();
	}

	public Parameter GetItemLastInGroup(string groupQualifier)
	{
		return this.Where((Parameter parameter) => string.Equals(groupQualifier, parameter.GroupQualifier, StringComparison.Ordinal)).LastOrDefault();
	}

	public bool IsCodingStringAssignedByClient(string groupQualifier)
	{
		return IsCodingStringState(groupQualifier, CodingStringState.AssignedByClient);
	}

	internal bool IsCodingStringState(string groupQualifier, CodingStringState targetState)
	{
		lock (groupCodingStringsStateLock)
		{
			return (groupCodingStringsState[groupQualifier] & targetState) != 0;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public static TargetEcuDetails GetTargetEcuDetails(string path, ParameterFileFormat parameterFileFormat)
	{
		StreamReader streamReader = new StreamReader(path);
		try
		{
			return GetTargetEcuDetails(streamReader, parameterFileFormat);
		}
		finally
		{
			streamReader.Close();
		}
	}

	public static TargetEcuDetails GetTargetEcuDetails(StreamReader streamReader, ParameterFileFormat parameterFileFormat)
	{
		string targetEcu = VcpHelper.GetIdentificationRecordValue("ECU", streamReader);
		string identificationRecordValue = VcpHelper.GetIdentificationRecordValue("DIAGNOSISVARIANT", streamReader);
		string assumedDiagnosisVariant = string.Empty;
		int num = int.MaxValue;
		Sapi sapi = Sapi.GetSapi();
		Ecu ecu = sapi.Ecus[targetEcu];
		if (ecu != null)
		{
			DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[identificationRecordValue];
			Ecu ecu2 = null;
			if (diagnosisVariant == null)
			{
				ecu2 = sapi.Ecus.FirstOrDefault((Ecu e) => e.Name == targetEcu && e != ecu);
				if (ecu2 != null)
				{
					diagnosisVariant = ecu2.DiagnosisVariants[identificationRecordValue];
				}
			}
			if (diagnosisVariant == null)
			{
				StringDictionary stringDictionary = new StringDictionary();
				VcpHelper.LoadDictionaryFromStream(streamReader, parameterFileFormat, stringDictionary);
				List<string> list = new List<string>();
				foreach (XmlNode item in from e in new Ecu[2] { ecu, ecu2 }
					where e != null && e.Xml != null
					select e.Xml)
				{
					XmlNodeList xmlNodeList = item.SelectNodes("Ecu/ServicesAsParameters/ServiceAsParameter");
					if (xmlNodeList != null)
					{
						for (int num2 = 0; num2 < xmlNodeList.Count; num2++)
						{
							string innerText = xmlNodeList.Item(num2).Attributes.GetNamedItem("Qualifier").InnerText;
							list.Add(innerText);
						}
					}
				}
				List<DiagnosisVariant> diagnosisVariants = new List<DiagnosisVariant>(ecu.DiagnosisVariants.Where((DiagnosisVariant v) => !v.IsBase));
				if (ecu2 != null)
				{
					diagnosisVariants.AddRange(ecu2.DiagnosisVariants.Where((DiagnosisVariant adv) => !adv.IsBase && !diagnosisVariants.Any((DiagnosisVariant dv) => dv.DiagnosticVersionLong == adv.DiagnosticVersionLong)));
				}
				foreach (DiagnosisVariant item2 in Enumerable.Reverse(diagnosisVariants))
				{
					int num3 = 0;
					foreach (DictionaryEntry entry in stringDictionary)
					{
						if (!item2.ParameterQualifiers.Select((Tuple<string, string> vq) => (((string)entry.Key).IndexOf('.') == -1) ? vq.Item2 : string.Join(".", vq.Item1, vq.Item2)).Contains((string)entry.Key, StringComparer.OrdinalIgnoreCase) && !list.Contains((string)entry.Key, StringComparer.OrdinalIgnoreCase))
						{
							num3++;
						}
					}
					Sapi.GetSapi().RaiseDebugInfoEvent(ecu, string.Format(CultureInfo.InvariantCulture, "Variant {0} has {1} mismatched parameters", item2.Name, num3));
					if (num3 < num)
					{
						num = num3;
						assumedDiagnosisVariant = item2.ToString();
						if (num3 == 0)
						{
							break;
						}
					}
				}
			}
		}
		return new TargetEcuDetails(targetEcu, identificationRecordValue, assumedDiagnosisVariant, num);
	}

	public static void LoadDictionaryFromStream(StreamReader stream, ParameterFileFormat parameterFileFormat, StringDictionary parameters)
	{
		VcpHelper.LoadDictionaryFromStream(stream, parameterFileFormat, parameters);
	}

	public static string GetIdentificationRecordValue(string recordName, StreamReader stream)
	{
		return VcpHelper.GetIdentificationRecordValue(recordName, stream);
	}

	public static string GetIdentificationRecordValue(string recordName, string path)
	{
		StreamReader streamReader = new StreamReader(path);
		try
		{
			return VcpHelper.GetIdentificationRecordValue(recordName, streamReader);
		}
		finally
		{
			streamReader.Close();
		}
	}

	private void InternalReadGroup(string groupQualifier, int startIndex, int endIndex, bool ignoreMarkedFlag)
	{
		if (!base[startIndex].ServiceAsParameter)
		{
			using (Varcode vh = channel.VCInit())
			{
				InternalReadGroup(groupQualifier, startIndex, endIndex, ignoreMarkedFlag, vh);
				return;
			}
		}
		InternalReadGroup(groupQualifier, startIndex, endIndex, ignoreMarkedFlag, null);
	}

	private bool InternalReadGroup(string groupQualifier, int startIndex, int endIndex, bool ignoreMarkedFlag, Varcode vh)
	{
		bool serviceAsParameter = base[startIndex].ServiceAsParameter;
		if (vh != null || serviceAsParameter)
		{
			CaesarException ex = null;
			if (!serviceAsParameter)
			{
				ex = UpdateCodingString(groupQualifier, vh, CodingStringSource.FromEcu);
			}
			for (int i = startIndex; i <= endIndex; i++)
			{
				Parameter parameter = base[i];
				if (!parameter.HasBeenReadFromEcu && (ignoreMarkedFlag || parameter.Marked))
				{
					if (ex == null)
					{
						parameter.InternalRead(vh, fromDevice: true);
					}
					else
					{
						parameter.RaiseParameterUpdateEvent(ex);
					}
				}
			}
			if (ex != null && !channel.ChannelRunning)
			{
				throw ex;
			}
			return ex == null;
		}
		if (channel.IsChannelErrorSet)
		{
			throw new CaesarException(channel.ChannelHandle);
		}
		return false;
	}

	private void UpdateHaveBeenReadFromEcuFlag()
	{
		haveBeenReadFromEcu = true;
		for (int i = 0; i < base.Count; i++)
		{
			Parameter parameter = base[i];
			if ((Sapi.GetSapi().HardwareAccess >= parameter.ReadAccess || Sapi.GetSapi().InitState == InitState.Offline) && !parameter.HasBeenReadFromEcu && parameter.Exception == null)
			{
				haveBeenReadFromEcu = false;
				break;
			}
		}
	}

	internal CaesarException UpdateCodingString(string groupQualifier, Varcode varcode, CodingStringSource from)
	{
		string text = null;
		CaesarException result = null;
		try
		{
			text = new Dump(varcode.GetCurrentCodingString(groupQualifier)).ToString();
		}
		catch (NullReferenceException)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Intentional catch of exception from VCCurrentCodingString. Comms failure?");
		}
		if (varcode.IsErrorSet)
		{
			result = varcode.Exception;
		}
		groupCodingStrings[groupQualifier] = text;
		switch (from)
		{
		case CodingStringSource.FromEcu:
		case CodingStringSource.FromLogFile:
			originalGroupCodingStrings[groupQualifier] = text;
			lock (groupCodingStringsStateLock)
			{
				groupCodingStringsState[groupQualifier] &= ~(CodingStringState.AssignedByClient | CodingStringState.Incomplete);
			}
			if (from == CodingStringSource.FromEcu)
			{
				channel.ParameterGroups[groupQualifier].CodingStringValues.Add(new CodingStringValue(new Dump(text), Sapi.Now));
			}
			break;
		case CodingStringSource.FromParameterUpdate:
		case CodingStringSource.FromIncompleteParameterUpdate:
		case CodingStringSource.FromDefaultString:
			lock (groupCodingStringsStateLock)
			{
				groupCodingStringsState[groupQualifier] &= ~CodingStringState.NeedsUpdate;
				if (from == CodingStringSource.FromIncompleteParameterUpdate)
				{
					groupCodingStringsState[groupQualifier] |= CodingStringState.Incomplete;
					break;
				}
				groupCodingStringsState[groupQualifier] &= ~CodingStringState.Incomplete;
				if (from == CodingStringSource.FromDefaultString)
				{
					groupCodingStringsState[groupQualifier] |= CodingStringState.AssignedByClient;
				}
			}
			break;
		}
		return result;
	}

	private void UpdateCodingStrings()
	{
		lock (channel.OfflineVarcodingHandleLock)
		{
			Varcode offlineVarcodingHandle = channel.OfflineVarcodingHandle;
			if (offlineVarcodingHandle == null)
			{
				return;
			}
			foreach (IGrouping<string, Parameter> item in from p in this
				where !p.ServiceAsParameter
				group p by p.GroupQualifier)
			{
				bool flag;
				lock (groupCodingStringsStateLock)
				{
					flag = (groupCodingStringsState[item.Key] & CodingStringState.NeedsUpdate) != 0;
				}
				if (!flag)
				{
					continue;
				}
				string text = ((!IsCodingStringState(item.Key, CodingStringState.Incomplete)) ? groupCodingStrings[item.Key] : null);
				if (!string.IsNullOrEmpty(text))
				{
					offlineVarcodingHandle.SetCurrentCodingString(item.Key, new Dump(text).Data.ToArray());
				}
				foreach (Parameter item2 in item)
				{
					if (item2.Value != null)
					{
						item2.InternalWrite(offlineVarcodingHandle);
					}
				}
				UpdateCodingString(item.Key, offlineVarcodingHandle, (!string.IsNullOrEmpty(text) || (item.All((Parameter p) => p.Value != null) && channel.ParameterGroups[item.Key].ParametersCoverGroup)) ? CodingStringSource.FromParameterUpdate : CodingStringSource.FromIncompleteParameterUpdate);
			}
		}
	}

	internal void UpdateGroupCodingStringFromLogFile(ParameterGroup group)
	{
		StringDictionary stringDictionary = groupCodingStrings;
		string qualifier = group.Qualifier;
		string value = (originalGroupCodingStrings[group.Qualifier] = null);
		stringDictionary[qualifier] = value;
		lock (channel.OfflineVarcodingHandleLock)
		{
			Varcode offlineVarcodingHandle = channel.OfflineVarcodingHandle;
			if (offlineVarcodingHandle == null || group.ServiceAsParameter || !group.GroupLength.HasValue)
			{
				return;
			}
			offlineVarcodingHandle.SetCurrentCodingString(group.Qualifier, group.CodingStringValues.Current?.Value?.Data?.ToArray() ?? new byte[group.GroupLength.Value]);
			foreach (Parameter item in group.Parameters.Where((Parameter p) => p.Value != null))
			{
				item.InternalWrite(offlineVarcodingHandle, resetHaveBeenReadFromEcu: false);
			}
			UpdateCodingString(group.Qualifier, offlineVarcodingHandle, CodingStringSource.FromLogFile);
			channel.CodingParameterGroups[group.Qualifier]?.AcquireDefaultStringandFragmentChoicesForCoding(groupCodingStrings[group.Qualifier]);
		}
	}

	private void Dispose(bool disposing)
	{
		if (!disposed && disposing && vcpHelper != null)
		{
			vcpHelper.Dispose();
			vcpHelper = null;
		}
		disposed = true;
	}

	private bool IsAccumulatorGroupQualifier(string groupQualifierString)
	{
		for (int i = 0; i < accumulatorPrefixes.Count; i++)
		{
			if (Regex.Match(groupQualifierString, accumulatorPrefixes[i]).Success)
			{
				return true;
			}
		}
		return false;
	}

	private void CheckAndAddParameterGroup(string groupQualifier, ref uint nParameterIndex, McdDBService readPrimitive, McdDBService writePrimitive)
	{
		Parameter parameter = null;
		bool persistable = !IsAccumulatorGroupQualifier(groupQualifier);
		string caesarEquivalentName = McdCaesarEquivalence.GetCaesarEquivalentName(readPrimitive);
		bool flag = channel.Ecu.IgnoreQualifier(groupQualifier);
		Service service = new Service(channel, ServiceTypes.ReadVarCode, "RVC_" + McdCaesarEquivalence.MakeQualifier(readPrimitive.Name));
		service.Acquire(readPrimitive.Name, readPrimitive, null, readPrimitive.SpecialData);
		Service service2 = new Service(channel, ServiceTypes.WriteVarCode, "WVC_" + McdCaesarEquivalence.MakeQualifier(writePrimitive.Name));
		service2.Acquire(writePrimitive.Name, writePrimitive, null, writePrimitive.SpecialData);
		Dictionary<string, int> existingSet = new Dictionary<string, int>();
		foreach (ServiceInputValue inputValue in service2.InputValues)
		{
			string text = inputValue.Name;
			int num = text.IndexOf('[');
			if (num != -1 && (num % 2 == 1 || num % 2 == 5))
			{
				string[] array = text.Substring(0, num).Split(" ".ToCharArray());
				if (array.Length == 2)
				{
					string text2 = ((array[0].StartsWith("e2p_", StringComparison.Ordinal) && !array[1].StartsWith("E2P_", StringComparison.Ordinal)) ? array[0].Substring(4) : array[0]);
					if (text2.Length == array[1].Length && text2.ToUpperInvariant() == array[1])
					{
						text = array[1] + text.Substring(num);
					}
				}
			}
			string text3 = McdCaesarEquivalence.MakeQualifier(text, existingSet, isFragmentName: true);
			if (!inputValue.IsReserved && !channel.Ecu.IgnoreQualifier(text3) && !flag && inputValue.Type != null)
			{
				parameter = new Parameter(channel, nParameterIndex++, groupQualifier, caesarEquivalentName, persistable, base.Count);
				parameter.Acquire(text3, inputValue, text, service, service2);
				base.Items.Add(parameter);
			}
			else
			{
				nParameterIndex++;
			}
		}
		if (parameter != null)
		{
			parameter.LastInGroup = true;
		}
	}

	private void CheckAndAddParameterGroup(Varcode offlineVarcoding, CaesarDIVarCodeDom varcodeDom, string groupQualifierString, ref uint nParameterIndex, Service readService, Service writeService)
	{
		Parameter parameter = null;
		bool persistable = !IsAccumulatorGroupQualifier(groupQualifierString);
		bool flag = channel.Ecu.IgnoreQualifier(groupQualifierString);
		uint defaultStringCount = varcodeDom.DefaultStringCount;
		byte[] array = null;
		if (defaultStringCount != 0)
		{
			array = varcodeDom.GetDefaultString(0u);
		}
		string longName = varcodeDom.LongName;
		uint varCodeFragCount = varcodeDom.VarCodeFragCount;
		if (offlineVarcoding != null && array != null)
		{
			offlineVarcoding.SetCurrentCodingString(groupQualifierString, array);
		}
		for (uint num = 0u; num < varCodeFragCount; num++)
		{
			CaesarDIVarCodeFrag val = varcodeDom.OpenVarCodeFrag(num);
			try
			{
				if (val != null)
				{
					if (!channel.Ecu.IgnoreQualifier(val.Qualifier) && !flag)
					{
						parameter = new Parameter(channel, nParameterIndex++, groupQualifierString, longName, persistable, base.Count);
						parameter.Acquire(offlineVarcoding, val, array, readService, writeService);
						base.Items.Add(parameter);
					}
					else
					{
						nParameterIndex++;
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (parameter != null)
		{
			parameter.LastInGroup = true;
		}
	}

	private static string StripUnderscores(string source)
	{
		return source.Replace("_", string.Empty);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_ItemByName is deprecated, please use GetItemByName(string) instead.")]
	public Parameter get_ItemByName(string index)
	{
		return GetItemByName(index);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_ItemFirstInGroup is deprecated, please use GetItemFirstInGroup(string) instead.")]
	public Parameter get_ItemFirstInGroup(string groupQualifier)
	{
		return GetItemFirstInGroup(groupQualifier);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_ItemLastInGroup is deprecated, please use GetItemFirstInGroup(string) instead.")]
	public Parameter get_ItemLastInGroup(string groupQualifier)
	{
		return GetItemLastInGroup(groupQualifier);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Load(StreamReader, ParameterFileFormat, ArrayList, bool) is deprecated, please use Load(StreamReader, ParameterFileFormat, Collection<string>, bool) instead.")]
	public void Load(StreamReader inputStream, ParameterFileFormat parameterFileFormat, ArrayList unknownList, bool respectAccessLevels)
	{
		StringDictionary stringDictionary = new StringDictionary();
		Load(inputStream, parameterFileFormat, stringDictionary, respectAccessLevels);
		if (unknownList == null)
		{
			return;
		}
		foreach (DictionaryEntry item in stringDictionary)
		{
			unknownList.Add(item.Key);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Load(string, ParameterFileFormat, ArrayList) is deprecated, please use Load(string, ParameterFileFormat, Collection<string>) instead.")]
	public void Load(string path, ParameterFileFormat parameterFileFormat, ArrayList unknownList)
	{
		using StreamReader inputStream = new StreamReader(path);
		Load(inputStream, parameterFileFormat, unknownList, respectAccessLevels: true);
	}
}
