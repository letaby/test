using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Check_VIN_Synchronization.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 75;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Check VIN Synchronization.panel";

	public override string Guid => "737a8bd6-3679-4480-9ea2-05fd17a81e2b";

	public override string DisplayName => "Check VIN Synchronization";

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 1;

	public override int MinDynamicAccessLevel => 1;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
