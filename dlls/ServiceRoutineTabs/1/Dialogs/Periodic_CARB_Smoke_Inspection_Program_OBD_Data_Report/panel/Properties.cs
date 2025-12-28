// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Periodic_CARB_Smoke_Inspection_Program_OBD_Data_Report.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Periodic_CARB_Smoke_Inspection_Program_OBD_Data_Report.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 652;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Periodic CARB Smoke Inspection Program OBD Data Report.panel";
    }
  }

  public virtual string Guid => "d8ea1042-432a-49bc-83db-888f9ccb4dce";

  public virtual string DisplayName => "Periodic CARB Smoke Inspection Program OBD Data Report";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "J1939-0",
        "J1939-1",
        "J1939-61"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "OBD";

  public virtual FilterTypes Filters => (FilterTypes) 16 /*0x10*/;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[4]
      {
        "J1939-1",
        "J1939-61",
        "J1939-0",
        "MCM"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[26]
      {
        "DT_1221_4_7",
        "DT_1221_4_3",
        "DT_1223_7_8",
        "DT_1222_5_8",
        "DT_1223_7_6",
        "DT_1222_5_6",
        "DT_1223_7_7",
        "DT_1222_5_7",
        "DT_1221_4_6",
        "DT_1221_4_2",
        "DT_1221_4_5",
        "DT_1221_4_1",
        "DT_1223_8_5",
        "DT_1222_6_5",
        "DT_1223_8_4",
        "DT_1222_6_4",
        "DT_1223_8_3",
        "DT_1222_6_3",
        "DT_1223_8_2",
        "DT_1222_6_2",
        "DT_3069",
        "DT_3294",
        "DT_3295",
        "DT_3296",
        "DT_1213",
        "DT_3302"
      };
    }
  }
}
