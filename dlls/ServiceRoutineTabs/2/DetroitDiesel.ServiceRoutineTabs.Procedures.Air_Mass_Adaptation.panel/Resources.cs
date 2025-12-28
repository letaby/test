using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_PleaseManuallyApplyChangesToTheEngineSpeedInOrderToCompleteTheAdaptation => ResourceManager.GetString("StringTable.Message_PleaseManuallyApplyChangesToTheEngineSpeedInOrderToCompleteTheAdaptation");

	internal static string Message_AdditionalFaultsDetectedPleaseAddressTheseBeforePerformingAirMassAdaptation => ResourceManager.GetString("StringTable.Message_AdditionalFaultsDetectedPleaseAddressTheseBeforePerformingAirMassAdaptation");

	internal static string Message_TheMCMDoesNotAppearToHaveControlOfTheEngine => ResourceManager.GetString("StringTable.Message_TheMCMDoesNotAppearToHaveControlOfTheEngine");

	internal static string MessageFormat_ConditionMet01CAnd23C => ResourceManager.GetString("StringTable.MessageFormat_ConditionMet01CAnd23C");

	internal static string MessageFormat_0Is1AndRequiresTemporaryAdjustment => ResourceManager.GetString("StringTable.MessageFormat_0Is1AndRequiresTemporaryAdjustment");

	internal static string MessageFormat_ShuttingOffEGRValveFailed0 => ResourceManager.GetString("StringTable.MessageFormat_ShuttingOffEGRValveFailed0");

	internal static string Message_CallingAAMARequestResultsRoutine => ResourceManager.GetString("StringTable.Message_CallingAAMARequestResultsRoutine");

	internal static string Message_UserRequestedMaximumEngineSpeedModification => ResourceManager.GetString("StringTable.Message_UserRequestedMaximumEngineSpeedModification");

	internal static string Message_EngineIsStarted => ResourceManager.GetString("StringTable.Message_EngineIsStarted");

	internal static string Message_CannotDetectMaximumEngineSpeedLimits => ResourceManager.GetString("StringTable.Message_CannotDetectMaximumEngineSpeedLimits");

	internal static string Message_UserDeclinedMaximumEngineSpeedModification => ResourceManager.GetString("StringTable.Message_UserDeclinedMaximumEngineSpeedModification");

	internal static string Message_ServiceError => ResourceManager.GetString("StringTable.Message_ServiceError");

	internal static string Message_FinalizingAdaptation => ResourceManager.GetString("StringTable.Message_FinalizingAdaptation");

	internal static string Message_UserDeclinedManualAirMassAdaptation => ResourceManager.GetString("StringTable.Message_UserDeclinedManualAirMassAdaptation");

	internal static string Message_MCMIsConnectedButEngineIsNotSupported => ResourceManager.GetString("StringTable.Message_MCMIsConnectedButEngineIsNotSupported");

	internal static string MessageFormat_AnErrorOccurredClearingTheCurrentAdaptation0 => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredClearingTheCurrentAdaptation0");

	internal static string Message_NoOtherFaultsDetected => ResourceManager.GetString("StringTable.Message_NoOtherFaultsDetected");

	internal static string MessageFormat_ShuttingOffFan1Failed0 => ResourceManager.GetString("StringTable.MessageFormat_ShuttingOffFan1Failed0");

	internal static string MessageFormat_TurningOnFan2Failed0 => ResourceManager.GetString("StringTable.MessageFormat_TurningOnFan2Failed0");

	internal static string MessageFormat_MaximumEngineSpeedLimitsAreBelow0RpmAndWillNeedAdjustment => ResourceManager.GetString("StringTable.MessageFormat_MaximumEngineSpeedLimitsAreBelow0RpmAndWillNeedAdjustment");

	internal static string Message_ServiceExecuted => ResourceManager.GetString("StringTable.Message_ServiceExecuted");

	internal static string MessageFormat_ResultOfRequestResultsRoutine0 => ResourceManager.GetString("StringTable.MessageFormat_ResultOfRequestResultsRoutine0");

	internal static string Message_LimitsValid => ResourceManager.GetString("StringTable.Message_LimitsValid");

	internal static string MessageFormat_MaximumEngineSpeedLimitsAreAtOrAbove0Rpm => ResourceManager.GetString("StringTable.MessageFormat_MaximumEngineSpeedLimitsAreAtOrAbove0Rpm");

	internal static string Message_ResultedInAnError => ResourceManager.GetString("StringTable.Message_ResultedInAnError");

	internal static string Message_Unknown => ResourceManager.GetString("StringTable.Message_Unknown");

	internal static string Message_AirMassAdaptationStarted => ResourceManager.GetString("StringTable.Message_AirMassAdaptationStarted");

	internal static string MessageFormat_ErrorRetrieving01 => ResourceManager.GetString("StringTable.MessageFormat_ErrorRetrieving01");

	internal static string Message_ResettingMaximumEngineSpeedLimits => ResourceManager.GetString("StringTable.Message_ResettingMaximumEngineSpeedLimits");

	internal static string Message_CPC2IsConnected => ResourceManager.GetString("StringTable.Message_CPC2IsConnected");

	internal static string Message_EngineHasNotBeenStartedPleaseStartTheEngine => ResourceManager.GetString("StringTable.Message_EngineHasNotBeenStartedPleaseStartTheEngine");

	internal static string Message_CannotDetectIfEngineIsStarted => ResourceManager.GetString("StringTable.Message_CannotDetectIfEngineIsStarted");

	internal static string Message_AirMassAdaptationIsNotRequired => ResourceManager.GetString("StringTable.Message_AirMassAdaptationIsNotRequired");

	internal static string Message_AutomaticCalibrationOfAirMassAdaptationHasFailedDoYouWantToTryManuallyAdaptingTheNodes => ResourceManager.GetString("StringTable.Message_AutomaticCalibrationOfAirMassAdaptationHasFailedDoYouWantToTryManuallyAdaptingTheNodes");

	internal static string MessageFormat_TurningOnFan1Failed0 => ResourceManager.GetString("StringTable.MessageFormat_TurningOnFan1Failed0");

	internal static string Message_UnableToResetMaxmimumEngineSpeedLimitsCPC2NotConnected => ResourceManager.GetString("StringTable.Message_UnableToResetMaxmimumEngineSpeedLimitsCPC2NotConnected");

	internal static string MessageFormat_ModifyingMaximumEngineSpeedLimitsFailed0 => ResourceManager.GetString("StringTable.MessageFormat_ModifyingMaximumEngineSpeedLimitsFailed0");

	internal static string MessageFormat_PerformingStep0 => ResourceManager.GetString("StringTable.MessageFormat_PerformingStep0");

	internal static string Message_ShuttingOffFans => ResourceManager.GetString("StringTable.Message_ShuttingOffFans");

	internal static string MessageFormat_WaitingForOperatingCondition01CAnd23C => ResourceManager.GetString("StringTable.MessageFormat_WaitingForOperatingCondition01CAnd23C");

	internal static string Message_CallingAAMAStartRoutine => ResourceManager.GetString("StringTable.Message_CallingAAMAStartRoutine");

	internal static string MessageFormat_IncreasingEngineSpeedTo0AndHoldingFor1Seconds => ResourceManager.GetString("StringTable.MessageFormat_IncreasingEngineSpeedTo0AndHoldingFor1Seconds");

	internal static string Message_CheckingMaximumEngineSpeedLimit => ResourceManager.GetString("StringTable.Message_CheckingMaximumEngineSpeedLimit");

	internal static string MessageFormat_0Is1AndRequiresTemporaryAdjustment1 => ResourceManager.GetString("StringTable.MessageFormat_0Is1AndRequiresTemporaryAdjustment1");

	internal static string Message_TheMaximumEngineSpeedLimitsMustBeTemporarilyModified => ResourceManager.GetString("StringTable.Message_TheMaximumEngineSpeedLimitsMustBeTemporarilyModified");

	internal static string Message_MCMIsNotConnected => ResourceManager.GetString("StringTable.Message_MCMIsNotConnected");

	internal static string Message_ItHasNotBeenPossibleToResetTheMaximumEngineSpeedLimitsAsTheCPC2IsNoLongerConnected => ResourceManager.GetString("StringTable.Message_ItHasNotBeenPossibleToResetTheMaximumEngineSpeedLimitsAsTheCPC2IsNoLongerConnected");

	internal static string MessageFormat_MaximumEngineSpeedLimitsHaveBeenTemporarilyAdjustedTo0Rpm => ResourceManager.GetString("StringTable.MessageFormat_MaximumEngineSpeedLimitsHaveBeenTemporarilyAdjustedTo0Rpm");

	internal static string Message_User => ResourceManager.GetString("StringTable.Message_User");

	internal static string Message_AreYouSureYouWishToProceed => ResourceManager.GetString("StringTable.Message_AreYouSureYouWishToProceed");

	internal static string Message_MCMIsBusy => ResourceManager.GetString("StringTable.Message_MCMIsBusy");

	internal static string Message_AirMassAdaptationIsRequired => ResourceManager.GetString("StringTable.Message_AirMassAdaptationIsRequired");

	internal static string Message_Modifying => ResourceManager.GetString("StringTable.Message_Modifying");

	internal static string Message_DPFRegenerationInProgressCannotPerformAirMassAdaptation => ResourceManager.GetString("StringTable.Message_DPFRegenerationInProgressCannotPerformAirMassAdaptation");

	internal static string Message_ShuttingOffEGRValve => ResourceManager.GetString("StringTable.Message_ShuttingOffEGRValve");

	internal static string Message_DisconnectFromTheDevicesCompleteTheAdaptationOrPressCancelToFinish => ResourceManager.GetString("StringTable.Message_DisconnectFromTheDevicesCompleteTheAdaptationOrPressCancelToFinish");

	internal static string Message_NotPerformingDPFRegeneration => ResourceManager.GetString("StringTable.Message_NotPerformingDPFRegeneration");

	internal static string Message_VIN => ResourceManager.GetString("StringTable.Message_VIN");

	internal static string Message_ESN => ResourceManager.GetString("StringTable.Message_ESN");

	internal static string Message_AAMAProcedureOutcomeDeterminedByMCM => ResourceManager.GetString("StringTable.Message_AAMAProcedureOutcomeDeterminedByMCM");

	internal static string Message_MCMIsConnectedAndEngineIsSupported => ResourceManager.GetString("StringTable.Message_MCMIsConnectedAndEngineIsSupported");

	internal static string Message_CannotDetectIfAdditionalFaultsNeedAddressing => ResourceManager.GetString("StringTable.Message_CannotDetectIfAdditionalFaultsNeedAddressing");

	internal static string MessageFormat_AirMassAdaptationStartedUser0 => ResourceManager.GetString("StringTable.MessageFormat_AirMassAdaptationStartedUser0");

	internal static string Message_AreYouSureYouWantToContinue => ResourceManager.GetString("StringTable.Message_AreYouSureYouWantToContinue");

	internal static string Message_CPC2IsBusy => ResourceManager.GetString("StringTable.Message_CPC2IsBusy");

	internal static string Message_CannotDetectIfDPFRegenerationIsInProgress => ResourceManager.GetString("StringTable.Message_CannotDetectIfDPFRegenerationIsInProgress");

	internal static string Message_SettingTheFaultWillRequire1 => ResourceManager.GetString("StringTable.Message_SettingTheFaultWillRequire1");

	internal static string Message_EngineSpeedLimitHandledAutomaticallyByAAMATest => ResourceManager.GetString("StringTable.Message_EngineSpeedLimitHandledAutomaticallyByAAMATest");

	internal static string Message_SettingTheFaultWillRequireAirMassAdaptationToBePerformed => ResourceManager.GetString("StringTable.Message_SettingTheFaultWillRequireAirMassAdaptationToBePerformed");

	internal static string Message_UserRequestedFaultResetForAirMassAdaptation => ResourceManager.GetString("StringTable.Message_UserRequestedFaultResetForAirMassAdaptation");

	internal static string Message_CannotDetectIfAirMassAdaptationIsRequired => ResourceManager.GetString("StringTable.Message_CannotDetectIfAirMassAdaptationIsRequired");

	internal static string MessageFormat_ShuttingOffFan2Failed0 => ResourceManager.GetString("StringTable.MessageFormat_ShuttingOffFan2Failed0");

	internal static string MessageFormat_AirMassAdaptationProcedureFinishedResult0 => ResourceManager.GetString("StringTable.MessageFormat_AirMassAdaptationProcedureFinishedResult0");

	internal static string Message_Report => ResourceManager.GetString("StringTable.Message_Report");

	internal static string Message_ResettingEGRValve => ResourceManager.GetString("StringTable.Message_ResettingEGRValve");

	internal static string Message_CPC2IsNotConnected => ResourceManager.GetString("StringTable.Message_CPC2IsNotConnected");

	internal static string Message_TurningFansOn => ResourceManager.GetString("StringTable.Message_TurningFansOn");

	internal static string Message_FailedToObtainCoolantAndOilTemperaturesAbortingAdaptation => ResourceManager.GetString("StringTable.Message_FailedToObtainCoolantAndOilTemperaturesAbortingAdaptation");

	internal static string Message_MaximumEngineSpeedLimitsHaveNotBeenRead => ResourceManager.GetString("StringTable.Message_MaximumEngineSpeedLimitsHaveNotBeenRead");

	internal static string Message_AirMassAdaptation => ResourceManager.GetString("StringTable.Message_AirMassAdaptation");

	internal static string MessageFormat_ResultOfStartRoutine0 => ResourceManager.GetString("StringTable.MessageFormat_ResultOfStartRoutine0");

	internal static string Message_UserRequestedManualAirMassAdaptation => ResourceManager.GetString("StringTable.Message_UserRequestedManualAirMassAdaptation");
}
