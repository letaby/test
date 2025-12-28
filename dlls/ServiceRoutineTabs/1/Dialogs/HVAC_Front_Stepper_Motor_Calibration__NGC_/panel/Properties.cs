// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.HVAC_Front_Stepper_Motor_Calibration__NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HVAC_Front_Stepper_Motor_Calibration__NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 15;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\HVAC Front Stepper Motor Calibration (NGC).panel";
    }
  }

  public virtual string Guid => "7880ccf0-611e-4640-83f4-8b4ba3e800be";

  public virtual string DisplayName => "HVAC Front Stepper Motor Calibration";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "HVAC_F01T"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 137;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[1]
      {
        new Qualifier((QualifierTypes) 2, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Start")
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
      return (IEnumerable<Qualifier>) new Qualifier[15]
      {
        new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Panel_Outlet_Air_Distribution_Motor"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Floor_Outlet_Air_Distribution_Motor"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Defrost_Outlet_Air_Distribution_Motor"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Air_Inlet_Recirculation_Motor"),
        new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Mode_Door_Actuator_Position_feedback_Defrost"),
        new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Recirc_Actuator_Door_Position_feedback_Recirc_Actuator_Door_Position_feedback"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_HVACFStepperCal"),
        new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Blend_Door_Actuator_Position_Mix_Door_Actuator_Position_feedback"),
        new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Mode_Door_Actuator_Position_feedback_Panel"),
        new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Mode_Door_Actuator_Position_feedback_Floor"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor"),
        new Qualifier((QualifierTypes) 2, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Air_Inlet_Recirculation_Motor"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_HVACFStepperCal"),
        new Qualifier((QualifierTypes) 2, "HVAC_F01T", "RT_Stepper_Motor_Calibration_Start"),
        new Qualifier((QualifierTypes) 2, "HVAC_F01T", "SES_Extended_P2s_CAN_ECU_max_physical")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("HVAC_F01T", "RT_Stepper_Motor_Calibration_Request_Results_Air_Inlet_Recirculation_Motor", (IEnumerable<string>) new string[0]),
        new ServiceCall("HVAC_F01T", "RT_Stepper_Motor_Calibration_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("HVAC_F01T", "SES_Extended_P2s_CAN_ECU_max_physical", (IEnumerable<string>) new string[0])
      };
    }
  }
}
