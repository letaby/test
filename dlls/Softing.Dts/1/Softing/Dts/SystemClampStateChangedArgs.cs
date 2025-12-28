// Decompiled with JetBrains decompiler
// Type: Softing.Dts.SystemClampStateChangedArgs
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public class SystemClampStateChangedArgs : EventArgs
{
  private string m_clamp;
  private MCDClampState m_clampState;

  public SystemClampStateChangedArgs(string clamp, MCDClampState clampState)
  {
    this.m_clamp = clamp;
    this.m_clampState = clampState;
  }

  public string Clamp => this.m_clamp;

  public MCDClampState ClampState => this.m_clampState;
}
