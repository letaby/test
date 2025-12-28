using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__EPA07_.panel;

public class UserPanel : CustomPanel
{
	private enum State
	{
		NotRunning,
		OpenFuelCutoffValve,
		WaitingToRecordIntialSample,
		WaitingBeforeOpeningHcDoserValve,
		OpenHcDoserValve,
		HcDoserWaitTimer,
		CloseHcDoserValve,
		WaitingToRecordAfterDosingSample,
		RequestHcDoserValveStop,
		RequestFuelCutoffValveStop,
		Stopping
	}

	private const string PWMRoutineProductionStartControl = "RT_SR003_PWM_Routine_for_Production_Start_PWM_Value";

	private const string PWMRoutineProductionStopControl = "RT_SR003_PWM_Routine_for_Production_Stop";

	private const string SwRoutineProductionStartControl = "RT_SR005_SW_Routine_for_Production_Start_SW_Operation";

	private const string SwRoutineProductionStopControl = "RT_SR005_SW_Routine_for_Production_Stop";

	private const string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";

	private const string FuelCompensationPressureInstrumentQualifier = "DT_AS024_Fuel_Compensation_Pressure";

	private const int OpenHCDoserControlValue = 25;

	private const int CloseHCDoserControlValue = 0;

	private const int HCDoserPwmIndex = 12;

	private const int FuelCutoffValveSwIndex = 8;

	private static readonly TimeSpan RepeatSwRoutinePeriod = TimeSpan.FromSeconds(30.0);

	private static readonly TimeSpan SwRoutineDuration = TimeSpan.FromSeconds(60.0);

	private Timer repeatSwRoutineTimer;

	private Stopwatch repeatSwRoutineStopwatch;

	private static readonly TimeSpan OpenHCDoserControlTime = TimeSpan.FromSeconds(15.0);

	private static readonly TimeSpan testDuration = TimeSpan.FromMinutes(15.0);

	private Channel channel;

	private Instrument engineSpeed = null;

	private Instrument fuelCompensationPressure = null;

	private State currentState = State.NotRunning;

	private Timer intialWaitTimer;

	private Timer beforeDosingWaitTimer;

	private Timer openHcDoserWaitTimer;

	private Timer afterDosingWaitTimer;

	private Timer sampleRecordWaitTimer;

	private DateTime sampleMonitoringStartTime;

	private double initialFuelCompensationPressureReading;

	private double finalFuelCompensationPressureReading;

	private double requiredPressureDropValue = 68.95;

	private double thresholdLeakValue = 34.48;

	private static readonly TimeSpan maxSamplesMonitoredDuration = TimeSpan.FromMinutes(10.0);

	private SeekTimeListView seekTimeListView;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel engineCheckTableLayoutPanel;

	private Checkmark engineSpeedCheck;

	private Label engineStatusLabel;

	private ChartInstrument chartInstrument;

	private TableLayoutPanel tableLayoutPanel2;

	private Button buttonStop;

	private Button buttonStart;

	private DigitalReadoutInstrument fuelCompensationPressureInstrument;

	private ScalingLabel labelNote;

	private bool CanStart => Online && !TestRunning && !Busy && engineSpeedCheck.Checked;

	private bool CanStop => Online && TestRunning;

	private bool CanReset => Online && !TestRunning;

	private bool Online => channel != null && channel.Online;

	private bool Busy => Online && channel.CommunicationsState != CommunicationsState.Online;

	private bool TestRunning => currentState != State.NotRunning;

	public UserPanel()
	{
		InitializeComponent();
		intialWaitTimer = new Timer();
		beforeDosingWaitTimer = new Timer();
		openHcDoserWaitTimer = new Timer();
		afterDosingWaitTimer = new Timer();
		sampleRecordWaitTimer = new Timer();
		repeatSwRoutineTimer = new Timer();
		repeatSwRoutineTimer.Interval = (int)RepeatSwRoutinePeriod.TotalMilliseconds / 2;
		repeatSwRoutineStopwatch = new Stopwatch();
		buttonStart.Click += OnStartButton;
		buttonStop.Click += OnStopButton;
		intialWaitTimer.Tick += OnInitialWaitTimerTick;
		beforeDosingWaitTimer.Tick += OnBeforeDosingWaitTimerTick;
		openHcDoserWaitTimer.Tick += OnOpenHcDoserWaitTimerTick;
		afterDosingWaitTimer.Tick += OnAfterDosingWaitTimerTick;
		sampleRecordWaitTimer.Tick += OnSampleRecordWaitTimerTick;
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM"));
		UpdateUserInterface();
	}

