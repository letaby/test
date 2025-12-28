using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 34;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Fuel System Integrity Check (EPA10).panel";

	public override string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

	public override string DisplayName => "Fuel System Integrity Check";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM02T" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DD13", "DD15", "DD16" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[22]
	{
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR015_Idle_Speed_Modification_Start"),
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR015_Idle_Speed_Modification_Stop"),
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR003_PWM_Routine_by_Function_for_Production_Start_Control_Value"),
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR003_PWM_Routine_by_Function_for_Production_Stop_Function_Name"),
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start"),
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop"),
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR076_P_RAIL_DESIRED_Direct_Input_Start"),
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR076_P_RAIL_DESIRED_Direct_Input_Stop"),
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR018_Disable_HC_Doser_Start"),
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR018_Disable_HC_Doser_Stop"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS098_desired_rail_pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS043_Rail_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS100_Quantity_Control_Valve_Current"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS024_Fuel_Compensation_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_ASL002_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS023_Engine_State"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_ASL005_Fuel_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS124_LPPO_Fuel_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_ASL005_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS121_Quantity_Control_Valve_Desired_Current")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[26]
	{
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS024_Fuel_Compensation_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS124_LPPO_Fuel_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS043_Rail_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS098_desired_rail_pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS001_KW_NW_validity_signal"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS014_Fuel_Temperature"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelSystemIntegrityCheck_Automatic_EPA10"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelSystemIntegrityCheck_LeakTest_EPA10"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelSystemIntegrityCheck_Manual_EPA10"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS024_Fuel_Compensation_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS124_LPPO_Fuel_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS043_Rail_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS098_desired_rail_pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS115_HP_Leak_Actual_Value"),
		new Qualifier((QualifierTypes)4, "MCM02T", "HP_Leak_Counter"),
		new Qualifier((QualifierTypes)4, "MCM02T", "HP_Leak_Learned_Counter"),
		new Qualifier((QualifierTypes)4, "MCM02T", "HP_Leak_Learned_Value"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS001_KW_NW_validity_signal"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "DT_AS115_HP_Leak_Actual_Value" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
