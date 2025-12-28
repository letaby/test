// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ABS___Valve_Activation__NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ABS___Valve_Activation__NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 202;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ABS - Valve Activation (NGC).panel";
    }
  }

  public virtual string Guid => "ada06140-d934-457a-ac23-ecc588f7b068";

  public virtual string DisplayName => "ABS Valve Activation";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ABS02T",
        "SSAM02T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Anti-lock Braking System";

  public virtual FilterTypes Filters => (FilterTypes) 8;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[13]
      {
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
        new Qualifier((QualifierTypes) 1, "J1939-0", "DT_84"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_F_StartRoutine_Start"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_E_StartRoutine_Start"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_D_StartRoutine_Start"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_C_StartRoutine_Start"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_Hold_Trailer_Control_Pressure_StartRoutine_Start"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_B_StartRoutine_Start"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_A_StartRoutine_Start"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_3_2_Solenoid_valve_A_actuate_StartRoutine_Start"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_3_2_Solenoid_valve_B_actuate_StartRoutine_Start"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_PressCrcut1_Stat_EAPU"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_PressCrcut2_Stat_EAPU")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[9]
      {
        new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_F_StartRoutine_Start", (IEnumerable<string>) new string[1]
        {
          "Timing=2000"
        }),
        new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_E_StartRoutine_Start", (IEnumerable<string>) new string[1]
        {
          "Timing=2000"
        }),
        new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_D_StartRoutine_Start", (IEnumerable<string>) new string[1]
        {
          "Timing=2000"
        }),
        new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_C_StartRoutine_Start", (IEnumerable<string>) new string[1]
        {
          "Timing=2000"
        }),
        new ServiceCall("ABS02T", "RT_Hold_Trailer_Control_Pressure_StartRoutine_Start", (IEnumerable<string>) new string[1]
        {
          "Timing=2000"
        }),
        new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_B_StartRoutine_Start", (IEnumerable<string>) new string[1]
        {
          "Timing=2000"
        }),
        new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_A_StartRoutine_Start", (IEnumerable<string>) new string[1]
        {
          "Timing=2000"
        }),
        new ServiceCall("ABS02T", "RT_3_2_Solenoid_valve_A_actuate_StartRoutine_Start", (IEnumerable<string>) new string[1]
        {
          "Timing=2000"
        }),
        new ServiceCall("ABS02T", "RT_3_2_Solenoid_valve_B_actuate_StartRoutine_Start", (IEnumerable<string>) new string[1]
        {
          "Timing=2000"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ABS02T" };
  }
}
