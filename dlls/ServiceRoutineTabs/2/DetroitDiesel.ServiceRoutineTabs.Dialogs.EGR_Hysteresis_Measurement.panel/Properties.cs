using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Hysteresis_Measurement.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 52;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGR Hysteresis Measurement.panel";

	public override string Guid => "2e0407ca-6cbf-4210-9950-be1966e8fb27";

	public override string DisplayName => "EGR Hysteresis Measurement";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[2] { "DDEC16-DD5", "DDEC16-DD8" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "EGR";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[14]
	{
		new Qualifier((QualifierTypes)64, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_am_egr_current_valve"),
		new Qualifier((QualifierTypes)64, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_is05_adc_pos_egr"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS023_Engine_State"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS032_EGR_Actual_Valve_Position"),
		new Qualifier((QualifierTypes)1, "virtual", "ignitionStatus"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS031_EGR_Commanded_Governor_Value"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_EGR_Hysteresis"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_am_egr_current_valve"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_EGR_Hysteresis"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Start"),
		new Qualifier((QualifierTypes)1, "virtual", "ignitionStatus"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)2, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Stop")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_am_egr_current_valve", (IEnumerable<string>)new string[0]),
		new ServiceCall("MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Stop", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_is05_adc_pos_egr" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
