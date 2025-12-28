using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Variable_Speed_Fan__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 70;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Tests\\Variable Speed Fan (MY13).panel";

	public override string Guid => "ca9a2e26-6276-4680-bf3c-6f9040780ae6";

	public override string DisplayName => "Variable Speed Fan Control";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	protected override IEnumerable<string> RequiredDataItemConditionsSource => new string[1] { "Parameter.MCM21T.Fan_Type:(Fault),(0,Fault),(1,Fault),(2,Ok),(3,Ok),(4,Fault),(5,Ok),(6,Fault),(7,Fault),(8,Ok),(9,Ok),(10,Ok),(11,Ok),(12,Ok),(13,Ok)" };

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[6]
	{
		new Qualifier((QualifierTypes)4, "MCM21T", "Fan_Type"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS026_Fan_Speed"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS169_Coolant_out_temperature"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "RT_SR003_PWM_Routine_by_Function_Start_Control_Value", "RT_SR003_PWM_Routine_by_Function_Stop_Function_Name" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
