// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Actuator_Functional_Check.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Actuator_Functional_Check.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 20;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGR Valve Actuator Functional Check.panel";
    }
  }

  public virtual string Guid => "8bbc8347-b40d-456d-abcc-328b5ff67d53";

  public virtual string DisplayName => "EGR Valve Actuator Functional Check";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "EGR";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[4]
      {
        new Qualifier((QualifierTypes) 2, "MCM", "RT_SR0B2_EGR_IAE_Test_Stop"),
        new Qualifier((QualifierTypes) 2, "MCM", "RT_SR0B2_EGR_IAE_Test_Start"),
        new Qualifier((QualifierTypes) 2, "MCM", "RT_SR0B2_EGR_IAE_Test_Request_Results_Status"),
        new Qualifier((QualifierTypes) 2, "MCM", "RT_SR0B2_EGR_IAE_Test_Request_Results_EGR_Value_Position")
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
      return (IEnumerable<Qualifier>) new Qualifier[10]
      {
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_EGR_IAE_EPA07"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "C80600"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "C80700"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "9A0F00"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "9A1000"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "C80900"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "4E0800"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM", "4E0500"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS023_Engine_State"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS013_Coolant_Temperature")
      };
    }
  }
}
