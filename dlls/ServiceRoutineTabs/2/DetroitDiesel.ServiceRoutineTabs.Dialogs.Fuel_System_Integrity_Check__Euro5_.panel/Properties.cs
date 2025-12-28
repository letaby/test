using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__Euro5_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 56;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Fuel System Integrity Check (Euro5).panel";

	public override string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

	public override string DisplayName => "Fuel System Integrity Check";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[2] { "DDEC16-DD16EURO5", "DDEC16-DD13EURO5" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR003_PWM_Routine_by_Function_for_Production_Start_Control_Value"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR003_PWM_Routine_by_Function_for_Production_Stop_Function_Name"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR076_P_RAIL_DESIRED_Direct_Input_Start"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR076_P_RAIL_DESIRED_Direct_Input_Stop"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR02FA_set_TM_mode_Start"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR02FA_set_TM_mode_Stop")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[24]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS043_Rail_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS098_desired_rail_pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS001_KW_NW_validity_signal"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS014_Fuel_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AAS_LPPO_Fuel_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AAS_LPPO_Fuel_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS043_Rail_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS098_desired_rail_pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS115_HP_Leak_Actual_Value"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Counter"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS001_KW_NW_validity_signal"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelSystemIntegrityCheck_Automatic_Euro5"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelSystemIntegrityCheck_LeakTest_Euro5"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelSystemIntegrityCheck_Manual_Euro5")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DT_AS115_HP_Leak_Actual_Value", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
