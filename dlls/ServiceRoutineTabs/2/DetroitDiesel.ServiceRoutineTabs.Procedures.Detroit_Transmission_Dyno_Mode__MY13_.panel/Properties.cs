using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 59;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Detroit Transmission Dyno Mode (MY13).panel";

	public override string Guid => "d818f22f-3601-45a5-b3f9-9eb2ee191095";

	public override string DisplayName => "Transmission Dyno Mode";

	public override IEnumerable<string> SupportedDevices => new string[3] { "CPC04T", "TCM01T", "TCM05T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)4;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[9]
	{
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "accelPedalPosition"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd07_Sollgang_Sollgang"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd08_Istgang_Istgang"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS001_Requested_Torque"),
		new Qualifier((QualifierTypes)1, "virtual", "engineTorque"),
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Accelerator_Pedal_Kick_Down_Status"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd16_Getriebe_Oelltemperatur_Getriebe_Oelltemperatur")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[4] { "CPC04T", "TCM01T", "UDS-03", "TCM05T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DT_ASL_Actual_Engine_Speed", "RT_Dynamometer_test_mode_status_Start", "RT_Dynamometer_test_mode_status_Stop" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
