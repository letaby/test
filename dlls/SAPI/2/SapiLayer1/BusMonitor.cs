using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CaesarAbstraction;
using J2534;
using McdAbstraction;

namespace SapiLayer1;

public abstract class BusMonitor : IDisposable
{
	private class BusMonitorCaesar : BusMonitor
	{
		private CaesarChannel caesarChannel;

		private CaesarMonitoring caesarMonitoring;

		private DateTime lastStatsCheck = DateTime.MinValue;

		protected internal override void Prepare()
		{
			//IL_006c: Expected O, but got Unknown
			Sapi sapi = Sapi.GetSapi();
			try
			{
				caesarChannel = sapi.Channels.OpenCaesarChannel(resource, addToPendingResources: false);
				caesarChannel.InitByteLevelNoInit();
				caesarMonitoring = caesarChannel.OpenMonitoringAdvanced();
				if (int.TryParse(caesarChannel.GetComParameter("CP_BAUDRATE"), out var result))
				{
					base.BaudRate = result;
				}
			}
			catch (CaesarErrorException ex)
			{
				throw new CaesarException(ex, null, null);
			}
			if (!caesarMonitoring.StartCanMonitoring())
			{
				throw new CaesarException(SapiError.FailedToStartCaesarMonitoring);
			}
		}

		protected internal override IEnumerable<BusMonitorFrame> ReadFrames()
		{
			List<CanMonitoringEntry> list = caesarMonitoring.GetCanMonitoringData((int)(ushort)framesForEvent).ToList();
			DateTime now = Sapi.Now;
			if (now > lastStatsCheck + TimeSpan.FromSeconds(1.0))
			{
				if (caesarMonitoring.Statistics == null && list.Count == 0)
				{
					throw new CaesarException(SapiError.UnableToRetrieveCanMonitoringStatistics);
				}
				lastStatsCheck = now;
			}
			return list.Select((CanMonitoringEntry m) => new BusMonitorFrame(m));
		}

		protected internal override void Cleanup()
		{
			if (caesarMonitoring != null)
			{
				caesarMonitoring.StopCanMonitoring();
				((CaesarHandle_003CCaesar_003A_003AMonitorHandleStruct_0020_002A_003E)caesarMonitoring).Dispose();
				caesarMonitoring = null;
			}
			if (caesarChannel != null)
			{
				caesarChannel.ExitByteLevelNoExit();
				((CaesarHandle_003CCaesar_003A_003AChannelHandleStruct_0020_002A_003E)caesarChannel).Dispose();
				caesarChannel = null;
			}
		}
	}

	private class BusMonitorMcd : BusMonitor
	{
		private McdMonitoringLink mcdMonitoring;

		protected internal override void Prepare()
		{
			try
			{
				if (networkInterface != null)
				{
					mcdMonitoring = McdRoot.CreateDoIPMonitorLink(networkInterface, path);
				}
				else
				{
					mcdMonitoring = McdRoot.CreateMonitoringLink(resource.MCDResourceQualifier);
				}
				mcdMonitoring.Start();
			}
			catch (McdException mcdError)
			{
				throw new CaesarException(mcdError);
			}
		}

		protected internal override IEnumerable<BusMonitorFrame> ReadFrames()
		{
			try
			{
				if (networkInterface != null)
				{
					return mcdMonitoring.FetchMonitoringFrames(framesForEvent).SelectMany((string m) => BusMonitorFrame.FromDoIPString(m));
				}
				return from m in mcdMonitoring.FetchMonitoringFrames(framesForEvent)
					select BusMonitorFrame.FromString(m, BusMonitorFrameStringFormat.Mcd, resource.PortIndex);
			}
			catch (McdException mcdError)
			{
				throw new CaesarException(mcdError);
			}
		}

		protected internal override void Cleanup()
		{
			if (mcdMonitoring != null)
			{
				mcdMonitoring.Stop();
				mcdMonitoring.Dispose();
				mcdMonitoring = null;
			}
		}
	}

	private class BusMonitorPassThru : BusMonitor
	{
		private uint channelId;

