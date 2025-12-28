using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.FaultCodeTabs.General.Monitor_Performance__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 99;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Fault Code Tabs\\General\\Monitor Performance (EPA10).panel";

	public override string Guid => "7b563059-cee7-452b-9964-935eae04965a";

	public override string DisplayName => "Monitor Performance";

	public override IEnumerable<string> SupportedDevices => new string[4] { "ACM02T", "ACM21T", "MCM02T", "MCM21T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "OBD";

	public override FilterTypes Filters => (FilterTypes)65535;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[4] { "DT_STO_Read_aggregated_RBM_group_rates_Assigned_RBM_group_number_1", "DT_STO_Read_aggregated_RBM_group_rates_Ignition_Cycle_counter", "DT_STO_Read_aggregated_RBM_group_rates_General_Denominator", "DT_STO_Read_aggregated_RBM_group_rates_Number_of_following_MU_rates" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
