using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Camera_Height.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 15;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance Camera Height.panel";

	public override string Guid => "abc8f5f8-a5a9-4b94-be78-f360a1f95fa5";

	public override string DisplayName => "Camera Height Adjustment";

	public override IEnumerable<string> SupportedDevices => new string[1] { "VRDU01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Detroit Assurance";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)4, "VRDU01T", "camera_height")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)3, (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)0, "VRDU01T", "00FBED")
	});

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)32, "VRDU01T", "00FBED"),
		new Qualifier((QualifierTypes)4, "VRDU01T", "camera_height")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "VRDU01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
