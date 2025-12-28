// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Position_Control.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Position_Control.panel;

public class UserPanel : CustomPanel
{
  private const string StartQualifier = "RT_SR080_Control_EGR_valve_position_Start_Status";
  private const string StopQualifier = "RT_SR080_Control_EGR_valve_position_Stop_Status";
  private Channel channel;
  private bool testRunning;
  private BarInstrument barInstrumentCoolantTemp;
  private TableLayoutPanel tableLayoutPanelInterface;
  private System.Windows.Forms.Label labelDesiredPosition;
  private TextBox textBoxDesiredPosition;
  private Button buttonStart;
  private Button buttonStop;
  private BarInstrument barInstrumentBatteryVoltage;
  private BarInstrument barInstrumentCommandedValue;
  private BarInstrument barInstrumentActualValue;
  private Panel panelButtons;
  private System.Windows.Forms.Label labelCanStart;
  private Checkmark checkmarkCanStart;
  private SeekTimeListView seekTimeListView1;
  private Button buttonMinus;
  private Button buttonPlus;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineState;
  private TableLayoutPanel tableLayoutPanelMain;

  public UserPanel() => this.InitializeComponent();

  public virtual void OnChannelsChanged() => this.SetChannel(this.GetChannel("MCM21T"));

  private void SetChannel(Channel channel)
  {
    if (this.channel == channel)
      return;
    if (this.channel != null)
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.channel = channel;
    if (this.channel != null)
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.UpdateUserInterface();
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.TestRunning)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.SetChannel((Channel) null);
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.buttonStart.Enabled = this.CanStart;
    this.buttonStop.Enabled = this.CanStop;
    this.checkmarkCanStart.CheckState = this.CanStart || this.TestRunning ? CheckState.Checked : CheckState.Unchecked;
    string str = Resources.Message_TestCanStart;
    if (!this.buttonStart.Enabled)
    {
      if (this.TestRunning)
        str = Resources.Message_TestIsInProgress;
      else if (this.Busy)
        str = Resources.Message_CannotStartAsDeviceIsBusy;
      else if (!this.Online)
        str = Resources.Message_CannotStartAsDeviceIsNotOnline;
      else if (!this.InputPosition.HasValue)
        str = Resources.Message_CannotStartAsPositionNotValid;
      else if (this.EngineRunning)
        str = Resources.Message_CannotStartAsEngineIsRunningStopEngine;
    }
    this.labelCanStart.Text = str;
  }

  private void Output(string text)
  {
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void StopTest(string reason)
  {
    this.testRunning = false;
    this.Output(reason);
    this.UpdateUserInterface();
    if (this.Online)
    {
      this.Output(Resources.Message_RequestEndEGRManipulation);
      this.channel.Services["RT_SR080_Control_EGR_valve_position_Stop_Status"].Execute(false);
    }
    else
      this.Output(Resources.Message_UnableToRequestEndEGRManipulation);
  }

  private void SetDesiredPosition(int position)
  {
    if (this.Online)
    {
      if (!this.Busy)
      {
        Service service = this.channel.Services["RT_SR080_Control_EGR_valve_position_Start_Status"];
        service.InputValues[0].Value = (object) 0;
        service.InputValues[1].Value = (object) position;
        service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.startService_ServiceCompleteEvent);
        service.Execute(false);
      }
      else
        this.Output(Resources.Message_ECUIsBusy);
    }
    else
      this.StopTest(Resources.Message_ECUOfflineTestAborted);
  }

  private bool CanStart
  {
    get
    {
      return this.Online && this.InputPosition.HasValue && this.ServicesAvailable && !this.TestRunning && !this.Busy && !this.EngineRunning;
    }
  }

  private bool CanStop => this.Online && this.TestRunning;

  private bool TestRunning => this.testRunning;

  private bool Online => this.channel != null && this.channel.Online;

  private bool Busy
  {
    get => this.Online && this.channel.CommunicationsState != CommunicationsState.Online;
  }

  private bool ServicesAvailable
  {
    get
    {
      return this.channel != null && this.channel.Services["RT_SR080_Control_EGR_valve_position_Start_Status"] != (Service) null && this.channel.Services["RT_SR080_Control_EGR_valve_position_Stop_Status"] != (Service) null;
    }
  }

  private int? InputPosition
  {
    get
    {
      int result;
      return int.TryParse(this.textBoxDesiredPosition.Text, out result) && result >= 0 && result <= 100 ? new int?(result) : new int?();
    }
  }

  private bool EngineRunning => this.digitalReadoutInstrumentEngineState.RepresentedState != 1;

  private void buttonStart_Click(object sender, EventArgs e)
  {
    this.testRunning = true;
    this.Output(Resources.Message_TestStarted);
    this.UpdateUserInterface();
    this.SetDesiredPosition(this.InputPosition.Value);
  }

  private void buttonStop_Click(object sender, EventArgs e)
  {
    this.StopTest(Resources.Message_TestCompleteDueToUserRequest);
  }

  private void buttonPlus_Click(object sender, EventArgs e)
  {
    int? nullable1 = this.InputPosition;
    if (!nullable1.HasValue || nullable1.Value > 95)
      return;
    int? nullable2 = nullable1;
    nullable1 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault() + 5) : new int?();
    this.textBoxDesiredPosition.Text = nullable1.Value.ToString();
  }

  private void buttonMinus_Click(object sender, EventArgs e)
  {
    int? nullable1 = this.InputPosition;
    if (!nullable1.HasValue || nullable1.Value < 5)
      return;
    int? nullable2 = nullable1;
    nullable1 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault() - 5) : new int?();
    this.textBoxDesiredPosition.Text = nullable1.Value.ToString();
  }

  private void textBoxDesiredPosition_TextChanged(object sender, EventArgs e)
  {
    if (this.testRunning)
    {
      int? inputPosition = this.InputPosition;
      if (!inputPosition.HasValue)
        return;
      this.SetDesiredPosition(inputPosition.Value);
    }
    else
      this.UpdateUserInterface();
  }

  private void startService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.startService_ServiceCompleteEvent);
    if (e.Succeeded)
      this.Output($"{Resources.Message_SetPosition}{service.InputValues[1].Value}%: {service.OutputValues[0].Value}");
    else
      this.Output($"{Resources.Message_SetPosition}{service.InputValues[1].Value}%: {e.Exception.Message}");
  }

  private void digitalReadoutInstrumentEngineState_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    if (this.testRunning && this.EngineRunning)
      this.StopTest(Resources.Message_EngineStateIncorrect);
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelInterface = new TableLayoutPanel();
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.panelButtons = new Panel();
    this.buttonMinus = new Button();
    this.labelDesiredPosition = new System.Windows.Forms.Label();
    this.textBoxDesiredPosition = new TextBox();
    this.seekTimeListView1 = new SeekTimeListView();
    this.buttonPlus = new Button();
    this.barInstrumentCoolantTemp = new BarInstrument();
    this.barInstrumentBatteryVoltage = new BarInstrument();
    this.barInstrumentCommandedValue = new BarInstrument();
    this.barInstrumentActualValue = new BarInstrument();
    this.digitalReadoutInstrumentEngineState = new DigitalReadoutInstrument();
    this.labelCanStart = new System.Windows.Forms.Label();
    this.checkmarkCanStart = new Checkmark();
    this.buttonStart = new Button();
    this.buttonStop = new Button();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelInterface).SuspendLayout();
    this.panelButtons.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelInterface, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.barInstrumentCoolantTemp, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.barInstrumentBatteryVoltage, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.barInstrumentCommandedValue, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.barInstrumentActualValue, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentEngineState, 2, 2);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInterface, "tableLayoutPanelInterface");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelInterface, 4);
    ((TableLayoutPanel) this.tableLayoutPanelInterface).Controls.Add((Control) this.buttonMinus, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInterface).Controls.Add((Control) this.labelDesiredPosition, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInterface).Controls.Add((Control) this.textBoxDesiredPosition, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInterface).Controls.Add((Control) this.seekTimeListView1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInterface).Controls.Add((Control) this.buttonPlus, 3, 0);
    ((Control) this.tableLayoutPanelInterface).Name = "tableLayoutPanelInterface";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.tableLayoutPanelInterface, 3);
    componentResourceManager.ApplyResources((object) this.buttonMinus, "buttonMinus");
    this.buttonMinus.Name = "buttonMinus";
    this.buttonMinus.UseCompatibleTextRendering = true;
    this.buttonMinus.UseVisualStyleBackColor = true;
    this.buttonMinus.Click += new EventHandler(this.buttonMinus_Click);
    componentResourceManager.ApplyResources((object) this.labelDesiredPosition, "labelDesiredPosition");
    this.labelDesiredPosition.Name = "labelDesiredPosition";
    ((TableLayoutPanel) this.tableLayoutPanelInterface).SetRowSpan((Control) this.labelDesiredPosition, 2);
    this.labelDesiredPosition.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxDesiredPosition, "textBoxDesiredPosition");
    this.textBoxDesiredPosition.Name = "textBoxDesiredPosition";
    ((TableLayoutPanel) this.tableLayoutPanelInterface).SetRowSpan((Control) this.textBoxDesiredPosition, 2);
    this.textBoxDesiredPosition.TextChanged += new EventHandler(this.textBoxDesiredPosition_TextChanged);
    ((TableLayoutPanel) this.tableLayoutPanelInterface).SetColumnSpan((Control) this.seekTimeListView1, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "EGRCommand";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "HH:mm:ss.fff";
    componentResourceManager.ApplyResources((object) this.buttonPlus, "buttonPlus");
    this.buttonPlus.Name = "buttonPlus";
    this.buttonPlus.UseCompatibleTextRendering = true;
    this.buttonPlus.UseVisualStyleBackColor = true;
    this.buttonPlus.Click += new EventHandler(this.buttonPlus_Click);
    this.barInstrumentCoolantTemp.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentCoolantTemp.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentCoolantTemp, "barInstrumentCoolantTemp");
    this.barInstrumentCoolantTemp.FontGroup = "EGRCommandVertical";
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS013_Coolant_Temperature");
    ((Control) this.barInstrumentCoolantTemp).Name = "barInstrumentCoolantTemp";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.barInstrumentCoolantTemp, 6);
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).UnitAlignment = StringAlignment.Near;
    this.barInstrumentBatteryVoltage.BarOrientation = (BarControl.ControlOrientation) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentBatteryVoltage, "barInstrumentBatteryVoltage");
    this.barInstrumentBatteryVoltage.FontGroup = "EGRCommandVertical";
    ((SingleInstrumentBase) this.barInstrumentBatteryVoltage).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentBatteryVoltage).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS021_Battery_Voltage");
    ((Control) this.barInstrumentBatteryVoltage).Name = "barInstrumentBatteryVoltage";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.barInstrumentBatteryVoltage, 6);
    ((SingleInstrumentBase) this.barInstrumentBatteryVoltage).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentBatteryVoltage).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentBatteryVoltage).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.barInstrumentCommandedValue, 4);
    componentResourceManager.ApplyResources((object) this.barInstrumentCommandedValue, "barInstrumentCommandedValue");
    this.barInstrumentCommandedValue.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentCommandedValue).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentCommandedValue).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS031_EGR_Commanded_Governor_Value");
    ((Control) this.barInstrumentCommandedValue).Name = "barInstrumentCommandedValue";
    ((SingleInstrumentBase) this.barInstrumentCommandedValue).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.barInstrumentActualValue, 4);
    componentResourceManager.ApplyResources((object) this.barInstrumentActualValue, "barInstrumentActualValue");
    this.barInstrumentActualValue.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentActualValue).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentActualValue).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS032_EGR_Actual_Valve_Position");
    ((Control) this.barInstrumentActualValue).Name = "barInstrumentActualValue";
    ((SingleInstrumentBase) this.barInstrumentActualValue).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.digitalReadoutInstrumentEngineState, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineState, "digitalReadoutInstrumentEngineState");
    this.digitalReadoutInstrumentEngineState.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).FreezeValue = false;
    this.digitalReadoutInstrumentEngineState.Gradient.Initialize((ValueState) 0, 8);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(1, -1.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(2, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(3, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(4, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(5, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(6, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(7, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(8, 6.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS023_Engine_State");
    ((Control) this.digitalReadoutInstrumentEngineState).Name = "digitalReadoutInstrumentEngineState";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentEngineState.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentEngineState_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.panelButtons, "panelButtons");
    this.panelButtons.Controls.Add((Control) this.labelCanStart);
    this.panelButtons.Controls.Add((Control) this.checkmarkCanStart);
    this.panelButtons.Controls.Add((Control) this.buttonStart);
    this.panelButtons.Controls.Add((Control) this.buttonStop);
    this.panelButtons.Name = "panelButtons";
    componentResourceManager.ApplyResources((object) this.labelCanStart, "labelCanStart");
    this.labelCanStart.Name = "labelCanStart";
    this.labelCanStart.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkCanStart, "checkmarkCanStart");
    ((Control) this.checkmarkCanStart).Name = "checkmarkCanStart";
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.buttonStart.Click += new EventHandler(this.buttonStart_Click);
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    this.buttonStop.Click += new EventHandler(this.buttonStop_Click);
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Controls.Add((Control) this.panelButtons);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInterface).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInterface).PerformLayout();
    this.panelButtons.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
