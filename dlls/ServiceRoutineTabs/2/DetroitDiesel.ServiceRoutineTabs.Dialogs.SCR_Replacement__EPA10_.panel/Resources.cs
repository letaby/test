using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__EPA10_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string ParametersHaveNotBeenVerifiedAndMayNotHaveBeenWritten => ResourceManager.GetString("StringTable.ParametersHaveNotBeenVerifiedAndMayNotHaveBeenWritten");

	internal static string Message_FailedToObtainMCMAshDistanceAccumulator => ResourceManager.GetString("StringTable.Message_FailedToObtainMCMAshDistanceAccumulator");

	internal static string MessageFormat_WhileWritingTheNewAshAccumulationDistanceTheFollowingErrorOccurred => ResourceManager.GetString("StringTable.MessageFormat_WhileWritingTheNewAshAccumulationDistanceTheFollowingErrorOccurred");

	internal static string Message_SerialNumberShouldBe14DigitsInLengthWithTheFormatOf12Xxxxxxxxxxxx => ResourceManager.GetString("StringTable.Message_SerialNumberShouldBe14DigitsInLengthWithTheFormatOf12Xxxxxxxxxxxx");

	internal static string Message_TheUserCanceledTheOperation => ResourceManager.GetString("StringTable.Message_TheUserCanceledTheOperation");

	internal static string Message_CommittingChanges => ResourceManager.GetString("StringTable.Message_CommittingChanges");

	internal static string Message_SCRAccumulatorsResetStarted => ResourceManager.GetString("StringTable.Message_SCRAccumulatorsResetStarted");

	internal static string Message_PleaseProvideTheSerialNumberForTheNewSCRUnit => ResourceManager.GetString("StringTable.Message_PleaseProvideTheSerialNumberForTheNewSCRUnit");

	internal static string Message_IsThisInformationCorrectAndDoYouWantToContinue => ResourceManager.GetString("StringTable.Message_IsThisInformationCorrectAndDoYouWantToContinue");

	internal static string Message_WhileResettingTheAccumulatorsTheFollowingWarningWasReported => ResourceManager.GetString("StringTable.Message_WhileResettingTheAccumulatorsTheFollowingWarningWasReported");

	internal static string Message_HereIsASummaryOfTheInformationThatYouHaveProvidedAndHasBeenCollectedByTheTool => ResourceManager.GetString("StringTable.Message_HereIsASummaryOfTheInformationThatYouHaveProvidedAndHasBeenCollectedByTheTool");

	internal static string Message_FailedToCommitTheChangesToTheACMYouMayNeedToRepeatThisProcedure => ResourceManager.GetString("StringTable.Message_FailedToCommitTheChangesToTheACMYouMayNeedToRepeatThisProcedure");

	internal static string Message_WhileResettingTheAccumulatorsTheFollowingErrorWasReported => ResourceManager.GetString("StringTable.Message_WhileResettingTheAccumulatorsTheFollowingErrorWasReported");

	internal static string Message_NoCommitServiceAvailable => ResourceManager.GetString("StringTable.Message_NoCommitServiceAvailable");

	internal static string Message_ToProceedWithTheSCRAccumulatorsReset => ResourceManager.GetString("StringTable.Message_ToProceedWithTheSCRAccumulatorsReset");

	internal static string Message_FailedToWriteTheAccumulators => ResourceManager.GetString("StringTable.Message_FailedToWriteTheAccumulators");

	internal static string Message_TheProcedureFailedToComplete => ResourceManager.GetString("StringTable.Message_TheProcedureFailedToComplete");

	internal static string MessageFormat_SCRSN2 => ResourceManager.GetString("StringTable.MessageFormat_SCRSN2");

	internal static string MessageFormat_SCRAccumulatorsResetRequestedSCRSN0 => ResourceManager.GetString("StringTable.MessageFormat_SCRAccumulatorsResetRequestedSCRSN0");

	internal static string Message_VIN => ResourceManager.GetString("StringTable.Message_VIN");

	internal static string Message_ESN => ResourceManager.GetString("StringTable.Message_ESN");

	internal static string MessageFormat_VIN1 => ResourceManager.GetString("StringTable.MessageFormat_VIN1");

	internal static string MessageFormat_ESN0 => ResourceManager.GetString("StringTable.MessageFormat_ESN0");

	internal static string Message_UnsupportedEquipment => ResourceManager.GetString("StringTable.Message_UnsupportedEquipment");

	internal static string Message_TheProcedureCompletedSuccessfully => ResourceManager.GetString("StringTable.Message_TheProcedureCompletedSuccessfully");

	internal static string Message_SerialNumber => ResourceManager.GetString("StringTable.Message_SerialNumber");

	internal static string Message_AllInformationWillBeRecordedAndAnyFalseInformationCouldVoidWarranty => ResourceManager.GetString("StringTable.Message_AllInformationWillBeRecordedAndAnyFalseInformationCouldVoidWarranty");

	internal static string Message_OneOrMoreDevicesDisconnected => ResourceManager.GetString("StringTable.Message_OneOrMoreDevicesDisconnected");

	internal static string Message_WritingNewValue => ResourceManager.GetString("StringTable.Message_WritingNewValue");

	internal static string Message_PleaseReviewTheInformationAndConfirmThatItIsCorrectAndYouWouldLike => ResourceManager.GetString("StringTable.Message_PleaseReviewTheInformationAndConfirmThatItIsCorrectAndYouWouldLike");
}
