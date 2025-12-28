using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Gear_Split_Range_Activation.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 219;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Transmission Gear Split Range Activation.panel";

	public override string Guid => "14ead815-96da-4931-885e-48ea5b531c88";

	public override string DisplayName => "Transmission Gear Split Range Activation";

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

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[9]
	{
		new Qualifier((QualifierTypes)256, "Extension", "SP_TCM_Gear_Split_Range_Select_TCM01T"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_TCM_Gear_Split_Range_Select_TCM05T"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2306_Aktuator_Stellung_Split_Aktuator_Stellung_Split"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS012_Vehicle_Speed"),
		new Qualifier((QualifierTypes)8, "TCM01T", "CO_TransType"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_2309_Aktuator_Stellung_Range_Aktuator_Stellung_Range"),
		new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd08_Istgang_Istgang")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "TCM01T", "UDS-03", "TCM05T" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[2] { "SP_TCM_Gear_Split_Range_Select_", "SP_TCM_Gear_Split_Range_Select" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
