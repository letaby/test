// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Injector_Codes__MY13_.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Injector_Codes__MY13_.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 179;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Injector Codes (MY13).panel";
    }
  }

  public virtual string Guid => "11a29a85-8936-447f-ade2-d065ffdaf568";

  public virtual string DisplayName => "Injector Codes";

  public virtual IEnumerable<string> SupportedDevices
  {
    get => (IEnumerable<string>) new string[1]{ "MCM21T" };
  }

  public virtual bool AllDevicesRequired => true;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 66;

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
      return (IEnumerable<Qualifier>) new Qualifier[14]
      {
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_6"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_1"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_6_NOP0"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_5_NOP0"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_4_NOP0"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_3_NOP0"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_2_NOP0"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_1_NOP0"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_5"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_4"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_3"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_2"),
        new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS023_Engine_State"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value")
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
        "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value",
        "DJ_Read_E_Trim",
        "DJ_Write_E_Trim",
        "RT_SR070_Injector_Change_Start",
        "RT_SR074_EcuInitPIR_Start",
        "RT_SR014_SET_EOL_Default_Values_Start",
        "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_1_NOP0",
        "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_2_NOP0",
        "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_3_NOP0",
        "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_4_NOP0",
        "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_5_NOP0",
        "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_6_NOP0"
      };
    }
  }
}
