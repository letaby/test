using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_MR2_for_Reprogramming.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ErrorUnlocking => ResourceManager.GetString("StringTable.Message_ErrorUnlocking");

	internal static string Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range => ResourceManager.GetString("StringTable.Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range");

	internal static string Message_RefreshingEcuData => ResourceManager.GetString("StringTable.Message_RefreshingEcuData");

	internal static string Message_ReadingLockConfigurationWait => ResourceManager.GetString("StringTable.Message_ReadingLockConfigurationWait");

	internal static string Message_UnlockingWait0 => ResourceManager.GetString("StringTable.Message_UnlockingWait0");

	internal static string Message_Unknown => ResourceManager.GetString("StringTable.Message_Unknown");

	internal static string Message_PleaseTypeInVeDocSKey => ResourceManager.GetString("StringTable.Message_PleaseTypeInVeDocSKey");

	internal static string Message_FailureReadingVeDocInputs => ResourceManager.GetString("StringTable.Message_FailureReadingVeDocInputs");

	internal static string Message_Done => ResourceManager.GetString("StringTable.Message_Done");

	internal static string Message_Closing => ResourceManager.GetString("StringTable.Message_Closing");

	internal static string Message_ReadingInputsWait => ResourceManager.GetString("StringTable.Message_ReadingInputsWait");

	internal static string Message_InitializingWait => ResourceManager.GetString("StringTable.Message_InitializingWait");

	internal static string Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range => ResourceManager.GetString("StringTable.Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range");
}
