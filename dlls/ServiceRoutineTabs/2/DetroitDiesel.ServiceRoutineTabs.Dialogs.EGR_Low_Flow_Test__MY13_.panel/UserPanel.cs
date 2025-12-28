using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__MY13_.panel;

public class UserPanel : CustomPanel
{
	private enum State
	{
		NotRunning,
		Timer,
		RequestIdleModificationStop,
		Stopping
	}

	private enum WaitState
	{
		NotWaiting,
		WaitingToInitiateRampUp,
		WaitingForRampUp,
		WaitingForThermalCondition,
		WaitingForEGRInRange,
		WaitingToInitiateRunOffRampUp,
		WaitingForRunOffRampUp,
		WaitingForRunOffPeriod
	}

	private enum Result
	{
		CompletePass,
		CompleteFail,
		FailTemperaturesOutOfRange,
		FailEGRNotInRangeForPeriod,
		UserCanceled,
		ServiceFailure,
		EcuOffline
	}

	private const string ChargeAirCoolerOutTemperatureInstrumentQualifier = "DT_AS060_Charge_Air_Cooler_Outlet_Temperature";

	private const string CoolantTemperatureInstrumentQualifier = "DT_AS013_Coolant_Temperature";

	private const int TestIdleSpeed = 1200;

	private const int RunOffIdleSpeed = 1800;

	private ServiceCall idleSpeedModificationStart;

	private ServiceCall idleSpeedModificationStop;

	private static readonly TimeSpan testDurationThermalCondition = TimeSpan.FromMinutes(6.0);

	private static readonly TimeSpan maxTestDurationEGRInRange = TimeSpan.FromMinutes(30.0);

	private static readonly TimeSpan testDurationRunOffPeriod = TimeSpan.FromSeconds(30.0);

	private Channel channel;

	private State currentState = State.NotRunning;

	private WaitState currentWaitState = WaitState.NotWaiting;

	private Timer timer;

	private DateTime startTime;

	private DateTime startTimeEGRInRange;

	private DateTime startTimeRunOff;

	private WarningManager warningManager;

	private static string warningFormat = Resources.MessageFormat_WARNING;

	private bool adrReturnValue = false;

	private DigitalReadoutInstrument digitalReadoutFault;

	private Button buttonStart;

	private Button buttonStop;

	private Panel panelButtons;

	private System.Windows.Forms.Label labelCanStart;

	private Checkmark checkmarkCanStart;

	private BarInstrument barInstrumentEGRDeltaPressure;

	private BarInstrument barInstrumentEGRCommandedValue;

	private DigitalReadoutInstrument digitalReadoutVehicleCheckStatus;

	private DigitalReadoutInstrument digitalReadoutEngineSpeed;

	private BarInstrument barInstrumentChargeAirCoolerOutTemp;

	private TextBox textBoxOutput;

	private BarInstrument barInstrumentCoolantTemp;

	private TableLayoutPanel tableLayoutPanelMain;

	private bool CanStart => Online && !TestRunning && !Busy && VehicleCheckStatusOk && EngineRunning && TemperaturesOk;

	private bool CanStop => Online && TestRunning;

	private bool TestRunning => currentState != State.NotRunning;

	private bool Online => channel != null && channel.Online;

	private bool Busy => Online && channel.CommunicationsState != CommunicationsState.Online;

	private bool EngineRunning => (int)digitalReadoutEngineSpeed.RepresentedState != 3;

	private bool VehicleCheckStatusOk => (int)digitalReadoutVehicleCheckStatus.RepresentedState != 3;

	private bool TemperaturesOk => GetInstrumentValue((SingleInstrumentBase)(object)barInstrumentChargeAirCoolerOutTemp) >= 16.0 && GetInstrumentValue((SingleInstrumentBase)(object)barInstrumentCoolantTemp) >= 80.0;

