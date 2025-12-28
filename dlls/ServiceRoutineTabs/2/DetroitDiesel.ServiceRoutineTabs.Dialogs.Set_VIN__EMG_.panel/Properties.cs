using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN__EMG_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 146;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Set VIN (EMG).panel";

	public override string Guid => "3b618fde-ca78-4447-a441-117388869096";

	public override string DisplayName => "Set Vehicle Identification Number";

	public override IEnumerable<string> SupportedDevices => new string[18]
	{
		"BMS01T", "BMS201T", "BMS301T", "BMS401T", "BMS501T", "BMS601T", "BMS701T", "BMS801T", "BMS901T", "DCB01T",
		"DCB02T", "DCL101T", "EAPU03T", "ECPC01T", "ETCM01T", "PTI101T", "PTI201T", "PTI301T"
	};

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[19]
	{
		"J1939-255", "ECPC01T", "ETCM01T", "BMS01T", "BMS201T", "BMS301T", "BMS401T", "BMS501T", "BMS601T", "BMS701T",
		"BMS801T", "BMS901T", "PTI101T", "PTI201T", "PTI301T", "DCL101T", "EAPU03T", "DCB01T", "DCB02T"
	};

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "FN_HardReset", "DL_ID_VIN_Current" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
