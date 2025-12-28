// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ESC_Learn_Procedure.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ESC_Learn_Procedure.panel;

public class UserPanel : CustomPanel
{
  private string absEcuName;
  private TableLayoutPanel tableLayoutPanel1;
  private Checkmark engineSpeedCheck;
  private Button buttonClose;
  private RunServiceButton runServiceButtonESCLearningStop;
  private DigitalReadoutInstrument digitalReadoutInstrumentServiceModeActive;
  private DigitalReadoutInstrument digitalReadoutInstrumentOffsetSteeringAngleLearned;
  private DigitalReadoutInstrument digitalReadoutInstrumentSteeringWheelAngle;
  private SeekTimeListView seekTimeListView;
  private DigitalReadoutInstrument digitalReadoutInstrumentLateralAccel;
  private DigitalReadoutInstrument digitalReadoutInstrumentOffsetLateralAccelerationLearned;
  private DialInstrument dialInstrumentVehicleSpeed;
  private RunServiceButton runServiceButtonESCLearningStart;
  private DigitalReadoutInstrument digitalReadoutInstrumentYawRate;
  private TableLayoutPanel tableLayoutPanel2;
  private TableLayoutPanel tableLayoutPanel3;
  private ScalingLabel scalingLabel1;
  private System.Windows.Forms.Label labelMessage;
  private System.Windows.Forms.Label engineStatusLabel;

  private bool ProcedureIsRunning { get; set; }

