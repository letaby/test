using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock__NGC_.panel;

public class UserPanel : CustomPanel
{
	private const string IcucName = "ICUC01T";

	private const string RealTimeClockYears = "paramTimeDateYears";

	private const string RealTimeClockMonths = "paramTimeDateMonths";

	private const string RealTimeClockDays = "paramTimeDateDays";

	private const string RealTimeClockHours = "paramTimeDateHours";

	private const string RealTimeClockMinutes = "paramTimeDateMinutes";

	private const string RealTimeClockSeconds = "paramTimeDateSeconds";

	private const string RealTimeClockMinutesLocalOffset = "paramTimeDateMinutesLocalOffset";

	private const string RealTimeClockHoursLocalOffset = "paramTimeDateHoursLocalOffset";

	private const string TimeFormat = "yyyy/MM/dd HH:mm:ss";

	private Control aControlThatGetsAPaint;

	private Timer timer;

	private Channel icuc;

	private WarningManager warningManager;

	private Parameter yearsParameter;

	private Parameter monthsParameter;

	private Parameter daysParameter;

	private Parameter hoursParameter;

	private Parameter minutesParameter;

	private Parameter secondsParameter;

	private Parameter minutesLocalOffsetParameter;

	private Parameter hoursLocalOffsetParameter;

	private bool working = false;

	private TextBox textboxTimeZone;

	private DateTimePicker dateTimePC;

	private Button buttonSetPC;

	private DateTimePicker dateTimeCustom;

	private Button buttonSetCustom;

	private TableLayoutPanel tableLayoutPanel2;

	private Button buttonClose;

	private System.Windows.Forms.Label label2;

	private System.Windows.Forms.Label label3;

	private System.Windows.Forms.Label label4;

	private System.Windows.Forms.Label label5;

	private System.Windows.Forms.Label label6;

	private System.Windows.Forms.Label label7;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private TextBox textboxResults;

	private bool Working => working;

	private bool Online => icuc != null && icuc.Online;

	private bool CanEditCustomTime => !Working && Online;

	private bool CanSetClock => !Working && Online && ParametersValid && PreconditionsValid;

	private bool PreconditionsValid => (int)digitalReadoutInstrumentEngineSpeed.RepresentedState != 3 && (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState != 3;

	private bool ParametersValid => yearsParameter != null && monthsParameter != null && daysParameter != null && hoursParameter != null && minutesParameter != null && secondsParameter != null && minutesLocalOffsetParameter != null && hoursLocalOffsetParameter != null;

	public UserPanel()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		InitializeComponent();
		warningManager = new WarningManager(Resources.Message_ClockResetWarning);
		aControlThatGetsAPaint = label2;
		aControlThatGetsAPaint.Paint += OnPaint;
		dateTimePC.CustomFormat = "yyyy/MM/dd HH:mm:ss";
		dateTimeCustom.CustomFormat = "yyyy/MM/dd HH:mm:ss";
		buttonSetPC.Click += OnSetPCClick;
		buttonSetCustom.Click += OnSetCustomClick;
		dateTimePC.Enabled = false;
		timer = new Timer();
		timer.Enabled = false;
		timer.Interval = 500;
		timer.Tick += OnTimerTick;
	}

