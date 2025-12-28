using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Deaeration_Battery__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 100;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Deaeration Battery (EMG).panel";

	public override string Guid => "2d7a2a0d-59da-498e-bfc1-bc0f2fdf0e18";

	public override string DisplayName => "De-Aeration Battery";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ECPC01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "ePowertrain";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[12]
	{
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery1_Circuit_Res"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery2_Circuit_Res"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "IOC_IOC_ETHM_Shutoff_ValveControl_Return_Control"),
		new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br1"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br2"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS236_BattCircValveActPos_BattCircValveActPos"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Request_Results_ETHM_Battery_Circuit_Deaeration"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Start_ETHM_Battery_Circuit_Deaeration"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Stop_ETHM_Battery_Circuit_Deaeration")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[6]
	{
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery1_Circuit_Res", (IEnumerable<string>)new string[2] { "WaterPump_Speed_Battery1_Circuit_Req=0", "WaterPump_Speed_Battery2_Circuit_Req=0" }),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery2_Circuit_Res", (IEnumerable<string>)new string[2] { "WaterPump_Speed_Battery1_Circuit_Req=0", "WaterPump_Speed_Battery2_Circuit_Req=0" }),
		new ServiceCall("ECPC01T", "IOC_IOC_ETHM_Shutoff_ValveControl_Return_Control", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Request_Results_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Start_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>)new string[1] { "Battery_Circuit_Deaeration_contro_start=1" }),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Stop_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>)new string[1] { "Battery_Circuit_Deaeration_control_start=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ECPC01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "DT_Current_value_percentage_water_pump_Br2" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
