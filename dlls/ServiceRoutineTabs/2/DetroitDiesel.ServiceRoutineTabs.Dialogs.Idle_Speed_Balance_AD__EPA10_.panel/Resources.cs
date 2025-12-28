using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD__EPA10_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_MCM2IsConnectedButEngineTypeIsNotSupported => ResourceManager.GetString("StringTable.Message_MCM2IsConnectedButEngineTypeIsNotSupported");

	internal static string Message_IdleSpeedBalanceTest => ResourceManager.GetString("StringTable.Message_IdleSpeedBalanceTest");

	internal static string Message_StartingTest => ResourceManager.GetString("StringTable.Message_StartingTest");

	internal static string MessageFormat_TheConnectedMCM2DoesNotSupportTheServiceRoutine0 => ResourceManager.GetString("StringTable.MessageFormat_TheConnectedMCM2DoesNotSupportTheServiceRoutine0");

	internal static string Message_Cylinder => ResourceManager.GetString("StringTable.Message_Cylinder");

	internal static string Message_EngineIsAtIdle => ResourceManager.GetString("StringTable.Message_EngineIsAtIdle");

	internal static string MessageFormat_CoolantTemperatureMustBeAtLeast0 => ResourceManager.GetString("StringTable.MessageFormat_CoolantTemperatureMustBeAtLeast0");

	internal static string Message_Unknown => ResourceManager.GetString("StringTable.Message_Unknown");

	internal static string Message_FuelAndCoolantTemperaturesAreInRange => ResourceManager.GetString("StringTable.Message_FuelAndCoolantTemperaturesAreInRange");

	internal static string Message_CannotDetectIfEngineIsStarted => ResourceManager.GetString("StringTable.Message_CannotDetectIfEngineIsStarted");

	internal static string Message_MCM2IsNotConnected => ResourceManager.GetString("StringTable.Message_MCM2IsNotConnected");

	internal static string Message_TestWasNotSuccessful => ResourceManager.GetString("StringTable.Message_TestWasNotSuccessful");

	internal static string Message_TestCompleteCloseThisWindowToContinueTroubleshooting => ResourceManager.GetString("StringTable.Message_TestCompleteCloseThisWindowToContinueTroubleshooting");

	internal static string Message_TheEngineIsNotAtIdle => ResourceManager.GetString("StringTable.Message_TheEngineIsNotAtIdle");

	internal static string Message_TestWasSuccessful => ResourceManager.GetString("StringTable.Message_TestWasSuccessful");

	internal static string Message_ErrorsOccurredDuringTheTest => ResourceManager.GetString("StringTable.Message_ErrorsOccurredDuringTheTest");

	internal static string Message_WhileTestingCylinder => ResourceManager.GetString("StringTable.Message_WhileTestingCylinder");

	internal static string Message_MCM2IsConnectedAndEngineTypeIsSupported => ResourceManager.GetString("StringTable.Message_MCM2IsConnectedAndEngineTypeIsSupported");

	internal static string Message_MCM2IsConnectedButIsBusy => ResourceManager.GetString("StringTable.Message_MCM2IsConnectedButIsBusy");

	internal static string MessageFormat_Cylinder0MayBeInFaultRN => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0MayBeInFaultRN");

	internal static string MessageFormat_FuelTemperatureMustBeAtLeast0 => ResourceManager.GetString("StringTable.MessageFormat_FuelTemperatureMustBeAtLeast0");

	internal static string Message_VehicleStatusIsOK => ResourceManager.GetString("StringTable.Message_VehicleStatusIsOK");

	internal static string Message_EngineIsStoppedStartTheEngineToProceed => ResourceManager.GetString("StringTable.Message_EngineIsStoppedStartTheEngineToProceed");

	internal static string Message_TheTestCompletedSuccessfully => ResourceManager.GetString("StringTable.Message_TheTestCompletedSuccessfully");

	internal static string Message_Cylinder1 => ResourceManager.GetString("StringTable.Message_Cylinder1");

	internal static string MessageFormat_Cylinder0MightBeCompensatingForCylinder1RN => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0MightBeCompensatingForCylinder1RN");

	internal static string Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON => ResourceManager.GetString("StringTable.Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON");
}
