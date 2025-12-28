using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.FICM_Routine.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_Test_Not_Run => ResourceManager.GetString("StringTable.Message_Test_Not_Run");

	internal static string Message_Stopped => ResourceManager.GetString("StringTable.Message_Stopped");

	internal static string Message_Success => ResourceManager.GetString("StringTable.Message_Success");
}
