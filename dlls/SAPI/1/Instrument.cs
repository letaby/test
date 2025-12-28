// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Instrument
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class Instrument : Presentation, IComparable, IDiogenesDataItem, IDisposable
{
  private int waitingForUpdate;
  private CaesarDiagServiceIO diagServiceIO;
  private McdDiagComPrimitive mcdDiagComPrimitive;
  private bool summary;
  private object heldValue;
  private object manipulatedValue;
  private InstrumentValueCollection instrumentValues;
  private string qualifier;
  private string mcdQualifier;
  private string name;
  private string description;
  private string groupName;
  private string groupQualifier;
  private bool hideServiceAsInstrument;
  private bool marked;
  private bool periodic;
  private int accessLevel;
  private ushort cacheTime;
  private int? minimumCacheTime;
  private bool forceRequest;
  private Dump requestMessage;
  private Dump proprietaryContent;
  private CaesarAPtr indexTag;
  private bool disposed;
  private ServiceInputValueCollection inputValues;
  private string userQualifier;
  private IEnumerable<string> enabledAdditionalAudiences;

  internal Instrument(Channel channel, string qualifier, ushort index = 0)
    : base(index)
  {
    this.name = string.Empty;
    this.description = string.Empty;
    this.groupName = string.Empty;
    this.groupQualifier = string.Empty;
    this.channel = channel;
    this.qualifier = qualifier;
    this.cacheTime = (ushort) 100;
    this.instrumentValues = new InstrumentValueCollection(this);
    this.marked = true;
  }

  internal Instrument(
    Channel channel,
    string qualifier,
    string userQualifier,
    string userName,
    ServiceInputValueCollection userValues,
    bool hide,
    bool marked)
    : base((ushort) 0)
  {
    this.name = userName;
    this.description = string.Empty;
    this.groupName = string.Empty;
    this.groupQualifier = string.Empty;
    this.channel = channel;
    this.qualifier = qualifier;
    this.userQualifier = userQualifier;
    this.cacheTime = (ushort) 100;
    this.inputValues = userValues;
    this.instrumentValues = new InstrumentValueCollection(this);
    this.hideServiceAsInstrument = hide;
    this.marked = marked;
  }

  ~Instrument() => this.Dispose(false);

  internal void AcquireFromRollCall(IDictionary<string, string> content)
  {
    this.MessageNumber = new int?(content.GetNamedPropertyValue<int>("MessageNumber", -1));
    this.proprietaryContent = content.ContainsKey("ProprietaryContent") ? new Dump(content.GetNamedPropertyValue<string>("ProprietaryContent", string.Empty)) : (Dump) null;
    this.requestMessage = new Dump(content.GetNamedPropertyValue<string>("RequestMessage", string.Empty));
    this.periodic = content.GetNamedPropertyValue<bool>("Periodic", false);
    this.cacheTime = (ushort) content.GetNamedPropertyValue<int>("CacheTime", 100);
    this.name = content.GetNamedPropertyValue<string>("Name", string.Empty);
    this.description = content.GetNamedPropertyValue<string>("Description", string.Empty);
    this.summary = content.GetNamedPropertyValue<bool>("Summary", false);
    string namedPropertyValue = content.GetNamedPropertyValue<string>("DestinationAddress", string.Empty);
    if (!string.IsNullOrEmpty(namedPropertyValue))
      this.DestinationAddress = new byte?(Convert.ToByte(namedPropertyValue, (IFormatProvider) CultureInfo.InvariantCulture));
    if (this.channel != null && this.channel.Ecu.RollCallManager.Protocol == Protocol.J1939 && this.channel.SourceAddress.HasValue && this.requestMessage.Data.Count > 6)
    {
      byte[] array = this.requestMessage.Data.ToArray<byte>();
      array[5] = this.channel.SourceAddress.Value;
      this.requestMessage = new Dump((IEnumerable<byte>) array);
    }
    Ecu ecu = this.channel != null ? this.channel.Ecu : (Ecu) null;
    string qualifier = this.qualifier;
    IDictionary<string, string> content1 = content;
    int? messageNumber = this.MessageNumber;
    int num1 = 61184;
    int num2 = messageNumber.GetValueOrDefault() == num1 ? (messageNumber.HasValue ? 1 : 0) : 0;
    this.AcquireFromRollCall(ecu, qualifier, content1, num2 != 0);
  }

  public int? MessageNumber { private set; get; }

  internal void UpdateFromRollCall(byte[] data)
  {
    if (this.waitingForUpdate != 0)
      return;
    try
    {
      if (!this.GetPresentation((object) data, false) && !this.channel.Instruments.UpdateOnEveryRead)
        return;
      this.RaiseInstrumentUpdateEvent((Exception) null, false);
    }
    catch (CaesarException ex)
    {
      this.RaiseInstrumentUpdateEvent((Exception) ex, false);
    }
  }

  internal void Acquire(string name, Service diagService, ServiceOutputValue responseParameter)
  {
    this.CombinedService = diagService;
    this.requestMessage = diagService.RequestMessage;
    this.name = name;
    this.mcdQualifier = diagService.McdQualifier;
    this.Acquire(this.channel, (Presentation) responseParameter);
    this.enabledAdditionalAudiences = diagService.EnabledAdditionalAudiences;
  }

  public Service CombinedService { private set; get; }

  internal void Acquire()
  {
    try
    {
      using (CaesarDiagService diagService = this.channel.EcuHandle.OpenDiagServiceHandle(this.qualifier))
      {
        if (diagService != null)
        {
          if (diagService.PresParamCount > 0U)
            this.Acquire(this.channel, diagService);
          this.periodic = diagService.IsSubscriptionSupported;
          if (this.name.Length == 0)
            this.name = diagService.Name;
          this.description = diagService.Description;
          IList<byte> requestMessage = diagService.RequestMessage;
          if (requestMessage != null)
            this.requestMessage = new Dump((IEnumerable<byte>) requestMessage);
          this.accessLevel = (int) diagService.AccessLevel;
          int? infoReadAccessLevel = this.Channel.DiagnosisVariant.GetEcuInfoReadAccessLevel(this.Qualifier);
          if (infoReadAccessLevel.HasValue)
            this.accessLevel = infoReadAccessLevel.Value;
        }
      }
    }
    catch (CaesarErrorException ex)
    {
      Sapi.GetSapi().RaiseExceptionEvent((object) this.qualifier, (Exception) new CaesarException(ex));
    }
    if (this.Units.Length > 0)
    {
      this.groupName = this.Units;
      this.groupQualifier = this.groupName.CreateQualifierFromName();
    }
    object obj = this.channel.Ecu.CacheTimeQualifier(this.qualifier);
    if (obj == null)
      return;
    this.minimumCacheTime = new int?((int) (this.cacheTime = Convert.ToUInt16(obj, (IFormatProvider) CultureInfo.InvariantCulture)));
    this.forceRequest = this.channel.Ecu.ForceRequestQualifier(this.qualifier);
  }

  internal void InternalRead(bool explicitread)
  {
    CaesarException ex1 = (CaesarException) null;
    bool flag = false;
    if (!this.channel.IsRollCall)
    {
      if (this.diagServiceIO == null && this.mcdDiagComPrimitive == null)
      {
        if (this.channel.ChannelHandle != null)
          this.diagServiceIO = this.channel.ChannelHandle.CreateDiagServiceIO(this.qualifier);
        else if (this.channel.McdChannelHandle != null)
          this.mcdDiagComPrimitive = this.channel.McdChannelHandle.GetService(this.mcdQualifier);
      }
      if (this.diagServiceIO != null || this.mcdDiagComPrimitive != null)
      {
        if (!this.channel.IsChannelErrorSet)
        {
          if (this.diagServiceIO != null)
          {
            if (this.inputValues != null)
            {
              for (int index = 0; index < this.inputValues.Count && !this.channel.IsChannelErrorSet; ++index)
                this.inputValues[index].SetPreparation(this.diagServiceIO);
            }
            this.diagServiceIO.Do((ushort) 100);
            if (this.channel.ChannelHandle.IsErrorSet)
            {
              ex1 = new CaesarException(this.channel.ChannelHandle);
              if (ex1.ErrorNumber == 6058L)
                ex1 = (CaesarException) null;
            }
          }
          else
          {
            if (this.inputValues != null)
            {
              for (int index = 0; index < this.inputValues.Count && !this.channel.IsChannelErrorSet; ++index)
                this.inputValues[index].SetPreparation(this.mcdDiagComPrimitive);
            }
            try
            {
              this.mcdDiagComPrimitive.Execute(100);
            }
            catch (McdException ex2)
            {
              ex1 = new CaesarException(ex2);
            }
          }
          if (ex1 == null)
          {
            try
            {
              flag = this.GetPresentation((object) this.diagServiceIO ?? (object) this.mcdDiagComPrimitive, explicitread);
            }
            catch (CaesarException ex3)
            {
              ex1 = ex3;
            }
            catch (McdException ex4)
            {
              ex1 = new CaesarException(ex4);
            }
          }
        }
        else
          ex1 = new CaesarException(this.channel.ChannelHandle);
      }
      else
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "DSIOHandle was NULL during Instrument::InternalRead");
    }
    else
    {
      try
      {
        Interlocked.Increment(ref this.waitingForUpdate);
        int? slotType = this.SlotType;
        int num = -1;
        if ((slotType.GetValueOrDefault() == num ? (!slotType.HasValue ? 1 : 0) : 1) != 0)
        {
          byte[] source = this.channel.Ecu.RollCallManager.ReadInstrument(this.channel, this.requestMessage.Data.ToArray<byte>(), this.MessageNumber.Value, (Predicate<Tuple<byte?, byte[]>>) (data => this.IsTarget(data.Item1, (IEnumerable<byte>) data.Item2)), (int) this.cacheTime);
          if (source != null)
            flag = this.GetPresentation((object) source, explicitread);
          else
            ex1 = new CaesarException(SapiError.BytePosGreaterThanMessageLength);
        }
        else
          this.channel.Ecu.RollCallManager.DoByteMessage(this.channel, this.RequestMessage.Data.ToArray<byte>(), (byte[]) null);
      }
      catch (CaesarException ex5)
      {
        this.InstrumentValues.Age();
        ex1 = ex5;
      }
      finally
      {
        Interlocked.Decrement(ref this.waitingForUpdate);
      }
    }
    if (ex1 != null)
    {
      this.RaiseInstrumentUpdateEvent((Exception) ex1, explicitread);
    }
    else
    {
      if (!(flag | explicitread) && !this.channel.Instruments.UpdateOnEveryRead)
        return;
      this.RaiseInstrumentUpdateEvent((Exception) null, explicitread);
    }
  }

  internal void ClearDiagServiceIO()
  {
    if (this.diagServiceIO != null)
    {
      ((CaesarHandle\u003CCaesar\u003A\u003ADiagServiceIO__2as\u0020\u002A\u003E) this.diagServiceIO).Dispose();
      this.diagServiceIO = (CaesarDiagServiceIO) null;
    }
    if (this.mcdDiagComPrimitive == null)
      return;
    this.mcdDiagComPrimitive.Dispose();
    this.mcdDiagComPrimitive = (McdDiagComPrimitive) null;
  }

  internal void RaiseInstrumentUpdateEvent(Exception ex, bool explicitread)
  {
    FireAndForget.Invoke((MulticastDelegate) this.InstrumentUpdateEvent, (object) this, (EventArgs) new ResultEventArgs(ex));
    this.channel.Instruments.RaiseInstrumentUpdateEvent(this, ex);
    if (!explicitread)
      return;
    this.channel.SyncDone(ex);
  }

  internal bool GetPresentation(object source, bool explicitread)
  {
    object obj = (object) null;
    switch (source)
    {
      case byte[] data:
        obj = this.GetPresentation(data);
        break;
      case CaesarDiagServiceIO diagServiceIO1:
        obj = this.GetPresentation(diagServiceIO1);
        break;
      case McdDiagComPrimitive diagServiceIO2:
        obj = this.GetPresentation(diagServiceIO2);
        break;
    }
    this.heldValue = obj != null ? obj : throw new CaesarException(SapiError.UnknownPresentationType);
    return this.instrumentValues.AddOrUpdate(this.manipulatedValue != null ? this.manipulatedValue : obj, explicitread);
  }

  internal CaesarAPtr IndexTag
  {
    get => this.indexTag;
    set => this.indexTag = value;
  }

  internal void SetQualifier(string replaceQualifier) => this.userQualifier = replaceQualifier;

  internal bool IsMatching(string qualifier)
  {
    return this.userQualifier.CompareNoCase(qualifier) || this.qualifier.CompareNoCase(qualifier);
  }

  internal void SetAccessLevel(int accessLevel) => this.accessLevel = accessLevel;

  internal static void LoadFromLog(
    XElement element,
    LogFileFormatTagCollection format,
    Channel channel,
    List<string> missingQualifierList,
    object missingInfoLock)
  {
    string qualifier = element.Attribute(format[TagName.Qualifier]).Value;
    Instrument instrument = channel.Instruments[qualifier];
    if (instrument != (Instrument) null)
    {
      if (instrument.DataInterfaceType == (Type) null)
      {
        XAttribute xattribute = element.Attribute(format[TagName.Type]);
        if (xattribute != null)
          instrument.SetType(Type.GetType(xattribute.Value));
      }
      XAttribute xattribute1 = element.Attribute(format[TagName.Precision]);
      if (xattribute1 != null)
        instrument.SetPrecision(Convert.ToUInt16(xattribute1.Value, (IFormatProvider) CultureInfo.InvariantCulture));
      DateTime dateTime = DateTime.MinValue;
      if (instrument.instrumentValues.Count > 0)
        dateTime = instrument.instrumentValues[instrument.instrumentValues.Count - 1].LastSampleTime;
      foreach (XElement element1 in element.Elements(format[TagName.Value]))
      {
        InstrumentValue instrumentValue = InstrumentValue.FromXElement(element1, format, instrument);
        if (instrumentValue.FirstSampleTime > dateTime)
          instrument.InstrumentValues.Add(instrumentValue, false);
      }
    }
    else
    {
      if (channel.Ecu.IgnoreQualifier(qualifier))
        return;
      lock (missingInfoLock)
        missingQualifierList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) channel.Ecu.Name, (object) qualifier));
    }
  }

  internal void WriteXmlTo(bool all, DateTime startTime, DateTime endTime, XmlWriter writer)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    writer.WriteStartElement(currentFormat[TagName.Instrument].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, this.Qualifier);
    if (this.DataInterfaceType == (Type) null && this.Type != (Type) null)
      writer.WriteAttributeString(currentFormat[TagName.Type].LocalName, this.Type.ToString());
    if (this.Precision != null)
      writer.WriteAttributeString(currentFormat[TagName.Precision].LocalName, this.Precision.ToString());
    foreach (InstrumentValue instrumentValue in this.instrumentValues)
    {
      if (instrumentValue.Value != null)
      {
        instrumentValue.WriteXmlTo(startTime, endTime, writer);
        if (!all)
          break;
      }
    }
    writer.WriteEndElement();
  }

  public void Read(bool synchronous) => this.channel.QueueAction((object) this, synchronous);

  public override string ToString() => this.qualifier;

  public Channel Channel => this.channel;

  public new string Name
  {
    get
    {
      if (this.channel.Ecu.RollCallManager != null)
      {
        if (this.Qualifier.StartsWith("DT_", StringComparison.Ordinal))
        {
          string name = this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.Qualifier.Substring(3), "SPN"), (string) null);
          if (name == null)
            name = this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.Qualifier, "SPN"), this.name);
          return name;
        }
        return this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.Qualifier, "SPN"), this.name);
      }
      return this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.Qualifier, nameof (Name)), this.name);
    }
  }

  public string ShortName => this.channel.Ecu.ShortName(this.Name);

  internal bool IsOriginalShortNameSame(Instrument match)
  {
    return string.Equals(this.channel.Ecu.ShortName(this.name), this.channel.Ecu.ShortName(match.name), StringComparison.OrdinalIgnoreCase);
  }

  public new string Description
  {
    get
    {
      return this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.Qualifier, nameof (Description)), this.description);
    }
  }

  public string Qualifier => this.userQualifier != null ? this.userQualifier : this.qualifier;

  public string ReferenceQualifier => this.userQualifier != null ? this.qualifier : (string) null;

  public string GroupName => this.groupName;

  public string GroupQualifier => this.groupQualifier;

  public bool Visible
  {
    get
    {
      if (this.Channel.LogFile != null && this.InstrumentValues.Count == 0 || this.inputValues != null && this.hideServiceAsInstrument)
        return false;
      if (this.Channel.IsRollCall)
      {
        int? slotType = this.SlotType;
        int num = -1;
        if ((slotType.GetValueOrDefault() == num ? (slotType.HasValue ? 1 : 0) : 0) != 0)
          return false;
      }
      return Sapi.GetSapi().ReadAccess >= this.accessLevel;
    }
  }

  public InstrumentValueCollection InstrumentValues => this.instrumentValues;

  public bool Marked
  {
    get => this.marked;
    set
    {
      if (this.marked == value)
        return;
      this.marked = value;
      if (this.marked)
        return;
      this.instrumentValues.Age();
    }
  }

  public bool Periodic => this.periodic;

  public int AccessLevel => this.accessLevel;

  public int CacheTimeout
  {
    get => (int) this.cacheTime;
    set
    {
      if (this.minimumCacheTime.HasValue)
        this.cacheTime = (ushort) Math.Max(value, this.minimumCacheTime.Value);
      else
        this.cacheTime = (ushort) value;
    }
  }

  public int? MinimumCacheTimeout => this.minimumCacheTime;

  public bool ForceRequest => this.forceRequest;

  public bool Summary
  {
    get
    {
      if (this.channel.Ecu.SummaryQualifier(this.qualifier) || this.summary)
        return true;
      int result;
      return this.channel.IsRollCall && this.channel.DataStreamSpns != null && int.TryParse(this.qualifier.Substring(3), out result) && this.channel.DataStreamSpns.Contains<int>(result);
    }
  }

  public Dump RequestMessage => this.requestMessage;

  public string McdQualifier => this.mcdQualifier;

  public Dump ProprietaryContent => this.proprietaryContent;

  public byte? DestinationAddress { get; private set; }

  internal bool IsTarget(byte? destinationAddress, IEnumerable<byte> data)
  {
    int? nullable1 = this.SlotType;
    int num = -1;
    if ((nullable1.GetValueOrDefault() == num ? (nullable1.HasValue ? 1 : 0) : 0) != 0)
      return false;
    if (this.DestinationAddress.HasValue)
    {
      if (destinationAddress.HasValue)
      {
        byte? nullable2 = destinationAddress;
        nullable1 = nullable2.HasValue ? new int?((int) nullable2.GetValueOrDefault()) : new int?();
        nullable2 = this.DestinationAddress;
        int? nullable3 = nullable2.HasValue ? new int?((int) nullable2.GetValueOrDefault()) : new int?();
        if ((nullable1.GetValueOrDefault() == nullable3.GetValueOrDefault() ? (nullable1.HasValue != nullable3.HasValue ? 1 : 0) : 1) == 0)
          goto label_6;
      }
      return false;
    }
label_6:
    if (this.ProprietaryContent == null)
      return true;
    IEnumerable<byte> data1 = (IEnumerable<byte>) this.ProprietaryContent.Data;
    return data1.SequenceEqual<byte>(data.Take<byte>(data1.Count<byte>()));
  }

  public override object ManipulatedValue
  {
    get => this.manipulatedValue;
    set
    {
      if (this.Channel.LogFile != null)
        throw new InvalidOperationException("Cannot manipulate a value for a log file channel");
      if (value == this.manipulatedValue)
        return;
      this.manipulatedValue = value;
      this.instrumentValues.AddOrUpdate(this.manipulatedValue ?? this.heldValue, true);
      this.RaiseInstrumentUpdateEvent((Exception) null, false);
      this.Channel.SetManipulatedState(this.Qualifier, value != null);
    }
  }

  public event InstrumentUpdateEventHandler InstrumentUpdateEvent;

  public int CompareTo(object obj)
  {
    return string.Compare(this.Qualifier, (obj as Instrument).Qualifier, StringComparison.Ordinal);
  }

  public override bool Equals(object obj) => base.Equals(obj);

  public override int GetHashCode() => base.GetHashCode();

  public static bool operator ==(Instrument object1, Instrument object2)
  {
    return object.Equals((object) object1, (object) object2);
  }

  public static bool operator !=(Instrument object1, Instrument object2)
  {
    return !object.Equals((object) object1, (object) object2);
  }

  public static bool operator <(Instrument r1, Instrument r2)
  {
    if (r1 == (Instrument) null)
      throw new ArgumentNullException(nameof (r1));
    return r1.CompareTo((object) r2) < 0;
  }

  public static bool operator >(Instrument r1, Instrument r2)
  {
    if (r1 == (Instrument) null)
      throw new ArgumentNullException(nameof (r1));
    return r1.CompareTo((object) r2) > 0;
  }

  internal new void AddStringsForTranslation(Dictionary<string, string> table)
  {
    table[Sapi.MakeTranslationIdentifier(this.Qualifier, "Name")] = this.name;
    if (!string.IsNullOrEmpty(this.description))
      table[Sapi.MakeTranslationIdentifier(this.Qualifier, "Description")] = this.description;
    base.AddStringsForTranslation(table);
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private void Dispose(bool disposing)
  {
    if (!this.disposed && disposing)
    {
      if (this.indexTag != null)
      {
        ((CaesarHandle\u003Cvoid\u0020\u002A\u003E) this.indexTag).Dispose();
        this.indexTag = (CaesarAPtr) null;
      }
      this.ClearDiagServiceIO();
    }
    this.disposed = true;
  }

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("CacheTime is deprecated due to non-CLS compliance, please use CacheTimeout instead.")]
  public ushort CacheTime
  {
    get => (ushort) this.CacheTimeout;
    set => this.CacheTimeout = (int) value;
  }

  public IEnumerable<string> EnabledAdditionalAudiences => this.enabledAdditionalAudiences;
}
