// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor_and_Unlock__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor_and_Unlock__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 52;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Tilt Sensor and Unlock (EMG).panel";
    }
  }

  public virtual string Guid => "53dd9aa7-2829-4b73-b91f-539495c446d9";

  public virtual string DisplayName => "Tilt Sensor & Unlock";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ETCM01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => false;

  public virtual string Category => "Transmission";

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
      return (IEnumerable<Qualifier>) new Qualifier[10]
      {
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition"),
        new Qualifier((QualifierTypes) 2, "ETCM01T", "DJ_Release_transport_security_for_eTCM"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Transmission_Teach_in_State_current_state"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Inclination_Sensor_Signal_Value_current_value"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "ETCM01T", "14F1EE"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "ETCM01T", "14F1ED"),
        new Qualifier((QualifierTypes) 2, "ETCM01T", "RT_Inclination_Sensor_Teach_in_Procedure_Start"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[2]
      {
        new ServiceCall("ETCM01T", "DJ_Release_transport_security_for_eTCM", (IEnumerable<string>) new string[0]),
        new ServiceCall("ETCM01T", "RT_Inclination_Sensor_Teach_in_Procedure_Start", (IEnumerable<string>) new string[0])
      };
    }
  }
}
