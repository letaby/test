// Decompiled with JetBrains decompiler
// Type: SapiLayer1.BusMonitorCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace SapiLayer1;

public sealed class BusMonitorCollection : ReadOnlyCollection<BusMonitor>
{
  internal BusMonitorCollection()
    : base((IList<BusMonitor>) new List<BusMonitor>())
  {
  }

  private void Add(BusMonitor busMonitor) => this.Items.Add(busMonitor);

  internal void Remove(BusMonitor busMonitor) => this.Items.Remove(busMonitor);

  public BusMonitor Connect(ConnectionResource resource, Control threadMarshalControl)
  {
    if (this.FirstOrDefault<BusMonitor>((Func<BusMonitor, bool>) (bm => bm.Resource == resource)) != null)
      throw new InvalidOperationException("Already connected");
    BusMonitor busMonitor = BusMonitor.Create(resource, threadMarshalControl);
    this.Add(busMonitor);
    return busMonitor;
  }

  public BusMonitor Connect(string networkInterface, string path, Control threadMarshalControl)
  {
    if (this.FirstOrDefault<BusMonitor>((Func<BusMonitor, bool>) (bm => bm.NetworkInterface == networkInterface)) != null)
      throw new InvalidOperationException("Already connected");
    BusMonitor busMonitor = BusMonitor.Create(networkInterface, path, threadMarshalControl);
    this.Add(busMonitor);
    return busMonitor;
  }

  public static ConnectionResourceCollection GetAvailableMonitoringResources()
  {
    Sapi sapi = Sapi.GetSapi();
    ConnectionResourceCollection monitoringResources = new ConnectionResourceCollection();
    DiagnosisProtocol diagnosisProtocol = sapi.DiagnosisProtocols["UDS"];
    if (diagnosisProtocol != null)
    {
      foreach (ConnectionResource connectionResource in (ReadOnlyCollection<ConnectionResource>) diagnosisProtocol.GetConnectionResources(byte.MaxValue))
        monitoringResources.Add(connectionResource);
    }
    if (McdRoot.Initialized)
    {
      foreach (ConnectionResource connectionResource in (ReadOnlyCollection<ConnectionResource>) sapi.DiagnosisProtocols["RAW_CAN_D"].GetConnectionResources(byte.MaxValue))
        monitoringResources.Add(connectionResource);
    }
    return monitoringResources;
  }
}
