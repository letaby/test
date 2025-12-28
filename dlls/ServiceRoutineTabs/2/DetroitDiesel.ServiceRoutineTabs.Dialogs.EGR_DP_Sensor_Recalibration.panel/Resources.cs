using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_MCMIsBusy => ResourceManager.GetString("StringTable.Message_MCMIsBusy");

	internal static string Message_MCMIsConnected => ResourceManager.GetString("StringTable.Message_MCMIsConnected");

	internal static string Message_MCMIsNotConnected => ResourceManager.GetString("StringTable.Message_MCMIsNotConnected");

	internal static string Message_EngineIsRunning => ResourceManager.GetString("StringTable.Message_EngineIsRunning");

	internal static string Message_EngineIsNotRunning => ResourceManager.GetString("StringTable.Message_EngineIsNotRunning");

	internal static string Message_EngineSpeedCannotBeDetected => ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetected");

	internal static string Message_PerformEGRDPSensorRecalibration => ResourceManager.GetString("StringTable.Message_PerformEGRDPSensorRecalibration");

	internal static string Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated => ResourceManager.GetString("StringTable.Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated");

	internal static string MessageFormat_AnErrorOccurredDuringRecalibration0 => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredDuringRecalibration0");

	internal static string MessageFormat_RecalibrationExecuted0 => ResourceManager.GetString("StringTable.MessageFormat_RecalibrationExecuted0");
}
