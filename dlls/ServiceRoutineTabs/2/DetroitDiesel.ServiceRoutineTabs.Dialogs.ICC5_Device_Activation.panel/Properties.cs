using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICC5_Device_Activation.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 6;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ICC5 Device Activation.panel";

	public override string Guid => "b94cef26-d0a3-4c73-8113-a230efa8f7c4";

	public override string DisplayName => "ICC5 Device Activation";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ICC501T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)256, "Extension", "SP_ICC5_DeviceActivation")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
