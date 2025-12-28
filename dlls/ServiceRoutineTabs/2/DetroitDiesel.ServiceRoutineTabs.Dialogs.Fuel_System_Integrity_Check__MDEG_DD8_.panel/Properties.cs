using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__MDEG_DD8_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 157;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Fuel System Integrity Check (MDEG-DD8).panel";

	public override string Guid => "f9d2af05-cbf0-4612-9755-105786bd1b3c";

	public override string DisplayName => "Fuel System Integrity Check";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[1] { "DDEC16-DD8" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[26]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS001_KW_NW_validity_signal"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS043_Rail_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS098_desired_rail_pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS001_KW_NW_validity_signal"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS014_Fuel_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS124_LPPO_Fuel_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS043_Rail_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS098_desired_rail_pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS115_HP_Leak_Actual_Value"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Counter"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelSystemIntegrityCheck_Automatic_MDEG_DD8"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelSystemIntegrityCheck_High_Pressure_Test_MDEG"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelSystemIntegrityCheck_Fuel_Filter_Pressure_Check_MDEG"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DT_AS115_HP_Leak_Actual_Value", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Counter", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
