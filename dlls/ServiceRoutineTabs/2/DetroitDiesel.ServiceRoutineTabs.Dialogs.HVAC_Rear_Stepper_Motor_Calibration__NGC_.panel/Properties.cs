using System;
using System.Collections.Generic;
using DetroitDiesel.Common;
using DetroitDiesel.Design;
using DetroitDiesel.Interfaces;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HVAC_Rear_Stepper_Motor_Calibration__NGC_.panel;

public class Properties : PanelProperties
{
	public override int FileVersion => 12;

	public override string FilePath => "C:\\Users\\Public\\Source\\drumroll\\drumroll\\Source\\\\Data\\Service Routine Tabs\\Dialogs\\HVAC Rear Stepper Motor Calibration (NGC).panel";

	public override string Guid => "7880ccf0-611e-4640-83f4-8b4ba3e800be";

	public override string DisplayName => "HVAC Rear Stepper Motor Calibration";

	public override IEnumerable<string> SupportedDevices => new string[1] { "HVAC_R01T" };

	public override bool AllDevicesRequired => false;

	public override bool IsDialog => true;

	public override string Category => "";

	public override FilterTypes Filters => (FilterTypes)137;

	public override PanelUseCases UseCases => (PanelUseCases)15;

	public override PanelTargets TargetHosts => (PanelTargets)1;

	public override int MinProductAccessLevel => 0;

	public override int MinDynamicAccessLevel => 0;

	public override IEnumerable<Qualifier> RequiredQualifiers => (IEnumerable<Qualifier>)(object)new Qualifier[1]
	{
		new Qualifier((QualifierTypes)2, "HVAC_R01T", "RT_Stepper_Motor_Calibration_Start")
	};

	public override FaultCondition RequiredFaultCondition => new FaultCondition((FaultConditionType)0, (IEnumerable<Qualifier>)(object)new Qualifier[0]);

	public override IEnumerable<Qualifier> DesignerQualifierReferences => (IEnumerable<Qualifier>)(object)new Qualifier[7]
	{
		new Qualifier((QualifierTypes)256, "Extension", "SP_HVACRStepperCal"),
		new Qualifier((QualifierTypes)1, "HVAC_R01T", "DT_Mix_Door_Actuator_Position_feedback_Mix_Door_Actuator_Position_feedback"),
		new Qualifier((QualifierTypes)64, "HVAC_R01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor"),
		new Qualifier((QualifierTypes)2, "HVAC_R01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor"),
		new Qualifier((QualifierTypes)256, "Extension", "SP_HVACRStepperCal"),
		new Qualifier((QualifierTypes)2, "HVAC_R01T", "RT_Stepper_Motor_Calibration_Start"),
		new Qualifier((QualifierTypes)2, "HVAC_R01T", "SES_Extended_P2s_CAN_ECU_max_physical")
	};

	public override IEnumerable<ServiceCall> DesignerServiceCallReferences => (IEnumerable<ServiceCall>)(object)new ServiceCall[3]
	{
		new ServiceCall("HVAC_R01T", "RT_Stepper_Motor_Calibration_Request_Results_Discharge_Temperature_Control_Motor", (IEnumerable<string>)new string[0]),
		new ServiceCall("HVAC_R01T", "RT_Stepper_Motor_Calibration_Start", (IEnumerable<string>)new string[0]),
		new ServiceCall("HVAC_R01T", "SES_Extended_P2s_CAN_ECU_max_physical", (IEnumerable<string>)new string[0])
	};

	public Properties(Type runtimeType)
		: base(runtimeType)
	{
	}
}
