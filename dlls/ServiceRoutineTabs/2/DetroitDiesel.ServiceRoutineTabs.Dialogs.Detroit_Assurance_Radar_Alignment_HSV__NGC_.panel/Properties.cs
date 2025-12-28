using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment_HSV__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 138;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance Radar Alignment HSV (NGC).panel";

	public override string Guid => "519cdc0d-804d-438f-afef-f60080743cd3";

	public override string DisplayName => "Radar Alignment";

	public override IEnumerable<string> SupportedDevices => new string[3] { "HSV", "RDF02T", "XMC02T" };

	public override bool AllDevicesRequired => true;

	public override bool IsDialog => true;

	public override string Category => "Detroit Assurance";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[6]
	{
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Request_Results_Progress"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Request_Results_Routine_result"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Request_Results_Service_alignment_angle_azimuth"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Request_Results_Service_alignment_angle_elevation"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Start"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Stop")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[7]
	{
		new Qualifier((QualifierTypes)1, "RDF02T", "DT_Service_Justage_Progress_service_justage_progress"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)32, "RDF02T", "0FFFE9"),
		new Qualifier((QualifierTypes)32, "RDF02T", "0FFFF3"),
		new Qualifier((QualifierTypes)4, "RDF02T", "VertPos"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DrivingRadarAlignment_NGC_HSV"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[2] { "RDF02T", "XMC02T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
