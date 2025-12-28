using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CaesarAbstraction;

namespace SapiLayer1;

public abstract class ChannelBaseCollection : ReadOnlyCollection<Channel>, IEnumerable<Channel>, IEnumerable
{
	internal Channel this[DiagnosisVariant index] => this.Where((Channel channel) => channel.DiagnosisVariant.Equals(index)).FirstOrDefault();

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("SyncRoot is deprecated and no longer necessary, because the collection returned by GetEnumerator is a (shallow) copy.")]
	public object SyncRoot => base.Items;

	public event ConnectProgressEventHandler ConnectProgressEvent;

	public event ConnectCompleteEventHandler ConnectCompleteEvent;

	public event EventHandler<ConnectCompleteEventArgs> ConnectCompleteEvent2;

	public event DisconnectCompleteEventHandler DisconnectCompleteEvent;

	internal ChannelBaseCollection()
		: base((IList<Channel>)new List<Channel>())
	{
	}

	internal Channel InternalConnectOffline(DiagnosisVariant diagnosisVariant, LogFile lf, ConnectionResource resource = null)
	{
		Channel channel = null;
		if (diagnosisVariant.Ecu.RollCallManager != null)
		{
			channel = new Channel(diagnosisVariant, this, lf);
		}
		else if (diagnosisVariant.Ecu.IsMcd)
		{
			string protocolName = diagnosisVariant.Ecu.ProtocolName;
			if (resource != null && resource.Interface != null)
			{
				protocolName = resource.Interface.ProtocolName;
			}
			channel = new Channel(diagnosisVariant.GetMcdDBLocationForProtocol(protocolName), diagnosisVariant, this, lf);
		}
		else
		{
			CaesarEcu val = diagnosisVariant.OpenEcuHandle();
			if (val != null)
			{
				channel = new Channel(val, diagnosisVariant, this, lf);
			}
		}
		if (channel != null)
		{
			lock (base.Items)
			{
				base.Items.Add(channel);
			}
		}
		return channel;
	}

	internal virtual void Remove(Channel c)
	{
		lock (base.Items)
		{
			base.Items.Remove(c);
		}
		FireAndForget.Invoke(this.DisconnectCompleteEvent, c, new EventArgs());
	}

	internal void RaiseConnectProgressEvent(ConnectionResource cr, double pc)
	{
		FireAndForget.Invoke(this.ConnectProgressEvent, cr, new ProgressEventArgs(pc));
	}

	internal void RaiseConnectCompleteEvent(object sender, Exception ce)
	{
		RaiseConnectCompleteEvent(sender, null, ce);
	}

	internal void RaiseConnectCompleteEvent(object sender, BackgroundConnect backgroundConnect, Exception ce)
	{
		ConnectCompleteEventArgs e = new ConnectCompleteEventArgs(backgroundConnect, ce);
		FireAndForget.Invoke(this.ConnectCompleteEvent, sender, e);
		FireAndForget.Invoke(this.ConnectCompleteEvent2, sender, e);
	}

	public new IEnumerator<Channel> GetEnumerator()
	{
		lock (base.Items)
		{
			return new List<Channel>(base.Items).GetEnumerator();
		}
	}

	IEnumerator<Channel> IEnumerable<Channel>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
