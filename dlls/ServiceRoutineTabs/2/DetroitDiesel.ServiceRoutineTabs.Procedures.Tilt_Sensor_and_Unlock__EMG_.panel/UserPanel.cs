using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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

	private Label labelTiltSensor;

	private Checkmark checkmarkTiltSensor;

	private DigitalReadoutInstrument digitalReadoutInstrumentTeachInState;

	private DialInstrument dialInstrumentTiltSensorSignal;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransmission;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;

	private RunServiceButton runServiceButtonReleaseLock;

	private Panel panelReleaseLock;

	private Label labelReleaseLock;

	private Checkmark checkmarkReleaseLock;

	private DigitalReadoutInstrument digitalReadoutInstrumentCharging;

	private DigitalReadoutInstrument digitalReadoutInstrumentNotCalibratedFault;

	private bool CanStartTiltSensor => LearnServiceAvailable && VehicleCheckOk && !((RunSharedProcedureButtonBase)runServiceButtonTiltSensor).IsBusy && !((RunSharedProcedureButtonBase)runServiceButtonReleaseLock).IsBusy;

	private bool LearnServiceAvailable
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			ServiceCall serviceCall = runServiceButtonTiltSensor.ServiceCall;
			return ((ServiceCall)(ref serviceCall)).GetService() != null;
		}
	}

	private bool VehicleCheckOk => (int)digitalReadoutInstrumentParkBrake.RepresentedState != 3 && (int)digitalReadoutInstrumentTransmission.RepresentedState == 1 && (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && !VehicleCharging;

	private bool VehicleCharging => (int)digitalReadoutInstrumentCharging.RepresentedState != 1;

	private bool CanStartReleaseLock => ReleaseLockServiceAvailable && !((RunSharedProcedureButtonBase)runServiceButtonTiltSensor).IsBusy && !((RunSharedProcedureButtonBase)runServiceButtonReleaseLock).IsBusy && !VehicleCharging;

	private bool ReleaseLockServiceAvailable
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			ServiceCall serviceCall = runServiceButtonReleaseLock.ServiceCall;
			return ((ServiceCall)(ref serviceCall)).GetService() != null;
		}
	}

	private bool LearnNeeded => (int)digitalReadoutInstrumentNotCalibratedFault.RepresentedState == 3;

	public static Bitmap RotateImage(Image image, PointF offset, float angle)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		Bitmap bitmap = new Bitmap(image.Width, image.Height);
		bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
		Graphics graphics = Graphics.FromImage(bitmap);
		graphics.TranslateTransform(offset.X, offset.Y);
		graphics.RotateTransform(angle);
		graphics.TranslateTransform(0f - offset.X, 0f - offset.Y);
		graphics.DrawImage(image, new PointF(0f, 0f));
		return bitmap;
	}

	public UserPanel()
	{
		InitializeComponent();
		initialTruckImage = pictureBoxTruck.Image;
		runServiceButtonTiltSensor.ServiceComplete += runServiceButtonTiltSensor_ServiceComplete;
		runServiceButtonReleaseLock.ServiceComplete += runServiceButtonReleaseLock_ServiceComplete;
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += precondition_RepresentedStateChanged;
		digitalReadoutInstrumentTransmission.RepresentedStateChanged += precondition_RepresentedStateChanged;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += precondition_RepresentedStateChanged;
		digitalReadoutInstrumentNotCalibratedFault.RepresentedStateChanged += precondition_RepresentedStateChanged;
	}

	public override void OnChannelsChanged()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		if (tiltSignalItem != null)
		{
			((DataItem)tiltSignalItem).UpdateEvent -= tiltSignal_UpdateEvent;
		}
		tiltSignalItem = (InstrumentDataItem)DataItem.Create(((SingleInstrumentBase)dialInstrumentTiltSensorSignal).Instrument, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
		if (tiltSignalItem != null)
		{
			((DataItem)tiltSignalItem).UpdateEvent += tiltSignal_UpdateEvent;
		}
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		((Control)(object)runServiceButtonTiltSensor).Enabled = CanStartTiltSensor;
		checkmarkTiltSensor.CheckState = (CanStartTiltSensor ? CheckState.Checked : CheckState.Unchecked);
		((Control)(object)runServiceButtonReleaseLock).Enabled = CanStartReleaseLock;
		checkmarkReleaseLock.CheckState = (CanStartReleaseLock ? CheckState.Checked : CheckState.Unchecked);
		if (CanStartTiltSensor)
		{
			if (LearnNeeded)
			{
				((Control)(object)labelTiltSensor).Text = Resources.Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton;
			}
			else
			{
				((Control)(object)labelTiltSensor).Text = Resources.Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton;
			}
		}
		else if (!LearnServiceAvailable)
		{
			((Control)(object)labelTiltSensor).Text = Resources.Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice;
		}
		else if (!VehicleCheckOk)
		{
			if (VehicleCharging)
			{
				((Control)(object)labelTiltSensor).Text = Resources.Message_CannotCalibrateTheTiltSensorTheVehicleIsCharging;
			}
			else
			{
				((Control)(object)labelTiltSensor).Text = Resources.Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied;
			}
		}
		else
		{
			((Control)(object)labelTiltSensor).Text = Resources.Message_CannotCalibrateTheTiltSensor;
		}
		if (CanStartReleaseLock)
		{
			((Control)(object)labelReleaseLock).Text = Resources.Message_Ready;
		}
		else if (((RunSharedProcedureButtonBase)runServiceButtonReleaseLock).IsBusy)
		{
			((Control)(object)labelReleaseLock).Text = Resources.Message_ReleasingTransportSecurity;
		}
		else if (!ReleaseLockServiceAvailable)
		{
			((Control)(object)labelReleaseLock).Text = Resources.Message_CannotReleaseTransportSecurityEitherTheTCMIsOfflineOrTheServiceCannotBeFound;
		}
		else if (VehicleCharging)
		{
			((Control)(object)labelReleaseLock).Text = Resources.Message_CannotReleaseTransportSecurityTheVehicleIsCharging;
		}
		else
		{
			((Control)(object)labelReleaseLock).Text = Resources.Message_CannotReleaseTransportSecurity;
		}
	}

	private void precondition_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void runServiceButtonTiltSensor_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLabelLog(Resources.Message_CalibrationOfTiltSensorWasSucessful);
		}
		else
		{
			AddLabelLog(Resources.Message_CalibrationOfTiltSensorWasNotSucessful + ((ResultEventArgs)(object)e).Exception.Message);
		}
	}

	private void runServiceButtonReleaseLock_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLabelLog(Resources.Message_SuccessfullyReleasedTransportSecurity);
		}
		else
		{
			AddLabelLog(string.Format(Resources.MessageFormat_UnableToReleaseTransportSecurity0, (((ResultEventArgs)(object)e).Exception == null) ? string.Empty : (Resources.Message_Error + ((ResultEventArgs)(object)e).Exception.Message)));
		}
	}

	private void AddLabelLog(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
	}

	private void tiltSignal_UpdateEvent(object sender, ResultEventArgs e)
	{
		if (tiltSignalItem == null || ((DataItem)tiltSignalItem).Value == null)
		{
			return;
		}
		double num = ((DataItem)tiltSignalItem).ValueAsDouble(((DataItem)tiltSignalItem).Value);
		if (!double.IsNaN(num))
		{
			Image image = pictureBoxTruck.Image;
			pictureBoxTruck.Image = RotateImage(initialTruckImage, new PointF(initialTruckImage.Width / 2, initialTruckImage.Height / 2), (float)num);
			if (image != initialTruckImage)
			{
				image.Dispose();
			}
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0948: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4f: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
		panelReleaseLock = new Panel();
		labelReleaseLock = new Label();
		checkmarkReleaseLock = new Checkmark();
		runServiceButtonReleaseLock = new RunServiceButton();
		digitalReadoutInstrumentTeachInState = new DigitalReadoutInstrument();
		dialInstrumentTiltSensorSignal = new DialInstrument();
		pictureBoxTruck = new PictureBox();
		digitalReadoutInstrumentSensorFailure = new DigitalReadoutInstrument();
		digitalReadoutInstrumentNotCalibratedFault = new DigitalReadoutInstrument();
		runServiceButtonTiltSensor = new RunServiceButton();
		seekTimeListView = new SeekTimeListView();
		panelCanStart = new Panel();
		labelTiltSensor = new Label();
		checkmarkTiltSensor = new Checkmark();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransmission = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		panelReleaseLock.SuspendLayout();
		((ISupportInitialize)pictureBoxTruck).BeginInit();
		panelCanStart.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentCharging, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(panelReleaseLock, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)runServiceButtonReleaseLock, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentTeachInState, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)dialInstrumentTiltSensorSignal, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(pictureBoxTruck, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentSensorFailure, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentNotCalibratedFault, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)runServiceButtonTiltSensor, 3, 6);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(panelCanStart, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentTransmission, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 1, 1);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCharging, "digitalReadoutInstrumentCharging");
		digitalReadoutInstrumentCharging.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).FreezeValue = false;
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentCharging.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentCharging.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentCharging.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).Instrument = new Qualifier((QualifierTypes)16, "fake", "FakeIsChargingPrecondition");
		((Control)(object)digitalReadoutInstrumentCharging).Name = "digitalReadoutInstrumentCharging";
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCharging).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentCharging.RepresentedStateChanged += precondition_RepresentedStateChanged;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)panelReleaseLock, 3);
		panelReleaseLock.Controls.Add((Control)(object)labelReleaseLock);
		panelReleaseLock.Controls.Add((Control)(object)checkmarkReleaseLock);
		componentResourceManager.ApplyResources(panelReleaseLock, "panelReleaseLock");
		panelReleaseLock.Name = "panelReleaseLock";
		labelReleaseLock.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelReleaseLock, "labelReleaseLock");
		((Control)(object)labelReleaseLock).Name = "labelReleaseLock";
		labelReleaseLock.Orientation = (TextOrientation)1;
		labelReleaseLock.UseSystemColors = true;
		componentResourceManager.ApplyResources(checkmarkReleaseLock, "checkmarkReleaseLock");
		((Control)(object)checkmarkReleaseLock).Name = "checkmarkReleaseLock";
		componentResourceManager.ApplyResources(runServiceButtonReleaseLock, "runServiceButtonReleaseLock");
		((Control)(object)runServiceButtonReleaseLock).Name = "runServiceButtonReleaseLock";
		runServiceButtonReleaseLock.ServiceCall = new ServiceCall("ETCM01T", "DJ_Release_transport_security_for_eTCM");
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTeachInState, "digitalReadoutInstrumentTeachInState");
		digitalReadoutInstrumentTeachInState.FontGroup = "";
		((SingleInstrumentBase)digitalReadoutInstrumentTeachInState).FreezeValue = false;
		digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentTeachInState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		digitalReadoutInstrumentTeachInState.Gradient.Initialize((ValueState)0, 6);
		digitalReadoutInstrumentTeachInState.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentTeachInState.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentTeachInState.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentTeachInState.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentTeachInState.Gradient.Modify(5, 4.0, (ValueState)0);
		digitalReadoutInstrumentTeachInState.Gradient.Modify(6, 255.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentTeachInState).Instrument = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Transmission_Teach_in_State_current_state");
		((Control)(object)digitalReadoutInstrumentTeachInState).Name = "digitalReadoutInstrumentTeachInState";
		((TableLayoutPanel)(object)tableLayoutPanel).SetRowSpan((Control)(object)digitalReadoutInstrumentTeachInState, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentTeachInState).UnitAlignment = StringAlignment.Near;
		dialInstrumentTiltSensorSignal.AngleRange = 120.0;
		dialInstrumentTiltSensorSignal.AngleStart = 120.0;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)dialInstrumentTiltSensorSignal, 2);
		componentResourceManager.ApplyResources(dialInstrumentTiltSensorSignal, "dialInstrumentTiltSensorSignal");
		dialInstrumentTiltSensorSignal.FontGroup = "TiltSensorAnalogs";
		((SingleInstrumentBase)dialInstrumentTiltSensorSignal).FreezeValue = false;
		((SingleInstrumentBase)dialInstrumentTiltSensorSignal).Instrument = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Inclination_Sensor_Signal_Value_current_value");
		((Control)(object)dialInstrumentTiltSensorSignal).Name = "dialInstrumentTiltSensorSignal";
		dialInstrumentTiltSensorSignal.Orientation = Orientation.Horizontal;
		((AxisSingleInstrumentBase)dialInstrumentTiltSensorSignal).PreferredAxisRange = new AxisRange(-60.0, 60.0, "%");
		((SingleInstrumentBase)dialInstrumentTiltSensorSignal).UnitAlignment = StringAlignment.Near;
		pictureBoxTruck.BackColor = Color.White;
		componentResourceManager.ApplyResources(pictureBoxTruck, "pictureBoxTruck");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)pictureBoxTruck, 2);
		pictureBoxTruck.Name = "pictureBoxTruck";
		pictureBoxTruck.TabStop = false;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentSensorFailure, "digitalReadoutInstrumentSensorFailure");
		digitalReadoutInstrumentSensorFailure.FontGroup = "TiltSensorText";
		((SingleInstrumentBase)digitalReadoutInstrumentSensorFailure).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentSensorFailure).Instrument = new Qualifier((QualifierTypes)32, "ETCM01T", "14F1EE");
		((Control)(object)digitalReadoutInstrumentSensorFailure).Name = "digitalReadoutInstrumentSensorFailure";
		((SingleInstrumentBase)digitalReadoutInstrumentSensorFailure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentNotCalibratedFault, "digitalReadoutInstrumentNotCalibratedFault");
		digitalReadoutInstrumentNotCalibratedFault.FontGroup = "TiltSensorText";
		((SingleInstrumentBase)digitalReadoutInstrumentNotCalibratedFault).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentNotCalibratedFault).Instrument = new Qualifier((QualifierTypes)32, "ETCM01T", "14F1ED");
		((Control)(object)digitalReadoutInstrumentNotCalibratedFault).Name = "digitalReadoutInstrumentNotCalibratedFault";
		((SingleInstrumentBase)digitalReadoutInstrumentNotCalibratedFault).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(runServiceButtonTiltSensor, "runServiceButtonTiltSensor");
		((Control)(object)runServiceButtonTiltSensor).Name = "runServiceButtonTiltSensor";
		runServiceButtonTiltSensor.ServiceCall = new ServiceCall("ETCM01T", "RT_Inclination_Sensor_Teach_in_Procedure_Start");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)seekTimeListView, 2);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Tilt Sensor";
		((TableLayoutPanel)(object)tableLayoutPanel).SetRowSpan((Control)(object)seekTimeListView, 4);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)panelCanStart, 3);
		panelCanStart.Controls.Add((Control)(object)labelTiltSensor);
		panelCanStart.Controls.Add((Control)(object)checkmarkTiltSensor);
		componentResourceManager.ApplyResources(panelCanStart, "panelCanStart");
		panelCanStart.Name = "panelCanStart";
		labelTiltSensor.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelTiltSensor, "labelTiltSensor");
		((Control)(object)labelTiltSensor).Name = "labelTiltSensor";
		labelTiltSensor.Orientation = (TextOrientation)1;
		labelTiltSensor.UseSystemColors = true;
		componentResourceManager.ApplyResources(checkmarkTiltSensor, "checkmarkTiltSensor");
		((Control)(object)checkmarkTiltSensor).Name = "checkmarkTiltSensor";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
		digitalReadoutInstrumentParkBrake.FontGroup = "";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).FreezeValue = false;
		digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState)2);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
		((Control)(object)digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransmission, "digitalReadoutInstrumentTransmission");
		digitalReadoutInstrumentTransmission.FontGroup = "TiltSensorText";
		((SingleInstrumentBase)digitalReadoutInstrumentTransmission).FreezeValue = false;
		digitalReadoutInstrumentTransmission.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrumentTransmission.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentTransmission.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentTransmission).Instrument = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
		((Control)(object)digitalReadoutInstrumentTransmission).Name = "digitalReadoutInstrumentTransmission";
		((SingleInstrumentBase)digitalReadoutInstrumentTransmission).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = "TiltSensorText";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Transmission_Tilt_Sensor_and_Unlock");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		panelReleaseLock.ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxTruck).EndInit();
		panelCanStart.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
