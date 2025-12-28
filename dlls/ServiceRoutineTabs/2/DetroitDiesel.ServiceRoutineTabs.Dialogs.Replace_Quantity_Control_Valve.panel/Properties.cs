using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Replace_Quantity_Control_Valve.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 23;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Replace Quantity Control Valve.panel";

	public override string Guid => "3bcb607b-6b5c-482f-8c62-ca64afcabb01";

	public override string DisplayName => "Replace Quantity Control Valve";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DD13", "DD15", "DD15EURO4" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Fuel System";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)4, "MCM", "Quantity_Control_Valve_Adaptation_Positive"),
		new Qualifier((QualifierTypes)4, "MCM", "Quantity_Control_Valve_Adaptation_Negative")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "RT_SR014_SET_EOL_Default_Values_Start" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
