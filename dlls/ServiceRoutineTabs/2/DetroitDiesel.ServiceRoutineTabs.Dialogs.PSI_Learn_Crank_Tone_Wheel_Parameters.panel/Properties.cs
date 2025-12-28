using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 133;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\PSI Learn Crank Tone Wheel Parameters.panel";

	public override string Guid => "6f54fa38-cea3-4cc8-ac2b-23261ac7f40d";

	public override string DisplayName => "PSI Learn Crank Tone Wheel Parameters";

	public override IEnumerable<string> SupportedEquipment => new string[2] { "FCCC-Petrol", "FCCC-LPG" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[6]
	{
		new Qualifier((QualifierTypes)1, "MT88ECU", "DT_110"),
		new Qualifier((QualifierTypes)1, "MT88ECU", "DT_190"),
		new Qualifier((QualifierTypes)1, "MT88ECU", "DT_91"),
		new Qualifier((QualifierTypes)1, "MT88ECU", "DT_168"),
		new Qualifier((QualifierTypes)1, "MT88ECU", "DT_70"),
		new Qualifier((QualifierTypes)1, "J1939-3", "DT_523")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "MT88ECU", "UDS-0" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
