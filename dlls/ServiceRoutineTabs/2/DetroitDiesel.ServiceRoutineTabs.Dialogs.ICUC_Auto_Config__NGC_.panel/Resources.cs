using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ICUC_Auto_Config__NGC_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ICUCNotPerformed => ResourceManager.GetString("StringTable.Message_ICUCNotPerformed");

	internal static string Message_ICUCSMF => ResourceManager.GetString("StringTable.Message_ICUCSMF");
}
