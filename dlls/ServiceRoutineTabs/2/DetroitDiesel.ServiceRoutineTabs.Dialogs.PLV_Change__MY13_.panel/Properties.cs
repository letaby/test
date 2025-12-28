using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PLV_Change__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 38;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\PLV Change (MY13).panel";

	public override string Guid => "58b9378c-d941-48c1-9966-c9efc86674b4";

	public override string DisplayName => "Pressure Limiting Valve (PLV) Change";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[23]
	{
		"DDEC13-DD13", "DDEC13-DD15", "DDEC13-DD16", "DDEC13-MDEG 4-Cylinder Tier4", "DDEC13-MDEG 6-Cylinder Tier4", "DDEC13-DD11 Tier4", "DDEC13-DD13 Tier4", "DDEC13-DD16 Tier4", "DDEC16-DD13", "DDEC16-DD15",
		"DDEC16-DD16", "DDEC20-DD13", "DDEC20-DD15", "DDEC20-DD16", "DDEC20-MDEG 4-Cylinder StageV", "DDEC20-MDEG 6-Cylinder StageV", "DDEC20-DD11 StageV", "DDEC20-DD13 StageV", "DDEC20-DD16 StageV", "DDEC16-MDEG 6-Cylinder StageV",
		"DDEC16-MDEG 4-Cylinder StageV", "DDEC16-DD13EURO5", "DDEC16-DD16EURO5"
	};

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_PLV_Open_Counter")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "DT_STO_ACC047_OP_Data_4_PLV_Open_Counter", "RT_SR014_SET_EOL_Default_Values_Start" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
