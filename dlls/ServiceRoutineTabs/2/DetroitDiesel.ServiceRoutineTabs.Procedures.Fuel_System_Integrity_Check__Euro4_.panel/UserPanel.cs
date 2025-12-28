using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Fuel_System_Integrity_Check__Euro4_.panel;

public class UserPanel : CustomPanel
{
	private enum TestSteps
	{
		notRunning,
		testStarted,
		waitingForEngineToStart,
		idleToWarmUpEngine,
		rampUpIdle,
		startIdleTimer,
		idleSteadyState,
		rampDownIdle,
		idleRampDownComplete,
		overrideCalibrationForLeakDetectionTestStart,
		setRailPressure,
		startRailPressureTimer,
		resetIdleBeforeShutdownEngine,
		resetRailPressureBeforeShutdownEngine,
		cutAllCylinders,
		waitingToStartLeakTest,
		runningLeakTest,
		leakTestResults,
		resetTest,
		resetIdle,
		resetRailPressure,
		resetOverrideCalibration,
		resetAllCyliners,
		resetComplete,
		manualTestStart,
		manualTestRunning
	}

	private enum FuelSystemTests
	{
		AutomaticFsicTest,
		LeakDetectionTest
	}

	private enum EngineState
	{
		Unknown = -1,
		Stopped,
		StarterEngaged,
		Other,
		Idle
	}

	private class IdleMonitor
	{
		private const int IdleLevelTime = 10;

		private DateTime setIdleStopTime;

		private int idleLevelRange;

		private int minIdleSpeed;

		private int maxIdleSpeed;

		internal void ResetMinMax()
		{
			minIdleSpeed = int.MaxValue;
			maxIdleSpeed = int.MinValue;
		}

		internal void CalculateIdleVariation(int engineSpeed)
		{
			if (engineSpeed < minIdleSpeed)
			{
				minIdleSpeed = engineSpeed;
			}
			if (engineSpeed > maxIdleSpeed)
			{
				maxIdleSpeed = engineSpeed;
			}
			if (idleLevelRange < maxIdleSpeed - minIdleSpeed)
			{
				idleLevelRange = maxIdleSpeed - minIdleSpeed;
			}
		}

		internal bool IdleIsStable(InstrumentValueCollection engineSpeeds)
		{
			bool result = false;
			TimeSpan timeSpan = DateTime.Now - setIdleStopTime;
			if (timeSpan > TimeSpan.FromSeconds(10.0))
			{
				InstrumentValue current = engineSpeeds.Current;
				int num = Convert.ToInt32(current.Value);
				InstrumentValue instrumentValue = engineSpeeds.GetCurrentAtTime(DateTime.Now - TimeSpan.FromSeconds(10.0)) ?? current;
				int num2 = Convert.ToInt32(instrumentValue.Value);
				if (Math.Abs(num - num2) <= idleLevelRange)
				{
					result = true;
				}
			}
			return result;
		}

		internal void InitIdleMonitor()
		{
			idleLevelRange = 0;
			ResetMinMax();
		}

		internal void BeginIdleStabilityMonitoring()
		{
			setIdleStopTime = DateTime.Now;
		}
	}

	private class FuelTemperatureTest
	{
		private const int LowFuelTemperatureSampleIdleSpeed = 850;

		private const int HighFuelTemperatureSampleIdleSpeed = 1800;

		private float minFuelTemperature;

		private float maxFuelTemperature;

		internal void InitFuelTemperatureTest()
		{
			minFuelTemperature = float.MaxValue;
			maxFuelTemperature = float.MinValue;
		}

		internal void CollectTemperatures(float fuelTemperature, TestSteps testStep, int idleSpeedIndex, int[] idleSpeeds)
		{
			if (testStep != TestSteps.idleSteadyState)
			{
				return;
			}
			if (idleSpeeds[idleSpeedIndex] == 850)
			{
				if (fuelTemperature < minFuelTemperature)
				{
					minFuelTemperature = fuelTemperature;
				}
			}
			else if (idleSpeeds[idleSpeedIndex] == 1800 && fuelTemperature > maxFuelTemperature)
			{
				maxFuelTemperature = fuelTemperature;
			}
		}

