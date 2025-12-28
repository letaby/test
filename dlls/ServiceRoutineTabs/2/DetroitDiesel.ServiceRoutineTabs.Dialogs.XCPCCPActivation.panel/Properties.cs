using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 36;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\XCPCCPActivation.panel";

	public override string Guid => "bc630f2c-a004-41b1-ac34-69069c227188";

	public override string DisplayName => "XCP/CCP Activation";

	public override IEnumerable<string> SupportedDevices => new string[3] { "ACM21T", "ACM301T", "MCM21T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)3;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[12]
	{
		new Qualifier((QualifierTypes)8, "ACM21T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration"),
		new Qualifier((QualifierTypes)8, "ACM21T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ID_Read0139_xcp_ccp_activation_mode_Function_supported_by_calibration"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ID_Read0139_xcp_ccp_activation_mode_State_of_measurement_CAN_in_ROM_ECU"),
		new Qualifier((QualifierTypes)2, "ACM21T", "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start"),
		new Qualifier((QualifierTypes)2, "ACM21T", "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0506_Deactivation_of_XCP_CCP_communication_Start"),
		new Qualifier((QualifierTypes)2, "MCM21T", "DJ_Read_AUT64_VeDoc_Input_for_UDS"),
		new Qualifier((QualifierTypes)8, "ACM301T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU"),
		new Qualifier((QualifierTypes)8, "ACM301T", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration"),
		new Qualifier((QualifierTypes)2, "ACM301T", "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start"),
		new Qualifier((QualifierTypes)2, "ACM301T", "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "ACM21T", "ACM301T", "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[12]
	{
		"DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_Function_supported_by_calibration", "DT_STO_ID_Read0139_xcp_ccp_activation_mode_Function_supported_by_calibration", "DT_STO_ID_Read0139_Read_XCP_CCP_activation_mode_State_of_measurement_CAN_in_ROM_ECU", "DT_STO_ID_Read0139_xcp_ccp_activation_mode_State_of_measurement_CAN_in_ROM_ECU", "DJ_Read_AUT64_VeDoc_Input_In_Application", "DJ_Read_AUT64_VeDoc_Input_for_UDS", "DT_STO_ID_Read_Curent_ECU_ID_ECU_ID_Current", "RT_SR0504_AUT64_Authentication_for_service_routines_Start_aut64_status_byte_2", "RT_SR0505_ROM_ECU_XCP_CCP_activation_Start", "RT_SR0506_Deactivation_of_XCP_CCP_communication_without_AUT_64_Start",
		"RT_SR0506_Deactivation_of_XCP_CCP_communication_Start", "FN_KeyOffOnReset"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
