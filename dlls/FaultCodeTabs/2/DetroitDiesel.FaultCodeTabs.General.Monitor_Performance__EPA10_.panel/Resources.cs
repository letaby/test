using System.ComponentModel;

namespace DetroitDiesel.FaultCodeTabs.General.Monitor_Performance__EPA10_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_Standard => ResourceManager.GetString("StringTable.Message_Standard");

	internal static string Message_MonitorPerformanceData => ResourceManager.GetString("StringTable.Message_MonitorPerformanceData");

	internal static string Message_ReadinessState => ResourceManager.GetString("StringTable.Message_ReadinessState");
}
