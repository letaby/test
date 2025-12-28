using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Replace_Quantity_Control_Valve.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_AreYouSureYouWantToResetTheLearnedData => ResourceManager.GetString("StringTable.Message_AreYouSureYouWantToResetTheLearnedData");
}
