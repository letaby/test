// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Quantity_Control_Valve__Tier4_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Quantity_Control_Valve__Tier4_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 294;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\FIS Fuel Quantity Control Valve (Tier4).panel";
    }
  }

  public virtual string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

  public virtual string DisplayName => "FIS Fuel Quantity Control Valve";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[12]
      {
        "DDEC13-MDEG 4-Cylinder Tier4",
        "DDEC13-MDEG 6-Cylinder Tier4",
        "DDEC13-DD11 Tier4",
        "DDEC13-DD13 Tier4",
        "DDEC13-DD16 Tier4",
        "DDEC20-MDEG 4-Cylinder StageV",
        "DDEC20-MDEG 6-Cylinder StageV",
        "DDEC20-DD11 StageV",
        "DDEC20-DD13 StageV",
        "DDEC20-DD16 StageV",
        "DDEC16-MDEG 4-Cylinder StageV",
        "DDEC16-MDEG 6-Cylinder StageV"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "Fuel System";

  public virtual FilterTypes Filters => (FilterTypes) 66;

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
      return (IEnumerable<Qualifier>) new Qualifier[10]
      {
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Positive"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Negative"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS043_Rail_Pressure"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineTorque"),
        new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS121_EU_Fuel_Metering_Unit_FMU_desired_current"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS100_Quantity_Control_Valve_Current")
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
      return (IEnumerable<string>) new string[21]
      {
        "DT_AS121_EU_Fuel_Metering_Unit_FMU_desired_current",
        "DT_AS100_Quantity_Control_Valve_Current",
        "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Positive",
        "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Negative",
        "DT_AS013_Coolant_Temperature",
        "DT_AS043_Rail_Pressure",
        "DT_AS122_Fuel_Metering_Unit_Stick_Diagnosis_State",
        "DT_AS123_Fuel_Metering_Unit_Diagnosis_Error_State",
        "DT_AS087_Actual_Fuel_Mass",
        "RT_SR071_FMU_Stick_Diagnosis_Function_Start_active_status",
        "RT_SR071_FMU_Stick_Diagnosis_Function_Request_Results_result_status",
        "RT_SR071_FMU_Stick_Diagnosis_Function_Request_Results_result_error_bit",
        "RT_SR071_FMU_Stick_Diagnosis_Function_Request_Results_result_fmu_value",
        "RT_SR071_FMU_Stick_Diagnosis_Function_Stop",
        "RT_SR014_SET_EOL_Default_Values_Start",
        "DT_DS003_CPC2_CAN_Ignition_Status",
        "DT_DS003_MCM_wired_Starter_Signal_Status",
        "DT_AS023_Engine_State",
        "DT_AS010_Engine_Speed",
        "DT_DS019_Vehicle_Check_Status",
        "DT_AS014_Fuel_Temperature"
      };
    }
  }
}
