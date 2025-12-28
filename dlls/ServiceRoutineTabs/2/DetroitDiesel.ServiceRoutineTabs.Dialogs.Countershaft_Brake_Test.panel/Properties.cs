using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Countershaft_Brake_Test.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 96;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Countershaft Brake Test.panel";

	public override string Guid => "5e980fb4-a3da-48b0-951b-dd2f266dd3aa";

	public override string DisplayName => "Countershaft Brake Test";

	public override IEnumerable<string> SupportedDevices => new string[3] { "MCM21T", "TCM01T", "TCM05T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)4;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[10]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_CountershaftBrakeTest_TCM01T"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_CountershaftBrakeTest_TCM05T"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd11_Prozentualer_Wegwert_Kupplung_Prozentualer_Wegwert_Kupplung"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd08_Istgang_Istgang"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_231A_Maximaler_Gradient_Vorgelegewellen_Drehzahl_max_Gradient_Vorgelegewellen_Drehzahl"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS023_Engine_State"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd03_Drehzahl_Vorgelegewelle_Drehzahl_Vorgelegewelle")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "TCM01T", "UDS-03", "TCM05T" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[1] { "SP_CountershaftBrakeTest_" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
