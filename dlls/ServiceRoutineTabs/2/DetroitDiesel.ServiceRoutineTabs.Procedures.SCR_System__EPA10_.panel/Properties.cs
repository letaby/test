using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 110;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\SCR System (EPA10).panel";

	public override string Guid => "07370e77-98a0-428c-9460-3994036f6d20";

	public override string DisplayName => "SCR System";

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

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[41]
	{
		new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS001_Clutch_Open"),
		new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS001_Parking_Brake"),
		new Qualifier((QualifierTypes)1, "CPC02T", "DT_DS006_Neutral_Switch"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS004_Line_Heater_1"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS004_Line_Heater_2"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS004_Line_Heater_3"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS004_Line_Heater_4"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS005_Coolant_Valve"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS008_Diffuser_Heater"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS117_SCR_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS118_SCR_Heat_Generation"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS024_DEF_Tank_Level"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS065_Actual_DPF_zone"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS014_DEF_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS016_DEF_Air_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS009_DPF_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS018_SCR_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS019_SCR_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS021_DEF_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS022_DEF_tank_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position"),
		new Qualifier((QualifierTypes)1, "virtual", "airInletPressure"),
		new Qualifier((QualifierTypes)1, "virtual", "engineload"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS086_Requested_DEF_Dosing_Quantity"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS085_Actual_DEF_Dosing_Quantity"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS036_SCR_Inlet_NOx_Sensor"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ChassisDynoBasicScrConversionCheck_EPA10"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_OutputComponentTest_EPA10"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ParkedScrEfficiencyTest_EPA10"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS035_SCR_Outlet_NOx_Sensor"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS101_Nox_conversion_efficiency"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS053_Ambient_Air_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS019_Barometric_Pressure")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
