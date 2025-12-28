using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VRDU_Snapshot.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 196;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\VRDU Snapshot.panel";

	public override string Guid => "1a826fa9-368f-4446-b8b4-929c83309206";

	public override string DisplayName => "VRDU Snapshot";

	public override IEnumerable<string> SupportedDevices => new string[2] { "VRDU01T", "VRDU02T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "VRDU02T", "VRDU01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "DT_STO_ABA_Function_Counter_ABA_Function_Counter", "FN_HardReset" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
