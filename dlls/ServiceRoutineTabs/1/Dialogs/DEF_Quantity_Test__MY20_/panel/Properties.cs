// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__MY20_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__MY20_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 33;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DEF Quantity Test (MY20).panel";
    }
  }

  public virtual string Guid => "4679ca29-9fcc-4b29-b9f0-57c49671ce62";

  public virtual string DisplayName => "DEF Quantity Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get
    {
      return (IEnumerable<string>) new string[2]
      {
        "ACM301T",
        "MCM21T"
      };
    }
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "Aftertreatment";

  public virtual FilterTypes Filters => (FilterTypes) 34;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

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
      return (IEnumerable<Qualifier>) new Qualifier[8]
      {
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS079_ADS_priming_request"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_DS002_Enable_ADS"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS160_Real_Time_ADS_DEF_Dosed_Quantity_g_hr"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS104_ADS_Doser_PWM"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS014_DEF_Pressure"),
        new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS143_ADS_Pump_Speed"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM301T", "RT_SCR_Dosing_Quantity_Check_Request_Results_status_of_service_function"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DEFQuantityTest_MY20")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ACM301T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "DT_AS014_DEF_Pressure"
      };
    }
  }

  public virtual IEnumerable<string> UserSourceSharedProcedureQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[1]
      {
        "SP_DEFQuantityTest_MY20"
      };
    }
  }
}
