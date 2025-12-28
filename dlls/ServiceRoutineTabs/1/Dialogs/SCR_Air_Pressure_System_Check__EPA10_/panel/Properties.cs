// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Air_Pressure_System_Check__EPA10_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Air_Pressure_System_Check__EPA10_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 32 /*0x20*/;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\SCR Air Pressure System Check (EPA10).panel";
    }
  }

  public virtual string Guid => "2e2eb1eb-6841-4978-a9de-128b97331afe";

  public virtual string DisplayName => "SCR Air Pressure System Check";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ACM02T" };
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

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

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
      return (IEnumerable<Qualifier>) new Qualifier[7]
      {
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS003_Enable_DEF_pump"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS016_DEF_Air_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS014_DEF_Pressure"),
        new Qualifier((QualifierTypes) 2, "ACM02T", "RT_SCR_Pressure_System_Check_Start"),
        new Qualifier((QualifierTypes) 2, "ACM02T", "RT_SCR_Pressure_System_Check_Stop"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[2]
      {
        new ServiceCall("ACM02T", "RT_SCR_Pressure_System_Check_Start", (IEnumerable<string>) new string[1]
        {
          "10"
        }),
        new ServiceCall("ACM02T", "RT_SCR_Pressure_System_Check_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ACM02T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "DT_AS001_Engine_Speed"
      };
    }
  }
}
