using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using CaesarAbstraction;
using J2534;
using McdAbstraction;

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

	private ManualResetEvent actionsQueueNonEmptyEvent = new ManualResetEvent(initialState: false);

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

	internal CaesarChannel ChannelHandle => caesarChannel;

	internal McdLogicalLink McdChannelHandle => mcdChannel;

	internal CaesarEcu EcuHandle => caesarEcu;

	internal McdDBLocation McdEcuHandle => mcdEcu;

	public ServiceCollection StructuredServices => structuredServices;

	internal List<Tuple<Service, ServiceOutputValue>> CaesarEquivalentServices
	{
		get
		{
			lock (caesarEquivalentServicesLock)
			{
				if (caesarEquivalentServices == null)
				{
					caesarEquivalentServices = new List<Tuple<Service, ServiceOutputValue>>();
					foreach (Service structuredService in StructuredServices)
					{
						bool flag = false;
						if (structuredService.ServiceTypes != ServiceTypes.DiagJob)
						{
							foreach (ServiceOutputValue outputValue in structuredService.OutputValues)
							{
								caesarEquivalentServices.Add(Tuple.Create(structuredService, outputValue));
								flag = true;
							}
						}
						if (!flag)
						{
							caesarEquivalentServices.Add(Tuple.Create<Service, ServiceOutputValue>(structuredService, null));
						}
					}
				}
				return caesarEquivalentServices;
			}
		}
	}

	internal byte? SourceAddress
	{
		get
		{
			int? sourceAddressLong = SourceAddressLong;
			if (!sourceAddressLong.HasValue)
			{
				return null;
			}
			return (byte)sourceAddressLong.Value;
		}
	}

	internal int? SourceAddressLong
	{
		get
		{
			ConnectionResource resource = connectionResource;
			if (resource == null && sessions.Any())
			{
				resource = sessions.First().Resource;
			}
			if (resource != null && resource.SourceAddressLong.HasValue)
			{
				return resource.SourceAddressLong;
			}
			return ecu.SourceAddressLong;
		}
	}

	internal object OfflineVarcodingHandleLock => offlineVarcodingHandleLock;

	internal Varcode OfflineVarcodingHandle
	{
		get
		{
			if (offlineVarcoding == null && !closing)
			{
				Sapi.GetSapi().EnsureCodingFilesLoaded();
				if (caesarEcu != null)
				{
					offlineVarcoding = new VarcodeCaesar(caesarEcu);
				}
				else if (mcdEcu != null && Sapi.GetSapi().InitState == InitState.Online)
				{
					ConnectionResource resource = variant.Ecu.GetConnectionResources().FirstOrDefault();
					if (resource != null)
					{
						McdInterface mcdInterface = McdRoot.CurrentInterfaces.FirstOrDefault((McdInterface i) => i.Qualifier == resource.MCDInterfaceQualifier);
						if (mcdInterface != null)
						{
							offlineVarcoding = new VarcodeMcd(this, resource.Interface.Qualifier, mcdInterface);
						}
					}
				}
			}
			return offlineVarcoding;
		}
	}

	internal bool Closing => closing;

	internal bool ActionWaiting => actionsQueue.Count > 0;

	internal ChoiceCollection CompleteFaultCodeStatusChoices
	{
		get
		{
			lock (faultCodeStatusChoicesLock)
			{
				if (completeFaultCodeStatusChoices == null)
				{
					completeFaultCodeStatusChoices = new ChoiceCollection();
					bool value = ecu.Properties.GetValue("SupportsPendingFaults", defaultIfNotSet: false);
					if (IsRollCall)
					{
						BuildFaultCodeStatusChoices(complete: true, value, FaultCodeStatus.Active | FaultCodeStatus.Pending | FaultCodeStatus.Stored | FaultCodeStatus.TestFailedSinceLastClear | FaultCodeStatus.Mil | FaultCodeStatus.Permanent | FaultCodeStatus.Immediate);
					}
					else
					{
						BuildFaultCodeStatusChoices(complete: true, value, FaultCodeStatus.Active | FaultCodeStatus.Pending | FaultCodeStatus.Stored | FaultCodeStatus.TestFailedSinceLastClear | FaultCodeStatus.Mil | FaultCodeStatus.Permanent);
					}
				}
				return completeFaultCodeStatusChoices;
			}
		}
	}

	internal ChoiceCollection FaultCodeStatusChoices
	{
		get
		{
			lock (faultCodeStatusChoicesLock)
			{
				if (faultCodeStatusChoices == null)
				{
					faultCodeStatusChoices = new ChoiceCollection();
					bool value = ecu.Properties.GetValue("SupportsPendingFaults", defaultIfNotSet: false);
					FaultCodeStatus possible = (value ? ((!IsRollCall) ? (FaultCodeStatus.Active | FaultCodeStatus.Pending | FaultCodeStatus.Stored | FaultCodeStatus.TestFailedSinceLastClear | FaultCodeStatus.Mil | FaultCodeStatus.Permanent) : (FaultCodeStatus.Active | FaultCodeStatus.Pending | FaultCodeStatus.Stored | FaultCodeStatus.TestFailedSinceLastClear | FaultCodeStatus.Mil | FaultCodeStatus.Permanent | FaultCodeStatus.Immediate)) : ((!ecu.IsRollCall) ? (FaultCodeStatus.Active | FaultCodeStatus.Stored) : (FaultCodeStatus.Active | FaultCodeStatus.TestFailedSinceLastClear)));
					BuildFaultCodeStatusChoices(complete: false, value, possible);
				}
				return faultCodeStatusChoices;
			}
		}
	}

	internal bool ChannelRunning
	{
		get
		{
			if (stopReason == StopReason.None)
			{
				if (ChannelHandle != null)
				{
					if ((channelOptions & ChannelOptions.StartStopCommunications) == 0)
					{
						return true;
					}
					return ChannelHandle.ChannelRunning;
				}
				if (ecu.RollCallManager != null)
				{
					return ecu.RollCallManager.IsChannelRunning(this);
				}
				if (mcdChannel != null)
				{
					if ((channelOptions & ChannelOptions.StartStopCommunications) == 0)
					{
						return true;
					}
					return mcdChannel.ChannelRunning;
				}
			}
			return false;
		}
	}

	internal byte? IntendedSession { get; private set; }

	public bool WriteAllParametersToSummaryFiles { get; set; }

	public Ecu Ecu => ecu;

	public bool IsRollCall => ecu.IsRollCall;

	public IEnumerable<Channel> RelatedChannels
	{
		get
		{
			List<Channel> list = new List<Channel>();
			if (IsRollCall)
			{
				list.AddRange(parent.Where((Channel c) => c.IsRelated(this)));
			}
			list.AddRange(parent.Where((Channel c) => c.IsRollCall && IsRelated(c)));
			return list;
		}
	}

	public Ecu ActualViaEcu
	{
		get
		{
			if (ecu.ViaEcus.Any())
			{
				return (from c in parent
					where ecu.ViaEcus.Contains(c.ecu)
					select c.Ecu).FirstOrDefault();
			}
			return null;
		}
	}

	public Channel ActualViaChannel
	{
		get
		{
			if (ecu.ViaEcus.Any())
			{
				return parent.Where((Channel c) => ecu.ViaEcus.Contains(c.ecu)).FirstOrDefault();
			}
			return null;
		}
	}

	public DiagnosisVariant DiagnosisVariant => variant;

	public ConnectionResource ConnectionResource => connectionResource;

	public CommunicationsState CommunicationsState => communicationsState;

	public EcuInfoCollection EcuInfos => ecuInfos;

	public ParameterCollection Parameters => parameters;

	public ParameterGroupCollection ParameterGroups => parameterGroups;

	public InstrumentCollection Instruments => instruments;

	public FaultCodeCollection FaultCodes => faultCodes;

	public ServiceCollection Services => services;

	public FlashAreaCollection FlashAreas => flashAreas;

	public CodingParameterGroupCollection CodingParameterGroups => codingParameterGroups;

	public SessionCollection Sessions => sessions;

	public DateTime StartTime => sessions.StartTime;

	public DateTime EndTime => sessions.EndTime;

	public LogFile LogFile => logFile;

	public string Identifier
	{
		get
		{
			if (ConnectionResource != null)
			{
				return ConnectionResource.Identifier;
			}
			if (Sessions.Count > 0)
			{
				Session session = Sessions[0];
				if (session.Resource != null)
				{
					return session.Resource.Identifier;
				}
			}
			return ecu.Identifier;
		}
	}

	public bool Online
	{
		get
		{
			CommunicationsState communicationsState = CommunicationsState;
			if ((uint)(communicationsState - 2) <= 1u || (uint)(communicationsState - 12) <= 1u)
			{
				return false;
			}
			return true;
		}
	}

	public ChannelExtension Extension
	{
		get
		{
			lock (channelExtensionLock)
			{
				if (!haveAttemptedExtensionLoad && channelExtension == null)
				{
					Type extensionType = ecu.ExtensionType;
					if (extensionType != null)
					{
						object[] args = new object[1] { this };
						channelExtension = (ChannelExtension)Activator.CreateInstance(extensionType, args);
					}
					haveAttemptedExtensionLoad = true;
				}
				return channelExtension;
			}
		}
	}

	public CommunicationsStateValueCollection CommunicationsStateValues => communicationsStateValues;

	internal RP1210ProtocolId? RP1210Protocol => rp1210Protocol;

	public long? RequestIdComParameter => requestId;

	public long? ResponseIdComParameter => responseId;

	public bool IsManipulated => manipulatedItems.Any();

	internal IEnumerable<int> DataStreamSpns => dataStreamSpns?.ToArray();

	public ChannelOptions ChannelOptions => channelOptions;

	internal bool IsChannelErrorSet
	{
		get
		{
			if (caesarChannel != null)
			{
				return caesarChannel.IsErrorSet;
			}
			return false;
		}
	}

	public CaesarException DisconnectionException { get; internal set; }

	public event CommunicationsStateUpdateEventHandler CommunicationsStateUpdateEvent;

	public event InitCompleteEventHandler InitCompleteEvent;

	public event ByteMessageCompleteEventHandler ByteMessageCompleteEvent;

	public event CommunicationsStateValueUpdateEventHandler CommunicationsStateValueUpdateEvent;

	public event SynchronousCheckEventHandler SynchronousCheckEvent;

	internal Channel(CaesarChannel ch, ConnectionResource cr, ChannelBaseCollection p, ChannelOptions options)
		: this(p, cr.Ecu, cr, CommunicationsState.OnlineButNotInitialized, null)
	{
		caesarChannel = ch;
		channelOptions = options;
		if (channelOptions != ChannelOptions.All)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "CAESAR channel with non-standard ChannelOptions: " + channelOptions);
		}
		fixedVariant = cr.Ecu.Properties["FixedDiagnosisVariant"];
		if (!string.IsNullOrEmpty(fixedVariant))
		{
			caesarChannel.Ecu.SetVariant(fixedVariant);
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "CAESAR channel selected fixed variant: " + fixedVariant);
		}
		if (caesarChannel.Ecu != null)
		{
			string variantName = caesarChannel.Ecu.VariantName;
			variant = ecu.DiagnosisVariants[variantName];
			if (variant == null)
			{
				variant = ecu.DiagnosisVariants.Base;
			}
			caesarEcu = variant.OpenEcuHandle();
		}
		else
		{
			variant = ecu.DiagnosisVariants.Last();
		}
		sessions.Add(new Session(this, Sapi.Now, DateTime.MinValue, ecu.DescriptionDataVersion, cr, variant.Name, !string.IsNullOrEmpty(fixedVariant), channelOptions));
		structuredServices = new ServiceCollection(this);
		Sapi.GetSapi().RaiseDebugInfoEvent(this, "Loaded " + structuredServices.Count + " structured services");
		ecu.IncrementConnectedChannelCount();
	}

	internal Channel(McdLogicalLink logicalLink, ConnectionResource connectionResource, ChannelBaseCollection parent, ChannelOptions options)
		: this(parent, connectionResource.Ecu, connectionResource, CommunicationsState.OnlineButNotInitialized, null)
	{
		channelOptions = options;
		if (channelOptions != ChannelOptions.All)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "MCD channel with non-standard ChannelOptions: " + channelOptions);
		}
		mcdChannel = logicalLink;
		this.connectionResource = connectionResource;
		fixedVariant = connectionResource.Ecu.Properties["FixedDiagnosisVariant"];
		if (!string.IsNullOrEmpty(fixedVariant))
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "MCD channel selected fixed variant: " + fixedVariant);
		}
		string variantName = mcdChannel.VariantName;
		variant = ecu.DiagnosisVariants.FirstOrDefault((DiagnosisVariant v) => v.Locations != null && v.Locations.Contains(variantName));
		if (variant == null)
		{
			variant = ((!ecu.IsByteMessaging) ? ecu.DiagnosisVariants.Base : ecu.DiagnosisVariants.Last());
		}
		sessions.Add(new Session(this, Sapi.Now, DateTime.MinValue, ecu.DescriptionDataVersion, connectionResource, variant.Name, !string.IsNullOrEmpty(fixedVariant), channelOptions));
		mcdEcu = mcdChannel.DBLocation;
		structuredServices = new ServiceCollection(this);
		Sapi.GetSapi().RaiseDebugInfoEvent(this, "Loaded " + structuredServices.Count + " structured services");
		ecu.IncrementConnectedChannelCount();
	}

	internal Channel(DiagnosisVariant v, ChannelBaseCollection p, LogFile lf)
		: this(p, v.Ecu, null, (lf != null) ? CommunicationsState.LogFilePaused : CommunicationsState.Offline, lf)
	{
		variant = v;
	}

	internal Channel(CaesarEcu ecuh, DiagnosisVariant variant, ChannelBaseCollection parent, LogFile logFile)
		: this(parent, variant.Ecu, null, (logFile != null) ? CommunicationsState.LogFilePaused : CommunicationsState.Offline, logFile)
	{
		caesarEcu = ecuh;
		structuredServices = new ServiceCollection(this);
		this.variant = variant;
	}

	internal Channel(McdDBLocation variantHandle, DiagnosisVariant variant, ChannelBaseCollection parent, LogFile logFile)
		: this(parent, variant.Ecu, null, (logFile != null) ? CommunicationsState.LogFilePaused : CommunicationsState.Offline, logFile)
	{
		mcdEcu = variantHandle;
		structuredServices = new ServiceCollection(this);
		this.variant = variant;
	}

	internal Channel(ChannelBaseCollection parent, ConnectionResource resource, DiagnosisVariant variant)
		: this(parent, resource.Ecu, resource, CommunicationsState.OnlineButNotInitialized, null)
	{
		sessions.Add(new Session(this, Sapi.Now, DateTime.MinValue, ecu.DescriptionDataVersion, resource, variant.Name, null, ChannelOptions.All));
		this.variant = variant;
		ecu.IncrementConnectedChannelCount();
		switch (ecu.RollCallManager.Protocol)
		{
		case Protocol.J1708:
			rp1210Protocol = RP1210ProtocolId.J1708;
			break;
		case Protocol.J1939:
			rp1210Protocol = RP1210ProtocolId.J1939;
			break;
		}
	}

	private Channel(ChannelBaseCollection parent, Ecu ecu, ConnectionResource resource, CommunicationsState initialState, LogFile logFile)
	{
		this.logFile = logFile;
		communicationsState = initialState;
		closing = false;
		this.parent = parent;
		this.ecu = ecu;
		connectionResource = resource;
		communicationsStateValues = new CommunicationsStateValueCollection(this);
		sessions = new SessionCollection();
		ecuInfos = new EcuInfoCollection(this);
		parameters = new ParameterCollection(this);
		parameterGroups = new ParameterGroupCollection(this);
		services = new ServiceCollection(this, ServiceTypes.Actuator | ServiceTypes.Adjustment | ServiceTypes.Download | ServiceTypes.DiagJob | ServiceTypes.Function | ServiceTypes.Global | ServiceTypes.IOControl | ServiceTypes.Routine | ServiceTypes.Security | ServiceTypes.Session | ServiceTypes.Static, reconstructCombined: true);
		instruments = new InstrumentCollection(this);
		faultCodes = new FaultCodeCollection(this);
		flashAreas = new FlashAreaCollection(this);
		codingParameterGroups = new CodingParameterGroupCollection(this);
	}

	internal void SetCommunicationsState(CommunicationsState cs)
	{
		SetCommunicationsState(cs, string.Empty);
	}

	internal void SetCommunicationsState(CommunicationsState cs, string additional)
	{
		if (cs != communicationsState)
		{
			communicationsState = cs;
			FireAndForget.Invoke(this.CommunicationsStateUpdateEvent, this, new CommunicationsStateEventArgs(cs));
			if (thread != null)
			{
				CommunicationsStateValue communicationsStateValue = new CommunicationsStateValue(this, cs, Sapi.Now, additional);
				communicationsStateValues.Add(communicationsStateValue, setcurrent: true);
				RaiseCommunicationsStateValueUpdateEvent(communicationsStateValue);
			}
		}
	}

	internal McdCaesarEquivalenceScaleInfo GetMcdCaesarEquivalenceScaleInfo(string qualifier, Type type, Func<McdDBDataObjectProp> getDataObjectProp)
	{
		lock (scaleInfoCache)
		{
			Tuple<string, int> key = Tuple.Create(qualifier, type.GetHashCode());
			if (!scaleInfoCache.TryGetValue(key, out var value))
			{
				value = (scaleInfoCache[key] = new McdCaesarEquivalenceScaleInfo(type, getDataObjectProp()));
			}
			return value;
		}
	}

	internal Varcode VCInit()
	{
		if (caesarChannel != null)
		{
			return new VarcodeCaesar(this);
		}
		if (mcdChannel != null)
		{
			return new VarcodeMcd(this, mcdChannel);
		}
		return null;
	}

	private void BuildFaultCodeStatusChoices(bool complete, bool obd, FaultCodeStatus possible)
	{
		for (int i = 0; i <= (int)possible; i++)
		{
			FaultCodeStatus faultCodeStatus = (FaultCodeStatus)((int)possible & i);
			if (faultCodeStatus == (FaultCodeStatus)i || i == 0)
			{
				if (complete)
				{
					completeFaultCodeStatusChoices.Add(new Choice(faultCodeStatus.ToStatusString(obd, ecu), faultCodeStatus));
				}
				else
				{
					faultCodeStatusChoices.Add(CompleteFaultCodeStatusChoices.GetItemFromRawValue(faultCodeStatus));
				}
			}
		}
	}

	internal void ResetFaultCodeStatusChoices()
	{
		lock (faultCodeStatusChoicesLock)
		{
			faultCodeStatusChoices = null;
			completeFaultCodeStatusChoices = null;
		}
	}

	public void Stop()
	{
		stopReason = StopReason.Closing;
	}

	public void Abort()
	{
		Abort(StopReason.Closing);
	}

	internal void Abort(StopReason reason)
	{
		lock (channelHandleLock)
		{
			if (caesarChannel != null)
			{
				caesarChannel.SetTimeout(1u);
			}
			else if (mcdChannel != null)
			{
				mcdChannel.Abort();
			}
		}
		stopReason = reason;
		switch (stopReason)
		{
		case StopReason.NoTraffic:
			DisconnectionException = new CaesarException(SapiError.NoTraffic);
			break;
		case StopReason.TranslatorDisconnected:
			DisconnectionException = new CaesarException(SapiError.TranslatorDisconnected);
			break;
		}
	}

	internal void QueueAction(object o, bool synchronous)
	{
		if (Online)
		{
			if (thread != null)
			{
				lock (actionsQueue)
				{
					if (synchronous)
					{
						syncDone = new ManualResetEvent(initialState: false);
					}
					actionsQueue.Enqueue(o);
					actionsQueueNonEmptyEvent.Set();
				}
				if (synchronous)
				{
					bool num = syncDone.WaitOne();
					syncDone.Close();
					syncDone = null;
					if (!num)
					{
						throw new ThreadStateException("Wait for synchronous call expired");
					}
					if (syncException != null)
					{
						Exception ex = syncException;
						syncException = null;
						throw ex;
					}
				}
			}
			else
			{
				if (CommunicationsState != CommunicationsState.OnlineButNotInitialized)
				{
					throw new InvalidOperationException("Cannot queue an action because the channel thread isn't running.");
				}
				if (synchronous)
				{
					throw new InvalidOperationException("You must call Channel.Init before queueing an action.");
				}
				actionsQueue.Enqueue(o);
				actionsQueueNonEmptyEvent.Set();
			}
		}
		else
		{
			if (thread == null)
			{
				throw new InvalidOperationException("Cannot queue an action in offline mode");
			}
			if (synchronous)
			{
				throw new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
			}
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "An attempt to queue an asynchronous action during disconnection was ignored.");
		}
	}

	internal void QueueAction(object action, bool synchronous, SynchronousCheckFailureHandler failureDelegate)
	{
		if (SynchronousCheck(action))
		{
			QueueAction(action, synchronous);
			return;
		}
		CaesarException ex = new CaesarException(SapiError.NegativeResponse);
		failureDelegate(action, ex);
		if (!synchronous)
		{
			return;
		}
		throw ex;
	}

	internal void SyncDone(Exception e)
	{
		lock (actionsQueue)
		{
			if (syncDone != null && actionsQueue.Count == 0)
			{
				syncException = e;
				syncDone.Set();
			}
		}
	}

	internal void InternalDisconnect()
	{
		if (closing)
		{
			return;
		}
		closing = true;
		if (thread != null)
		{
			thread.Join();
		}
		if (caesarChannel != null || mcdChannel != null)
		{
			CloseChannel();
		}
		else if (ecu.RollCallManager != null && connectionResource != null)
		{
			ecu.RollCallManager.RemoveChannel(this);
			SetCommunicationsState(CommunicationsState.Offline);
		}
		if (offlineVarcoding != null)
		{
			offlineVarcoding.Dispose();
			offlineVarcoding = null;
		}
		mcdEcu = null;
		if (caesarEcu != null)
		{
			((CaesarHandle_003CCaesar_003A_003AECUHandleStructS_0020_002A_003E)caesarEcu).Dispose();
			caesarEcu = null;
		}
		if (parent != null)
		{
			parent.Remove(this);
		}
		if (sessions.Count > 0)
		{
			sessions[0].UpdateEndTime(Sapi.Now);
			Sapi sapi = Sapi.GetSapi();
			if (sapi.LogFiles.Logging && LogFile == null)
			{
				sapi.LogFiles.LogChannel(this);
			}
		}
	}

	internal bool GetActiveDiagnosticInformation(out byte? responseSession, out uint? responseVariant)
	{
		ByteMessage byteMessage = new ByteMessage(this, new Dump("22F100"));
		byteMessage.InternalDoMessage(internalRequest: true);
		if (byteMessage.Response != null && byteMessage.Response.Data.Count > 6)
		{
			responseSession = byteMessage.Response.Data[6];
			responseVariant = (uint)(byteMessage.Response.Data[5] + (byteMessage.Response.Data[4] << 8));
		}
		else
		{
			responseSession = null;
			responseVariant = null;
		}
		return responseSession.HasValue;
	}

	private void MaintainIntendedSession()
	{
		if (!GetActiveDiagnosticInformation(out var responseSession, out var responseVariant) || responseSession.Equals(IntendedSession))
		{
			return;
		}
		if (DiagnosisVariant.DiagnosticVersionLong.HasValue && string.IsNullOrEmpty(fixedVariant) && responseVariant != (DiagnosisVariant.DiagnosticVersionLong.Value & 0xFFFF))
		{
			if (!haveShownChangedLongDiagVersionError)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "Session ended, but no action taken because long diagnostic version has changed to 0x{0:x}", responseVariant);
				Sapi.GetSapi().LogFiles.LabelLog(text);
				Sapi.GetSapi().RaiseDebugInfoEvent(this, text);
				haveShownChangedLongDiagVersionError = true;
			}
		}
		else
		{
			Sapi.GetSapi().LogFiles.LabelLog("Session ended, calling intialize service to recover.", ecu, this);
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Session ended, calling intialize service to recover.");
			ExecuteInitializeService();
		}
	}

	internal void ExecuteInitializeService()
	{
		if ((channelOptions & ChannelOptions.ExecuteInitializeService) != ChannelOptions.None)
		{
			services.InternalDereferencedExecute("InitializeService");
		}
	}

	internal CaesarException Reset()
	{
		CaesarException result = null;
		if (mcdChannel != null)
		{
			try
			{
				mcdChannel.Close();
				instruments.Invalidate();
				mcdChannel.OpenLogicalLink((channelOptions & ChannelOptions.StartStopCommunications) != 0);
				if (mcdChannel.State == mcdChannel.TargetState)
				{
					mcdChannel.IdentifyVariant();
				}
				ExecuteInitializeService();
			}
			catch (McdException mcdError)
			{
				result = new CaesarException(mcdError);
			}
		}
		else
		{
			caesarChannel.Exit();
			if (!caesarChannel.IsErrorSet)
			{
				ChannelCollection.InitComParameterCollection(caesarChannel, ConnectionResource);
				if (caesarChannel.Init())
				{
					if (!string.IsNullOrEmpty(fixedVariant))
					{
						caesarChannel.Ecu.SetVariant(fixedVariant);
					}
					ExecuteInitializeService();
				}
			}
			if (caesarChannel.IsErrorSet)
			{
				result = new CaesarException(caesarChannel);
			}
		}
		return result;
	}

	internal static void LoadFromLog(XElement channelElement, LogFileFormatTagCollection format, LogFile file, object ecusLock, object allChannelsLock, List<string> missingEcuList, List<string> missingVariantList, List<string> missingQualifierList, object missingInfoLock)
	{
		string ecuName = channelElement.Elements(format[TagName.EcuName]).First().Value;
		XElement xElement = channelElement.Elements(format[TagName.EcuVariant]).First();
		string value = xElement.Value;
		XAttribute xAttribute = xElement.Attribute(format[TagName.Fixed]);
		bool? isFixedVariant = ((xAttribute != null) ? new bool?(xAttribute.Value == "1") : ((bool?)null));
		XElement xElement2 = channelElement.Elements(format[TagName.ChannelOptions]).FirstOrDefault();
		ChannelOptions channelOptions = ((xElement2 != null) ? ((ChannelOptions)Enum.Parse(typeof(ChannelOptions), xElement2.Value)) : ChannelOptions.All);
		IEnumerable<XElement> enumerable = channelElement.Elements(format[TagName.EcuInfos]).First().Elements(format[TagName.EcuInfo]);
		XElement xElement3 = channelElement.Elements(format[TagName.Instruments]).FirstOrDefault();
		IEnumerable<XElement> enumerable2 = null;
		if (xElement3 != null)
		{
			enumerable2 = xElement3.Elements(format[TagName.Instrument]);
		}
		XElement xElement4 = channelElement.Elements(format[TagName.Services]).FirstOrDefault();
		IEnumerable<XElement> enumerable3 = null;
		if (xElement4 != null)
		{
			enumerable3 = xElement4.Elements(format[TagName.Service]);
		}
		XElement xElement5 = channelElement.Elements(format[TagName.Parameters]).FirstOrDefault();
		IEnumerable<XElement> enumerable4 = null;
		if (xElement5 != null)
		{
			enumerable4 = xElement5.Elements(format[TagName.Group]);
		}
		Sapi sapi = Sapi.GetSapi();
		DiagnosisVariant diagnosisVariant = null;
		Ecu ecu = null;
		lock (ecusLock)
		{
			ecu = sapi.Ecus[ecuName];
			if (ecu == null)
			{
				try
				{
					ecu = Ecu.CreateFromRollCallLog(ecuName);
				}
				catch (ArgumentException)
				{
				}
				catch (IndexOutOfRangeException)
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
			diagnosisVariant = ecu.DiagnosisVariants[value];
			if (diagnosisVariant == null)
			{
				Ecu ecu2 = sapi.Ecus.FirstOrDefault((Ecu e) => e.Name == ecuName && e != ecu);
				if (ecu2 != null)
				{
					diagnosisVariant = ecu2.DiagnosisVariants[value];
					if (diagnosisVariant != null)
					{
						ecu = ecu2;
					}
				}
			}
		}
		if (diagnosisVariant == null)
		{
			IEnumerable<string> enumerable6;
			if (enumerable2 == null)
			{
				IEnumerable<string> enumerable5 = new List<string>();
				enumerable6 = enumerable5;
			}
			else
			{
				enumerable6 = enumerable2.Select((XElement e) => e.Attribute(format[TagName.Qualifier]).Value);
			}
			IEnumerable<string> first = enumerable6;
			IEnumerable<string> second = enumerable.Select((XElement e) => e.Attribute(format[TagName.Qualifier]).Value);
			IEnumerable<string> requiredQualifiers = first.Union(second).ToList();
			var matches = from ecuVariant in ecu.DiagnosisVariants
				let matchCount = ecuVariant.DiagServiceQualifiers.Intersect(requiredQualifiers).Count()
				orderby matchCount
				select new
				{
					Variant = ecuVariant,
					Count = matchCount
				};
			diagnosisVariant = matches.First(m => m.Count == matches.Last().Count).Variant;
			lock (missingInfoLock)
			{
				missingVariantList.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ecuName, value));
			}
		}
		XElement xElement6 = channelElement.Elements(format[TagName.Resource]).FirstOrDefault();
		ConnectionResource resource = ((xElement6 != null) ? ConnectionResource.FromString(ecu, xElement6.Value) : null);
		Channel channel;
		lock (allChannelsLock)
		{
			channel = file.AllChannels[diagnosisVariant];
			if (channel == null)
			{
				channel = file.AllChannels.InternalConnectOffline(diagnosisVariant, file, resource);
			}
		}
		XElement xElement7 = channelElement.Elements(format[TagName.Description]).FirstOrDefault();
		DateTime dateTime = Sapi.TimeFromString(channelElement.Attribute(format[TagName.StartTime]).Value);
		DateTime dateTime2 = Sapi.TimeFromString(channelElement.Attribute(format[TagName.EndTime]).Value);
		channel.sessions.Add(new Session(channel, dateTime, dateTime2, (xElement7 != null) ? xElement7.Value : string.Empty, resource, value, isFixedVariant, channelOptions));
		foreach (XElement item in enumerable)
		{
			EcuInfo.LoadFromLog(item, format, channel, missingQualifierList, missingInfoLock);
		}
		foreach (EcuInfo item2 in channel.ecuInfos.Where((EcuInfo ei) => ei.EcuInfoType == EcuInfoType.Compound))
		{
			item2.LoadCompoundFromLog(dateTime, dateTime2);
		}
		if (enumerable2 != null)
		{
			foreach (XElement item3 in enumerable2)
			{
				Instrument.LoadFromLog(item3, format, channel, missingQualifierList, missingInfoLock);
			}
		}
		else
		{
			file.Summary = true;
		}
		if (enumerable3 != null)
		{
			foreach (XElement item4 in enumerable3)
			{
				Service.LoadFromLog(item4, format, channel, missingQualifierList, missingInfoLock);
			}
		}
		if (enumerable4 != null)
		{
			bool flag = false;
			foreach (XElement item5 in enumerable4)
			{
				flag |= ParameterGroup.LoadFromLog(item5, item5.Attribute(format[TagName.Qualifier]).Value, format, channel, missingQualifierList, missingInfoLock);
			}
			channel.Parameters.ValuesLoadedFromLog = flag;
		}
		foreach (XElement item6 in channelElement.Elements(format[TagName.FaultCodes]).First().Elements(format[TagName.FaultCode]))
		{
			FaultCodeIncident faultCodeIncident = FaultCodeIncident.FromXElement(item6, format, channel);
			if (faultCodeIncident.Functions == ReadFunctions.Snapshot)
			{
				faultCodeIncident.FaultCode.Snapshots.Add(faultCodeIncident);
			}
			else
			{
				faultCodeIncident.FaultCode.FaultCodeIncidents.Add(faultCodeIncident);
			}
		}
		XElement xElement8 = channelElement.Elements(format[TagName.CommunicationsStates]).FirstOrDefault();
		if (xElement8 == null)
		{
			return;
		}
		foreach (CommunicationsStateValue item7 in from el in xElement8.Elements(format[TagName.CommunicationsState])
			select CommunicationsStateValue.FromXElement(el, format, channel) into csv
			orderby csv.Time
			select csv)
		{
			channel.CommunicationsStateValues.Add(item7, setcurrent: false);
		}
	}

	internal void WriteXmlTo(XmlWriter writer)
	{
		WriteXmlTo(all: true, StartTime, (EndTime != DateTime.MinValue) ? EndTime : Sapi.Now, writer);
	}

	internal void WriteSummaryXmlTo(XmlWriter writer)
	{
		WriteXmlTo(all: false, StartTime, (EndTime != DateTime.MinValue) ? EndTime : Sapi.Now, writer);
	}

	internal void WriteXmlTo(bool all, DateTime startTime, DateTime endTime, XmlWriter writer)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		writer.WriteStartElement(currentFormat[TagName.Channel].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.StartTime].LocalName, Sapi.TimeToString(startTime));
		writer.WriteAttributeString(currentFormat[TagName.EndTime].LocalName, Sapi.TimeToString(endTime));
		writer.WriteElementString(currentFormat[TagName.EcuName], Ecu.Name);
		writer.WriteStartElement(currentFormat[TagName.EcuVariant].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.Fixed].LocalName, (!string.IsNullOrEmpty(fixedVariant)) ? "1" : "0");
		writer.WriteValue(DiagnosisVariant.Name);
		writer.WriteEndElement();
		if (channelOptions != ChannelOptions.All)
		{
			writer.WriteElementString(currentFormat[TagName.ChannelOptions], channelOptions.ToNumberString());
		}
		writer.WriteElementString(currentFormat[TagName.Description], sessions[0].DescriptionVersion);
		writer.WriteElementString(currentFormat[TagName.Resource], (ConnectionResource != null) ? ConnectionResource.ToString() : "NULL");
		XElement xElement = new XElement(currentFormat[TagName.EcuInfos]);
		if (ecuInfos.Acquired)
		{
			foreach (EcuInfo ecuInfo in ecuInfos)
			{
				if (ecuInfo.EcuInfoType != EcuInfoType.Compound && ecuInfo.EcuInfoValues.Count > 0 && (all || ecuInfo.Common || ecuInfo.Summary))
				{
					xElement.Add(ecuInfo.GetXElement(startTime, endTime));
				}
			}
		}
		xElement.WriteTo(writer);
		writer.WriteStartElement(currentFormat[TagName.Instruments].LocalName);
		if (instruments.Acquired)
		{
			foreach (Instrument instrument in instruments)
			{
				if (instrument.InstrumentValues.Count > 0 && (all | instrument.Summary))
				{
					instrument.WriteXmlTo(all, startTime, endTime, writer);
				}
			}
		}
		writer.WriteEndElement();
		if (all)
		{
			writer.WriteStartElement(currentFormat[TagName.Services].LocalName);
			List<Service> list = new List<Service>();
			if (services.Acquired)
			{
				list.AddRange(services.Where((Service s) => s.Executions.Count > 0));
			}
			if (structuredServices != null && structuredServices.Acquired)
			{
				list.AddRange(structuredServices.Where((Service s) => s.Executions.Count > 0));
			}
			foreach (Service item in list.Distinct())
			{
				item.WriteXmlTo(startTime, endTime, writer);
			}
			writer.WriteEndElement();
		}
		writer.WriteStartElement(currentFormat[TagName.Parameters].LocalName);
		if (parameters.Acquired)
		{
			foreach (ParameterGroup parameterGroup in parameterGroups)
			{
				parameterGroup.WriteXmlTo(startTime, endTime, writer, all || WriteAllParametersToSummaryFiles);
			}
		}
		writer.WriteEndElement();
		XElement xElement2 = new XElement(currentFormat[TagName.FaultCodes]);
		if (faultCodes.Acquired)
		{
			foreach (FaultCode faultCode in faultCodes)
			{
				foreach (FaultCodeIncident faultCodeIncident in faultCode.FaultCodeIncidents)
				{
					XElement xElement3 = faultCodeIncident.GetXElement(startTime, endTime);
					if (xElement3 != null)
					{
						xElement2.Add(xElement3);
					}
				}
				foreach (FaultCodeIncident snapshot in faultCode.Snapshots)
				{
					XElement xElement4 = snapshot.GetXElement(startTime, endTime);
					if (xElement4 != null)
					{
						xElement2.Add(xElement4);
					}
				}
			}
		}
		xElement2.WriteTo(writer);
		XElement xElement5 = new XElement(currentFormat[TagName.CommunicationsStates]);
		CommunicationsStateValue communicationsStateValue = null;
		foreach (CommunicationsStateValue item2 in communicationsStateValues.OrderBy((CommunicationsStateValue csv) => csv.Time))
		{
			if (item2.Time >= startTime)
			{
				if (communicationsStateValue != null)
				{
					xElement5.Add(communicationsStateValue.GetXElement(startTime));
					communicationsStateValue = null;
				}
				if (item2.Time > endTime)
				{
					break;
				}
				xElement5.Add(item2.GetXElement(startTime));
			}
			else
			{
				communicationsStateValue = item2;
			}
		}
		xElement5.WriteTo(writer);
		writer.WriteEndElement();
	}

	internal static void ExtractMetadata(XmlReader xmlReader, List<LogMetadataItem> result)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		string attribute = xmlReader.GetAttribute(currentFormat[TagName.StartTime].LocalName);
		xmlReader.ReadToDescendant(currentFormat[TagName.EcuName].LocalName);
		string ecuName = xmlReader.ReadElementContentAsString();
		string text = xmlReader.ReadElementContentAsString();
		result.Add(new LogMetadataItem(LogMetadataType.Identification, ecuName, text, attribute));
		xmlReader.ReadToNextSibling(currentFormat[TagName.FaultCodes].LocalName);
		if (xmlReader.ReadToDescendant(currentFormat[TagName.FaultCode].LocalName))
		{
			do
			{
				result.Add(FaultCode.ExtractMetadata(xmlReader, ecuName, text));
			}
			while (xmlReader.NodeType == XmlNodeType.Element);
		}
		xmlReader.Skip();
		if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == currentFormat[TagName.CommunicationsStates].LocalName)
		{
			xmlReader.Skip();
		}
		xmlReader.Skip();
	}

	internal void RaiseByteMessageComplete(ByteMessage s, Exception e)
	{
		FireAndForget.Invoke(this.ByteMessageCompleteEvent, s, new ResultEventArgs(e));
		SyncDone(e);
	}

	internal void RaiseCommunicationsStateValueUpdateEvent(CommunicationsStateValue current)
	{
		FireAndForget.Invoke(this.CommunicationsStateValueUpdateEvent, this, new CommunicationsStateValueEventArgs(current));
	}

	public void Init(bool autoread)
	{
		instruments.AutoRead = autoread;
		faultCodes.AutoRead = autoread;
		Init();
	}

	public void Init()
	{
		if (!Online)
		{
			throw new InvalidOperationException("Cannot initialize an offline channel");
		}
		if (closing)
		{
			throw new InvalidOperationException("Cannot initialize a channel that is closing");
		}
		if (thread != null)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "An attempt to reinitialize this channel was ignored");
			return;
		}
		thread = new Thread(ThreadFunc);
		thread.Name = GetType().Name + ": " + ecu.Name;
		thread.Start();
	}

	public void Disconnect()
	{
		if (LogFile == null)
		{
			InternalDisconnect();
			return;
		}
		throw new InvalidOperationException("Cannot disconnect a channel in a log file");
	}

	public override string ToString()
	{
		return ecu.Name;
	}

	public ByteMessage SendByteMessage(Dump dump, bool synchronous)
	{
		ByteMessage byteMessage = new ByteMessage(this, dump);
		QueueAction(byteMessage, synchronous);
		return byteMessage;
	}

	public ByteMessage SendByteMessage(Dump dump, Dump requiredResponse, bool synchronous)
	{
		ByteMessage byteMessage = new ByteMessage(this, dump, requiredResponse);
		QueueAction(byteMessage, synchronous);
		return byteMessage;
	}

	private bool IsRelated(Channel rollCallChannel)
	{
		return ecu.IsRelated(rollCallChannel, ActualViaEcu);
	}

	public bool ActiveAtTime(DateTime time)
	{
		if (logFile != null)
		{
			return sessions.ActiveAtTime(time) != null;
		}
		return false;
	}

	public void SetCommunicationParameter(string name, string value)
	{
		//IL_0027: Expected O, but got Unknown
		if (ChannelHandle != null)
		{
			try
			{
				ChannelHandle.SetComParameter(name, value);
				return;
			}
			catch (CaesarErrorException ex)
			{
				throw new CaesarException(ex, null, null);
			}
		}
		if (McdChannelHandle != null)
		{
			try
			{
				McdChannelHandle.SetComParameter(name, value);
				return;
			}
			catch (McdException mcdError)
			{
				throw new CaesarException(mcdError);
			}
		}
		throw new InvalidOperationException("The CAESAR channel is NULL. This method only functions for an online channel.");
	}

	public string GetCommunicationParameter(string name)
	{
		//IL_0026: Expected O, but got Unknown
		if (ChannelHandle != null)
		{
			try
			{
				return ChannelHandle.GetComParameter(name);
			}
			catch (CaesarErrorException ex)
			{
				throw new CaesarException(ex, null, null);
			}
		}
		if (McdChannelHandle != null)
		{
			try
			{
				return McdChannelHandle.GetComParameter(name)?.ToString();
			}
			catch (McdException mcdError)
			{
				throw new CaesarException(mcdError);
			}
		}
		throw new InvalidOperationException("The CAESAR channel is NULL. This method only functions for an online channel.");
	}

	private void SetRP1210Protocol(long requestId)
	{
		uint channelId = 0u;
		RP1210ProtocolId rp1210ProtocolId = RP1210ProtocolId.Can;
		ushort physicalChannel = 0;
		string rp1210ProtocolString = null;
		byte[] data = BitConverter.GetBytes(requestId).Take(4).Reverse()
			.ToArray();
		try
		{
			if (Sid.GetChannelInfo(new PassThruMsg(ProtocolId.Iso15765, data), ref channelId, ref rp1210ProtocolId, ref physicalChannel, ref rp1210ProtocolString) == J2534Error.NoError)
			{
				rp1210Protocol = rp1210ProtocolId;
			}
			else
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to determine J2534 channel info as an error was returned");
			}
		}
		catch (SEHException)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(this, "Unable to determine J2534 channel info as SID is not loaded");
		}
	}

	private void GetActualRequestResponseIds()
	{
		requestId = GetCommunicationParameterAsLong((ChannelHandle != null) ? "CP_REQUEST_CANIDENTIFIER" : ((McdChannelHandle != null && !McdChannelHandle.IsEthernet) ? "CP_CanPhysReqId" : null));
		responseId = GetCommunicationParameterAsLong((ChannelHandle != null) ? "CP_RESPONSE_CANIDENTIFIER" : ((McdChannelHandle != null && !McdChannelHandle.IsEthernet) ? "CP_CanRespUSDTId" : null));
		long? GetCommunicationParameterAsLong(string parameter)
		{
			if (!string.IsNullOrEmpty(parameter))
			{
				string communicationParameter = GetCommunicationParameter(parameter);
				if (communicationParameter != null)
				{
					return long.Parse(communicationParameter, CultureInfo.InvariantCulture);
				}
			}
			return null;
		}
	}

	internal bool TryGetOfflineDiagnosticService(byte[] message, ByteMessageDirection direction, out List<Service> obj)
	{
		if (offlineServices == null)
		{
			offlineServices = (from s in StructuredServices
				where s.ServiceTypes != ServiceTypes.DiagJob && s.BaseRequestMessage != null && s.BaseRequestMessage.Data.Count > 0
				group s by Tuple.Create(s.BaseRequestMessage, s.RequestMessageMask) into g
				group g by g.Key.Item1.Data[0]).ToDictionary((IGrouping<byte, IGrouping<Tuple<Dump, Dump>, Service>> k) => k.Key, (IGrouping<byte, IGrouping<Tuple<Dump, Dump>, Service>> v) => v.Select((IGrouping<Tuple<Dump, Dump>, Service> g) => Tuple.Create(g.Key.Item1, g.Key.Item2, g.ToList())).ToList());
		}
		if (direction == ByteMessageDirection.RX)
		{
			message[0] -= 64;
		}
		if (offlineServices.TryGetValue(message[0], out var value))
		{
			foreach (Tuple<Dump, Dump, List<Service>> item in value)
			{
				if (Dump.MaskedMatch(message, item.Item1, item.Item2))
				{
					obj = item.Item3;
					return true;
				}
			}
		}
		obj = null;
		return false;
	}

	internal void SetManipulatedState(string item, bool manipulated)
	{
		string text;
		if (manipulated)
		{
			if (!manipulatedItems.Contains(item))
			{
				manipulatedItems.Add(item);
			}
			text = string.Format(CultureInfo.InvariantCulture, "MANIPULATION: {0}", item);
		}
		else
		{
			manipulatedItems.Remove(item);
			text = string.Format(CultureInfo.InvariantCulture, "END MANIPULATION: {0}", item);
		}
		Sapi.GetSapi().LogFiles.LabelLog(text, ecu, this);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private bool SynchronousCheck(object action)
	{
		if (this.SynchronousCheckEvent != null)
		{
			SynchronousCheckEventArgs e = new SynchronousCheckEventArgs(action);
			this.SynchronousCheckEvent(this, e);
			return !e.Cancel;
		}
		return true;
	}

	internal bool IsOwner(CaesarChannel requested)
	{
		if (requested != null)
		{
			lock (channelHandleLock)
			{
				return ((object)requested).Equals((object)caesarChannel);
			}
		}
		return false;
	}

	internal bool IsOwner(McdLogicalLink requested)
	{
		if (requested != null)
		{
			lock (channelHandleLock)
			{
				return requested.Equals(mcdChannel) || (offlineVarcoding is VarcodeMcd varcodeMcd && varcodeMcd.IsOwner(requested));
			}
		}
		return false;
	}

	private void CloseChannel()
	{
		ecu.DecrementConnectedChannelCount();
		if (caesarChannel != null)
		{
			CaesarChannel val = null;
			lock (channelHandleLock)
			{
				val = caesarChannel;
				caesarChannel = null;
			}
			if (val != null)
			{
				val.Exit();
				((CaesarHandle_003CCaesar_003A_003AChannelHandleStruct_0020_002A_003E)val).Dispose();
			}
		}
		else if (mcdChannel != null)
		{
			McdLogicalLink mcdLogicalLink = null;
			lock (channelHandleLock)
			{
				mcdLogicalLink = mcdChannel;
				mcdChannel = null;
			}
			mcdLogicalLink?.Dispose();
		}
		SetCommunicationsState(CommunicationsState.Offline);
	}

	internal void SetDataStreamSpns(int[] dataStreamSpns)
	{
		this.dataStreamSpns = dataStreamSpns;
	}

	private void Dispose(bool disposing)
	{
		if (!disposed && disposing)
		{
			InternalDisconnect();
			if (channelExtension != null)
			{
				channelExtension.Dispose();
				channelExtension = null;
			}
			if (syncDone != null)
			{
				syncDone.Close();
				syncDone = null;
			}
			if (parameters != null)
			{
				parameters.Dispose();
				parameters = null;
			}
			if (instruments != null)
			{
				instruments.Dispose();
				instruments = null;
			}
			if (actionsQueueNonEmptyEvent != null)
			{
				actionsQueueNonEmptyEvent.Close();
				actionsQueueNonEmptyEvent = null;
			}
		}
		disposed = true;
	}

	internal void ClearCache()
	{
		if (ChannelHandle != null)
		{
			ChannelHandle.ClearCache();
		}
		else if (McdChannelHandle != null)
		{
			McdChannelHandle.ClearCache();
		}
	}

	private void ThreadFunc()
	{
		DateTime now = Sapi.Now;
		DateTime now2 = Sapi.Now;
		DateTime now3 = Sapi.Now;
		bool flag = false;
		bool flag2 = !IsRollCall && instruments.Any((Instrument i) => i.Periodic);
		if (!IsRollCall && ecu.IsUds)
		{
			GetActualRequestResponseIds();
			if (RequestIdComParameter.HasValue && ResponseIdComParameter.HasValue)
			{
				BusMonitorFrame.AddEcuCustomIdentifiers(ecu, RequestIdComParameter.Value, ResponseIdComParameter.Value);
				SetRP1210Protocol(requestId.Value);
			}
		}
		try
		{
			if (!closing && ChannelRunning)
			{
				ExecuteInitializeService();
			}
			if ((channelOptions & ChannelOptions.CyclicRead) != ChannelOptions.None)
			{
				if (!closing && ecuInfos.AutoRead && ChannelRunning)
				{
					SetCommunicationsState(CommunicationsState.ReadEcuInfo);
					ecuInfos.InternalRead(explicitread: false);
				}
				if (!closing && parameters.AutoReadSummaryParameters)
				{
					foreach (Parameter item in parameters.Where((Parameter p) => p.Summary).DistinctBy((Parameter p) => p.GroupQualifier))
					{
						if (!closing && ChannelRunning)
						{
							SetCommunicationsState(CommunicationsState.ReadParameters, item.GroupQualifier);
							parameters.InternalReadGroup(item, explicitRead: false);
						}
					}
				}
			}
			if (!closing && ChannelRunning)
			{
				FireAndForget.Invoke(this.InitCompleteEvent, this, new EventArgs());
				SetCommunicationsState(CommunicationsState.Online);
			}
			int num = 0;
			bool flag3 = false;
			bool result = false;
			if (ChannelRunning && !IsRollCall && ecu.Properties.ContainsKey("MaintainSession") && bool.TryParse(ecu.Properties["MaintainSession"], out result) && result)
			{
				if (mcdChannel != null)
				{
					int? sessionType = mcdChannel.SessionType;
					if (sessionType.HasValue)
					{
						IntendedSession = (byte)sessionType.Value;
					}
				}
				else if (ecu.ProtocolComParameters != null && ecu.ProtocolComParameters.Contains("CP_INIT_SESSION_TYPE"))
				{
					IntendedSession = Convert.ToByte(ecu.ProtocolComParameters["CP_INIT_SESSION_TYPE"], CultureInfo.InvariantCulture);
				}
			}
			while (!closing && Online)
			{
				if (ChannelRunning && ((caesarChannel != null && !caesarChannel.IsErrorSet) || ecu.RollCallManager != null || mcdChannel != null))
				{
					if (flag2 && !flag)
					{
						instruments.RegisterPeriodicListeners(reg: true);
						flag = true;
					}
					if (CommunicationsState != CommunicationsState.Online)
					{
						instruments.AgeAll();
					}
					else
					{
						instruments.InvalidateAged();
					}
					switch (CommunicationsState)
					{
					case CommunicationsState.Online:
					{
						if (actionsQueue.Count > 0)
						{
							object obj = actionsQueue.Dequeue();
							if (obj.GetType() == typeof(ServiceExecution))
							{
								ServiceExecution serviceExecution = (ServiceExecution)obj;
								SetCommunicationsState(CommunicationsState.ExecuteService, serviceExecution.Service.Qualifier);
								serviceExecution.Service.InternalExecute(Service.ExecuteType.UserInvoke, serviceExecution);
								SetCommunicationsState(CommunicationsState.Online);
							}
							else if (obj.GetType() == typeof(Instrument))
							{
								Instrument instrument = (Instrument)obj;
								SetCommunicationsState(CommunicationsState.ReadInstrument, instrument.Qualifier);
								instrument.InternalRead(explicitread: true);
								SetCommunicationsState(CommunicationsState.Online);
							}
							else if (obj.GetType() == typeof(FaultCode))
							{
								FaultCode faultCode = (FaultCode)obj;
								SetCommunicationsState(CommunicationsState.ResetFault, faultCode.Code);
								faultCode.InternalReset();
								num = 0;
								SetCommunicationsState(CommunicationsState.Online);
							}
							else if (obj.GetType() == typeof(string))
							{
								SetCommunicationsState(CommunicationsState.ExecuteService, obj.ToString());
								services.InternalExecute(obj.ToString(), userInvoke: true);
								SetCommunicationsState(CommunicationsState.Online);
							}
							else if (obj.GetType() == typeof(EcuInfo))
							{
								EcuInfo ecuInfo = (EcuInfo)obj;
								SetCommunicationsState(CommunicationsState.ReadEcuInfo, ecuInfo.Qualifier);
								ecuInfo.InternalRead(explicitread: true);
								SetCommunicationsState(CommunicationsState.Online);
							}
							else if (obj.GetType() == typeof(Parameter))
							{
								Parameter parameter = (Parameter)obj;
								SetCommunicationsState(CommunicationsState.ReadParameters, parameter.GroupQualifier);
								parameters.InternalReadGroup(parameter, explicitRead: true);
							}
							else if (obj.GetType() == typeof(ByteMessage))
							{
								ByteMessage byteMessage = (ByteMessage)obj;
								SetCommunicationsState(CommunicationsState.ByteMessage, byteMessage.Request.ToString());
								byteMessage.InternalDoMessage();
								SetCommunicationsState(CommunicationsState.Online);
							}
							else
							{
								SetCommunicationsState((CommunicationsState)obj);
							}
							break;
						}
						actionsQueueNonEmptyEvent.Reset();
						bool flag4 = caesarChannel != null && flashing;
						switch (num)
						{
						case 0:
							if ((channelOptions & ChannelOptions.MaintainSession) != ChannelOptions.None && IntendedSession.HasValue && (caesarChannel != null || mcdChannel != null) && (Sapi.Now - now3).TotalSeconds >= 1.0)
							{
								MaintainIntendedSession();
								flag3 = true;
								now3 = Sapi.Now;
							}
							break;
						case 1:
							if ((channelOptions & ChannelOptions.CyclicRead) != ChannelOptions.None && faultCodes.SupportsFaultRead && faultCodes.AutoRead && !ecu.IsVirtual)
							{
								if ((caesarChannel != null || mcdChannel != null) && !flag4 && (Sapi.Now - now).TotalSeconds >= 1.0)
								{
									faultCodes.InternalRead(explicitread: false);
									now = Sapi.Now;
									flag3 = true;
								}
								if (faultCodes.SupportsSnapshot && !flag4 && (Sapi.Now - now2).TotalSeconds >= 10.0)
								{
									faultCodes.InternalReadSnapshot(explicitread: false);
									now2 = Sapi.Now;
									flag3 = true;
								}
							}
							break;
						case 2:
							if ((channelOptions & ChannelOptions.CyclicRead) != ChannelOptions.None && instruments.AutoRead && !flag4 && instruments.InternalRead())
							{
								flag3 = true;
							}
							break;
						case 3:
							if ((channelOptions & ChannelOptions.CyclicRead) != ChannelOptions.None && ecuInfos.AutoRead && ecuInfos.InternalRead(EcuInfoInternalReadType.CyclicRead))
							{
								flag3 = true;
							}
							break;
						}
						if (++num > 3)
						{
							if (!flag3)
							{
								actionsQueueNonEmptyEvent.WaitOne(IsRollCall ? 100 : 10);
							}
							else
							{
								flag3 = false;
							}
							num = 0;
						}
						break;
					}
					case CommunicationsState.ReadParameters:
						parameters.InternalRead();
						break;
					case CommunicationsState.WriteParameters:
						parameters.InternalWrite();
						break;
					case CommunicationsState.ResetFaults:
						faultCodes.InternalReset();
						num = 0;
						break;
					case CommunicationsState.ReadFaults:
						faultCodes.InternalRead(explicitread: true);
						break;
					case CommunicationsState.ReadSnapshot:
						faultCodes.InternalReadSnapshot(explicitread: true);
						break;
					case CommunicationsState.Flash:
						flashing = true;
						flashAreas.InternalFlash();
						flashing = false;
						break;
					case CommunicationsState.ProcessVcp:
						parameters.InternalProcessVcp();
						break;
					case CommunicationsState.ReadEcuInfo:
						ecuInfos.InternalRead(explicitread: true);
						break;
					}
				}
				bool channelRunning = ChannelRunning;
				if ((caesarChannel != null && caesarChannel.IsErrorSet) || (mcdChannel != null && mcdChannel.ChannelError != null))
				{
					CaesarException ex = (DisconnectionException = ((caesarChannel != null) ? new CaesarException(caesarChannel) : new CaesarException(mcdChannel.ChannelError)));
					CaesarException ex3 = ex;
					if (mcdChannel != null)
					{
						mcdChannel.ResetChannelError();
					}
					Sapi.GetSapi().RaiseExceptionEvent(this, ex3);
					if (Sapi.GetSapi().LogFiles.Logging)
					{
						Sapi.GetSapi().LogFiles.LabelLog(string.Format(CultureInfo.InvariantCulture, "Communications Error: {0}", ex3.Message), ecu, this);
					}
				}
				if (!channelRunning)
				{
					SetCommunicationsState(CommunicationsState.Disconnecting, (DisconnectionException != null) ? DisconnectionException.Message : ((stopReason != StopReason.None) ? stopReason.ToString() : "Channel stopped running"));
					actionsQueue.Clear();
				}
			}
		}
		catch (Exception ex4)
		{
			if (CommunicationsState == CommunicationsState.Flash)
			{
				flashing = false;
			}
			SetCommunicationsState(CommunicationsState.Disconnecting, string.Format(CultureInfo.InvariantCulture, "Fatal Error: {0}", ex4.Message));
			actionsQueue.Clear();
			Sapi.GetSapi().RaiseExceptionEvent(this, ex4);
		}
		if (syncDone != null)
		{
			SyncDone(new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation));
		}
		if (flag2 && flag)
		{
			instruments.RegisterPeriodicListeners(reg: false);
		}
		instruments.Invalidate();
		faultCodes.Invalidate();
		CloseChannel();
	}
}
