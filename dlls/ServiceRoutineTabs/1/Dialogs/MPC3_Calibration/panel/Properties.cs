// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC3_Calibration.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC3_Calibration.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 49;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\MPC3 Calibration.panel";
    }
  }

  public virtual string Guid => "58aaec29-d99f-40d2-9898-3248280abf01";

  public virtual string DisplayName => "MPC3 Calibration";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MPC03T" };
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
      return (IEnumerable<Qualifier>) new Qualifier[11]
      {
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_MPC3Calibration"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "MPC03T", "RT_Initial_Online_Calibration_Request_Results_IOCAL_Routine_Status"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 4, "MPC03T", "Camera_Height_Over_Ground"),
        new Qualifier((QualifierTypes) 1, "MPC03T", "DT_Initial_Calibration_Misalignment_Information_calibrationStatus"),
        new Qualifier((QualifierTypes) 1, "MPC03T", "DT_Initial_Calibration_Misalignment_Information_errorDetails"),
        new Qualifier((QualifierTypes) 2, "MPC03T", "RT_Initial_Online_Calibration_Request_Results_IOCAL_Routine_Status"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_MPC3Calibration"),
        new Qualifier((QualifierTypes) 2, "MPC03T", "RT_Initial_Online_Calibration_Start"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 2, "MPC03T", "RT_Initial_Online_Calibration_Stop")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("MPC03T", "RT_Initial_Online_Calibration_Request_Results_IOCAL_Routine_Status", (IEnumerable<string>) new string[0]),
        new ServiceCall("MPC03T", "RT_Initial_Online_Calibration_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("MPC03T", "RT_Initial_Online_Calibration_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MPC03T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "RT_Initial_Online_Calibration_Request_Results_Progress_in_Percentage"
      };
    }
  }
}
