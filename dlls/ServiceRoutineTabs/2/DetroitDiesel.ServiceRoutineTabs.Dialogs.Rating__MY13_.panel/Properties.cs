using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Rating__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 37;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Rating (MY13).panel";

	public override string Guid => "e79a3dfe-9e51-49d3-bf4f-22e01f3a680c";

	public override string DisplayName => "Rating";

	public override IEnumerable<string> SupportedDevices => new string[2] { "CPC04T", "MCM21T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "CPC04T", "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[6] { "DT_STO_ID_Rated_brake_power_for_rat_0_Rated_brake_power_for_rat_0", "DT_STO_ID_Rated_engine_speed_for_rat_0_Rated_engine_speed_for_rat_0", "DT_STO_ID_Rated_brake_power_for_rat_1_Rated_brake_power_for_rat_1", "DT_STO_ID_Rated_engine_speed_for_rat_1_Rated_engine_speed_for_rat_1", "DT_STO_ID_Maximum_Engine_Torque_Maximum_Engine_Torque", "DT_STO_ID_Maximum_Torque_Speed_Maximum_Torque_Speed" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
