using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock__MY13_.panel;

public class UserPanel : CustomPanel
{
	private const string RealTimeClockService = "DL_ID_Real_Time_Clock";

	private const string RealTimeClockEcuInfo = "CO_RealTimeClock";

	private const string TimeFormat = "yyyy/MM/dd HH:mm:ss";

	private Control aControlThatGetsAPaint;

	private Timer timer;

	private Channel cpc = null;

	private Service service = null;

	private EcuInfo ecuInfo = null;

	private DateTime ecuTimeLastReadAt;

	private TextBox textboxEcuTime;

	private TextBox textboxTimeZone;

	private DateTimePicker dateTimePC;

	private Button buttonSetPC;

	private DateTimePicker dateTimeCustom;

	private Button buttonSetCustom;

	private TableLayoutPanel tableLayoutPanel2;

	private Button buttonClose;

	private System.Windows.Forms.Label label1;

	private System.Windows.Forms.Label label2;

	private System.Windows.Forms.Label label3;

	private System.Windows.Forms.Label label4;

	private System.Windows.Forms.Label label5;

	private System.Windows.Forms.Label label6;

	private System.Windows.Forms.Label label7;

	private TextBox textboxResults;

	private bool CanClose => !Working;

	private bool Working => service != null;

	private bool Online => cpc != null && cpc.CommunicationsState == CommunicationsState.Online;

	private bool CanEditCustomTime => !Working && Online;

	private bool CanSetClock => !Working && Online;

