using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration__EPA10_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_EngineIsNotRunning => ResourceManager.GetString("StringTable.Message_EngineIsNotRunning");

	internal static string Message_EngineSpeedCannotBeDetected => ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetected");

	internal static string MessageFormat_RecalibrationExecuted => ResourceManager.GetString("StringTable.MessageFormat_RecalibrationExecuted");

	internal static string Message_PerformEGRDPSensorRecalibration => ResourceManager.GetString("StringTable.Message_PerformEGRDPSensorRecalibration");

	internal static string Message_EngineIsRunning => ResourceManager.GetString("StringTable.Message_EngineIsRunning");

	internal static string Message_MCM2IsNotConnected => ResourceManager.GetString("StringTable.Message_MCM2IsNotConnected");

	internal static string Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated => ResourceManager.GetString("StringTable.Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated");

	internal static string Message_MCM2IsConnected => ResourceManager.GetString("StringTable.Message_MCM2IsConnected");

	internal static string Message_MCM2IsBusy => ResourceManager.GetString("StringTable.Message_MCM2IsBusy");

	internal static string MessageFormat_AnErrorOccurredDuringRecalibration0 => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredDuringRecalibration0");
}
