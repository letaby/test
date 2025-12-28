using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

internal class BackgroundConnect : IDisposable
{
	private volatile bool aborting;

	private Thread thread;

	private object handleToInitLock = new object();

	private CaesarChannel cCHToInit;

	private McdLogicalLink logicalLinkToInit;

	private ConnectionResource resToInit;

	private ChannelCollection parent;

	private Channel channelCreated;

	private CaesarException channelCreationError;

	private bool initialised;

	private bool variantMatched;

	private bool autoConnect;

	private bool disposed;

	internal ChannelOptions ChannelOptions { get; }

	internal bool AutoConnect => autoConnect;

	internal Thread BackgroundThread => thread;

	internal Channel Channel => channelCreated;

	internal CaesarChannel CaesarChannel => cCHToInit;

	internal McdLogicalLink MCDLogicalLink => logicalLinkToInit;

	internal bool Initialised => initialised;

	internal bool VariantMatched => variantMatched;

	internal CaesarException CaesarException => channelCreationError;

	internal ConnectionResource ConnectionResource => resToInit;

	internal BackgroundConnect Child { get; set; }

	internal BackgroundConnect(ChannelCollection parent, CaesarChannel ch, ConnectionResource res, ChannelOptions options = ChannelOptions.All, bool autoConnect = false)
	{
		cCHToInit = ch;
		resToInit = res;
		this.parent = parent;
		this.autoConnect = autoConnect;
		ChannelOptions = options;
	}

	internal BackgroundConnect(ChannelCollection parent, McdLogicalLink ll, ConnectionResource res, ChannelOptions options = ChannelOptions.All, bool autoConnect = false)
	{
		logicalLinkToInit = ll;
		resToInit = res;
		this.parent = parent;
		this.autoConnect = autoConnect;
		ChannelOptions = options;
	}

	internal void Start()
	{
		thread = new Thread(ThreadFunc);
		thread.Name = GetType().Name + ": " + resToInit.Ecu.Name + " " + resToInit.ToString();
		thread.Start();
	}

	private void ThreadFunc()
	{
		if (cCHToInit != null)
		{
			ThreadFuncCaesar();
		}
		else
		{
			ThreadFuncMCD();
		}
	}

	private void ThreadFuncMCD()
	{
		try
		{
			Sapi sapi = Sapi.GetSapi();
			if (logicalLinkToInit.IsEthernet && !RollCallDoIP.GlobalInstance.ProtocolAlive && !sapi.Channels.Any((Channel ch) => ch.McdChannelHandle != null && ch.McdChannelHandle.IsEthernet))
			{
				RollCallDoIP.GlobalInstance.ActivateInterface(waitForOnline: true);
				McdRoot.DetectInterfaces(sapi.ConfigurationItems["McdEthernetDetectionString"].Value);
			}
			logicalLinkToInit.OpenLogicalLink((ChannelOptions & ChannelOptions.StartStopCommunications) != 0);
			if (!aborting)
			{
				initialised = logicalLinkToInit.State == logicalLinkToInit.TargetState;
				if (initialised)
				{
					if (!resToInit.Ecu.IsByteMessaging)
					{
						logicalLinkToInit.IdentifyVariant();
						variantMatched = logicalLinkToInit.VariantName != null;
					}
					else
					{
						variantMatched = true;
					}
				}
				else
				{
					channelCreationError = new CaesarException(SapiError.PendingConnectionTargetStateNotReached);
				}
			}
			else
			{
				channelCreationError = new CaesarException(SapiError.PendingConnectionAborted);
			}
		}
		catch (McdException mcdError)
		{
			channelCreationError = new CaesarException(mcdError);
		}
		if (!initialised)
		{
			lock (handleToInitLock)
			{
				if (logicalLinkToInit != null)
				{
					logicalLinkToInit.Dispose();
					logicalLinkToInit = null;
				}
			}
		}
		if (parent != null)
		{
			channelCreated = parent.ConnectComplete(this);
			lock (handleToInitLock)
			{
				logicalLinkToInit = null;
			}
		}
	}

