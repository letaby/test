using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DCB_Unlock.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 11;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\DCB Unlock.panel";

	public override string Guid => "21b2728f-1965-4bd9-9b71-d7980cf7c74d";

	public override string DisplayName => "DCB Unlock";

	public override IEnumerable<string> SupportedDevices => new string[2] { "DCB01T", "DCB02T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)2, "DCB02T", "DL_High_Voltage_Lock"),
		new Qualifier((QualifierTypes)8, "DCB01T", "DT_STO_High_Voltage_Lock_HV_Lock_Status"),
		new Qualifier((QualifierTypes)8, "DCB02T", "DT_STO_High_Voltage_Lock_HV_Lock_Status"),
		new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat"),
		new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed"),
		new Qualifier((QualifierTypes)2, "DCB01T", "DL_High_Voltage_Lock")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[2]
	{
		new ServiceCall("DCB02T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "HV_Lock_Status=0" }),
		new ServiceCall("DCB01T", "DL_High_Voltage_Lock", (IEnumerable<string>)new string[1] { "HV_Lock_Status=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "DCB01T", "DCB02T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
