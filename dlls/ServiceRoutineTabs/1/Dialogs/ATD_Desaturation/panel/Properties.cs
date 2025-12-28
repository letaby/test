// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Desaturation.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Desaturation.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 27;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ATD Desaturation.panel";
    }
  }

  public virtual string Guid => "0fe4854e-6fd3-42c6-959f-edfef0264047";

  public virtual string DisplayName => "ATD Desaturation";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[10]
      {
        "ACM02T",
        "ACM21T",
        "ACM301T",
        "CPC02T",
        "CPC04T",
        "CPC302T",
        "CPC501T",
        "CPC502T",
        "MCM02T",
        "MCM21T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

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
      return (IEnumerable<Qualifier>) new Qualifier[7]
      {
        new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "DOCInletTemperature"),
        new Qualifier((QualifierTypes) 1, "virtual", "DOCOutletTemperature"),
        new Qualifier((QualifierTypes) 1, "virtual", "DPFOutletTemperature"),
        new Qualifier((QualifierTypes) 1, "virtual", "SCRInletTemperature"),
        new Qualifier((QualifierTypes) 1, "virtual", "SCROutletTemperature"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DisableHcDoserParkedRegen_NGC")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[6]
      {
        "CPC02T",
        "CPC04T",
        "CPC302T",
        "CPC501T",
        "CPC502T",
        "ACM301T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceSharedProcedureQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[5]
      {
        "SP_DisableHcDoserParkedRegen_NGC",
        "SP_DisableHcDoserParkedRegen_MY13",
        "SP_DisableHcDoserParkedRegen_CPC5",
        "SP_DisableHcDoserParkedRegen_45X",
        "SP_DisableHcDoserParkedRegen_EPA10"
      };
    }
  }
}
