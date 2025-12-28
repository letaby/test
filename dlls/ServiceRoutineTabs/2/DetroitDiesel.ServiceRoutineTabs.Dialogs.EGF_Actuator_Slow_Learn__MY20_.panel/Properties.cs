using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGF_Actuator_Slow_Learn__MY20_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 44;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGF Actuator Slow Learn (MY20).panel";

	public override string Guid => "1550b8c6-4855-4e57-bcad-7e92a09af8fc";

	public override string DisplayName => "Exhaust Gas Flap Actuator Slow Learn";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[1] { "DDEC20-DD13" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS050_SRA3_Status_Code"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_EGFActuatorSlowLearn_MY20")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
