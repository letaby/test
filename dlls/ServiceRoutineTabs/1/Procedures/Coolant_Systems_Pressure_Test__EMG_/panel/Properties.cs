// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Coolant_Systems_Pressure_Test__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Coolant_Systems_Pressure_Test__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 27;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Coolant Systems Pressure Test (EMG).panel";
    }
  }

  public virtual string Guid => "9f9f7a0c-8377-4b46-aebc-ed3012418c9d";

  public virtual string DisplayName => "Coolant Systems Pressure Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 1;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

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
      return (IEnumerable<Qualifier>) new Qualifier[12]
      {
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS243_PwmOutput05ReqDutyCycle_PwmOutput05ReqDutyCycle"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_ETHM_PtcCtrl_Request_Results_OTF_ETHM_Cabin_PTC2_High_Voltage"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS253_EDrvCircOutTemp_EDrvCircOutTemp"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS202_Batt_Circ_Temp_Batt_Circ_Temp"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS082_LIN2_PTC_Cab1_DutyCycle_LIN2_PTC_Cab1_DutyCycle"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS081_LIN1_PTC_Batt2_DutyCycle_LIN1_PTC_Batt2_DutyCycle"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS008_HV_Ready"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS007_AmbientAirTemperature_AmbientAirTemperature"),
        new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Blower_Speed_feedback_from_blower_Blower_Speed_feedback_from_blower"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS080_LIN1_PTC_Batt1_DutyCycle_LIN1_PTC_Batt1_DutyCycle")
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
      return (IEnumerable<string>) new string[6]
      {
        "RT_OTF_3by2_wayValvePositionControl_Start_3by2_Valve_ExtensionCkt_Req_1",
        "RT_OTF_ETHM_BCWaterPumpCtrl_Start",
        "RT_OTF_ETHM_PtcCtrl_Start",
        "RT_OTF_ETHM_PtcCtrl_Stop",
        "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Start_e_drive_circuit_deaeration_start_resp",
        "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Stop_e_drive_circuit_deaeration_stop_resp"
      };
    }
  }
}
