using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock__NGC_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet => ResourceManager.GetString("StringTable.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet");

	internal static string MessageFormat_SettingRealTimeClockTo => ResourceManager.GetString("StringTable.MessageFormat_SettingRealTimeClockTo");

	internal static string Message_Finished => ResourceManager.GetString("StringTable.Message_Finished");

	internal static string MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock01PleaseCycleTheIgnitionAndTryAgain1IfTheEngineIsRunningPleaseStopTheEngineBeforeRetrying => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock01PleaseCycleTheIgnitionAndTryAgain1IfTheEngineIsRunningPleaseStopTheEngineBeforeRetrying");

	internal static string Message_ClockResetWarning => ResourceManager.GetString("StringTable.Message_ClockResetWarning");
}
