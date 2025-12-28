using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 69;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Detroit Assurance Radar Alignment (NGC).panel";

	public override string Guid => "519cdc0d-804d-438f-afef-f60080743cd3";

	public override string DisplayName => "Radar Alignment";

	public override IEnumerable<string> SupportedDevices => new string[1] { "RDF02T" };

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
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Request_Results_Progress"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Request_Results_Routine_result"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Request_Results_Service_alignment_angle_azimuth"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Request_Results_Service_alignment_angle_elevation"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Start"),
		new Qualifier((QualifierTypes)2, "RDF02T", "RT_Service_Alignment_Stop")
	};

	public override IEnumerable<Qualifier> ProhibitedQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[43]
	{
		new Qualifier((QualifierTypes)1, "HSV", "DT_1733"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1734"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1736"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1737"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1738"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1739"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1740"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_Level_Control_Mode"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1742"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1743"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1744"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1745"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1746"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1754"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1755"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1756"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1822"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1823"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1824"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1825"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1826"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1827"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_5294"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_5296"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_5432"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1721"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1722"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1723"),
		new Qualifier((QualifierTypes)1, "HSV", "DT_1724"),
		new Qualifier((QualifierTypes)8, "HSV", "2838"),
		new Qualifier((QualifierTypes)8, "HSV", "2841"),
		new Qualifier((QualifierTypes)8, "HSV", "586"),
		new Qualifier((QualifierTypes)8, "HSV", "587"),
		new Qualifier((QualifierTypes)8, "HSV", "588"),
		new Qualifier((QualifierTypes)8, "HSV", "233"),
		new Qualifier((QualifierTypes)8, "HSV", "234"),
		new Qualifier((QualifierTypes)8, "HSV", "237"),
		new Qualifier((QualifierTypes)8, "HSV", "2901"),
		new Qualifier((QualifierTypes)8, "HSV", "2902"),
		new Qualifier((QualifierTypes)8, "HSV", "2903"),
		new Qualifier((QualifierTypes)8, "HSV", "2904"),
		new Qualifier((QualifierTypes)8, "HSV", "4304"),
		new Qualifier((QualifierTypes)2, "HSV", "RT_Enter_Dyno_Mode")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[5]
	{
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)1, "RDF02T", "DT_Service_Justage_Progress_service_justage_progress"),
		new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake"),
		new Qualifier((QualifierTypes)4, "RDF02T", "VertPos"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_DrivingRadarAlignment_NGC")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "RDF02T" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
