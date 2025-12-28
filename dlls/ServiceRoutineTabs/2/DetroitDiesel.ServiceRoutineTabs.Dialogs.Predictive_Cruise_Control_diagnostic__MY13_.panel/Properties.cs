using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Predictive_Cruise_Control_diagnostic__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 36;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Predictive Cruise Control diagnostic (MY13).panel";

	public override string Guid => "f5599253-cc55-4ed8-82ca-893b4653c6f0";

	public override string DisplayName => "Predictive Cruise Control Diagnostic";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CPC04T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 2;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)4, "CPC04T", "PCC_Enable")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "J1939-17" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
