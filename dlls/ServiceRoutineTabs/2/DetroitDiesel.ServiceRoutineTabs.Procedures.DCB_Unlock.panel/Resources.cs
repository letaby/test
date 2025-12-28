using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.DCB_Unlock.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_0UnlockFailed1 => ResourceManager.GetString("StringTable.MessageFormat_0UnlockFailed1");

	internal static string MessageFormat_0UnlockComplete => ResourceManager.GetString("StringTable.MessageFormat_0UnlockComplete");

	internal static string Message_Error => ResourceManager.GetString("StringTable.Message_Error");
}
