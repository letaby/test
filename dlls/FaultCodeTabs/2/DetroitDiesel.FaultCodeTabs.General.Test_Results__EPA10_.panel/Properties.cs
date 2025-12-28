using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.FaultCodeTabs.General.Test_Results__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 94;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Fault Code Tabs\\General\\Test Results (EPA10).panel";

	public override string Guid => "7b563059-cee7-452b-9964-935eae04965a";

	public override string DisplayName => "Test Results";

	public override IEnumerable<string> SupportedDevices => new string[4] { "ACM02T", "ACM21T", "MCM02T", "MCM21T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "OBD";

	public override FilterTypes Filters => (FilterTypes)65535;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
