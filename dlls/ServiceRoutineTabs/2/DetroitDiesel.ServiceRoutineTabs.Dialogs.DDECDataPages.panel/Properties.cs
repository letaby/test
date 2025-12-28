using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DDECDataPages.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 80;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DDECDataPages.panel";

	public override string Guid => "632fb72f-3b29-47e9-8c41-2d5c0812fdcf";

	public override string DisplayName => "DDEC DataPages";

	public override IEnumerable<string> SupportedDevices => new string[6] { "CPC02T", "CPC04T", "CPC2", "CPC302T", "CPC501T", "CPC502T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Fleet Management";

	public override FilterTypes Filters => (FilterTypes)3;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
