// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor_and_Unlock__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor_and_Unlock__EMG_.panel;

public class UserPanel : CustomPanel
{
  private InstrumentDataItem tiltSignalItem;
  private Image initialTruckImage;
  private TableLayoutPanel tableLayoutPanel;
  private DigitalReadoutInstrument digitalReadoutInstrumentSensorFailure;
  private PictureBox pictureBoxTruck;
  private RunServiceButton runServiceButtonTiltSensor;
  private SeekTimeListView seekTimeListView;
  private Panel panelCanStart;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelTiltSensor;
  private Checkmark checkmarkTiltSensor;
  private DigitalReadoutInstrument digitalReadoutInstrumentTeachInState;
  private DialInstrument dialInstrumentTiltSensorSignal;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransmission;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private RunServiceButton runServiceButtonReleaseLock;
  private Panel panelReleaseLock;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelReleaseLock;
  private Checkmark checkmarkReleaseLock;
  private DigitalReadoutInstrument digitalReadoutInstrumentCharging;
  private DigitalReadoutInstrument digitalReadoutInstrumentNotCalibratedFault;

  public static Bitmap RotateImage(Image image, PointF offset, float angle)
  {
    if (image == null)
      throw new ArgumentNullException(nameof (image));
    Bitmap bitmap = new Bitmap(image.Width, image.Height);
    bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
    Graphics graphics = Graphics.FromImage((Image) bitmap);
    graphics.TranslateTransform(offset.X, offset.Y);
    graphics.RotateTransform(angle);
    graphics.TranslateTransform(-offset.X, -offset.Y);
    graphics.DrawImage(image, new PointF(0.0f, 0.0f));
    return bitmap;
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.initialTruckImage = this.pictureBoxTruck.Image;
    this.runServiceButtonTiltSensor.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonTiltSensor_ServiceComplete);
    this.runServiceButtonReleaseLock.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButtonReleaseLock_ServiceComplete);
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.precondition_RepresentedStateChanged);
    this.digitalReadoutInstrumentTransmission.RepresentedStateChanged += new EventHandler(this.precondition_RepresentedStateChanged);
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.precondition_RepresentedStateChanged);
    this.digitalReadoutInstrumentNotCalibratedFault.RepresentedStateChanged += new EventHandler(this.precondition_RepresentedStateChanged);
  }

  public virtual void OnChannelsChanged()
  {
    if (this.tiltSignalItem != null)
      ((DataItem) this.tiltSignalItem).UpdateEvent -= new EventHandler<ResultEventArgs>(this.tiltSignal_UpdateEvent);
    this.tiltSignalItem = (InstrumentDataItem) DataItem.Create(((SingleInstrumentBase) this.dialInstrumentTiltSensorSignal).Instrument, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
    if (this.tiltSignalItem != null)
      ((DataItem) this.tiltSignalItem).UpdateEvent += new EventHandler<ResultEventArgs>(this.tiltSignal_UpdateEvent);
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    ((Control) this.runServiceButtonTiltSensor).Enabled = this.CanStartTiltSensor;
    this.checkmarkTiltSensor.CheckState = this.CanStartTiltSensor ? CheckState.Checked : CheckState.Unchecked;
    ((Control) this.runServiceButtonReleaseLock).Enabled = this.CanStartReleaseLock;
    this.checkmarkReleaseLock.CheckState = this.CanStartReleaseLock ? CheckState.Checked : CheckState.Unchecked;
    if (this.CanStartTiltSensor)
    {
      if (this.LearnNeeded)
        ((Control) this.labelTiltSensor).Text = Resources.Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton;
      else
        ((Control) this.labelTiltSensor).Text = Resources.Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton;
    }
    else if (!this.LearnServiceAvailable)
      ((Control) this.labelTiltSensor).Text = Resources.Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice;
    else if (!this.VehicleCheckOk)
    {
      if (this.VehicleCharging)
        ((Control) this.labelTiltSensor).Text = Resources.Message_CannotCalibrateTheTiltSensorTheVehicleIsCharging;
      else
        ((Control) this.labelTiltSensor).Text = Resources.Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied;
    }
    else
      ((Control) this.labelTiltSensor).Text = Resources.Message_CannotCalibrateTheTiltSensor;
    if (this.CanStartReleaseLock)
      ((Control) this.labelReleaseLock).Text = Resources.Message_Ready;
    else if (((RunSharedProcedureButtonBase) this.runServiceButtonReleaseLock).IsBusy)
      ((Control) this.labelReleaseLock).Text = Resources.Message_ReleasingTransportSecurity;
    else if (!this.ReleaseLockServiceAvailable)
      ((Control) this.labelReleaseLock).Text = Resources.Message_CannotReleaseTransportSecurityEitherTheTCMIsOfflineOrTheServiceCannotBeFound;
    else if (this.VehicleCharging)
      ((Control) this.labelReleaseLock).Text = Resources.Message_CannotReleaseTransportSecurityTheVehicleIsCharging;
    else
      ((Control) this.labelReleaseLock).Text = Resources.Message_CannotReleaseTransportSecurity;
  }

  private bool CanStartTiltSensor
  {
    get
    {
      return this.LearnServiceAvailable && this.VehicleCheckOk && !((RunSharedProcedureButtonBase) this.runServiceButtonTiltSensor).IsBusy && !((RunSharedProcedureButtonBase) this.runServiceButtonReleaseLock).IsBusy;
    }
  }

  private bool LearnServiceAvailable
  {
    get
    {
      ServiceCall serviceCall = this.runServiceButtonTiltSensor.ServiceCall;
      return ((ServiceCall) ref serviceCall).GetService() != (Service) null;
    }
  }

  private bool VehicleCheckOk
  {
    get
    {
      return this.digitalReadoutInstrumentParkBrake.RepresentedState != 3 && this.digitalReadoutInstrumentTransmission.RepresentedState == 1 && this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && !this.VehicleCharging;
    }
  }

  private bool VehicleCharging => this.digitalReadoutInstrumentCharging.RepresentedState != 1;

  private bool CanStartReleaseLock
  {
    get
    {
      return this.ReleaseLockServiceAvailable && !((RunSharedProcedureButtonBase) this.runServiceButtonTiltSensor).IsBusy && !((RunSharedProcedureButtonBase) this.runServiceButtonReleaseLock).IsBusy && !this.VehicleCharging;
    }
  }

  private bool ReleaseLockServiceAvailable
  {
    get
    {
      ServiceCall serviceCall = this.runServiceButtonReleaseLock.ServiceCall;
      return ((ServiceCall) ref serviceCall).GetService() != (Service) null;
    }
  }

  private bool LearnNeeded => this.digitalReadoutInstrumentNotCalibratedFault.RepresentedState == 3;

  private void precondition_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void runServiceButtonTiltSensor_ServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
      this.AddLabelLog(Resources.Message_CalibrationOfTiltSensorWasSucessful);
    else
      this.AddLabelLog(Resources.Message_CalibrationOfTiltSensorWasNotSucessful + ((ResultEventArgs) e).Exception.Message);
  }

  private void runServiceButtonReleaseLock_ServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
      this.AddLabelLog(Resources.Message_SuccessfullyReleasedTransportSecurity);
    else
      this.AddLabelLog(string.Format(Resources.MessageFormat_UnableToReleaseTransportSecurity0, ((ResultEventArgs) e).Exception == null ? (object) string.Empty : (object) (Resources.Message_Error + ((ResultEventArgs) e).Exception.Message)));
  }

  private void AddLabelLog(string text)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void tiltSignal_UpdateEvent(object sender, ResultEventArgs e)
  {
    if (this.tiltSignalItem == null || ((DataItem) this.tiltSignalItem).Value == null)
      return;
    double num = ((DataItem) this.tiltSignalItem).ValueAsDouble(((DataItem) this.tiltSignalItem).Value);
    if (!double.IsNaN(num))
    {
      Image image = this.pictureBoxTruck.Image;
      this.pictureBoxTruck.Image = (Image) UserPanel.RotateImage(this.initialTruckImage, new PointF((float) (this.initialTruckImage.Width / 2), (float) (this.initialTruckImage.Height / 2)), (float) num);
      if (image != this.initialTruckImage)
        image.Dispose();
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel = new TableLayoutPanel();
    this.digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
    this.panelReleaseLock = new Panel();
    this.labelReleaseLock = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.checkmarkReleaseLock = new Checkmark();
    this.runServiceButtonReleaseLock = new RunServiceButton();
    this.digitalReadoutInstrumentTeachInState = new DigitalReadoutInstrument();
    this.dialInstrumentTiltSensorSignal = new DialInstrument();
    this.pictureBoxTruck = new PictureBox();
    this.digitalReadoutInstrumentSensorFailure = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentNotCalibratedFault = new DigitalReadoutInstrument();
    this.runServiceButtonTiltSensor = new RunServiceButton();
    this.seekTimeListView = new SeekTimeListView();
    this.panelCanStart = new Panel();
    this.labelTiltSensor = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.checkmarkTiltSensor = new Checkmark();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransmission = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    this.panelReleaseLock.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxTruck).BeginInit();
    this.panelCanStart.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentCharging, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.panelReleaseLock, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.runServiceButtonReleaseLock, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentTeachInState, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.dialInstrumentTiltSensorSignal, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.pictureBoxTruck, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentSensorFailure, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentNotCalibratedFault, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.runServiceButtonTiltSensor, 3, 6);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.panelCanStart, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentTransmission, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 1, 1);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCharging, "digitalReadoutInstrumentCharging");
    this.digitalReadoutInstrumentCharging.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).FreezeValue = false;
    this.digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentCharging.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentCharging.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentCharging.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition");
    ((Control) this.digitalReadoutInstrumentCharging).Name = "digitalReadoutInstrumentCharging";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).ShowUnits = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentCharging.RepresentedStateChanged += new EventHandler(this.precondition_RepresentedStateChanged);
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.panelReleaseLock, 3);
    this.panelReleaseLock.Controls.Add((Control) this.labelReleaseLock);
    this.panelReleaseLock.Controls.Add((Control) this.checkmarkReleaseLock);
    componentResourceManager.ApplyResources((object) this.panelReleaseLock, "panelReleaseLock");
    this.panelReleaseLock.Name = "panelReleaseLock";
    this.labelReleaseLock.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelReleaseLock, "labelReleaseLock");
    ((Control) this.labelReleaseLock).Name = "labelReleaseLock";
    this.labelReleaseLock.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelReleaseLock.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.checkmarkReleaseLock, "checkmarkReleaseLock");
    ((Control) this.checkmarkReleaseLock).Name = "checkmarkReleaseLock";
    componentResourceManager.ApplyResources((object) this.runServiceButtonReleaseLock, "runServiceButtonReleaseLock");
    ((Control) this.runServiceButtonReleaseLock).Name = "runServiceButtonReleaseLock";
    this.runServiceButtonReleaseLock.ServiceCall = new ServiceCall("ETCM01T", "DJ_Release_transport_security_for_eTCM");
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTeachInState, "digitalReadoutInstrumentTeachInState");
    this.digitalReadoutInstrumentTeachInState.FontGroup = "";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTeachInState).FreezeValue = false;
    this.digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    this.digitalReadoutInstrumentTeachInState.Gradient.Initialize((ValueState) 0, 6);
    this.digitalReadoutInstrumentTeachInState.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentTeachInState.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentTeachInState.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentTeachInState.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentTeachInState.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentTeachInState.Gradient.Modify(6, (double) byte.MaxValue, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTeachInState).Instrument = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Transmission_Teach_in_State_current_state");
    ((Control) this.digitalReadoutInstrumentTeachInState).Name = "digitalReadoutInstrumentTeachInState";
    ((TableLayoutPanel) this.tableLayoutPanel).SetRowSpan((Control) this.digitalReadoutInstrumentTeachInState, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTeachInState).UnitAlignment = StringAlignment.Near;
    this.dialInstrumentTiltSensorSignal.AngleRange = 120.0;
    this.dialInstrumentTiltSensorSignal.AngleStart = 120.0;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.dialInstrumentTiltSensorSignal, 2);
    componentResourceManager.ApplyResources((object) this.dialInstrumentTiltSensorSignal, "dialInstrumentTiltSensorSignal");
    this.dialInstrumentTiltSensorSignal.FontGroup = "TiltSensorAnalogs";
    ((SingleInstrumentBase) this.dialInstrumentTiltSensorSignal).FreezeValue = false;
    ((SingleInstrumentBase) this.dialInstrumentTiltSensorSignal).Instrument = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Inclination_Sensor_Signal_Value_current_value");
    ((Control) this.dialInstrumentTiltSensorSignal).Name = "dialInstrumentTiltSensorSignal";
    this.dialInstrumentTiltSensorSignal.Orientation = Orientation.Horizontal;
    ((AxisSingleInstrumentBase) this.dialInstrumentTiltSensorSignal).PreferredAxisRange = new AxisRange(-60.0, 60.0, "%");
    ((SingleInstrumentBase) this.dialInstrumentTiltSensorSignal).UnitAlignment = StringAlignment.Near;
    this.pictureBoxTruck.BackColor = Color.White;
    componentResourceManager.ApplyResources((object) this.pictureBoxTruck, "pictureBoxTruck");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.pictureBoxTruck, 2);
    this.pictureBoxTruck.Name = "pictureBoxTruck";
    this.pictureBoxTruck.TabStop = false;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentSensorFailure, "digitalReadoutInstrumentSensorFailure");
    this.digitalReadoutInstrumentSensorFailure.FontGroup = "TiltSensorText";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentSensorFailure).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentSensorFailure).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "ETCM01T", "14F1EE");
    ((Control) this.digitalReadoutInstrumentSensorFailure).Name = "digitalReadoutInstrumentSensorFailure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentSensorFailure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentNotCalibratedFault, "digitalReadoutInstrumentNotCalibratedFault");
    this.digitalReadoutInstrumentNotCalibratedFault.FontGroup = "TiltSensorText";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNotCalibratedFault).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNotCalibratedFault).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "ETCM01T", "14F1ED");
    ((Control) this.digitalReadoutInstrumentNotCalibratedFault).Name = "digitalReadoutInstrumentNotCalibratedFault";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNotCalibratedFault).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.runServiceButtonTiltSensor, "runServiceButtonTiltSensor");
    ((Control) this.runServiceButtonTiltSensor).Name = "runServiceButtonTiltSensor";
    this.runServiceButtonTiltSensor.ServiceCall = new ServiceCall("ETCM01T", "RT_Inclination_Sensor_Teach_in_Procedure_Start");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.seekTimeListView, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Tilt Sensor";
    ((TableLayoutPanel) this.tableLayoutPanel).SetRowSpan((Control) this.seekTimeListView, 4);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.panelCanStart, 3);
    this.panelCanStart.Controls.Add((Control) this.labelTiltSensor);
    this.panelCanStart.Controls.Add((Control) this.checkmarkTiltSensor);
    componentResourceManager.ApplyResources((object) this.panelCanStart, "panelCanStart");
    this.panelCanStart.Name = "panelCanStart";
    this.labelTiltSensor.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelTiltSensor, "labelTiltSensor");
    ((Control) this.labelTiltSensor).Name = "labelTiltSensor";
    this.labelTiltSensor.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelTiltSensor.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.checkmarkTiltSensor, "checkmarkTiltSensor");
    ((Control) this.checkmarkTiltSensor).Name = "checkmarkTiltSensor";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = "";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState) 2);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransmission, "digitalReadoutInstrumentTransmission");
    this.digitalReadoutInstrumentTransmission.FontGroup = "TiltSensorText";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmission).FreezeValue = false;
    this.digitalReadoutInstrumentTransmission.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrumentTransmission.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmission.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmission).Instrument = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
    ((Control) this.digitalReadoutInstrumentTransmission).Name = "digitalReadoutInstrumentTransmission";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmission).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = "TiltSensorText";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Transmission_Tilt_Sensor_and_Unlock");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    this.panelReleaseLock.ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxTruck).EndInit();
    this.panelCanStart.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
