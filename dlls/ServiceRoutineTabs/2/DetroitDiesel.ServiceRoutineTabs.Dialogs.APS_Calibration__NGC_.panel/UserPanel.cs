using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__NGC_.panel;

public class UserPanel : CustomPanel
{
	private static string ChannelName = "APS301T";

	private static string AbsName = "ABS02T";

	private static string SsamName = "SSAM02T";

	private static string VrduName = "VRDU02T";

	private static string CalibrateCenterQualifier = "RT_Calibrate_extern_and_intern_steering_angle_Start_Calibration_state";

	private static string CalibrateLeftEndstopQualifier = "RT_Calibrate_Left_Endstop_Start_Endstop_Learnconditions_Endstop_state";

	private static string CalibrateRightEndstopQualifier = "RT_Calibrate_Right_Endstop_Start_Endstop_Learnconditions_Endstop_state";

	private static string CalibrateTorsionBarTorqueStartQualifier = "RT_Calibrate_TorsionBarTorqueOffset_Start";

	private static string CalibrateTorsionBarTorqueResultQualifier = "RT_Calibrate_TorsionBarTorqueOffset_Request_Results_Tbtoffset_calibration_request_status";

	private static string TorsionBarTorqueOffsetParameterQualifier = "TorsionBarTorqueOffset";

	private static string HardResetEcuQualifier = "FN_HardReset";

	private static string SteeringAngleQualifier = "DT_Steering_Angle_Steering_Angle";

	private static string ResetCalibrationDataQualifier = "RT_Discard_calibration_data_Start";

	private static int ResetCalibrationDataRawValue = 1;

	private static double TorsionBarRangeValue = 0.5;

	private static int HardResetDelayTime = 15;

	private CalibrationStep[] calibrationSteps;

