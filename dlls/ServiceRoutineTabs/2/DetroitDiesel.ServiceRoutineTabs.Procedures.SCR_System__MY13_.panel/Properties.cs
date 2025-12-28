using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.SCR_System__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 150;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\SCR System (MY13).panel";

	public override string Guid => "07370e77-98a0-428c-9460-3994036f6d20";

	public override string DisplayName => "SCR System";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ACM21T", "MCM21T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[46]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS019_Barometric_Pressure"),
		new Qualifier((QualifierTypes)1, "virtual", "airInletPressure"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Clutch_Open"),
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Parking_Brake"),
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Neutral_Switch"),
		new Qualifier((QualifierTypes)1, "CPC302T", "DT_DS255_Blocktransfer_ClutchOpen"),
		new Qualifier((QualifierTypes)1, "CPC302T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
		new Qualifier((QualifierTypes)1, "CPC302T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_ClutchStatus"),
		new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_DrivingModeNeutralRequest"),
		new Qualifier((QualifierTypes)1, "CPC501T", "DT_DS255_Blocktransfer_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS004_Line_Heater_1"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS004_Line_Heater_2"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS004_Line_Heater_3"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS004_Line_Heater_4"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS012_Diffuser_Heater"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_DS011_heater_state_dosing_unit"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS128_SCR_Out_Model_Delay"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS129_SCR_Heat_Generation"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS024_DEF_Tank_Level"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS065_Actual_DPF_zone"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ChassisDynoBasicScrConversionCheck_MY13"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_OutputComponentTest_MY13"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ParkedScrEfficiencyTest_MY13"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS009_DPF_Oultlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS008_DOC_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS007_DOC_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS080_ADS_DEF_Quantity_Request"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS110_ADS_DEF_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS022_DEF_tank_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS019_SCR_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS018_SCR_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "virtual", "engineload"),
		new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS160_Real_Time_ADS_DEF_Dosed_Quantity_g_hr"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS143_ADS_Pump_Speed"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS101_Nox_conversion_efficiency"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS053_Ambient_Air_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS036_SCR_Inlet_NOx_Sensor"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS035_SCR_Outlet_NOx_Sensor")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ACM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "DT_AS014_DEF_Pressure", "DT_AS110_ADS_DEF_Pressure_2" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
