using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Pump_Priming.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 21;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DEF Pump Priming.panel";

	public override string Guid => "848a9732-7941-4c64-a30d-bf8561f9ede5";

	public override string DisplayName => "DEF Pump Priming";

	public override IEnumerable<string> SupportedDevices => new string[2] { "ACM21T", "ACM301T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[6]
	{
		new Qualifier((QualifierTypes)1, "virtual", "AmbientAirTemperature"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DEFPumpPriming"),
		new Qualifier((QualifierTypes)1, "virtual", "DEFTankTemperature"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "ADSPumpSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "DEFPressure")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
