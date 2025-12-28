// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__NGC_.panel.CalibrationStep
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__NGC_.panel;

public class CalibrationStep
{
  public string DisplayText { get; set; }

  public string DisplaySubText { get; set; }

  public CalibrationActions Action { get; set; }

  public bool ButtonEnabled { get; set; }

  public double Offset { get; set; }

  public AxisRange VisibleRange { get; set; }

  public string GradientString { get; set; }

  public bool DialEnabled { get; set; }

  public CalibrationStep(
    string displayText,
    string displaySubText,
    CalibrationActions action,
    bool buttonEnabled,
    double offset,
    AxisRange visibleRange,
    string gradientString)
  {
    this.DisplayText = displayText;
    this.DisplaySubText = displaySubText;
    this.Action = action;
    this.ButtonEnabled = buttonEnabled;
    this.Offset = offset;
    this.VisibleRange = visibleRange;
    this.GradientString = gradientString;
    this.DialEnabled = true;
  }

  public CalibrationStep(
    string displayText,
    string displaySubText,
    CalibrationActions action,
    bool buttonEnabled,
    double offset,
    double visibleMinimum,
    double visibleMaximum,
    string gradientString)
    : this(displayText, displaySubText, action, buttonEnabled, offset, new AxisRange(visibleMinimum, visibleMaximum), gradientString)
  {
  }

  public CalibrationStep(
    string displayText,
    string displaySubText,
    CalibrationActions action,
    bool buttonEnabled)
  {
    this.DisplayText = displayText;
    this.DisplaySubText = displaySubText;
    this.Action = action;
    this.ButtonEnabled = buttonEnabled;
    this.DialEnabled = false;
  }
}
