using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.FICM_Routine.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 24;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\FICM Routine.panel";

	public override string Guid => "defa4a63-bdf9-46fd-a5bd-0baf415142e8";

	public override string DisplayName => "Fuel Injector Cleaning Machine Routine";

	public override IEnumerable<string> SupportedDevices => new string[9] { "CPC02T", "CPC04T", "CPC2", "CPC302T", "CPC501T", "CPC502T", "MCM", "MCM02T", "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[23]
	{
		"DDEC10-DD13", "DDEC10-DD15", "DDEC10-DD16", "DDEC10-DD15EURO4", "DDEC13-DD13", "DDEC13-DD15", "DDEC13-DD16", "DDEC13-DD13 Tier4", "DDEC13-DD16 Tier4", "DDEC16-DD13",
		"DDEC16-DD15", "DDEC16-DD16", "DDEC16-DD13EURO5", "DDEC16-DD16EURO5", "DDEC20-DD13", "DDEC20-DD15", "DDEC20-DD16", "DDEC20-DD13 StageV", "DDEC20-DD16 StageV", "DDEC6-DD16",
		"DDEC6-DD15", "DDEC6-DD15EURO4", "DDEC6-DD13"
	};

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)3;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[6]
	{
		new Qualifier((QualifierTypes)256, "Extension", "SP_Powertrain_Repair_Validation_Routine1"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_FuelInjectorCleaningMachine_Routine"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)1, "virtual", "VehicleCheckStatus"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
