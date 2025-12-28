// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__Euro5_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__Euro5_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 56;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Fuel System Integrity Check (Euro5).panel";
    }
  }

  public virtual string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

  public virtual string DisplayName => "Fuel System Integrity Check";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "DDEC16-DD16EURO5",
        "DDEC16-DD13EURO5"
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
      return (IEnumerable<Qualifier>) new Qualifier[8]
      {
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR003_PWM_Routine_by_Function_for_Production_Start_Control_Value"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR003_PWM_Routine_by_Function_for_Production_Stop_Function_Name"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR076_P_RAIL_DESIRED_Direct_Input_Start"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR076_P_RAIL_DESIRED_Direct_Input_Stop"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR02FA_set_TM_mode_Start"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR02FA_set_TM_mode_Stop")
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
      return (IEnumerable<Qualifier>) new Qualifier[24]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS043_Rail_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS098_desired_rail_pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS001_KW_NW_validity_signal"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS013_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS014_Fuel_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AAS_LPPO_Fuel_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AAS_LPPO_Fuel_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS043_Rail_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS098_desired_rail_pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS115_HP_Leak_Actual_Value"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Counter"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS001_KW_NW_validity_signal"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelSystemIntegrityCheck_Automatic_Euro5"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelSystemIntegrityCheck_LeakTest_Euro5"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelSystemIntegrityCheck_Manual_Euro5")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "DT_AS115_HP_Leak_Actual_Value",
        "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter",
        "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value"
      };
    }
  }
}
