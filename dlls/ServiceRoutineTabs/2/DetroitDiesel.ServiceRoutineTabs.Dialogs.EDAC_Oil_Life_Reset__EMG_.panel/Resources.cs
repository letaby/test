using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EDAC_Oil_Life_Reset__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TheResetOperationSucceeded => ResourceManager.GetString("StringTable.Message_TheResetOperationSucceeded");

	internal static string Message_TheResetOperationFailed => ResourceManager.GetString("StringTable.Message_TheResetOperationFailed");
}
