using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Periodic_CARB_Smoke_Inspection_Program_OBD_Data_Report.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 652;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Periodic CARB Smoke Inspection Program OBD Data Report.panel";

	public override string Guid => "d8ea1042-432a-49bc-83db-888f9ccb4dce";

	public override string DisplayName => "Periodic CARB Smoke Inspection Program OBD Data Report";

	public override IEnumerable<string> SupportedDevices => new string[3] { "J1939-0", "J1939-1", "J1939-61" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "OBD";

	public override FilterTypes Filters => (FilterTypes)16;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[4] { "J1939-1", "J1939-61", "J1939-0", "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[26]
	{
		"DT_1221_4_7", "DT_1221_4_3", "DT_1223_7_8", "DT_1222_5_8", "DT_1223_7_6", "DT_1222_5_6", "DT_1223_7_7", "DT_1222_5_7", "DT_1221_4_6", "DT_1221_4_2",
		"DT_1221_4_5", "DT_1221_4_1", "DT_1223_8_5", "DT_1222_6_5", "DT_1223_8_4", "DT_1222_6_4", "DT_1223_8_3", "DT_1222_6_3", "DT_1223_8_2", "DT_1222_6_2",
		"DT_3069", "DT_3294", "DT_3295", "DT_3296", "DT_1213", "DT_3302"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
