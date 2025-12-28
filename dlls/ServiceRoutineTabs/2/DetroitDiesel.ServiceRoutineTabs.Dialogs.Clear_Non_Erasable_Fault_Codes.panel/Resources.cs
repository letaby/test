using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clear_Non_Erasable_Fault_Codes.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_FailureReadingInputs => ResourceManager.GetString("StringTable.Message_FailureReadingInputs");

	internal static string Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range => ResourceManager.GetString("StringTable.Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range");

	internal static string Message_SwitchToServerUnlock => ResourceManager.GetString("StringTable.Message_SwitchToServerUnlock");

	internal static string Message_ClearingFaultCodesWait => ResourceManager.GetString("StringTable.Message_ClearingFaultCodesWait");

	internal static string StatusFormat => ResourceManager.GetString("StringTable.StatusFormat");

	internal static string Message_Unknown => ResourceManager.GetString("StringTable.Message_Unknown");

	internal static string Message_PleaseTypeInUnlockKey => ResourceManager.GetString("StringTable.Message_PleaseTypeInUnlockKey");

	internal static string Message_Done => ResourceManager.GetString("StringTable.Message_Done");

	internal static string Message_SwitchToManualUnlock => ResourceManager.GetString("StringTable.Message_SwitchToManualUnlock");

	internal static string Message_Closing => ResourceManager.GetString("StringTable.Message_Closing");

	internal static string Message_ReadyToClearFaults => ResourceManager.GetString("StringTable.Message_ReadyToClearFaults");

	internal static string Message_UnlockKeyIsIncorrect => ResourceManager.GetString("StringTable.Message_UnlockKeyIsIncorrect");

	internal static string Message_ReadingInputsWait => ResourceManager.GetString("StringTable.Message_ReadingInputsWait");

	internal static string Message_InitializingWait => ResourceManager.GetString("StringTable.Message_InitializingWait");

	internal static string Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range => ResourceManager.GetString("StringTable.Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range");
}
