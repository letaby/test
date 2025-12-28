using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.FaultCodeTabs.General.Virtual_Technician_Data.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 136;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Fault Code Tabs\\General\\Virtual Technician Data.panel";

	public override string Guid => "1b9bb3c8-4405-4267-b37c-bfb7d661b8f8";

	public override string DisplayName => "Virtual Technician Data";

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
