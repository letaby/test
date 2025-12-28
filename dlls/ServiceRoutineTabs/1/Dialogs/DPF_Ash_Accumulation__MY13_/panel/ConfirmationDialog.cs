// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__MY13_.panel.ConfirmationDialog
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__MY13_.panel;

internal static class ConfirmationDialog
{
  private static string FormatString = $"{Resources.Message_HereIsASummaryOfTheInformationThatYouHaveProvidedAndHasBeenCollectedByTheToolRN}\r\n{Resources.Message_AllInformationWillBeRecordedAndAnyFalseInformationCouldVoidWarrantyRN}\r\n{Resources.Message_PleaseReviewTheInformationAndConfirmThatItIsCorrectAndYouWouldLikeRN}\r\n{Resources.Message_ToProceedWithTheRequestedChangeToTheAshVolumeRatioRN}\r\n\r\n{Resources.MessageFormat_VIN0RN}\r\nDPF SN: {{1}}\r\n{Resources.MessageFormat_NewAshVolumeRatio2RN}\r\n\r\n{Resources.Message_IsThisInformationCorrectAndDoYouWantToContinue}";
  private static string LogEntryForConfirmation = $"{Resources.Message_AshVolumeRatioChangeRequested}DPFSN:{{0}},{Resources.MessageFormat_Ratio1}";

  public static bool Show(string vin, AtsType type, string dpfsn1, string dpfsn2, string action)
  {
    string empty1 = string.Empty;
    string empty2 = string.Empty;
    string str = dpfsn1 + (type == AtsType.OneBoxTwoFilters ? ", " + dpfsn2 : string.Empty);
    bool flag;
    if (DialogResult.Yes == MessageBox.Show(!string.IsNullOrEmpty(str) && !str.Equals(", ") ? string.Format(ConfirmationDialog.FormatString, (object) vin, (object) str, (object) action) : string.Format(ConfirmationDialog.FormatString.Replace("DPF SN: {1}\r\n", ""), (object) vin, (object) str, (object) action), ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
    {
      Log.AddEvent(!string.IsNullOrEmpty(str) && !str.Equals(", ") ? string.Format(ConfirmationDialog.LogEntryForConfirmation, (object) str, (object) action) : string.Format(ConfirmationDialog.LogEntryForConfirmation.Replace("DPFSN:{0},", ""), (object) str, (object) action));
      flag = true;
    }
    else
      flag = false;
    return flag;
  }
}
