using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_CPC_Odometer__GHG14_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_WritingOdometerValue => ResourceManager.GetString("StringTable.Message_WritingOdometerValue");

	internal static string Message_TheOdometerCannotBeSetBecauseTheProposedValueIsEffectivelyEqualToCurrentOdometer => ResourceManager.GetString("StringTable.Message_TheOdometerCannotBeSetBecauseTheProposedValueIsEffectivelyEqualToCurrentOdometer");

	internal static string Message_ErrorTheOdometerCannotBeSetBecauseTheProposedValueIsLessThanTheCurrentOdometer => ResourceManager.GetString("StringTable.Message_ErrorTheOdometerCannotBeSetBecauseTheProposedValueIsLessThanTheCurrentOdometer");

	internal static string MessageFormat_UnsupportedSoftwareVersion => ResourceManager.GetString("StringTable.MessageFormat_UnsupportedSoftwareVersion");

	internal static string Message_TheOdometerIsReadyToBeSetBecauseTheProposedValueIsGreaterThanTheCurrentOdometer => ResourceManager.GetString("StringTable.Message_TheOdometerIsReadyToBeSetBecauseTheProposedValueIsGreaterThanTheCurrentOdometer");

	internal static string Message_TheOdometerValueIsUnknownAndCannotBeSet => ResourceManager.GetString("StringTable.Message_TheOdometerValueIsUnknownAndCannotBeSet");

	internal static string Message_TheOdometerHasBeenSetTheValueShownMayBeSlightlyDifferentThanTheOneEnteredDueToRoundingIssues => ResourceManager.GetString("StringTable.Message_TheOdometerHasBeenSetTheValueShownMayBeSlightlyDifferentThanTheOneEnteredDueToRoundingIssues");

	internal static string Message_DDECReportsLifeToDateDataWillBeResetClickOkToContinueOrClickCancelToAbortChangesAndExit => ResourceManager.GetString("StringTable.Message_DDECReportsLifeToDateDataWillBeResetClickOkToContinueOrClickCancelToAbortChangesAndExit");

	internal static string MessageFormat_AnErrorOccurredSettingTheOdometer0 => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredSettingTheOdometer0");

	internal static string Message_UnsupportedSoftwareVersionTitle => ResourceManager.GetString("StringTable.Message_UnsupportedSoftwareVersionTitle");

	internal static string MessageFormat_UnsupportedSoftwareVersionNoRepair => ResourceManager.GetString("StringTable.MessageFormat_UnsupportedSoftwareVersionNoRepair");
}
