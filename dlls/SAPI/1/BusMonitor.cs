// Decompiled with JetBrains decompiler
// Type: SapiLayer1.BusMonitor
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using J2534;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace SapiLayer1;

public abstract class BusMonitor : IDisposable
{
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

  private BusMonitor()
  {
  }

  public event EventHandler FramesReady;

  public event EventHandler RunningChanged;

  public event EventHandler BaudRateChanged;

  public event EventHandler ProvidesCustomFrameTypesChanged;

  public ConnectionResource Resource => this.resource;

  public string NetworkInterface => this.networkInterface;

  public string Path => this.path;

  public int BaudRate
  {
    get => this.baudrate;
    private set
    {
      if (value == this.baudrate)
        return;
      this.baudrate = value;
      FireAndForget.Invoke((MulticastDelegate) this.BaudRateChanged, (object) this, new EventArgs(), this.threadMarshalControl);
    }
  }

  public bool ProvidesCustomFrameTypes
  {
    get => this.providesCustomFrameTypes;
    private set
    {
      if (value == this.providesCustomFrameTypes)
        return;
      this.providesCustomFrameTypes = value;
      FireAndForget.Invoke((MulticastDelegate) this.ProvidesCustomFrameTypesChanged, (object) this, new EventArgs(), this.threadMarshalControl);
    }
  }

  public CaesarException Exception => this.exception;

  public bool Running => this.backgroundThread != null && this.backgroundThread.IsAlive;

  internal static BusMonitor Create(ConnectionResource resource, Control threadMarshalControl)
  {
    BusMonitor busMonitor = resource.MCDResourceQualifier == null ? (!resource.IsPassThru ? (BusMonitor) new BusMonitor.BusMonitorCaesar() : (BusMonitor) new BusMonitor.BusMonitorPassThru()) : (BusMonitor) new BusMonitor.BusMonitorMcd();
    busMonitor.resource = resource;
    busMonitor.threadMarshalControl = threadMarshalControl;
    return busMonitor;
  }

  internal static BusMonitor Create(
    string networkInterface,
    string path,
    Control threadMarshalControl)
  {
    BusMonitor.BusMonitorMcd busMonitorMcd = new BusMonitor.BusMonitorMcd();
    busMonitorMcd.networkInterface = networkInterface;
    busMonitorMcd.path = path;
    busMonitorMcd.threadMarshalControl = threadMarshalControl;
    return (BusMonitor) busMonitorMcd;
  }

  public void Start(int timeForEvent, int framesForEvent)
  {
    this.timeForEvent = timeForEvent;
    this.framesForEvent = framesForEvent;
    try
    {
      this.Prepare();
    }
    catch (CaesarException ex)
    {
      this.Cleanup();
      throw;
    }
    this.closingEvent = new ManualResetEvent(false);
    this.backgroundThread = new Thread(new ThreadStart(this.ThreadFunc));
    this.backgroundThread.Name = this.GetType().Name;
    this.backgroundThread.Start();
  }

  protected internal abstract void Prepare();

  private void ThreadFunc()
  {
    do
    {
      try
      {
        IEnumerable<BusMonitorFrame> busMonitorFrames = this.ReadFrames();
        lock (this.queueLock)
        {
          foreach (BusMonitorFrame busMonitorFrame in busMonitorFrames)
            this.queue.Enqueue(busMonitorFrame);
        }
        FireAndForget.Invoke((MulticastDelegate) this.FramesReady, (object) this, new EventArgs(), this.threadMarshalControl);
      }
      catch (CaesarException ex)
      {
        this.exception = ex;
        break;
      }
    }
    while (!this.closingEvent.WaitOne(this.timeForEvent));
    this.Cleanup();
    FireAndForget.Invoke((MulticastDelegate) this.RunningChanged, (object) this, new EventArgs(), this.threadMarshalControl);
  }

  protected internal abstract IEnumerable<BusMonitorFrame> ReadFrames();

  public void Stop()
  {
    if (this.backgroundThread != null)
    {
      this.closingEvent.Set();
      this.backgroundThread.Join();
      this.backgroundThread = (Thread) null;
    }
    if (this.closingEvent != null)
    {
      this.closingEvent.Close();
      this.closingEvent = (ManualResetEvent) null;
    }
    Sapi.GetSapi().BusMonitors.Remove(this);
  }

  protected internal abstract void Cleanup();

