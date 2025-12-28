// Decompiled with JetBrains decompiler
// Type: Softing.Dts.PrimitiveHasResultArgs
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public class PrimitiveHasResultArgs : EventArgs
{
  private MCDDiagComPrimitive m_primitive;
  private MCDLogicalLink m_link;
  private MCDResultState m_resultstate;

  public PrimitiveHasResultArgs(
    MCDDiagComPrimitive primitive,
    MCDLogicalLink link,
    MCDResultState resultstate)
  {
    this.m_primitive = primitive;
    this.m_link = link;
    this.m_resultstate = resultstate;
  }

  public MCDDiagComPrimitive Primitive => this.m_primitive;

  public MCDLogicalLink Link => this.m_link;

  public MCDResultState Resultstate => this.m_resultstate;
}
