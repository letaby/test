// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Automatic_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Automatic_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 115;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Tests\\Cylinder Cutout Test (Automatic).panel";
    }
  }

  public virtual string Guid => "a7fae696-7489-4f66-8e9e-2482602f4f97";

  public virtual string DisplayName => "Cylinder Cutout (Automatic)";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get => (IEnumerable<string>) new string[1]{ "S60" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 66;

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
      return (IEnumerable<Qualifier>) new Qualifier[5]
      {
        new Qualifier((QualifierTypes) 1, "MCM", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS013_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS003_Actual_Torque"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS003_Actual_Torque")
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
      return (IEnumerable<string>) new string[11]
      {
        "RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder",
        "DT_AS010_Engine_Speed",
        "DT_AS003_Actual_Torque",
        "RT_SR004_Engine_Cylinder_Cut_Off_Stop",
        "DT_AS072_DPF_Zone",
        "RT_SR015_Idle_Speed_Modification_Start",
        "RT_SR015_Idle_Speed_Modification_Stop",
        "DT_AS013_Coolant_Temperature",
        "DT_AS032_EGR_Actual_Valve_Position",
        "DT_AS034_Throttle_Valve_Actual_Position",
        "DT_AS117_Percentage_of_current_VGT_position"
      };
    }
  }
}
