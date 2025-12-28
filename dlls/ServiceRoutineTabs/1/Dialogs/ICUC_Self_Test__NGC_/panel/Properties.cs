// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Self_Test__NGC_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Self_Test__NGC_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 16 /*0x10*/;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ICUC Self Test (NGC).panel";
    }
  }

  public virtual string Guid => "b3ef4c71-5453-49fc-aea6-7daa40b9fb73";

  public virtual string DisplayName => "ICUC Self Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "ICUC01T" };
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

  public virtual IEnumerable<Qualifier> DesignerQualifierReferences
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[8]
      {
        new Qualifier((QualifierTypes) 2, "ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5"),
        new Qualifier((QualifierTypes) 2, "ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5"),
        new Qualifier((QualifierTypes) 2, "ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5"),
        new Qualifier((QualifierTypes) 2, "ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5"),
        new Qualifier((QualifierTypes) 2, "ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5"),
        new Qualifier((QualifierTypes) 2, "ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5"),
        new Qualifier((QualifierTypes) 2, "ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5"),
        new Qualifier((QualifierTypes) 2, "ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[8]
      {
        new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
        {
          "OptionRecord_Byte5=1",
          "OptionRecord_Byte6=00",
          "OptionRecord_Byte7=00"
        }),
        new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
        {
          "OptionRecord_Byte5=4",
          "OptionRecord_Byte6=00",
          "OptionRecord_Byte7=00"
        }),
        new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
        {
          "OptionRecord_Byte5=2",
          "OptionRecord_Byte6=00",
          "OptionRecord_Byte7=00"
        }),
        new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
        {
          "OptionRecord_Byte5=2",
          "OptionRecord_Byte6=00",
          "OptionRecord_Byte7=00"
        }),
        new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
        {
          "OptionRecord_Byte5=1",
          "OptionRecord_Byte6=00",
          "OptionRecord_Byte7=00"
        }),
        new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
        {
          "OptionRecord_Byte5=3",
          "OptionRecord_Byte6=00",
          "OptionRecord_Byte7=00"
        }),
        new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Stop_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
        {
          "OptionRecord_Byte5=3",
          "OptionRecord_Byte6=00",
          "OptionRecord_Byte7=00"
        }),
        new ServiceCall("ICUC01T", "RT_Self_Test_Routine_Start_OptionRecord_Byte5", (IEnumerable<string>) new string[3]
        {
          "OptionRecord_Byte5=4",
          "OptionRecord_Byte6=00",
          "OptionRecord_Byte7=00"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ICUC01T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "FN_HardReset"
      };
    }
  }
}
