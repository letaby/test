// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Tests.Variable_Speed_Fan__MY13_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Tests.Variable_Speed_Fan__MY13_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 70;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Tests\\Variable Speed Fan (MY13).panel";
    }
  }

  public virtual string Guid => "ca9a2e26-6276-4680-bf3c-6f9040780ae6";

  public virtual string DisplayName => "Variable Speed Fan Control";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  protected virtual IEnumerable<string> RequiredDataItemConditionsSource
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "Parameter.MCM21T.Fan_Type:(Fault),(0,Fault),(1,Fault),(2,Ok),(3,Ok),(4,Fault),(5,Ok),(6,Fault),(7,Fault),(8,Ok),(9,Ok),(10,Ok),(11,Ok),(12,Ok),(13,Ok)"
      };
    }
  }

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 4, "MCM21T", "Fan_Type"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS026_Fan_Speed"),
        new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS169_Coolant_out_temperature"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "RT_SR003_PWM_Routine_by_Function_Start_Control_Value",
        "RT_SR003_PWM_Routine_by_Function_Stop_Function_Name"
      };
    }
  }
}
