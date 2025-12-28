using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_CPC_Odometer__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 94;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Set CPC Odometer (EPA10).panel";

	public override string Guid => "bceccbd2-7786-4771-936c-820b9a390cae";

	public override string DisplayName => "Set Odometer";

	public override IEnumerable<string> SupportedDevices => new string[1] { "CPC02T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 1;

	public override int MinDynamicAccessLevel => 1;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)8, "CPC02T", "CO_Odometer")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "CPC02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "DL_ID_Odometer" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
