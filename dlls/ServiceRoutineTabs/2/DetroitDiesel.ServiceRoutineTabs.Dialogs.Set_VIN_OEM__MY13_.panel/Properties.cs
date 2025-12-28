using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN_OEM__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 56;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Set VIN OEM (MY13).panel";

	public override string Guid => "2f8f3c8c-e7d7-47b8-9884-0ef1d480672d";

	public override string DisplayName => "Set Engine Serial Number/Vehicle Identification Number for Non-Captive OEM";

	public override IEnumerable<string> SupportedDevices => new string[3] { "ACM21T", "CPC04T", "MCM21T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "MCM21T", "CPC04T", "ACM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[4] { "DL_ID_Write_Engine_Serial_Number", "FN_KeyOffOnReset", "DL_ID_Write_VIN_Current", "DL_ID_VIN_Current" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
