using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_Odometers__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 84;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Set Odometers (NGC).panel";

	public override string Guid => "bceccbd2-7786-4771-936c-820b9a390cae";

	public override string DisplayName => "Set Odometers";

	public override IEnumerable<string> SupportedDevices => new string[5] { "CPC302T", "CPC501T", "CPC502T", "ICC501T", "ICUC01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)10;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 1;

	public override int MinDynamicAccessLevel => 1;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)16, "fake", "fakeIcOdometer"),
		new Qualifier((QualifierTypes)1, "J1939-0", "DT_917")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[5] { "ICUC01T", "ICC501T", "CPC302T", "CPC501T", "J1939-0" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "DL_ID_Odometer", "DT_917" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
