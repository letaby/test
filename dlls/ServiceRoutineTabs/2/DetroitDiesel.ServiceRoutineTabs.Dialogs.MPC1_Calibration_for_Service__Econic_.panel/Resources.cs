using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC1_Calibration_for_Service__Econic_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_EcuDisconnectedBeforeCompletion => ResourceManager.GetString("StringTable.Message_EcuDisconnectedBeforeCompletion");

	internal static string Message_VerifyingResult => ResourceManager.GetString("StringTable.Message_VerifyingResult");

	internal static string Message_UnlockingDevice => ResourceManager.GetString("StringTable.Message_UnlockingDevice");

	internal static string Message_Complete => ResourceManager.GetString("StringTable.Message_Complete");

	internal static string Message_ConfiguringStaticCalibration => ResourceManager.GetString("StringTable.Message_ConfiguringStaticCalibration");

	internal static string Message_ResettingDevice => ResourceManager.GetString("StringTable.Message_ResettingDevice");

	internal static string Message_SettingParameters => ResourceManager.GetString("StringTable.Message_SettingParameters");

	internal static string Message_ConfiguringOnlineCalibration => ResourceManager.GetString("StringTable.Message_ConfiguringOnlineCalibration");
}
