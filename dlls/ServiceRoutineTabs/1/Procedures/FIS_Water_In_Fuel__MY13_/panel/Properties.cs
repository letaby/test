// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_In_Fuel__MY13_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_In_Fuel__MY13_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 225;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\FIS Water In Fuel (MY13).panel";
    }
  }

  public virtual string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

  public virtual string DisplayName => "FIS Water in Fuel";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[21]
      {
        "DDEC13-DD13",
        "DDEC13-DD15",
        "DDEC13-DD16",
        "DDEC13-MDEG 4-Cylinder Tier4",
        "DDEC13-MDEG 6-Cylinder Tier4",
        "DDEC13-DD11 Tier4",
        "DDEC13-DD13 Tier4",
        "DDEC13-DD16 Tier4",
        "DDEC16-DD13",
        "DDEC16-DD15",
        "DDEC16-DD16",
        "DDEC20-DD13",
        "DDEC20-DD15",
        "DDEC20-DD16",
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

  public virtual int MinProductAccessLevel => 3;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[1]
      {
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR0AB_Reset_Water_in_Fuel_Values_Start")
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
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS047_Ignition_Cycle_Counter"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS045_Engine_Operating_Hours"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_hours"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC065_OP_Data_Oil_e2p_i_water_raised_eng_starts"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM21T", "61000F"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM21T", "610010")
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
      return (IEnumerable<string>) new string[5]
      {
        "DT_AS045_Engine_Operating_Hours",
        "DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_hours",
        "DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_starts",
        "DT_AS087_Actual_Fuel_Mass",
        "RT_SR0AB_Reset_Water_in_Fuel_Values_Start"
      };
    }
  }
}
