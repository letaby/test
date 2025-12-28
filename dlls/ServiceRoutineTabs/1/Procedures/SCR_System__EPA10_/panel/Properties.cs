// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__EPA10_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__EPA10_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 110;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\SCR System (EPA10).panel";
    }
  }

  public virtual string Guid => "07370e77-98a0-428c-9460-3994036f6d20";

  public virtual string DisplayName => "SCR System";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "ACM02T",
        "CPC02T",
        "MCM02T"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "DDEC10-DD13",
        "DDEC10-DD15",
        "DDEC10-DD16"
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
      return (IEnumerable<Qualifier>) new Qualifier[41]
      {
        new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS001_Clutch_Open"),
        new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS001_Parking_Brake"),
        new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DS006_Neutral_Switch"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS004_Line_Heater_1"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS004_Line_Heater_2"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS004_Line_Heater_3"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS004_Line_Heater_4"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS005_Coolant_Valve"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS008_Diffuser_Heater"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS117_SCR_Out_Model_Delay"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS118_SCR_Heat_Generation"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS024_DEF_Tank_Level"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS065_Actual_DPF_zone"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS014_DEF_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS016_DEF_Air_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS009_DPF_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS018_SCR_Inlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS019_SCR_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS021_DEF_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS022_DEF_tank_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position"),
        new Qualifier((QualifierTypes) 1, "virtual", "airInletPressure"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineload"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS086_Requested_DEF_Dosing_Quantity"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS085_Actual_DEF_Dosing_Quantity"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS036_SCR_Inlet_NOx_Sensor"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_ChassisDynoBasicScrConversionCheck_EPA10"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_OutputComponentTest_EPA10"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_ParkedScrEfficiencyTest_EPA10"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS035_SCR_Outlet_NOx_Sensor"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS101_Nox_conversion_efficiency"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS053_Ambient_Air_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS019_Barometric_Pressure")
      };
    }
  }
}