	private void SetMCM(Channel channel)
	{
		if (this.channel == channel)
		{
			return;
		}
		if (this.channel != null)
		{
			this.channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		if (engineSpeed != null)
		{
			engineSpeed.InstrumentUpdateEvent -= OnEngineSpeedUpdate;
			engineSpeed = null;
		}
		this.channel = channel;
		if (this.channel != null)
		{
			this.channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			engineSpeed = this.channel.Instruments["DT_AS010_Engine_Speed"];
			fuelCompensationPressure = this.channel.Instruments["DT_AS024_Fuel_Compensation_Pressure"];
			if (engineSpeed != null)
			{
				engineSpeed.InstrumentUpdateEvent += OnEngineSpeedUpdate;
			}
		}
	}

	private void OnStartButton(object sender, EventArgs e)
	{
		currentState = State.OpenFuelCutoffValve;
		GoMachine();
	}

	private void OnStopButton(object sender, EventArgs e)
	{
		StopTest(Resources.Message_TestAbortedUserCanceledTheTest);
	}

	private void OnInitialWaitTimerTick(object sender, EventArgs e)
	{
		intialWaitTimer.Stop();
		double instrumentValue = GetInstrumentValue(fuelCompensationPressure);
		if (!double.IsNaN(instrumentValue))
		{
			initialFuelCompensationPressureReading = instrumentValue;
			GoMachine();
		}
		else
		{
			StopTest(Resources.Message_TestAbortedUnableToReadTheFuelCompensationPressureValue);
		}
	}

	private void OnBeforeDosingWaitTimerTick(object sender, EventArgs e)
	{
		beforeDosingWaitTimer.Stop();
		GoMachine();
	}

	private void OnOpenHcDoserWaitTimerTick(object sender, EventArgs e)
	{
		openHcDoserWaitTimer.Stop();
		GoMachine();
	}

	private void OnAfterDosingWaitTimerTick(object sender, EventArgs e)
	{
		afterDosingWaitTimer.Stop();
		double instrumentValue = GetInstrumentValue(fuelCompensationPressure);
		if (!double.IsNaN(instrumentValue))
		{
			if (instrumentValue >= initialFuelCompensationPressureReading - requiredPressureDropValue)
			{
				StopTest(Resources.Message_TestWillBeAbortedPleaseTurnOffTheAirSupplyAndEnsureFuelHasBeenPurgedFromTheSystemAndPerformTheTestAgain);
				return;
			}
			initialFuelCompensationPressureReading = instrumentValue;
			sampleMonitoringStartTime = DateTime.Now;
			AddLogLabel(Resources.Message_LowPressureLeakDetectionCheckStarted);
			WaitForNextSampleTime(instrumentValue);
		}
		else
		{
			StopTest(Resources.Message_TestAbortedUnableToReadTheFuelCompensationPressureValue1);
		}
	}

	private void OnSampleRecordWaitTimerTick(object sender, EventArgs e)
	{
		sampleRecordWaitTimer.Stop();
		double instrumentValue = GetInstrumentValue(fuelCompensationPressure);
		if (!double.IsNaN(instrumentValue))
		{
			if (initialFuelCompensationPressureReading - instrumentValue > thresholdLeakValue)
			{
				finalFuelCompensationPressureReading = instrumentValue;
				ReportFuelPressureResult();
				StopTest(Resources.Message_TestFailedLeakWasDetected);
			}
			else if (instrumentValue >= initialFuelCompensationPressureReading + thresholdLeakValue)
			{
				StopTest(Resources.Message_TestWillBeAbortedPleaseTurnOffTheAirSupplyAndEnsureFuelHasBeenPurgedFromTheSystemAndPerformTheTestAgain1);
			}
			else
			{
				AddLogLabel(Resources.Message_CheckInProgress);
				WaitForNextSampleTime(instrumentValue);
			}
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnEngineSpeedUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		UpdateEngineStatus();
		buttonStart.Enabled = CanStart;
		buttonStop.Enabled = CanStop;
	}

	private void UpdateEngineStatus()
	{
		bool flag = false;
		string text = Resources.Message_EngineSpeedCannotBeDetected;
		double instrumentValue = GetInstrumentValue(engineSpeed);
		if (!double.IsNaN(instrumentValue))
		{
			if (instrumentValue == 0.0)
			{
				text = Resources.Message_EngineIsNotRunningTestCanStart;
				flag = true;
			}
			else
			{
				text = Resources.Message_EngineIsRunningTestCannotStart;
			}
		}
		((Control)(object)engineStatusLabel).Text = text;
		engineSpeedCheck.Checked = flag;
	}

	private double GetInstrumentValue(Instrument instrument)
	{
		double result = double.NaN;
		if (instrument != null && instrument.InstrumentValues.Current != null && double.TryParse(instrument.InstrumentValues.Current.Value.ToString(), out result))
		{
			result = Math.Round(result, 2);
		}
		return result;
	}

	private void WaitForNextSampleTime(double currentInstrumentValue)
	{
		if (DateTime.Now <= sampleMonitoringStartTime + maxSamplesMonitoredDuration)
		{
			sampleRecordWaitTimer.Interval = 30000;
			sampleRecordWaitTimer.Start();
		}
		else
		{
			finalFuelCompensationPressureReading = currentInstrumentValue;
			ReportFuelPressureResult();
			StopTest(Resources.Message_TestPassedLeakWasNotDetected);
		}
	}

	private void ReportFuelPressureResult()
	{
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TheIntialFuelCompensationPressureObservedWas0, ReportInstrumentValue(fuelCompensationPressure, initialFuelCompensationPressureReading)));
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TheFinalFuelCompensationPressureObservedWas0, ReportInstrumentValue(fuelCompensationPressure, finalFuelCompensationPressureReading)));
	}

	private string ReportInstrumentValue(Instrument instrument, double instrumentValue)
	{
		string empty = string.Empty;
		Conversion conversion = Converter.GlobalInstance.GetConversion(instrument.Units);
		if (conversion == null)
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} {1}", Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)instrumentValue, instrument.Units, instrument.Precision), instrument.Units);
		}
		return string.Format(CultureInfo.CurrentCulture, "{0} {1}", Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)instrumentValue, conversion, instrument.Precision), conversion.OutputUnit);
	}

	private void AddLogLabel(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
	}

	private void StopTest(string result)
	{
		intialWaitTimer.Stop();
		beforeDosingWaitTimer.Stop();
		openHcDoserWaitTimer.Stop();
		afterDosingWaitTimer.Stop();
		sampleRecordWaitTimer.Stop();
		repeatSwRoutineTimer.Stop();
		repeatSwRoutineTimer.Tick -= OnRepeatSwRoutineTimerTick;
		repeatSwRoutineStopwatch.Stop();
		AddLogLabel(result);
		UpdateUserInterface();
		if (currentState >= State.HcDoserWaitTimer)
		{
			if (Online)
			{
				currentState = State.RequestHcDoserValveStop;
				GoMachine();
			}
			else
			{
				AddLogLabel(Resources.Message_UnableToRequestStopFuelCutoffAndHCDoserValveRoutines);
				currentState = State.NotRunning;
			}
		}
		else if (currentState >= State.WaitingToRecordIntialSample)
		{
			if (Online)
			{
				currentState = State.RequestFuelCutoffValveStop;
				GoMachine();
			}
			else
			{
				AddLogLabel(Resources.Message_UnableToRequestStopFuelCutoffRoutine);
				currentState = State.NotRunning;
			}
		}
		else
		{
			currentState = State.NotRunning;
		}
	}

	private void GoMachine()
	{
		if (!Online)
		{
			StopTest(Resources.Message_TestAbortedDeviceWentOffline);
			return;
		}
		switch (currentState)
		{
		case State.OpenFuelCutoffValve:
			AddLogLabel(Resources.Message_RequestingOpenFuelCutoffValve);
			OpenFuelCutOffValve();
			break;
		case State.WaitingToRecordIntialSample:
			intialWaitTimer.Interval = (int)TimeSpan.FromSeconds(20.0).TotalMilliseconds;
			intialWaitTimer.Start();
			AddLogLabel(Resources.Message_WaitingFor20Seconds);
			break;
		case State.WaitingBeforeOpeningHcDoserValve:
			beforeDosingWaitTimer.Interval = (int)TimeSpan.FromSeconds(10.0).TotalMilliseconds;
			beforeDosingWaitTimer.Start();
			AddLogLabel(Resources.Message_WaitingFor10Seconds);
			break;
		case State.OpenHcDoserValve:
			AddLogLabel(Resources.Message_RequestingOpenHCDoserValve);
			OpenHCDoserValve();
			break;
		case State.HcDoserWaitTimer:
			openHcDoserWaitTimer.Interval = (int)TimeSpan.FromSeconds(15.0).TotalMilliseconds;
			openHcDoserWaitTimer.Start();
			AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_OpenedTheHCDoserValveFor0For1Seconds, 25, (int)OpenHCDoserControlTime.TotalSeconds));
			break;
		case State.CloseHcDoserValve:
			AddLogLabel(Resources.Message_RequestingCloseHCDoserValve);
			CloseHCDoserValve();
			break;
		case State.WaitingToRecordAfterDosingSample:
			AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ClosedTheHCDoserValveFor0Minutes, (int)testDuration.TotalMinutes));
			afterDosingWaitTimer.Interval = (int)TimeSpan.FromSeconds(45.0).TotalMilliseconds;
			afterDosingWaitTimer.Start();
			AddLogLabel(Resources.Message_WaitingFor45Seconds);
			break;
		case State.RequestHcDoserValveStop:
			AddLogLabel(Resources.Message_RequestingStopHCDoserValve);
			StopHCDoserValve();
			break;
		case State.RequestFuelCutoffValveStop:
			AddLogLabel(Resources.Message_RequestingStopFuelCutoffValve);
			StopFuelCutOffValve();
			break;
		case State.Stopping:
			AddLogLabel(Resources.Message_TestCompleted);
			currentState = State.NotRunning;
			return;
		}
		currentState++;
	}

	private void OpenFuelCutOffValve()
	{
		SetSwRoutineStart((int)testDuration.TotalMilliseconds);
	}

	private void OpenHCDoserValve()
	{
		SetPwmRoutineStart(25, (int)OpenHCDoserControlTime.TotalMilliseconds);
	}

	private void CloseHCDoserValve()
	{
		SetPwmRoutineStart(0, (int)testDuration.TotalMilliseconds);
	}

	private void SetPwmRoutineStart(int controlValue, int controlTime)
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR003_PWM_Routine_for_Production_Start_PWM_Value");
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0To1, service.InputValues[0].Choices.GetItemFromRawValue(12), controlValue));
		service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(12);
		service.InputValues[1].Value = controlValue;
		service.InputValues[2].Value = controlTime;
		service.ServiceCompleteEvent += OnPwmRoutineServiceCompleteEvent;
		service.Execute(synchronous: false);
	}

	private void SetSwRoutineStart(int controlTime)
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR005_SW_Routine_for_Production_Start_SW_Operation");
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0, service.InputValues[0].Choices.GetItemFromRawValue(8)));
		service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(8);
		service.InputValues[1].Value = service.InputValues[1].Choices.GetItemFromRawValue(1);
		service.InputValues[2].Value = (float)SwRoutineDuration.TotalMilliseconds;
		service.ServiceCompleteEvent += OnPwmRoutineServiceCompleteEvent;
		service.Execute(synchronous: false);
		repeatSwRoutineStopwatch.Reset();
		repeatSwRoutineStopwatch.Start();
		repeatSwRoutineTimer.Tick += OnRepeatSwRoutineTimerTick;
		repeatSwRoutineTimer.Start();
	}

	private void OnRepeatSwRoutineTimerTick(object sender, EventArgs args)
	{
		if (repeatSwRoutineStopwatch.Elapsed >= RepeatSwRoutinePeriod)
		{
			repeatSwRoutineStopwatch.Reset();
			Service service = ((CustomPanel)this).GetService("MCM", "RT_SR005_SW_Routine_for_Production_Start_SW_Operation");
			if (service != null)
			{
				service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(8);
				service.InputValues[1].Value = service.InputValues[1].Choices.GetItemFromRawValue(1);
				service.InputValues[2].Value = (float)SwRoutineDuration.TotalMilliseconds;
				service.ServiceCompleteEvent += OnServiceCompleteEventNoAdvance;
				service.Execute(synchronous: false);
				repeatSwRoutineStopwatch.Start();
			}
			else if (TestRunning)
			{
				StopTest(Resources.Message_CancellingTheTestUnableToKeepFuelCutoffValveOpen);
			}
		}
	}

	private void OnServiceCompleteEventNoAdvance(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= OnServiceCompleteEventNoAdvance;
		if (!e.Succeeded)
		{
			StopTest(e.Exception.Message);
		}
	}

	private void OnPwmRoutineServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= OnPwmRoutineServiceCompleteEvent;
		if (e.Succeeded)
		{
			GoMachine();
		}
		else
		{
			StopTest(e.Exception.Message);
		}
	}

	private void StopFuelCutOffValve()
	{
		SetSwRoutineStop();
	}

	private void StopHCDoserValve()
	{
		SetPwmRoutineStop();
	}

	private void SetPwmRoutineStop()
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR003_PWM_Routine_for_Production_Stop");
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Resetting0, service.InputValues[0].Choices.GetItemFromRawValue(12)));
		service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(12);
		service.ServiceCompleteEvent += OnPwmRoutineStopServiceCompleteEvent;
		service.Execute(synchronous: false);
	}

	private void SetSwRoutineStop()
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR005_SW_Routine_for_Production_Stop");
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Resetting01, service.InputValues[0].Choices.GetItemFromRawValue(8)));
		service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(8);
		service.ServiceCompleteEvent += OnPwmRoutineStopServiceCompleteEvent;
		service.Execute(synchronous: false);
	}

	private void OnPwmRoutineStopServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= OnPwmRoutineStopServiceCompleteEvent;
		if (!e.Succeeded)
		{
			AddLogLabel(e.Exception.Message);
		}
		GoMachine();
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
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel2 = new TableLayoutPanel();
		engineCheckTableLayoutPanel = new TableLayoutPanel();
		tableLayoutPanel1 = new TableLayoutPanel();
		seekTimeListView = new SeekTimeListView();
		engineSpeedCheck = new Checkmark();
		engineStatusLabel = new Label();
		chartInstrument = new ChartInstrument();
		buttonStop = new Button();
		buttonStart = new Button();
		fuelCompensationPressureInstrument = new DigitalReadoutInstrument();
		labelNote = new ScalingLabel();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)engineCheckTableLayoutPanel).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)engineCheckTableLayoutPanel, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)chartInstrument, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)fuelCompensationPressureInstrument, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelNote, 0, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView, 2);
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "FIS Low Pressure Leak Test";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView, 2);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss:f";
		componentResourceManager.ApplyResources(engineCheckTableLayoutPanel, "engineCheckTableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)engineCheckTableLayoutPanel, 3);
		((TableLayoutPanel)(object)engineCheckTableLayoutPanel).Controls.Add((Control)(object)engineSpeedCheck, 0, 0);
		((TableLayoutPanel)(object)engineCheckTableLayoutPanel).Controls.Add((Control)(object)engineStatusLabel, 1, 0);
		((Control)(object)engineCheckTableLayoutPanel).Name = "engineCheckTableLayoutPanel";
		componentResourceManager.ApplyResources(engineSpeedCheck, "engineSpeedCheck");
		((Control)(object)engineSpeedCheck).Name = "engineSpeedCheck";
		engineStatusLabel.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(engineStatusLabel, "engineStatusLabel");
		((Control)(object)engineStatusLabel).Name = "engineStatusLabel";
		engineStatusLabel.Orientation = (TextOrientation)1;
		engineStatusLabel.ShowBorder = false;
		engineStatusLabel.UseSystemColors = true;
		componentResourceManager.ApplyResources(chartInstrument, "chartInstrument");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)chartInstrument, 3);
		((Collection<Qualifier>)(object)chartInstrument.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM", "DT_AS024_Fuel_Compensation_Pressure"));
		((Control)(object)chartInstrument).Name = "chartInstrument";
		chartInstrument.SelectedTime = null;
		chartInstrument.ShowButtonPanel = false;
		chartInstrument.ShowEvents = false;
		chartInstrument.ShowLegend = false;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonStop, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonStart, 0, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(fuelCompensationPressureInstrument, "fuelCompensationPressureInstrument");
		fuelCompensationPressureInstrument.FontGroup = null;
		((SingleInstrumentBase)fuelCompensationPressureInstrument).FreezeValue = false;
		((SingleInstrumentBase)fuelCompensationPressureInstrument).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS024_Fuel_Compensation_Pressure");
		((Control)(object)fuelCompensationPressureInstrument).Name = "fuelCompensationPressureInstrument";
		((SingleInstrumentBase)fuelCompensationPressureInstrument).UnitAlignment = StringAlignment.Near;
		labelNote.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelNote, 3);
		componentResourceManager.ApplyResources(labelNote, "labelNote");
		labelNote.FontGroup = null;
		labelNote.LineAlignment = StringAlignment.Center;
		((Control)(object)labelNote).Name = "labelNote";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("PanelFISLowPressureLeakTest");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)engineCheckTableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
