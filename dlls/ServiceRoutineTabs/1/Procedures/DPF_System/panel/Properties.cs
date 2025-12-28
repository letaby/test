// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 68;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\DPF System.panel";
    }
  }

  public virtual string Guid => "07370e77-98a0-428c-9460-3994036f6d20";

  public virtual string DisplayName => "DPF System";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "CPC2",
        "MCM"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[6]
      {
        "DD13",
        "DD15",
        "DD16",
        "MBE4000",
        "MBE900",
        "S60"
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
      return (IEnumerable<Qualifier>) new Qualifier[46]
      {
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS040_DOC_Inlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS041_DOC_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS053_DPF_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "airInletTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS022_Active_Governor_Type"),
        new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS019_Barometric_Pressure"),
        new Qualifier((QualifierTypes) 1, "virtual", "airInletPressure"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS071_Smoke_Control_Status"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS033_Throttle_Valve_Commanded_Value"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS034_Throttle_Valve_Actual_Position"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS036_DPF_Inlet_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS037_DPF_Outlet_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS077_Fuel_Cut_Off_Valve"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS035_Fuel_Doser_Injection_Status"),
        new Qualifier((QualifierTypes) 1, "virtual", "fuelPressure"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS038_Doser_Fuel_Line_Pressure"),
        new Qualifier((QualifierTypes) 0, "CPC2", "DT_DS001_Clutch_Open"),
        new Qualifier((QualifierTypes) 0, "CPC2", "DT_DS001_Parking_Brake"),
        new Qualifier((QualifierTypes) 0, "CPC2", "DT_DS006_Neutral_Switch"),
        new Qualifier((QualifierTypes) 0, "CPC2", "DT_DS008_DPF_Regen_Switch_Status"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_AS072_DPF_Zone"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_DS014_DPF_Regen_Flag"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_DS014_DPF_CAN_manual_regen"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_DS014_DPF_CAN_high_idle_regen"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_AS073_Regeneration_Time"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_AS074_DPF_Target_Temperature"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_AS075_DOC_Out_Model_No_Delay"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_AS076_DOC_Out_Model_Delay"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_AS055_Temperature_Compressor_In"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_AS058_Temperature_Compressor_Out"),
        new Qualifier((QualifierTypes) 0, "MCM", "DT_AS056_Pressure_Compressor_Out"),
        new Qualifier((QualifierTypes) 0, "CPC2", "DT_DS003_Engine_Brake_Disable"),
        new Qualifier((QualifierTypes) 0, "CPC2", "DT_DS003_Engine_Brake_Low"),
        new Qualifier((QualifierTypes) 0, "CPC2", "DT_DS003_Engine_Brake_Medium"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineload"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeFuelCompensationGaugePressure"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeDoserFuelLineGaugePressure"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeBoostPressure"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_HCDoserPurge_EPA07"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_OverTheRoadRegen_EPA07"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_ParkedRegen_EPA07")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "DT_AS077_Fuel_Cut_Off_Valve"
      };
    }
  }
}
