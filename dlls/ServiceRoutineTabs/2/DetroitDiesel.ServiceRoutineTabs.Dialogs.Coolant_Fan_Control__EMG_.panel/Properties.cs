using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Coolant_Fan_Control__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 8;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Coolant Fan Control (EMG).panel";

	public override string Guid => "4b7fdfab-7f2e-4349-ac37-9ea54dea6d06";

	public override string DisplayName => "Coolant Fan Control";

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

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[12]
	{
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS252_DrvCircInTemp_u16_DrvCircInTemp_u16"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS202_Batt_Circ_Temp_Batt_Circ_Temp"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_CoolantFanControl"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_CoolantFanControl"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Start"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_FanCtrl_Stop")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Start", (IEnumerable<string>)new string[3] { "Requested_duty_cycle_for_Brake_Resistor_1=0", "Requested_duty_cycle_for_Brake_Resistor_2=0", "Requested_duty_cycle_for_Edrive_Fan=50" }),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_FanCtrl_Stop", (IEnumerable<string>)new string[3] { "OTF_ETHM_FanCtrl_FanBrakeResistor1=0", "OTF_ETHM_FanCtrl_FanBrakeResistor2=0", "OTF_ETHM_FanCtrl_eDriveFan=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ECPC01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "RT_OTF_ETHM_FanCtrl_Request_Results_Requested_duty_cycle_for_Edrive_Fan" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
