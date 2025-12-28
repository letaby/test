using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EHPS_Pumps__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_EHPS201TOffline => ResourceManager.GetString("StringTable.Message_EHPS201TOffline");

	internal static string Message_EHPS401TOffline => ResourceManager.GetString("StringTable.Message_EHPS401TOffline");

	internal static string Message_Null => ResourceManager.GetString("StringTable.Message_Null");

	internal static string Message_EHPS401TPumpTest => ResourceManager.GetString("StringTable.Message_EHPS401TPumpTest");

	internal static string MessageFormat_ServiceStarted01 => ResourceManager.GetString("StringTable.MessageFormat_ServiceStarted01");

	internal static string MessageFormat_ServiceCouldNotBeStarted01 => ResourceManager.GetString("StringTable.MessageFormat_ServiceCouldNotBeStarted01");

	internal static string Message_EHPS201TPumpTest => ResourceManager.GetString("StringTable.Message_EHPS201TPumpTest");
}
