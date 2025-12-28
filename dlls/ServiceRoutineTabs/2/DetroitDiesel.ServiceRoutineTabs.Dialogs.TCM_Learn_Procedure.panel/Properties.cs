using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Learn_Procedure.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 75;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\TCM Learn Procedure.panel";

	public override string Guid => "a0d55242-5240-4888-910b-fa326bb32484";

	public override string DisplayName => "Transmission Learn Procedure";

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

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)16, "fake", "IgnitionStatusRequest"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_231B_Status_Einlernen_Kupplung_Status_Einlernen_Kupplung"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_TCM_Learn"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2111_Status_Einlernen_Getriebe_stGbLrn"),
		new Qualifier((QualifierTypes)64, "TCM01T", "RT_0400_Einlernvorgang_Service_Request_Results_Fehler_Lernvorgang"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2112_Anforderung_zum_Motorstart_waehrend_des_Einlernvorgangs_Anforderung_Motorstart")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "TCM01T", "UDS-03", "TCM05T" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[1] { "SP_TCM_Learn" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
