using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC3_Calibration.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 49;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\MPC3 Calibration.panel";

	public override string Guid => "58aaec29-d99f-40d2-9898-3248280abf01";

	public override string DisplayName => "MPC3 Calibration";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MPC03T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Detroit Assurance";

	public override FilterTypes Filters => (FilterTypes)9;

	public override PanelUseCases UseCases => (PanelUseCases)10;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[11]
	{
		new Qualifier((QualifierTypes)256, "Extension", "SP_MPC3Calibration"),
		new Qualifier((QualifierTypes)64, "MPC03T", "RT_Initial_Online_Calibration_Request_Results_IOCAL_Routine_Status"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)4, "MPC03T", "Camera_Height_Over_Ground"),
		new Qualifier((QualifierTypes)1, "MPC03T", "DT_Initial_Calibration_Misalignment_Information_calibrationStatus"),
		new Qualifier((QualifierTypes)1, "MPC03T", "DT_Initial_Calibration_Misalignment_Information_errorDetails"),
		new Qualifier((QualifierTypes)2, "MPC03T", "RT_Initial_Online_Calibration_Request_Results_IOCAL_Routine_Status"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_MPC3Calibration"),
		new Qualifier((QualifierTypes)2, "MPC03T", "RT_Initial_Online_Calibration_Start"),
		new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
		new Qualifier((QualifierTypes)2, "MPC03T", "RT_Initial_Online_Calibration_Stop")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("MPC03T", "RT_Initial_Online_Calibration_Request_Results_IOCAL_Routine_Status", (IEnumerable<string>)new string[0]),
		new ServiceCall("MPC03T", "RT_Initial_Online_Calibration_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("MPC03T", "RT_Initial_Online_Calibration_Stop", (IEnumerable<string>)new string[0])
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MPC03T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[1] { "RT_Initial_Online_Calibration_Request_Results_Progress_in_Percentage" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
