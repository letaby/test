using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clutch_Apply_Leak_Test.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 23;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\TCM Clutch Apply Leak Test.panel";

	public override string Guid => "64c4cefc-a784-4881-a544-d734205ec720";

	public override string DisplayName => "Clutch Apply Leak Test";

	public override IEnumerable<string> SupportedDevices => new string[3] { "MCM21T", "TCM01T", "TCM05T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)4;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[4]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS023_Engine_State"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[4] { "TCM01T", "MCM21T", "UDS-03", "TCM05T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[5] { "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start", "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status", "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
