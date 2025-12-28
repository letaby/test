// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdInterface
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdInterface : IDisposable
{
  private MCDInterface theInterface;
  private IEnumerable<McdInterfaceResource> resources;
  private bool disposedValue = false;

  internal McdInterface(MCDInterface theInterface)
  {
    this.theInterface = theInterface;
    if (this.theInterface.Status != MCDInterfaceStatus.eAVAILABLE)
      return;
    this.theInterface.Connect();
  }

  public string Qualifier => this.theInterface.ShortName;

  public string Name => this.theInterface.LongName;

  public string HardwareName => this.theInterface.HardwareName;

  public IEnumerable<McdInterfaceResource> Resources
  {
    get
    {
      if (this.resources == null && this.theInterface != null)
        this.resources = (IEnumerable<McdInterfaceResource>) this.theInterface.InterfaceResources.OfType<MCDInterfaceResource>().Select<MCDInterfaceResource, McdInterfaceResource>((Func<MCDInterfaceResource, McdInterfaceResource>) (r => new McdInterfaceResource(r))).ToList<McdInterfaceResource>();
      return this.resources;
    }
  }

  internal MCDInterface Handle => this.theInterface;

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing && this.theInterface != null)
    {
      if (this.theInterface.Status == MCDInterfaceStatus.eREADY)
        this.theInterface.Disconnect();
      this.theInterface.Dispose();
      this.theInterface = (MCDInterface) null;
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
