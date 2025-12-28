// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance__MY13_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance__MY13_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 164;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Idle Speed Balance (MY13).panel";
    }
  }

  public virtual string Guid => "df1d35fa-b57f-41a2-ad37-db7cf26b86d8";

  public virtual string DisplayName => "Idle Speed Balance";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => false;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 2;

  public virtual PanelUseCases UseCases => (PanelUseCases) 10;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> ProhibitedQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[12]
      {
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_FuelmapPartNumber"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_CertificationNumber"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_SoftwareMode"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_ApplicationCode"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_ApplicationCodePartNumber"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_VIN"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_SoftwareVersion"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_DiagnosisVersion"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_EcuSerialNumber"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_HardwarePartNumber"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "CO_SoftwarePartNumber"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "DiagnosisVariant")
      };
    }
  }

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
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
        new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp"),
        new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp")
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual IEnumerable<string> UserSourceInstrumentQualifierReferences
  {
    get
    {
      return (IEnumerable<string>) new string[12]
      {
        "RT_SR02FA_set_TM_mode_Start",
        "RT_SR02FA_set_TM_mode_Stop",
        "RT_SR09A_Force_TMC_to_TMx_Mode_Start",
        "RT_SR066_Idle_Speed_Balance_Test_Start",
        "RT_SR0C5_Reset_ISC_Counter_in_ISC_Modul_Start",
        "DT_AS023_Engine_State",
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