		private uint filterID;

		private bool haveAttemptedPassthruBaudRateRead;

		protected internal override void Prepare()
		{
			haveAttemptedPassthruBaudRateRead = false;
			Sapi.GetSapi();
			if (Sid.Connect((ProtocolId)(5 | (resource.PortIndex - 1 << 28)), 0u, ref channelId) == J2534Error.NoError)
			{
				Sid.SetUseConnectionMutex(channelId, 0);
				if (resource.BaudRate != 0)
				{
					Sid.SetBaudRate(channelId, (uint)resource.BaudRate);
				}
				Tuple<byte[], byte[]> tuple = new Tuple<byte[], byte[]>(new byte[6], new byte[6]);
				if (Sid.StartMsgFilter(channelId, FilterType.Pass, new PassThruMsg(ProtocolId.Can, tuple.Item1), new PassThruMsg(ProtocolId.Can, tuple.Item2), null, ref filterID) == J2534Error.NoError)
				{
					Sid.SetAllFiltersToPass(channelId, 1);
					J2534Error j2534Error = Sid.SetProvideCustomMessageTypes(channelId, 7);
					base.ProvidesCustomFrameTypes = j2534Error == J2534Error.NoError;
					return;
				}
				throw new CaesarException(SapiError.CannotStartMessageFilterForPassThruDevice, Sid.GetLastError());
			}
			throw new CaesarException(SapiError.CannotConnectToPassThruDevice, Sid.GetLastError());
		}

		protected internal override IEnumerable<BusMonitorFrame> ReadFrames()
		{
			List<PassThruMsg> list = new List<PassThruMsg>();
			switch (Sid.ReadMsgs(channelId, list, (uint)framesForEvent, 0u))
			{
			case J2534Error.NoError:
			case J2534Error.Timeout:
				if (list.Count <= 0)
				{
					break;
				}
				if (base.ProvidesCustomFrameTypes)
				{
					PassThruMsg passThruMsg = list.LastOrDefault((PassThruMsg m) => m.IsBaudRate);
					if (passThruMsg != null)
					{
						byte[] data = passThruMsg.GetData();
						base.BaudRate = (data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3];
						haveAttemptedPassthruBaudRateRead = true;
					}
				}
				if (!haveAttemptedPassthruBaudRateRead)
				{
					haveAttemptedPassthruBaudRateRead = true;
					uint baudRate = 0u;
					Sid.GetBaudRate(channelId, ref baudRate);
					if (baudRate != 0)
					{
						base.BaudRate = (int)baudRate;
					}
				}
				return list.Select((PassThruMsg m) => new BusMonitorFrame(m, resource.PortIndex));
			case J2534Error.BufferEmpty:
			{
				uint errorCode = 0u;
				if (Sid.GetErrorCode(channelId, ref errorCode) == J2534Error.NoError && errorCode == 61514)
				{
					throw new CaesarException(SapiError.HardwareNotResponding);
				}
				break;
			}
			default:
				throw new CaesarException(SapiError.PassThruDeviceError, Sid.GetLastError());
			}
			return new BusMonitorFrame[0];
		}

