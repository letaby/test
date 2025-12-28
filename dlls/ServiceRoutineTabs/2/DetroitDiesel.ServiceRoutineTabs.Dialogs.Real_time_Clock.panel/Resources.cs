using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet => ResourceManager.GetString("StringTable.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet");

	internal static string Message_NoteThatTheRealTimeClockValuesAreDisplayedInTheComputerSLocalTimeZone => ResourceManager.GetString("StringTable.Message_NoteThatTheRealTimeClockValuesAreDisplayedInTheComputerSLocalTimeZone");

	internal static string Message_Invalid => ResourceManager.GetString("StringTable.Message_Invalid");

	internal static string Message_Finished => ResourceManager.GetString("StringTable.Message_Finished");

	internal static string Message_Unavailable => ResourceManager.GetString("StringTable.Message_Unavailable");

	internal static string MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock0 => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock0");

	internal static string MessageFormat_SettingRealTimeClockTo01 => ResourceManager.GetString("StringTable.MessageFormat_SettingRealTimeClockTo01");
}
