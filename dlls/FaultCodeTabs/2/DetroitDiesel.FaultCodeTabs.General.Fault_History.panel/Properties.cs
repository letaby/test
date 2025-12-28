using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.FaultCodeTabs.General.Fault_History.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 156;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Fault Code Tabs\\General\\Fault History.panel";

	public override string Guid => "768c6072-83fe-4ffa-b86b-c8b805edda75";

	public override string DisplayName => "Fault History";

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
