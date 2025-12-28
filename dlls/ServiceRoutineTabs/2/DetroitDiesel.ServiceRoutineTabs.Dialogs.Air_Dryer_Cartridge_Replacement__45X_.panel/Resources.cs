using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Air_Dryer_Cartridge_Replacement__45X_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TheResetOperationSucceeded => ResourceManager.GetString("StringTable.Message_TheResetOperationSucceeded");

	internal static string Message_TheResetOperationFailed => ResourceManager.GetString("StringTable.Message_TheResetOperationFailed");
}
