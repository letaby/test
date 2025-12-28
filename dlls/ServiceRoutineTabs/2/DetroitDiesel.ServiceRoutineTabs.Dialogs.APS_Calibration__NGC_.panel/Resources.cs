using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__NGC_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_VerifyingTorsionBarTorqueOffset => ResourceManager.GetString("StringTable.Message_VerifyingTorsionBarTorqueOffset");

	internal static string Message_StartEngine => ResourceManager.GetString("StringTable.Message_StartEngine");

	internal static string Message_TurnSteeringWheelToLEFTEndstop => ResourceManager.GetString("StringTable.Message_TurnSteeringWheelToLEFTEndstop");

	internal static string Message_RemoveHandsFromWheelPressNextToBeginTorsionBarCalibration => ResourceManager.GetString("StringTable.Message_RemoveHandsFromWheelPressNextToBeginTorsionBarCalibration");

	internal static string Message_UnableToResetCalibrationValues => ResourceManager.GetString("StringTable.Message_UnableToResetCalibrationValues");

	internal static string Message_CalibrationFailedDuringEcuReset => ResourceManager.GetString("StringTable.Message_CalibrationFailedDuringEcuReset");

	internal static string Message_CalibrationFailedDuringTorsionBarCalibration => ResourceManager.GetString("StringTable.Message_CalibrationFailedDuringTorsionBarCalibration");

	internal static string MessageFormat_Connect01ToBeginTheCalibration => ResourceManager.GetString("StringTable.MessageFormat_Connect01ToBeginTheCalibration");

	internal static string Message_CenterSteeringWheel => ResourceManager.GetString("StringTable.Message_CenterSteeringWheel");

	internal static string Message_CalibratingDoNotTouchSteeringWheel => ResourceManager.GetString("StringTable.Message_CalibratingDoNotTouchSteeringWheel");

	internal static string MessageFormat_Connect012ToBeginTheCalibration => ResourceManager.GetString("StringTable.MessageFormat_Connect012ToBeginTheCalibration");

	internal static string Message_ResettingCalibrationData => ResourceManager.GetString("StringTable.Message_ResettingCalibrationData");

	internal static string Message_CalibrationFailedCouldNotFindTorsionBarOffsetParameter => ResourceManager.GetString("StringTable.Message_CalibrationFailedCouldNotFindTorsionBarOffsetParameter");

	internal static string Message_PerformingHardReset => ResourceManager.GetString("StringTable.Message_PerformingHardReset");

	internal static string Message_VerifyingCalibration => ResourceManager.GetString("StringTable.Message_VerifyingCalibration");

	internal static string Message_ClickNextToContinue => ResourceManager.GetString("StringTable.Message_ClickNextToContinue");

	internal static string Message_PressNextToBeginCalibrationProcess => ResourceManager.GetString("StringTable.Message_PressNextToBeginCalibrationProcess");

	internal static string Message_ClickNextToContinueToSteeringTorsionBarTorqueOffset => ResourceManager.GetString("StringTable.Message_ClickNextToContinueToSteeringTorsionBarTorqueOffset");

	internal static string Message_CalibrateRightEndstop => ResourceManager.GetString("StringTable.Message_CalibrateRightEndstop");

	internal static string MessageFormat_TorsionBarOffsetValues012 => ResourceManager.GetString("StringTable.MessageFormat_TorsionBarOffsetValues012");

	internal static string Message_TurnSteeringWheelToRightSlightly => ResourceManager.GetString("StringTable.Message_TurnSteeringWheelToRightSlightly");

	internal static string Message_EndstopCalibrationFailed => ResourceManager.GetString("StringTable.Message_EndstopCalibrationFailed");

	internal static string Message_CalibrationFailedTorsionBarOffsetFailedToReadOrOutOfRange => ResourceManager.GetString("StringTable.Message_CalibrationFailedTorsionBarOffsetFailedToReadOrOutOfRange");

	internal static string Message_TurnTheSteeringWheelToTheRightUntilItReachesTheEndstopAndHold => ResourceManager.GetString("StringTable.Message_TurnTheSteeringWheelToTheRightUntilItReachesTheEndstopAndHold");

	internal static string Message_ResolveTheActiveFaultsBeforeBeginning => ResourceManager.GetString("StringTable.Message_ResolveTheActiveFaultsBeforeBeginning");

	internal static string Message_CalibrationFailed => ResourceManager.GetString("StringTable.Message_CalibrationFailed");

	internal static string Message_CalibrateLeftEndstop => ResourceManager.GetString("StringTable.Message_CalibrateLeftEndstop");

	internal static string Message_CalibratingCenter => ResourceManager.GetString("StringTable.Message_CalibratingCenter");

	internal static string Message_TurnSteeringWheelToRIGHTEndstop => ResourceManager.GetString("StringTable.Message_TurnSteeringWheelToRIGHTEndstop");

	internal static string Message_EcuIsDisconnected => ResourceManager.GetString("StringTable.Message_EcuIsDisconnected");

	internal static string MessageFormat_Connect0ToBeginTheCalibration => ResourceManager.GetString("StringTable.MessageFormat_Connect0ToBeginTheCalibration");

	internal static string Message_CalibrationFailedCouldNotStartTorsionBarCalibration => ResourceManager.GetString("StringTable.Message_CalibrationFailedCouldNotStartTorsionBarCalibration");

	internal static string Message_CalibrationWasSuccessful => ResourceManager.GetString("StringTable.Message_CalibrationWasSuccessful");

	internal static string MessageFormat_Connect0123ToBeginTheCalibration => ResourceManager.GetString("StringTable.MessageFormat_Connect0123ToBeginTheCalibration");

	internal static string MessageFormat_PleaseWait0 => ResourceManager.GetString("StringTable.MessageFormat_PleaseWait0");

	internal static string Message_CenterCalibrationServiceFailed => ResourceManager.GetString("StringTable.Message_CenterCalibrationServiceFailed");

	internal static string MessageFormat_TorsionBarOffsetInvalid0 => ResourceManager.GetString("StringTable.MessageFormat_TorsionBarOffsetInvalid0");

	internal static string Message_TurnSteeringWheelToLeftSlightly => ResourceManager.GetString("StringTable.Message_TurnSteeringWheelToLeftSlightly");

	internal static string Message_SlowlyTurnTheSteeringWheelToTheRightUntilItReachesTheEndstopAndHold => ResourceManager.GetString("StringTable.Message_SlowlyTurnTheSteeringWheelToTheRightUntilItReachesTheEndstopAndHold");

	internal static string Message_SlowlyTurnTheSteeringWheelToTheLeftUntilItReachesTheEndstopAndHold => ResourceManager.GetString("StringTable.Message_SlowlyTurnTheSteeringWheelToTheLeftUntilItReachesTheEndstopAndHold");

	internal static string Message_TurnTheSteeringWheelToTheLeftUntilItReachesTheEndstopAndHold => ResourceManager.GetString("StringTable.Message_TurnTheSteeringWheelToTheLeftUntilItReachesTheEndstopAndHold");

	internal static string Message_CalibrationFailedReachedEndOfRoutineWithEndstopsNotLearned => ResourceManager.GetString("StringTable.Message_CalibrationFailedReachedEndOfRoutineWithEndstopsNotLearned");

	internal static string Message_ClickNextToContinueToCalibration => ResourceManager.GetString("StringTable.Message_ClickNextToContinueToCalibration");

	internal static string Message_YouMayCloseThisCalibration => ResourceManager.GetString("StringTable.Message_YouMayCloseThisCalibration");

	internal static string Message_UnableToPerformCalibrationMissingService => ResourceManager.GetString("StringTable.Message_UnableToPerformCalibrationMissingService");

	internal static string Message_CalibratingTorsionBarTorqueOffset => ResourceManager.GetString("StringTable.Message_CalibratingTorsionBarTorqueOffset");

	internal static string Message_CalibrationFailedForUnknownReason => ResourceManager.GetString("StringTable.Message_CalibrationFailedForUnknownReason");

	internal static string MessageFormat_CalibrationFailed0 => ResourceManager.GetString("StringTable.MessageFormat_CalibrationFailed0");
}
