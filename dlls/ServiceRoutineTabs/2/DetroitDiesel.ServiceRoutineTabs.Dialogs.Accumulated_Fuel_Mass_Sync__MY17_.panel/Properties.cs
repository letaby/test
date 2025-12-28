using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Accumulated_Fuel_Mass_Sync__MY17_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 143;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Accumulated Fuel Mass Sync (MY17).panel";

	public override string Guid => "2e0407ca-6cbf-4210-9950-be1966e8fb27";

	public override string DisplayName => "Accumulated Fuel Mass Sync";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ACM21T", "MCM21T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[6]
	{
		new Qualifier((QualifierTypes)2, "ACM21T", "RT_SR02EB_ATS_lifetime_ageing_strategy_Start"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0EB_ATS_lifetime_ageing_strategy_Start"),
		new Qualifier((QualifierTypes)2, "ACM21T", "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR012_Save_EOL_Data_Request_Start"),
		new Qualifier((QualifierTypes)2, "ACM21T", "RT_Save_EOL_Data_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)64, "MCM21T", "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"),
		new Qualifier((QualifierTypes)64, "ACM21T", "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "MCM21T", "ACM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[6] { "RT_SR0EB_ATS_lifetime_ageing_strategy_Start", "RT_SR012_Save_EOL_Data_Request_Start", "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven", "RT_SR02EB_ATS_lifetime_ageing_strategy_Start", "RT_Save_EOL_Data_Start", "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
