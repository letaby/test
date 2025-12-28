// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ResultEventArgs
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public class ResultEventArgs : EventArgs
{
  private Exception exception;
  private DateTime timestamp;

  public ResultEventArgs(Exception exception)
  {
    this.exception = exception;
    this.timestamp = Sapi.Now;
  }

  public Exception Exception => this.exception;

  public bool Succeeded => this.exception == null;

  public DateTime Timestamp => this.timestamp;
}
