using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EBS_Activation__Econic_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string ErrorUnknown => ResourceManager.GetString("StringTable.ErrorUnknown");
}
