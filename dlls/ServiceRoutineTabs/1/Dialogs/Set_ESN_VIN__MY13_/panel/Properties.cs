// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_ESN_VIN__MY13_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_ESN_VIN__MY13_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 133;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Set ESN VIN (MY13).panel";
    }
  }

  public virtual string Guid => "3b618fde-ca78-4447-a441-117388869096";

  public virtual string DisplayName => "Set Engine Serial Number/Vehicle Identification Number";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[14]
      {
        "ACM02T",
        "ACM21T",
        "ACM301T",
        "CPC02T",
        "CPC04T",
        "CPC2",
        "CPC302T",
        "CPC501T",
        "CPC502T",
        "MCM",
        "MCM02T",
        "MCM21T",
        "MR201T",
        "TCM01T"
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

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[1]
      {
        new Qualifier((QualifierTypes) 8, "MCM21T", "CO_EquipmentType")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get
    {
      return (IEnumerable<string>) new string[10]
      {
        "MCM21T",
        "ACM21T",
        "CPC04T",
        "TCM01T",
        "MR201T",
        "CPC302T",
        "CPC501T",
        "CPC502T",
        "J1939-255",
        "MCM"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[6]
      {
        "DL_ID_Write_Engine_Serial_Number",
        "DL_ID_Engine_Serial_Number",
        "FN_KeyOffOnReset",
        "FN_HardReset",
        "DL_ID_Write_VIN_Current",
        "DL_ID_VIN_Current"
      };
    }
  }
}
