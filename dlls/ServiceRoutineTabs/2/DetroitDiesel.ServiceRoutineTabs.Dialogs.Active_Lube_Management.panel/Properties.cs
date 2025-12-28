using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Active_Lube_Management.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 115;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Active Lube Management.panel";

	public override string Guid => "edd97055-81df-46ec-a8ea-fe34a2e959c7";

	public override string DisplayName => "Active Lube Management";

	public override IEnumerable<string> SupportedDevices => new string[1] { "SSAM02T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> ForceQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_ACS_Diagnostic_Displayables_DDACS_ActvLubMgntValve"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_ACS_Diagnostic_Displayables_DDACS_ALMFunction_Stat")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_ACS_Diagnostic_Displayables_DDACS_ALMFunction_Stat"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_ACS_Diagnostic_Displayables_DDACS_ActvLubMgntValve")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "SSAM02T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
