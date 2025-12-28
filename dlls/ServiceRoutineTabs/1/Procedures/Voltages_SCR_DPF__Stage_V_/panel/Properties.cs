// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages_SCR_DPF__Stage_V_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages_SCR_DPF__Stage_V_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 60;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Voltages SCR DPF (Stage V).panel";
    }
  }

  public virtual string Guid => "66ebe157-c680-431e-b3a8-5a4a06f37bdd";

  public virtual string DisplayName => "SCR and DPF Voltages";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ACM301T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[15]
      {
        "DDEC13-DD13",
        "DDEC13-DD15",
        "DDEC13-DD16",
        "DDEC13-DD11 Tier4",
        "DDEC13-DD13 Tier4",
        "DDEC13-DD16 Tier4",
        "DDEC16-DD13",
        "DDEC16-DD15",
        "DDEC16-DD16",
        "DDEC20-DD16 StageV",
        "DDEC20-DD13 StageV",
        "DDEC20-DD11 StageV",
        "DDEC20-DD13",
        "DDEC20-DD15",
        "DDEC20-DD16"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 34;

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
      return (IEnumerable<Qualifier>) new Qualifier[8]
      {
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS093_DEF_Tank_Temperature_Sensor_Voltage"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS094_DEF_Tank_Level_Sensor_Voltage"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS148_DPF_Outlet_Temperature_Sensor_Voltage"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS147_T_DOC_Outlet_Temperature_Sensor_Voltage"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS146_T_DOC_Inlet_Temperature_Sensor_Voltage"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS098_DEF_Pressure_Sensor_Voltage"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS192_PM_sensor_supply_voltage"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS096_SCR_Oulet_Temperature_Sensor_Voltage")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ACM301T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "DT_AS146_T_DOC_Inlet_Temperature_Sensor_Voltage",
        "DT_AS147_T_DOC_Outlet_Temperature_Sensor_Voltage",
        "DT_AS148_DPF_Outlet_Temperature_Sensor_Voltage"
      };
    }
  }
}
