// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_in_Fuel.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_in_Fuel.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 216;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\FIS Water in Fuel.panel";
    }
  }

  public virtual string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

  public virtual string DisplayName => "FIS Water in Fuel";

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

  public virtual bool IsDialog => false;

  public virtual string Category => "Fuel System";

  public virtual FilterTypes Filters => (FilterTypes) 66;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 3;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[1]
      {
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR0AB_Reset_Water_in_Fuel_Values_Start")
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
      return (IEnumerable<Qualifier>) new Qualifier[11]
      {
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineTorque"),
        new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS047_Ignition_Cycle_Counter"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS045_Engine_Operating_Hours"),
        new Qualifier((QualifierTypes) 4, "MCM02T", "e2p_l_water_raised_eng_hours"),
        new Qualifier((QualifierTypes) 4, "MCM02T", "e2p_l_water_raised_eng_starts"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM02T", "61000F"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM02T", "610010")
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
      return (IEnumerable<string>) new string[3]
      {
        "DT_ASL007_Engine_Operating_Hours",
        "DT_AS087_Actual_Fuel_Mass",
        "RT_SR0AB_Reset_Water_in_Fuel_Values_Start"
      };
    }
  }
}
