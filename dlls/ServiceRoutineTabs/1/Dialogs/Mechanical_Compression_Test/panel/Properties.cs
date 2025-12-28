// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Mechanical_Compression_Test.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Mechanical_Compression_Test.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 122;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Mechanical Compression Test.panel";
    }
  }

  public virtual string Guid => "2e0407ca-6cbf-4210-9950-be1966e8fb27";

  public virtual string DisplayName => "Mechanical Compression Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[18]
      {
        "DDEC13-DD13",
        "DDEC13-DD15",
        "DDEC13-DD16",
        "DDEC16-DD5",
        "DDEC16-DD8",
        "DDEC16-DD13",
        "DDEC16-DD15",
        "DDEC16-DD16",
        "DDEC20-DD13",
        "DDEC20-DD15",
        "DDEC20-DD16",
        "DDEC20-MDEG 4-Cylinder StageV",
        "DDEC20-MDEG 6-Cylinder StageV",
        "DDEC20-DD11 StageV",
        "DDEC20-DD13 StageV",
        "DDEC20-DD16 StageV",
        "DDEC16-DD13EURO5",
        "DDEC16-DD16EURO5"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0")
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
      return (IEnumerable<Qualifier>) new Qualifier[13]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS021_Battery_Voltage"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS012_Vehicle_Speed"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_Compression_Test"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_Compression_Test"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS012_Vehicle_Speed"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS021_Battery_Voltage"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status", (IEnumerable<string>) new string[0]),
        new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status", (IEnumerable<string>) new string[0]),
        new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", (IEnumerable<string>) new string[0])
      };
    }
  }
}
