using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Nox_Sensor_Drift_Verification__MY20_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 209;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Nox Sensor Drift Verification (MY20).panel";

	public override string Guid => "7147a4a2-6d6d-4565-a46d-cfe2f9536475";

	public override string DisplayName => "NOx Sensor Verification";

	public override IEnumerable<string> SupportedDevices => new string[5] { "ACM301T", "CPC302T", "CPC501T", "CPC502T", "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[7] { "DDEC20-DD13", "DDEC20-DD15", "DDEC20-DD16", "New Cascadia CEEAce-Sleeper", "New Cascadia CEEAce-Daycab", "New Cascadia 2020-Daycab", "New Cascadia 2020-Sleeper" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)33;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[17]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS064_DPF_Regen_State"),
		new Qualifier((QualifierTypes)1, "virtual", "NeutralSwitch"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS111_NOx_raw_concentration"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS114_NOx_out_concentration"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS036_SCR_Inlet_NOx_Sensor"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS035_SCR_Outlet_NOx_Sensor"),
		new Qualifier((QualifierTypes)4, "ACM301T", "e2p_nox_out_dia_sens_runtime"),
		new Qualifier((QualifierTypes)4, "ACM301T", "e2p_nox_raw_dia_sens_runtime"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS019_SCR_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS007_DOC_Inlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS008_DOC_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS009_DPF_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[5] { "MCM21T", "ACM301T", "CPC302T", "CPC501T", "CPC502T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[11]
	{
		"DT_AS111_NOx_raw_concentration", "DT_AS114_NOx_out_concentration", "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet", "DT_AS064_DPF_Regen_State", "RT_SCR_Dosing_Quantity_Check_Start_Status", "RT_RC0400_DPF_High_Idle_regeneration_Start", "RT_DPF_High_Idle_regeneration_Start", "RT_SCR_Dosing_Quantity_Check_Stop_status", "RT_RC0400_DPF_High_Idle_regeneration_Stop",
		"RT_DPF_High_Idle_regeneration_Stop"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
