// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Maintenance_System.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Maintenance_System.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 149;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Detroit Maintenance System.panel";
    }
  }

  public virtual string Guid => "1d511cb7-6bf8-44a6-b2fb-e5dda2d0ba2b";

  public virtual string DisplayName => "Detroit Maintenance System";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "CGW05T",
        "MS01T"
      };
    }
  }

  public virtual IEnumerable<string> ProhibitedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "EMOBILITY-eDrive Powertrain",
        "EMOBILITY-eDrive Powertrain"
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

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 8, "CGW05T", "DT_STO_MS_Air_filter_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_LF"),
        new Qualifier((QualifierTypes) 8, "MS01T", "DT_STO_Air_filter_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_LF")
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
      return (IEnumerable<Qualifier>) new Qualifier[7]
      {
        new Qualifier((QualifierTypes) 2, "MS01T", "DL_Reset_service_information_selected_channel"),
        new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "virtual", "ignitionStatus"),
        new Qualifier((QualifierTypes) 4, "MS01T", "Minimum_driven_distance_reset_PAR_FsMinRs_FZG"),
        new Qualifier((QualifierTypes) 2, "MS01T", "DL_Reset_service_information_selected_channel"),
        new Qualifier((QualifierTypes) 2, "MS01T", "DL_Reset_service_information_selected_channel"),
        new Qualifier((QualifierTypes) 2, "MS01T", "DL_Reset_service_information_selected_channel")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[4]
      {
        new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>) new string[2]
        {
          "Channel_number=2",
          "Filter_condition_Diesel_particle_filter_only=0"
        }),
        new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>) new string[2]
        {
          "Channel_number=1",
          "Filter_condition_Diesel_particle_filter_only=0"
        }),
        new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>) new string[2]
        {
          "Channel_number=6",
          "Filter_condition_Diesel_particle_filter_only=0"
        }),
        new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>) new string[2]
        {
          "Channel_number=20",
          "Filter_condition_Diesel_particle_filter_only=0"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "MS01T",
        "CGW05T",
        "MSF01T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[14]
      {
        "DL_Reset_service_information_selected_channel",
        "DT_STO_Engine_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_MOT",
        "DT_STO_Engine_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_MOT",
        "DT_STO_Transmission_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_GET",
        "DT_STO_Transmission_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_GET",
        "DT_STO_Rear_axle_1_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_HA1",
        "DT_STO_Rear_axle_1_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_HA1",
        "DT_STO_Rear_axle_2_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_HA2",
        "DT_STO_Rear_axle_2_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_HA2",
        "DT_STO",
        "DL_",
        "DT_STO_",
        "DT_STO_MS_",
        "DL_MS_"
      };
    }
  }
}
