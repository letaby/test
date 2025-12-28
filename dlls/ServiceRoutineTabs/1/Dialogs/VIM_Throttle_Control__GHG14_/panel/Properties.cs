// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.VIM_Throttle_Control__GHG14_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VIM_Throttle_Control__GHG14_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 146;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\VIM Throttle Control (GHG14).panel";
    }
  }

  public virtual string Guid => "2b4946a0-299d-4b4d-b754-d0cdbc510356";

  public virtual string DisplayName => "VIM Throttle Control";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "CPC04T" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[1]
      {
        new Qualifier((QualifierTypes) 4, "CPC04T", "Accel_Pedal_Type")
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
      return (IEnumerable<Qualifier>) new Qualifier[3]
      {
        new Qualifier((QualifierTypes) 1, "CPC04T", "DT_ASL_Accelerator_Pedal_Position"),
        new Qualifier((QualifierTypes) 1, "CPC04T", "DT_ASL_Actual_Engine_Speed"),
        new Qualifier((QualifierTypes) 4, "CPC04T", "Accel_Pedal_Type")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "CPC04T" };
  }
}
