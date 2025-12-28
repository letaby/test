// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Oil_Pump_Controls__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Oil_Pump_Controls__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 22;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Transmission Oil Pump Controls (EMG).panel";
    }
  }

  public virtual string Guid => "fc0285bc-3d5f-4a5f-909d-91e48558cb03";

  public virtual string DisplayName => "eTransmission Oil Pump Controls";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "ePowertrain";

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
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump1_Control_results_resp"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump1_Control_resp"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump1_Control_stop_resp"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump2_Control_results_resp"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump2_Control_resp"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump2_Control_stop_resp")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[6]
      {
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump1_Control_results_resp", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump1_Control_resp", (IEnumerable<string>) new string[2]
        {
          "ETHM_Oil_Pump1_Control=100",
          "ETHM_Oil_Pump2_Control=0"
        }),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump1_Control_stop_resp", (IEnumerable<string>) new string[2]
        {
          "ETHM_Oil_Pump1_Control_stop=0",
          "ETHM_Oil_Pump2_Control_stop=0"
        }),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump2_Control_results_resp", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump2_Control_resp", (IEnumerable<string>) new string[2]
        {
          "ETHM_Oil_Pump1_Control=0",
          "ETHM_Oil_Pump2_Control=100"
        }),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump2_Control_stop_resp", (IEnumerable<string>) new string[2]
        {
          "ETHM_Oil_Pump1_Control_stop=0",
          "ETHM_Oil_Pump2_Control_stop=0"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }
}
