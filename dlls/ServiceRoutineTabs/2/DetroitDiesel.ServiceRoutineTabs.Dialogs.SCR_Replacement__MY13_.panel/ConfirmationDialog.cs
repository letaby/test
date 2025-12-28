using System.Windows.Forms;
using DetroitDiesel.Common;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__MY13_.panel;

internal static class ConfirmationDialog
{
	private static string FormatString = Resources.Message_HereIsASummaryOfTheInformationThatYouHaveProvidedAndHasBeenCollectedByTheTool + "\r\n" + Resources.Message_AllInformationWillBeRecordedAndAnyFalseInformationCouldVoidWarranty + "\r\n" + Resources.Message_PleaseReviewTheInformationAndConfirmThatItIsCorrectAndYouWouldLike + "\r\n" + Resources.Message_ToProceedWithTheSCRAccumulatorsReset + "\r\n\r\n" + Resources.MessageFormat_VIN1 + "\r\n" + Resources.MessageFormat_ESN0 + "\r\n" + Resources.MessageFormat_SCRSN2 + "\r\n\r\n" + Resources.Message_IsThisInformationCorrectAndDoYouWantToContinue;

	private static string LogEntryForConfirmation = Resources.MessageFormat_SCRAccumulatorsResetRequestedSCRSN0;

	public static bool Show(string esn, string vin, string scrsn)
	{
		bool flag = false;
		string text = string.Format(FormatString, esn, vin, scrsn);
		if (DialogResult.Yes == MessageBox.Show(text, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
		{
			string eventText = string.Format(LogEntryForConfirmation, scrsn);
			Log.AddEvent(eventText);
			return true;
		}
		return false;
	}
}
