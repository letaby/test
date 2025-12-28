using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PLV_Change.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_AreYouSureYouWantToResetTheLearnedData0 => ResourceManager.GetString("StringTable.Message_AreYouSureYouWantToResetTheLearnedData0");
}
