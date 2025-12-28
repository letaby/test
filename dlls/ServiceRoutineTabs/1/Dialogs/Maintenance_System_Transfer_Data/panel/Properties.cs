// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 16 /*0x10*/;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Maintenance System Transfer Data.panel";
    }
  }

  public virtual string Guid => "c887b6fe-150e-4c81-9848-70a85a1781a0";

  public virtual string DisplayName => "Maintenance System Transfer Data";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MS01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 9;

  public virtual PanelUseCases UseCases => (PanelUseCases) 14;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[3]
      {
        new Qualifier((QualifierTypes) 2, "MS01T", "RT_Transfer_data_to_the_mirror_memory_Start"),
        new Qualifier((QualifierTypes) 2, "MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory"),
        new Qualifier((QualifierTypes) 2, "MS01T", "RT_Transfer_data_from_the_mirror_memory_Start")
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
      return (IEnumerable<Qualifier>) new Qualifier[6]
      {
        new Qualifier((QualifierTypes) 2, "MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_TransferDataFromMirrorMemory"),
        new Qualifier((QualifierTypes) 2, "MS01T", "RT_Transfer_data_from_the_mirror_memory_Start"),
        new Qualifier((QualifierTypes) 2, "MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_TransferDataFromMirrorMemory"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[3]
      {
        new ServiceCall("MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory", (IEnumerable<string>) new string[0]),
        new ServiceCall("MS01T", "RT_Transfer_data_from_the_mirror_memory_Start", (IEnumerable<string>) new string[0]),
        new ServiceCall("MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory", (IEnumerable<string>) new string[0])
      };
    }
  }
}
