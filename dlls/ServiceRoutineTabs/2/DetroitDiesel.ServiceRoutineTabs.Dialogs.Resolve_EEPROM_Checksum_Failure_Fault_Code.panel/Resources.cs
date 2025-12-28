using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Resolve_EEPROM_Checksum_Failure_Fault_Code.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TheCPC2IsNotOnlineSoFaultCodesCouldNotBeRead => ResourceManager.GetString("StringTable.Message_TheCPC2IsNotOnlineSoFaultCodesCouldNotBeRead");

	internal static string Message_ProcessingPleaseWait => ResourceManager.GetString("StringTable.Message_ProcessingPleaseWait");

	internal static string Message_TheEEPROMChecksumFailureFaultCodeIsPresentAndMustBeCleared => ResourceManager.GetString("StringTable.Message_TheEEPROMChecksumFailureFaultCodeIsPresentAndMustBeCleared");

	internal static string Message_TheEEPROMChecksumFailureFaultCodeIsNotPresentNoActionIsNecessary => ResourceManager.GetString("StringTable.Message_TheEEPROMChecksumFailureFaultCodeIsNotPresentNoActionIsNecessary");

	internal static string MessageFormat_PreparingToWriteBack0Parameters => ResourceManager.GetString("StringTable.MessageFormat_PreparingToWriteBack0Parameters");

	internal static string Message_UpdateSeed => ResourceManager.GetString("StringTable.Message_UpdateSeed");

	internal static string Message_AssigningParameters => ResourceManager.GetString("StringTable.Message_AssigningParameters");

	internal static string Message_ReadingDefaultParametersAfterReset => ResourceManager.GetString("StringTable.Message_ReadingDefaultParametersAfterReset");

	internal static string Message_PerformingEEPROMReset => ResourceManager.GetString("StringTable.Message_PerformingEEPROMReset");

	internal static string Message_PreparingForEEPROMReset => ResourceManager.GetString("StringTable.Message_PreparingForEEPROMReset");

	internal static string Message_ReadingParameters => ResourceManager.GetString("StringTable.Message_ReadingParameters");

	internal static string MessageFormat_Writing0Parameters => ResourceManager.GetString("StringTable.MessageFormat_Writing0Parameters");

	internal static string Message_TheProcedureFailedToComplete => ResourceManager.GetString("StringTable.Message_TheProcedureFailedToComplete");

	internal static string Message_Retry => ResourceManager.GetString("StringTable.Message_Retry");

	internal static string Message_Start => ResourceManager.GetString("StringTable.Message_Start");

	internal static string Message_Complete => ResourceManager.GetString("StringTable.Message_Complete");

	internal static string Message_NoParametersAreChangedFromDefaultUseProgramDeviceToRestoreServerConfiguration => ResourceManager.GetString("StringTable.Message_NoParametersAreChangedFromDefaultUseProgramDeviceToRestoreServerConfiguration");

	internal static string Message_UnknownStage => ResourceManager.GetString("StringTable.Message_UnknownStage");

	internal static string Message_FailedToUnlock => ResourceManager.GetString("StringTable.Message_FailedToUnlock");

	internal static string Message_FailedToObtainService => ResourceManager.GetString("StringTable.Message_FailedToObtainService");

	internal static string Message_FailedToReadExistingParameters => ResourceManager.GetString("StringTable.Message_FailedToReadExistingParameters");

	internal static string Message_FailedToExecuteService => ResourceManager.GetString("StringTable.Message_FailedToExecuteService");

	internal static string Message_TheCPC2WasDisconnected => ResourceManager.GetString("StringTable.Message_TheCPC2WasDisconnected");
}
