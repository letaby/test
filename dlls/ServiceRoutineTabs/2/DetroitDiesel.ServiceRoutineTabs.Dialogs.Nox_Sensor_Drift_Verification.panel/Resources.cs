using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Nox_Sensor_Drift_Verification.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Error_DPF_Regen_Stop_Service_Not_Found => ResourceManager.GetString("StringTable.Error_DPF_Regen_Stop_Service_Not_Found");

	internal static string Error_Failed_to_set_the_DEF_Valve => ResourceManager.GetString("StringTable.Error_Failed_to_set_the_DEF_Valve");

	internal static string Error_Failed_To_Stop_the_DPF_Regeneration => ResourceManager.GetString("StringTable.Error_Failed_To_Stop_the_DPF_Regeneration");

	internal static string Message_Stopping_the_DPF_Regen => ResourceManager.GetString("StringTable.Message_Stopping_the_DPF_Regen");

	internal static string Message_Starting_NOx_Sensor_Calibration_Drift_Test => ResourceManager.GetString("StringTable.Message_Starting_NOx_Sensor_Calibration_Drift_Test");

	internal static string Error_Closing => ResourceManager.GetString("StringTable.Error_Closing");

	internal static string Message_DewPoint_Outlet_Sensor_Off => ResourceManager.GetString("StringTable.Message_DewPoint_Outlet_Sensor_Off");

	internal static string Message_Setting_the_DEF_valve => ResourceManager.GetString("StringTable.Message_Setting_the_DEF_valve");

	internal static string Message_DewPoint_Outlet_Sensor_On => ResourceManager.GetString("StringTable.Message_DewPoint_Outlet_Sensor_On");

	internal static string Message_DewPoint_Inlet_Sensor_On => ResourceManager.GetString("StringTable.Message_DewPoint_Inlet_Sensor_On");

	internal static string Error_Failed_to_start_DPF_Regen => ResourceManager.GetString("StringTable.Error_Failed_to_start_DPF_Regen");

	internal static string WarningManagerMessage => ResourceManager.GetString("StringTable.WarningManagerMessage");

	internal static string Error_Failed_To_Reset_The_DEF_Valve => ResourceManager.GetString("StringTable.Error_Failed_To_Reset_The_DEF_Valve");

	internal static string Message_Results_NOx_Sensor_Values_Are_Consistent => ResourceManager.GetString("StringTable.Message_Results_NOx_Sensor_Values_Are_Consistent");

	internal static string Text_Null => ResourceManager.GetString("StringTable.Text_Null");

	internal static string Error_Failed_To_Get_MCM_Engine_Idle_Modify_Stop_service => ResourceManager.GetString("StringTable.Error_Failed_To_Get_MCM_Engine_Idle_Modify_Stop_service");

	internal static string Message_Test_Ready_To_Be_Run => ResourceManager.GetString("StringTable.Message_Test_Ready_To_Be_Run");

	internal static string Message_Restarting_The_DPF_Regen => ResourceManager.GetString("StringTable.Message_Restarting_The_DPF_Regen");

	internal static string Message_Waiting_For_Regen_To_Complete => ResourceManager.GetString("StringTable.Message_Waiting_For_Regen_To_Complete");

	internal static string Error_Disconnected => ResourceManager.GetString("StringTable.Error_Disconnected");

	internal static string Error_Failed_To_Set_Engine_Idle_Speed => ResourceManager.GetString("StringTable.Error_Failed_To_Set_Engine_Idle_Speed");

	internal static string MessageRunningRegenAgain => ResourceManager.GetString("StringTable.MessageRunningRegenAgain");

	internal static string Message_Checking_DewPoint_Sensors_At => ResourceManager.GetString("StringTable.Message_Checking_DewPoint_Sensors_At");

	internal static string Message_The_Instrument_Had_Value => ResourceManager.GetString("StringTable.Message_The_Instrument_Had_Value");

	internal static string Message_Results_Average_Sensor_Values => ResourceManager.GetString("StringTable.Message_Results_Average_Sensor_Values");

	internal static string Error_NOxSensorAverageZero => ResourceManager.GetString("StringTable.Error_NOxSensorAverageZero");

	internal static string Error_FailedServiceExecute => ResourceManager.GetString("StringTable.Error_FailedServiceExecute");

	internal static string Message_NOx_Inlet_Sensor_Is_Not_Ready => ResourceManager.GetString("StringTable.Message_NOx_Inlet_Sensor_Is_Not_Ready");

	internal static string Message_NOx_Inlet_Sensor_Is_Ready => ResourceManager.GetString("StringTable.Message_NOx_Inlet_Sensor_Is_Ready");

	internal static string Message_NOx_Outlet_Sensor_Is_Ready => ResourceManager.GetString("StringTable.Message_NOx_Outlet_Sensor_Is_Ready");

	internal static string Message_Setting_Engine_Idle_Speed => ResourceManager.GetString("StringTable.Message_Setting_Engine_Idle_Speed");

	internal static string Message_Cooling_Down_Engine => ResourceManager.GetString("StringTable.Message_Cooling_Down_Engine");

	internal static string Error_EngineIdleSpeedRampSharedProcedureNotFound => ResourceManager.GetString("StringTable.Error_EngineIdleSpeedRampSharedProcedureNotFound");

	internal static string Message_DewPoint_Inlet_Sensor_Off => ResourceManager.GetString("StringTable.Message_DewPoint_Inlet_Sensor_Off");

	internal static string Message_Test_Result_Test_Aborted => ResourceManager.GetString("StringTable.Message_Test_Result_Test_Aborted");

	internal static string Message_Test_Complete => ResourceManager.GetString("StringTable.Message_Test_Complete");

	internal static string Message_Results_NOx_Sensor_Values_Are_Not_Consistent => ResourceManager.GetString("StringTable.Message_Results_NOx_Sensor_Values_Are_Not_Consistent");

	internal static string Error_NOxSensorReadingsInvalid => ResourceManager.GetString("StringTable.Error_NOxSensorReadingsInvalid");

	internal static string Error_DewpointSensorsNotOn => ResourceManager.GetString("StringTable.Error_DewpointSensorsNotOn");

	internal static string Message_Starting_DPF_Regen => ResourceManager.GetString("StringTable.Message_Starting_DPF_Regen");

	internal static string Error_Test_Not_Ready_To_Be_Run => ResourceManager.GetString("StringTable.Error_Test_Not_Ready_To_Be_Run");

	internal static string Message_NOx_Sensor_Test => ResourceManager.GetString("StringTable.Message_NOx_Sensor_Test");

	internal static string Message_The_Sensors_Were_Ready_To_be_Compared => ResourceManager.GetString("StringTable.Message_The_Sensors_Were_Ready_To_be_Compared");

	internal static string Message_Canceled => ResourceManager.GetString("StringTable.Message_Canceled");

	internal static string Message_Test_Result_Failed => ResourceManager.GetString("StringTable.Message_Test_Result_Failed");

	internal static string Message_End_Of_Test => ResourceManager.GetString("StringTable.Message_End_Of_Test");

	internal static string Error_Message_Is => ResourceManager.GetString("StringTable.Error_Message_Is");

	internal static string Message_ResettingTheDEFValve => ResourceManager.GetString("StringTable.Message_ResettingTheDEFValve");

	internal static string Message_NOx_Outlet_Sensor_Is_Not_Ready => ResourceManager.GetString("StringTable.Message_NOx_Outlet_Sensor_Is_Not_Ready");

	internal static string Error_The_DPF_Regen_Ended_Before_The_Sensors_Were_Ready => ResourceManager.GetString("StringTable.Error_The_DPF_Regen_Ended_Before_The_Sensors_Were_Ready");

	internal static string Message_Results_Sensor_Value_Difference => ResourceManager.GetString("StringTable.Message_Results_Sensor_Value_Difference");

	internal static string Message_Test_Result_Passed => ResourceManager.GetString("StringTable.Message_Test_Result_Passed");

	internal static string Error_EngineIdleSpeedRampSharedProcedureCannotStart => ResourceManager.GetString("StringTable.Error_EngineIdleSpeedRampSharedProcedureCannotStart");

	internal static string Error_Failed_To_Reset_Engine_Idle => ResourceManager.GetString("StringTable.Error_Failed_To_Reset_Engine_Idle");

	internal static string Error_Failed_To_Get_CPC_Engine_Idle_Modify_Stop_service => ResourceManager.GetString("StringTable.Error_Failed_To_Get_CPC_Engine_Idle_Modify_Stop_service");
}
