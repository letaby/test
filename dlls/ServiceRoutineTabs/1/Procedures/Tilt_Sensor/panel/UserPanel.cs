// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor.panel;

public class UserPanel : CustomPanel
{
  private Channel Tcm = (Channel) null;
  private InstrumentDataItem tiltSignalItem;
  private Image initialTruckImage;
  private TableLayoutPanel tableLayoutPanel;
  private DigitalReadoutInstrument digitalReadoutInstrumentTiltSignalError;
  private PictureBox pictureBoxTruck;
  private RunServiceButton runServiceButtonTiltSensor;
  private SeekTimeListView seekTimeListView;
  private Panel panelCanStart;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelCanStart;
  private Checkmark checkmarkCanStart;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleCheck;
  private DialInstrument dialInstrumentTiltSensorSignal;
  private DigitalReadoutInstrument digitalReadoutInstrumentNotLearnedFault;

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
    this.runServiceButtonTiltSensor.ServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.runServiceButton_ServiceComplete);
    this.digitalReadoutInstrumentVehicleCheck.RepresentedStateChanged += new EventHandler(this.precondition_RepresentedStateChanged);
    this.digitalReadoutInstrumentNotLearnedFault.RepresentedStateChanged += new EventHandler(this.precondition_RepresentedStateChanged);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetTcm(this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 7));
    if (this.tiltSignalItem != null)
      ((DataItem) this.tiltSignalItem).UpdateEvent -= new EventHandler<ResultEventArgs>(this.tiltSignal_UpdateEvent);
    this.tiltSignalItem = (InstrumentDataItem) DataItem.Create(((SingleInstrumentBase) this.dialInstrumentTiltSensorSignal).Instrument, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
    if (this.tiltSignalItem != null)
      ((DataItem) this.tiltSignalItem).UpdateEvent += new EventHandler<ResultEventArgs>(this.tiltSignal_UpdateEvent);
    this.UpdateUserInterface();
  }

  private void SetTcm(Channel tcm)
  {
    if (this.Tcm == tcm)
      return;
    this.Tcm = tcm;
    if (this.Tcm != null)
    {
      foreach (SingleInstrumentBase singleInstrumentBase1 in CustomPanel.GetControlsOfType(((Control) this).Controls, typeof (SingleInstrumentBase)))
      {
        Qualifier instrument = singleInstrumentBase1.Instrument;
        Ecu ecuByName = SapiManager.GetEcuByName(((Qualifier) ref instrument).Ecu);
        if (ecuByName != null && ecuByName.Identifier == this.Tcm.Ecu.Identifier && ecuByName.Name != this.Tcm.Ecu.Name)
        {
          SingleInstrumentBase singleInstrumentBase2 = singleInstrumentBase1;
          instrument = singleInstrumentBase1.Instrument;
          QualifierTypes type = ((Qualifier) ref instrument).Type;
          string name1 = this.Tcm.Ecu.Name;
          instrument = singleInstrumentBase1.Instrument;
          string name2 = ((Qualifier) ref instrument).Name;
          Qualifier qualifier = new Qualifier(type, name1, name2);
          singleInstrumentBase2.Instrument = qualifier;
        }
      }
      this.runServiceButtonTiltSensor.ServiceCall = new ServiceCall(this.Tcm.Ecu.Name, "RT_0430_Nullpunktabgleich_Neigungssensor_Start");
    }
  }

  private void UpdateUserInterface()
  {
    ((Control) this.runServiceButtonTiltSensor).Enabled = this.CanStart;
    this.checkmarkCanStart.CheckState = this.CanStart ? CheckState.Checked : CheckState.Unchecked;
    string str = Resources.Message_CannotCalibrateTheTiltSensor;
    if (this.CanStart)
      str = !this.LearnNeeded ? Resources.Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton : Resources.Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton;
    else if (!this.LearnServiceAvailable)
      str = Resources.Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice;
    else if (!this.VehicleCheckOk)
      str = Resources.Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied;
    ((Control) this.labelCanStart).Text = str;
  }

  private bool CanStart => this.LearnServiceAvailable && this.VehicleCheckOk;

  private bool LearnServiceAvailable
  {
    get
    {
      ServiceCall serviceCall = this.runServiceButtonTiltSensor.ServiceCall;
      return ((ServiceCall) ref serviceCall).GetService() != (Service) null;
    }
  }

  private bool VehicleCheckOk => this.digitalReadoutInstrumentVehicleCheck.RepresentedState != 3;

  private bool LearnNeeded => this.digitalReadoutInstrumentNotLearnedFault.RepresentedState == 3;

  private void precondition_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void runServiceButton_ServiceComplete(object sender, SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
      this.AddLabelLog(Resources.Message_CalibrationOfTiltSensorWasSucessful);
    else
      this.AddLabelLog(Resources.Message_CalibrationOfTiltSensorWasNotSucessful + ((ResultEventArgs) e).Exception.Message);
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
    this.digitalReadoutInstrumentVehicleCheck = new DigitalReadoutInstrument();
    this.dialInstrumentTiltSensorSignal = new DialInstrument();
    this.pictureBoxTruck = new PictureBox();
    this.digitalReadoutInstrumentTiltSignalError = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentNotLearnedFault = new DigitalReadoutInstrument();
    this.runServiceButtonTiltSensor = new RunServiceButton();
    this.seekTimeListView = new SeekTimeListView();
    this.panelCanStart = new Panel();
    this.labelCanStart = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.checkmarkCanStart = new Checkmark();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((ISupportInitialize) this.pictureBoxTruck).BeginInit();
    this.panelCanStart.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentVehicleCheck, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.dialInstrumentTiltSensorSignal, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.pictureBoxTruck, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentTiltSignalError, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentNotLearnedFault, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.runServiceButtonTiltSensor, 4, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.seekTimeListView, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.panelCanStart, 0, 4);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.digitalReadoutInstrumentVehicleCheck, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleCheck, "digitalReadoutInstrumentVehicleCheck");
    this.digitalReadoutInstrumentVehicleCheck.FontGroup = "TiltSensorText";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheck).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleCheck.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentVehicleCheck.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleCheck.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleCheck.Gradient.Modify(3, 2.0, (ValueState) 2);
    this.digitalReadoutInstrumentVehicleCheck.Gradient.Modify(4, 3.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheck).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutInstrumentVehicleCheck).Name = "digitalReadoutInstrumentVehicleCheck";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleCheck).UnitAlignment = StringAlignment.Near;
    this.dialInstrumentTiltSensorSignal.AngleRange = 120.0;
    this.dialInstrumentTiltSensorSignal.AngleStart = 120.0;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.dialInstrumentTiltSensorSignal, 2);
    componentResourceManager.ApplyResources((object) this.dialInstrumentTiltSensorSignal, "dialInstrumentTiltSensorSignal");
    this.dialInstrumentTiltSensorSignal.FontGroup = "TiltSensorAnalogs";
    ((SingleInstrumentBase) this.dialInstrumentTiltSensorSignal).FreezeValue = false;
    ((SingleInstrumentBase) this.dialInstrumentTiltSensorSignal).Instrument = new Qualifier((QualifierTypes) 1, "TCM01T", "DT_msd18_Signal_Neigungssensor_Signal_Neigungssensor");
    ((Control) this.dialInstrumentTiltSensorSignal).Name = "dialInstrumentTiltSensorSignal";
    this.dialInstrumentTiltSensorSignal.Orientation = Orientation.Horizontal;
    ((AxisSingleInstrumentBase) this.dialInstrumentTiltSensorSignal).PreferredAxisRange = new AxisRange(-60.0, 60.0, "%");
    ((SingleInstrumentBase) this.dialInstrumentTiltSensorSignal).UnitAlignment = StringAlignment.Near;
    this.pictureBoxTruck.BackColor = Color.Black;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.pictureBoxTruck, 3);
    componentResourceManager.ApplyResources((object) this.pictureBoxTruck, "pictureBoxTruck");
    this.pictureBoxTruck.Name = "pictureBoxTruck";
    this.pictureBoxTruck.TabStop = false;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.digitalReadoutInstrumentTiltSignalError, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTiltSignalError, "digitalReadoutInstrumentTiltSignalError");
    this.digitalReadoutInstrumentTiltSignalError.FontGroup = "TiltSensorText";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTiltSignalError).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTiltSignalError).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "TCM01T", "26F1EE");
    ((Control) this.digitalReadoutInstrumentTiltSignalError).Name = "digitalReadoutInstrumentTiltSignalError";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTiltSignalError).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.digitalReadoutInstrumentNotLearnedFault, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentNotLearnedFault, "digitalReadoutInstrumentNotLearnedFault");
    this.digitalReadoutInstrumentNotLearnedFault.FontGroup = "TiltSensorText";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNotLearnedFault).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNotLearnedFault).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "TCM01T", "26F1ED");
    ((Control) this.digitalReadoutInstrumentNotLearnedFault).Name = "digitalReadoutInstrumentNotLearnedFault";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNotLearnedFault).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.runServiceButtonTiltSensor, "runServiceButtonTiltSensor");
    ((Control) this.runServiceButtonTiltSensor).Name = "runServiceButtonTiltSensor";
    this.runServiceButtonTiltSensor.ServiceCall = new ServiceCall("TCM01T", "RT_0430_Nullpunktabgleich_Neigungssensor_Start");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.seekTimeListView, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Tilt Sensor";
    ((TableLayoutPanel) this.tableLayoutPanel).SetRowSpan((Control) this.seekTimeListView, 3);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.panelCanStart, 4);
    this.panelCanStart.Controls.Add((Control) this.labelCanStart);
    this.panelCanStart.Controls.Add((Control) this.checkmarkCanStart);
    componentResourceManager.ApplyResources((object) this.panelCanStart, "panelCanStart");
    this.panelCanStart.Name = "panelCanStart";
    this.labelCanStart.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelCanStart, "labelCanStart");
    ((Control) this.labelCanStart).Name = "labelCanStart";
    this.labelCanStart.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelCanStart.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.checkmarkCanStart, "checkmarkCanStart");
    ((Control) this.checkmarkCanStart).Name = "checkmarkCanStart";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Tilt_Sensor");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxTruck).EndInit();
    this.panelCanStart.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
