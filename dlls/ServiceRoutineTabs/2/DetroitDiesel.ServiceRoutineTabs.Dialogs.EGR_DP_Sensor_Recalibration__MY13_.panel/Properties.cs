using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 36;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGR DP Sensor Recalibration (MY13).panel";

	public override string Guid => "edd97055-81df-46ec-a8ea-fe34a2e959c7";

	public override string DisplayName => "EGR Delta Pressure Sensor Recalibration";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[21]
	{
		"DDEC13-DD13", "DDEC13-DD15", "DDEC13-DD16", "DDEC13-MDEG 4-Cylinder Tier4", "DDEC13-MDEG 6-Cylinder Tier4", "DDEC13-DD11 Tier4", "DDEC13-DD13 Tier4", "DDEC13-DD16 Tier4", "DDEC16-DD13", "DDEC16-DD15",
		"DDEC16-DD16", "DDEC20-DD13", "DDEC20-DD15", "DDEC20-DD16", "DDEC20-MDEG 4-Cylinder StageV", "DDEC20-MDEG 6-Cylinder StageV", "DDEC20-DD11 StageV", "DDEC20-DD13 StageV", "DDEC20-DD16 StageV", "DDEC16-MDEG 4-Cylinder StageV",
		"DDEC16-MDEG 6-Cylinder StageV"
	};

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "EGR";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "RT_SR065_Forced_Auto_Cal_EGR_Delta_P_Sensor_Start_status", "DT_AS010_Engine_Speed" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
