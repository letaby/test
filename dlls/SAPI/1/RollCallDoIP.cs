// Decompiled with JetBrains decompiler
// Type: SapiLayer1.RollCallDoIP
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using J2534;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

#nullable disable
namespace SapiLayer1;

internal sealed class RollCallDoIP : RollCall, IDisposable, IRollCall
{
  private static RollCallDoIP globalInstance = new RollCallDoIP();
  private uint activationChannelId;
  private uint activationFilterId;
  private bool activationSupported;
  private const int ActivateOperationalStatusTimeout = 10000;
  private static Regex entityRegex = new Regex("(?<name>([A-Z]|[a-z])+)(?<index>\\d+)='(?<value>[^']*)'", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
  private static Regex countRegex = new Regex("Count='(?<count>\\d+)'", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
  private static Regex detectionRegex = new Regex("(?<name>([A-Z]|_|[a-z])+)='(?<value>[^']*)'", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
  private const int ConnectionRetryInterval = 10000;
  private const int TcpPort = 13400;
  private int mcdEthernetDetectionTimeout;
  private string detectionString;

  internal static RollCallDoIP GlobalInstance => RollCallDoIP.globalInstance;

  private RollCallDoIP()
    : base(Protocol.DoIP)
  {
  }

  internal override void Init()
  {
    Sapi sapi = Sapi.GetSapi();
    if (Sid.Connect((ProtocolId) (this.protocolId | (Protocol) 268435456 /*0x10000000*/), 0U, ref this.activationChannelId) == J2534Error.NoError && this.activationChannelId != 0U)
    {
      string libraryName = (string) null;
      string libraryVersion = (string) null;
      string driverVersion = (string) null;
      string firmwareVersion = (string) null;
      string supportedProtocols = (string) null;
      Sid.GetDeviceVersionInfo(this.activationChannelId, ref libraryName, ref libraryVersion, ref driverVersion, ref firmwareVersion, ref supportedProtocols);
      if (supportedProtocols != null && supportedProtocols.IndexOf("ETH", StringComparison.Ordinal) != -1)
      {
        this.activationSupported = true;
        this.DeviceName = Sid.GetDeviceName(this.activationChannelId);
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "RP1210 ethernet interface is " + this.DeviceName);
        this.ActivateInterface(false);
      }
      else
        sapi.RaiseDebugInfoEvent((object) this.protocolId, "RP1210 ethernet activation not supported by " + libraryName);
    }
    if (McdRoot.Initialized)
    {
      if (McdRoot.IsVciAccessLayerPrepared)
      {
        if (McdRoot.CurrentInterfaces.Any<McdInterface>((Func<McdInterface, bool>) (i => i.Resources.Any<McdInterfaceResource>((Func<McdInterfaceResource, bool>) (r => r.IsEthernet)))))
        {
          this.detectionString = sapi.ConfigurationItems["McdEthernetDetectionString"].Value;
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          foreach (Match match in RollCallDoIP.detectionRegex.Matches(this.detectionString))
          {
            string key = match.Groups["name"].Value;
            string str = match.Groups["value"].Value;
            dictionary[key] = str;
            sapi.RaiseDebugInfoEvent((object) this.protocolId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DoIP roll-call detection option: {0}='{1}'", (object) key, (object) str));
          }
          string s;
          if (dictionary.TryGetValue("VehicleDiscoveryTime", out s) && int.TryParse(s, out this.mcdEthernetDetectionTimeout))
            this.State = ConnectionState.Initialized;
          else
            sapi.RaiseDebugInfoEvent((object) this.protocolId, "DoIP roll-call services are unavailable: unable to parse VehicleDiscoveryTime from McdEthernetDetectionString.");
        }
        else
          sapi.RaiseDebugInfoEvent((object) this.protocolId, "DoIP roll-call services are unavailable: no ethernet interfaces are available. CHeck DoIP PDU-API installation.");
      }
      else
        sapi.RaiseDebugInfoEvent((object) this.protocolId, "DoIP roll-call services are unavailable: the MVCI kernel VCI access layer is not prepared.");
    }
    else
      sapi.RaiseDebugInfoEvent((object) this.protocolId, "DoIP roll-call services are unavailable: the MVCI kernel is not initialized.");
  }

  public string EthernetGuid { private set; get; }

  internal void ActivateInterface(bool waitForOnline)
  {
    if (this.activationSupported && this.activationChannelId != 0U)
    {
      if (this.activationFilterId != 0U && !string.IsNullOrEmpty(this.EthernetGuid) && !((IEnumerable<NetworkInterface>) NetworkInterface.GetAllNetworkInterfaces()).Any<NetworkInterface>((Func<NetworkInterface, bool>) (ni => ni.Id.Equals(this.EthernetGuid, StringComparison.OrdinalIgnoreCase))))
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, this.EthernetGuid + " is no longer connected");
        this.StopActivationFilter();
      }
      if (this.activationFilterId == 0U)
      {
        Tuple<byte[], byte[]> tuple = new Tuple<byte[], byte[]>(new byte[6], new byte[6]);
        J2534Error j2534Error = Sid.StartMsgFilter(this.activationChannelId, FilterType.Pass, new PassThruMsg((ProtocolId) this.protocolId, tuple.Item1), new PassThruMsg((ProtocolId) this.protocolId, tuple.Item2), (PassThruMsg) null, ref this.activationFilterId);
        if (j2534Error != J2534Error.NoError)
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, $"Result from J2534.StartMsgFilter() is {j2534Error.ToString()} GetLastError is {Sid.GetLastError()}");
        }
        else
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "Started RP1210 ethernet activation");
          string guid = (string) null;
          if (Sid.GetEthernetGuid(this.activationChannelId, ref guid) == J2534Error.NoError)
          {
            this.EthernetGuid = guid;
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "RP1210 ethernet activation GUID is " + this.EthernetGuid);
          }
        }
      }
    }
    if (!waitForOnline || !this.activationSupported || this.activationFilterId == 0U || string.IsNullOrEmpty(this.EthernetGuid))
      return;
    DateTime now = Sapi.Now;
    while ((Sapi.Now - now).TotalMilliseconds < 10000.0)
    {
      NetworkInterface networkInterface = ((IEnumerable<NetworkInterface>) NetworkInterface.GetAllNetworkInterfaces()).FirstOrDefault<NetworkInterface>((Func<NetworkInterface, bool>) (ni => ni.Id.Equals(this.EthernetGuid, StringComparison.OrdinalIgnoreCase)));
      if (networkInterface != null && networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.GetIPProperties().UnicastAddresses.Count > 1)
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, $"{this.EthernetGuid} ({networkInterface.Description}) is ready");
        Thread.Sleep(2000);
        break;
      }
    }
  }

  private void StopActivationFilter()
  {
    Sapi sapi = Sapi.GetSapi();
    if (this.activationFilterId == 0U)
      return;
    J2534Error j2534Error = Sid.StopMsgFilter(this.activationChannelId, this.activationFilterId);
    if (j2534Error != J2534Error.NoError)
      sapi.RaiseDebugInfoEvent((object) this.protocolId, $"Result from J2534.StopMsgFilter() is {j2534Error.ToString()} GetLastError is {Sid.GetLastError()}");
    else
      sapi.RaiseDebugInfoEvent((object) this.protocolId, "Stopped RP1210 ethernet activation");
    this.activationFilterId = 0U;
    this.EthernetGuid = (string) null;
  }

  internal override void Exit()
  {
    if (this.activationChannelId != 0U)
    {
      this.StopActivationFilter();
      J2534Error j2534Error = Sid.Disconnect(this.activationChannelId);
      if (j2534Error != J2534Error.NoError)
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, $"Result from J2534.Disconnect for channel {(object) this.activationChannelId} is {j2534Error.ToString()}");
      this.activationChannelId = 0U;
      this.activationSupported = false;
    }
    this.State = ConnectionState.NotInitialized;
    this.ClearEcus();
  }

  public static IEnumerable<Dictionary<string, string>> GetEntities(string entityString)
  {
    int num = int.Parse(RollCallDoIP.countRegex.Match(entityString).Groups["count"].Value, (IFormatProvider) CultureInfo.InvariantCulture);
    List<Dictionary<string, string>> entities = new List<Dictionary<string, string>>();
    for (int index = 0; index < num; ++index)
      entities.Add(new Dictionary<string, string>());
    foreach (Match match in RollCallDoIP.entityRegex.Matches(entityString))
      entities[int.Parse(match.Groups["index"].Value, (IFormatProvider) CultureInfo.InvariantCulture)][match.Groups["name"].Value] = match.Groups["value"].Value;
    return (IEnumerable<Dictionary<string, string>>) entities;
  }

  private static bool IsPortOpen(string host, int port, TimeSpan timeout)
  {
    using (TcpClient tcpClient = new TcpClient())
    {
      try
      {
        return tcpClient.ConnectAsync(host, port).Wait(timeout) && tcpClient.Connected;
      }
      catch (AggregateException ex)
      {
        return false;
      }
      finally
      {
        tcpClient.Close();
      }
    }
  }

  protected override void ThreadFunc()
  {
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this.protocolId, "DoIP detection string: " + this.detectionString);
    do
    {
      lock (this.addressInformation)
        this.State = this.addressInformation.Any<KeyValuePair<int, RollCall.ChannelInformation>>() ? ConnectionState.ChannelsConnected : ConnectionState.ChannelsConnecting;
      this.ActivateInterface(true);
      McdRoot.DetectInterfaces(this.detectionString);
      foreach (McdInterface mcdInterface in McdRoot.CurrentInterfaces.Where<McdInterface>((Func<McdInterface, bool>) (i => i.Resources.Any<McdInterfaceResource>((Func<McdInterfaceResource, bool>) (r => r.IsEthernet)))))
      {
        foreach (Dictionary<string, string> entity in RollCallDoIP.GetEntities(mcdInterface.Name))
        {
          string address;
          string host;
          int sourceAddress;
          if (this.ConnectEnabled && entity.TryGetValue("LA", out address) && entity.TryGetValue("IP", out host) && address.TryParseSourceAddress(out sourceAddress))
          {
            bool flag = RollCallDoIP.IsPortOpen(host, 13400, TimeSpan.FromMilliseconds(500.0));
            lock (this.addressInformation)
            {
              RollCall.ChannelInformation channelInformation;
              if (!this.addressInformation.TryGetValue(sourceAddress, out channelInformation))
              {
                if (flag)
                  this.addressInformation.Add(sourceAddress, (RollCall.ChannelInformation) new RollCallDoIP.ChannelInformation(this, this.CreateConnectionResource(sourceAddress, mcdInterface.HardwareName), entity));
              }
              else if (flag)
                channelInformation.UpdateLastSeen();
              else
                this.RaiseDebugInfoEvent(sourceAddress, host + " is no longer connected; the roll-call channel will be removed");
            }
          }
        }
      }
      lock (this.addressInformation)
        this.State = this.addressInformation.Any<KeyValuePair<int, RollCall.ChannelInformation>>() ? ConnectionState.ChannelsConnected : ConnectionState.TranslatorConnected;
    }
    while (!this.closingEvent.WaitOne(10000));
  }

  private ConnectionResource CreateConnectionResource(int sourceAddress, string hardwareName)
  {
    return new ConnectionResource(this.GetEcu(sourceAddress, new int?()), Protocol.DoIP, hardwareName, 0U, 0, sourceAddress);
  }

  internal override void CreateEcuInfos(EcuInfoCollection ecuInfos)
  {
    if (ecuInfos.Channel.Online)
    {
      lock (this.addressInformation)
      {
        RollCall.ChannelInformation channelInformation;
        if (!this.addressInformation.TryGetValue(ecuInfos.Channel.SourceAddressLong.Value, out channelInformation))
          return;
        channelInformation.CreateEcuInfos(ecuInfos);
      }
    }
    else
    {
      if (ecuInfos.Channel.LogFile != null)
        return;
      string[] strArray = new string[5]
      {
        "LA",
        "EID",
        "IP",
        "VIN",
        "GroupID"
      };
      foreach (string qualifier in strArray)
        this.CreateEcuInfo(ecuInfos, qualifier);
    }
  }

  internal override EcuInfo CreateEcuInfo(EcuInfoCollection ecuInfos, string qualifier)
  {
    EcuInfo ecuInfo = new EcuInfo(ecuInfos.Channel, EcuInfoType.RollCall, qualifier, qualifier, "Common", "Rollcall/Common", (Presentation) null, 0, true, true, true);
    ecuInfos.AddFromRollCall(ecuInfo);
    return ecuInfo;
  }

  protected override TimeSpan RollCallValidLastSeenTime
  {
    get => TimeSpan.FromMilliseconds((double) (10000 + this.mcdEthernetDetectionTimeout + 1000));
  }

  internal override int ChannelTimeout => 20000;

  private new sealed class ChannelInformation(
    RollCallDoIP rollCallManager,
    ConnectionResource resource,
    Dictionary<string, string> identification) : RollCall.ChannelInformation((RollCall) rollCallManager, resource, Enumerable.Select<KeyValuePair<string, string>, RollCall.IdentificationInformation>(identification, (Func<KeyValuePair<string, string>, RollCall.IdentificationInformation>) (kv => new RollCall.IdentificationInformation(kv.Key, kv.Value))))
  {
    protected override void ThreadFunc()
    {
      if (this.Connect(this.Resource.Ecu.DiagnosisVariants["ROLLCALL"]))
        return;
      Sapi.GetSapi().Channels.RaiseConnectCompleteEvent((object) this.Resource, (Exception) new CaesarException(SapiError.DeviceInvalid));
    }
  }
}
