// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__EPA10_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__EPA10_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 34;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Fuel System Integrity Check (EPA10).panel";
    }
  }

  public virtual string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

  public virtual string DisplayName => "Fuel System Integrity Check";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM02T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "DD13",
        "DD15",
        "DD16"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Fuel System";

  public virtual FilterTypes Filters => (FilterTypes) 66;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[22]
      {
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR015_Idle_Speed_Modification_Start"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR015_Idle_Speed_Modification_Stop"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR003_PWM_Routine_by_Function_for_Production_Start_Control_Value"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR003_PWM_Routine_by_Function_for_Production_Stop_Function_Name"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR076_P_RAIL_DESIRED_Direct_Input_Start"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR076_P_RAIL_DESIRED_Direct_Input_Stop"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR018_Disable_HC_Doser_Start"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR018_Disable_HC_Doser_Stop"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS098_desired_rail_pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS043_Rail_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS100_Quantity_Control_Valve_Current"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS024_Fuel_Compensation_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_ASL002_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS023_Engine_State"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_ASL005_Fuel_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS124_LPPO_Fuel_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_ASL005_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS121_Quantity_Control_Valve_Desired_Current")
      };
    }
  }

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[26]
      {
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS024_Fuel_Compensation_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS124_LPPO_Fuel_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS043_Rail_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS098_desired_rail_pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS001_KW_NW_validity_signal"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS013_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS014_Fuel_Temperature"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelSystemIntegrityCheck_Automatic_EPA10"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelSystemIntegrityCheck_LeakTest_EPA10"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelSystemIntegrityCheck_Manual_EPA10"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS024_Fuel_Compensation_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS124_LPPO_Fuel_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS043_Rail_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS098_desired_rail_pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS115_HP_Leak_Actual_Value"),
        new Qualifier((QualifierTypes) 4, "MCM02T", "HP_Leak_Counter"),
        new Qualifier((QualifierTypes) 4, "MCM02T", "HP_Leak_Learned_Counter"),
        new Qualifier((QualifierTypes) 4, "MCM02T", "HP_Leak_Learned_Value"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS001_KW_NW_validity_signal"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
        new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MCM02T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "DT_AS115_HP_Leak_Actual_Value"
      };
    }
  }
}
