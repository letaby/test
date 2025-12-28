using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Calibration__45X_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ResettingFaultCodes => ResourceManager.GetString("StringTable.Message_ResettingFaultCodes");

	internal static string Message_MessageText => ResourceManager.GetString("StringTable.Message_MessageText");

	internal static string Message_AuthenticationStateChanged => ResourceManager.GetString("StringTable.Message_AuthenticationStateChanged");

	internal static string Message_UnableToChangeAuthhenticatedState => ResourceManager.GetString("StringTable.Message_UnableToChangeAuthhenticatedState");

	internal static string Message_DynamicCalibrationSDAStopped => ResourceManager.GetString("StringTable.Message_DynamicCalibrationSDAStopped");

	internal static string Message_DynamicCalibrationSDAStarted => ResourceManager.GetString("StringTable.Message_DynamicCalibrationSDAStarted");

	internal static string Message_UnableToStopDynamicCalibrationSDA => ResourceManager.GetString("StringTable.Message_UnableToStopDynamicCalibrationSDA");

	internal static string MessageFormat_StatusMessage => ResourceManager.GetString("StringTable.MessageFormat_StatusMessage");
}
