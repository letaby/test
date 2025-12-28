using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Left_Calibration__NGC_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ResettingFaultCodes => ResourceManager.GetString("StringTable.Message_ResettingFaultCodes");
}
