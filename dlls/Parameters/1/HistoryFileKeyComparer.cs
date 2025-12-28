// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.HistoryFileKeyComparer
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public class HistoryFileKeyComparer : IComparer<HistoryFileKey>
{
  public int Compare(HistoryFileKey x, HistoryFileKey y)
  {
    return x.CreationDate == y.CreationDate ? string.CompareOrdinal(x.Reason, y.Reason) : x.CreationDate.CompareTo(y.CreationDate);
  }
}
