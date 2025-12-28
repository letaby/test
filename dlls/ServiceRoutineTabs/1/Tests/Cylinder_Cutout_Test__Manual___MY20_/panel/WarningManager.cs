// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual___MY20_.panel.WarningManager
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using SapiLayer1;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual___MY20_.panel;

internal static class WarningManager
{
  private static bool warningRequired = true;
  private static string warningFormat = $"{Resources.Message_CautionTransAndParkingBrake}\r\n{Resources.MessageFormat_ContinueWith0}";
  private static string message;

  public static void SetJobName(string jobName)
  {
    if (string.IsNullOrEmpty(jobName))
      WarningManager.message = string.Format(WarningManager.warningFormat, (object) "test");
    else
      WarningManager.message = string.Format(WarningManager.warningFormat, (object) jobName);
  }

  private static string Message
  {
    get
    {
      if (string.IsNullOrEmpty(WarningManager.message))
        WarningManager.SetJobName(string.Empty);
      return WarningManager.message;
    }
  }

  public static void Reset() => WarningManager.warningRequired = true;

  public static bool RequestContinue()
  {
    bool flag = false;
    if (!WarningManager.warningRequired || DialogResult.Yes == MessageBox.Show(WarningManager.Message, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
    {
      flag = true;
      WarningManager.warningRequired = false;
      if (Sapi.GetSapi() != null && Sapi.GetSapi().LogFiles.Logging)
        SapiExtensions.LabelLogWithPrefix(Sapi.GetSapi().LogFiles, "Cylinder Cutout (Manual)", Resources.Message_UserAcknowledgedCaution);
    }
    return flag;
  }
}
