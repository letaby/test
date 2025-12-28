// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Coolant_Systems_Pressure_Test__EMG_.panel.CoolantTestStep
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Coolant_Systems_Pressure_Test__EMG_.panel;

public class CoolantTestStep
{
  public TestTypes TestType { get; set; }

  public string DisplayText { get; set; }

  public CoolantTestActions Action { get; set; }

  public CoolantTestStep(TestTypes testType, string displayText, CoolantTestActions action)
  {
    this.TestType = testType;
    this.DisplayText = displayText;
    this.Action = action;
  }
}
