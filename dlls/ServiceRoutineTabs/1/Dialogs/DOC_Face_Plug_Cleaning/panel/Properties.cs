// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 88;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DOC Face Plug Cleaning.panel";
    }
  }

  public virtual string Guid => "f6578baa-e2b9-4884-aed0-17639d770e09";

  public virtual string DisplayName => "DOC Face Plug Cleaning";

  public virtual IEnumerable<string> SupportedDevices
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

  public virtual IEnumerable<string> ProhibitedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "DDEC16-DD13EURO5",
        "DDEC16-DD16EURO5"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[3]
      {
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_DOC_Face_Plug_Unclogging_Stop"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_DOC_Face_Plug_Unclogging_Start"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status")
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
      return (IEnumerable<Qualifier>) new Qualifier[27]
      {
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DocFacePlugUnclogging"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS143_ADS_Pump_Speed"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS094_Actual_Torque_Load"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM21T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status"),
        new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_DPF_Regen_Switch_Status"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS065_Actual_DPF_zone"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS069_Jake_Brake_1_PWM13"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS036_SCR_Inlet_NOx_Sensor"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS035_SCR_Outlet_NOx_Sensor"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS110_ADS_DEF_Pressure_2"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS005_DOC_Inlet_Pressure"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS033_Throttle_Valve_Commanded_Value"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS006_DPF_Outlet_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS008_DOC_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS009_DPF_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS018_SCR_Inlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS019_SCR_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS007_DOC_Inlet_Temperature"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DocFacePlugUnclogging"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_DOC_Face_Plug_Unclogging_Start"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_DOC_Face_Plug_Unclogging_Stop")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("ACM21T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status", (IEnumerable<string>) new string[0]),
        new ServiceCall("ACM21T", "RT_DOC_Face_Plug_Unclogging_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("ACM21T", "RT_DOC_Face_Plug_Unclogging_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "CPC04T",
        "SSAM02T",
        "ACM21T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[4]
      {
        "DT_DSL_DPF_Regen_Switch_Status",
        "RT_MSC_GetSwState_Start_Switch_033",
        "DT_AS014_DEF_Pressure",
        "DT_AS110_ADS_DEF_Pressure_2"
      };
    }
  }
}
