// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__EPA07_.panel.SetupInformation
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__EPA07_.panel;

public class SetupInformation
{
  public SetupInformation(
    double egrMinimumPosition,
    int initialIdleSpeed,
    int runOffIdleSpeed,
    int thermalWaitLength)
  {
    this.EGRMinimumPosition = egrMinimumPosition;
    this.TestIdleSpeed = initialIdleSpeed;
    this.RunOffIdleSpeed = runOffIdleSpeed;
    this.TestDurationThermalCondition = TimeSpan.FromMinutes((double) thermalWaitLength);
  }

  public double EGRMinimumPosition { get; set; }

  public int TestIdleSpeed { get; set; }

  public int RunOffIdleSpeed { get; set; }

  public TimeSpan TestDurationThermalCondition { get; set; }
}
