using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Three_By_Two_Way_Valve_Teach_In__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 31;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Three By Two Way Valve Teach In (EMG).panel";

	public override string Guid => "10d60094-a21f-45b9-bd02-17ba4671a240";

	public override string DisplayName => "3by2 Way Valve Teach In";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ECPC01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "ePowertrain";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[21]
	{
		new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_BatteryCoolant3By2WayValveTeachIn"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_EdriveCoolant3By2WayTeachIn"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_BatteryCoolant3By2WayValveTeachIn"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_BatteryCirc"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
		new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_BasicCircValvePosCtrlState"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_EdriveCoolant3By2WayTeachIn"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_ExtensionCkt_1"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
		new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_ExtCircValvePosCtrlState_1")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[6]
	{
		new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_BatteryCirc", (IEnumerable<string>)new string[3] { "3by2WayValveBatteryCircuit=1", "3by2WayValveExtCircuit1=0", "3by2WayValveExtCircuit2=0" }),
		new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_BasicCircValvePosCtrlState", (IEnumerable<string>)new string[3] { "BasicCircValvePosCtrlState=1", "ExtCircValvePosCtrlState_1=0", "ExtCircValvePosCtrlState_2=0" }),
		new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_ExtensionCkt_1", (IEnumerable<string>)new string[3] { "3by2WayValveBatteryCircuit=0", "3by2WayValveExtCircuit1=1", "3by2WayValveExtCircuit2=0" }),
		new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_ExtCircValvePosCtrlState_1", (IEnumerable<string>)new string[3] { "BasicCircValvePosCtrlState=0", "ExtCircValvePosCtrlState_1=1", "ExtCircValvePosCtrlState_2=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ECPC01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
