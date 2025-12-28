using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__Euro5_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 109;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Relative Compression Test (Euro5).panel";

	public override string Guid => "cb372ec6-6be5-46a0-a980-0cda9046762e";

	public override string DisplayName => "Relative Compression Test";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MR201T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "MR201T", "DT_AAS_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "CPC04T", "DT_DSL_Parking_Brake")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MR201T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "RT_SR0207_Automatic_Compression_Detection_Start", "RT_SR0207_Automatic_Compression_Detection_Request_Results_State_Byte" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
