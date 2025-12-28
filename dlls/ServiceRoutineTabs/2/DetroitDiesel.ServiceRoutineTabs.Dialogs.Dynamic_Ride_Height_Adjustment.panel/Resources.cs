using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Adjustment.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_Stationary => ResourceManager.GetString("StringTable.Message_Stationary");

	internal static string MessageFormat_ServiceCannotBeStarted0 => ResourceManager.GetString("StringTable.MessageFormat_ServiceCannotBeStarted0");

	internal static string Message_Raising => ResourceManager.GetString("StringTable.Message_Raising");

	internal static string Message_Lowering => ResourceManager.GetString("StringTable.Message_Lowering");

	internal static string Message_Up => ResourceManager.GetString("StringTable.Message_Up");

	internal static string Message_Stop => ResourceManager.GetString("StringTable.Message_Stop");

	internal static string Message_Down => ResourceManager.GetString("StringTable.Message_Down");

	internal static string Message_SNA => ResourceManager.GetString("StringTable.Message_SNA");

	internal static string Message_WARNING => ResourceManager.GetString("StringTable.Message_WARNING");
}
