// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 30;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Tests\\Cylinder Cutout Test (Manual).panel";
    }
  }

  public virtual string Guid => "7e051f68-2dc7-4e63-a96e-89045749e827";

  public virtual string DisplayName => "Cylinder Cutout (Manual)";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

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
      return (IEnumerable<string>) new string[4]
      {
        "RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder",
        "RT_SR004_Engine_Cylinder_Cut_Off_Stop",
        "RT_SR015_Idle_Speed_Modification_Start",
        "RT_SR015_Idle_Speed_Modification_Stop"
      };
    }
  }
}
