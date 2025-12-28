// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__MY13_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__MY13_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 101;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Relative Compression Test (MY13).panel";
    }
  }

  public virtual string Guid => "cb372ec6-6be5-46a0-a980-0cda9046762e";

  public virtual string DisplayName => "Relative Compression Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[5]
      {
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR003_PWM_Routine_by_Function_Start_Function_Name"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR003_PWM_Routine_by_Function_Stop_Function_Name"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Request_Results_acd_activate_status_bit_0")
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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status")
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
      return (IEnumerable<string>) new string[5]
      {
        "RT_SR006_Automatic_Compression_Test_Start_Status",
        "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0",
        "RT_SR006_Automatic_Compression_Test_Request_Results_acd_activate_status_bit_0",
        "RT_SR003_PWM_Routine_by_Function_Start_Function_Name",
        "RT_SR003_PWM_Routine_by_Function_Stop_Function_Name"
      };
    }
  }
}
