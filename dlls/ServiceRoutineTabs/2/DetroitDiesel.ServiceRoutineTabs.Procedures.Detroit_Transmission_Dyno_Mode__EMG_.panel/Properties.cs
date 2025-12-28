using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 157;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Detroit Transmission Dyno Mode (EMG).panel";

	public override string Guid => "d818f22f-3601-45a5-b3f9-9eb2ee191095";

	public override string DisplayName => "Transmission Dyno Mode";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ECPC01T", "ETCM01T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)4;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[11]
	{
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS126_Actual_E_Motor_Speed_E_Motor_3_Actual_E_Motor_Speed_E_Motor_3"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS124_Actual_E_Motor_Speed_E_Motor_1_Actual_E_Motor_Speed_E_Motor_1"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS052_CalculatedGear_CalculatedGear"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Desired_Gear_current_value"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS063_IgnitionSwitchStatus_IgnitionSwitchStatus"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS134_Current_Torque_Axle_2_Current_Torque_Axle_2"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS133_Current_Torque_Axle_1_Current_Torque_Axle_1"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS004_Kickdown"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Transmission_Oil_Temperature_current_value")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ECPC01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "RT_OTF_DynoMode_Start", "RT_OTF_DynoMode_Stop" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
