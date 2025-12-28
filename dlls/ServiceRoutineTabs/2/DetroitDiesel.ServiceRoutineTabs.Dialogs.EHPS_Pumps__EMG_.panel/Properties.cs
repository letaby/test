using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EHPS_Pumps__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 24;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EHPS Pumps (EMG).panel";

	public override string Guid => "a63c66f1-754e-4877-983a-e9869dd95dec";

	public override string DisplayName => "ePower Steering";

	public override IEnumerable<string> SupportedDevices => new string[2] { "EHPS201T", "EHPS401T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "ePowertrain";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "EHPS201T", "EHPS401T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "RT_Pump_Routine_Start", "RT_Pump_Routine_Stop" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
