using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_ESN_VIN__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_SkippingMCMSetVINProcessAsTheMCMIsNotConnected => ResourceManager.GetString("StringTable.Message_SkippingMCMSetVINProcessAsTheMCMIsNotConnected");

	internal static string Message_ResettingMCMFaults => ResourceManager.GetString("StringTable.Message_ResettingMCMFaults");

	internal static string Message_SkippingSettingOfCPCVINAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingSettingOfCPCVINAsTheServiceCannotBeFound");

	internal static string MessageFormat_The01HasNotBeenChangedAndHasAValueOf2 => ResourceManager.GetString("StringTable.MessageFormat_The01HasNotBeenChangedAndHasAValueOf2");

	internal static string Message_SkippingTCMSetVINProcessAsTheTCMIsNotConnected => ResourceManager.GetString("StringTable.Message_SkippingTCMSetVINProcessAsTheTCMIsNotConnected");

	internal static string Message_WritingChangesToPermanentMemory => ResourceManager.GetString("StringTable.Message_WritingChangesToPermanentMemory");

	internal static string Message_ExecutingServices => ResourceManager.GetString("StringTable.Message_ExecutingServices");

	internal static string Message_SynchronizingESNToCPCViaKeyOffOnReset => ResourceManager.GetString("StringTable.Message_SynchronizingESNToCPCViaKeyOffOnReset");

	internal static string Message_TurnTheIgnitionOffToFinalizeChanges => ResourceManager.GetString("StringTable.Message_TurnTheIgnitionOffToFinalizeChanges");

	internal static string Message_SkippingSettingOfACMVINAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingSettingOfACMVINAsTheServiceCannotBeFound");

	internal static string Message_Executing => ResourceManager.GetString("StringTable.Message_Executing");

	internal static string Message_ServiceError => ResourceManager.GetString("StringTable.Message_ServiceError");

	internal static string Message_SkippingMCMCommitProcessAsTheServiceCouldNotBeFound => ResourceManager.GetString("StringTable.Message_SkippingMCMCommitProcessAsTheServiceCouldNotBeFound");

	internal static string MessageFormat_SettingESNTo0 => ResourceManager.GetString("StringTable.MessageFormat_SettingESNTo0");

	internal static string Message_ServiceExecuted => ResourceManager.GetString("StringTable.Message_ServiceExecuted");

	internal static string Message_SkippingCPCCommitProcessAsTheCPCIsNotConnected => ResourceManager.GetString("StringTable.Message_SkippingCPCCommitProcessAsTheCPCIsNotConnected");

	internal static string Message_SkippingSettingOfMCMVINAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingSettingOfMCMVINAsTheServiceCannotBeFound");

	internal static string Message_FailedToObtainService => ResourceManager.GetString("StringTable.Message_FailedToObtainService");

	internal static string Message_TheMCMWasDisconnected => ResourceManager.GetString("StringTable.Message_TheMCMWasDisconnected");

	internal static string MessageFormat_FailedToReadECUInfoCannotVerifyThe01Error2 => ResourceManager.GetString("StringTable.MessageFormat_FailedToReadECUInfoCannotVerifyThe01Error2");

	internal static string MessageFormat_SettingVINTo0 => ResourceManager.GetString("StringTable.MessageFormat_SettingVINTo0");

	internal static string Message_SkippingSettingOfTCMVINAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingSettingOfTCMVINAsTheServiceCannotBeFound");

	internal static string Message_TheProcedureFailedToComplete => ResourceManager.GetString("StringTable.Message_TheProcedureFailedToComplete");

	internal static string Message_TurnTheIgnitionOnToVerifyTheChanges => ResourceManager.GetString("StringTable.Message_TurnTheIgnitionOnToVerifyTheChanges");

	internal static string Message_FailedToExecuteService => ResourceManager.GetString("StringTable.Message_FailedToExecuteService");

	internal static string MessageFormat_The01HasSuccessfullyBeenSetTo2 => ResourceManager.GetString("StringTable.MessageFormat_The01HasSuccessfullyBeenSetTo2");

	internal static string Message_ResettingACMFaults => ResourceManager.GetString("StringTable.Message_ResettingACMFaults");

	internal static string Message_ResettingJ1939Faults => ResourceManager.GetString("StringTable.Message_ResettingJ1939Faults");

	internal static string Message_WaitingForTheCPCToComeBackOnline => ResourceManager.GetString("StringTable.Message_WaitingForTheCPCToComeBackOnline");

	internal static string Message_SkippingCPCResetAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingCPCResetAsTheServiceCannotBeFound");

	internal static string Message_SkippingACMSetVINProcessAsTheACMIsNotConnected => ResourceManager.GetString("StringTable.Message_SkippingACMSetVINProcessAsTheACMIsNotConnected");

	internal static string Message_VerifyingResults => ResourceManager.GetString("StringTable.Message_VerifyingResults");

	internal static string Message_SkippingCPCSetVINProcessAsTheCPCIsNotConnected => ResourceManager.GetString("StringTable.Message_SkippingCPCSetVINProcessAsTheCPCIsNotConnected");

	internal static string MessageFormat_01CannotBeVerified => ResourceManager.GetString("StringTable.MessageFormat_01CannotBeVerified");

	internal static string Message_CommittingChangesToCPCViaHardReset => ResourceManager.GetString("StringTable.Message_CommittingChangesToCPCViaHardReset");

	internal static string MessageFormat_FailedToReadECUInfoThe0IsUnavailableError1 => ResourceManager.GetString("StringTable.MessageFormat_FailedToReadECUInfoThe0IsUnavailableError1");

	internal static string Message_ResettingCPCFaults => ResourceManager.GetString("StringTable.Message_ResettingCPCFaults");
}
