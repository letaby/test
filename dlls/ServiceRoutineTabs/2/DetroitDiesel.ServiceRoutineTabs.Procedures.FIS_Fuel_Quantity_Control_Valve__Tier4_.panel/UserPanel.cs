using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Quantity_Control_Valve__Tier4_.panel;

public class UserPanel : CustomPanel
{
	private enum EngineState
	{
		Unknown = -1,
		Stopped,
		StarterEngaged,
		Idle,
		Other
	}

	private enum StarterState
	{
		Unknown = -1,
		Disabled,
		Enabled
	}

	private enum BooleanAndUnknown
	{
		False,
		True,
		Unknown
	}

	private const string MCMName = "MCM21T";

	private const string Fuel_Metering_Unit_FMU_Desired_Current_Qualifier = "DT_AS121_EU_Fuel_Metering_Unit_FMU_desired_current";

	private const string Quantity_Control_Valve_Current_Qualifier = "DT_AS100_Quantity_Control_Valve_Current";

	private const string Quantity_Control_Valve_Adaptation_Positive_Qualifier = "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Positive";

	private const string Quantity_Control_Valve_Adaptation_Negative_Qualifier = "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Negative";

	private const string CoolantTemperature_Qualifier = "DT_AS013_Coolant_Temperature";

	private const string RailPressure_Qualifier = "DT_AS043_Rail_Pressure";

	private const string FmuStickTestStatus_Qualifier = "DT_AS122_Fuel_Metering_Unit_Stick_Diagnosis_State";

	private const string FmuStickTestResult_Qualifier = "DT_AS123_Fuel_Metering_Unit_Diagnosis_Error_State";

	private const int TabControlSelectionFMUStickTest = 0;

	private const int TabControlSelectionFMUAdaptation = 1;

	private Channel channel;

	private Instrument instrumentCoolantTemperature;

	private Instrument instrumentFuelTemperature;

	private Instrument instrumentEngineSpeed;

	private Instrument instrumentEngineState;

	private Instrument instrumentRailPressure;

	private Instrument instrumentVehicleStatusCheck;

	private Instrument instrumentIgnitionState;

	private Instrument instrumentStarterSignalState;

	private Instrument instrumentValveSensorCurrent;

	private Instrument instrumentFmuStickTestStatus;

	private Instrument instrumentFmuStickTestResult;

	private Qualifier[] ambientQualifiers;

	private Qualifier[] fmuCurrentQualifiers;

	private Qualifier[] fmuStickTestQualifiers;

	private ToolTip tooltipFmuStickTestIgnitionAndEngineStatus;

	private ToolTip tooltipFmuStickTestFaultCodesStatus;

	private ToolTip tooltipFmuStickTestInstrumentsStatus;

	private ToolTip tooltipFmuStickTestVehicleCheckStatus;

	private string[] fmuFaultCodes;

	private WarningManager warningManager;

	private bool fmuTestRunning = false;

	private Timer fmuTestTimer;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrumentQuantity_Control_Valve_Current;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current;

	private DigitalReadoutInstrument digitalReadoutInstrumentCoolantTemperature;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private TableLayoutPanel tableLayoutPanelHeader;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineLoad;

	private ChartInstrument chartInstrument1;

	private CheckBox checkBox1;

	private CheckBox checkBox2;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuelMass;

	private TabControl tabControl1;

	private TabPage tabPageFmuAdaption;

	private TableLayoutPanel tableLayoutPanel2;

	private ScalingLabel scalingLabel2;

	private DigitalReadoutInstrument digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive;

	private DigitalReadoutInstrument digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative;

	private Button buttonReadFMUAdaptation;

	private Button buttonResetFMUAdaptation;

	private TabPage tabPageFmuStickTest;

	private TableLayoutPanel tableLayoutPanelFmuStickTest;

	private Button buttonFmuStickTestStop;

	private Button buttonFmuStickTestStart;

	private DigitalReadoutInstrument digitalReadoutInstrumentRailPressure;

	private Label labelFmuStickTestInstrumentsStatus;

	private Label labelFmuStickTestFaultCodesStatus;

	private Checkmark checkmarkFmuStickTestIgnitionAndEngineStatus;

	private Checkmark checkmarkInstrumentsStatus;

	private Checkmark checkmarkFmuStickTestFaultCodesStatus;

	private TableLayoutPanel tableLayoutPanel3;

	private CheckBox checkBox3;

	private Label labelFmuStickTestResultValueTitle;

	private ScalingLabel scalingLabelFmuStickTestResultValue;

	private Checkmark checkmarkFmuStickTestVehicleCheckStatus;

	private Label labelFmuStickTestVehicleCheckStatus;

	private TableLayoutPanel tableLayoutPanelFmuStickTestResults;

	private Label labelFmuStickTestIgnitionAndEngineStatus;

	private Label labelFmuStickTestStatusTitle;

	private Label labelFmuStickTestResult;

	private ScalingLabel scalingLabelFmuStickTestStatus;

	private ScalingLabel scalingLabelFmuStickTestResult;

	private SeekTimeListView seekTimeListView;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuelTemperature;

	private double EngineSpeed
	{
		get
		{
			double result = double.NaN;
			object instrumentCurrentValue = GetInstrumentCurrentValue(instrumentEngineSpeed);
			if (instrumentCurrentValue != null)
			{
				result = Convert.ToDouble(instrumentEngineSpeed.InstrumentValues.Current.Value);
			}
			return result;
		}
	}

	private EngineState EngineStatus
	{
		get
		{
			EngineState result = EngineState.Unknown;
			object instrumentCurrentValue = GetInstrumentCurrentValue(instrumentEngineState);
			if (instrumentCurrentValue != null)
			{
				result = ((instrumentCurrentValue != instrumentEngineState.Choices.GetItemFromRawValue(0)) ? ((instrumentCurrentValue == instrumentEngineState.Choices.GetItemFromRawValue(2)) ? EngineState.StarterEngaged : ((instrumentCurrentValue == instrumentEngineState.Choices.GetItemFromRawValue(3)) ? EngineState.Idle : ((instrumentCurrentValue != instrumentEngineState.Choices.GetItemFromRawValue(-1)) ? EngineState.Other : EngineState.Unknown))) : EngineState.Stopped);
			}
			return result;
		}
	}

	private BooleanAndUnknown VehicleCheckStatus
	{
		get
		{
			BooleanAndUnknown result = BooleanAndUnknown.Unknown;
			object instrumentCurrentValue = GetInstrumentCurrentValue(instrumentVehicleStatusCheck);
			if (instrumentCurrentValue != null)
			{
				if (instrumentCurrentValue == instrumentVehicleStatusCheck.Choices.GetItemFromRawValue(0))
				{
					result = BooleanAndUnknown.False;
				}
				else if (instrumentCurrentValue == instrumentVehicleStatusCheck.Choices.GetItemFromRawValue(1))
				{
					result = BooleanAndUnknown.True;
				}
			}
			return result;
		}
	}

	private bool Online => channel != null && channel.CommunicationsState == CommunicationsState.Online;

