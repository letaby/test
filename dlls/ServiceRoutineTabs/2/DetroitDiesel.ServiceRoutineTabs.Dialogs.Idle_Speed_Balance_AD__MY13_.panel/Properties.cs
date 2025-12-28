using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD__MY13_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 130;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Idle Speed Balance AD (MY13).panel";

	public override string Guid => "df1d35fa-b57f-41a2-ad37-db7cf26b86d8";

	public override string DisplayName => "Idle Speed Balance";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[10] { "DDEC13-DD13", "DDEC13-DD15", "DDEC13-DD16", "DDEC16-DD5", "DDEC16-DD8", "DDEC16-DD13", "DDEC16-DD15", "DDEC16-DD16", "DDEC13-MDEG 4-Cylinder Tier4", "DDEC13-MDEG 6-Cylinder Tier4" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)2;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)2;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[3]
	{
		new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status"),
		new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
		new Qualifier((QualifierTypes)1, "virtual", "coolantTemp")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[12]
	{
		"RT_SR02FA_set_TM_mode_Start", "RT_SR02FA_set_TM_mode_Stop", "RT_SR09A_Force_TMC_to_TMx_Mode_Start", "RT_SR066_Idle_Speed_Balance_Test_Start", "RT_SR0C5_Reset_ISC_Counter_in_ISC_Modul_Start", "DT_AS023_Engine_State", "DT_Idle_Speed_Balance_Values_Cylinder_1", "DT_Idle_Speed_Balance_Values_Cylinder_2", "DT_Idle_Speed_Balance_Values_Cylinder_3", "DT_Idle_Speed_Balance_Values_Cylinder_4",
		"DT_Idle_Speed_Balance_Values_Cylinder_5", "DT_Idle_Speed_Balance_Values_Cylinder_6"
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
