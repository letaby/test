// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Battery_Management__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Battery_Management__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 21;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Battery Management (EMG).panel";
    }
  }

  public virtual string Guid => "58aecf4c-17be-4558-85f2-ce1d6a313ffe";

  public virtual string DisplayName => "Battery Management";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "BMS01T" };
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
      return (IEnumerable<Qualifier>) new Qualifier[30]
      {
        new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
        new Qualifier((QualifierTypes) 2, "BMS901T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS801T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS701T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS601T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS501T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS401T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS301T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS201T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS901T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS801T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS701T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS601T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS501T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS401T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS301T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS201T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "BMS901T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "BMS801T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "BMS701T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "BMS601T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "BMS501T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "BMS401T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "BMS301T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "BMS201T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 8, "BMS01T", "DT_STO_High_Voltage_Lock_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS01T", "DL_High_Voltage_Lock"),
        new Qualifier((QualifierTypes) 2, "BMS01T", "DL_High_Voltage_Lock")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[18]
      {
        new ServiceCall("BMS901T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=0"
        }),
        new ServiceCall("BMS801T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=0"
        }),
        new ServiceCall("BMS701T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=0"
        }),
        new ServiceCall("BMS601T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=0"
        }),
        new ServiceCall("BMS501T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=0"
        }),
        new ServiceCall("BMS401T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=0"
        }),
        new ServiceCall("BMS301T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=0"
        }),
        new ServiceCall("BMS201T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=0"
        }),
        new ServiceCall("BMS901T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=1"
        }),
        new ServiceCall("BMS801T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=1"
        }),
        new ServiceCall("BMS701T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=1"
        }),
        new ServiceCall("BMS601T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=1"
        }),
        new ServiceCall("BMS501T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=1"
        }),
        new ServiceCall("BMS401T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=1"
        }),
        new ServiceCall("BMS301T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=1"
        }),
        new ServiceCall("BMS201T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=1"
        }),
        new ServiceCall("BMS01T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=1"
        }),
        new ServiceCall("BMS01T", "DL_High_Voltage_Lock", (IEnumerable<string>) new string[1]
        {
          "High_Voltage_Lock=0"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[10]
      {
        "BMS01T",
        "BMS201T",
        "BMS301T",
        "BMS401T",
        "BMS501T",
        "BMS601T",
        "BMS701T",
        "BMS801T",
        "BMS901T",
        "ECPC01T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "DL_High_Voltage_Lock",
        "DT_STO_High_Voltage_Lock_High_Voltage_Lock"
      };
    }
  }
}
