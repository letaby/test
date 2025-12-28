using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_3G_Sundown.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 31;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\CTP 3G Sundown.panel";

	public override string Guid => "e904e510-09cf-4849-a44e-c6c455fb4f79";

	public override string DisplayName => "CTP 3G Sundown";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CTP01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Telematics";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)4, "CTP01T", "WPA2KEY")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "CTP01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
