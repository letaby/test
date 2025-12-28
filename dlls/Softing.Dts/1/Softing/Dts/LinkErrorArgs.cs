// Decompiled with JetBrains decompiler
// Type: Softing.Dts.LinkErrorArgs
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public class LinkErrorArgs : EventArgs
{
  private MCDLogicalLink m_link;
  private MCDError m_error;

  public LinkErrorArgs(MCDLogicalLink link, MCDError error)
  {
    this.m_link = link;
    this.m_error = error;
  }

  public MCDLogicalLink Link => this.m_link;

  public MCDError Error => this.m_error;
}