	public UserPanel()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		InitializeComponent();
		warningManager = new WarningManager(string.Empty, Resources.Message_FISFuelQuantityControlValve, seekTimeListView.RequiredUserLabelPrefix);
		tooltipFmuStickTestIgnitionAndEngineStatus = new ToolTip();
		tooltipFmuStickTestFaultCodesStatus = new ToolTip();
		tooltipFmuStickTestInstrumentsStatus = new ToolTip();
		tooltipFmuStickTestVehicleCheckStatus = new ToolTip();
		InitQualifiers();
		((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)fmuCurrentQualifiers);
		((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)fmuStickTestQualifiers);
		checkBox1.CheckStateChanged += CheckBoxCheckStateChanged;
		checkBox2.CheckStateChanged += CheckBoxCheckStateChanged;
		checkBox3.CheckStateChanged += CheckBoxCheckStateChanged;
		buttonReadFMUAdaptation.Click += OnReadFMUAdaptationClick;
		buttonResetFMUAdaptation.Click += buttonResetFMUAdaptationClick;
		tabControl1.SelectedIndexChanged += tabControl1SelectedIndexChanged;
		InitFmuStickTest();
	}

	private void InitQualifiers()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		ambientQualifiers = (Qualifier[])(object)new Qualifier[5]
		{
			new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
			new Qualifier((QualifierTypes)1, "virtual", "engineTorque"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
			new Qualifier((QualifierTypes)1, "virtual", "fuelTemp"),
			new Qualifier((QualifierTypes)1, "virtual", "coolantTemp")
		};
		fmuCurrentQualifiers = (Qualifier[])(object)new Qualifier[2]
		{
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS121_EU_Fuel_Metering_Unit_FMU_desired_current"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS100_Quantity_Control_Valve_Current")
		};
		fmuStickTestQualifiers = (Qualifier[])(object)new Qualifier[3]
		{
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS043_Rail_Pressure"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS122_Fuel_Metering_Unit_Stick_Diagnosis_State"),
			new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS123_Fuel_Metering_Unit_Diagnosis_Error_State")
		};
	}

	private static object GetInstrumentCurrentValue(Instrument instrument)
	{
		object result = null;
		if (instrument != null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
		{
			result = instrument.InstrumentValues.Current.Value;
		}
		return result;
	}

	private void InitFmuStickTest()
	{
		buttonFmuStickTestStart.MouseClick += ButtonFmuStickTestStartMouseClick;
		buttonFmuStickTestStop.MouseClick += buttonFmuStickTestStopMouseClick;
		fmuTestTimer = new Timer();
		fmuTestTimer.Interval = 2000;
		fmuTestTimer.Tick += FmuTestTimerTick;
		FmuStickTestInitFaults();
	}

	private void ExecuteService(string serviceName, ServiceCompleteEventHandler serviceCompleteEvent)
	{
		if (channel != null)
		{
			Service service = channel.Services[serviceName];
			if (service != null)
			{
				service.ServiceCompleteEvent += serviceCompleteEvent;
				service.Execute(synchronous: false);
			}
		}
	}

	private void FmuTestTimerTick(object sender, EventArgs e)
	{
		fmuTestTimer.Stop();
		if (fmuTestRunning)
		{
			FmuStickTestPollStatusCall();
			fmuTestTimer.Start();
		}
	}

	private void ButtonFmuStickTestStartMouseClick(object sender, MouseEventArgs e)
	{
		if (warningManager.RequestContinue())
		{
			FmuStickTestStart();
		}
	}

	private void FmuStickTestStart()
	{
		DialogResult dialogResult = MessageBox.Show((IWin32Window)this, Resources.Message_PressTheOKButtonToBeginTheTestWhenThisDialogClosesPleaseTurnTheIgnitionKeyToCrankTheEngineTheEngineWillCrankButNotStartWhenItStopsCrankingReleaseTheKeyNN + Resources.Message_ResultsWillBeDisplayedBelow, Resources.Message_PreparingToStartTheFMUStickTest, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
		if (dialogResult == DialogResult.OK)
		{
			FmuStickTestStartServiceCall();
		}
	}

	private void FmuStickTestStartServiceCall()
	{
		ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Start_active_status", FmuStickTestServiceStartCompleteEvent);
		FmuStickTestClearMessages();
		FmuStickTestDisplayMessage(Resources.Message_FMUStickTestStartRequested);
	}

	private void FmuStickTestServiceStartCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= FmuStickTestServiceStartCompleteEvent;
		}
		if (e.Succeeded)
		{
			ServiceOutputValue serviceOutputValue = service.OutputValues[0];
			StringBuilder stringBuilder = new StringBuilder();
			if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(1))
			{
				fmuTestRunning = true;
				fmuTestTimer.Start();
				FmuStickTestDisplayMessage(Resources.Message_RunningFMUStickTest + Resources.Message_ContinueCrankingTheEngineUntilTheTestFinishedInstrumentSaysRPG_DIA_FMU_TESTEDOrAnErrorIsDisplayed);
				return;
			}
			if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(2))
			{
				fmuTestRunning = true;
				stringBuilder.Append(Resources.Message_FMUTestIsBusyPressFMUTestStopButtonToTerminateTheTest);
				FmuStickTestDisplayMessage(stringBuilder.ToString());
				fmuTestTimer.Start();
				return;
			}
			fmuTestRunning = false;
			stringBuilder.Append(Resources.Message_TestFailedToStart);
			if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(4))
			{
				stringBuilder.Append(Resources.Message_NoIgnitionDetected + Resources.Message_StartTheTestAgainAndCrankTheEngineImmediatelyAfterSelectingOKToCloseTheDialog);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(8))
			{
				stringBuilder.Append(Resources.Message_EngineSpeedIsNotZeroOrARailPressureExists);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(16))
			{
				stringBuilder.Append(Resources.Message_TheFMUTestIsNotSupportedByThisVersionOfMCM2Software3);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(32))
			{
				stringBuilder.Append(Resources.Message_ThereIsASensorOrLeakError);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(24))
			{
				stringBuilder.Append(Resources.Message_TheFMUTestIsNotSupportedByThisVersionOfMCM2Software2);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(40))
			{
				stringBuilder.Append(Resources.Message_EngineSpeedIsNotZeroOrARailPressureExistsAndThereIsASensorOrLeakError);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(48))
			{
				stringBuilder.Append(Resources.Message_TheFMUTestIsNotSupportedByThisVersionOfMCM2Software1);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(56))
			{
				stringBuilder.Append(Resources.Message_TheFMUTestIsNotSupportedByThisVersionOfMCM2Software);
			}
			else
			{
				stringBuilder.Append(Resources.Message_UnknownResponseFromTheECU + serviceOutputValue.Value.ToString());
			}
			FmuStickTestDisplayMessage(stringBuilder.ToString());
		}
		else
		{
			FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestError0, e.Exception.Message));
		}
	}

	private void FmuStickTestPollStatusCall()
	{
		ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Request_Results_result_status", FmuStickTestPollStatusServiceCompleteEvent);
	}

	private void FmuStickTestPollStatusServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= FmuStickTestPollStatusServiceCompleteEvent;
		}
		if (e.Succeeded)
		{
			ServiceOutputValue serviceOutputValue = service.OutputValues[0];
			if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(1))
			{
				fmuTestRunning = true;
				FmuStickTestDisplayMessage(Resources.Message_TheFMUStickTestHasStarted);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(2))
			{
				fmuTestRunning = true;
				FmuStickTestDisplayMessage(Resources.Message_TheFMUStickTestIsRunning);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(4))
			{
				fmuTestRunning = false;
				FmuStickTestDisplayMessage(Resources.Message_TheFMUStickTestIsComplete);
				FmuTestRequestResults();
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(8))
			{
				fmuTestRunning = false;
				FmuStickTestDisplayMessage(Resources.Message_TheFMUStickTestWasStopped);
			}
		}
		else
		{
			fmuTestRunning = false;
			FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestError01, e.Exception.Message));
		}
	}

	private void FmuTestRequestResults()
	{
		ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Request_Results_result_error_bit", FmuTestRequestResultsResultErrorBitServiceCompleteEvent);
		ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Request_Results_result_fmu_value", FmuTestRequestResultsResultFmuValueCompleteEvent);
		fmuTestRunning = false;
	}

	private void FmuTestRequestResultsResultErrorBitServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= FmuTestRequestResultsResultErrorBitServiceCompleteEvent;
		}
		if (e.Succeeded)
		{
			ServiceOutputValue serviceOutputValue = service.OutputValues[0];
			if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(0))
			{
				scalingLabelFmuStickTestResult.RepresentedState = (ValueState)1;
				((Control)(object)scalingLabelFmuStickTestResult).Text = Resources.Message_FMUIsNOTSticking1;
				FmuStickTestDisplayMessage(Resources.Message_FMUIsNOTSticking);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(1))
			{
				scalingLabelFmuStickTestResult.RepresentedState = (ValueState)3;
				((Control)(object)scalingLabelFmuStickTestResult).Text = Resources.Message_FMUISSticking1;
				FmuStickTestDisplayMessage(Resources.Message_FMUISSticking);
			}
			else if (serviceOutputValue.Value == serviceOutputValue.Choices.GetItemFromRawValue(255))
			{
				scalingLabelFmuStickTestResult.RepresentedState = (ValueState)2;
				((Control)(object)scalingLabelFmuStickTestResult).Text = Resources.Message_FMUSignalIsNotAvailable1;
				FmuStickTestDisplayMessage(Resources.Message_FMUSignalIsNotAvailable);
			}
			else
			{
				scalingLabelFmuStickTestResult.RepresentedState = (ValueState)2;
				((Control)(object)scalingLabelFmuStickTestResult).Text = Resources.Message_FMUValueUnknown1;
				FmuStickTestDisplayMessage(Resources.Message_FMUValueUnknown);
			}
		}
		else
		{
			scalingLabelFmuStickTestResult.RepresentedState = (ValueState)2;
			((Control)(object)scalingLabelFmuStickTestResult).Text = string.Format(Resources.MessageFormat_FMUStickTestError03, e.Exception.Message);
			FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestError02, e.Exception.Message));
		}
	}

	private void FmuTestRequestResultsResultFmuValueCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= FmuTestRequestResultsResultFmuValueCompleteEvent;
		}
		if (e.Succeeded)
		{
			ServiceOutputValue serviceOutputValue = service.OutputValues[0];
			if (serviceOutputValue != null && serviceOutputValue.Value != null)
			{
				scalingLabelFmuStickTestResultValue.RepresentedState = (ValueState)1;
				((Control)(object)scalingLabelFmuStickTestResultValue).Text = $"{serviceOutputValue.Value}";
				FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestResults0MA, ObjectToDouble(serviceOutputValue.Value, "mA")));
			}
		}
		else
		{
			scalingLabelFmuStickTestResultValue.RepresentedState = (ValueState)2;
			FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestError04, e.Exception.Message));
		}
	}

	private void buttonFmuStickTestStopMouseClick(object sender, MouseEventArgs e)
	{
		if (fmuTestRunning)
		{
			ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Stop", FmuStickTestStopCompleteEvent);
		}
	}

	private void FmuStickTestStopCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= FmuStickTestStopCompleteEvent;
		}
		FmuStickTestDisplayMessage(Resources.Message_TestTerminatedByTheUser);
		fmuTestRunning = false;
	}

	private void FmuStickTestInitFaults()
	{
		fmuFaultCodes = new string[24]
		{
			"350405", "350406", "35040E", "35041F", "980D03", "980D04", "980D1F", "9A0D03", "9A0D04", "9A0D05",
			"9A0D07", "9D0002", "9D000A", "9D0012", "A40000", "A40001", "A40003", "A40004", "A40005", "A40014",
			"A40015", "A70207", "AE0003", "AE0004"
		};
	}

	private int FmuStickTestFaultsActive(ref string faultMessage)
	{
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = fmuFaultCodes;
		foreach (string code in array)
		{
			if (channel == null || channel.FaultCodes == null)
			{
				continue;
			}
			FaultCode faultCode = channel.FaultCodes[code];
			if (faultCode != null && faultCode.FaultCodeIncidents != null)
			{
				FaultCodeIncident current = faultCode.FaultCodeIncidents.Current;
				if (current != null && current.Active == ActiveStatus.Active)
				{
					num++;
					stringBuilder.AppendLine($"{faultCode.Text}: ({faultCode.Number}/{faultCode.Mode})");
				}
			}
		}
		faultMessage = stringBuilder.ToString();
		return num;
	}

	private static bool InstrumentInSpec(Instrument instrument, DigitalReadoutInstrument reference)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		bool result = false;
		object instrumentCurrentValue = GetInstrumentCurrentValue(instrument);
		if (instrumentCurrentValue != null && double.TryParse(instrumentCurrentValue.ToString(), out var result2) && !double.IsNaN(result2) && (int)reference.Gradient.GetState(result2, false) != 3)
		{
			result = true;
		}
		return result;
	}

	private void FmuStickTestUpdateEngineStatus()
	{
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		if (Online)
		{
			if (!InstrumentInSpec(instrumentEngineSpeed, digitalReadoutInstrumentEngineSpeed))
			{
				stringBuilder2.Append(Resources.Message_ErrorTheEngineIsRunningStopTheEngineButKeepIgnitionSwitchOn);
				stringBuilder.Append(Resources.Message_NotReady1);
			}
			else
			{
				stringBuilder.Append(Resources.Message_Ignition);
				((Control)(object)labelFmuStickTestIgnitionAndEngineStatus).Text = string.Empty;
				switch (EngineStatus)
				{
				case EngineState.Idle:
					stringBuilder2.Append(Resources.Message_ErrorEngineIsRunningTurnTheEngineOffButLeaveTheIginitionOn);
					stringBuilder.Append(Resources.Message_NotReady);
					break;
				case EngineState.Stopped:
					stringBuilder.Append(Resources.Message_Ready);
					flag = true;
					break;
				case EngineState.StarterEngaged:
					stringBuilder2.Append(Resources.Message_ErrorTheStarterIsEnagedTurnTheKeySoThatTheIgnitionIsOnButNotStartingTheEngine);
					stringBuilder.Append(Resources.Message_NotReady3);
					break;
				case EngineState.Unknown:
					stringBuilder2.Append(Resources.Message_ErrorTheIgnitionStateOrTheEngineSpeedAreNotKnown);
					stringBuilder.Append(Resources.Message_Unknown);
					break;
				case EngineState.Other:
					stringBuilder2.Append(Resources.Message_ErrorTheEngineIsNotStoppedTurnTheKeySoThatTheIgnitionIsOnButNotStartingTheEngine);
					stringBuilder.Append(Resources.Message_NotReady2);
					break;
				default:
					throw new IndexOutOfRangeException("Undefined engine state");
				}
			}
		}
		else
		{
			flag = false;
			stringBuilder.Append(Resources.Message_Offline);
		}
		checkmarkFmuStickTestIgnitionAndEngineStatus.Checked = flag;
		((Control)(object)labelFmuStickTestIgnitionAndEngineStatus).Text = stringBuilder.ToString();
		tooltipFmuStickTestIgnitionAndEngineStatus.SetToolTip((Control)(object)labelFmuStickTestIgnitionAndEngineStatus, stringBuilder2.ToString());
		FmuStickTestUpdateButtons();
	}

	private void FmuStickTestUpdateVehicleCheckStatus()
	{
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder(Resources.Message_VehicleCheck);
		StringBuilder stringBuilder2 = new StringBuilder();
		if (Online)
		{
			((Control)(object)labelFmuStickTestVehicleCheckStatus).Text = string.Empty;
			switch (VehicleCheckStatus)
			{
			case BooleanAndUnknown.True:
				stringBuilder.Append(Resources.Message_Ready1);
				flag = true;
				break;
			case BooleanAndUnknown.False:
			case BooleanAndUnknown.Unknown:
				stringBuilder.Append(Resources.Message_NotReady4);
				stringBuilder2.Append(Resources.Message_ErrorTheTransmissionMustBeInNuetralAndTheParkingBrakeMustBeON);
				flag = false;
				break;
			default:
				throw new IndexOutOfRangeException("New boolean and unknown status error.");
			}
		}
		else
		{
			flag = false;
			stringBuilder.Append(Resources.Message_Offline1);
		}
		checkmarkFmuStickTestVehicleCheckStatus.Checked = flag;
		((Control)(object)labelFmuStickTestVehicleCheckStatus).Text = stringBuilder.ToString();
		tooltipFmuStickTestVehicleCheckStatus.SetToolTip((Control)(object)labelFmuStickTestVehicleCheckStatus, stringBuilder2.ToString());
		FmuStickTestUpdateButtons();
	}

	private void FmuStickTestUpdateInstrumentsStatus()
	{
		bool flag = false;
		bool flag2 = true;
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		string text;
		if (Online)
		{
			stringBuilder.AppendLine(Resources.Message_InstrumentsNotReady);
			if (!InstrumentInSpec(instrumentFuelTemperature, digitalReadoutInstrumentFuelTemperature))
			{
				stringBuilder.AppendLine(string.Format(Resources.MessageFormat_0IsTooLow, instrumentFuelTemperature.Name));
				num++;
				flag2 = false;
			}
			if (!InstrumentInSpec(instrumentRailPressure, digitalReadoutInstrumentRailPressure))
			{
				stringBuilder.AppendLine(string.Format(Resources.MessageFormat_0IsTooHigh, instrumentRailPressure.Name));
				num++;
				flag2 = false;
			}
			if (flag2)
			{
				stringBuilder.Length = 0;
				text = Resources.Message_InstrumentsReady;
			}
			else
			{
				text = string.Format(Resources.MessageFormat_0Instrument1NotReady, num, (num >= 2) ? "s" : "");
			}
		}
		else
		{
			flag2 = false;
			text = Resources.Message_Offline2;
		}
		((Control)(object)labelFmuStickTestInstrumentsStatus).Text = text;
		checkmarkInstrumentsStatus.Checked = flag2;
		tooltipFmuStickTestInstrumentsStatus.SetToolTip((Control)(object)labelFmuStickTestInstrumentsStatus, stringBuilder.ToString());
		FmuStickTestUpdateButtons();
	}

	private void OnIgnitionUpdateEvent(object sender, ResultEventArgs e)
	{
		FmuStickTestUpdateEngineStatus();
	}

	private void OnVehicleCheckUpdateEvent(object sender, ResultEventArgs e)
	{
		FmuStickTestUpdateVehicleCheckStatus();
	}

	private void OnInstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		FmuStickTestUpdateInstrumentsStatus();
	}

	private void FmuStickTestUpdateFaultConditions()
	{
		if (Online)
		{
			string faultMessage = string.Empty;
			int num = FmuStickTestFaultsActive(ref faultMessage);
			checkmarkFmuStickTestFaultCodesStatus.Checked = num == 0;
			if (num > 0)
			{
				((Control)(object)labelFmuStickTestFaultCodesStatus).Text = string.Format(Resources.MessageFormat_0FaultCode1, num, (num >= 2) ? "s" : "");
				tooltipFmuStickTestFaultCodesStatus.SetToolTip((Control)(object)labelFmuStickTestFaultCodesStatus, faultMessage);
			}
			else
			{
				((Control)(object)labelFmuStickTestFaultCodesStatus).Text = Resources.Message_NoActiveFaults;
			}
		}
		else
		{
			((Control)(object)labelFmuStickTestFaultCodesStatus).Text = Resources.Message_Offline3;
			checkmarkFmuStickTestFaultCodesStatus.Checked = false;
		}
		FmuStickTestUpdateButtons();
	}

	private void FmuStickTestUpdateButtons()
	{
		bool flag = false;
		bool enabled = fmuTestRunning;
		if (!Online)
		{
			flag = false;
			enabled = false;
		}
		else
		{
			flag = checkmarkInstrumentsStatus.Checked && checkmarkFmuStickTestIgnitionAndEngineStatus.Checked && checkmarkFmuStickTestVehicleCheckStatus.Checked && checkmarkFmuStickTestFaultCodesStatus.Checked;
		}
		buttonFmuStickTestStart.Enabled = flag;
		buttonFmuStickTestStop.Enabled = enabled;
	}

	private void FmuStickTestDisplayMessage(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
		((CustomPanel)this).AddStatusMessage(text);
	}

	private void FmuStickTestClearMessages()
	{
		((Control)(object)scalingLabelFmuStickTestResult).Text = string.Empty;
		((Control)(object)scalingLabelFmuStickTestStatus).Text = string.Empty;
		((Control)(object)scalingLabelFmuStickTestResultValue).Text = string.Empty;
		scalingLabelFmuStickTestStatus.RepresentedState = (ValueState)0;
		scalingLabelFmuStickTestResult.RepresentedState = (ValueState)0;
		scalingLabelFmuStickTestResultValue.RepresentedState = (ValueState)0;
	}

	private void buttonResetFMUAdaptationClick(object sender, EventArgs e)
	{
		Service service = channel.Services["RT_SR014_SET_EOL_Default_Values_Start"];
		if (service != null)
		{
			service.ServiceCompleteEvent += OnServiceComplete;
			if (service.InputValues.Count > 0)
			{
				service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(25);
			}
			CustomPanel.ExecuteService(service);
		}
	}

	private void OnReadFMUAdaptationClick(object sender, EventArgs e)
	{
		ReadStoredData();
	}

	private static double ObjectToDouble(object value, string units)
	{
		double num = double.NaN;
		if (value != null)
		{
			Choice choice = value as Choice;
			if (choice != null)
			{
				num = Convert.ToDouble(choice.RawValue);
			}
			else
			{
				try
				{
					num = Convert.ToDouble(value);
					Conversion conversion = Converter.GlobalInstance.GetConversion(units);
					if (conversion != null)
					{
						num = conversion.Convert(num);
					}
				}
				catch (InvalidCastException)
				{
					num = double.NaN;
				}
				catch (FormatException)
				{
					num = double.NaN;
				}
			}
		}
		return num;
	}

	private void CheckBoxCheckStateChanged(object sender, EventArgs e)
	{
		((Collection<Qualifier>)(object)chartInstrument1.Instruments).Clear();
		if (checkBox1.Checked)
		{
			((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)ambientQualifiers);
		}
		if (checkBox2.Checked)
		{
			((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)fmuCurrentQualifiers);
		}
		if (checkBox3.Checked)
		{
			((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)fmuStickTestQualifiers);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			SetChannel(null);
		}
	}

	public override void OnChannelsChanged()
	{
		SetChannel(((CustomPanel)this).GetChannel("MCM21T"));
		warningManager.Reset();
	}

	private static Parameter DisconnectParameter(ref Parameter parameter, ParameterUpdateEventHandler updateEventHandler)
	{
		if (parameter != null)
		{
			parameter.ParameterUpdateEvent -= updateEventHandler;
		}
		return null;
	}

	private static Parameter ConnectParameter(ParameterCollection parameters, string parameterQualifier, ParameterUpdateEventHandler updateEventHandler)
	{
		Parameter parameter = parameters[parameterQualifier];
		if (parameter != null)
		{
			parameter.ParameterUpdateEvent += updateEventHandler;
		}
		return parameter;
	}

	private static Instrument DisconnectInstrument(Instrument instrument, InstrumentUpdateEventHandler updateEventHandler)
	{
		if (instrument != null)
		{
			instrument.InstrumentUpdateEvent -= updateEventHandler;
		}
		return null;
	}

	private static Instrument ConnectInstrument(InstrumentCollection instruments, string qualifier, InstrumentUpdateEventHandler updateEventHandler)
	{
		Instrument instrument = instruments[qualifier];
		if (instrument != null)
		{
			instrument.InstrumentUpdateEvent += updateEventHandler;
		}
		return instrument;
	}

	private void SetChannel(Channel channel)
	{
		if (this.channel != channel)
		{
			if (this.channel != null)
			{
				instrumentIgnitionState = DisconnectInstrument(instrumentIgnitionState, OnIgnitionUpdateEvent);
				instrumentEngineState = DisconnectInstrument(instrumentEngineState, OnIgnitionUpdateEvent);
				instrumentStarterSignalState = DisconnectInstrument(instrumentStarterSignalState, OnIgnitionUpdateEvent);
				instrumentEngineSpeed = DisconnectInstrument(instrumentEngineSpeed, OnIgnitionUpdateEvent);
				instrumentVehicleStatusCheck = DisconnectInstrument(instrumentVehicleStatusCheck, OnVehicleCheckUpdateEvent);
				instrumentCoolantTemperature = DisconnectInstrument(instrumentCoolantTemperature, OnInstrumentUpdateEvent);
				instrumentFuelTemperature = DisconnectInstrument(instrumentFuelTemperature, OnInstrumentUpdateEvent);
				instrumentRailPressure = DisconnectInstrument(instrumentRailPressure, OnInstrumentUpdateEvent);
				instrumentValveSensorCurrent = DisconnectInstrument(instrumentValveSensorCurrent, OnInstrumentUpdateEvent);
				instrumentFmuStickTestStatus = DisconnectInstrument(instrumentFmuStickTestStatus, OnFmuStickTestStatusUpdateEvent);
				instrumentFmuStickTestResult = DisconnectInstrument(instrumentFmuStickTestResult, OnFmuStickTestResultUpdateEvent);
				this.channel.FaultCodes.FaultCodesUpdateEvent -= FaultCodesFaultCodesUpdateEvent;
				this.channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			this.channel = channel;
			if (this.channel != null)
			{
				instrumentIgnitionState = ConnectInstrument(this.channel.Instruments, "DT_DS003_CPC2_CAN_Ignition_Status", OnIgnitionUpdateEvent);
				instrumentStarterSignalState = ConnectInstrument(this.channel.Instruments, "DT_DS003_MCM_wired_Starter_Signal_Status", OnIgnitionUpdateEvent);
				instrumentEngineState = ConnectInstrument(this.channel.Instruments, "DT_AS023_Engine_State", OnIgnitionUpdateEvent);
				instrumentEngineSpeed = ConnectInstrument(this.channel.Instruments, "DT_AS010_Engine_Speed", OnIgnitionUpdateEvent);
				instrumentVehicleStatusCheck = ConnectInstrument(this.channel.Instruments, "DT_DS019_Vehicle_Check_Status", OnVehicleCheckUpdateEvent);
				instrumentCoolantTemperature = ConnectInstrument(this.channel.Instruments, "DT_AS013_Coolant_Temperature", OnInstrumentUpdateEvent);
				instrumentFuelTemperature = ConnectInstrument(this.channel.Instruments, "DT_AS014_Fuel_Temperature", OnInstrumentUpdateEvent);
				instrumentRailPressure = ConnectInstrument(this.channel.Instruments, "DT_AS043_Rail_Pressure", OnInstrumentUpdateEvent);
				instrumentValveSensorCurrent = ConnectInstrument(this.channel.Instruments, "DT_AS100_Quantity_Control_Valve_Current", OnInstrumentUpdateEvent);
				instrumentFmuStickTestStatus = ConnectInstrument(this.channel.Instruments, "DT_AS122_Fuel_Metering_Unit_Stick_Diagnosis_State", OnFmuStickTestStatusUpdateEvent);
				instrumentFmuStickTestResult = ConnectInstrument(this.channel.Instruments, "DT_AS123_Fuel_Metering_Unit_Diagnosis_Error_State", OnFmuStickTestResultUpdateEvent);
				this.channel.FaultCodes.FaultCodesUpdateEvent += FaultCodesFaultCodesUpdateEvent;
				this.channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				FmuStickTestClearMessages();
				ReadStoredData();
				UpdateUserInterface();
			}
		}
	}

	private void OnFmuStickTestStatusUpdateEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= OnFmuStickTestStatusUpdateEvent;
		}
		if (!e.Succeeded)
		{
			return;
		}
		string text = string.Empty;
		object instrumentCurrentValue = GetInstrumentCurrentValue(instrumentFmuStickTestStatus);
		if (instrumentCurrentValue != null && instrumentFmuStickTestStatus.Choices != null && instrumentFmuStickTestStatus.Choices.Count >= 4)
		{
			text = Resources.Message_Test;
			if (instrumentCurrentValue == instrumentFmuStickTestStatus.Choices.GetItemFromRawValue(1))
			{
				text += Resources.Message_Started;
				scalingLabelFmuStickTestStatus.RepresentedState = (ValueState)0;
			}
			else if (instrumentCurrentValue == instrumentFmuStickTestStatus.Choices.GetItemFromRawValue(2))
			{
				text += Resources.Message_Running;
				scalingLabelFmuStickTestStatus.RepresentedState = (ValueState)0;
			}
			else if (instrumentCurrentValue == instrumentFmuStickTestStatus.Choices.GetItemFromRawValue(4))
			{
				text += Resources.Message_Complete;
				scalingLabelFmuStickTestStatus.RepresentedState = (ValueState)1;
			}
			else if (instrumentCurrentValue == instrumentFmuStickTestStatus.Choices.GetItemFromRawValue(8))
			{
				text += Resources.Message_Stopped;
				scalingLabelFmuStickTestStatus.RepresentedState = (ValueState)0;
			}
		}
		((Control)(object)scalingLabelFmuStickTestStatus).Text = text;
	}

	private void OnFmuStickTestResultUpdateEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= OnFmuStickTestResultUpdateEvent;
		}
		if (!e.Succeeded)
		{
			return;
		}
		string text = string.Empty;
		object instrumentCurrentValue = GetInstrumentCurrentValue(instrumentFmuStickTestResult);
		if (instrumentCurrentValue != null && instrumentFmuStickTestResult.Choices != null && instrumentFmuStickTestResult.Choices.Count >= 3)
		{
			if (instrumentCurrentValue == instrumentFmuStickTestResult.Choices.GetItemFromRawValue(0))
			{
				text += Resources.Message_Passed;
				scalingLabelFmuStickTestResult.RepresentedState = (ValueState)1;
			}
			else if (instrumentCurrentValue == instrumentFmuStickTestResult.Choices.GetItemFromRawValue(1))
			{
				text += Resources.Message_Failed;
				scalingLabelFmuStickTestResult.RepresentedState = (ValueState)3;
			}
			else if (instrumentCurrentValue == instrumentFmuStickTestResult.Choices.GetItemFromRawValue(255))
			{
				text += Resources.Message_Unknown1;
				scalingLabelFmuStickTestResult.RepresentedState = (ValueState)2;
			}
		}
		((Control)(object)scalingLabelFmuStickTestResult).Text = text;
	}

	private void FaultCodesFaultCodesUpdateEvent(object sender, ResultEventArgs e)
	{
		if (tabControl1.SelectedIndex == 0)
		{
			FmuStickTestUpdateFaultConditions();
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		if (tabControl1.SelectedIndex == 0)
		{
			UpdateFmuStickTestInterface();
		}
		else
		{
			UpdateFmuAdaptationInterface();
		}
	}

	private void UpdateFmuStickTestInterface()
	{
		FmuStickTestUpdateEngineStatus();
		FmuStickTestUpdateFaultConditions();
		FmuStickTestUpdateInstrumentsStatus();
	}

	private void UpdateFmuAdaptationInterface()
	{
		buttonReadFMUAdaptation.Enabled = Online;
		buttonResetFMUAdaptation.Enabled = Online;
	}

	private void tabControl1SelectedIndexChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void ReadStoredData()
	{
		ReadEcuInfo(channel.EcuInfos["DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Positive"]);
		ReadEcuInfo(channel.EcuInfos["DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Negative"]);
	}

	private void ReadParameter(Parameter parameter)
	{
		if (parameter != null && parameter.Channel.Online)
		{
			string groupQualifier = parameter.GroupQualifier;
			parameter.Channel.Parameters.ReadGroup(groupQualifier, fromCache: false, synchronous: false);
		}
	}

	private void ReadEcuInfo(EcuInfo ecuInfo)
	{
		if (ecuInfo != null && ecuInfo.Channel.Online)
		{
			ecuInfo.Channel.EcuInfos[ecuInfo.Qualifier].Read(synchronous: false);
		}
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		if (channel != null)
		{
			Service service = sender as Service;
			if (service != null)
			{
				service.ServiceCompleteEvent -= OnServiceComplete;
			}
			else
			{
				channel.Services.ServiceCompleteEvent -= OnServiceComplete;
			}
		}
		ReadStoredData();
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
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
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fff: Unknown result type (might be due to invalid IL or missing references)
		//IL_109d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1107: Unknown result type (might be due to invalid IL or missing references)
		//IL_122a: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_146d: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel3 = new TableLayoutPanel();
		checkBox1 = new CheckBox();
		checkBox2 = new CheckBox();
		checkBox3 = new CheckBox();
		tableLayoutPanel2 = new TableLayoutPanel();
		scalingLabel2 = new ScalingLabel();
		digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive = new DigitalReadoutInstrument();
		digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative = new DigitalReadoutInstrument();
		buttonReadFMUAdaptation = new Button();
		buttonResetFMUAdaptation = new Button();
		tableLayoutPanelFmuStickTestResults = new TableLayoutPanel();
		labelFmuStickTestResultValueTitle = new Label();
		scalingLabelFmuStickTestResultValue = new ScalingLabel();
		labelFmuStickTestStatusTitle = new Label();
		labelFmuStickTestResult = new Label();
		scalingLabelFmuStickTestStatus = new ScalingLabel();
		scalingLabelFmuStickTestResult = new ScalingLabel();
		tableLayoutPanelFmuStickTest = new TableLayoutPanel();
		seekTimeListView = new SeekTimeListView();
		checkmarkFmuStickTestFaultCodesStatus = new Checkmark();
		labelFmuStickTestFaultCodesStatus = new Label();
		labelFmuStickTestInstrumentsStatus = new Label();
		checkmarkInstrumentsStatus = new Checkmark();
		checkmarkFmuStickTestIgnitionAndEngineStatus = new Checkmark();
		digitalReadoutInstrumentRailPressure = new DigitalReadoutInstrument();
		checkmarkFmuStickTestVehicleCheckStatus = new Checkmark();
		labelFmuStickTestVehicleCheckStatus = new Label();
		buttonFmuStickTestStart = new Button();
		buttonFmuStickTestStop = new Button();
		labelFmuStickTestIgnitionAndEngineStatus = new Label();
		tableLayoutPanelHeader = new TableLayoutPanel();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineLoad = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFuelTemperature = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFuelMass = new DigitalReadoutInstrument();
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current = new DigitalReadoutInstrument();
		digitalReadoutInstrumentQuantity_Control_Valve_Current = new DigitalReadoutInstrument();
		tabControl1 = new TabControl();
		tabPageFmuStickTest = new TabPage();
		tabPageFmuAdaption = new TabPage();
		chartInstrument1 = new ChartInstrument();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanelFmuStickTestResults).SuspendLayout();
		((Control)(object)tableLayoutPanelFmuStickTest).SuspendLayout();
		((Control)(object)tableLayoutPanelHeader).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		tabControl1.SuspendLayout();
		tabPageFmuStickTest.SuspendLayout();
		tabPageFmuAdaption.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(checkBox1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(checkBox2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(checkBox3, 2, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(checkBox1, "checkBox1");
		checkBox1.Name = "checkBox1";
		checkBox1.UseCompatibleTextRendering = true;
		checkBox1.UseVisualStyleBackColor = true;
		checkBox2.Checked = true;
		checkBox2.CheckState = CheckState.Checked;
		componentResourceManager.ApplyResources(checkBox2, "checkBox2");
		checkBox2.Name = "checkBox2";
		checkBox2.UseCompatibleTextRendering = true;
		checkBox2.UseVisualStyleBackColor = true;
		checkBox3.Checked = true;
		checkBox3.CheckState = CheckState.Checked;
		componentResourceManager.ApplyResources(checkBox3, "checkBox3");
		checkBox3.Name = "checkBox3";
		checkBox3.UseCompatibleTextRendering = true;
		checkBox3.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)scalingLabel2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonReadFMUAdaptation, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonResetFMUAdaptation, 1, 3);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		scalingLabel2.Alignment = StringAlignment.Center;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)scalingLabel2, 2);
		componentResourceManager.ApplyResources(scalingLabel2, "scalingLabel2");
		scalingLabel2.FontGroup = "TestTitle";
		scalingLabel2.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel2).Name = "scalingLabel2";
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive, "digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive");
		digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive).Instrument = new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Positive");
		((Control)(object)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive).Name = "digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive";
		((SingleInstrumentBase)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative, "digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative");
		digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative).Instrument = new Qualifier((QualifierTypes)8, "MCM21T", "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Negative");
		((Control)(object)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative).Name = "digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative";
		((SingleInstrumentBase)digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonReadFMUAdaptation, "buttonReadFMUAdaptation");
		buttonReadFMUAdaptation.Name = "buttonReadFMUAdaptation";
		buttonReadFMUAdaptation.UseCompatibleTextRendering = true;
		buttonReadFMUAdaptation.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonResetFMUAdaptation, "buttonResetFMUAdaptation");
		buttonResetFMUAdaptation.Name = "buttonResetFMUAdaptation";
		buttonResetFMUAdaptation.UseCompatibleTextRendering = true;
		buttonResetFMUAdaptation.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanelFmuStickTestResults, "tableLayoutPanelFmuStickTestResults");
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).SetColumnSpan((Control)(object)tableLayoutPanelFmuStickTestResults, 4);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTestResults).Controls.Add((Control)(object)labelFmuStickTestResultValueTitle, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTestResults).Controls.Add((Control)(object)scalingLabelFmuStickTestResultValue, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTestResults).Controls.Add((Control)(object)labelFmuStickTestStatusTitle, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTestResults).Controls.Add((Control)(object)labelFmuStickTestResult, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTestResults).Controls.Add((Control)(object)scalingLabelFmuStickTestStatus, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTestResults).Controls.Add((Control)(object)scalingLabelFmuStickTestResult, 0, 3);
		((Control)(object)tableLayoutPanelFmuStickTestResults).Name = "tableLayoutPanelFmuStickTestResults";
		labelFmuStickTestResultValueTitle.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFmuStickTestResultValueTitle, "labelFmuStickTestResultValueTitle");
		((Control)(object)labelFmuStickTestResultValueTitle).Name = "labelFmuStickTestResultValueTitle";
		labelFmuStickTestResultValueTitle.Orientation = (TextOrientation)1;
		scalingLabelFmuStickTestResultValue.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelFmuStickTestResultValue, "scalingLabelFmuStickTestResultValue");
		scalingLabelFmuStickTestResultValue.FontGroup = "TestResults";
		scalingLabelFmuStickTestResultValue.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelFmuStickTestResultValue).Name = "scalingLabelFmuStickTestResultValue";
		labelFmuStickTestStatusTitle.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFmuStickTestStatusTitle, "labelFmuStickTestStatusTitle");
		((Control)(object)labelFmuStickTestStatusTitle).Name = "labelFmuStickTestStatusTitle";
		labelFmuStickTestStatusTitle.Orientation = (TextOrientation)1;
		labelFmuStickTestResult.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFmuStickTestResult, "labelFmuStickTestResult");
		((Control)(object)labelFmuStickTestResult).Name = "labelFmuStickTestResult";
		labelFmuStickTestResult.Orientation = (TextOrientation)1;
		scalingLabelFmuStickTestStatus.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelFmuStickTestStatus, "scalingLabelFmuStickTestStatus");
		scalingLabelFmuStickTestStatus.FontGroup = "TestResults";
		scalingLabelFmuStickTestStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelFmuStickTestStatus).Name = "scalingLabelFmuStickTestStatus";
		scalingLabelFmuStickTestResult.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelFmuStickTestResult, "scalingLabelFmuStickTestResult");
		scalingLabelFmuStickTestResult.FontGroup = "TestResults";
		scalingLabelFmuStickTestResult.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelFmuStickTestResult).Name = "scalingLabelFmuStickTestResult";
		componentResourceManager.ApplyResources(tableLayoutPanelFmuStickTest, "tableLayoutPanelFmuStickTest");
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)seekTimeListView, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)checkmarkFmuStickTestFaultCodesStatus, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)labelFmuStickTestFaultCodesStatus, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)labelFmuStickTestInstrumentsStatus, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)checkmarkInstrumentsStatus, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)checkmarkFmuStickTestIgnitionAndEngineStatus, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)digitalReadoutInstrumentRailPressure, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)checkmarkFmuStickTestVehicleCheckStatus, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)labelFmuStickTestVehicleCheckStatus, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add(buttonFmuStickTestStart, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add(buttonFmuStickTestStop, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)tableLayoutPanelFmuStickTestResults, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).Controls.Add((Control)(object)labelFmuStickTestIgnitionAndEngineStatus, 1, 1);
		((Control)(object)tableLayoutPanelFmuStickTest).Name = "tableLayoutPanelFmuStickTest";
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).SetColumnSpan((Control)(object)seekTimeListView, 4);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Fis Fuel Quanity Control Valve";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		componentResourceManager.ApplyResources(checkmarkFmuStickTestFaultCodesStatus, "checkmarkFmuStickTestFaultCodesStatus");
		((Control)(object)checkmarkFmuStickTestFaultCodesStatus).Name = "checkmarkFmuStickTestFaultCodesStatus";
		labelFmuStickTestFaultCodesStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFmuStickTestFaultCodesStatus, "labelFmuStickTestFaultCodesStatus");
		((Control)(object)labelFmuStickTestFaultCodesStatus).Name = "labelFmuStickTestFaultCodesStatus";
		labelFmuStickTestFaultCodesStatus.Orientation = (TextOrientation)1;
		labelFmuStickTestFaultCodesStatus.ShowBorder = false;
		labelFmuStickTestFaultCodesStatus.UseSystemColors = true;
		labelFmuStickTestInstrumentsStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFmuStickTestInstrumentsStatus, "labelFmuStickTestInstrumentsStatus");
		((Control)(object)labelFmuStickTestInstrumentsStatus).Name = "labelFmuStickTestInstrumentsStatus";
		labelFmuStickTestInstrumentsStatus.Orientation = (TextOrientation)1;
		labelFmuStickTestInstrumentsStatus.ShowBorder = false;
		labelFmuStickTestInstrumentsStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(checkmarkInstrumentsStatus, "checkmarkInstrumentsStatus");
		((Control)(object)checkmarkInstrumentsStatus).Name = "checkmarkInstrumentsStatus";
		componentResourceManager.ApplyResources(checkmarkFmuStickTestIgnitionAndEngineStatus, "checkmarkFmuStickTestIgnitionAndEngineStatus");
		((Control)(object)checkmarkFmuStickTestIgnitionAndEngineStatus).Name = "checkmarkFmuStickTestIgnitionAndEngineStatus";
		((TableLayoutPanel)(object)tableLayoutPanelFmuStickTest).SetColumnSpan((Control)(object)digitalReadoutInstrumentRailPressure, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentRailPressure, "digitalReadoutInstrumentRailPressure");
		digitalReadoutInstrumentRailPressure.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentRailPressure).FreezeValue = false;
		digitalReadoutInstrumentRailPressure.Gradient.Initialize((ValueState)1, 1, "bar");
		digitalReadoutInstrumentRailPressure.Gradient.Modify(1, 10.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentRailPressure).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS043_Rail_Pressure");
		((Control)(object)digitalReadoutInstrumentRailPressure).Name = "digitalReadoutInstrumentRailPressure";
		((SingleInstrumentBase)digitalReadoutInstrumentRailPressure).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(checkmarkFmuStickTestVehicleCheckStatus, "checkmarkFmuStickTestVehicleCheckStatus");
		((Control)(object)checkmarkFmuStickTestVehicleCheckStatus).Name = "checkmarkFmuStickTestVehicleCheckStatus";
		labelFmuStickTestVehicleCheckStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFmuStickTestVehicleCheckStatus, "labelFmuStickTestVehicleCheckStatus");
		((Control)(object)labelFmuStickTestVehicleCheckStatus).Name = "labelFmuStickTestVehicleCheckStatus";
		labelFmuStickTestVehicleCheckStatus.Orientation = (TextOrientation)1;
		labelFmuStickTestVehicleCheckStatus.ShowBorder = false;
		labelFmuStickTestVehicleCheckStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(buttonFmuStickTestStart, "buttonFmuStickTestStart");
		buttonFmuStickTestStart.Name = "buttonFmuStickTestStart";
		buttonFmuStickTestStart.UseCompatibleTextRendering = true;
		buttonFmuStickTestStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonFmuStickTestStop, "buttonFmuStickTestStop");
		buttonFmuStickTestStop.Name = "buttonFmuStickTestStop";
		buttonFmuStickTestStop.UseCompatibleTextRendering = true;
		buttonFmuStickTestStop.UseVisualStyleBackColor = true;
		labelFmuStickTestIgnitionAndEngineStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFmuStickTestIgnitionAndEngineStatus, "labelFmuStickTestIgnitionAndEngineStatus");
		((Control)(object)labelFmuStickTestIgnitionAndEngineStatus).Name = "labelFmuStickTestIgnitionAndEngineStatus";
		labelFmuStickTestIgnitionAndEngineStatus.Orientation = (TextOrientation)1;
		labelFmuStickTestIgnitionAndEngineStatus.ShowBorder = false;
		labelFmuStickTestIgnitionAndEngineStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(tableLayoutPanelHeader, "tableLayoutPanelHeader");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanelHeader, 4);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentEngineLoad, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentCoolantTemperature, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentFuelTemperature, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelHeader).Controls.Add((Control)(object)digitalReadoutInstrumentFuelMass, 2, 0);
		((Control)(object)tableLayoutPanelHeader).Name = "tableLayoutPanelHeader";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)0, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 0.1, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineLoad, "digitalReadoutInstrumentEngineLoad");
		digitalReadoutInstrumentEngineLoad.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineLoad).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineLoad).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineTorque");
		((Control)(object)digitalReadoutInstrumentEngineLoad).Name = "digitalReadoutInstrumentEngineLoad";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineLoad).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCoolantTemperature, "digitalReadoutInstrumentCoolantTemperature");
		digitalReadoutInstrumentCoolantTemperature.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).FreezeValue = false;
		digitalReadoutInstrumentCoolantTemperature.Gradient.Initialize((ValueState)2, 1, "°C");
		digitalReadoutInstrumentCoolantTemperature.Gradient.Modify(1, 20.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "coolantTemp");
		((Control)(object)digitalReadoutInstrumentCoolantTemperature).Name = "digitalReadoutInstrumentCoolantTemperature";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFuelTemperature, "digitalReadoutInstrumentFuelTemperature");
		digitalReadoutInstrumentFuelTemperature.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelTemperature).FreezeValue = false;
		digitalReadoutInstrumentFuelTemperature.Gradient.Initialize((ValueState)3, 1, "°C");
		digitalReadoutInstrumentFuelTemperature.Gradient.Modify(1, 20.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentFuelTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "fuelTemp");
		((Control)(object)digitalReadoutInstrumentFuelTemperature).Name = "digitalReadoutInstrumentFuelTemperature";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelTemperature).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFuelMass, "digitalReadoutInstrumentFuelMass");
		digitalReadoutInstrumentFuelMass.FontGroup = "StatusBar";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelMass).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentFuelMass).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS087_Actual_Fuel_Mass");
		((Control)(object)digitalReadoutInstrumentFuelMass).Name = "digitalReadoutInstrumentFuelMass";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelMass).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentQuantity_Control_Valve_Current, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(tabControl1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)chartInstrument1, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 2, 4);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current, "digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current");
		digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS121_EU_Fuel_Metering_Unit_FMU_desired_current");
		((Control)(object)digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current).Name = "digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentQuantity_Control_Valve_Current, "digitalReadoutInstrumentQuantity_Control_Valve_Current");
		digitalReadoutInstrumentQuantity_Control_Valve_Current.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentQuantity_Control_Valve_Current).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentQuantity_Control_Valve_Current).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS100_Quantity_Control_Valve_Current");
		((Control)(object)digitalReadoutInstrumentQuantity_Control_Valve_Current).Name = "digitalReadoutInstrumentQuantity_Control_Valve_Current";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)digitalReadoutInstrumentQuantity_Control_Valve_Current, 2);
		((SingleInstrumentBase)digitalReadoutInstrumentQuantity_Control_Valve_Current).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tabControl1, "tabControl1");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)tabControl1, 2);
		tabControl1.Controls.Add(tabPageFmuStickTest);
		tabControl1.Controls.Add(tabPageFmuAdaption);
		tabControl1.Name = "tabControl1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)tabControl1, 4);
		tabControl1.SelectedIndex = 0;
		tabPageFmuStickTest.Controls.Add((Control)(object)tableLayoutPanelFmuStickTest);
		componentResourceManager.ApplyResources(tabPageFmuStickTest, "tabPageFmuStickTest");
		tabPageFmuStickTest.Name = "tabPageFmuStickTest";
		tabPageFmuStickTest.UseVisualStyleBackColor = true;
		tabPageFmuAdaption.Controls.Add((Control)(object)tableLayoutPanel2);
		componentResourceManager.ApplyResources(tabPageFmuAdaption, "tabPageFmuAdaption");
		tabPageFmuAdaption.Name = "tabPageFmuAdaption";
		tabPageFmuAdaption.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)chartInstrument1, 2);
		componentResourceManager.ApplyResources(chartInstrument1, "chartInstrument1");
		((Control)(object)chartInstrument1).Name = "chartInstrument1";
		chartInstrument1.SelectedTime = null;
		chartInstrument1.ShowButtonPanel = false;
		chartInstrument1.ShowEvents = false;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_FISFuelQuantityControlValve");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFmuStickTestResults).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFmuStickTest).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelFmuStickTest).PerformLayout();
		((Control)(object)tableLayoutPanelHeader).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		tabControl1.ResumeLayout(performLayout: false);
		tabPageFmuStickTest.ResumeLayout(performLayout: false);
		tabPageFmuAdaption.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
