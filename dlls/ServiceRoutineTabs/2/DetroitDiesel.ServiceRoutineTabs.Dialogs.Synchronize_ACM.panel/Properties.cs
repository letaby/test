using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Synchronize_ACM.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 76;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Synchronize ACM.panel";

	public override string Guid => "c7d783b8-a5af-4a00-9106-b42107f1f715";

	public override string DisplayName => "Synchronize ACM";

	public override IEnumerable<string> SupportedDevices => new string[3] { "ACM21T", "ACM301T", "MCM21T" };

	public override IEnumerable<string> SupportedEquipment => new string[2] { "DDEC16-DD13EURO5", "DDEC16-DD16EURO5" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)32, "ACM21T", "ED000D")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "ACM21T", "ACM301T", "MCM21T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[4] { "RT_Start_ECU_Marriage_Routine_Start", "RT_Start_ECU_Marriage_Routine_Request_Results_ECU_Marriage_Routine_Status_Byte", "RT_Start_ECU_Marriage_Routine_Stop", "RT_SR087_Delete_Non_Erasable_FC_Start" };

	public override IEnumerable<string> UserSourceSharedProcedureQualifierReferences => new string[1] { "SP_SecurityUnlock_ACM21T_UnlockXN" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
