// Decompiled with JetBrains decompiler
// Type: Softing.Dts.PrimitiveCanceledFromQueueArgs
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public class PrimitiveCanceledFromQueueArgs : EventArgs
{
  private MCDDiagComPrimitive m_primitive;
  private MCDLogicalLink m_link;

  public PrimitiveCanceledFromQueueArgs(MCDDiagComPrimitive primitive, MCDLogicalLink link)
  {
    this.m_primitive = primitive;
    this.m_link = link;
  }

  public MCDDiagComPrimitive Primitive => this.m_primitive;

  public MCDLogicalLink Link => this.m_link;
}
