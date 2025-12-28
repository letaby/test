using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Calibration__45X_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 43;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\Side Radar Calibration (45X).panel";

	public override string Guid => "58aaec29-d99f-40d2-9898-3248280abf02";

	public override string DisplayName => "Side Radar Calibration";

	public override IEnumerable<string> SupportedDevices => new string[4] { "SRRFL02T", "SRRFR02T", "SRRL02T", "SRRR02T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Detroit Assurance";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[33]
	{
		new Qualifier((QualifierTypes)64, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress"),
		new Qualifier((QualifierTypes)64, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress"),
		new Qualifier((QualifierTypes)64, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress"),
		new Qualifier((QualifierTypes)64, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress"),
		new Qualifier((QualifierTypes)64, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus"),
		new Qualifier((QualifierTypes)64, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus"),
		new Qualifier((QualifierTypes)64, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus"),
		new Qualifier((QualifierTypes)64, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRRCalibration"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRFRCalibration"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRFLCalibration"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRLCalibration"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)2, "SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRRCalibration"),
		new Qualifier((QualifierTypes)2, "SRRR02T", "RT_DynamicCalibrationSDA_Start"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)2, "SRRR02T", "RT_DynamicCalibrationSDA_Stop"),
		new Qualifier((QualifierTypes)2, "SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRLCalibration"),
		new Qualifier((QualifierTypes)2, "SRRL02T", "RT_DynamicCalibrationSDA_Start"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)2, "SRRL02T", "RT_DynamicCalibrationSDA_Stop"),
		new Qualifier((QualifierTypes)2, "SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRFRCalibration"),
		new Qualifier((QualifierTypes)2, "SRRFR02T", "RT_DynamicCalibrationSDA_Start"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)2, "SRRFR02T", "RT_DynamicCalibrationSDA_Stop"),
		new Qualifier((QualifierTypes)2, "SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_SRRFLCalibration"),
		new Qualifier((QualifierTypes)2, "SRRFL02T", "RT_DynamicCalibrationSDA_Start"),
		new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed"),
		new Qualifier((QualifierTypes)2, "SRRFL02T", "RT_DynamicCalibrationSDA_Stop")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[12]
	{
		new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRR02T", "RT_DynamicCalibrationSDA_Stop", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRL02T", "RT_DynamicCalibrationSDA_Stop", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRFR02T", "RT_DynamicCalibrationSDA_Stop", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Request_Results_routineInfo", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("SRRFL02T", "RT_DynamicCalibrationSDA_Stop", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[4] { "SRRR02T", "SRRL02T", "SRRFR02T", "SRRFL02T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationStatus", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationProgress", "RT_DynamicCalibrationSDA_Request_Results_DynamicCalibrationOutOfProfileCause" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
