// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__EPA10_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__EPA10_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 14;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DEF Quantity Test (EPA10).panel";
    }
  }

  public virtual string Guid => "a4336286-5029-47aa-a1d4-2a4a49d7a1fa";

  public virtual string DisplayName => "DEF Quantity Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ACM02T",
        "CPC02T"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "DD13",
        "DD15",
        "DD16"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

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
      return (IEnumerable<Qualifier>) new Qualifier[8]
      {
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS003_Enable_DEF_pump"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS014_DEF_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS016_DEF_Air_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS085_Actual_DEF_Dosing_Quantity"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS086_Requested_DEF_Dosing_Quantity"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DEFQuantityTest_EPA10")
      };
    }
  }
}
