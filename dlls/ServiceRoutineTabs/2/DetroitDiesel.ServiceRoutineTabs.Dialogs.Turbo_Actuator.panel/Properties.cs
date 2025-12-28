using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Turbo_Actuator.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 37;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Turbo Actuator.panel";

	public override string Guid => "c285aa6b-2e21-42fa-8dfe-73546b74256e";

	public override string DisplayName => "Turbo Actuator";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM" };

	public override IEnumerable<string> SupportedEquipment => new string[1] { "S60" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[10] { "DT_AS052_SRA5_Status_Code", "RT_SR061_Pre_install_Routine_Start_ActuatorStatus", "RT_SR061_Pre_install_Routine_Request_Results_ActuatorResult", "RT_SR061_Pre_install_Routine_Stop_ActuatorNumber", "RT_SR062_Self_Calibration_Routine_Start_ActuatorStartStatus", "RT_SR062_Self_Calibration_Routine_Request_Results_ActuatorResultStatus", "RT_SR062_Self_Calibration_Routine_Stop_ActuatorNumber", "RT_SR063_Hysteres_Test_Routine_Start_ActuatorStartStatus", "RT_SR063_Hysteres_Test_Routine_Request_Results_Data", "RT_SR063_Hysteres_Test_Routine_Stop_ActuatorNumber" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
