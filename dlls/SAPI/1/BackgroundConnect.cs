// Decompiled with JetBrains decompiler
// Type: SapiLayer1.BackgroundConnect
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

#nullable disable
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

  internal BackgroundConnect(
    ChannelCollection parent,
    CaesarChannel ch,
    ConnectionResource res,
    ChannelOptions options = ChannelOptions.All,
    bool autoConnect = false)
  {
    this.cCHToInit = ch;
    this.resToInit = res;
    this.parent = parent;
    this.autoConnect = autoConnect;
    this.ChannelOptions = options;
  }

  internal BackgroundConnect(
    ChannelCollection parent,
    McdLogicalLink ll,
    ConnectionResource res,
    ChannelOptions options = ChannelOptions.All,
    bool autoConnect = false)
  {
    this.logicalLinkToInit = ll;
    this.resToInit = res;
    this.parent = parent;
    this.autoConnect = autoConnect;
    this.ChannelOptions = options;
  }

  internal ChannelOptions ChannelOptions { get; }

  internal bool AutoConnect => this.autoConnect;

  internal Thread BackgroundThread => this.thread;

  internal Channel Channel => this.channelCreated;

  internal CaesarChannel CaesarChannel => this.cCHToInit;

  internal McdLogicalLink MCDLogicalLink => this.logicalLinkToInit;

  internal bool Initialised => this.initialised;

  internal bool VariantMatched => this.variantMatched;

  internal CaesarException CaesarException => this.channelCreationError;

  internal ConnectionResource ConnectionResource => this.resToInit;

  internal BackgroundConnect Child { get; set; }

  internal void Start()
  {
    this.thread = new Thread(new ThreadStart(this.ThreadFunc));
    this.thread.Name = $"{this.GetType().Name}: {this.resToInit.Ecu.Name} {this.resToInit.ToString()}";
    this.thread.Start();
  }

  private void ThreadFunc()
  {
    if (this.cCHToInit != null)
      this.ThreadFuncCaesar();
    else
      this.ThreadFuncMCD();
  }

  private void ThreadFuncMCD()
  {
    try
    {
      Sapi sapi = Sapi.GetSapi();
      if (this.logicalLinkToInit.IsEthernet && !RollCallDoIP.GlobalInstance.ProtocolAlive && !sapi.Channels.Any<Channel>((Func<Channel, bool>) (ch => ch.McdChannelHandle != null && ch.McdChannelHandle.IsEthernet)))
      {
        RollCallDoIP.GlobalInstance.ActivateInterface(true);
        McdRoot.DetectInterfaces(sapi.ConfigurationItems["McdEthernetDetectionString"].Value);
      }
      this.logicalLinkToInit.OpenLogicalLink((this.ChannelOptions & ChannelOptions.StartStopCommunications) != 0);
      if (!this.aborting)
      {
        this.initialised = this.logicalLinkToInit.State == this.logicalLinkToInit.TargetState;
        if (this.initialised)
        {
          if (!this.resToInit.Ecu.IsByteMessaging)
          {
            this.logicalLinkToInit.IdentifyVariant();
            this.variantMatched = this.logicalLinkToInit.VariantName != null;
          }
          else
            this.variantMatched = true;
        }
        else
          this.channelCreationError = new CaesarException(SapiError.PendingConnectionTargetStateNotReached);
      }
      else
        this.channelCreationError = new CaesarException(SapiError.PendingConnectionAborted);
    }
    catch (McdException ex)
    {
      this.channelCreationError = new CaesarException(ex);
    }
    if (!this.initialised)
    {
      lock (this.handleToInitLock)
      {
        if (this.logicalLinkToInit != null)
        {
          this.logicalLinkToInit.Dispose();
          this.logicalLinkToInit = (McdLogicalLink) null;
        }
      }
    }
    if (this.parent == null)
      return;
    this.channelCreated = this.parent.ConnectComplete(this);
    lock (this.handleToInitLock)
      this.logicalLinkToInit = (McdLogicalLink) null;
  }

  private void ThreadFuncCaesar()
  {
    CaesarException caesarException = (CaesarException) null;
    bool flag = !string.IsNullOrEmpty(this.resToInit.Ecu.Properties["FixedDiagnosisVariant"]);
    if (!this.aborting)
    {
      if ((this.ChannelOptions & ChannelOptions.StartStopCommunications) == ChannelOptions.None)
      {
        this.cCHToInit.SetComParameter("CPI_INITTYPE", "0");
        this.cCHToInit.SetComParameter("CPI_EXITTYPE", "0");
      }
      if (flag)
        this.cCHToInit.SetComParameter("CPI_READIDBLOCK", "0");
      Type extensionType = this.resToInit.Ecu.ExtensionType;
      if (extensionType != (Type) null)
      {
        MethodInfo method = extensionType.GetMethod("PreConnect");
        if (method != (MethodInfo) null)
          this.initialised = (bool) method.Invoke((object) null, new object[1]
          {
            (object) this.cCHToInit
          });
      }
      if (!this.initialised)
        this.initialised = this.cCHToInit.Init();
      if (this.cCHToInit.IsErrorSet)
      {
        caesarException = new CaesarException(this.cCHToInit);
        if (caesarException.ErrorNumber == 3029L && this.resToInit.Ecu.Properties.ContainsKey("ExtendedSessionRetryComParameters"))
        {
          caesarException = (CaesarException) null;
          ChannelCollection.InitComParameterCollection(this.cCHToInit, this.resToInit.Ecu.Properties["ExtendedSessionRetryComParameters"]);
          this.initialised = this.cCHToInit.Init();
          if (this.cCHToInit.IsErrorSet)
            caesarException = new CaesarException(this.cCHToInit);
        }
      }
      if (caesarException != null)
        this.channelCreationError = caesarException;
    }
    else
      this.channelCreationError = new CaesarException(SapiError.CannotSendMessageToDevice);
    if (!this.initialised)
    {
      lock (this.handleToInitLock)
      {
        this.cCHToInit.Exit();
        ((CaesarHandle\u003CCaesar\u003A\u003AChannelHandleStruct\u0020\u002A\u003E) this.cCHToInit).Dispose();
        this.cCHToInit = (CaesarChannel) null;
      }
    }
    if (this.initialised)
    {
      CaesarEcu ecu = this.cCHToInit.Ecu;
      this.variantMatched = ecu == null || !string.IsNullOrEmpty(ecu.VariantName) | flag;
    }
    if (this.parent == null)
      return;
    this.channelCreated = this.parent.ConnectComplete(this);
    lock (this.handleToInitLock)
      this.cCHToInit = (CaesarChannel) null;
  }

  internal void SetConnectCompleteFailure(SapiError result)
  {
    this.initialised = false;
    lock (this.handleToInitLock)
    {
      if (this.cCHToInit != null)
      {
        this.cCHToInit.Exit();
        ((CaesarHandle\u003CCaesar\u003A\u003AChannelHandleStruct\u0020\u002A\u003E) this.cCHToInit).Dispose();
        this.cCHToInit = (CaesarChannel) null;
      }
      else if (this.logicalLinkToInit != null)
      {
        this.logicalLinkToInit.Dispose();
        this.logicalLinkToInit = (McdLogicalLink) null;
      }
    }
    this.channelCreationError = new CaesarException(result);
  }

  internal void Abort()
  {
    if (this.Child != null)
    {
      this.Child.Abort();
    }
    else
    {
      if (this.aborting)
        return;
      this.aborting = true;
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.AbortCore), (object) 0);
    }
  }

  private void AbortCore(object obj)
  {
    lock (this.handleToInitLock)
    {
      if (this.cCHToInit != null)
        this.cCHToInit.SetTimeout(1U);
      else if (this.logicalLinkToInit != null)
        this.logicalLinkToInit.Abort();
      else if (this.channelCreated != null)
      {
        this.channelCreated.Abort();
      }
      else
      {
        if (this.CaesarException != null)
          return;
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, "No CAESAR channel available to abort pending connection " + this.resToInit.Ecu.Name);
      }
    }
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
      lock (this.handleToInitLock)
      {
        if (this.cCHToInit != null)
        {
          this.cCHToInit.Exit();
          ((CaesarHandle\u003CCaesar\u003A\u003AChannelHandleStruct\u0020\u002A\u003E) this.cCHToInit).Dispose();
          this.cCHToInit = (CaesarChannel) null;
          this.initialised = false;
        }
        if (this.logicalLinkToInit != null)
        {
          this.logicalLinkToInit.Dispose();
          this.logicalLinkToInit = (McdLogicalLink) null;
          this.initialised = false;
        }
      }
    }
    this.disposed = true;
  }
}
