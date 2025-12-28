using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Coolant_Valve_Control.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 73;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DEF Coolant Valve Control.panel";

	public override string Guid => "020f8264-dbc8-4073-9385-94b286bdc0a5";

	public override string DisplayName => "DEF Coolant Valve Control";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ACM02T" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DD13", "DD15", "DD16" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[5]
	{
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS022_DEF_tank_Temperature"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_DSR_Coolant_Valve_Open_Start"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_DSR_Coolant_Valve_Open_Stop"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS005_Coolant_Valve")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[2]
	{
		new ServiceCall("ACM02T", "RT_DSR_Coolant_Valve_Open_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("ACM02T", "RT_DSR_Coolant_Valve_Open_Stop", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ACM02T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
