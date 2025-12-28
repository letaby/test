// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FatalEventArgs
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public sealed class FatalEventArgs : EventArgs
{
  private int param1;
  private int param2;
  private string message;
  private string fileName;
  private int line;
  private DateTime timestamp;

  internal FatalEventArgs(int param1, int param2, string message, string fileName, int line)
  {
    this.param1 = param1;
    this.param2 = param2;
    this.message = message;
    this.fileName = fileName;
    this.line = line;
    this.timestamp = Sapi.Now;
  }

  public int Param1 => this.param1;

  public int Param2 => this.param2;

  public string Message => this.message;

  public string FileName => this.fileName;

  public int Line => this.line;

  public DateTime Timestamp => this.timestamp;
}
