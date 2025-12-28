// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__EPA10_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__EPA10_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 83;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\SCR Replacement (EPA10).panel";
    }
  }

  public virtual string Guid => "c57cbaf5-c651-4e76-84a2-4925e6ac6f12";

  public virtual string DisplayName => "SCR Replacement";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ACM02T",
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
        new Qualifier((QualifierTypes) 4, "ACM02T", "Time_Above_SCR_Inlet_Temp_1_Hour"),
        new Qualifier((QualifierTypes) 4, "ACM02T", "Time_Above_SCR_Inlet_Temp_1_Min"),
        new Qualifier((QualifierTypes) 4, "ACM02T", "Time_Above_SCR_Inlet_Temp_1_Sec"),
        new Qualifier((QualifierTypes) 4, "ACM02T", "Time_Above_SCR_Inlet_Temp_2"),
        new Qualifier((QualifierTypes) 4, "ACM02T", "Time_Above_SCR_Outlet_Temp")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ACM02T",
        "MCM02T"
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
