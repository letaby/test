// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Fuel_System_Integrity_Check.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Fuel_System_Integrity_Check.panel;

public class UserPanel : CustomPanel
{
  private const string IdleSpeedSetRoutine = "RT_SR015_Idle_Speed_Modification_Start";
  private const string IdleSpeedResetRoutine = "RT_SR015_Idle_Speed_Modification_Stop";
  private const string PWMRoutineProductionStartControl = "RT_SR003_PWM_Routine_for_Production_Start_PWM_Value";
  private const string PWMRoutineProductionStopControl = "RT_SR003_PWM_Routine_for_Production_Stop";
  private const string SwRoutineProductionStart = "RT_SR005_SW_Routine_for_Production_Start_SW_Operation";
  private const string SwRoutineProductionStop = "RT_SR005_SW_Routine_for_Production_Stop";
  private const string CalibrationOverrideForLeakDetectionTestStart = "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start";
  private const string CalibrationOverrideForLeakDetectionTestStop = "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop";
  private const string RailPressureSetRoutine = "RT_SR076_Desired_Rail_Pressure_Start_Status";
  private const string RailPressureResetRoutine = "RT_SR076_Desired_Rail_Pressure_Stop";
  private const string HCDoserDisableStartRoutine = "RT_SR018_Disable_HC_Doser_Start";
  private const string HCDoserDisableStopRoutine = "RT_SR018_Disable_HC_Doser_Stop";
  private const string MCMName = "MCM";
  private const double LeakTestPreTestPressure = 800.0;
  private const double LeakTestAltPreTestPressure = 400.0;
  private const int LeakTestAltIdleSpeed = 1400;
  private const double LeakTestStartThresholdPressure = 280.0;
  private const double LeakTestStopThresholdPressure = 10.0;
  private static readonly TimeSpan RepeatSwRoutinePeriod = TimeSpan.FromSeconds(30.0);
  private static readonly TimeSpan SwRoutineDuration = TimeSpan.FromSeconds(60.0);
  private static readonly TimeSpan TestDuration = TimeSpan.FromHours(1.0);
  private static readonly TimeSpan TestHoldStateDuration = TimeSpan.FromSeconds(20.0);
  private static readonly TimeSpan AutoFsicTestIdleToWarmUpEngineDuration = TimeSpan.FromMinutes(2.0);
  private static readonly TimeSpan LeakTestIdleToWarmUpEngineDuration = TimeSpan.FromMinutes(1.0);
  private int HCDoserPwmIndex = 12;
  private int FuelCutoffValveSwIndex = 8;
  private int[] IdleSpeeds = new int[6]
  {
    600,
    850,
    950,
    1100,
    1500,
    1800
  };
  private Timer commandTimer;
  private Timer repeatSwRoutineTimer;
  private Stopwatch repeatSwRoutineStopwatch;
  private UserPanel.TestSteps testStep;
  private int idleSpeedIndex;
  private double leakTestPreTestPressure;
  private UserPanel.FuelTemperatureTest fuelTempTest;
  private UserPanel.IdleMonitor idleMonitor;
  private UserPanel.DisplayCounter displayCounter;
  private UserPanel.StopEngineProcedure stopEngineProcedure;
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

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningManager = new WarningManager(string.Empty, Resources.Message_Test, this.seekTimeListView.RequiredUserLabelPrefix);
    this.comboBoxSelectTest.SelectedIndex = 0;
  }

  private void InitializeChart()
  {
    ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) new Qualifier[8]
    {
      new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
      new Qualifier((QualifierTypes) 1, "MCM", "DT_AS087_Actual_Fuel_Mass"),
      new Qualifier((QualifierTypes) 1, "MCM", "DT_AS098_desired_rail_pressure"),
      new Qualifier((QualifierTypes) 1, "MCM", "DT_AS043_Rail_Pressure"),
      new Qualifier((QualifierTypes) 1, "MCM", "DT_AS100_Quantity_Control_Valve_Current"),
      new Qualifier((QualifierTypes) 1, "MCM", "DT_AS024_Fuel_Compensation_Pressure"),
      new Qualifier((QualifierTypes) 1, "MCM", "DT_DS001_KW_NW_validity_signal"),
      new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp")
    });
  }

  private void InitializeTest()
  {
    this.idleSpeedIndex = 0;
    this.fuelTempTest.InitFuelTemperatureTest();
    this.idleMonitor.InitIdleMonitor();
    this.displayCounter.Reset();
  }

  private void ConnectFormControlEventHandlers(bool connect)
  {
    if (connect)
    {
      this.buttonRunTest.Click += new EventHandler(this.OnButtonRunTestClick);
      this.buttonStop.Click += new EventHandler(this.OnButtonStopClick);
      this.comboBoxSelectTest.SelectedIndexChanged += new EventHandler(this.ComboBoxSelectTestSelectedIndexChanged);
      this.comboBoxSelectTest.SelectedIndex = 0;
      this.digitalReadoutInstrumentEngineState.RepresentedStateChanged += new EventHandler(this.OnDigitalReadoutInstrumentRepresentedStateChanged);
      this.digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged += new EventHandler(this.OnDigitalReadoutInstrumentRepresentedStateChanged);
      this.digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentVehicleStatus_RepresentedStateChanged);
      this.digitalReadoutInstrumentCoolantTemperature.RepresentedStateChanged += new EventHandler(this.OnDigitalReadoutInstrumentRepresentedStateChanged);
      this.digitalReadoutInstrumentFuelTemperature.RepresentedStateChanged += new EventHandler(this.OnDigitalReadoutInstrumentRepresentedStateChanged);
      this.seekTimeListView.TimeActivate += new EventHandler(this.SeekTimeListViewTimeActivate);
      this.seekTimeListView.SelectedTimeChanged += new EventHandler(this.SeekTimeListViewSelectedTimeChanged);
    }
    else
    {
      if (this.buttonRunTest != null)
        this.buttonRunTest.Click -= new EventHandler(this.OnButtonRunTestClick);
      if (this.buttonStop != null)
        this.buttonStop.Click -= new EventHandler(this.OnButtonStopClick);
      if (this.comboBoxSelectTest != null)
        this.comboBoxSelectTest.SelectedIndexChanged -= new EventHandler(this.ComboBoxSelectTestSelectedIndexChanged);
      if (this.digitalReadoutInstrumentEngineState != null)
        this.digitalReadoutInstrumentEngineState.RepresentedStateChanged -= new EventHandler(this.OnDigitalReadoutInstrumentRepresentedStateChanged);
      if (this.digitalReadoutInstrumentVehicleStatus != null)
      {
        this.digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged -= new EventHandler(this.OnDigitalReadoutInstrumentRepresentedStateChanged);
        this.digitalReadoutInstrumentVehicleStatus.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrumentVehicleStatus_RepresentedStateChanged);
      }
      if (this.digitalReadoutInstrumentCoolantTemperature != null)
        this.digitalReadoutInstrumentCoolantTemperature.RepresentedStateChanged -= new EventHandler(this.OnDigitalReadoutInstrumentRepresentedStateChanged);
      if (this.digitalReadoutInstrumentFuelTemperature != null)
        this.digitalReadoutInstrumentFuelTemperature.RepresentedStateChanged -= new EventHandler(this.OnDigitalReadoutInstrumentRepresentedStateChanged);
      if (this.seekTimeListView != null)
      {
        this.seekTimeListView.TimeActivate -= new EventHandler(this.SeekTimeListViewTimeActivate);
        this.seekTimeListView.SelectedTimeChanged -= new EventHandler(this.SeekTimeListViewSelectedTimeChanged);
      }
    }
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    this.commandTimer = new Timer();
    this.repeatSwRoutineTimer = new Timer();
    this.repeatSwRoutineTimer.Interval = (int) UserPanel.RepeatSwRoutinePeriod.TotalMilliseconds / 2;
    this.repeatSwRoutineStopwatch = new Stopwatch();
    this.fuelTempTest = new UserPanel.FuelTemperatureTest();
    this.idleMonitor = new UserPanel.IdleMonitor();
    this.displayCounter = new UserPanel.DisplayCounter(UserPanel.AutoFsicTestIdleToWarmUpEngineDuration, this.scalingLabelTimerDisplay, this.progressBarTest);
    this.stopEngineProcedure = new UserPanel.StopEngineProcedure(this.mcm);
    this.ConnectFormControlEventHandlers(true);
    this.InitializeChart();
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    if (((ContainerControl) this).ParentForm != null)
      ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.ConnectFormControlEventHandlers(false);
    this.StopRepeatSwRoutineTimer();
    this.SetMCM((Channel) null);
    ((Control) this).Tag = (object) new object[2]
    {
      (object) false,
      (object) this.seekTimeListView.Text
    };
  }

  private Channel GetActiveChannel(string name)
  {
    foreach (Channel activeChannel in SapiManager.GlobalInstance.ActiveChannels)
    {
      if (activeChannel.Ecu.Name == name)
        return activeChannel;
    }
    return (Channel) null;
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetActiveChannel("MCM"));
    this.UpdateUserInterface();
  }

  private static void DisconnectInstrument(
    Instrument instrument,
    InstrumentUpdateEventHandler updateEventHandler)
  {
    if (!(instrument != (Instrument) null))
      return;
    instrument.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(updateEventHandler.Invoke);
  }

  private static Instrument ConnectInstrument(
    InstrumentCollection instruments,
    string qualifier,
    InstrumentUpdateEventHandler updateEventHandler)
  {
    Instrument instrument = instruments[qualifier];
    if (instrument != (Instrument) null)
      instrument.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(updateEventHandler.Invoke);
    return instrument;
  }

  private void SetMCM(Channel mcm)
  {
    if (this.mcm == mcm)
      return;
    this.warningManager.Reset();
    if (this.commandTimer != null && this.commandTimer.Enabled)
      this.StopCommandTimer();
    if (this.testStep != UserPanel.TestSteps.notRunning)
      this.ReportResult(Resources.Message_CancellingCurrentTestBecauseTheMCMHasBecomeDisconnected);
    this.EndTest();
    if (this.mcm != null)
    {
      UserPanel.DisconnectInstrument(this.engineSpeedInstrument, new InstrumentUpdateEventHandler(this.OnEngineSpeedInstrumentUpdateEvent));
      UserPanel.DisconnectInstrument(this.railPressureInstrument, new InstrumentUpdateEventHandler(this.OnRailPressureInstrumentUpdateEvent));
      UserPanel.DisconnectInstrument(this.engineStateInstrument, new InstrumentUpdateEventHandler(this.OnEngineStateInstrumentUpdateEvent));
      UserPanel.DisconnectInstrument(this.fuelTemperatureInstrument, new InstrumentUpdateEventHandler(this.OnFuelTemperatureInstrumentUpdateEvent));
      this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    }
    this.mcm = mcm;
    if (this.stopEngineProcedure != null)
      this.stopEngineProcedure.Mcm = mcm;
    if (this.mcm != null)
    {
      this.engineSpeedInstrument = UserPanel.ConnectInstrument(this.mcm.Instruments, "DT_ASL002_Engine_Speed", new InstrumentUpdateEventHandler(this.OnEngineSpeedInstrumentUpdateEvent));
      this.railPressureInstrument = UserPanel.ConnectInstrument(this.mcm.Instruments, "DT_AS043_Rail_Pressure", new InstrumentUpdateEventHandler(this.OnRailPressureInstrumentUpdateEvent));
      this.engineStateInstrument = UserPanel.ConnectInstrument(this.mcm.Instruments, "DT_AS023_Engine_State", new InstrumentUpdateEventHandler(this.OnEngineStateInstrumentUpdateEvent));
      this.fuelTemperatureInstrument = UserPanel.ConnectInstrument(this.mcm.Instruments, "DT_ASL005_Fuel_Temperature", new InstrumentUpdateEventHandler(this.OnFuelTemperatureInstrumentUpdateEvent));
      this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    }
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private bool TestIsRunning => this.Online && this.testStep != UserPanel.TestSteps.notRunning;

  private bool TestIsResetting
  {
    get
    {
      bool testIsResetting;
      switch (this.testStep)
      {
        case UserPanel.TestSteps.resetIdle:
        case UserPanel.TestSteps.resetRailPressure:
        case UserPanel.TestSteps.resetOverrideCalibration:
        case UserPanel.TestSteps.resetFuelCutoffValve:
        case UserPanel.TestSteps.resetHcDoserValve:
        case UserPanel.TestSteps.resetAllCyliners:
        case UserPanel.TestSteps.resetDisableHCDoserStart:
        case UserPanel.TestSteps.resetComplete:
          testIsResetting = true;
          break;
        default:
          testIsResetting = false;
          break;
      }
      return testIsResetting;
    }
  }

  private bool CanSelectTest => this.Online && this.testStep == UserPanel.TestSteps.notRunning;

  private string SelectedTestName => this.comboBoxSelectTest.SelectedItem.ToString();

  private void UpdateUserInterface()
  {
    bool testCanStart = true;
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(Resources.Message_The + this.SelectedTestName);
    if (!this.Online)
    {
      testCanStart = false;
      stringBuilder.Append(this.TestCannotRunMcmIsOfflineString);
    }
    else
    {
      switch (this.comboBoxSelectTest.SelectedIndex)
      {
        case 0:
          if (this.TestIsRunning)
          {
            testCanStart = false;
            if (this.testStep == UserPanel.TestSteps.waitingForEngineToStart)
            {
              if (this.EngineStatus == UserPanel.EngineState.Idle || this.EngineStatus == UserPanel.EngineState.StarterEngaged)
              {
                this.checkmarkTestCanProceedStatus.Checked = true;
                break;
              }
              this.checkmarkTestCanProceedStatus.Checked = false;
              this.checkmarkTestCanProceedStatus.CheckState = CheckState.Indeterminate;
              stringBuilder.Append(Resources.Message_CannotRunItIsWaitingForYouToStartTheEngine);
              break;
            }
            stringBuilder.Append(this.TestIsRunningString);
            break;
          }
          stringBuilder.Append(this.AutomaticFsicTestInstrumentsReadyToStartTest(ref testCanStart));
          break;
        case 1:
          testCanStart = this.testStep == UserPanel.TestSteps.notRunning;
          stringBuilder.Append(this.TestIsRunning ? this.TestIsRunningString : this.TestIsReadyString);
          break;
        case 2:
          if (this.TestIsRunning)
          {
            testCanStart = false;
            stringBuilder.Append(this.TestIsRunningString);
            break;
          }
          stringBuilder.Append(this.LeakTestInstrumentsReadyToStartTest(ref testCanStart));
          break;
        default:
          throw new IndexOutOfRangeException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The selected test index value ({0}) is undefined. ", (object) this.comboBoxSelectTest.SelectedIndex));
      }
    }
    this.buttonStop.Enabled = this.Online;
    this.comboBoxSelectTest.Enabled = this.CanSelectTest;
    this.checkmarkTestCanProceedStatus.Checked = testCanStart || this.TestIsRunning;
    this.buttonRunTest.Enabled = !this.TestIsRunning && testCanStart;
    this.labelTestStartStatus.Text = stringBuilder.ToString();
    this.labelTestStartStatus.Enabled = this.Online;
  }

  private string AutomaticFsicTestInstrumentsReadyToStartTest(ref bool testCanStart)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.EngineStatus != UserPanel.EngineState.Stopped)
    {
      testCanStart = false;
      stringBuilder.Append(Resources.Message_CannotStartBecauseTheEngineIsNotStoppedPleaseStopTheEngine);
    }
    else if (this.digitalReadoutInstrumentVehicleStatus.RepresentedState == 3)
    {
      testCanStart = false;
      stringBuilder.Append(Resources.Message_CannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL);
    }
    else if (this.digitalReadoutInstrumentCoolantTemperature.RepresentedState == 3)
    {
      testCanStart = false;
      string minimumOkValueString = this.GetMinimumOKValueString(this.digitalReadoutInstrumentCoolantTemperature, this.mcm.Instruments["DT_ASL005_Coolant_Temperature"]);
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotStartBecauseTheCoolantTemperatureIs0, string.IsNullOrEmpty(minimumOkValueString) ? (object) Resources.Message_TooLow : (object) (Resources.Message_LessThan + minimumOkValueString));
    }
    else if (this.digitalReadoutInstrumentFuelTemperature.RepresentedState == 3)
    {
      testCanStart = false;
      string minimumOkValueString = this.GetMinimumOKValueString(this.digitalReadoutInstrumentFuelTemperature, this.fuelTemperatureInstrument);
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotStartBecauseTheFuelTemperatureIs0, string.IsNullOrEmpty(minimumOkValueString) ? (object) Resources.Message_TooLow1 : (object) (Resources.Message_LessThan1 + minimumOkValueString));
    }
    else
    {
      testCanStart = true;
      stringBuilder.Append(this.TestIsReadyString);
    }
    return stringBuilder.ToString();
  }

  private string LeakTestInstrumentsReadyToStartTest(ref bool testCanStart)
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.digitalReadoutInstrumentVehicleStatus.RepresentedState == 3)
    {
      testCanStart = false;
      stringBuilder.Append(Resources.Message_CannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL1);
    }
    else if (this.digitalReadoutInstrumentCoolantTemperature.RepresentedState == 3)
    {
      testCanStart = false;
      string minimumOkValueString = this.GetMinimumOKValueString(this.digitalReadoutInstrumentCoolantTemperature, this.mcm.Instruments["DT_ASL005_Coolant_Temperature"]);
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotStartBecauseTheCoolantTemperatureIs01, string.IsNullOrEmpty(minimumOkValueString) ? (object) Resources.Message_TooLow2 : (object) (Resources.Message_LessThan2 + minimumOkValueString));
    }
    else
    {
      testCanStart = true;
      stringBuilder.Append(this.TestIsReadyString);
    }
    return stringBuilder.ToString();
  }

  private void OnDigitalReadoutInstrumentRepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrumentVehicleStatus_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    if (this.digitalReadoutInstrumentVehicleStatus.RepresentedState != 3 || !this.TestIsRunning)
      return;
    this.ReportResult(Resources.Message_Error_StoppedTest_VehicleCheckStatus);
    this.CancelTest();
  }

  private bool Online
  {
    get
    {
      bool online = false;
      if (this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online)
        online = true;
      return online;
    }
  }

  private UserPanel.EngineState EngineStatus
  {
    get
    {
      UserPanel.EngineState engineStatus = UserPanel.EngineState.Unknown;
      object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(this.engineStateInstrument);
      if (instrumentCurrentValue != null)
        engineStatus = instrumentCurrentValue != (object) this.engineStateInstrument.Choices.GetItemFromRawValue((object) UserPanel.EngineState.Stopped) ? (instrumentCurrentValue != (object) this.engineStateInstrument.Choices.GetItemFromRawValue((object) UserPanel.EngineState.StarterEngaged) ? (instrumentCurrentValue != (object) this.engineStateInstrument.Choices.GetItemFromRawValue((object) UserPanel.EngineState.Idle) ? (instrumentCurrentValue != (object) this.engineStateInstrument.Choices.GetItemFromRawValue((object) UserPanel.EngineState.Unknown) ? UserPanel.EngineState.Other : UserPanel.EngineState.Unknown) : UserPanel.EngineState.Idle) : UserPanel.EngineState.StarterEngaged) : UserPanel.EngineState.Stopped;
      return engineStatus;
    }
  }

  private bool CanClose
  {
    get
    {
      return !this.Online || this.testStep == UserPanel.TestSteps.notRunning || this.testStep == UserPanel.TestSteps.manualTestRunning;
    }
  }

  private void OnFuelTemperatureInstrumentUpdateEvent(object sender, ResultEventArgs e)
  {
    this.RecordInstrumentValue();
  }

  private void RecordInstrumentValue()
  {
    object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(this.fuelTemperatureInstrument);
    if (instrumentCurrentValue == null)
      return;
    this.fuelTempTest.CollectTemperatures(Convert.ToSingle(instrumentCurrentValue), this.testStep, this.idleSpeedIndex, this.IdleSpeeds);
  }

  private void OnEngineSpeedInstrumentUpdateEvent(object sender, ResultEventArgs e)
  {
    object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(this.engineSpeedInstrument);
    if (instrumentCurrentValue == null)
      return;
    int int32 = Convert.ToInt32(instrumentCurrentValue);
    switch (this.testStep)
    {
      case UserPanel.TestSteps.startIdleTimer:
        if (int32 >= this.IdleSpeeds[this.idleSpeedIndex])
        {
          this.RunFuelSystemCheck();
          break;
        }
        break;
      case UserPanel.TestSteps.idleSteadyState:
        this.idleMonitor.CalculateIdleVariation(int32);
        break;
      case UserPanel.TestSteps.idleRampDownComplete:
        if (this.idleMonitor.IdleIsStable(this.engineSpeedInstrument.InstrumentValues))
        {
          this.RunFuelSystemCheck();
          break;
        }
        break;
    }
  }

  private void OnRailPressureInstrumentUpdateEvent(object sender, ResultEventArgs e)
  {
    object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(this.railPressureInstrument);
    if (instrumentCurrentValue == null)
      return;
    double num = Convert.ToDouble(instrumentCurrentValue);
    switch (this.testStep)
    {
      case UserPanel.TestSteps.startRailPressureTimer:
        if (num >= this.leakTestPreTestPressure)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_RailPressureSetTo0, (object) this.leakTestPreTestPressure));
          this.RunFuelSystemCheck();
          break;
        }
        break;
      case UserPanel.TestSteps.runningLeakTest:
        if (num <= 280.0)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_RailPressureBleedOffTestRunningCurrentRailPressure000BarAbs, (object) num));
          this.injectorLeakTestStart = DateTime.Now;
          this.RunFuelSystemCheck();
          break;
        }
        break;
      case UserPanel.TestSteps.leakTestResults:
        if (num <= 10.0)
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TimeElapsedForTheFuelPressureToLeakDownFrom0To1BarAbs2, (object) 280.0, (object) 10.0, (object) UserPanel.DisplayTime(DateTime.Now - this.injectorLeakTestStart)));
          this.RunFuelSystemCheck();
          break;
        }
        break;
    }
  }

  private void OnEngineStateInstrumentUpdateEvent(object sender, ResultEventArgs e)
  {
    if (this.testStep == UserPanel.TestSteps.waitingForEngineToStart && this.EngineStatus == UserPanel.EngineState.Idle)
    {
      this.ReportResult(Resources.Message_EngineStarted);
      this.RunFuelSystemCheck();
    }
    this.UpdateUserInterface();
  }

  private void StartCommandTimer()
  {
    string text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Waiting0, (object) UserPanel.DisplayTime(TimeSpan.FromMilliseconds((double) this.commandTimer.Interval)));
    this.ReportResult(text);
    this.labelTestStartStatus.Text = text;
    ((Control) this).Cursor = Cursors.WaitCursor;
    this.commandTimer.Tick += new EventHandler(this.OnCommandTimerTick);
    this.displayCounter.Duration = TimeSpan.FromMilliseconds((double) this.commandTimer.Interval);
    this.displayCounter.StartCountDown();
    this.commandTimer.Start();
  }

  internal void StopCommandTimer()
  {
    this.commandTimer.Stop();
    this.commandTimer.Tick -= new EventHandler(this.OnCommandTimerTick);
    this.displayCounter.Reset();
    ((Control) this).Cursor = Cursors.Default;
  }

  private void OnCommandTimerTick(object sender, EventArgs e)
  {
    this.StopCommandTimer();
    this.RunFuelSystemCheck();
  }

  private static object GetInstrumentCurrentValue(Instrument instrument)
  {
    object instrumentCurrentValue = (object) null;
    if (instrument != (Instrument) null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
      instrumentCurrentValue = instrument.InstrumentValues.Current.Value;
    return instrumentCurrentValue;
  }

  private void AdvanceTestStep()
  {
    if (this.testStep == UserPanel.TestSteps.notRunning)
      throw new InvalidOperationException("Step sequence out of order.");
    ++this.testStep;
  }

  private void RunFuelSystemCheck()
  {
    switch (this.testStep)
    {
      case UserPanel.TestSteps.notRunning:
        break;
      case UserPanel.TestSteps.testStarted:
        this.idleSpeedIndex = 0;
        this.AdvanceTestStep();
        this.RunFuelSystemCheck();
        break;
      case UserPanel.TestSteps.disableHCDoserStart:
        this.AdvanceTestStep();
        this.DisableHCDoserStart();
        break;
      case UserPanel.TestSteps.setHcDoserValve:
        this.AdvanceTestStep();
        this.CloseHCDoserValveStart();
        break;
      case UserPanel.TestSteps.setFuelCutoffValve:
        this.AdvanceTestStep();
        this.OpenFuelCutOffValveStart();
        break;
      case UserPanel.TestSteps.waitingForEngineToStart:
        this.WaitForEngineToStart();
        break;
      case UserPanel.TestSteps.idleToWarmUpEngine:
        if (this.leakDetectionTestRunning)
        {
          this.testStep = UserPanel.TestSteps.overrideCalibrationForLeakDetectionTestStart;
          this.commandTimer.Interval = (int) UserPanel.LeakTestIdleToWarmUpEngineDuration.TotalMilliseconds;
        }
        else
        {
          this.AdvanceTestStep();
          this.commandTimer.Interval = (int) UserPanel.AutoFsicTestIdleToWarmUpEngineDuration.TotalMilliseconds;
        }
        this.StartCommandTimer();
        break;
      case UserPanel.TestSteps.rampUpIdle:
        this.AdvanceTestStep();
        this.SetIdleStart(this.IdleSpeeds[this.idleSpeedIndex]);
        break;
      case UserPanel.TestSteps.startIdleTimer:
        this.AdvanceTestStep();
        this.RecordInstrumentValue();
        this.commandTimer.Interval = (int) UserPanel.TestHoldStateDuration.TotalMilliseconds;
        this.StartCommandTimer();
        break;
      case UserPanel.TestSteps.idleSteadyState:
        this.idleMonitor.ResetMinMax();
        if (this.idleSpeedIndex < this.IdleSpeeds.Length - 1)
        {
          ++this.idleSpeedIndex;
          this.testStep = UserPanel.TestSteps.rampUpIdle;
        }
        else
          this.testStep = UserPanel.TestSteps.rampDownIdle;
        this.RunFuelSystemCheck();
        break;
      case UserPanel.TestSteps.rampDownIdle:
        this.AdvanceTestStep();
        this.SetIdleStopWaitUntilEngineSpeedIsStable();
        break;
      case UserPanel.TestSteps.idleRampDownComplete:
        this.AdvanceTestStep();
        this.RunFuelSystemCheck();
        break;
      case UserPanel.TestSteps.overrideCalibrationForLeakDetectionTestStart:
        this.AdvanceTestStep();
        this.commandTimer.Interval = (int) UserPanel.TestHoldStateDuration.TotalMilliseconds;
        this.OverrideCalibrationForLeakDetectionTestStart();
        break;
      case UserPanel.TestSteps.setRailPressure:
        this.AdvanceTestStep();
        this.leakTestPreTestPressure = 800.0;
        this.SetRailPressureStart(this.leakTestPreTestPressure);
        break;
      case UserPanel.TestSteps.startRailPressureTimer:
        this.AdvanceTestStep();
        this.StartCommandTimer();
        break;
      case UserPanel.TestSteps.resetIdleBeforeShutdownEngine:
        this.AdvanceTestStep();
        this.SetIdleStop();
        break;
      case UserPanel.TestSteps.resetRailPressureBeforeShutdownEngine:
        this.AdvanceTestStep();
        this.SetRailPressureStop();
        break;
      case UserPanel.TestSteps.cutAllCylinders:
        this.AdvanceTestStep();
        this.ReportResult(Resources.Message_ShuttingOffEngine);
        bool flag;
        try
        {
          flag = this.stopEngineProcedure.SetCylindersOff(new UserPanel.StopEngineProcedure.ProcedureCompleteEventHandler(this.OnProcedureCompleteEvent));
        }
        catch (InvalidOperationException ex)
        {
          flag = false;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Error01, (object) ex.Message));
        }
        catch (CaesarException ex)
        {
          flag = false;
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Error0, (object) ex.Message));
        }
        if (flag)
          break;
        this.ReportResult(Resources.Message_ErrorShuttingOffTheEngineEndingTheTest);
        this.StopTest();
        break;
      case UserPanel.TestSteps.waitingToStartLeakTest:
        this.AdvanceTestStep();
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForRailPressureToReach0BarAbsToStartLeakTest, (object) 280.0));
        break;
      case UserPanel.TestSteps.runningLeakTest:
        this.AdvanceTestStep();
        this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_RailPressureBleedOffTestRunningWaitingForRailPressureToReach0BarAbsToCompleteLeakTest, (object) 10.0));
        break;
      case UserPanel.TestSteps.leakTestResults:
        string text = this.fuelTempTest.ReportChangeInFuelTemperature(this.fuelTemperatureInstrument);
        if (!this.leakDetectionTestRunning)
          this.ReportResult(text);
        this.ReportResult(Resources.Message_TestsComplete);
        if (this.leakDetectionTestRunning)
          this.leakDetectionTestRunning = false;
        this.AdvanceTestStep();
        this.RunFuelSystemCheck();
        break;
      case UserPanel.TestSteps.resetTest:
        if (this.commandTimer != null && this.commandTimer.Enabled)
          this.StopCommandTimer();
        this.AdvanceTestStep();
        this.RunFuelSystemCheck();
        break;
      case UserPanel.TestSteps.resetIdle:
        this.AdvanceTestStep();
        this.SetIdleStop();
        break;
      case UserPanel.TestSteps.resetRailPressure:
        this.AdvanceTestStep();
        this.SetRailPressureStop();
        break;
      case UserPanel.TestSteps.resetOverrideCalibration:
        this.AdvanceTestStep();
        this.ResetOverrideCalibrationForLeakDetectionTest();
        break;
      case UserPanel.TestSteps.resetFuelCutoffValve:
        this.AdvanceTestStep();
        this.OpenFuelCutOffValveStop();
        break;
      case UserPanel.TestSteps.resetHcDoserValve:
        this.AdvanceTestStep();
        this.CloseHCDoserValveStop();
        break;
      case UserPanel.TestSteps.resetAllCyliners:
        this.AdvanceTestStep();
        this.stopEngineProcedure.SetCylindersOn(new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoFail));
        break;
      case UserPanel.TestSteps.resetDisableHCDoserStart:
        this.AdvanceTestStep();
        this.DisableHCDoserStop();
        break;
      case UserPanel.TestSteps.resetComplete:
        this.ReportResult(Resources.Message_TestConditionsReset);
        this.EndTest();
        this.RunFuelSystemCheck();
        break;
      case UserPanel.TestSteps.manualTestStart:
        this.idleSpeedIndex = 0;
        this.AdvanceTestStep();
        this.RunFuelSystemCheck();
        break;
      case UserPanel.TestSteps.manualTestDisableHCDoserStart:
        this.AdvanceTestStep();
        this.DisableHCDoserStart();
        break;
      case UserPanel.TestSteps.manualTestSetHcDoserValve:
        this.AdvanceTestStep();
        this.CloseHCDoserValveStart();
        break;
      case UserPanel.TestSteps.manualTestSetFuelCutoffValve:
        this.AdvanceTestStep();
        this.OpenFuelCutOffValveStart();
        break;
      case UserPanel.TestSteps.manualTestRunning:
        break;
      default:
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} is not a defined test step.", (object) this.testStep));
    }
  }

  private void CancelTest()
  {
    this.testStep = UserPanel.TestSteps.resetTest;
    this.RunFuelSystemCheck();
  }

  private void EndTest() => this.testStep = UserPanel.TestSteps.notRunning;

  private void WaitForEngineToStart()
  {
    switch (this.EngineStatus)
    {
      case UserPanel.EngineState.Unknown:
        this.ReportResult(Resources.Message_CannotProceedUntilEngineIsAtIdle);
        break;
      case UserPanel.EngineState.Stopped:
        if (MessageBox.Show((IWin32Window) ((ContainerControl) this).ParentForm, Resources.Message_PleaseStartTheEngine, Resources.Message_EngineNotAtIdle, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.OK)
          break;
        this.CancelTest();
        break;
      case UserPanel.EngineState.StarterEngaged:
        this.ReportResult(Resources.Message_EngineStarting);
        break;
      case UserPanel.EngineState.Other:
        this.ReportResult(Resources.Message_CannotProceedUntilEngineIsAtIdle1);
        break;
      case UserPanel.EngineState.Idle:
        this.AdvanceTestStep();
        this.RunFuelSystemCheck();
        break;
      default:
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_EngineStateNotPossible0, (object) this.EngineStatus));
    }
  }

  private void ServiceNotFoundErrorMethod(string serviceName)
  {
    if (this.Online)
      this.ReportResult(Resources.Message_EndingTheTestTheService + serviceName + Resources.Message_IsNotAvailable);
    else
      this.ReportResult(Resources.Message_EndingTheTestTheMCMIsNoLongerConnected);
    this.EndTest();
    this.RunFuelSystemCheck();
  }

  private void CloseHCDoserValveStart()
  {
    Service service = this.GetService("MCM", "RT_SR003_PWM_Routine_for_Production_Start_PWM_Value");
    if (service != (Service) null)
    {
      this.ReportResult(Resources.Message_ClosingTheHCDoserValve);
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) this.HCDoserPwmIndex);
      service.InputValues[1].Value = (object) 0.0;
      service.InputValues[2].Value = (object) (float) UserPanel.TestDuration.TotalMilliseconds;
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR003_PWM_Routine_for_Production_Start_PWM_Value");
  }

  private void CloseHCDoserValveStop()
  {
    Service service = this.GetService("MCM", "RT_SR003_PWM_Routine_for_Production_Stop");
    if (service != (Service) null)
    {
      this.ReportResult(Resources.Message_ResettingHCDoserValve);
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) this.HCDoserPwmIndex);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoFail);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR003_PWM_Routine_for_Production_Stop");
  }

  private void OpenFuelCutOffValveStart()
  {
    Service service = this.GetService("MCM", "RT_SR005_SW_Routine_for_Production_Start_SW_Operation");
    if (service != (Service) null)
    {
      this.ReportResult(Resources.Message_OpeningFuelCutoffValve);
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) this.FuelCutoffValveSwIndex);
      service.InputValues[1].Value = (object) service.InputValues[1].Choices.GetItemFromRawValue((object) 1);
      service.InputValues[2].Value = (object) (float) UserPanel.SwRoutineDuration.TotalMilliseconds;
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      service.Execute(false);
      this.repeatSwRoutineStopwatch.Reset();
      this.repeatSwRoutineStopwatch.Start();
      this.repeatSwRoutineTimer.Tick += new EventHandler(this.OnRepeatSwRoutineTimerTick);
      this.repeatSwRoutineTimer.Start();
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR005_SW_Routine_for_Production_Start_SW_Operation");
  }

  private void OnRepeatSwRoutineTimerTick(object sender, EventArgs args)
  {
    if (!(this.repeatSwRoutineStopwatch.Elapsed >= UserPanel.RepeatSwRoutinePeriod))
      return;
    this.repeatSwRoutineStopwatch.Reset();
    Service service = this.GetService("MCM", "RT_SR005_SW_Routine_for_Production_Start_SW_Operation");
    if (service != (Service) null)
    {
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) this.FuelCutoffValveSwIndex);
      service.InputValues[1].Value = (object) service.InputValues[1].Choices.GetItemFromRawValue((object) 1);
      service.InputValues[2].Value = (object) (float) UserPanel.SwRoutineDuration.TotalMilliseconds;
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoAdvance);
      service.Execute(false);
      this.repeatSwRoutineStopwatch.Start();
    }
    else if (this.TestIsRunning)
    {
      this.StopRepeatSwRoutineTimer();
      if (!this.TestIsResetting)
      {
        this.ReportResult(Resources.Message_CancellingTheTestUnableToKeepTheFuelCutoffValveOpen);
        this.CancelTest();
      }
    }
  }

  private void OpenFuelCutOffValveStop()
  {
    this.StopRepeatSwRoutineTimer();
    Service service = this.GetService("MCM", "RT_SR005_SW_Routine_for_Production_Stop");
    if (service != (Service) null)
    {
      this.ReportResult(Resources.Message_ResettingFuelCutoffValve);
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) this.FuelCutoffValveSwIndex);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoFail);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR005_SW_Routine_for_Production_Stop");
  }

  private void StopRepeatSwRoutineTimer()
  {
    if (this.repeatSwRoutineTimer != null)
    {
      this.repeatSwRoutineTimer.Stop();
      this.repeatSwRoutineTimer.Tick -= new EventHandler(this.OnRepeatSwRoutineTimerTick);
    }
    if (this.repeatSwRoutineStopwatch == null)
      return;
    this.repeatSwRoutineStopwatch.Stop();
  }

  private void SetIdleStart(int idleSpeed)
  {
    Service service = this.GetService("MCM", "RT_SR015_Idle_Speed_Modification_Start");
    if (service != (Service) null)
    {
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_SettingEngineIdleSpeedTo0, (object) idleSpeed));
      service.InputValues[0].Value = (object) idleSpeed;
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoAdvance);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR015_Idle_Speed_Modification_Start");
  }

  private void SetIdleStopWaitUntilEngineSpeedIsStable()
  {
    Service service = this.GetService("MCM", "RT_SR015_Idle_Speed_Modification_Stop");
    if (service != (Service) null)
    {
      this.idleMonitor.BeginIdleStabilityMonitoring();
      this.ReportResult(Resources.Message_ResettingEngineIdleSpeed1);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoAdvance);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR015_Idle_Speed_Modification_Stop");
  }

  private void SetIdleStop()
  {
    Service service = this.GetService("MCM", "RT_SR015_Idle_Speed_Modification_Stop");
    if (service != (Service) null)
    {
      this.ReportResult(Resources.Message_ResettingEngineIdleSpeed);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoFail);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR015_Idle_Speed_Modification_Stop");
  }

  private static double FindFirstOkGradientCellLowerBoundary(DigitalReadoutInstrument instrument)
  {
    GradientCell gradientCell = instrument.Gradient.Cells.Where<GradientCell>((Func<GradientCell, bool>) (x => ((GradientCell) ref x).IsValid && ((GradientCell) ref x).State == 1)).FirstOrDefault<GradientCell>();
    return ((GradientCell) ref gradientCell).LowerBoundary;
  }

  private string GetMinimumOKValueString(
    DigitalReadoutInstrument digitalReadoutInstrument,
    Instrument instrument)
  {
    string minimumOkValueString = string.Empty;
    double cellLowerBoundary = UserPanel.FindFirstOkGradientCellLowerBoundary(digitalReadoutInstrument);
    if (!double.IsNaN(cellLowerBoundary))
    {
      Conversion conversion = Converter.GlobalInstance.GetConversion(instrument.Units);
      minimumOkValueString = conversion == null || !(conversion.OutputUnit != instrument.Units) ? Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) cellLowerBoundary, instrument.Units, instrument.Precision) + instrument.Units : Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) cellLowerBoundary, conversion, instrument.Precision) + conversion.OutputUnit;
    }
    return minimumOkValueString;
  }

  private void OverrideCalibrationForLeakDetectionTestStart()
  {
    Service service = this.GetService("MCM", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start");
    if (service != (Service) null)
    {
      service.InputValues[0].Value = (object) UserPanel.FindFirstOkGradientCellLowerBoundary(this.digitalReadoutInstrumentFuelTemperature);
      service.InputValues[1].Value = (object) UserPanel.FindFirstOkGradientCellLowerBoundary(this.digitalReadoutInstrumentCoolantTemperature);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Start");
  }

  private void ResetOverrideCalibrationForLeakDetectionTest()
  {
    Service service = this.GetService("MCM", "RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop");
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoFail);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR07D_Calibration_Override_for_Leak_Detection_Test_Stop");
  }

  private void SetRailPressureStart(double pressure)
  {
    Service service = this.GetService("MCM", "RT_SR076_Desired_Rail_Pressure_Start_Status");
    if (service != (Service) null)
    {
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_SettingDesiredRailPressureTo0, (object) pressure));
      service.InputValues[0].Value = (object) pressure;
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnSetRailPressureServiceCompleteEvent);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR076_Desired_Rail_Pressure_Start_Status");
  }

  private void OnSetRailPressureServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnSetRailPressureServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.leakTestPreTestPressure = 800.0;
    }
    else
    {
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ErrorSettingRailPressure0UsingIdleSpeedToIncreaseTheRailPressure, (object) e.Exception.Message));
      this.SetIdleStart(1400);
      this.leakTestPreTestPressure = 400.0;
    }
  }

  private void SetRailPressureStop()
  {
    Service service = this.GetService("MCM", "RT_SR076_Desired_Rail_Pressure_Stop");
    if (service != (Service) null)
    {
      this.ReportResult(Resources.Message_ResettingDesiredRailPressure);
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoFail);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR076_Desired_Rail_Pressure_Stop");
  }

  private void DisableHCDoserStart()
  {
    Service service = this.GetService("MCM", "RT_SR018_Disable_HC_Doser_Start");
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR018_Disable_HC_Doser_Start");
  }

  private void DisableHCDoserStop()
  {
    Service service = this.GetService("MCM", "RT_SR018_Disable_HC_Doser_Stop");
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoFail);
      service.Execute(false);
    }
    else
      this.ServiceNotFoundErrorMethod("RT_SR018_Disable_HC_Doser_Stop");
  }

  private void OnProcedureCompleteEvent(object sender, ResultEventArgs e)
  {
    this.stopEngineProcedure.OnProcedureCompleteEvent -= new UserPanel.StopEngineProcedure.ProcedureCompleteEventHandler(this.OnProcedureCompleteEvent);
    if (e.Succeeded)
    {
      this.RunFuelSystemCheck();
    }
    else
    {
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Error03, (object) e.Exception.Message));
      this.CancelTest();
    }
  }

  private void OnServiceCompleteEventNoAdvance(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoAdvance);
    if (e.Succeeded)
      return;
    this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Error02, (object) e.Exception.Message));
    this.CancelTest();
  }

  private void OnServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.RunFuelSystemCheck();
    }
    else
    {
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Error04, (object) e.Exception.Message));
      this.CancelTest();
    }
  }

  private void OnServiceCompleteEventNoFail(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceCompleteEventNoFail);
    if (!e.Succeeded)
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Error05, (object) e.Exception.Message));
    this.RunFuelSystemCheck();
  }

  private void StartAutomaticFsicTest()
  {
    if (!this.warningManager.RequestContinue())
      return;
    this.InitializeTest();
    this.ReportResult(Resources.Message_TheAutomaticFSICTestHasStarted);
    this.testStep = UserPanel.TestSteps.testStarted;
    this.RunFuelSystemCheck();
  }

  private void StartManualFsicTest()
  {
    if (this.testStep == UserPanel.TestSteps.notRunning)
    {
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.CurrentCulture, $"{Resources.MessageFormat_StartingTheManualFSICTestTheHCDoserWillRemainClosedAndTheFuelCutoffValveOpenFor0}\r\n{Resources.PressTheStopAllButtonToResetTheValves}", (object) UserPanel.DisplayTime(UserPanel.TestDuration)));
      this.testStep = UserPanel.TestSteps.manualTestStart;
      this.RunFuelSystemCheck();
    }
    this.UpdateUserInterface();
  }

  private void StartLeakTest()
  {
    this.leakDetectionTestRunning = true;
    this.testStep = UserPanel.TestSteps.testStarted;
    this.ReportResult(Resources.Message_StartingTheRailPressureBleedOffLeakDetectionTest);
    this.RunFuelSystemCheck();
    this.UpdateUserInterface();
  }

  private void OnButtonRunTestClick(object sender, EventArgs e)
  {
    switch (this.comboBoxSelectTest.SelectedIndex)
    {
      case 0:
        this.StartAutomaticFsicTest();
        break;
      case 1:
        this.StartManualFsicTest();
        break;
      case 2:
        this.StartLeakTest();
        break;
    }
  }

  private void StopTest()
  {
    this.ReportResult(Resources.Message_TestStoppedByUser);
    this.CancelTest();
    this.UpdateUserInterface();
  }

  private void OnButtonStopClick(object sender, EventArgs e) => this.StopTest();

  private void ComboBoxSelectTestSelectedIndexChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void SeekTimeListViewSelectedTimeChanged(object sender, EventArgs e)
  {
    this.chartInstrument1.SelectedTime = this.seekTimeListView.SelectedTime;
  }

  private void SeekTimeListViewTimeActivate(object sender, EventArgs e)
  {
    if (!SapiManager.GlobalInstance.LogFileIsOpen || !this.seekTimeListView.SelectedTime.HasValue)
      return;
    SapiManager.GlobalInstance.LogFileChannels.LogFile.CurrentTime = this.seekTimeListView.SelectedTime.Value;
  }

  private static string DisplayTime(TimeSpan time)
  {
    string str1 = string.Empty;
    string str2 = string.Empty;
    string str3 = string.Empty;
    if (time.Hours == 1)
      str1 = Resources.Message_1Hour;
    else if (time.Hours > 1)
      str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0Hours, (object) time.Hours);
    if (time.Hours > 0 && (time.Minutes > 0 || time.Seconds > 0))
      str1 += ", ";
    if (time.Minutes == 1)
      str2 = Resources.Message_1Minute;
    else if (time.Hours > 0 || time.Minutes > 1)
      str2 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0Minutes, (object) time.Minutes);
    if (time.Seconds > 0)
    {
      if (time.Hours > 0 || time.Minutes > 0)
        str2 += Resources.Message_And;
      str3 = time.Seconds != 1 ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0Seconds, (object) time.Seconds) : Resources.Message_1Second;
    }
    return str1 + str2 + str3;
  }

  private void ReportResult(string text)
  {
    this.AddStatusMessage(text);
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelTestControls = new TableLayoutPanel();
    this.checkmarkTestCanProceedStatus = new Checkmark();
    this.seekTimeListView = new SeekTimeListView();
    this.buttonRunTest = new Button();
    this.buttonStop = new Button();
    this.progressBarTest = new ProgressBar();
    this.scalingLabelTimerDisplay = new ScalingLabel();
    this.comboBoxSelectTest = new ComboBox();
    this.labelTestStartStatus = new System.Windows.Forms.Label();
    this.tableLayoutPanelRightColumn = new TableLayoutPanel();
    this.chartInstrument1 = new ChartInstrument();
    this.tableLayoutPanelLeftColumn = new TableLayoutPanel();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentActualFuelMass = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFuelTemperature = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentRailPressure = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentDesiredRailPressure = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFuelCompensationPressure = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentQuantityControlValveCurrent = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineState = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentKwNwValiditySignal = new DigitalReadoutInstrument();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.splitContainerWholePanel = new SplitContainer();
    ((Control) this.tableLayoutPanelTestControls).SuspendLayout();
    ((Control) this.tableLayoutPanelRightColumn).SuspendLayout();
    ((Control) this.tableLayoutPanelLeftColumn).SuspendLayout();
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    this.splitContainerWholePanel.BeginInit();
    this.splitContainerWholePanel.Panel1.SuspendLayout();
    this.splitContainerWholePanel.Panel2.SuspendLayout();
    this.splitContainerWholePanel.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTestControls, "tableLayoutPanelTestControls");
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.checkmarkTestCanProceedStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.seekTimeListView, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.buttonRunTest, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.buttonStop, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.progressBarTest, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.scalingLabelTimerDisplay, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.comboBoxSelectTest, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).Controls.Add((Control) this.labelTestStartStatus, 1, 0);
    ((Control) this.tableLayoutPanelTestControls).Name = "tableLayoutPanelTestControls";
    componentResourceManager.ApplyResources((object) this.checkmarkTestCanProceedStatus, "checkmarkTestCanProceedStatus");
    ((Control) this.checkmarkTestCanProceedStatus).Name = "checkmarkTestCanProceedStatus";
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Fuel System Integrity Check";
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).SetRowSpan((Control) this.seekTimeListView, 5);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).SetColumnSpan((Control) this.buttonRunTest, 2);
    componentResourceManager.ApplyResources((object) this.buttonRunTest, "buttonRunTest");
    this.buttonRunTest.Name = "buttonRunTest";
    this.buttonRunTest.UseCompatibleTextRendering = true;
    this.buttonRunTest.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).SetColumnSpan((Control) this.progressBarTest, 2);
    componentResourceManager.ApplyResources((object) this.progressBarTest, "progressBarTest");
    this.progressBarTest.Name = "progressBarTest";
    this.scalingLabelTimerDisplay.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelTimerDisplay, "scalingLabelTimerDisplay");
    this.scalingLabelTimerDisplay.FontGroup = (string) null;
    this.scalingLabelTimerDisplay.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelTimerDisplay).Name = "scalingLabelTimerDisplay";
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).SetColumnSpan((Control) this.comboBoxSelectTest, 3);
    componentResourceManager.ApplyResources((object) this.comboBoxSelectTest, "comboBoxSelectTest");
    this.comboBoxSelectTest.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboBoxSelectTest.FormattingEnabled = true;
    this.comboBoxSelectTest.Items.AddRange(new object[3]
    {
      (object) componentResourceManager.GetString("comboBoxSelectTest.Items"),
      (object) componentResourceManager.GetString("comboBoxSelectTest.Items1"),
      (object) componentResourceManager.GetString("comboBoxSelectTest.Items2")
    });
    this.comboBoxSelectTest.Name = "comboBoxSelectTest";
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).SetColumnSpan((Control) this.labelTestStartStatus, 2);
    componentResourceManager.ApplyResources((object) this.labelTestStartStatus, "labelTestStartStatus");
    this.labelTestStartStatus.Name = "labelTestStartStatus";
    ((TableLayoutPanel) this.tableLayoutPanelTestControls).SetRowSpan((Control) this.labelTestStartStatus, 2);
    this.labelTestStartStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelRightColumn, "tableLayoutPanelRightColumn");
    ((TableLayoutPanel) this.tableLayoutPanelRightColumn).Controls.Add((Control) this.tableLayoutPanelTestControls, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelRightColumn).Controls.Add((Control) this.chartInstrument1, 0, 0);
    ((Control) this.tableLayoutPanelRightColumn).Name = "tableLayoutPanelRightColumn";
    componentResourceManager.ApplyResources((object) this.chartInstrument1, "chartInstrument1");
    ((Control) this.chartInstrument1).Name = "chartInstrument1";
    this.chartInstrument1.SelectedTime = new DateTime?();
    this.chartInstrument1.ShowEvents = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelLeftColumn, "tableLayoutPanelLeftColumn");
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentActualFuelMass, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentFuelTemperature, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentRailPressure, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentDesiredRailPressure, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentFuelCompensationPressure, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentQuantityControlValveCurrent, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentVehicleStatus, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentEngineState, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentCoolantTemperature, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).Controls.Add((Control) this.digitalReadoutInstrumentKwNwValiditySignal, 0, 6);
    ((Control) this.tableLayoutPanelLeftColumn).Name = "tableLayoutPanelLeftColumn";
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).SetColumnSpan((Control) this.digitalReadoutInstrumentEngineSpeed, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).SetColumnSpan((Control) this.digitalReadoutInstrumentActualFuelMass, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentActualFuelMass, "digitalReadoutInstrumentActualFuelMass");
    this.digitalReadoutInstrumentActualFuelMass.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualFuelMass).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualFuelMass).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS087_Actual_Fuel_Mass");
    ((Control) this.digitalReadoutInstrumentActualFuelMass).Name = "digitalReadoutInstrumentActualFuelMass";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentActualFuelMass).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFuelTemperature, "digitalReadoutInstrumentFuelTemperature");
    this.digitalReadoutInstrumentFuelTemperature.FontGroup = "HalfWidth";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelTemperature).FreezeValue = false;
    this.digitalReadoutInstrumentFuelTemperature.Gradient.Initialize((ValueState) 3, 2, "degC");
    this.digitalReadoutInstrumentFuelTemperature.Gradient.Modify(1, -40.0, (ValueState) 1);
    this.digitalReadoutInstrumentFuelTemperature.Gradient.Modify(2, 150.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp");
    ((Control) this.digitalReadoutInstrumentFuelTemperature).Name = "digitalReadoutInstrumentFuelTemperature";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelTemperature).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).SetColumnSpan((Control) this.digitalReadoutInstrumentRailPressure, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentRailPressure, "digitalReadoutInstrumentRailPressure");
    this.digitalReadoutInstrumentRailPressure.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRailPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRailPressure).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS043_Rail_Pressure");
    ((Control) this.digitalReadoutInstrumentRailPressure).Name = "digitalReadoutInstrumentRailPressure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRailPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).SetColumnSpan((Control) this.digitalReadoutInstrumentDesiredRailPressure, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDesiredRailPressure, "digitalReadoutInstrumentDesiredRailPressure");
    this.digitalReadoutInstrumentDesiredRailPressure.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDesiredRailPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDesiredRailPressure).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS098_desired_rail_pressure");
    ((Control) this.digitalReadoutInstrumentDesiredRailPressure).Name = "digitalReadoutInstrumentDesiredRailPressure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDesiredRailPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).SetColumnSpan((Control) this.digitalReadoutInstrumentFuelCompensationPressure, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFuelCompensationPressure, "digitalReadoutInstrumentFuelCompensationPressure");
    this.digitalReadoutInstrumentFuelCompensationPressure.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelCompensationPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelCompensationPressure).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS024_Fuel_Compensation_Pressure");
    ((Control) this.digitalReadoutInstrumentFuelCompensationPressure).Name = "digitalReadoutInstrumentFuelCompensationPressure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelCompensationPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).SetColumnSpan((Control) this.digitalReadoutInstrumentQuantityControlValveCurrent, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentQuantityControlValveCurrent, "digitalReadoutInstrumentQuantityControlValveCurrent");
    this.digitalReadoutInstrumentQuantityControlValveCurrent.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantityControlValveCurrent).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantityControlValveCurrent).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS100_Quantity_Control_Valve_Current");
    ((Control) this.digitalReadoutInstrumentQuantityControlValveCurrent).Name = "digitalReadoutInstrumentQuantityControlValveCurrent";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantityControlValveCurrent).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleStatus, "digitalReadoutInstrumentVehicleStatus");
    this.digitalReadoutInstrumentVehicleStatus.FontGroup = "HalfWidth";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleStatus).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleStatus.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentVehicleStatus.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleStatus.Gradient.Modify(3, 2.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutInstrumentVehicleStatus).Name = "digitalReadoutInstrumentVehicleStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleStatus).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineState, "digitalReadoutInstrumentEngineState");
    this.digitalReadoutInstrumentEngineState.FontGroup = "HalfWidth";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).FreezeValue = false;
    this.digitalReadoutInstrumentEngineState.Gradient.Initialize((ValueState) 0, 7);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(7, 6.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS023_Engine_State");
    ((Control) this.digitalReadoutInstrumentEngineState).Name = "digitalReadoutInstrumentEngineState";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCoolantTemperature, "digitalReadoutInstrumentCoolantTemperature");
    this.digitalReadoutInstrumentCoolantTemperature.FontGroup = "HalfWidth";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).FreezeValue = false;
    this.digitalReadoutInstrumentCoolantTemperature.Gradient.Initialize((ValueState) 3, 1, "degC");
    this.digitalReadoutInstrumentCoolantTemperature.Gradient.Modify(1, 10.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp");
    ((Control) this.digitalReadoutInstrumentCoolantTemperature).Name = "digitalReadoutInstrumentCoolantTemperature";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelLeftColumn).SetColumnSpan((Control) this.digitalReadoutInstrumentKwNwValiditySignal, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentKwNwValiditySignal, "digitalReadoutInstrumentKwNwValiditySignal");
    this.digitalReadoutInstrumentKwNwValiditySignal.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentKwNwValiditySignal).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentKwNwValiditySignal).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_DS001_KW_NW_validity_signal");
    ((Control) this.digitalReadoutInstrumentKwNwValiditySignal).Name = "digitalReadoutInstrumentKwNwValiditySignal";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentKwNwValiditySignal).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.splitContainerWholePanel, 0, 0);
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    componentResourceManager.ApplyResources((object) this.splitContainerWholePanel, "splitContainerWholePanel");
    this.splitContainerWholePanel.Name = "splitContainerWholePanel";
    this.splitContainerWholePanel.Panel1.Controls.Add((Control) this.tableLayoutPanelLeftColumn);
    this.splitContainerWholePanel.Panel2.Controls.Add((Control) this.tableLayoutPanelRightColumn);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_FuelSystemEPA07");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelTestControls).ResumeLayout(false);
    ((Control) this.tableLayoutPanelRightColumn).ResumeLayout(false);
    ((Control) this.tableLayoutPanelLeftColumn).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    this.splitContainerWholePanel.Panel1.ResumeLayout(false);
    this.splitContainerWholePanel.Panel2.ResumeLayout(false);
    this.splitContainerWholePanel.EndInit();
    this.splitContainerWholePanel.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum TestSteps
  {
    notRunning,
    testStarted,
    disableHCDoserStart,
    setHcDoserValve,
    setFuelCutoffValve,
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
    resetFuelCutoffValve,
    resetHcDoserValve,
    resetAllCyliners,
    resetDisableHCDoserStart,
    resetComplete,
    manualTestStart,
    manualTestDisableHCDoserStart,
    manualTestSetHcDoserValve,
    manualTestSetFuelCutoffValve,
    manualTestRunning,
  }

  private enum FuelSystemTests
  {
    AutomaticFsicTest,
    ManualFsicTest,
    LeakDetectionTest,
  }

  private enum EngineState
  {
    Unknown = -1, // 0xFFFFFFFF
    Stopped = 0,
    StarterEngaged = 1,
    Other = 2,
    Idle = 3,
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
      this.minIdleSpeed = int.MaxValue;
      this.maxIdleSpeed = int.MinValue;
    }

    internal void CalculateIdleVariation(int engineSpeed)
    {
      if (engineSpeed < this.minIdleSpeed)
        this.minIdleSpeed = engineSpeed;
      if (engineSpeed > this.maxIdleSpeed)
        this.maxIdleSpeed = engineSpeed;
      if (this.idleLevelRange >= this.maxIdleSpeed - this.minIdleSpeed)
        return;
      this.idleLevelRange = this.maxIdleSpeed - this.minIdleSpeed;
    }

    internal bool IdleIsStable(InstrumentValueCollection engineSpeeds)
    {
      bool flag = false;
      if (DateTime.Now - this.setIdleStopTime > TimeSpan.FromSeconds(10.0))
      {
        InstrumentValue current = engineSpeeds.Current;
        if (Math.Abs(Convert.ToInt32(current.Value) - Convert.ToInt32((engineSpeeds.GetCurrentAtTime(DateTime.Now - TimeSpan.FromSeconds(10.0)) ?? current).Value)) < this.idleLevelRange)
          flag = true;
      }
      return flag;
    }

    internal void InitIdleMonitor()
    {
      this.idleLevelRange = 0;
      this.ResetMinMax();
    }

    internal void BeginIdleStabilityMonitoring() => this.setIdleStopTime = DateTime.Now;
  }

  private class FuelTemperatureTest
  {
    private const int LowFuelTemperatureSampleIdleSpeed = 850;
    private const int HighFuelTemperatureSampleIdleSpeed = 1800;
    private float minFuelTemperature;
    private float maxFuelTemperature;

    internal void InitFuelTemperatureTest()
    {
      this.minFuelTemperature = float.MaxValue;
      this.maxFuelTemperature = float.MinValue;
    }

    internal void CollectTemperatures(
      float fuelTemperature,
      UserPanel.TestSteps testStep,
      int idleSpeedIndex,
      int[] idleSpeeds)
    {
      if (testStep != UserPanel.TestSteps.idleSteadyState)
        return;
      if (idleSpeeds[idleSpeedIndex] == 850)
      {
        if ((double) fuelTemperature < (double) this.minFuelTemperature)
          this.minFuelTemperature = fuelTemperature;
      }
      else if (idleSpeeds[idleSpeedIndex] == 1800 && (double) fuelTemperature > (double) this.maxFuelTemperature)
        this.maxFuelTemperature = fuelTemperature;
    }

    internal string ReportChangeInFuelTemperature(Instrument fuelTemp)
    {
      Conversion conversion = Converter.GlobalInstance.GetConversion(fuelTemp.Units);
      float num1;
      float num2;
      string str1;
      if (conversion != null && conversion.OutputUnit != fuelTemp.Units)
      {
        num1 = (float) conversion.Convert((double) this.minFuelTemperature);
        num2 = (float) conversion.Convert((double) this.maxFuelTemperature);
        str1 = conversion.OutputUnit.ToString();
      }
      else
      {
        num1 = this.minFuelTemperature;
        num2 = this.maxFuelTemperature;
        str1 = fuelTemp.Units.ToString();
      }
      string str2 = Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) num1, str1, fuelTemp.Precision) + str1;
      string str3 = Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) num2, str1, fuelTemp.Precision) + str1;
      string str4 = Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) (float) ((double) num2 - (double) num1), str1, fuelTemp.Precision) + str1;
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TheMinimumFuelTemperatureAt0RPMWas1 + "\r\n", (object) 850, (object) str2) + string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TheMaximumFuelTemperatureAt0RPMWas1 + "\r\n", (object) 1800, (object) str3) + string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TheChangeInFuelTemperatureIs0 + "\r\n", (object) str4);
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
    private bool enabled;

    private DisplayCounter()
    {
    }

    internal DisplayCounter(TimeSpan duration, ScalingLabel label, ProgressBar progress)
    {
      this.updateCounter = new Timer();
      this.updateCounter.Interval = (int) TimeSpan.FromSeconds(1.0).TotalMilliseconds;
      this.duration = duration;
      this.outputText = label;
      this.progress = progress;
      this.endTime = DateTime.Now + duration;
      this.Reset();
    }

    internal TimeSpan Duration
    {
      get => this.duration;
      set
      {
        this.duration = !(value < UserPanel.DisplayCounter.updateInterval) ? value : throw new ArgumentOutOfRangeException("The total time cannot be smaller than the display interval.");
        this.progress.Maximum = (int) this.duration.TotalSeconds;
      }
    }

    internal void StartCountDown()
    {
      this.Enabled = true;
      this.endTime = DateTime.Now + this.duration;
      this.updateCounter.Tick += new EventHandler(this.UpdateCounterTick);
      this.updateCounter.Start();
      this.ShowValue();
    }

    internal void StopCountDown()
    {
      if (this.updateCounter != null && this.updateCounter.Enabled)
        this.updateCounter.Stop();
      this.updateCounter.Tick -= new EventHandler(this.UpdateCounterTick);
      this.progress.Value = 0;
      ((Control) this.outputText).Text = string.Empty;
    }

    internal void Reset()
    {
      this.StopCountDown();
      this.progress.Minimum = 0;
      this.progress.Maximum = (int) this.duration.TotalSeconds;
      this.Enabled = false;
    }

    internal bool Enabled
    {
      get => this.enabled;
      set
      {
        this.enabled = value;
        this.progress.Enabled = value;
        ((Control) this.outputText).Enabled = value;
      }
    }

    internal int Value
    {
      get
      {
        TimeSpan timeSpan = this.endTime - DateTime.Now;
        return timeSpan < new TimeSpan(0L) ? 0 : (int) timeSpan.TotalSeconds;
      }
    }

    internal void ShowValue()
    {
      ((Control) this.outputText).Text = this.Value.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      this.progress.Value = this.progress.Maximum - this.Value;
    }

    internal void UpdateCounterTick(object sender, EventArgs e) => this.ShowValue();
  }

  internal class StopEngineProcedure
  {
    private const int CylinderCount = 6;
    private const string CylinderCutStartStopRoutine = "RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder";
    private const string AllCylindersOnRoutine = "RT_SR004_Engine_Cylinder_Cut_Off_Stop";
    private const string EngineSpeedInstrumentQualifier = "DT_ASL002_Engine_Speed";
    private Channel channel;
    internal UserPanel.StopEngineProcedure.ProcedureCompleteEventHandler OnProcedureCompleteEvent;
    private string serviceExecuteList;

    private StopEngineProcedure()
    {
    }

    internal StopEngineProcedure(Channel channel) => this.channel = channel;

    internal Channel Mcm
    {
      get => this.channel;
      set
      {
        if (this.channel == value)
          return;
        if (this.channel != null)
          this.DisconnectInstrumentUpdateEvent();
        this.channel = value;
      }
    }

    private void DisconnectInstrumentUpdateEvent()
    {
      Instrument instrument = this.channel.Instruments["DT_ASL002_Engine_Speed"];
      if (!(instrument != (Instrument) null))
        return;
      instrument.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnEngineSpeedInstrumentUpdateEvent);
    }

    private string ServiceExecuteList
    {
      get
      {
        if (string.IsNullOrEmpty(this.serviceExecuteList))
        {
          StringBuilder stringBuilder = new StringBuilder();
          for (int index = 1; index <= 6; ++index)
            stringBuilder.AppendFormat("RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder({0},0);", (object) index);
          this.serviceExecuteList = stringBuilder.ToString();
        }
        return this.serviceExecuteList;
      }
    }

    internal bool SetCylindersOff(
      UserPanel.StopEngineProcedure.ProcedureCompleteEventHandler onProcedureCompleteEvent)
    {
      this.OnProcedureCompleteEvent = onProcedureCompleteEvent;
      if (this.OnProcedureCompleteEvent != null)
      {
        Instrument instrument = this.channel.Instruments["DT_ASL002_Engine_Speed"];
        if (!(instrument != (Instrument) null))
          return false;
        instrument.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnEngineSpeedInstrumentUpdateEvent);
      }
      return this.channel.Services.Execute(this.ServiceExecuteList, true) == 6;
    }

    private void OnEngineSpeedInstrumentUpdateEvent(object sender, ResultEventArgs e)
    {
      Instrument instrument = sender as Instrument;
      object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(instrument);
      if (instrumentCurrentValue == null || Convert.ToInt32(instrumentCurrentValue) != 0)
        return;
      instrument.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnEngineSpeedInstrumentUpdateEvent);
      this.OnProcedureCompleteEvent((object) this.OnProcedureCompleteEvent, e);
    }

    internal void SetCylindersOn(ServiceCompleteEventHandler onServiceCompleteEvent)
    {
      Service service = this.channel.Services["RT_SR004_Engine_Cylinder_Cut_Off_Stop"];
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(onServiceCompleteEvent.Invoke);
      service.Execute(false);
    }

    internal delegate void ProcedureCompleteEventHandler(object sender, ResultEventArgs e);
  }
}
