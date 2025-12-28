using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clear_Non_Erasable_Fault_Codes.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 46;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Clear Non Erasable Fault Codes.panel";

	public override string Guid => "57f1e09c-d30a-4f4a-bdb2-61b1b93d59ab";

	public override string DisplayName => "Clear Non Erasable Fault code(s)";

	public override IEnumerable<string> SupportedDevices => new string[3] { "ACM21T", "ACM301T", "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[14]
	{
		"DDEC13-MDEG 4-Cylinder Tier4", "DDEC13-MDEG 6-Cylinder Tier4", "DDEC13-DD11 Tier4", "DDEC13-DD13 Tier4", "DDEC13-DD16 Tier4", "DDEC16-MDEG 4-Cylinder StageV", "DDEC16-MDEG 6-Cylinder StageV", "DDEC16-DD13EURO5", "DDEC20-MDEG 4-Cylinder StageV", "DDEC20-MDEG 6-Cylinder StageV",
		"DDEC20-DD11 StageV", "DDEC20-DD13 StageV", "DDEC20-DD16 StageV", "DDEC16-DD16EURO5"
	};

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "ACM21T", "ACM301T", "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[6] { "DJ_Read_AUT64_VeDoc_Input", "DNU_Config_3_Seed_Request", "DJ_ZKB_Config_3_WriteKeyFingerprint", "RT_SR087_Delete_Non_Erasable_FC_Start", "DJ_Get_ECU_Serial_Number", "DT_STO_Read_look_up_table_1_sys_look_up_table1_1m" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
