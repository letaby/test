using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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
			if (!double.IsNaN(decimalTextBoxTime.Value))
			{
				return Convert.ToInt32(decimalTextBoxTime.Value, CultureInfo.InvariantCulture);
			}
			return 1;
		}
		set
		{
			decimalTextBoxTime.Value = value;
		}
	}

	private int FanSpeed
	{
		get
		{
			if (!double.IsNaN(decimalTextBoxSpeed.Value))
			{
				return Convert.ToInt32(decimalTextBoxSpeed.Value, CultureInfo.InvariantCulture);
			}
			return 1;
		}
		set
		{
			decimalTextBoxSpeed.Value = value;
		}
	}

	private bool Online => mcm != null && mcm.Online;

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += RepresentedStateChanged;
		digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged += RepresentedStateChanged;
		digitalReadoutInstrumentFanType.RepresentedStateChanged += RepresentedStateChanged;
		InitTrackBars();
		UpdateUserInterface();
	}

	private void InitTrackBars()
	{
		trackBarSpeed.Minimum = (int)decimalTextBoxSpeed.MinimumValue;
		trackBarSpeed.Maximum = (int)decimalTextBoxSpeed.MaximumValue;
		trackBarSpeed.LargeChange = 10;
		trackBarSpeed.SmallChange = 1;
		trackBarSpeed.TickFrequency = trackBarSpeed.LargeChange;
		trackBarTime.Minimum = (int)decimalTextBoxTime.MinimumValue;
		trackBarTime.Maximum = (int)decimalTextBoxTime.MaximumValue;
		trackBarTime.LargeChange = (((int)decimalTextBoxTime.MaximumValue / 10 <= 0) ? 1 : ((int)decimalTextBoxTime.MaximumValue / 10));
		trackBarTime.SmallChange = ((trackBarTime.LargeChange / 10 <= 0) ? 1 : (trackBarTime.LargeChange / 10));
		trackBarTime.TickFrequency = trackBarTime.LargeChange;
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM21T"));
		UpdateUserInterface();
	}

	private bool SetMCM(Channel mcm)
	{
		bool result = false;
		if (this.mcm != mcm)
		{
			result = true;
			if (this.mcm != null)
			{
				StopFan();
				this.mcm.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				this.mcm.Parameters.ParametersReadCompleteEvent -= ParametersReadCompleteEvent;
				StartService = null;
				StopService = null;
			}
			this.mcm = mcm;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				this.mcm.Parameters.ParametersReadCompleteEvent += ParametersReadCompleteEvent;
				StartService = this.mcm.Services["RT_SR003_PWM_Routine_by_Function_Start_Control_Value"];
				StopService = this.mcm.Services["RT_SR003_PWM_Routine_by_Function_Stop_Function_Name"];
				Parameter parameter = this.mcm.Parameters["Fan_Type"];
				if (parameter != null)
				{
					this.mcm.Parameters.ReadGroup(parameter.GroupQualifier, fromCache: false, synchronous: false);
				}
			}
		}
		return result;
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		if (!e.Succeeded)
		{
			OutputMessage(string.Format(Resources.MessageFormat_ErrorReadingTheParametersTheFanTypeMayBeIncorrectError0, e.Exception.Message.ToString()));
		}
		UpdateUserInterface();
	}

	private void UpdateFanControls(bool enable)
	{
		trackBarSpeed.Enabled = enable;
		trackBarTime.Enabled = enable;
		((Control)(object)decimalTextBoxSpeed).Enabled = enable;
		((Control)(object)decimalTextBoxTime).Enabled = enable;
	}

	private void UpdateUserInterface()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Invalid comparison between Unknown and I4
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Invalid comparison between Unknown and I4
		bool enable = false;
		checkmarkVehicleStatus.Checked = false;
		if (!Online)
		{
			((Control)(object)scalingLabelVehicleStatus).Text = Resources.Message_TheFanCannotBeRunBecauseTheMCMIsOffline;
			buttonStop.Enabled = false;
			buttonStart.Enabled = false;
		}
		else if ((int)digitalReadoutInstrumentFanType.RepresentedState != 1)
		{
			((Control)(object)scalingLabelVehicleStatus).Text = Resources.Message_TheFanIsNotAVariableSpeedType;
			buttonStop.Enabled = false;
			buttonStart.Enabled = false;
		}
		else if ((int)digitalReadoutInstrumentVehicleStatus.RepresentedState != 1)
		{
			((Control)(object)scalingLabelVehicleStatus).Text = Resources.Message_TheFanCannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL;
			buttonStop.Enabled = true;
			buttonStart.Enabled = false;
		}
		else if ((int)digitalReadoutInstrumentEngineSpeed.RepresentedState != 1)
		{
			((Control)(object)scalingLabelVehicleStatus).Text = Resources.Message_TheFanCannotStartUntilTheEngineIsRunning;
			buttonStop.Enabled = true;
			buttonStart.Enabled = false;
		}
		else
		{
			((Control)(object)scalingLabelVehicleStatus).Text = Resources.Message_TheFanCanBeStarted;
			buttonStart.Enabled = true;
			buttonStop.Enabled = true;
			enable = true;
			checkmarkVehicleStatus.Checked = true;
		}
		UpdateFanControls(enable);
	}

	private void RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void trackBarSpeed_Scroll(object sender, EventArgs e)
	{
		if (!synchronizingFanSpeed)
		{
			FanSpeed = trackBarSpeed.Value;
		}
	}

	private void trackBarTime_Scroll(object sender, EventArgs e)
	{
		if (!synchronizingFanTime)
		{
			FanTime = trackBarTime.Value;
		}
	}

	private void decimalTextBoxSpeed_ValueChanged(object sender, EventArgs e)
	{
		synchronizingFanSpeed = true;
		trackBarSpeed.Value = FanSpeed;
		synchronizingFanSpeed = false;
	}

	private void decimalTextBoxTime_ValueChanged(object sender, EventArgs e)
	{
		synchronizingFanTime = true;
		trackBarTime.Value = FanTime;
		synchronizingFanTime = false;
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		OutputMessage(Resources.Message_StartingFan);
		StartFan();
	}

	private void buttonStop_Click(object sender, EventArgs e)
	{
		OutputMessage(Resources.Message_StoppingFan);
		StopFan();
	}

	private void StartFan()
	{
		if (Online && StartService != null)
		{
			StartService.InputValues[0].Value = StartService.InputValues[0].Choices.GetItemFromRawValue(5);
			StartService.InputValues[1].Value = 100 - FanSpeed;
			StartService.InputValues[2].Value = FanTime * 1000;
			StartService.ServiceCompleteEvent += StartServiceCompleteEvent;
			StartService.Execute(synchronous: false);
		}
	}

	private void StopFan()
	{
		if (Online && StopService != null)
		{
			StopService.InputValues[0].Value = StopService.InputValues[0].Choices.GetItemFromRawValue(5);
			StopService.ServiceCompleteEvent += StopServiceCompleteEvent;
			StopService.Execute(synchronous: false);
		}
	}

	private void StartServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		(sender as Service).ServiceCompleteEvent -= StartServiceCompleteEvent;
		if (e.Succeeded)
		{
			OutputMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TheFanIsProgrammedToRunFor0Seconds, FanTime));
		}
		else
		{
			OutputMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_AnErrorOccurredStartingTheFan0, e.Exception.Message));
		}
		UpdateUserInterface();
	}

	private void StopServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		(sender as Service).ServiceCompleteEvent -= StopServiceCompleteEvent;
		if (e.Succeeded)
		{
			OutputMessage(Resources.Message_TheFanHasBeenStopped);
		}
		else
		{
			OutputMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_AnErrorOccurredStoppingTheFan, e.Exception.Message));
		}
		UpdateUserInterface();
	}

	private void OutputMessage(string message)
	{
		SapiExtensions.LabelLogWithPrefix(Sapi.GetSapi().LogFiles, seekTimeListView.RequiredUserLabelPrefix, message);
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
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
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_095f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0efe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3a: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelFanSpeedControl = new TableLayoutPanel();
		trackBarSpeed = new TrackBar();
		label1 = new System.Windows.Forms.Label();
		label3 = new System.Windows.Forms.Label();
		decimalTextBoxSpeed = new DecimalTextBox();
		tableLayoutPanelCheckMarkAndLabel = new TableLayoutPanel();
		checkmarkVehicleStatus = new Checkmark();
		scalingLabelVehicleStatus = new ScalingLabel();
		tableLayoutPanelFanTimeControl = new TableLayoutPanel();
		trackBarTime = new TrackBar();
		label5 = new System.Windows.Forms.Label();
		label6 = new System.Windows.Forms.Label();
		decimalTextBoxTime = new DecimalTextBox();
		tableLayoutPanelRunFanControls = new TableLayoutPanel();
		digitalReadoutInstrumentFanType = new DigitalReadoutInstrument();
		seekTimeListView = new SeekTimeListView();
		buttonStop = new Button();
		buttonStart = new Button();
		digitalReadoutInstrumentVehicleStatus = new DigitalReadoutInstrument();
		tableLayoutPanelPanel = new TableLayoutPanel();
		digitalReadoutInstrumentFanSpeed = new DigitalReadoutInstrument();
		barInstrumentCoolantTemp = new BarInstrument();
		barInstrumentCoolantOutTemp = new BarInstrument();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanelFanSpeedControl).SuspendLayout();
		((ISupportInitialize)trackBarSpeed).BeginInit();
		((Control)(object)tableLayoutPanelCheckMarkAndLabel).SuspendLayout();
		((Control)(object)tableLayoutPanelFanTimeControl).SuspendLayout();
		((ISupportInitialize)trackBarTime).BeginInit();
		((Control)(object)tableLayoutPanelRunFanControls).SuspendLayout();
		((Control)(object)tableLayoutPanelPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelFanSpeedControl, "tableLayoutPanelFanSpeedControl");
		((TableLayoutPanel)(object)tableLayoutPanelFanSpeedControl).Controls.Add(trackBarSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFanSpeedControl).Controls.Add(label1, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelFanSpeedControl).Controls.Add(label3, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelFanSpeedControl).Controls.Add((Control)(object)decimalTextBoxSpeed, 1, 1);
		((Control)(object)tableLayoutPanelFanSpeedControl).Name = "tableLayoutPanelFanSpeedControl";
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).SetRowSpan((Control)(object)tableLayoutPanelFanSpeedControl, 2);
		((TableLayoutPanel)(object)tableLayoutPanelFanSpeedControl).SetColumnSpan((Control)trackBarSpeed, 3);
		componentResourceManager.ApplyResources(trackBarSpeed, "trackBarSpeed");
		trackBarSpeed.LargeChange = 10;
		trackBarSpeed.Maximum = 95;
		trackBarSpeed.Minimum = 1;
		trackBarSpeed.Name = "trackBarSpeed";
		trackBarSpeed.TickFrequency = 20;
		trackBarSpeed.Value = 95;
		trackBarSpeed.Scroll += trackBarSpeed_Scroll;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label3, "label3");
		label3.Name = "label3";
		label3.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(decimalTextBoxSpeed, "decimalTextBoxSpeed");
		decimalTextBoxSpeed.MaximumValue = 95.0;
		decimalTextBoxSpeed.MinimumValue = 1.0;
		((Control)(object)decimalTextBoxSpeed).Name = "decimalTextBoxSpeed";
		decimalTextBoxSpeed.Precision = 0;
		decimalTextBoxSpeed.Value = 95.0;
		decimalTextBoxSpeed.ValueChanged += decimalTextBoxSpeed_ValueChanged;
		componentResourceManager.ApplyResources(tableLayoutPanelCheckMarkAndLabel, "tableLayoutPanelCheckMarkAndLabel");
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).SetColumnSpan((Control)(object)tableLayoutPanelCheckMarkAndLabel, 2);
		((TableLayoutPanel)(object)tableLayoutPanelCheckMarkAndLabel).Controls.Add((Control)(object)checkmarkVehicleStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelCheckMarkAndLabel).Controls.Add((Control)(object)scalingLabelVehicleStatus, 1, 0);
		((Control)(object)tableLayoutPanelCheckMarkAndLabel).Name = "tableLayoutPanelCheckMarkAndLabel";
		componentResourceManager.ApplyResources(checkmarkVehicleStatus, "checkmarkVehicleStatus");
		((Control)(object)checkmarkVehicleStatus).Name = "checkmarkVehicleStatus";
		scalingLabelVehicleStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(scalingLabelVehicleStatus, "scalingLabelVehicleStatus");
		scalingLabelVehicleStatus.FontGroup = null;
		scalingLabelVehicleStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelVehicleStatus).Name = "scalingLabelVehicleStatus";
		componentResourceManager.ApplyResources(tableLayoutPanelFanTimeControl, "tableLayoutPanelFanTimeControl");
		((TableLayoutPanel)(object)tableLayoutPanelFanTimeControl).Controls.Add(trackBarTime, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFanTimeControl).Controls.Add(label5, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelFanTimeControl).Controls.Add(label6, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelFanTimeControl).Controls.Add((Control)(object)decimalTextBoxTime, 1, 1);
		((Control)(object)tableLayoutPanelFanTimeControl).Name = "tableLayoutPanelFanTimeControl";
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).SetRowSpan((Control)(object)tableLayoutPanelFanTimeControl, 2);
		((TableLayoutPanel)(object)tableLayoutPanelFanTimeControl).SetColumnSpan((Control)trackBarTime, 3);
		componentResourceManager.ApplyResources(trackBarTime, "trackBarTime");
		trackBarTime.LargeChange = 30;
		trackBarTime.Maximum = 600;
		trackBarTime.Minimum = 1;
		trackBarTime.Name = "trackBarTime";
		trackBarTime.TickFrequency = 20;
		trackBarTime.Value = 30;
		trackBarTime.Scroll += trackBarTime_Scroll;
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		label5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label6, "label6");
		label6.Name = "label6";
		label6.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(decimalTextBoxTime, "decimalTextBoxTime");
		decimalTextBoxTime.MaximumValue = 600.0;
		decimalTextBoxTime.MinimumValue = 1.0;
		((Control)(object)decimalTextBoxTime).Name = "decimalTextBoxTime";
		decimalTextBoxTime.Precision = 0;
		decimalTextBoxTime.Value = 30.0;
		decimalTextBoxTime.ValueChanged += decimalTextBoxTime_ValueChanged;
		componentResourceManager.ApplyResources(tableLayoutPanelRunFanControls, "tableLayoutPanelRunFanControls");
		((TableLayoutPanel)(object)tableLayoutPanelPanel).SetColumnSpan((Control)(object)tableLayoutPanelRunFanControls, 2);
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).Controls.Add((Control)(object)digitalReadoutInstrumentFanType, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).Controls.Add((Control)(object)tableLayoutPanelFanTimeControl, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).Controls.Add((Control)(object)tableLayoutPanelCheckMarkAndLabel, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).Controls.Add((Control)(object)tableLayoutPanelFanSpeedControl, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).Controls.Add((Control)(object)seekTimeListView, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).Controls.Add(buttonStop, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).Controls.Add(buttonStart, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleStatus, 1, 3);
		((Control)(object)tableLayoutPanelRunFanControls).Name = "tableLayoutPanelRunFanControls";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFanType, "digitalReadoutInstrumentFanType");
		digitalReadoutInstrumentFanType.FontGroup = "VSP Half Digital Inst";
		((SingleInstrumentBase)digitalReadoutInstrumentFanType).FreezeValue = false;
		digitalReadoutInstrumentFanType.Gradient.Initialize((ValueState)3, 14);
		digitalReadoutInstrumentFanType.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentFanType.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentFanType.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentFanType.Gradient.Modify(4, 3.0, (ValueState)1);
		digitalReadoutInstrumentFanType.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutInstrumentFanType.Gradient.Modify(6, 5.0, (ValueState)1);
		digitalReadoutInstrumentFanType.Gradient.Modify(7, 6.0, (ValueState)3);
		digitalReadoutInstrumentFanType.Gradient.Modify(8, 7.0, (ValueState)3);
		digitalReadoutInstrumentFanType.Gradient.Modify(9, 8.0, (ValueState)1);
		digitalReadoutInstrumentFanType.Gradient.Modify(10, 9.0, (ValueState)1);
		digitalReadoutInstrumentFanType.Gradient.Modify(11, 10.0, (ValueState)1);
		digitalReadoutInstrumentFanType.Gradient.Modify(12, 11.0, (ValueState)1);
		digitalReadoutInstrumentFanType.Gradient.Modify(13, 12.0, (ValueState)1);
		digitalReadoutInstrumentFanType.Gradient.Modify(14, 13.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentFanType).Instrument = new Qualifier((QualifierTypes)4, "MCM21T", "Fan_Type");
		((Control)(object)digitalReadoutInstrumentFanType).Name = "digitalReadoutInstrumentFanType";
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).SetRowSpan((Control)(object)digitalReadoutInstrumentFanType, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentFanType).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).SetColumnSpan((Control)(object)seekTimeListView, 2);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "VariableSpeedFan";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.f";
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		buttonStop.Click += buttonStop_Click;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		buttonStart.Click += buttonStart_Click;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleStatus, "digitalReadoutInstrumentVehicleStatus");
		digitalReadoutInstrumentVehicleStatus.FontGroup = "VSP Half Digital Inst";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleStatus).FreezeValue = false;
		digitalReadoutInstrumentVehicleStatus.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentVehicleStatus.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentVehicleStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentVehicleStatus.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentVehicleStatus.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutInstrumentVehicleStatus).Name = "digitalReadoutInstrumentVehicleStatus";
		((TableLayoutPanel)(object)tableLayoutPanelRunFanControls).SetRowSpan((Control)(object)digitalReadoutInstrumentVehicleStatus, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleStatus).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelPanel, "tableLayoutPanelPanel");
		((TableLayoutPanel)(object)tableLayoutPanelPanel).Controls.Add((Control)(object)tableLayoutPanelRunFanControls, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelPanel).Controls.Add((Control)(object)digitalReadoutInstrumentFanSpeed, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelPanel).Controls.Add((Control)(object)barInstrumentCoolantTemp, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelPanel).Controls.Add((Control)(object)barInstrumentCoolantOutTemp, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelPanel).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 0, 0);
		((Control)(object)tableLayoutPanelPanel).Name = "tableLayoutPanelPanel";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFanSpeed, "digitalReadoutInstrumentFanSpeed");
		digitalReadoutInstrumentFanSpeed.FontGroup = "VSF Digital Instruments";
		((SingleInstrumentBase)digitalReadoutInstrumentFanSpeed).FreezeValue = false;
		digitalReadoutInstrumentFanSpeed.Gradient.Initialize((ValueState)0, 1);
		digitalReadoutInstrumentFanSpeed.Gradient.Modify(1, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentFanSpeed).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS026_Fan_Speed");
		((Control)(object)digitalReadoutInstrumentFanSpeed).Name = "digitalReadoutInstrumentFanSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentFanSpeed).UnitAlignment = StringAlignment.Near;
		barInstrumentCoolantTemp.BarOrientation = (ControlOrientation)1;
		barInstrumentCoolantTemp.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentCoolantTemp, "barInstrumentCoolantTemp");
		barInstrumentCoolantTemp.FontGroup = "VSF Thermometers";
		((SingleInstrumentBase)barInstrumentCoolantTemp).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentCoolantTemp).Instrument = new Qualifier((QualifierTypes)1, "virtual", "coolantTemp");
		((Control)(object)barInstrumentCoolantTemp).Name = "barInstrumentCoolantTemp";
		((TableLayoutPanel)(object)tableLayoutPanelPanel).SetRowSpan((Control)(object)barInstrumentCoolantTemp, 2);
		((SingleInstrumentBase)barInstrumentCoolantTemp).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentCoolantTemp).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentCoolantTemp).UnitAlignment = StringAlignment.Near;
		barInstrumentCoolantOutTemp.BarOrientation = (ControlOrientation)1;
		barInstrumentCoolantOutTemp.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentCoolantOutTemp, "barInstrumentCoolantOutTemp");
		barInstrumentCoolantOutTemp.FontGroup = "VSF Thermometers";
		((SingleInstrumentBase)barInstrumentCoolantOutTemp).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentCoolantOutTemp).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS169_Coolant_out_temperature");
		((Control)(object)barInstrumentCoolantOutTemp).Name = "barInstrumentCoolantOutTemp";
		((TableLayoutPanel)(object)tableLayoutPanelPanel).SetRowSpan((Control)(object)barInstrumentCoolantOutTemp, 2);
		((SingleInstrumentBase)barInstrumentCoolantOutTemp).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentCoolantOutTemp).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentCoolantOutTemp).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = "VSF Digital Instruments";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_VariableSpeedFanControl");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelFanSpeedControl).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFanSpeedControl).PerformLayout();
		((ISupportInitialize)trackBarSpeed).EndInit();
		((Control)(object)tableLayoutPanelCheckMarkAndLabel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFanTimeControl).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFanTimeControl).PerformLayout();
		((ISupportInitialize)trackBarTime).EndInit();
		((Control)(object)tableLayoutPanelRunFanControls).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelPanel).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
