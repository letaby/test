// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.DCB_Unlock.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DCB_Unlock.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 11;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\DCB Unlock.panel";
    }
  }

  public virtual string Guid => "21b2728f-1965-4bd9-9b71-d7980cf7c74d";

  public virtual string DisplayName => "DCB Unlock";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "DCB01T",
        "DCB02T"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => false;

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
      return (IEnumerable<Qualifier>) new Qualifier[8]
      {
        new Qualifier((QualifierTypes) 2, "DCB02T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "DCB01T", "DT_STO_High_Voltage_Lock_HV_Lock_Status"),
        new Qualifier((QualifierTypes) 8, "DCB02T", "DT_STO_High_Voltage_Lock_HV_Lock_Status"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 2, "DCB01T", "DL_High_Voltage_Lock")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[2]
      {
        new ServiceCall("DCB02T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "HV_Lock_Status=0"
        }),
        new ServiceCall("DCB01T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "HV_Lock_Status=0"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "DCB01T",
        "DCB02T"
      };
    }
  }
}
