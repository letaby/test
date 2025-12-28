using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Voltages_SCR_DPF__Stage_V_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 60;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Voltages SCR DPF (Stage V).panel";

	public override string Guid => "66ebe157-c680-431e-b3a8-5a4a06f37bdd";

	public override string DisplayName => "SCR and DPF Voltages";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ACM301T" };

	public override IEnumerable<string> SupportedEquipment => new string[15]
	{
		"DDEC13-DD13", "DDEC13-DD15", "DDEC13-DD16", "DDEC13-DD11 Tier4", "DDEC13-DD13 Tier4", "DDEC13-DD16 Tier4", "DDEC16-DD13", "DDEC16-DD15", "DDEC16-DD16", "DDEC20-DD16 StageV",
		"DDEC20-DD13 StageV", "DDEC20-DD11 StageV", "DDEC20-DD13", "DDEC20-DD15", "DDEC20-DD16"
	};

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS093_DEF_Tank_Temperature_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS094_DEF_Tank_Level_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS148_DPF_Outlet_Temperature_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS147_T_DOC_Outlet_Temperature_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS146_T_DOC_Inlet_Temperature_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS098_DEF_Pressure_Sensor_Voltage"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS192_PM_sensor_supply_voltage"),
		new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS096_SCR_Oulet_Temperature_Sensor_Voltage")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ACM301T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DT_AS146_T_DOC_Inlet_Temperature_Sensor_Voltage", "DT_AS147_T_DOC_Outlet_Temperature_Sensor_Voltage", "DT_AS148_DPF_Outlet_Temperature_Sensor_Voltage" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
