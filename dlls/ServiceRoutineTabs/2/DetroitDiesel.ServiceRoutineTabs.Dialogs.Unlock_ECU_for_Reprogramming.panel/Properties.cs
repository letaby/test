using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_ECU_for_Reprogramming.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 56;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Unlock ECU for Reprogramming.panel";

	public override string Guid => "dfc5ce6d-f348-4e46-9ebb-a5848d1dccab";

	public override string DisplayName => "Unlock ECU for Reprogramming";

	public override IEnumerable<string> SupportedDevices => new string[3] { "ACM21T", "ACM301T", "MCM21T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Off-Highway";

	public override FilterTypes Filters => (FilterTypes)3;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "ACM21T", "ACM301T", "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DJ_Read_Z_Status_and_Fuelmap_Status", "DJ_Read_AUT64_VeDoc_Input", "RT_SR089_X_Routine_improved_Start_Status" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
