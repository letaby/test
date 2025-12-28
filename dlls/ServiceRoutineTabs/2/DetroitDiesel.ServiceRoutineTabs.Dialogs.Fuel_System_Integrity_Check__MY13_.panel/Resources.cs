using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Fuel_System_Integrity_Check__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_Selected_Procedure_Aborted => ResourceManager.GetString("StringTable.Message_Selected_Procedure_Aborted");

	internal static string Message_System_Not_Leaking => ResourceManager.GetString("StringTable.Message_System_Not_Leaking");

	internal static string Message_System_Leaking => ResourceManager.GetString("StringTable.Message_System_Leaking");

	internal static string Message_Error_Reading_Values => ResourceManager.GetString("StringTable.Message_Error_Reading_Values");

	internal static string Message_Selected_Procedure_Canceled => ResourceManager.GetString("StringTable.Message_Selected_Procedure_Canceled");

	internal static string Message_Test_Not_Run => ResourceManager.GetString("StringTable.Message_Test_Not_Run");
}
