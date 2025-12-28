using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Dynamic_Ride_Height_Calibration.panel;

public class UserPanel : CustomPanel
{
	private enum InstrumentState
	{
		NotActive,
		Active,
		Error,
		SNA
	}

	private enum ServiceMode
	{
		Normal = 0,
		Calibration = 12
	}

	private enum Level
	{
		None = 0,
		Normal1 = 1,
		Normal2 = 2,
		Upper = 6,
		Lowest = 7
	}

	private enum ProcessState
	{
		NotRunning,
		Starting,
		StartingOnNext,
		Step1,
		Step1OnNext,
		Step2,
		Step2OnNext,
		Step3,
		Step3OnNext,
		Step4,
		Step4OnNext,
		Step5,
		Step5OnNext,
		Complete
	}

	private Channel xmc02t;

	private Channel hsv;

	private static string FrontAxleLiftingInstrument = "DT_1739";

	private static string RearAxleLiftingInstrument = "DT_1756";

	private static string FrontAxleLoweringInstrument = "DT_1740";

	private static string RearAxleLoweringInstrument = "DT_1755";

	private static string HsvName = "HSV";

	private static string Xmc02tName = "XMC02T";

	private static string MoveServiceQualifier = "IOC_RHC_OutputCtrl_Control(DiagRqData_OC_NomLvlRqFAx_Enbl={0},Nominal_Level_Request_Front_Axle={2},DiagRqData_OC_NomLvlRqRAx={2},DiagRqData_OC_NomLvlRqRAx_Enbl={1},DiagRqData_OC_LvlCtrlMd_Rq={3},Level_Control_Mode_Request=1)";

	private static string HsvHighestLvlQualifier = "RT_HIGHESTLVL";

	private static string HsvNomLvl1Qualifier = "RT_NOMLVL1";

	private static string HsvNomLvl2Qualifier = "RT_NOMLVL2";

	private static string HsvLowestLvlQualifier = "RT_LOESTLVL";

	private Timer timer;

	private Timer lastStepFinishedTimer;

	private DateTime lastMoved;

	private Dictionary<Level, string> hsvServices = new Dictionary<Level, string>();

	private Timer hsvCalibrateTimer;

	private string hsvServiceQualifier = string.Empty;

	private readonly ProcessState[] userStates = new ProcessState[8]
	{
		ProcessState.NotRunning,
		ProcessState.Starting,
		ProcessState.Step1,
		ProcessState.Step2,
		ProcessState.Step3,
		ProcessState.Step4,
		ProcessState.Step5,
		ProcessState.Complete
	};

	private bool popupShown = false;

	private bool aborted = false;

	private bool working = false;

	private bool lastStepFinished = false;

	private ProcessState state;

	private bool hasFrontAxel = true;

	private TableLayoutPanel tableLayoutPanelInstruments;

	private TableLayoutPanel tableLayoutPanelMain;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrumentAirPressure;

	private CheckBox checkBoxTruckOnStand;

	private CheckBox checkBoxFullSetOfBlocks;

	private CheckBox checkBoxNoOneAround;

	private TableLayoutPanel tableLayoutPanel1;

	private Label label3;

	private ScalingLabel scalingLabelRearAxleStatus;

	private TableLayoutPanel tableLayoutPanel5;

	private Label label20;

	private ScalingLabel scalingLabelFrontAxleStatus;

	private TextBox textBoxInstructions;

	private System.Windows.Forms.Label label1;

	private TableLayoutPanel tableLayoutPanelControls;

	private Button buttonClose;

	private TableLayoutPanel tableLayoutPanel2;

	private Checkmark checkmarkStatus;

	private System.Windows.Forms.Label labelStatus;

	private Button buttonStartNext;

	private Button buttonAbort;

	private TabControl tabControl;

	private TabPage tabPageAxles;

	private PictureBox pictureBoxAxles;

	private TabPage tabPageFrontAxle;

	private PictureBox pictureBoxFrontAxle;

	private TabPage tabPageCenterAxle;

	private PictureBox pictureBoxCenterAxle;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private bool HasFrontAxel => hasFrontAxel;

