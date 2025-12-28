// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Clear_Non_Erasable_Fault_Codes.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clear_Non_Erasable_Fault_Codes.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 46;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Clear Non Erasable Fault Codes.panel";
    }
  }

  public virtual string Guid => "57f1e09c-d30a-4f4a-bdb2-61b1b93d59ab";

  public virtual string DisplayName => "Clear Non Erasable Fault code(s)";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[3]
      {
        "ACM21T",
        "ACM301T",
        "MCM21T"
      };
    }
  }

  public virtual IEnumerable<string> SupportedEquipment
  {
    get
    {
      return (IEnumerable<string>) new string[14]
      {
        "DDEC13-MDEG 4-Cylinder Tier4",
        "DDEC13-MDEG 6-Cylinder Tier4",
        "DDEC13-DD11 Tier4",
        "DDEC13-DD13 Tier4",
        "DDEC13-DD16 Tier4",
        "DDEC16-MDEG 4-Cylinder StageV",
        "DDEC16-MDEG 6-Cylinder StageV",
        "DDEC16-DD13EURO5",
        "DDEC20-MDEG 4-Cylinder StageV",
        "DDEC20-MDEG 6-Cylinder StageV",
        "DDEC20-DD11 StageV",
        "DDEC20-DD13 StageV",
        "DDEC20-DD16 StageV",
        "DDEC16-DD16EURO5"
      };
    }
  }

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 1;

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
      return (IEnumerable<string>) new string[3]
      {
        "ACM21T",
        "ACM301T",
        "MCM21T"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[6]
      {
        "DJ_Read_AUT64_VeDoc_Input",
        "DNU_Config_3_Seed_Request",
        "DJ_ZKB_Config_3_WriteKeyFingerprint",
        "RT_SR087_Delete_Non_Erasable_FC_Start",
        "DJ_Get_ECU_Serial_Number",
        "DT_STO_Read_look_up_table_1_sys_look_up_table1_1m"
      };
    }
  }
}
