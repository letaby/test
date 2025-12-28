using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_ADS_Self_Check__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_NoResultAvailable => ResourceManager.GetString("StringTable.Message_NoResultAvailable");
}
