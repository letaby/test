using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN_OEM__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_HasNotBeenChangedAndHasAValueOf => ResourceManager.GetString("StringTable.Message_HasNotBeenChangedAndHasAValueOf");

	internal static string Message_ConnectToServer => ResourceManager.GetString("StringTable.Message_ConnectToServer");

	internal static string Message_ErrorReadingParameters => ResourceManager.GetString("StringTable.Message_ErrorReadingParameters");

	internal static string Message_ParametersReadSuccessfully => ResourceManager.GetString("StringTable.Message_ParametersReadSuccessfully");

	internal static string Message_ReadParametersAndUpload => ResourceManager.GetString("StringTable.Message_ReadParametersAndUpload");

	internal static string Message_UnlockingParameterWriteService1 => ResourceManager.GetString("StringTable.Message_UnlockingParameterWriteService1");

	internal static string Message_WritingChangesToPermanentMemory => ResourceManager.GetString("StringTable.Message_WritingChangesToPermanentMemory");

	internal static string Message_ExecutingServices => ResourceManager.GetString("StringTable.Message_ExecutingServices");

	internal static string Message_Executing => ResourceManager.GetString("StringTable.Message_Executing");

	internal static string Message_UnlockingParameterWriteService => ResourceManager.GetString("StringTable.Message_UnlockingParameterWriteService");

	internal static string Message_FailedToReadECUInfoThe => ResourceManager.GetString("StringTable.Message_FailedToReadECUInfoThe");

	internal static string Message_SkippingMCM21TSetVINProcessAsTheMCM21TIsNotConnected => ResourceManager.GetString("StringTable.Message_SkippingMCM21TSetVINProcessAsTheMCM21TIsNotConnected");

	internal static string Message_TheMCM21TWasDisconnected => ResourceManager.GetString("StringTable.Message_TheMCM21TWasDisconnected");

	internal static string Message_WaitingForTheCPC04TToComeBackOnline => ResourceManager.GetString("StringTable.Message_WaitingForTheCPC04TToComeBackOnline");

	internal static string Message_SkippingMCM21TCommitProcessAsTheServiceCouldNotBeFound => ResourceManager.GetString("StringTable.Message_SkippingMCM21TCommitProcessAsTheServiceCouldNotBeFound");

	internal static string Message_SkippingMCM21TParameterWriteUnlockingAsTheServiceCouldNotBeFound => ResourceManager.GetString("StringTable.Message_SkippingMCM21TParameterWriteUnlockingAsTheServiceCouldNotBeFound");

	internal static string Message_FailedToObtainService => ResourceManager.GetString("StringTable.Message_FailedToObtainService");

	internal static string Message_HasSuccessfullyBeenSetTo => ResourceManager.GetString("StringTable.Message_HasSuccessfullyBeenSetTo");

	internal static string Message_SkippingACM21TSetVINProcessAsTheACM21TIsNotConnected => ResourceManager.GetString("StringTable.Message_SkippingACM21TSetVINProcessAsTheACM21TIsNotConnected");

	internal static string Message_SynchronizingESNToCPC04TViaKeyOffOnReset => ResourceManager.GetString("StringTable.Message_SynchronizingESNToCPC04TViaKeyOffOnReset");

	internal static string Message_TheProcedureFailedToComplete => ResourceManager.GetString("StringTable.Message_TheProcedureFailedToComplete");

	internal static string Message_SettingVINTo1 => ResourceManager.GetString("StringTable.Message_SettingVINTo1");

	internal static string Message_SettingVINTo2 => ResourceManager.GetString("StringTable.Message_SettingVINTo2");

	internal static string Message_The1 => ResourceManager.GetString("StringTable.Message_The1");

	internal static string Message_The2 => ResourceManager.GetString("StringTable.Message_The2");

	internal static string Message_SkippingSettingOfACM21TVINAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingSettingOfACM21TVINAsTheServiceCannotBeFound");

	internal static string Message_FailedToExecuteService => ResourceManager.GetString("StringTable.Message_FailedToExecuteService");

	internal static string Message_CannotBeVerified => ResourceManager.GetString("StringTable.Message_CannotBeVerified");

	internal static string Message_SettingESNTo => ResourceManager.GetString("StringTable.Message_SettingESNTo");

	internal static string Message_SettingVINTo => ResourceManager.GetString("StringTable.Message_SettingVINTo");

	internal static string Message_ReadingParameters => ResourceManager.GetString("StringTable.Message_ReadingParameters");

	internal static string Message_The => ResourceManager.GetString("StringTable.Message_The");

	internal static string Message_SkippingCPC04TResetAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingCPC04TResetAsTheServiceCannotBeFound");

	internal static string Message_IsUnavailableError => ResourceManager.GetString("StringTable.Message_IsUnavailableError");

	internal static string Message_ReadParameters => ResourceManager.GetString("StringTable.Message_ReadParameters");

	internal static string Message_SkippingSettingOfMCM21TVINAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingSettingOfMCM21TVINAsTheServiceCannotBeFound");

	internal static string Message_SkippingSettingOfCPC04TVINAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingSettingOfCPC04TVINAsTheServiceCannotBeFound");

	internal static string Message_Error => ResourceManager.GetString("StringTable.Message_Error");

	internal static string Message_FailedToReadECUInfoCannotVerifyThe => ResourceManager.GetString("StringTable.Message_FailedToReadECUInfoCannotVerifyThe");

	internal static string Message_UseESNAsVIN => ResourceManager.GetString("StringTable.Message_UseESNAsVIN");

	internal static string Message_SkippingACM21TParameterWriteUnlockingAsTheServiceCouldNotBeFound => ResourceManager.GetString("StringTable.Message_SkippingACM21TParameterWriteUnlockingAsTheServiceCouldNotBeFound");

	internal static string Message_SkippingCPC04TCommitProcessAsTheCPC04TIsNotConnected => ResourceManager.GetString("StringTable.Message_SkippingCPC04TCommitProcessAsTheCPC04TIsNotConnected");

	internal static string Message_SkippingCPC04TSetVINProcessAsTheCPC04TIsNotConnected => ResourceManager.GetString("StringTable.Message_SkippingCPC04TSetVINProcessAsTheCPC04TIsNotConnected");
}
