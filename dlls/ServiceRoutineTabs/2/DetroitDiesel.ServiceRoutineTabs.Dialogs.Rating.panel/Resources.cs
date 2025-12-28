using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Rating.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_DeviceWasNotUnlockedByUser => ResourceManager.GetString("StringTable.Message_DeviceWasNotUnlockedByUser");

	internal static string Message_AcquiringDeviceLockStatus => ResourceManager.GetString("StringTable.Message_AcquiringDeviceLockStatus");

	internal static string MessageFormat_Power012Torque34 => ResourceManager.GetString("StringTable.MessageFormat_Power012Torque34");

	internal static string Message_DeviceWasUnlocked => ResourceManager.GetString("StringTable.Message_DeviceWasUnlocked");

	internal static string MessageFormat_Power01Torque231 => ResourceManager.GetString("StringTable.MessageFormat_Power01Torque231");

	internal static string Message_ErrorWhileUnlockingDevice => ResourceManager.GetString("StringTable.Message_ErrorWhileUnlockingDevice");

	internal static string Message_ErrorWhileSendingSetting => ResourceManager.GetString("StringTable.Message_ErrorWhileSendingSetting");

	internal static string Message_ErrorWhileReadingSetting => ResourceManager.GetString("StringTable.Message_ErrorWhileReadingSetting");

	internal static string MessageFormat_Power01Torque23 => ResourceManager.GetString("StringTable.MessageFormat_Power01Torque23");

	internal static string Message_DeviceIsUnlocked => ResourceManager.GetString("StringTable.Message_DeviceIsUnlocked");

	internal static string Message_DeviceIsLocked => ResourceManager.GetString("StringTable.Message_DeviceIsLocked");

	internal static string Message_ReadingSettingFromCPC2 => ResourceManager.GetString("StringTable.Message_ReadingSettingFromCPC2");

	internal static string Message_SendingSettingToCPC2 => ResourceManager.GetString("StringTable.Message_SendingSettingToCPC2");

	internal static string Message_SettingSuccessfullyRead => ResourceManager.GetString("StringTable.Message_SettingSuccessfullyRead");

	internal static string Message_SettingSuccessfullySent0 => ResourceManager.GetString("StringTable.Message_SettingSuccessfullySent0");
}
