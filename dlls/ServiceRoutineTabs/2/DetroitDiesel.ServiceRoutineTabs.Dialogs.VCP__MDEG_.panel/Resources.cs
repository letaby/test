using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VCP__MDEG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_StopVCPTest => ResourceManager.GetString("StringTable.Message_StopVCPTest");
}
