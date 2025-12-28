// Decompiled with JetBrains decompiler
// Type: SapiLayer1.EcuInfo
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class EcuInfo : IDiogenesDataItem
{
  private const int RollCallMessageCacheTime = 1000;
  private static byte[] DeviceIdentificationPrefix = new byte[2]
  {
    (byte) 34,
    (byte) 241
  };
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

  internal EcuInfo(
    Channel channel,
    EcuInfoType type,
    string qualifier,
    string name,
    string groupQualifier,
    string groupName,
    Presentation presentation,
    int messageNumber,
    bool visible,
    bool common,
    bool summary,
    int? cacheTime = null)
    : this(channel, type, qualifier, name, groupQualifier, groupName, string.Empty, (string) null, visible)
  {
    this.MessageNumber = new int?(messageNumber);
    this.presentation = presentation;
    this.cacheTime = cacheTime.HasValue ? (object) cacheTime.Value : (object) null;
    this.Common = common;
    this.summary = summary;
  }

  internal EcuInfo(
    Channel channel,
    EcuInfoType type,
    string qualifier,
    string name,
    string groupQualifier,
    string groupName,
    string description,
    string formatString,
    bool visible,
    List<Tuple<EcuInfo, int?>> references,
    int? presentationIndex = null)
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
    this.accessLevel = 1;
    this.marked = true;
    this.referenced = (IList<EcuInfo>) new List<EcuInfo>(references.Select<Tuple<EcuInfo, int?>, EcuInfo>((Func<Tuple<EcuInfo, int?>, EcuInfo>) (r => r.Item1))).AsReadOnly();
    this.referencedArrayIndexes = references.Any<Tuple<EcuInfo, int?>>((Func<Tuple<EcuInfo, int?>, bool>) (r => r.Item2.HasValue)) ? (IList<int?>) new List<int?>(references.Select<Tuple<EcuInfo, int?>, int?>((Func<Tuple<EcuInfo, int?>, int?>) (r => r.Item2))).AsReadOnly() : (IList<int?>) null;
    if (presentationIndex.HasValue && presentationIndex.Value < this.referenced.Count && this.referenced[presentationIndex.Value] != null)
      this.presentation = this.referenced[presentationIndex.Value].Presentation;
    this.ecuInfoValues = new EcuInfoValueCollection(this);
    this.cacheTime = this.Channel.Ecu.CacheTimeQualifier(this.qualifier);
    this.Common = string.Equals(this.GroupQualifier, nameof (Common), StringComparison.Ordinal);
    if (!this.Common)
      return;
    foreach (EcuInfo ecuInfo in this.referenced.Where<EcuInfo>((Func<EcuInfo, bool>) (r => r != null)))
      ecuInfo.Common = true;
  }

  internal EcuInfo(
    Channel channel,
    EcuInfoType type,
    string qualifier,
    string name,
    string groupQualifier,
    string groupName,
    string description,
    string formatString,
    bool visible)
    : this(channel, type, qualifier, name, groupQualifier, groupName, description, formatString, visible, new List<Tuple<EcuInfo, int?>>())
  {
  }

  internal EcuInfo(Channel channel, Service service)
  {
    this.channel = channel;
    this.type = EcuInfoType.Service;
    this.qualifier = service.Qualifier;
    this.name = service.Name;
    this.groupQualifier = "StoredData";
    this.groupName = service.BaseRequestMessage.Data.Take<byte>(2).SequenceEqual<byte>((IEnumerable<byte>) EcuInfo.DeviceIdentificationPrefix) ? "Stored Data/Device Identification" : "Stored Data";
    this.description = service.Description;
    this.formatString = "{0}";
    this.marked = true;
    this.visible = service.Visible;
    this.accessLevel = service.Access;
    this.referenced = (IList<EcuInfo>) new List<EcuInfo>();
    this.service = service;
    this.ecuInfoValues = new EcuInfoValueCollection(this);
    this.cacheTime = this.Channel.Ecu.CacheTimeQualifier(this.qualifier);
  }

  internal void InternalRead(bool explicitread)
  {
    this.InternalRead(explicitread ? EcuInfo.ReadType.UserInvoke : EcuInfo.ReadType.SystemInvoke);
  }

  private void InternalRead(EcuInfo.ReadType readType)
  {
    CaesarException e = (CaesarException) null;
    object obj = (object) null;
    if (!this.channel.IsRollCall)
    {
      if (!this.channel.IsChannelErrorSet)
      {
        switch (this.type)
        {
          case EcuInfoType.IdBlock:
            if (this.channel.ChannelHandle != null)
            {
              CaesarIdBlock idBlock = this.channel.ChannelHandle.IdBlock;
              if (string.Equals(this.qualifier, "MBNumber", StringComparison.Ordinal))
                obj = (object) idBlock.PartNumber;
              ushort? nullable;
              if (string.Equals(this.qualifier, "SWVersionNumber", StringComparison.Ordinal))
              {
                nullable = idBlock.SoftwareVersion;
                obj = (object) nullable.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              }
              if (string.Equals(this.qualifier, "DiagVersion", StringComparison.Ordinal))
              {
                nullable = idBlock.DiagVersion;
                obj = (object) nullable.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                break;
              }
              break;
            }
            if (this.channel.McdChannelHandle != null)
              throw new NotSupportedException("This content not supported for MCD");
            break;
          case EcuInfoType.DiagnosisVariant:
            if (string.Equals(this.qualifier, "DiagnosisVariant", StringComparison.Ordinal))
            {
              obj = this.channel.EcuHandle != null ? (object) this.channel.EcuHandle.VariantName : (object) this.channel.DiagnosisVariant.Name;
              break;
            }
            if (string.Equals(this.qualifier, "DiagnosisVariantPartNumber", StringComparison.Ordinal) && this.channel.DiagnosisVariant.PartNumber != null)
            {
              obj = (object) this.channel.DiagnosisVariant.PartNumber.ToString();
              break;
            }
            break;
          case EcuInfoType.Service:
            if (this.service != (Service) null && !this.channel.IsChannelErrorSet)
            {
              e = this.service.InternalExecute(readType != EcuInfo.ReadType.SystemInvoke ? Service.ExecuteType.EcuInfoUserInvoke : Service.ExecuteType.EcuInfoInvoke);
              ServiceOutputValue outputValue = this.service.OutputValues[0];
              if (outputValue.Value != null)
              {
                obj = !(outputValue.Value.GetType() == typeof (string)) ? outputValue.Value : (object) outputValue.Value.ToString().Trim();
                break;
              }
              break;
            }
            break;
          case EcuInfoType.Compound:
            object[] output = new object[this.referenced.Count];
            for (int index = 0; index < this.referenced.Count; ++index)
            {
              EcuInfo ecuInfo = this.referenced[index];
              output[index] = (object) string.Empty;
              if (ecuInfo != null && !this.PreventReferencedServiceRead)
              {
                ecuInfo.InternalRead(readType == EcuInfo.ReadType.UserInvoke ? EcuInfo.ReadType.ReferencedUserInvoke : EcuInfo.ReadType.SystemInvoke);
                if (ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null)
                  output[index] = this.GetValueOrSpecificIndexValue(ecuInfo.EcuInfoValues.Current, index);
              }
            }
            obj = this.BuildCompoundContent(output);
            break;
        }
      }
      if (this.channel.IsChannelErrorSet)
        e = new CaesarException(this.channel.ChannelHandle);
    }
    else
    {
      try
      {
        if (this.ecuInfoValues.Current == null || this.lastRollCallUpdateTime < Sapi.Now - TimeSpan.FromMilliseconds(1000.0))
          this.channel.Ecu.RollCallManager.ReadEcuInfo(this);
        obj = this.ecuInfoValues.Current.Value;
      }
      catch (CaesarException ex)
      {
        e = ex;
      }
    }
    bool flag = true;
    if (this.ecuInfoValues.Current == null || !object.Equals(obj, this.ecuInfoValues.Current.Value) || readType != EcuInfo.ReadType.SystemInvoke)
    {
      this.heldValue = obj;
      if (this.manipulatedValue == null)
        this.ecuInfoValues.Add(new EcuInfoValue(obj, Sapi.Now));
      else
        flag = false;
    }
    else
      flag = false;
    if (!flag && e == null && readType != EcuInfo.ReadType.UserInvoke)
      return;
    this.RaiseEcuInfoUpdateEvent((Exception) e, readType == EcuInfo.ReadType.UserInvoke);
  }

  private object GetValueOrSpecificIndexValue(EcuInfoValue value, int referenceIndex)
  {
    object specificIndexValue = value.Value;
    if (this.referencedArrayIndexes != null && referenceIndex < this.referencedArrayIndexes.Count && specificIndexValue.GetType().IsArray)
    {
      int? referencedArrayIndex = this.referencedArrayIndexes[referenceIndex];
      if (referencedArrayIndex.HasValue)
      {
        Array array = specificIndexValue as Array;
        specificIndexValue = referencedArrayIndex.Value < array.Length ? array.GetValue(referencedArrayIndex.Value) : (object) null;
      }
    }
    return specificIndexValue;
  }

  private object BuildCompoundContent(object[] output)
  {
    if (string.Equals(this.formatString, "{0}"))
      return output[0];
    if (this.formatString.StartsWith("extension:", StringComparison.OrdinalIgnoreCase))
      return this.channel.Extension.Invoke(this.formatString.Substring("extension:".Length), output);
    if (!this.channel.Ecu.IsMcd || output.Length == 0 || !((IEnumerable<object>) output).All<object>((Func<object, bool>) (o => o is object[])))
      return (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.formatString, output);
    List<object[]> list = ((IEnumerable<object>) output).Select<object, object[]>((Func<object, object[]>) (o => o as object[])).ToList<object[]>();
    object[] objArray = (object[]) new string[list.Max<object[]>((Func<object[], int>) (o => o.Length))];
    for (int i = 0; i < objArray.Length; i++)
    {
      object[] array = list.Select<object[], object>((Func<object[], object>) (o => o.Length <= i ? (object) null : o[i])).ToArray<object>();
      objArray[i] = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.formatString, array);
    }
    return (object) objArray;
  }

  internal void UpdateFromRollCall(object value)
  {
    if (this.Choices != null)
    {
      if (value.GetType().IsArray)
      {
        value = (object) ((Array) value).Cast<object>().Select(item => new
        {
          item = item,
          relatedChoice = this.Choices.GetItemFromRawValue(item)
        }).Select(_param1 => !(_param1.relatedChoice != (object) null) ? _param1.item : (object) _param1.relatedChoice).ToArray<object>();
      }
      else
      {
        Choice itemFromRawValue = this.Choices.GetItemFromRawValue(value);
        if (itemFromRawValue != (object) null)
          value = (object) itemFromRawValue;
      }
    }
    this.heldValue = value;
    if (this.manipulatedValue == null)
    {
      bool flag;
      if (this.ecuInfoValues.Current == null || this.ecuInfoValues.Current.Value == null)
        flag = true;
      else if (value.GetType().IsArray && this.ecuInfoValues.Current.Value.GetType().IsArray)
      {
        Array source1 = (Array) value;
        Array source2 = (Array) this.ecuInfoValues.Current.Value;
        flag = !source1.OfType<object>().SequenceEqual<object>(source2.OfType<object>());
      }
      else
        flag = !object.Equals(value, this.ecuInfoValues.Current.Value);
      if (flag)
      {
        this.ecuInfoValues.Add(new EcuInfoValue(value, Sapi.Now));
        this.RaiseEcuInfoUpdateEvent((Exception) null, false);
      }
    }
    this.lastRollCallUpdateTime = Sapi.Now;
  }

  internal void AddValue(EcuInfoValue v)
  {
    this.ecuInfoValues.Add(v);
    if (this.EcuInfoType != EcuInfoType.IdBlock)
      return;
    this.visible = true;
  }

  internal XElement GetXElement(DateTime startTime, DateTime endTime)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    XElement xelement = new XElement(currentFormat[TagName.EcuInfo], (object) new XAttribute(currentFormat[TagName.Qualifier], (object) this.Qualifier));
    EcuInfoValue ecuInfoValue1 = (EcuInfoValue) null;
    foreach (EcuInfoValue ecuInfoValue2 in this.ecuInfoValues)
    {
      if (ecuInfoValue2.Value != null)
      {
        if (ecuInfoValue2.Time >= startTime)
        {
          if (ecuInfoValue1 != null)
          {
            xelement.Add((object) ecuInfoValue1.GetXElement(startTime));
            ecuInfoValue1 = (EcuInfoValue) null;
          }
          if (!(ecuInfoValue2.Time > endTime))
            xelement.Add((object) ecuInfoValue2.GetXElement(startTime));
          else
            break;
        }
        else
          ecuInfoValue1 = ecuInfoValue2;
      }
    }
    if (ecuInfoValue1 != null)
      xelement.Add((object) ecuInfoValue1.GetXElement(startTime));
    return xelement;
  }

  internal static void LoadFromLog(
    XElement element,
    LogFileFormatTagCollection format,
    Channel channel,
    List<string> missingQualifierList,
    object missingInfoLock)
  {
    string str = element.Attribute(format[TagName.Qualifier]).Value;
    EcuInfo ecuInfo = channel.EcuInfos[str];
    if (ecuInfo == null)
    {
      if (channel.Ecu.RollCallManager != null)
      {
        try
        {
          ecuInfo = channel.Ecu.RollCallManager.CreateEcuInfo(channel.EcuInfos, str);
        }
        catch (FormatException ex)
        {
        }
      }
    }
    if (ecuInfo != null)
    {
      IEnumerable<XElement> source = element.Elements(format[TagName.Value]);
      if (source.Any<XElement>())
      {
        foreach (XElement element1 in source)
          ecuInfo.AddValue(EcuInfoValue.FromXElement(element1, format, ecuInfo));
      }
      else
      {
        if (string.IsNullOrEmpty(element.Value))
          return;
        ecuInfo.AddValue(new EcuInfoValue((object) element.Value, channel.Sessions[channel.Sessions.Count - 1].StartTime));
      }
    }
    else
    {
      if (channel.EcuInfos.GetItemContaining(str) != null || channel.Ecu.IgnoreQualifier(str))
        return;
      lock (missingInfoLock)
        missingQualifierList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) channel.Ecu.Name, (object) str));
    }
  }

  internal void RaiseEcuInfoUpdateEvent(Exception e, bool explicitread)
  {
    FireAndForget.Invoke((MulticastDelegate) this.EcuInfoUpdateEvent, (object) this, (EventArgs) new ResultEventArgs(e));
    this.channel.EcuInfos.RaiseEcuInfoUpdateEvent(this, e);
    if (!explicitread)
      return;
    this.channel.SyncDone(e);
  }

  internal bool NeedsUpdate
  {
    get
    {
      return this.cacheTime != null && Sapi.Now > this.LastCyclicAttemptTime + TimeSpan.FromMilliseconds((double) Convert.ToInt32(this.cacheTime, (IFormatProvider) CultureInfo.InvariantCulture));
    }
  }

  internal DateTime LastCyclicAttemptTime { get; set; }

  public Presentation Presentation
  {
    get
    {
      if (this.presentation != null)
        return this.presentation;
      if (this.referenced.Count == 1)
      {
        if (this.referenced[0] != null)
          return this.referenced[0].Presentation;
      }
      else if (this.service != (Service) null && this.service.OutputValues.Count > 0)
        return (Presentation) this.service.OutputValues[0];
      return (Presentation) null;
    }
  }

  public Channel Channel => this.channel;

  public string Qualifier => this.qualifier;

  public string Name
  {
    get
    {
      return this.channel.Ecu.RollCallManager != null ? this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.qualifier, "SPN"), this.name) : this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.qualifier, nameof (Name)), this.name);
    }
  }

  public string ShortName => this.channel.Ecu.ShortName(this.Name);

  public string Description
  {
    get
    {
      return this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.qualifier, nameof (Description)), this.description);
    }
  }

  public string GroupName
  {
    get
    {
      if (this.derivedGroupQualifier == null)
        this.derivedGroupQualifier = this.groupName.CreateQualifierFromName();
      int length = this.groupName.IndexOf('/');
      return length != -1 ? $"{this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.derivedGroupQualifier, nameof (GroupName)), this.groupName.Substring(0, length))}/{this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.derivedGroupQualifier, "GroupSubName"), this.groupName.Substring(length + 1))}" : this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.derivedGroupQualifier, nameof (GroupName)), this.groupName);
    }
  }

  public string OriginalGroupName => this.groupName;

  public string GroupQualifier => this.groupQualifier;

  public string Value
  {
    get
    {
      string str = string.Empty;
      EcuInfoValue current = this.ecuInfoValues.Current;
      if (current != null && current.Value != null)
        str = !current.Value.GetType().IsArray ? current.Value.ToString() : string.Join<object>(", ", ((Array) current.Value).Cast<object>());
      return str;
    }
  }

  public object ManipulatedValue
  {
    get => this.manipulatedValue;
    set
    {
      if (this.Channel.LogFile != null)
        throw new InvalidOperationException("Cannot manipulate a value for a log file channel");
      if (value == this.manipulatedValue)
        return;
      this.manipulatedValue = value;
      this.ecuInfoValues.Add(new EcuInfoValue(this.manipulatedValue ?? this.heldValue, Sapi.Now));
      this.RaiseEcuInfoUpdateEvent((Exception) null, false);
      this.Channel.SetManipulatedState(this.Qualifier, value != null);
    }
  }

  public ChoiceCollection Choices
  {
    get => this.Presentation == null ? (ChoiceCollection) null : this.Presentation.Choices;
  }

  public EcuInfoValueCollection EcuInfoValues => this.ecuInfoValues;

  public EcuInfoType EcuInfoType => this.type;

  public bool Visible
  {
    get => (this.Channel.LogFile == null || this.EcuInfoValues.Count != 0) && this.visible;
    internal set => this.visible = value;
  }

  public int AccessLevel => this.accessLevel;

  public string Units => this.Presentation == null ? string.Empty : this.Presentation.Units;

  public object Precision
  {
    get => this.Presentation == null ? (object) null : this.Presentation.Precision;
  }

  public Type Type => this.Presentation == null ? (Type) null : this.Presentation.Type;

  public bool Marked
  {
    get => this.marked;
    set => this.marked = value;
  }

  public object CacheTime
  {
    get => this.cacheTime;
    set => this.cacheTime = value;
  }

  public IList<Service> Services
  {
    get
    {
      return this.service != (Service) null ? (IList<Service>) Enumerable.Repeat<Service>(this.service, 1).ToList<Service>() : (IList<Service>) this.referenced.Select<EcuInfo, Service>((Func<EcuInfo, Service>) (ei => ei?.service)).ToList<Service>();
    }
  }

  public IList<int?> ServiceDataArrayIndexes => this.referencedArrayIndexes;

  public Service CombinedService
  {
    get
    {
      if (this.service != (Service) null)
        return this.service.CombinedService;
      if (this.referenced == null)
        return (Service) null;
      IEnumerable<IGrouping<Service, EcuInfo>> source = this.referenced.Where<EcuInfo>((Func<EcuInfo, bool>) (e => e != null)).GroupBy<EcuInfo, Service>((Func<EcuInfo, Service>) (e => e.CombinedService));
      return source.Count<IGrouping<Service, EcuInfo>>() != 1 ? (Service) null : source.First<IGrouping<Service, EcuInfo>>().Key;
    }
  }

  public bool PreventReferencedServiceRead { get; set; }

  public bool Common { get; internal set; }

  public bool Summary => this.channel.Ecu.SummaryQualifier(this.Qualifier) || this.summary;

  public int? MessageNumber { private set; get; }

  public IEnumerable<string> EnabledAdditionalAudiences
  {
    get
    {
      if (this.service != (Service) null)
        return this.service.EnabledAdditionalAudiences;
      if (this.referenced != null)
      {
        IEnumerable<EcuInfo> referencesWithAdditionalAudiences = this.referenced.Where<EcuInfo>((Func<EcuInfo, bool>) (r => r != null && r.EnabledAdditionalAudiences != null));
        if (referencesWithAdditionalAudiences.Any<EcuInfo>())
          return referencesWithAdditionalAudiences.SelectMany<EcuInfo, string>((Func<EcuInfo, IEnumerable<string>>) (r => r.EnabledAdditionalAudiences)).Distinct<string>().Where<string>((Func<string, bool>) (audience => referencesWithAdditionalAudiences.All<EcuInfo>((Func<EcuInfo, bool>) (or => or.EnabledAdditionalAudiences.Contains<string>(audience)))));
      }
      return (IEnumerable<string>) null;
    }
  }

  public void Read(bool synchronous) => this.channel.QueueAction((object) this, synchronous);

  public event EcuInfoUpdateEventHandler EcuInfoUpdateEvent;

  internal void AddStringsForTranslation(Dictionary<string, string> table)
  {
    table[Sapi.MakeTranslationIdentifier(this.qualifier, "Name")] = this.name;
    string qualifierFromName = this.groupName.CreateQualifierFromName();
    int length = this.groupName.IndexOf('/');
    if (length != -1)
    {
      table[Sapi.MakeTranslationIdentifier(qualifierFromName, "GroupName")] = this.groupName.Substring(0, length);
      table[Sapi.MakeTranslationIdentifier(qualifierFromName, "GroupSubName")] = this.groupName.Substring(length + 1);
    }
    else
      table[Sapi.MakeTranslationIdentifier(qualifierFromName, "GroupName")] = this.groupName;
    if (!string.IsNullOrEmpty(this.description))
      table[Sapi.MakeTranslationIdentifier(this.qualifier, "Description")] = this.description;
    if (this.Choices == null)
      return;
    foreach (Choice choice in (ReadOnlyCollection<Choice>) this.Choices)
      choice.AddStringsForTranslation(table);
  }

  internal void LoadCompoundFromLog(DateTime sessionStartTime, DateTime sessionEndTime)
  {
    if (this.referenced.Count > 0)
    {
      IEnumerable<DateTime> source1 = (IEnumerable<DateTime>) this.referenced.Where<EcuInfo>((Func<EcuInfo, bool>) (r => r != null)).SelectMany<EcuInfo, EcuInfoValue>((Func<EcuInfo, IEnumerable<EcuInfoValue>>) (ei => (IEnumerable<EcuInfoValue>) ei.EcuInfoValues)).Where<EcuInfoValue>((Func<EcuInfoValue, bool>) (eiv => eiv.Time >= sessionStartTime && eiv.Time <= sessionEndTime)).Select<EcuInfoValue, DateTime>((Func<EcuInfoValue, DateTime>) (eiv => eiv.Time)).Distinct<DateTime>().OrderBy<DateTime, DateTime>((Func<DateTime, DateTime>) (t => t));
      if (!source1.Any<DateTime>() || this.ecuInfoValues.Any<EcuInfoValue>((Func<EcuInfoValue, bool>) (eiv => eiv.Time >= sessionStartTime && eiv.Time <= sessionEndTime)))
        return;
      List<Tuple<DateTime, object[]>> source2 = new List<Tuple<DateTime, object[]>>();
      foreach (DateTime time in source1)
      {
        object[] objArray = new object[this.referenced.Count];
        for (int index = 0; index < this.referenced.Count; ++index)
        {
          if (this.referenced[index] != null)
          {
            EcuInfoValue currentAtTime = this.referenced[index].EcuInfoValues.GetCurrentAtTime(time);
            if (currentAtTime != null && currentAtTime.Value != null)
              objArray[index] = this.GetValueOrSpecificIndexValue(currentAtTime, index);
          }
        }
        source2.Add(Tuple.Create<DateTime, object[]>(time, objArray));
      }
      foreach (Tuple<DateTime, object[]> tuple in (IEnumerable<Tuple<DateTime, object[]>>) source2.GroupBy<Tuple<DateTime, object[]>, int>((Func<Tuple<DateTime, object[]>, int>) (dpc => ((IEnumerable<object>) dpc.Item2).Count<object>((Func<object, bool>) (v => v != null)))).OrderBy<IGrouping<int, Tuple<DateTime, object[]>>, int>((Func<IGrouping<int, Tuple<DateTime, object[]>>, int>) (g => g.Key)).Last<IGrouping<int, Tuple<DateTime, object[]>>>())
        this.ecuInfoValues.Add(new EcuInfoValue(this.BuildCompoundContent(((IEnumerable<object>) tuple.Item2).Select<object, object>((Func<object, object>) (v => v ?? (object) string.Empty)).ToArray<object>()), tuple.Item1));
    }
    else
      this.ecuInfoValues.Add(new EcuInfoValue(this.BuildCompoundContent(new object[0]), sessionStartTime));
  }

  private enum ReadType
  {
    UserInvoke,
    ReferencedUserInvoke,
    SystemInvoke,
  }
}
