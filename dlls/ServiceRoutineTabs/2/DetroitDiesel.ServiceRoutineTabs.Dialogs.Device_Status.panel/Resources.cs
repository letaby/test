using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Device_Status.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_AllFieldsMustBeFilledIn => ResourceManager.GetString("StringTable.Message_AllFieldsMustBeFilledIn");
}
