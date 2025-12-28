// Decompiled with JetBrains decompiler
// Type: SapiLayer1.DebugInfoEventArgs
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public sealed class DebugInfoEventArgs : EventArgs
{
  private string message;
  private DateTime timestamp;

  internal DebugInfoEventArgs(string msg)
  {
    this.message = msg;
    this.timestamp = Sapi.Now;
  }

  public string Message => this.message;

  public DateTime Timestamp => this.timestamp;
}
