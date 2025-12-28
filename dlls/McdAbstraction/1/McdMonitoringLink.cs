// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdMonitoringLink
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;

#nullable disable
namespace McdAbstraction;

public class McdMonitoringLink : IDisposable
{
  private MCDMonitoringLink link;
  private bool isLoggingEthernet;
  private bool disposedValue = false;

  internal McdMonitoringLink(MCDMonitoringLink link, string path)
  {
    if (path != null)
    {
      this.isLoggingEthernet = true;
      ((DtsMonitoringLink) link).OpenFileTrace(path);
    }
    this.link = link;
  }

  public void Start()
  {
    try
    {
      if (this.isLoggingEthernet)
        ((DtsMonitoringLink) this.link).StartFileTrace();
      this.link.Start();
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, nameof (Start));
    }
  }

  public void Stop()
  {
    this.link.Stop();
    if (!this.isLoggingEthernet)
      return;
    ((DtsMonitoringLink) this.link).StopFileTrace();
  }

  public string[] FetchMonitoringFrames(int numberRequired)
  {
    try
    {
      return this.link.FetchMonitoringFrames((uint) numberRequired);
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, "FetchMonitoringFrame");
    }
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing && this.link != null)
    {
      if (this.isLoggingEthernet)
        ((DtsMonitoringLink) this.link).CloseFileTrace();
      McdRoot.RemoveMonitoringLink(this.link);
      this.link.Dispose();
      this.link = (MCDMonitoringLink) null;
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
