using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 77;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Real-time Clock (NGC).panel";

	public override string Guid => "696adc9c-bfb2-4caa-862f-e7e4524b8858";

	public override string DisplayName => "Real-time Clock";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ICUC01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ICUC01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
