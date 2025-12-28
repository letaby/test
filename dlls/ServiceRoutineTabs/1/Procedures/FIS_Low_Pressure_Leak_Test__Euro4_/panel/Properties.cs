// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__Euro4_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__Euro4_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 98;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\FIS Low Pressure Leak Test (Euro4).panel";
    }
  }

  public virtual string Guid => "a0414769-2da4-4114-beb1-bbe336a960e1";

  public virtual string DisplayName => "FIS Low Pressure Leak Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "DD15EURO4"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 66;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure")
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
      return (IEnumerable<string>) new string[2]
      {
        "DT_AS010_Engine_Speed",
        "DT_AS124_Low_Pressure_Pump_Outlet_Pressure"
      };
    }
  }
}
