using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Global_Radar_Dyno_Mode.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 57;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance Global Radar Dyno Mode.panel";

	public override string Guid => "465c8707-d011-4bdf-b605-1d2cd9f17c89";

	public override string DisplayName => "Active Brake Assist - Enable/Disable";

	public override IEnumerable<string> SupportedDevices => new string[1] { "VRDU01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Detroit Assurance";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "VRDU01T", "RT_ABA_release_Request_Results_release_state")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)64, "VRDU01T", "RT_ABA_release_Request_Results_release_state"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_GlobalRadarDisable")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "VRDU01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "RT_ABA_release_Request_Results_release_state" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[2] { "SP_GlobalRadarDisable", "SP_GlobalRadarEnable" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
