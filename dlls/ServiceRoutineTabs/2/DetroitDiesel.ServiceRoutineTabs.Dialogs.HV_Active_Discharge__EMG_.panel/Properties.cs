using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HV_Active_Discharge__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 37;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\HV Active Discharge (EMG).panel";

	public override string Guid => "341d2647-af02-44cb-91fd-6448e7402af1";

	public override string DisplayName => "High Voltage Active Discharge";

	public override IEnumerable<string> SupportedDevices => new string[1] { "ECPC01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "ePowertrain";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> ForceQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS219_globalhvil_globalhvil"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS008_HV_Ready")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[9]
	{
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_OTF_HV_ActiveDischarge"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Start"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS008_HV_Ready"),
		new Qualifier((QualifierTypes)2, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Stop"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_OTF_HV_ActiveDischarge"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS008_HV_Ready"),
		new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status"),
		new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS219_globalhvil_globalhvil")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Stop", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
