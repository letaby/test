// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Clutch_Overload_Monitoring.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Clutch_Overload_Monitoring.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 49;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\TCM Clutch Overload Monitoring.panel";
    }
  }

  public virtual string Guid => "fa191bc3-9e6b-405c-9ff0-a96c7a79ba5b";

  public virtual string DisplayName => "Clutch Overload Monitoring";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "TCM01T",
        "TCM05T"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 4;

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
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_duration"),
        new Qualifier((QualifierTypes) 8, "TCM05T", "DT_2611_Clutch_overload_monitoring_Ring_buffer_event_1_Duration")
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
      return (IEnumerable<Qualifier>) new Qualifier[49]
      {
        new Qualifier((QualifierTypes) 2, "TCM01T", "RT_0471_KupplungUeberdrueckt_Zaehler_zuruecksetzen_Start"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2612_clutch_overload_monitoring_ring_buffer_event_2_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2613_clutch_overload_monitoring_ring_buffer_event_3_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2614_clutch_overload_monitoring_ring_buffer_event_4_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2615_clutch_overload_monitoring_ring_buffer_event_5_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2616_clutch_overload_monitoring_ring_buffer_event_6_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2617_clutch_overload_monitoring_ring_buffer_event_7_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2618_clutch_overload_monitoring_ring_buffer_event_8_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2619_clutch_overload_monitoring_ring_buffer_event_9_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261A_clutch_overload_monitoring_ring_buffer_event_10_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261B_clutch_overload_monitoring_ring_buffer_event_11_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261C_clutch_overload_monitoring_ring_buffer_event_12_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261D_clutch_overload_monitoring_ring_buffer_event_13_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261E_clutch_overload_monitoring_ring_buffer_event_14_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261F_clutch_overload_monitoring_ring_buffer_event_15_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2612_clutch_overload_monitoring_ring_buffer_event_2_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2613_clutch_overload_monitoring_ring_buffer_event_3_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2614_clutch_overload_monitoring_ring_buffer_event_4_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2615_clutch_overload_monitoring_ring_buffer_event_5_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2616_clutch_overload_monitoring_ring_buffer_event_6_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2617_clutch_overload_monitoring_ring_buffer_event_7_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2618_clutch_overload_monitoring_ring_buffer_event_8_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2619_clutch_overload_monitoring_ring_buffer_event_9_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261A_clutch_overload_monitoring_ring_buffer_event_10_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261B_clutch_overload_monitoring_ring_buffer_event_11_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261C_clutch_overload_monitoring_ring_buffer_event_12_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261D_clutch_overload_monitoring_ring_buffer_event_13_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261E_clutch_overload_monitoring_ring_buffer_event_14_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261F_clutch_overload_monitoring_ring_buffer_event_15_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_duration"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_peak_temperature"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2611_clutch_overload_monitoring_ring_buffer_event_1_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2612_clutch_overload_monitoring_ring_buffer_event_2_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2613_clutch_overload_monitoring_ring_buffer_event_3_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2614_clutch_overload_monitoring_ring_buffer_event_4_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2615_clutch_overload_monitoring_ring_buffer_event_5_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2616_clutch_overload_monitoring_ring_buffer_event_6_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2617_clutch_overload_monitoring_ring_buffer_event_7_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2618_clutch_overload_monitoring_ring_buffer_event_8_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_2619_clutch_overload_monitoring_ring_buffer_event_9_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261A_clutch_overload_monitoring_ring_buffer_event_10_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261B_clutch_overload_monitoring_ring_buffer_event_11_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261C_clutch_overload_monitoring_ring_buffer_event_12_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261D_clutch_overload_monitoring_ring_buffer_event_13_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261E_clutch_overload_monitoring_ring_buffer_event_14_milage"),
        new Qualifier((QualifierTypes) 8, "TCM01T", "DT_261F_clutch_overload_monitoring_ring_buffer_event_15_milage")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[1]
      {
        new ServiceCall("TCM01T", "RT_0471_KupplungUeberdrueckt_Zaehler_zuruecksetzen_Start", (IEnumerable<string>) new string[0])
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "TCM01T",
        "UDS-03",
        "TCM05T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "RT_0471_KupplungUeberdrueckt_Zaehler_zuruecksetzen_Start",
        "RT_0471_Reset_clutch_overload_event_counter_Start"
      };
    }
  }
}
