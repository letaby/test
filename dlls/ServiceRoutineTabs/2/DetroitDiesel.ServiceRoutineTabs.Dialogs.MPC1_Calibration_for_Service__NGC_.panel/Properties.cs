using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC1_Calibration_for_Service__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 28;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\MPC1 Calibration for Service (NGC).panel";

	public override string Guid => "882a394f-bc7a-4553-aaec-a129138a9aef";

	public override string DisplayName => "MPC1 Camera Alignment - Service";

	public override IEnumerable<string> SupportedDevices => new string[1] { "MPC01T" };

	public override IEnumerable<string> ProhibitedEquipment => new string[1] { "Econic-Waste" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "Detroit Assurance";

	public override FilterTypes Filters => (FilterTypes)1;

	public override PanelUseCases UseCases => (PanelUseCases)8;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[7]
	{
		new Qualifier((QualifierTypes)1, "MPC01T", "DT_disc01_Camera_Calibration_Overall_Camera_Calibration_Status"),
		new Qualifier((QualifierTypes)1, "MPC01T", "DT_disc01_Camera_Calibration_Online_Camera_Calibration_Status"),
		new Qualifier((QualifierTypes)32, "MPC01T", "00FBED"),
		new Qualifier((QualifierTypes)64, "MPC01T", "RT_End_of_Line_Calibration_RequestResults_Static_Camera_Calibration_Result"),
		new Qualifier((QualifierTypes)1, "MPC01T", "DT_disc01_Camera_Calibration_Static_Camera_Calibration_Status"),
		new Qualifier((QualifierTypes)1, "MPC01T", "DT_disc02_LDW_Function_Data_LDW_Function_State"),
		new Qualifier((QualifierTypes)4, "MPC01T", "camera_height")
	};

	public override IEnumerable<string> UserSourceEcuReferences => new string[1] { "MPC01T" };

	public override IEnumerable<string> UserSourceInstrumentQualifierReferences => new string[3] { "DL_Static_Camera_Calibration_Data", "FN_HardReset", "DJ_SecurityAccess_Config_Dev" };

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
