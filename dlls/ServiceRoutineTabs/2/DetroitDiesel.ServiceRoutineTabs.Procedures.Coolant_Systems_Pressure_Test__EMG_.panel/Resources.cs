using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Coolant_Systems_Pressure_Test__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_WaitUntilTemperatureIs50DegCOr10MinutesHasPassed => ResourceManager.GetString("StringTable.Message_WaitUntilTemperatureIs50DegCOr10MinutesHasPassed");

	internal static string Message_WaitingUntilTemperatureReaches38DegCOrFor0Seconds => ResourceManager.GetString("StringTable.Message_WaitingUntilTemperatureReaches38DegCOrFor0Seconds");

	internal static string Message_EDriveDeaerationStopped => ResourceManager.GetString("StringTable.Message_EDriveDeaerationStopped");

	internal static string Message_EDriveDeaearationFailed => ResourceManager.GetString("StringTable.Message_EDriveDeaearationFailed");

	internal static string Message_SetBattery3x2ValveTo0 => ResourceManager.GetString("StringTable.Message_SetBattery3x2ValveTo0");

	internal static string Message_TemperatureIs01 => ResourceManager.GetString("StringTable.Message_TemperatureIs01");

	internal static string Message_PTCsAreOff => ResourceManager.GetString("StringTable.Message_PTCsAreOff");

	internal static string Message_ExecutingTheDeaerationEDrvieRoutine => ResourceManager.GetString("StringTable.Message_ExecutingTheDeaerationEDrvieRoutine");

	internal static string Message_FailedTurningOffThePTCs => ResourceManager.GetString("StringTable.Message_FailedTurningOffThePTCs");

	internal static string Message_FailedSettingBatteryCircuitCoolantPumpsTo60 => ResourceManager.GetString("StringTable.Message_FailedSettingBatteryCircuitCoolantPumpsTo60");

	internal static string Message_EDriveWaitingFor0Seconds => ResourceManager.GetString("StringTable.Message_EDriveWaitingFor0Seconds");

	internal static string Message_FailedSettingBattery3x2ValveTo0 => ResourceManager.GetString("StringTable.Message_FailedSettingBattery3x2ValveTo0");

	internal static string Message_EDriveCoolantTestHasFinished => ResourceManager.GetString("StringTable.Message_EDriveCoolantTestHasFinished");

	internal static string Message_TurningOffPTCs => ResourceManager.GetString("StringTable.Message_TurningOffPTCs");

	internal static string Message_BatteryCoolantTestHasFinished => ResourceManager.GetString("StringTable.Message_BatteryCoolantTestHasFinished");

	internal static string Message_SetBatteryCircuitCoolantPumpsTo60 => ResourceManager.GetString("StringTable.Message_SetBatteryCircuitCoolantPumpsTo60");

	internal static string Message_ReadyToStart => ResourceManager.GetString("StringTable.Message_ReadyToStart");

	internal static string Message_TurnOnPTCsTo100 => ResourceManager.GetString("StringTable.Message_TurnOnPTCsTo100");

	internal static string Message_TurningOnTheCabCircuitPTCs => ResourceManager.GetString("StringTable.Message_TurningOnTheCabCircuitPTCs");

	internal static string Message_TestMessageFormat => ResourceManager.GetString("StringTable.Message_TestMessageFormat");

	internal static string Message_WaitingUntilTemperatureReaches50DegCOrFor0Seconds => ResourceManager.GetString("StringTable.Message_WaitingUntilTemperatureReaches50DegCOrFor0Seconds");

	internal static string Message_UnableToStartBattery => ResourceManager.GetString("StringTable.Message_UnableToStartBattery");

	internal static string Message_WaitUntilTemperatureIs38DegCOr10MinutesHasPassed => ResourceManager.GetString("StringTable.Message_WaitUntilTemperatureIs38DegCOr10MinutesHasPassed");

	internal static string Message_FailedTuringOnThePTCs => ResourceManager.GetString("StringTable.Message_FailedTuringOnThePTCs");

	internal static string Message_UnableToPerformTestMissingService => ResourceManager.GetString("StringTable.Message_UnableToPerformTestMissingService");

	internal static string Message_TestStopped => ResourceManager.GetString("StringTable.Message_TestStopped");

	internal static string Message_UnableToStartEDrive => ResourceManager.GetString("StringTable.Message_UnableToStartEDrive");

	internal static string Message_EDriveDeaearationStopFailed => ResourceManager.GetString("StringTable.Message_EDriveDeaearationStopFailed");
}
