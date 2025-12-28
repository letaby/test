// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 54;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Air Mass Adaptation.panel";
    }
  }

  public virtual string Guid => "c32d3dc4-29cb-4d5f-8895-877e686cfcce";

  public virtual string DisplayName => "Air Mass Adaptation";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "MCM",
        "CPC2"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "MBE4000",
        "MBE900"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "Engine Configuration";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

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
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS031_EGR_Commanded_Governor_Value"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS068_Fan_PWM06"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS032_EGR_Actual_Valve_Position"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_DS014_DPF_Regen_Flag"),
        new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "oilTemp"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS054_Differential_Pressure_Compressor_In")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "MCM",
        "CPC2"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[16 /*0x10*/]
      {
        "DT_DS014_DPF_Regen_Flag",
        "DT_AS010_Engine_Speed",
        "DT_AS013_Coolant_Temperature",
        "DT_AS016_Oil_Temperature",
        "RT_SR014_SET_EOL_Default_Values_Start",
        "RT_SR015_Idle_Speed_Modification_Start",
        "RT_SR015_Idle_Speed_Modification_Stop",
        "RT_SR002_Engine_Torque_Demand_Substitution_Start_CAN_Torque_Demand",
        "RT_SR002_Engine_Torque_Demand_Substitution_Stop",
        "RT_SR005_SW_Routine_Start_SW_Operation",
        "RT_SR005_SW_Routine_Stop",
        "RT_SR003_PWM_Routine_Start_PWM_Value",
        "RT_SR003_PWM_Routine_Stop",
        "RT_SR07E_Automated_Air_Mass_Adaption_Start_status",
        "RT_SR07E_Automated_Air_Mass_Adaption_Stop",
        "RT_SR07E_Automated_Air_Mass_Adaption_Request_Results_Results_Status"
      };
    }
  }
}
