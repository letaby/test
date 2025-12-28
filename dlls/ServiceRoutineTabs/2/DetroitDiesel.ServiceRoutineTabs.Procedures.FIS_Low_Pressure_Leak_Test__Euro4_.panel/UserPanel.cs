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

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__Euro4_.panel;

public class UserPanel : CustomPanel
{
	private enum State
	{
		NotRunning,
		WaitingLetAirPressureStabilize,
		StartMonitorLppoPresure
	}

	private const string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";

	private const string LppoPressureInstrumentQualifier = "DT_AS124_Low_Pressure_Pump_Outlet_Pressure";

	private const double ThresholdLeakValue = 0.03447;

	private Channel channel;

	private Instrument engineSpeed;

	private Instrument lppoPressure;

	private State currentState = State.NotRunning;

	private readonly Timer airPressureStabilizeTimer;

	private readonly Timer monitorLppoPresureTimer;

	private readonly Timer progressBarTimer;

	private double finalLppoPressureReading;

	private double initialLppoPressureReading;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonStop;

	private Button buttonStart;

	private DigitalReadoutInstrument fuelCompensationPressureInstrument;

	private SeekTimeListView seekTimeListView;

	private ProgressBar progressBar;

	private TableLayoutPanel engineCheckTableLayoutPanel;

	private Checkmark engineSpeedCheck;

	private Label engineStatusLabel;

	private ChartInstrument chartInstrument;

	private bool CanStart => Online && !TestRunning && !Busy && engineSpeedCheck.Checked;

	private bool CanStop => Online && TestRunning;

	private bool Online => channel != null && channel.Online;

	private bool Busy => Online && channel.CommunicationsState != CommunicationsState.Online;

	private bool TestRunning => currentState != State.NotRunning;

	public UserPanel()
	{
		InitializeComponent();
		airPressureStabilizeTimer = new Timer();
		airPressureStabilizeTimer.Tick += AirPressureStabilizeTimer_Tick;
		monitorLppoPresureTimer = new Timer();
		monitorLppoPresureTimer.Tick += MonitorLppoPresureTimer_Tick;
		progressBarTimer = new Timer();
		progressBarTimer.Tick += ProgressBarTimer_Tick;
		buttonStart.Click += OnStartButton;
		buttonStop.Click += OnStopButton;
		UpdateUserInterface();
	}

	public override void OnChannelsChanged()
	{
		SetMcm(((CustomPanel)this).GetChannel("MCM"));
		UpdateUserInterface();
	}

