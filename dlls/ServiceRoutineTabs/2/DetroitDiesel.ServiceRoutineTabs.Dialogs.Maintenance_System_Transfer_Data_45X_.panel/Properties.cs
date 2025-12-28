using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data_45X_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 22;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Maintenance System Transfer Data(45X).panel";

	public override string Guid => "c887b6fe-150e-4c81-9848-70a85a1781a0";

	public override string DisplayName => "Maintenance System Transfer Data";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CGW05T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)14;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "CGW05T", "RT_Transfer_data_from_the_mirror_memory_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "CGW05T", "RT_Transfer_data_from_the_mirror_memory_Start")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("CGW05T", "RT_Transfer_data_from_the_mirror_memory_Start", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
