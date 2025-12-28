// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdInterfaceResource
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;

#nullable disable
namespace McdAbstraction;

public class McdInterfaceResource : IDisposable
{
  private MCDInterfaceResource theInterfaceResource;
  private bool disposedValue = false;

  internal McdInterfaceResource(MCDInterfaceResource theInterfaceResource)
  {
    this.theInterfaceResource = theInterfaceResource;
    this.ProtocolType = theInterfaceResource.ProtocolType;
    this.Name = theInterfaceResource.LongName;
    this.Qualifier = theInterfaceResource.ShortName;
    this.IsPassThru = theInterfaceResource.Interface.PDUApiSoftwareName == "PDUAPI_DTNA_SID";
    switch (((DtsDbPhysicalVehicleLinkOrInterface) theInterfaceResource.DbPhysicalInterfaceLink).PILType)
    {
      case DtsPhysicalLinkOrInterfaceType.eCAN:
        this.PhysicalInterfaceLinkType = "CAN";
        break;
      case DtsPhysicalLinkOrInterfaceType.eCANHS:
        this.PhysicalInterfaceLinkType = "CANHS";
        break;
      case DtsPhysicalLinkOrInterfaceType.eCANLS:
        this.PhysicalInterfaceLinkType = "CANLS";
        break;
      case DtsPhysicalLinkOrInterfaceType.eETHERNET:
        this.PhysicalInterfaceLinkType = "ETHERNET";
        this.IsEthernet = true;
        break;
    }
  }

  public bool IsPassThru { get; }

  public bool IsEthernet { get; }

  public string PhysicalInterfaceLinkType { get; private set; }

  public string ProtocolType { get; private set; }

  public string Qualifier { get; private set; }

  public string Name { get; private set; }

  internal MCDInterfaceResource Handle => this.theInterfaceResource;

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing && this.theInterfaceResource != null)
    {
      this.theInterfaceResource.Dispose();
      this.theInterfaceResource = (MCDInterfaceResource) null;
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
