using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Coolant_Systems_Pressure_Test__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 27;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Coolant Systems Pressure Test (EMG).panel";

	public override string Guid => "9f9f7a0c-8377-4b46-aebc-ed3012418c9d";

	public override string DisplayName => "Coolant Systems Pressure Test";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ECPC01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[12]
	{
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS243_PwmOutput05ReqDutyCycle_PwmOutput05ReqDutyCycle"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_ETHM_PtcCtrl_Request_Results_OTF_ETHM_Cabin_PTC2_High_Voltage"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS253_EDrvCircOutTemp_EDrvCircOutTemp"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS202_Batt_Circ_Temp_Batt_Circ_Temp"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS082_LIN2_PTC_Cab1_DutyCycle_LIN2_PTC_Cab1_DutyCycle"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS081_LIN1_PTC_Batt2_DutyCycle_LIN1_PTC_Batt2_DutyCycle"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS008_HV_Ready"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS007_AmbientAirTemperature_AmbientAirTemperature"),
		new Qualifier((QualifierTypes)1, "HVAC_F01T", "DT_Blower_Speed_feedback_from_blower_Blower_Speed_feedback_from_blower"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS080_LIN1_PTC_Batt1_DutyCycle_LIN1_PTC_Batt1_DutyCycle")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ECPC01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[6] { "RT_OTF_3by2_wayValvePositionControl_Start_3by2_Valve_ExtensionCkt_Req_1", "RT_OTF_ETHM_BCWaterPumpCtrl_Start", "RT_OTF_ETHM_PtcCtrl_Start", "RT_OTF_ETHM_PtcCtrl_Stop", "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Start_e_drive_circuit_deaeration_start_resp", "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Stop_e_drive_circuit_deaeration_stop_resp" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
