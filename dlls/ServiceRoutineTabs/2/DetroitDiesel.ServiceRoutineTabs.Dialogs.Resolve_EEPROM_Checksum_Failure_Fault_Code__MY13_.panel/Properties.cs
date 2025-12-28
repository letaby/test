using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Resolve_EEPROM_Checksum_Failure_Fault_Code__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 46;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Resolve EEPROM Checksum Failure Fault Code (MY13).panel";

	public override string Guid => "737a8bd6-3679-4480-9ea2-05fd17a81e2b";

	public override string DisplayName => "Resolve EEPROM Checksum Failure Fault Code";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CPC04T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 2;

	public override int MinDynamicAccessLevel => 2;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)8, (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)0, "CPC04T", "740202")
	});

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "CPC04T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "RT_Reload_Original_CPC_Factory_Settings_Start_Routine_Status", "FN_KeyOffOnReset" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