	private CalibrationStep[] steeringAngleCalibrationSteps = new CalibrationStep[15]
	{
		new CalibrationStep(Resources.Message_PressNextToBeginCalibrationProcess, string.Empty, CalibrationActions.DisplayText, buttonEnabled: true),
		new CalibrationStep(Resources.Message_ResettingCalibrationData, string.Empty, CalibrationActions.ResetCalibrationData, buttonEnabled: false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
		new CalibrationStep(Resources.Message_StartEngine, Resources.Message_ClickNextToContinueToCalibration, CalibrationActions.StartEngine, buttonEnabled: true),
		new CalibrationStep(Resources.Message_CenterSteeringWheel, Resources.Message_ClickNextToContinue, CalibrationActions.DisplayText, buttonEnabled: true),
		new CalibrationStep(Resources.Message_CenterSteeringWheel, string.Empty, CalibrationActions.ConfirmCenter, buttonEnabled: false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
		new CalibrationStep(Resources.Message_CalibratingCenter, string.Empty, CalibrationActions.CalibrateCenter, buttonEnabled: false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
		new CalibrationStep(Resources.Message_TurnTheSteeringWheelToTheLeftUntilItReachesTheEndstopAndHold, string.Empty, CalibrationActions.DisplayText, buttonEnabled: true, 0.0, 0.0, 1500.0, "(Fault),(900,Ok),(1250,Fault)"),
		new CalibrationStep(Resources.Message_CalibrateLeftEndstop, string.Empty, CalibrationActions.CalibrateLeftEndstop, buttonEnabled: false, 0.0, 0.0, 1500.0, "(Fault),(900,Ok),(1250,Fault)"),
		new CalibrationStep(Resources.Message_TurnTheSteeringWheelToTheRightUntilItReachesTheEndstopAndHold, string.Empty, CalibrationActions.DisplayText, buttonEnabled: true, 0.0, -1500.0, 0.0, "(Fault),(-1250,Ok),(-900,Fault)"),
		new CalibrationStep(Resources.Message_CalibrateRightEndstop, string.Empty, CalibrationActions.CalibrateRightEndstop, buttonEnabled: false, 0.0, -1500.0, 0.0, "(Fault),(-1250,Ok),(-900,Fault)"),
		new CalibrationStep(Resources.Message_CenterSteeringWheel, Resources.Message_ClickNextToContinue, CalibrationActions.DisplayText, buttonEnabled: true, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
		new CalibrationStep(Resources.Message_PerformingHardReset, string.Empty, CalibrationActions.HardReset, buttonEnabled: false),
		new CalibrationStep(Resources.Message_PerformingHardReset, string.Format(Resources.MessageFormat_PleaseWait0, HardResetDelayTime), CalibrationActions.HardResetDelay, buttonEnabled: false),
		new CalibrationStep(Resources.Message_VerifyingCalibration, string.Empty, CalibrationActions.VerifyCalibration, buttonEnabled: false),
		new CalibrationStep(Resources.Message_CalibrationWasSuccessful, Resources.Message_YouMayCloseThisCalibration, CalibrationActions.Complete, buttonEnabled: false)
	};

	private CalibrationStep[] steeringAngleWithTorsionBarCalibrationSteps = new CalibrationStep[20]
	{
		new CalibrationStep(Resources.Message_PressNextToBeginCalibrationProcess, string.Empty, CalibrationActions.DisplayText, buttonEnabled: true),
		new CalibrationStep(Resources.Message_ResettingCalibrationData, string.Empty, CalibrationActions.ResetCalibrationData, buttonEnabled: false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
		new CalibrationStep(Resources.Message_StartEngine, Resources.Message_ClickNextToContinueToCalibration, CalibrationActions.StartEngine, buttonEnabled: true),
		new CalibrationStep(Resources.Message_CenterSteeringWheel, Resources.Message_ClickNextToContinue, CalibrationActions.DisplayText, buttonEnabled: true),
		new CalibrationStep(Resources.Message_CenterSteeringWheel, string.Empty, CalibrationActions.ConfirmCenter, buttonEnabled: false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
		new CalibrationStep(Resources.Message_CalibratingCenter, string.Empty, CalibrationActions.CalibrateCenter, buttonEnabled: false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
		new CalibrationStep(Resources.Message_RemoveHandsFromWheelPressNextToBeginTorsionBarCalibration, string.Empty, CalibrationActions.DisplayText, buttonEnabled: true),
		new CalibrationStep(Resources.Message_CalibratingDoNotTouchSteeringWheel, string.Empty, CalibrationActions.TorsionBarCalibration, buttonEnabled: false),
		new CalibrationStep(Resources.Message_PerformingHardReset, string.Empty, CalibrationActions.HardReset, buttonEnabled: false),
		new CalibrationStep(Resources.Message_PerformingHardReset, string.Format(Resources.MessageFormat_PleaseWait0, HardResetDelayTime), CalibrationActions.HardResetDelay, buttonEnabled: false),
		new CalibrationStep(Resources.Message_VerifyingTorsionBarTorqueOffset, string.Empty, CalibrationActions.VerifyTorsionBarCalibration, buttonEnabled: false),
		new CalibrationStep(Resources.Message_SlowlyTurnTheSteeringWheelToTheLeftUntilItReachesTheEndstopAndHold, string.Empty, CalibrationActions.DisplayText, buttonEnabled: true, 0.0, 0.0, 1500.0, "(Fault),(900,Ok),(1250,Fault)"),
		new CalibrationStep(Resources.Message_CalibrateLeftEndstop, string.Empty, CalibrationActions.CalibrateLeftEndstop, buttonEnabled: false, 0.0, 0.0, 1500.0, "(Fault),(900,Ok),(1250,Fault)"),
		new CalibrationStep(Resources.Message_SlowlyTurnTheSteeringWheelToTheRightUntilItReachesTheEndstopAndHold, string.Empty, CalibrationActions.DisplayText, buttonEnabled: true, 0.0, -1500.0, 0.0, "(Fault),(-1250,Ok),(-900,Fault)"),
		new CalibrationStep(Resources.Message_CalibrateRightEndstop, string.Empty, CalibrationActions.CalibrateRightEndstop, buttonEnabled: false, 0.0, -1500.0, 0.0, "(Fault),(-1250,Ok),(-900,Fault)"),
		new CalibrationStep(Resources.Message_CenterSteeringWheel, Resources.Message_ClickNextToContinue, CalibrationActions.DisplayText, buttonEnabled: true),
		new CalibrationStep(Resources.Message_PerformingHardReset, string.Empty, CalibrationActions.HardReset, buttonEnabled: false),
		new CalibrationStep(Resources.Message_PerformingHardReset, string.Format(Resources.MessageFormat_PleaseWait0, HardResetDelayTime), CalibrationActions.HardResetDelay, buttonEnabled: false),
		new CalibrationStep(Resources.Message_VerifyingCalibration, string.Empty, CalibrationActions.VerifyCalibration, buttonEnabled: false),
		new CalibrationStep(Resources.Message_CalibrationWasSuccessful, Resources.Message_YouMayCloseThisCalibration, CalibrationActions.Complete, buttonEnabled: false)
	};

	private readonly Tuple<string, string, string>[] VrduFaultList = new Tuple<string, string, string>[41]
	{
		new Tuple<string, string, string>("VRDU02T", "524130", "9"),
		new Tuple<string, string, string>("VRDU02T", "524231", "19"),
		new Tuple<string, string, string>("VRDU02T", "524231", "9"),
		new Tuple<string, string, string>("VRDU02T", "524128", "19"),
		new Tuple<string, string, string>("VRDU02T", "524128", "9"),
		new Tuple<string, string, string>("VRDU02T", "524037", "19"),
		new Tuple<string, string, string>("VRDU02T", "524047", "19"),
		new Tuple<string, string, string>("VRDU02T", "524047", "9"),
		new Tuple<string, string, string>("VRDU02T", "524000", "19"),
		new Tuple<string, string, string>("VRDU02T", "524000", "9"),
		new Tuple<string, string, string>("VRDU02T", "524236", "19"),
		new Tuple<string, string, string>("VRDU02T", "524236", "9"),
		new Tuple<string, string, string>("VRDU02T", "524011", "19"),
		new Tuple<string, string, string>("VRDU02T", "524011", "9"),
		new Tuple<string, string, string>("VRDU02T", "524023", "19"),
		new Tuple<string, string, string>("VRDU02T", "524023", "9"),
		new Tuple<string, string, string>("VRDU02T", "523000", "12"),
		new Tuple<string, string, string>("VRDU02T", "524127", "19"),
		new Tuple<string, string, string>("VRDU02T", "524127", "9"),
		new Tuple<string, string, string>("VRDU02T", "524049", "19"),
		new Tuple<string, string, string>("VRDU02T", "524049", "9"),
		new Tuple<string, string, string>("VRDU02T", "524230", "19"),
		new Tuple<string, string, string>("VRDU02T", "524230", "9"),
		new Tuple<string, string, string>("VRDU02T", "524042", "19"),
		new Tuple<string, string, string>("VRDU02T", "524042", "9"),
		new Tuple<string, string, string>("VRDU02T", "524033", "19"),
		new Tuple<string, string, string>("VRDU02T", "524033", "9"),
		new Tuple<string, string, string>("VRDU02T", "524071", "19"),
		new Tuple<string, string, string>("VRDU02T", "524071", "9"),
		new Tuple<string, string, string>("VRDU02T", "524134", "19"),
		new Tuple<string, string, string>("VRDU02T", "524134", "9"),
		new Tuple<string, string, string>("VRDU02T", "524133", "19"),
		new Tuple<string, string, string>("VRDU02T", "524133", "9"),
		new Tuple<string, string, string>("VRDU02T", "524019", "19"),
		new Tuple<string, string, string>("VRDU02T", "524019", "9"),
		new Tuple<string, string, string>("VRDU02T", "1231", "11"),
		new Tuple<string, string, string>("VRDU02T", "1231", "3"),
		new Tuple<string, string, string>("VRDU02T", "1231", "9"),
		new Tuple<string, string, string>("VRDU02T", "639", "11"),
		new Tuple<string, string, string>("VRDU02T", "639", "3"),
		new Tuple<string, string, string>("VRDU02T", "639", "9")
	};

	private CalibrationStep calibrationFailureStep = new CalibrationStep(Resources.Message_CalibrationFailed, string.Empty, CalibrationActions.Complete, buttonEnabled: false);

	private CalibrationStep currentStep = null;

	private int currentStepIndex = 0;

	private bool monitorCurrentSteeringWheelAngle = false;

	private Channel channel;

	private Channel ssamChannel;

	private Channel absChannel;

	private Channel vrduChannel;

	private bool calibrationIncomplete = false;

	private bool busy = false;

	private bool calibrationInProgress = false;

	private bool engineStarted = false;

	private bool hasActiveFaults = false;

	private bool instrumentsAreInitialized = false;

	private Service resetService;

	private int resetCounter;

	private Timer resetTimer = null;

	private Service calibrationService;

	private Service resetCalibrationService;

	private StartMonitorStopServiceSharedProcedure torsionBarCalibrationProcedure;

	private Parameter torsionBarOffset = null;

	private TableLayoutPanel tableLayoutPanel1;

	private DialInstrument dialInstrumentCurrentStep;

	private TableLayoutPanel tableLayoutPanelInstruments;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DialInstrument dialInstrumentRightEndstop;

	private DialInstrument dialInstrument2;

	private DialInstrument dialInstrumentSteeringWheelAngle;

	private DigitalReadoutInstrument digitalReadoutInstrumentLeftEndstopCalibration;

	private DigitalReadoutInstrument digitalReadoutInstrumentRightEndstopCalibration;

	private DialInstrument dialInstrumentTorsionBarTorqueOffset;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineState;

	private TableLayoutPanel tableLayoutPanelLabels;

	private ScalingLabel scalingLabelCurrentStep;

	private ScalingLabel scalingLabelCurrentStepSubText;

	private ListViewEx listViewFaultCodes;

	private ColumnHeader columnHeaderChannel;

	private ColumnHeader columnHeaderNumber;

	private ColumnHeader columnHeaderMode;

	private ColumnHeader columnHeaderStatus;

	private TableLayoutPanel tableLayoutPanel3;

	private Button buttonReset;

	private Button buttonNext;

	private SeekTimeListView seekTimeListView1;

	public bool CalibrationIncomplete
	{
		get
		{
			return calibrationIncomplete;
		}
		private set
		{
			calibrationIncomplete = value;
			UpdateUI();
		}
	}

	public bool Busy
	{
		get
		{
			return busy;
		}
		private set
		{
			busy = value;
			UpdateUI();
		}
	}

	public bool CalibrationInProgress
	{
		get
		{
			return calibrationInProgress;
		}
		private set
		{
			calibrationInProgress = value;
			UpdateUI();
		}
	}

	public bool EngineStarted
	{
		get
		{
			return engineStarted;
		}
		private set
		{
			engineStarted = value;
			UpdateUI();
		}
	}

	public bool HasActiveFaults
	{
		get
		{
			return hasActiveFaults;
		}
		private set
		{
			hasActiveFaults = value;
			UpdateUI();
		}
	}

	public bool InstrumentsAreInitialized
	{
		get
		{
			return instrumentsAreInitialized;
		}
		private set
		{
			instrumentsAreInitialized = value;
			UpdateUI();
		}
	}

	public bool SupportsTorsionBarCalibration => channel != null && channel.Services[CalibrateTorsionBarTorqueStartQualifier] != null;

	public UserPanel()
	{
		InitializeComponent();
		scalingLabelCurrentStep.AutoSize = true;
		scalingLabelCurrentStep.MinimumSize = new Size(441, 73);
		UpdateUI();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (Busy)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			if (CalibrationIncomplete)
			{
				PerformResetCalibration(fireAndForget: true);
			}
			if (resetTimer != null)
			{
				StopHardResetTimer();
			}
			SetChannel(null, ref channel);
		}
	}

	public override void OnChannelsChanged()
	{
		SetChannel(((CustomPanel)this).GetChannel(ChannelName), ref channel);
		SetChannel(((CustomPanel)this).GetChannel(AbsName), ref absChannel);
		SetChannel(((CustomPanel)this).GetChannel(SsamName), ref ssamChannel);
		SetVrduChannel(((CustomPanel)this).GetChannel(VrduName));
		UpdateDisplayedFaults();
	}

	private void SetChannel(Channel newChannel, ref Channel channel)
	{
		if (newChannel == channel)
		{
			return;
		}
		if (channel != null)
		{
			channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		channel = newChannel;
		if (channel == null)
		{
			if (currentStep.Action != CalibrationActions.HardReset)
			{
				ResetToBeginning();
			}
		}
		else
		{
			ResetToBeginning();
		}
		if (channel != null)
		{
			channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUI();
	}

	private void ResetToBeginning()
	{
		if (channel != null)
		{
			if (SupportsTorsionBarCalibration)
			{
				calibrationSteps = steeringAngleWithTorsionBarCalibrationSteps;
			}
			else
			{
				calibrationSteps = steeringAngleCalibrationSteps;
			}
			currentStepIndex = 0;
			PerformStep();
		}
		else
		{
			currentStep = new CalibrationStep(Resources.Message_EcuIsDisconnected, string.Empty, CalibrationActions.DisplayText, buttonEnabled: false);
			UpdateUI();
		}
		CalibrationInProgress = false;
	}

	private void PerformNextStep()
	{
		if (calibrationSteps.Length > currentStepIndex)
		{
			currentStepIndex++;
		}
		else
		{
			currentStepIndex = 0;
		}
		PerformStep();
	}

	private void PerformStep()
	{
		currentStep = calibrationSteps[currentStepIndex];
		switch (currentStep.Action)
		{
		case CalibrationActions.DisplayText:
			UpdateUI();
			break;
		case CalibrationActions.ResetCalibrationData:
			PerformResetCalibration(fireAndForget: false);
			break;
		case CalibrationActions.StartEngine:
			CheckIfEngineIsStarted();
			break;
		case CalibrationActions.ConfirmCenter:
			UpdateUI();
			if (ConfirmSteeringWheelAngleIsValid())
			{
				PerformNextStep();
			}
			else
			{
				monitorCurrentSteeringWheelAngle = true;
			}
			break;
		case CalibrationActions.CalibrateCenter:
			CalibrationInProgress = true;
			PerformCenterCalibration();
			break;
		case CalibrationActions.CalibrateLeftEndstop:
			PerformEndstopCalibration(leftEndstop: true);
			break;
		case CalibrationActions.CalibrateRightEndstop:
			PerformEndstopCalibration(leftEndstop: false);
			break;
		case CalibrationActions.HardReset:
			PerformHardReset();
			break;
		case CalibrationActions.HardResetDelay:
			StartHardResetCounter();
			break;
		case CalibrationActions.VerifyCalibration:
			if (ConfirmEndstopCalibration())
			{
				PerformNextStep();
			}
			else
			{
				ReportCalibrationFailed(Resources.Message_CalibrationFailedReachedEndOfRoutineWithEndstopsNotLearned);
			}
			break;
		case CalibrationActions.TorsionBarCalibration:
			PerformTorsionBarTorqueCalibration();
			break;
		case CalibrationActions.VerifyTorsionBarCalibration:
			VerifyTorsionBarTorqueOffset();
			break;
		case CalibrationActions.Complete:
			ReportCalibrationPassed();
			break;
		case CalibrationActions.Unknown:
			ReportCalibrationFailed(Resources.Message_CalibrationFailedForUnknownReason);
			break;
		case CalibrationActions.ReportFailure:
			break;
		}
	}

	private bool IsChannelConnected(Channel channel)
	{
		return channel != null && channel.CommunicationsState == CommunicationsState.Online;
	}

	private void UpdateUI()
	{
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		bool logFileIsOpen = SapiManager.GlobalInstance.LogFileIsOpen;
		if (currentStep != null)
		{
			((Control)(object)scalingLabelCurrentStep).Text = currentStep.DisplayText;
			((Control)(object)scalingLabelCurrentStepSubText).Text = currentStep.DisplaySubText;
			bool flag = currentStep.ButtonEnabled && !logFileIsOpen && !HasActiveFaults && InstrumentsAreInitialized && IsChannelConnected(channel) && IsChannelConnected(vrduChannel) && IsChannelConnected(absChannel) && IsChannelConnected(ssamChannel);
			buttonNext.Enabled = ((currentStep.Action != CalibrationActions.StartEngine) ? flag : (flag && EngineStarted));
			string connectEcusMessage = GetConnectEcusMessage();
			if (!string.IsNullOrEmpty(connectEcusMessage))
			{
				((Control)(object)scalingLabelCurrentStep).Text = connectEcusMessage;
			}
			if (currentStep.DialEnabled)
			{
				((AxisSingleInstrumentBase)dialInstrumentCurrentStep).CustomOffset = currentStep.Offset;
				((AxisSingleInstrumentBase)dialInstrumentCurrentStep).CustomScalingFactor = 1.0;
				((AxisSingleInstrumentBase)dialInstrumentCurrentStep).PreferredAxisRange = currentStep.VisibleRange;
				((AxisSingleInstrumentBase)dialInstrumentCurrentStep).Gradient = Gradient.FromString(currentStep.GradientString);
				((Control)(object)dialInstrumentCurrentStep).Enabled = true;
				((Control)(object)dialInstrumentCurrentStep).Visible = true;
				((Control)(object)dialInstrumentCurrentStep).Refresh();
			}
			else
			{
				((Control)(object)dialInstrumentCurrentStep).Visible = false;
				((Control)(object)dialInstrumentCurrentStep).Enabled = false;
				((Control)(object)dialInstrumentCurrentStep).Refresh();
			}
		}
		else
		{
			buttonNext.Enabled = !logFileIsOpen && EngineStarted && !HasActiveFaults;
		}
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).ColumnStyles[3].Width = (SupportsTorsionBarCalibration ? 25 : 0);
		buttonReset.Enabled = !Busy && !logFileIsOpen;
	}

	private string GetConnectEcusMessage()
	{
		string text = string.Empty;
		ArrayList arrayList = new ArrayList();
		if (!IsChannelConnected(channel))
		{
			arrayList.Add(ChannelName);
		}
		if (!IsChannelConnected(absChannel))
		{
			arrayList.Add(AbsName);
		}
		if (!IsChannelConnected(ssamChannel))
		{
			arrayList.Add(SsamName);
		}
		if (!IsChannelConnected(vrduChannel))
		{
			arrayList.Add(VrduName);
		}
		if (arrayList.Count == 4)
		{
			text = string.Format(Resources.MessageFormat_Connect0123ToBeginTheCalibration, arrayList[0], arrayList[1], arrayList[2], arrayList[3]);
		}
		if (arrayList.Count == 3)
		{
			text = string.Format(Resources.MessageFormat_Connect012ToBeginTheCalibration, arrayList[0], arrayList[1], arrayList[2]);
		}
		else if (arrayList.Count == 2)
		{
			text = string.Format(Resources.MessageFormat_Connect01ToBeginTheCalibration, arrayList[0], arrayList[1]);
		}
		else if (arrayList.Count == 1)
		{
			text = string.Format(Resources.MessageFormat_Connect0ToBeginTheCalibration, arrayList[0]);
		}
		return text.Trim();
	}

	private void ReportCalibrationFailed(string reason)
	{
		ReportCalibrationFailed(reason, string.Empty);
	}

	private void ReportCalibrationFailed(string reason, string exception)
	{
		PerformResetCalibration(fireAndForget: true);
		AddLogLabel(string.Format(Resources.MessageFormat_CalibrationFailed0, reason, exception));
		currentStep.DisplayText = reason;
		currentStep.DisplaySubText = exception;
		currentStep.Action = CalibrationActions.ReportFailure;
		currentStep.DialEnabled = false;
		currentStep.ButtonEnabled = false;
		CalibrationInProgress = false;
		UpdateUI();
	}

	private void ReportCalibrationPassed()
	{
		AddLogLabel(Resources.Message_CalibrationWasSuccessful);
		CalibrationIncomplete = false;
		currentStep.DisplayText = Resources.Message_CalibrationWasSuccessful;
		currentStep.Action = CalibrationActions.Complete;
		currentStep.DialEnabled = false;
		currentStep.ButtonEnabled = false;
		CalibrationInProgress = false;
		UpdateUI();
	}

	private bool ConfirmEndstopCalibration()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		return (int)digitalReadoutInstrumentLeftEndstopCalibration.RepresentedState == 1 && (int)digitalReadoutInstrumentRightEndstopCalibration.RepresentedState == 1;
	}

	private bool ConfirmSteeringWheelAngleIsValid()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		return (int)dialInstrumentSteeringWheelAngle.RepresentedState == 1;
	}

	private int ConvertChoiceValueObjectToRawValue(ServiceOutputValue value)
	{
		Choice choice = value.Value as Choice;
		try
		{
			return Convert.ToInt32(choice.RawValue);
		}
		catch (InvalidCastException ex)
		{
			ReportCalibrationFailed(ex.Message);
		}
		return -1;
	}

	private int ReadCurrentSteeringAngle()
	{
		try
		{
			return Convert.ToInt32(channel.Instruments[SteeringAngleQualifier].InstrumentValues.Current.Value);
		}
		catch (InvalidCastException ex)
		{
			ReportCalibrationFailed(ex.Message);
		}
		catch (NullReferenceException ex2)
		{
			ReportCalibrationFailed(ex2.Message);
		}
		return int.MinValue;
	}

	private void PerformHardReset()
	{
		UpdateUI();
		resetService = channel.Services[HardResetEcuQualifier];
		if (resetService != null)
		{
			Busy = true;
			resetService.ServiceCompleteEvent += resetService_ServiceCompleteEvent;
			resetService.Execute(synchronous: false);
		}
		else
		{
			ReportCalibrationFailed(Resources.Message_UnableToPerformCalibrationMissingService);
		}
	}

	private void resetService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender as Service == resetService)
		{
			Busy = false;
			resetService.ServiceCompleteEvent -= resetService_ServiceCompleteEvent;
			if (e.Succeeded)
			{
				PerformNextStep();
			}
			else
			{
				ReportCalibrationFailed(Resources.Message_CalibrationFailedDuringEcuReset);
			}
		}
	}

	private void StartHardResetCounter()
	{
		if (resetTimer != null)
		{
			StopHardResetTimer();
		}
		resetCounter = HardResetDelayTime;
		resetTimer = new Timer();
		resetTimer.Interval = 1000;
		resetTimer.Tick += ResetTimer_Tick;
		resetTimer.Start();
	}

	private void ResetTimer_Tick(object sender, EventArgs e)
	{
		resetCounter--;
		if (resetCounter <= 0)
		{
			StopHardResetTimer();
			PerformNextStep();
		}
		else
		{
			((Control)(object)scalingLabelCurrentStepSubText).Text = string.Format(Resources.MessageFormat_PleaseWait0, resetCounter);
		}
	}

	private void StopHardResetTimer()
	{
		if (resetTimer != null)
		{
			resetTimer.Stop();
			resetTimer.Tick -= ResetTimer_Tick;
			resetTimer.Dispose();
			resetTimer = null;
		}
	}

	private void PerformCenterCalibration()
	{
		UpdateUI();
		calibrationService = channel.Services[CalibrateCenterQualifier];
		if (calibrationService != null)
		{
			CalibrationIncomplete = true;
			Busy = true;
			calibrationService.ServiceCompleteEvent += centerCalibrationService_ServiceCompleteEvent;
			calibrationService.Execute(synchronous: false);
		}
		else
		{
			ReportCalibrationFailed(Resources.Message_UnableToPerformCalibrationMissingService);
		}
	}

	private void centerCalibrationService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender as Service == calibrationService)
		{
			Busy = false;
			calibrationService.ServiceCompleteEvent -= centerCalibrationService_ServiceCompleteEvent;
			if (e.Succeeded && ConvertChoiceValueObjectToRawValue(calibrationService.OutputValues[0]) == 1)
			{
				PerformNextStep();
			}
			else
			{
				ReportCalibrationFailed(Resources.Message_CenterCalibrationServiceFailed);
			}
		}
	}

	private void PerformEndstopCalibration(bool leftEndstop)
	{
		UpdateUI();
		calibrationService = channel.Services[leftEndstop ? CalibrateLeftEndstopQualifier : CalibrateRightEndstopQualifier];
		if (calibrationService != null)
		{
			CalibrationIncomplete = true;
			Busy = true;
			calibrationService.ServiceCompleteEvent += endstopCalibrationService_ServiceCompleteEvent;
			calibrationService.Execute(synchronous: false);
		}
		else
		{
			ReportCalibrationFailed(Resources.Message_UnableToPerformCalibrationMissingService);
		}
	}

	private void endstopCalibrationService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender as Service == calibrationService)
		{
			Busy = false;
			calibrationService.ServiceCompleteEvent -= endstopCalibrationService_ServiceCompleteEvent;
			if (e.Succeeded && ConvertChoiceValueObjectToRawValue(calibrationService.OutputValues[0]) <= 1)
			{
				PerformNextStep();
			}
			else
			{
				ReportCalibrationFailed(Resources.Message_EndstopCalibrationFailed);
			}
		}
	}

	private bool PerformResetCalibration(bool fireAndForget)
	{
		UpdateUI();
		resetCalibrationService = channel.Services[ResetCalibrationDataQualifier];
		if (resetCalibrationService != null)
		{
			if (!fireAndForget)
			{
				Busy = true;
				resetCalibrationService.ServiceCompleteEvent += resetCalibrationService_ServiceCompleteEvent;
			}
			resetCalibrationService.InputValues[0].Value = resetCalibrationService.InputValues[0].Choices.GetItemFromRawValue(ResetCalibrationDataRawValue);
			resetCalibrationService.Execute(synchronous: false);
			return true;
		}
		return false;
	}

	private void resetCalibrationService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender as Service == resetCalibrationService)
		{
			Busy = false;
			resetCalibrationService.ServiceCompleteEvent -= resetCalibrationService_ServiceCompleteEvent;
			if (e.Succeeded)
			{
				PerformNextStep();
			}
			else
			{
				ReportCalibrationFailed(Resources.Message_UnableToResetCalibrationValues);
			}
		}
	}

	private void PerformTorsionBarTorqueCalibration()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		UpdateUI();
		torsionBarCalibrationProcedure = new StartMonitorStopServiceSharedProcedure("Torsion Bar Torque Calibration", "SP_TorsionBarTorqueCalibration", new ServiceCall(ChannelName, CalibrateTorsionBarTorqueStartQualifier), new ServiceCall(ChannelName, CalibrateTorsionBarTorqueResultQualifier), new ServiceCall(ChannelName, CalibrateTorsionBarTorqueResultQualifier), 1000, Gradient.FromString("(Default),(0,Fault),(1,Fault),(2,Default),(3,Default),(4,Ok),(5,Default),(6,Default),(7,Default),(8,Fault)"), true, (IEnumerable<DataItemCondition>)null, false);
		if (torsionBarCalibrationProcedure != null && ((SharedProcedureBase)torsionBarCalibrationProcedure).CanStart)
		{
			((SharedProcedureBase)torsionBarCalibrationProcedure).StartComplete += torsionBarCalibrationProcedure_StartComplete;
			((SharedProcedureBase)torsionBarCalibrationProcedure).StopComplete += torsionBarCalibrationProcedure_StopComplete;
			((SharedProcedureBase)torsionBarCalibrationProcedure).Start();
		}
	}

	private void torsionBarCalibrationProcedure_StartComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		((SharedProcedureBase)torsionBarCalibrationProcedure).StartComplete -= torsionBarCalibrationProcedure_StartComplete;
		if (!((ResultEventArgs)(object)e).Succeeded || (int)e.Result == 0)
		{
			ReportCalibrationFailed(Resources.Message_CalibrationFailedCouldNotStartTorsionBarCalibration, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty);
		}
	}

	private void torsionBarCalibrationProcedure_StopComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		((SharedProcedureBase)torsionBarCalibrationProcedure).StopComplete -= torsionBarCalibrationProcedure_StopComplete;
		if (((ResultEventArgs)(object)e).Succeeded && (int)e.Result == 1)
		{
			PerformNextStep();
		}
		else
		{
			ReportCalibrationFailed(Resources.Message_CalibrationFailedDuringTorsionBarCalibration, (((ResultEventArgs)(object)e).Exception != null) ? ((ResultEventArgs)(object)e).Exception.Message : string.Empty);
		}
	}

	private void VerifyTorsionBarTorqueOffset()
	{
		torsionBarOffset = channel.Parameters[TorsionBarTorqueOffsetParameterQualifier];
		if (torsionBarOffset != null)
		{
			channel.Parameters.ParametersReadCompleteEvent += channelParameter_ReadComplete;
			channel.Parameters.ReadAll(synchronous: false);
		}
		else
		{
			ReportCalibrationFailed(Resources.Message_CalibrationFailedCouldNotFindTorsionBarOffsetParameter);
		}
	}

	private void channelParameter_ReadComplete(object sender, ResultEventArgs e)
	{
		channel.Parameters.ParametersReadCompleteEvent -= channelParameter_ReadComplete;
		double num = double.NaN;
		if (e.Succeeded)
		{
			try
			{
				num = Convert.ToDouble(torsionBarOffset.Value);
				if (num != double.NaN && num >= -1.0 * TorsionBarRangeValue && num <= TorsionBarRangeValue)
				{
					PerformNextStep();
					return;
				}
			}
			catch (FormatException ex)
			{
				AddLogLabel(string.Format(Resources.MessageFormat_TorsionBarOffsetInvalid0, ex.Message));
			}
			catch (InvalidCastException ex2)
			{
				AddLogLabel(string.Format(Resources.MessageFormat_TorsionBarOffsetInvalid0, ex2.Message));
			}
		}
		AddLogLabel(string.Format(Resources.MessageFormat_TorsionBarOffsetValues012, e.Succeeded, torsionBarOffset.Value, num));
		ReportCalibrationFailed(Resources.Message_CalibrationFailedTorsionBarOffsetFailedToReadOrOutOfRange);
	}

	private void buttonNext_Click(object sender, EventArgs e)
	{
		PerformNextStep();
	}

	private void buttonReset_Click(object sender, EventArgs e)
	{
		ResetToBeginning();
	}

	private void dialInstrumentSteeringWheelAngle_RepresentedStateChanged(object sender, EventArgs e)
	{
		if (monitorCurrentSteeringWheelAngle && ConfirmSteeringWheelAngleIsValid())
		{
			monitorCurrentSteeringWheelAngle = false;
			PerformNextStep();
		}
	}

	private void AddLogLabel(string text)
	{
		if (text != string.Empty)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
		}
	}

	private void SetVrduChannel(Channel newChannel)
	{
		if (vrduChannel != null)
		{
			vrduChannel.FaultCodes.FaultCodesUpdateEvent -= FaultCodes_FaultCodesUpdateEvent;
			vrduChannel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		vrduChannel = newChannel;
		if (vrduChannel != null)
		{
			vrduChannel.FaultCodes.FaultCodesUpdateEvent += FaultCodes_FaultCodesUpdateEvent;
			vrduChannel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
		}
		UpdateDisplayedFaults();
	}

	private void UpdateDisplayedFaults()
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		if (!CalibrationInProgress)
		{
			bool flag = hasActiveFaults;
			listViewFaultCodes.BeginUpdate();
			((ListView)(object)listViewFaultCodes).Items.Clear();
			Tuple<string, string, string>[] vrduFaultList = VrduFaultList;
			foreach (Tuple<string, string, string> fault in vrduFaultList)
			{
				if (vrduChannel != null)
				{
					FaultCode faultCode = vrduChannel.FaultCodes.FirstOrDefault((FaultCode fc1) => fc1.FaultCodeIncidents.Count > 0 && fc1.FaultCodeIncidents.Current != null && fc1.FaultCodeIncidents.Current.Active == ActiveStatus.Active && fc1.FaultCodeIncidents.Current.FaultCode.Number.Equals(fault.Item2) && fc1.FaultCodeIncidents.Current.FaultCode.Mode.Equals(fault.Item3));
					if (faultCode != null)
					{
						ListViewExGroupItem val = new ListViewExGroupItem(new string[4]
						{
							vrduChannel.Ecu.Name,
							faultCode.Text,
							faultCode.Number,
							faultCode.Mode
						});
						((ListViewItem)(object)val).ForeColor = Color.Red;
						((ListView)(object)listViewFaultCodes).Items.Add((ListViewItem)(object)val);
					}
				}
			}
			listViewFaultCodes.EndUpdate();
			HasActiveFaults = ((ListView)(object)listViewFaultCodes).Items.Count > 0;
			if (HasActiveFaults)
			{
				currentStep = new CalibrationStep(Resources.Message_ResolveTheActiveFaultsBeforeBeginning, string.Empty, CalibrationActions.DisplayText, buttonEnabled: false);
			}
			else if (flag)
			{
				ResetToBeginning();
			}
		}
		UpdateUI();
	}

	private void CheckIfEngineIsStarted()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		EngineStarted = (int)digitalReadoutInstrumentEngineState.RepresentedState == 1;
		if (EngineStarted)
		{
			PerformNextStep();
		}
		else
		{
			UpdateUI();
		}
	}

	private void FaultCodes_FaultCodesUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateDisplayedFaults();
	}

	private void digitalReadoutInstrumentEngineState_RepresentedStateChanged(object sender, EventArgs e)
	{
		if (currentStep != null && currentStep.Action == CalibrationActions.StartEngine)
		{
			CheckIfEngineIsStarted();
		}
	}

	private void dialInstrumentTorsionBarTorqueOffset_DataChanged(object sender, EventArgs e)
	{
		if (SupportsTorsionBarCalibration && !InstrumentsAreInitialized && ((SingleInstrumentBase)dialInstrumentTorsionBarTorqueOffset).DataItem.RawValue != null)
		{
			InstrumentsAreInitialized = true;
			ResetToBeginning();
		}
	}

	private void dialInstrumentRightEndstop_DataChanged(object sender, EventArgs e)
	{
		if (!SupportsTorsionBarCalibration && !InstrumentsAreInitialized && ((SingleInstrumentBase)dialInstrumentRightEndstop).DataItem.RawValue != null)
		{
			InstrumentsAreInitialized = true;
			ResetToBeginning();
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
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0955: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e41: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanelInstruments = new TableLayoutPanel();
		dialInstrumentRightEndstop = new DialInstrument();
		dialInstrument2 = new DialInstrument();
		dialInstrumentSteeringWheelAngle = new DialInstrument();
		digitalReadoutInstrumentLeftEndstopCalibration = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentRightEndstopCalibration = new DigitalReadoutInstrument();
		dialInstrumentTorsionBarTorqueOffset = new DialInstrument();
		dialInstrumentCurrentStep = new DialInstrument();
		seekTimeListView1 = new SeekTimeListView();
		tableLayoutPanel3 = new TableLayoutPanel();
		buttonReset = new Button();
		buttonNext = new Button();
		digitalReadoutInstrumentEngineState = new DigitalReadoutInstrument();
		tableLayoutPanelLabels = new TableLayoutPanel();
		scalingLabelCurrentStep = new ScalingLabel();
		scalingLabelCurrentStepSubText = new ScalingLabel();
		listViewFaultCodes = new ListViewEx();
		columnHeaderChannel = new ColumnHeader();
		columnHeaderNumber = new ColumnHeader();
		columnHeaderMode = new ColumnHeader();
		columnHeaderStatus = new ColumnHeader();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanelInstruments).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanelLabels).SuspendLayout();
		((ISupportInitialize)listViewFaultCodes).BeginInit();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelInstruments, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)dialInstrumentCurrentStep, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentEngineState, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelLabels, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)listViewFaultCodes, 1, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelInstruments, 4);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)dialInstrumentRightEndstop, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)dialInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)dialInstrumentSteeringWheelAngle, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentLeftEndstopCalibration, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentRightEndstopCalibration, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)dialInstrumentTorsionBarTorqueOffset, 3, 0);
		((Control)(object)tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
		dialInstrumentRightEndstop.AngleRange = 180.0;
		dialInstrumentRightEndstop.AngleStart = 0.0;
		componentResourceManager.ApplyResources(dialInstrumentRightEndstop, "dialInstrumentRightEndstop");
		dialInstrumentRightEndstop.FontGroup = "DialGroup";
		((SingleInstrumentBase)dialInstrumentRightEndstop).FreezeValue = false;
		((AxisSingleInstrumentBase)dialInstrumentRightEndstop).Gradient.Initialize((ValueState)3, 2, "°");
		((AxisSingleInstrumentBase)dialInstrumentRightEndstop).Gradient.Modify(1, -1100.0, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrumentRightEndstop).Gradient.Modify(2, -800.0, (ValueState)3);
		((SingleInstrumentBase)dialInstrumentRightEndstop).Instrument = new Qualifier((QualifierTypes)1, "APS301T", "DT_Endstop_Right_Endstop");
		((Control)(object)dialInstrumentRightEndstop).Name = "dialInstrumentRightEndstop";
		((AxisSingleInstrumentBase)dialInstrumentRightEndstop).PreferredAxisRange = new AxisRange(-1200.0, 1.0, (string)null);
		((SingleInstrumentBase)dialInstrumentRightEndstop).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)dialInstrumentRightEndstop).DataChanged += dialInstrumentRightEndstop_DataChanged;
		dialInstrument2.AngleRange = 180.0;
		dialInstrument2.AngleStart = 0.0;
		componentResourceManager.ApplyResources(dialInstrument2, "dialInstrument2");
		dialInstrument2.FontGroup = "DialGroup";
		((SingleInstrumentBase)dialInstrument2).FreezeValue = false;
		((AxisSingleInstrumentBase)dialInstrument2).Gradient.Initialize((ValueState)3, 2, "°");
		((AxisSingleInstrumentBase)dialInstrument2).Gradient.Modify(1, 800.0, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrument2).Gradient.Modify(2, 1100.0, (ValueState)3);
		((SingleInstrumentBase)dialInstrument2).Instrument = new Qualifier((QualifierTypes)1, "APS301T", "DT_Endstop_Left_Endstop");
		((Control)(object)dialInstrument2).Name = "dialInstrument2";
		((AxisSingleInstrumentBase)dialInstrument2).PreferredAxisRange = new AxisRange(-1.0, 1200.0, (string)null);
		((SingleInstrumentBase)dialInstrument2).UnitAlignment = StringAlignment.Near;
		dialInstrumentSteeringWheelAngle.AngleRange = 180.0;
		dialInstrumentSteeringWheelAngle.AngleStart = 0.0;
		componentResourceManager.ApplyResources(dialInstrumentSteeringWheelAngle, "dialInstrumentSteeringWheelAngle");
		dialInstrumentSteeringWheelAngle.FontGroup = "DialGroup";
		((SingleInstrumentBase)dialInstrumentSteeringWheelAngle).FreezeValue = false;
		((AxisSingleInstrumentBase)dialInstrumentSteeringWheelAngle).Gradient.Initialize((ValueState)3, 2);
		((AxisSingleInstrumentBase)dialInstrumentSteeringWheelAngle).Gradient.Modify(1, 0.0, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrumentSteeringWheelAngle).Gradient.Modify(2, 0.5, (ValueState)3);
		((SingleInstrumentBase)dialInstrumentSteeringWheelAngle).Instrument = new Qualifier((QualifierTypes)1, "ABS02T", "DT_Steering_wheel_angle_sensor_Read_Steering_wheel_angle");
		((Control)(object)dialInstrumentSteeringWheelAngle).Name = "dialInstrumentSteeringWheelAngle";
		((AxisSingleInstrumentBase)dialInstrumentSteeringWheelAngle).PreferredAxisRange = new AxisRange(-1.0, 1.0, "");
		((SingleInstrumentBase)dialInstrumentSteeringWheelAngle).UnitAlignment = StringAlignment.Near;
		dialInstrumentSteeringWheelAngle.RepresentedStateChanged += dialInstrumentSteeringWheelAngle_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentLeftEndstopCalibration, "digitalReadoutInstrumentLeftEndstopCalibration");
		digitalReadoutInstrumentLeftEndstopCalibration.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentLeftEndstopCalibration).FreezeValue = false;
		digitalReadoutInstrumentLeftEndstopCalibration.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentLeftEndstopCalibration.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentLeftEndstopCalibration.Gradient.Modify(2, 1.0, (ValueState)5);
		digitalReadoutInstrumentLeftEndstopCalibration.Gradient.Modify(3, 2.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentLeftEndstopCalibration).Instrument = new Qualifier((QualifierTypes)1, "APS301T", "DT_Endstop_Calibration_Status_Left_Calibration_State");
		((Control)(object)digitalReadoutInstrumentLeftEndstopCalibration).Name = "digitalReadoutInstrumentLeftEndstopCalibration";
		((SingleInstrumentBase)digitalReadoutInstrumentLeftEndstopCalibration).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		digitalReadoutInstrument3.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrument3.Gradient.Modify(1, 1.0, (ValueState)1);
		digitalReadoutInstrument3.Gradient.Modify(2, 15.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "APS301T", "DT_Steering_Angle_Calibration_Status_Calibration_Status");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentRightEndstopCalibration, "digitalReadoutInstrumentRightEndstopCalibration");
		digitalReadoutInstrumentRightEndstopCalibration.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentRightEndstopCalibration).FreezeValue = false;
		digitalReadoutInstrumentRightEndstopCalibration.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentRightEndstopCalibration.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentRightEndstopCalibration.Gradient.Modify(2, 1.0, (ValueState)5);
		digitalReadoutInstrumentRightEndstopCalibration.Gradient.Modify(3, 2.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentRightEndstopCalibration).Instrument = new Qualifier((QualifierTypes)1, "APS301T", "DT_Endstop_Calibration_Status_Right_Calibration_State");
		((Control)(object)digitalReadoutInstrumentRightEndstopCalibration).Name = "digitalReadoutInstrumentRightEndstopCalibration";
		((SingleInstrumentBase)digitalReadoutInstrumentRightEndstopCalibration).UnitAlignment = StringAlignment.Near;
		dialInstrumentTorsionBarTorqueOffset.AngleRange = 180.0;
		dialInstrumentTorsionBarTorqueOffset.AngleStart = 0.0;
		componentResourceManager.ApplyResources(dialInstrumentTorsionBarTorqueOffset, "dialInstrumentTorsionBarTorqueOffset");
		dialInstrumentTorsionBarTorqueOffset.FontGroup = "DialGroup";
		((SingleInstrumentBase)dialInstrumentTorsionBarTorqueOffset).FreezeValue = false;
		((AxisSingleInstrumentBase)dialInstrumentTorsionBarTorqueOffset).Gradient.Initialize((ValueState)3, 2, "Nm");
		((AxisSingleInstrumentBase)dialInstrumentTorsionBarTorqueOffset).Gradient.Modify(1, 0.0, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrumentTorsionBarTorqueOffset).Gradient.Modify(2, 0.5, (ValueState)3);
		((SingleInstrumentBase)dialInstrumentTorsionBarTorqueOffset).Instrument = new Qualifier((QualifierTypes)4, "APS301T", "TorsionBarTorqueOffset");
		((Control)(object)dialInstrumentTorsionBarTorqueOffset).Name = "dialInstrumentTorsionBarTorqueOffset";
		((AxisSingleInstrumentBase)dialInstrumentTorsionBarTorqueOffset).PreferredAxisRange = new AxisRange(-1.0, 1.0, (string)null);
		((SingleInstrumentBase)dialInstrumentTorsionBarTorqueOffset).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)dialInstrumentTorsionBarTorqueOffset).DataChanged += dialInstrumentTorsionBarTorqueOffset_DataChanged;
		dialInstrumentCurrentStep.AngleRange = 180.0;
		dialInstrumentCurrentStep.AngleStart = 0.0;
		componentResourceManager.ApplyResources(dialInstrumentCurrentStep, "dialInstrumentCurrentStep");
		dialInstrumentCurrentStep.FontGroup = null;
		((SingleInstrumentBase)dialInstrumentCurrentStep).FreezeValue = false;
		((SingleInstrumentBase)dialInstrumentCurrentStep).Instrument = new Qualifier((QualifierTypes)1, "APS301T", "DT_Steering_Angle_Steering_Angle");
		((Control)(object)dialInstrumentCurrentStep).Name = "dialInstrumentCurrentStep";
		((AxisSingleInstrumentBase)dialInstrumentCurrentStep).PreferredAxisRange = new AxisRange(-50.0, 50.0, "");
		((SingleInstrumentBase)dialInstrumentCurrentStep).ShowValueReadout = false;
		((SingleInstrumentBase)dialInstrumentCurrentStep).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 3);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "APSCalibration";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(buttonReset, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(buttonNext, 0, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(buttonReset, "buttonReset");
		buttonReset.Name = "buttonReset";
		buttonReset.UseCompatibleTextRendering = true;
		buttonReset.UseVisualStyleBackColor = true;
		buttonReset.Click += buttonReset_Click;
		componentResourceManager.ApplyResources(buttonNext, "buttonNext");
		buttonNext.Name = "buttonNext";
		buttonNext.UseCompatibleTextRendering = true;
		buttonNext.UseVisualStyleBackColor = true;
		buttonNext.Click += buttonNext_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentEngineState, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineState, "digitalReadoutInstrumentEngineState");
		digitalReadoutInstrumentEngineState.FontGroup = "DialGroup";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).FreezeValue = false;
		digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentEngineState.Gradient.Initialize((ValueState)0, 7);
		digitalReadoutInstrumentEngineState.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentEngineState.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentEngineState.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentEngineState.Gradient.Modify(4, 3.0, (ValueState)1);
		digitalReadoutInstrumentEngineState.Gradient.Modify(5, 4.0, (ValueState)0);
		digitalReadoutInstrumentEngineState.Gradient.Modify(6, 5.0, (ValueState)0);
		digitalReadoutInstrumentEngineState.Gradient.Modify(7, 15.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_ESC_Diagnostic_Displayables_DDESC_EngineState");
		((Control)(object)digitalReadoutInstrumentEngineState).Name = "digitalReadoutInstrumentEngineState";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentEngineState.RepresentedStateChanged += digitalReadoutInstrumentEngineState_RepresentedStateChanged;
		componentResourceManager.ApplyResources(tableLayoutPanelLabels, "tableLayoutPanelLabels");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelLabels, 3);
		((TableLayoutPanel)(object)tableLayoutPanelLabels).Controls.Add((Control)(object)scalingLabelCurrentStep, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelLabels).Controls.Add((Control)(object)scalingLabelCurrentStepSubText, 0, 1);
		((Control)(object)tableLayoutPanelLabels).Name = "tableLayoutPanelLabels";
		scalingLabelCurrentStep.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanelLabels).SetColumnSpan((Control)(object)scalingLabelCurrentStep, 2);
		componentResourceManager.ApplyResources(scalingLabelCurrentStep, "scalingLabelCurrentStep");
		scalingLabelCurrentStep.FontGroup = null;
		scalingLabelCurrentStep.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelCurrentStep).Name = "scalingLabelCurrentStep";
		scalingLabelCurrentStepSubText.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanelLabels).SetColumnSpan((Control)(object)scalingLabelCurrentStepSubText, 2);
		componentResourceManager.ApplyResources(scalingLabelCurrentStepSubText, "scalingLabelCurrentStepSubText");
		scalingLabelCurrentStepSubText.FontGroup = null;
		scalingLabelCurrentStepSubText.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelCurrentStepSubText).Name = "scalingLabelCurrentStepSubText";
		listViewFaultCodes.CanDelete = false;
		((ListView)(object)listViewFaultCodes).Columns.AddRange(new ColumnHeader[4] { columnHeaderChannel, columnHeaderNumber, columnHeaderMode, columnHeaderStatus });
		componentResourceManager.ApplyResources(listViewFaultCodes, "listViewFaultCodes");
		listViewFaultCodes.EditableColumn = -1;
		((Control)(object)listViewFaultCodes).Name = "listViewFaultCodes";
		((ListView)(object)listViewFaultCodes).UseCompatibleStateImageBehavior = false;
		componentResourceManager.ApplyResources(columnHeaderChannel, "columnHeaderChannel");
		componentResourceManager.ApplyResources(columnHeaderNumber, "columnHeaderNumber");
		componentResourceManager.ApplyResources(columnHeaderMode, "columnHeaderMode");
		componentResourceManager.ApplyResources(columnHeaderStatus, "columnHeaderStatus");
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInstruments).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelLabels).ResumeLayout(performLayout: false);
		((ISupportInitialize)listViewFaultCodes).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
