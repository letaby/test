// Decompiled with JetBrains decompiler
// Type: Softing.Dts.LinkActivityStateRunningArgs
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public class LinkActivityStateRunningArgs : EventArgs
{
  private MCDLogicalLink m_link;
  private MCDLogicalLinkState m_linkstate;

  public LinkActivityStateRunningArgs(MCDLogicalLink link, MCDLogicalLinkState linkstate)
  {
    this.m_link = link;
    this.m_linkstate = linkstate;
  }

  public MCDLogicalLink Link => this.m_link;

  public MCDLogicalLinkState Linkstate => this.m_linkstate;
}
