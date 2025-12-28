using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SapiLayer1;

internal abstract class RollCall : IRollCall, IDisposable
{
	protected abstract class ChannelInformation : IDisposable
	{
		protected ConnectionResource resource;

		private bool disposed;

		private int[] dataStreamSpns;

		private DateTime firstSeen;

		private DateTime lastSeen;

		internal readonly List<IdentificationInformation> Identification;

		protected RollCall rollCallManager;

		private Thread backgroundThread;

		internal ConnectionResource Resource
		{
			get
			{
				return resource;
			}
			set
			{
				if (resource == null && value != null)
				{
					resource = value;
					backgroundThread = new Thread(ThreadFunc);
					backgroundThread.Name = GetType().Name + ": " + resource.Ecu.Identifier;
					backgroundThread.Start();
				}
			}
		}

		internal bool IsRunning => lastSeen + TimeSpan.FromMilliseconds(rollCallManager.ChannelTimeout) > DateTime.UtcNow;

		internal TimeSpan TimeSinceLastSeen => DateTime.UtcNow - lastSeen;

		internal TimeSpan TimeSinceFirstSeen => DateTime.UtcNow - firstSeen;

		internal Channel Channel { get; private set; }

		internal bool Invalid { get; private set; }

		internal ChannelInformation(RollCall rollCallManager, ConnectionResource resource, IEnumerable<IdentificationInformation> identification)
		{
			Identification = identification.ToList();
			this.rollCallManager = rollCallManager;
			Resource = resource;
			firstSeen = DateTime.UtcNow;
		}

		protected abstract void ThreadFunc();

		protected bool Connect(DiagnosisVariant variant)
		{
			if (rollCallManager.ProtocolAlive)
			{
				Channel = Sapi.GetSapi().Channels.AddFromRollCall(Resource, variant);
				UpdateLastSeen();
				if (dataStreamSpns != null)
				{
					Channel?.SetDataStreamSpns(dataStreamSpns);
				}
			}
			if (Channel == null)
			{
				Invalid = true;
			}
			return !Invalid;
		}

		internal void CreateEcuInfos(EcuInfoCollection ecuInfos)
		{
			foreach (IdentificationInformation item in Identification.Where((IdentificationInformation i) => i.Value != null))
			{
				EcuInfo ecuInfo = rollCallManager.CreateEcuInfo(ecuInfos, item.IdString ?? item.Id.ToNumberString());
				if (ecuInfo != null)
				{
					item.SetEcuInfo(ecuInfo);
				}
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed && disposing && backgroundThread != null)
			{
				backgroundThread.Join();
				backgroundThread = null;
			}
			disposed = true;
		}

		internal void UpdateLastSeen()
		{
			lastSeen = DateTime.UtcNow;
		}

		internal void SetDataStreamSpns(int[] dataStreamSpns)
		{
			this.dataStreamSpns = dataStreamSpns;
			Channel?.SetDataStreamSpns(dataStreamSpns);
		}
	}

	internal enum ID : uint
	{
		Make = 586u,
		Model = 587u,
		SerialNumber = 588u,
		UnitNumber = 233u,
		SoftwareIdentification = 234u,
		VehicleIdentificationNumber = 237u,
		SuspectParameterNumber = 1214u,
		FailureModeIdentifier = 1215u,
		OccurrenceCount = 1216u,
		OnBoardDiagnosticCompliance = 1220u,
		CalibrationVerificationNumber = 1634u,
		CalibrationInformation = 1635u,
		ManufacturerCode = 2838u,
		Function = 2841u,
		EcuPartNumber = 2901u,
		EcuSerialNumber = 2902u,
		EcuLocation = 2903u,
		EcuType = 2904u,
		SPNOfApplicableSystemMonitor = 3066u,
		ApplicableSystemMonitorNumerator = 3067u,
		ApplicableSystemMonitorDenominator = 3068u,
		SPNSupported = 3146u,
		SupportedInExpandedFreezeFrame = 4100u,
		SupportedInDataStream = 4101u,
		SupportedInScaledTestResults = 4102u,
		SPNDataLength = 4103u,
		TestIdentifier = 1224u,
		SlotIdentifier = 4109u,
		TestValue = 4110u,
		TestLimitMaximum = 4111u,
		TestLimitMinimum = 4112u,
		AECDNumber = 4124u,
		AECDEngineHoursTimer1 = 4125u,
		AECDEngineHoursTimer2 = 4126u,
		EcuManufacturerName = 4304u,
		UnitSystem = 5197u
	}

