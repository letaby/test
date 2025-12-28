// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Three_By_Two_Way_Valve_Teach_In__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Three_By_Two_Way_Valve_Teach_In__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 31 /*0x1F*/;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Three By Two Way Valve Teach In (EMG).panel";
    }
  }

  public virtual string Guid => "10d60094-a21f-45b9-bd02-17ba4671a240";

  public virtual string DisplayName => "3by2 Way Valve Teach In";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "ePowertrain";

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
      return (IEnumerable<Qualifier>) new Qualifier[21]
      {
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_BatteryCoolant3By2WayValveTeachIn"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_EdriveCoolant3By2WayTeachIn"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_BatteryCoolant3By2WayValveTeachIn"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_BatteryCirc"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_BasicCircValvePosCtrlState"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_EdriveCoolant3By2WayTeachIn"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_ExtensionCkt_1"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_ExtCircValvePosCtrlState_1")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[6]
      {
        new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_BatteryCirc", (IEnumerable<string>) new string[3]
        {
          "3by2WayValveBatteryCircuit=1",
          "3by2WayValveExtCircuit1=0",
          "3by2WayValveExtCircuit2=0"
        }),
        new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_BasicCircValvePosCtrlState", (IEnumerable<string>) new string[3]
        {
          "BasicCircValvePosCtrlState=1",
          "ExtCircValvePosCtrlState_1=0",
          "ExtCircValvePosCtrlState_2=0"
        }),
        new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_ExtensionCkt_1", (IEnumerable<string>) new string[3]
        {
          "3by2WayValveBatteryCircuit=0",
          "3by2WayValveExtCircuit1=1",
          "3by2WayValveExtCircuit2=0"
        }),
        new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_ExtCircValvePosCtrlState_1", (IEnumerable<string>) new string[3]
        {
          "BasicCircValvePosCtrlState=0",
          "ExtCircValvePosCtrlState_1=1",
          "ExtCircValvePosCtrlState_2=0"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }
}
