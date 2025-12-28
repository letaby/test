// Decompiled with JetBrains decompiler
// Type: SapiLayer1.LogFileChannelCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public sealed class LogFileChannelCollection : ChannelBaseCollection
{
  private LogFile parent;

  internal LogFileChannelCollection(LogFile parent) => this.parent = parent;

  internal void Add(Channel c)
  {
    this.Items.Add(c);
    this.RaiseConnectCompleteEvent((object) c, (Exception) null);
  }

  internal bool ChannelExists(Channel c) => this.Items.IndexOf(c) != -1;

  public LogFile LogFile => this.parent;

  internal new object SyncRoot => (object) this.Items;
}
