// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.FaultCodeTabs.General.Monitor_Performance__EPA10_.panel.Properties
// Assembly: FaultCodeTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 35DAF471-66CA-4F8E-B39E-2FF7E69A8BE3
// Assembly location: C:\Users\petra\Downloads\Архив (2)\FaultCodeTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.FaultCodeTabs.General.Monitor_Performance__EPA10_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 99;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Fault Code Tabs\\General\\Monitor Performance (EPA10).panel";
    }
  }

  public virtual string Guid => "7b563059-cee7-452b-9964-935eae04965a";

  public virtual string DisplayName => "Monitor Performance";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[4]
      {
        "ACM02T",
        "ACM21T",
        "MCM02T",
        "MCM21T"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => false;

  public virtual string Category => "OBD";

  public virtual FilterTypes Filters => (FilterTypes) (int) ushort.MaxValue;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[4]
      {
        "DT_STO_Read_aggregated_RBM_group_rates_Assigned_RBM_group_number_1",
        "DT_STO_Read_aggregated_RBM_group_rates_Ignition_Cycle_counter",
        "DT_STO_Read_aggregated_RBM_group_rates_General_Denominator",
        "DT_STO_Read_aggregated_RBM_group_rates_Number_of_following_MU_rates"
      };
    }
  }
}
