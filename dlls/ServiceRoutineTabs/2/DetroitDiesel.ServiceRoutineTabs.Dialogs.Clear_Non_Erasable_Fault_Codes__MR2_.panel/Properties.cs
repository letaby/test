using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clear_Non_Erasable_Fault_Codes__MR2_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 45;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Clear Non Erasable Fault Codes (MR2).panel";

	public override string Guid => "57f1e09c-d30a-4f4a-bdb2-61b1b93d59ab";

	public override string DisplayName => "Clear Non Erasable Fault code(s)";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MR201T" };

	public override IEnumerable<string> SupportedEquipment => new string[2] { "MBE-MBE4000", "MBE-MBE900" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MR201T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DJ_Get_ECU_Serial_Number", "DT_STO_ID_AUT64_Challenge_challenge", "RT_SR0903_EGR_Function_lock_Start_State_Byte" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
