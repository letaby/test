// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 88;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Idle Speed Balance.panel";
    }
  }

  public virtual string Guid => "df1d35fa-b57f-41a2-ad37-db7cf26b86d8";

  public virtual string DisplayName => "Idle Speed Balance";

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
        "DD15EURO4",
        "MBE4000",
        "MBE900",
        "S60"
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
      return (IEnumerable<Qualifier>) new Qualifier[3]
      {
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS013_Coolant_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_AS014_Fuel_Temperature"),
        new Qualifier((QualifierTypes) 1, "MCM", "DT_DS019_Vehicle_Check_Status")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MCM" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[8]
      {
        "RT_SR066_Idle_Speed_Balance_Test_Start",
        "DT_ASL004_Engine_State",
        "DT_Idle_Speed_Balance_Values_Cylinder_1",
        "DT_Idle_Speed_Balance_Values_Cylinder_2",
        "DT_Idle_Speed_Balance_Values_Cylinder_3",
        "DT_Idle_Speed_Balance_Values_Cylinder_4",
        "DT_Idle_Speed_Balance_Values_Cylinder_5",
        "DT_Idle_Speed_Balance_Values_Cylinder_6"
      };
    }
  }
}
