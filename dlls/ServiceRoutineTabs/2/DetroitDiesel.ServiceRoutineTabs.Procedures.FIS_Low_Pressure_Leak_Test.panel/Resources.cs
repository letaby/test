using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TestFailedLeakWasDetected => ResourceManager.GetString("StringTable.Message_TestFailedLeakWasDetected");

	internal static string Message_TestCompleted => ResourceManager.GetString("StringTable.Message_TestCompleted");

	internal static string Message_EngineIsRunningTestCannotStart => ResourceManager.GetString("StringTable.Message_EngineIsRunningTestCannotStart");

	internal static string Message_TestWillBeAbortedPleaseTurnOffTheAirSupplyAndEnsureFuelHasBeenPurgedFromTheSystemAndPerformTheTestAgain => ResourceManager.GetString("StringTable.Message_TestWillBeAbortedPleaseTurnOffTheAirSupplyAndEnsureFuelHasBeenPurgedFromTheSystemAndPerformTheTestAgain");

	internal static string MessageFormat_Resetting0 => ResourceManager.GetString("StringTable.MessageFormat_Resetting0");

	internal static string Message_EngineIsNotRunningTestCanStart => ResourceManager.GetString("StringTable.Message_EngineIsNotRunningTestCanStart");

	internal static string MessageFormat_OpenedTheHCDoserValveFor0For1Seconds => ResourceManager.GetString("StringTable.MessageFormat_OpenedTheHCDoserValveFor0For1Seconds");

	internal static string Message_EngineSpeedCannotBeDetected => ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetected");

	internal static string Message_TestAbortedUnableToReadTheFuelCompensationPressureValue1 => ResourceManager.GetString("StringTable.Message_TestAbortedUnableToReadTheFuelCompensationPressureValue1");

	internal static string MessageFormat_ClosedTheHCDoserValveFor0Minutes => ResourceManager.GetString("StringTable.MessageFormat_ClosedTheHCDoserValveFor0Minutes");

	internal static string MessageFormat_TheFinalFuelCompensationPressureObservedWas0 => ResourceManager.GetString("StringTable.MessageFormat_TheFinalFuelCompensationPressureObservedWas0");

	internal static string Message_WaitingFor20Seconds => ResourceManager.GetString("StringTable.Message_WaitingFor20Seconds");

	internal static string Message_CheckInProgress => ResourceManager.GetString("StringTable.Message_CheckInProgress");

	internal static string Message_RequestingStopFuelCutoffValve => ResourceManager.GetString("StringTable.Message_RequestingStopFuelCutoffValve");

	internal static string Message_TestAbortedDeviceWentOffline => ResourceManager.GetString("StringTable.Message_TestAbortedDeviceWentOffline");

	internal static string Message_WaitingFor45Seconds => ResourceManager.GetString("StringTable.Message_WaitingFor45Seconds");

	internal static string MessageFormat_Setting0To1 => ResourceManager.GetString("StringTable.MessageFormat_Setting0To1");

	internal static string Message_RequestingOpenHCDoserValve => ResourceManager.GetString("StringTable.Message_RequestingOpenHCDoserValve");

	internal static string Message_UnableToRequestStopFuelCutoffRoutine => ResourceManager.GetString("StringTable.Message_UnableToRequestStopFuelCutoffRoutine");

	internal static string Message_RequestingStopHCDoserValve => ResourceManager.GetString("StringTable.Message_RequestingStopHCDoserValve");

	internal static string Message_TestWillBeAbortedPleaseTurnOffTheAirSupplyAndEnsureFuelHasBeenPurgedFromTheSystemAndPerformTheTestAgain1 => ResourceManager.GetString("StringTable.Message_TestWillBeAbortedPleaseTurnOffTheAirSupplyAndEnsureFuelHasBeenPurgedFromTheSystemAndPerformTheTestAgain1");

	internal static string Message_LowPressureLeakDetectionCheckStarted => ResourceManager.GetString("StringTable.Message_LowPressureLeakDetectionCheckStarted");

	internal static string Message_WaitingFor10Seconds => ResourceManager.GetString("StringTable.Message_WaitingFor10Seconds");

	internal static string Message_TestAbortedUnableToReadTheFuelCompensationPressureValue => ResourceManager.GetString("StringTable.Message_TestAbortedUnableToReadTheFuelCompensationPressureValue");

	internal static string Message_TestPassedLeakWasNotDetected => ResourceManager.GetString("StringTable.Message_TestPassedLeakWasNotDetected");

	internal static string Message_RequestingCloseHCDoserValve => ResourceManager.GetString("StringTable.Message_RequestingCloseHCDoserValve");

	internal static string Message_UnableToRequestStopFuelCutoffAndHCDoserValveRoutines => ResourceManager.GetString("StringTable.Message_UnableToRequestStopFuelCutoffAndHCDoserValveRoutines");

	internal static string MessageFormat_TheIntialFuelCompensationPressureObservedWas0 => ResourceManager.GetString("StringTable.MessageFormat_TheIntialFuelCompensationPressureObservedWas0");

	internal static string Message_RequestingOpenFuelCutoffValve => ResourceManager.GetString("StringTable.Message_RequestingOpenFuelCutoffValve");

	internal static string Message_TestAbortedUserCanceledTheTest => ResourceManager.GetString("StringTable.Message_TestAbortedUserCanceledTheTest");
}
