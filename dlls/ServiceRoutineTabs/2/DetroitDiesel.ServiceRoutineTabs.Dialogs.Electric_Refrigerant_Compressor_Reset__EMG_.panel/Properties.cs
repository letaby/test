using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Electric_Refrigerant_Compressor_Reset__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 25;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Electric Refrigerant Compressor Reset (EMG).panel";

	public override string Guid => "56f70650-3d3a-4684-8d3d-2efdfb2e272d";

	public override string DisplayName => "Electric Refrigerant Compressor Reset";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ECPC01T" };

	public override IEnumerable<string> SupportedEquipment => new string[1] { "EMOBILITY-eDrive Powertrain" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "ePowertrain";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[10]
	{
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ElectricRefrigerantCompressorReset"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results_ERC_Reset_Ctrl"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ElectricRefrigerantCompressorReset"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Start"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Stop")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Stop", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
