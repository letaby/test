// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel.Properties
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;
using System;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel;

public class Properties(Type runtimeType) : PanelProperties(runtimeType)
{
  public virtual int FileVersion => 36;

  public virtual string FilePath
  {
    get
    {
      return "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\XCPCCPActivation.panel";
    }
  }

  public virtual string Guid => "bc630f2c-a004-41b1-ac34-69069c227188";

  public virtual string DisplayName => "XCP/CCP Activation";

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

  public virtual bool AllDevicesRequired => false;

  public virtual bool IsDialog => true;

  public virtual string Category => "";

  public virtual FilterTypes Filters => (FilterTypes) 3;

  public virtual PanelUseCases UseCases => (PanelUseCases) 8;

  public virtual PanelTargets TargetHosts => (PanelTargets) 1;

  public virtual int MinProductAccessLevel => 0;

  public virtual int MinDynamicAccessLevel => 0;

  public virtual IEnumerable<Qualifier> RequiredQualifiers
  {
    get
    {
      return (IEnumerable<Qualifier>) new Qualifier[12]
      {
        new Qualifier((QualifierTypes) 8, "ACM21T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration"),
        new Qualifier((QualifierTypes) 8, "ACM21T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ID_Read0139_xcp_ccp_activation_mode_Function_supported_by_calibration"),
        new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ID_Read0139_xcp_ccp_activation_mode_State_of_measurement_CAN_in_ROM_ECU"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start"),
        new Qualifier((QualifierTypes) 2, "ACM21T", "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "RT_SR0506_Deactivation_of_XCP_CCP_communication_Start"),
        new Qualifier((QualifierTypes) 2, "MCM21T", "DJ_Read_AUT64_VeDoc_Input_for_UDS"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU"),
        new Qualifier((QualifierTypes) 8, "ACM301T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration"),
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start"),
        new Qualifier((QualifierTypes) 2, "ACM301T", "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start")
      };
    }
  }

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
      return (IEnumerable<string>) new string[12]
      {
        "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration",
        "DT_STO_ID_Read0139_xcp_ccp_activation_mode_Function_supported_by_calibration",
        "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU",
        "DT_STO_ID_Read0139_xcp_ccp_activation_mode_State_of_measurement_CAN_in_ROM_ECU",
        "DJ_Read_AUT64_VeDoc_Input_In_Application",
        "DJ_Read_AUT64_VeDoc_Input_for_UDS",
        "DT_STO_ID_Read_Curent_ECU_ID_ECU_ID_Current",
        "RT_SR0504_AUT64_Authentication_for_service_routines_Start_aut64_status_byte_2",
        "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start",
        "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start",
        "RT_SR0506_Deactivation_of_XCP_CCP_communication_Start",
        "FN_KeyOffOnReset"
      };
    }
  }
}
