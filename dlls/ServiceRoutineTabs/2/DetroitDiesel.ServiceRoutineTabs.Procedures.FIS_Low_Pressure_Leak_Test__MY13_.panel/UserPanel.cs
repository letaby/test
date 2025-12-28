using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__MY13_.panel;

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

	private enum PWMFunctionIndices
	{
		HCDoser = 10,
		FuelCutOffValve = 15
	}

	private const string PWMRoutineProductionStartControl = "RT_SR003_PWM_Routine_by_Function_for_Production_Start_Control_Value";

	private const string PWMRoutineProductionStopControl = "RT_SR003_PWM_Routine_by_Function_for_Production_Stop_Function_Name";

	private const string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";

	private const string FuelCompensationPressureInstrumentQualifier = "DT_AS024_Fuel_Compensation_Pressure";

	private const int FuelCutoffControlValue = 100;

	private const int OpenHCDoserControlValue = 25;

	private const int CloseHCDoserControlValue = 0;

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
		SetMCM(((CustomPanel)this).GetChannel("MCM21T"));
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
		StopTest(Resources.Message_TestAbortedUserCanceledTheTest0);
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
		SetPwmRoutineStart(PWMFunctionIndices.FuelCutOffValve, 100, (int)testDuration.TotalMilliseconds);
	}

	private void OpenHCDoserValve()
	{
		SetPwmRoutineStart(PWMFunctionIndices.HCDoser, 25, (int)OpenHCDoserControlTime.TotalMilliseconds);
	}

	private void CloseHCDoserValve()
	{
		SetPwmRoutineStart(PWMFunctionIndices.HCDoser, 0, (int)testDuration.TotalMilliseconds);
	}

	private void SetPwmRoutineStart(PWMFunctionIndices pwmFunctionIndex, int controlValue, int controlTime)
	{
		Service service = ((CustomPanel)this).GetService("MCM21T", "RT_SR003_PWM_Routine_by_Function_for_Production_Start_Control_Value");
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0To1, SapiExtensions.NameFromRawValue(service.InputValues[0].Choices, (object)pwmFunctionIndex), controlValue));
		service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(pwmFunctionIndex);
		service.InputValues[1].Value = controlValue;
		service.InputValues[2].Value = controlTime;
		service.ServiceCompleteEvent += OnPwmRoutineServiceCompleteEvent;
		service.Execute(synchronous: false);
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
		SetPwmRoutineStop(PWMFunctionIndices.FuelCutOffValve);
	}

	private void StopHCDoserValve()
	{
		SetPwmRoutineStop(PWMFunctionIndices.HCDoser);
	}

	private void SetPwmRoutineStop(PWMFunctionIndices pwmFunctionIndex)
	{
		Service service = ((CustomPanel)this).GetService("MCM21T", "RT_SR003_PWM_Routine_by_Function_for_Production_Stop_Function_Name");
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Resetting0, SapiExtensions.NameFromRawValue(service.InputValues[0].Choices, (object)pwmFunctionIndex)));
		service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(pwmFunctionIndex);
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
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel2 = new TableLayoutPanel();
		buttonStop = new Button();
		buttonStart = new Button();
		engineCheckTableLayoutPanel = new TableLayoutPanel();
		engineSpeedCheck = new Checkmark();
		engineStatusLabel = new Label();
		tableLayoutPanel1 = new TableLayoutPanel();
		seekTimeListView = new SeekTimeListView();
		chartInstrument = new ChartInstrument();
		fuelCompensationPressureInstrument = new DigitalReadoutInstrument();
		labelNote = new ScalingLabel();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)engineCheckTableLayoutPanel).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
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
		componentResourceManager.ApplyResources(chartInstrument, "chartInstrument");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)chartInstrument, 3);
		((Collection<Qualifier>)(object)chartInstrument.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure"));
		((Control)(object)chartInstrument).Name = "chartInstrument";
		chartInstrument.SelectedTime = null;
		chartInstrument.ShowButtonPanel = false;
		chartInstrument.ShowEvents = false;
		chartInstrument.ShowLegend = false;
		componentResourceManager.ApplyResources(fuelCompensationPressureInstrument, "fuelCompensationPressureInstrument");
		fuelCompensationPressureInstrument.FontGroup = null;
		((SingleInstrumentBase)fuelCompensationPressureInstrument).FreezeValue = false;
		((SingleInstrumentBase)fuelCompensationPressureInstrument).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure");
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
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)engineCheckTableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
