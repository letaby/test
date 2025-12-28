using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ESC_Learn_Procedure.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 105;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ESC Learn Procedure.panel";

	public override string Guid => "1550b8c6-4855-4e57-bcad-7e92a09af8fc";

	public override string DisplayName => "ESC Learning Procedure";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ABS02T", "SBSP01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Anti-lock Braking System";

	public override FilterTypes Filters => (FilterTypes)8;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[9]
	{
		new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Yaw_rate_plausibility"),
		new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Offset_lateral_acceleration_learned"),
		new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_1_Read_Offset_steering_wheel_angle_learned"),
		new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_1_Read_Lateral_acceleration_plausibility"),
		new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Service_mode_active"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Steering_wheel_angle_plausibility"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_Start_ESC_learning_Start_Routine_Start"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_Start_ESC_learning_Start_Routine_Start")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[2]
	{
		new ServiceCall("ABS02T", "RT_Start_ESC_learning_Start_Routine_Start", (IEnumerable<string>)new string[1] { "Learning_mode=2" }),
		new ServiceCall("ABS02T", "RT_Start_ESC_learning_Start_Routine_Start", (IEnumerable<string>)new string[1] { "Learning_mode=3" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ABS02T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