	public UserPanel()
	{
		InitializeComponent();
		UpdateUI();
		timer = new Timer();
		timer.Interval = 1000;
		timer.Tick += UpdateUI;
		timer.Start();
		lastMoved = DateTime.Now.AddSeconds(-5.0);
		lastStepFinishedTimer = new Timer();
		lastStepFinishedTimer.Interval = 10000;
		lastStepFinishedTimer.Tick += LastStepFinished;
		hsvCalibrateTimer = new Timer();
		hsvCalibrateTimer.Interval = 100;
		hsvCalibrateTimer.Tick += hsvCalibrateTimer_Tick;
		hsvServices.Add(Level.Normal1, HsvNomLvl1Qualifier);
		hsvServices.Add(Level.Normal2, HsvNomLvl2Qualifier);
		hsvServices.Add(Level.Upper, HsvHighestLvlQualifier);
		hsvServices.Add(Level.Lowest, HsvLowestLvlQualifier);
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		timer.Stop();
		timer.Tick -= UpdateUI;
		lastStepFinishedTimer.Stop();
		lastStepFinishedTimer.Tick -= LastStepFinished;
		StopHsvService();
		hsvCalibrateTimer.Tick -= hsvCalibrateTimer_Tick;
		if (working)
		{
			Abort("Form Closed");
		}
		if (xmc02t != null)
		{
			xmc02t.Services.ServiceCompleteEvent -= channel_ServiceCompleteEvent;
		}
		MoveService(moveFront: false, moveRear: false, Level.None, ServiceMode.Normal);
		SetChannelXmc02t(null);
		SetChannelHsv(null);
	}

	private void UpdateUI(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void LastStepFinished(object sender, EventArgs e)
	{
		lastStepFinishedTimer.Stop();
		lastStepFinished = true;
		UpdateUI();
	}

	private void Log(string message)
	{
		((CustomPanel)this).LabelLog("HSVCalibrate", message);
	}

	private void UpdateInstructions(string step, string message, string instructions)
	{
		textBoxInstructions.Text = step + Environment.NewLine + message + Environment.NewLine + instructions;
		labelStatus.Text = instructions;
		Log(message);
	}

	private void UpdateInstructionsWithSeparateMessage(string step, string message, string instructions)
	{
		textBoxInstructions.Text = step + Environment.NewLine + message;
		labelStatus.Text = instructions;
		Log(message);
	}

	private void GoMachine()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Invalid comparison between Unknown and I4
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		if (!aborted)
		{
			working = true;
			state++;
			switch (state)
			{
			case ProcessState.Starting:
				lastStepFinished = false;
				hasFrontAxel = (int)scalingLabelFrontAxleStatus.RepresentedState == 1 || (int)scalingLabelFrontAxleStatus.RepresentedState == 2;
				UpdateInstructions(Resources.Message_WARNING, Resources.Message_CalibrationProcedure, Resources.Message_ToBeginClickNext);
				if (!popupShown)
				{
					popupShown = true;
					MessageBox.Show(Resources.Message_WARNING1, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				}
				break;
			case ProcessState.StartingOnNext:
				MoveService(HasFrontAxel, moveRear: true, Level.None, ServiceMode.Normal);
				break;
			case ProcessState.Step1:
				UpdateInstructions(Resources.Message_Step1, Resources.Message_RaiseTruckToMaximumHeightThenAutomaticallyDrop14, Resources.Message_ClickNextToRaiseTruck);
				break;
			case ProcessState.Step1OnNext:
				lastMoved = DateTime.Now;
				MoveService(HasFrontAxel, moveRear: true, Level.Upper, ServiceMode.Calibration);
				break;
			case ProcessState.Step2:
				UpdateInstructions(Resources.Message_Step2, "", Resources.Message_TruckIsCurrentlyRaising);
				break;
			case ProcessState.Step2OnNext:
				lastMoved = DateTime.Now;
				MoveService(HasFrontAxel, moveRear: true, Level.Normal1, ServiceMode.Calibration);
				break;
			case ProcessState.Step3:
				UpdateInstructions(Resources.Message_Step3, "", Resources.Message_TruckIsCurrentlyLowering);
				break;
			case ProcessState.Step3OnNext:
				lastMoved = DateTime.Now;
				MoveService(HasFrontAxel, moveRear: true, Level.Normal2, ServiceMode.Calibration);
				break;
			case ProcessState.Step4:
				UpdateInstructions(Resources.Message_Step4, "", Resources.Message_TruckIsCurrentlyLowering);
				break;
			case ProcessState.Step4OnNext:
				lastMoved = DateTime.Now;
				MoveService(HasFrontAxel, moveRear: true, Level.Lowest, ServiceMode.Calibration);
				break;
			case ProcessState.Step5:
				UpdateInstructionsWithSeparateMessage(Resources.Message_Step5, Resources.Message_TruckIsCurrentlyLoweringStep5Instructions, Resources.Message_TruckIsCurrentlyLowering);
				lastStepFinishedTimer.Start();
				break;
			case ProcessState.Step5OnNext:
				StopHsvService();
				lastMoved = DateTime.Now;
				MoveService(moveFront: false, moveRear: false, Level.None, ServiceMode.Normal);
				break;
			case ProcessState.Complete:
				StopHsvService();
				UpdateInstructions(Resources.Message_Step6, Resources.Message_HeightValuesHaveBeenStored, Resources.Message_CalibrationCompleted);
				working = false;
				break;
			}
		}
		UpdateUI();
	}

	private void channel_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (aborted)
		{
			return;
		}
		if (e.Succeeded)
		{
			if (xmc02t != null)
			{
				GoMachine();
			}
			else
			{
				Abort(Resources.Message_EcuDisconnectedBeforeCompletion);
			}
		}
		else
		{
			Abort(e.Exception.Message);
		}
	}

