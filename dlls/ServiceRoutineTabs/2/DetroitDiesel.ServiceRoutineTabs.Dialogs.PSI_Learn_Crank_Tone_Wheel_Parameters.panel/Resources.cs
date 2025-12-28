using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_BatteryVoltageMustBeBetween1116Volts => ResourceManager.GetString("StringTable.Message_BatteryVoltageMustBeBetween1116Volts");

	internal static string Message_AcceleratorReleasedBeforeProcedureHadCompleted => ResourceManager.GetString("StringTable.Message_AcceleratorReleasedBeforeProcedureHadCompleted");

	internal static string Message_PSIMisfireDetection => ResourceManager.GetString("StringTable.Message_PSIMisfireDetection");

	internal static string Message_TheProcedureCanStart => ResourceManager.GetString("StringTable.Message_TheProcedureCanStart");

	internal static string Message_TransmissionMustBeInParkOrNeutral => ResourceManager.GetString("StringTable.Message_TransmissionMustBeInParkOrNeutral");

	internal static string Message_KeyRequestFailed => ResourceManager.GetString("StringTable.Message_KeyRequestFailed");

	internal static string Message_EngineSpeedMustBeAbove200RPM => ResourceManager.GetString("StringTable.Message_EngineSpeedMustBeAbove200RPM");

	internal static string MessageFormat_KeepTheIgnitionOffFor0Seconds => ResourceManager.GetString("StringTable.MessageFormat_KeepTheIgnitionOffFor0Seconds");

	internal static string Message_StoppingProcedure => ResourceManager.GetString("StringTable.Message_StoppingProcedure");

	internal static string Message_TheProcedureCanNotStart => ResourceManager.GetString("StringTable.Message_TheProcedureCanNotStart");

	internal static string Message_IgnitionTurnedOnBeforeLearnComplete => ResourceManager.GetString("StringTable.Message_IgnitionTurnedOnBeforeLearnComplete");

	internal static string Message_EngineCoolantTemperatureIsLow => ResourceManager.GetString("StringTable.Message_EngineCoolantTemperatureIsLow");

	internal static string Message_PressAndHoldAccelerator => ResourceManager.GetString("StringTable.Message_PressAndHoldAccelerator");

	internal static string Message_RoutineCompletedAutomaticallyReleaseAcceleratorTurnTheIgnitionOffFor15SecondsToFinalizeValues => ResourceManager.GetString("StringTable.Message_RoutineCompletedAutomaticallyReleaseAcceleratorTurnTheIgnitionOffFor15SecondsToFinalizeValues");

	internal static string Message_RoutineComplete => ResourceManager.GetString("StringTable.Message_RoutineComplete");

	internal static string MessageFormat_RoutineTerminatedManuallyTurnTheIgnitionOffFor15SecondsToFinalizeValues => ResourceManager.GetString("StringTable.MessageFormat_RoutineTerminatedManuallyTurnTheIgnitionOffFor15SecondsToFinalizeValues");

	internal static string Message_PSILearnCrankToneWheelParameters => ResourceManager.GetString("StringTable.Message_PSILearnCrankToneWheelParameters");

	internal static string MessageFormat_RoutineTimeoutProcessExceeded0Seconds => ResourceManager.GetString("StringTable.MessageFormat_RoutineTimeoutProcessExceeded0Seconds");

	internal static string Message_ContinueToHoldAccelerator => ResourceManager.GetString("StringTable.Message_ContinueToHoldAccelerator");

	internal static string Message_StartingProcedure => ResourceManager.GetString("StringTable.Message_StartingProcedure");

	internal static string Message_SetShortTermAdjustRequestFailed => ResourceManager.GetString("StringTable.Message_SetShortTermAdjustRequestFailed");

	internal static string Message_ErrorCouldNotDisableShortTermAdjustment => ResourceManager.GetString("StringTable.Message_ErrorCouldNotDisableShortTermAdjustment");

	internal static string Message_ParkingBrakeMustBeOn => ResourceManager.GetString("StringTable.Message_ParkingBrakeMustBeOn");

	internal static string Message_SeedRequestFailed => ResourceManager.GetString("StringTable.Message_SeedRequestFailed");

	internal static string Message_AcceleratorReleasedTerminatingProcedure => ResourceManager.GetString("StringTable.Message_AcceleratorReleasedTerminatingProcedure");
}
