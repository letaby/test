using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Replacement__MY16_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 80;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\TCM Replacement (MY16).panel";

	public override string Guid => "e89f325e-33f6-4d9c-b9b5-53359137ee24";

	public override string DisplayName => "TCM Replacement";

	public override IEnumerable<string> SupportedDevices => new string[1] { "TCM01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)4;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "TCM01T", "RT_0530_Kupplungsaktuatortyp_setzen_Service_Start_aktueller_Kupplungsaktuatortyp")
	};

	public override IEnumerable<Qualifier> ProhibitedQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "TCM01T", "RT_0410_Getriebetyp_setzen_und_Wegwerte_loeschen_Service_Start_Getriebetyp")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)8, "TCM01T", "DT_STO_Getriebe_Merkmale_Range_Klaue"),
		new Qualifier((QualifierTypes)8, "TCM01T", "CO_TransType")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "TCM01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[4] { "RT_0530_Kupplungsaktuatortyp_setzen_Service_Start_aktueller_Kupplungsaktuatortyp", "RT_0411_Getriebetyp_und_merkmale_setzen_Getriebe_Wegwerte_loeschen_Service_Start", "RT_0412_Set_transmission_type_and_features_Clear_transmission_learned_values_Service_Start", "DJ_Release_transport_security_for_TCM" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
