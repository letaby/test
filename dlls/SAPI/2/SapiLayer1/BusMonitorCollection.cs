using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using McdAbstraction;

namespace SapiLayer1;

public sealed class BusMonitorCollection : ReadOnlyCollection<BusMonitor>
{
	internal BusMonitorCollection()
		: base((IList<BusMonitor>)new List<BusMonitor>())
	{
	}

	private void Add(BusMonitor busMonitor)
	{
		base.Items.Add(busMonitor);
	}

	internal void Remove(BusMonitor busMonitor)
	{
		base.Items.Remove(busMonitor);
	}

	public BusMonitor Connect(ConnectionResource resource, Control threadMarshalControl)
	{
		BusMonitor busMonitor = this.FirstOrDefault((BusMonitor bm) => bm.Resource == resource);
		if (busMonitor != null)
		{
			throw new InvalidOperationException("Already connected");
		}
		busMonitor = BusMonitor.Create(resource, threadMarshalControl);
		Add(busMonitor);
		return busMonitor;
	}

	public BusMonitor Connect(string networkInterface, string path, Control threadMarshalControl)
	{
		BusMonitor busMonitor = this.FirstOrDefault((BusMonitor bm) => bm.NetworkInterface == networkInterface);
		if (busMonitor != null)
		{
			throw new InvalidOperationException("Already connected");
		}
		busMonitor = BusMonitor.Create(networkInterface, path, threadMarshalControl);
		Add(busMonitor);
		return busMonitor;
	}

	public static ConnectionResourceCollection GetAvailableMonitoringResources()
	{
		Sapi sapi = Sapi.GetSapi();
		ConnectionResourceCollection connectionResourceCollection = new ConnectionResourceCollection();
		DiagnosisProtocol diagnosisProtocol = sapi.DiagnosisProtocols["UDS"];
		if (diagnosisProtocol != null)
		{
			foreach (ConnectionResource connectionResource in diagnosisProtocol.GetConnectionResources(byte.MaxValue))
			{
				connectionResourceCollection.Add(connectionResource);
			}
		}
		if (McdRoot.Initialized)
		{
			foreach (ConnectionResource connectionResource2 in sapi.DiagnosisProtocols["RAW_CAN_D"].GetConnectionResources(byte.MaxValue))
			{
				connectionResourceCollection.Add(connectionResource2);
			}
		}
		return connectionResourceCollection;
	}
}
