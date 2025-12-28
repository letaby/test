// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Auto_Config__NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Auto_Config__NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 42;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ICUC Auto Config (NGC).panel";
    }
  }

  public virtual string Guid => "a76e1ffc-16b4-4c5f-a6ac-9a36646737e0";

  public virtual string DisplayName => "Instrument Cluster Automatic Configuration";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ICC501T",
        "ICUC01T"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 9;

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
      return (IEnumerable<Qualifier>) new Qualifier[5]
      {
        new Qualifier((QualifierTypes) 32 /*0x20*/, "ICUC01T", "0DFBFF"),
        new Qualifier((QualifierTypes) 32 /*0x20*/, "ICUC01T", "0FFBFF"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_ICUC01T_AutoConfig"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_ICUC01T_AutoConfig_PID20"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_ICUC01T_AutoConfig_PID25")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "UDS-23",
        "ICUC01T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceSharedProcedureQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "SP_ICUC01T_AutoConfig",
        "SP_ICUC01T_AutoConfig_PID20",
        "SP_ICUC01T_AutoConfig_PID25"
      };
    }
  }
}
