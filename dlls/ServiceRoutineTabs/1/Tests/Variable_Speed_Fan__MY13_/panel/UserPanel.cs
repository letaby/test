// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Tests.Variable_Speed_Fan__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Tests.Variable_Speed_Fan__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string PWMStartCommand = "RT_SR003_PWM_Routine_by_Function_Start_Control_Value";
  private const string PWMStopCommand = "RT_SR003_PWM_Routine_by_Function_Stop_Function_Name";
  public const string FanTypeParameterName = "Fan_Type";
  private const int Fan2Function = 5;
  private Service StartService;
  private Service StopService;
  private Channel mcm;
  private bool synchronizingFanTime;
  private bool synchronizingFanSpeed;
  private TableLayoutPanel tableLayoutPanelPanel;
  private DigitalReadoutInstrument digitalReadoutInstrumentFanSpeed;
  private BarInstrument barInstrumentCoolantTemp;
  private BarInstrument barInstrumentCoolantOutTemp;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;
  private TableLayoutPanel tableLayoutPanelRunFanControls;
  private TrackBar trackBarSpeed;
  private DecimalTextBox decimalTextBoxSpeed;
  private TableLayoutPanel tableLayoutPanelCheckMarkAndLabel;
  private Checkmark checkmarkVehicleStatus;
  private ScalingLabel scalingLabelVehicleStatus;
  private System.Windows.Forms.Label label1;
  private Button buttonStart;
  private Button buttonStop;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleStatus;
  private DigitalReadoutInstrument digitalReadoutInstrumentFanType;
  private System.Windows.Forms.Label label3;
  private TableLayoutPanel tableLayoutPanelFanTimeControl;
  private TrackBar trackBarTime;
  private System.Windows.Forms.Label label5;
  private System.Windows.Forms.Label label6;
  private DecimalTextBox decimalTextBoxTime;
  private TableLayoutPanel tableLayoutPanelFanSpeedControl;
  private SeekTimeListView seekTimeListView;

  private int FanTime
  {
    get
    {
      return !double.IsNaN(this.decimalTextBoxTime.Value) ? Convert.ToInt32((object) this.decimalTextBoxTime.Value, (IFormatProvider) CultureInfo.InvariantCulture) : 1;
    }
    set => this.decimalTextBoxTime.Value = (double) value;
  }

  private int FanSpeed
  {
    get
    {
      return !double.IsNaN(this.decimalTextBoxSpeed.Value) ? Convert.ToInt32((object) this.decimalTextBoxSpeed.Value, (IFormatProvider) CultureInfo.InvariantCulture) : 1;
    }
    set => this.decimalTextBoxSpeed.Value = (double) value;
  }

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += new EventHandler(this.RepresentedStateChanged);
    this.digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged += new EventHandler(this.RepresentedStateChanged);
    this.digitalReadoutInstrumentFanType.RepresentedStateChanged += new EventHandler(this.RepresentedStateChanged);
    this.InitTrackBars();
    this.UpdateUserInterface();
  }

  private void InitTrackBars()
  {
    this.trackBarSpeed.Minimum = (int) this.decimalTextBoxSpeed.MinimumValue;
    this.trackBarSpeed.Maximum = (int) this.decimalTextBoxSpeed.MaximumValue;
    this.trackBarSpeed.LargeChange = 10;
    this.trackBarSpeed.SmallChange = 1;
    this.trackBarSpeed.TickFrequency = this.trackBarSpeed.LargeChange;
    this.trackBarTime.Minimum = (int) this.decimalTextBoxTime.MinimumValue;
    this.trackBarTime.Maximum = (int) this.decimalTextBoxTime.MaximumValue;
    this.trackBarTime.LargeChange = (int) this.decimalTextBoxTime.MaximumValue / 10 > 0 ? (int) this.decimalTextBoxTime.MaximumValue / 10 : 1;
    this.trackBarTime.SmallChange = this.trackBarTime.LargeChange / 10 > 0 ? this.trackBarTime.LargeChange / 10 : 1;
    this.trackBarTime.TickFrequency = this.trackBarTime.LargeChange;
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM21T"));
    this.UpdateUserInterface();
  }

  private bool SetMCM(Channel mcm)
  {
    bool flag = false;
    if (this.mcm != mcm)
    {
      flag = true;
      if (this.mcm != null)
      {
        this.StopFan();
        this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        this.mcm.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.ParametersReadCompleteEvent);
        this.StartService = (Service) null;
        this.StopService = (Service) null;
      }
      this.mcm = mcm;
      if (this.mcm != null)
      {
        this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        this.mcm.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.ParametersReadCompleteEvent);
        this.StartService = this.mcm.Services["RT_SR003_PWM_Routine_by_Function_Start_Control_Value"];
        this.StopService = this.mcm.Services["RT_SR003_PWM_Routine_by_Function_Stop_Function_Name"];
        Parameter parameter = this.mcm.Parameters["Fan_Type"];
        if (parameter != null)
          this.mcm.Parameters.ReadGroup(parameter.GroupQualifier, false, false);
      }
    }
    return flag;
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!e.Succeeded)
      this.OutputMessage(string.Format(Resources.MessageFormat_ErrorReadingTheParametersTheFanTypeMayBeIncorrectError0, (object) e.Exception.Message.ToString()));
    this.UpdateUserInterface();
  }

  private bool Online => this.mcm != null && this.mcm.Online;

  private void UpdateFanControls(bool enable)
  {
    this.trackBarSpeed.Enabled = enable;
    this.trackBarTime.Enabled = enable;
    ((Control) this.decimalTextBoxSpeed).Enabled = enable;
    ((Control) this.decimalTextBoxTime).Enabled = enable;
  }

  private void UpdateUserInterface()
  {
    bool enable = false;
    this.checkmarkVehicleStatus.Checked = false;
    if (!this.Online)
    {
      ((Control) this.scalingLabelVehicleStatus).Text = Resources.Message_TheFanCannotBeRunBecauseTheMCMIsOffline;
      this.buttonStop.Enabled = false;
      this.buttonStart.Enabled = false;
    }
    else if (this.digitalReadoutInstrumentFanType.RepresentedState != 1)
    {
      ((Control) this.scalingLabelVehicleStatus).Text = Resources.Message_TheFanIsNotAVariableSpeedType;
      this.buttonStop.Enabled = false;
      this.buttonStart.Enabled = false;
    }
    else if (this.digitalReadoutInstrumentVehicleStatus.RepresentedState != 1)
    {
      ((Control) this.scalingLabelVehicleStatus).Text = Resources.Message_TheFanCannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL;
      this.buttonStop.Enabled = true;
      this.buttonStart.Enabled = false;
    }
    else if (this.digitalReadoutInstrumentEngineSpeed.RepresentedState != 1)
    {
      ((Control) this.scalingLabelVehicleStatus).Text = Resources.Message_TheFanCannotStartUntilTheEngineIsRunning;
      this.buttonStop.Enabled = true;
      this.buttonStart.Enabled = false;
    }
    else
    {
      ((Control) this.scalingLabelVehicleStatus).Text = Resources.Message_TheFanCanBeStarted;
      this.buttonStart.Enabled = true;
      this.buttonStop.Enabled = true;
      enable = true;
      this.checkmarkVehicleStatus.Checked = true;
    }
    this.UpdateFanControls(enable);
  }

  private void RepresentedStateChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void trackBarSpeed_Scroll(object sender, EventArgs e)
  {
    if (this.synchronizingFanSpeed)
      return;
    this.FanSpeed = this.trackBarSpeed.Value;
  }

  private void trackBarTime_Scroll(object sender, EventArgs e)
  {
    if (this.synchronizingFanTime)
      return;
    this.FanTime = this.trackBarTime.Value;
  }

  private void decimalTextBoxSpeed_ValueChanged(object sender, EventArgs e)
  {
    this.synchronizingFanSpeed = true;
    this.trackBarSpeed.Value = this.FanSpeed;
    this.synchronizingFanSpeed = false;
  }

  private void decimalTextBoxTime_ValueChanged(object sender, EventArgs e)
  {
    this.synchronizingFanTime = true;
    this.trackBarTime.Value = this.FanTime;
    this.synchronizingFanTime = false;
  }

  private void buttonStart_Click(object sender, EventArgs e)
  {
    this.OutputMessage(Resources.Message_StartingFan);
    this.StartFan();
  }

  private void buttonStop_Click(object sender, EventArgs e)
  {
    this.OutputMessage(Resources.Message_StoppingFan);
    this.StopFan();
  }

  private void StartFan()
  {
    if (!this.Online || !(this.StartService != (Service) null))
      return;
    this.StartService.InputValues[0].Value = (object) this.StartService.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.StartService.InputValues[1].Value = (object) (100 - this.FanSpeed);
    this.StartService.InputValues[2].Value = (object) (this.FanTime * 1000);
    this.StartService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.StartServiceCompleteEvent);
    this.StartService.Execute(false);
  }

  private void StopFan()
  {
    if (!this.Online || !(this.StopService != (Service) null))
      return;
    this.StopService.InputValues[0].Value = (object) this.StopService.InputValues[0].Choices.GetItemFromRawValue((object) 5);
    this.StopService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.StopServiceCompleteEvent);
    this.StopService.Execute(false);
  }

  private void StartServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.StartServiceCompleteEvent);
    if (e.Succeeded)
      this.OutputMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TheFanIsProgrammedToRunFor0Seconds, (object) this.FanTime));
    else
      this.OutputMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_AnErrorOccurredStartingTheFan0, (object) e.Exception.Message));
    this.UpdateUserInterface();
  }

  private void StopServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.StopServiceCompleteEvent);
    if (e.Succeeded)
      this.OutputMessage(Resources.Message_TheFanHasBeenStopped);
    else
      this.OutputMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_AnErrorOccurredStoppingTheFan, (object) e.Exception.Message));
    this.UpdateUserInterface();
  }

  private void OutputMessage(string message)
  {
    SapiExtensions.LabelLogWithPrefix(Sapi.GetSapi().LogFiles, this.seekTimeListView.RequiredUserLabelPrefix, message);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelFanSpeedControl = new TableLayoutPanel();
    this.trackBarSpeed = new TrackBar();
    this.label1 = new System.Windows.Forms.Label();
    this.label3 = new System.Windows.Forms.Label();
    this.decimalTextBoxSpeed = new DecimalTextBox();
    this.tableLayoutPanelCheckMarkAndLabel = new TableLayoutPanel();
    this.checkmarkVehicleStatus = new Checkmark();
    this.scalingLabelVehicleStatus = new ScalingLabel();
    this.tableLayoutPanelFanTimeControl = new TableLayoutPanel();
    this.trackBarTime = new TrackBar();
    this.label5 = new System.Windows.Forms.Label();
    this.label6 = new System.Windows.Forms.Label();
    this.decimalTextBoxTime = new DecimalTextBox();
    this.tableLayoutPanelRunFanControls = new TableLayoutPanel();
    this.digitalReadoutInstrumentFanType = new DigitalReadoutInstrument();
    this.seekTimeListView = new SeekTimeListView();
    this.buttonStop = new Button();
    this.buttonStart = new Button();
    this.digitalReadoutInstrumentVehicleStatus = new DigitalReadoutInstrument();
    this.tableLayoutPanelPanel = new TableLayoutPanel();
    this.digitalReadoutInstrumentFanSpeed = new DigitalReadoutInstrument();
    this.barInstrumentCoolantTemp = new BarInstrument();
    this.barInstrumentCoolantOutTemp = new BarInstrument();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanelFanSpeedControl).SuspendLayout();
    this.trackBarSpeed.BeginInit();
    ((Control) this.tableLayoutPanelCheckMarkAndLabel).SuspendLayout();
    ((Control) this.tableLayoutPanelFanTimeControl).SuspendLayout();
    this.trackBarTime.BeginInit();
    ((Control) this.tableLayoutPanelRunFanControls).SuspendLayout();
    ((Control) this.tableLayoutPanelPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFanSpeedControl, "tableLayoutPanelFanSpeedControl");
    ((TableLayoutPanel) this.tableLayoutPanelFanSpeedControl).Controls.Add((Control) this.trackBarSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFanSpeedControl).Controls.Add((Control) this.label1, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelFanSpeedControl).Controls.Add((Control) this.label3, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelFanSpeedControl).Controls.Add((Control) this.decimalTextBoxSpeed, 1, 1);
    ((Control) this.tableLayoutPanelFanSpeedControl).Name = "tableLayoutPanelFanSpeedControl";
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).SetRowSpan((Control) this.tableLayoutPanelFanSpeedControl, 2);
    ((TableLayoutPanel) this.tableLayoutPanelFanSpeedControl).SetColumnSpan((Control) this.trackBarSpeed, 3);
    componentResourceManager.ApplyResources((object) this.trackBarSpeed, "trackBarSpeed");
    this.trackBarSpeed.LargeChange = 10;
    this.trackBarSpeed.Maximum = 95;
    this.trackBarSpeed.Minimum = 1;
    this.trackBarSpeed.Name = "trackBarSpeed";
    this.trackBarSpeed.TickFrequency = 20;
    this.trackBarSpeed.Value = 95;
    this.trackBarSpeed.Scroll += new EventHandler(this.trackBarSpeed_Scroll);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    this.label3.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.decimalTextBoxSpeed, "decimalTextBoxSpeed");
    this.decimalTextBoxSpeed.MaximumValue = 95.0;
    this.decimalTextBoxSpeed.MinimumValue = 1.0;
    ((Control) this.decimalTextBoxSpeed).Name = "decimalTextBoxSpeed";
    this.decimalTextBoxSpeed.Precision = new int?(0);
    this.decimalTextBoxSpeed.Value = 95.0;
    this.decimalTextBoxSpeed.ValueChanged += new EventHandler(this.decimalTextBoxSpeed_ValueChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelCheckMarkAndLabel, "tableLayoutPanelCheckMarkAndLabel");
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).SetColumnSpan((Control) this.tableLayoutPanelCheckMarkAndLabel, 2);
    ((TableLayoutPanel) this.tableLayoutPanelCheckMarkAndLabel).Controls.Add((Control) this.checkmarkVehicleStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelCheckMarkAndLabel).Controls.Add((Control) this.scalingLabelVehicleStatus, 1, 0);
    ((Control) this.tableLayoutPanelCheckMarkAndLabel).Name = "tableLayoutPanelCheckMarkAndLabel";
    componentResourceManager.ApplyResources((object) this.checkmarkVehicleStatus, "checkmarkVehicleStatus");
    ((Control) this.checkmarkVehicleStatus).Name = "checkmarkVehicleStatus";
    this.scalingLabelVehicleStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.scalingLabelVehicleStatus, "scalingLabelVehicleStatus");
    this.scalingLabelVehicleStatus.FontGroup = (string) null;
    this.scalingLabelVehicleStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelVehicleStatus).Name = "scalingLabelVehicleStatus";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFanTimeControl, "tableLayoutPanelFanTimeControl");
    ((TableLayoutPanel) this.tableLayoutPanelFanTimeControl).Controls.Add((Control) this.trackBarTime, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFanTimeControl).Controls.Add((Control) this.label5, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelFanTimeControl).Controls.Add((Control) this.label6, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelFanTimeControl).Controls.Add((Control) this.decimalTextBoxTime, 1, 1);
    ((Control) this.tableLayoutPanelFanTimeControl).Name = "tableLayoutPanelFanTimeControl";
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).SetRowSpan((Control) this.tableLayoutPanelFanTimeControl, 2);
    ((TableLayoutPanel) this.tableLayoutPanelFanTimeControl).SetColumnSpan((Control) this.trackBarTime, 3);
    componentResourceManager.ApplyResources((object) this.trackBarTime, "trackBarTime");
    this.trackBarTime.LargeChange = 30;
    this.trackBarTime.Maximum = 600;
    this.trackBarTime.Minimum = 1;
    this.trackBarTime.Name = "trackBarTime";
    this.trackBarTime.TickFrequency = 20;
    this.trackBarTime.Value = 30;
    this.trackBarTime.Scroll += new EventHandler(this.trackBarTime_Scroll);
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    this.label5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label6, "label6");
    this.label6.Name = "label6";
    this.label6.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.decimalTextBoxTime, "decimalTextBoxTime");
    this.decimalTextBoxTime.MaximumValue = 600.0;
    this.decimalTextBoxTime.MinimumValue = 1.0;
    ((Control) this.decimalTextBoxTime).Name = "decimalTextBoxTime";
    this.decimalTextBoxTime.Precision = new int?(0);
    this.decimalTextBoxTime.Value = 30.0;
    this.decimalTextBoxTime.ValueChanged += new EventHandler(this.decimalTextBoxTime_ValueChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelRunFanControls, "tableLayoutPanelRunFanControls");
    ((TableLayoutPanel) this.tableLayoutPanelPanel).SetColumnSpan((Control) this.tableLayoutPanelRunFanControls, 2);
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).Controls.Add((Control) this.digitalReadoutInstrumentFanType, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).Controls.Add((Control) this.tableLayoutPanelFanTimeControl, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).Controls.Add((Control) this.tableLayoutPanelCheckMarkAndLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).Controls.Add((Control) this.tableLayoutPanelFanSpeedControl, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).Controls.Add((Control) this.seekTimeListView, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).Controls.Add((Control) this.buttonStop, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).Controls.Add((Control) this.buttonStart, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).Controls.Add((Control) this.digitalReadoutInstrumentVehicleStatus, 1, 3);
    ((Control) this.tableLayoutPanelRunFanControls).Name = "tableLayoutPanelRunFanControls";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFanType, "digitalReadoutInstrumentFanType");
    this.digitalReadoutInstrumentFanType.FontGroup = "VSP Half Digital Inst";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFanType).FreezeValue = false;
    this.digitalReadoutInstrumentFanType.Gradient.Initialize((ValueState) 3, 14);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(4, 3.0, (ValueState) 1);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(6, 5.0, (ValueState) 1);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(7, 6.0, (ValueState) 3);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(8, 7.0, (ValueState) 3);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(9, 8.0, (ValueState) 1);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(10, 9.0, (ValueState) 1);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(11, 10.0, (ValueState) 1);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(12, 11.0, (ValueState) 1);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(13, 12.0, (ValueState) 1);
    this.digitalReadoutInstrumentFanType.Gradient.Modify(14, 13.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFanType).Instrument = new Qualifier((QualifierTypes) 4, "MCM21T", "Fan_Type");
    ((Control) this.digitalReadoutInstrumentFanType).Name = "digitalReadoutInstrumentFanType";
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).SetRowSpan((Control) this.digitalReadoutInstrumentFanType, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFanType).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).SetColumnSpan((Control) this.seekTimeListView, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "VariableSpeedFan";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.f";
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    this.buttonStop.Click += new EventHandler(this.buttonStop_Click);
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.buttonStart.Click += new EventHandler(this.buttonStart_Click);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleStatus, "digitalReadoutInstrumentVehicleStatus");
    this.digitalReadoutInstrumentVehicleStatus.FontGroup = "VSP Half Digital Inst";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleStatus).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleStatus.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentVehicleStatus.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleStatus.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutInstrumentVehicleStatus).Name = "digitalReadoutInstrumentVehicleStatus";
    ((TableLayoutPanel) this.tableLayoutPanelRunFanControls).SetRowSpan((Control) this.digitalReadoutInstrumentVehicleStatus, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleStatus).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelPanel, "tableLayoutPanelPanel");
    ((TableLayoutPanel) this.tableLayoutPanelPanel).Controls.Add((Control) this.tableLayoutPanelRunFanControls, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelPanel).Controls.Add((Control) this.digitalReadoutInstrumentFanSpeed, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelPanel).Controls.Add((Control) this.barInstrumentCoolantTemp, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelPanel).Controls.Add((Control) this.barInstrumentCoolantOutTemp, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelPanel).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 0, 0);
    ((Control) this.tableLayoutPanelPanel).Name = "tableLayoutPanelPanel";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFanSpeed, "digitalReadoutInstrumentFanSpeed");
    this.digitalReadoutInstrumentFanSpeed.FontGroup = "VSF Digital Instruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFanSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentFanSpeed.Gradient.Initialize((ValueState) 0, 1);
    this.digitalReadoutInstrumentFanSpeed.Gradient.Modify(1, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFanSpeed).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS026_Fan_Speed");
    ((Control) this.digitalReadoutInstrumentFanSpeed).Name = "digitalReadoutInstrumentFanSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFanSpeed).UnitAlignment = StringAlignment.Near;
    this.barInstrumentCoolantTemp.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentCoolantTemp.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentCoolantTemp, "barInstrumentCoolantTemp");
    this.barInstrumentCoolantTemp.FontGroup = "VSF Thermometers";
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp");
    ((Control) this.barInstrumentCoolantTemp).Name = "barInstrumentCoolantTemp";
    ((TableLayoutPanel) this.tableLayoutPanelPanel).SetRowSpan((Control) this.barInstrumentCoolantTemp, 2);
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).UnitAlignment = StringAlignment.Near;
    this.barInstrumentCoolantOutTemp.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentCoolantOutTemp.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentCoolantOutTemp, "barInstrumentCoolantOutTemp");
    this.barInstrumentCoolantOutTemp.FontGroup = "VSF Thermometers";
    ((SingleInstrumentBase) this.barInstrumentCoolantOutTemp).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentCoolantOutTemp).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS169_Coolant_out_temperature");
    ((Control) this.barInstrumentCoolantOutTemp).Name = "barInstrumentCoolantOutTemp";
    ((TableLayoutPanel) this.tableLayoutPanelPanel).SetRowSpan((Control) this.barInstrumentCoolantOutTemp, 2);
    ((SingleInstrumentBase) this.barInstrumentCoolantOutTemp).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentCoolantOutTemp).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentCoolantOutTemp).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = "VSF Digital Instruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_VariableSpeedFanControl");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelFanSpeedControl).ResumeLayout(false);
    ((Control) this.tableLayoutPanelFanSpeedControl).PerformLayout();
    this.trackBarSpeed.EndInit();
    ((Control) this.tableLayoutPanelCheckMarkAndLabel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelFanTimeControl).ResumeLayout(false);
    ((Control) this.tableLayoutPanelFanTimeControl).PerformLayout();
    this.trackBarTime.EndInit();
    ((Control) this.tableLayoutPanelRunFanControls).ResumeLayout(false);
    ((Control) this.tableLayoutPanelPanel).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
