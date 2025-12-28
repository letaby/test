// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC1_Calibration_for_Service__NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC1_Calibration_for_Service__NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 28;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\MPC1 Calibration for Service (NGC).panel";
    }
  }

  public virtual string Guid => "882a394f-bc7a-4553-aaec-a129138a9aef";

  public virtual string DisplayName => "MPC1 Camera Alignment - Service";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MPC01T" };
  }

  public virtual IEnumerable<string> ProhibitedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "Econic-Waste"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Detroit Assurance";

  public virtual FilterTypes Filters => (FilterTypes) 1;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

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
      return (IEnumerable<Qualifier>) new Qualifier[7]
      {
        new Qualifier((QualifierTypes) 1, "MPC01T", "DT_disc01_Camera_Calibration_Overall_Camera_Calibration_Status"),
        new Qualifier((QualifierTypes) 1, "MPC01T", "DT_disc01_Camera_Calibration_Online_Camera_Calibration_Status"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MPC01T", "00FBED"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "MPC01T", "RT_End_of_Line_Calibration_RequestResults_Static_Camera_Calibration_Result"),
        new Qualifier((QualifierTypes) 1, "MPC01T", "DT_disc01_Camera_Calibration_Static_Camera_Calibration_Status"),
        new Qualifier((QualifierTypes) 1, "MPC01T", "DT_disc02_LDW_Function_Data_LDW_Function_State"),
        new Qualifier((QualifierTypes) 4, "MPC01T", "camera_height")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MPC01T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "DL_Static_Camera_Calibration_Data",
        "FN_HardReset",
        "DJ_SecurityAccess_Config_Dev"
      };
    }
  }
}
