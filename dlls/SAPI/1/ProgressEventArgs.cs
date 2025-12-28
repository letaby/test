// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ProgressEventArgs
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public sealed class ProgressEventArgs : EventArgs
{
  private double percentComplete;

  internal ProgressEventArgs(double pc) => this.percentComplete = pc;

  public double PercentComplete => this.percentComplete;
}
