using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_in_Fuel.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 216;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\FIS Water in Fuel.panel";

	public override string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

	public override string DisplayName => "FIS Water in Fuel";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM02T" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DD13", "DD15", "DD16" };

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
		new Qualifier((QualifierTypes)2, "MCM02T", "RT_SR0AB_Reset_Water_in_Fuel_Values_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[11]
	{
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "engineTorque"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS047_Ignition_Cycle_Counter"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS045_Engine_Operating_Hours"),
		new Qualifier((QualifierTypes)4, "MCM02T", "e2p_l_water_raised_eng_hours"),
		new Qualifier((QualifierTypes)4, "MCM02T", "e2p_l_water_raised_eng_starts"),
		new Qualifier((QualifierTypes)32, "MCM02T", "61000F"),
		new Qualifier((QualifierTypes)32, "MCM02T", "610010")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DT_ASL007_Engine_Operating_Hours", "DT_AS087_Actual_Fuel_Mass", "RT_SR0AB_Reset_Water_in_Fuel_Values_Start" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
