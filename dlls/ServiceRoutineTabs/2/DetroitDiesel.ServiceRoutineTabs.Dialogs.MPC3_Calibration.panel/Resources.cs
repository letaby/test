using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC3_Calibration.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ResettingFaultCodes => ResourceManager.GetString("StringTable.Message_ResettingFaultCodes");

	internal static string Button_StartTitle => ResourceManager.GetString("StringTable.Button_StartTitle");

	internal static string WarningManagerMessage => ResourceManager.GetString("StringTable.WarningManagerMessage");

	internal static string WarningManagerJobName => ResourceManager.GetString("StringTable.WarningManagerJobName");

	internal static string Message_CommittingCalibration => ResourceManager.GetString("StringTable.Message_CommittingCalibration");

	internal static string Message_Cancelled => ResourceManager.GetString("StringTable.Message_Cancelled");

	internal static string Message_CalibrationNotComplete => ResourceManager.GetString("StringTable.Message_CalibrationNotComplete");

	internal static string Messagre_CalibrationComplete => ResourceManager.GetString("StringTable.Messagre_CalibrationComplete");
}
