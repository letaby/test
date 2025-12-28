using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Countershaft_Brake_Test.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageCounterShaftBrakeValueUnavailable => ResourceManager.GetString("StringTable.MessageCounterShaftBrakeValueUnavailable");

	internal static string MessageTimeout => ResourceManager.GetString("StringTable.MessageTimeout");
}
