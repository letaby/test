// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Accumulated_Fuel_Mass_Sync__MY17_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Accumulated_Fuel_Mass_Sync__MY17_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 143;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Accumulated Fuel Mass Sync (MY17).panel";
    }
  }

  public virtual string Guid => "2e0407ca-6cbf-4210-9950-be1966e8fb27";

  public virtual string DisplayName => "Accumulated Fuel Mass Sync";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ACM21T",
        "MCM21T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR02EB_ATS_lifetime_ageing_strategy_Start"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR0EB_ATS_lifetime_ageing_strategy_Start"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR012_Save_EOL_Data_Request_Start"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_Save_EOL_Data_Start")
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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 64 /*0x40*/, "MCM21T", "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM21T", "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "MCM21T",
        "ACM21T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[6]
      {
        "RT_SR0EB_ATS_lifetime_ageing_strategy_Start",
        "RT_SR012_Save_EOL_Data_Request_Start",
        "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven",
        "RT_SR02EB_ATS_lifetime_ageing_strategy_Start",
        "RT_Save_EOL_Data_Start",
        "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"
      };
    }
  }
}
