using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 61;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\FIS Low Pressure Leak Test.panel";

	public override string Guid => "a0414769-2da4-4114-beb1-bbe336a960e1";

	public override string DisplayName => "FIS Low Pressure Leak Test";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM02T" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DD13", "DD15", "DD16" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)66;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS024_Fuel_Compensation_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS024_Fuel_Compensation_Pressure")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[4] { "RT_SR003_PWM_Routine_by_Function_for_Production_Start_Control_Value", "RT_SR003_PWM_Routine_by_Function_for_Production_Stop_Function_Name", "DT_AS010_Engine_Speed", "DT_AS024_Fuel_Compensation_Pressure" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
