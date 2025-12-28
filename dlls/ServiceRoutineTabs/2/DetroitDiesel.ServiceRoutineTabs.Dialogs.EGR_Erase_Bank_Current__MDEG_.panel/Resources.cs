using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Erase_Bank_Current__MDEG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Label_ClickButtonToEraseBank => ResourceManager.GetString("StringTable.Label_ClickButtonToEraseBank");

	internal static string Message_SuccessfulExecution => ResourceManager.GetString("StringTable.Message_SuccessfulExecution");

	internal static string Message_Error => ResourceManager.GetString("StringTable.Message_Error");

	internal static string Message_FailedExecution => ResourceManager.GetString("StringTable.Message_FailedExecution");
}
