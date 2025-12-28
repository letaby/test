// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ChannelExtension
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public abstract class ChannelExtension : IDisposable
{
  private Channel channel;

  ~ChannelExtension() => this.Dispose(false);

  public abstract object Invoke(string method, object[] inputs);

  public virtual void PrepareVcp()
  {
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected ChannelExtension(Channel channel) => this.channel = channel;

  protected virtual void Dispose(bool disposing)
  {
  }

  protected void RaiseExceptionEvent(Exception exception)
  {
    Sapi.GetSapi().RaiseExceptionEvent((object) this, exception);
  }

  protected void RaiseDebugInfoEvent(string message)
  {
    Sapi.GetSapi().RaiseDebugInfoEvent((object) this, message);
  }

  protected static void RaiseDebugInfoEvent(object sender, string message)
  {
    Sapi.GetSapi().RaiseDebugInfoEvent(sender, message);
  }

  protected Channel Channel => this.channel;
}
