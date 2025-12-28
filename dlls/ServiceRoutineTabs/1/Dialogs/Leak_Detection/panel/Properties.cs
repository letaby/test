// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Leak_Detection.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Leak_Detection.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 59;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Leak Detection.panel";
    }
  }

  public virtual string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

  public virtual string DisplayName => "Leak Detection";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "DD13",
        "DD15"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Fuel System";

  public virtual FilterTypes Filters => (FilterTypes) 66;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

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
      return (IEnumerable<Qualifier>) new Qualifier[9]
      {
        new Qualifier((QualifierTypes) 4, "MCM", "HP_Leak_Counter"),
        new Qualifier((QualifierTypes) 4, "MCM", "HP_Leak_Learned_Value"),
        new Qualifier((QualifierTypes) 4, "MCM", "HP_Leak_Learned_Counter"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS114_RPG_COMPENSATION"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS115_HP_Leak_Actual_Value"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS067_Coolant_Temperatures_2"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS014_Fuel_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS023_Engine_State")
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
      return (IEnumerable<string>) new string[5]
      {
        "RT_SR076_Desired_Rail_Pressure_Start_Status",
        "RT_SR07B_Enable_Calibration_Overide_for_Leak_Detection_Test_Start",
        "DT_AS067_Coolant_Temperatures_2",
        "DT_AS014_Fuel_Temperature",
        "RT_SR014_SET_EOL_Default_Values_Start"
      };
    }
  }
}
