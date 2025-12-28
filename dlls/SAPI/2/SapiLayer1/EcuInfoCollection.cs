using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml;
using CaesarAbstraction;

namespace SapiLayer1;

public sealed class EcuInfoCollection : LateLoadReadOnlyCollection<EcuInfo>
{
	private Dictionary<string, EcuInfo> cache;

	private bool autoRead;

	private Channel channel;

	public Channel Channel => channel;

	public EcuInfo this[string qualifier]
	{
		get
		{
			if (qualifier != null)
			{
				qualifier = qualifier.RemoveArguments();
				if (cache == null)
				{
					cache = ToDictionary((EcuInfo e) => e.Qualifier, StringComparer.OrdinalIgnoreCase);
				}
				EcuInfo value = null;
				if (!cache.TryGetValue(qualifier, out value) && channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out var value2))
				{
					cache.TryGetValue(value2, out value);
				}
				return value;
			}
			return null;
		}
	}

	public bool AutoRead
	{
		get
		{
			return autoRead;
		}
		set
		{
			autoRead = value;
		}
	}

	public event EcuInfoUpdateEventHandler EcuInfoUpdateEvent;

	public event EcuInfosReadCompleteEventHandler EcuInfosReadCompleteEvent;

	internal EcuInfoCollection(Channel c)
	{
		channel = c;
		autoRead = true;
	}

	internal bool InternalRead(EcuInfoInternalReadType readType)
	{
		bool result = false;
		for (int i = 0; i < base.Count; i++)
		{
			if (channel.Closing)
			{
				break;
			}
			if (!channel.ChannelRunning)
			{
				break;
			}
			EcuInfo ecuInfo = base[i];
			if (ecuInfo.Marked && (readType != EcuInfoInternalReadType.CyclicRead || ecuInfo.NeedsUpdate))
			{
				ecuInfo.InternalRead(explicitread: false);
				result = true;
				if (readType != EcuInfoInternalReadType.ExplicitRead)
				{
					ecuInfo.LastCyclicAttemptTime = Sapi.Now;
				}
			}
		}
		if (readType != EcuInfoInternalReadType.CyclicRead)
		{
			FireAndForget.Invoke(this.EcuInfosReadCompleteEvent, this, new ResultEventArgs(null));
		}
		if (readType == EcuInfoInternalReadType.ExplicitRead)
		{
			if (!channel.Closing)
			{
				channel.SetCommunicationsState(CommunicationsState.Online);
			}
			channel.SyncDone(null);
		}
		return result;
	}

	internal void InternalRead(bool explicitread)
	{
		if (!channel.Closing && channel.ChannelRunning && !channel.IsRollCall)
		{
			channel.ClearCache();
		}
		InternalRead((!explicitread) ? EcuInfoInternalReadType.ImplicitRead : EcuInfoInternalReadType.ExplicitRead);
	}

	internal void AddFromRollCall(EcuInfo ecuInfo)
	{
		base.Items.Add(ecuInfo);
		if (cache != null)
		{
			cache.Add(ecuInfo.Qualifier, ecuInfo);
		}
	}

	protected override void AcquireList()
	{
		if (channel.Ecu.RollCallManager != null)
		{
			channel.Ecu.RollCallManager.CreateEcuInfos(this);
			return;
		}
		if (!channel.Ecu.IsMcd)
		{
			if (channel.Online)
			{
				if (channel.ChannelHandle != null)
				{
					CaesarIdBlock idBlock = channel.ChannelHandle.IdBlock;
					if (idBlock.PartNumber != null)
					{
						base.Items.Add(new EcuInfo(channel, EcuInfoType.IdBlock, "MBNumber", "MB Number", "Common", "Common/ID Block", string.Empty, null, visible: true));
					}
					if (idBlock.SoftwareVersion.HasValue)
					{
						base.Items.Add(new EcuInfo(channel, EcuInfoType.IdBlock, "SWVersionNumber", "Software Version", "Common", "Common/ID Block", string.Empty, null, visible: true));
					}
					if (idBlock.DiagVersion.HasValue)
					{
						base.Items.Add(new EcuInfo(channel, EcuInfoType.IdBlock, "DiagVersion", "Diagnostic Version", "Common", "Common/ID Block", string.Empty, null, visible: true));
					}
					if (channel.IsChannelErrorSet)
					{
						CaesarException e = new CaesarException(channel.ChannelHandle);
						Sapi.GetSapi().RaiseExceptionEvent(this, e);
					}
				}
			}
			else
			{
				base.Items.Add(new EcuInfo(channel, EcuInfoType.IdBlock, "MBNumber", "MB Number", "Common", "Common/ID Block", string.Empty, null, visible: false));
				base.Items.Add(new EcuInfo(channel, EcuInfoType.IdBlock, "SWVersionNumber", "Software Version", "Common", "Common/ID Block", string.Empty, null, visible: false));
				base.Items.Add(new EcuInfo(channel, EcuInfoType.IdBlock, "DiagVersion", "Diagnostic Version", "Common", "Common/ID Block", string.Empty, null, visible: false));
			}
		}
		base.Items.Add(new EcuInfo(channel, EcuInfoType.DiagnosisVariant, "DiagnosisVariant", "Diagnostic Variant", "Common", "Common/Diagnostic Variant", string.Empty, null, visible: true));
		if (channel.DiagnosisVariant.PartNumber != null)
		{
			base.Items.Add(new EcuInfo(channel, EcuInfoType.DiagnosisVariant, "DiagnosisVariantPartNumber", "Diagnostic Variant Part Number", "Common", "Common/Diagnostic Variant", string.Empty, null, visible: true));
		}
		IEnumerable<EcuInfo> enumerable = (from s in new ServiceCollection(channel, ServiceTypes.StoredData)
			where s.OutputValues.Count > 0
			select new EcuInfo(channel, s)).ToList();
		XmlNode xml = channel.Ecu.Xml;
		if (xml != null)
		{
			XmlNodeList xmlNodeList = xml.SelectNodes("Ecu/EcuInfos/EcuInfo");
			if (xmlNodeList != null)
			{
				foreach (XmlNode item3 in xmlNodeList)
				{
					string innerText = item3.Attributes.GetNamedItem("Qualifier").InnerText;
					string innerText2 = item3.Attributes.GetNamedItem("Name").InnerText;
					string innerText3 = item3.Attributes.GetNamedItem("GroupQualifier").InnerText;
					string innerText4 = item3.Attributes.GetNamedItem("GroupName").InnerText;
					string innerText5 = item3.Attributes.GetNamedItem("FormatString").InnerText;
					string description = string.Empty;
					XmlNode namedItem = item3.Attributes.GetNamedItem("Description");
					if (namedItem != null)
					{
						description = namedItem.InnerText;
					}
					int? presentationIndex = null;
					XmlNode namedItem2 = item3.Attributes.GetNamedItem("PresentationIndex");
					if (namedItem2 != null)
					{
						presentationIndex = Convert.ToInt32(namedItem2.InnerText, CultureInfo.InvariantCulture);
					}
					bool flag = false;
					List<Tuple<EcuInfo, int?>> list = new List<Tuple<EcuInfo, int?>>();
					XmlNodeList xmlNodeList2 = item3.SelectNodes("Reference");
					foreach (XmlNode item4 in xmlNodeList2)
					{
						string[] array = item4.Attributes.GetNamedItem("Qualifier").InnerText.Split("[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						string serviceQualifier = array[0];
						int? item = ((array.Length > 1) ? new int?(int.Parse(array[1], CultureInfo.InvariantCulture)) : ((int?)null));
						string alternateServiceQualifier;
						bool alternateAvailable = channel.Ecu.AlternateQualifiers.TryGetValue(serviceQualifier, out alternateServiceQualifier);
						EcuInfo ecuInfo = enumerable.FirstOrDefault((EcuInfo ei) => ei.Qualifier == serviceQualifier || (alternateAvailable && ei.Qualifier == alternateServiceQualifier));
						list.Add(Tuple.Create(ecuInfo, item));
						if (ecuInfo != null)
						{
							flag = true;
						}
					}
					if (flag || xmlNodeList2.Count == 0)
					{
						EcuInfo item2 = new EcuInfo(channel, EcuInfoType.Compound, innerText, innerText2, innerText3, innerText4, description, innerText5, visible: true, list, presentationIndex);
						base.Items.Add(item2);
					}
				}
			}
		}
		foreach (EcuInfo item5 in enumerable)
		{
			base.Items.Add(item5);
		}
	}

	internal void RaiseEcuInfoUpdateEvent(EcuInfo i, Exception e)
	{
		FireAndForget.Invoke(this.EcuInfoUpdateEvent, i, new ResultEventArgs(e));
	}

	internal void UpdateFromRollCall(int id, byte[] data)
	{
		using IEnumerator<EcuInfo> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			EcuInfo current = enumerator.Current;
			int? messageNumber = current.MessageNumber;
			if (messageNumber.GetValueOrDefault() == id && messageNumber.HasValue && current.Presentation != null && current.Presentation.BytePosition.HasValue)
			{
				try
				{
					current.UpdateFromRollCall(current.Presentation.GetPresentation(data));
				}
				catch (CaesarException e)
				{
					current.RaiseEcuInfoUpdateEvent(e, explicitread: false);
				}
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("get_ItemContaining is deprecated, please use GetItemContaining(string) instead.")]
	public EcuInfo get_ItemContaining(string index)
	{
		return GetItemContaining(index);
	}

	public EcuInfo GetItemContaining(string index)
	{
		if (index != null)
		{
			index = index.RemoveArguments();
			string alternateIndex;
			bool alternateAvailable = channel.Ecu.AlternateQualifiers.TryGetValue(index, out alternateIndex);
			return this.Where((EcuInfo ecuInfo) => ecuInfo.Services != null && ecuInfo.Services.Any((Service service) => service != null && (service.Qualifier.CompareNoCase(index) || (alternateAvailable && service.Qualifier.CompareNoCase(alternateIndex))))).FirstOrDefault();
		}
		return null;
	}

	public void Read(bool synchronous)
	{
		channel.QueueAction(CommunicationsState.ReadEcuInfo, synchronous);
	}
}
