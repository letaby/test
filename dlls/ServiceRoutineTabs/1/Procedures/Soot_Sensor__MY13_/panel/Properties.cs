// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Soot_Sensor__MY13_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Soot_Sensor__MY13_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 256 /*0x0100*/;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Soot Sensor (MY13).panel";
    }
  }

  public virtual string Guid => "07370e77-98a0-428c-9460-3994036f6d20";

  public virtual string DisplayName => "Soot Sensor";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ACM21T",
        "MCM21T"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[5]
      {
        "DDEC16-DD5",
        "DDEC16-DD13",
        "DDEC16-DD15",
        "DDEC16-DD16",
        "DDEC16-DD8"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 32 /*0x20*/;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[9]
      {
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Start"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR09F_Set_NOX_engine_start_restriction_Start"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_DPS_short_circuit_failure_value"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_DPS_sum_failure"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_Measurement_active_value"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_End_of_DSR_shutdown_in_case_of_cooled_down_sensor"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR09F_Set_NOX_engine_start_restriction_Stop"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR09F_Set_NOX_engine_start_restriction_Request_Results_Status"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Stop")
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
      return (IEnumerable<Qualifier>) new Qualifier[20]
      {
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_PMSensorInspection"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_End_of_DSR_shutdown_in_case_of_cooled_down_sensor"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_DS013_Soot_Sensor_Data_Prot_tube_monitoring_release"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS195_isp_dps_cid"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_DPS_short_circuit_failure_value"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_Measurement_active_value"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_DS012_PM_sensor_active_status"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_DS012_PM_sensor_regen_status"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_DS014_Soot_Sensor_Data_regeneration_cycle_finished"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_DS013_Soot_Sensor_Data_Regeneration_active"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM21T", "RT_SR0D3_PM_Sensor_inspection_Request_Results_DPS_sum_failure"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS189_absolute_current_of_PM_sensor"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS019_SCR_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS190_measured_temp_at_PM_sensor"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS194_isp_dps_t_mea_iv"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS053_Ambient_Air_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS009_DPF_Outlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS018_SCR_Inlet_Temperature"),
        new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS191_PM_sensor_PWM_control")
      };
    }
  }
}
