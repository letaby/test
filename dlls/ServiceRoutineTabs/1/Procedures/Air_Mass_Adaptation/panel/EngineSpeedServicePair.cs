// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel.EngineSpeedServicePair
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

internal sealed class EngineSpeedServicePair
{
  public readonly string StartModificationService;
  public readonly string StopModificationService;
  public readonly int TargetSpeed;
  public readonly int HoldTimeSeconds;
  public readonly TimeSpan HoldTimeSpan;

  public EngineSpeedServicePair(string start, string stop, int targetSpeed, int holdTimeSeconds)
  {
    this.StartModificationService = start;
    this.StopModificationService = stop;
    this.TargetSpeed = targetSpeed;
    this.HoldTimeSeconds = holdTimeSeconds;
    this.HoldTimeSpan = new TimeSpan(0, 0, holdTimeSeconds);
  }
}
