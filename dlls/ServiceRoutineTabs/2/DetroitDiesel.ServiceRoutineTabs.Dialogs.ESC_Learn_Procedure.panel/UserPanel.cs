using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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

	private bool VehicleIsMoving
	{
		get
		{
			if (((SingleInstrumentBase)dialInstrumentVehicleSpeed).DataItem != null)
			{
				return ((SingleInstrumentBase)dialInstrumentVehicleSpeed).DataItem.ValueAsDouble(((SingleInstrumentBase)dialInstrumentVehicleSpeed).DataItem.Value) > 0.0;
			}
			return false;
		}
	}

	private bool VehicleIsNotMoving
	{
		get
		{
			if (((SingleInstrumentBase)dialInstrumentVehicleSpeed).DataItem != null)
			{
				return ((SingleInstrumentBase)dialInstrumentVehicleSpeed).DataItem.ValueAsDouble(((SingleInstrumentBase)dialInstrumentVehicleSpeed).DataItem.Value) <= 0.0;
			}
			return false;
		}
	}

	private bool InstrumentsHaveLearnedStatus => (int)digitalReadoutInstrumentOffsetSteeringAngleLearned.RepresentedState == 1 && (int)digitalReadoutInstrumentOffsetLateralAccelerationLearned.RepresentedState == 1;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		InitUserInterface();
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		runServiceButtonESCLearningStart.ServiceComplete += runServiceButtonESCLearningStart_ServiceComplete;
		runServiceButtonESCLearningStop.ServiceComplete += runServiceButtonESCLearningStop_ServiceComplete;
		dialInstrumentVehicleSpeed.RepresentedStateChanged += StartCondition_RepresentedStateChanged;
		digitalReadoutInstrumentServiceModeActive.RepresentedStateChanged += ServiceModeActive_RepresentedStateChanged;
	}

	private void InitUserInterface()
	{
		Channel channel = ((CustomPanel)this).GetChannel("ABS02T", (ChannelLookupOptions)5);
		absEcuName = ((channel == null) ? "ABS02T" : channel.Ecu.Name);
		SetDigitalReadoutInstrument(digitalReadoutInstrumentServiceModeActive);
		SetDigitalReadoutInstrument(digitalReadoutInstrumentSteeringWheelAngle);
		SetDigitalReadoutInstrument(digitalReadoutInstrumentYawRate);
		SetDigitalReadoutInstrument(digitalReadoutInstrumentOffsetSteeringAngleLearned);
		SetDigitalReadoutInstrument(digitalReadoutInstrumentOffsetLateralAccelerationLearned);
		((Control)(object)digitalReadoutInstrumentLateralAccel).Visible = absEcuName == "ABS02T";
		SetRunServiceButton(runServiceButtonESCLearningStart);
		SetRunServiceButton(runServiceButtonESCLearningStop);
	}

	private void SetDigitalReadoutInstrument(DigitalReadoutInstrument digitalReadoutInstrument)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (digitalReadoutInstrument != null)
		{
			_ = ((SingleInstrumentBase)digitalReadoutInstrument).Instrument;
			if (true)
			{
				string text = absEcuName;
				Qualifier instrument = ((SingleInstrumentBase)digitalReadoutInstrument).Instrument;
				((SingleInstrumentBase)digitalReadoutInstrument).Instrument = new Qualifier(text, ((Qualifier)(ref instrument)).Name);
			}
		}
	}

	private void SetRunServiceButton(RunServiceButton runServiceButton)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (runServiceButton != null)
		{
			_ = runServiceButton.ServiceCall;
			ServiceCall serviceCall = runServiceButton.ServiceCall;
			if (((ServiceCall)(ref serviceCall)).Qualifier != null)
			{
				string text = absEcuName;
				serviceCall = runServiceButton.ServiceCall;
				string qualifier = ((ServiceCall)(ref serviceCall)).Qualifier;
				serviceCall = runServiceButton.ServiceCall;
				runServiceButton.ServiceCall = new ServiceCall(text, qualifier, (IEnumerable<string>)((ServiceCall)(ref serviceCall)).Inputs);
			}
		}
	}

	private void UpdateUserInterface()
	{
		engineSpeedCheck.Checked = (ProcedureIsRunning ? VehicleIsMoving : VehicleIsNotMoving);
		((Control)(object)runServiceButtonESCLearningStart).Enabled = !ProcedureIsRunning && VehicleIsNotMoving;
		((Control)(object)runServiceButtonESCLearningStop).Enabled = ProcedureIsRunning;
		UpdateProcedureatureStatusText();
		SetVehicleSpeedGradient();
	}

	private void AddLogLabel(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
	}

	private void UpdateProcedureatureStatusText()
	{
		if (ProcedureIsComplete)
		{
			engineStatusLabel.Text = Resources.Message_ESCLearnProcedureComplete;
			if (VehicleIsMoving)
			{
				System.Windows.Forms.Label label = engineStatusLabel;
				label.Text = label.Text + " " + Resources.Message_YouMayStopTheVehicle;
			}
		}
		else if (ProcedureIsRunning)
		{
			if (VehicleIsMoving)
			{
				engineStatusLabel.Text = Resources.Message_ProcedureRunning;
			}
			else
			{
				engineStatusLabel.Text = Resources.Message_StartDrivingTheTruck;
			}
		}
		else if (VehicleIsNotMoving)
		{
			engineStatusLabel.Text = Resources.Message_ProcedureCanStart;
		}
		else
		{
			engineStatusLabel.Text = Resources.Message_WheelBasedSpeedMustBeZero;
		}
	}

	private void UpdateInstrumentLearnedStatus()
	{
		if (InstrumentsHaveLearnedStatus && ProcedureIsRunning)
		{
			ProcedureIsComplete = true;
			ProcedureIsRunning = false;
		}
		UpdateUserInterface();
	}

	private void SetVehicleSpeedGradient()
	{
		if (ProcedureIsRunning)
		{
			((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient = Gradient.FromString("(Fault),(1,Default),(15,Ok)");
		}
		else
		{
			((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient = Gradient.FromString("(Ok),(1,Fault)");
		}
		((Control)(object)dialInstrumentVehicleSpeed).Refresh();
	}

	private void StartCondition_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void ServiceModeActive_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if ((((RunSharedProcedureButtonBase)runServiceButtonESCLearningStart).InProgress || ((RunSharedProcedureButtonBase)runServiceButtonESCLearningStop).InProgress) && e.CloseReason == CloseReason.UserClosing)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			runServiceButtonESCLearningStart.ServiceComplete -= runServiceButtonESCLearningStart_ServiceComplete;
			runServiceButtonESCLearningStop.ServiceComplete -= runServiceButtonESCLearningStop_ServiceComplete;
			dialInstrumentVehicleSpeed.RepresentedStateChanged -= StartCondition_RepresentedStateChanged;
			digitalReadoutInstrumentOffsetLateralAccelerationLearned.RepresentedStateChanged -= digitalReadoutInstrumentOffsetLateralAccelerationAngleLearned_RepresentedStateChanged;
			dialInstrumentVehicleSpeed.RepresentedStateChanged -= dialInstrumentVehicleSpeed_RepresentedStateChanged;
			digitalReadoutInstrumentOffsetSteeringAngleLearned.RepresentedStateChanged -= digitalReadoutInstrumentOffsetSteeringAngleLearned_RepresentedStateChanged;
		}
	}

	private void digitalReadoutInstrumentOffsetSteeringAngleLearned_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateInstrumentLearnedStatus();
	}

	private void digitalReadoutInstrumentOffsetLateralAccelerationAngleLearned_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateInstrumentLearnedStatus();
	}

	private void dialInstrumentVehicleSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void runServiceButtonESCLearningStop_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel(Resources.Message_ProcedureStopped);
			ProcedureIsRunning = false;
		}
		else
		{
			AddLogLabel(string.Format(Resources.Message_ErrorStoppingProcedure, ((ResultEventArgs)(object)e).Exception.Message));
		}
		UpdateUserInterface();
	}

	private void runServiceButtonESCLearningStart_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel(Resources.Message_ProcedureStarted);
			ProcedureIsRunning = true;
			ProcedureIsComplete = false;
		}
		else
		{
			AddLogLabel(string.Format(Resources.Message_ErrorStartingProcedure, ((ResultEventArgs)(object)e).Exception.Message));
		}
		UpdateUserInterface();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
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
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
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
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_082c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b31: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		labelMessage = new System.Windows.Forms.Label();
		digitalReadoutInstrumentYawRate = new DigitalReadoutInstrument();
		digitalReadoutInstrumentOffsetLateralAccelerationLearned = new DigitalReadoutInstrument();
		digitalReadoutInstrumentOffsetSteeringAngleLearned = new DigitalReadoutInstrument();
		digitalReadoutInstrumentLateralAccel = new DigitalReadoutInstrument();
		digitalReadoutInstrumentServiceModeActive = new DigitalReadoutInstrument();
		dialInstrumentVehicleSpeed = new DialInstrument();
		seekTimeListView = new SeekTimeListView();
		digitalReadoutInstrumentSteeringWheelAngle = new DigitalReadoutInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		runServiceButtonESCLearningStart = new RunServiceButton();
		runServiceButtonESCLearningStop = new RunServiceButton();
		buttonClose = new Button();
		scalingLabel1 = new ScalingLabel();
		tableLayoutPanel3 = new TableLayoutPanel();
		engineSpeedCheck = new Checkmark();
		engineStatusLabel = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelMessage, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentYawRate, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentOffsetLateralAccelerationLearned, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentOffsetSteeringAngleLearned, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentLateralAccel, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentServiceModeActive, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)dialInstrumentVehicleSpeed, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentSteeringWheelAngle, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 0, 7);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(labelMessage, "labelMessage");
		labelMessage.BackColor = Color.Transparent;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelMessage, 2);
		labelMessage.Name = "labelMessage";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentYawRate, "digitalReadoutInstrumentYawRate");
		digitalReadoutInstrumentYawRate.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentYawRate).FreezeValue = false;
		digitalReadoutInstrumentYawRate.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentYawRate.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentYawRate.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentYawRate.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentYawRate.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentYawRate).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Yaw_rate_plausibility");
		((Control)(object)digitalReadoutInstrumentYawRate).Name = "digitalReadoutInstrumentYawRate";
		((SingleInstrumentBase)digitalReadoutInstrumentYawRate).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentOffsetLateralAccelerationLearned, "digitalReadoutInstrumentOffsetLateralAccelerationLearned");
		digitalReadoutInstrumentOffsetLateralAccelerationLearned.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentOffsetLateralAccelerationLearned).FreezeValue = false;
		digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentOffsetLateralAccelerationLearned.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentOffsetLateralAccelerationLearned).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Offset_lateral_acceleration_learned");
		((Control)(object)digitalReadoutInstrumentOffsetLateralAccelerationLearned).Name = "digitalReadoutInstrumentOffsetLateralAccelerationLearned";
		((SingleInstrumentBase)digitalReadoutInstrumentOffsetLateralAccelerationLearned).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentOffsetLateralAccelerationLearned.RepresentedStateChanged += digitalReadoutInstrumentOffsetLateralAccelerationAngleLearned_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentOffsetSteeringAngleLearned, "digitalReadoutInstrumentOffsetSteeringAngleLearned");
		digitalReadoutInstrumentOffsetSteeringAngleLearned.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentOffsetSteeringAngleLearned).FreezeValue = false;
		digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentOffsetSteeringAngleLearned.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentOffsetSteeringAngleLearned).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_1_Read_Offset_steering_wheel_angle_learned");
		((Control)(object)digitalReadoutInstrumentOffsetSteeringAngleLearned).Name = "digitalReadoutInstrumentOffsetSteeringAngleLearned";
		((SingleInstrumentBase)digitalReadoutInstrumentOffsetSteeringAngleLearned).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentOffsetSteeringAngleLearned.RepresentedStateChanged += digitalReadoutInstrumentOffsetSteeringAngleLearned_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentLateralAccel, "digitalReadoutInstrumentLateralAccel");
		digitalReadoutInstrumentLateralAccel.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentLateralAccel).FreezeValue = false;
		digitalReadoutInstrumentLateralAccel.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentLateralAccel.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentLateralAccel.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentLateralAccel.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentLateralAccel.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentLateralAccel).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_1_Read_Lateral_acceleration_plausibility");
		((Control)(object)digitalReadoutInstrumentLateralAccel).Name = "digitalReadoutInstrumentLateralAccel";
		((SingleInstrumentBase)digitalReadoutInstrumentLateralAccel).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentServiceModeActive, "digitalReadoutInstrumentServiceModeActive");
		digitalReadoutInstrumentServiceModeActive.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentServiceModeActive).FreezeValue = false;
		digitalReadoutInstrumentServiceModeActive.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentServiceModeActive.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentServiceModeActive.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentServiceModeActive.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentServiceModeActive.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentServiceModeActive).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Service_mode_active");
		((Control)(object)digitalReadoutInstrumentServiceModeActive).Name = "digitalReadoutInstrumentServiceModeActive";
		((SingleInstrumentBase)digitalReadoutInstrumentServiceModeActive).UnitAlignment = StringAlignment.Near;
		dialInstrumentVehicleSpeed.AngleRange = 135.0;
		dialInstrumentVehicleSpeed.AngleStart = -180.0;
		componentResourceManager.ApplyResources(dialInstrumentVehicleSpeed, "dialInstrumentVehicleSpeed");
		dialInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).FreezeValue = false;
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Initialize((ValueState)3, 2, "km/h");
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(1, 1.0, (ValueState)0);
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(2, 15.0, (ValueState)1);
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)dialInstrumentVehicleSpeed).Name = "dialInstrumentVehicleSpeed";
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).PreferredAxisRange = new AxisRange(0.0, 100.0, "km/h");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)dialInstrumentVehicleSpeed, 4);
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		dialInstrumentVehicleSpeed.RepresentedStateChanged += dialInstrumentVehicleSpeed_RepresentedStateChanged;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView, 2);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "ESC Leaning Procedure";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.f";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentSteeringWheelAngle, "digitalReadoutInstrumentSteeringWheelAngle");
		digitalReadoutInstrumentSteeringWheelAngle.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentSteeringWheelAngle).FreezeValue = false;
		digitalReadoutInstrumentSteeringWheelAngle.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentSteeringWheelAngle.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentSteeringWheelAngle.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentSteeringWheelAngle.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentSteeringWheelAngle.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentSteeringWheelAngle).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_ESC_End_of_line_status_2_Read_Steering_wheel_angle_plausibility");
		((Control)(object)digitalReadoutInstrumentSteeringWheelAngle).Name = "digitalReadoutInstrumentSteeringWheelAngle";
		((SingleInstrumentBase)digitalReadoutInstrumentSteeringWheelAngle).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)runServiceButtonESCLearningStart, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)runServiceButtonESCLearningStop, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClose, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)scalingLabel1, 2, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(runServiceButtonESCLearningStart, "runServiceButtonESCLearningStart");
		((Control)(object)runServiceButtonESCLearningStart).Name = "runServiceButtonESCLearningStart";
		runServiceButtonESCLearningStart.ServiceCall = new ServiceCall("ABS02T", "RT_Start_ESC_learning_Start_Routine_Start", (IEnumerable<string>)new string[1] { "Learning_mode=2" });
		componentResourceManager.ApplyResources(runServiceButtonESCLearningStop, "runServiceButtonESCLearningStop");
		((Control)(object)runServiceButtonESCLearningStop).Name = "runServiceButtonESCLearningStop";
		runServiceButtonESCLearningStop.ServiceCall = new ServiceCall("ABS02T", "RT_Start_ESC_learning_Start_Routine_Start", (IEnumerable<string>)new string[1] { "Learning_mode=3" });
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		scalingLabel1.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(scalingLabel1, "scalingLabel1");
		scalingLabel1.FontGroup = null;
		scalingLabel1.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel1).Name = "scalingLabel1";
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)engineSpeedCheck, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(engineStatusLabel, 1, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(engineSpeedCheck, "engineSpeedCheck");
		((Control)(object)engineSpeedCheck).Name = "engineSpeedCheck";
		componentResourceManager.ApplyResources(engineStatusLabel, "engineStatusLabel");
		engineStatusLabel.Name = "engineStatusLabel";
		engineStatusLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
