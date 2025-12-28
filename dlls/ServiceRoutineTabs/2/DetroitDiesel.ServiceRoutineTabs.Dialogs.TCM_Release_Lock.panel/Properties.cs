using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Release_Lock.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 66;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\TCM Release Lock.panel";

	public override string Guid => "e89f325e-33f6-4d9c-b9b5-53359137ee24";

	public override string DisplayName => "TCM Release Lock";

	public override IEnumerable<string> SupportedDevices => new string[2] { "TCM01T", "TCM05T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)4;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)2, "TCM01T", "DJ_Release_transport_security_for_TCM"),
		new Qualifier((QualifierTypes)2, "TCM05T", "DJ_Release_transport_security_for_TCM")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "TCM01T", "TCM05T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DJ_Release_transport_security_for_TCM", "DJ_SecurityAccess_AntiTheftInit", "DJ_SecurityAccess_RoutineIO_AS" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
