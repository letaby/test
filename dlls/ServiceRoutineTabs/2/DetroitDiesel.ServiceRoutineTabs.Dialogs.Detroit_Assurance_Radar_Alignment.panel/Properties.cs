using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 37;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance Radar Alignment.panel";

	public override string Guid => "519cdc0d-804d-438f-afef-f60080743cd3";

	public override string DisplayName => "Radar Alignment";

	public override IEnumerable<string> SupportedDevices => new string[1] { "RDF01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Detroit Assurance";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[6]
	{
		new Qualifier((QualifierTypes)2, "RDF01T", "RT_Service_Justage_Request_Results_Progress"),
		new Qualifier((QualifierTypes)2, "RDF01T", "RT_Service_Justage_Start"),
		new Qualifier((QualifierTypes)2, "RDF01T", "RT_Service_Justage_Stop"),
		new Qualifier((QualifierTypes)2, "RDF01T", "RT_Service_Justage_Request_Results_Routine_Result_State"),
		new Qualifier((QualifierTypes)2, "RDF01T", "DJ_SecurityAccess_RepairShop"),
		new Qualifier((QualifierTypes)2, "RDF01T", "SES_StandStill_P2_CAN_ECU_max_physical")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[4]
	{
		new Qualifier((QualifierTypes)256, "Extension", "SP_DrivingRadarAlignment"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "RDF01T", "DT_Service_Justage_Progress_service_justage_progress"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake")
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
