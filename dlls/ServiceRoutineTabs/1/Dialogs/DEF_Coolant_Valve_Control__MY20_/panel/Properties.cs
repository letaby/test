// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Coolant_Valve_Control__MY20_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Coolant_Valve_Control__MY20_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 91;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DEF Coolant Valve Control (MY20).panel";
    }
  }

  public virtual string Guid => "020f8264-dbc8-4073-9385-94b286bdc0a5";

  public virtual string DisplayName => "DEF Coolant Valve Control";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ACM301T" };
  }

  public virtual bool AllDevicesRequired => false;

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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_Coolant_Valve_Open_Start_Coolant_Valve_Open_Status"),
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_Coolant_Valve_Open_Stop")
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
      return (IEnumerable<Qualifier>) new Qualifier[5]
      {
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS002_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS022_DEF_tank_Temperature"),
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_Coolant_Valve_Open_Start_Coolant_Valve_Open_Status"),
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_Coolant_Valve_Open_Stop"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_DS005_Coolant_Valve")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[2]
      {
        new ServiceCall("ACM301T", "RT_Coolant_Valve_Open_Start_Coolant_Valve_Open_Status", (IEnumerable<string>) new string[0]),
        new ServiceCall("ACM301T", "RT_Coolant_Valve_Open_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ACM301T" };
  }
}
