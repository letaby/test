// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Actuator_Slow_Learn__MY13_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Actuator_Slow_Learn__MY13_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 42;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGR Actuator Slow Learn (MY13).panel";
    }
  }

  public virtual string Guid => "1550b8c6-4855-4e57-bcad-7e92a09af8fc";

  public virtual string DisplayName => "EGR Actuator Slow Learn";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[23]
      {
        "DDEC13-DD13",
        "DDEC13-DD15",
        "DDEC13-DD16",
        "DDEC13-MDEG 4-Cylinder Tier4",
        "DDEC13-MDEG 6-Cylinder Tier4",
        "DDEC13-DD11 Tier4",
        "DDEC13-DD13 Tier4",
        "DDEC13-DD16 Tier4",
        "DDEC16-DD13",
        "DDEC16-DD15",
        "DDEC16-DD16",
        "DDEC16-MDEG 4-Cylinder StageV",
        "DDEC16-MDEG 6-Cylinder StageV",
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

  public virtual string Category => "EGR";

  public virtual FilterTypes Filters => (FilterTypes) 2;

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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS050_SRA3_Status_Code"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_EGRActuatorSlowLearn_MY13")
      };
    }
  }
}
