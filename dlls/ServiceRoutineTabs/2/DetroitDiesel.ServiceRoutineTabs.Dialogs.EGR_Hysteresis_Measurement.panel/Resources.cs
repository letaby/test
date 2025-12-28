using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Hysteresis_Measurement.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_StopHysteresisTest => ResourceManager.GetString("StringTable.Message_StopHysteresisTest");
}
