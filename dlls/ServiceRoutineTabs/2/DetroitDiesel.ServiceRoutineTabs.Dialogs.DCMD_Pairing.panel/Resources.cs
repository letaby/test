using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DCMD_Pairing.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_CannotReloadEcuInfo => ResourceManager.GetString("StringTable.Message_CannotReloadEcuInfo");

	internal static string Message_NoKeyfobsChanged => ResourceManager.GetString("StringTable.Message_NoKeyfobsChanged");

	internal static string Message_TheProcedureCanStart => ResourceManager.GetString("StringTable.Message_TheProcedureCanStart");

	internal static string Message_ValuesRead => ResourceManager.GetString("StringTable.Message_ValuesRead");

	internal static string Message_InvalidKeyfob => ResourceManager.GetString("StringTable.Message_InvalidKeyfob");

	internal static string Message_PreconditionsNotMet => ResourceManager.GetString("StringTable.Message_PreconditionsNotMet");

	internal static string Message_DuplicateKeyfob => ResourceManager.GetString("StringTable.Message_DuplicateKeyfob");

	internal static string Message_ValuesWritten => ResourceManager.GetString("StringTable.Message_ValuesWritten");

	internal static string Message_ReadingValues => ResourceManager.GetString("StringTable.Message_ReadingValues");

	internal static string MessageFormat_CanNotStart0 => ResourceManager.GetString("StringTable.MessageFormat_CanNotStart0");

	internal static string Message_ResettingUnit => ResourceManager.GetString("StringTable.Message_ResettingUnit");

	internal static string Message_WritingValues => ResourceManager.GetString("StringTable.Message_WritingValues");

	internal static string MessageFormat_0Failed => ResourceManager.GetString("StringTable.MessageFormat_0Failed");
}
