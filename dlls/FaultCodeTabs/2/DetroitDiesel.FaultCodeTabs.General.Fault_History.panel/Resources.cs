using System.ComponentModel;

namespace DetroitDiesel.FaultCodeTabs.General.Fault_History.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_NoConnectedVIN1 => ResourceManager.GetString("StringTable.Message_NoConnectedVIN1");

	internal static string Message_NoConnectedVIN => ResourceManager.GetString("StringTable.Message_NoConnectedVIN");

	internal static string Message_YouAreNotConnected => ResourceManager.GetString("StringTable.Message_YouAreNotConnected");

	internal static string Message_ConnectedVIN => ResourceManager.GetString("StringTable.Message_ConnectedVIN");

	internal static string Message_ConnectedVIN1 => ResourceManager.GetString("StringTable.Message_ConnectedVIN1");
}
