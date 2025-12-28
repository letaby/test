using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_In_Fuel__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 225;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\FIS Water In Fuel (MY13).panel";

	public override string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

	public override string DisplayName => "FIS Water in Fuel";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[21]
	{
		"DDEC13-DD13", "DDEC13-DD15", "DDEC13-DD16", "DDEC13-MDEG 4-Cylinder Tier4", "DDEC13-MDEG 6-Cylinder Tier4", "DDEC13-DD11 Tier4", "DDEC13-DD13 Tier4", "DDEC13-DD16 Tier4", "DDEC16-DD13", "DDEC16-DD15",
		"DDEC16-DD16", "DDEC20-DD13", "DDEC20-DD15", "DDEC20-DD16", "DDEC20-MDEG 4-Cylinder StageV", "DDEC20-MDEG 6-Cylinder StageV", "DDEC20-DD11 StageV", "DDEC20-DD13 StageV", "DDEC20-DD16 StageV", "DDEC16-MDEG 4-Cylinder StageV",
		"DDEC16-MDEG 6-Cylinder StageV"
	};

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 3;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0AB_Reset_Water_in_Fuel_Values_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[11]
	{
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "engineTorque"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS047_Ignition_Cycle_Counter"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS045_Engine_Operating_Hours"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_hours"),
		new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC065_OP_Data_Oil_e2p_i_water_raised_eng_starts"),
		new Qualifier((QualifierTypes)32, "MCM21T", "61000F"),
		new Qualifier((QualifierTypes)32, "MCM21T", "610010")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[5] { "DT_AS045_Engine_Operating_Hours", "DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_hours", "DT_STO_ACC065_OP_Data_Oil_e2p_l_water_raised_eng_starts", "DT_AS087_Actual_Fuel_Mass", "RT_SR0AB_Reset_Water_in_Fuel_Values_Start" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
