// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.FICM_Routine.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.FICM_Routine.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 24;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\FICM Routine.panel";
    }
  }

  public virtual string Guid => "defa4a63-bdf9-46fd-a5bd-0baf415142e8";

  public virtual string DisplayName => "Fuel Injector Cleaning Machine Routine";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[9]
      {
        "CPC02T",
        "CPC04T",
        "CPC2",
        "CPC302T",
        "CPC501T",
        "CPC502T",
        "MCM",
        "MCM02T",
        "MCM21T"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[23]
      {
        "DDEC10-DD13",
        "DDEC10-DD15",
        "DDEC10-DD16",
        "DDEC10-DD15EURO4",
        "DDEC13-DD13",
        "DDEC13-DD15",
        "DDEC13-DD16",
        "DDEC13-DD13 Tier4",
        "DDEC13-DD16 Tier4",
        "DDEC16-DD13",
        "DDEC16-DD15",
        "DDEC16-DD16",
        "DDEC16-DD13EURO5",
        "DDEC16-DD16EURO5",
        "DDEC20-DD13",
        "DDEC20-DD15",
        "DDEC20-DD16",
        "DDEC20-DD13 StageV",
        "DDEC20-DD16 StageV",
        "DDEC6-DD16",
        "DDEC6-DD15",
        "DDEC6-DD15EURO4",
        "DDEC6-DD13"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Fuel System";

  public virtual FilterTypes Filters => (FilterTypes) 3;

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
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_Powertrain_Repair_Validation_Routine1"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_FuelInjectorCleaningMachine_Routine"),
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "VehicleCheckStatus"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed")
      };
    }
  }
}
