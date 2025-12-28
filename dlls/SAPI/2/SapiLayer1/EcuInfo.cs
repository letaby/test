using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using CaesarAbstraction;

namespace SapiLayer1;

public sealed class EcuInfo : IDiogenesDataItem
{
	private enum ReadType
	{
		UserInvoke,
		ReferencedUserInvoke,
		SystemInvoke
	}

	private const int RollCallMessageCacheTime = 1000;

	private static byte[] DeviceIdentificationPrefix = new byte[2] { 34, 241 };

	private const string ExtensionReferencePrefix = "extension:";

	private string derivedGroupQualifier;

	private object heldValue;

	private object manipulatedValue;

	private bool summary;

	private Channel channel;

	private EcuInfoType type;

	private string qualifier;

	private string name;

	private string groupName;

	private string groupQualifier;

	private string description;

	private string formatString;

	private bool visible;

	private bool marked;

	private int accessLevel;

	private Service service;

	private IList<EcuInfo> referenced;

	private IList<int?> referencedArrayIndexes;

	private readonly Presentation presentation;

	private EcuInfoValueCollection ecuInfoValues;

	private object cacheTime;

	private DateTime lastRollCallUpdateTime;

	internal bool NeedsUpdate
	{
		get
		{
			if (cacheTime != null && Sapi.Now > LastCyclicAttemptTime + TimeSpan.FromMilliseconds(Convert.ToInt32(cacheTime, CultureInfo.InvariantCulture)))
			{
				return true;
			}
			return false;
		}
	}

	internal DateTime LastCyclicAttemptTime { get; set; }

	public Presentation Presentation
	{
		get
		{
			if (presentation != null)
			{
				return presentation;
			}
			if (referenced.Count == 1)
			{
				if (referenced[0] != null)
				{
					return referenced[0].Presentation;
				}
			}
			else if (service != null && service.OutputValues.Count > 0)
			{
				return service.OutputValues[0];
			}
			return null;
		}
	}

	public Channel Channel => channel;

	public string Qualifier => qualifier;

