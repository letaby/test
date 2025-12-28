using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_UnableToRequestEndOfManipulation => ResourceManager.GetString("StringTable.Message_UnableToRequestEndOfManipulation");

	internal static string Message_TestCanStart => ResourceManager.GetString("StringTable.Message_TestCanStart");

	internal static string Message_TESTCOMPLETEPASSED => ResourceManager.GetString("StringTable.Message_TESTCOMPLETEPASSED");

	internal static string Message_CannotStartTheTestAsTheDeviceIsBusy => ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheDeviceIsBusy");

	internal static string Message_CannotStartTheTestAsTheRequiredTemperaturesAreNotInRange => ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheRequiredTemperaturesAreNotInRange");

	internal static string Message_ThermalConditionWaitCompleteWaitingForEGRToBeInRange => ResourceManager.GetString("StringTable.Message_ThermalConditionWaitCompleteWaitingForEGRToBeInRange");

	internal static string Message_CannotStartTheTestAsTheEngineIsNotRunningStartTheEngine => ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheEngineIsNotRunningStartTheEngine");

	internal static string Message_EGRValveInRange => ResourceManager.GetString("StringTable.Message_EGRValveInRange");

	internal static string Message_TESTABORTEDUserCanceledTheTest => ResourceManager.GetString("StringTable.Message_TESTABORTEDUserCanceledTheTest");

	internal static string Message_WaitingForTimer => ResourceManager.GetString("StringTable.Message_WaitingForTimer");

	internal static string MessageFormat_TheObservedValueOf0Was12 => ResourceManager.GetString("StringTable.MessageFormat_TheObservedValueOf0Was12");

	internal static string MessageFormat_WARNING => ResourceManager.GetString("StringTable.MessageFormat_WARNING");

	internal static string MessageFormat_WaitingForRunoff0SecondsRemaining => ResourceManager.GetString("StringTable.MessageFormat_WaitingForRunoff0SecondsRemaining");

	internal static string MessageFormat_WaitingForEGRValveToReachRange0SecondsRemaining => ResourceManager.GetString("StringTable.MessageFormat_WaitingForEGRValveToReachRange0SecondsRemaining");

	internal static string Message_EngineIsAtSpeedWaitingToGetPastThermalCondition => ResourceManager.GetString("StringTable.Message_EngineIsAtSpeedWaitingToGetPastThermalCondition");

	internal static string Message_TESTFAILEDTemperaturesFellOutRangeCorrectAndRestartTheTest => ResourceManager.GetString("StringTable.Message_TESTFAILEDTemperaturesFellOutRangeCorrectAndRestartTheTest");

	internal static string Message_RequestEndIdleSpeedManipulation => ResourceManager.GetString("StringTable.Message_RequestEndIdleSpeedManipulation");

	internal static string Message_ServiceRoutinesInProgress => ResourceManager.GetString("StringTable.Message_ServiceRoutinesInProgress");

	internal static string Message_CloseThisWindowToContinueTroubleshooting => ResourceManager.GetString("StringTable.Message_CloseThisWindowToContinueTroubleshooting");

	internal static string Message_Rpm => ResourceManager.GetString("StringTable.Message_Rpm");

	internal static string Message_CloseThisWindowToContinueTroubleshooting1 => ResourceManager.GetString("StringTable.Message_CloseThisWindowToContinueTroubleshooting1");

	internal static string Message_EngineIsAtRunoffSpeedWaitingForRunoffPeriod => ResourceManager.GetString("StringTable.Message_EngineIsAtRunoffSpeedWaitingForRunoffPeriod");

	internal static string MessageFormat_ThermalConditionWait0SecondsRemaining => ResourceManager.GetString("StringTable.MessageFormat_ThermalConditionWait0SecondsRemaining");

	internal static string MessageFormat_WaitingForEngineSpeedToReach0Rpm1 => ResourceManager.GetString("StringTable.MessageFormat_WaitingForEngineSpeedToReach0Rpm1");

	internal static string Message_TestSequenceEnded => ResourceManager.GetString("StringTable.Message_TestSequenceEnded");

	internal static string Message_TESTFAILEDEGRWasNotInRangeForTheRequiredPeriodCorrectAndRestartTheTest => ResourceManager.GetString("StringTable.Message_TESTFAILEDEGRWasNotInRangeForTheRequiredPeriodCorrectAndRestartTheTest");

	internal static string Message_EGRLowFlowTest => ResourceManager.GetString("StringTable.Message_EGRLowFlowTest");

	internal static string Message_ManipulateEngineSpeedTo => ResourceManager.GetString("StringTable.Message_ManipulateEngineSpeedTo");

	internal static string Message_TESTFAILEDDeviceWentOffline => ResourceManager.GetString("StringTable.Message_TESTFAILEDDeviceWentOffline");

	internal static string Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionInNeutral => ResourceManager.GetString("StringTable.Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionInNeutral");

	internal static string Message_CannotStartTheTestAsTheDeviceIsNotOnline => ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheDeviceIsNotOnline");

	internal static string Message_TESTFAILEDServicesFailedToExecute => ResourceManager.GetString("StringTable.Message_TESTFAILEDServicesFailedToExecute");

	internal static string Message_TESTCOMPLETEFAILED => ResourceManager.GetString("StringTable.Message_TESTCOMPLETEFAILED");

	internal static string Message_EGRValveDroppedOutOfRangeWaitingForItToBeInRangeAgain => ResourceManager.GetString("StringTable.Message_EGRValveDroppedOutOfRangeWaitingForItToBeInRangeAgain");

	internal static string MessageFormat_WaitingForEngineSpeedToReach0Rpm => ResourceManager.GetString("StringTable.MessageFormat_WaitingForEngineSpeedToReach0Rpm");
}
