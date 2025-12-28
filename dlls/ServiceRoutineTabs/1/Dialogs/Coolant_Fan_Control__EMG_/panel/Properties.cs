// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Coolant_Fan_Control__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Coolant_Fan_Control__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 8;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Coolant Fan Control (EMG).panel";
    }
  }

  public virtual string Guid => "4b7fdfab-7f2e-4349-ac37-9ea54dea6d06";

  public virtual string DisplayName => "Coolant Fan Control";

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
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS252_DrvCircInTemp_u16_DrvCircInTemp_u16"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS202_Batt_Circ_Temp_Batt_Circ_Temp"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_CoolantFanControl"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_CoolantFanControl"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Start"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Stop")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Start", (IEnumerable<string>) new string[3]
        {
          "Requested_duty_cycle_for_Brake_Resistor_1=0",
          "Requested_duty_cycle_for_Brake_Resistor_2=0",
          "Requested_duty_cycle_for_Edrive_Fan=50"
        }),
        new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Stop", (IEnumerable<string>) new string[3]
        {
          "OTF_ETHM_FanCtrl_FanBrakeResistor1=0",
          "OTF_ETHM_FanCtrl_FanBrakeResistor2=0",
          "OTF_ETHM_FanCtrl_eDriveFan=0"
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
        "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan"
      };
    }
  }
}
