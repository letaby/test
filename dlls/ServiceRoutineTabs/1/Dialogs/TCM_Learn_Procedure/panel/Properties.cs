// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Learn_Procedure.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Learn_Procedure.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 75;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\TCM Learn Procedure.panel";
    }
  }

  public virtual string Guid => "a0d55242-5240-4888-910b-fa326bb32484";

  public virtual string DisplayName => "Transmission Learn Procedure";

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
      return (IEnumerable<Qualifier>) new Qualifier[8]
      {
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "IgnitionStatusRequest"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_231B_Status_Einlernen_Kupplung_Status_Einlernen_Kupplung"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_TCM_Learn"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2111_Status_Einlernen_Getriebe_stGbLrn"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "TCM01T", "RT_0400_Einlernvorgang_Service_Request_Results_Fehler_Lernvorgang"),
        new Qualifier((QualifierTypes) 1, "TCM01T", "DT_2112_Anforderung_zum_Motorstart_waehrend_des_Einlernvorgangs_Anforderung_Motorstart")
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
        "SP_TCM_Learn"
      };
    }
  }
}
