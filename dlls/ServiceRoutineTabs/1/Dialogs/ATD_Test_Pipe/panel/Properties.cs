// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Test_Pipe.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Test_Pipe.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 41;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ATD Test Pipe.panel";
    }
  }

  public virtual string Guid => "2b4946a0-299d-4b4d-b754-d0cdbc510356";

  public virtual string DisplayName => "ATD Test Pipe";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[6]
      {
        "DD13",
        "DD15",
        "DD16",
        "MBE4000",
        "MBE900",
        "S60"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

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
      return (IEnumerable<Qualifier>) new Qualifier[2]
      {
        new Qualifier((QualifierTypes) 2, "MCM", "RT_SR017_ATD_Test_Sensor_Package_Start"),
        new Qualifier((QualifierTypes) 2, "MCM", "RT_SR017_ATD_Test_Sensor_Package_Stop")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[2]
      {
        new ServiceCall("MCM", "RT_SR017_ATD_Test_Sensor_Package_Start", (IEnumerable<string>) new string[1]
        {
          "20000"
        }),
        new ServiceCall("MCM", "RT_SR017_ATD_Test_Sensor_Package_Stop", (IEnumerable<string>) new string[0])
      };
    }
  }
}
