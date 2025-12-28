// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__EPA10_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__EPA10_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 122;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DPF Ash Accumulation (EPA10).panel";
    }
  }

  public virtual string Guid => "c57cbaf5-c651-4e76-84a2-4925e6ac6f12";

  public virtual string DisplayName => "DPF Ash Accumulator";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "ACM02T",
        "CPC02T",
        "MCM02T"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "DDEC10-DD13",
        "DDEC10-DD15",
        "DDEC10-DD16"
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

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[3]
      {
        new Qualifier((QualifierTypes) 2, "CPC02T", "RT_DPF_Ash_Volume_Reset_Start"),
        new Qualifier((QualifierTypes) 2, "CPC02T", "RT_DPF_Ash_Volume_Read_Request_Results_Status"),
        new Qualifier((QualifierTypes) 2, "ACM02T", "RT_Ash_Volume_Ratio_Update_Start")
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
        new Qualifier((QualifierTypes) 1, "CPC02T", "DT_AS052_DPF_Ash_Volume"),
        new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS109_Ash_Filter_Full_Volume")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "ACM02T",
        "CPC02T",
        "MCM02T"
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
        "RT_DPF_Ash_Volume_Reset_Start",
        "RT_DPF_Ash_Volume_Read_Request_Results_Status",
        "RT_Ash_Volume_Ratio_Update_Start",
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
