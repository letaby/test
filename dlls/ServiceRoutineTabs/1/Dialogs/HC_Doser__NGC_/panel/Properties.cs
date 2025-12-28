// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.HC_Doser__NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HC_Doser__NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 74;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\HC Doser (NGC).panel";
    }
  }

  public virtual string Guid => "07370e77-98a0-428c-9460-3994036f6d20";

  public virtual string DisplayName => "HC Doser";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[6]
      {
        "ACM21T",
        "ACM301T",
        "CPC302T",
        "CPC501T",
        "CPC502T",
        "MCM21T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

  public virtual PanelTargets TargetHosts => (PanelTargets) 2;

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
      return (IEnumerable<Qualifier>) new Qualifier[55]
      {
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS007_DOC_Inlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS008_DOC_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS009_DPF_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS013_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS017_Inlet_Manifold_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS012_Vehicle_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS022_Active_Governor_Type"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS023_Engine_State"),
        new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS019_Barometric_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS018_Inlet_Manifold_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS071_Smoke_Control_Status"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS033_Throttle_Valve_Commanded_Value"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS005_DOC_Inlet_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS006_DPF_Outlet_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS077_Fuel_Cut_Off_Valve"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS035_Fuel_Doser_Injection_Status"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS038_Doser_Fuel_Line_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "CPC302T", "DT_DS255_Blocktransfer_ClutchStatus"),
        new Qualifier((QualifierTypes) 1, "CPC501T", "DT_DS255_Blocktransfer_ClutchStatus"),
        new Qualifier((QualifierTypes) 1, "CPC502T", "DT_DS255_Blocktransfer_ClutchStatus"),
        new Qualifier((QualifierTypes) 1, "CPC302T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "CPC501T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "CPC502T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "CPC302T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
        new Qualifier((QualifierTypes) 1, "CPC501T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
        new Qualifier((QualifierTypes) 1, "CPC502T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_MSC_GetSwState_033"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_MSC_Diagnostic_Displayables_DDMSC_AutoManSw_Rq_SAM"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS065_Actual_DPF_zone"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS064_DPF_Regen_State"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS119_Regeneration_Time"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS120_DPF_Target_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS122_DOC_Out_Model_Delay"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS065_Actual_DPF_zone"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS064_DPF_Regen_State"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS119_Regeneration_Time"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS120_DPF_Target_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS122_DOC_Out_Model_Delay"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS055_Temperature_Compressor_In"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS058_Temperature_Compressor_Out"),
        new Qualifier((QualifierTypes) 1, "CPC302T", "DT_AS255_Blocktransfer_EnduranceBrakeLeverPosition"),
        new Qualifier((QualifierTypes) 1, "CPC501T", "DT_AS255_Blocktransfer_EnduranceBrakeLeverPosition"),
        new Qualifier((QualifierTypes) 1, "CPC502T", "DT_AS255_Blocktransfer_EnduranceBrakeLeverPosition"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeBoostPressureMY13"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineload"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeFuelCompensationGaugePressureMY13"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeDoserFuelLineGaugePressureMY13"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_HCDoserPurge_MY13")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ACM21T",
        "MCM21T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[6]
      {
        "DT_AS006_DPF_Outlet_Pressure",
        "DT_AS005_DOC_Inlet_Pressure",
        "DT_AS007_DOC_Inlet_Temperature",
        "DT_AS008_DOC_Outlet_Temperature",
        "DT_AS009_DPF_Outlet_Temperature",
        "DT_AS077_Fuel_Cut_Off_Valve"
      };
    }
  }
}
