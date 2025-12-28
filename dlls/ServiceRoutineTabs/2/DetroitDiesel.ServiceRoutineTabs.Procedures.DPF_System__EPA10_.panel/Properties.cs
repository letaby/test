using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DPF_System__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 71;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\DPF System (EPA10).panel";

	public override string Guid => "07370e77-98a0-428c-9460-3994036f6d20";

	public override string DisplayName => "DPF System";

	public override IEnumerable<string> SupportedDevices => new string[3] { "ACM02T", "CPC02T", "MCM02T" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DDEC10-DD13", "DDEC10-DD15", "DDEC10-DD16" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[43]
	{
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS022_Active_Governor_Type"),
		new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS019_Barometric_Pressure"),
		new Qualifier((QualifierTypes)1, "virtual", "airInletPressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS071_Smoke_Control_Status"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS033_Throttle_Valve_Commanded_Value"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS005_DOC_Inlet_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS077_Fuel_Cut_Off_Valve"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS035_Fuel_Doser_Injection_Status"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelPressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS038_Doser_Fuel_Line_Pressure"),
		new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS001_Clutch_Open"),
		new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS001_Parking_Brake"),
		new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS006_Neutral_Switch"),
		new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS008_DPF_Regen_Switch_Status"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS111_DOC_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS112_DOC_Out_Model_Delay_Non_fueling"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS113_DPF_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS064_DPF_Regen_State"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS065_Actual_DPF_zone"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS120_DPF_Target_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS055_Temperature_Compressor_In"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS058_Temperature_Compressor_Out"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS056_Pressure_Compressor_Out"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS069_Jake_Brake_1_PWM07"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS009_DPF_Oultlet_Temperature"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "airInletTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "engineload"),
		new Qualifier((QualifierTypes)16, "fake", "FakeFuelCompensationGaugePressureEPA10"),
		new Qualifier((QualifierTypes)16, "fake", "FakeDoserFuelLineGaugePressureEPA10"),
		new Qualifier((QualifierTypes)16, "fake", "FakeBoostPressureEPA10"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_HCDoserPurge_EPA10"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_OverTheRoadRegen_EPA10"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ParkedRegen_EPA10"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DisableHcDoserParkedRegen_EPA10")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "DT_AS077_Fuel_Cut_Off_Valve" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
