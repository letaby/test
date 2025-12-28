using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 116;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Transmission Clutch Control (MY13).panel";

	public override string Guid => "10cdc8d7-5bc1-4696-8226-741cc9ee85f0";

	public override string DisplayName => "Detroit Transmission Clutch Control";

	public override IEnumerable<string> SupportedDevices => new string[2] { "TCM01T", "TCM05T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)4;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd08_Istgang_Istgang"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2314_Kupplungssollwert_Kupplungssollwert"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd03_Drehzahl_Vorgelegewelle_Drehzahl_Vorgelegewelle"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "TCM01T", "UDS-03", "TCM05T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[7] { "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start", "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop", "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Aktuelle_Position_Kupplung", "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status", "RT_052C_Kupplungsventile_abschalten_Start", "RT_052C_Kupplungsventile_abschalten_Stop", "RT_052C_Kupplungsventile_abschalten_Request_Results_Routine_Status" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
