// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_ADS_Self_Check__MY20_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_ADS_Self_Check__MY20_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 60;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\SCR ADS Self Check (MY20).panel";
    }
  }

  public virtual string Guid => "8bbc8347-b40d-456d-abcc-328b5ff67d53";

  public virtual string DisplayName => "SCR ADS Self-check";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ACM301T" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[3]
      {
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_SCR_ADS_SelfCheck_Routine_Request_Results_status_of_service_function"),
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_SCR_ADS_SelfCheck_Routine_Start"),
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_SCR_ADS_SelfCheck_Routine_Stop")
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
      return (IEnumerable<Qualifier>) new Qualifier[8]
      {
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_SCR_ADS_Self_Check_MY20"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS079_ADS_priming_request"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_DS011_ADS_dosing_valve_state"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS143_ADS_Pump_Speed"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS001_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS077_Vehicle_speed_from_ISP100ms"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM301T", "RT_SCR_ADS_SelfCheck_Routine_Request_Results_status_of_service_function"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS014_DEF_Pressure")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ACM301T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "DT_AS014_DEF_Pressure"
      };
    }
  }
}
