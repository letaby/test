// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 89;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\APS Calibration (EMG).panel";
    }
  }

  public virtual string Guid => "976d3e5e-6714-4734-a5ca-e17ce5c73d53";

  public virtual string DisplayName => "Active Powersteering Alignment";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "APS301T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "eCascadia-Daycab",
        "eCascadia-Sleeper"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 9;

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
      return (IEnumerable<Qualifier>) new Qualifier[9]
      {
        new Qualifier((QualifierTypes) 1, "APS301T", "DT_Endstop_Right_Endstop"),
        new Qualifier((QualifierTypes) 1, "APS301T", "DT_Endstop_Left_Endstop"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Steering_wheel_angle_sensor_Read_Steering_wheel_angle"),
        new Qualifier((QualifierTypes) 1, "APS301T", "DT_Endstop_Calibration_Status_Left_Calibration_State"),
        new Qualifier((QualifierTypes) 1, "APS301T", "DT_Steering_Angle_Calibration_Status_Calibration_Status"),
        new Qualifier((QualifierTypes) 1, "APS301T", "DT_Endstop_Calibration_Status_Right_Calibration_State"),
        new Qualifier((QualifierTypes) 4, "APS301T", "TorsionBarTorqueOffset"),
        new Qualifier((QualifierTypes) 1, "APS301T", "DT_Steering_Angle_Steering_Angle"),
        new Qualifier((QualifierTypes) 1, "J1939-0", "DT_7315")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[5]
      {
        "APS301T",
        "ABS02T",
        "ECPC01T",
        "VRDU02T",
        "J1939-0"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[8]
      {
        "RT_Calibrate_extern_and_intern_steering_angle_Start_Calibration_state",
        "RT_Calibrate_Left_Endstop_Start_Endstop_Learnconditions_Endstop_state",
        "RT_Calibrate_Right_Endstop_Start_Endstop_Learnconditions_Endstop_state",
        "RT_Calibrate_TorsionBarTorqueOffset_Start",
        "RT_Calibrate_TorsionBarTorqueOffset_Request_Results_Tbtoffset_calibration_request_status",
        "FN_HardReset",
        "DT_Steering_Angle_Steering_Angle",
        "RT_Discard_calibration_data_Start"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceSharedProcedureQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "SP_TorsionBarTorqueCalibration"
      };
    }
  }
}
