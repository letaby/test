using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment__45X_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string WarningManagerMessage => ResourceManager.GetString("StringTable.WarningManagerMessage");

	internal static string WarningManagerJobName => ResourceManager.GetString("StringTable.WarningManagerJobName");

	internal static string Message_Cancelled => ResourceManager.GetString("StringTable.Message_Cancelled");
}
