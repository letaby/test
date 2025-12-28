using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Right_Calibration__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 33;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Side Radar Right Calibration (NGC).panel";

	public override string Guid => "58aaec29-d99f-40d2-9898-3248280abf02";

	public override string DisplayName => "Side Radar Right Calibration";

	public override IEnumerable<string> SupportedDevices => new string[1] { "SRRR01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Detroit Assurance";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[8]
	{
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRRCalibration"),
		new Qualifier((QualifierTypes)64, "SRRR01T", "RT_Service_Alignment_Azimuth_Request_Results_Routine_Status"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)2, "SRRR01T", "RT_Service_Alignment_Azimuth_Request_Results_Routine_Status"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRRCalibration"),
		new Qualifier((QualifierTypes)2, "SRRR01T", "RT_Service_Alignment_Azimuth_Start"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)2, "SRRR01T", "RT_Service_Alignment_Azimuth_Stop")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("SRRR01T", "RT_Service_Alignment_Azimuth_Request_Results_Routine_Status", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRR01T", "RT_Service_Alignment_Azimuth_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRR01T", "RT_Service_Alignment_Azimuth_Stop", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "SRRR01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "RT_Service_Alignment_Azimuth_Request_Results_Routine_Progress" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
