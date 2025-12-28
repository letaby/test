// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
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

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningManager = new WarningManager(Resources.Message_ClockResetWarning);
    this.aControlThatGetsAPaint = (Control) this.label2;
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

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void UpdateChannels()
  {
    Channel channel = this.GetChannel("ICUC01T");
    if (this.icuc == channel)
      return;
    if (this.icuc != null)
    {
      this.icuc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.RemoveParameters();
      this.ClearResults();
      this.ReportResult(Resources.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet);
    }
    this.icuc = channel;
    if (this.icuc != null)
    {
      this.ClearResults();
      this.icuc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.yearsParameter = this.icuc.Parameters["paramTimeDateYears"];
      this.monthsParameter = this.icuc.Parameters["paramTimeDateMonths"];
      this.daysParameter = this.icuc.Parameters["paramTimeDateDays"];
      this.hoursParameter = this.icuc.Parameters["paramTimeDateHours"];
      this.minutesParameter = this.icuc.Parameters["paramTimeDateMinutes"];
      this.secondsParameter = this.icuc.Parameters["paramTimeDateSeconds"];
      this.minutesLocalOffsetParameter = this.icuc.Parameters["paramTimeDateMinutesLocalOffset"];
      this.hoursLocalOffsetParameter = this.icuc.Parameters["paramTimeDateHoursLocalOffset"];
      if (!this.ParametersValid)
      {
        this.ClearResults();
        this.ReportResult(Resources.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet);
      }
    }
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.Working;
    if (this.Working)
      return;
    if (this.icuc != null)
    {
      this.icuc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.RemoveParameters();
    }
    if (((ContainerControl) this).ParentForm != null)
      ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
  }

  public virtual void OnChannelsChanged()
  {
    this.UpdateChannels();
    this.UpdateUserInterface();
  }

  private bool Working => this.working;

  private bool Online => this.icuc != null && this.icuc.Online;

  private bool CanEditCustomTime => !this.Working && this.Online;

  private bool CanSetClock
  {
    get => !this.Working && this.Online && this.ParametersValid && this.PreconditionsValid;
  }

  private bool PreconditionsValid
  {
    get
    {
      return this.digitalReadoutInstrumentEngineSpeed.RepresentedState != 3 && this.digitalReadoutInstrumentVehicleSpeed.RepresentedState != 3;
    }
  }

  private bool ParametersValid
  {
    get
    {
      return this.yearsParameter != null && this.monthsParameter != null && this.daysParameter != null && this.hoursParameter != null && this.minutesParameter != null && this.secondsParameter != null && this.minutesLocalOffsetParameter != null && this.hoursLocalOffsetParameter != null;
    }
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnPaint(object sender, PaintEventArgs e)
  {
    this.UpdatePCTime();
    this.timer.Start();
  }

  private void OnTimerTick(object sender, EventArgs e)
  {
    this.timer.Stop();
    this.aControlThatGetsAPaint.Invalidate();
  }

  private void UpdateUserInterface()
  {
    this.buttonClose.Enabled = !this.Working;
    this.dateTimeCustom.Enabled = this.CanEditCustomTime;
    this.buttonSetPC.Enabled = this.CanSetClock;
    this.buttonSetCustom.Enabled = this.CanSetClock;
    this.UpdateTimeZone();
  }

  private void UpdatePCTime() => this.dateTimePC.Value = DateTime.Now;

  private void UpdateTimeZone()
  {
    if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now))
      this.textboxTimeZone.Text = TimeZone.CurrentTimeZone.DaylightName;
    else
      this.textboxTimeZone.Text = TimeZone.CurrentTimeZone.StandardName;
  }

  private void OnSetPCClick(object sender, EventArgs e)
  {
    if (!this.warningManager.RequestContinue() || !this.CanSetClock)
      return;
    this.SetRealTimeClock(DateTime.Now);
  }

  private void OnSetCustomClick(object sender, EventArgs e)
  {
    if (!this.warningManager.RequestContinue() || !this.CanSetClock)
      return;
    this.SetRealTimeClock(this.dateTimeCustom.Value);
  }

  private void SetRealTimeClock(DateTime dateTime)
  {
    if (!this.CanSetClock)
      return;
    this.ClearResults();
    if (this.icuc != null)
    {
      this.UpdateUserInterface();
      if (this.ParametersValid)
      {
        this.working = true;
        DateTime universalTime = dateTime.ToUniversalTime();
        string empty = string.Empty;
        string str = !dateTime.IsDaylightSavingTime() ? TimeZone.CurrentTimeZone.StandardName : TimeZone.CurrentTimeZone.DaylightName;
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_SettingRealTimeClockTo, (object) dateTime.ToString("yyyy/MM/dd HH:mm:ss", (IFormatProvider) CultureInfo.CurrentCulture), (object) str, (object) universalTime.ToString("yyyy/MM/dd HH:mm:ss")));
        this.yearsParameter.Value = (object) (universalTime.Year - 1985);
        this.monthsParameter.Value = (object) universalTime.Month;
        this.daysParameter.Value = (object) (universalTime.Day * 4);
        this.hoursParameter.Value = (object) universalTime.Hour;
        this.minutesParameter.Value = (object) universalTime.Minute;
        this.secondsParameter.Value = (object) universalTime.Second;
        this.minutesLocalOffsetParameter.Value = (object) 125;
        this.hoursLocalOffsetParameter.Value = (object) (125 + TimeZone.CurrentTimeZone.GetUtcOffset(dateTime).Hours);
        this.icuc.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.icuc_ParametersWriteCompleteEvent);
        this.UpdateUserInterface();
        this.icuc.Parameters.Write(false);
      }
    }
    else
      this.ReportResult(Resources.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet);
  }

  private void icuc_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.icuc != null)
      this.icuc.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.icuc_ParametersWriteCompleteEvent);
    if (!e.Succeeded)
    {
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock01PleaseCycleTheIgnitionAndTryAgain1IfTheEngineIsRunningPleaseStopTheEngineBeforeRetrying, (object) e.Exception.Message, (object) Environment.NewLine));
      this.working = false;
    }
    else
    {
      this.ReportResult(Resources.Message_Finished);
      this.working = false;
    }
    this.UpdateUserInterface();
  }

  private void ClearResults()
  {
    if (this.textboxResults == null)
      return;
    this.textboxResults.Text = string.Empty;
  }

  private void ReportResult(string text)
  {
    if (this.textboxResults != null)
    {
      StringBuilder stringBuilder = new StringBuilder(this.textboxResults.Text);
      stringBuilder.AppendLine(text);
      this.textboxResults.Text = stringBuilder.ToString();
      this.textboxResults.SelectionStart = this.textboxResults.TextLength;
      this.textboxResults.SelectionLength = 0;
      this.textboxResults.ScrollToCaret();
    }
    this.AddStatusMessage(text);
  }

  private void RemoveParameters()
  {
    this.yearsParameter = (Parameter) null;
    this.monthsParameter = (Parameter) null;
    this.daysParameter = (Parameter) null;
    this.hoursParameter = (Parameter) null;
    this.minutesParameter = (Parameter) null;
    this.secondsParameter = (Parameter) null;
    this.minutesLocalOffsetParameter = (Parameter) null;
    this.hoursLocalOffsetParameter = (Parameter) null;
  }

  private void digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUserInterface();
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
    this.buttonSetCustom = new Button();
    this.buttonSetPC = new Button();
    this.label2 = new System.Windows.Forms.Label();
    this.label3 = new System.Windows.Forms.Label();
    this.label4 = new System.Windows.Forms.Label();
    this.label5 = new System.Windows.Forms.Label();
    this.label6 = new System.Windows.Forms.Label();
    this.label7 = new System.Windows.Forms.Label();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textboxResults, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClose, 2, 9);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.dateTimeCustom, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.dateTimePC, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textboxTimeZone, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonSetCustom, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonSetPC, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label3, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label4, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label5, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label6, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label7, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.tableLayoutPanel1, 0, 8);
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
    componentResourceManager.ApplyResources((object) this.buttonSetCustom, "buttonSetCustom");
    this.buttonSetCustom.Name = "buttonSetCustom";
    this.buttonSetCustom.UseCompatibleTextRendering = true;
    this.buttonSetCustom.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonSetPC, "buttonSetPC");
    this.buttonSetPC.Name = "buttonSetPC";
    this.buttonSetPC.UseCompatibleTextRendering = true;
    this.buttonSetPC.UseVisualStyleBackColor = true;
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
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.tableLayoutPanel1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentEngineSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentEngineSpeed_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_RealTimeClock");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel2);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
