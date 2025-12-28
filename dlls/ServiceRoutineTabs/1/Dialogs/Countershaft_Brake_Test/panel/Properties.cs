// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Countershaft_Brake_Test.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Countershaft_Brake_Test.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 96 /*0x60*/;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Countershaft Brake Test.panel";
    }
  }

  public virtual string Guid => "5e980fb4-a3da-48b0-951b-dd2f266dd3aa";

  public virtual string DisplayName => "Countershaft Brake Test";

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
      return (IEnumerable<Qualifier>) new Qualifier[10]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_CountershaftBrakeTest_TCM01T"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_CountershaftBrakeTest_TCM05T"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd08_Istgang_Istgang"),
        new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_231A_Maximaler_Gradient_Vorgelegewellen_Drehzahl_max_Gradient_Vorgelegewellen_Drehzahl"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS023_Engine_State"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd03_Drehzahl_Vorgelegewelle_Drehzahl_Vorgelegewelle")
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
      return (IEnumerable<string>) new string[1]
      {
        "SP_CountershaftBrakeTest_"
      };
    }
  }
}
