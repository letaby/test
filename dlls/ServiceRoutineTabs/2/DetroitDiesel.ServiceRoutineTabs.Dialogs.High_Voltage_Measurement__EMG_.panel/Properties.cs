using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.High_Voltage_Measurement__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 27;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\High Voltage Measurement (EMG).panel";

	public override string Guid => "341d2647-af02-44cb-91fd-6448e7402af1";

	public override string DisplayName => "High Voltage Measurement";

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

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[24]
	{
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_VoltageReadoutStat"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_OTF_Readout"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_HV_Readout_Start"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_HV_Readout_Stop"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_OTF_Readout"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_PtcCab2"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS01"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS02"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS03"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS04"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS05"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS06"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS07"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS08"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS09"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti1ActDcVolt"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti2ActDcVolt"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti3ActDcVolt_3"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HVDCLinkVoltCvalDCL"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_PtcCab1"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvPtcBatt1HvVoltage"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvPtcBatt2HvVoltage"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HVDCLinkVoltCvalEComp"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_ErcHvVolt")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Request_Results_VoltageReadoutStat", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Stop", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "ECPC01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