		internal string ReportChangeInFuelTemperature(Instrument fuelTemp)
		{
			Conversion conversion = Converter.GlobalInstance.GetConversion(fuelTemp.Units);
			float num;
			float num2;
			string text;
			if (conversion != null && conversion.OutputUnit != fuelTemp.Units)
			{
				num = (float)conversion.Convert((double)minFuelTemperature);
				num2 = (float)conversion.Convert((double)maxFuelTemperature);
				text = conversion.OutputUnit.ToString();
			}
			else
			{
				num = minFuelTemperature;
				num2 = maxFuelTemperature;
				text = fuelTemp.Units.ToString();
			}
			string arg = Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)num, text, fuelTemp.Precision) + text;
			string arg2 = Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)num2, text, fuelTemp.Precision) + text;
			string arg3 = Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)(num2 - num), text, fuelTemp.Precision) + text;
			return string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TheMinimumFuelTemperatureAt0RPMWas1 + "\r\n", 850, arg) + string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TheMaximumFuelTemperatureAt0RPMWas1 + "\r\n", 1800, arg2) + string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TheChangeInFuelTemperatureIs0 + "\r\n", arg3);
		}
	}

	private class DisplayCounter
	{
		private Timer updateCounter;

		private static readonly TimeSpan updateInterval = TimeSpan.FromSeconds(1.0);

		private TimeSpan duration;

		private ScalingLabel outputText;

		private ProgressBar progress;

		private DateTime endTime;

		internal TimeSpan Duration
		{
			set
			{
				if (value < updateInterval)
				{
					throw new ArgumentOutOfRangeException("The total time cannot be smaller than the display interval.");
				}
				duration = value;
				progress.Maximum = (int)duration.TotalSeconds;
			}
		}

		internal int Value
		{
			get
			{
				TimeSpan timeSpan = endTime - DateTime.Now;
				if (timeSpan < new TimeSpan(0L))
				{
					return 0;
				}
				return (int)timeSpan.TotalSeconds;
			}
		}

		private DisplayCounter()
		{
		}

		internal DisplayCounter(TimeSpan duration, ScalingLabel label, ProgressBar progress)
		{
			updateCounter = new Timer();
			updateCounter.Interval = (int)TimeSpan.FromSeconds(1.0).TotalMilliseconds;
			this.duration = duration;
			outputText = label;
			this.progress = progress;
			endTime = DateTime.Now + duration;
			Reset();
		}

		internal void StartCountDown()
		{
			endTime = DateTime.Now + duration;
			updateCounter.Tick += UpdateCounterTick;
			updateCounter.Start();
			ShowValue();
		}

		internal void StopCountDown()
		{
			if (updateCounter != null && updateCounter.Enabled)
			{
				updateCounter.Stop();
			}
			updateCounter.Tick -= UpdateCounterTick;
			progress.Value = 0;
			((Control)(object)outputText).Text = string.Empty;
		}

		internal void Reset()
		{
			StopCountDown();
			progress.Minimum = 0;
			progress.Maximum = (int)duration.TotalSeconds;
		}

		internal void ShowValue()
		{
			((Control)(object)outputText).Text = Value.ToString(CultureInfo.CurrentCulture);
			progress.Value = progress.Maximum - Value;
		}

		internal void UpdateCounterTick(object sender, EventArgs e)
		{
			ShowValue();
		}
	}

	internal class StopEngineProcedure
	{
		internal delegate void ProcedureCompleteEventHandler(object sender, ResultEventArgs e);

		private const int CylinderCount = 6;

		private const string CylinderCutStartStopRoutine = "RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder";

		private const string AllCylindersOnRoutine = "RT_SR004_Engine_Cylinder_Cut_Off_Stop";

		private const string EngineSpeedInstrumentQualifier = "DT_ASL002_Engine_Speed";

		private Channel channel;

		internal ProcedureCompleteEventHandler OnProcedureCompleteEvent;

		private string serviceExecuteList;

		internal Channel Mcm
		{
			set
			{
				if (channel != value)
				{
					if (channel != null)
					{
						DisconnectInstrumentUpdateEvent();
					}
					channel = value;
				}
			}
		}

		private string ServiceExecuteList
		{
			get
			{
				if (string.IsNullOrEmpty(serviceExecuteList))
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 1; i <= 6; i++)
					{
						stringBuilder.AppendFormat("RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder({0},0);", i);
					}
					serviceExecuteList = stringBuilder.ToString();
				}
				return serviceExecuteList;
			}
		}

		private StopEngineProcedure()
		{
		}

		internal StopEngineProcedure(Channel channel)
		{
			this.channel = channel;
		}

		private void DisconnectInstrumentUpdateEvent()
		{
			Instrument instrument = channel.Instruments["DT_ASL002_Engine_Speed"];
			if (instrument != null)
			{
				instrument.InstrumentUpdateEvent -= OnEngineSpeedInstrumentUpdateEvent;
			}
		}

		internal bool SetCylindersOff(ProcedureCompleteEventHandler onProcedureCompleteEvent)
		{
			OnProcedureCompleteEvent = onProcedureCompleteEvent;
			if (OnProcedureCompleteEvent != null)
			{
				Instrument instrument = channel.Instruments["DT_ASL002_Engine_Speed"];
				if (!(instrument != null))
				{
					return false;
				}
				instrument.InstrumentUpdateEvent += OnEngineSpeedInstrumentUpdateEvent;
			}
			int num = channel.Services.Execute(ServiceExecuteList, synchronous: true);
			return num == 6;
		}

		private void OnEngineSpeedInstrumentUpdateEvent(object sender, ResultEventArgs e)
		{
			Instrument instrument = sender as Instrument;
			object instrumentCurrentValue = GetInstrumentCurrentValue(instrument);
			if (instrumentCurrentValue != null && Convert.ToInt32(instrumentCurrentValue) == 0)
			{
				instrument.InstrumentUpdateEvent -= OnEngineSpeedInstrumentUpdateEvent;
				OnProcedureCompleteEvent(OnProcedureCompleteEvent, e);
			}
		}

		internal void SetCylindersOn(ServiceCompleteEventHandler onServiceCompleteEvent)
		{
			Service service = channel.Services["RT_SR004_Engine_Cylinder_Cut_Off_Stop"];
			service.ServiceCompleteEvent += onServiceCompleteEvent.Invoke;
			service.Execute(synchronous: false);
		}
	}

	private const string IdleSpeedSetRoutine = "RT_SR015_Idle_Speed_Modification_Start";

	private const string IdleSpeedResetRoutine = "RT_SR015_Idle_Speed_Modification_Stop";

	private const string SwRoutineProductionStart = "RT_SR005_SW_Routine_for_Production_Start_SW_Operation";

	private const string SwRoutineProductionStop = "RT_SR005_SW_Routine_for_Production_Stop";

	private const string CalibrationOverrideForLeakDetectionTestStart = "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start";

	private const string CalibrationOverrideForLeakDetectionTestStop = "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop";

	private const string RailPressureSetRoutine = "RT_SR076_Desired_Rail_Pressure_Start_Status";

	private const string RailPressureResetRoutine = "RT_SR076_Desired_Rail_Pressure_Stop";

	private const string MCMName = "MCM";

	private const double LeakTestPreTestPressure = 800.0;

	private const double LeakTestAltPreTestPressure = 400.0;

	private const int LeakTestAltIdleSpeed = 1400;

	private const double LeakTestStartThresholdPressure = 280.0;

	private const double LeakTestStopThresholdPressure = 10.0;

	private static readonly TimeSpan TestHoldStateDuration = TimeSpan.FromSeconds(20.0);

	private static readonly TimeSpan AutoFsicTestIdleToWarmUpEngineDuration = TimeSpan.FromMinutes(2.0);

	private static readonly TimeSpan LeakTestIdleToWarmUpEngineDuration = TimeSpan.FromMinutes(1.0);

	private int[] IdleSpeeds = new int[6] { 600, 850, 950, 1100, 1500, 1800 };

	private Timer commandTimer;

	private TestSteps testStep;

	private int idleSpeedIndex;

	private double leakTestPreTestPressure;

	private FuelTemperatureTest fuelTempTest;

	private IdleMonitor idleMonitor;

	private DisplayCounter displayCounter;

	private StopEngineProcedure stopEngineProcedure;

	private DateTime injectorLeakTestStart;

	private Channel mcm;

	private Instrument engineSpeedInstrument;

	private Instrument railPressureInstrument;

	private Instrument engineStateInstrument;

	private Instrument fuelTemperatureInstrument;

	private WarningManager warningManager;

	private string TestCannotRunMcmIsOfflineString = Resources.Message_CannotRunTheMCMIsOffline;

	private string TestIsRunningString = Resources.Message_IsRunning;

	private string TestIsReadyString = Resources.Message_IsReadyToBeRun;

	private bool leakDetectionTestRunning;

	private SplitContainer splitContainerWholePanel;

	private TableLayoutPanel tableLayoutPanelLeftColumn;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private DigitalReadoutInstrument digitalReadoutInstrumentActualFuelMass;

	private DigitalReadoutInstrument digitalReadoutInstrumentRailPressure;

	private DigitalReadoutInstrument digitalReadoutInstrumentDesiredRailPressure;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuelCompensationPressure;

	private DigitalReadoutInstrument digitalReadoutInstrumentQuantityControlValveCurrent;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleStatus;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineState;

	private TableLayoutPanel tableLayoutPanelRightColumn;

	private TableLayoutPanel tableLayoutPanelTestControls;

	private Button buttonRunTest;

	private Checkmark checkmarkTestCanProceedStatus;

	private ChartInstrument chartInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrumentFuelTemperature;

	private DigitalReadoutInstrument digitalReadoutInstrumentCoolantTemperature;

	private ScalingLabel scalingLabelTimerDisplay;

	private ProgressBar progressBarTest;

	private DigitalReadoutInstrument digitalReadoutInstrumentKwNwValiditySignal;

	private Button buttonStop;

	private SeekTimeListView seekTimeListView;

	private ComboBox comboBoxSelectTest;

	private System.Windows.Forms.Label labelTestStartStatus;

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private bool TestIsRunning => Online && testStep != TestSteps.notRunning;

	private bool CanSelectTest => Online && testStep == TestSteps.notRunning;

	private string SelectedTestName => comboBoxSelectTest.SelectedItem.ToString();

	private bool Online
	{
		get
		{
			bool result = false;
			if (mcm != null && mcm.CommunicationsState == CommunicationsState.Online)
			{
				result = true;
			}
			return result;
		}
	}

	private EngineState EngineStatus
	{
		get
		{
			EngineState result = EngineState.Unknown;
			object instrumentCurrentValue = GetInstrumentCurrentValue(engineStateInstrument);
			if (instrumentCurrentValue != null)
			{
				result = ((instrumentCurrentValue != engineStateInstrument.Choices.GetItemFromRawValue(EngineState.Stopped)) ? ((instrumentCurrentValue == engineStateInstrument.Choices.GetItemFromRawValue(EngineState.StarterEngaged)) ? EngineState.StarterEngaged : ((instrumentCurrentValue == engineStateInstrument.Choices.GetItemFromRawValue(EngineState.Idle)) ? EngineState.Idle : ((instrumentCurrentValue != engineStateInstrument.Choices.GetItemFromRawValue(EngineState.Unknown)) ? EngineState.Other : EngineState.Unknown))) : EngineState.Stopped);
			}
			return result;
		}
	}

	private bool CanClose => !Online || testStep == TestSteps.notRunning || testStep == TestSteps.manualTestRunning;

	public UserPanel()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		InitializeComponent();
		warningManager = new WarningManager(string.Empty, Resources.Message_Test, seekTimeListView.RequiredUserLabelPrefix);
		comboBoxSelectTest.SelectedIndex = 0;
	}

	private void InitializeChart()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		Qualifier[] array = (Qualifier[])(object)new Qualifier[8]
		{
			new Qualifier((QualifierTypes)1, "virtual", "engineSpeed"),
			new Qualifier((QualifierTypes)1, "MCM", "DT_AS087_Actual_Fuel_Mass"),
			new Qualifier((QualifierTypes)1, "MCM", "DT_AS098_desired_rail_pressure"),
			new Qualifier((QualifierTypes)1, "MCM", "DT_AS043_Rail_Pressure"),
			new Qualifier((QualifierTypes)1, "MCM", "DT_AS100_Quantity_Control_Valve_Current"),
			new Qualifier((QualifierTypes)1, "MCM", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure"),
			new Qualifier((QualifierTypes)1, "MCM", "DT_DS001_KW_NW_validity_signal"),
			new Qualifier((QualifierTypes)1, "virtual", "fuelTemp")
		};
		((NotifyCollection<Qualifier>)(object)chartInstrument1.Instruments).AddRange((IEnumerable)array);
	}

	private void InitializeTest()
	{
		idleSpeedIndex = 0;
		fuelTempTest.InitFuelTemperatureTest();
		idleMonitor.InitIdleMonitor();
		displayCounter.Reset();
	}

	private void ConnectFormControlEventHandlers(bool connect)
	{
		if (connect)
		{
			buttonRunTest.Click += OnButtonRunTestClick;
			buttonStop.Click += OnButtonStopClick;
			comboBoxSelectTest.SelectedIndexChanged += ComboBoxSelectTestSelectedIndexChanged;
			comboBoxSelectTest.SelectedIndex = 0;
			digitalReadoutInstrumentEngineState.RepresentedStateChanged += OnDigitalReadoutInstrumentRepresentedStateChanged;
			digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged += OnDigitalReadoutInstrumentRepresentedStateChanged;
			digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged += digitalReadoutInstrumentVehicleStatus_RepresentedStateChanged;
			digitalReadoutInstrumentCoolantTemperature.RepresentedStateChanged += OnDigitalReadoutInstrumentRepresentedStateChanged;
			digitalReadoutInstrumentFuelTemperature.RepresentedStateChanged += OnDigitalReadoutInstrumentRepresentedStateChanged;
			seekTimeListView.TimeActivate += SeekTimeListViewTimeActivate;
			seekTimeListView.SelectedTimeChanged += SeekTimeListViewSelectedTimeChanged;
			return;
		}
		if (buttonRunTest != null)
		{
			buttonRunTest.Click -= OnButtonRunTestClick;
		}
		if (buttonStop != null)
		{
			buttonStop.Click -= OnButtonStopClick;
		}
		if (comboBoxSelectTest != null)
		{
			comboBoxSelectTest.SelectedIndexChanged -= ComboBoxSelectTestSelectedIndexChanged;
		}
		if (digitalReadoutInstrumentEngineState != null)
		{
			digitalReadoutInstrumentEngineState.RepresentedStateChanged -= OnDigitalReadoutInstrumentRepresentedStateChanged;
		}
		if (digitalReadoutInstrumentVehicleStatus != null)
		{
			digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged -= OnDigitalReadoutInstrumentRepresentedStateChanged;
			digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged -= digitalReadoutInstrumentVehicleStatus_RepresentedStateChanged;
		}
		if (digitalReadoutInstrumentCoolantTemperature != null)
		{
			digitalReadoutInstrumentCoolantTemperature.RepresentedStateChanged -= OnDigitalReadoutInstrumentRepresentedStateChanged;
		}
		if (digitalReadoutInstrumentFuelTemperature != null)
		{
			digitalReadoutInstrumentFuelTemperature.RepresentedStateChanged -= OnDigitalReadoutInstrumentRepresentedStateChanged;
		}
		if (seekTimeListView != null)
		{
			seekTimeListView.TimeActivate -= SeekTimeListViewTimeActivate;
			seekTimeListView.SelectedTimeChanged -= SeekTimeListViewSelectedTimeChanged;
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		commandTimer = new Timer();
		fuelTempTest = new FuelTemperatureTest();
		idleMonitor = new IdleMonitor();
		displayCounter = new DisplayCounter(AutoFsicTestIdleToWarmUpEngineDuration, scalingLabelTimerDisplay, progressBarTest);
		stopEngineProcedure = new StopEngineProcedure(mcm);
		ConnectFormControlEventHandlers(connect: true);
		InitializeChart();
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			if (((ContainerControl)this).ParentForm != null)
			{
				((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			}
			ConnectFormControlEventHandlers(connect: false);
			SetMCM(null);
			((Control)this).Tag = new object[2] { false, seekTimeListView.Text };
		}
	}

	private Channel GetActiveChannel(string name)
	{
		foreach (Channel activeChannel in SapiManager.GlobalInstance.ActiveChannels)
		{
			if (activeChannel.Ecu.Name == name)
			{
				return activeChannel;
			}
		}
		return null;
	}

	public override void OnChannelsChanged()
	{
		SetMCM(GetActiveChannel("MCM"));
		UpdateUserInterface();
	}

	private static void DisconnectInstrument(Instrument instrument, InstrumentUpdateEventHandler updateEventHandler)
	{
		if (instrument != null)
		{
			instrument.InstrumentUpdateEvent -= updateEventHandler.Invoke;
		}
	}

	private static Instrument ConnectInstrument(InstrumentCollection instruments, string qualifier, InstrumentUpdateEventHandler updateEventHandler)
	{
		Instrument instrument = instruments[qualifier];
		if (instrument != null)
		{
			instrument.InstrumentUpdateEvent += updateEventHandler.Invoke;
		}
		return instrument;
	}

	private void SetMCM(Channel mcm)
	{
		if (this.mcm != mcm)
		{
			warningManager.Reset();
			if (commandTimer != null && commandTimer.Enabled)
			{
				StopCommandTimer();
			}
			if (testStep != TestSteps.notRunning)
			{
				ReportResult(Resources.Message_CancellingCurrentTestBecauseTheMCMHasBecomeDisconnected);
			}
			EndTest();
			if (this.mcm != null)
			{
				DisconnectInstrument(engineSpeedInstrument, OnEngineSpeedInstrumentUpdateEvent);
				DisconnectInstrument(railPressureInstrument, OnRailPressureInstrumentUpdateEvent);
				DisconnectInstrument(engineStateInstrument, OnEngineStateInstrumentUpdateEvent);
				DisconnectInstrument(fuelTemperatureInstrument, OnFuelTemperatureInstrumentUpdateEvent);
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			this.mcm = mcm;
			if (stopEngineProcedure != null)
			{
				stopEngineProcedure.Mcm = mcm;
			}
			if (this.mcm != null)
			{
				engineSpeedInstrument = ConnectInstrument(this.mcm.Instruments, "DT_ASL002_Engine_Speed", OnEngineSpeedInstrumentUpdateEvent);
				railPressureInstrument = ConnectInstrument(this.mcm.Instruments, "DT_AS043_Rail_Pressure", OnRailPressureInstrumentUpdateEvent);
				engineStateInstrument = ConnectInstrument(this.mcm.Instruments, "DT_AS023_Engine_State", OnEngineStateInstrumentUpdateEvent);
				fuelTemperatureInstrument = ConnectInstrument(this.mcm.Instruments, "DT_ASL005_Fuel_Temperature", OnFuelTemperatureInstrumentUpdateEvent);
				this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		bool testCanStart = true;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Resources.Message_The + SelectedTestName);
		if (!Online)
		{
			testCanStart = false;
			stringBuilder.Append(TestCannotRunMcmIsOfflineString);
		}
		else
		{
			switch (comboBoxSelectTest.SelectedIndex)
			{
			case 0:
				if (TestIsRunning)
				{
					testCanStart = false;
					if (testStep == TestSteps.waitingForEngineToStart)
					{
						if (EngineStatus == EngineState.Idle || EngineStatus == EngineState.StarterEngaged)
						{
							checkmarkTestCanProceedStatus.Checked = true;
							break;
						}
						checkmarkTestCanProceedStatus.Checked = false;
						checkmarkTestCanProceedStatus.CheckState = CheckState.Indeterminate;
						stringBuilder.Append(Resources.Message_CannotRunItIsWaitingForYouToStartTheEngine);
					}
					else
					{
						stringBuilder.Append(TestIsRunningString);
					}
				}
				else
				{
					stringBuilder.Append(AutomaticFsicTestInstrumentsReadyToStartTest(ref testCanStart));
				}
				break;
			case 1:
				if (TestIsRunning)
				{
					testCanStart = false;
					stringBuilder.Append(TestIsRunningString);
				}
				else
				{
					stringBuilder.Append(LeakTestInstrumentsReadyToStartTest(ref testCanStart));
				}
				break;
			default:
				throw new IndexOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "The selected test index value ({0}) is undefined. ", comboBoxSelectTest.SelectedIndex));
			}
		}
		buttonStop.Enabled = Online;
		comboBoxSelectTest.Enabled = CanSelectTest;
		checkmarkTestCanProceedStatus.Checked = testCanStart || TestIsRunning;
		buttonRunTest.Enabled = !TestIsRunning && testCanStart;
		labelTestStartStatus.Text = stringBuilder.ToString();
		labelTestStartStatus.Enabled = Online;
	}

	private string AutomaticFsicTestInstrumentsReadyToStartTest(ref bool testCanStart)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Invalid comparison between Unknown and I4
		StringBuilder stringBuilder = new StringBuilder();
		if (EngineStatus != EngineState.Stopped)
		{
			testCanStart = false;
			stringBuilder.Append(Resources.Message_CannotStartBecauseTheEngineIsNotStoppedPleaseStopTheEngine);
		}
		else if ((int)digitalReadoutInstrumentVehicleStatus.RepresentedState == 3)
		{
			testCanStart = false;
			stringBuilder.Append(Resources.Message_CannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL);
		}
		else if ((int)digitalReadoutInstrumentCoolantTemperature.RepresentedState == 3)
		{
			testCanStart = false;
			string minimumOKValueString = GetMinimumOKValueString(digitalReadoutInstrumentCoolantTemperature, mcm.Instruments["DT_ASL005_Coolant_Temperature"]);
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotStartBecauseTheCoolantTemperatureIs0, string.IsNullOrEmpty(minimumOKValueString) ? Resources.Message_TooLow : (Resources.Message_LessThan1 + minimumOKValueString));
		}
		else if ((int)digitalReadoutInstrumentFuelTemperature.RepresentedState == 3)
		{
			testCanStart = false;
			string minimumOKValueString = GetMinimumOKValueString(digitalReadoutInstrumentFuelTemperature, fuelTemperatureInstrument);
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotStartBecauseTheFuelTemperatureIs0, string.IsNullOrEmpty(minimumOKValueString) ? Resources.Message_TooLow1 : (Resources.Message_LessThan + minimumOKValueString));
		}
		else
		{
			testCanStart = true;
			stringBuilder.Append(TestIsReadyString);
		}
		return stringBuilder.ToString();
	}

	private string LeakTestInstrumentsReadyToStartTest(ref bool testCanStart)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		StringBuilder stringBuilder = new StringBuilder();
		if ((int)digitalReadoutInstrumentVehicleStatus.RepresentedState == 3)
		{
			testCanStart = false;
			stringBuilder.Append(Resources.Message_CannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL1);
		}
		else if ((int)digitalReadoutInstrumentCoolantTemperature.RepresentedState == 3)
		{
			testCanStart = false;
			string minimumOKValueString = GetMinimumOKValueString(digitalReadoutInstrumentCoolantTemperature, mcm.Instruments["DT_ASL005_Coolant_Temperature"]);
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotStartBecauseTheCoolantTemperatureIs01, string.IsNullOrEmpty(minimumOKValueString) ? Resources.Message_TooLow2 : (Resources.Message_LessThan2 + minimumOKValueString));
		}
		else
		{
			testCanStart = true;
			stringBuilder.Append(TestIsReadyString);
		}
		return stringBuilder.ToString();
	}

	private void OnDigitalReadoutInstrumentRepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void digitalReadoutInstrumentVehicleStatus_RepresentedStateChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)digitalReadoutInstrumentVehicleStatus.RepresentedState == 3 && TestIsRunning)
		{
			ReportResult(Resources.Message_Error_StoppedTest_VehicleCheckStatus);
			CancelTest();
		}
	}

	private void OnFuelTemperatureInstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		RecordInstrumentValue();
	}

	private void RecordInstrumentValue()
	{
		object instrumentCurrentValue = GetInstrumentCurrentValue(fuelTemperatureInstrument);
		if (instrumentCurrentValue != null)
		{
			float fuelTemperature = Convert.ToSingle(instrumentCurrentValue);
			fuelTempTest.CollectTemperatures(fuelTemperature, testStep, idleSpeedIndex, IdleSpeeds);
		}
	}

	private void OnEngineSpeedInstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		object instrumentCurrentValue = GetInstrumentCurrentValue(engineSpeedInstrument);
		if (instrumentCurrentValue == null)
		{
			return;
		}
		int num = Convert.ToInt32(instrumentCurrentValue);
		switch (testStep)
		{
		case TestSteps.idleSteadyState:
			idleMonitor.CalculateIdleVariation(num);
			break;
		case TestSteps.startIdleTimer:
			if (num >= IdleSpeeds[idleSpeedIndex])
			{
				RunFuelSystemCheck();
			}
			break;
		case TestSteps.idleRampDownComplete:
			if (idleMonitor.IdleIsStable(engineSpeedInstrument.InstrumentValues))
			{
				RunFuelSystemCheck();
			}
			break;
		case TestSteps.rampDownIdle:
			break;
		}
	}

	private void OnRailPressureInstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		object instrumentCurrentValue = GetInstrumentCurrentValue(railPressureInstrument);
		if (instrumentCurrentValue == null)
		{
			return;
		}
		double num = Convert.ToDouble(instrumentCurrentValue);
		switch (testStep)
		{
		case TestSteps.startRailPressureTimer:
			if (num >= leakTestPreTestPressure)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_RailPressureSetTo0, leakTestPreTestPressure));
				RunFuelSystemCheck();
			}
			break;
		case TestSteps.runningLeakTest:
			if (num <= 280.0)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_RailPressureBleedOffTestRunningCurrentRailPressure000BarAbs, num));
				injectorLeakTestStart = DateTime.Now;
				RunFuelSystemCheck();
			}
			break;
		case TestSteps.leakTestResults:
			if (num <= 10.0)
			{
				TimeSpan time = DateTime.Now - injectorLeakTestStart;
				string text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TimeElapsedForTheFuelPressureToLeakDownFrom0To1BarAbs2, 280.0, 10.0, DisplayTime(time));
				ReportResult(text);
				RunFuelSystemCheck();
			}
			break;
		}
	}

	private void OnEngineStateInstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		if (testStep == TestSteps.waitingForEngineToStart && EngineStatus == EngineState.Idle)
		{
			ReportResult(Resources.Message_EngineStarted);
			RunFuelSystemCheck();
		}
		UpdateUserInterface();
	}

	private void StartCommandTimer()
	{
		string text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Waiting0, DisplayTime(TimeSpan.FromMilliseconds(commandTimer.Interval)));
		ReportResult(text);
		labelTestStartStatus.Text = text;
		((Control)(object)this).Cursor = Cursors.WaitCursor;
		commandTimer.Tick += OnCommandTimerTick;
		displayCounter.Duration = TimeSpan.FromMilliseconds(commandTimer.Interval);
		displayCounter.StartCountDown();
		commandTimer.Start();
	}

	internal void StopCommandTimer()
	{
		commandTimer.Stop();
		commandTimer.Tick -= OnCommandTimerTick;
		displayCounter.Reset();
		((Control)(object)this).Cursor = Cursors.Default;
	}

	private void OnCommandTimerTick(object sender, EventArgs e)
	{
		StopCommandTimer();
		RunFuelSystemCheck();
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

	private void AdvanceTestStep()
	{
		if (testStep != TestSteps.notRunning)
		{
			testStep++;
			return;
		}
		throw new InvalidOperationException("Step sequence out of order.");
	}

	private void RunFuelSystemCheck()
	{
		switch (testStep)
		{
		case TestSteps.notRunning:
			break;
		case TestSteps.testStarted:
			idleSpeedIndex = 0;
			AdvanceTestStep();
			RunFuelSystemCheck();
			break;
		case TestSteps.waitingForEngineToStart:
			WaitForEngineToStart();
			break;
		case TestSteps.idleToWarmUpEngine:
			if (leakDetectionTestRunning)
			{
				testStep = TestSteps.overrideCalibrationForLeakDetectionTestStart;
				commandTimer.Interval = (int)LeakTestIdleToWarmUpEngineDuration.TotalMilliseconds;
			}
			else
			{
				AdvanceTestStep();
				commandTimer.Interval = (int)AutoFsicTestIdleToWarmUpEngineDuration.TotalMilliseconds;
			}
			StartCommandTimer();
			break;
		case TestSteps.rampUpIdle:
			AdvanceTestStep();
			SetIdleStart(IdleSpeeds[idleSpeedIndex]);
			break;
		case TestSteps.startIdleTimer:
			AdvanceTestStep();
			RecordInstrumentValue();
			commandTimer.Interval = (int)TestHoldStateDuration.TotalMilliseconds;
			StartCommandTimer();
			break;
		case TestSteps.idleSteadyState:
			idleMonitor.ResetMinMax();
			if (idleSpeedIndex < IdleSpeeds.Length - 1)
			{
				idleSpeedIndex++;
				testStep = TestSteps.rampUpIdle;
			}
			else
			{
				testStep = TestSteps.rampDownIdle;
			}
			RunFuelSystemCheck();
			break;
		case TestSteps.rampDownIdle:
			AdvanceTestStep();
			SetIdleStopWaitUntilEngineSpeedIsStable();
			break;
		case TestSteps.idleRampDownComplete:
			AdvanceTestStep();
			RunFuelSystemCheck();
			break;
		case TestSteps.overrideCalibrationForLeakDetectionTestStart:
			AdvanceTestStep();
			commandTimer.Interval = (int)TestHoldStateDuration.TotalMilliseconds;
			OverrideCalibrationForLeakDetectionTestStart();
			break;
		case TestSteps.setRailPressure:
			AdvanceTestStep();
			leakTestPreTestPressure = 800.0;
			SetRailPressureStart(leakTestPreTestPressure);
			break;
		case TestSteps.startRailPressureTimer:
			AdvanceTestStep();
			StartCommandTimer();
			break;
		case TestSteps.resetIdleBeforeShutdownEngine:
			AdvanceTestStep();
			SetIdleStop();
			break;
		case TestSteps.resetRailPressureBeforeShutdownEngine:
			AdvanceTestStep();
			SetRailPressureStop();
			break;
		case TestSteps.cutAllCylinders:
		{
			AdvanceTestStep();
			ReportResult(Resources.Message_ShuttingOffEngine);
			bool flag;
			try
			{
				flag = stopEngineProcedure.SetCylindersOff(OnProcedureCompleteEvent);
			}
			catch (InvalidOperationException ex)
			{
				flag = false;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Error0, ex.Message));
			}
			catch (CaesarException ex2)
			{
				flag = false;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Error01, ex2.Message));
			}
			if (!flag)
			{
				ReportResult(Resources.Message_ErrorShuttingOffTheEngineEndingTheTest);
				StopTest();
			}
			break;
		}
		case TestSteps.waitingToStartLeakTest:
			AdvanceTestStep();
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForRailPressureToReach0BarAbsToStartLeakTest, 280.0));
			break;
		case TestSteps.runningLeakTest:
			AdvanceTestStep();
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_RailPressureBleedOffTestRunningWaitingForRailPressureToReach0BarAbsToCompleteLeakTest, 10.0));
			break;
		case TestSteps.leakTestResults:
		{
			string text = fuelTempTest.ReportChangeInFuelTemperature(fuelTemperatureInstrument);
			if (!leakDetectionTestRunning)
			{
				ReportResult(text);
			}
			ReportResult(Resources.Message_TestsComplete);
			if (leakDetectionTestRunning)
			{
				leakDetectionTestRunning = false;
			}
			AdvanceTestStep();
			RunFuelSystemCheck();
			break;
		}
		case TestSteps.resetTest:
			if (commandTimer != null && commandTimer.Enabled)
			{
				StopCommandTimer();
			}
			AdvanceTestStep();
			RunFuelSystemCheck();
			break;
		case TestSteps.resetIdle:
			AdvanceTestStep();
			SetIdleStop();
			break;
		case TestSteps.resetRailPressure:
			AdvanceTestStep();
			SetRailPressureStop();
			break;
		case TestSteps.resetOverrideCalibration:
			AdvanceTestStep();
			ResetOverrideCalibrationForLeakDetectionTest();
			break;
		case TestSteps.resetAllCyliners:
			AdvanceTestStep();
			stopEngineProcedure.SetCylindersOn(OnServiceCompleteEventNoFail);
			break;
		case TestSteps.resetComplete:
			ReportResult(Resources.Message_TestConditionsReset);
			EndTest();
			RunFuelSystemCheck();
			break;
		case TestSteps.manualTestStart:
			idleSpeedIndex = 0;
			AdvanceTestStep();
			RunFuelSystemCheck();
			break;
		case TestSteps.manualTestRunning:
			break;
		default:
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "{0} is not a defined test step.", testStep));
		}
	}

	private void CancelTest()
	{
		testStep = TestSteps.resetTest;
		RunFuelSystemCheck();
	}

	private void EndTest()
	{
		testStep = TestSteps.notRunning;
	}

	private void WaitForEngineToStart()
	{
		switch (EngineStatus)
		{
		case EngineState.Idle:
			AdvanceTestStep();
			RunFuelSystemCheck();
			break;
		case EngineState.StarterEngaged:
			ReportResult(Resources.Message_EngineStarting);
			break;
		case EngineState.Stopped:
		{
			string message_PleaseStartTheEngine = Resources.Message_PleaseStartTheEngine;
			DialogResult dialogResult = MessageBox.Show(((ContainerControl)this).ParentForm, message_PleaseStartTheEngine, Resources.Message_EngineNotAtIdle, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
			if (dialogResult != DialogResult.OK)
			{
				CancelTest();
			}
			break;
		}
		case EngineState.Other:
			ReportResult(Resources.Message_CannotProceedUntilEngineIsAtIdle1);
			break;
		case EngineState.Unknown:
			ReportResult(Resources.Message_CannotProceedUntilEngineIsAtIdle);
			break;
		default:
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Engine state not possible {0}.", EngineStatus));
		}
	}

	private void ServiceNotFoundErrorMethod(string serviceName)
	{
		if (Online)
		{
			ReportResult(Resources.Message_EndingTheTestTheService + serviceName + Resources.Message_IsNotAvailable);
		}
		else
		{
			ReportResult(Resources.Message_EndingTheTestTheMCMIsNoLongerConnected);
		}
		EndTest();
		RunFuelSystemCheck();
	}

	private void SetIdleStart(int idleSpeed)
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR015_Idle_Speed_Modification_Start");
		if (service != null)
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_SettingEngineIdleSpeedTo0, idleSpeed));
			service.InputValues[0].Value = idleSpeed;
			service.ServiceCompleteEvent += OnServiceCompleteEventNoAdvance;
			service.Execute(synchronous: false);
		}
		else
		{
			ServiceNotFoundErrorMethod("RT_SR015_Idle_Speed_Modification_Start");
		}
	}

	private void SetIdleStopWaitUntilEngineSpeedIsStable()
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR015_Idle_Speed_Modification_Stop");
		if (service != null)
		{
			idleMonitor.BeginIdleStabilityMonitoring();
			ReportResult(Resources.Message_ResettingEngineIdleSpeed);
			service.ServiceCompleteEvent += OnServiceCompleteEventNoAdvance;
			service.Execute(synchronous: false);
		}
		else
		{
			ServiceNotFoundErrorMethod("RT_SR015_Idle_Speed_Modification_Stop");
		}
	}

	private void SetIdleStop()
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR015_Idle_Speed_Modification_Stop");
		if (service != null)
		{
			ReportResult(Resources.Message_ResettingEngineIdleSpeed1);
			service.ServiceCompleteEvent += OnServiceCompleteEventNoFail;
			service.Execute(synchronous: false);
		}
		else
		{
			ServiceNotFoundErrorMethod("RT_SR015_Idle_Speed_Modification_Stop");
		}
	}

	private static double FindFirstOkGradientCellLowerBoundary(DigitalReadoutInstrument instrument)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		GradientCell val = instrument.Gradient.Cells.Where((GradientCell x) => ((GradientCell)(ref x)).IsValid && (int)((GradientCell)(ref x)).State == 1).FirstOrDefault();
		return ((GradientCell)(ref val)).LowerBoundary;
	}

	private string GetMinimumOKValueString(DigitalReadoutInstrument digitalReadoutInstrument, Instrument instrument)
	{
		string result = string.Empty;
		double num = FindFirstOkGradientCellLowerBoundary(digitalReadoutInstrument);
		if (!double.IsNaN(num))
		{
			Conversion conversion = Converter.GlobalInstance.GetConversion(instrument.Units);
			result = ((conversion == null || !(conversion.OutputUnit != instrument.Units)) ? (Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)num, instrument.Units, instrument.Precision) + instrument.Units) : (Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, (object)num, conversion, instrument.Precision) + conversion.OutputUnit));
		}
		return result;
	}

	private void OverrideCalibrationForLeakDetectionTestStart()
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start");
		if (service != null)
		{
			service.InputValues[0].Value = FindFirstOkGradientCellLowerBoundary(digitalReadoutInstrumentFuelTemperature);
			service.InputValues[1].Value = FindFirstOkGradientCellLowerBoundary(digitalReadoutInstrumentCoolantTemperature);
			service.ServiceCompleteEvent += OnServiceCompleteEvent;
			service.Execute(synchronous: false);
		}
		else
		{
			ServiceNotFoundErrorMethod("RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start");
		}
	}

	private void ResetOverrideCalibrationForLeakDetectionTest()
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop");
		if (service != null)
		{
			service.ServiceCompleteEvent += OnServiceCompleteEventNoFail;
			service.Execute(synchronous: false);
		}
		else
		{
			ServiceNotFoundErrorMethod("RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop");
		}
	}

	private void SetRailPressureStart(double pressure)
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR076_Desired_Rail_Pressure_Start_Status");
		if (service != null)
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_SettingDesiredRailPressureTo0, pressure));
			service.InputValues[0].Value = pressure;
			service.ServiceCompleteEvent += OnSetRailPressureServiceCompleteEvent;
			service.Execute(synchronous: false);
		}
		else
		{
			ServiceNotFoundErrorMethod("RT_SR076_Desired_Rail_Pressure_Start_Status");
		}
	}

	private void OnSetRailPressureServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= OnSetRailPressureServiceCompleteEvent;
		if (e.Succeeded)
		{
			leakTestPreTestPressure = 800.0;
			return;
		}
		ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ErrorSettingRailPressure0UsingIdleSpeedToIncreaseTheRailPressure, e.Exception.Message));
		SetIdleStart(1400);
		leakTestPreTestPressure = 400.0;
	}

	private void SetRailPressureStop()
	{
		Service service = ((CustomPanel)this).GetService("MCM", "RT_SR076_Desired_Rail_Pressure_Stop");
		if (service != null)
		{
			ReportResult(Resources.Message_ResettingDesiredRailPressure);
			service.ServiceCompleteEvent += OnServiceCompleteEventNoFail;
			service.Execute(synchronous: false);
		}
		else
		{
			ServiceNotFoundErrorMethod("RT_SR076_Desired_Rail_Pressure_Stop");
		}
	}

	private void OnProcedureCompleteEvent(object sender, ResultEventArgs e)
	{
		StopEngineProcedure obj = stopEngineProcedure;
		obj.OnProcedureCompleteEvent = (StopEngineProcedure.ProcedureCompleteEventHandler)Delegate.Remove(obj.OnProcedureCompleteEvent, new StopEngineProcedure.ProcedureCompleteEventHandler(OnProcedureCompleteEvent));
		if (e.Succeeded)
		{
			RunFuelSystemCheck();
			return;
		}
		ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Error02, e.Exception.Message));
		CancelTest();
	}

	private void OnServiceCompleteEventNoAdvance(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= OnServiceCompleteEventNoAdvance;
		if (!e.Succeeded)
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Error03, e.Exception.Message));
			CancelTest();
		}
	}

	private void OnServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= OnServiceCompleteEvent;
		if (e.Succeeded)
		{
			RunFuelSystemCheck();
			return;
		}
		ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Error04, e.Exception.Message));
		CancelTest();
	}

	private void OnServiceCompleteEventNoFail(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= OnServiceCompleteEventNoFail;
		if (!e.Succeeded)
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Error05, e.Exception.Message));
		}
		RunFuelSystemCheck();
	}

	private void StartAutomaticFsicTest()
	{
		if (warningManager.RequestContinue())
		{
			InitializeTest();
			ReportResult(Resources.Message_TheAutomaticFSICTestHasStarted);
			testStep = TestSteps.testStarted;
			RunFuelSystemCheck();
		}
	}

	private void StartLeakTest()
	{
		leakDetectionTestRunning = true;
		testStep = TestSteps.testStarted;
		ReportResult(Resources.Message_StartingTheRailPressureBleedOffLeakDetectionTest);
		RunFuelSystemCheck();
		UpdateUserInterface();
	}

	private void OnButtonRunTestClick(object sender, EventArgs e)
	{
		switch (comboBoxSelectTest.SelectedIndex)
		{
		case 0:
			StartAutomaticFsicTest();
			break;
		case 1:
			StartLeakTest();
			break;
		}
	}

	private void StopTest()
	{
		ReportResult(Resources.Message_TestStoppedByUser);
		CancelTest();
		UpdateUserInterface();
	}

	private void OnButtonStopClick(object sender, EventArgs e)
	{
		StopTest();
	}

	private void ComboBoxSelectTestSelectedIndexChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void SeekTimeListViewSelectedTimeChanged(object sender, EventArgs e)
	{
		chartInstrument1.SelectedTime = seekTimeListView.SelectedTime;
	}

	private void SeekTimeListViewTimeActivate(object sender, EventArgs e)
	{
		if (SapiManager.GlobalInstance.LogFileIsOpen && seekTimeListView.SelectedTime.HasValue)
		{
			SapiManager.GlobalInstance.LogFileChannels.LogFile.CurrentTime = seekTimeListView.SelectedTime.Value;
		}
	}

	private static string DisplayTime(TimeSpan time)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		if (time.Hours == 1)
		{
			text = Resources.Message_1Hour;
		}
		else if (time.Hours > 1)
		{
			text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0Hours, time.Hours);
		}
		if (time.Hours > 0 && (time.Minutes > 0 || time.Seconds > 0))
		{
			text += ", ";
		}
		if (time.Minutes == 1)
		{
			text2 = "1 minute";
		}
		else if (time.Hours > 0 || time.Minutes > 1)
		{
			text2 = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0Minutes, time.Minutes);
		}
		if (time.Seconds > 0)
		{
			if (time.Hours > 0 || time.Minutes > 0)
			{
				text2 += Resources.Message_And;
			}
			text3 = ((time.Seconds != 1) ? string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_0Seconds, time.Seconds) : Resources.Message_1Second);
		}
		return text + text2 + text3;
	}

	private void ReportResult(string text)
	{
		((CustomPanel)this).AddStatusMessage(text);
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
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
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0977: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e95: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelLeftColumn = new TableLayoutPanel();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentActualFuelMass = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFuelTemperature = new DigitalReadoutInstrument();
		digitalReadoutInstrumentRailPressure = new DigitalReadoutInstrument();
		digitalReadoutInstrumentDesiredRailPressure = new DigitalReadoutInstrument();
		digitalReadoutInstrumentFuelCompensationPressure = new DigitalReadoutInstrument();
		digitalReadoutInstrumentQuantityControlValveCurrent = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineState = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
		digitalReadoutInstrumentKwNwValiditySignal = new DigitalReadoutInstrument();
		tableLayoutPanelRightColumn = new TableLayoutPanel();
		tableLayoutPanelTestControls = new TableLayoutPanel();
		checkmarkTestCanProceedStatus = new Checkmark();
		seekTimeListView = new SeekTimeListView();
		buttonRunTest = new Button();
		buttonStop = new Button();
		progressBarTest = new ProgressBar();
		scalingLabelTimerDisplay = new ScalingLabel();
		comboBoxSelectTest = new ComboBox();
		labelTestStartStatus = new System.Windows.Forms.Label();
		chartInstrument1 = new ChartInstrument();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		splitContainerWholePanel = new SplitContainer();
		((Control)(object)tableLayoutPanelLeftColumn).SuspendLayout();
		((Control)(object)tableLayoutPanelRightColumn).SuspendLayout();
		((Control)(object)tableLayoutPanelTestControls).SuspendLayout();
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((ISupportInitialize)splitContainerWholePanel).BeginInit();
		splitContainerWholePanel.Panel1.SuspendLayout();
		splitContainerWholePanel.Panel2.SuspendLayout();
		splitContainerWholePanel.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelLeftColumn, "tableLayoutPanelLeftColumn");
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentActualFuelMass, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentFuelTemperature, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentRailPressure, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentDesiredRailPressure, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentFuelCompensationPressure, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentQuantityControlValveCurrent, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleStatus, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentEngineState, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentCoolantTemperature, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).Controls.Add((Control)(object)digitalReadoutInstrumentKwNwValiditySignal, 0, 6);
		((Control)(object)tableLayoutPanelLeftColumn).Name = "tableLayoutPanelLeftColumn";
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).SetColumnSpan((Control)(object)digitalReadoutInstrumentEngineSpeed, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS010_Engine_Speed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).SetColumnSpan((Control)(object)digitalReadoutInstrumentActualFuelMass, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentActualFuelMass, "digitalReadoutInstrumentActualFuelMass");
		digitalReadoutInstrumentActualFuelMass.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentActualFuelMass).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentActualFuelMass).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS087_Actual_Fuel_Mass");
		((Control)(object)digitalReadoutInstrumentActualFuelMass).Name = "digitalReadoutInstrumentActualFuelMass";
		((SingleInstrumentBase)digitalReadoutInstrumentActualFuelMass).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFuelTemperature, "digitalReadoutInstrumentFuelTemperature");
		digitalReadoutInstrumentFuelTemperature.FontGroup = "HalfWidth";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelTemperature).FreezeValue = false;
		digitalReadoutInstrumentFuelTemperature.Gradient.Initialize((ValueState)3, 2, "degC");
		digitalReadoutInstrumentFuelTemperature.Gradient.Modify(1, -40.0, (ValueState)1);
		digitalReadoutInstrumentFuelTemperature.Gradient.Modify(2, 150.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentFuelTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "fuelTemp");
		((Control)(object)digitalReadoutInstrumentFuelTemperature).Name = "digitalReadoutInstrumentFuelTemperature";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelTemperature).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).SetColumnSpan((Control)(object)digitalReadoutInstrumentRailPressure, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentRailPressure, "digitalReadoutInstrumentRailPressure");
		digitalReadoutInstrumentRailPressure.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentRailPressure).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentRailPressure).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS043_Rail_Pressure");
		((Control)(object)digitalReadoutInstrumentRailPressure).Name = "digitalReadoutInstrumentRailPressure";
		((SingleInstrumentBase)digitalReadoutInstrumentRailPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).SetColumnSpan((Control)(object)digitalReadoutInstrumentDesiredRailPressure, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDesiredRailPressure, "digitalReadoutInstrumentDesiredRailPressure");
		digitalReadoutInstrumentDesiredRailPressure.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentDesiredRailPressure).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentDesiredRailPressure).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS098_desired_rail_pressure");
		((Control)(object)digitalReadoutInstrumentDesiredRailPressure).Name = "digitalReadoutInstrumentDesiredRailPressure";
		((SingleInstrumentBase)digitalReadoutInstrumentDesiredRailPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).SetColumnSpan((Control)(object)digitalReadoutInstrumentFuelCompensationPressure, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentFuelCompensationPressure, "digitalReadoutInstrumentFuelCompensationPressure");
		digitalReadoutInstrumentFuelCompensationPressure.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelCompensationPressure).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentFuelCompensationPressure).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure");
		((Control)(object)digitalReadoutInstrumentFuelCompensationPressure).Name = "digitalReadoutInstrumentFuelCompensationPressure";
		((SingleInstrumentBase)digitalReadoutInstrumentFuelCompensationPressure).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).SetColumnSpan((Control)(object)digitalReadoutInstrumentQuantityControlValveCurrent, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentQuantityControlValveCurrent, "digitalReadoutInstrumentQuantityControlValveCurrent");
		digitalReadoutInstrumentQuantityControlValveCurrent.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentQuantityControlValveCurrent).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentQuantityControlValveCurrent).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS100_Quantity_Control_Valve_Current");
		((Control)(object)digitalReadoutInstrumentQuantityControlValveCurrent).Name = "digitalReadoutInstrumentQuantityControlValveCurrent";
		((SingleInstrumentBase)digitalReadoutInstrumentQuantityControlValveCurrent).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleStatus, "digitalReadoutInstrumentVehicleStatus");
		digitalReadoutInstrumentVehicleStatus.FontGroup = "HalfWidth";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleStatus).FreezeValue = false;
		digitalReadoutInstrumentVehicleStatus.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentVehicleStatus.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentVehicleStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentVehicleStatus.Gradient.Modify(3, 2.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutInstrumentVehicleStatus).Name = "digitalReadoutInstrumentVehicleStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleStatus).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineState, "digitalReadoutInstrumentEngineState");
		digitalReadoutInstrumentEngineState.FontGroup = "HalfWidth";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).FreezeValue = false;
		digitalReadoutInstrumentEngineState.Gradient.Initialize((ValueState)0, 7);
		digitalReadoutInstrumentEngineState.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentEngineState.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentEngineState.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentEngineState.Gradient.Modify(4, 3.0, (ValueState)0);
		digitalReadoutInstrumentEngineState.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(6, 5.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(7, 6.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS023_Engine_State");
		((Control)(object)digitalReadoutInstrumentEngineState).Name = "digitalReadoutInstrumentEngineState";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCoolantTemperature, "digitalReadoutInstrumentCoolantTemperature");
		digitalReadoutInstrumentCoolantTemperature.FontGroup = "HalfWidth";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).FreezeValue = false;
		digitalReadoutInstrumentCoolantTemperature.Gradient.Initialize((ValueState)3, 1, "degC");
		digitalReadoutInstrumentCoolantTemperature.Gradient.Modify(1, 10.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "coolantTemp");
		((Control)(object)digitalReadoutInstrumentCoolantTemperature).Name = "digitalReadoutInstrumentCoolantTemperature";
		((SingleInstrumentBase)digitalReadoutInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelLeftColumn).SetColumnSpan((Control)(object)digitalReadoutInstrumentKwNwValiditySignal, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentKwNwValiditySignal, "digitalReadoutInstrumentKwNwValiditySignal");
		digitalReadoutInstrumentKwNwValiditySignal.FontGroup = "Body";
		((SingleInstrumentBase)digitalReadoutInstrumentKwNwValiditySignal).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentKwNwValiditySignal).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_DS001_KW_NW_validity_signal");
		((Control)(object)digitalReadoutInstrumentKwNwValiditySignal).Name = "digitalReadoutInstrumentKwNwValiditySignal";
		((SingleInstrumentBase)digitalReadoutInstrumentKwNwValiditySignal).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelRightColumn, "tableLayoutPanelRightColumn");
		((TableLayoutPanel)(object)tableLayoutPanelRightColumn).Controls.Add((Control)(object)tableLayoutPanelTestControls, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelRightColumn).Controls.Add((Control)(object)chartInstrument1, 0, 0);
		((Control)(object)tableLayoutPanelRightColumn).Name = "tableLayoutPanelRightColumn";
		componentResourceManager.ApplyResources(tableLayoutPanelTestControls, "tableLayoutPanelTestControls");
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add((Control)(object)checkmarkTestCanProceedStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add((Control)(object)seekTimeListView, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(buttonRunTest, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(buttonStop, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(progressBarTest, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add((Control)(object)scalingLabelTimerDisplay, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(comboBoxSelectTest, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(labelTestStartStatus, 1, 0);
		((Control)(object)tableLayoutPanelTestControls).Name = "tableLayoutPanelTestControls";
		componentResourceManager.ApplyResources(checkmarkTestCanProceedStatus, "checkmarkTestCanProceedStatus");
		((Control)(object)checkmarkTestCanProceedStatus).Name = "checkmarkTestCanProceedStatus";
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Fuel System Integrity Check";
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).SetRowSpan((Control)(object)seekTimeListView, 5);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).SetColumnSpan((Control)buttonRunTest, 2);
		componentResourceManager.ApplyResources(buttonRunTest, "buttonRunTest");
		buttonRunTest.Name = "buttonRunTest";
		buttonRunTest.UseCompatibleTextRendering = true;
		buttonRunTest.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).SetColumnSpan((Control)progressBarTest, 2);
		componentResourceManager.ApplyResources(progressBarTest, "progressBarTest");
		progressBarTest.Name = "progressBarTest";
		scalingLabelTimerDisplay.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelTimerDisplay, "scalingLabelTimerDisplay");
		scalingLabelTimerDisplay.FontGroup = null;
		scalingLabelTimerDisplay.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelTimerDisplay).Name = "scalingLabelTimerDisplay";
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).SetColumnSpan((Control)comboBoxSelectTest, 3);
		componentResourceManager.ApplyResources(comboBoxSelectTest, "comboBoxSelectTest");
		comboBoxSelectTest.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBoxSelectTest.FormattingEnabled = true;
		comboBoxSelectTest.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBoxSelectTest.Items"),
			componentResourceManager.GetString("comboBoxSelectTest.Items1")
		});
		comboBoxSelectTest.Name = "comboBoxSelectTest";
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).SetColumnSpan((Control)labelTestStartStatus, 2);
		componentResourceManager.ApplyResources(labelTestStartStatus, "labelTestStartStatus");
		labelTestStartStatus.Name = "labelTestStartStatus";
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).SetRowSpan((Control)labelTestStartStatus, 2);
		labelTestStartStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(chartInstrument1, "chartInstrument1");
		((Control)(object)chartInstrument1).Name = "chartInstrument1";
		chartInstrument1.SelectedTime = null;
		chartInstrument1.ShowButtonPanel = false;
		chartInstrument1.ShowEvents = false;
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add(splitContainerWholePanel, 0, 0);
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		componentResourceManager.ApplyResources(splitContainerWholePanel, "splitContainerWholePanel");
		splitContainerWholePanel.Name = "splitContainerWholePanel";
		splitContainerWholePanel.Panel1.Controls.Add((Control)(object)tableLayoutPanelLeftColumn);
		splitContainerWholePanel.Panel2.Controls.Add((Control)(object)tableLayoutPanelRightColumn);
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_FuelSystemEPA07");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelLeftColumn).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelRightColumn).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelTestControls).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		splitContainerWholePanel.Panel1.ResumeLayout(performLayout: false);
		splitContainerWholePanel.Panel2.ResumeLayout(performLayout: false);
		((ISupportInitialize)splitContainerWholePanel).EndInit();
		splitContainerWholePanel.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
