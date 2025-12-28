// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 145;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DPF Ash Accumulation.panel";
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
        "CPC2",
        "MCM"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[7]
      {
        "DDEC6-S60",
        "DDEC6-DD16",
        "DDEC6-DD15",
        "DDEC6-DD13",
        "DDEC6-MBE900",
        "DDEC6-MBE4000",
        "DDEC6-DD15EURO4"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 1, "CPC2", "DT_AS056_DPF_Ash_Content_Mileage"),
        new Qualifier((QualifierTypes) 4, "MCM", "e2p_dpf_ash_last_clean_dist")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "MCM",
        "CPC2"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[5]
      {
        "DT_AS078_Distance_till_Ash_Full",
        "RT_DPF_Ash_Mileage_Reset_Start",
        "RT_DPF_Ash_Mileage_Read_Request_Results_Status",
        "RT_SR014_SET_EOL_Default_Values_Start",
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
        "e2p_dpf_ash_last_clean_dist"
      };
    }
  }
}
