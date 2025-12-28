using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_USB_stick_Flashing.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_MaxFunctionalServiceFailed0 => ResourceManager.GetString("StringTable.MessageFormat_MaxFunctionalServiceFailed0");

	internal static string Message_HardResetFailed => ResourceManager.GetString("StringTable.Message_HardResetFailed");

	internal static string MessageFormat_CanNotUpdateSoftwareVersion0 => ResourceManager.GetString("StringTable.MessageFormat_CanNotUpdateSoftwareVersion0");

	internal static string Message_SoftwareFlashingCompleteSoftwareVersionInvalid => ResourceManager.GetString("StringTable.Message_SoftwareFlashingCompleteSoftwareVersionInvalid");

	internal static string Message_ConnectedToCTP => ResourceManager.GetString("StringTable.Message_ConnectedToCTP");

	internal static string Message_WaitingForCTPToReset => ResourceManager.GetString("StringTable.Message_WaitingForCTPToReset");

	internal static string Message_ReadyToStart => ResourceManager.GetString("StringTable.Message_ReadyToStart");

	internal static string MessageFormat_FlashFailed0 => ResourceManager.GetString("StringTable.MessageFormat_FlashFailed0");

	internal static string Message_CouldNotStartReprogrammingCheckUSBDrive => ResourceManager.GetString("StringTable.Message_CouldNotStartReprogrammingCheckUSBDrive");

	internal static string Message_ResettingCTPBeforeFlashing => ResourceManager.GetString("StringTable.Message_ResettingCTPBeforeFlashing");

	internal static string Message_FlashingComplete => ResourceManager.GetString("StringTable.Message_FlashingComplete");

	internal static string Message_InvalidSoftwarePartNumber => ResourceManager.GetString("StringTable.Message_InvalidSoftwarePartNumber");

	internal static string MessageFormat_CouldNotStartReprogramming0 => ResourceManager.GetString("StringTable.MessageFormat_CouldNotStartReprogramming0");

	internal static string Message_Unlocking => ResourceManager.GetString("StringTable.Message_Unlocking");

	internal static string Message_Reprogramming => ResourceManager.GetString("StringTable.Message_Reprogramming");

	internal static string MessageFormat_ServiceDoesNotExist0 => ResourceManager.GetString("StringTable.MessageFormat_ServiceDoesNotExist0");

	internal static string MessageFormat_FlashFailed01 => ResourceManager.GetString("StringTable.MessageFormat_FlashFailed01");

	internal static string MessageFormat_FlashFailed02 => ResourceManager.GetString("StringTable.MessageFormat_FlashFailed02");

	internal static string Message_NotConnectedToCTP => ResourceManager.GetString("StringTable.Message_NotConnectedToCTP");

	internal static string Message_ResettingCTPAfterFlashing => ResourceManager.GetString("StringTable.Message_ResettingCTPAfterFlashing");

	internal static string Message_CTPBusy => ResourceManager.GetString("StringTable.Message_CTPBusy");

	internal static string Message_SoftwareFlashingCompleteSoftwareVersionValid => ResourceManager.GetString("StringTable.Message_SoftwareFlashingCompleteSoftwareVersionValid");

	internal static string Message_WaitingForCTPToReset1 => ResourceManager.GetString("StringTable.Message_WaitingForCTPToReset1");

	internal static string Message_CopyingData => ResourceManager.GetString("StringTable.Message_CopyingData");

	internal static string Message_InvalidHardwarePartNumber => ResourceManager.GetString("StringTable.Message_InvalidHardwarePartNumber");

	internal static string Message_Failed => ResourceManager.GetString("StringTable.Message_Failed");
}