	private bool TestPassed
	{
		get
		{
			double instrumentValue = GetInstrumentValue((SingleInstrumentBase)(object)barInstrumentEGRCommandedValue);
			double instrumentValue2 = GetInstrumentValue((SingleInstrumentBase)(object)barInstrumentEGRDeltaPressure);
			return instrumentValue >= 20.0 && instrumentValue <= 40.0 && instrumentValue2 >= 50.0 && instrumentValue2 <= 150.0;
		}
	}

	private bool EGRValveInTestRange
	{
		get
		{
			double instrumentValue = GetInstrumentValue((SingleInstrumentBase)(object)barInstrumentEGRCommandedValue);
			return instrumentValue >= 20.0;
		}
	}

	private bool EngineAtTestSpeed => (int)digitalReadoutEngineSpeed.RepresentedState == 2;

	private bool EngineAtRunOffSpeed => (int)digitalReadoutEngineSpeed.RepresentedState == 1;

	public UserPanel()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		warningManager = new WarningManager(warningFormat, Resources.Message_EGRLowFlowTest);
		InitializeComponent();
		timer = new Timer();
		buttonStart.Click += OnStartButton;
		buttonStop.Click += OnStopButton;
		timer.Tick += OnTimerTick;
		digitalReadoutVehicleCheckStatus.RepresentedStateChanged += OnPreconditionStateChanged;
		digitalReadoutEngineSpeed.RepresentedStateChanged += OnPreconditionStateChanged;
		idleSpeedModificationStop = SapiManager.GetCancelSetEngineSpeedService(SapiManager.GlobalInstance.ActiveChannels);
		idleSpeedModificationStart = SapiManager.GetSetEngineSpeedService(SapiManager.GlobalInstance.ActiveChannels, new string[0]);
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		UpdateUserInterface();
	}

	public override void OnChannelsChanged()
	{
		warningManager.Reset();
		SetChannel(((CustomPanel)this).GetChannel("MCM21T"));
	}

	private void SetChannel(Channel channel)
	{
		if (this.channel != channel)
		{
			if (this.channel != null)
			{
				this.channel.Instruments["DT_AS060_Charge_Air_Cooler_Outlet_Temperature"].InstrumentUpdateEvent -= OnTemperaturePreconditionChanged;
				this.channel.Instruments["DT_AS013_Coolant_Temperature"].InstrumentUpdateEvent -= OnTemperaturePreconditionChanged;
				this.channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			this.channel = channel;
			if (this.channel != null)
			{
				this.channel.Instruments["DT_AS060_Charge_Air_Cooler_Outlet_Temperature"].InstrumentUpdateEvent += OnTemperaturePreconditionChanged;
				this.channel.Instruments["DT_AS013_Coolant_Temperature"].InstrumentUpdateEvent += OnTemperaturePreconditionChanged;
				this.channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
			UpdateUserInterface();
		}
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (TestRunning)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			buttonStart.Click -= OnStartButton;
			buttonStop.Click -= OnStopButton;
			timer.Tick -= OnTimerTick;
			digitalReadoutVehicleCheckStatus.RepresentedStateChanged -= OnPreconditionStateChanged;
			digitalReadoutEngineSpeed.RepresentedStateChanged -= OnPreconditionStateChanged;
			SetChannel(null);
			((Control)this).Tag = new object[2] { adrReturnValue, textBoxOutput.Text };
		}
	}

	private void OnStartButton(object sender, EventArgs e)
	{
		if (warningManager.RequestContinue())
		{
			startTime = DateTime.Now;
			currentState = State.Timer;
			currentWaitState = WaitState.NotWaiting;
			startTimeEGRInRange = DateTime.MinValue;
			startTimeRunOff = DateTime.MinValue;
			GoMachine();
		}
	}

	private void OnStopButton(object sender, EventArgs e)
	{
		StopTest(Result.UserCanceled);
	}

	private void OnTimerTick(object sender, EventArgs e)
	{
		if (Online)
		{
			if (!TemperaturesOk)
			{
				StopTest(Result.FailTemperaturesOutOfRange);
			}
			else
			{
				switch (currentWaitState)
				{
				case WaitState.WaitingToInitiateRampUp:
					if (ManipulateIdleSpeed(1200))
					{
						currentWaitState = WaitState.WaitingForRampUp;
					}
					break;
				case WaitState.WaitingForRampUp:
					if (EngineAtTestSpeed)
					{
						currentWaitState = WaitState.WaitingForThermalCondition;
						Output(Resources.Message_EngineIsAtSpeedWaitingToGetPastThermalCondition);
					}
					break;
				case WaitState.WaitingForThermalCondition:
					if (DateTime.Now >= startTime + testDurationThermalCondition)
					{
						currentWaitState = WaitState.WaitingForEGRInRange;
						startTimeEGRInRange = DateTime.Now;
						Output(Resources.Message_ThermalConditionWaitCompleteWaitingForEGRToBeInRange);
					}
					break;
				case WaitState.WaitingForEGRInRange:
					if (EGRValveInTestRange)
					{
						Output(Resources.Message_EGRValveInRange);
						currentWaitState = WaitState.WaitingToInitiateRunOffRampUp;
					}
					else if (DateTime.Now >= startTimeEGRInRange + maxTestDurationEGRInRange)
					{
						StopTest(Result.FailEGRNotInRangeForPeriod);
					}
					break;
				case WaitState.WaitingToInitiateRunOffRampUp:
					if (ManipulateIdleSpeed(1800))
					{
						currentWaitState = WaitState.WaitingForRunOffRampUp;
					}
					break;
				case WaitState.WaitingForRunOffRampUp:
					if (EngineAtRunOffSpeed)
					{
						currentWaitState = WaitState.WaitingForRunOffPeriod;
						startTimeRunOff = DateTime.Now;
						Output(Resources.Message_EngineIsAtRunoffSpeedWaitingForRunoffPeriod);
					}
					break;
				case WaitState.WaitingForRunOffPeriod:
					if (EGRValveInTestRange)
					{
						if (DateTime.Now >= startTimeRunOff + testDurationRunOffPeriod)
						{
							StopTest((!TestPassed) ? Result.CompleteFail : Result.CompletePass);
						}
					}
					else
					{
						Output(Resources.Message_EGRValveDroppedOutOfRangeWaitingForItToBeInRangeAgain);
						currentWaitState = WaitState.WaitingForEGRInRange;
					}
					break;
				}
			}
			UpdateUserInterface();
		}
		else
		{
			StopTest(Result.EcuOffline);
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnPreconditionStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnTemperaturePreconditionChanged(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		buttonStart.Enabled = CanStart;
		buttonStop.Enabled = CanStop;
		checkmarkCanStart.CheckState = ((CanStart || TestRunning) ? CheckState.Checked : CheckState.Unchecked);
		string text = Resources.Message_TestCanStart;
		if (!buttonStart.Enabled)
		{
			if (TestRunning)
			{
				switch (currentWaitState)
				{
				case WaitState.NotWaiting:
				case WaitState.WaitingToInitiateRampUp:
				case WaitState.WaitingToInitiateRunOffRampUp:
					text = Resources.Message_ServiceRoutinesInProgress;
					break;
				case WaitState.WaitingForRampUp:
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForEngineSpeedToReach0Rpm, 1200);
					break;
				case WaitState.WaitingForRunOffRampUp:
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForEngineSpeedToReach0Rpm1, 1800);
					break;
				case WaitState.WaitingForThermalCondition:
				{
					TimeSpan timeSpan = startTime + testDurationThermalCondition - DateTime.Now;
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ThermalConditionWait0SecondsRemaining, (int)timeSpan.TotalSeconds);
					break;
				}
				case WaitState.WaitingForEGRInRange:
				{
					TimeSpan timeSpan = startTimeEGRInRange + maxTestDurationEGRInRange - DateTime.Now;
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForEGRValveToReachRange0SecondsRemaining, (int)timeSpan.TotalSeconds);
					break;
				}
				case WaitState.WaitingForRunOffPeriod:
				{
					TimeSpan timeSpan = startTimeRunOff + testDurationRunOffPeriod - DateTime.Now;
					text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForRunoff0SecondsRemaining, (int)timeSpan.TotalSeconds);
					break;
				}
				}
			}
			else if (Busy)
			{
				text = Resources.Message_CannotStartTheTestAsTheDeviceIsBusy;
			}
			else if (!Online)
			{
				text = Resources.Message_CannotStartTheTestAsTheDeviceIsNotOnline;
			}
			else if (!EngineRunning)
			{
				text = Resources.Message_CannotStartTheTestAsTheEngineIsNotRunningStartTheEngine;
			}
			else if (!VehicleCheckStatusOk)
			{
				text = Resources.Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionInNeutral;
			}
			else if (!TemperaturesOk)
			{
				text = Resources.Message_CannotStartTheTestAsTheRequiredTemperaturesAreNotInRange;
			}
		}
		labelCanStart.Text = text;
	}

	private void Output(string text)
	{
		TextBox textBox = textBoxOutput;
		textBox.Text = textBox.Text + text + Environment.NewLine;
		textBoxOutput.SelectionLength = 0;
		textBoxOutput.SelectionStart = textBoxOutput.Text.Length;
		textBoxOutput.ScrollToCaret();
	}

	private void StopTest(Result result)
	{
		timer.Stop();
		switch (result)
		{
		case Result.CompletePass:
			Output(Resources.Message_TESTCOMPLETEPASSED);
			Output(Resources.Message_CloseThisWindowToContinueTroubleshooting);
			adrReturnValue = true;
			break;
		case Result.CompleteFail:
			Output(Resources.Message_TESTCOMPLETEFAILED);
			Output(Resources.Message_CloseThisWindowToContinueTroubleshooting1);
			break;
		case Result.FailTemperaturesOutOfRange:
			Output(Resources.Message_TESTFAILEDTemperaturesFellOutRangeCorrectAndRestartTheTest);
			break;
		case Result.FailEGRNotInRangeForPeriod:
			Output(Resources.Message_TESTFAILEDEGRWasNotInRangeForTheRequiredPeriodCorrectAndRestartTheTest);
			break;
		case Result.UserCanceled:
			Output(Resources.Message_TESTABORTEDUserCanceledTheTest);
			break;
		case Result.ServiceFailure:
			Output(Resources.Message_TESTFAILEDServicesFailedToExecute);
			break;
		case Result.EcuOffline:
			Output(Resources.Message_TESTFAILEDDeviceWentOffline);
			break;
		}
		ReportInstrumentResults();
		UpdateUserInterface();
		currentWaitState = WaitState.NotWaiting;
		if (currentState <= State.RequestIdleModificationStop)
		{
			if (Online)
			{
				currentState = State.RequestIdleModificationStop;
				GoMachine();
			}
			else
			{
				Output(Resources.Message_UnableToRequestEndOfManipulation);
				currentState = State.NotRunning;
			}
		}
		else
		{
			currentState = State.NotRunning;
		}
		UpdateUserInterface();
	}

	private void ReportInstrumentResults()
	{
		GetInstrumentItemForDisplay((SingleInstrumentBase)(object)barInstrumentEGRCommandedValue);
		GetInstrumentItemForDisplay((SingleInstrumentBase)(object)barInstrumentEGRDeltaPressure);
		GetInstrumentItemForDisplay((SingleInstrumentBase)(object)barInstrumentCoolantTemp);
		GetInstrumentItemForDisplay((SingleInstrumentBase)(object)barInstrumentChargeAirCoolerOutTemp);
	}

	private void GoMachine()
	{
		if (!Online)
		{
			StopTest(Result.EcuOffline);
			return;
		}
		switch (currentState)
		{
		case State.Timer:
			currentWaitState = WaitState.WaitingToInitiateRampUp;
			timer.Interval = 1000;
			timer.Start();
			Output(Resources.Message_WaitingForTimer);
			UpdateUserInterface();
			break;
		case State.RequestIdleModificationStop:
		{
			Output(Resources.Message_RequestEndIdleSpeedManipulation);
			Service service = ((ServiceCall)(ref idleSpeedModificationStop)).GetService();
			if (service != null)
			{
				service.ServiceCompleteEvent += OnIdleSpeedStopServiceComplete;
				service.Execute(synchronous: false);
			}
			else
			{
				StopTest(Result.ServiceFailure);
			}
			break;
		}
		case State.Stopping:
			Output(Resources.Message_TestSequenceEnded);
			currentState = State.NotRunning;
			UpdateUserInterface();
			return;
		}
		currentState++;
	}

	private bool ManipulateIdleSpeed(int speed)
	{
		Output(Resources.Message_ManipulateEngineSpeedTo + speed + Resources.Message_Rpm);
		Service service = ((ServiceCall)(ref idleSpeedModificationStart)).GetService();
		if (service != null)
		{
			service.ServiceCompleteEvent += OnIdleSpeedStartServiceComplete;
			service.InputValues[0].Value = speed;
			service.Execute(synchronous: false);
		}
		else
		{
			StopTest(Result.ServiceFailure);
		}
		return service != null;
	}

	private double GetInstrumentValue(SingleInstrumentBase control)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		double result = double.NaN;
		Qualifier instrument = control.Instrument;
		Channel channel = ((CustomPanel)this).GetChannel(((Qualifier)(ref instrument)).Ecu);
		if (channel != null)
		{
			InstrumentCollection instruments = channel.Instruments;
			instrument = control.Instrument;
			Instrument instrument2 = instruments[((Qualifier)(ref instrument)).Name];
			if (instrument2 != null && instrument2.InstrumentValues.Current != null && instrument2.InstrumentValues.Current.Value != null)
			{
				result = Convert.ToDouble(instrument2.InstrumentValues.Current.Value);
			}
		}
		return result;
	}

	private void GetInstrumentItemForDisplay(SingleInstrumentBase control)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DataItem obj = DataItem.Create(control.Instrument, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
		InstrumentDataItem val = (InstrumentDataItem)(object)((obj is InstrumentDataItem) ? obj : null);
		if (val != null)
		{
			Output(string.Format(Resources.MessageFormat_TheObservedValueOf0Was12, ((DataItem)val).Name, Math.Round(((DataItem)val).ValueAsDouble(((DataItem)val).Value), ((DataItem)val).Precision).ToString(), ((DataItem)val).Units));
		}
	}

	private void OnIdleSpeedStartServiceComplete(object sender, ResultEventArgs e)
	{
		(sender as Service).ServiceCompleteEvent -= OnIdleSpeedStartServiceComplete;
		if (!e.Succeeded)
		{
			Output(e.Exception.Message);
			StopTest(Result.ServiceFailure);
		}
	}

	private void OnIdleSpeedStopServiceComplete(object sender, ResultEventArgs e)
	{
		(sender as Service).ServiceCompleteEvent -= OnIdleSpeedStopServiceComplete;
		if (!e.Succeeded)
		{
			Output(e.Exception.Message);
		}
		GoMachine();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
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
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		panelButtons = new Panel();
		barInstrumentCoolantTemp = new BarInstrument();
		textBoxOutput = new TextBox();
		barInstrumentEGRDeltaPressure = new BarInstrument();
		barInstrumentEGRCommandedValue = new BarInstrument();
		barInstrumentChargeAirCoolerOutTemp = new BarInstrument();
		digitalReadoutFault = new DigitalReadoutInstrument();
		digitalReadoutVehicleCheckStatus = new DigitalReadoutInstrument();
		digitalReadoutEngineSpeed = new DigitalReadoutInstrument();
		labelCanStart = new System.Windows.Forms.Label();
		checkmarkCanStart = new Checkmark();
		buttonStart = new Button();
		buttonStop = new Button();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		panelButtons.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)barInstrumentCoolantTemp, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add(textBoxOutput, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)barInstrumentEGRDeltaPressure, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)barInstrumentEGRCommandedValue, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)barInstrumentChargeAirCoolerOutTemp, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutFault, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutVehicleCheckStatus, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutEngineSpeed, 0, 2);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		barInstrumentCoolantTemp.BarOrientation = (ControlOrientation)1;
		barInstrumentCoolantTemp.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentCoolantTemp, "barInstrumentCoolantTemp");
		barInstrumentCoolantTemp.FontGroup = "lowflow_temps";
		((SingleInstrumentBase)barInstrumentCoolantTemp).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentCoolantTemp).Gradient.Initialize((ValueState)3, 1, "°C");
		((AxisSingleInstrumentBase)barInstrumentCoolantTemp).Gradient.Modify(1, 80.0, (ValueState)1);
		((SingleInstrumentBase)barInstrumentCoolantTemp).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS013_Coolant_Temperature");
		((Control)(object)barInstrumentCoolantTemp).Name = "barInstrumentCoolantTemp";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)barInstrumentCoolantTemp, 7);
		((SingleInstrumentBase)barInstrumentCoolantTemp).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentCoolantTemp).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentCoolantTemp).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)textBoxOutput, 3);
		componentResourceManager.ApplyResources(textBoxOutput, "textBoxOutput");
		textBoxOutput.Name = "textBoxOutput";
		textBoxOutput.ReadOnly = true;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)textBoxOutput, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)barInstrumentEGRDeltaPressure, 3);
		componentResourceManager.ApplyResources(barInstrumentEGRDeltaPressure, "barInstrumentEGRDeltaPressure");
		barInstrumentEGRDeltaPressure.FontGroup = "lowflow_pressures";
		((SingleInstrumentBase)barInstrumentEGRDeltaPressure).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentEGRDeltaPressure).Gradient.Initialize((ValueState)3, 2, "mbar");
		((AxisSingleInstrumentBase)barInstrumentEGRDeltaPressure).Gradient.Modify(1, 80.0, (ValueState)1);
		((AxisSingleInstrumentBase)barInstrumentEGRDeltaPressure).Gradient.Modify(2, 151.0, (ValueState)3);
		((SingleInstrumentBase)barInstrumentEGRDeltaPressure).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS025_EGR_Delta_Pressure");
		((Control)(object)barInstrumentEGRDeltaPressure).Name = "barInstrumentEGRDeltaPressure";
		((SingleInstrumentBase)barInstrumentEGRDeltaPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)barInstrumentEGRCommandedValue, 3);
		componentResourceManager.ApplyResources(barInstrumentEGRCommandedValue, "barInstrumentEGRCommandedValue");
		barInstrumentEGRCommandedValue.FontGroup = "lowflow_pressures";
		((SingleInstrumentBase)barInstrumentEGRCommandedValue).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentEGRCommandedValue).Gradient.Initialize((ValueState)3, 2);
		((AxisSingleInstrumentBase)barInstrumentEGRCommandedValue).Gradient.Modify(1, 20.0, (ValueState)1);
		((AxisSingleInstrumentBase)barInstrumentEGRCommandedValue).Gradient.Modify(2, 41.0, (ValueState)2);
		((SingleInstrumentBase)barInstrumentEGRCommandedValue).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS031_EGR_Commanded_Governor_Value");
		((Control)(object)barInstrumentEGRCommandedValue).Name = "barInstrumentEGRCommandedValue";
		((SingleInstrumentBase)barInstrumentEGRCommandedValue).UnitAlignment = StringAlignment.Near;
		barInstrumentChargeAirCoolerOutTemp.BarOrientation = (ControlOrientation)1;
		barInstrumentChargeAirCoolerOutTemp.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrumentChargeAirCoolerOutTemp, "barInstrumentChargeAirCoolerOutTemp");
		barInstrumentChargeAirCoolerOutTemp.FontGroup = "lowflow_temps";
		((SingleInstrumentBase)barInstrumentChargeAirCoolerOutTemp).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentChargeAirCoolerOutTemp).Gradient.Initialize((ValueState)3, 1, "°C");
		((AxisSingleInstrumentBase)barInstrumentChargeAirCoolerOutTemp).Gradient.Modify(1, 16.0, (ValueState)1);
		((SingleInstrumentBase)barInstrumentChargeAirCoolerOutTemp).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS060_Charge_Air_Cooler_Outlet_Temperature");
		((Control)(object)barInstrumentChargeAirCoolerOutTemp).Name = "barInstrumentChargeAirCoolerOutTemp";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)barInstrumentChargeAirCoolerOutTemp, 7);
		((SingleInstrumentBase)barInstrumentChargeAirCoolerOutTemp).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrumentChargeAirCoolerOutTemp).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrumentChargeAirCoolerOutTemp).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)digitalReadoutFault, 3);
		componentResourceManager.ApplyResources(digitalReadoutFault, "digitalReadoutFault");
		digitalReadoutFault.FontGroup = "lowflow_digitals";
		((SingleInstrumentBase)digitalReadoutFault).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutFault).Instrument = new Qualifier((QualifierTypes)32, "MCM21T", "630A12");
		((Control)(object)digitalReadoutFault).Name = "digitalReadoutFault";
		((SingleInstrumentBase)digitalReadoutFault).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)digitalReadoutVehicleCheckStatus, 3);
		componentResourceManager.ApplyResources(digitalReadoutVehicleCheckStatus, "digitalReadoutVehicleCheckStatus");
		digitalReadoutVehicleCheckStatus.FontGroup = "lowflow_digitals";
		((SingleInstrumentBase)digitalReadoutVehicleCheckStatus).FreezeValue = false;
		digitalReadoutVehicleCheckStatus.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutVehicleCheckStatus).Name = "digitalReadoutVehicleCheckStatus";
		((SingleInstrumentBase)digitalReadoutVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)digitalReadoutEngineSpeed, 3);
		componentResourceManager.ApplyResources(digitalReadoutEngineSpeed, "digitalReadoutEngineSpeed");
		digitalReadoutEngineSpeed.FontGroup = "lowflow_digitals";
		((SingleInstrumentBase)digitalReadoutEngineSpeed).FreezeValue = false;
		digitalReadoutEngineSpeed.Gradient.Initialize((ValueState)3, 3);
		digitalReadoutEngineSpeed.Gradient.Modify(1, 150.0, (ValueState)0);
		digitalReadoutEngineSpeed.Gradient.Modify(2, 1150.0, (ValueState)2);
		digitalReadoutEngineSpeed.Gradient.Modify(3, 1750.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)digitalReadoutEngineSpeed).Name = "digitalReadoutEngineSpeed";
		((SingleInstrumentBase)digitalReadoutEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(panelButtons, "panelButtons");
		panelButtons.Controls.Add(labelCanStart);
		panelButtons.Controls.Add((Control)(object)checkmarkCanStart);
		panelButtons.Controls.Add(buttonStart);
		panelButtons.Controls.Add(buttonStop);
		panelButtons.Name = "panelButtons";
		componentResourceManager.ApplyResources(labelCanStart, "labelCanStart");
		labelCanStart.Name = "labelCanStart";
		labelCanStart.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkCanStart, "checkmarkCanStart");
		((Control)(object)checkmarkCanStart).Name = "checkmarkCanStart";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Controls.Add(panelButtons);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		panelButtons.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
