using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 54;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Air Mass Adaptation.panel";

	public override string Guid => "c32d3dc4-29cb-4d5f-8895-877e686cfcce";

	public override string DisplayName => "Air Mass Adaptation";

	public override IEnumerable<string> SupportedDevices => new string[2] { "MCM", "CPC2" };

	public override IEnumerable<string> SupportedEquipment => new string[2] { "MBE4000", "MBE900" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "Engine Configuration";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[10]
	{
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS031_EGR_Commanded_Governor_Value"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS068_Fan_PWM06"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS032_EGR_Actual_Valve_Position"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_DS014_DPF_Regen_Flag"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "oilTemp"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS054_Differential_Pressure_Compressor_In")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "MCM", "CPC2" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[16]
	{
		"DT_DS014_DPF_Regen_Flag", "DT_AS010_Engine_Speed", "DT_AS013_Coolant_Temperature", "DT_AS016_Oil_Temperature", "RT_SR014_SET_EOL_Default_Values_Start", "RT_SR015_Idle_Speed_Modification_Start", "RT_SR015_Idle_Speed_Modification_Stop", "RT_SR002_Engine_Torque_Demand_Substitution_Start_CAN_Torque_Demand", "RT_SR002_Engine_Torque_Demand_Substitution_Stop", "RT_SR005_SW_Routine_Start_SW_Operation",
		"RT_SR005_SW_Routine_Stop", "RT_SR003_PWM_Routine_Start_PWM_Value", "RT_SR003_PWM_Routine_Stop", "RT_SR07E_Automated_Air_Mass_Adaption_Start_status", "RT_SR07E_Automated_Air_Mass_Adaption_Stop", "RT_SR07E_Automated_Air_Mass_Adaption_Request_Results_Results_Status"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
