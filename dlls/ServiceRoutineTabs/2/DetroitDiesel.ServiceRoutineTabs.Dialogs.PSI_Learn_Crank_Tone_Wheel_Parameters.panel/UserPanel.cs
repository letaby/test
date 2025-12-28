using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel;

public class UserPanel : CustomPanel
{
	private const int TestTimeoutSeconds = 60;

	private const double StabilizationFactor = 0.86;

	private const int TestCompletedSeconds = 6;

	private const string SeedMessage = "2701";

	private const string KeyMessage = "2702BBBB";

	private const string ShortTermAdjustMessage = "2F0519030001";

	private const string ShortTermAdjustResponse = "6F051903";

	private const string ReturnControlMessage = "2F051900";

	private const string ReturnControlResponse = "6F051900";

	private Channel mt88;

	private Channel mt88Uds;

	private bool working = false;

	private bool ecuUnlocked = false;

	private bool ecuSeeded = false;

	private bool pedalWasPressed = false;

	private bool aborted = false;

	private ProcessState state = ProcessState.NotRunning;

	private double maxRpm = 0.0;

	private int secondsRunning = 0;

	private int secondsAtMaxRpm = 0;

	private Timer checkStateTimer;

	private WarningManager warningManager;

	private TableLayoutPanel tableLayoutPanelMain;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineCoolant;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkingBreak;

	private DigitalReadoutInstrument digitalReadoutInstrumentBatteryVoltage;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private Button buttonStartRoutine;

	private Button buttonStopRoutine;

	private DigitalReadoutInstrument digitalReadoutInstrumentAccelerator;

	private TableLayoutPanel tableLayoutPanelClose;

	private System.Windows.Forms.Label labelInstructions;

	private Button buttonClose;

	private Checkmark checkmarkState;

	private TableLayoutPanel tableLayoutPanelButtons;

	private DigitalReadoutInstrument digitalReadoutInstrumentTransmissionGear;

	private System.Windows.Forms.Label labelInstructionsBig;

	private TimerControl timerControlEngineShutoff;

	private SeekTimeListView seekTimeListView;

	private bool mt88Online => mt88Uds != null && mt88 != null;

