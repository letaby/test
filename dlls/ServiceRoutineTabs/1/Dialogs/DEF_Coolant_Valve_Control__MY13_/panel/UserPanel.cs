// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Coolant_Valve_Control__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Coolant_Valve_Control__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const int durationDefault = 30;
  private const int durationMinimum = 0;
  private const int durationMaximum = 300;
  private Timer durationTimer;
  private Channel acm;
  private TableLayoutPanel tableLayoutPanelBase;
  private BarInstrument barInstrumentCoolantTemperature;
  private RunServiceButton runServiceButtonCoolantValveOpenStart;
  private RunServiceButton runServiceButtonCoolantValveOpenStop;
  private Button buttonClose;
  private DigitalReadoutInstrument digitalReadoutInstrumentCoolantValve;
  private TableLayoutPanel tableLayoutPanel1;
  private DecimalTextBox decimalTextBoxDuration;
  private System.Windows.Forms.Label labelDuration;
  private System.Windows.Forms.Label labelSeconds;
  private TextBox textBoxOutput;
  private BarInstrument barInstrumentDefTankTemperature;

  public UserPanel()
  {
    this.InitializeComponent();
    this.durationTimer = new Timer();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void OnRunServiceButtonCoolantValveOpenStartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
      this.CoolantValveOpenStart();
    else
      this.WriteMessage(string.Format(Resources.MessageFormat_TheValveFailedToOpen0, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty));
  }

  private void OnRunServiceButtonCoolantValveOpenStopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.StopTimer();
    if (((ResultEventArgs) e).Succeeded)
      this.UpdateServiceStoppedMessage();
    else
      this.WriteMessage(string.Format(Resources.MessageFormat_TheValveFailedToClose0, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty));
  }

  private void OnDigitalReadoutInstrumentCoolantValveRepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUserInterface();
  }

  public virtual void OnChannelsChanged()
  {
    base.OnChannelsChanged();
    this.SetAcm(this.GetChannel("ACM21T"));
  }

  private void SetAcm(Channel acm)
  {
    if (this.acm == acm)
      return;
    if (this.acm != null)
    {
      this.acm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnAcmCommunicationsStateUpdateEvent);
      this.runServiceButtonCoolantValveOpenStart.ServiceComplete -= new EventHandler<SingleServiceResultEventArgs>(this.OnRunServiceButtonCoolantValveOpenStartServiceComplete);
      this.runServiceButtonCoolantValveOpenStop.ServiceComplete -= new EventHandler<SingleServiceResultEventArgs>(this.OnRunServiceButtonCoolantValveOpenStopServiceComplete);
      this.digitalReadoutInstrumentCoolantValve.RepresentedStateChanged -= new EventHandler(this.OnDigitalReadoutInstrumentCoolantValveRepresentedStateChanged);
      if (this.durationTimer.Enabled && acm == null)
      {
        this.StopTimer();
        this.WriteMessage(Resources.Message_DisconnectionDetectedWhileProcedureRunningDEFCoolantValveMayStillBeOpen);
      }
    }
    this.acm = acm;
    if (this.acm != null)
    {
      this.acm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnAcmCommunicationsStateUpdateEvent);
      this.runServiceButtonCoolantValveOpenStart.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.OnRunServiceButtonCoolantValveOpenStartServiceComplete);
      this.runServiceButtonCoolantValveOpenStop.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.OnRunServiceButtonCoolantValveOpenStopServiceComplete);
      this.digitalReadoutInstrumentCoolantValve.RepresentedStateChanged += new EventHandler(this.OnDigitalReadoutInstrumentCoolantValveRepresentedStateChanged);
    }
    this.UpdateUserInterface();
  }

  private void OnAcmCommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.CanClose)
      this.SetAcm((Channel) null);
    else
      e.Cancel = true;
  }

  private void OnDurationTimerTick(object sender, EventArgs e)
  {
    this.StopTimer();
    if (!this.Online)
      return;
    ServiceCollection services = this.acm.Services;
    ServiceCall serviceCall = this.runServiceButtonCoolantValveOpenStop.ServiceCall;
    string qualifier = ((ServiceCall) ref serviceCall).Qualifier;
    Service service = services[qualifier];
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnStopServiceServiceCompleteEvent);
      service.Execute(false);
    }
  }

  private void StopTimer()
  {
    this.durationTimer.Stop();
    this.durationTimer.Tick -= new EventHandler(this.OnDurationTimerTick);
  }

  private void OnStopServiceServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (!(service != (Service) null))
      return;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnStopServiceServiceCompleteEvent);
    this.UpdateServiceStoppedMessage();
  }

  private void UpdateServiceStoppedMessage()
  {
    this.WriteMessage(Resources.Message_TheDEFCoolantValveHasBeenSetToClosed);
    this.UpdateUserInterface();
  }

  private void CoolantValveOpenStart()
  {
    int num = 30;
    if (!string.IsNullOrEmpty(((Control) this.decimalTextBoxDuration).Text) && this.decimalTextBoxDuration.Value > 0.0 && this.decimalTextBoxDuration.Value <= 300.0)
      num = Convert.ToInt32(this.decimalTextBoxDuration.Value);
    else
      ((Control) this.decimalTextBoxDuration).Text = num.ToString();
    this.durationTimer.Interval = num * 1000;
    this.durationTimer.Start();
    this.durationTimer.Tick += new EventHandler(this.OnDurationTimerTick);
    this.ClearMessages();
    this.WriteMessage(string.Format(Resources.MessageFormat_TheDEFCoolantValveHasBeenSetToOpenFor0Seconds, (object) num));
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    ((Control) this.runServiceButtonCoolantValveOpenStop).Enabled = this.CanStop;
    ((Control) this.runServiceButtonCoolantValveOpenStart).Enabled = this.CanStart;
    this.buttonClose.Enabled = this.CanClose;
  }

  private bool CoolantValveOpen
  {
    get
    {
      return this.acm != null && this.acm.Online && this.digitalReadoutInstrumentCoolantValve.RepresentedState == 1;
    }
  }

  private bool CanStart
  {
    get
    {
      return this.Online && !((RunSharedProcedureButtonBase) this.runServiceButtonCoolantValveOpenStart).IsBusy && !this.durationTimer.Enabled;
    }
  }

  private bool CanStop
  {
    get
    {
      return this.Online && !((RunSharedProcedureButtonBase) this.runServiceButtonCoolantValveOpenStop).IsBusy && this.durationTimer.Enabled;
    }
  }

  private bool CanClose => !this.durationTimer.Enabled;

  private bool Online => this.acm != null && this.acm.Online;

  private void ClearMessages() => this.textBoxOutput.Text = string.Empty;

  private void WriteMessage(string message)
  {
    this.textBoxOutput.AppendText(message);
    this.textBoxOutput.AppendText("\r\n");
    this.AddStatusMessage(message);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelBase = new TableLayoutPanel();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.barInstrumentCoolantTemperature = new BarInstrument();
    this.barInstrumentDefTankTemperature = new BarInstrument();
    this.runServiceButtonCoolantValveOpenStart = new RunServiceButton();
    this.runServiceButtonCoolantValveOpenStop = new RunServiceButton();
    this.buttonClose = new Button();
    this.digitalReadoutInstrumentCoolantValve = new DigitalReadoutInstrument();
    this.decimalTextBoxDuration = new DecimalTextBox();
    this.labelDuration = new System.Windows.Forms.Label();
    this.labelSeconds = new System.Windows.Forms.Label();
    this.textBoxOutput = new TextBox();
    ((Control) this.tableLayoutPanelBase).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelBase, "tableLayoutPanelBase");
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.barInstrumentCoolantTemperature, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.barInstrumentDefTankTemperature, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.runServiceButtonCoolantValveOpenStart, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.runServiceButtonCoolantValveOpenStop, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.buttonClose, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.digitalReadoutInstrumentCoolantValve, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.tableLayoutPanel1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.textBoxOutput, 0, 1);
    ((Control) this.tableLayoutPanelBase).Name = "tableLayoutPanelBase";
    this.barInstrumentCoolantTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentCoolantTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentCoolantTemperature, "barInstrumentCoolantTemperature");
    this.barInstrumentCoolantTemperature.FontGroup = "mainInstruments";
    ((SingleInstrumentBase) this.barInstrumentCoolantTemperature).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS002_Coolant_Temperature");
    ((Control) this.barInstrumentCoolantTemperature).Name = "barInstrumentCoolantTemperature";
    ((TableLayoutPanel) this.tableLayoutPanelBase).SetRowSpan((Control) this.barInstrumentCoolantTemperature, 3);
    ((SingleInstrumentBase) this.barInstrumentCoolantTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
    this.barInstrumentDefTankTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentDefTankTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentDefTankTemperature, "barInstrumentDefTankTemperature");
    this.barInstrumentDefTankTemperature.FontGroup = "mainInstruments";
    ((SingleInstrumentBase) this.barInstrumentDefTankTemperature).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentDefTankTemperature).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_AS022_DEF_tank_Temperature");
    ((Control) this.barInstrumentDefTankTemperature).Name = "barInstrumentDefTankTemperature";
    ((TableLayoutPanel) this.tableLayoutPanelBase).SetRowSpan((Control) this.barInstrumentDefTankTemperature, 3);
    ((SingleInstrumentBase) this.barInstrumentDefTankTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentDefTankTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentDefTankTemperature).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.runServiceButtonCoolantValveOpenStart, "runServiceButtonCoolantValveOpenStart");
    ((Control) this.runServiceButtonCoolantValveOpenStart).Name = "runServiceButtonCoolantValveOpenStart";
    this.runServiceButtonCoolantValveOpenStart.ServiceCall = new ServiceCall("ACM21T", "RT_Coolant_Valve_Open_Start_Coolant_Valve_Open_Status");
    componentResourceManager.ApplyResources((object) this.runServiceButtonCoolantValveOpenStop, "runServiceButtonCoolantValveOpenStop");
    ((Control) this.runServiceButtonCoolantValveOpenStop).Name = "runServiceButtonCoolantValveOpenStop";
    this.runServiceButtonCoolantValveOpenStop.ServiceCall = new ServiceCall("ACM21T", "RT_Coolant_Valve_Open_Stop");
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanelBase).SetColumnSpan((Control) this.digitalReadoutInstrumentCoolantValve, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCoolantValve, "digitalReadoutInstrumentCoolantValve");
    this.digitalReadoutInstrumentCoolantValve.FontGroup = "mainInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantValve).FreezeValue = false;
    this.digitalReadoutInstrumentCoolantValve.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentCoolantValve.Gradient.Modify(1, 0.0, (ValueState) 2);
    this.digitalReadoutInstrumentCoolantValve.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentCoolantValve.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentCoolantValve.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantValve).Instrument = new Qualifier((QualifierTypes) 1, "ACM21T", "DT_DS005_Coolant_Valve");
    ((Control) this.digitalReadoutInstrumentCoolantValve).Name = "digitalReadoutInstrumentCoolantValve";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantValve).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanelBase).SetColumnSpan((Control) this.tableLayoutPanel1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.decimalTextBoxDuration, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelDuration, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelSeconds, 2, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.decimalTextBoxDuration, "decimalTextBoxDuration");
    this.decimalTextBoxDuration.MaximumValue = 300.0;
    this.decimalTextBoxDuration.MinimumValue = 0.0;
    ((Control) this.decimalTextBoxDuration).Name = "decimalTextBoxDuration";
    this.decimalTextBoxDuration.Precision = new int?(0);
    this.decimalTextBoxDuration.Value = 30.0;
    componentResourceManager.ApplyResources((object) this.labelDuration, "labelDuration");
    this.labelDuration.Name = "labelDuration";
    this.labelDuration.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelSeconds, "labelSeconds");
    this.labelSeconds.Name = "labelSeconds";
    this.labelSeconds.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanelBase).SetColumnSpan((Control) this.textBoxOutput, 2);
    componentResourceManager.ApplyResources((object) this.textBoxOutput, "textBoxOutput");
    this.textBoxOutput.Name = "textBoxOutput";
    this.textBoxOutput.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_DEFCoolantValveControl");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelBase);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelBase).ResumeLayout(false);
    ((Control) this.tableLayoutPanelBase).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
