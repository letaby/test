using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_MR2_for_Reprogramming.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 85;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Unlock MR2 for Reprogramming.panel";

	public override string Guid => "dfc5ce6d-f348-4e46-9ebb-a5848d1dccab";

	public override string DisplayName => "Unlock ECU for Reprogramming";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MR201T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)3;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MR201T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[7] { "DJ_Get_ECU_Serial_Number", "DT_STO_ID_AUT64_Challenge_challenge", "DT_STO_ID_Transponder_Code_TranspCode", "DT_STO_ID_Number_of_Transponder_Code_number_of_TPC_s", "DT_STO_ID_Read_Fuelmap_Status_Fuelmap_Status", "DJ_SecurityAccess_Routine_1", "RT_SR0401_Immobilizer_Classic_X_Routine_Dec_Start_State" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
