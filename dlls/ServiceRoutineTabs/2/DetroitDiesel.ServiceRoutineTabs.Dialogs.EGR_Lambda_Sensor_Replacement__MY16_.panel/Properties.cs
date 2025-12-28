using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Lambda_Sensor_Replacement__MY16_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 9;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGR Lambda Sensor Replacement (MY16).panel";

	public override string Guid => "a79bea4d-c5a5-4237-b18c-2c749f1f3406";

	public override string DisplayName => "EGR Lambda Sensor Replacement";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[2] { "DDEC16-DD5", "DDEC16-DD8" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "EGR";

	public override FilterTypes Filters => (FilterTypes)3;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR081_Reset_Lambda_sensor_Start_Status")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("MCM21T", "RT_SR081_Reset_Lambda_sensor_Start_Status", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
