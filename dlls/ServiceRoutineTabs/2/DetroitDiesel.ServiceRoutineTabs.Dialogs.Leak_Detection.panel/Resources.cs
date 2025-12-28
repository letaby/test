using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Leak_Detection.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_AreYouSureYouWantToResetTheLearnedData => ResourceManager.GetString("StringTable.Message_AreYouSureYouWantToResetTheLearnedData");

	internal static string Message_AreYouSure => ResourceManager.GetString("StringTable.Message_AreYouSure");
}
