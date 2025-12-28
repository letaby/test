using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 84;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\APS Calibration (NGC).panel";

	public override string Guid => "976d3e5e-6714-4734-a5ca-e17ce5c73d53";

	public override string DisplayName => "Active Powersteering Alignment";

	public override IEnumerable<string> SupportedDevices => new string[1] { "APS301T" };

	public override IEnumerable<string> ProhibitedEquipment => new string[3] { "eCascadia-Daycab", "eCascadia-Sleeper", "EMOBILITY-eDrive Powertrain" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[9]
	{
		new Qualifier((QualifierTypes)1, "APS301T", "DT_Endstop_Right_Endstop"),
		new Qualifier((QualifierTypes)1, "APS301T", "DT_Endstop_Left_Endstop"),
		new Qualifier((QualifierTypes)1, "ABS02T", "DT_Steering_wheel_angle_sensor_Read_Steering_wheel_angle"),
		new Qualifier((QualifierTypes)1, "APS301T", "DT_Endstop_Calibration_Status_Left_Calibration_State"),
		new Qualifier((QualifierTypes)1, "APS301T", "DT_Steering_Angle_Calibration_Status_Calibration_Status"),
		new Qualifier((QualifierTypes)1, "APS301T", "DT_Endstop_Calibration_Status_Right_Calibration_State"),
		new Qualifier((QualifierTypes)4, "APS301T", "TorsionBarTorqueOffset"),
		new Qualifier((QualifierTypes)1, "APS301T", "DT_Steering_Angle_Steering_Angle"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_ESC_Diagnostic_Displayables_DDESC_EngineState")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[4] { "APS301T", "ABS02T", "SSAM02T", "VRDU02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[8] { "RT_Calibrate_extern_and_intern_steering_angle_Start_Calibration_state", "RT_Calibrate_Left_Endstop_Start_Endstop_Learnconditions_Endstop_state", "RT_Calibrate_Right_Endstop_Start_Endstop_Learnconditions_Endstop_state", "RT_Calibrate_TorsionBarTorqueOffset_Start", "RT_Calibrate_TorsionBarTorqueOffset_Request_Results_Tbtoffset_calibration_request_status", "FN_HardReset", "DT_Steering_Angle_Steering_Angle", "RT_Discard_calibration_data_Start" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[1] { "SP_TorsionBarTorqueCalibration" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
