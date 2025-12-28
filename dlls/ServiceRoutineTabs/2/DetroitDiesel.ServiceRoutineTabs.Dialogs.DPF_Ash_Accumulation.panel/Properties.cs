using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 145;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DPF Ash Accumulation.panel";

	public override string Guid => "c57cbaf5-c651-4e76-84a2-4925e6ac6f12";

	public override string DisplayName => "DPF Ash Accumulator";

	public override IEnumerable<string> SupportedDevices => new string[2] { "CPC2", "MCM" };

	public override IEnumerable<string> SupportedEquipment => new string[7] { "DDEC6-S60", "DDEC6-DD16", "DDEC6-DD15", "DDEC6-DD13", "DDEC6-MBE900", "DDEC6-MBE4000", "DDEC6-DD15EURO4" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "CPC2", "DT_AS056_DPF_Ash_Content_Mileage"),
		new Qualifier((QualifierTypes)4, "MCM", "e2p_dpf_ash_last_clean_dist")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "MCM", "CPC2" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[5] { "DT_AS078_Distance_till_Ash_Full", "RT_DPF_Ash_Mileage_Reset_Start", "RT_DPF_Ash_Mileage_Read_Request_Results_Status", "RT_SR014_SET_EOL_Default_Values_Start", "DT_AS045_Engine_Operating_Hours" };

	public override IEnumerable<string> UserSourceParameterQualifierReferences => new string[1] { "e2p_dpf_ash_last_clean_dist" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
