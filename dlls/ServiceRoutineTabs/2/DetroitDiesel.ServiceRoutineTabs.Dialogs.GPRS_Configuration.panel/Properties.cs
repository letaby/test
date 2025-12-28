using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.GPRS_Configuration.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 20;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\GPRS Configuration.panel";

	public override string Guid => "8a00baba-3cd4-4d14-95e0-0f29e22d8c3a";

	public override string DisplayName => "GPRS Configuration";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CTP01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Telematics";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "CTP01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DT_STO_Gprs_Config_1_gprsConfig", "DL_Gprs_Config_1", "FN_HardReset" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
