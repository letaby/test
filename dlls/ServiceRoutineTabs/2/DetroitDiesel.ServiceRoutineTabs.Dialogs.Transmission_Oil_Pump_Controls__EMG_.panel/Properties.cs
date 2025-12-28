using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Oil_Pump_Controls__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 22;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Transmission Oil Pump Controls (EMG).panel";

	public override string Guid => "fc0285bc-3d5f-4a5f-909d-91e48558cb03";

	public override string DisplayName => "eTransmission Oil Pump Controls";

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
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump1_Control_results_resp"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump1_Control_resp"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump1_Control_stop_resp"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump2_Control_results_resp"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump2_Control_resp"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump2_Control_stop_resp")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[6]
	{
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump1_Control_results_resp", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump1_Control_resp", (IEnumerable<string>)new string[2] { "ETHM_Oil_Pump1_Control=100", "ETHM_Oil_Pump2_Control=0" }),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump1_Control_stop_resp", (IEnumerable<string>)new string[2] { "ETHM_Oil_Pump1_Control_stop=0", "ETHM_Oil_Pump2_Control_stop=0" }),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump2_Control_results_resp", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump2_Control_resp", (IEnumerable<string>)new string[2] { "ETHM_Oil_Pump1_Control=0", "ETHM_Oil_Pump2_Control=100" }),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump2_Control_stop_resp", (IEnumerable<string>)new string[2] { "ETHM_Oil_Pump1_Control_stop=0", "ETHM_Oil_Pump2_Control_stop=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ECPC01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
