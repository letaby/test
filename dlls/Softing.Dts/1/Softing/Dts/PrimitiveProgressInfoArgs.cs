// Decompiled with JetBrains decompiler
// Type: Softing.Dts.PrimitiveProgressInfoArgs
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public class PrimitiveProgressInfoArgs : EventArgs
{
  private MCDDiagComPrimitive m_primitive;
  private MCDLogicalLink m_link;
  private byte m_progress;

  public PrimitiveProgressInfoArgs(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    byte progress)
  {
    this.m_primitive = primitive;
    this.m_link = link;
    this.m_progress = progress;
  }

  public MCDDiagComPrimitive Primitive => this.m_primitive;

  public MCDLogicalLink Link => this.m_link;

  public byte Progress => this.m_progress;
}
