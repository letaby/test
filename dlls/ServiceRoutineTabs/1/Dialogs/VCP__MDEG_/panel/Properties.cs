// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.VCP__MDEG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VCP__MDEG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 56;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\VCP (MDEG).panel";
    }
  }

  public virtual string Guid => "2e0407ca-6cbf-4210-9950-be1966e8fb27";

  public virtual string DisplayName => "Variable Camshaft Phaser (VCP)";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "DDEC16-DD5",
        "DDEC16-DD8"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 34;

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
      return (IEnumerable<Qualifier>) new Qualifier[11]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS012_Vehicle_Speed"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_VCP_Test"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Request_Results_Routine_status"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_VCP_Test"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Start"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS012_Vehicle_Speed"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Stop")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Request_Results_Routine_status", (IEnumerable<string>) new string[0]),
        new ServiceCall("MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }
}
