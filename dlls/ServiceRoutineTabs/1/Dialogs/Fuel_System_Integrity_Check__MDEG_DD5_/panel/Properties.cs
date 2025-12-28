// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__MDEG_DD5_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__MDEG_DD5_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 155;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Fuel System Integrity Check (MDEG-DD5).panel";
    }
  }

  public virtual string Guid => "f9d2af05-cbf0-4612-9755-105786bd1b3c";

  public virtual string DisplayName => "Fuel System Integrity Check";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "DDEC16-DD5"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Fuel System";

  public virtual FilterTypes Filters => (FilterTypes) 66;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

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
      return (IEnumerable<Qualifier>) new Qualifier[26]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS001_KW_NW_validity_signal"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS043_Rail_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS098_desired_rail_pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS001_KW_NW_validity_signal"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS013_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS014_Fuel_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS043_Rail_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS098_desired_rail_pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS115_HP_Leak_Actual_Value"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Counter"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelSystemIntegrityCheck_Automatic_MDEG_DD5"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelSystemIntegrityCheck_High_Pressure_Test_MDEG"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelSystemIntegrityCheck_Fuel_Filter_Pressure_Check_MDEG"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake")
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
