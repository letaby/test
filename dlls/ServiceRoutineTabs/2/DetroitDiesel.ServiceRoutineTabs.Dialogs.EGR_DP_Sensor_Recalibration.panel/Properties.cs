using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 26;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGR DP Sensor Recalibration.panel";

	public override string Guid => "edd97055-81df-46ec-a8ea-fe34a2e959c7";

	public override string DisplayName => "EGR Delta Pressure Sensor Recalibration";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM" };

	public override IEnumerable<string> SupportedEquipment => new string[4] { "DD13", "DD15", "DD15EURO4", "S60" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "EGR";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "RT_SR065_Forced_Auto_Cal_EGR_Delta_P_Sensor_Start_status", "DT_AS010_Engine_Speed" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
