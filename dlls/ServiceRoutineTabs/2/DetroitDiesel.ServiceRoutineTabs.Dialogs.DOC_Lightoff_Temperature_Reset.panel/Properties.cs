using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Lightoff_Temperature_Reset.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 27;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DOC Lightoff Temperature Reset.panel";

	public override string Guid => "a79bea4d-c5a5-4237-b18c-2c749f1f3406";

	public override string DisplayName => "DOC Lightoff Temperature Reset";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ACM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[5] { "DDEC16-DD5", "DDEC16-DD8", "DDEC16-DD16", "DDEC16-DD13", "DDEC16-DD15" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)3;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "ACM21T", "RT_SR0D4_Reset_Lightoff_Enhancer_Temp_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)2, "ACM21T", "RT_SR0D4_Reset_Lightoff_Enhancer_Temp_Start"),
		new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS001_Engine_Speed")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("ACM21T", "RT_SR0D4_Reset_Lightoff_Enhancer_Temp_Start", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
