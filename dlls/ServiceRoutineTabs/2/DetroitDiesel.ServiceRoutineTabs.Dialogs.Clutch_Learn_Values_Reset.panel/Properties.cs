using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clutch_Learn_Values_Reset.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 57;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Clutch Learn Values Reset.panel";

	public override string Guid => "ce3d9121-5247-4a5b-8d12-014f38978893";

	public override string DisplayName => "Clutch Learn Values Reset";

	public override IEnumerable<string> SupportedDevices => new string[1] { "TCM05T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)4;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[13]
	{
		new Qualifier((QualifierTypes)8, "TCM05T", "DT_STO_2316_Clutch_Minimum_learned_value_Clutch_Minimum_learned_value"),
		new Qualifier((QualifierTypes)8, "TCM05T", "DT_STO_2317_Clutch_Maximum_learned_value_Clutch_Maximum_learned_value"),
		new Qualifier((QualifierTypes)2, "TCM05T", "RT_0528_Reset_Clutch_learned_values_Start"),
		new Qualifier((QualifierTypes)2, "TCM05T", "DL_B101_Clutch_replacement_Actual_clutch_facing_wear"),
		new Qualifier((QualifierTypes)2, "TCM05T", "FN_HardReset"),
		new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_No_gearshift_active"),
		new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Vehicle_standstill"),
		new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Engine_standstill"),
		new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Park_brake_activated"),
		new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_Transmission_in_neutral"),
		new Qualifier((QualifierTypes)1, "TCM05T", "DT_1F18_Environmental_conditions_Diagnostic_routines_No_learn_procedure_active"),
		new Qualifier((QualifierTypes)1, "TCM05T", "DT_2651_Clutch_facing_data_Clutch_facing_wear_Actual_value"),
		new Qualifier((QualifierTypes)1, "TCM05T", "DT_2651_Clutch_facing_data_Clutch_facing_Remaining_thickness")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("TCM05T", "RT_0528_Reset_Clutch_learned_values_Start", (IEnumerable<string>)new string[1] { "Reset_Clutch_learned_values=4" }),
		new ServiceCall("TCM05T", "DL_B101_Clutch_replacement_Actual_clutch_facing_wear", (IEnumerable<string>)new string[1] { "Clutch_replacement_Actual_clutch_facing_wear=0" }),
		new ServiceCall("TCM05T", "FN_HardReset", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "TCM05T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
