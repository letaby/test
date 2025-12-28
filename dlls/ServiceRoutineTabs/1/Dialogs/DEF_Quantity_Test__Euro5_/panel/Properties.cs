// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__Euro5_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__Euro5_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 38;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DEF Quantity Test (Euro5).panel";
    }
  }

  public virtual string Guid => "4679ca29-9fcc-4b29-b9f0-57c49671ce62";

  public virtual string DisplayName => "DEF Quantity Test";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MR201T" };
  }

  public virtual bool AllDevicesRequired => false;

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
        new Qualifier((QualifierTypes) 1, "MR201T", "DT_ADS_Priming_Request"),
        new Qualifier((QualifierTypes) 1, "MR201T", "DT_ADS_Pressure_dosing_enable_UPS"),
        new Qualifier((QualifierTypes) 1, "MR201T", "DT_AAS_Actual_DEF_Dosing_Quantity"),
        new Qualifier((QualifierTypes) 1, "MR201T", "DT_ADS_Status_DEF_pump"),
        new Qualifier((QualifierTypes) 1, "MR201T", "DT_AAS_DEF_Pressure"),
        new Qualifier((QualifierTypes) 1, "MR201T", "DT_AAS_Urea_Pump_Speed"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "MR201T", "RT_SR029D_EDU_Diagnosis_Routine_Request_Results_Urea_Quantity_Check"),
        new Qualifier((QualifierTypes) 256 /*0x0100*/, "Extension", "SP_DEFQuantityTest_MR2")
      };
    }
  }
}
