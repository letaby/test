// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages__Euro5_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages__Euro5_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 9;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Voltages (Euro5).panel";
    }
  }

  public virtual string Guid => "66ebe157-c680-431e-b3a8-5a4a06f37bdd";

  public virtual string DisplayName => "Voltages";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "DDEC16-DD13EURO5",
        "DDEC16-DD16EURO5"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

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
      return (IEnumerable<Qualifier>) new Qualifier[13]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[14]
      {
        "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value0",
        "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value1",
        "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value4",
        "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value5",
        "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value6",
        "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value7",
        "RT_SR001_Sensor_Voltage_3V_Request_Results_Sensor_Value8",
        "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value2",
        "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value4",
        "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value5",
        "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value8",
        "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value9",
        "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value10",
        "RT_SR001_Sensor_Voltage_5V_Request_Results_Sensor_Value12"
      };
    }
  }
}
