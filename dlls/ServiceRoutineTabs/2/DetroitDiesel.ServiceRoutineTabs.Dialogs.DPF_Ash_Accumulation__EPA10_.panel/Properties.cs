using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__EPA10_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 122;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\DPF Ash Accumulation (EPA10).panel";

	public override string Guid => "c57cbaf5-c651-4e76-84a2-4925e6ac6f12";

	public override string DisplayName => "DPF Ash Accumulator";

	public override IEnumerable<string> SupportedDevices => new string[3] { "ACM02T", "CPC02T", "MCM02T" };

	public override IEnumerable<string> SupportedEquipment => new string[3] { "DDEC10-DD13", "DDEC10-DD15", "DDEC10-DD16" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Aftertreatment";

	public override FilterTypes Filters => (FilterTypes)34;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)3;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[3]
	{
		new Qualifier((QualifierTypes)2, "CPC02T", "RT_DPF_Ash_Volume_Reset_Start"),
		new Qualifier((QualifierTypes)2, "CPC02T", "RT_DPF_Ash_Volume_Read_Request_Results_Status"),
		new Qualifier((QualifierTypes)2, "ACM02T", "RT_Ash_Volume_Ratio_Update_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[2]
	{
		new Qualifier((QualifierTypes)1, "CPC02T", "DT_AS052_DPF_Ash_Volume"),
		new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS109_Ash_Filter_Full_Volume")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[3] { "ACM02T", "CPC02T", "MCM02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[5] { "DT_AS109_Ash_Filter_Full_Volume", "RT_DPF_Ash_Volume_Reset_Start", "RT_DPF_Ash_Volume_Read_Request_Results_Status", "RT_Ash_Volume_Ratio_Update_Start", "DT_AS045_Engine_Operating_Hours" };

	public override IEnumerable<string> UserSourceParameterQualifierReferences => new string[1] { "ATD_Hardware_Type" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
