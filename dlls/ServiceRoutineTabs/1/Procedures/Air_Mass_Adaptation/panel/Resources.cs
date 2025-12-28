// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_PleaseManuallyApplyChangesToTheEngineSpeedInOrderToCompleteTheAdaptation
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_PleaseManuallyApplyChangesToTheEngineSpeedInOrderToCompleteTheAdaptation");
    }
  }

  internal static string Message_AdditionalFaultsDetectedPleaseAddressTheseBeforePerformingAirMassAdaptation
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_AdditionalFaultsDetectedPleaseAddressTheseBeforePerformingAirMassAdaptation");
    }
  }

  internal static string Message_TheMCMDoesNotAppearToHaveControlOfTheEngine
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheMCMDoesNotAppearToHaveControlOfTheEngine");
    }
  }

  internal static string MessageFormat_ConditionMet01CAnd23C
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_ConditionMet01CAnd23C");
  }

  internal static string MessageFormat_0Is1AndRequiresTemporaryAdjustment
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_0Is1AndRequiresTemporaryAdjustment");
    }
  }

  internal static string MessageFormat_ShuttingOffEGRValveFailed0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_ShuttingOffEGRValveFailed0");
    }
  }

  internal static string Message_CallingAAMARequestResultsRoutine
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CallingAAMARequestResultsRoutine");
    }
  }

  internal static string Message_UserRequestedMaximumEngineSpeedModification
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UserRequestedMaximumEngineSpeedModification");
    }
  }

  internal static string Message_EngineIsStarted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineIsStarted");
  }

  internal static string Message_CannotDetectMaximumEngineSpeedLimits
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotDetectMaximumEngineSpeedLimits");
    }
  }

  internal static string Message_UserDeclinedMaximumEngineSpeedModification
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UserDeclinedMaximumEngineSpeedModification");
    }
  }

  internal static string Message_ServiceError
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ServiceError");
  }

  internal static string Message_FinalizingAdaptation
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_FinalizingAdaptation");
  }

  internal static string Message_UserDeclinedManualAirMassAdaptation
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UserDeclinedManualAirMassAdaptation");
    }
  }

  internal static string Message_MCMIsConnectedButEngineIsNotSupported
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_MCMIsConnectedButEngineIsNotSupported");
    }
  }

  internal static string MessageFormat_AnErrorOccurredClearingTheCurrentAdaptation0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredClearingTheCurrentAdaptation0");
    }
  }

  internal static string Message_NoOtherFaultsDetected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_NoOtherFaultsDetected");
  }

  internal static string MessageFormat_ShuttingOffFan1Failed0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_ShuttingOffFan1Failed0");
  }

  internal static string MessageFormat_TurningOnFan2Failed0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_TurningOnFan2Failed0");
  }

  internal static string MessageFormat_MaximumEngineSpeedLimitsAreBelow0RpmAndWillNeedAdjustment
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_MaximumEngineSpeedLimitsAreBelow0RpmAndWillNeedAdjustment");
    }
  }

  internal static string Message_ServiceExecuted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ServiceExecuted");
  }

  internal static string MessageFormat_ResultOfRequestResultsRoutine0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_ResultOfRequestResultsRoutine0");
    }
  }

  internal static string Message_LimitsValid
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_LimitsValid");
  }

  internal static string MessageFormat_MaximumEngineSpeedLimitsAreAtOrAbove0Rpm
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_MaximumEngineSpeedLimitsAreAtOrAbove0Rpm");
    }
  }

  internal static string Message_ResultedInAnError
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ResultedInAnError");
  }

  internal static string Message_Unknown
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Unknown");
  }

  internal static string Message_AirMassAdaptationStarted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_AirMassAdaptationStarted");
  }

  internal static string MessageFormat_ErrorRetrieving01
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_ErrorRetrieving01");
  }

  internal static string Message_ResettingMaximumEngineSpeedLimits
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ResettingMaximumEngineSpeedLimits");
    }
  }

  internal static string Message_CPC2IsConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CPC2IsConnected");
  }

  internal static string Message_EngineHasNotBeenStartedPleaseStartTheEngine
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineHasNotBeenStartedPleaseStartTheEngine");
    }
  }

  internal static string Message_CannotDetectIfEngineIsStarted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CannotDetectIfEngineIsStarted");
  }

  internal static string Message_AirMassAdaptationIsNotRequired
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_AirMassAdaptationIsNotRequired");
    }
  }

  internal static string Message_AutomaticCalibrationOfAirMassAdaptationHasFailedDoYouWantToTryManuallyAdaptingTheNodes
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_AutomaticCalibrationOfAirMassAdaptationHasFailedDoYouWantToTryManuallyAdaptingTheNodes");
    }
  }

  internal static string MessageFormat_TurningOnFan1Failed0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_TurningOnFan1Failed0");
  }

  internal static string Message_UnableToResetMaxmimumEngineSpeedLimitsCPC2NotConnected
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UnableToResetMaxmimumEngineSpeedLimitsCPC2NotConnected");
    }
  }

  internal static string MessageFormat_ModifyingMaximumEngineSpeedLimitsFailed0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_ModifyingMaximumEngineSpeedLimitsFailed0");
    }
  }

  internal static string MessageFormat_PerformingStep0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_PerformingStep0");
  }

  internal static string Message_ShuttingOffFans
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ShuttingOffFans");
  }

  internal static string MessageFormat_WaitingForOperatingCondition01CAnd23C
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_WaitingForOperatingCondition01CAnd23C");
    }
  }

  internal static string Message_CallingAAMAStartRoutine
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CallingAAMAStartRoutine");
  }

  internal static string MessageFormat_IncreasingEngineSpeedTo0AndHoldingFor1Seconds
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_IncreasingEngineSpeedTo0AndHoldingFor1Seconds");
    }
  }

  internal static string Message_CheckingMaximumEngineSpeedLimit
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CheckingMaximumEngineSpeedLimit");
    }
  }

  internal static string MessageFormat_0Is1AndRequiresTemporaryAdjustment1
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_0Is1AndRequiresTemporaryAdjustment1");
    }
  }

  internal static string Message_TheMaximumEngineSpeedLimitsMustBeTemporarilyModified
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheMaximumEngineSpeedLimitsMustBeTemporarilyModified");
    }
  }

  internal static string Message_MCMIsNotConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCMIsNotConnected");
  }

  internal static string Message_ItHasNotBeenPossibleToResetTheMaximumEngineSpeedLimitsAsTheCPC2IsNoLongerConnected
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ItHasNotBeenPossibleToResetTheMaximumEngineSpeedLimitsAsTheCPC2IsNoLongerConnected");
    }
  }

  internal static string MessageFormat_MaximumEngineSpeedLimitsHaveBeenTemporarilyAdjustedTo0Rpm
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_MaximumEngineSpeedLimitsHaveBeenTemporarilyAdjustedTo0Rpm");
    }
  }

  internal static string Message_User
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_User");
  }

  internal static string Message_AreYouSureYouWishToProceed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_AreYouSureYouWishToProceed");
  }

  internal static string Message_MCMIsBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCMIsBusy");
  }

  internal static string Message_AirMassAdaptationIsRequired
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_AirMassAdaptationIsRequired");
  }

  internal static string Message_Modifying
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Modifying");
  }

  internal static string Message_DPFRegenerationInProgressCannotPerformAirMassAdaptation
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_DPFRegenerationInProgressCannotPerformAirMassAdaptation");
    }
  }

  internal static string Message_ShuttingOffEGRValve
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ShuttingOffEGRValve");
  }

  internal static string Message_DisconnectFromTheDevicesCompleteTheAdaptationOrPressCancelToFinish
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_DisconnectFromTheDevicesCompleteTheAdaptationOrPressCancelToFinish");
    }
  }

  internal static string Message_NotPerformingDPFRegeneration
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_NotPerformingDPFRegeneration");
  }

  internal static string Message_VIN
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_VIN");
  }

  internal static string Message_ESN
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ESN");
  }

  internal static string Message_AAMAProcedureOutcomeDeterminedByMCM
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_AAMAProcedureOutcomeDeterminedByMCM");
    }
  }

  internal static string Message_MCMIsConnectedAndEngineIsSupported
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_MCMIsConnectedAndEngineIsSupported");
    }
  }

  internal static string Message_CannotDetectIfAdditionalFaultsNeedAddressing
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotDetectIfAdditionalFaultsNeedAddressing");
    }
  }

  internal static string MessageFormat_AirMassAdaptationStartedUser0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AirMassAdaptationStartedUser0");
    }
  }

  internal static string Message_AreYouSureYouWantToContinue
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_AreYouSureYouWantToContinue");
  }

  internal static string Message_CPC2IsBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CPC2IsBusy");
  }

  internal static string Message_CannotDetectIfDPFRegenerationIsInProgress
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotDetectIfDPFRegenerationIsInProgress");
    }
  }

  internal static string Message_SettingTheFaultWillRequire1
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SettingTheFaultWillRequire1");
  }

  internal static string Message_EngineSpeedLimitHandledAutomaticallyByAAMATest
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineSpeedLimitHandledAutomaticallyByAAMATest");
    }
  }

  internal static string Message_SettingTheFaultWillRequireAirMassAdaptationToBePerformed
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_SettingTheFaultWillRequireAirMassAdaptationToBePerformed");
    }
  }

  internal static string Message_UserRequestedFaultResetForAirMassAdaptation
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UserRequestedFaultResetForAirMassAdaptation");
    }
  }

  internal static string Message_CannotDetectIfAirMassAdaptationIsRequired
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotDetectIfAirMassAdaptationIsRequired");
    }
  }

  internal static string MessageFormat_ShuttingOffFan2Failed0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_ShuttingOffFan2Failed0");
  }

  internal static string MessageFormat_AirMassAdaptationProcedureFinishedResult0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AirMassAdaptationProcedureFinishedResult0");
    }
  }

  internal static string Message_Report
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Report");
  }

  internal static string Message_ResettingEGRValve
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ResettingEGRValve");
  }

  internal static string Message_CPC2IsNotConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CPC2IsNotConnected");
  }

  internal static string Message_TurningFansOn
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TurningFansOn");
  }

  internal static string Message_FailedToObtainCoolantAndOilTemperaturesAbortingAdaptation
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_FailedToObtainCoolantAndOilTemperaturesAbortingAdaptation");
    }
  }

  internal static string Message_MaximumEngineSpeedLimitsHaveNotBeenRead
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_MaximumEngineSpeedLimitsHaveNotBeenRead");
    }
  }

  internal static string Message_AirMassAdaptation
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_AirMassAdaptation");
  }

  internal static string MessageFormat_ResultOfStartRoutine0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_ResultOfStartRoutine0");
  }

  internal static string Message_UserRequestedManualAirMassAdaptation
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UserRequestedManualAirMassAdaptation");
    }
  }
}
