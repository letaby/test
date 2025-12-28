// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Metering_Unit_Flood_Routine__EPA10_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Metering_Unit_Flood_Routine__EPA10_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 17;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Metering Unit Flood Routine (EPA10).panel";
    }
  }

  public virtual string Guid => "8a4516f0-6cd3-4d0d-9a36-80e1f28c663e";

  public virtual string DisplayName => "Metering Unit Flood Routine";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ACM02T" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 66;

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
      return (IEnumerable<Qualifier>) new Qualifier[4]
      {
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS016_DEF_Air_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS014_DEF_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DEFDoserPurgeRoutine_EPA10")
      };
    }
  }
}
