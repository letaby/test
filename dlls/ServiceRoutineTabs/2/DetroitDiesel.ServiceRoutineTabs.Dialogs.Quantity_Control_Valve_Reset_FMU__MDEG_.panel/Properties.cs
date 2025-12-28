using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Quantity_Control_Valve_Reset_FMU__MDEG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 19;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Quantity Control Valve Reset FMU (MDEG).panel";

	public override string Guid => "2e0407ca-6cbf-4210-9950-be1966e8fb27";

	public override string DisplayName => "Quantity Control Valve Reset FMU";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[2] { "DDEC16-DD5", "DDEC16-DD8" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR014_SET_EOL_Default_Values_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR014_SET_EOL_Default_Values_Start")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("MCM21T", "RT_SR014_SET_EOL_Default_Values_Start", (IEnumerable<string>)new string[1] { "E2P_Logical_Block_Number=25" })
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
