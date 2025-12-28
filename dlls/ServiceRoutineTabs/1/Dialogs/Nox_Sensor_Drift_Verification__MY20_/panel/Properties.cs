// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Nox_Sensor_Drift_Verification__MY20_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Nox_Sensor_Drift_Verification__MY20_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 209;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Nox Sensor Drift Verification (MY20).panel";
    }
  }

  public virtual string Guid => "7147a4a2-6d6d-4565-a46d-cfe2f9536475";

  public virtual string DisplayName => "NOx Sensor Verification";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[5]
      {
        "ACM301T",
        "CPC302T",
        "CPC501T",
        "CPC502T",
        "MCM21T"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[7]
      {
        "DDEC20-DD13",
        "DDEC20-DD15",
        "DDEC20-DD16",
        "New Cascadia CEEAce-Sleeper",
        "New Cascadia CEEAce-Daycab",
        "New Cascadia 2020-Daycab",
        "New Cascadia 2020-Sleeper"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 33;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

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
      return (IEnumerable<Qualifier>) new Qualifier[17]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS064_DPF_Regen_State"),
        new Qualifier((QualifierTypes) 1, "virtual", "NeutralSwitch"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS111_NOx_raw_concentration"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS114_NOx_out_concentration"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS036_SCR_Inlet_NOx_Sensor"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS035_SCR_Outlet_NOx_Sensor"),
        new Qualifier((QualifierTypes) 4, "ACM301T", "e2p_nox_out_dia_sens_runtime"),
        new Qualifier((QualifierTypes) 4, "ACM301T", "e2p_nox_raw_dia_sens_runtime"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS019_SCR_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS007_DOC_Inlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS008_DOC_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS009_DPF_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[5]
      {
        "MCM21T",
        "ACM301T",
        "CPC302T",
        "CPC501T",
        "CPC502T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[11]
      {
        "DT_AS111_NOx_raw_concentration",
        "DT_AS114_NOx_out_concentration",
        "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet",
        "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet",
        "DT_AS064_DPF_Regen_State",
        "RT_SCR_Dosing_Quantity_Check_Start_Status",
        "RT_RC0400_DPF_High_Idle_regeneration_Start",
        "RT_DPF_High_Idle_regeneration_Start",
        "RT_SCR_Dosing_Quantity_Check_Stop_status",
        "RT_RC0400_DPF_High_Idle_regeneration_Stop",
        "RT_DPF_High_Idle_regeneration_Stop"
      };
    }
  }
}
