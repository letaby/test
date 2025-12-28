using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning__EPA10_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_PressAndHoldTheRegenSwitchOnTheDashForFiveSecondsToStartTheRegen => ResourceManager.GetString("StringTable.Message_PressAndHoldTheRegenSwitchOnTheDashForFiveSecondsToStartTheRegen");

	internal static string Message_NotAvailable => ResourceManager.GetString("StringTable.Message_NotAvailable");
}