	public string Name
	{
		get
		{
			if (channel.Ecu.RollCallManager != null)
			{
				return channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(qualifier, "SPN"), name);
			}
			return channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(qualifier, "Name"), name);
		}
	}

	public string ShortName => channel.Ecu.ShortName(Name);

	public string Description => channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(qualifier, "Description"), description);

	public string GroupName
	{
		get
		{
			if (derivedGroupQualifier == null)
			{
				derivedGroupQualifier = groupName.CreateQualifierFromName();
			}
			int num = groupName.IndexOf('/');
			if (num != -1)
			{
				return channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(derivedGroupQualifier, "GroupName"), groupName.Substring(0, num)) + "/" + channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(derivedGroupQualifier, "GroupSubName"), groupName.Substring(num + 1));
			}
			return channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(derivedGroupQualifier, "GroupName"), groupName);
		}
	}

	public string OriginalGroupName => groupName;

	public string GroupQualifier => groupQualifier;

	public string Value
	{
		get
		{
			string result = string.Empty;
			EcuInfoValue current = ecuInfoValues.Current;
			if (current != null && current.Value != null)
			{
				result = ((!current.Value.GetType().IsArray) ? current.Value.ToString() : string.Join(", ", ((Array)current.Value).Cast<object>()));
			}
			return result;
		}
	}

	public object ManipulatedValue
	{
		get
		{
			return manipulatedValue;
		}
		set
		{
			if (Channel.LogFile != null)
			{
				throw new InvalidOperationException("Cannot manipulate a value for a log file channel");
			}
			if (value != manipulatedValue)
			{
				manipulatedValue = value;
				object ecuInfoValue = manipulatedValue ?? heldValue;
				ecuInfoValues.Add(new EcuInfoValue(ecuInfoValue, Sapi.Now));
				RaiseEcuInfoUpdateEvent(null, explicitread: false);
				Channel.SetManipulatedState(Qualifier, value != null);
			}
		}
	}

	public ChoiceCollection Choices
	{
		get
		{
			if (Presentation == null)
			{
				return null;
			}
			return Presentation.Choices;
		}
	}

	public EcuInfoValueCollection EcuInfoValues => ecuInfoValues;

	public EcuInfoType EcuInfoType => type;

	public bool Visible
	{
		get
		{
			if (Channel.LogFile != null && EcuInfoValues.Count == 0)
			{
				return false;
			}
			return visible;
		}
		internal set
		{
			visible = value;
		}
	}

	public int AccessLevel => accessLevel;

	public string Units
	{
		get
		{
			if (Presentation == null)
			{
				return string.Empty;
			}
			return Presentation.Units;
		}
	}

	public object Precision
	{
		get
		{
			if (Presentation == null)
			{
				return null;
			}
			return Presentation.Precision;
		}
	}

	public Type Type
	{
		get
		{
			if (Presentation == null)
			{
				return null;
			}
			return Presentation.Type;
		}
	}

	public bool Marked
	{
		get
		{
			return marked;
		}
		set
		{
			marked = value;
		}
	}

	public object CacheTime
	{
		get
		{
			return cacheTime;
		}
		set
		{
			cacheTime = value;
		}
	}

	public IList<Service> Services
	{
		get
		{
			if (service != null)
			{
				return Enumerable.Repeat(service, 1).ToList();
			}
			return referenced.Select((EcuInfo ei) => ei?.service).ToList();
		}
	}

	public IList<int?> ServiceDataArrayIndexes => referencedArrayIndexes;

	public Service CombinedService
	{
		get
		{
			if (service != null)
			{
				return service.CombinedService;
			}
			if (referenced != null)
			{
				IEnumerable<IGrouping<Service, EcuInfo>> source = from e in referenced
					where e != null
					group e by e.CombinedService;
				if (source.Count() != 1)
				{
					return null;
				}
				return source.First().Key;
			}
			return null;
		}
	}

	public bool PreventReferencedServiceRead { get; set; }

	public bool Common { get; internal set; }

	public bool Summary
	{
		get
		{
			if (!channel.Ecu.SummaryQualifier(Qualifier))
			{
				return summary;
			}
			return true;
		}
	}

	public int? MessageNumber { get; private set; }

	public IEnumerable<string> EnabledAdditionalAudiences
	{
		get
		{
			if (service != null)
			{
				return service.EnabledAdditionalAudiences;
			}
			if (referenced != null)
			{
				IEnumerable<EcuInfo> referencesWithAdditionalAudiences = referenced.Where((EcuInfo r) => r != null && r.EnabledAdditionalAudiences != null);
				if (referencesWithAdditionalAudiences.Any())
				{
					return from audience in referencesWithAdditionalAudiences.SelectMany((EcuInfo r) => r.EnabledAdditionalAudiences).Distinct()
						where referencesWithAdditionalAudiences.All((EcuInfo or) => or.EnabledAdditionalAudiences.Contains(audience))
						select audience;
				}
			}
			return null;
		}
	}

	public event EcuInfoUpdateEventHandler EcuInfoUpdateEvent;

	internal EcuInfo(Channel channel, EcuInfoType type, string qualifier, string name, string groupQualifier, string groupName, Presentation presentation, int messageNumber, bool visible, bool common, bool summary, int? cacheTime = null)
		: this(channel, type, qualifier, name, groupQualifier, groupName, string.Empty, null, visible)
	{
		MessageNumber = messageNumber;
		this.presentation = presentation;
		this.cacheTime = (cacheTime.HasValue ? ((object)cacheTime.Value) : null);
		Common = common;
		this.summary = summary;
	}

	internal EcuInfo(Channel channel, EcuInfoType type, string qualifier, string name, string groupQualifier, string groupName, string description, string formatString, bool visible, List<Tuple<EcuInfo, int?>> references, int? presentationIndex = null)
	{
		this.channel = channel;
		this.type = type;
		this.qualifier = qualifier;
		this.name = name;
		this.groupName = groupName;
		this.groupQualifier = groupQualifier;
		this.description = description;
		this.formatString = formatString;
		this.visible = visible;
		accessLevel = 1;
		marked = true;
		referenced = new List<EcuInfo>(references.Select((Tuple<EcuInfo, int?> r) => r.Item1)).AsReadOnly();
		referencedArrayIndexes = (references.Any((Tuple<EcuInfo, int?> r) => r.Item2.HasValue) ? new List<int?>(references.Select((Tuple<EcuInfo, int?> r) => r.Item2)).AsReadOnly() : null);
		if (presentationIndex.HasValue && presentationIndex.Value < referenced.Count && referenced[presentationIndex.Value] != null)
		{
			presentation = referenced[presentationIndex.Value].Presentation;
		}
		ecuInfoValues = new EcuInfoValueCollection(this);
		cacheTime = Channel.Ecu.CacheTimeQualifier(this.qualifier);
		Common = string.Equals(GroupQualifier, "Common", StringComparison.Ordinal);
		if (!Common)
		{
			return;
		}
		foreach (EcuInfo item in referenced.Where((EcuInfo r) => r != null))
		{
			item.Common = true;
		}
	}

	internal EcuInfo(Channel channel, EcuInfoType type, string qualifier, string name, string groupQualifier, string groupName, string description, string formatString, bool visible)
		: this(channel, type, qualifier, name, groupQualifier, groupName, description, formatString, visible, new List<Tuple<EcuInfo, int?>>())
	{
	}

	internal EcuInfo(Channel channel, Service service)
	{
		this.channel = channel;
		type = EcuInfoType.Service;
		qualifier = service.Qualifier;
		name = service.Name;
		groupQualifier = "StoredData";
		groupName = (service.BaseRequestMessage.Data.Take(2).SequenceEqual(DeviceIdentificationPrefix) ? "Stored Data/Device Identification" : "Stored Data");
		description = service.Description;
		formatString = "{0}";
		marked = true;
		visible = service.Visible;
		accessLevel = service.Access;
		referenced = new List<EcuInfo>();
		this.service = service;
		ecuInfoValues = new EcuInfoValueCollection(this);
		cacheTime = Channel.Ecu.CacheTimeQualifier(qualifier);
	}

	internal void InternalRead(bool explicitread)
	{
		InternalRead((!explicitread) ? ReadType.SystemInvoke : ReadType.UserInvoke);
	}

	private void InternalRead(ReadType readType)
	{
		CaesarException ex = null;
		object obj = null;
		if (!channel.IsRollCall)
		{
			if (!channel.IsChannelErrorSet)
			{
				switch (type)
				{
				case EcuInfoType.IdBlock:
					if (channel.ChannelHandle != null)
					{
						CaesarIdBlock idBlock = channel.ChannelHandle.IdBlock;
						if (string.Equals(qualifier, "MBNumber", StringComparison.Ordinal))
						{
							obj = idBlock.PartNumber;
						}
						if (string.Equals(qualifier, "SWVersionNumber", StringComparison.Ordinal))
						{
							obj = idBlock.SoftwareVersion.Value.ToString(CultureInfo.InvariantCulture);
						}
						if (string.Equals(qualifier, "DiagVersion", StringComparison.Ordinal))
						{
							obj = idBlock.DiagVersion.Value.ToString(CultureInfo.InvariantCulture);
						}
					}
					else if (channel.McdChannelHandle != null)
					{
						throw new NotSupportedException("This content not supported for MCD");
					}
					break;
				case EcuInfoType.Compound:
				{
					object[] array = new object[referenced.Count];
					for (int i = 0; i < referenced.Count; i++)
					{
						EcuInfo ecuInfo = referenced[i];
						array[i] = string.Empty;
						if (ecuInfo != null && !PreventReferencedServiceRead)
						{
							ecuInfo.InternalRead((readType == ReadType.UserInvoke) ? ReadType.ReferencedUserInvoke : ReadType.SystemInvoke);
							if (ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null)
							{
								array[i] = GetValueOrSpecificIndexValue(ecuInfo.EcuInfoValues.Current, i);
							}
						}
					}
					obj = BuildCompoundContent(array);
					break;
				}
				case EcuInfoType.Service:
					if (service != null && !channel.IsChannelErrorSet)
					{
						ex = service.InternalExecute((readType != ReadType.SystemInvoke) ? Service.ExecuteType.EcuInfoUserInvoke : Service.ExecuteType.EcuInfoInvoke);
						ServiceOutputValue serviceOutputValue = service.OutputValues[0];
						if (serviceOutputValue.Value != null)
						{
							obj = ((!(serviceOutputValue.Value.GetType() == typeof(string))) ? serviceOutputValue.Value : serviceOutputValue.Value.ToString().Trim());
						}
					}
					break;
				case EcuInfoType.DiagnosisVariant:
					if (string.Equals(qualifier, "DiagnosisVariant", StringComparison.Ordinal))
					{
						obj = ((channel.EcuHandle != null) ? channel.EcuHandle.VariantName : channel.DiagnosisVariant.Name);
					}
					else if (string.Equals(qualifier, "DiagnosisVariantPartNumber", StringComparison.Ordinal) && channel.DiagnosisVariant.PartNumber != null)
					{
						obj = channel.DiagnosisVariant.PartNumber.ToString();
					}
					break;
				}
			}
			if (channel.IsChannelErrorSet)
			{
				ex = new CaesarException(channel.ChannelHandle);
			}
		}
		else
		{
			try
			{
				if (ecuInfoValues.Current == null || lastRollCallUpdateTime < Sapi.Now - TimeSpan.FromMilliseconds(1000.0))
				{
					channel.Ecu.RollCallManager.ReadEcuInfo(this);
				}
				obj = ecuInfoValues.Current.Value;
			}
			catch (CaesarException ex2)
			{
				ex = ex2;
			}
		}
		bool flag = true;
		if (ecuInfoValues.Current == null || !object.Equals(obj, ecuInfoValues.Current.Value) || readType != ReadType.SystemInvoke)
		{
			heldValue = obj;
			if (manipulatedValue == null)
			{
				ecuInfoValues.Add(new EcuInfoValue(obj, Sapi.Now));
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			flag = false;
		}
		if (flag || ex != null || readType == ReadType.UserInvoke)
		{
			RaiseEcuInfoUpdateEvent(ex, readType == ReadType.UserInvoke);
		}
	}

	private object GetValueOrSpecificIndexValue(EcuInfoValue value, int referenceIndex)
	{
		object obj = value.Value;
		if (referencedArrayIndexes != null && referenceIndex < referencedArrayIndexes.Count && obj.GetType().IsArray)
		{
			int? num = referencedArrayIndexes[referenceIndex];
			if (num.HasValue)
			{
				Array array = obj as Array;
				obj = ((num.Value < array.Length) ? array.GetValue(num.Value) : null);
			}
		}
		return obj;
	}

	private object BuildCompoundContent(object[] output)
	{
		if (string.Equals(formatString, "{0}"))
		{
			return output[0];
		}
		if (formatString.StartsWith("extension:", StringComparison.OrdinalIgnoreCase))
		{
			return channel.Extension.Invoke(formatString.Substring("extension:".Length), output);
		}
		if (channel.Ecu.IsMcd && output.Length != 0 && output.All((object o) => o is object[]))
		{
			List<object[]> source = output.Select((object o) => o as object[]).ToList();
			object[] array = new string[source.Max((object[] o) => o.Length)];
			int i;
			for (i = 0; i < array.Length; i++)
			{
				object[] args = source.Select((object[] o) => (o.Length <= i) ? null : o[i]).ToArray();
				array[i] = string.Format(CultureInfo.InvariantCulture, formatString, args);
			}
			return array;
		}
		return string.Format(CultureInfo.InvariantCulture, formatString, output);
	}

	internal void UpdateFromRollCall(object value)
	{
		if (Choices != null)
		{
			if (value.GetType().IsArray)
			{
				value = (from object item in (Array)value
					let relatedChoice = Choices.GetItemFromRawValue(item)
					select (!(relatedChoice != null)) ? item : relatedChoice).ToArray();
			}
			else
			{
				Choice itemFromRawValue = Choices.GetItemFromRawValue(value);
				if (itemFromRawValue != null)
				{
					value = itemFromRawValue;
				}
			}
		}
		heldValue = value;
		if (manipulatedValue == null)
		{
			bool flag = false;
			if (ecuInfoValues.Current == null || ecuInfoValues.Current.Value == null)
			{
				flag = true;
			}
			else if (value.GetType().IsArray && ecuInfoValues.Current.Value.GetType().IsArray)
			{
				Array source = (Array)value;
				flag = !Enumerable.SequenceEqual(second: ((Array)ecuInfoValues.Current.Value).OfType<object>(), first: source.OfType<object>());
			}
			else
			{
				flag = !object.Equals(value, ecuInfoValues.Current.Value);
			}
			if (flag)
			{
				ecuInfoValues.Add(new EcuInfoValue(value, Sapi.Now));
				RaiseEcuInfoUpdateEvent(null, explicitread: false);
			}
		}
		lastRollCallUpdateTime = Sapi.Now;
	}

	internal void AddValue(EcuInfoValue v)
	{
		ecuInfoValues.Add(v);
		if (EcuInfoType == EcuInfoType.IdBlock)
		{
			visible = true;
		}
	}

	internal XElement GetXElement(DateTime startTime, DateTime endTime)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		XElement xElement = new XElement(currentFormat[TagName.EcuInfo], new XAttribute(currentFormat[TagName.Qualifier], Qualifier));
		EcuInfoValue ecuInfoValue = null;
		foreach (EcuInfoValue ecuInfoValue2 in ecuInfoValues)
		{
			if (ecuInfoValue2.Value == null)
			{
				continue;
			}
			if (ecuInfoValue2.Time >= startTime)
			{
				if (ecuInfoValue != null)
				{
					xElement.Add(ecuInfoValue.GetXElement(startTime));
					ecuInfoValue = null;
				}
				if (ecuInfoValue2.Time > endTime)
				{
					break;
				}
				xElement.Add(ecuInfoValue2.GetXElement(startTime));
			}
			else
			{
				ecuInfoValue = ecuInfoValue2;
			}
		}
		if (ecuInfoValue != null)
		{
			xElement.Add(ecuInfoValue.GetXElement(startTime));
		}
		return xElement;
	}

	internal static void LoadFromLog(XElement element, LogFileFormatTagCollection format, Channel channel, List<string> missingQualifierList, object missingInfoLock)
	{
		string value = element.Attribute(format[TagName.Qualifier]).Value;
		EcuInfo ecuInfo = channel.EcuInfos[value];
		if (ecuInfo == null && channel.Ecu.RollCallManager != null)
		{
			try
			{
				ecuInfo = channel.Ecu.RollCallManager.CreateEcuInfo(channel.EcuInfos, value);
			}
			catch (FormatException)
			{
			}
		}
		if (ecuInfo != null)
		{
			IEnumerable<XElement> enumerable = element.Elements(format[TagName.Value]);
			if (enumerable.Any())
			{
				foreach (XElement item in enumerable)
				{
					ecuInfo.AddValue(EcuInfoValue.FromXElement(item, format, ecuInfo));
				}
				return;
			}
			if (!string.IsNullOrEmpty(element.Value))
			{
				ecuInfo.AddValue(new EcuInfoValue(element.Value, channel.Sessions[channel.Sessions.Count - 1].StartTime));
			}
		}
		else if (channel.EcuInfos.GetItemContaining(value) == null && !channel.Ecu.IgnoreQualifier(value))
		{
			lock (missingInfoLock)
			{
				missingQualifierList.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", channel.Ecu.Name, value));
			}
		}
	}

	internal void RaiseEcuInfoUpdateEvent(Exception e, bool explicitread)
	{
		FireAndForget.Invoke(this.EcuInfoUpdateEvent, this, new ResultEventArgs(e));
		channel.EcuInfos.RaiseEcuInfoUpdateEvent(this, e);
		if (explicitread)
		{
			channel.SyncDone(e);
		}
	}

	public void Read(bool synchronous)
	{
		channel.QueueAction(this, synchronous);
	}

	internal void AddStringsForTranslation(Dictionary<string, string> table)
	{
		table[Sapi.MakeTranslationIdentifier(qualifier, "Name")] = name;
		string text = groupName.CreateQualifierFromName();
		int num = groupName.IndexOf('/');
		if (num != -1)
		{
			table[Sapi.MakeTranslationIdentifier(text, "GroupName")] = groupName.Substring(0, num);
			table[Sapi.MakeTranslationIdentifier(text, "GroupSubName")] = groupName.Substring(num + 1);
		}
		else
		{
			table[Sapi.MakeTranslationIdentifier(text, "GroupName")] = groupName;
		}
		if (!string.IsNullOrEmpty(description))
		{
			table[Sapi.MakeTranslationIdentifier(qualifier, "Description")] = description;
		}
		if (Choices == null)
		{
			return;
		}
		foreach (Choice choice in Choices)
		{
			choice.AddStringsForTranslation(table);
		}
	}

	internal void LoadCompoundFromLog(DateTime sessionStartTime, DateTime sessionEndTime)
	{
		if (referenced.Count > 0)
		{
			IEnumerable<DateTime> enumerable = from t in (from eiv in referenced.Where((EcuInfo r) => r != null).SelectMany((EcuInfo ei) => ei.EcuInfoValues)
					where eiv.Time >= sessionStartTime && eiv.Time <= sessionEndTime
					select eiv.Time).Distinct()
				orderby t
				select t;
			if (!enumerable.Any() || ecuInfoValues.Any((EcuInfoValue eiv) => eiv.Time >= sessionStartTime && eiv.Time <= sessionEndTime))
			{
				return;
			}
			List<Tuple<DateTime, object[]>> list = new List<Tuple<DateTime, object[]>>();
			foreach (DateTime item in enumerable)
			{
				object[] array = new object[referenced.Count];
				for (int num = 0; num < referenced.Count; num++)
				{
					if (referenced[num] != null)
					{
						EcuInfoValue currentAtTime = referenced[num].EcuInfoValues.GetCurrentAtTime(item);
						if (currentAtTime != null && currentAtTime.Value != null)
						{
							array[num] = GetValueOrSpecificIndexValue(currentAtTime, num);
						}
					}
				}
				list.Add(Tuple.Create(item, array));
			}
			{
				foreach (Tuple<DateTime, object[]> item2 in (from dpc in list
					group dpc by dpc.Item2.Count((object v) => v != null) into g
					orderby g.Key
					select g).Last())
				{
					object ecuInfoValue = BuildCompoundContent(item2.Item2.Select((object v) => v ?? string.Empty).ToArray());
					ecuInfoValues.Add(new EcuInfoValue(ecuInfoValue, item2.Item1));
				}
				return;
			}
		}
		ecuInfoValues.Add(new EcuInfoValue(BuildCompoundContent(new object[0]), sessionStartTime));
	}
}
