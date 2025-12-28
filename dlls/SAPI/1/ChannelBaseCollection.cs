// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ChannelBaseCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public abstract class ChannelBaseCollection : 
  ReadOnlyCollection<Channel>,
  IEnumerable<Channel>,
  IEnumerable
{
  internal ChannelBaseCollection()
    : base((IList<Channel>) new List<Channel>())
  {
  }

  internal Channel InternalConnectOffline(
    DiagnosisVariant diagnosisVariant,
    LogFile lf,
    ConnectionResource resource = null)
  {
    Channel channel = (Channel) null;
    if (diagnosisVariant.Ecu.RollCallManager != null)
      channel = new Channel(diagnosisVariant, this, lf);
    else if (diagnosisVariant.Ecu.IsMcd)
    {
      string protocolName = diagnosisVariant.Ecu.ProtocolName;
      if (resource != null && resource.Interface != null)
        protocolName = resource.Interface.ProtocolName;
      channel = new Channel(diagnosisVariant.GetMcdDBLocationForProtocol(protocolName), diagnosisVariant, this, lf);
    }
    else
    {
      CaesarEcu ecuh = diagnosisVariant.OpenEcuHandle();
      if (ecuh != null)
        channel = new Channel(ecuh, diagnosisVariant, this, lf);
    }
    if (channel != null)
    {
      lock (this.Items)
        this.Items.Add(channel);
    }
    return channel;
  }

  internal virtual void Remove(Channel c)
  {
    lock (this.Items)
      this.Items.Remove(c);
    FireAndForget.Invoke((MulticastDelegate) this.DisconnectCompleteEvent, (object) c, new EventArgs());
  }

  internal void RaiseConnectProgressEvent(ConnectionResource cr, double pc)
  {
    FireAndForget.Invoke((MulticastDelegate) this.ConnectProgressEvent, (object) cr, (EventArgs) new ProgressEventArgs(pc));
  }

  internal void RaiseConnectCompleteEvent(object sender, Exception ce)
  {
    this.RaiseConnectCompleteEvent(sender, (BackgroundConnect) null, ce);
  }

  internal void RaiseConnectCompleteEvent(
    object sender,
    BackgroundConnect backgroundConnect,
    Exception ce)
  {
    ConnectCompleteEventArgs e = new ConnectCompleteEventArgs(backgroundConnect, ce);
    FireAndForget.Invoke((MulticastDelegate) this.ConnectCompleteEvent, sender, (EventArgs) e);
    FireAndForget.Invoke((MulticastDelegate) this.ConnectCompleteEvent2, sender, (EventArgs) e);
  }

  internal Channel this[DiagnosisVariant index]
  {
    get
    {
      return this.Where<Channel>((Func<Channel, bool>) (channel => channel.DiagnosisVariant.Equals((object) index))).FirstOrDefault<Channel>();
    }
  }

  public new IEnumerator<Channel> GetEnumerator()
  {
    lock (this.Items)
      return (IEnumerator<Channel>) new List<Channel>((IEnumerable<Channel>) this.Items).GetEnumerator();
  }

  IEnumerator<Channel> IEnumerable<Channel>.GetEnumerator() => this.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
  public object SyncRoot => (object) this.Items;

  public event ConnectProgressEventHandler ConnectProgressEvent;

  public event ConnectCompleteEventHandler ConnectCompleteEvent;

  public event EventHandler<ConnectCompleteEventArgs> ConnectCompleteEvent2;

  public event DisconnectCompleteEventHandler DisconnectCompleteEvent;
}
