using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Shift_Abort_Counter.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 45;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\TCM Shift Abort Counter.panel";

	public override string Guid => "2d9349d3-82e9-4643-883b-979fe429a54f";

	public override string DisplayName => "Transmission Shift Abort Counter";

	public override IEnumerable<string> SupportedDevices => new string[2] { "TCM01T", "TCM05T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)6;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)2, "TCM01T", "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start"),
		new Qualifier((QualifierTypes)2, "TCM01T", "RT_0440_Schaltabbruchzaehler_sichern_Start")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[2]
	{
		new ServiceCall("TCM01T", "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("TCM01T", "RT_0440_Schaltabbruchzaehler_sichern_Start", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "TCM01T", "TCM05T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[4] { "RT_0441_Schaltabbruchzaehler_zuruecksetzen_Start", "RT_0440_Schaltabbruchzaehler_sichern_Start", "RT_0441_Reset_shift_abort_counters_Start", "RT_0440_Save_copy_of_shift_abort_counters_Start" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
