// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__MY13_.panel.ConfirmationDialog
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__MY13_.panel;

internal static class ConfirmationDialog
{
  private static string FormatString = $"{Resources.Message_HereIsASummaryOfTheInformationThatYouHaveProvidedAndHasBeenCollectedByTheTool}\r\n{Resources.Message_AllInformationWillBeRecordedAndAnyFalseInformationCouldVoidWarranty}\r\n{Resources.Message_PleaseReviewTheInformationAndConfirmThatItIsCorrectAndYouWouldLike}\r\n{Resources.Message_ToProceedWithTheSCRAccumulatorsReset}\r\n\r\n{Resources.MessageFormat_VIN1}\r\n{Resources.MessageFormat_ESN0}\r\n{Resources.MessageFormat_SCRSN2}\r\n\r\n{Resources.Message_IsThisInformationCorrectAndDoYouWantToContinue}";
  private static string LogEntryForConfirmation = Resources.MessageFormat_SCRAccumulatorsResetRequestedSCRSN0;

  public static bool Show(string esn, string vin, string scrsn)
  {
    bool flag;
    if (DialogResult.Yes == MessageBox.Show(string.Format(ConfirmationDialog.FormatString, (object) esn, (object) vin, (object) scrsn), ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
    {
      Log.AddEvent(string.Format(ConfirmationDialog.LogEntryForConfirmation, (object) scrsn));
      flag = true;
    }
    else
      flag = false;
    return flag;
  }
}
