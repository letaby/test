using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_IdleSpeedBalanceTest => ResourceManager.GetString("StringTable.Message_IdleSpeedBalanceTest");

	internal static string MessageFormat_TheConnectedMCMDoesNotSupportTheServiceRoutine0 => ResourceManager.GetString("StringTable.MessageFormat_TheConnectedMCMDoesNotSupportTheServiceRoutine0");

	internal static string Message_StartingTest => ResourceManager.GetString("StringTable.Message_StartingTest");

	internal static string Message_MCMIsConnectedButEngineTypeIsNotSupported => ResourceManager.GetString("StringTable.Message_MCMIsConnectedButEngineTypeIsNotSupported");

	internal static string Message_MCMIsConnectedAndEngineTypeIsSupported => ResourceManager.GetString("StringTable.Message_MCMIsConnectedAndEngineTypeIsSupported");

	internal static string Message_Cylinder => ResourceManager.GetString("StringTable.Message_Cylinder");

	internal static string Message_EngineIsAtIdle => ResourceManager.GetString("StringTable.Message_EngineIsAtIdle");

	internal static string MessageFormat_CoolantTemperatureMustBeAtLeast0 => ResourceManager.GetString("StringTable.MessageFormat_CoolantTemperatureMustBeAtLeast0");

	internal static string Message_MCMIsConnectedButIsBusy => ResourceManager.GetString("StringTable.Message_MCMIsConnectedButIsBusy");

	internal static string Message_Unknown => ResourceManager.GetString("StringTable.Message_Unknown");

	internal static string Message_FuelAndCoolantTemperaturesAreInRange => ResourceManager.GetString("StringTable.Message_FuelAndCoolantTemperaturesAreInRange");

	internal static string Message_CannotDetectIfEngineIsStarted => ResourceManager.GetString("StringTable.Message_CannotDetectIfEngineIsStarted");

	internal static string Message_MCMIsNotConnected => ResourceManager.GetString("StringTable.Message_MCMIsNotConnected");

	internal static string Message_TestWasNotSuccessful => ResourceManager.GetString("StringTable.Message_TestWasNotSuccessful");

	internal static string Message_TheEngineIsNotAtIdle => ResourceManager.GetString("StringTable.Message_TheEngineIsNotAtIdle");

	internal static string Message_TestWasSuccessful => ResourceManager.GetString("StringTable.Message_TestWasSuccessful");

	internal static string Message_ErrorsOccurredDuringTheTest => ResourceManager.GetString("StringTable.Message_ErrorsOccurredDuringTheTest");

	internal static string Message_WhileTestingCylinder => ResourceManager.GetString("StringTable.Message_WhileTestingCylinder");

	internal static string Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON0 => ResourceManager.GetString("StringTable.Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON0");

	internal static string MessageFormat_FuelTemperatureMustBeAtLeast0 => ResourceManager.GetString("StringTable.MessageFormat_FuelTemperatureMustBeAtLeast0");

	internal static string Message_VehicleStatusIsOK => ResourceManager.GetString("StringTable.Message_VehicleStatusIsOK");

	internal static string Message_EngineIsStoppedStartTheEngineToProceed => ResourceManager.GetString("StringTable.Message_EngineIsStoppedStartTheEngineToProceed");

	internal static string Message_TheTestCompletedSuccessfully => ResourceManager.GetString("StringTable.Message_TheTestCompletedSuccessfully");

	internal static string Message_Cylinder1 => ResourceManager.GetString("StringTable.Message_Cylinder1");
}
