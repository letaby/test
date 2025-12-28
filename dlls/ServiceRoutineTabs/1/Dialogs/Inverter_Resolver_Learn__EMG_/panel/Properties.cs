// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Inverter_Resolver_Learn__EMG_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Inverter_Resolver_Learn__EMG_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 41;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Inverter Resolver Learn (EMG).panel";
    }
  }

  public virtual string Guid => "93ff5319-c809-4d26-9861-c91dc263af0f";

  public virtual string DisplayName => "Inverter Resolver Learn";

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
      return (IEnumerable<Qualifier>) new Qualifier[24]
      {
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS124_Actual_E_Motor_Speed_E_Motor_1_Actual_E_Motor_Speed_E_Motor_1"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS125_Actual_E_Motor_Speed_E_Motor_2_Actual_E_Motor_Speed_E_Motor_2"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS126_Actual_E_Motor_Speed_E_Motor_3_Actual_E_Motor_Speed_E_Motor_3"),
        new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
        new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT1"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT1"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT2"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT2"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT3"),
        new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
        new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
        new Qualifier((QualifierTypes) 2, "ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT3")
      };
    }
  }

  public virtual IEnumerable<ServiceCall> DesignerServiceCallReferences
  {
    get
    {
      return (IEnumerable<ServiceCall>) new ServiceCall[9]
      {
        new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT1", (IEnumerable<string>) new string[3]
        {
          "Resolver_teach_in_Var1=1",
          "Resolver_teach_in_Var2=0",
          "Resolver_teach_in_Var3=0"
        }),
        new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT1", (IEnumerable<string>) new string[3]
        {
          "Resolver_teach_in_Var1=1",
          "Resolver_teach_in_Var2=0",
          "Resolver_teach_in_Var3=0"
        }),
        new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT2", (IEnumerable<string>) new string[3]
        {
          "Resolver_teach_in_Var1=0",
          "Resolver_teach_in_Var2=1",
          "Resolver_teach_in_Var3=0"
        }),
        new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT2", (IEnumerable<string>) new string[3]
        {
          "Resolver_teach_in_Var1=0",
          "Resolver_teach_in_Var2=1",
          "Resolver_teach_in_Var3=0"
        }),
        new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3", (IEnumerable<string>) new string[0]),
        new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT3", (IEnumerable<string>) new string[3]
        {
          "Resolver_teach_in_Var1=0",
          "Resolver_teach_in_Var2=0",
          "Resolver_teach_in_Var3=1"
        }),
        new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT3", (IEnumerable<string>) new string[3]
        {
          "Resolver_teach_in_Var1=0",
          "Resolver_teach_in_Var2=0",
          "Resolver_teach_in_Var3=1"
        })
      };
    }
  }

  public virtual IEnumerable<string> UserSourceEcuReferences
  {
    get => (IEnumerable<string>) new string[1]{ "ECPC01T" };
  }
}
