using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using J2534;
using McdAbstraction;

namespace SapiLayer1;

internal sealed class RollCallDoIP : RollCall, IDisposable, IRollCall
{
	private new sealed class ChannelInformation : RollCall.ChannelInformation
	{
		public ChannelInformation(RollCallDoIP rollCallManager, ConnectionResource resource, Dictionary<string, string> identification)
			: base(rollCallManager, resource, identification.Select((KeyValuePair<string, string> kv) => new IdentificationInformation(kv.Key, kv.Value)))
		{
		}

		protected override void ThreadFunc()
		{
			if (!Connect(base.Resource.Ecu.DiagnosisVariants["ROLLCALL"]))
			{
				Sapi.GetSapi().Channels.RaiseConnectCompleteEvent(base.Resource, new CaesarException(SapiError.DeviceInvalid));
			}
		}
	}

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

	internal static RollCallDoIP GlobalInstance => globalInstance;

	public string EthernetGuid { get; private set; }

	protected override TimeSpan RollCallValidLastSeenTime => TimeSpan.FromMilliseconds(10000 + mcdEthernetDetectionTimeout + 1000);

	internal override int ChannelTimeout => 20000;

	private RollCallDoIP()
		: base(Protocol.DoIP)
	{
	}

	internal override void Init()
	{
		Sapi sapi = Sapi.GetSapi();
		if (Sid.Connect((ProtocolId)(protocolId | (Protocol)268435456), 0u, ref activationChannelId) == J2534Error.NoError && activationChannelId != 0)
		{
			string libraryName = null;
			string libraryVersion = null;
			string driverVersion = null;
			string firmwareVersion = null;
			string supportedProtocols = null;
			Sid.GetDeviceVersionInfo(activationChannelId, ref libraryName, ref libraryVersion, ref driverVersion, ref firmwareVersion, ref supportedProtocols);
			if (supportedProtocols != null && supportedProtocols.IndexOf("ETH", StringComparison.Ordinal) != -1)
			{
				activationSupported = true;
				base.DeviceName = Sid.GetDeviceName(activationChannelId);
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "RP1210 ethernet interface is " + base.DeviceName);
				ActivateInterface(waitForOnline: false);
			}
			else
			{
				sapi.RaiseDebugInfoEvent(protocolId, "RP1210 ethernet activation not supported by " + libraryName);
			}
		}
		if (McdRoot.Initialized)
		{
			if (McdRoot.IsVciAccessLayerPrepared)
			{
				if (McdRoot.CurrentInterfaces.Any((McdInterface i) => i.Resources.Any((McdInterfaceResource r) => r.IsEthernet)))
				{
					detectionString = sapi.ConfigurationItems["McdEthernetDetectionString"].Value;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					foreach (Match item in detectionRegex.Matches(detectionString))
					{
						string value = item.Groups["name"].Value;
						string arg = (dictionary[value] = item.Groups["value"].Value);
						sapi.RaiseDebugInfoEvent(protocolId, string.Format(CultureInfo.InvariantCulture, "DoIP roll-call detection option: {0}='{1}'", value, arg));
					}
					if (dictionary.TryGetValue("VehicleDiscoveryTime", out var value3) && int.TryParse(value3, out mcdEthernetDetectionTimeout))
					{
						base.State = ConnectionState.Initialized;
					}
					else
					{
						sapi.RaiseDebugInfoEvent(protocolId, "DoIP roll-call services are unavailable: unable to parse VehicleDiscoveryTime from McdEthernetDetectionString.");
					}
				}
				else
				{
					sapi.RaiseDebugInfoEvent(protocolId, "DoIP roll-call services are unavailable: no ethernet interfaces are available. CHeck DoIP PDU-API installation.");
				}
			}
			else
			{
				sapi.RaiseDebugInfoEvent(protocolId, "DoIP roll-call services are unavailable: the MVCI kernel VCI access layer is not prepared.");
			}
		}
		else
		{
			sapi.RaiseDebugInfoEvent(protocolId, "DoIP roll-call services are unavailable: the MVCI kernel is not initialized.");
		}
	}

	internal void ActivateInterface(bool waitForOnline)
	{
		if (activationSupported && activationChannelId != 0)
		{
			if (activationFilterId != 0 && !string.IsNullOrEmpty(EthernetGuid) && !NetworkInterface.GetAllNetworkInterfaces().Any((NetworkInterface ni) => ni.Id.Equals(EthernetGuid, StringComparison.OrdinalIgnoreCase)))
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, EthernetGuid + " is no longer connected");
				StopActivationFilter();
			}
			if (activationFilterId == 0)
			{
				Tuple<byte[], byte[]> tuple = new Tuple<byte[], byte[]>(new byte[6], new byte[6]);
				J2534Error j2534Error = Sid.StartMsgFilter(activationChannelId, FilterType.Pass, new PassThruMsg((ProtocolId)protocolId, tuple.Item1), new PassThruMsg((ProtocolId)protocolId, tuple.Item2), null, ref activationFilterId);
				if (j2534Error != J2534Error.NoError)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Result from J2534.StartMsgFilter() is " + j2534Error.ToString() + " GetLastError is " + Sid.GetLastError());
				}
				else
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Started RP1210 ethernet activation");
					string guid = null;
					if (Sid.GetEthernetGuid(activationChannelId, ref guid) == J2534Error.NoError)
					{
						EthernetGuid = guid;
						Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "RP1210 ethernet activation GUID is " + EthernetGuid);
					}
				}
			}
		}
		if (!waitForOnline || !activationSupported || activationFilterId == 0 || string.IsNullOrEmpty(EthernetGuid))
		{
			return;
		}
		DateTime now = Sapi.Now;
		while ((Sapi.Now - now).TotalMilliseconds < 10000.0)
		{
			NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault((NetworkInterface ni) => ni.Id.Equals(EthernetGuid, StringComparison.OrdinalIgnoreCase));
			if (networkInterface != null && networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.GetIPProperties().UnicastAddresses.Count > 1)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, EthernetGuid + " (" + networkInterface.Description + ") is ready");
				Thread.Sleep(2000);
				break;
			}
		}
	}

	private void StopActivationFilter()
	{
		Sapi sapi = Sapi.GetSapi();
		if (activationFilterId != 0)
		{
			J2534Error j2534Error = Sid.StopMsgFilter(activationChannelId, activationFilterId);
			if (j2534Error != J2534Error.NoError)
			{
				sapi.RaiseDebugInfoEvent(protocolId, "Result from J2534.StopMsgFilter() is " + j2534Error.ToString() + " GetLastError is " + Sid.GetLastError());
			}
			else
			{
				sapi.RaiseDebugInfoEvent(protocolId, "Stopped RP1210 ethernet activation");
			}
			activationFilterId = 0u;
			EthernetGuid = null;
		}
	}

	internal override void Exit()
	{
		if (activationChannelId != 0)
		{
			StopActivationFilter();
			J2534Error j2534Error = Sid.Disconnect(activationChannelId);
			if (j2534Error != J2534Error.NoError)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Result from J2534.Disconnect for channel " + activationChannelId + " is " + j2534Error.ToString());
			}
			activationChannelId = 0u;
			activationSupported = false;
		}
		base.State = ConnectionState.NotInitialized;
		ClearEcus();
	}

	public static IEnumerable<Dictionary<string, string>> GetEntities(string entityString)
	{
		int num = int.Parse(countRegex.Match(entityString).Groups["count"].Value, CultureInfo.InvariantCulture);
		List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
		for (int i = 0; i < num; i++)
		{
			list.Add(new Dictionary<string, string>());
		}
		foreach (Match item in entityRegex.Matches(entityString))
		{
			list[int.Parse(item.Groups["index"].Value, CultureInfo.InvariantCulture)][item.Groups["name"].Value] = item.Groups["value"].Value;
		}
		return list;
	}

	private static bool IsPortOpen(string host, int port, TimeSpan timeout)
	{
		using TcpClient tcpClient = new TcpClient();
		try
		{
			return tcpClient.ConnectAsync(host, port).Wait(timeout) && tcpClient.Connected;
		}
		catch (AggregateException)
		{
			return false;
		}
		finally
		{
			tcpClient.Close();
		}
	}

	protected override void ThreadFunc()
	{
		Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "DoIP detection string: " + detectionString);
		do
		{
			lock (addressInformation)
			{
				base.State = (addressInformation.Any() ? ConnectionState.ChannelsConnected : ConnectionState.ChannelsConnecting);
			}
			ActivateInterface(waitForOnline: true);
			McdRoot.DetectInterfaces(detectionString);
			foreach (McdInterface item in McdRoot.CurrentInterfaces.Where((McdInterface i) => i.Resources.Any((McdInterfaceResource r) => r.IsEthernet)))
			{
				foreach (Dictionary<string, string> entity in GetEntities(item.Name))
				{
					if (!base.ConnectEnabled || !entity.TryGetValue("LA", out var value) || !entity.TryGetValue("IP", out var value2) || !value.TryParseSourceAddress(out var sourceAddress))
					{
						continue;
					}
					bool flag = IsPortOpen(value2, 13400, TimeSpan.FromMilliseconds(500.0));
					lock (addressInformation)
					{
						if (!addressInformation.TryGetValue(sourceAddress, out var value3))
						{
							if (flag)
							{
								addressInformation.Add(sourceAddress, new ChannelInformation(this, CreateConnectionResource(sourceAddress, item.HardwareName), entity));
							}
						}
						else if (flag)
						{
							value3.UpdateLastSeen();
						}
						else
						{
							RaiseDebugInfoEvent(sourceAddress, value2 + " is no longer connected; the roll-call channel will be removed");
						}
					}
				}
			}
			lock (addressInformation)
			{
				base.State = (addressInformation.Any() ? ConnectionState.ChannelsConnected : ConnectionState.TranslatorConnected);
			}
		}
		while (!closingEvent.WaitOne(10000));
	}

	private ConnectionResource CreateConnectionResource(int sourceAddress, string hardwareName)
	{
		return new ConnectionResource(GetEcu(sourceAddress, null), Protocol.DoIP, hardwareName, 0u, 0, sourceAddress);
	}

	internal override void CreateEcuInfos(EcuInfoCollection ecuInfos)
	{
		if (ecuInfos.Channel.Online)
		{
			lock (addressInformation)
			{
				if (addressInformation.TryGetValue(ecuInfos.Channel.SourceAddressLong.Value, out var value))
				{
					value.CreateEcuInfos(ecuInfos);
				}
				return;
			}
		}
		if (ecuInfos.Channel.LogFile == null)
		{
			string[] array = new string[5] { "LA", "EID", "IP", "VIN", "GroupID" };
			foreach (string qualifier in array)
			{
				CreateEcuInfo(ecuInfos, qualifier);
			}
		}
	}

	internal override EcuInfo CreateEcuInfo(EcuInfoCollection ecuInfos, string qualifier)
	{
		EcuInfo ecuInfo = new EcuInfo(ecuInfos.Channel, EcuInfoType.RollCall, qualifier, qualifier, "Common", "Rollcall/Common", null, 0, visible: true, common: true, summary: true);
		ecuInfos.AddFromRollCall(ecuInfo);
		return ecuInfo;
	}
}