	internal class IdentificationInformation
	{
		private object value;

		private EcuInfo ecuInfo;

		internal readonly ID Id;

		internal readonly string IdString;

		internal object Value
		{
			get
			{
				return value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;
					if (ecuInfo != null)
					{
						ecuInfo.UpdateFromRollCall(value);
					}
				}
			}
		}

		internal IdentificationInformation(ID id)
		{
			Id = id;
		}

		internal IdentificationInformation(string id, string value)
		{
			IdString = id;
			this.value = value;
		}

		internal void SetEcuInfo(EcuInfo ecuInfo)
		{
			this.ecuInfo = ecuInfo;
			if (this.ecuInfo != null)
			{
				ecuInfo.UpdateFromRollCall(value);
			}
		}
	}

	internal const string FixedVariantName = "ROLLCALL";

	protected List<Ecu> ecus;

	protected object ecusLock = new object();

	protected bool disposed;

	protected Protocol protocolId;

	protected ManualResetEvent closingEvent;

	protected ushort debugLevel;

	private Thread backgroundThread;

	private static Dictionary<Protocol, RollCall> managers = new Dictionary<Protocol, RollCall>();

	protected Dictionary<int, ChannelInformation> addressInformation = new Dictionary<int, ChannelInformation>();

	private volatile ConnectionState state;

	private float? load;

	private object loadLock = new object();

	private volatile bool connectEnabled;

	public bool IsAutoBaudRate { get; protected set; }

	public string DeviceName { get; protected set; }

	public string DeviceLibraryVersion { get; protected set; }

	public string DeviceDriverVersion { get; protected set; }

	public string DeviceFirmwareVersion { get; protected set; }

	public string DeviceLibraryName { get; protected set; }

	public string DeviceSupportedProtocols { get; protected set; }

	public virtual IEnumerable<byte> PowertrainAddresses => new byte[0];

	public bool ConnectEnabled
	{
		get
		{
			return connectEnabled;
		}
		set
		{
			if (connectEnabled != value)
			{
				connectEnabled = value;
			}
		}
	}

	public bool Running => backgroundThread != null;

	public ConnectionState State
	{
		get
		{
			return state;
		}
		set
		{
			if (value != state)
			{
				state = value;
				FireAndForget.Invoke(this.StateChangedEvent, this, new StateChangedEventArgs(value));
			}
		}
	}

	public float? Load
	{
		get
		{
			lock (loadLock)
			{
				return load;
			}
		}
		set
		{
			lock (loadLock)
			{
				if (value != load)
				{
					load = value;
					FireAndForget.Invoke(this.LoadChangedEvent, this, new EventArgs());
				}
			}
		}
	}

	internal bool ProtocolAlive
	{
		get
		{
			if (Running)
			{
				ConnectionState connectionState = State;
				if ((uint)(connectionState - 3) <= 2u)
				{
					return true;
				}
			}
			return false;
		}
	}

	public Protocol Protocol => protocolId;

	internal IEnumerable<Ecu> Ecus
	{
		get
		{
			lock (ecusLock)
			{
				if (ecus == null)
				{
					ecus = LoadMonitorEcus().ToList();
				}
				return new List<Ecu>(ecus);
			}
		}
	}

	protected abstract TimeSpan RollCallValidLastSeenTime { get; }

	internal virtual Dictionary<string, TranslationEntry> Translations => new Dictionary<string, TranslationEntry>();

	public virtual IDictionary<string, string> SuspectParameters => null;

	public virtual IDictionary<int, string> ParameterGroupLabels => null;

	public virtual IDictionary<int, string> ParameterGroupAcronyms => null;

	internal abstract int ChannelTimeout { get; }

	public event EventHandler<StateChangedEventArgs> StateChangedEvent;

	public event EventHandler<EventArgs> LoadChangedEvent;

	protected RollCall(Protocol ProtocolId)
	{
		protocolId = ProtocolId;
		managers[ProtocolId] = this;
		debugLevel = Convert.ToUInt16(Sapi.GetSapi().ConfigurationItems["RollCallDebugLevel"].Value, CultureInfo.InvariantCulture);
	}

	internal abstract void Init();

	public virtual void SetRestrictedAddressList(IEnumerable<byte> restrictedSourceAddresses)
	{
		throw new NotImplementedException();
	}

	internal abstract void Exit();

	public void Start()
	{
		if (debugLevel > 0)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Roll-call start");
		}
		if (State != ConnectionState.NotInitialized)
		{
			if (backgroundThread == null)
			{
				if (closingEvent == null)
				{
					closingEvent = new ManualResetEvent(initialState: false);
				}
				else
				{
					closingEvent.Reset();
				}
				backgroundThread = new Thread(ThreadFunc);
				backgroundThread.Name = GetType().Name + ": " + Protocol;
				backgroundThread.Start();
				State = ConnectionState.WaitingForTranslator;
			}
		}
		else
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Could not start roll-call as the manager is not initialized.");
		}
	}

	public void Stop()
	{
		if (debugLevel > 0)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(protocolId, "Roll-call stop");
		}
		if (backgroundThread != null)
		{
			closingEvent.Set();
			backgroundThread.Join();
			backgroundThread = null;
			State = ConnectionState.Initialized;
		}
	}

	protected virtual IEnumerable<Ecu> LoadMonitorEcus()
	{
		return new List<Ecu>();
	}

	protected void ClearEcus()
	{
		lock (ecusLock)
		{
			ecus = null;
		}
	}

	internal Ecu GetEcu(int sourceAddress, int? function)
	{
		lock (ecusLock)
		{
			Ecu ecu = ecus.Where((Ecu e) => e.IsRollCallBaseEcu && e.SourceAddressLong == sourceAddress && e.Function == function).FirstOrDefault();
			if (ecu == null)
			{
				ecu = new Ecu(sourceAddress, function, this);
				ecu.AcquireFromRollCall(string.Empty, null);
				ecus.Add(ecu);
			}
			return ecu;
		}
	}

	internal virtual void NotifyEcuInfoValue(EcuInfo ecuInfo, object ecuInfoValue)
	{
	}

	internal void RemoveChannel(Channel channel)
	{
		lock (addressInformation)
		{
			if (addressInformation.TryGetValue(channel.SourceAddressLong.Value, out var value) && value.Channel == channel)
			{
				addressInformation.Remove(channel.SourceAddressLong.Value);
			}
		}
	}

	internal virtual bool IsVirtual(int sourceAddress)
	{
		return false;
	}

	internal IEnumerable<int> GetActiveAddresses()
	{
		lock (addressInformation)
		{
			return (from ai in addressInformation
				where ai.Value.TimeSinceLastSeen < RollCallValidLastSeenTime
				select ai.Key).ToList();
		}
	}

	internal object GetIdentificationValue(int sourceAddress, string requestedId)
	{
		if (ProtocolAlive)
		{
			lock (addressInformation)
			{
				if (addressInformation.ContainsKey(sourceAddress))
				{
					ChannelInformation channelInformation = addressInformation[sourceAddress];
					if (channelInformation != null)
					{
						IdentificationInformation identificationInformation = channelInformation.Identification.FirstOrDefault((IdentificationInformation id) => id.IdString == requestedId || id.Id.ToNumberString() == requestedId);
						if (identificationInformation != null)
						{
							return identificationInformation.Value;
						}
					}
				}
			}
		}
		return null;
	}

	internal bool IsChannelRunning(Channel channel)
	{
		bool result = false;
		if (ProtocolAlive)
		{
			if (IsVirtual(channel.SourceAddress.Value))
			{
				lock (addressInformation)
				{
					result = addressInformation.Any((KeyValuePair<int, ChannelInformation> ai) => ai.Key != channel.SourceAddress.Value);
				}
			}
			else
			{
				lock (addressInformation)
				{
					if (addressInformation.TryGetValue(channel.SourceAddressLong.Value, out var value) && value.Channel == channel)
					{
						result = value.IsRunning;
					}
				}
			}
		}
		if (debugLevel > 1)
		{
			RaiseDebugInfoEvent(channel.SourceAddress.Value, "IsChannelRunning() = " + result);
		}
		return result;
	}

	protected abstract void ThreadFunc();

	protected void ClearAddressInformation()
	{
		IEnumerable<ChannelInformation> enumerable;
		lock (addressInformation)
		{
			enumerable = addressInformation.Select((KeyValuePair<int, ChannelInformation> ai) => ai.Value).ToList();
			addressInformation.Clear();
		}
		foreach (ChannelInformation item in enumerable)
		{
			item.Dispose();
		}
	}

	internal abstract EcuInfo CreateEcuInfo(EcuInfoCollection ecuInfos, string qualifier);

	internal abstract void CreateEcuInfos(EcuInfoCollection ecuInfos);

	internal virtual IEnumerable<Instrument> CreateInstruments(Channel channel)
	{
		return new List<Instrument>();
	}

	internal virtual Instrument CreateBaseInstrument(Channel channel, string qualifier)
	{
		throw new NotImplementedException();
	}

	internal virtual IEnumerable<Service> CreateServices(Channel channel, ServiceTypes type)
	{
		return new List<Service>();
	}

	public virtual void WriteTranslationFile(CultureInfo culture, IEnumerable<TranslationEntry> translations)
	{
	}

	internal virtual Dictionary<string, string> GetSuspectParametersForEcu(Ecu ecu)
	{
		return null;
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

	internal virtual byte[] ReadInstrument(Channel channel, byte[] data, int responseId, Predicate<Tuple<byte?, byte[]>> additionalResponseCheck, int responseTimeout)
	{
		throw new NotImplementedException();
	}

	internal virtual bool IsSnapshotSupported(Channel channel)
	{
		return false;
	}

	internal virtual void ReadEcuInfo(EcuInfo ecuInfo)
	{
	}

	internal virtual void ReadSnapshot(Channel channel)
	{
	}

	internal virtual void ReadFaultCodes(Channel channel)
	{
	}

	public static RollCall GetManager(Protocol protocolId)
	{
		if (managers.ContainsKey(protocolId))
		{
			return managers[protocolId];
		}
		return null;
	}

	protected virtual void RaiseDebugInfoEvent(int sourceAddress, string text)
	{
		string text2 = sourceAddress.ToString(CultureInfo.InvariantCulture);
		Sapi.GetSapi().RaiseDebugInfoEvent(string.Concat(protocolId, "-", text2), text);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			if (closingEvent != null)
			{
				closingEvent.Set();
				if (backgroundThread != null)
				{
					backgroundThread.Join();
					backgroundThread = null;
				}
			}
			ClearAddressInformation();
			DisposeInternal();
			if (closingEvent != null)
			{
				closingEvent.Close();
				closingEvent = null;
			}
		}
		disposed = true;
		managers[protocolId] = null;
	}

	protected virtual void DisposeInternal()
	{
	}
}
