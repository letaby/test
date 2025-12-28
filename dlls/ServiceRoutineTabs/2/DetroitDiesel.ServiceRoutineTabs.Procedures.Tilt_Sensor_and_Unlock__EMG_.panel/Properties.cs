using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor_and_Unlock__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 52;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Tilt Sensor and Unlock (EMG).panel";

	public override string Guid => "53dd9aa7-2829-4b73-b91f-539495c446d9";

	public override string DisplayName => "Tilt Sensor & Unlock";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ETCM01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "Transmission";

	public override FilterTypes Filters => (FilterTypes)6;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[10]
	{
		new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition"),
		new Qualifier((QualifierTypes)2, "ETCM01T", "DJ_Release_transport_security_for_eTCM"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Transmission_Teach_in_State_current_state"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Inclination_Sensor_Signal_Value_current_value"),
		new Qualifier((QualifierTypes)32, "ETCM01T", "14F1EE"),
		new Qualifier((QualifierTypes)32, "ETCM01T", "14F1ED"),
		new Qualifier((QualifierTypes)2, "ETCM01T", "RT_Inclination_Sensor_Teach_in_Procedure_Start"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[2]
	{
		new ServiceCall("ETCM01T", "DJ_Release_transport_security_for_eTCM", (IEnumerable<string>)new string[0]),
		new ServiceCall("ETCM01T", "RT_Inclination_Sensor_Teach_in_Procedure_Start", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
