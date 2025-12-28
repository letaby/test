// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Deaeration_Battery__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Deaeration_Battery__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 100;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Deaeration Battery (EMG).panel";
    }
  }

  public virtual string Guid => "2d7a2a0d-59da-498e-bfc1-bc0f2fdf0e18";

  public virtual string DisplayName => "De-Aeration Battery";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "ePowertrain";

  public virtual FilterTypes Filters => (FilterTypes) 2;

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
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery1_Circuit_Res"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery2_Circuit_Res"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "IOC_IOC_ETHM_Shutoff_ValveControl_Return_Control"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br1"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br2"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS236_BattCircValveActPos_BattCircValveActPos"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Request_Results_ETHM_Battery_Circuit_Deaeration"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Start_ETHM_Battery_Circuit_Deaeration"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Stop_ETHM_Battery_Circuit_Deaeration")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[6]
      {
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery1_Circuit_Res", (IEnumerable<string>) new string[2]
        {
          "WaterPump_Speed_Battery1_Circuit_Req=0",
          "WaterPump_Speed_Battery2_Circuit_Req=0"
        }),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery2_Circuit_Res", (IEnumerable<string>) new string[2]
        {
          "WaterPump_Speed_Battery1_Circuit_Req=0",
          "WaterPump_Speed_Battery2_Circuit_Req=0"
        }),
        new ServiceCall("ECPC01T", "IOC_IOC_ETHM_Shutoff_ValveControl_Return_Control", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Request_Results_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Start_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>) new string[1]
        {
          "Battery_Circuit_Deaeration_contro_start=1"
        }),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Stop_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>) new string[1]
        {
          "Battery_Circuit_Deaeration_control_start=0"
        })
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
      return (IEnumerable<string>) new string[1]
      {
        "DT_Current_value_percentage_water_pump_Br2"
      };
    }
  }
}
