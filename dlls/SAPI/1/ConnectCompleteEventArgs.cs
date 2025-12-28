// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ConnectCompleteEventArgs
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public class ConnectCompleteEventArgs : ResultEventArgs
{
  internal ConnectCompleteEventArgs(BackgroundConnect backgroundConnect, Exception exception)
    : base(exception)
  {
    this.AutoConnect = backgroundConnect != null && backgroundConnect.AutoConnect;
    this.ChannelOptions = backgroundConnect != null ? backgroundConnect.ChannelOptions : ChannelOptions.All;
  }

  public bool AutoConnect { get; }

  public ChannelOptions ChannelOptions { get; }
}
