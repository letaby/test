// Decompiled with JetBrains decompiler
// Type: Softing.Dts.InterfaceDetectedArgs
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public class InterfaceDetectedArgs : EventArgs
{
  private MCDInterface m_interface_;

  public InterfaceDetectedArgs(MCDInterface interface_) => this.m_interface_ = interface_;

  public MCDInterface Interface_ => this.m_interface_;
}
