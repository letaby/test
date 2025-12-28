// Decompiled with JetBrains decompiler
// Type: SapiLayer1.RollCall
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
namespace SapiLayer1;

internal abstract class RollCall : IRollCall, IDisposable
{
  internal const string FixedVariantName = "ROLLCALL";
  protected List<Ecu> ecus;
  protected object ecusLock = new object();
  protected bool disposed;
  protected Protocol protocolId;
  protected ManualResetEvent closingEvent;
  protected ushort debugLevel;
  private Thread backgroundThread;
  private static Dictionary<Protocol, RollCall> managers = new Dictionary<Protocol, RollCall>();
  protected Dictionary<int, RollCall.ChannelInformation> addressInformation = new Dictionary<int, RollCall.ChannelInformation>();
  private volatile ConnectionState state;
  private float? load;
  private object loadLock = new object();
  private volatile bool connectEnabled;

  protected RollCall(Protocol ProtocolId)
  {
    this.protocolId = ProtocolId;
    RollCall.managers[ProtocolId] = this;
    this.debugLevel = Convert.ToUInt16(Sapi.GetSapi().ConfigurationItems["RollCallDebugLevel"].Value, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  internal abstract void Init();

  public bool IsAutoBaudRate { get; protected set; }

  public string DeviceName { get; protected set; }

  public string DeviceLibraryVersion { get; protected set; }

  public string DeviceDriverVersion { get; protected set; }

  public string DeviceFirmwareVersion { get; protected set; }

  public string DeviceLibraryName { get; protected set; }

  public string DeviceSupportedProtocols { get; protected set; }

  public virtual IEnumerable<byte> PowertrainAddresses => (IEnumerable<byte>) new byte[0];

  public virtual void SetRestrictedAddressList(IEnumerable<byte> restrictedSourceAddresses)
  {
    throw new NotImplementedException();
  }

  internal abstract void Exit();

  public void Start()
  {
    if (this.debugLevel > (ushort) 0)
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Roll-call start");
    if (this.State != ConnectionState.NotInitialized)
    {
      if (this.backgroundThread != null)
        return;
      if (this.closingEvent == null)
        this.closingEvent = new ManualResetEvent(false);
      else
        this.closingEvent.Reset();
      this.backgroundThread = new Thread(new ThreadStart(this.ThreadFunc));
      this.backgroundThread.Name = $"{this.GetType().Name}: {this.Protocol.ToString()}";
      this.backgroundThread.Start();
      this.State = ConnectionState.WaitingForTranslator;
    }
    else
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Could not start roll-call as the manager is not initialized.");
  }

  public void Stop()
  {
    if (this.debugLevel > (ushort) 0)
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Roll-call stop");
    if (this.backgroundThread == null)
      return;
    this.closingEvent.Set();
    this.backgroundThread.Join();
    this.backgroundThread = (Thread) null;
    this.State = ConnectionState.Initialized;
  }

  public bool ConnectEnabled
  {
    get => this.connectEnabled;
    set
    {
      if (this.connectEnabled == value)
        return;
      this.connectEnabled = value;
    }
  }

  public bool Running => this.backgroundThread != null;

  public event EventHandler<StateChangedEventArgs> StateChangedEvent;

  public event EventHandler<EventArgs> LoadChangedEvent;

  public ConnectionState State
  {
    get => this.state;
    set
    {
      if (value == this.state)
        return;
      this.state = value;
      FireAndForget.Invoke((MulticastDelegate) this.StateChangedEvent, (object) this, (EventArgs) new StateChangedEventArgs(value));
    }
  }

  public float? Load
  {
    get
    {
      lock (this.loadLock)
        return this.load;
    }
    set
    {
      lock (this.loadLock)
      {
        float? nullable = value;
        float? load = this.load;
        if (((double) nullable.GetValueOrDefault() == (double) load.GetValueOrDefault() ? (nullable.HasValue != load.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.load = value;
        FireAndForget.Invoke((MulticastDelegate) this.LoadChangedEvent, (object) this, new EventArgs());
      }
    }
  }

  internal bool ProtocolAlive
  {
    get
    {
      if (this.Running)
      {
        switch (this.State)
        {
          case ConnectionState.TranslatorConnected:
          case ConnectionState.ChannelsConnecting:
          case ConnectionState.ChannelsConnected:
            return true;
        }
      }
      return false;
    }
  }

  public Protocol Protocol => this.protocolId;

  internal IEnumerable<Ecu> Ecus
  {
    get
    {
      lock (this.ecusLock)
      {
        if (this.ecus == null)
          this.ecus = this.LoadMonitorEcus().ToList<Ecu>();
        return (IEnumerable<Ecu>) new List<Ecu>((IEnumerable<Ecu>) this.ecus);
      }
    }
  }

  protected virtual IEnumerable<Ecu> LoadMonitorEcus() => (IEnumerable<Ecu>) new List<Ecu>();

  protected void ClearEcus()
  {
    lock (this.ecusLock)
      this.ecus = (List<Ecu>) null;
  }

  internal Ecu GetEcu(int sourceAddress, int? function)
  {
    lock (this.ecusLock)
    {
      Ecu ecu = this.ecus.Where<Ecu>((Func<Ecu, bool>) (e =>
      {
        if (e.IsRollCallBaseEcu)
        {
          int? nullable1 = e.SourceAddressLong;
          int num = sourceAddress;
          if ((nullable1.GetValueOrDefault() == num ? (nullable1.HasValue ? 1 : 0) : 0) != 0)
          {
            nullable1 = e.Function;
            int? nullable2 = function;
            return nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() && nullable1.HasValue == nullable2.HasValue;
          }
        }
        return false;
      })).FirstOrDefault<Ecu>();
      if (ecu == null)
      {
        ecu = new Ecu(sourceAddress, function, this);
        ecu.AcquireFromRollCall(string.Empty, (IEnumerable<DiagnosisVariant>) null);
        this.ecus.Add(ecu);
      }
      return ecu;
    }
  }

  internal virtual void NotifyEcuInfoValue(EcuInfo ecuInfo, object ecuInfoValue)
  {
  }

  internal void RemoveChannel(Channel channel)
  {
    lock (this.addressInformation)
    {
      Dictionary<int, RollCall.ChannelInformation> addressInformation1 = this.addressInformation;
      int? sourceAddressLong = channel.SourceAddressLong;
      int key1 = sourceAddressLong.Value;
      RollCall.ChannelInformation channelInformation;
      ref RollCall.ChannelInformation local = ref channelInformation;
      if (!addressInformation1.TryGetValue(key1, out local) || channelInformation.Channel != channel)
        return;
      Dictionary<int, RollCall.ChannelInformation> addressInformation2 = this.addressInformation;
      sourceAddressLong = channel.SourceAddressLong;
      int key2 = sourceAddressLong.Value;
      addressInformation2.Remove(key2);
    }
  }

  internal virtual bool IsVirtual(int sourceAddress) => false;

  protected abstract TimeSpan RollCallValidLastSeenTime { get; }

  internal IEnumerable<int> GetActiveAddresses()
  {
    lock (this.addressInformation)
      return (IEnumerable<int>) this.addressInformation.Where<KeyValuePair<int, RollCall.ChannelInformation>>((Func<KeyValuePair<int, RollCall.ChannelInformation>, bool>) (ai => ai.Value.TimeSinceLastSeen < this.RollCallValidLastSeenTime)).Select<KeyValuePair<int, RollCall.ChannelInformation>, int>((Func<KeyValuePair<int, RollCall.ChannelInformation>, int>) (ai => ai.Key)).ToList<int>();
  }

  internal object GetIdentificationValue(int sourceAddress, string requestedId)
  {
    if (this.ProtocolAlive)
    {
      lock (this.addressInformation)
      {
        if (this.addressInformation.ContainsKey(sourceAddress))
        {
          RollCall.ChannelInformation channelInformation = this.addressInformation[sourceAddress];
          if (channelInformation != null)
          {
            RollCall.IdentificationInformation identificationInformation = channelInformation.Identification.FirstOrDefault<RollCall.IdentificationInformation>((Func<RollCall.IdentificationInformation, bool>) (id => id.IdString == requestedId || id.Id.ToNumberString() == requestedId));
            if (identificationInformation != null)
              return identificationInformation.Value;
          }
        }
      }
    }
    return (object) null;
  }

  internal bool IsChannelRunning(Channel channel)
  {
    bool flag = false;
    if (this.ProtocolAlive)
    {
      if (this.IsVirtual((int) channel.SourceAddress.Value))
      {
        lock (this.addressInformation)
          flag = this.addressInformation.Any<KeyValuePair<int, RollCall.ChannelInformation>>((Func<KeyValuePair<int, RollCall.ChannelInformation>, bool>) (ai => ai.Key != (int) channel.SourceAddress.Value));
      }
      else
      {
        lock (this.addressInformation)
        {
          RollCall.ChannelInformation channelInformation;
          if (this.addressInformation.TryGetValue(channel.SourceAddressLong.Value, out channelInformation))
          {
            if (channelInformation.Channel == channel)
              flag = channelInformation.IsRunning;
          }
        }
      }
    }
    if (this.debugLevel > (ushort) 1)
      this.RaiseDebugInfoEvent((int) channel.SourceAddress.Value, "IsChannelRunning() = " + flag.ToString());
    return flag;
  }

  protected abstract void ThreadFunc();

  protected void ClearAddressInformation()
  {
    IEnumerable<RollCall.ChannelInformation> list;
    lock (this.addressInformation)
    {
      list = (IEnumerable<RollCall.ChannelInformation>) this.addressInformation.Select<KeyValuePair<int, RollCall.ChannelInformation>, RollCall.ChannelInformation>((Func<KeyValuePair<int, RollCall.ChannelInformation>, RollCall.ChannelInformation>) (ai => ai.Value)).ToList<RollCall.ChannelInformation>();
      this.addressInformation.Clear();
    }
    foreach (RollCall.ChannelInformation channelInformation in list)
      channelInformation.Dispose();
  }

  internal abstract EcuInfo CreateEcuInfo(EcuInfoCollection ecuInfos, string qualifier);

  internal abstract void CreateEcuInfos(EcuInfoCollection ecuInfos);

  internal virtual IEnumerable<Instrument> CreateInstruments(Channel channel)
  {
    return (IEnumerable<Instrument>) new List<Instrument>();
  }

  internal virtual Instrument CreateBaseInstrument(Channel channel, string qualifier)
  {
    throw new NotImplementedException();
  }

  internal virtual IEnumerable<Service> CreateServices(Channel channel, ServiceTypes type)
  {
    return (IEnumerable<Service>) new List<Service>();
  }

  internal virtual Dictionary<string, TranslationEntry> Translations
  {
    get => new Dictionary<string, TranslationEntry>();
  }

  public virtual IDictionary<string, string> SuspectParameters
  {
    get => (IDictionary<string, string>) null;
  }

  public virtual IDictionary<int, string> ParameterGroupLabels => (IDictionary<int, string>) null;

  public virtual IDictionary<int, string> ParameterGroupAcronyms => (IDictionary<int, string>) null;

  public virtual void WriteTranslationFile(
    CultureInfo culture,
    IEnumerable<TranslationEntry> translations)
  {
  }

  internal virtual Dictionary<string, string> GetSuspectParametersForEcu(Ecu ecu)
  {
    return (Dictionary<string, string>) null;
  }

  internal virtual string GetFaultText(Channel channel, string number, string mode)
  {
    throw new NotImplementedException();
  }

  internal virtual void ClearErrors(Channel channel)
  {
  }

  internal virtual byte[] DoByteMessage(Channel channel, byte[] data, byte[] requiredResponse)
  {
    throw new NotImplementedException();
  }

  internal virtual byte[] ReadInstrument(
    Channel channel,
    byte[] data,
    int responseId,
    Predicate<Tuple<byte?, byte[]>> additionalResponseCheck,
    int responseTimeout)
  {
    throw new NotImplementedException();
  }

  internal virtual bool IsSnapshotSupported(Channel channel) => false;

  internal virtual void ReadEcuInfo(EcuInfo ecuInfo)
  {
  }

  internal virtual void ReadSnapshot(Channel channel)
  {
  }

  internal virtual void ReadFaultCodes(Channel channel)
  {
  }

  internal abstract int ChannelTimeout { get; }

  public static RollCall GetManager(Protocol protocolId)
  {
    return RollCall.managers.ContainsKey(protocolId) ? RollCall.managers[protocolId] : (RollCall) null;
  }

  protected virtual void RaiseDebugInfoEvent(int sourceAddress, string text)
  {
    string str = sourceAddress.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    Sapi.GetSapi().RaiseDebugInfoEvent((object) $"{(object) this.protocolId}-{str}", text);
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
      if (this.closingEvent != null)
      {
        this.closingEvent.Set();
        if (this.backgroundThread != null)
        {
          this.backgroundThread.Join();
          this.backgroundThread = (Thread) null;
        }
      }
      this.ClearAddressInformation();
      this.DisposeInternal();
      if (this.closingEvent != null)
      {
        this.closingEvent.Close();
        this.closingEvent = (ManualResetEvent) null;
      }
    }
    this.disposed = true;
    RollCall.managers[this.protocolId] = (RollCall) null;
  }

  protected virtual void DisposeInternal()
  {
  }

  protected abstract class ChannelInformation : IDisposable
  {
    protected ConnectionResource resource;
    private bool disposed;
    private int[] dataStreamSpns;
    private DateTime firstSeen;
    private DateTime lastSeen;
    internal readonly List<RollCall.IdentificationInformation> Identification;
    protected RollCall rollCallManager;
    private Thread backgroundThread;

    internal ChannelInformation(
      RollCall rollCallManager,
      ConnectionResource resource,
      IEnumerable<RollCall.IdentificationInformation> identification)
    {
      this.Identification = identification.ToList<RollCall.IdentificationInformation>();
      this.rollCallManager = rollCallManager;
      this.Resource = resource;
      this.firstSeen = DateTime.UtcNow;
    }

    internal ConnectionResource Resource
    {
      get => this.resource;
      set
      {
        if (this.resource != null || value == null)
          return;
        this.resource = value;
        this.backgroundThread = new Thread(new ThreadStart(this.ThreadFunc));
        this.backgroundThread.Name = $"{this.GetType().Name}: {this.resource.Ecu.Identifier}";
        this.backgroundThread.Start();
      }
    }

    protected abstract void ThreadFunc();

    protected bool Connect(DiagnosisVariant variant)
    {
      if (this.rollCallManager.ProtocolAlive)
      {
        this.Channel = Sapi.GetSapi().Channels.AddFromRollCall(this.Resource, variant);
        this.UpdateLastSeen();
        if (this.dataStreamSpns != null)
          this.Channel?.SetDataStreamSpns(this.dataStreamSpns);
      }
      if (this.Channel == null)
        this.Invalid = true;
      return !this.Invalid;
    }

    internal void CreateEcuInfos(EcuInfoCollection ecuInfos)
    {
      foreach (RollCall.IdentificationInformation identificationInformation in this.Identification.Where<RollCall.IdentificationInformation>((Func<RollCall.IdentificationInformation, bool>) (i => i.Value != null)))
      {
        EcuInfo ecuInfo = this.rollCallManager.CreateEcuInfo(ecuInfos, identificationInformation.IdString ?? identificationInformation.Id.ToNumberString());
        if (ecuInfo != null)
          identificationInformation.SetEcuInfo(ecuInfo);
      }
    }

    internal bool IsRunning
    {
      get
      {
        return this.lastSeen + TimeSpan.FromMilliseconds((double) this.rollCallManager.ChannelTimeout) > DateTime.UtcNow;
      }
    }

    internal TimeSpan TimeSinceLastSeen => DateTime.UtcNow - this.lastSeen;

    internal TimeSpan TimeSinceFirstSeen => DateTime.UtcNow - this.firstSeen;

    internal Channel Channel { get; private set; }

    internal bool Invalid { get; private set; }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!this.disposed && disposing && this.backgroundThread != null)
      {
        this.backgroundThread.Join();
        this.backgroundThread = (Thread) null;
      }
      this.disposed = true;
    }

    internal void UpdateLastSeen() => this.lastSeen = DateTime.UtcNow;

    internal void SetDataStreamSpns(int[] dataStreamSpns)
    {
      this.dataStreamSpns = dataStreamSpns;
      this.Channel?.SetDataStreamSpns(dataStreamSpns);
    }
  }

  internal enum ID : uint
  {
    UnitNumber = 233, // 0x000000E9
    SoftwareIdentification = 234, // 0x000000EA
    VehicleIdentificationNumber = 237, // 0x000000ED
    Make = 586, // 0x0000024A
    Model = 587, // 0x0000024B
    SerialNumber = 588, // 0x0000024C
    SuspectParameterNumber = 1214, // 0x000004BE
    FailureModeIdentifier = 1215, // 0x000004BF
    OccurrenceCount = 1216, // 0x000004C0
    OnBoardDiagnosticCompliance = 1220, // 0x000004C4
    TestIdentifier = 1224, // 0x000004C8
    CalibrationVerificationNumber = 1634, // 0x00000662
    CalibrationInformation = 1635, // 0x00000663
    ManufacturerCode = 2838, // 0x00000B16
    Function = 2841, // 0x00000B19
    EcuPartNumber = 2901, // 0x00000B55
    EcuSerialNumber = 2902, // 0x00000B56
    EcuLocation = 2903, // 0x00000B57
    EcuType = 2904, // 0x00000B58
    SPNOfApplicableSystemMonitor = 3066, // 0x00000BFA
    ApplicableSystemMonitorNumerator = 3067, // 0x00000BFB
    ApplicableSystemMonitorDenominator = 3068, // 0x00000BFC
    SPNSupported = 3146, // 0x00000C4A
    SupportedInExpandedFreezeFrame = 4100, // 0x00001004
    SupportedInDataStream = 4101, // 0x00001005
    SupportedInScaledTestResults = 4102, // 0x00001006
    SPNDataLength = 4103, // 0x00001007
    SlotIdentifier = 4109, // 0x0000100D
    TestValue = 4110, // 0x0000100E
    TestLimitMaximum = 4111, // 0x0000100F
    TestLimitMinimum = 4112, // 0x00001010
    AECDNumber = 4124, // 0x0000101C
    AECDEngineHoursTimer1 = 4125, // 0x0000101D
    AECDEngineHoursTimer2 = 4126, // 0x0000101E
    EcuManufacturerName = 4304, // 0x000010D0
    UnitSystem = 5197, // 0x0000144D
  }

  internal class IdentificationInformation
  {
    private object value;
    private EcuInfo ecuInfo;
    internal readonly RollCall.ID Id;
    internal readonly string IdString;

    internal IdentificationInformation(RollCall.ID id) => this.Id = id;

    internal IdentificationInformation(string id, string value)
    {
      this.IdString = id;
      this.value = (object) value;
    }

    internal object Value
    {
      set
      {
        if (this.value == value)
          return;
        this.value = value;
        if (this.ecuInfo == null)
          return;
        this.ecuInfo.UpdateFromRollCall(value);
      }
      get => this.value;
    }

    internal void SetEcuInfo(EcuInfo ecuInfo)
    {
      this.ecuInfo = ecuInfo;
      if (this.ecuInfo == null)
        return;
      ecuInfo.UpdateFromRollCall(this.value);
    }
  }
}
