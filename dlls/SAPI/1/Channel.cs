// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Channel
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using J2534;
using McdAbstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class Channel : IDisposable
{
  private ServiceCollection structuredServices;
  private object caesarEquivalentServicesLock = new object();
  private List<Tuple<Service, ServiceOutputValue>> caesarEquivalentServices;
  private Dictionary<Tuple<string, int>, McdCaesarEquivalenceScaleInfo> scaleInfoCache = new Dictionary<Tuple<string, int>, McdCaesarEquivalenceScaleInfo>();
  private object offlineVarcodingHandleLock = new object();
  private object faultCodeStatusChoicesLock = new object();
  private ChoiceCollection completeFaultCodeStatusChoices;
  private ChoiceCollection faultCodeStatusChoices;
  private volatile StopReason stopReason;
  private bool haveShownChangedLongDiagVersionError;
  private long? requestId;
  private long? responseId;
  private RP1210ProtocolId? rp1210Protocol;
  private Dictionary<byte, List<Tuple<Dump, Dump, List<Service>>>> offlineServices;
  private List<string> manipulatedItems = new List<string>();
  private object channelHandleLock = new object();
  private int[] dataStreamSpns;
  private EcuInfoCollection ecuInfos;
  private ParameterCollection parameters;
  private ParameterGroupCollection parameterGroups;
  private InstrumentCollection instruments;
  private FaultCodeCollection faultCodes;
  private ServiceCollection services;
  private FlashAreaCollection flashAreas;
  private object channelExtensionLock = new object();
  private Ecu ecu;
  private DiagnosisVariant variant;
  private ConnectionResource connectionResource;
  private CommunicationsState communicationsState;
  private CaesarChannel caesarChannel;
  private CaesarEcu caesarEcu;
  private Varcode offlineVarcoding;
  private McdLogicalLink mcdChannel;
  private McdDBLocation mcdEcu;
  private static volatile bool flashing;
  private Thread thread;
  private volatile bool closing;
  private Queue actionsQueue = Queue.Synchronized(new Queue());
  private ManualResetEvent actionsQueueNonEmptyEvent = new ManualResetEvent(false);
  private ChannelBaseCollection parent;
  private ManualResetEvent syncDone;
  private Exception syncException;
  private LogFile logFile;
  private SessionCollection sessions;
  private bool disposed;
  private CodingParameterGroupCollection codingParameterGroups;
  private ChannelExtension channelExtension;
  private bool haveAttemptedExtensionLoad;
  private CommunicationsStateValueCollection communicationsStateValues;
  private string fixedVariant;
  private ChannelOptions channelOptions = ChannelOptions.All;

  internal Channel(
    CaesarChannel ch,
    ConnectionResource cr,
    ChannelBaseCollection p,
    ChannelOptions options)
    : this(p, cr.Ecu, cr, CommunicationsState.OnlineButNotInitialized, (LogFile) null)
  {
    this.caesarChannel = ch;
    this.channelOptions = options;
    if (this.channelOptions != ChannelOptions.All)
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "CAESAR channel with non-standard ChannelOptions: " + (object) this.channelOptions);
    this.fixedVariant = cr.Ecu.Properties["FixedDiagnosisVariant"];
    if (!string.IsNullOrEmpty(this.fixedVariant))
    {
      this.caesarChannel.Ecu.SetVariant(this.fixedVariant);
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "CAESAR channel selected fixed variant: " + this.fixedVariant);
    }
    if (this.caesarChannel.Ecu != null)
    {
      this.variant = this.ecu.DiagnosisVariants[this.caesarChannel.Ecu.VariantName];
      if (this.variant == null)
        this.variant = this.ecu.DiagnosisVariants.Base;
      this.caesarEcu = this.variant.OpenEcuHandle();
    }
    else
      this.variant = this.ecu.DiagnosisVariants.Last<DiagnosisVariant>();
    this.sessions.Add(new Session(this, Sapi.Now, DateTime.MinValue, this.ecu.DescriptionDataVersion, cr, this.variant.Name, new bool?(!string.IsNullOrEmpty(this.fixedVariant)), this.channelOptions));
    this.structuredServices = new ServiceCollection(this);
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"Loaded {(object) this.structuredServices.Count} structured services");
    this.ecu.IncrementConnectedChannelCount();
  }

  internal Channel(
    McdLogicalLink logicalLink,
    ConnectionResource connectionResource,
    ChannelBaseCollection parent,
    ChannelOptions options)
    : this(parent, connectionResource.Ecu, connectionResource, CommunicationsState.OnlineButNotInitialized, (LogFile) null)
  {
    this.channelOptions = options;
    if (this.channelOptions != ChannelOptions.All)
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "MCD channel with non-standard ChannelOptions: " + (object) this.channelOptions);
    this.mcdChannel = logicalLink;
    this.connectionResource = connectionResource;
    this.fixedVariant = connectionResource.Ecu.Properties["FixedDiagnosisVariant"];
    if (!string.IsNullOrEmpty(this.fixedVariant))
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "MCD channel selected fixed variant: " + this.fixedVariant);
    string variantName = this.mcdChannel.VariantName;
    this.variant = this.ecu.DiagnosisVariants.FirstOrDefault<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (v => v.Locations != null && v.Locations.Contains<string>(variantName)));
    if (this.variant == null)
      this.variant = !this.ecu.IsByteMessaging ? this.ecu.DiagnosisVariants.Base : this.ecu.DiagnosisVariants.Last<DiagnosisVariant>();
    this.sessions.Add(new Session(this, Sapi.Now, DateTime.MinValue, this.ecu.DescriptionDataVersion, connectionResource, this.variant.Name, new bool?(!string.IsNullOrEmpty(this.fixedVariant)), this.channelOptions));
    this.mcdEcu = this.mcdChannel.DBLocation;
    this.structuredServices = new ServiceCollection(this);
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"Loaded {(object) this.structuredServices.Count} structured services");
    this.ecu.IncrementConnectedChannelCount();
  }

  internal Channel(DiagnosisVariant v, ChannelBaseCollection p, LogFile lf)
    : this(p, v.Ecu, (ConnectionResource) null, lf != null ? CommunicationsState.LogFilePaused : CommunicationsState.Offline, lf)
  {
    this.variant = v;
  }

  internal Channel(
    CaesarEcu ecuh,
    DiagnosisVariant variant,
    ChannelBaseCollection parent,
    LogFile logFile)
    : this(parent, variant.Ecu, (ConnectionResource) null, logFile != null ? CommunicationsState.LogFilePaused : CommunicationsState.Offline, logFile)
  {
    this.caesarEcu = ecuh;
    this.structuredServices = new ServiceCollection(this);
    this.variant = variant;
  }

  internal Channel(
    McdDBLocation variantHandle,
    DiagnosisVariant variant,
    ChannelBaseCollection parent,
    LogFile logFile)
    : this(parent, variant.Ecu, (ConnectionResource) null, logFile != null ? CommunicationsState.LogFilePaused : CommunicationsState.Offline, logFile)
  {
    this.mcdEcu = variantHandle;
    this.structuredServices = new ServiceCollection(this);
    this.variant = variant;
  }

  internal Channel(
    ChannelBaseCollection parent,
    ConnectionResource resource,
    DiagnosisVariant variant)
    : this(parent, resource.Ecu, resource, CommunicationsState.OnlineButNotInitialized, (LogFile) null)
  {
    this.sessions.Add(new Session(this, Sapi.Now, DateTime.MinValue, this.ecu.DescriptionDataVersion, resource, variant.Name, new bool?(), ChannelOptions.All));
    this.variant = variant;
    this.ecu.IncrementConnectedChannelCount();
    switch (this.ecu.RollCallManager.Protocol)
    {
      case Protocol.J1708:
        this.rp1210Protocol = new RP1210ProtocolId?(RP1210ProtocolId.J1708);
        break;
      case Protocol.J1939:
        this.rp1210Protocol = new RP1210ProtocolId?(RP1210ProtocolId.J1939);
        break;
    }
  }

  private Channel(
    ChannelBaseCollection parent,
    Ecu ecu,
    ConnectionResource resource,
    CommunicationsState initialState,
    LogFile logFile)
  {
    this.logFile = logFile;
    this.communicationsState = initialState;
    this.closing = false;
    this.parent = parent;
    this.ecu = ecu;
    this.connectionResource = resource;
    this.communicationsStateValues = new CommunicationsStateValueCollection(this);
    this.sessions = new SessionCollection();
    this.ecuInfos = new EcuInfoCollection(this);
    this.parameters = new ParameterCollection(this);
    this.parameterGroups = new ParameterGroupCollection(this);
    this.services = new ServiceCollection(this, ServiceTypes.Actuator | ServiceTypes.Adjustment | ServiceTypes.Download | ServiceTypes.DiagJob | ServiceTypes.Function | ServiceTypes.Global | ServiceTypes.IOControl | ServiceTypes.Routine | ServiceTypes.Security | ServiceTypes.Session | ServiceTypes.Static, true);
    this.instruments = new InstrumentCollection(this);
    this.faultCodes = new FaultCodeCollection(this);
    this.flashAreas = new FlashAreaCollection(this);
    this.codingParameterGroups = new CodingParameterGroupCollection(this);
  }

  internal void SetCommunicationsState(CommunicationsState cs)
  {
    this.SetCommunicationsState(cs, string.Empty);
  }

  internal void SetCommunicationsState(CommunicationsState cs, string additional)
  {
    if (cs == this.communicationsState)
      return;
    this.communicationsState = cs;
    FireAndForget.Invoke((MulticastDelegate) this.CommunicationsStateUpdateEvent, (object) this, (EventArgs) new CommunicationsStateEventArgs(cs));
    if (this.thread == null)
      return;
    CommunicationsStateValue communicationsStateValue = new CommunicationsStateValue(this, cs, Sapi.Now, additional);
    this.communicationsStateValues.Add(communicationsStateValue, true);
    this.RaiseCommunicationsStateValueUpdateEvent(communicationsStateValue);
  }

  internal CaesarChannel ChannelHandle => this.caesarChannel;

  internal McdLogicalLink McdChannelHandle => this.mcdChannel;

  internal CaesarEcu EcuHandle => this.caesarEcu;

  internal McdDBLocation McdEcuHandle => this.mcdEcu;

  public ServiceCollection StructuredServices => this.structuredServices;

  internal List<Tuple<Service, ServiceOutputValue>> CaesarEquivalentServices
  {
    get
    {
      lock (this.caesarEquivalentServicesLock)
      {
        if (this.caesarEquivalentServices == null)
        {
          this.caesarEquivalentServices = new List<Tuple<Service, ServiceOutputValue>>();
          foreach (Service structuredService in (ReadOnlyCollection<Service>) this.StructuredServices)
          {
            bool flag = false;
            if (structuredService.ServiceTypes != ServiceTypes.DiagJob)
            {
              foreach (ServiceOutputValue outputValue in (ReadOnlyCollection<ServiceOutputValue>) structuredService.OutputValues)
              {
                this.caesarEquivalentServices.Add(Tuple.Create<Service, ServiceOutputValue>(structuredService, outputValue));
                flag = true;
              }
            }
            if (!flag)
              this.caesarEquivalentServices.Add(Tuple.Create<Service, ServiceOutputValue>(structuredService, (ServiceOutputValue) null));
          }
        }
        return this.caesarEquivalentServices;
      }
    }
  }

  internal McdCaesarEquivalenceScaleInfo GetMcdCaesarEquivalenceScaleInfo(
    string qualifier,
    Type type,
    Func<McdDBDataObjectProp> getDataObjectProp)
  {
    lock (this.scaleInfoCache)
    {
      Tuple<string, int> key = Tuple.Create<string, int>(qualifier, type.GetHashCode());
      McdCaesarEquivalenceScaleInfo equivalenceScaleInfo;
      if (!this.scaleInfoCache.TryGetValue(key, out equivalenceScaleInfo))
        this.scaleInfoCache[key] = equivalenceScaleInfo = new McdCaesarEquivalenceScaleInfo(type, getDataObjectProp());
      return equivalenceScaleInfo;
    }
  }

  internal byte? SourceAddress
  {
    get
    {
      int? sourceAddressLong = this.SourceAddressLong;
      return !sourceAddressLong.HasValue ? new byte?() : new byte?((byte) sourceAddressLong.Value);
    }
  }

  internal int? SourceAddressLong
  {
    get
    {
      ConnectionResource connectionResource = this.connectionResource;
      if (connectionResource == null && this.sessions.Any<Session>())
        connectionResource = this.sessions.First<Session>().Resource;
      return connectionResource != null && connectionResource.SourceAddressLong.HasValue ? connectionResource.SourceAddressLong : this.ecu.SourceAddressLong;
    }
  }

  internal Varcode VCInit()
  {
    if (this.caesarChannel != null)
      return (Varcode) new VarcodeCaesar(this);
    return this.mcdChannel != null ? (Varcode) new VarcodeMcd(this, this.mcdChannel) : (Varcode) null;
  }

  internal object OfflineVarcodingHandleLock => this.offlineVarcodingHandleLock;

  internal Varcode OfflineVarcodingHandle
  {
    get
    {
      if (this.offlineVarcoding == null && !this.closing)
      {
        Sapi.GetSapi().EnsureCodingFilesLoaded();
        if (this.caesarEcu != null)
          this.offlineVarcoding = (Varcode) new VarcodeCaesar(this.caesarEcu);
        else if (this.mcdEcu != null && Sapi.GetSapi().InitState == InitState.Online)
        {
          ConnectionResource resource = this.variant.Ecu.GetConnectionResources().FirstOrDefault<ConnectionResource>();
          if (resource != null)
          {
            McdInterface mcdInterface = McdRoot.CurrentInterfaces.FirstOrDefault<McdInterface>((Func<McdInterface, bool>) (i => i.Qualifier == resource.MCDInterfaceQualifier));
            if (mcdInterface != null)
              this.offlineVarcoding = (Varcode) new VarcodeMcd(this, resource.Interface.Qualifier, mcdInterface);
          }
        }
      }
      return this.offlineVarcoding;
    }
  }

  internal bool Closing => this.closing;

  internal bool ActionWaiting => this.actionsQueue.Count > 0;

  internal ChoiceCollection CompleteFaultCodeStatusChoices
  {
    get
    {
      lock (this.faultCodeStatusChoicesLock)
      {
        if (this.completeFaultCodeStatusChoices == null)
        {
          this.completeFaultCodeStatusChoices = new ChoiceCollection();
          bool obd = this.ecu.Properties.GetValue<bool>("SupportsPendingFaults", false);
          if (this.IsRollCall)
            this.BuildFaultCodeStatusChoices(true, obd, FaultCodeStatus.Active | FaultCodeStatus.Pending | FaultCodeStatus.Stored | FaultCodeStatus.TestFailedSinceLastClear | FaultCodeStatus.Mil | FaultCodeStatus.Permanent | FaultCodeStatus.Immediate);
          else
            this.BuildFaultCodeStatusChoices(true, obd, FaultCodeStatus.Active | FaultCodeStatus.Pending | FaultCodeStatus.Stored | FaultCodeStatus.TestFailedSinceLastClear | FaultCodeStatus.Mil | FaultCodeStatus.Permanent);
        }
        return this.completeFaultCodeStatusChoices;
      }
    }
  }

  internal ChoiceCollection FaultCodeStatusChoices
  {
    get
    {
      lock (this.faultCodeStatusChoicesLock)
      {
        if (this.faultCodeStatusChoices == null)
        {
          this.faultCodeStatusChoices = new ChoiceCollection();
          bool obd = this.ecu.Properties.GetValue<bool>("SupportsPendingFaults", false);
          FaultCodeStatus possible = !obd ? (!this.ecu.IsRollCall ? FaultCodeStatus.Active | FaultCodeStatus.Stored : FaultCodeStatus.Active | FaultCodeStatus.TestFailedSinceLastClear) : (!this.IsRollCall ? FaultCodeStatus.Active | FaultCodeStatus.Pending | FaultCodeStatus.Stored | FaultCodeStatus.TestFailedSinceLastClear | FaultCodeStatus.Mil | FaultCodeStatus.Permanent : FaultCodeStatus.Active | FaultCodeStatus.Pending | FaultCodeStatus.Stored | FaultCodeStatus.TestFailedSinceLastClear | FaultCodeStatus.Mil | FaultCodeStatus.Permanent | FaultCodeStatus.Immediate);
          this.BuildFaultCodeStatusChoices(false, obd, possible);
        }
        return this.faultCodeStatusChoices;
      }
    }
  }

  private void BuildFaultCodeStatusChoices(bool complete, bool obd, FaultCodeStatus possible)
  {
    for (int index = 0; (FaultCodeStatus) index <= possible; ++index)
    {
      FaultCodeStatus faultCodeStatus = possible & (FaultCodeStatus) index;
      if (faultCodeStatus == (FaultCodeStatus) index || index == 0)
      {
        if (complete)
          this.completeFaultCodeStatusChoices.Add(new Choice(faultCodeStatus.ToStatusString(obd, this.ecu), (object) faultCodeStatus));
        else
          this.faultCodeStatusChoices.Add(this.CompleteFaultCodeStatusChoices.GetItemFromRawValue((object) faultCodeStatus));
      }
    }
  }

  internal void ResetFaultCodeStatusChoices()
  {
    lock (this.faultCodeStatusChoicesLock)
    {
      this.faultCodeStatusChoices = (ChoiceCollection) null;
      this.completeFaultCodeStatusChoices = (ChoiceCollection) null;
    }
  }

  public void Stop() => this.stopReason = StopReason.Closing;

  public void Abort() => this.Abort(StopReason.Closing);

  internal void Abort(StopReason reason)
  {
    lock (this.channelHandleLock)
    {
      if (this.caesarChannel != null)
        this.caesarChannel.SetTimeout(1U);
      else if (this.mcdChannel != null)
        this.mcdChannel.Abort();
    }
    this.stopReason = reason;
    switch (this.stopReason)
    {
      case StopReason.TranslatorDisconnected:
        this.DisconnectionException = new CaesarException(SapiError.TranslatorDisconnected);
        break;
      case StopReason.NoTraffic:
        this.DisconnectionException = new CaesarException(SapiError.NoTraffic);
        break;
    }
  }

  internal bool ChannelRunning
  {
    get
    {
      if (this.stopReason == StopReason.None)
      {
        if (this.ChannelHandle != null)
          return (this.channelOptions & ChannelOptions.StartStopCommunications) == ChannelOptions.None || this.ChannelHandle.ChannelRunning;
        if (this.ecu.RollCallManager != null)
          return this.ecu.RollCallManager.IsChannelRunning(this);
        if (this.mcdChannel != null)
          return (this.channelOptions & ChannelOptions.StartStopCommunications) == ChannelOptions.None || this.mcdChannel.ChannelRunning;
      }
      return false;
    }
  }

  internal void QueueAction(object o, bool synchronous)
  {
    if (this.Online)
    {
      if (this.thread != null)
      {
        lock (this.actionsQueue)
        {
          if (synchronous)
            this.syncDone = new ManualResetEvent(false);
          this.actionsQueue.Enqueue(o);
          this.actionsQueueNonEmptyEvent.Set();
        }
        if (!synchronous)
          return;
        int num = this.syncDone.WaitOne() ? 1 : 0;
        this.syncDone.Close();
        this.syncDone = (ManualResetEvent) null;
        if (num == 0)
          throw new ThreadStateException("Wait for synchronous call expired");
        if (this.syncException != null)
        {
          Exception syncException = this.syncException;
          this.syncException = (Exception) null;
          throw syncException;
        }
      }
      else
      {
        if (this.CommunicationsState != CommunicationsState.OnlineButNotInitialized)
          throw new InvalidOperationException("Cannot queue an action because the channel thread isn't running.");
        if (synchronous)
          throw new InvalidOperationException("You must call Channel.Init before queueing an action.");
        this.actionsQueue.Enqueue(o);
        this.actionsQueueNonEmptyEvent.Set();
      }
    }
    else
    {
      if (this.thread == null)
        throw new InvalidOperationException("Cannot queue an action in offline mode");
      if (synchronous)
        throw new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "An attempt to queue an asynchronous action during disconnection was ignored.");
    }
  }

  internal void QueueAction(
    object action,
    bool synchronous,
    SynchronousCheckFailureHandler failureDelegate)
  {
    if (this.SynchronousCheck(action))
    {
      this.QueueAction(action, synchronous);
    }
    else
    {
      CaesarException e = new CaesarException(SapiError.NegativeResponse);
      failureDelegate(action, e);
      if (synchronous)
        throw e;
    }
  }

  internal void SyncDone(Exception e)
  {
    lock (this.actionsQueue)
    {
      if (this.syncDone == null || this.actionsQueue.Count != 0)
        return;
      this.syncException = e;
      this.syncDone.Set();
    }
  }

  internal void InternalDisconnect()
  {
    if (this.closing)
      return;
    this.closing = true;
    if (this.thread != null)
      this.thread.Join();
    if (this.caesarChannel != null || this.mcdChannel != null)
      this.CloseChannel();
    else if (this.ecu.RollCallManager != null && this.connectionResource != null)
    {
      this.ecu.RollCallManager.RemoveChannel(this);
      this.SetCommunicationsState(CommunicationsState.Offline);
    }
    if (this.offlineVarcoding != null)
    {
      this.offlineVarcoding.Dispose();
      this.offlineVarcoding = (Varcode) null;
    }
    this.mcdEcu = (McdDBLocation) null;
    if (this.caesarEcu != null)
    {
      ((CaesarHandle\u003CCaesar\u003A\u003AECUHandleStructS\u0020\u002A\u003E) this.caesarEcu).Dispose();
      this.caesarEcu = (CaesarEcu) null;
    }
    if (this.parent != null)
      this.parent.Remove(this);
    if (this.sessions.Count <= 0)
      return;
    this.sessions[0].UpdateEndTime(Sapi.Now);
    Sapi sapi = Sapi.GetSapi();
    if (!sapi.LogFiles.Logging || this.LogFile != null)
      return;
    sapi.LogFiles.LogChannel(this);
  }

  internal bool GetActiveDiagnosticInformation(out byte? responseSession, out uint? responseVariant)
  {
    ByteMessage byteMessage = new ByteMessage(this, new Dump("22F100"));
    byteMessage.InternalDoMessage(true);
    if (byteMessage.Response != null && byteMessage.Response.Data.Count > 6)
    {
      responseSession = new byte?(byteMessage.Response.Data[6]);
      responseVariant = new uint?((uint) byteMessage.Response.Data[5] + ((uint) byteMessage.Response.Data[4] << 8));
    }
    else
    {
      responseSession = new byte?();
      responseVariant = new uint?();
    }
    return responseSession.HasValue;
  }

  private void MaintainIntendedSession()
  {
    byte? responseSession;
    uint? responseVariant;
    if (!this.GetActiveDiagnosticInformation(out responseSession, out responseVariant) || responseSession.Equals((object) this.IntendedSession))
      return;
    long? nullable1 = this.DiagnosisVariant.DiagnosticVersionLong;
    if (nullable1.HasValue && string.IsNullOrEmpty(this.fixedVariant))
    {
      uint? nullable2 = responseVariant;
      nullable1 = nullable2.HasValue ? new long?((long) nullable2.GetValueOrDefault()) : new long?();
      long num = this.DiagnosisVariant.DiagnosticVersionLong.Value & (long) ushort.MaxValue;
      if ((nullable1.GetValueOrDefault() == num ? (!nullable1.HasValue ? 1 : 0) : 1) != 0)
      {
        if (this.haveShownChangedLongDiagVersionError)
          return;
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Session ended, but no action taken because long diagnostic version has changed to 0x{0:x}", (object) responseVariant);
        Sapi.GetSapi().LogFiles.LabelLog(str);
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, str);
        this.haveShownChangedLongDiagVersionError = true;
        return;
      }
    }
    Sapi.GetSapi().LogFiles.LabelLog("Session ended, calling intialize service to recover.", this.ecu, this);
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Session ended, calling intialize service to recover.");
    this.ExecuteInitializeService();
  }

  internal byte? IntendedSession { private set; get; }

  internal void ExecuteInitializeService()
  {
    if ((this.channelOptions & ChannelOptions.ExecuteInitializeService) == ChannelOptions.None)
      return;
    this.services.InternalDereferencedExecute("InitializeService");
  }

  internal CaesarException Reset()
  {
    CaesarException caesarException = (CaesarException) null;
    if (this.mcdChannel != null)
    {
      try
      {
        this.mcdChannel.Close();
        this.instruments.Invalidate();
        this.mcdChannel.OpenLogicalLink((this.channelOptions & ChannelOptions.StartStopCommunications) != 0);
        if (this.mcdChannel.State == this.mcdChannel.TargetState)
          this.mcdChannel.IdentifyVariant();
        this.ExecuteInitializeService();
      }
      catch (McdException ex)
      {
        caesarException = new CaesarException(ex);
      }
    }
    else
    {
      this.caesarChannel.Exit();
      if (!this.caesarChannel.IsErrorSet)
      {
        ChannelCollection.InitComParameterCollection(this.caesarChannel, this.ConnectionResource);
        if (this.caesarChannel.Init())
        {
          if (!string.IsNullOrEmpty(this.fixedVariant))
            this.caesarChannel.Ecu.SetVariant(this.fixedVariant);
          this.ExecuteInitializeService();
        }
      }
      if (this.caesarChannel.IsErrorSet)
        caesarException = new CaesarException(this.caesarChannel);
    }
    return caesarException;
  }

  internal static void LoadFromLog(
    XElement channelElement,
    LogFileFormatTagCollection format,
    LogFile file,
    object ecusLock,
    object allChannelsLock,
    List<string> missingEcuList,
    List<string> missingVariantList,
    List<string> missingQualifierList,
    object missingInfoLock)
  {
    string ecuName = channelElement.Elements(format[TagName.EcuName]).First<XElement>().Value;
    XElement xelement1 = channelElement.Elements(format[TagName.EcuVariant]).First<XElement>();
    string str = xelement1.Value;
    XAttribute xattribute = xelement1.Attribute(format[TagName.Fixed]);
    bool? isFixedVariant = xattribute != null ? new bool?(xattribute.Value == "1") : new bool?();
    XElement xelement2 = channelElement.Elements(format[TagName.ChannelOptions]).FirstOrDefault<XElement>();
    ChannelOptions channelOptions = xelement2 != null ? (ChannelOptions) Enum.Parse(typeof (ChannelOptions), xelement2.Value) : ChannelOptions.All;
    IEnumerable<XElement> source1 = channelElement.Elements(format[TagName.EcuInfos]).First<XElement>().Elements(format[TagName.EcuInfo]);
    XElement xelement3 = channelElement.Elements(format[TagName.Instruments]).FirstOrDefault<XElement>();
    IEnumerable<XElement> source2 = (IEnumerable<XElement>) null;
    if (xelement3 != null)
      source2 = xelement3.Elements(format[TagName.Instrument]);
    XElement xelement4 = channelElement.Elements(format[TagName.Services]).FirstOrDefault<XElement>();
    IEnumerable<XElement> xelements1 = (IEnumerable<XElement>) null;
    if (xelement4 != null)
      xelements1 = xelement4.Elements(format[TagName.Service]);
    XElement xelement5 = channelElement.Elements(format[TagName.Parameters]).FirstOrDefault<XElement>();
    IEnumerable<XElement> xelements2 = (IEnumerable<XElement>) null;
    if (xelement5 != null)
      xelements2 = xelement5.Elements(format[TagName.Group]);
    Sapi sapi = Sapi.GetSapi();
    DiagnosisVariant diagnosisVariant = (DiagnosisVariant) null;
    Ecu ecu = (Ecu) null;
    lock (ecusLock)
    {
      ecu = sapi.Ecus[ecuName];
      if (ecu == null)
      {
        try
        {
          ecu = Ecu.CreateFromRollCallLog(ecuName);
        }
        catch (ArgumentException ex)
        {
        }
        catch (IndexOutOfRangeException ex)
        {
        }
        if (ecu == null)
        {
          lock (missingInfoLock)
          {
            missingEcuList.Add(ecuName);
            return;
          }
        }
      }
      diagnosisVariant = ecu.DiagnosisVariants[str];
      if (diagnosisVariant == null)
      {
        Ecu ecu1 = sapi.Ecus.FirstOrDefault<Ecu>((Func<Ecu, bool>) (e => e.Name == ecuName && e != ecu));
        if (ecu1 != null)
        {
          diagnosisVariant = ecu1.DiagnosisVariants[str];
          if (diagnosisVariant != null)
            ecu = ecu1;
        }
      }
    }
    if (diagnosisVariant == null)
    {
      IEnumerable<string> requiredQualifiers = (IEnumerable<string>) (source2 != null ? source2.Select<XElement, string>((Func<XElement, string>) (e => e.Attribute(format[TagName.Qualifier]).Value)) : (IEnumerable<string>) new List<string>()).Union<string>(source1.Select<XElement, string>((Func<XElement, string>) (e => e.Attribute(format[TagName.Qualifier]).Value))).ToList<string>();
      IEnumerable<\u003C\u003Ef__AnonymousType1<DiagnosisVariant, int>> matches = ecu.DiagnosisVariants.Select(ecuVariant => new
      {
        ecuVariant = ecuVariant,
        matchCount = ecuVariant.DiagServiceQualifiers.Intersect<string>(requiredQualifiers).Count<string>()
      }).OrderBy(_param1 => _param1.matchCount).Select(_param1 => new
      {
        Variant = _param1.ecuVariant,
        Count = _param1.matchCount
      });
      diagnosisVariant = matches.First(m => m.Count == matches.Last().Count).Variant;
      lock (missingInfoLock)
        missingVariantList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) ecuName, (object) str));
    }
    XElement xelement6 = channelElement.Elements(format[TagName.Resource]).FirstOrDefault<XElement>();
    ConnectionResource resource = xelement6 != null ? ConnectionResource.FromString(ecu, xelement6.Value) : (ConnectionResource) null;
    Channel channel;
    lock (allChannelsLock)
    {
      channel = file.AllChannels[diagnosisVariant];
      if (channel == null)
        channel = file.AllChannels.InternalConnectOffline(diagnosisVariant, file, resource);
    }
    XElement xelement7 = channelElement.Elements(format[TagName.Description]).FirstOrDefault<XElement>();
    DateTime dateTime1 = Sapi.TimeFromString(channelElement.Attribute(format[TagName.StartTime]).Value);
    DateTime dateTime2 = Sapi.TimeFromString(channelElement.Attribute(format[TagName.EndTime]).Value);
    channel.sessions.Add(new Session(channel, dateTime1, dateTime2, xelement7 != null ? xelement7.Value : string.Empty, resource, str, isFixedVariant, channelOptions));
    foreach (XElement element in source1)
      EcuInfo.LoadFromLog(element, format, channel, missingQualifierList, missingInfoLock);
    foreach (EcuInfo ecuInfo in channel.ecuInfos.Where<EcuInfo>((Func<EcuInfo, bool>) (ei => ei.EcuInfoType == EcuInfoType.Compound)))
      ecuInfo.LoadCompoundFromLog(dateTime1, dateTime2);
    if (source2 != null)
    {
      foreach (XElement element in source2)
        Instrument.LoadFromLog(element, format, channel, missingQualifierList, missingInfoLock);
    }
    else
      file.Summary = true;
    if (xelements1 != null)
    {
      foreach (XElement element in xelements1)
        Service.LoadFromLog(element, format, channel, missingQualifierList, missingInfoLock);
    }
    if (xelements2 != null)
    {
      bool flag = false;
      foreach (XElement element in xelements2)
        flag |= ParameterGroup.LoadFromLog(element, element.Attribute(format[TagName.Qualifier]).Value, format, channel, missingQualifierList, missingInfoLock);
      channel.Parameters.ValuesLoadedFromLog = flag;
    }
    foreach (XElement element in channelElement.Elements(format[TagName.FaultCodes]).First<XElement>().Elements(format[TagName.FaultCode]))
    {
      FaultCodeIncident incident = FaultCodeIncident.FromXElement(element, format, channel);
      if (incident.Functions == ReadFunctions.Snapshot)
        incident.FaultCode.Snapshots.Add(incident);
      else
        incident.FaultCode.FaultCodeIncidents.Add(incident);
    }
    XElement xelement8 = channelElement.Elements(format[TagName.CommunicationsStates]).FirstOrDefault<XElement>();
    if (xelement8 == null)
      return;
    foreach (CommunicationsStateValue newValue in (IEnumerable<CommunicationsStateValue>) xelement8.Elements(format[TagName.CommunicationsState]).Select<XElement, CommunicationsStateValue>((Func<XElement, CommunicationsStateValue>) (el => CommunicationsStateValue.FromXElement(el, format, channel))).OrderBy<CommunicationsStateValue, DateTime>((Func<CommunicationsStateValue, DateTime>) (csv => csv.Time)))
      channel.CommunicationsStateValues.Add(newValue, false);
  }

  public bool WriteAllParametersToSummaryFiles { get; set; }

  internal void WriteXmlTo(XmlWriter writer)
  {
    this.WriteXmlTo(true, this.StartTime, this.EndTime != DateTime.MinValue ? this.EndTime : Sapi.Now, writer);
  }

  internal void WriteSummaryXmlTo(XmlWriter writer)
  {
    this.WriteXmlTo(false, this.StartTime, this.EndTime != DateTime.MinValue ? this.EndTime : Sapi.Now, writer);
  }

  internal void WriteXmlTo(bool all, DateTime startTime, DateTime endTime, XmlWriter writer)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    writer.WriteStartElement(currentFormat[TagName.Channel].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.StartTime].LocalName, Sapi.TimeToString(startTime));
    writer.WriteAttributeString(currentFormat[TagName.EndTime].LocalName, Sapi.TimeToString(endTime));
    writer.WriteElementString(currentFormat[TagName.EcuName], this.Ecu.Name);
    writer.WriteStartElement(currentFormat[TagName.EcuVariant].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.Fixed].LocalName, !string.IsNullOrEmpty(this.fixedVariant) ? "1" : "0");
    writer.WriteValue(this.DiagnosisVariant.Name);
    writer.WriteEndElement();
    if (this.channelOptions != ChannelOptions.All)
      writer.WriteElementString(currentFormat[TagName.ChannelOptions], this.channelOptions.ToNumberString());
    writer.WriteElementString(currentFormat[TagName.Description], this.sessions[0].DescriptionVersion);
    writer.WriteElementString(currentFormat[TagName.Resource], this.ConnectionResource != null ? this.ConnectionResource.ToString() : "NULL");
    XElement xelement1 = new XElement(currentFormat[TagName.EcuInfos]);
    if (this.ecuInfos.Acquired)
    {
      foreach (EcuInfo ecuInfo in (ReadOnlyCollection<EcuInfo>) this.ecuInfos)
      {
        if (ecuInfo.EcuInfoType != EcuInfoType.Compound && ecuInfo.EcuInfoValues.Count > 0 && (all || ecuInfo.Common || ecuInfo.Summary))
          xelement1.Add((object) ecuInfo.GetXElement(startTime, endTime));
      }
    }
    xelement1.WriteTo(writer);
    writer.WriteStartElement(currentFormat[TagName.Instruments].LocalName);
    if (this.instruments.Acquired)
    {
      foreach (Instrument instrument in (ReadOnlyCollection<Instrument>) this.instruments)
      {
        if (instrument.InstrumentValues.Count > 0 && all | instrument.Summary)
          instrument.WriteXmlTo(all, startTime, endTime, writer);
      }
    }
    writer.WriteEndElement();
    if (all)
    {
      writer.WriteStartElement(currentFormat[TagName.Services].LocalName);
      List<Service> source = new List<Service>();
      if (this.services.Acquired)
        source.AddRange(this.services.Where<Service>((Func<Service, bool>) (s => s.Executions.Count > 0)));
      if (this.structuredServices != null && this.structuredServices.Acquired)
        source.AddRange(this.structuredServices.Where<Service>((Func<Service, bool>) (s => s.Executions.Count > 0)));
      foreach (Service service in source.Distinct<Service>())
        service.WriteXmlTo(startTime, endTime, writer);
      writer.WriteEndElement();
    }
    writer.WriteStartElement(currentFormat[TagName.Parameters].LocalName);
    if (this.parameters.Acquired)
    {
      foreach (ParameterGroup parameterGroup in (ReadOnlyCollection<ParameterGroup>) this.parameterGroups)
        parameterGroup.WriteXmlTo(startTime, endTime, writer, all || this.WriteAllParametersToSummaryFiles);
    }
    writer.WriteEndElement();
    XElement xelement2 = new XElement(currentFormat[TagName.FaultCodes]);
    if (this.faultCodes.Acquired)
    {
      foreach (FaultCode faultCode in this.faultCodes)
      {
        foreach (FaultCodeIncident faultCodeIncident in faultCode.FaultCodeIncidents)
        {
          XElement xelement3 = faultCodeIncident.GetXElement(startTime, endTime);
          if (xelement3 != null)
            xelement2.Add((object) xelement3);
        }
        foreach (FaultCodeIncident snapshot in faultCode.Snapshots)
        {
          XElement xelement4 = snapshot.GetXElement(startTime, endTime);
          if (xelement4 != null)
            xelement2.Add((object) xelement4);
        }
      }
    }
    xelement2.WriteTo(writer);
    XElement xelement5 = new XElement(currentFormat[TagName.CommunicationsStates]);
    CommunicationsStateValue communicationsStateValue1 = (CommunicationsStateValue) null;
    foreach (CommunicationsStateValue communicationsStateValue2 in (IEnumerable<CommunicationsStateValue>) this.communicationsStateValues.OrderBy<CommunicationsStateValue, DateTime>((Func<CommunicationsStateValue, DateTime>) (csv => csv.Time)))
    {
      if (communicationsStateValue2.Time >= startTime)
      {
        if (communicationsStateValue1 != null)
        {
          xelement5.Add((object) communicationsStateValue1.GetXElement(startTime));
          communicationsStateValue1 = (CommunicationsStateValue) null;
        }
        if (!(communicationsStateValue2.Time > endTime))
          xelement5.Add((object) communicationsStateValue2.GetXElement(startTime));
        else
          break;
      }
      else
        communicationsStateValue1 = communicationsStateValue2;
    }
    xelement5.WriteTo(writer);
    writer.WriteEndElement();
  }

  internal static void ExtractMetadata(XmlReader xmlReader, List<LogMetadataItem> result)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    string attribute = xmlReader.GetAttribute(currentFormat[TagName.StartTime].LocalName);
    xmlReader.ReadToDescendant(currentFormat[TagName.EcuName].LocalName);
    string str1 = xmlReader.ReadElementContentAsString();
    string str2 = xmlReader.ReadElementContentAsString();
    result.Add(new LogMetadataItem(LogMetadataType.Identification, str1, str2, attribute));
    xmlReader.ReadToNextSibling(currentFormat[TagName.FaultCodes].LocalName);
    if (xmlReader.ReadToDescendant(currentFormat[TagName.FaultCode].LocalName))
    {
      do
      {
        result.Add(FaultCode.ExtractMetadata(xmlReader, str1, str2));
      }
      while (xmlReader.NodeType == XmlNodeType.Element);
    }
    xmlReader.Skip();
    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == currentFormat[TagName.CommunicationsStates].LocalName)
      xmlReader.Skip();
    xmlReader.Skip();
  }

  internal void RaiseByteMessageComplete(ByteMessage s, Exception e)
  {
    FireAndForget.Invoke((MulticastDelegate) this.ByteMessageCompleteEvent, (object) s, (EventArgs) new ResultEventArgs(e));
    this.SyncDone(e);
  }

  internal void RaiseCommunicationsStateValueUpdateEvent(CommunicationsStateValue current)
  {
    FireAndForget.Invoke((MulticastDelegate) this.CommunicationsStateValueUpdateEvent, (object) this, (EventArgs) new CommunicationsStateValueEventArgs(current));
  }

  public void Init(bool autoread)
  {
    this.instruments.AutoRead = autoread;
    this.faultCodes.AutoRead = autoread;
    this.Init();
  }

  public void Init()
  {
    if (!this.Online)
      throw new InvalidOperationException("Cannot initialize an offline channel");
    if (this.closing)
      throw new InvalidOperationException("Cannot initialize a channel that is closing");
    if (this.thread != null)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "An attempt to reinitialize this channel was ignored");
    }
    else
    {
      this.thread = new Thread(new ThreadStart(this.ThreadFunc));
      this.thread.Name = $"{this.GetType().Name}: {this.ecu.Name}";
      this.thread.Start();
    }
  }

  public void Disconnect()
  {
    if (this.LogFile != null)
      throw new InvalidOperationException("Cannot disconnect a channel in a log file");
    this.InternalDisconnect();
  }

  public override string ToString() => this.ecu.Name;

  public ByteMessage SendByteMessage(Dump dump, bool synchronous)
  {
    ByteMessage o = new ByteMessage(this, dump);
    this.QueueAction((object) o, synchronous);
    return o;
  }

  public ByteMessage SendByteMessage(Dump dump, Dump requiredResponse, bool synchronous)
  {
    ByteMessage o = new ByteMessage(this, dump, requiredResponse);
    this.QueueAction((object) o, synchronous);
    return o;
  }

  public Ecu Ecu => this.ecu;

  public bool IsRollCall => this.ecu.IsRollCall;

  public IEnumerable<Channel> RelatedChannels
  {
    get
    {
      List<Channel> relatedChannels = new List<Channel>();
      if (this.IsRollCall)
        relatedChannels.AddRange(this.parent.Where<Channel>((Func<Channel, bool>) (c => c.IsRelated(this))));
      relatedChannels.AddRange(this.parent.Where<Channel>((Func<Channel, bool>) (c => c.IsRollCall && this.IsRelated(c))));
      return (IEnumerable<Channel>) relatedChannels;
    }
  }

  private bool IsRelated(Channel rollCallChannel)
  {
    return this.ecu.IsRelated(rollCallChannel, this.ActualViaEcu);
  }

  public Ecu ActualViaEcu
  {
    get
    {
      return this.ecu.ViaEcus.Any<Ecu>() ? this.parent.Where<Channel>((Func<Channel, bool>) (c => this.ecu.ViaEcus.Contains(c.ecu))).Select<Channel, Ecu>((Func<Channel, Ecu>) (c => c.Ecu)).FirstOrDefault<Ecu>() : (Ecu) null;
    }
  }

  public Channel ActualViaChannel
  {
    get
    {
      return this.ecu.ViaEcus.Any<Ecu>() ? this.parent.Where<Channel>((Func<Channel, bool>) (c => this.ecu.ViaEcus.Contains(c.ecu))).FirstOrDefault<Channel>() : (Channel) null;
    }
  }

  public DiagnosisVariant DiagnosisVariant => this.variant;

  public ConnectionResource ConnectionResource => this.connectionResource;

  public CommunicationsState CommunicationsState => this.communicationsState;

  public EcuInfoCollection EcuInfos => this.ecuInfos;

  public ParameterCollection Parameters => this.parameters;

  public ParameterGroupCollection ParameterGroups => this.parameterGroups;

  public InstrumentCollection Instruments => this.instruments;

  public FaultCodeCollection FaultCodes => this.faultCodes;

  public ServiceCollection Services => this.services;

  public FlashAreaCollection FlashAreas => this.flashAreas;

  public CodingParameterGroupCollection CodingParameterGroups => this.codingParameterGroups;

  public SessionCollection Sessions => this.sessions;

  public DateTime StartTime => this.sessions.StartTime;

  public DateTime EndTime => this.sessions.EndTime;

  public LogFile LogFile => this.logFile;

  public bool ActiveAtTime(DateTime time)
  {
    return this.logFile != null && this.sessions.ActiveAtTime(time) != (Session) null;
  }

  public string Identifier
  {
    get
    {
      if (this.ConnectionResource != null)
        return this.ConnectionResource.Identifier;
      if (this.Sessions.Count > 0)
      {
        Session session = this.Sessions[0];
        if (session.Resource != null)
          return session.Resource.Identifier;
      }
      return this.ecu.Identifier;
    }
  }

  public bool Online
  {
    get
    {
      switch (this.CommunicationsState)
      {
        case CommunicationsState.Disconnecting:
        case CommunicationsState.Offline:
        case CommunicationsState.LogFilePlayback:
        case CommunicationsState.LogFilePaused:
          return false;
        default:
          return true;
      }
    }
  }

  public ChannelExtension Extension
  {
    get
    {
      lock (this.channelExtensionLock)
      {
        if (!this.haveAttemptedExtensionLoad && this.channelExtension == null)
        {
          Type extensionType = this.ecu.ExtensionType;
          if (extensionType != (Type) null)
          {
            object[] objArray = new object[1]
            {
              (object) this
            };
            this.channelExtension = (ChannelExtension) Activator.CreateInstance(extensionType, objArray);
          }
          this.haveAttemptedExtensionLoad = true;
        }
        return this.channelExtension;
      }
    }
  }

  public CommunicationsStateValueCollection CommunicationsStateValues
  {
    get => this.communicationsStateValues;
  }

  public void SetCommunicationParameter(string name, string value)
  {
    if (this.ChannelHandle != null)
    {
      try
      {
        this.ChannelHandle.SetComParameter(name, value);
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        throw new CaesarException(ex, negativeResponseCode);
      }
    }
    else
    {
      if (this.McdChannelHandle == null)
        throw new InvalidOperationException("The CAESAR channel is NULL. This method only functions for an online channel.");
      try
      {
        this.McdChannelHandle.SetComParameter(name, (object) value);
      }
      catch (McdException ex)
      {
        throw new CaesarException(ex);
      }
    }
  }

  public string GetCommunicationParameter(string name)
  {
    if (this.ChannelHandle != null)
    {
      try
      {
        return this.ChannelHandle.GetComParameter(name);
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        throw new CaesarException(ex, negativeResponseCode);
      }
    }
    else
    {
      if (this.McdChannelHandle == null)
        throw new InvalidOperationException("The CAESAR channel is NULL. This method only functions for an online channel.");
      try
      {
        return this.McdChannelHandle.GetComParameter(name)?.ToString();
      }
      catch (McdException ex)
      {
        throw new CaesarException(ex);
      }
    }
  }

  internal RP1210ProtocolId? RP1210Protocol => this.rp1210Protocol;

  private void SetRP1210Protocol(long requestId)
  {
    uint channelId = 0;
    RP1210ProtocolId rp1210ProtocolId = RP1210ProtocolId.Can;
    ushort physicalChannel = 0;
    string rp1210ProtocolString = (string) null;
    byte[] array = ((IEnumerable<byte>) BitConverter.GetBytes(requestId)).Take<byte>(4).Reverse<byte>().ToArray<byte>();
    try
    {
      if (Sid.GetChannelInfo(new PassThruMsg(ProtocolId.Iso15765, array), ref channelId, ref rp1210ProtocolId, ref physicalChannel, ref rp1210ProtocolString) == J2534Error.NoError)
        this.rp1210Protocol = new RP1210ProtocolId?(rp1210ProtocolId);
      else
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to determine J2534 channel info as an error was returned");
    }
    catch (SEHException ex)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "Unable to determine J2534 channel info as SID is not loaded");
    }
  }

  private void GetActualRequestResponseIds()
  {
    this.requestId = GetCommunicationParameterAsLong(this.ChannelHandle != null ? "CP_REQUEST_CANIDENTIFIER" : (this.McdChannelHandle == null || this.McdChannelHandle.IsEthernet ? (string) null : "CP_CanPhysReqId"));
    this.responseId = GetCommunicationParameterAsLong(this.ChannelHandle != null ? "CP_RESPONSE_CANIDENTIFIER" : (this.McdChannelHandle == null || this.McdChannelHandle.IsEthernet ? (string) null : "CP_CanRespUSDTId"));

    long? GetCommunicationParameterAsLong(string parameter)
    {
      if (!string.IsNullOrEmpty(parameter))
      {
        string communicationParameter = this.GetCommunicationParameter(parameter);
        if (communicationParameter != null)
          return new long?(long.Parse(communicationParameter, (IFormatProvider) CultureInfo.InvariantCulture));
      }
      return new long?();
    }
  }

  public long? RequestIdComParameter => this.requestId;

  public long? ResponseIdComParameter => this.responseId;

  internal bool TryGetOfflineDiagnosticService(
    byte[] message,
    ByteMessageDirection direction,
    out List<Service> obj)
  {
    if (this.offlineServices == null)
      this.offlineServices = this.StructuredServices.Where<Service>((Func<Service, bool>) (s => s.ServiceTypes != ServiceTypes.DiagJob && s.BaseRequestMessage != null && s.BaseRequestMessage.Data.Count > 0)).GroupBy<Service, Tuple<Dump, Dump>>((Func<Service, Tuple<Dump, Dump>>) (s => Tuple.Create<Dump, Dump>(s.BaseRequestMessage, s.RequestMessageMask))).GroupBy<IGrouping<Tuple<Dump, Dump>, Service>, byte>((Func<IGrouping<Tuple<Dump, Dump>, Service>, byte>) (g => g.Key.Item1.Data[0])).ToDictionary<IGrouping<byte, IGrouping<Tuple<Dump, Dump>, Service>>, byte, List<Tuple<Dump, Dump, List<Service>>>>((Func<IGrouping<byte, IGrouping<Tuple<Dump, Dump>, Service>>, byte>) (k => k.Key), (Func<IGrouping<byte, IGrouping<Tuple<Dump, Dump>, Service>>, List<Tuple<Dump, Dump, List<Service>>>>) (v => v.Select<IGrouping<Tuple<Dump, Dump>, Service>, Tuple<Dump, Dump, List<Service>>>((Func<IGrouping<Tuple<Dump, Dump>, Service>, Tuple<Dump, Dump, List<Service>>>) (g => Tuple.Create<Dump, Dump, List<Service>>(g.Key.Item1, g.Key.Item2, g.ToList<Service>()))).ToList<Tuple<Dump, Dump, List<Service>>>()));
    if (direction == ByteMessageDirection.RX)
      message[0] -= (byte) 64 /*0x40*/;
    List<Tuple<Dump, Dump, List<Service>>> tupleList;
    if (this.offlineServices.TryGetValue(message[0], out tupleList))
    {
      foreach (Tuple<Dump, Dump, List<Service>> tuple in tupleList)
      {
        if (Dump.MaskedMatch(message, tuple.Item1, tuple.Item2))
        {
          obj = tuple.Item3;
          return true;
        }
      }
    }
    obj = (List<Service>) null;
    return false;
  }

  internal void SetManipulatedState(string item, bool manipulated)
  {
    string text;
    if (manipulated)
    {
      if (!this.manipulatedItems.Contains(item))
        this.manipulatedItems.Add(item);
      text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "MANIPULATION: {0}", (object) item);
    }
    else
    {
      this.manipulatedItems.Remove(item);
      text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "END MANIPULATION: {0}", (object) item);
    }
    Sapi.GetSapi().LogFiles.LabelLog(text, this.ecu, this);
  }

  public bool IsManipulated => this.manipulatedItems.Any<string>();

  public event CommunicationsStateUpdateEventHandler CommunicationsStateUpdateEvent;

  public event InitCompleteEventHandler InitCompleteEvent;

  public event ByteMessageCompleteEventHandler ByteMessageCompleteEvent;

  public event CommunicationsStateValueUpdateEventHandler CommunicationsStateValueUpdateEvent;

  public event SynchronousCheckEventHandler SynchronousCheckEvent;

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private bool SynchronousCheck(object action)
  {
    if (this.SynchronousCheckEvent == null)
      return true;
    SynchronousCheckEventArgs e = new SynchronousCheckEventArgs(action);
    this.SynchronousCheckEvent((object) this, e);
    return !e.Cancel;
  }

  internal bool IsOwner(CaesarChannel requested)
  {
    if (requested == null)
      return false;
    lock (this.channelHandleLock)
      return requested.Equals((object) this.caesarChannel);
  }

  internal bool IsOwner(McdLogicalLink requested)
  {
    if (requested == null)
      return false;
    lock (this.channelHandleLock)
      return requested.Equals((object) this.mcdChannel) || this.offlineVarcoding is VarcodeMcd offlineVarcoding && offlineVarcoding.IsOwner(requested);
  }

  private void CloseChannel()
  {
    this.ecu.DecrementConnectedChannelCount();
    if (this.caesarChannel != null)
    {
      CaesarChannel caesarChannel = (CaesarChannel) null;
      lock (this.channelHandleLock)
      {
        caesarChannel = this.caesarChannel;
        this.caesarChannel = (CaesarChannel) null;
      }
      if (caesarChannel != null)
      {
        caesarChannel.Exit();
        ((CaesarHandle\u003CCaesar\u003A\u003AChannelHandleStruct\u0020\u002A\u003E) caesarChannel).Dispose();
      }
    }
    else if (this.mcdChannel != null)
    {
      McdLogicalLink mcdLogicalLink = (McdLogicalLink) null;
      lock (this.channelHandleLock)
      {
        mcdLogicalLink = this.mcdChannel;
        this.mcdChannel = (McdLogicalLink) null;
      }
      mcdLogicalLink?.Dispose();
    }
    this.SetCommunicationsState(CommunicationsState.Offline);
  }

  internal void SetDataStreamSpns(int[] dataStreamSpns) => this.dataStreamSpns = dataStreamSpns;

  internal IEnumerable<int> DataStreamSpns
  {
    get
    {
      int[] dataStreamSpns = this.dataStreamSpns;
      return dataStreamSpns == null ? (IEnumerable<int>) null : (IEnumerable<int>) ((IEnumerable<int>) dataStreamSpns).ToArray<int>();
    }
  }

  public ChannelOptions ChannelOptions => this.channelOptions;

  private void Dispose(bool disposing)
  {
    if (!this.disposed && disposing)
    {
      this.InternalDisconnect();
      if (this.channelExtension != null)
      {
        this.channelExtension.Dispose();
        this.channelExtension = (ChannelExtension) null;
      }
      if (this.syncDone != null)
      {
        this.syncDone.Close();
        this.syncDone = (ManualResetEvent) null;
      }
      if (this.parameters != null)
      {
        this.parameters.Dispose();
        this.parameters = (ParameterCollection) null;
      }
      if (this.instruments != null)
      {
        this.instruments.Dispose();
        this.instruments = (InstrumentCollection) null;
      }
      if (this.actionsQueueNonEmptyEvent != null)
      {
        this.actionsQueueNonEmptyEvent.Close();
        this.actionsQueueNonEmptyEvent = (ManualResetEvent) null;
      }
    }
    this.disposed = true;
  }

  internal bool IsChannelErrorSet => this.caesarChannel != null && this.caesarChannel.IsErrorSet;

  internal void ClearCache()
  {
    if (this.ChannelHandle != null)
    {
      this.ChannelHandle.ClearCache();
    }
    else
    {
      if (this.McdChannelHandle == null)
        return;
      this.McdChannelHandle.ClearCache();
    }
  }

  private void ThreadFunc()
  {
    DateTime now1 = Sapi.Now;
    DateTime now2 = Sapi.Now;
    DateTime now3 = Sapi.Now;
    bool flag1 = false;
    bool flag2 = !this.IsRollCall && this.instruments.Any<Instrument>((Func<Instrument, bool>) (i => i.Periodic));
    if (!this.IsRollCall && this.ecu.IsUds)
    {
      this.GetActualRequestResponseIds();
      long? nullable = this.RequestIdComParameter;
      if (nullable.HasValue)
      {
        nullable = this.ResponseIdComParameter;
        if (nullable.HasValue)
        {
          Ecu ecu = this.ecu;
          nullable = this.RequestIdComParameter;
          long requestId = nullable.Value;
          nullable = this.ResponseIdComParameter;
          long responseId = nullable.Value;
          BusMonitorFrame.AddEcuCustomIdentifiers(ecu, requestId, responseId);
          this.SetRP1210Protocol(this.requestId.Value);
        }
      }
    }
    try
    {
      if (!this.closing && this.ChannelRunning)
        this.ExecuteInitializeService();
      if ((this.channelOptions & ChannelOptions.CyclicRead) != ChannelOptions.None)
      {
        if (!this.closing && this.ecuInfos.AutoRead && this.ChannelRunning)
        {
          this.SetCommunicationsState(CommunicationsState.ReadEcuInfo);
          this.ecuInfos.InternalRead(false);
        }
        if (!this.closing && this.parameters.AutoReadSummaryParameters)
        {
          foreach (Parameter parameter in this.parameters.Where<Parameter>((Func<Parameter, bool>) (p => p.Summary)).DistinctBy<Parameter, string>((Func<Parameter, string>) (p => p.GroupQualifier)))
          {
            if (!this.closing && this.ChannelRunning)
            {
              this.SetCommunicationsState(CommunicationsState.ReadParameters, parameter.GroupQualifier);
              this.parameters.InternalReadGroup(parameter, false);
            }
          }
        }
      }
      if (!this.closing && this.ChannelRunning)
      {
        FireAndForget.Invoke((MulticastDelegate) this.InitCompleteEvent, (object) this, new EventArgs());
        this.SetCommunicationsState(CommunicationsState.Online);
      }
      int num1 = 0;
      bool flag3 = false;
      bool result = false;
      if (this.ChannelRunning && !this.IsRollCall && this.ecu.Properties.ContainsKey("MaintainSession") && bool.TryParse(this.ecu.Properties["MaintainSession"], out result) & result)
      {
        if (this.mcdChannel != null)
        {
          int? sessionType = this.mcdChannel.SessionType;
          if (sessionType.HasValue)
            this.IntendedSession = new byte?((byte) sessionType.Value);
        }
        else if (this.ecu.ProtocolComParameters != null && this.ecu.ProtocolComParameters.Contains((object) "CP_INIT_SESSION_TYPE"))
          this.IntendedSession = new byte?(Convert.ToByte(this.ecu.ProtocolComParameters[(object) "CP_INIT_SESSION_TYPE"], (IFormatProvider) CultureInfo.InvariantCulture));
      }
      while (!this.closing)
      {
        if (this.Online)
        {
          if (this.ChannelRunning && (this.caesarChannel != null && !this.caesarChannel.IsErrorSet || this.ecu.RollCallManager != null || this.mcdChannel != null))
          {
            if (flag2 && !flag1)
            {
              this.instruments.RegisterPeriodicListeners(true);
              flag1 = true;
            }
            if (this.CommunicationsState != CommunicationsState.Online)
              this.instruments.AgeAll();
            else
              this.instruments.InvalidateAged();
            switch (this.CommunicationsState)
            {
              case CommunicationsState.Online:
                if (this.actionsQueue.Count > 0)
                {
                  object cs = this.actionsQueue.Dequeue();
                  if (cs.GetType() == typeof (ServiceExecution))
                  {
                    ServiceExecution currentExecution = (ServiceExecution) cs;
                    this.SetCommunicationsState(CommunicationsState.ExecuteService, currentExecution.Service.Qualifier);
                    currentExecution.Service.InternalExecute(Service.ExecuteType.UserInvoke, currentExecution);
                    this.SetCommunicationsState(CommunicationsState.Online);
                    break;
                  }
                  if (cs.GetType() == typeof (Instrument))
                  {
                    Instrument instrument = (Instrument) cs;
                    this.SetCommunicationsState(CommunicationsState.ReadInstrument, instrument.Qualifier);
                    instrument.InternalRead(true);
                    this.SetCommunicationsState(CommunicationsState.Online);
                    break;
                  }
                  if (cs.GetType() == typeof (FaultCode))
                  {
                    FaultCode faultCode = (FaultCode) cs;
                    this.SetCommunicationsState(CommunicationsState.ResetFault, faultCode.Code);
                    faultCode.InternalReset();
                    num1 = 0;
                    this.SetCommunicationsState(CommunicationsState.Online);
                    break;
                  }
                  if (cs.GetType() == typeof (string))
                  {
                    this.SetCommunicationsState(CommunicationsState.ExecuteService, cs.ToString());
                    this.services.InternalExecute(cs.ToString(), true);
                    this.SetCommunicationsState(CommunicationsState.Online);
                    break;
                  }
                  if (cs.GetType() == typeof (EcuInfo))
                  {
                    EcuInfo ecuInfo = (EcuInfo) cs;
                    this.SetCommunicationsState(CommunicationsState.ReadEcuInfo, ecuInfo.Qualifier);
                    ecuInfo.InternalRead(true);
                    this.SetCommunicationsState(CommunicationsState.Online);
                    break;
                  }
                  if (cs.GetType() == typeof (Parameter))
                  {
                    Parameter parameter = (Parameter) cs;
                    this.SetCommunicationsState(CommunicationsState.ReadParameters, parameter.GroupQualifier);
                    this.parameters.InternalReadGroup(parameter, true);
                    break;
                  }
                  if (cs.GetType() == typeof (ByteMessage))
                  {
                    ByteMessage byteMessage = (ByteMessage) cs;
                    this.SetCommunicationsState(CommunicationsState.ByteMessage, byteMessage.Request.ToString());
                    byteMessage.InternalDoMessage();
                    this.SetCommunicationsState(CommunicationsState.Online);
                    break;
                  }
                  this.SetCommunicationsState((CommunicationsState) cs);
                  break;
                }
                this.actionsQueueNonEmptyEvent.Reset();
                bool flag4 = this.caesarChannel != null && Channel.flashing;
                switch (num1)
                {
                  case 0:
                    if ((this.channelOptions & ChannelOptions.MaintainSession) != ChannelOptions.None && this.IntendedSession.HasValue && (this.caesarChannel != null || this.mcdChannel != null) && (Sapi.Now - now3).TotalSeconds >= 1.0)
                    {
                      this.MaintainIntendedSession();
                      flag3 = true;
                      now3 = Sapi.Now;
                      break;
                    }
                    break;
                  case 1:
                    if ((this.channelOptions & ChannelOptions.CyclicRead) != ChannelOptions.None && this.faultCodes.SupportsFaultRead && this.faultCodes.AutoRead && !this.ecu.IsVirtual)
                    {
                      if ((this.caesarChannel != null || this.mcdChannel != null) && !flag4 && (Sapi.Now - now1).TotalSeconds >= 1.0)
                      {
                        this.faultCodes.InternalRead(false);
                        now1 = Sapi.Now;
                        flag3 = true;
                      }
                      if (this.faultCodes.SupportsSnapshot && !flag4 && (Sapi.Now - now2).TotalSeconds >= 10.0)
                      {
                        this.faultCodes.InternalReadSnapshot(false);
                        now2 = Sapi.Now;
                        flag3 = true;
                        break;
                      }
                      break;
                    }
                    break;
                  case 2:
                    if ((this.channelOptions & ChannelOptions.CyclicRead) != ChannelOptions.None && this.instruments.AutoRead && !flag4 && this.instruments.InternalRead())
                    {
                      flag3 = true;
                      break;
                    }
                    break;
                  case 3:
                    if ((this.channelOptions & ChannelOptions.CyclicRead) != ChannelOptions.None && this.ecuInfos.AutoRead && this.ecuInfos.InternalRead(EcuInfoInternalReadType.CyclicRead))
                    {
                      flag3 = true;
                      break;
                    }
                    break;
                }
                if (++num1 > 3)
                {
                  if (!flag3)
                    this.actionsQueueNonEmptyEvent.WaitOne(this.IsRollCall ? 100 : 10);
                  else
                    flag3 = false;
                  num1 = 0;
                  break;
                }
                break;
              case CommunicationsState.ReadEcuInfo:
                this.ecuInfos.InternalRead(true);
                break;
              case CommunicationsState.ReadParameters:
                this.parameters.InternalRead();
                break;
              case CommunicationsState.WriteParameters:
                this.parameters.InternalWrite();
                break;
              case CommunicationsState.Flash:
                Channel.flashing = true;
                this.flashAreas.InternalFlash();
                Channel.flashing = false;
                break;
              case CommunicationsState.ResetFaults:
                this.faultCodes.InternalReset();
                num1 = 0;
                break;
              case CommunicationsState.ReadFaults:
                this.faultCodes.InternalRead(true);
                break;
              case CommunicationsState.ProcessVcp:
                this.parameters.InternalProcessVcp();
                break;
              case CommunicationsState.ReadSnapshot:
                this.faultCodes.InternalReadSnapshot(true);
                break;
            }
          }
          int num2 = this.ChannelRunning ? 1 : 0;
          if (this.caesarChannel != null && this.caesarChannel.IsErrorSet || this.mcdChannel != null && this.mcdChannel.ChannelError != null)
          {
            CaesarException e = this.DisconnectionException = this.caesarChannel != null ? new CaesarException(this.caesarChannel) : new CaesarException(this.mcdChannel.ChannelError);
            if (this.mcdChannel != null)
              this.mcdChannel.ResetChannelError();
            Sapi.GetSapi().RaiseExceptionEvent((object) this, (Exception) e);
            if (Sapi.GetSapi().LogFiles.Logging)
              Sapi.GetSapi().LogFiles.LabelLog(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Communications Error: {0}", (object) e.Message), this.ecu, this);
          }
          if (num2 == 0)
          {
            this.SetCommunicationsState(CommunicationsState.Disconnecting, this.DisconnectionException != null ? this.DisconnectionException.Message : (this.stopReason != StopReason.None ? this.stopReason.ToString() : "Channel stopped running"));
            this.actionsQueue.Clear();
          }
        }
        else
          break;
      }
    }
    catch (Exception ex)
    {
      if (this.CommunicationsState == CommunicationsState.Flash)
        Channel.flashing = false;
      this.SetCommunicationsState(CommunicationsState.Disconnecting, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Fatal Error: {0}", (object) ex.Message));
      this.actionsQueue.Clear();
      Sapi.GetSapi().RaiseExceptionEvent((object) this, ex);
    }
    if (this.syncDone != null)
      this.SyncDone((Exception) new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation));
    if (flag2 & flag1)
      this.instruments.RegisterPeriodicListeners(false);
    this.instruments.Invalidate();
    this.faultCodes.Invalidate();
    this.CloseChannel();
  }

  public CaesarException DisconnectionException { internal set; get; }
}
