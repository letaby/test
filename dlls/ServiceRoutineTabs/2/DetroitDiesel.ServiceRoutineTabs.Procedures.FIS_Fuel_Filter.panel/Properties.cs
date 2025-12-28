using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 214;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\FIS Fuel Filter.panel";

	public override string Guid => "99a0244a-4e0f-4d76-bd31-61cdb21ceff9";

	public override string DisplayName => "FIS Fuel Filter";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM02T" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DD13", "DD15", "DD16" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[7]
	{
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "engineTorque"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS087_Actual_Fuel_Mass"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS124_LPPO_Fuel_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS125_Fuel_Filter_State")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[6] { "DT_AS067_Coolant_Temperatures_2", "DT_AS014_Fuel_Temperature", "DT_AS087_Actual_Fuel_Mass", "DT_AS125_Fuel_Filter_State", "DT_AS124_LPPO_Fuel_Pressure", "RT_SR082_Fuel_Filter_Reset_Start_Status" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
