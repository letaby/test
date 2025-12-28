// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.HV_Active_Discharge__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HV_Active_Discharge__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 37;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\HV Active Discharge (EMG).panel";
    }
  }

  public virtual string Guid => "341d2647-af02-44cb-91fd-6448e7402af1";

  public virtual string DisplayName => "High Voltage Active Discharge";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "ePowertrain";

  public virtual FilterTypes Filters => (FilterTypes) 1;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> ForceQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS219_globalhvil_globalhvil"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS008_HV_Ready")
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
      return (IEnumerable<Qualifier>) new Qualifier[9]
      {
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_OTF_HV_ActiveDischarge"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Start"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS008_HV_Ready"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Stop"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_OTF_HV_ActiveDischarge"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS008_HV_Ready"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS219_globalhvil_globalhvil")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }
}
