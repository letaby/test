// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC2_Calibration__NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC2_Calibration__NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 43;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\MPC2 Calibration (NGC).panel";
    }
  }

  public virtual string Guid => "58aaec29-d99f-40d2-9898-3248280abf01";

  public virtual string DisplayName => "MPC2 Calibration";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MPC02T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Detroit Assurance";

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
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_MPC2Calibration"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "MPC02T", "RT_iOCAL_Routine_Control_Request_Results_Status"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 4, "MPC02T", "camera_height"),
        new Qualifier((QualifierTypes) 2, "MPC02T", "RT_iOCAL_Routine_Control_Request_Results_Status"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_MPC2Calibration"),
        new Qualifier((QualifierTypes) 2, "MPC02T", "RT_iOCAL_Routine_Control_Start"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 2, "MPC02T", "RT_iOCAL_Routine_Control_Stop")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("MPC02T", "RT_iOCAL_Routine_Control_Request_Results_Status", (IEnumerable<string>) new string[0]),
        new ServiceCall("MPC02T", "RT_iOCAL_Routine_Control_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("MPC02T", "RT_iOCAL_Routine_Control_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MPC02T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "RT_iOCAL_Routine_Control_Request_Results_Progress"
      };
    }
  }
}
