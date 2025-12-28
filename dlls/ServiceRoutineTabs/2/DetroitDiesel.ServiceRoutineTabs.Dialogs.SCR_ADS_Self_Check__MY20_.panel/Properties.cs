using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_ADS_Self_Check__MY20_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 60;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\SCR ADS Self Check (MY20).panel";

	public override string Guid => "8bbc8347-b40d-456d-abcc-328b5ff67d53";

	public override string DisplayName => "SCR ADS Self-check";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ACM301T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[3]
	{
		new Qualifier((QualifierTypes)2, "ACM301T", "RT_SCR_ADS_SelfCheck_Routine_Request_Results_status_of_service_function"),
		new Qualifier((QualifierTypes)2, "ACM301T", "RT_SCR_ADS_SelfCheck_Routine_Start"),
		new Qualifier((QualifierTypes)2, "ACM301T", "RT_SCR_ADS_SelfCheck_Routine_Stop")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)256, "Extension", "SP_SCR_ADS_Self_Check_MY20"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS079_ADS_priming_request"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_DS011_ADS_dosing_valve_state"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS143_ADS_Pump_Speed"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS001_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS077_Vehicle_speed_from_ISP100ms"),
		new Qualifier((QualifierTypes)64, "ACM301T", "RT_SCR_ADS_SelfCheck_Routine_Request_Results_status_of_service_function"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS014_DEF_Pressure")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ACM301T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "DT_AS014_DEF_Pressure" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
