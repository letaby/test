// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ByteMessageEventArgs
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public sealed class ByteMessageEventArgs : EventArgs
{
  private ByteMessageDirection direction;
  private Dump data;
  private DateTime timestamp;

  internal ByteMessageEventArgs(ByteMessageDirection direction, Dump data)
  {
    this.direction = direction;
    this.data = data;
    this.timestamp = Sapi.Now;
  }

  public ByteMessageDirection Direction => this.direction;

  public Dump Data => this.data;

  public DateTime Timestamp => this.timestamp;
}
