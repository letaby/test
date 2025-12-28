using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Learn_Procedure.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_PleaseEnterANumericValue => ResourceManager.GetString("StringTable.Message_PleaseEnterANumericValue");

	internal static string Message_ProcedureWasNotRun => ResourceManager.GetString("StringTable.Message_ProcedureWasNotRun");
}
