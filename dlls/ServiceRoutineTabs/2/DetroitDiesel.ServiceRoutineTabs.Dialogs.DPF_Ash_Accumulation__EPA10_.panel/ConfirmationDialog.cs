using System.Windows.Forms;
using DetroitDiesel.Common;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__EPA10_.panel;

internal static class ConfirmationDialog
{
	private static string FormatString = Resources.Message_HereIsASummaryOfTheInformationThatYouHaveProvidedAndHasBeenCollectedByTheToolRN + "\r\n" + Resources.Message_AllInformationWillBeRecordedAndAnyFalseInformationCouldVoidWarrantyRN + "\r\n" + Resources.Message_PleaseReviewTheInformationAndConfirmThatItIsCorrectAndYouWouldLikeRN + "\r\n" + Resources.Message_ToProceedWithTheRequestedChangeToTheAshVolumeRatioRN + "\r\n\r\n" + Resources.MessageFormat_VIN0RN + "\r\nDPF SN: {1}\r\n" + Resources.MessageFormat_NewAshVolumeRatio2RN + "\r\n\r\n" + Resources.Message_IsThisInformationCorrectAndDoYouWantToContinue;

	private static string LogEntryForConfirmation = Resources.Message_AshVolumeRatioChangeRequested + "(DPFSN:{0}," + Resources.MessageFormat_Ratio1;

	public static bool Show(string vin, AtsType type, string dpfsn1, string dpfsn2, string action)
	{
		bool flag = false;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string text = dpfsn1 + ((type == AtsType.OneBox) ? (", " + dpfsn2) : string.Empty);
		empty = ((!string.IsNullOrEmpty(text) && !text.Equals(", ")) ? string.Format(FormatString, vin, text, action) : string.Format(FormatString.Replace("DPF SN: {1}\r\n", ""), vin, text, action));
		if (DialogResult.Yes == MessageBox.Show(empty, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
		{
			empty2 = ((!string.IsNullOrEmpty(text) && !text.Equals(", ")) ? string.Format(LogEntryForConfirmation, text, action) : string.Format(LogEntryForConfirmation.Replace("DPFSN:{0},", ""), text, action));
			Log.AddEvent(empty2);
			return true;
		}
		return false;
	}
}