		protected internal override void Cleanup()
		{
			if (channelId != 0)
			{
				J2534Error j2534Error = Sid.StopMsgFilter(channelId, filterID);
				if (j2534Error != J2534Error.NoError)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(ProtocolId.Can, "Result from J2534.StopMsgFilter() is " + j2534Error.ToString() + " GetLastError is " + Sid.GetLastError());
				}
				Sid.Disconnect(channelId);
				channelId = 0u;
			}
		}
	}

	private ManualResetEvent closingEvent;

	private Thread backgroundThread;

	private Control threadMarshalControl;

	private int timeForEvent;

	private int framesForEvent;

	private object queueLock = new object();

	private Queue<BusMonitorFrame> queue = new Queue<BusMonitorFrame>();

	private ConnectionResource resource;

	private string networkInterface;

	private string path;

	private int baudrate;

	private bool providesCustomFrameTypes;

	private CaesarException exception;

	private bool disposedValue;

	public ConnectionResource Resource => resource;

	public string NetworkInterface => networkInterface;

	public string Path => path;

	public int BaudRate
	{
		get
		{
			return baudrate;
		}
		private set
		{
			if (value != baudrate)
			{
				baudrate = value;
				FireAndForget.Invoke(this.BaudRateChanged, this, new EventArgs(), threadMarshalControl);
			}
		}
	}

	public bool ProvidesCustomFrameTypes
	{
		get
		{
			return providesCustomFrameTypes;
		}
		private set
		{
			if (value != providesCustomFrameTypes)
			{
				providesCustomFrameTypes = value;
				FireAndForget.Invoke(this.ProvidesCustomFrameTypesChanged, this, new EventArgs(), threadMarshalControl);
			}
		}
	}

	public CaesarException Exception => exception;

	public bool Running
	{
		get
		{
			if (backgroundThread != null)
			{
				return backgroundThread.IsAlive;
			}
			return false;
		}
	}

	public event EventHandler FramesReady;

	public event EventHandler RunningChanged;

	public event EventHandler BaudRateChanged;

	public event EventHandler ProvidesCustomFrameTypesChanged;

	private BusMonitor()
	{
	}

	internal static BusMonitor Create(ConnectionResource resource, Control threadMarshalControl)
	{
		BusMonitor busMonitor = ((resource.MCDResourceQualifier != null) ? new BusMonitorMcd() : ((!resource.IsPassThru) ? ((BusMonitor)new BusMonitorCaesar()) : ((BusMonitor)new BusMonitorPassThru())));
		busMonitor.resource = resource;
		busMonitor.threadMarshalControl = threadMarshalControl;
		return busMonitor;
	}

	internal static BusMonitor Create(string networkInterface, string path, Control threadMarshalControl)
	{
		return new BusMonitorMcd
		{
			networkInterface = networkInterface,
			path = path,
			threadMarshalControl = threadMarshalControl
		};
	}

	public void Start(int timeForEvent, int framesForEvent)
	{
		this.timeForEvent = timeForEvent;
		this.framesForEvent = framesForEvent;
		try
		{
			Prepare();
		}
		catch (CaesarException)
		{
			Cleanup();
			throw;
		}
		closingEvent = new ManualResetEvent(initialState: false);
		backgroundThread = new Thread(ThreadFunc);
		backgroundThread.Name = GetType().Name;
		backgroundThread.Start();
	}

	protected internal abstract void Prepare();

	private void ThreadFunc()
	{
		do
		{
			try
			{
				IEnumerable<BusMonitorFrame> enumerable = ReadFrames();
				lock (queueLock)
				{
					foreach (BusMonitorFrame item in enumerable)
					{
						queue.Enqueue(item);
					}
				}
				FireAndForget.Invoke(this.FramesReady, this, new EventArgs(), threadMarshalControl);
			}
			catch (CaesarException ex)
			{
				exception = ex;
				break;
			}
		}
		while (!closingEvent.WaitOne(timeForEvent));
		Cleanup();
		FireAndForget.Invoke(this.RunningChanged, this, new EventArgs(), threadMarshalControl);
	}

	protected internal abstract IEnumerable<BusMonitorFrame> ReadFrames();

	public void Stop()
	{
		if (backgroundThread != null)
		{
			closingEvent.Set();
			backgroundThread.Join();
			backgroundThread = null;
		}
		if (closingEvent != null)
		{
			closingEvent.Close();
			closingEvent = null;
		}
		Sapi.GetSapi().BusMonitors.Remove(this);
	}

	protected internal abstract void Cleanup();

	public BusMonitorFrame[] FetchFrames(int numberRequired)
	{
		List<BusMonitorFrame> list = new List<BusMonitorFrame>();
		lock (queueLock)
		{
			while (queue.Count > 0)
			{
				list.Add(queue.Dequeue());
				if (list.Count == numberRequired)
				{
					break;
				}
			}
		}
		return list.ToArray();
	}

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				Stop();
				Cleanup();
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
