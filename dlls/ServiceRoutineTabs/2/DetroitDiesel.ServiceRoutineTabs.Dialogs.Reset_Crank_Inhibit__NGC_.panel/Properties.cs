using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Reset_Crank_Inhibit__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 4;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Reset Crank Inhibit (NGC).panel";

	public override string Guid => "3d383230-0338-42b7-91c3-d20dba8deb7e";

	public override string DisplayName => "Crank Inhibit Reset";

	public override IEnumerable<string> SupportedDevices => new string[2] { "CTP01T", "SSAM02T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Telematics";

	public override FilterTypes Filters => (FilterTypes)11;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[4]
	{
		new Qualifier((QualifierTypes)64, "CTP01T", "RT_Reset_Crank_inhibition_Start_status"),
		new Qualifier((QualifierTypes)2, "CTP01T", "RT_Reset_Crank_inhibition_Start_status"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_FOA_Diagnostic_Displayables_DDFOA_FOTA_InProcess"),
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_FOA_Diagnostic_Displayables_DDFOA_CrankIntrlService_Cmd")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[1]
	{
		new ServiceCall("CTP01T", "RT_Reset_Crank_inhibition_Start_status", (IEnumerable<string>)new string[1] { "InhibitionStatus=0" })
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "SSAM02T", "CTP01T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
