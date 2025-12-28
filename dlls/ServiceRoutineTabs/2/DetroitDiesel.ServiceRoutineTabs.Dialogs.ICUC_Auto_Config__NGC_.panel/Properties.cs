using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Auto_Config__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 42;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\ICUC Auto Config (NGC).panel";

	public override string Guid => "a76e1ffc-16b4-4c5f-a6ac-9a36646737e0";

	public override string DisplayName => "Instrument Cluster Automatic Configuration";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ICC501T", "ICUC01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[5]
	{
		new Qualifier((QualifierTypes)32, "ICUC01T", "0DFBFF"),
		new Qualifier((QualifierTypes)32, "ICUC01T", "0FFBFF"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ICUC01T_AutoConfig"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ICUC01T_AutoConfig_PID20"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_ICUC01T_AutoConfig_PID25")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "UDS-23", "ICUC01T" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[3] { "SP_ICUC01T_AutoConfig", "SP_ICUC01T_AutoConfig_PID20", "SP_ICUC01T_AutoConfig_PID25" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
