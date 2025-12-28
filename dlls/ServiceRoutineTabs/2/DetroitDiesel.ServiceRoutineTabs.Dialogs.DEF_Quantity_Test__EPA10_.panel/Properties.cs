using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 14;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DEF Quantity Test (EPA10).panel";

	public override string Guid => "a4336286-5029-47aa-a1d4-2a4a49d7a1fa";

	public override string DisplayName => "DEF Quantity Test";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ACM02T", "CPC02T" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DD13", "DD15", "DD16" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS003_Enable_DEF_pump"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS014_DEF_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS016_DEF_Air_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS085_Actual_DEF_Dosing_Quantity"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS086_Requested_DEF_Dosing_Quantity"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DEFQuantityTest_EPA10")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