	public UserPanel()
	{
		InitializeComponent();
		ecuTimeLastReadAt = DateTime.FromBinary(0L);
		aControlThatGetsAPaint = label1;
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

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			SetCPC(null);
		}
	}

	public override void OnChannelsChanged()
	{
		SetCPC(((CustomPanel)this).GetChannel("CPC04T"));
		UpdateUserInterface();
	}

	private void SetCPC(Channel cpc)
	{
		if (this.cpc == cpc)
		{
			return;
		}
		ResetData();
		if (this.cpc != null)
		{
			this.cpc.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			if (ecuInfo != null)
			{
				ecuInfo.EcuInfoUpdateEvent -= OnEcuTimeUpdate;
				ecuInfo = null;
			}
		}
		this.cpc = cpc;
		if (this.cpc != null)
		{
			this.cpc.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			ecuInfo = this.cpc.EcuInfos["CO_RealTimeClock"];
			if (ecuInfo != null)
			{
				ecuInfo.EcuInfoUpdateEvent += OnEcuTimeUpdate;
			}
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnEcuTimeUpdate(object sender, ResultEventArgs e)
	{
		ecuTimeLastReadAt = DateTime.Now;
		UpdateEcuTime();
	}

	private void OnPaint(object sender, PaintEventArgs e)
	{
		UpdatePCTime();
		UpdateEcuTime();
		timer.Start();
	}

	private void OnTimerTick(object sender, EventArgs e)
	{
		timer.Stop();
		aControlThatGetsAPaint.Invalidate();
	}

	private void UpdateUserInterface()
	{
		buttonClose.Enabled = CanClose;
		dateTimeCustom.Enabled = CanEditCustomTime;
		buttonSetPC.Enabled = CanSetClock;
		buttonSetCustom.Enabled = CanSetClock;
		UpdateTimeZone();
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

	private void UpdatePCTime()
	{
		dateTimePC.Value = DateTime.Now;
	}

	private void UpdateEcuTime()
	{
		if (ecuInfo != null && ecuInfo.Value != null && !ecuTimeLastReadAt.Equals(DateTime.FromBinary(0L)))
		{
			IFormatProvider provider = new CultureInfo("en-US", useUserOverride: true);
			try
			{
				DateTime dateTime = DateTime.Parse(ecuInfo.Value, provider, DateTimeStyles.AssumeUniversal).ToLocalTime();
				TimeSpan timeSpan = DateTime.Now - ecuTimeLastReadAt;
				DateTime dateTime2 = dateTime + timeSpan;
				textboxEcuTime.Text = dateTime2.ToString("yyyy/MM/dd HH:mm:ss");
				return;
			}
			catch (FormatException)
			{
				textboxEcuTime.Text = Resources.Message_Invalid;
				return;
			}
		}
		textboxEcuTime.Text = Resources.Message_Unavailable;
		if (Online && ecuInfo != null)
		{
			ecuInfo.Read(synchronous: false);
		}
	}

	private void ResetData()
	{
		ClearResults();
	}

	private void OnSetPCClick(object sender, EventArgs e)
	{
		if (CanSetClock)
		{
			SetRealTimeClock(DateTime.Now);
		}
	}

	private void OnSetCustomClick(object sender, EventArgs e)
	{
		if (CanSetClock)
		{
			SetRealTimeClock(dateTimeCustom.Value);
		}
	}

	private void SetRealTimeClock(DateTime dateTime)
	{
		if (CanSetClock)
		{
			ClearResults();
			service = ((CustomPanel)this).GetService("CPC04T", "DL_ID_Real_Time_Clock");
			if (service != null)
			{
				UpdateUserInterface();
				string empty = string.Empty;
				empty = ((!dateTime.IsDaylightSavingTime()) ? TimeZone.CurrentTimeZone.StandardName : TimeZone.CurrentTimeZone.DaylightName);
				DateTime dateTime2 = dateTime.ToUniversalTime();
				ReportResult(string.Format(Resources.MessageFormat_SettingRealTimeClockTo, dateTime.ToString("yyyy/MM/dd HH:mm:ss"), empty, dateTime2.ToString("yyyy/MM/dd HH:mm:ss")));
				service.InputValues[0].Value = dateTime2.Second;
				service.InputValues[1].Value = dateTime2.Minute;
				service.InputValues[2].Value = dateTime2.Hour;
				service.InputValues[3].Value = dateTime2.Day;
				service.InputValues[4].Value = service.InputValues[4].Choices[(int)dateTime2.DayOfWeek];
				service.InputValues[5].Value = service.InputValues[5].Choices[dateTime2.Month - 1];
				service.InputValues[6].Value = dateTime2.Year;
				service.ServiceCompleteEvent += OnServiceComplete;
				service.Execute(synchronous: false);
			}
			else
			{
				ReportResult(Resources.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet);
			}
		}
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		service.ServiceCompleteEvent -= OnServiceComplete;
		if (e.Succeeded)
		{
			ReportResult(Resources.Message_Finished);
		}
		else
		{
			ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock01PleaseCycleTheIgnitionAndTryAgain1IfTheEngineIsRunningPleaseStopTheEngineBeforeRetrying, e.Exception.Message, Environment.NewLine));
		}
		service = null;
		ecuInfo.Read(synchronous: true);
		UpdateUserInterface();
	}

	private void ClearResults()
	{
		if (textboxResults != null)
		{
			textboxResults.Text = Resources.Message_NoteThatTheRealTimeClockValuesAreDisplayedInTheComputerSLocalTimeZone + "\r\n";
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

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel2 = new TableLayoutPanel();
		textboxResults = new TextBox();
		buttonClose = new Button();
		dateTimeCustom = new DateTimePicker();
		dateTimePC = new DateTimePicker();
		textboxTimeZone = new TextBox();
		textboxEcuTime = new TextBox();
		buttonSetCustom = new Button();
		buttonSetPC = new Button();
		label1 = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		label3 = new System.Windows.Forms.Label();
		label4 = new System.Windows.Forms.Label();
		label5 = new System.Windows.Forms.Label();
		label6 = new System.Windows.Forms.Label();
		label7 = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textboxResults, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClose, 2, 8);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(dateTimeCustom, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(dateTimePC, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textboxTimeZone, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(textboxEcuTime, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonSetCustom, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonSetPC, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label4, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label5, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label6, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(label7, 0, 6);
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
		componentResourceManager.ApplyResources(textboxEcuTime, "textboxEcuTime");
		textboxEcuTime.Name = "textboxEcuTime";
		textboxEcuTime.ReadOnly = true;
		componentResourceManager.ApplyResources(buttonSetCustom, "buttonSetCustom");
		buttonSetCustom.Name = "buttonSetCustom";
		buttonSetCustom.UseCompatibleTextRendering = true;
		buttonSetCustom.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonSetPC, "buttonSetPC");
		buttonSetPC.Name = "buttonSetPC";
		buttonSetPC.UseCompatibleTextRendering = true;
		buttonSetPC.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
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
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_RealTimeClock");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel2);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
