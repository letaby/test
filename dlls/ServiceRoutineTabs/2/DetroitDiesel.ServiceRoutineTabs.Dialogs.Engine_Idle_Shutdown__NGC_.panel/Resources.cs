using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown__NGC_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_DeviceWasNotUnlockedByUser => ResourceManager.GetString("StringTable.Message_DeviceWasNotUnlockedByUser");

	internal static string Message_AcquiringDeviceLockStatus => ResourceManager.GetString("StringTable.Message_AcquiringDeviceLockStatus");

	internal static string Message_DeviceWasUnlocked => ResourceManager.GetString("StringTable.Message_DeviceWasUnlocked");

	internal static string Message_ErrorWhileUnlockingDevice => ResourceManager.GetString("StringTable.Message_ErrorWhileUnlockingDevice");

	internal static string Message_StatusOffline => ResourceManager.GetString("StringTable.Message_StatusOffline");

	internal static string Message_ErrorWritingParameter => ResourceManager.GetString("StringTable.Message_ErrorWritingParameter");

	internal static string Message_ReadingParameter => ResourceManager.GetString("StringTable.Message_ReadingParameter");

	internal static string Message_StatusOther => ResourceManager.GetString("StringTable.Message_StatusOther");

	internal static string Message_ErrorReadingParameter => ResourceManager.GetString("StringTable.Message_ErrorReadingParameter");

	internal static string Message_StatusActive => ResourceManager.GetString("StringTable.Message_StatusActive");

	internal static string Message_WritingParameter => ResourceManager.GetString("StringTable.Message_WritingParameter");

	internal static string Message_DeviceIsUnlocked => ResourceManager.GetString("StringTable.Message_DeviceIsUnlocked");

	internal static string Message_DeviceIsLocked => ResourceManager.GetString("StringTable.Message_DeviceIsLocked");

	internal static string Message_StatusNotActive => ResourceManager.GetString("StringTable.Message_StatusNotActive");

	internal static string Message_CannotClose => ResourceManager.GetString("StringTable.Message_CannotClose");
}
