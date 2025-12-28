// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.IProvideProgrammingData
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Net;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public interface IProvideProgrammingData
{
  IEnumerable<DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgrammingData> ProgrammingData { get; set; }

  UnitInformation SelectedUnit { get; set; }

  bool RequiresCompatibilityChecks { get; }
}
