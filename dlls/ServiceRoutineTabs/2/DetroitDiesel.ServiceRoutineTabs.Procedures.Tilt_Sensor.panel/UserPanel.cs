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

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor.panel;

public class UserPanel : CustomPanel
{
	private Channel Tcm = null;

	private InstrumentDataItem tiltSignalItem;

	private Image initialTruckImage;

	private TableLayoutPanel tableLayoutPanel;

	private DigitalReadoutInstrument digitalReadoutInstrumentTiltSignalError;

	private PictureBox pictureBoxTruck;

	private RunServiceButton runServiceButtonTiltSensor;

	private SeekTimeListView seekTimeListView;

	private Panel panelCanStart;

	private Label labelCanStart;

	private Checkmark checkmarkCanStart;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleCheck;

	private DialInstrument dialInstrumentTiltSensorSignal;

	private DigitalReadoutInstrument digitalReadoutInstrumentNotLearnedFault;

	private bool CanStart => LearnServiceAvailable && VehicleCheckOk;

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

	private bool VehicleCheckOk => (int)digitalReadoutInstrumentVehicleCheck.RepresentedState != 3;

	private bool LearnNeeded => (int)digitalReadoutInstrumentNotLearnedFault.RepresentedState == 3;

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
		runServiceButtonTiltSensor.ServiceComplete += runServiceButton_ServiceComplete;
		digitalReadoutInstrumentVehicleCheck.RepresentedStateChanged += precondition_RepresentedStateChanged;
		digitalReadoutInstrumentNotLearnedFault.RepresentedStateChanged += precondition_RepresentedStateChanged;
	}

	public override void OnChannelsChanged()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		SetTcm(((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)7));
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

	private void SetTcm(Channel tcm)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (Tcm == tcm)
		{
			return;
		}
		Tcm = tcm;
		if (Tcm == null)
		{
			return;
		}
		foreach (SingleInstrumentBase item in CustomPanel.GetControlsOfType(((Control)this).Controls, typeof(SingleInstrumentBase)))
		{
			SingleInstrumentBase val = item;
			Qualifier instrument = val.Instrument;
			Ecu ecuByName = SapiManager.GetEcuByName(((Qualifier)(ref instrument)).Ecu);
			if (ecuByName != null && ecuByName.Identifier == Tcm.Ecu.Identifier && ecuByName.Name != Tcm.Ecu.Name)
			{
				instrument = val.Instrument;
				QualifierTypes type = ((Qualifier)(ref instrument)).Type;
				string name = Tcm.Ecu.Name;
				instrument = val.Instrument;
				val.Instrument = new Qualifier(type, name, ((Qualifier)(ref instrument)).Name);
			}
		}
		runServiceButtonTiltSensor.ServiceCall = new ServiceCall(Tcm.Ecu.Name, "RT_0430_Nullpunktabgleich_Neigungssensor_Start");
	}

	private void UpdateUserInterface()
	{
		((Control)(object)runServiceButtonTiltSensor).Enabled = CanStart;
		checkmarkCanStart.CheckState = (CanStart ? CheckState.Checked : CheckState.Unchecked);
		string text = Resources.Message_CannotCalibrateTheTiltSensor;
		if (CanStart)
		{
			text = ((!LearnNeeded) ? Resources.Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton : Resources.Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton);
		}
		else if (!LearnServiceAvailable)
		{
			text = Resources.Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice;
		}
		else if (!VehicleCheckOk)
		{
			text = Resources.Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied;
		}
		((Control)(object)labelCanStart).Text = text;
	}

	private void precondition_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void runServiceButton_ServiceComplete(object sender, SingleServiceResultEventArgs e)
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
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
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
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		digitalReadoutInstrumentVehicleCheck = new DigitalReadoutInstrument();
		dialInstrumentTiltSensorSignal = new DialInstrument();
		pictureBoxTruck = new PictureBox();
		digitalReadoutInstrumentTiltSignalError = new DigitalReadoutInstrument();
		digitalReadoutInstrumentNotLearnedFault = new DigitalReadoutInstrument();
		runServiceButtonTiltSensor = new RunServiceButton();
		seekTimeListView = new SeekTimeListView();
		panelCanStart = new Panel();
		labelCanStart = new Label();
		checkmarkCanStart = new Checkmark();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((ISupportInitialize)pictureBoxTruck).BeginInit();
		panelCanStart.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleCheck, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)dialInstrumentTiltSensorSignal, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(pictureBoxTruck, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentTiltSignalError, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentNotLearnedFault, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)runServiceButtonTiltSensor, 4, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)seekTimeListView, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(panelCanStart, 0, 4);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)digitalReadoutInstrumentVehicleCheck, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleCheck, "digitalReadoutInstrumentVehicleCheck");
		digitalReadoutInstrumentVehicleCheck.FontGroup = "TiltSensorText";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheck).FreezeValue = false;
		digitalReadoutInstrumentVehicleCheck.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentVehicleCheck.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentVehicleCheck.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentVehicleCheck.Gradient.Modify(3, 2.0, (ValueState)2);
		digitalReadoutInstrumentVehicleCheck.Gradient.Modify(4, 3.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheck).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutInstrumentVehicleCheck).Name = "digitalReadoutInstrumentVehicleCheck";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheck).UnitAlignment = StringAlignment.Near;
		dialInstrumentTiltSensorSignal.AngleRange = 120.0;
		dialInstrumentTiltSensorSignal.AngleStart = 120.0;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)dialInstrumentTiltSensorSignal, 2);
		componentResourceManager.ApplyResources(dialInstrumentTiltSensorSignal, "dialInstrumentTiltSensorSignal");
		dialInstrumentTiltSensorSignal.FontGroup = "TiltSensorAnalogs";
		((SingleInstrumentBase)dialInstrumentTiltSensorSignal).FreezeValue = false;
		((SingleInstrumentBase)dialInstrumentTiltSensorSignal).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd18_Signal_Neigungssensor_Signal_Neigungssensor");
		((Control)(object)dialInstrumentTiltSensorSignal).Name = "dialInstrumentTiltSensorSignal";
		dialInstrumentTiltSensorSignal.Orientation = Orientation.Horizontal;
		((AxisSingleInstrumentBase)dialInstrumentTiltSensorSignal).PreferredAxisRange = new AxisRange(-60.0, 60.0, "%");
		((SingleInstrumentBase)dialInstrumentTiltSensorSignal).UnitAlignment = StringAlignment.Near;
		pictureBoxTruck.BackColor = Color.Black;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)pictureBoxTruck, 3);
		componentResourceManager.ApplyResources(pictureBoxTruck, "pictureBoxTruck");
		pictureBoxTruck.Name = "pictureBoxTruck";
		pictureBoxTruck.TabStop = false;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)digitalReadoutInstrumentTiltSignalError, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTiltSignalError, "digitalReadoutInstrumentTiltSignalError");
		digitalReadoutInstrumentTiltSignalError.FontGroup = "TiltSensorText";
		((SingleInstrumentBase)digitalReadoutInstrumentTiltSignalError).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentTiltSignalError).Instrument = new Qualifier((QualifierTypes)32, "TCM01T", "26F1EE");
		((Control)(object)digitalReadoutInstrumentTiltSignalError).Name = "digitalReadoutInstrumentTiltSignalError";
		((SingleInstrumentBase)digitalReadoutInstrumentTiltSignalError).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)digitalReadoutInstrumentNotLearnedFault, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentNotLearnedFault, "digitalReadoutInstrumentNotLearnedFault");
		digitalReadoutInstrumentNotLearnedFault.FontGroup = "TiltSensorText";
		((SingleInstrumentBase)digitalReadoutInstrumentNotLearnedFault).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentNotLearnedFault).Instrument = new Qualifier((QualifierTypes)32, "TCM01T", "26F1ED");
		((Control)(object)digitalReadoutInstrumentNotLearnedFault).Name = "digitalReadoutInstrumentNotLearnedFault";
		((SingleInstrumentBase)digitalReadoutInstrumentNotLearnedFault).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(runServiceButtonTiltSensor, "runServiceButtonTiltSensor");
		((Control)(object)runServiceButtonTiltSensor).Name = "runServiceButtonTiltSensor";
		runServiceButtonTiltSensor.ServiceCall = new ServiceCall("TCM01T", "RT_0430_Nullpunktabgleich_Neigungssensor_Start");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)seekTimeListView, 3);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Tilt Sensor";
		((TableLayoutPanel)(object)tableLayoutPanel).SetRowSpan((Control)(object)seekTimeListView, 3);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)panelCanStart, 4);
		panelCanStart.Controls.Add((Control)(object)labelCanStart);
		panelCanStart.Controls.Add((Control)(object)checkmarkCanStart);
		componentResourceManager.ApplyResources(panelCanStart, "panelCanStart");
		panelCanStart.Name = "panelCanStart";
		labelCanStart.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelCanStart, "labelCanStart");
		((Control)(object)labelCanStart).Name = "labelCanStart";
		labelCanStart.Orientation = (TextOrientation)1;
		labelCanStart.UseSystemColors = true;
		componentResourceManager.ApplyResources(checkmarkCanStart, "checkmarkCanStart");
		((Control)(object)checkmarkCanStart).Name = "checkmarkCanStart";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Tilt_Sensor");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxTruck).EndInit();
		panelCanStart.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
