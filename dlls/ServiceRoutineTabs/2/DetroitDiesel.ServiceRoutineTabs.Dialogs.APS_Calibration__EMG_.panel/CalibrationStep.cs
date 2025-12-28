using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__EMG_.panel;

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

	public CalibrationStep(string displayText, string displaySubText, CalibrationActions action, bool buttonEnabled, double offset, AxisRange visibleRange, string gradientString)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		DisplayText = displayText;
		DisplaySubText = displaySubText;
		Action = action;
		ButtonEnabled = buttonEnabled;
		Offset = offset;
		VisibleRange = visibleRange;
		GradientString = gradientString;
		DialEnabled = true;
	}

	public CalibrationStep(string displayText, string displaySubText, CalibrationActions action, bool buttonEnabled, double offset, double visibleMinimum, double visibleMaximum, string gradientString)
		: this(displayText, displaySubText, action, buttonEnabled, offset, new AxisRange(visibleMinimum, visibleMaximum), gradientString)
	{
	}//IL_000c: Unknown result type (might be due to invalid IL or missing references)


	public CalibrationStep(string displayText, string displaySubText, CalibrationActions action, bool buttonEnabled)
	{
		DisplayText = displayText;
		DisplaySubText = displaySubText;
		Action = action;
		ButtonEnabled = buttonEnabled;
		DialEnabled = false;
	}
}
