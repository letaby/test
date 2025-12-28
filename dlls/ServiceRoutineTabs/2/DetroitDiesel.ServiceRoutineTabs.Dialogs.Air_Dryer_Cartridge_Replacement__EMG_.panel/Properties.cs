using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Air_Dryer_Cartridge_Replacement__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 6;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Air Dryer Cartridge Replacement (EMG).panel";

	public override string Guid => "634f9189-9ff2-4381-9688-64fcffa2f7f2";

	public override string DisplayName => "Air Dryer Cartridge Replacement";

	public override IEnumerable<string> SupportedDevices => new string[1] { "EAPU03T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Air Treatment";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "EAPU03T", "RT_Reset_wetness_Start")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("EAPU03T", "RT_Reset_wetness_Start", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
