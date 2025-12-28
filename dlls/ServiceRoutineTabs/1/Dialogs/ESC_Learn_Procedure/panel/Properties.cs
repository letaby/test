// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ESC_Learn_Procedure.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ESC_Learn_Procedure.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 105;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ESC Learn Procedure.panel";
    }
  }

  public virtual string Guid => "1550b8c6-4855-4e57-bcad-7e92a09af8fc";

  public virtual string DisplayName => "ESC Learning Procedure";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ABS02T",
        "SBSP01T"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Anti-lock Braking System";

  public virtual FilterTypes Filters => (FilterTypes) 8;

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
      return (IEnumerable<Qualifier>) new Qualifier[9]
      {
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Yaw_rate_plausibility"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Offset_lateral_acceleration_learned"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_1_Read_Offset_steering_wheel_angle_learned"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_1_Read_Lateral_acceleration_plausibility"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Service_mode_active"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Steering_wheel_angle_plausibility"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_Start_ESC_learning_Start_Routine_Start"),
        new Qualifier((QualifierTypes) 2, "ABS02T", "RT_Start_ESC_learning_Start_Routine_Start")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[2]
      {
        new ServiceCall("ABS02T", "RT_Start_ESC_learning_Start_Routine_Start", (IEnumerable<string>) new string[1]
        {
          "Learning_mode=2"
        }),
        new ServiceCall("ABS02T", "RT_Start_ESC_learning_Start_Routine_Start", (IEnumerable<string>) new string[1]
        {
          "Learning_mode=3"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ABS02T" };
  }
}