	private void ThreadFuncCaesar()
	{
		CaesarException ex = null;
		bool flag = !string.IsNullOrEmpty(resToInit.Ecu.Properties["FixedDiagnosisVariant"]);
		if (!aborting)
		{
			if ((ChannelOptions & ChannelOptions.StartStopCommunications) == 0)
			{
				cCHToInit.SetComParameter("CPI_INITTYPE", "0");
				cCHToInit.SetComParameter("CPI_EXITTYPE", "0");
			}
			if (flag)
			{
				cCHToInit.SetComParameter("CPI_READIDBLOCK", "0");
			}
			Type extensionType = resToInit.Ecu.ExtensionType;
			if (extensionType != null)
			{
				MethodInfo method = extensionType.GetMethod("PreConnect");
				if (method != null)
				{
					initialised = (bool)method.Invoke(null, new object[1] { cCHToInit });
				}
			}
			if (!initialised)
			{
				initialised = cCHToInit.Init();
			}
			if (cCHToInit.IsErrorSet)
			{
				ex = new CaesarException(cCHToInit);
				if (ex.ErrorNumber == 3029 && resToInit.Ecu.Properties.ContainsKey("ExtendedSessionRetryComParameters"))
				{
					ex = null;
					ChannelCollection.InitComParameterCollection(cCHToInit, resToInit.Ecu.Properties["ExtendedSessionRetryComParameters"]);
					initialised = cCHToInit.Init();
					if (cCHToInit.IsErrorSet)
					{
						ex = new CaesarException(cCHToInit);
					}
				}
			}
			if (ex != null)
			{
				channelCreationError = ex;
			}
		}
		else
		{
			channelCreationError = new CaesarException(SapiError.CannotSendMessageToDevice);
		}
		if (!initialised)
		{
			lock (handleToInitLock)
			{
				cCHToInit.Exit();
				((CaesarHandle_003CCaesar_003A_003AChannelHandleStruct_0020_002A_003E)cCHToInit).Dispose();
				cCHToInit = null;
			}
		}
		if (initialised)
		{
			CaesarEcu ecu = cCHToInit.Ecu;
			variantMatched = ecu == null || !string.IsNullOrEmpty(ecu.VariantName) || flag;
		}
		if (parent != null)
		{
			channelCreated = parent.ConnectComplete(this);
			lock (handleToInitLock)
			{
				cCHToInit = null;
			}
		}
	}

	internal void SetConnectCompleteFailure(SapiError result)
	{
		initialised = false;
		lock (handleToInitLock)
		{
			if (cCHToInit != null)
			{
				cCHToInit.Exit();
				((CaesarHandle_003CCaesar_003A_003AChannelHandleStruct_0020_002A_003E)cCHToInit).Dispose();
				cCHToInit = null;
			}
			else if (logicalLinkToInit != null)
			{
				logicalLinkToInit.Dispose();
				logicalLinkToInit = null;
			}
		}
		channelCreationError = new CaesarException(result);
	}

	internal void Abort()
	{
		if (Child != null)
		{
			Child.Abort();
		}
		else if (!aborting)
		{
			aborting = true;
			ThreadPool.QueueUserWorkItem(AbortCore, 0);
		}
	}

	private void AbortCore(object obj)
	{
		lock (handleToInitLock)
		{
			if (cCHToInit != null)
			{
				cCHToInit.SetTimeout(1u);
			}
			else if (logicalLinkToInit != null)
			{
				logicalLinkToInit.Abort();
			}
			else if (channelCreated != null)
			{
				channelCreated.Abort();
			}
			else if (CaesarException == null)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, "No CAESAR channel available to abort pending connection " + resToInit.Ecu.Name);
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
		if (!disposed && disposing)
		{
			lock (handleToInitLock)
			{
				if (cCHToInit != null)
				{
					cCHToInit.Exit();
					((CaesarHandle_003CCaesar_003A_003AChannelHandleStruct_0020_002A_003E)cCHToInit).Dispose();
					cCHToInit = null;
					initialised = false;
				}
				if (logicalLinkToInit != null)
				{
					logicalLinkToInit.Dispose();
					logicalLinkToInit = null;
					initialised = false;
				}
			}
		}
		disposed = true;
	}
}
