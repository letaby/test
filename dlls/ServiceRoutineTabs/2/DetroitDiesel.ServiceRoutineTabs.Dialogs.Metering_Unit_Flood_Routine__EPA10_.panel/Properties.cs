using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Metering_Unit_Flood_Routine__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 17;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Metering Unit Flood Routine (EPA10).panel";

	public override string Guid => "8a4516f0-6cd3-4d0d-9a36-80e1f28c663e";

	public override string DisplayName => "Metering Unit Flood Routine";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ACM02T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[4]
	{
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS016_DEF_Air_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS014_DEF_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DEFDoserPurgeRoutine_EPA10")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
