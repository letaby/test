// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Turbo_Actuator.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Turbo_Actuator.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 37;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Turbo Actuator.panel";
    }
  }

  public virtual string Guid => "c285aa6b-2e21-42fa-8dfe-73546b74256e";

  public virtual string DisplayName => "Turbo Actuator";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get => (IEnumerable<string>) new string[1]{ "S60" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 3;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[10]
      {
        "DT_AS052_SRA5_Status_Code",
        "RT_SR061_Pre_install_Routine_Start_ActuatorStatus",
        "RT_SR061_Pre_install_Routine_Request_Results_ActuatorResult",
        "RT_SR061_Pre_install_Routine_Stop_ActuatorNumber",
        "RT_SR062_Self_Calibration_Routine_Start_ActuatorStartStatus",
        "RT_SR062_Self_Calibration_Routine_Request_Results_ActuatorResultStatus",
        "RT_SR062_Self_Calibration_Routine_Stop_ActuatorNumber",
        "RT_SR063_Hysteres_Test_Routine_Start_ActuatorStartStatus",
        "RT_SR063_Hysteres_Test_Routine_Request_Results_Data",
        "RT_SR063_Hysteres_Test_Routine_Stop_ActuatorNumber"
      };
    }
  }
}
