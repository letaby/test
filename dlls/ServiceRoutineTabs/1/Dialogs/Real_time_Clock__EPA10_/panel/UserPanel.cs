// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock__EPA10_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private const string RealTimeClockService = "DL_ID_Real_Time_Clock";
  private const string RealTimeClockEcuInfo = "CO_RealTimeClock";
  private const string TimeFormat = "yyyy/MM/dd HH:mm:ss";
  private Control aControlThatGetsAPaint;
  private Timer timer;
  private Channel cpc2 = (Channel) null;
  private Service service = (Service) null;
  private EcuInfo ecuInfo = (EcuInfo) null;
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

  public UserPanel()
  {
    this.InitializeComponent();
    this.ecuTimeLastReadAt = DateTime.FromBinary(0L);
    this.aControlThatGetsAPaint = (Control) this.label1;
    this.aControlThatGetsAPaint.Paint += new PaintEventHandler(this.OnPaint);
    this.dateTimePC.CustomFormat = "yyyy/MM/dd HH:mm:ss";
    this.dateTimeCustom.CustomFormat = "yyyy/MM/dd HH:mm:ss";
    this.buttonSetPC.Click += new EventHandler(this.OnSetPCClick);
    this.buttonSetCustom.Click += new EventHandler(this.OnSetCustomClick);
    this.dateTimePC.Enabled = false;
    this.timer = new Timer();
    this.timer.Enabled = false;
    this.timer.Interval = 500;
    this.timer.Tick += new EventHandler(this.OnTimerTick);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.ClearResults();
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    this.SetCPC2((Channel) null);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetCPC2(this.GetChannel("CPC02T"));
    this.UpdateUserInterface();
  }

  private void SetCPC2(Channel cpc2)
  {
    if (this.cpc2 == cpc2)
      return;
    this.ResetData();
    if (this.cpc2 != null)
    {
      this.cpc2.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      if (this.ecuInfo != null)
      {
        this.ecuInfo.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.OnEcuTimeUpdate);
        this.ecuInfo = (EcuInfo) null;
      }
    }
    this.cpc2 = cpc2;
    if (this.cpc2 != null)
    {
      this.cpc2.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.ecuInfo = this.cpc2.EcuInfos["CO_RealTimeClock"];
      if (this.ecuInfo != null)
        this.ecuInfo.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.OnEcuTimeUpdate);
    }
  }

  private bool CanClose => !this.Working;

  private bool Working => this.service != (Service) null;

  private bool Online
  {
    get => this.cpc2 != null && this.cpc2.CommunicationsState == CommunicationsState.Online;
  }

  private bool CanEditCustomTime => !this.Working && this.Online;

  private bool CanSetClock => !this.Working && this.Online;

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnEcuTimeUpdate(object sender, ResultEventArgs e)
  {
    this.ecuTimeLastReadAt = DateTime.Now;
    this.UpdateEcuTime();
  }

  private void OnPaint(object sender, PaintEventArgs e)
  {
    this.UpdatePCTime();
    this.UpdateEcuTime();
    this.timer.Start();
  }

  private void OnTimerTick(object sender, EventArgs e)
  {
    this.timer.Stop();
    this.aControlThatGetsAPaint.Invalidate();
  }

  private void UpdateUserInterface()
  {
    this.buttonClose.Enabled = this.CanClose;
    this.dateTimeCustom.Enabled = this.CanEditCustomTime;
    this.buttonSetPC.Enabled = this.CanSetClock;
    this.buttonSetCustom.Enabled = this.CanSetClock;
    this.UpdateTimeZone();
  }

  private void UpdateTimeZone()
  {
    if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now))
      this.textboxTimeZone.Text = TimeZone.CurrentTimeZone.DaylightName;
    else
      this.textboxTimeZone.Text = TimeZone.CurrentTimeZone.StandardName;
  }

  private void UpdatePCTime() => this.dateTimePC.Value = DateTime.Now;

  private void UpdateEcuTime()
  {
    if (this.ecuInfo != null && this.ecuInfo.Value != null && !this.ecuTimeLastReadAt.Equals(DateTime.FromBinary(0L)))
    {
      IFormatProvider provider = (IFormatProvider) new CultureInfo("en-US", true);
      try
      {
        DateTime localTime = DateTime.Parse(this.ecuInfo.Value, provider, DateTimeStyles.AssumeUniversal);
        localTime = localTime.ToLocalTime();
        TimeSpan timeSpan = DateTime.Now - this.ecuTimeLastReadAt;
        this.textboxEcuTime.Text = (localTime + timeSpan).ToString("yyyy/MM/dd HH:mm:ss");
      }
      catch (FormatException ex)
      {
        this.textboxEcuTime.Text = Resources.Message_Invalid;
      }
    }
    else
    {
      this.textboxEcuTime.Text = Resources.Message_Unavailable;
      if (this.Online && this.ecuInfo != null)
        this.ecuInfo.Read(false);
    }
  }

  private void ResetData() => this.ClearResults();

  private void OnSetPCClick(object sender, EventArgs e)
  {
    if (!this.CanSetClock)
      return;
    this.SetRealTimeClock(DateTime.Now);
  }

  private void OnSetCustomClick(object sender, EventArgs e)
  {
    if (!this.CanSetClock)
      return;
    this.SetRealTimeClock(this.dateTimeCustom.Value);
  }

  private void SetRealTimeClock(DateTime dateTime)
  {
    if (!this.CanSetClock)
      return;
    this.ClearResults();
    this.service = this.GetService("CPC02T", "DL_ID_Real_Time_Clock");
    if (this.service != (Service) null)
    {
      this.UpdateUserInterface();
      string empty = string.Empty;
      string str = !dateTime.IsDaylightSavingTime() ? TimeZone.CurrentTimeZone.StandardName : TimeZone.CurrentTimeZone.DaylightName;
      DateTime universalTime = dateTime.ToUniversalTime();
      this.ReportResult(string.Format(Resources.MessageFormat_SettingRealTimeClockTo01, (object) dateTime.ToString("yyyy/MM/dd HH:mm:ss"), (object) str, (object) universalTime.ToString("yyyy/MM/dd HH:mm:ss")));
      this.service.InputValues[0].Value = (object) universalTime.Second;
      this.service.InputValues[1].Value = (object) universalTime.Minute;
      this.service.InputValues[2].Value = (object) universalTime.Hour;
      this.service.InputValues[3].Value = (object) universalTime.Day;
      this.service.InputValues[4].Value = (object) this.service.InputValues[4].Choices[(int) universalTime.DayOfWeek];
      this.service.InputValues[5].Value = (object) this.service.InputValues[5].Choices[universalTime.Month - 1];
      this.service.InputValues[6].Value = (object) universalTime.Year;
      this.service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
      this.service.Execute(false);
    }
    else
      this.ReportResult(Resources.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet);
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    this.service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    if (e.Succeeded)
      this.ReportResult(Resources.Message_Finished);
    else
      this.ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock0, (object) e.Exception.Message));
    this.service = (Service) null;
    this.ecuInfo.Read(true);
    this.UpdateUserInterface();
  }

  private void ClearResults()
  {
    if (this.textboxResults == null)
      return;
    this.textboxResults.Text = Resources.Message_NoteThatTheRealTimeClockValuesAreDisplayedInTheComputerSLocalTimeZone + "\r\n";
  }

  private void ReportResult(string text)
  {
    if (this.textboxResults != null)
    {
      StringBuilder stringBuilder = new StringBuilder(this.textboxResults.Text);
      stringBuilder.Append(text);
      stringBuilder.Append("\r\n");
      this.textboxResults.Text = stringBuilder.ToString();
      this.textboxResults.SelectionStart = this.textboxResults.TextLength;
      this.textboxResults.SelectionLength = 0;
      this.textboxResults.ScrollToCaret();
    }
    this.AddStatusMessage(text);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.textboxResults = new TextBox();
    this.buttonClose = new Button();
    this.dateTimeCustom = new DateTimePicker();
    this.dateTimePC = new DateTimePicker();
    this.textboxTimeZone = new TextBox();
    this.textboxEcuTime = new TextBox();
    this.buttonSetCustom = new Button();
    this.buttonSetPC = new Button();
    this.label1 = new System.Windows.Forms.Label();
    this.label2 = new System.Windows.Forms.Label();
    this.label3 = new System.Windows.Forms.Label();
    this.label4 = new System.Windows.Forms.Label();
    this.label5 = new System.Windows.Forms.Label();
    this.label6 = new System.Windows.Forms.Label();
    this.label7 = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textboxResults, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClose, 2, 8);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.dateTimeCustom, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.dateTimePC, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textboxTimeZone, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textboxEcuTime, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonSetCustom, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonSetPC, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label3, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label4, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label5, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label6, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label7, 0, 6);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.textboxResults, 3);
    componentResourceManager.ApplyResources((object) this.textboxResults, "textboxResults");
    this.textboxResults.Name = "textboxResults";
    this.textboxResults.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.dateTimeCustom, "dateTimeCustom");
    this.dateTimeCustom.Format = DateTimePickerFormat.Custom;
    this.dateTimeCustom.Name = "dateTimeCustom";
    this.dateTimeCustom.ShowUpDown = true;
    componentResourceManager.ApplyResources((object) this.dateTimePC, "dateTimePC");
    this.dateTimePC.Format = DateTimePickerFormat.Custom;
    this.dateTimePC.Name = "dateTimePC";
    this.dateTimePC.ShowUpDown = true;
    componentResourceManager.ApplyResources((object) this.textboxTimeZone, "textboxTimeZone");
    this.textboxTimeZone.Name = "textboxTimeZone";
    this.textboxTimeZone.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.textboxEcuTime, "textboxEcuTime");
    this.textboxEcuTime.Name = "textboxEcuTime";
    this.textboxEcuTime.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.buttonSetCustom, "buttonSetCustom");
    this.buttonSetCustom.Name = "buttonSetCustom";
    this.buttonSetCustom.UseCompatibleTextRendering = true;
    this.buttonSetCustom.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonSetPC, "buttonSetPC");
    this.buttonSetPC.Name = "buttonSetPC";
    this.buttonSetPC.UseCompatibleTextRendering = true;
    this.buttonSetPC.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.label2, 3);
    this.label2.ForeColor = Color.Red;
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    this.label3.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label4, "label4");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.label4, 3);
    this.label4.Name = "label4";
    this.label4.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    this.label5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label6, "label6");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.label6, 3);
    this.label6.Name = "label6";
    this.label6.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label7, "label7");
    this.label7.Name = "label7";
    this.label7.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_RealTimeClock");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel2);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
