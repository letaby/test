// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 133;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\PSI Learn Crank Tone Wheel Parameters.panel";
    }
  }

  public virtual string Guid => "6f54fa38-cea3-4cc8-ac2b-23261ac7f40d";

  public virtual string DisplayName => "PSI Learn Crank Tone Wheel Parameters";

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "FCCC-Petrol",
        "FCCC-LPG"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

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
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_110"),
        new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_190"),
        new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_91"),
        new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_168"),
        new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_70"),
        new Qualifier((QualifierTypes) 1, "J1939-3", "DT_523")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "MT88ECU",
        "UDS-0"
      };
    }
  }
}
