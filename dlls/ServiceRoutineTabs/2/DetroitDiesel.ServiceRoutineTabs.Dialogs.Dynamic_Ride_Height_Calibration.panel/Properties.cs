using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Calibration.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 177;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Dynamic Ride Height Calibration.panel";

	public override string Guid => "ef958949-eeaf-4c69-b402-b1007369fd52";

	public override string DisplayName => "Aerodynamic Height Control Calibration";

	public override IEnumerable<string> SupportedDevices => new string[2] { "HSV", "XMC02T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[7]
	{
		new Qualifier((QualifierTypes)1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_BrkAirPress2_Stat_EAPU"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1724"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1722"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1721"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1723"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "HSV", "XMC02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[8] { "DT_1739", "DT_1756", "DT_1740", "DT_1755", "RT_HIGHESTLVL", "RT_NOMLVL1", "RT_NOMLVL2", "RT_LOESTLVL" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
