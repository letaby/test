// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__MY20_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__MY20_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 173;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DPF Ash Accumulation (MY20).panel";
    }
  }

  public virtual string Guid => "c57cbaf5-c651-4e76-84a2-4925e6ac6f12";

  public virtual string DisplayName => "DPF Ash Accumulator";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ACM301T",
        "MCM21T"
      };
    }
  }

  public virtual IEnumerable<string> ProhibitedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "DDEC16-DD13EURO5",
        "DDEC16-DD16EURO5"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 15;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS109_Ash_Filter_Full_Volume"),
        new Qualifier((QualifierTypes) 4, "ACM301T", "ATD_Hardware_Type"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR08B_DPF_ash_volume_ratio_update_Start"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR08B_DPF_ash_volume_ratio_update_Request_Results_E2P_DPF_ASH_VOL_ACM"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS237_DPF_Ash_volume_from_ACM")
      };
    }
  }

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS237_DPF_Ash_volume_from_ACM"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS109_Ash_Filter_Full_Volume")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "ACM301T",
        "MCM21T",
        "CPC04T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[5]
      {
        "DT_AS109_Ash_Filter_Full_Volume",
        "RT_SR08B_DPF_ash_volume_ratio_update_Start",
        "RT_SR08B_DPF_ash_volume_ratio_update_Request_Results_E2P_DPF_ASH_VOL_ACM",
        "RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction",
        "DT_AS045_Engine_Operating_Hours"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceParameterQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "ATD_Hardware_Type"
      };
    }
  }
}
