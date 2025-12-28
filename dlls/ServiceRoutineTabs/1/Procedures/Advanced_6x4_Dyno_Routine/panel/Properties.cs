// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Advanced_6x4_Dyno_Routine.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Advanced_6x4_Dyno_Routine.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 33;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Advanced_6x4_Dyno_Routine.panel";
    }
  }

  public virtual string Guid => "f3745a2d-d69f-4cb4-bb22-02c7c1f38a5d";

  public virtual string DisplayName => "Advanced 6x4 Dyno Routine";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "XMC02T" };
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
      return (IEnumerable<Qualifier>) new Qualifier[20]
      {
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_AFE_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_AFE_OutputCtrl_Return_Control"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_4_Read_Wheelspeed_4"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_3_Read_Wheelspeed_3"),
        new Qualifier((QualifierTypes) 1, "virtual", "accelPedalPosition"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_1_Read_Wheelspeed_1"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_2_Read_Wheelspeed_2"),
        new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_MDC_Sol_Stat"),
        new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_MDC_Open_Fb"),
        new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_MDC_Closed_Fb"),
        new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_DogCltch_Sol_Stat"),
        new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_RA2_Lt_Clutch_Fb"),
        new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_RA2_Rt_Clutch_Fb"),
        new Qualifier((QualifierTypes) 1, "XMC02T", "DT_AFE_Diagnostic_Displayables_DDAFE_AxlCurrState_Cval"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_AFE_OutputCtrl_Control"),
        new Qualifier((QualifierTypes) 2, "XMC02T", "IOC_AFE_OutputCtrl_Return_Control"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_5_Read_Wheelspeed_5"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Wheelspeed_Wheel_6_Read_Wheelspeed_6")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[4]
      {
        new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Control", (IEnumerable<string>) new string[4]
        {
          "DiagRqData_OC_MDC_Cmd_Enbl=1",
          "DiagRqData_OC_MDC_Cmd=1",
          "DiagRqData_OC_DgCltch_Cmd_Enbl=0",
          "DiagRqData_OC_DgCltch_Cmd=0"
        }),
        new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Return_Control", (IEnumerable<string>) new string[0]),
        new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Control", (IEnumerable<string>) new string[4]
        {
          "DiagRqData_OC_MDC_Cmd_Enbl=0",
          "DiagRqData_OC_MDC_Cmd=0",
          "DiagRqData_OC_DgCltch_Cmd_Enbl=1",
          "DiagRqData_OC_DgCltch_Cmd=1"
        }),
        new ServiceCall("XMC02T", "IOC_AFE_OutputCtrl_Return_Control", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "XMC02T" };
  }
}
