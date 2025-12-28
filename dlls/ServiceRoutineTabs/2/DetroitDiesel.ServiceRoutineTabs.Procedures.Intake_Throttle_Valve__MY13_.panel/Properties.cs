using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Intake_Throttle_Valve__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 18;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Intake Throttle Valve (MY13).panel";

	public override string Guid => "67a6ba9d-5527-4e3f-ad52-b8524e401379";

	public override string DisplayName => "Intake Throttle Valve";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[4] { "DDEC13-DD13", "DDEC13-DD15", "DDEC13-DD16", "DDEC16-DD16" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "RT_SR068_Control_IAT_Start_status", "RT_SR068_Control_IAT_Stop", "DT_AS010_Engine_Speed" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
