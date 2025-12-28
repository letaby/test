using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_Status => ResourceManager.GetString("StringTable.Message_Status");

	internal static string Message_Started => ResourceManager.GetString("StringTable.Message_Started");

	internal static string Message_Stopped => ResourceManager.GetString("StringTable.Message_Stopped");

	internal static string Message_Error => ResourceManager.GetString("StringTable.Message_Error");

	internal static string Message_CompleteSuccess => ResourceManager.GetString("StringTable.Message_CompleteSuccess");
}
