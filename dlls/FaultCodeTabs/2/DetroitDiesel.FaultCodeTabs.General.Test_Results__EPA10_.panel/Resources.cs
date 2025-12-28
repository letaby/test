using System.ComponentModel;

namespace DetroitDiesel.FaultCodeTabs.General.Test_Results__EPA10_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_Standard => ResourceManager.GetString("StringTable.Message_Standard");

	internal static string Message_TestResultsData => ResourceManager.GetString("StringTable.Message_TestResultsData");
}