  private bool ProcedureIsComplete { get; set; }

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.InitUserInterface();
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    this.runServiceButtonESCLearningStart.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonESCLearningStart_ServiceComplete);
    this.runServiceButtonESCLearningStop.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonESCLearningStop_ServiceComplete);
    this.dialInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.StartCondition_RepresentedStateChanged);
    this.digitalReadoutInstrumentServiceModeActive.RepresentedStateChanged += new EventHandler(this.ServiceModeActive_RepresentedStateChanged);
  }

  private void InitUserInterface()
  {
    Channel channel = this.GetChannel("ABS02T", (CustomPanel.ChannelLookupOptions) 5);
    this.absEcuName = channel == null ? "ABS02T" : channel.Ecu.Name;
    this.SetDigitalReadoutInstrument(this.digitalReadoutInstrumentServiceModeActive);
    this.SetDigitalReadoutInstrument(this.digitalReadoutInstrumentSteeringWheelAngle);
    this.SetDigitalReadoutInstrument(this.digitalReadoutInstrumentYawRate);
    this.SetDigitalReadoutInstrument(this.digitalReadoutInstrumentOffsetSteeringAngleLearned);
    this.SetDigitalReadoutInstrument(this.digitalReadoutInstrumentOffsetLateralAccelerationLearned);
    ((Control) this.digitalReadoutInstrumentLateralAccel).Visible = this.absEcuName == "ABS02T";
    this.SetRunServiceButton(this.runServiceButtonESCLearningStart);
    this.SetRunServiceButton(this.runServiceButtonESCLearningStop);
  }

  private void SetDigitalReadoutInstrument(DigitalReadoutInstrument digitalReadoutInstrument)
  {
    int num;
    if (digitalReadoutInstrument != null)
    {
      Qualifier instrument = ((SingleInstrumentBase) digitalReadoutInstrument).Instrument;
      num = false ? 1 : 0;
    }
    else
      num = 1;
    if (num != 0)
      return;
    DigitalReadoutInstrument readoutInstrument = digitalReadoutInstrument;
    string absEcuName = this.absEcuName;
    Qualifier instrument1 = ((SingleInstrumentBase) digitalReadoutInstrument).Instrument;
    string name = ((Qualifier) ref instrument1).Name;
    Qualifier qualifier = new Qualifier(absEcuName, name);
    ((SingleInstrumentBase) readoutInstrument).Instrument = qualifier;
  }

  private void SetRunServiceButton(RunServiceButton runServiceButton)
  {
    int num;
    if (runServiceButton != null)
    {
      ServiceCall serviceCall1 = runServiceButton.ServiceCall;
      ServiceCall serviceCall2 = runServiceButton.ServiceCall;
      num = ((ServiceCall) ref serviceCall2).Qualifier == null ? 1 : 0;
    }
    else
      num = 1;
    if (num != 0)
      return;
    RunServiceButton runServiceButton1 = runServiceButton;
    string absEcuName = this.absEcuName;
    ServiceCall serviceCall3 = runServiceButton.ServiceCall;
    string qualifier = ((ServiceCall) ref serviceCall3).Qualifier;
    ServiceCall serviceCall4 = runServiceButton.ServiceCall;
    IList<string> inputs = ((ServiceCall) ref serviceCall4).Inputs;
    ServiceCall serviceCall5 = new ServiceCall(absEcuName, qualifier, (IEnumerable<string>) inputs);
    runServiceButton1.ServiceCall = serviceCall5;
  }

  private bool VehicleIsMoving
  {
    get
    {
      return ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).DataItem != null && ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).DataItem.ValueAsDouble(((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).DataItem.Value) > 0.0;
    }
  }

  private bool VehicleIsNotMoving
  {
    get
    {
      return ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).DataItem != null && ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).DataItem.ValueAsDouble(((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).DataItem.Value) <= 0.0;
    }
  }

  private bool InstrumentsHaveLearnedStatus
  {
    get
    {
      return this.digitalReadoutInstrumentOffsetSteeringAngleLearned.RepresentedState == 1 && this.digitalReadoutInstrumentOffsetLateralAccelerationLearned.RepresentedState == 1;
    }
  }

  private void UpdateUserInterface()
  {
    this.engineSpeedCheck.Checked = this.ProcedureIsRunning ? this.VehicleIsMoving : this.VehicleIsNotMoving;
    ((Control) this.runServiceButtonESCLearningStart).Enabled = !this.ProcedureIsRunning && this.VehicleIsNotMoving;
    ((Control) this.runServiceButtonESCLearningStop).Enabled = this.ProcedureIsRunning;
    this.UpdateProcedureatureStatusText();
    this.SetVehicleSpeedGradient();
  }

  private void AddLogLabel(string text)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void UpdateProcedureatureStatusText()
  {
    if (this.ProcedureIsComplete)
    {
      this.engineStatusLabel.Text = Resources.Message_ESCLearnProcedureComplete;
      if (!this.VehicleIsMoving)
        return;
      System.Windows.Forms.Label engineStatusLabel = this.engineStatusLabel;
      engineStatusLabel.Text = $"{engineStatusLabel.Text} {Resources.Message_YouMayStopTheVehicle}";
    }
    else if (this.ProcedureIsRunning)
    {
      if (this.VehicleIsMoving)
        this.engineStatusLabel.Text = Resources.Message_ProcedureRunning;
      else
        this.engineStatusLabel.Text = Resources.Message_StartDrivingTheTruck;
    }
    else if (this.VehicleIsNotMoving)
      this.engineStatusLabel.Text = Resources.Message_ProcedureCanStart;
    else
      this.engineStatusLabel.Text = Resources.Message_WheelBasedSpeedMustBeZero;
  }

  private void UpdateInstrumentLearnedStatus()
  {
    if (this.InstrumentsHaveLearnedStatus && this.ProcedureIsRunning)
    {
      this.ProcedureIsComplete = true;
      this.ProcedureIsRunning = false;
    }
    this.UpdateUserInterface();
  }

  private void SetVehicleSpeedGradient()
  {
    if (this.ProcedureIsRunning)
      ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient = Gradient.FromString("(Fault),(1,Default),(15,Ok)");
    else
      ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient = Gradient.FromString("(Ok),(1,Fault)");
    ((Control) this.dialInstrumentVehicleSpeed).Refresh();
  }

  private void StartCondition_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void ServiceModeActive_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if ((((RunSharedProcedureButtonBase) this.runServiceButtonESCLearningStart).InProgress || ((RunSharedProcedureButtonBase) this.runServiceButtonESCLearningStop).InProgress) && e.CloseReason == CloseReason.UserClosing)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.runServiceButtonESCLearningStart.ServiceComplete -= new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonESCLearningStart_ServiceComplete);
    this.runServiceButtonESCLearningStop.ServiceComplete -= new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonESCLearningStop_ServiceComplete);
    this.dialInstrumentVehicleSpeed.RepresentedStateChanged -= new EventHandler(this.StartCondition_RepresentedStateChanged);
    this.digitalReadoutInstrumentOffsetLateralAccelerationLearned.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrumentOffsetLateralAccelerationAngleLearned_RepresentedStateChanged);
    this.dialInstrumentVehicleSpeed.RepresentedStateChanged -= new EventHandler(this.dialInstrumentVehicleSpeed_RepresentedStateChanged);
    this.digitalReadoutInstrumentOffsetSteeringAngleLearned.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrumentOffsetSteeringAngleLearned_RepresentedStateChanged);
  }

  private void digitalReadoutInstrumentOffsetSteeringAngleLearned_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateInstrumentLearnedStatus();
  }

  private void digitalReadoutInstrumentOffsetLateralAccelerationAngleLearned_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateInstrumentLearnedStatus();
  }

  private void dialInstrumentVehicleSpeed_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void runServiceButtonESCLearningStop_ServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      this.AddLogLabel(Resources.Message_ProcedureStopped);
      this.ProcedureIsRunning = false;
    }
    else
      this.AddLogLabel(string.Format(Resources.Message_ErrorStoppingProcedure, (object) ((ResultEventArgs) e).Exception.Message));
    this.UpdateUserInterface();
  }

  private void runServiceButtonESCLearningStart_ServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      this.AddLogLabel(Resources.Message_ProcedureStarted);
      this.ProcedureIsRunning = true;
      this.ProcedureIsComplete = false;
    }
    else
      this.AddLogLabel(string.Format(Resources.Message_ErrorStartingProcedure, (object) ((ResultEventArgs) e).Exception.Message));
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.labelMessage = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentYawRate = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentOffsetLateralAccelerationLearned = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentOffsetSteeringAngleLearned = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentLateralAccel = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentServiceModeActive = new DigitalReadoutInstrument();
    this.dialInstrumentVehicleSpeed = new DialInstrument();
    this.seekTimeListView = new SeekTimeListView();
    this.digitalReadoutInstrumentSteeringWheelAngle = new DigitalReadoutInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.runServiceButtonESCLearningStart = new RunServiceButton();
    this.runServiceButtonESCLearningStop = new RunServiceButton();
    this.buttonClose = new Button();
    this.scalingLabel1 = new ScalingLabel();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.engineSpeedCheck = new Checkmark();
    this.engineStatusLabel = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelMessage, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentYawRate, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentOffsetLateralAccelerationLearned, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentOffsetSteeringAngleLearned, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentLateralAccel, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentServiceModeActive, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dialInstrumentVehicleSpeed, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentSteeringWheelAngle, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 0, 7);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.labelMessage, "labelMessage");
    this.labelMessage.BackColor = Color.Transparent;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelMessage, 2);
    this.labelMessage.Name = "labelMessage";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentYawRate, "digitalReadoutInstrumentYawRate");
    this.digitalReadoutInstrumentYawRate.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentYawRate).FreezeValue = false;
    this.digitalReadoutInstrumentYawRate.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentYawRate.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentYawRate.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentYawRate.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentYawRate.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentYawRate).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Yaw_rate_plausibility");
    ((Control) this.digitalReadoutInstrumentYawRate).Name = "digitalReadoutInstrumentYawRate";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentYawRate).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentOffsetLateralAccelerationLearned, "digitalReadoutInstrumentOffsetLateralAccelerationLearned");
    this.digitalReadoutInstrumentOffsetLateralAccelerationLearned.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOffsetLateralAccelerationLearned).FreezeValue = false;
    this.digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOffsetLateralAccelerationLearned).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Offset_lateral_acceleration_learned");
    ((Control) this.digitalReadoutInstrumentOffsetLateralAccelerationLearned).Name = "digitalReadoutInstrumentOffsetLateralAccelerationLearned";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOffsetLateralAccelerationLearned).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentOffsetLateralAccelerationLearned.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentOffsetLateralAccelerationAngleLearned_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentOffsetSteeringAngleLearned, "digitalReadoutInstrumentOffsetSteeringAngleLearned");
    this.digitalReadoutInstrumentOffsetSteeringAngleLearned.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOffsetSteeringAngleLearned).FreezeValue = false;
    this.digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOffsetSteeringAngleLearned).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_1_Read_Offset_steering_wheel_angle_learned");
    ((Control) this.digitalReadoutInstrumentOffsetSteeringAngleLearned).Name = "digitalReadoutInstrumentOffsetSteeringAngleLearned";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOffsetSteeringAngleLearned).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentOffsetSteeringAngleLearned.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentOffsetSteeringAngleLearned_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentLateralAccel, "digitalReadoutInstrumentLateralAccel");
    this.digitalReadoutInstrumentLateralAccel.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentLateralAccel).FreezeValue = false;
    this.digitalReadoutInstrumentLateralAccel.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentLateralAccel.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentLateralAccel.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentLateralAccel.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentLateralAccel.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentLateralAccel).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_1_Read_Lateral_acceleration_plausibility");
    ((Control) this.digitalReadoutInstrumentLateralAccel).Name = "digitalReadoutInstrumentLateralAccel";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentLateralAccel).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentServiceModeActive, "digitalReadoutInstrumentServiceModeActive");
    this.digitalReadoutInstrumentServiceModeActive.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentServiceModeActive).FreezeValue = false;
    this.digitalReadoutInstrumentServiceModeActive.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentServiceModeActive.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentServiceModeActive.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentServiceModeActive.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentServiceModeActive.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentServiceModeActive).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Service_mode_active");
    ((Control) this.digitalReadoutInstrumentServiceModeActive).Name = "digitalReadoutInstrumentServiceModeActive";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentServiceModeActive).UnitAlignment = StringAlignment.Near;
    this.dialInstrumentVehicleSpeed.AngleRange = 135.0;
    this.dialInstrumentVehicleSpeed.AngleStart = -180.0;
    componentResourceManager.ApplyResources((object) this.dialInstrumentVehicleSpeed, "dialInstrumentVehicleSpeed");
    this.dialInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Initialize((ValueState) 3, 2, "km/h");
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(1, 1.0, (ValueState) 0);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(2, 15.0, (ValueState) 1);
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.dialInstrumentVehicleSpeed).Name = "dialInstrumentVehicleSpeed";
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).PreferredAxisRange = new AxisRange(0.0, 100.0, "km/h");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.dialInstrumentVehicleSpeed, 4);
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.dialInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.dialInstrumentVehicleSpeed_RepresentedStateChanged);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "ESC Leaning Procedure";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.f";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentSteeringWheelAngle, "digitalReadoutInstrumentSteeringWheelAngle");
    this.digitalReadoutInstrumentSteeringWheelAngle.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentSteeringWheelAngle).FreezeValue = false;
    this.digitalReadoutInstrumentSteeringWheelAngle.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentSteeringWheelAngle.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentSteeringWheelAngle.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentSteeringWheelAngle.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentSteeringWheelAngle.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentSteeringWheelAngle).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Steering_wheel_angle_plausibility");
    ((Control) this.digitalReadoutInstrumentSteeringWheelAngle).Name = "digitalReadoutInstrumentSteeringWheelAngle";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentSteeringWheelAngle).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.runServiceButtonESCLearningStart, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.runServiceButtonESCLearningStop, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClose, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.scalingLabel1, 2, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.runServiceButtonESCLearningStart, "runServiceButtonESCLearningStart");
    ((Control) this.runServiceButtonESCLearningStart).Name = "runServiceButtonESCLearningStart";
    this.runServiceButtonESCLearningStart.ServiceCall = new ServiceCall("ABS02T", "RT_Start_ESC_learning_Start_Routine_Start", (IEnumerable<string>) new string[1]
    {
      "Learning_mode=2"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonESCLearningStop, "runServiceButtonESCLearningStop");
    ((Control) this.runServiceButtonESCLearningStop).Name = "runServiceButtonESCLearningStop";
    this.runServiceButtonESCLearningStop.ServiceCall = new ServiceCall("ABS02T", "RT_Start_ESC_learning_Start_Routine_Start", (IEnumerable<string>) new string[1]
    {
      "Learning_mode=3"
    });
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.scalingLabel1.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.scalingLabel1, "scalingLabel1");
    this.scalingLabel1.FontGroup = (string) null;
    this.scalingLabel1.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel1).Name = "scalingLabel1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.engineSpeedCheck, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.engineStatusLabel, 1, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.engineSpeedCheck, "engineSpeedCheck");
    ((Control) this.engineSpeedCheck).Name = "engineSpeedCheck";
    componentResourceManager.ApplyResources((object) this.engineStatusLabel, "engineStatusLabel");
    this.engineStatusLabel.Name = "engineStatusLabel";
    this.engineStatusLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
