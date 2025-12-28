using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Actuator_Functional_Check.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 20;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGR Valve Actuator Functional Check.panel";

	public override string Guid => "8bbc8347-b40d-456d-abcc-328b5ff67d53";

	public override string DisplayName => "EGR Valve Actuator Functional Check";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "EGR";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[4]
	{
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR0B2_EGR_IAE_Test_Stop"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR0B2_EGR_IAE_Test_Start"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR0B2_EGR_IAE_Test_Request_Results_Status"),
		new Qualifier((QualifierTypes)2, "MCM", "RT_SR0B2_EGR_IAE_Test_Request_Results_EGR_Value_Position")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[10]
	{
		new Qualifier((QualifierTypes)256, "Extension", "SP_EGR_IAE_EPA07"),
		new Qualifier((QualifierTypes)32, "MCM", "C80600"),
		new Qualifier((QualifierTypes)32, "MCM", "C80700"),
		new Qualifier((QualifierTypes)32, "MCM", "9A0F00"),
		new Qualifier((QualifierTypes)32, "MCM", "9A1000"),
		new Qualifier((QualifierTypes)32, "MCM", "C80900"),
		new Qualifier((QualifierTypes)32, "MCM", "4E0800"),
		new Qualifier((QualifierTypes)32, "MCM", "4E0500"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS023_Engine_State"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS013_Coolant_Temperature")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
