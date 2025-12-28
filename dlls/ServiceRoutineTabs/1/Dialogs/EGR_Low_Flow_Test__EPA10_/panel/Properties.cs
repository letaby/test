// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__EPA10_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__EPA10_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 195;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGR Low Flow Test (EPA10).panel";
    }
  }

  public virtual string Guid => "33d784e1-2169-485b-abf5-771d4bd56023";

  public virtual string DisplayName => "EGR Low Flow Test";

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
        "DDEC10-DD16",
        "DDEC10-DD15",
        "DDEC10-DD13"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "EGR";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR015_Idle_Speed_Modification_Start"),
        new Qualifier((QualifierTypes) 2, "MCM02T", "RT_SR015_Idle_Speed_Modification_Stop"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS060_Charge_Air_Cooler_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS013_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS031_EGR_Commanded_Governor_Value"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS025_EGR_Delta_Pressure")
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
      return (IEnumerable<Qualifier>) new Qualifier[7]
      {
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS013_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS025_EGR_Delta_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS031_EGR_Commanded_Governor_Value"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS060_Charge_Air_Cooler_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM02T", "630A12"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS010_Engine_Speed")
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
      return (IEnumerable<string>) new string[4]
      {
        "RT_SR015_Idle_Speed_Modification_Start",
        "RT_SR015_Idle_Speed_Modification_Stop",
        "DT_AS060_Charge_Air_Cooler_Outlet_Temperature",
        "DT_AS013_Coolant_Temperature"
      };
    }
  }
}