	private void SetMcm(Channel mcmChannel)
	{
		if (channel == mcmChannel)
		{
			return;
		}
		if (channel != null)
		{
			channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		if (engineSpeed != null)
		{
			engineSpeed.InstrumentUpdateEvent -= OnEngineSpeedUpdate;
			engineSpeed = null;
		}
		if (lppoPressure != null)
		{
			lppoPressure.InstrumentUpdateEvent -= LppoPressure_InstrumentUpdateEvent;
		}
		channel = mcmChannel;
		if (channel != null)
		{
			channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			engineSpeed = channel.Instruments["DT_AS010_Engine_Speed"];
			lppoPressure = channel.Instruments["DT_AS124_Low_Pressure_Pump_Outlet_Pressure"];
			if (engineSpeed != null)
			{
				engineSpeed.InstrumentUpdateEvent += OnEngineSpeedUpdate;
			}
			if (lppoPressure != null)
			{
				lppoPressure.InstrumentUpdateEvent += LppoPressure_InstrumentUpdateEvent;
			}
		}
	}

	private void OnStartButton(object sender, EventArgs e)
	{
		currentState = State.WaitingLetAirPressureStabilize;
		GoMachine();
	}

	private void OnStopButton(object sender, EventArgs e)
	{
		StopTest(Resources.Message_TestAbortedUserCanceledTheTest);
	}

	private void SampleFuelPressure()
	{
		double instrumentValue = GetInstrumentValue(lppoPressure);
		if (!double.IsNaN(instrumentValue))
		{
			if (initialLppoPressureReading - instrumentValue > 0.03447)
			{
				finalLppoPressureReading = instrumentValue;
				ReportLppoPressureResult();
				StopTest(Resources.Message_TestFailedLeakWasDetected);
			}
		}
		else
		{
			StopTest(Resources.Message_TestFailedPressureCouldNotBeRead);
		}
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
				if (currentState != State.NotRunning)
				{
					StopTest(Resources.Message_TestFailedEnigneIsRunning);
				}
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

	private void ReportLppoPressureResult()
	{
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TheInitialLPPOPressureObservedWas0, ReportInstrumentValue(lppoPressure, initialLppoPressureReading)));
		AddLogLabel(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TheFinalLPPOPressureObservedWas0, ReportInstrumentValue(lppoPressure, finalLppoPressureReading)));
	}

	private string ReportInstrumentValue(Instrument instrument, double instrumentValue)
	{
		Conversion conversion = Converter.GlobalInstance.GetConversion(instrument.Units);
		if (conversion != null)
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} {1}", Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)instrumentValue, conversion, instrument.Precision), conversion.OutputUnit);
		}
		return string.Format(CultureInfo.CurrentCulture, "{0} {1}", Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)instrumentValue, instrument.Units, instrument.Precision), instrument.Units);
	}

	private void AddLogLabel(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
	}

	private void StopTest(string result)
	{
		airPressureStabilizeTimer.Stop();
		monitorLppoPresureTimer.Stop();
		progressBarTimer.Stop();
		progressBar.Value = 0;
		AddLogLabel(Resources.Message_TestCompleted);
		AddLogLabel(result);
		currentState = State.NotRunning;
		UpdateUserInterface();
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
		case State.WaitingLetAirPressureStabilize:
			progressBar.Minimum = 0;
			progressBar.Maximum = 300;
			progressBar.Value = 0;
			progressBarTimer.Interval = (int)TimeSpan.FromSeconds(1.0).TotalMilliseconds;
			airPressureStabilizeTimer.Interval = (int)TimeSpan.FromMinutes(5.0).TotalMilliseconds;
			airPressureStabilizeTimer.Start();
			progressBarTimer.Start();
			AddLogLabel(Resources.Message_WaitingFiveMinutesToLetTheAirPressureStabilize);
			break;
		case State.StartMonitorLppoPresure:
			progressBar.Minimum = 0;
			progressBar.Maximum = 600;
			progressBar.Value = 0;
			progressBarTimer.Interval = (int)TimeSpan.FromSeconds(1.0).TotalMilliseconds;
			monitorLppoPresureTimer.Interval = (int)TimeSpan.FromMinutes(10.0).TotalMilliseconds;
			monitorLppoPresureTimer.Start();
			progressBarTimer.Start();
			initialLppoPressureReading = GetInstrumentValue(lppoPressure);
			AddLogLabel(Resources.Message_WaitingTenMinutesWhileThePressureIsMonitoredForADropOfMoreThan5Psi);
			break;
		}
		UpdateUserInterface();
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnEngineSpeedUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void AirPressureStabilizeTimer_Tick(object sender, EventArgs e)
	{
		airPressureStabilizeTimer.Stop();
		currentState = State.StartMonitorLppoPresure;
		GoMachine();
	}

	private void MonitorLppoPresureTimer_Tick(object sender, EventArgs e)
	{
		monitorLppoPresureTimer.Stop();
		StopTest(Resources.Message_TestPassedNoLeakDetected);
	}

	private void LppoPressure_InstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		if (currentState == State.StartMonitorLppoPresure)
		{
			SampleFuelPressure();
		}
	}

	private void ProgressBarTimer_Tick(object sender, EventArgs e)
	{
		progressBar.Value++;
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
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		engineCheckTableLayoutPanel = new TableLayoutPanel();
		engineSpeedCheck = new Checkmark();
		engineStatusLabel = new Label();
		seekTimeListView = new SeekTimeListView();
		chartInstrument = new ChartInstrument();
		fuelCompensationPressureInstrument = new DigitalReadoutInstrument();
		progressBar = new ProgressBar();
		buttonStop = new Button();
		buttonStart = new Button();
		((Control)(object)engineCheckTableLayoutPanel).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(engineCheckTableLayoutPanel, "engineCheckTableLayoutPanel");
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
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)chartInstrument, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)fuelCompensationPressureInstrument, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(progressBar, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)engineCheckTableLayoutPanel, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStop, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStart, 0, 3);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "FIS Low Pressure Leak Test";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss:f";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)chartInstrument, 3);
		componentResourceManager.ApplyResources(chartInstrument, "chartInstrument");
		((Collection<Qualifier>)(object)chartInstrument.Instruments).Add(new Qualifier((QualifierTypes)1, "MCM", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure"));
		((Control)(object)chartInstrument).Name = "chartInstrument";
		chartInstrument.SelectedTime = null;
		chartInstrument.ShowButtonPanel = false;
		chartInstrument.ShowEvents = false;
		chartInstrument.ShowLegend = false;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)fuelCompensationPressureInstrument, 2);
		componentResourceManager.ApplyResources(fuelCompensationPressureInstrument, "fuelCompensationPressureInstrument");
		fuelCompensationPressureInstrument.FontGroup = null;
		((SingleInstrumentBase)fuelCompensationPressureInstrument).FreezeValue = false;
		((SingleInstrumentBase)fuelCompensationPressureInstrument).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure");
		((Control)(object)fuelCompensationPressureInstrument).Name = "fuelCompensationPressureInstrument";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)fuelCompensationPressureInstrument, 2);
		((SingleInstrumentBase)fuelCompensationPressureInstrument).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(progressBar, "progressBar");
		progressBar.Name = "progressBar";
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("PanelFISLowPressureLeakTest");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)engineCheckTableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
