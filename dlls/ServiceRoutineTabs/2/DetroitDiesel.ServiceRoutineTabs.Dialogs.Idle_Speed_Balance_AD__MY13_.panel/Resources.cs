using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_RunningTestFor0Seconds => ResourceManager.GetString("StringTable.MessageFormat_RunningTestFor0Seconds");

	internal static string Message_MCM21TIsConnectedAndEngineTypeIsSupported => ResourceManager.GetString("StringTable.Message_MCM21TIsConnectedAndEngineTypeIsSupported");

	internal static string Message_StartingTest => ResourceManager.GetString("StringTable.Message_StartingTest");

	internal static string Message_WarningToEnsureProperResultsAllActiveFaultCodesShouldBeDiagnosedPriorRunningAnIdleSpeedBalanceTest => ResourceManager.GetString("StringTable.Message_WarningToEnsureProperResultsAllActiveFaultCodesShouldBeDiagnosedPriorRunningAnIdleSpeedBalanceTest");

	internal static string Message_TestFailed => ResourceManager.GetString("StringTable.Message_TestFailed");

	internal static string Message_Cylinder => ResourceManager.GetString("StringTable.Message_Cylinder");

	internal static string Message_EngineIsAtIdle => ResourceManager.GetString("StringTable.Message_EngineIsAtIdle");

	internal static string MessageFormat_CoolantTemperatureMustBeAtLeast0 => ResourceManager.GetString("StringTable.MessageFormat_CoolantTemperatureMustBeAtLeast0");

	internal static string Message_Unknown => ResourceManager.GetString("StringTable.Message_Unknown");

	internal static string Message_FuelAndCoolantTemperaturesAreInRange => ResourceManager.GetString("StringTable.Message_FuelAndCoolantTemperaturesAreInRange");

	internal static string Message_CannotDetectIfEngineIsStarted => ResourceManager.GetString("StringTable.Message_CannotDetectIfEngineIsStarted");

	internal static string Message_TestPassed => ResourceManager.GetString("StringTable.Message_TestPassed");

	internal static string Message_TestWasNotRan => ResourceManager.GetString("StringTable.Message_TestWasNotRan");

	internal static string Message_TestWasNotSuccessful => ResourceManager.GetString("StringTable.Message_TestWasNotSuccessful");

	internal static string Message_MCM21TIsNotConnected => ResourceManager.GetString("StringTable.Message_MCM21TIsNotConnected");

	internal static string MessageFormat_ClearingFaults0Seconds => ResourceManager.GetString("StringTable.MessageFormat_ClearingFaults0Seconds");

	internal static string Message_TestCompleteCloseThisWindowToContinueTroubleshooting => ResourceManager.GetString("StringTable.Message_TestCompleteCloseThisWindowToContinueTroubleshooting");

	internal static string Message_CannotSetThermalManagementMode => ResourceManager.GetString("StringTable.Message_CannotSetThermalManagementMode");

	internal static string MessageFormat_TheConnectedMCM21TDoesNotSupportTheServiceRoutine => ResourceManager.GetString("StringTable.MessageFormat_TheConnectedMCM21TDoesNotSupportTheServiceRoutine");

	internal static string Message_ResettingCounters => ResourceManager.GetString("StringTable.Message_ResettingCounters");

	internal static string Message_TheEngineIsNotAtIdle => ResourceManager.GetString("StringTable.Message_TheEngineIsNotAtIdle");

	internal static string Message_TestWasSuccessful => ResourceManager.GetString("StringTable.Message_TestWasSuccessful");

	internal static string Message_ErrorsOccurredDuringTheTest => ResourceManager.GetString("StringTable.Message_ErrorsOccurredDuringTheTest");

	internal static string Message_WhileTestingCylinder => ResourceManager.GetString("StringTable.Message_WhileTestingCylinder");

	internal static string Message_TheISCCounterHasBeenResetTheTestContinues => ResourceManager.GetString("StringTable.Message_TheISCCounterHasBeenResetTheTestContinues");

	internal static string Message_CannotStopThermalManagementMode => ResourceManager.GetString("StringTable.Message_CannotStopThermalManagementMode");

	internal static string MessageFormat_Cylinder0MayBeInFaultRN => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0MayBeInFaultRN");

	internal static string Message_ThermalManagementModeStopped => ResourceManager.GetString("StringTable.Message_ThermalManagementModeStopped");

	internal static string MessageFormat_FuelTemperatureMustBeAtLeast0 => ResourceManager.GetString("StringTable.MessageFormat_FuelTemperatureMustBeAtLeast0");

	internal static string Message_VehicleStatusIsOK => ResourceManager.GetString("StringTable.Message_VehicleStatusIsOK");

	internal static string Message_ThermalManagementModeSet => ResourceManager.GetString("StringTable.Message_ThermalManagementModeSet");

	internal static string Message_EngineIsStoppedStartTheEngineToProceed => ResourceManager.GetString("StringTable.Message_EngineIsStoppedStartTheEngineToProceed");

	internal static string Message_MCM21TIsConnectedButIsBusy => ResourceManager.GetString("StringTable.Message_MCM21TIsConnectedButIsBusy");

	internal static string Message_MCM21TIsConnectedButEngineTypeIsNotSupported => ResourceManager.GetString("StringTable.Message_MCM21TIsConnectedButEngineTypeIsNotSupported");

	internal static string Message_Cylinder1 => ResourceManager.GetString("StringTable.Message_Cylinder1");

	internal static string MessageFormat_Cylinder0MightBeCompensatingForCylinder1RN => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0MightBeCompensatingForCylinder1RN");

	internal static string Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON => ResourceManager.GetString("StringTable.Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON");

	internal static string Message_ResettingTheISCCounterServiceRoutineFailedToRun => ResourceManager.GetString("StringTable.Message_ResettingTheISCCounterServiceRoutineFailedToRun");
}