	private void Abort(string reason)
	{
		StopHsvService();
		state = ProcessState.Complete;
		aborted = true;
		working = false;
		UpdateInstructions(Resources.Message_Abort, reason, Resources.Message_Aborted);
		MoveService(moveFront: true, moveRear: true, Level.Normal1, ServiceMode.Normal);
		UpdateUI();
	}

	public override void OnChannelsChanged()
	{
		SetChannelXmc02t(((CustomPanel)this).GetChannel(Xmc02tName, (ChannelLookupOptions)3));
		SetChannelHsv(((CustomPanel)this).GetChannel(HsvName, (ChannelLookupOptions)3));
		UpdateUI();
	}

	private void UpdateUI()
	{
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		CheckBox checkBox = checkBoxTruckOnStand;
		CheckBox checkBox2 = checkBoxFullSetOfBlocks;
		bool flag = (checkBoxNoOneAround.Enabled = !working);
		flag = (checkBox2.Enabled = flag);
		checkBox.Enabled = flag;
		buttonStartNext.Text = (working ? Resources.Message_Next : Resources.Message_StartCalibration);
		buttonAbort.Enabled = working;
		if (state == ProcessState.Step5)
		{
			Checkmark obj = checkmarkStatus;
			flag = (buttonStartNext.Enabled = (working ? lastStepFinished : CanStart()));
			obj.Checked = flag;
		}
		else
		{
			Checkmark obj2 = checkmarkStatus;
			flag = (buttonStartNext.Enabled = (working ? ((DateTime.Now - lastMoved).Seconds >= 5) : CanStart()));
			obj2.Checked = flag;
		}
		if (buttonStartNext.Enabled)
		{
			switch (state)
			{
			case ProcessState.Step2:
				UpdateInstructions(Resources.Message_Step2, Resources.Message_TruckHasBeenRaised, Resources.Message_OnceBlocksAreInPlaceClickNextTruckWillBeginToLower);
				break;
			case ProcessState.Step3:
				UpdateInstructions(Resources.Message_Step3, Resources.Message_TruckHasBeenLowered, Resources.Message_OnceAeroHeightGaugeBlocksAreInPlaceClickNextToLowerTheTruck);
				break;
			case ProcessState.Step4:
				UpdateInstructions(Resources.Message_Step4, Resources.Message_TruckHasBeenLoweredRemoveAeroGaugeBlocks, Resources.Message_OnceBlocksAreRemovedClickNextTruckWillBeginToLower);
				break;
			case ProcessState.Step5:
				UpdateInstructionsWithSeparateMessage(Resources.Message_Step5, Resources.Message_TruckIsCurrentlyLoweringStep5Instructions, Resources.Message_ClickNextToReturnToStandardHeight);
				break;
			}
		}
		UpdateScalingLabelStatus(scalingLabelFrontAxleStatus, FrontAxleLiftingInstrument, FrontAxleLoweringInstrument);
		UpdateScalingLabelStatus(scalingLabelRearAxleStatus, RearAxleLiftingInstrument, RearAxleLoweringInstrument);
	}