	public UserPanel()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		InitializeComponent();
		warningManager = new WarningManager(string.Empty, Resources.Message_PSILearnCrankToneWheelParameters, seekTimeListView.RequiredUserLabelPrefix);
		checkStateTimer = new Timer();
		checkStateTimer.Tick += checkState_Tick;
		checkStateTimer.Interval = 1000;
		checkStateTimer.Enabled = false;
		buttonClose.Enabled = true;
		System.Windows.Forms.Label label = labelInstructionsBig;
		string text = (labelInstructions.Text = Resources.Message_TheProcedureCanNotStart);
		label.Text = text;
		checkmarkState.CheckState = CheckState.Unchecked;
		UpdateUI();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
	}

	protected override void Dispose(bool disposing)
	{
		if (checkStateTimer != null)
		{
			checkStateTimer.Stop();
			checkStateTimer.Dispose();
			checkStateTimer = null;
		}
		((CustomPanel)this).Dispose(disposing);
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (state != ProcessState.Complete && state != ProcessState.NotRunning && state != ProcessState.WaitingOnShutdown)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			state = ProcessState.NotRunning;
			if (mt88Online)
			{
				mt88Uds.Disconnect();
			}
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	private void LogMessage(string message)
	{
		if (labelInstructions.Text != message)
		{
			((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, message);
		}
		System.Windows.Forms.Label label = labelInstructionsBig;
		string text = (labelInstructions.Text = message);
		label.Text = text;
	}

	public override void OnChannelsChanged()
	{
		Channel channel = ((CustomPanel)this).GetChannel("MT88ECU", (ChannelLookupOptions)1);
		Channel channel2 = ((CustomPanel)this).GetChannel("UDS-0", (ChannelLookupOptions)1);
		if (mt88 != channel)
		{
			if (mt88 != null)
			{
				state = ProcessState.NotRunning;
			}
			mt88 = channel;
			if (mt88 != null && mt88Uds == null && channel2 == null)
			{
				ConnectToGenericUds(0u);
				ecuUnlocked = false;
				ecuSeeded = false;
				state = ProcessState.NotRunning;
				warningManager.Reset();
			}
		}
		if (mt88Uds != channel2)
		{
			if (mt88Uds != null)
			{
				mt88Uds.ByteMessageCompleteEvent -= OnByteMessageComplete;
				if (channel2 != null)
				{
					state = ProcessState.NotRunning;
				}
				else if (state == ProcessState.WaitingOnShutdown)
				{
					timerControlEngineShutoff.Reset();
					timerControlEngineShutoff.Start();
				}
			}
			mt88Uds = channel2;
			if (mt88Uds != null)
			{
				mt88Uds.ByteMessageCompleteEvent += OnByteMessageComplete;
			}
		}
		UpdateUI();
	}

	public void ConnectToGenericUds(uint sourceAddress)
	{
		Sapi sapi = Sapi.GetSapi();
		DiagnosisProtocol diagnosisProtocol = sapi.DiagnosisProtocols["UDS"];
		ConnectionResource connectionResource = null;
		if (sapi != null && (connectionResource = diagnosisProtocol.GetConnectionResources((byte)sourceAddress).FirstOrDefault((ConnectionResource cr) => cr.Type == "CANHS" && cr.PortIndex == 1)) != null)
		{
			uint num = 416940273 + (sourceAddress << 8);
			uint num2 = 417001728 + sourceAddress;
			connectionResource.Ecu.EcuInfoComParameters["CP_IDENTIFIER_LENGTH"] = 29;
			connectionResource.Ecu.EcuInfoComParameters["CP_REQUEST_CANIDENTIFIER"] = num;
			connectionResource.Ecu.EcuInfoComParameters["CP_RESPONSE_CANIDENTIFIER"] = num2;
			sapi.Channels.Connect(connectionResource, synchronous: false);
		}
	}

	private void OnByteMessageComplete(object sender, ResultEventArgs e)
	{
		ByteMessage byteMessage = sender as ByteMessage;
		if (e.Succeeded && byteMessage != null)
		{
			switch (state)
			{
			case ProcessState.RequestSeed:
				ecuSeeded = true;
				GoMachine();
				break;
			case ProcessState.SendKey:
				ecuUnlocked = true;
				GoMachine();
				break;
			case ProcessState.SetShortTermAdjust:
				if (byteMessage.Response.ToString().Equals("6F051903", StringComparison.OrdinalIgnoreCase))
				{
					state = ProcessState.Running;
					checkStateTimer.Enabled = true;
					checkStateTimer.Start();
				}
				else
				{
					LogMessage(Resources.Message_SetShortTermAdjustRequestFailed);
					checkmarkState.CheckState = CheckState.Unchecked;
					state = ProcessState.NotRunning;
					checkStateTimer.Enabled = false;
					checkStateTimer.Stop();
				}
				break;
			case ProcessState.ReturnControl:
				if (!byteMessage.Response.ToString().Equals("6F051900", StringComparison.OrdinalIgnoreCase))
				{
					state = ProcessState.ReturnControlFailed;
				}
				GoMachine();
				break;
			}
		}
		else
		{
			state = ProcessState.NotRunning;
		}
		UpdateUI();
	}

	private void EvaluateResults()
	{
		if (aborted)
		{
			LogMessage(Resources.Message_AcceleratorReleasedBeforeProcedureHadCompleted);
			checkmarkState.CheckState = CheckState.Indeterminate;
		}
		else if (secondsRunning > 60)
		{
			LogMessage(string.Format(Resources.MessageFormat_RoutineTimeoutProcessExceeded0Seconds, 60));
			checkmarkState.CheckState = CheckState.Unchecked;
			state = ProcessState.Complete;
		}
		else if (state == ProcessState.WaitingOnShutdown)
		{
			if (secondsAtMaxRpm > 6)
			{
				LogMessage(Resources.Message_RoutineCompletedAutomaticallyReleaseAcceleratorTurnTheIgnitionOffFor15SecondsToFinalizeValues);
			}
			else
			{
				LogMessage(Resources.MessageFormat_RoutineTerminatedManuallyTurnTheIgnitionOffFor15SecondsToFinalizeValues);
			}
			checkmarkState.CheckState = CheckState.Checked;
		}
		else
		{
			LogMessage(Resources.Message_ErrorCouldNotDisableShortTermAdjustment);
			checkmarkState.CheckState = CheckState.Unchecked;
		}
	}

	private void GoMachine()
	{
		switch (state)
		{
		case ProcessState.Start:
			checkmarkState.CheckState = CheckState.Checked;
			state = ProcessState.RequestSeed;
			if (!ecuSeeded)
			{
				SendMessage("2701");
			}
			else
			{
				GoMachine();
			}
			break;
		case ProcessState.RequestSeed:
			state = ProcessState.SendKey;
			if (!ecuUnlocked)
			{
				SendMessage("2702BBBB");
			}
			else
			{
				GoMachine();
			}
			break;
		case ProcessState.SendKey:
			state = ProcessState.SetShortTermAdjust;
			secondsRunning = 0;
			secondsAtMaxRpm = 0;
			SendMessage("2F0519030001");
			break;
		case ProcessState.Stopping:
			checkStateTimer.Enabled = false;
			checkStateTimer.Stop();
			state = ProcessState.ReturnControl;
			SendMessage("2F051900");
			break;
		case ProcessState.ReturnControl:
			state = ProcessState.WaitingOnShutdown;
			EvaluateResults();
			break;
		case ProcessState.ReturnControlFailed:
			EvaluateResults();
			state = ProcessState.Complete;
			break;
		}
		UpdateUI();
	}

	private void SendMessage(string message)
	{
		if (mt88Uds != null && mt88Uds.CommunicationsState == CommunicationsState.Online)
		{
			mt88Uds.SendByteMessage(new Dump(message), synchronous: false);
		}
		else
		{
			state = ProcessState.NotRunning;
		}
	}

	private void UpdateUI()
	{
		buttonStopRoutine.Enabled = mt88Online && state == ProcessState.Running;
		buttonClose.Enabled = state == ProcessState.Complete || state == ProcessState.NotRunning;
		EnableStartButton();
		switch (state)
		{
		case ProcessState.RequestSeed:
		case ProcessState.SendKey:
		case ProcessState.SetShortTermAdjust:
			LogMessage(Resources.Message_StartingProcedure);
			break;
		case ProcessState.Running:
			LogMessage(Resources.Message_PressAndHoldAccelerator);
			break;
		case ProcessState.Stopping:
			LogMessage(Resources.Message_StoppingProcedure);
			break;
		}
	}

	private void buttonStartRoutine_Click(object sender, EventArgs e)
	{
		if (warningManager.RequestContinue())
		{
			state = ProcessState.Start;
			pedalWasPressed = false;
			aborted = false;
			GoMachine();
		}
	}

	private void buttonStopRoutine_Click(object sender, EventArgs e)
	{
		state = ProcessState.Stopping;
		GoMachine();
	}

	private void checkState_Tick(object sender, EventArgs e)
	{
		secondsRunning++;
		double result = 0.0;
		if (((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).DataItem.Value != null)
		{
			double.TryParse(((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).DataItem.Value.ToString(), out result);
		}
		double result2 = 0.0;
		if (((SingleInstrumentBase)digitalReadoutInstrumentAccelerator).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentAccelerator).DataItem.Value != null)
		{
			double.TryParse(((SingleInstrumentBase)digitalReadoutInstrumentAccelerator).DataItem.Value.ToString(), out result2);
		}
		if (result2 > 95.0)
		{
			if (result > 500.0 && result >= maxRpm * 0.86)
			{
				if (result > maxRpm)
				{
					maxRpm = result;
				}
				secondsAtMaxRpm++;
			}
			else
			{
				secondsAtMaxRpm = 0;
			}
			if (!pedalWasPressed)
			{
				pedalWasPressed = true;
				LogMessage(Resources.Message_ContinueToHoldAccelerator);
			}
		}
		if (state != ProcessState.Running || secondsAtMaxRpm > 6 || secondsRunning > 60)
		{
			state = ProcessState.Stopping;
			GoMachine();
		}
	}

	private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
		if (!InstrumentErrors(displayMessage: true) && mt88Online)
		{
			System.Windows.Forms.Label label = labelInstructionsBig;
			string text = (labelInstructions.Text = Resources.Message_TheProcedureCanStart);
			label.Text = text;
			checkmarkState.CheckState = CheckState.Checked;
		}
		else if (!mt88Online)
		{
			checkmarkState.CheckState = CheckState.Unchecked;
			System.Windows.Forms.Label label2 = labelInstructionsBig;
			string text = (labelInstructions.Text = Resources.Message_TheProcedureCanNotStart);
			label2.Text = text;
		}
	}

	private void EnableStartButton()
	{
		buttonStartRoutine.Enabled = mt88Online && !working && (state == ProcessState.Complete || state == ProcessState.NotRunning) && !InstrumentErrors(displayMessage: false);
	}

	private bool CheckInstrumentForErrors(DigitalReadoutInstrument digitalReadoutInstrument, string errorMessage, bool displayMessage)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		if (state != ProcessState.WaitingOnShutdown && displayMessage && (int)digitalReadoutInstrument.RepresentedState != 1)
		{
			System.Windows.Forms.Label label = labelInstructionsBig;
			string text = (labelInstructions.Text = errorMessage);
			label.Text = text;
			checkmarkState.CheckState = CheckState.Unchecked;
		}
		return (int)digitalReadoutInstrument.RepresentedState != 1;
	}

	private bool InstrumentErrors(bool displayMessage)
	{
		return CheckInstrumentForErrors(digitalReadoutInstrumentEngineCoolant, Resources.Message_EngineCoolantTemperatureIsLow, displayMessage) || CheckInstrumentForErrors(digitalReadoutInstrumentEngineSpeed, Resources.Message_EngineSpeedMustBeAbove200RPM, displayMessage) || CheckInstrumentForErrors(digitalReadoutInstrumentBatteryVoltage, Resources.Message_BatteryVoltageMustBeBetween1116Volts, displayMessage) || CheckInstrumentForErrors(digitalReadoutInstrumentTransmissionGear, Resources.Message_TransmissionMustBeInParkOrNeutral, displayMessage) || CheckInstrumentForErrors(digitalReadoutInstrumentParkingBreak, Resources.Message_ParkingBrakeMustBeOn, displayMessage);
	}

	private void timerControlEngineShutoff_TimerCountdownCompleted(object sender, EventArgs e)
	{
		timerControlEngineShutoff.Stop();
		LogMessage(Resources.Message_RoutineComplete);
	}

	private void timerControlEngineShutoff_TimerDisplayUpdated(object sender, EventArgs e)
	{
		Channel channel = ((CustomPanel)this).GetChannel("MT88ECU", (ChannelLookupOptions)1);
		Channel channel2 = ((CustomPanel)this).GetChannel("UDS-0", (ChannelLookupOptions)1);
		if (channel != null && channel2 != null && state == ProcessState.WaitingOnShutdown)
		{
			LogMessage(Resources.Message_IgnitionTurnedOnBeforeLearnComplete);
			state = ProcessState.NotRunning;
			timerControlEngineShutoff.Stop();
		}
		else
		{
			LogMessage(string.Format(Resources.MessageFormat_KeepTheIgnitionOffFor0Seconds, timerControlEngineShutoff.RemainingSeconds));
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d93: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		digitalReadoutInstrumentEngineCoolant = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentAccelerator = new DigitalReadoutInstrument();
		tableLayoutPanelClose = new TableLayoutPanel();
		labelInstructions = new System.Windows.Forms.Label();
		buttonClose = new Button();
		checkmarkState = new Checkmark();
		digitalReadoutInstrumentBatteryVoltage = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkingBreak = new DigitalReadoutInstrument();
		digitalReadoutInstrumentTransmissionGear = new DigitalReadoutInstrument();
		tableLayoutPanelButtons = new TableLayoutPanel();
		buttonStopRoutine = new Button();
		buttonStartRoutine = new Button();
		seekTimeListView = new SeekTimeListView();
		timerControlEngineShutoff = new TimerControl();
		labelInstructionsBig = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelClose).SuspendLayout();
		((Control)(object)tableLayoutPanelButtons).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentEngineCoolant, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentAccelerator, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelClose, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentBatteryVoltage, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentParkingBreak, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentTransmissionGear, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelButtons, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(labelInstructionsBig, 0, 0);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineCoolant, "digitalReadoutInstrumentEngineCoolant");
		digitalReadoutInstrumentEngineCoolant.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineCoolant).FreezeValue = false;
		digitalReadoutInstrumentEngineCoolant.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrumentEngineCoolant.Gradient.Modify(1, 60.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineCoolant).Instrument = new Qualifier((QualifierTypes)1, "MT88ECU", "DT_110");
		((Control)(object)digitalReadoutInstrumentEngineCoolant).Name = "digitalReadoutInstrumentEngineCoolant";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineCoolant).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentEngineCoolant.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 200.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "MT88ECU", "DT_190");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAccelerator, "digitalReadoutInstrumentAccelerator");
		digitalReadoutInstrumentAccelerator.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAccelerator).FreezeValue = false;
		digitalReadoutInstrumentAccelerator.Gradient.Initialize((ValueState)3, 4);
		digitalReadoutInstrumentAccelerator.Gradient.Modify(1, 0.0, (ValueState)2);
		digitalReadoutInstrumentAccelerator.Gradient.Modify(2, 90.0, (ValueState)1);
		digitalReadoutInstrumentAccelerator.Gradient.Modify(3, 110.0, (ValueState)3);
		digitalReadoutInstrumentAccelerator.Gradient.Modify(4, double.NaN, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentAccelerator).Instrument = new Qualifier((QualifierTypes)1, "MT88ECU", "DT_91");
		((Control)(object)digitalReadoutInstrumentAccelerator).Name = "digitalReadoutInstrumentAccelerator";
		((SingleInstrumentBase)digitalReadoutInstrumentAccelerator).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelClose, "tableLayoutPanelClose");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelClose, 4);
		((TableLayoutPanel)(object)tableLayoutPanelClose).Controls.Add(labelInstructions, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelClose).Controls.Add(buttonClose, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelClose).Controls.Add((Control)(object)checkmarkState, 0, 0);
		((Control)(object)tableLayoutPanelClose).Name = "tableLayoutPanelClose";
		componentResourceManager.ApplyResources(labelInstructions, "labelInstructions");
		labelInstructions.ForeColor = SystemColors.ControlText;
		labelInstructions.Name = "labelInstructions";
		labelInstructions.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		checkmarkState.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkState, "checkmarkState");
		((Control)(object)checkmarkState).Name = "checkmarkState";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBatteryVoltage, "digitalReadoutInstrumentBatteryVoltage");
		digitalReadoutInstrumentBatteryVoltage.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryVoltage).FreezeValue = false;
		digitalReadoutInstrumentBatteryVoltage.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentBatteryVoltage.Gradient.Modify(1, 11.0, (ValueState)1);
		digitalReadoutInstrumentBatteryVoltage.Gradient.Modify(2, 16.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryVoltage).Instrument = new Qualifier((QualifierTypes)1, "MT88ECU", "DT_168");
		((Control)(object)digitalReadoutInstrumentBatteryVoltage).Name = "digitalReadoutInstrumentBatteryVoltage";
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryVoltage).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentBatteryVoltage.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkingBreak, "digitalReadoutInstrumentParkingBreak");
		digitalReadoutInstrumentParkingBreak.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBreak).FreezeValue = false;
		digitalReadoutInstrumentParkingBreak.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentParkingBreak.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkingBreak.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBreak).Instrument = new Qualifier((QualifierTypes)1, "MT88ECU", "DT_70");
		((Control)(object)digitalReadoutInstrumentParkingBreak).Name = "digitalReadoutInstrumentParkingBreak";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBreak).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentParkingBreak.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentTransmissionGear, "digitalReadoutInstrumentTransmissionGear");
		digitalReadoutInstrumentTransmissionGear.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentTransmissionGear).FreezeValue = false;
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
		digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
		digitalReadoutInstrumentTransmissionGear.Gradient.Initialize((ValueState)3, 12);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(6, 5.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(7, 6.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(8, 7.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(9, 8.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(10, 9.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(11, 10.0, (ValueState)3);
		digitalReadoutInstrumentTransmissionGear.Gradient.Modify(12, "Parameter specific ($FB)", (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentTransmissionGear).Instrument = new Qualifier((QualifierTypes)1, "J1939-3", "DT_523");
		((Control)(object)digitalReadoutInstrumentTransmissionGear).Name = "digitalReadoutInstrumentTransmissionGear";
		((SingleInstrumentBase)digitalReadoutInstrumentTransmissionGear).ShowUnits = false;
		((SingleInstrumentBase)digitalReadoutInstrumentTransmissionGear).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentTransmissionGear).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentTransmissionGear.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(tableLayoutPanelButtons, "tableLayoutPanelButtons");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelButtons, 2);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add(buttonStopRoutine, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add(buttonStartRoutine, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)seekTimeListView, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelButtons).Controls.Add((Control)(object)timerControlEngineShutoff, 0, 1);
		((Control)(object)tableLayoutPanelButtons).Name = "tableLayoutPanelButtons";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)tableLayoutPanelButtons, 3);
		componentResourceManager.ApplyResources(buttonStopRoutine, "buttonStopRoutine");
		buttonStopRoutine.Name = "buttonStopRoutine";
		buttonStopRoutine.UseCompatibleTextRendering = true;
		buttonStopRoutine.UseVisualStyleBackColor = true;
		buttonStopRoutine.Click += buttonStopRoutine_Click;
		componentResourceManager.ApplyResources(buttonStartRoutine, "buttonStartRoutine");
		buttonStartRoutine.Name = "buttonStartRoutine";
		buttonStartRoutine.UseCompatibleTextRendering = true;
		buttonStartRoutine.UseVisualStyleBackColor = true;
		buttonStartRoutine.Click += buttonStartRoutine_Click;
		((TableLayoutPanel)(object)tableLayoutPanelButtons).SetColumnSpan((Control)(object)seekTimeListView, 3);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "PSI Learn Crank Tone Wheel Parameters";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		timerControlEngineShutoff.Duration = TimeSpan.Parse("00:00:15");
		timerControlEngineShutoff.FontGroup = null;
		componentResourceManager.ApplyResources(timerControlEngineShutoff, "timerControlEngineShutoff");
		((Control)(object)timerControlEngineShutoff).Name = "timerControlEngineShutoff";
		timerControlEngineShutoff.TimerCountdownCompletedDisplayMessage = null;
		timerControlEngineShutoff.TimerCountdownCompleted += timerControlEngineShutoff_TimerCountdownCompleted;
		timerControlEngineShutoff.TimerDisplayUpdated += timerControlEngineShutoff_TimerDisplayUpdated;
		componentResourceManager.ApplyResources(labelInstructionsBig, "labelInstructionsBig");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)labelInstructionsBig, 4);
		labelInstructionsBig.ForeColor = Color.Red;
		labelInstructionsBig.Name = "labelInstructionsBig";
		labelInstructionsBig.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_PSI_Learn_Crank_Tone_Wheel_Parameters");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanelClose).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelButtons).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
