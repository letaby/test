// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.HistoryFileKey
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using System;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public class HistoryFileKey
{
  private DateTime creationDate;
  private string reason;

  public DateTime CreationDate => this.creationDate;

  public string Reason => this.reason;

  public HistoryFileKey(DateTime dateTime, string reason)
  {
    this.creationDate = dateTime;
    this.reason = reason;
  }
}
