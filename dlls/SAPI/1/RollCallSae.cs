// Decompiled with JetBrains decompiler
// Type: SapiLayer1.RollCallSae
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using J2534;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

internal abstract class RollCallSae : RollCall, IDisposable, IRollCall
{
  private const int J2534ConnectionRetryInterval = 2500;
  private const int PortIndex = 1;
  private static object stopStartLock = new object();
  private Dictionary<string, TranslationEntry> translation;
  private Dictionary<byte?, Dictionary<string, string>> suspectParametersByAddress;
  private Dictionary<string, string> commonSuspectParameters;
  private Dictionary<int, Tuple<string, string>> parameterGroups;
  private const int RetryCount = 3;
  protected const int BetweenBusyRequestInterval = 1000;
  protected const int BusyRetryCount = 5;
  protected const int SynchronousRequestTimeout = 2000;
  private int channelTimeout;
  protected uint channelId;
  private ushort? restrictedAddress;
  private IEnumerable<byte> clientRestrictedAddresses;
  private volatile int messagesThisLoadPeriod;
  private DateTime lastLoadMeasurement;
  private object queuedItemsLock = new object();
  private List<RollCallSae.QueueItem> queuedItems = new List<RollCallSae.QueueItem>();
  private RollCallSae.MonitorData monitorData;

