using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Reset_Crank_Inhibit__NGC_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_DisablingCrankInhibit => ResourceManager.GetString("StringTable.Message_DisablingCrankInhibit");

	internal static string Message_FlashingOverTheAirIsCurrentlyInProgress => ResourceManager.GetString("StringTable.Message_FlashingOverTheAirIsCurrentlyInProgress");

	internal static string Message_CrankingIsNotInhibited => ResourceManager.GetString("StringTable.Message_CrankingIsNotInhibited");

	internal static string Message_UnableToDisableCrankInhibit => ResourceManager.GetString("StringTable.Message_UnableToDisableCrankInhibit");

	internal static string Message_EcusMustBeConnectedToDisableCrankInhibit => ResourceManager.GetString("StringTable.Message_EcusMustBeConnectedToDisableCrankInhibit");

	internal static string Message_CrankInhibitWasDisabled => ResourceManager.GetString("StringTable.Message_CrankInhibitWasDisabled");

	internal static string Message_Ready => ResourceManager.GetString("StringTable.Message_Ready");
}
