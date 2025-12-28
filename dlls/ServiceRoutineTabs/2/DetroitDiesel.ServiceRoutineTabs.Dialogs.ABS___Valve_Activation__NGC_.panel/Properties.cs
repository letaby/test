using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ABS___Valve_Activation__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 202;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ABS - Valve Activation (NGC).panel";

	public override string Guid => "ada06140-d934-457a-ac23-ecc588f7b068";

	public override string DisplayName => "ABS Valve Activation";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ABS02T", "SSAM02T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Anti-lock Braking System";

	public override FilterTypes Filters => (FilterTypes)8;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[13]
	{
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
		new Qualifier((QualifierTypes)1, "J1939-0", "DT_84"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_F_StartRoutine_Start"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_E_StartRoutine_Start"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_D_StartRoutine_Start"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_C_StartRoutine_Start"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_Hold_Trailer_Control_Pressure_StartRoutine_Start"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_B_StartRoutine_Start"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_A_StartRoutine_Start"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_3_2_Solenoid_valve_A_actuate_StartRoutine_Start"),
		new Qualifier((QualifierTypes)2, "ABS02T", "RT_3_2_Solenoid_valve_B_actuate_StartRoutine_Start"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_PressCrcut1_Stat_EAPU"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_PressCrcut2_Stat_EAPU")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[9]
	{
		new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_F_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }),
		new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_E_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }),
		new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_D_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }),
		new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_C_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }),
		new ServiceCall("ABS02T", "RT_Hold_Trailer_Control_Pressure_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }),
		new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_B_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }),
		new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_A_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }),
		new ServiceCall("ABS02T", "RT_3_2_Solenoid_valve_A_actuate_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" }),
		new ServiceCall("ABS02T", "RT_3_2_Solenoid_valve_B_actuate_StartRoutine_Start", (IEnumerable<string>)new string[1] { "Timing=2000" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ABS02T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
