using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_ABA_Misuse_Reset.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 70;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance ABA Misuse Reset.panel";

	public override string Guid => "e89f325e-33f6-4d9c-b9b5-53359137ee24";

	public override string DisplayName => "ABA Misuse Reset";

	public override IEnumerable<string> SupportedDevices => new string[1] { "VRDU01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Detroit Assurance";

	public override FilterTypes Filters => (FilterTypes)65535;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)2, "VRDU01T", "RT_Delete_permanent_errors_Start"),
		new Qualifier((QualifierTypes)2, "VRDU01T", "RT_Delete_permanent_errors_Request_Results_Delete_Results")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)3, (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)0, "VRDU01T", "02FBFF")
	});

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[5]
	{
		new Qualifier((QualifierTypes)2, "VRDU01T", "DJ_SecurityAccess_Routine"),
		new Qualifier((QualifierTypes)2, "VRDU01T", "RT_Delete_permanent_errors_Start"),
		new Qualifier((QualifierTypes)2, "VRDU01T", "RT_Delete_permanent_errors_Request_Results_Delete_Results"),
		new Qualifier((QualifierTypes)64, "VRDU01T", "RT_Delete_permanent_errors_Request_Results_Delete_Results"),
		new Qualifier((QualifierTypes)32, "VRDU01T", "02FBFF")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("VRDU01T", "DJ_SecurityAccess_Routine", (IEnumerable<string>)new string[0]),
		new ServiceCall("VRDU01T", "RT_Delete_permanent_errors_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("VRDU01T", "RT_Delete_permanent_errors_Request_Results_Delete_Results", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "VRDU01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