	private bool CanStart()
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Invalid comparison between Unknown and I4
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Invalid comparison between Unknown and I4
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Invalid comparison between Unknown and I4
		if (xmc02t == null || !xmc02t.Online)
		{
			labelStatus.Text = Resources.Message_XMC02TOffline;
			return false;
		}
		if (!checkBoxTruckOnStand.Checked)
		{
			labelStatus.Text = Resources.Message_ViewCalibrationBlockPhotos;
			return false;
		}
		if (!checkBoxFullSetOfBlocks.Checked)
		{
			labelStatus.Text = Resources.Message_ConfirmThatFullSetOfHeightGaugeBlocksAvailable;
			return false;
		}
		if (!checkBoxNoOneAround.Checked)
		{
			labelStatus.Text = Resources.Message_ConfirmThatNoOneIsWorkingOnOrNearTheTruck;
			return false;
		}
		if ((int)digitalReadoutInstrumentAirPressure.RepresentedState != 1)
		{
			labelStatus.Text = Resources.Message_AirPressureInvalid;
			return false;
		}
		if ((int)digitalReadoutInstrumentParkingBrake.RepresentedState != 1)
		{
			labelStatus.Text = Resources.Message_ParkingBrakeInvalid;
			return false;
		}
		if ((int)digitalReadoutInstrumentEngineSpeed.RepresentedState != 1)
		{
			labelStatus.Text = Resources.Message_EngineSpeedInvalid;
			return false;
		}
		labelStatus.Text = Resources.Message_ReadyToStart;
		return true;
	}

	private ValueState UpdateScalingLabelStatus(ScalingLabel scalingLabel, string liftingQualifier, string loweringQualifier)
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		Choice choice = ReadChoiceValue(hsv, liftingQualifier);
		Choice choice2 = ReadChoiceValue(hsv, loweringQualifier);
		if (choice == null || choice.Index == 2 || choice.Index == 3 || choice2 == null || choice2.Index == 2 || choice2.Index == 3)
		{
			((Control)(object)scalingLabel).Text = Resources.Message_SNA;
			scalingLabel.RepresentedState = (ValueState)3;
		}
		else if (choice.Index == 1)
		{
			lastMoved = DateTime.Now;
			((Control)(object)scalingLabel).Text = Resources.Message_Raising;
			scalingLabel.RepresentedState = (ValueState)2;
		}
		else if (choice2.Index == 1)
		{
			lastMoved = DateTime.Now;
			((Control)(object)scalingLabel).Text = Resources.Message_Lowering;
			scalingLabel.RepresentedState = (ValueState)2;
		}
		else
		{
			((Control)(object)scalingLabel).Text = Resources.Message_Stationary;
			scalingLabel.RepresentedState = (ValueState)1;
		}
		return scalingLabel.RepresentedState;
	}

	private Choice ReadChoiceValue(Channel channel, string qualifier)
	{
		if (channel != null && channel.Instruments != null && channel.Instruments[qualifier] != null && channel.Instruments[qualifier].InstrumentValues != null && channel.Instruments[qualifier].InstrumentValues.Current != null)
		{
			return (Choice)channel.Instruments[qualifier].InstrumentValues.Current.Value;
		}
		return null;
	}

	private void SetChannelXmc02t(Channel channel)
	{
		if (xmc02t != channel)
		{
			StopHsvService();
			if (xmc02t != null)
			{
				xmc02t.Services.ServiceCompleteEvent -= channel_ServiceCompleteEvent;
			}
			xmc02t = channel;
			if (xmc02t != null)
			{
				channel.Services.ServiceCompleteEvent += channel_ServiceCompleteEvent;
			}
		}
	}

	private void SetChannelHsv(Channel channel)
	{
		if (hsv != channel)
		{
			StopHsvService();
			if (hsv != null)
			{
				hsv.Instruments[FrontAxleLiftingInstrument].InstrumentUpdateEvent -= OnInstrumentUpdateEvent;
				hsv.Instruments[RearAxleLiftingInstrument].InstrumentUpdateEvent -= OnInstrumentUpdateEvent;
				hsv.Instruments[FrontAxleLoweringInstrument].InstrumentUpdateEvent -= OnInstrumentUpdateEvent;
				hsv.Instruments[RearAxleLoweringInstrument].InstrumentUpdateEvent -= OnInstrumentUpdateEvent;
			}
			hsv = channel;
			if (hsv != null)
			{
				channel.Instruments[FrontAxleLiftingInstrument].InstrumentUpdateEvent += OnInstrumentUpdateEvent;
				channel.Instruments[RearAxleLiftingInstrument].InstrumentUpdateEvent += OnInstrumentUpdateEvent;
				channel.Instruments[FrontAxleLoweringInstrument].InstrumentUpdateEvent += OnInstrumentUpdateEvent;
				channel.Instruments[RearAxleLoweringInstrument].InstrumentUpdateEvent += OnInstrumentUpdateEvent;
			}
		}
	}

	private void OnInstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateUI();
	}

	private bool MoveService(bool moveFront, bool moveRear, Level level, ServiceMode mode)
	{
		bool result = true;
		if (mode == ServiceMode.Calibration)
		{
			hsvServiceQualifier = hsvServices[level];
			Log(hsvServiceQualifier);
			hsvCalibrateTimer.Start();
			GoMachine();
		}
		else
		{
			StopHsvService();
			string text = string.Format(MoveServiceQualifier, moveFront ? 1 : 0, moveRear ? 1 : 0, (int)level, (int)mode);
			Log(text);
			if (!RunService(xmc02t, text) && !aborted)
			{
				result = false;
				Abort(string.Format(Resources.MessageFormat_ServiceCannotBeStarted0, text));
			}
		}
		return result;
	}

	private bool RunService(Channel channel, string serviceQualifier)
	{
		if (channel != null && channel.Online)
		{
			Service service = channel.Services[serviceQualifier];
			if (service != null)
			{
				service.InputValues.ParseValues(serviceQualifier);
				service.Execute(synchronous: false);
				return true;
			}
		}
		return false;
	}

	private void StopHsvService()
	{
		hsvCalibrateTimer.Stop();
		hsvServiceQualifier = string.Empty;
	}

	private void hsvCalibrateTimer_Tick(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(hsvServiceQualifier) || aborted || !RunService(hsv, hsvServiceQualifier))
		{
			StopHsvService();
		}
	}

	private void buttonStartNext_Click(object sender, EventArgs e)
	{
		if (userStates.Contains(state))
		{
			if (!working)
			{
				state = ProcessState.NotRunning;
				aborted = false;
			}
			GoMachine();
		}
		UpdateUI();
	}

	private void buttonAbort_Click(object sender, EventArgs e)
	{
		Abort(Resources.Message_UserAbortedTest);
	}

	private void checkBoxTruckOnStand_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxTruckOnStand.Checked)
		{
			Log(Resources.Message_UserConfirmedTruckOnStand);
		}
		UpdateUI();
	}

	private void checkBoxFullSetOfBlocks_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxFullSetOfBlocks.Checked)
		{
			Log(Resources.Message_UserConfirmedFullSetOfBlocks);
		}
		UpdateUI();
	}

	private void checkBoxNoOneAround_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBoxNoOneAround.Checked)
		{
			Log(Resources.Message_UserConfirmedNoOneWorkingOnOrAroundTruck);
		}
		UpdateUI();
	}

	private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
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
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
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
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1063: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelInstruments = new TableLayoutPanel();
		tableLayoutPanel1 = new TableLayoutPanel();
		label3 = new Label();
		scalingLabelRearAxleStatus = new ScalingLabel();
		tableLayoutPanel5 = new TableLayoutPanel();
		label20 = new Label();
		scalingLabelFrontAxleStatus = new ScalingLabel();
		checkBoxNoOneAround = new CheckBox();
		digitalReadoutInstrumentAirPressure = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		checkBoxTruckOnStand = new CheckBox();
		checkBoxFullSetOfBlocks = new CheckBox();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
		label1 = new System.Windows.Forms.Label();
		tableLayoutPanelMain = new TableLayoutPanel();
		tableLayoutPanelControls = new TableLayoutPanel();
		buttonClose = new Button();
		tableLayoutPanel2 = new TableLayoutPanel();
		checkmarkStatus = new Checkmark();
		labelStatus = new System.Windows.Forms.Label();
		buttonStartNext = new Button();
		buttonAbort = new Button();
		textBoxInstructions = new TextBox();
		tabControl = new TabControl();
		tabPageAxles = new TabPage();
		pictureBoxAxles = new PictureBox();
		tabPageFrontAxle = new TabPage();
		pictureBoxFrontAxle = new PictureBox();
		tabPageCenterAxle = new TabPage();
		pictureBoxCenterAxle = new PictureBox();
		((Control)(object)tableLayoutPanelInstruments).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel5).SuspendLayout();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelControls).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		tabControl.SuspendLayout();
		tabPageAxles.SuspendLayout();
		((ISupportInitialize)pictureBoxAxles).BeginInit();
		tabPageFrontAxle.SuspendLayout();
		((ISupportInitialize)pictureBoxFrontAxle).BeginInit();
		tabPageCenterAxle.SuspendLayout();
		((ISupportInitialize)pictureBoxCenterAxle).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)tableLayoutPanel1, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)tableLayoutPanel5, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add(checkBoxNoOneAround, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentAirPressure, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument3, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add(checkBoxTruckOnStand, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add(checkBoxFullSetOfBlocks, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentParkingBrake, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add(label1, 0, 3);
		((Control)(object)tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)label3, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)scalingLabelRearAxleStatus, 0, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		label3.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label3, "label3");
		((Control)(object)label3).Name = "label3";
		label3.Orientation = (TextOrientation)1;
		scalingLabelRearAxleStatus.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelRearAxleStatus, "scalingLabelRearAxleStatus");
		scalingLabelRearAxleStatus.FontGroup = null;
		scalingLabelRearAxleStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelRearAxleStatus).Name = "scalingLabelRearAxleStatus";
		componentResourceManager.ApplyResources(tableLayoutPanel5, "tableLayoutPanel5");
		((TableLayoutPanel)(object)tableLayoutPanel5).Controls.Add((Control)(object)label20, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel5).Controls.Add((Control)(object)scalingLabelFrontAxleStatus, 0, 1);
		((Control)(object)tableLayoutPanel5).Name = "tableLayoutPanel5";
		label20.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(label20, "label20");
		((Control)(object)label20).Name = "label20";
		label20.Orientation = (TextOrientation)1;
		scalingLabelFrontAxleStatus.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelFrontAxleStatus, "scalingLabelFrontAxleStatus");
		scalingLabelFrontAxleStatus.FontGroup = null;
		scalingLabelFrontAxleStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelFrontAxleStatus).Name = "scalingLabelFrontAxleStatus";
		componentResourceManager.ApplyResources(checkBoxNoOneAround, "checkBoxNoOneAround");
		checkBoxNoOneAround.BackColor = SystemColors.Control;
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)checkBoxNoOneAround, 3);
		checkBoxNoOneAround.Name = "checkBoxNoOneAround";
		checkBoxNoOneAround.UseCompatibleTextRendering = true;
		checkBoxNoOneAround.UseVisualStyleBackColor = false;
		checkBoxNoOneAround.CheckedChanged += checkBoxNoOneAround_CheckedChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentAirPressure, "digitalReadoutInstrumentAirPressure");
		digitalReadoutInstrumentAirPressure.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentAirPressure).FreezeValue = false;
		digitalReadoutInstrumentAirPressure.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrumentAirPressure.Gradient.Modify(1, 193.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentAirPressure).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_BrkAirPress2_Stat_EAPU");
		((Control)(object)digitalReadoutInstrumentAirPressure).Name = "digitalReadoutInstrumentAirPressure";
		((SingleInstrumentBase)digitalReadoutInstrumentAirPressure).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentAirPressure.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "HSV", "DT_1724");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "HSV", "DT_1722");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "HSV", "DT_1721");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "HSV", "DT_1723");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(checkBoxTruckOnStand, "checkBoxTruckOnStand");
		checkBoxTruckOnStand.BackColor = SystemColors.Control;
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)checkBoxTruckOnStand, 3);
		checkBoxTruckOnStand.Name = "checkBoxTruckOnStand";
		checkBoxTruckOnStand.UseCompatibleTextRendering = true;
		checkBoxTruckOnStand.UseVisualStyleBackColor = false;
		checkBoxTruckOnStand.CheckedChanged += checkBoxTruckOnStand_CheckedChanged;
		componentResourceManager.ApplyResources(checkBoxFullSetOfBlocks, "checkBoxFullSetOfBlocks");
		checkBoxFullSetOfBlocks.BackColor = SystemColors.Control;
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)checkBoxFullSetOfBlocks, 3);
		checkBoxFullSetOfBlocks.Name = "checkBoxFullSetOfBlocks";
		checkBoxFullSetOfBlocks.UseCompatibleTextRendering = true;
		checkBoxFullSetOfBlocks.UseVisualStyleBackColor = false;
		checkBoxFullSetOfBlocks.CheckedChanged += checkBoxFullSetOfBlocks_CheckedChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(2, 200.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
		digitalReadoutInstrumentParkingBrake.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).FreezeValue = false;
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState)3, 3);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentParkingBrake.RepresentedStateChanged += digitalReadoutInstrument_RepresentedStateChanged;
		componentResourceManager.ApplyResources(label1, "label1");
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)label1, 3);
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelInstruments, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelControls, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxInstructions, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(tabControl, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(tableLayoutPanelControls, "tableLayoutPanelControls");
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(buttonClose, 6, 0);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add((Control)(object)tableLayoutPanel2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(buttonStartNext, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelControls).Controls.Add(buttonAbort, 4, 0);
		((Control)(object)tableLayoutPanelControls).Name = "tableLayoutPanelControls";
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmarkStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(labelStatus, 1, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonStartNext, "buttonStartNext");
		buttonStartNext.Name = "buttonStartNext";
		buttonStartNext.UseCompatibleTextRendering = true;
		buttonStartNext.UseVisualStyleBackColor = true;
		buttonStartNext.Click += buttonStartNext_Click;
		componentResourceManager.ApplyResources(buttonAbort, "buttonAbort");
		buttonAbort.Name = "buttonAbort";
		buttonAbort.UseCompatibleTextRendering = true;
		buttonAbort.UseVisualStyleBackColor = true;
		buttonAbort.Click += buttonAbort_Click;
		componentResourceManager.ApplyResources(textBoxInstructions, "textBoxInstructions");
		textBoxInstructions.Name = "textBoxInstructions";
		textBoxInstructions.ReadOnly = true;
		componentResourceManager.ApplyResources(tabControl, "tabControl");
		tabControl.Controls.Add(tabPageAxles);
		tabControl.Controls.Add(tabPageFrontAxle);
		tabControl.Controls.Add(tabPageCenterAxle);
		tabControl.Name = "tabControl";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)tabControl, 3);
		tabControl.SelectedIndex = 0;
		tabPageAxles.Controls.Add(pictureBoxAxles);
		componentResourceManager.ApplyResources(tabPageAxles, "tabPageAxles");
		tabPageAxles.Name = "tabPageAxles";
		tabPageAxles.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(pictureBoxAxles, "pictureBoxAxles");
		pictureBoxAxles.Name = "pictureBoxAxles";
		pictureBoxAxles.TabStop = false;
		tabPageFrontAxle.Controls.Add(pictureBoxFrontAxle);
		componentResourceManager.ApplyResources(tabPageFrontAxle, "tabPageFrontAxle");
		tabPageFrontAxle.Name = "tabPageFrontAxle";
		tabPageFrontAxle.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(pictureBoxFrontAxle, "pictureBoxFrontAxle");
		pictureBoxFrontAxle.Name = "pictureBoxFrontAxle";
		pictureBoxFrontAxle.TabStop = false;
		tabPageCenterAxle.Controls.Add(pictureBoxCenterAxle);
		componentResourceManager.ApplyResources(tabPageCenterAxle, "tabPageCenterAxle");
		tabPageCenterAxle.Name = "tabPageCenterAxle";
		tabPageCenterAxle.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(pictureBoxCenterAxle, "pictureBoxCenterAxle");
		pictureBoxCenterAxle.Name = "pictureBoxCenterAxle";
		pictureBoxCenterAxle.TabStop = false;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Aerodynamic_Ride_Height_Calibration");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanelInstruments).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInstruments).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel5).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanelControls).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		tabControl.ResumeLayout(performLayout: false);
		tabPageAxles.ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxAxles).EndInit();
		tabPageFrontAxle.ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxFrontAxle).EndInit();
		tabPageCenterAxle.ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxCenterAxle).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
