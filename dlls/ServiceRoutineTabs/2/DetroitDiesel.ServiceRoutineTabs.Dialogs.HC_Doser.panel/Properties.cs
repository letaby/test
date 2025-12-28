using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HC_Doser.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 55;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\HC Doser.panel";

	public override string Guid => "07370e77-98a0-428c-9460-3994036f6d20";

	public override string DisplayName => "HC Doser";

	public override IEnumerable<string> SupportedDevices => new string[2] { "CPC2", "MCM" };

	public override IEnumerable<string> SupportedEquipment => new string[6] { "DD13", "DD15", "DD16", "MBE4000", "MBE900", "S60" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)2;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[45]
	{
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS038_Doser_Fuel_Line_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS040_DOC_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS041_DOC_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS053_DPF_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS017_Inlet_Manifold_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS037_DPF_Outlet_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS036_DPF_Inlet_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS034_Throttle_Valve_Actual_Position"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS033_Throttle_Valve_Commanded_Value"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS018_Inlet_Manifold_Pressure"),
		new Qualifier((QualifierTypes)1, "CPC2", "DT_AS005_Accelerator_Pedal_Position"),
		new Qualifier((QualifierTypes)0, "CPC2", "DT_DS001_Clutch_Open"),
		new Qualifier((QualifierTypes)0, "CPC2", "DT_DS001_Parking_Brake"),
		new Qualifier((QualifierTypes)0, "CPC2", "DT_DS006_Neutral_Switch"),
		new Qualifier((QualifierTypes)0, "CPC2", "DT_DS008_DPF_Regen_Switch_Status"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_AS072_DPF_Zone"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_DS014_DPF_Regen_Flag"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_DS014_DPF_CAN_manual_regen"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_DS014_DPF_CAN_high_idle_regen"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_AS073_Regeneration_Time"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_AS074_DPF_Target_Temperature"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_AS075_DOC_Out_Model_No_Delay"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_AS076_DOC_Out_Model_Delay"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_AS055_Temperature_Compressor_In"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_AS058_Temperature_Compressor_Out"),
		new Qualifier((QualifierTypes)0, "MCM", "DT_AS056_Pressure_Compressor_Out"),
		new Qualifier((QualifierTypes)0, "CPC2", "DT_DS003_Engine_Brake_Disable"),
		new Qualifier((QualifierTypes)0, "CPC2", "DT_DS003_Engine_Brake_Low"),
		new Qualifier((QualifierTypes)0, "CPC2", "DT_DS003_Engine_Brake_Medium"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS012_Vehicle_Speed"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS022_Active_Governor_Type"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS023_Engine_State"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS019_Barometric_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS071_Smoke_Control_Status"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS077_Fuel_Cut_Off_Valve"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS035_Fuel_Doser_Injection_Status"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS024_Fuel_Compensation_Pressure"),
		new Qualifier((QualifierTypes)16, "fake", "FakeBoostPressure"),
		new Qualifier((QualifierTypes)1, "virtual", "engineload"),
		new Qualifier((QualifierTypes)16, "fake", "FakeFuelCompensationGaugePressure"),
		new Qualifier((QualifierTypes)16, "fake", "FakeDoserFuelLineGaugePressure"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_HCDoserPurge_EPA07")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "DT_AS077_Fuel_Cut_Off_Valve" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
