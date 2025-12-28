// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 157;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Detroit Transmission Dyno Mode (EMG).panel";
    }
  }

  public virtual string Guid => "d818f22f-3601-45a5-b3f9-9eb2ee191095";

  public virtual string DisplayName => "Transmission Dyno Mode";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ECPC01T",
        "ETCM01T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "Transmission";

  public virtual FilterTypes Filters => (FilterTypes) 4;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

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
      return (IEnumerable<Qualifier>) new Qualifier[11]
      {
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS126_Actual_E_Motor_Speed_E_Motor_3_Actual_E_Motor_Speed_E_Motor_3"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS124_Actual_E_Motor_Speed_E_Motor_1_Actual_E_Motor_Speed_E_Motor_1"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS052_CalculatedGear_CalculatedGear"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Desired_Gear_current_value"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS063_IgnitionSwitchStatus_IgnitionSwitchStatus"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS134_Current_Torque_Axle_2_Current_Torque_Axle_2"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS133_Current_Torque_Axle_1_Current_Torque_Axle_1"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS004_Kickdown"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Transmission_Oil_Temperature_current_value")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "RT_OTF_DynoMode_Start",
        "RT_OTF_DynoMode_Stop"
      };
    }
  }
}
