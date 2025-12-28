// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 146;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Set VIN (EMG).panel";
    }
  }

  public virtual string Guid => "3b618fde-ca78-4447-a441-117388869096";

  public virtual string DisplayName => "Set Vehicle Identification Number";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[18]
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
        "DCB01T",
        "DCB02T",
        "DCL101T",
        "EAPU03T",
        "ECPC01T",
        "ETCM01T",
        "PTI101T",
        "PTI201T",
        "PTI301T"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual FaultCondition RequiredFaultCondition
  {
    get => new FaultCondition((FaultConditionType) 0, (IEnumerable<Qualifier>) new Qualifier[0]);
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[19]
      {
        "J1939-255",
        "ECPC01T",
        "ETCM01T",
        "BMS01T",
        "BMS201T",
        "BMS301T",
        "BMS401T",
        "BMS501T",
        "BMS601T",
        "BMS701T",
        "BMS801T",
        "BMS901T",
        "PTI101T",
        "PTI201T",
        "PTI301T",
        "DCL101T",
        "EAPU03T",
        "DCB01T",
        "DCB02T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "FN_HardReset",
        "DL_ID_VIN_Current"
      };
    }
  }
}
