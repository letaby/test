using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_EngineIsNotRunning => ResourceManager.GetString("StringTable.Message_EngineIsNotRunning");

	internal static string MessageFormat_AnErrorOccurredDuringRecalibration => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredDuringRecalibration");

	internal static string Message_MCM21TIsBusy => ResourceManager.GetString("StringTable.Message_MCM21TIsBusy");

	internal static string Message_EngineSpeedCannotBeDetected => ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetected");

	internal static string MessageFormat_RecalibrationExecuted0 => ResourceManager.GetString("StringTable.MessageFormat_RecalibrationExecuted0");

	internal static string Message_PerformEGRDPSensorRecalibration => ResourceManager.GetString("StringTable.Message_PerformEGRDPSensorRecalibration");

	internal static string Message_EngineIsRunning => ResourceManager.GetString("StringTable.Message_EngineIsRunning");

	internal static string Message_MCM21TIsNotConnected => ResourceManager.GetString("StringTable.Message_MCM21TIsNotConnected");

	internal static string Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated => ResourceManager.GetString("StringTable.Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated");

	internal static string Message_MCM21TIsConnected => ResourceManager.GetString("StringTable.Message_MCM21TIsConnected");
}
