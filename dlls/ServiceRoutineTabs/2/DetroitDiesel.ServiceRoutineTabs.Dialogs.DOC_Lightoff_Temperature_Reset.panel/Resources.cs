using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Lightoff_Temperature_Reset.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ProcedureReady => ResourceManager.GetString("StringTable.Message_ProcedureReady");

	internal static string Message_TurnEngineOff => ResourceManager.GetString("StringTable.Message_TurnEngineOff");

	internal static string Message_TheValueWasSuccessfullyChanged => ResourceManager.GetString("StringTable.Message_TheValueWasSuccessfullyChanged");
}
