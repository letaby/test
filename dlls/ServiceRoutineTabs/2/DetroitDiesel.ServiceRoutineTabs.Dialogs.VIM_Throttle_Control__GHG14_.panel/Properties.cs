using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VIM_Throttle_Control__GHG14_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 146;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\VIM Throttle Control (GHG14).panel";

	public override string Guid => "2b4946a0-299d-4b4d-b754-d0cdbc510356";

	public override string DisplayName => "VIM Throttle Control";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CPC04T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)4, "CPC04T", "Accel_Pedal_Type")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[3]
	{
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_ASL_Accelerator_Pedal_Position"),
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_ASL_Actual_Engine_Speed"),
		new Qualifier((QualifierTypes)4, "CPC04T", "Accel_Pedal_Type")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "CPC04T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
