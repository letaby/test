using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 88;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Procedures\\Idle Speed Balance.panel";

	public override string Guid => "df1d35fa-b57f-41a2-ad37-db7cf26b86d8";

	public override string DisplayName => "Idle Speed Balance";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM" };

	public override IEnumerable<string> SupportedEquipment => new string[6] { "DD13", "DD15", "DD15EURO4", "MBE4000", "MBE900", "S60" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => false;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[3]
	{
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS013_Coolant_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_AS014_Fuel_Temperature"),
		new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[8] { "RT_SR066_Idle_Speed_Balance_Test_Start", "DT_ASL004_Engine_State", "DT_Idle_Speed_Balance_Values_Cylinder_1", "DT_Idle_Speed_Balance_Values_Cylinder_2", "DT_Idle_Speed_Balance_Values_Cylinder_3", "DT_Idle_Speed_Balance_Values_Cylinder_4", "DT_Idle_Speed_Balance_Values_Cylinder_5", "DT_Idle_Speed_Balance_Values_Cylinder_6" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
