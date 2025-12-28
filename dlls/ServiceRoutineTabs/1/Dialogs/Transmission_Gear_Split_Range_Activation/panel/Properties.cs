// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Gear_Split_Range_Activation.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Gear_Split_Range_Activation.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 219;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Transmission Gear Split Range Activation.panel";
    }
  }

  public virtual string Guid => "14ead815-96da-4931-885e-48ea5b531c88";

  public virtual string DisplayName => "Transmission Gear Split Range Activation";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "MCM21T",
        "TCM01T",
        "TCM05T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Transmission";

  public virtual FilterTypes Filters => (FilterTypes) 4;

  public virtual PanelUseCases UseCases => (PanelUseCases) 15;

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
      return (IEnumerable<Qualifier>) new Qualifier[9]
      {
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_TCM_Gear_Split_Range_Select_TCM01T"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_TCM_Gear_Split_Range_Select_TCM05T"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2306_Aktuator_Stellung_Split_Aktuator_Stellung_Split"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS012_Vehicle_Speed"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "CO_TransType"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2309_Aktuator_Stellung_Range_Aktuator_Stellung_Range"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd08_Istgang_Istgang")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "TCM01T",
        "UDS-03",
        "TCM05T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceSharedProcedureQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "SP_TCM_Gear_Split_Range_Select_",
        "SP_TCM_Gear_Split_Range_Select"
      };
    }
  }
}
