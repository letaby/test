using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 205;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\EGR Low Flow Test (MY13).panel";

	public override string Guid => "33d784e1-2169-485b-abf5-771d4bd56023";

	public override string DisplayName => "EGR Low Flow Test";

	public override IEnumerable<string> SupportedDevices => new string[3] { "CPC04T", "CPC302T", "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[6] { "DDEC13-DD15", "DDEC13-DD16", "DDEC13-DD13", "DDEC16-DD15", "DDEC16-DD16", "DDEC16-DD13" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "EGR";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[4]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS060_Charge_Air_Cooler_Outlet_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS031_EGR_Commanded_Governor_Value"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS025_EGR_Delta_Pressure")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[7]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS025_EGR_Delta_Pressure"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS031_EGR_Commanded_Governor_Value"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS060_Charge_Air_Cooler_Outlet_Temperature"),
		new Qualifier((QualifierTypes)32, "MCM21T", "630A12"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[2] { "DT_AS060_Charge_Air_Cooler_Outlet_Temperature", "DT_AS013_Coolant_Temperature" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
