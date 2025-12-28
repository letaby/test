// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Shift_Abort_Counter.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Shift_Abort_Counter.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 45;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\TCM Shift Abort Counter.panel";
    }
  }

  public virtual string Guid => "2d9349d3-82e9-4643-883b-979fe429a54f";

  public virtual string DisplayName => "Transmission Shift Abort Counter";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "TCM01T",
        "TCM05T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 6;

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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 2, "TCM01T", "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start"),
        new Qualifier((QualifierTypes) 2, "TCM01T", "RT_0440_Schaltabbruchzaehler_sichern_Start")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[2]
      {
        new ServiceCall("TCM01T", "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("TCM01T", "RT_0440_Schaltabbruchzaehler_sichern_Start", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "TCM01T",
        "TCM05T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[4]
      {
        "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start",
        "RT_0440_Schaltabbruchzaehler_sichern_Start",
        "RT_0441_Reset_shift_abort_counters_Start",
        "RT_0440_Save_copy_of_shift_abort_counters_Start"
      };
    }
  }
}
