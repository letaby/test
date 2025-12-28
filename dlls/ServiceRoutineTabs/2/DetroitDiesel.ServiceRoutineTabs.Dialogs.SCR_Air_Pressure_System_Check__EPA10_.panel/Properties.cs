using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Air_Pressure_System_Check__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 32;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\SCR Air Pressure System Check (EPA10).panel";

	public override string Guid => "2e2eb1eb-6841-4978-a9de-128b97331afe";

	public override string DisplayName => "SCR Air Pressure System Check";

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

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[7]
	{
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS003_Enable_DEF_pump"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS016_DEF_Air_Pressure"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS014_DEF_Pressure"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_SCR_Pressure_System_Check_Start"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_SCR_Pressure_System_Check_Stop"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[2]
	{
		new ServiceCall("ACM02T", "RT_SCR_Pressure_System_Check_Start", (IEnumerable<string>)new string[1] { "10" }),
		new ServiceCall("ACM02T", "RT_SCR_Pressure_System_Check_Stop", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ACM02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "DT_AS001_Engine_Speed" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
