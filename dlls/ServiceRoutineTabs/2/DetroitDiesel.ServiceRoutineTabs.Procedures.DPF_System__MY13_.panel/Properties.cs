using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 127;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\DPF System (MY13).panel";

	public override string Guid => "07370e77-98a0-428c-9460-3994036f6d20";

	public override string DisplayName => "DPF System";

	public override IEnumerable<string> SupportedDevices => new string[5] { "ACM21T", "ACM301T", "ACM311T", "MCM21T", "MCM30T" };

	public override IEnumerable<string> ProhibitedEquipment => new string[2] { "DDEC16-DD13EURO5", "DDEC16-DD16EURO5" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[65]
	{
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS022_Active_Governor_Type"),
		new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS019_Barometric_Pressure"),
		new Qualifier((QualifierTypes)1, "virtual", "airInletPressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS071_Smoke_Control_Status"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS033_Throttle_Valve_Commanded_Value"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS005_DOC_Inlet_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS006_DPF_Outlet_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS077_Fuel_Cut_Off_Valve"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS035_Fuel_Doser_Injection_Status"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelPressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS038_Doser_Fuel_Line_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "virtual", "NeutralSwitch"),
		new Qualifier((QualifierTypes)1, "virtual", "ClutchSwitch"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_MSC_GetSwState_033"),
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_DPF_Regen_Switch_Status"),
		new Qualifier((QualifierTypes)1, "MCM30T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS122_DOC_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS125_DPF_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS064_DPF_Regen_State"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS065_Actual_DPF_zone"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS120_DPF_Target_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS032_EGR_Actual_Valve_Position"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS027_Turbo_Speed_1"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS090_Wastegate_return_position"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS122_DOC_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS122_DOC_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS124_DOC_Out_Model_Delay_Non_fueling"),
		new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS125_DPF_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS125_DPF_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS064_DPF_Regen_State"),
		new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS064_DPF_Regen_State"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS065_Actual_DPF_zone"),
		new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS065_Actual_DPF_zone"),
		new Qualifier((QualifierTypes)1, "ACM311T", "DT_AS120_DPF_Target_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS120_DPF_Target_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS032_EGR_Actual_Valve_Position"),
		new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS027_Turbo_Speed_1"),
		new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS090_Wastegate_return_position"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS055_Temperature_Compressor_In"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS058_Temperature_Compressor_Out"),
		new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS055_Temperature_Compressor_In"),
		new Qualifier((QualifierTypes)1, "MCM30T", "DT_ASL005_Temperature_Compressor_Out"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS069_Jake_Brake_1_PWM13"),
		new Qualifier((QualifierTypes)1, "MCM30T", "DT_AS069_Jake_Brake_1_PWM13"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS007_DOC_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS008_DOC_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS009_DPF_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "airInletTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "engineload"),
		new Qualifier((QualifierTypes)16, "fake", "FakeFuelCompensationGaugePressureMY13"),
		new Qualifier((QualifierTypes)16, "fake", "FakeDoserFuelLineGaugePressureMY13"),
		new Qualifier((QualifierTypes)16, "fake", "FakeBoostPressureMY13"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_HCDoserPurge_MY13"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_OverTheRoadRegen_MY13"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ParkedRegen_MY13"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DisableHcDoserParkedRegen_MY13")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[8] { "CPC302T", "MCM21T", "ACM21T", "MCM", "MCM30T", "J1939-0", "CPC501T", "CPC502T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "DT_AS077_Fuel_Cut_Off_Valve", "DT_105" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[14]
	{
		"SP_HCDoserPurge_MY13", "SP_HCDoserPurge_MY25", "SP_OverTheRoadRegen_NGC", "SP_ParkedRegen_NGC", "SP_DisableHcDoserParkedRegen_NGC", "SP_OverTheRoadRegen_CPC5", "SP_ParkedRegen_CPC5", "SP_DisableHcDoserParkedRegen_CPC5", "SP_OverTheRoadRegen_MY25", "SP_ParkedRegen_MY25",
		"SP_DisableHcDoserParkedRegen_MY25", "SP_OverTheRoadRegen_MY13", "SP_ParkedRegen_MY13", "SP_DisableHcDoserParkedRegen_MY13"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
