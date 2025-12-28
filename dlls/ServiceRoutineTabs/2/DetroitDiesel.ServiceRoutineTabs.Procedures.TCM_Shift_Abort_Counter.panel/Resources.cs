using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Shift_Abort_Counter.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_CompletedSuccessfully => ResourceManager.GetString("StringTable.Message_CompletedSuccessfully");

	internal static string Message_Failed0 => ResourceManager.GetString("StringTable.Message_Failed0");
}
