// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Calibration.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Calibration.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 177;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Dynamic Ride Height Calibration.panel";
    }
  }

  public virtual string Guid => "ef958949-eeaf-4c69-b402-b1007369fd52";

  public virtual string DisplayName => "Aerodynamic Height Control Calibration";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "HSV",
        "XMC02T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 1;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

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
      return (IEnumerable<Qualifier>) new Qualifier[7]
      {
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_BrkAirPress2_Stat_EAPU"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1724"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1722"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1721"),
        new Qualifier((QualifierTypes) 1, "HSV", "DT_1723"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "HSV",
        "XMC02T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[8]
      {
        "DT_1739",
        "DT_1756",
        "DT_1740",
        "DT_1755",
        "RT_HIGHESTLVL",
        "RT_NOMLVL1",
        "RT_NOMLVL2",
        "RT_LOESTLVL"
      };
    }
  }
}
