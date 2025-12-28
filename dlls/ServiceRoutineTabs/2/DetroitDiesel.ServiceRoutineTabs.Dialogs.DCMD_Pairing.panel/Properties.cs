using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DCMD_Pairing.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 55;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DCMD Pairing.panel";

	public override string Guid => "79254125-0d0b-4b96-ae88-284b25e26ca4";

	public override string DisplayName => "DCMD Keyfob Pairing";

	public override IEnumerable<string> SupportedDevices => new string[1] { "DCMD02T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)2, "DCMD02T", "DL_ID_Keyfob_IDs"),
		new Qualifier((QualifierTypes)8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_1"),
		new Qualifier((QualifierTypes)8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_2"),
		new Qualifier((QualifierTypes)8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_3"),
		new Qualifier((QualifierTypes)8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_4"),
		new Qualifier((QualifierTypes)8, "DCMD02T", "DT_STO_ID_Keyfob_IDs_KeyID_5"),
		new Qualifier((QualifierTypes)2, "DCMD02T", "DJ_SecurityAccess_Config_EOL"),
		new Qualifier((QualifierTypes)2, "DCMD02T", "FN_HardReset_physical")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[3]
	{
		new Qualifier((QualifierTypes)1, "DCMD02T", "DT_RKE_Button_3_IN_RKE_Button3"),
		new Qualifier((QualifierTypes)1, "DCMD02T", "DT_RKE_Button_1_IN_RKE_Button1"),
		new Qualifier((QualifierTypes)1, "DCMD02T", "DT_RKE_Button_2_IN_RKE_Button2")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "DCMD02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[8] { "DT_STO_ID_Keyfob_IDs_KeyID_1", "DT_STO_ID_Keyfob_IDs_KeyID_2", "DT_STO_ID_Keyfob_IDs_KeyID_3", "DT_STO_ID_Keyfob_IDs_KeyID_4", "DT_STO_ID_Keyfob_IDs_KeyID_5", "DL_ID_Remote_SC", "DL_ID_Keyfob_IDs", "FN_HardReset_physical" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
