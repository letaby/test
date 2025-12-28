// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdFlashJob
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;

#nullable disable
namespace McdAbstraction;

public class McdFlashJob : IDisposable
{
  private MCDFlashJob job;
  private MCDLogicalLink link;
  private bool disposedValue = false;

  internal McdFlashJob(MCDLogicalLink link, MCDFlashJob flashJob)
  {
    this.job = flashJob;
    this.link = link;
  }

  public byte Progress => this.job.Progress;

  public bool Running => this.job.State == MCDDiagComPrimitiveState.ePENDING;

  public void Execute()
  {
    try
    {
      this.job.ExecuteAsync();
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (Execute));
    }
  }

  public void FetchResults()
  {
    using (MCDResult itemByIndex = this.job.FetchResults(0).GetItemByIndex(0U))
    {
      if (itemByIndex.HasError)
        throw new McdException(itemByIndex.Error, nameof (FetchResults));
    }
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
      this.link.RemoveDiagComPrimitive((MCDDiagComPrimitive) this.job);
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
