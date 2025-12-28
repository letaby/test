using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Replacement__MY16_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TheRoutineWasSuccessful => ResourceManager.GetString("StringTable.Message_TheRoutineWasSuccessful");
}
