using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Quantity_Test__EPA10_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_DEFDosingQuantityCheckCompleted => ResourceManager.GetString("StringTable.Message_DEFDosingQuantityCheckCompleted");

	internal static string Message_NoResultsAvailable => ResourceManager.GetString("StringTable.Message_NoResultsAvailable");

	internal static string Message_DEFDosingQuantityCheckFailedOrTerminatedWithUnknownResult => ResourceManager.GetString("StringTable.Message_DEFDosingQuantityCheckFailedOrTerminatedWithUnknownResult");
}