  public BusMonitorFrame[] FetchFrames(int numberRequired)
  {
    List<BusMonitorFrame> busMonitorFrameList = new List<BusMonitorFrame>();
    lock (this.queueLock)
    {
      while (this.queue.Count > 0)
      {
        busMonitorFrameList.Add(this.queue.Dequeue());
        if (busMonitorFrameList.Count == numberRequired)
          break;
      }
    }
    return busMonitorFrameList.ToArray();
  }

  private void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
    {
      this.Stop();
      this.Cleanup();
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private class BusMonitorCaesar : BusMonitor
  {
    private CaesarChannel caesarChannel;
    private CaesarMonitoring caesarMonitoring;
    private DateTime lastStatsCheck = DateTime.MinValue;

    protected internal override void Prepare()
    {
      Sapi sapi = Sapi.GetSapi();
      try
      {
        this.caesarChannel = sapi.Channels.OpenCaesarChannel(this.resource, false);
        this.caesarChannel.InitByteLevelNoInit();
        this.caesarMonitoring = this.caesarChannel.OpenMonitoringAdvanced();
        int result;
        if (int.TryParse(this.caesarChannel.GetComParameter("CP_BAUDRATE"), out result))
          this.BaudRate = result;
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        throw new CaesarException(ex, negativeResponseCode);
      }
      if (!this.caesarMonitoring.StartCanMonitoring())
        throw new CaesarException(SapiError.FailedToStartCaesarMonitoring);
    }

    protected internal override IEnumerable<BusMonitorFrame> ReadFrames()
    {
      List<CanMonitoringEntry> list = this.caesarMonitoring.GetCanMonitoringData((int) (ushort) this.framesForEvent).ToList<CanMonitoringEntry>();
      DateTime now = Sapi.Now;
      if (now > this.lastStatsCheck + TimeSpan.FromSeconds(1.0))
      {
        if (this.caesarMonitoring.Statistics == null && list.Count == 0)
          throw new CaesarException(SapiError.UnableToRetrieveCanMonitoringStatistics);
        this.lastStatsCheck = now;
      }
      return list.Select<CanMonitoringEntry, BusMonitorFrame>((Func<CanMonitoringEntry, BusMonitorFrame>) (m => new BusMonitorFrame(m)));
    }

    protected internal override void Cleanup()
    {
      if (this.caesarMonitoring != null)
      {
        this.caesarMonitoring.StopCanMonitoring();
        ((CaesarHandle\u003CCaesar\u003A\u003AMonitorHandleStruct\u0020\u002A\u003E) this.caesarMonitoring).Dispose();
        this.caesarMonitoring = (CaesarMonitoring) null;
      }
      if (this.caesarChannel == null)
        return;
      this.caesarChannel.ExitByteLevelNoExit();
      ((CaesarHandle\u003CCaesar\u003A\u003AChannelHandleStruct\u0020\u002A\u003E) this.caesarChannel).Dispose();
      this.caesarChannel = (CaesarChannel) null;
    }
  }

  private class BusMonitorMcd : BusMonitor
  {
    private McdMonitoringLink mcdMonitoring;

    protected internal override void Prepare()
    {
      try
      {
        this.mcdMonitoring = this.networkInterface == null ? McdRoot.CreateMonitoringLink(this.resource.MCDResourceQualifier) : McdRoot.CreateDoIPMonitorLink(this.networkInterface, this.path);
        this.mcdMonitoring.Start();
      }
      catch (McdException ex)
      {
        throw new CaesarException(ex);
      }
    }

    protected internal override IEnumerable<BusMonitorFrame> ReadFrames()
    {
      try
      {
        return this.networkInterface != null ? ((IEnumerable<string>) this.mcdMonitoring.FetchMonitoringFrames(this.framesForEvent)).SelectMany<string, BusMonitorFrame>((Func<string, IEnumerable<BusMonitorFrame>>) (m => BusMonitorFrame.FromDoIPString(m))) : ((IEnumerable<string>) this.mcdMonitoring.FetchMonitoringFrames(this.framesForEvent)).Select<string, BusMonitorFrame>((Func<string, BusMonitorFrame>) (m => BusMonitorFrame.FromString(m, BusMonitorFrameStringFormat.Mcd, new int?(this.resource.PortIndex))));
      }
      catch (McdException ex)
      {
        throw new CaesarException(ex);
      }
    }

    protected internal override void Cleanup()
    {
      if (this.mcdMonitoring == null)
        return;
      this.mcdMonitoring.Stop();
      this.mcdMonitoring.Dispose();
      this.mcdMonitoring = (McdMonitoringLink) null;
    }
  }

  private class BusMonitorPassThru : BusMonitor
  {
    private uint channelId;
    private uint filterID;
    private bool haveAttemptedPassthruBaudRateRead;

    protected internal override void Prepare()
    {
      this.haveAttemptedPassthruBaudRateRead = false;
      Sapi.GetSapi();
      if (Sid.Connect((ProtocolId) (5 | this.resource.PortIndex - 1 << 28), 0U, ref this.channelId) != J2534Error.NoError)
        throw new CaesarException(SapiError.CannotConnectToPassThruDevice, Sid.GetLastError());
      int num1 = (int) Sid.SetUseConnectionMutex(this.channelId, (byte) 0);
      if (this.resource.BaudRate != 0)
      {
        int num2 = (int) Sid.SetBaudRate(this.channelId, (uint) this.resource.BaudRate);
      }
      Tuple<byte[], byte[]> tuple = new Tuple<byte[], byte[]>(new byte[6], new byte[6]);
      if (Sid.StartMsgFilter(this.channelId, FilterType.Pass, new PassThruMsg(ProtocolId.Can, tuple.Item1), new PassThruMsg(ProtocolId.Can, tuple.Item2), (PassThruMsg) null, ref this.filterID) != J2534Error.NoError)
        throw new CaesarException(SapiError.CannotStartMessageFilterForPassThruDevice, Sid.GetLastError());
      int pass = (int) Sid.SetAllFiltersToPass(this.channelId, (byte) 1);
      this.ProvidesCustomFrameTypes = Sid.SetProvideCustomMessageTypes(this.channelId, (byte) 7) == J2534Error.NoError;
    }

    protected internal override IEnumerable<BusMonitorFrame> ReadFrames()
    {
      List<PassThruMsg> passThruMsgList = new List<PassThruMsg>();
      switch (Sid.ReadMsgs(this.channelId, (IList<PassThruMsg>) passThruMsgList, (uint) this.framesForEvent, 0U))
      {
        case J2534Error.NoError:
        case J2534Error.Timeout:
          if (passThruMsgList.Count > 0)
          {
            if (this.ProvidesCustomFrameTypes)
            {
              PassThruMsg passThruMsg = passThruMsgList.LastOrDefault<PassThruMsg>((Func<PassThruMsg, bool>) (m => m.IsBaudRate));
              if (passThruMsg != null)
              {
                byte[] data = passThruMsg.GetData();
                this.BaudRate = ((int) data[0] << 24) + ((int) data[1] << 16 /*0x10*/) + ((int) data[2] << 8) + (int) data[3];
                this.haveAttemptedPassthruBaudRateRead = true;
              }
            }
            if (!this.haveAttemptedPassthruBaudRateRead)
            {
              this.haveAttemptedPassthruBaudRateRead = true;
              uint baudRate1 = 0;
              int baudRate2 = (int) Sid.GetBaudRate(this.channelId, ref baudRate1);
              if (baudRate1 != 0U)
                this.BaudRate = (int) baudRate1;
            }
            return passThruMsgList.Select<PassThruMsg, BusMonitorFrame>((Func<PassThruMsg, BusMonitorFrame>) (m => new BusMonitorFrame(m, this.resource.PortIndex)));
          }
          break;
        case J2534Error.BufferEmpty:
          uint errorCode = 0;
          if (Sid.GetErrorCode(this.channelId, ref errorCode) == J2534Error.NoError && errorCode == 61514U)
            throw new CaesarException(SapiError.HardwareNotResponding);
          break;
        default:
          throw new CaesarException(SapiError.PassThruDeviceError, Sid.GetLastError());
      }
      return (IEnumerable<BusMonitorFrame>) new BusMonitorFrame[0];
    }

    protected internal override void Cleanup()
    {
      if (this.channelId == 0U)
        return;
      J2534Error j2534Error = Sid.StopMsgFilter(this.channelId, this.filterID);
      if (j2534Error != J2534Error.NoError)
        Sapi.GetSapi().RaiseDebugInfoEvent((object) ProtocolId.Can, $"Result from J2534.StopMsgFilter() is {j2534Error.ToString()} GetLastError is {Sid.GetLastError()}");
      int num = (int) Sid.Disconnect(this.channelId);
      this.channelId = 0U;
    }
  }
}
