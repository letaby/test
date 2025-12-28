using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Deaeration_eDrive__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 101;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Deaeration eDrive (EMG).panel";

	public override string Guid => "2d7a2a0d-59da-498e-bfc1-bc0f2fdf0e18";

	public override string DisplayName => "De-Aeration eDrive";

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

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[14]
	{
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery1_Circuit_Res"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery2_Circuit_Res"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "IOC_IOC_ETHM_Shutoff_ValveControl_Return_Control"),
		new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_Cur_Val_Per_Water_Pump_eDrv"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_Cur_Val_Per_Water_Pump2_eDrv"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_Cur_Val_Per_Water_Pump3_eDrv"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS238_HeatingCircValveActPos_HeatingCircValveActPos"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS237_ExtCircValveActPos_ExtCircValveActPos"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Request_Results_e_drive_circuit_deaeration_result_resp"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Start_e_drive_circuit_deaeration_start_resp"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Stop_e_drive_circuit_deaeration_stop_resp")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[6]
	{
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery1_Circuit_Res", (IEnumerable<string>)new string[2] { "WaterPump_Speed_Battery1_Circuit_Req=0", "WaterPump_Speed_Battery2_Circuit_Req=0" }),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery2_Circuit_Res", (IEnumerable<string>)new string[2] { "WaterPump_Speed_Battery1_Circuit_Req=0", "WaterPump_Speed_Battery2_Circuit_Req=0" }),
		new ServiceCall("ECPC01T", "IOC_IOC_ETHM_Shutoff_ValveControl_Return_Control", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Request_Results_e_drive_circuit_deaeration_result_resp", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Start_e_drive_circuit_deaeration_start_resp", (IEnumerable<string>)new string[1] { "e_drive_circuit_deaeration_start=1" }),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Stop_e_drive_circuit_deaeration_stop_resp", (IEnumerable<string>)new string[1] { "e_drive_circuit_deaeration_stop=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ECPC01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "DT_Cur_Val_Per_Water_Pump3_eDrv" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