	protected override void OnLoad(EventArgs e)
	{
		ClearResults();
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateChannels()
	{
		Channel channel = ((CustomPanel)this).GetChannel("ICUC01T");
		if (icuc == channel)
		{
			return;
		}
		if (icuc != null)
		{
			icuc.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			RemoveParameters();
			ClearResults();
			ReportResult(Resources.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet);
		}
		icuc = channel;
		if (icuc != null)
		{
			ClearResults();
			icuc.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			yearsParameter = icuc.Parameters["paramTimeDateYears"];
			monthsParameter = icuc.Parameters["paramTimeDateMonths"];
			daysParameter = icuc.Parameters["paramTimeDateDays"];
			hoursParameter = icuc.Parameters["paramTimeDateHours"];
			minutesParameter = icuc.Parameters["paramTimeDateMinutes"];
			secondsParameter = icuc.Parameters["paramTimeDateSeconds"];
			minutesLocalOffsetParameter = icuc.Parameters["paramTimeDateMinutesLocalOffset"];
			hoursLocalOffsetParameter = icuc.Parameters["paramTimeDateHoursLocalOffset"];
			if (!ParametersValid)
			{
				ClearResults();
				ReportResult(Resources.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet);
			}
		}
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = Working;
		if (!Working)
		{
			if (icuc != null)
			{
				icuc.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				RemoveParameters();
			}
			if (((ContainerControl)this).ParentForm != null)
			{
				((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			}
		}
	}

	public override void OnChannelsChanged()
	{
		UpdateChannels();
		UpdateUserInterface();
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnPaint(object sender, PaintEventArgs e)
	{
		UpdatePCTime();
		timer.Start();
	}

	private void OnTimerTick(object sender, EventArgs e)
	{
		timer.Stop();
		aControlThatGetsAPaint.Invalidate();
	}

	private void UpdateUserInterface()
	{
		buttonClose.Enabled = !Working;
		dateTimeCustom.Enabled = CanEditCustomTime;
		buttonSetPC.Enabled = CanSetClock;
		buttonSetCustom.Enabled = CanSetClock;
		UpdateTimeZone();
	}

	private void UpdatePCTime()
	{
		dateTimePC.Value = DateTime.Now;
	}

	private void UpdateTimeZone()
	{
		if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now))
		{
			textboxTimeZone.Text = TimeZone.CurrentTimeZone.DaylightName;
		}
		else
		{
			textboxTimeZone.Text = TimeZone.CurrentTimeZone.StandardName;
		}
	}

	private void OnSetPCClick(object sender, EventArgs e)
	{
		if (warningManager.RequestContinue() && CanSetClock)
		{
			SetRealTimeClock(DateTime.Now);
		}
	}

	private void OnSetCustomClick(object sender, EventArgs e)
	{
		if (warningManager.RequestContinue() && CanSetClock)
		{
			SetRealTimeClock(dateTimeCustom.Value);
		}
	}

	private void SetRealTimeClock(DateTime dateTime)
	{
		if (!CanSetClock)
		{
			return;
		}
		ClearResults();
		if (icuc != null)
		{
			UpdateUserInterface();
			if (ParametersValid)
			{
				working = true;
				DateTime dateTime2 = dateTime.ToUniversalTime();
				string empty = string.Empty;
				ReportResult(string.Format(arg1: (!dateTime.IsDaylightSavingTime()) ? TimeZone.CurrentTimeZone.StandardName : TimeZone.CurrentTimeZone.DaylightName, provider: CultureInfo.CurrentCulture, format: Resources.MessageFormat_SettingRealTimeClockTo, arg0: dateTime.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture), arg2: dateTime2.ToString("yyyy/MM/dd HH:mm:ss")));
				yearsParameter.Value = dateTime2.Year - 1985;
				monthsParameter.Value = dateTime2.Month;
				daysParameter.Value = dateTime2.Day * 4;
				hoursParameter.Value = dateTime2.Hour;
				minutesParameter.Value = dateTime2.Minute;
				secondsParameter.Value = dateTime2.Second;
				minutesLocalOffsetParameter.Value = 125;
				hoursLocalOffsetParameter.Value = 125 + TimeZone.CurrentTimeZone.GetUtcOffset(dateTime).Hours;
				icuc.Parameters.ParametersWriteCompleteEvent += icuc_ParametersWriteCompleteEvent;
				UpdateUserInterface();
				icuc.Parameters.Write(synchronous: false);
			}
		}
		else
		{
			ReportResult(Resources.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet);
		}
	}

	private void icuc_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		if (icuc != null)
		{
			icuc.Parameters.ParametersWriteCompleteEvent -= icuc_ParametersWriteCompleteEvent;
		}
		if (!e.Succeeded)
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock01PleaseCycleTheIgnitionAndTryAgain1IfTheEngineIsRunningPleaseStopTheEngineBeforeRetrying, e.Exception.Message, Environment.NewLine));
			working = false;
		}
		else
		{
			ReportResult(Resources.Message_Finished);
			working = false;
		}
		UpdateUserInterface();
	}

	private void ClearResults()
	{
		if (textboxResults != null)
		{
			textboxResults.Text = string.Empty;
		}
	}

	private void ReportResult(string text)
	{
		if (textboxResults != null)
		{
			StringBuilder stringBuilder = new StringBuilder(textboxResults.Text);
			stringBuilder.AppendLine(text);
			textboxResults.Text = stringBuilder.ToString();
			textboxResults.SelectionStart = textboxResults.TextLength;
			textboxResults.SelectionLength = 0;
			textboxResults.ScrollToCaret();
		}
		((CustomPanel)this).AddStatusMessage(text);
	}

	private void RemoveParameters()
	{
		yearsParameter = null;
		monthsParameter = null;
		daysParameter = null;
		hoursParameter = null;
		minutesParameter = null;
		secondsParameter = null;
		minutesLocalOffsetParameter = null;
		hoursLocalOffsetParameter = null;
	}

	private void digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel2 = new TableLayoutPanel();
		textboxResults = new TextBox();
		buttonClose = new Button();
		dateTimeCustom = new DateTimePicker();
		dateTimePC = new DateTimePicker();
		textboxTimeZone = new TextBox();
		buttonSetCustom = new Button();
		buttonSetPC = new Button();
		label2 = new System.Windows.Forms.Label();
		label3 = new System.Windows.Forms.Label();
		label4 = new System.Windows.Forms.Label();
		label5 = new System.Windows.Forms.Label();
		label6 = new System.Windows.Forms.Label();
		label7 = new System.Windows.Forms.Label();
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textboxResults, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClose, 2, 9);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(dateTimeCustom, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(dateTimePC, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textboxTimeZone, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonSetCustom, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonSetPC, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label5, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label6, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label7, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)tableLayoutPanel1, 0, 8);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)textboxResults, 3);
		componentResourceManager.ApplyResources(textboxResults, "textboxResults");
		textboxResults.Name = "textboxResults";
		textboxResults.ReadOnly = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(dateTimeCustom, "dateTimeCustom");
		dateTimeCustom.Format = DateTimePickerFormat.Custom;
		dateTimeCustom.Name = "dateTimeCustom";
		dateTimeCustom.ShowUpDown = true;
		componentResourceManager.ApplyResources(dateTimePC, "dateTimePC");
		dateTimePC.Format = DateTimePickerFormat.Custom;
		dateTimePC.Name = "dateTimePC";
		dateTimePC.ShowUpDown = true;
		componentResourceManager.ApplyResources(textboxTimeZone, "textboxTimeZone");
		textboxTimeZone.Name = "textboxTimeZone";
		textboxTimeZone.ReadOnly = true;
		componentResourceManager.ApplyResources(buttonSetCustom, "buttonSetCustom");
		buttonSetCustom.Name = "buttonSetCustom";
		buttonSetCustom.UseCompatibleTextRendering = true;
		buttonSetCustom.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonSetPC, "buttonSetPC");
		buttonSetPC.Name = "buttonSetPC";
		buttonSetPC.UseCompatibleTextRendering = true;
		buttonSetPC.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(label2, "label2");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)label2, 3);
		label2.ForeColor = Color.Red;
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label3, "label3");
		label3.Name = "label3";
		label3.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label4, "label4");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)label4, 3);
		label4.Name = "label4";
		label4.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		label5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label6, "label6");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)label6, 3);
		label6.Name = "label6";
		label6.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label7, "label7");
		label7.Name = "label7";
		label7.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)tableLayoutPanel1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentVehicleSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentEngineSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_RealTimeClock");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel2);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
