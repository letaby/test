using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Maximum_Sensor_Value_Reset.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_AnErrorOccurredDuringTheReset0 => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredDuringTheReset0");

	internal static string MessageFormat_ResetExecuted0 => ResourceManager.GetString("StringTable.MessageFormat_ResetExecuted0");

	internal static string Message_Executing => ResourceManager.GetString("StringTable.Message_Executing");

	internal static string Message_PerformSensorValueReset => ResourceManager.GetString("StringTable.Message_PerformSensorValueReset");

	internal static string Message_ResetExecutedSuccessfully => ResourceManager.GetString("StringTable.Message_ResetExecutedSuccessfully");

	internal static string Message_PleaseConnectTheMCM => ResourceManager.GetString("StringTable.Message_PleaseConnectTheMCM");

	internal static string Message_MCMIsBusy => ResourceManager.GetString("StringTable.Message_MCMIsBusy");

	internal static string Message_UnableToAcquireTheResetServiceATDMaximumSensorValuesCannotBeReset => ResourceManager.GetString("StringTable.Message_UnableToAcquireTheResetServiceATDMaximumSensorValuesCannotBeReset");

	internal static string Message_MCMIsConnected => ResourceManager.GetString("StringTable.Message_MCMIsConnected");
}