  protected RollCallSae(Protocol ProtocolId)
    : base(ProtocolId)
  {
    string s = Sapi.GetSapi().ConfigurationItems[$"RollCallRestrict{this.Protocol.ToString()}Address"].Value;
    ushort result;
    if (string.IsNullOrEmpty(s) || !ushort.TryParse(s, out result))
      return;
    this.restrictedAddress = new ushort?(result);
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "WARNING: Monitoring is restricted to address " + (object) this.restrictedAddress);
  }

  internal override void Init()
  {
    try
    {
      J2534Error j2534Error = Sid.Connect((ProtocolId) this.protocolId, 0U, ref this.channelId);
      if (j2534Error == J2534Error.NoError)
      {
        int num = (int) Sid.SetPassthruCallback(this.channelId, new J2534.PassthruCallback(this.PassthruCallback));
        if (this.debugLevel > (ushort) 0)
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Result from J2534.Connect() is " + (object) this.channelId);
        this.State = ConnectionState.Initialized;
        bool isCapable = false;
        if (Sid.IsAutoBaudRateCapable(this.channelId, ref isCapable) == J2534Error.NoError)
          this.IsAutoBaudRate = isCapable;
        this.DeviceName = Sid.GetDeviceName(this.channelId);
        this.UpdateDeviceVersionInfo();
      }
      else
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Result from J2534.Connect() is " + j2534Error.ToString());
        this.State = ConnectionState.NotInitialized;
      }
    }
    catch (SEHException ex)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Roll-call services are unavailable as SID is not loaded.");
      this.State = ConnectionState.NotInitialized;
    }
  }

  private void UpdateDeviceVersionInfo()
  {
    string libraryName = (string) null;
    string libraryVersion = (string) null;
    string driverVersion = (string) null;
    string firmwareVersion = (string) null;
    string supportedProtocols = (string) null;
    Sid.GetDeviceVersionInfo(this.channelId, ref libraryName, ref libraryVersion, ref driverVersion, ref firmwareVersion, ref supportedProtocols);
    this.DeviceLibraryName = libraryName;
    this.DeviceLibraryVersion = libraryVersion;
    this.DeviceDriverVersion = driverVersion;
    this.DeviceFirmwareVersion = firmwareVersion;
    this.DeviceSupportedProtocols = supportedProtocols;
  }

  public override void SetRestrictedAddressList(IEnumerable<byte> restrictedSourceAddresses)
  {
    this.clientRestrictedAddresses = restrictedSourceAddresses;
  }

  internal override void Exit()
  {
    if (this.channelId != 0U)
    {
      int num = (int) Sid.SetPassthruCallback(this.channelId, (J2534.PassthruCallback) null);
      J2534Error j2534Error = Sid.Disconnect(this.channelId);
      if (j2534Error != J2534Error.NoError)
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, $"Result from J2534.Disconnect for channel {(object) this.channelId} is {j2534Error.ToString()}");
      this.channelId = 0U;
      this.State = ConnectionState.NotInitialized;
      this.DeviceName = (string) null;
      this.DeviceLibraryVersion = (string) null;
      this.DeviceFirmwareVersion = (string) null;
      this.DeviceDriverVersion = (string) null;
      this.DeviceSupportedProtocols = (string) null;
      this.IsAutoBaudRate = false;
    }
    this.monitorData = (RollCallSae.MonitorData) null;
    this.ClearEcus();
  }

  private ConnectionResource CreateConnectionResource(
    byte sourceAddress,
    int? function,
    bool overrideFunctionRequirement = false)
  {
    return ((!this.RequiresFunction(sourceAddress) ? 1 : (function.HasValue ? 1 : 0)) | (overrideFunctionRequirement ? 1 : 0)) != 0 ? new ConnectionResource(this.GetEcu((int) sourceAddress, function), this.protocolId, this.DeviceName, this.BaudRate, 1, (int) sourceAddress) : (ConnectionResource) null;
  }

  protected override IEnumerable<Ecu> LoadMonitorEcus()
  {
    this.monitorData = RollCallSae.MonitorData.Load(this);
    return (IEnumerable<Ecu>) new List<Ecu>(this.monitorData.GetEcus(this));
  }

  protected void AddIdentification(byte sourceAddress, RollCall.ID id, object value)
  {
    lock (this.addressInformation)
    {
      RollCall.ChannelInformation channelInformation = this.addressInformation[(int) sourceAddress];
      if (channelInformation.Resource == null && id == RollCall.ID.Function)
        channelInformation.Resource = this.CreateConnectionResource(sourceAddress, new int?((int) value));
      if (channelInformation.Invalid)
        return;
      RollCall.IdentificationInformation identificationInformation = channelInformation.Identification.Where<RollCall.IdentificationInformation>((Func<RollCall.IdentificationInformation, bool>) (ci => ci.Id == id)).FirstOrDefault<RollCall.IdentificationInformation>();
      if (identificationInformation == null)
        return;
      identificationInformation.Value = value;
      if (this.debugLevel <= (ushort) 1)
        return;
      this.RaiseDebugInfoEvent((int) sourceAddress, $"Got identification {(object) id}: {value}");
    }
  }

  protected void SetDataStreamSpns(byte sourceAddress, int[] dataStreamSpns)
  {
    lock (this.addressInformation)
    {
      RollCall.ChannelInformation channelInformation = this.addressInformation[(int) sourceAddress];
      if (channelInformation.Invalid)
        return;
      channelInformation.SetDataStreamSpns(dataStreamSpns);
    }
  }

  internal override bool IsVirtual(int sourceAddress)
  {
    return (int) this.GlobalRequestAddress == sourceAddress;
  }

  protected override TimeSpan RollCallValidLastSeenTime => TimeSpan.FromSeconds(2.0);

  protected override void ThreadFunc()
  {
    Sapi sapi = Sapi.GetSapi();
    do
    {
      bool flag = false;
      lock (this.ecusLock)
        flag = this.ecus != null;
      if (flag && !sapi.Channels.Any<Channel>((Func<Channel, bool>) (c => c.Ecu.RollCallManager == this)))
      {
        IEnumerable<Tuple<byte[], byte[]>> tuples = this.Protocol != Protocol.J1939 || this.clientRestrictedAddresses == null ? Enumerable.Repeat<Tuple<byte[], byte[]>>(new Tuple<byte[], byte[]>(new byte[6], new byte[6]), 1) : this.clientRestrictedAddresses.Select<byte, Tuple<byte[], byte[]>>((Func<byte, Tuple<byte[], byte[]>>) (sa =>
        {
          byte[] numArray1 = new byte[6];
          numArray1[4] = byte.MaxValue;
          byte[] numArray2 = new byte[6];
          numArray2[4] = sa;
          return new Tuple<byte[], byte[]>(numArray1, numArray2);
        }));
        List<uint> uintList = new List<uint>();
        J2534Error j2534Error1 = J2534Error.NoError;
        lock (RollCallSae.stopStartLock)
        {
          foreach (Tuple<byte[], byte[]> tuple in tuples)
          {
            uint filterId = 0;
            j2534Error1 = Sid.StartMsgFilter(this.channelId, FilterType.Pass, new PassThruMsg((ProtocolId) this.protocolId, tuple.Item1), new PassThruMsg((ProtocolId) this.protocolId, tuple.Item2), (PassThruMsg) null, ref filterId);
            if (j2534Error1 == J2534Error.NoError)
              uintList.Add(filterId);
            else
              break;
          }
        }
        if (j2534Error1 == J2534Error.NoError)
        {
          if (this.clientRestrictedAddresses == null || this.Protocol == Protocol.J1708)
          {
            int pass = (int) Sid.SetAllFiltersToPass(this.channelId, (byte) 1);
          }
          this.UpdateDeviceVersionInfo();
          this.State = ConnectionState.TranslatorConnected;
          this.Load = new float?(100f);
          StopReason reason = this.InnerConnectionLoop();
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "InnerConnectionLoop has stopped with reason " + (object) reason);
          if (reason == StopReason.TranslatorDisconnected)
            this.State = ConnectionState.TranslatorDisconnected;
          this.Load = new float?();
          if (this.Protocol == Protocol.J1939 && reason == StopReason.TranslatorDisconnected)
          {
            foreach (Channel channel in (ChannelBaseCollection) sapi.Channels)
              channel.Abort(reason);
            sapi.Channels.AbortPendingConnections((Func<ConnectionResource, bool>) (cr => !cr.IsEthernet));
          }
          this.State = ConnectionState.WaitingForTranslator;
          foreach (uint filterId in uintList)
          {
            J2534Error j2534Error2;
            lock (RollCallSae.stopStartLock)
              j2534Error2 = Sid.StopMsgFilter(this.channelId, filterId);
            if (j2534Error2 != J2534Error.NoError)
              Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, $"Result from J2534.StopMsgFilter() is {j2534Error2.ToString()} GetLastError is {Sid.GetLastError()}");
          }
          this.ClearAddressInformation();
        }
        else
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, $"Result from J2534.StartMsgFilter() is {j2534Error1.ToString()} GetLastError is {Sid.GetLastError()}");
          if (this.Protocol == Protocol.J1939)
            sapi.Channels.AbortPendingConnections((Func<ConnectionResource, bool>) (cr => !cr.IsEthernet));
        }
      }
      else if (flag)
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Waiting for channels to go offline: " + string.Join(", ", sapi.Channels.Where<Channel>((Func<Channel, bool>) (c => c.Ecu.RollCallManager == this)).Select<Channel, string>((Func<Channel, string>) (c => c.Ecu.Name))));
    }
    while (!this.closingEvent.WaitOne(2500));
  }

  protected virtual Presentation CreatePresentation(Ecu ecu, string qualifier)
  {
    return (Presentation) null;
  }

  internal override EcuInfo CreateEcuInfo(EcuInfoCollection ecuInfos, string qualifier)
  {
    int requestId = this.MapIdToRequestId((RollCall.ID) Convert.ToInt32(qualifier, (IFormatProvider) CultureInfo.InvariantCulture));
    bool visible = this.IsRequestIdContentVisible(requestId);
    EcuInfo ecuInfo = new EcuInfo(ecuInfos.Channel, EcuInfoType.RollCall, qualifier, string.Empty, "Common", "Rollcall/Common", this.CreatePresentation(ecuInfos.Channel.Ecu, qualifier), requestId, visible, true, true);
    ecuInfos.AddFromRollCall(ecuInfo);
    return ecuInfo;
  }

  internal override void CreateEcuInfos(EcuInfoCollection ecuInfos)
  {
    if (ecuInfos.Channel.Online)
    {
      lock (this.addressInformation)
      {
        if (this.addressInformation.ContainsKey((int) ecuInfos.Channel.SourceAddress.Value))
          this.addressInformation[(int) ecuInfos.Channel.SourceAddress.Value].CreateEcuInfos(ecuInfos);
      }
    }
    else if (ecuInfos.Channel.LogFile == null)
    {
      foreach (RollCall.ID identificationId in this.GetIdentificationIds(ecuInfos.Channel.SourceAddress))
        this.CreateEcuInfo(ecuInfos, identificationId.ToNumberString());
    }
    foreach (XElement definition in this.monitorData.GetDefinitions(ecuInfos.Channel, typeof (EcuInfo)))
    {
      Instrument instrument1 = new Instrument(ecuInfos.Channel, definition.Attribute((XName) "qualifier").Value);
      Dictionary<string, string> dictionary = definition.Elements((XName) "Property").ToDictionary<XElement, string, string>((Func<XElement, string>) (k => k.Attribute((XName) "name").Value), (Func<XElement, string>) (v => v.Value));
      instrument1.AcquireFromRollCall((IDictionary<string, string>) dictionary);
      string namedPropertyValue1 = dictionary.GetNamedPropertyValue<string>("GroupQualifier", "StoredData");
      string namedPropertyValue2 = dictionary.GetNamedPropertyValue<string>("GroupName", "Stored Data");
      bool namedPropertyValue3 = dictionary.GetNamedPropertyValue<bool>("Visible", false);
      bool namedPropertyValue4 = dictionary.GetNamedPropertyValue<bool>("Common", false);
      bool namedPropertyValue5 = dictionary.GetNamedPropertyValue<bool>("Summary", false);
      Dictionary<string, string> elements = dictionary;
      int? nullable = new int?();
      int? defaultIfNotSet = nullable;
      int? namedPropertyValue6 = elements.GetNamedPropertyValue<int?>("CacheTime", defaultIfNotSet);
      Channel channel = ecuInfos.Channel;
      string qualifier = instrument1.Qualifier;
      string name = instrument1.Name;
      string groupQualifier = namedPropertyValue1;
      string groupName = namedPropertyValue2;
      Instrument instrument2 = instrument1;
      nullable = instrument1.MessageNumber;
      int messageNumber = nullable.Value;
      int num1 = namedPropertyValue3 ? 1 : 0;
      int num2 = namedPropertyValue4 ? 1 : 0;
      int num3 = namedPropertyValue5 ? 1 : 0;
      int? cacheTime = namedPropertyValue6;
      EcuInfo ecuInfo = new EcuInfo(channel, EcuInfoType.RollCall, qualifier, name, groupQualifier, groupName, (Presentation) instrument2, messageNumber, num1 != 0, num2 != 0, num3 != 0, cacheTime);
      ecuInfos.AddFromRollCall(ecuInfo);
    }
  }

  internal override IEnumerable<Instrument> CreateInstruments(Channel channel)
  {
    foreach (XElement definition in this.monitorData.GetDefinitions(channel, typeof (Instrument)))
    {
      Instrument instrument = new Instrument(channel, definition.Attribute((XName) "qualifier").Value);
      instrument.AcquireFromRollCall((IDictionary<string, string>) definition.Elements((XName) "Property").ToDictionary<XElement, string, string>((Func<XElement, string>) (k => k.Attribute((XName) "name").Value), (Func<XElement, string>) (v => v.Value)));
      yield return instrument;
    }
  }

  internal override Instrument CreateBaseInstrument(Channel channel, string qualifier)
  {
    Instrument baseInstrument = new Instrument(channel, qualifier);
    XElement baseDefinition = this.monitorData.GetBaseDefinition(qualifier, typeof (Instrument));
    if (baseDefinition != null)
      baseInstrument.AcquireFromRollCall((IDictionary<string, string>) baseDefinition.Elements((XName) "Property").ToDictionary<XElement, string, string>((Func<XElement, string>) (k => k.Attribute((XName) "name").Value), (Func<XElement, string>) (v => v.Value)));
    else
      baseInstrument.AcquireFromRollCall((IDictionary<string, string>) new Dictionary<string, string>()
      {
        ["SlotType"] = "4",
        ["Units"] = "raw"
      });
    return baseInstrument;
  }

  internal override IEnumerable<Service> CreateServices(Channel channel, ServiceTypes type)
  {
    if (type == ServiceTypes.Environment)
    {
      Service service = new Service(channel, type, 1216.ToString());
      service.AcquireFromRollCall();
      yield return service;
    }
    else
    {
      foreach (XElement definition in this.monitorData.GetDefinitions(channel, typeof (Service)))
      {
        Service service = new Service(channel, ServiceTypes.Routine, definition.Attribute((XName) "qualifier").Value);
        service.AcquireFromRollCall((IDictionary<string, string>) definition.Elements((XName) "Property").ToDictionary<XElement, string, string>((Func<XElement, string>) (k => k.Attribute((XName) "name").Value), (Func<XElement, string>) (v => v.Value)));
        yield return service;
      }
    }
  }

  private StopReason InnerConnectionLoop()
  {
    int num1 = 0;
    List<int> list = this.CycleGlobalRequestIds.ToList<int>();
    while (true)
    {
      do
      {
        byte hardwareStatus;
        byte protocolStatusJ1939;
        byte protocolStatusJ1708;
        byte protocolStatusCan;
        byte protocolStatusIso15765;
        do
        {
          if (this.State != ConnectionState.TranslatorConnectedNoTraffic)
          {
            lock (this.addressInformation)
            {
              bool flag1 = this.addressInformation.Count > 0;
              bool flag2 = this.addressInformation.Values.Any<RollCall.ChannelInformation>((Func<RollCall.ChannelInformation, bool>) (ci => ci.Resource != null && ci.Channel == null && !ci.Invalid));
              this.State = !flag1 || !this.ConnectEnabled ? ConnectionState.TranslatorConnected : (flag2 ? ConnectionState.ChannelsConnecting : ConnectionState.ChannelsConnected);
              if (flag1)
              {
                if (!this.addressInformation.ContainsKey((int) this.GlobalRequestAddress))
                  this.addressInformation.Add((int) this.GlobalRequestAddress, (RollCall.ChannelInformation) new RollCallSae.ChannelInformation(this, this.CreateConnectionResource(this.GlobalRequestAddress, new int?()), (IEnumerable<RollCall.ID>) new List<RollCall.ID>()));
              }
            }
            int id = list[num1++];
            if (num1 == list.Count)
              num1 = 0;
            if (!this.RequestId(id, this.GlobalRequestAddress))
              return StopReason.TranslatorDisconnected;
          }
          if (this.closingEvent.WaitOne(this.BetweenGlobalIdRequestInterval))
            return StopReason.Closing;
          this.UpdateLoadCalculation();
          if (this.protocolId == Protocol.J1939 && this.addressInformation.Count > 0)
          {
            float? load = this.Load;
            float num2 = 0.0f;
            if (((double) load.GetValueOrDefault() == (double) num2 ? (load.HasValue ? 1 : 0) : 0) != 0)
              goto label_18;
          }
          if (this.State != ConnectionState.TranslatorConnectedNoTraffic)
            continue;
label_18:
          hardwareStatus = (byte) 0;
          protocolStatusJ1939 = (byte) 0;
          protocolStatusJ1708 = (byte) 0;
          protocolStatusCan = (byte) 0;
          protocolStatusIso15765 = (byte) 0;
        }
        while (Sid.GetHardwareStatus(this.channelId, ref hardwareStatus, ref protocolStatusJ1939, ref protocolStatusJ1708, ref protocolStatusCan, ref protocolStatusIso15765) != J2534Error.NoError);
        if (((int) hardwareStatus & 1) == 0)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "The hardware is not connected, according to GetHardwareStatus. Channels will be aborted.");
          return StopReason.TranslatorDisconnected;
        }
        this.CheckStatusAndAbortChannels(protocolStatusJ1939, RP1210ProtocolId.J1939);
        this.CheckStatusAndAbortChannels(protocolStatusIso15765, RP1210ProtocolId.Iso15765);
        this.CheckStatusAndAbortChannels(protocolStatusCan, RP1210ProtocolId.Can);
        if (((int) protocolStatusJ1939 & 2) == 0)
        {
          if (this.State != ConnectionState.TranslatorConnectedNoTraffic)
          {
            this.State = ConnectionState.TranslatorConnectedNoTraffic;
            this.ClearAddressInformation();
          }
        }
      }
      while (this.State != ConnectionState.TranslatorConnectedNoTraffic);
      this.State = ConnectionState.TranslatorConnected;
    }
  }

  private void CheckStatusAndAbortChannels(byte statusByte, RP1210ProtocolId protocol)
  {
    if (((int) statusByte & 2) != 0)
      return;
    List<Channel> list = Sapi.GetSapi().Channels.Where<Channel>((Func<Channel, bool>) (c =>
    {
      RP1210ProtocolId? rp1210Protocol = c.RP1210Protocol;
      RP1210ProtocolId rp1210ProtocolId = protocol;
      return rp1210Protocol.GetValueOrDefault() == rp1210ProtocolId && rp1210Protocol.HasValue;
    })).ToList<Channel>();
    if (list.Count <= 0)
      return;
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"Traffic stopped for {(object) protocol}; aborting channels {string.Join(" ", list.Select<Channel, string>((Func<Channel, string>) (c => c.Ecu.Name)).ToArray<string>())}");
    list.ForEach((Action<Channel>) (c => c.Abort(StopReason.NoTraffic)));
  }

  private void UpdateLoadCalculation()
  {
    double totalSeconds = (Sapi.Now - this.lastLoadMeasurement).TotalSeconds;
    int messagesThisLoadPeriod = this.messagesThisLoadPeriod;
    this.messagesThisLoadPeriod = 0;
    this.lastLoadMeasurement = Sapi.Now;
    double messagesPerSecond = (double) this.TotalMessagesPerSecond;
    double num = totalSeconds * messagesPerSecond;
    this.Load = new float?((float) ((double) messagesThisLoadPeriod / num * 100.0));
  }

  internal void PassthruCallback(PassThruMsg message)
  {
    if (!this.ProtocolAlive)
      return;
    ++this.messagesThisLoadPeriod;
    byte[] data = message.GetData();
    byte address;
    int id;
    if (!this.TryExtractMessage(data, out address, out id, out data) || this.restrictedAddress.HasValue && (int) address != (int) this.restrictedAddress.Value || this.clientRestrictedAddresses != null && !this.clientRestrictedAddresses.Contains<byte>(address) || !this.ConnectEnabled)
      return;
    Channel channel = (Channel) null;
    lock (this.addressInformation)
    {
      if (!this.addressInformation.ContainsKey((int) address))
      {
        this.addressInformation.Add((int) address, (RollCall.ChannelInformation) new RollCallSae.ChannelInformation(this, this.CreateConnectionResource(address, new int?()), this.GetIdentificationIds(new byte?(address))));
        if (this.debugLevel > (ushort) 1)
          this.RaiseDebugInfoEvent((int) address, "Adding (connection) channel");
      }
      else
      {
        RollCallSae.ChannelInformation channelInformation = (RollCallSae.ChannelInformation) this.addressInformation[(int) address];
        channelInformation.UpdateLastSeen();
        channel = channelInformation.Channel;
        if (channelInformation.Resource == null)
        {
          if (this.RequiresFunction(address))
          {
            if (channelInformation.TimeSinceFirstSeen > TimeSpan.FromSeconds(30.0))
              channelInformation.Resource = this.CreateConnectionResource(address, new int?(), true);
          }
        }
      }
    }
    this.HandleIncomingMessage(address, id, data, channel);
  }

  internal override Dictionary<string, TranslationEntry> Translations
  {
    get
    {
      if (this.translation == null)
        this.translation = new DiagnosisProtocol(this.Protocol.ToString()).Translations;
      return this.translation;
    }
  }

  public override void WriteTranslationFile(
    CultureInfo culture,
    IEnumerable<TranslationEntry> translations)
  {
    TranslationEntry.WriteTranslationFile(this.Protocol.ToString(), culture, string.Empty, translations, true);
  }

  private void InitializeSuspectParameters()
  {
    this.suspectParametersByAddress = new Dictionary<byte?, Dictionary<string, string>>();
    this.commonSuspectParameters = new Dictionary<string, string>();
    foreach (KeyValuePair<string, string> keyValuePair in this.Translations.Where<KeyValuePair<string, TranslationEntry>>((Func<KeyValuePair<string, TranslationEntry>, bool>) (q => q.Key.EndsWith(".SPN", StringComparison.Ordinal))).ToDictionary<KeyValuePair<string, TranslationEntry>, string, string>((Func<KeyValuePair<string, TranslationEntry>, string>) (k => k.Key), (Func<KeyValuePair<string, TranslationEntry>, string>) (v => v.Value.Translation)))
    {
      string[] strArray = keyValuePair.Key.Split(".".ToCharArray());
      string key1 = strArray[0];
      byte? key2 = new byte?();
      Dictionary<string, string> dictionary = this.commonSuspectParameters;
      byte result;
      if (strArray.Length > 2 && byte.TryParse(strArray[1], out result))
      {
        key2 = new byte?(result);
        if (!this.suspectParametersByAddress.TryGetValue(key2, out dictionary))
        {
          dictionary = new Dictionary<string, string>();
          this.suspectParametersByAddress[key2] = dictionary;
        }
      }
      if (!dictionary.ContainsKey(key1))
        dictionary.Add(key1, keyValuePair.Value);
    }
  }

  public override IDictionary<string, string> SuspectParameters
  {
    get
    {
      if (this.commonSuspectParameters == null)
        this.InitializeSuspectParameters();
      return (IDictionary<string, string>) this.commonSuspectParameters;
    }
  }

  public IDictionary<int, Tuple<string, string>> ParameterGroups
  {
    get
    {
      if (this.parameterGroups == null)
      {
        this.parameterGroups = new Dictionary<int, Tuple<string, string>>();
        Dictionary<string, string> dictionary = this.Translations.Where<KeyValuePair<string, TranslationEntry>>((Func<KeyValuePair<string, TranslationEntry>, bool>) (q => q.Key.EndsWith(".PGN", StringComparison.Ordinal) || q.Key.EndsWith(".Acronym", StringComparison.Ordinal))).ToDictionary<KeyValuePair<string, TranslationEntry>, string, string>((Func<KeyValuePair<string, TranslationEntry>, string>) (k => k.Key), (Func<KeyValuePair<string, TranslationEntry>, string>) (v => v.Value.Translation));
        foreach (KeyValuePair<string, string> keyValuePair in dictionary.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (q => q.Key.EndsWith(".PGN", StringComparison.Ordinal))))
        {
          string str1 = keyValuePair.Key.Split(".".ToCharArray())[0];
          string str2 = string.Empty;
          string str3;
          if (dictionary.TryGetValue(str1 + ".Acronym", out str3))
            str2 = str3;
          this.parameterGroups.Add(Convert.ToInt32(str1, (IFormatProvider) CultureInfo.InvariantCulture), new Tuple<string, string>(keyValuePair.Value, str2));
        }
      }
      return (IDictionary<int, Tuple<string, string>>) this.parameterGroups;
    }
  }

  public override IDictionary<int, string> ParameterGroupLabels
  {
    get
    {
      return this.ParameterGroups == null ? (IDictionary<int, string>) null : (IDictionary<int, string>) this.ParameterGroups.ToDictionary<KeyValuePair<int, Tuple<string, string>>, int, string>((Func<KeyValuePair<int, Tuple<string, string>>, int>) (k => k.Key), (Func<KeyValuePair<int, Tuple<string, string>>, string>) (v => v.Value.Item1));
    }
  }

  public override IDictionary<int, string> ParameterGroupAcronyms
  {
    get
    {
      return this.ParameterGroups == null ? (IDictionary<int, string>) null : (IDictionary<int, string>) this.ParameterGroups.ToDictionary<KeyValuePair<int, Tuple<string, string>>, int, string>((Func<KeyValuePair<int, Tuple<string, string>>, int>) (k => k.Key), (Func<KeyValuePair<int, Tuple<string, string>>, string>) (v => v.Value.Item2));
    }
  }

  internal override Dictionary<string, string> GetSuspectParametersForEcu(Ecu ecu)
  {
    if (this.suspectParametersByAddress == null)
      this.InitializeSuspectParameters();
    return ecu.SourceAddress.HasValue && this.suspectParametersByAddress.ContainsKey(ecu.SourceAddress) ? this.suspectParametersByAddress[ecu.SourceAddress] : (Dictionary<string, string>) null;
  }

  protected abstract bool TryExtractMessage(
    byte[] source,
    out byte address,
    out int id,
    out byte[] data);

  protected abstract void HandleIncomingMessage(
    byte address,
    int id,
    byte[] data,
    Channel channel);

  internal virtual bool IsRequestIdContentVisible(int id) => true;

  internal override void ReadEcuInfo(EcuInfo ecuInfo)
  {
    this.RequestAndWait(ecuInfo.MessageNumber.Value, ecuInfo.Channel.SourceAddress.Value);
  }

  protected abstract PassThruMsg CreateRequestMessage(int id, byte destinationAddress);

  private bool RequestId(int id, byte destinationAddress)
  {
    J2534Error j2534Error = this.Write(this.CreateRequestMessage(id, destinationAddress));
    if (j2534Error != J2534Error.NoError)
      Sapi.GetSapi().RaiseDebugInfoEvent((object) $"{(object) this.protocolId}-{(object) destinationAddress}", $"ID {(object) id}: Result from J2534.WriteMsgs is {j2534Error.ToString()} GetLastError is {Sid.GetLastError()}");
    return j2534Error == J2534Error.NoError;
  }

  protected J2534Error Write(PassThruMsg requestMessage)
  {
    return Sid.WriteMsgs(this.channelId, (IList<PassThruMsg>) new List<PassThruMsg>()
    {
      requestMessage
    }, 0U);
  }

  protected virtual byte[] RequestAndWait(int id, byte destinationAddress)
  {
    return this.RequestAndWait(new RollCallSae.QueueItem(this.CreateRequestMessage(id, destinationAddress), id, destinationAddress));
  }

  protected byte[] RequestAndWait(RollCallSae.QueueItem queueItem)
  {
    lock (this.queuedItemsLock)
      this.queuedItems.Add(queueItem);
    try
    {
      return queueItem.Request(this);
    }
    finally
    {
      lock (this.queuedItemsLock)
      {
        this.queuedItems.Remove(queueItem);
        queueItem.Dispose();
      }
    }
  }

  protected void NotifyQueueItem(
    int id,
    byte destinationAddress,
    byte[] response,
    RollCallSae.Acknowledgment acknowledgement)
  {
    RollCallSae.QueueItem queueItem = (RollCallSae.QueueItem) null;
    lock (this.queuedItemsLock)
    {
      if (this.queuedItems.Count > 0)
        queueItem = this.queuedItems.FirstOrDefault<RollCallSae.QueueItem>((Func<RollCallSae.QueueItem, bool>) (qi => qi.BelongsTo(id, destinationAddress, response)));
    }
    queueItem?.Notify(response, acknowledgement);
  }

  protected abstract int BetweenGlobalIdRequestInterval { get; }

  protected abstract uint BaudRate { get; }

  protected abstract int TotalMessagesPerSecond { get; }

  internal override int ChannelTimeout
  {
    get
    {
      if (this.channelTimeout == 0)
        this.channelTimeout = Math.Max(10000, this.CycleGlobalRequestIds.Count<int>() * this.BetweenGlobalIdRequestInterval) + 2000;
      return this.channelTimeout;
    }
  }

  protected abstract IEnumerable<RollCall.ID> GetIdentificationIds(byte? address);

  protected abstract bool RequiresFunction(byte address);

  protected abstract byte GlobalRequestAddress { get; }

  protected abstract IEnumerable<int> CycleGlobalRequestIds { get; }

  protected abstract int MapIdToRequestId(RollCall.ID id);

  protected override void RaiseDebugInfoEvent(int sourceAddress, string text)
  {
    string str = sourceAddress == (int) this.GlobalRequestAddress ? "(global)" : sourceAddress.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    Sapi.GetSapi().RaiseDebugInfoEvent((object) $"{(object) this.protocolId}-{str}", text);
  }

  protected override void DisposeInternal()
  {
    lock (this.queuedItemsLock)
    {
      foreach (RollCallSae.QueueItem queuedItem in this.queuedItems)
        queuedItem.Dispose();
      this.queuedItems.Clear();
    }
  }

  protected new sealed class ChannelInformation : RollCall.ChannelInformation, IDisposable
  {
    internal ChannelInformation(
      RollCallSae rollCallManager,
      ConnectionResource resource,
      IEnumerable<RollCall.ID> identificationIdentifiers)
      : base((RollCall) rollCallManager, resource, identificationIdentifiers.Select<RollCall.ID, RollCall.IdentificationInformation>((Func<RollCall.ID, RollCall.IdentificationInformation>) (id => new RollCall.IdentificationInformation(id))))
    {
    }

    protected override void ThreadFunc()
    {
      byte sourceAddress = this.Resource.SourceAddress.Value;
      int num1 = 0;
      IEnumerable<IGrouping<int, RollCall.IdentificationInformation>> source1 = this.Identification.GroupBy<RollCall.IdentificationInformation, int>((Func<RollCall.IdentificationInformation, int>) (id => ((RollCallSae) this.rollCallManager).MapIdToRequestId(id.Id)));
      if (source1.Any<IGrouping<int, RollCall.IdentificationInformation>>())
      {
        double num2 = (double) (100 / source1.Count<IGrouping<int, RollCall.IdentificationInformation>>());
        foreach (IGrouping<int, RollCall.IdentificationInformation> grouping in source1)
        {
          Sapi.GetSapi().Channels.RaiseConnectProgressEvent(this.Resource, (double) num1++ * num2 + num2 / 2.0);
          try
          {
            ((RollCallSae) this.rollCallManager).RequestAndWait(grouping.Key, sourceAddress);
          }
          catch (CaesarException ex)
          {
            switch (ex.ErrorNumber)
            {
              case 6607:
              case 6620:
                Sapi.GetSapi().Channels.RaiseConnectCompleteEvent((object) this.Resource, (Exception) ex);
                return;
              case 6623:
                using (IEnumerator<RollCall.IdentificationInformation> enumerator = grouping.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                    enumerator.Current.Value = (object) ex.Message;
                  continue;
                }
              default:
                continue;
            }
          }
        }
      }
      if (this.rollCallManager.debugLevel > (ushort) 1)
        this.rollCallManager.RaiseDebugInfoEvent((int) sourceAddress, "Create channel");
      int? function = new int?();
      RollCall.IdentificationInformation identificationInformation = this.Identification.FirstOrDefault<RollCall.IdentificationInformation>((Func<RollCall.IdentificationInformation, bool>) (i => i.Id == RollCall.ID.Function));
      if (identificationInformation != null && identificationInformation.Value != null)
        function = new int?((int) identificationInformation.Value);
      IEnumerable<Tuple<RollCall.ID, object>> readIdBlock = this.Identification.Where<RollCall.IdentificationInformation>((Func<RollCall.IdentificationInformation, bool>) (id => id.Value != null)).Select<RollCall.IdentificationInformation, Tuple<RollCall.ID, object>>((Func<RollCall.IdentificationInformation, Tuple<RollCall.ID, object>>) (id => new Tuple<RollCall.ID, object>(id.Id, id.Value)));
      IEnumerable<DiagnosisVariant> source2 = this.rollCallManager.Ecus.Where<Ecu>((Func<Ecu, bool>) (e =>
      {
        byte? sourceAddress1 = e.SourceAddress;
        int? nullable1 = sourceAddress1.HasValue ? new int?((int) sourceAddress1.GetValueOrDefault()) : new int?();
        int num3 = (int) sourceAddress;
        int? nullable2;
        if ((nullable1.GetValueOrDefault() == num3 ? (nullable1.HasValue ? 1 : 0) : 0) != 0)
        {
          nullable1 = e.Function;
          nullable2 = function;
          if ((nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? (nullable1.HasValue == nullable2.HasValue ? 1 : 0) : 0) != 0)
            goto label_12;
        }
        byte? sourceAddress2 = e.SourceAddress;
        int? nullable3;
        if (!sourceAddress2.HasValue)
        {
          nullable1 = new int?();
          nullable3 = nullable1;
        }
        else
          nullable3 = new int?((int) sourceAddress2.GetValueOrDefault());
        nullable2 = nullable3;
        int num4 = (int) sourceAddress;
        if ((nullable2.GetValueOrDefault() == num4 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
        {
          nullable2 = e.Function;
          if (!nullable2.HasValue)
            goto label_12;
        }
        if (e.SourceAddress.HasValue)
          return false;
        nullable2 = e.Function;
        nullable1 = function;
        return nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() && nullable2.HasValue == nullable1.HasValue;
label_12:
        return true;
      })).SelectMany<Ecu, DiagnosisVariant>((Func<Ecu, IEnumerable<DiagnosisVariant>>) (e => e.DiagnosisVariants.Where<DiagnosisVariant>((Func<DiagnosisVariant, bool>) (v => v.IsMatch(readIdBlock)))));
      DiagnosisVariant variant;
      if (source2.Any<DiagnosisVariant>())
      {
        variant = source2.OrderBy<DiagnosisVariant, int>((Func<DiagnosisVariant, int>) (v => v.RollCallIdentificationCount)).ThenBy<DiagnosisVariant, bool>((Func<DiagnosisVariant, bool>) (v => !v.Ecu.IsRollCallBaseEcu)).Last<DiagnosisVariant>();
        this.resource = new ConnectionResource(variant.Ecu, this.rollCallManager.Protocol, this.Resource.HardwareName, (uint) this.Resource.BaudRate, 1, (int) sourceAddress);
      }
      else
        variant = this.Resource.Ecu.DiagnosisVariants["ROLLCALL"];
      if (this.Connect(variant))
        return;
      Sapi.GetSapi().Channels.RaiseConnectCompleteEvent((object) this.Resource, (Exception) new CaesarException(SapiError.DeviceInvalid));
    }
  }

  protected delegate void IdentificationAction(RollCall.IdentificationInformation identfication);

  protected enum Acknowledgment : uint
  {
    Positive,
    Negative,
    AccessDenied,
    Busy,
  }

  internal class MonitorData
  {
    private const string Base = "BASE";
    public Dictionary<string, XElement> definitions = new Dictionary<string, XElement>();
    public Dictionary<Tuple<string, string>, XElement> baseDefinitions = new Dictionary<Tuple<string, string>, XElement>();

    private XElement GetEcuContent(Ecu ecu)
    {
      if (ecu.Function.HasValue)
      {
        string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "F{0}", (object) ecu.Function.Value);
        if (this.definitions.ContainsKey(key))
          return this.definitions[key];
      }
      if (ecu.SourceAddress.HasValue)
      {
        string key = ecu.SourceAddress.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (this.definitions.ContainsKey(key))
          return this.definitions[key];
      }
      return (XElement) null;
    }

    public XElement GetBaseDefinition(string qualifier, Type type)
    {
      XElement baseDefinition;
      if (!this.baseDefinitions.TryGetValue(new Tuple<string, string>(type.Name, qualifier), out baseDefinition))
        baseDefinition = (XElement) null;
      return baseDefinition;
    }

    public IEnumerable<XElement> GetDefinitions(Channel channel, Type type)
    {
      XElement typedEcuContent = this.GetEcuContent(channel.Ecu)?.Element((XName) (type.Name + "s"));
      foreach (string serviceQualifier in channel.DiagnosisVariant.DiagServiceQualifiers)
      {
        string qualifier = serviceQualifier;
        XElement definition = (XElement) null;
        if (typedEcuContent != null)
          definition = typedEcuContent.Elements((XName) type.Name).FirstOrDefault<XElement>((Func<XElement, bool>) (ac => ac.Attribute((XName) "qualifier").Value == qualifier));
        if (definition == null)
          definition = this.GetBaseDefinition(qualifier, type);
        if (definition != null)
          yield return definition;
      }
    }

    private static IEnumerable<Tuple<RollCall.ID, string>> GetIdBlock(XElement variantElement)
    {
      XElement xelement = variantElement.Element((XName) "Identifications");
      if (xelement != null)
      {
        foreach (XElement element in xelement.Elements((XName) "Identification"))
          yield return new Tuple<RollCall.ID, string>((RollCall.ID) Enum.Parse(typeof (RollCall.ID), element.Attribute((XName) "qualifier").Value, true), element.Value);
      }
    }

    private static IEnumerable<DiagnosisVariant> GetVariants(XElement ecuElement, Ecu ecu)
    {
      foreach (XElement element in ecuElement.Element((XName) "Variants").Elements((XName) "Variant"))
      {
        string name = element.GetAttribute("name");
        IEnumerable<Tuple<RollCall.ID, string>> idBlock = RollCallSae.MonitorData.GetIdBlock(element);
        if (idBlock.Any<Tuple<RollCall.ID, string>>())
        {
          switch (name)
          {
            case null:
              name = !ecu.IsRollCallBaseEcu ? ecu.Name : throw new InvalidOperationException($"Diagnostic data for base Ecu '{ecu.Name}' contains an ID block but doesn't specify a Variant name. A Variant name is required.");
              break;
            case "ROLLCALL":
              throw new InvalidOperationException($"Diagnostic data for '{ecu.Name}' invalidly specifies the reserved Variant name '{name}'.");
            default:
              if (!(name == ecu.Name))
                break;
              goto case "ROLLCALL";
          }
        }
        else
          name = name == null ? "ROLLCALL" : throw new InvalidOperationException($"Diagnostic data for Ecu '{ecu.Name}' invalidly specifies a name '{name}' for a Variant that has no ID block data. S-API is responsible for assigning the name for such Variants.");
        yield return new DiagnosisVariant(ecu, name, element.GetAttribute("equipment"), idBlock, element.Element((XName) "References").Elements((XName) "Reference").Select<XElement, string>((Func<XElement, string>) (i => i.Attribute((XName) "qualifier").Value)));
      }
    }

    public IEnumerable<Ecu> GetEcus(RollCallSae manager)
    {
      foreach (XElement xelement in this.definitions.Select<KeyValuePair<string, XElement>, XElement>((Func<KeyValuePair<string, XElement>, XElement>) (p => p.Value)))
      {
        XAttribute xattribute1 = xelement.Attribute((XName) "address");
        byte? address = xattribute1 != null ? new byte?(Convert.ToByte(xattribute1.Value, (IFormatProvider) CultureInfo.InvariantCulture)) : new byte?();
        IEnumerable<byte> alternateAddresses = RollCallSae.MonitorData.GetAlternateAddresses(xelement);
        XAttribute xattribute2 = xelement.Attribute((XName) "function");
        byte? function = xattribute2 != null ? new byte?(Convert.ToByte(xattribute2.Value, (IFormatProvider) CultureInfo.InvariantCulture)) : new byte?();
        string version = xelement.GetAttribute("version") ?? string.Empty;
        XAttribute xattribute3 = xelement.Attribute((XName) "otherProtocolAddress");
        byte? otherProtocolAddress = xattribute3 != null ? new byte?(Convert.ToByte(xattribute3.Value, (IFormatProvider) CultureInfo.InvariantCulture)) : new byte?();
        foreach (XElement ecuElement in xelement.Element((XName) "Ecus").Elements((XName) "Ecu"))
        {
          string name = ecuElement.GetAttribute("name");
          string description = ecuElement.GetAttribute("description");
          string category = ecuElement.GetAttribute("category");
          string family = ecuElement.GetAttribute("family");
          string supportedEquipment = ecuElement.GetAttribute("supportedEquipment");
          if (!alternateAddresses.Any<byte>())
          {
            byte? nullable1 = address;
            int? sourceAddress = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
            byte? nullable2 = function;
            int? function1 = nullable2.HasValue ? new int?((int) nullable2.GetValueOrDefault()) : new int?();
            string ecuName = name;
            RollCallSae rollCallManager = manager;
            string shortDescription = description;
            string category1 = category;
            string family1 = family;
            string supportedEquipment1 = supportedEquipment;
            byte? otherProtocolAddress1 = otherProtocolAddress;
            Ecu ecu = new Ecu(sourceAddress, function1, ecuName, DiagnosisSource.RollCallDatabase, (RollCall) rollCallManager, shortDescription, category1, family1, supportedEquipment1, otherProtocolAddress1);
            ecu.AcquireFromRollCall(version, RollCallSae.MonitorData.GetVariants(ecuElement, ecu));
            yield return ecu;
          }
          else
          {
            int index = 1;
            foreach (int num in Enumerable.Repeat<byte>(address.Value, 1).Union<byte>(alternateAddresses))
            {
              string str1 = name != null ? $"{name}-{(object) index}" : (string) null;
              string str2 = description != null ? $"{description} {(object) index}" : (string) null;
              int? sourceAddress = new int?(num);
              byte? nullable = function;
              int? function2 = nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?();
              string ecuName = str1;
              RollCallSae rollCallManager = manager;
              string shortDescription = str2;
              string category2 = category;
              string family2 = family;
              string supportedEquipment2 = supportedEquipment;
              nullable = new byte?();
              byte? otherProtocolAddress2 = nullable;
              Ecu ecu = new Ecu(sourceAddress, function2, ecuName, DiagnosisSource.RollCallDatabase, (RollCall) rollCallManager, shortDescription, category2, family2, supportedEquipment2, otherProtocolAddress2);
              ecu.AcquireFromRollCall(version, RollCallSae.MonitorData.GetVariants(ecuElement, ecu));
              yield return ecu;
              ++index;
            }
          }
          name = (string) null;
          description = (string) null;
          category = (string) null;
          family = (string) null;
          supportedEquipment = (string) null;
        }
        address = new byte?();
        alternateAddresses = (IEnumerable<byte>) null;
        function = new byte?();
        version = (string) null;
        otherProtocolAddress = new byte?();
      }
    }

    private static IEnumerable<byte> GetAlternateAddresses(XElement definition)
    {
      int i = 1;
      while (true)
      {
        XAttribute xattribute = definition.Attribute((XName) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "alternateAddress{0}", (object) i++));
        if (xattribute != null)
          yield return Convert.ToByte(xattribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
        else
          break;
      }
    }

    public static RollCallSae.MonitorData Load(RollCallSae manager)
    {
      RollCallSae.MonitorData monitorData = new RollCallSae.MonitorData();
      string path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}monitor.sbf", (object) Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value, (object) manager.Protocol.ToString());
      if (File.Exists(path))
      {
        using (Stream stream = (Stream) new FileStream(path, FileMode.Open, FileAccess.Read))
        {
          using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress))
          {
            foreach (XElement element in XDocument.Load((Stream) gzipStream).Element((XName) "Content").Element((XName) "Sources").Elements((XName) "Source"))
            {
              XAttribute xattribute1 = element.Attribute((XName) "address");
              string key = xattribute1 != null ? xattribute1.Value : string.Empty;
              if (key == "BASE")
              {
                foreach (XElement xelement in element.Elements().SelectMany<XElement, XElement>((Func<XElement, IEnumerable<XElement>>) (t => t.Elements())))
                  monitorData.baseDefinitions.Add(new Tuple<string, string>(xelement.Name.ToString(), xelement.Attribute((XName) "qualifier").Value), xelement);
              }
              else
              {
                XAttribute xattribute2 = element.Attribute((XName) "function");
                if (xattribute2 != null)
                  monitorData.definitions.Add("F" + xattribute2.Value, element);
                else
                  monitorData.definitions.Add(key, element);
              }
            }
          }
        }
      }
      return monitorData;
    }

    private static bool IsSourceElementForAddress(XElement element, string address)
    {
      XAttribute xattribute = element.Attribute((XName) nameof (address));
      return xattribute != null && xattribute.Value == address || RollCallSae.MonitorData.GetAlternateAddresses(element).Contains<byte>(Convert.ToByte(address, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static void SaveDataItemDescription(
      Protocol protocol,
      byte address,
      Type type,
      string qualifier,
      IDictionary<string, string> content)
    {
      string path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}monitor.sbf", (object) Sapi.GetSapi().ConfigurationItems["CBFFiles"].Value, (object) protocol.ToString());
      XDocument xdocument;
      using (Stream stream = (Stream) new FileStream(path, FileMode.Open, FileAccess.Read))
      {
        using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress))
        {
          xdocument = XDocument.Load((Stream) gzipStream);
          XElement xelement1 = xdocument.Element((XName) "Content").Element((XName) "Sources");
          XElement content1 = (XElement) null;
          XElement content2 = (XElement) null;
          string sourceAddressString = address.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          XElement content3 = xelement1.Elements((XName) "Source").FirstOrDefault<XElement>((Func<XElement, bool>) (v => RollCallSae.MonitorData.IsSourceElementForAddress(v, sourceAddressString)));
          XElement xelement2;
          if (content3 == null)
          {
            content3 = new XElement((XName) "Source", (object) new XAttribute((XName) nameof (address), (object) sourceAddressString));
            xelement1.Add((object) content3);
            xelement2 = new XElement((XName) "Ecus");
            content3.Add((object) new XAttribute((XName) "version", (object) "1"), (object) xelement2);
          }
          else
          {
            xelement2 = content3.Element((XName) "Ecus");
            content1 = content3.Element((XName) "Services");
            content2 = content3.Element((XName) "Instruments");
            XAttribute xattribute = content3.Attribute((XName) "version");
            if (xattribute != null)
            {
              int int32 = Convert.ToInt32(xattribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
              xattribute.Value = (int32 + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
            }
            else
              content3.Add((object) new XAttribute((XName) "version", (object) "1"));
          }
          bool flag1 = false;
          XElement xelement3;
          string name;
          if (type == typeof (Instrument))
          {
            if (Regex.Match(qualifier, "DT_[0-9]*").Value == qualifier)
            {
              XElement xelement4 = xelement1.Elements((XName) "Source").FirstOrDefault<XElement>((Func<XElement, bool>) (v => v.Attribute((XName) nameof (address)) != null && v.Attribute((XName) nameof (address)).Value == "BASE"));
              if (xelement4 != null)
              {
                content2 = xelement4.Element((XName) "Instruments");
                flag1 = true;
              }
            }
            if (content2 == null)
            {
              content2 = new XElement((XName) "Instruments");
              content3.Add((object) content2);
            }
            xelement3 = content2;
            name = "Instrument";
          }
          else
          {
            if (content1 == null)
            {
              content1 = new XElement((XName) "Services");
              content3.Add((object) content1);
            }
            xelement3 = content1;
            name = "Service";
          }
          IEnumerable<XElement> content4 = content.Select<KeyValuePair<string, string>, XElement>((Func<KeyValuePair<string, string>, XElement>) (c => new XElement((XName) "Property", new object[2]
          {
            (object) new XAttribute((XName) "name", (object) c.Key),
            (object) c.Value
          })));
          XElement xelement5 = xelement3.Elements((XName) name).FirstOrDefault<XElement>((Func<XElement, bool>) (xe => xe.Attribute((XName) nameof (qualifier)).Value == qualifier));
          if (xelement5 != null)
          {
            xelement5.RemoveNodes();
            xelement5.Add((object) content4);
          }
          else
          {
            bool flag2 = false;
            if (flag1)
            {
              int int32 = Convert.ToInt32(qualifier.Substring(3), (IFormatProvider) CultureInfo.InvariantCulture);
              foreach (XElement element in xelement3.Elements())
              {
                string str = element.Attribute((XName) nameof (qualifier)) != null ? element.Attribute((XName) nameof (qualifier)).Value : (string) null;
                int result;
                if (str != null && str.StartsWith("DT_", StringComparison.Ordinal) && int.TryParse(str.Substring(3), out result) && result > int32)
                {
                  element.AddBeforeSelf((object) new XElement((XName) name, new object[2]
                  {
                    (object) new XAttribute((XName) nameof (qualifier), (object) qualifier),
                    (object) content4
                  }));
                  flag2 = true;
                  break;
                }
              }
            }
            if (!flag2)
              xelement3.Add((object) new XElement((XName) name, new object[2]
              {
                (object) new XAttribute((XName) nameof (qualifier), (object) qualifier),
                (object) content4
              }));
          }
          if (xelement2.Elements().Count<XElement>() == 0)
            xelement2.Add((object) new XElement((XName) "Ecu", (object) new XElement((XName) "Variants")));
          foreach (XContainer element1 in xelement2.Elements())
          {
            XElement xelement6 = element1.Element((XName) "Variants");
            if (xelement6.Elements().Count<XElement>() == 0)
              xelement6.Add((object) new XElement((XName) "Variant", (object) new XElement((XName) "References")));
            foreach (XContainer element2 in xelement6.Elements())
              element2.Element((XName) "References").Add((object) new XElement((XName) "Reference", (object) new XAttribute((XName) nameof (qualifier), (object) qualifier)));
          }
        }
      }
      using (Stream stream = (Stream) new FileStream(path, FileMode.Create))
      {
        using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Compress))
          xdocument.Save((Stream) gzipStream);
      }
      RollCall.GetManager(protocol).ClearEcus();
      Sapi.GetSapi().Ecus.ClearList();
    }
  }

  protected class QueueItem : IDisposable
  {
    private readonly Predicate<byte[]> additionalResponseCheck;
    private readonly int retryCount;
    private readonly int synchronousRequestTimeout;
    private readonly int busyRetryCount;
    private readonly int betweenBusyRequestInterval;
    private object eventLock = new object();
    private bool disposed;

    internal QueueItem(PassThruMsg requestMessage, int responseId, byte destinationAddress)
      : this(requestMessage, responseId, destinationAddress, (Predicate<byte[]>) null, 3, 2000, 5, 1000)
    {
    }

    internal QueueItem(
      PassThruMsg requestMessage,
      int responseId,
      byte destinationAddress,
      Predicate<byte[]> additionalResponseCheck,
      int retryCount,
      int synchronousRequestTimeout,
      int busyRetryCount,
      int betweenBusyRequestInterval)
    {
      this.RequestMessage = requestMessage;
      this.ResponseId = responseId;
      this.DestinationAddress = destinationAddress;
      this.Event = new ManualResetEvent(false);
      this.additionalResponseCheck = additionalResponseCheck;
      this.retryCount = retryCount;
      this.synchronousRequestTimeout = synchronousRequestTimeout;
      this.busyRetryCount = busyRetryCount;
      this.betweenBusyRequestInterval = betweenBusyRequestInterval;
    }

    private PassThruMsg RequestMessage { get; set; }

    private int ResponseId { get; set; }

    private byte DestinationAddress { get; set; }

    private ManualResetEvent Event { get; set; }

    private RollCallSae.Acknowledgment Acknowledgement { get; set; }

    private byte[] Response { get; set; }

    internal byte[] Request(RollCallSae rollCallManager)
    {
      int num1 = 0;
      int num2 = this.retryCount;
      CaesarException caesarException = (CaesarException) null;
      ManualResetEvent closingEvent = rollCallManager.closingEvent;
      while (num1++ < num2)
      {
        if (closingEvent == null || !rollCallManager.ProtocolAlive)
          throw new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
        this.Event.Reset();
        if (rollCallManager.debugLevel > (ushort) 0)
          rollCallManager.RaiseDebugInfoEvent((int) this.DestinationAddress, $"ID {(object) this.ResponseId}: synchronous request (attempt {(object) num1})");
        J2534Error j2534Error = rollCallManager.Write(this.RequestMessage);
        if (j2534Error == J2534Error.NoError)
        {
          switch (WaitHandle.WaitAny(new WaitHandle[2]
          {
            (WaitHandle) closingEvent,
            (WaitHandle) this.Event
          }, this.synchronousRequestTimeout))
          {
            case 0:
              throw new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
            case 1:
              if (rollCallManager.debugLevel > (ushort) 0)
                rollCallManager.RaiseDebugInfoEvent((int) this.DestinationAddress, $"ID {(object) this.ResponseId}: synchronous response {(object) this.Acknowledgement}");
              switch (this.Acknowledgement)
              {
                case RollCallSae.Acknowledgment.Positive:
                  return this.Response;
                case RollCallSae.Acknowledgment.Negative:
                case RollCallSae.Acknowledgment.AccessDenied:
                  throw new CaesarException(SapiError.NegativeResponseMessageFromDevice);
                case RollCallSae.Acknowledgment.Busy:
                  caesarException = new CaesarException(SapiError.BusyResponseMessageFromDevice);
                  if (closingEvent.WaitOne(this.betweenBusyRequestInterval))
                    throw new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
                  num2 = this.busyRetryCount;
                  continue;
                default:
                  continue;
              }
            case 258:
              if (rollCallManager.debugLevel > (ushort) 0)
                rollCallManager.RaiseDebugInfoEvent((int) this.DestinationAddress, $"ID {(object) this.ResponseId}: synchronous timeout");
              caesarException = new CaesarException(SapiError.TimeoutReceivingMessageFromDevice);
              continue;
            default:
              continue;
          }
        }
        else
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) $"{(object) rollCallManager.Protocol}-{(object) this.DestinationAddress}", $"ID {(object) this.ResponseId}: Result from J2534.WriteMsgs is {j2534Error.ToString()} GetLastError is {Sid.GetLastError()}");
          throw new CaesarException(SapiError.CannotSendMessageToDevice);
        }
      }
      throw caesarException;
    }

    internal bool BelongsTo(int id, byte destinationAddress, byte[] response)
    {
      if ((int) this.DestinationAddress != (int) destinationAddress || this.ResponseId != id)
        return false;
      return response == null || this.additionalResponseCheck == null || this.additionalResponseCheck(response);
    }

    internal void Notify(byte[] response, RollCallSae.Acknowledgment acknowledgment)
    {
      this.Response = response;
      this.Acknowledgement = acknowledgment;
      lock (this.eventLock)
      {
        if (this.disposed || this.Event == null)
          return;
        this.Event.Set();
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      this.disposed = true;
      if (!disposing)
        return;
      lock (this.eventLock)
      {
        if (this.Event == null)
          return;
        this.Event.Set();
        this.Event.Close();
        this.Event = (ManualResetEvent) null;
      }
    }
  }
}
