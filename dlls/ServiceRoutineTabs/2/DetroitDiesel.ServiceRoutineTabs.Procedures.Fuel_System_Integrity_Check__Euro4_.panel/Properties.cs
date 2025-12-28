using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Fuel_System_Integrity_Check__Euro4_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 317;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Fuel System Integrity Check (Euro4).panel";

	public override string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

	public override string DisplayName => "Fuel System Integrity Check";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM" };

	public override IEnumerable<string> SupportedEquipment => new string[1] { "DD15EURO4" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[24]
	{
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR004_Engine_Cylinder_Cut_Off_Stop"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR015_Idle_Speed_Modification_Start"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR015_Idle_Speed_Modification_Stop"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR003_PWM_Routine_for_Production_Start_PWM_Value"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR003_PWM_Routine_for_Production_Stop"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR005_SW_Routine_for_Production_Start_SW_Operation"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR005_SW_Routine_for_Production_Stop"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR076_Desired_Rail_Pressure_Start_Status"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR076_Desired_Rail_Pressure_Stop"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR018_Disable_HC_Doser_Start"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR018_Disable_HC_Doser_Stop"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS098_desired_rail_pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS043_Rail_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS100_Quantity_Control_Valve_Current"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS024_Fuel_Compensation_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_ASL002_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS023_Engine_State"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_ASL005_Fuel_Temperature")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[11]
	{
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS043_Rail_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS098_desired_rail_pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS100_Quantity_Control_Valve_Current"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS023_Engine_State"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_DS001_KW_NW_validity_signal")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[20]
	{
		"RT_SR015_Idle_Speed_Modification_Start", "RT_SR015_Idle_Speed_Modification_Stop", "RT_SR005_SW_Routine_for_Production_Start_SW_Operation", "RT_SR005_SW_Routine_for_Production_Stop", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop", "RT_SR076_Desired_Rail_Pressure_Start_Status", "RT_SR076_Desired_Rail_Pressure_Stop", "DT_AS087_Actual_Fuel_Mass", "DT_AS098_desired_rail_pressure",
		"DT_AS043_Rail_Pressure", "DT_AS100_Quantity_Control_Valve_Current", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure", "DT_DS001_KW_NW_validity_signal", "DT_ASL002_Engine_Speed", "DT_AS023_Engine_State", "DT_ASL005_Fuel_Temperature", "DT_ASL005_Coolant_Temperature", "RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder", "RT_SR004_Engine_Cylinder_Cut_Off_Stop"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
