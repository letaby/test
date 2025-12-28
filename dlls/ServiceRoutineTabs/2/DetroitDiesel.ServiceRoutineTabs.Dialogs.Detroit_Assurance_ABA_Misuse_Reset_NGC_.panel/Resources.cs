using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_ABA_Misuse_Reset_NGC_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_TheFaultHasBeenReset0 => ResourceManager.GetString("StringTable.MessageFormat_TheFaultHasBeenReset0");

	internal static string MessageFormat_UnableToResetError0 => ResourceManager.GetString("StringTable.MessageFormat_UnableToResetError0");

	internal static string Message_TheFaultCannotBeResetBecauseTheVRDUIsOffline => ResourceManager.GetString("StringTable.Message_TheFaultCannotBeResetBecauseTheVRDUIsOffline");

	internal static string Message_Ready => ResourceManager.GetString("StringTable.Message_Ready");

	internal static string Message_ResettingTheFault => ResourceManager.GetString("StringTable.Message_ResettingTheFault");
}
