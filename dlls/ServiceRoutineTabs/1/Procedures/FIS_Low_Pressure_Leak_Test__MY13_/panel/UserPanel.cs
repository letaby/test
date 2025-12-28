// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__MY13_.panel;

public class UserPanel : CustomPanel
{
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
  private Instrument engineSpeed = (Instrument) null;
  private Instrument fuelCompensationPressure = (Instrument) null;
  private UserPanel.State currentState = UserPanel.State.NotRunning;
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
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label engineStatusLabel;
  private ChartInstrument chartInstrument;
  private TableLayoutPanel tableLayoutPanel2;
  private Button buttonStop;
  private Button buttonStart;
  private DigitalReadoutInstrument fuelCompensationPressureInstrument;
  private ScalingLabel labelNote;

  public UserPanel()
  {
    this.InitializeComponent();
    this.intialWaitTimer = new Timer();
    this.beforeDosingWaitTimer = new Timer();
    this.openHcDoserWaitTimer = new Timer();
    this.afterDosingWaitTimer = new Timer();
    this.sampleRecordWaitTimer = new Timer();
    this.buttonStart.Click += new EventHandler(this.OnStartButton);
    this.buttonStop.Click += new EventHandler(this.OnStopButton);
    this.intialWaitTimer.Tick += new EventHandler(this.OnInitialWaitTimerTick);
    this.beforeDosingWaitTimer.Tick += new EventHandler(this.OnBeforeDosingWaitTimerTick);
    this.openHcDoserWaitTimer.Tick += new EventHandler(this.OnOpenHcDoserWaitTimerTick);
    this.afterDosingWaitTimer.Tick += new EventHandler(this.OnAfterDosingWaitTimerTick);
    this.sampleRecordWaitTimer.Tick += new EventHandler(this.OnSampleRecordWaitTimerTick);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM21T"));
    this.UpdateUserInterface();
  }

  private void SetMCM(Channel channel)
  {
    if (this.channel == channel)
      return;
    if (this.channel != null)
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    if (this.engineSpeed != (Instrument) null)
    {
      this.engineSpeed.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
      this.engineSpeed = (Instrument) null;
    }
    this.channel = channel;
    if (this.channel != null)
    {
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.engineSpeed = this.channel.Instruments["DT_AS010_Engine_Speed"];
      this.fuelCompensationPressure = this.channel.Instruments["DT_AS024_Fuel_Compensation_Pressure"];
      if (this.engineSpeed != (Instrument) null)
        this.engineSpeed.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
    }
  }

  private void OnStartButton(object sender, EventArgs e)
  {
    this.currentState = UserPanel.State.OpenFuelCutoffValve;
    this.GoMachine();
  }

  private void OnStopButton(object sender, EventArgs e)
  {
    this.StopTest(Resources.Message_TestAbortedUserCanceledTheTest0);
  }

  private void OnInitialWaitTimerTick(object sender, EventArgs e)
  {
    this.intialWaitTimer.Stop();
    double instrumentValue = this.GetInstrumentValue(this.fuelCompensationPressure);
    if (!double.IsNaN(instrumentValue))
    {
      this.initialFuelCompensationPressureReading = instrumentValue;
      this.GoMachine();
    }
    else
      this.StopTest(Resources.Message_TestAbortedUnableToReadTheFuelCompensationPressureValue);
  }

  private void OnBeforeDosingWaitTimerTick(object sender, EventArgs e)
  {
    this.beforeDosingWaitTimer.Stop();
    this.GoMachine();
  }

  private void OnOpenHcDoserWaitTimerTick(object sender, EventArgs e)
  {
    this.openHcDoserWaitTimer.Stop();
    this.GoMachine();
  }

  private void OnAfterDosingWaitTimerTick(object sender, EventArgs e)
  {
    this.afterDosingWaitTimer.Stop();
    double instrumentValue = this.GetInstrumentValue(this.fuelCompensationPressure);
    if (!double.IsNaN(instrumentValue))
    {
      if (instrumentValue >= this.initialFuelCompensationPressureReading - this.requiredPressureDropValue)
      {
        this.StopTest(Resources.Message_TestWillBeAbortedPleaseTurnOffTheAirSupplyAndEnsureFuelHasBeenPurgedFromTheSystemAndPerformTheTestAgain);
      }
      else
      {
        this.initialFuelCompensationPressureReading = instrumentValue;
        this.sampleMonitoringStartTime = DateTime.Now;
        this.AddLogLabel(Resources.Message_LowPressureLeakDetectionCheckStarted);
        this.WaitForNextSampleTime(instrumentValue);
      }
    }
    else
      this.StopTest(Resources.Message_TestAbortedUnableToReadTheFuelCompensationPressureValue1);
  }

  private void OnSampleRecordWaitTimerTick(object sender, EventArgs e)
  {
    this.sampleRecordWaitTimer.Stop();
    double instrumentValue = this.GetInstrumentValue(this.fuelCompensationPressure);
    if (double.IsNaN(instrumentValue))
      return;
    if (this.initialFuelCompensationPressureReading - instrumentValue > this.thresholdLeakValue)
    {
      this.finalFuelCompensationPressureReading = instrumentValue;
      this.ReportFuelPressureResult();
      this.StopTest(Resources.Message_TestFailedLeakWasDetected);
    }
    else if (instrumentValue >= this.initialFuelCompensationPressureReading + this.thresholdLeakValue)
    {
      this.StopTest(Resources.Message_TestWillBeAbortedPleaseTurnOffTheAirSupplyAndEnsureFuelHasBeenPurgedFromTheSystemAndPerformTheTestAgain1);
    }
    else
    {
      this.AddLogLabel(Resources.Message_CheckInProgress);
      this.WaitForNextSampleTime(instrumentValue);
    }
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnEngineSpeedUpdate(object sender, ResultEventArgs e) => this.UpdateUserInterface();

  private void UpdateUserInterface()
  {
    this.UpdateEngineStatus();
    this.buttonStart.Enabled = this.CanStart;
    this.buttonStop.Enabled = this.CanStop;
  }

  private void UpdateEngineStatus()
  {
    bool flag = false;
    string str = Resources.Message_EngineSpeedCannotBeDetected;
    double instrumentValue = this.GetInstrumentValue(this.engineSpeed);
    if (!double.IsNaN(instrumentValue))
    {
      if (instrumentValue == 0.0)
      {
        str = Resources.Message_EngineIsNotRunningTestCanStart;
        flag = true;
      }
      else
        str = Resources.Message_EngineIsRunningTestCannotStart;
    }
    ((Control) this.engineStatusLabel).Text = str;
    this.engineSpeedCheck.Checked = flag;
  }

  private double GetInstrumentValue(Instrument instrument)
  {
    double result = double.NaN;
    if (instrument != (Instrument) null && instrument.InstrumentValues.Current != null && double.TryParse(instrument.InstrumentValues.Current.Value.ToString(), out result))
      result = Math.Round(result, 2);
    return result;
  }

  private void WaitForNextSampleTime(double currentInstrumentValue)
  {
    if (DateTime.Now <= this.sampleMonitoringStartTime + UserPanel.maxSamplesMonitoredDuration)
    {
      this.sampleRecordWaitTimer.Interval = 30000;
      this.sampleRecordWaitTimer.Start();
    }
    else
    {
      this.finalFuelCompensationPressureReading = currentInstrumentValue;
      this.ReportFuelPressureResult();
      this.StopTest(Resources.Message_TestPassedLeakWasNotDetected);
    }
  }

  private void ReportFuelPressureResult()
  {
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TheIntialFuelCompensationPressureObservedWas0, (object) this.ReportInstrumentValue(this.fuelCompensationPressure, this.initialFuelCompensationPressureReading)));
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TheFinalFuelCompensationPressureObservedWas0, (object) this.ReportInstrumentValue(this.fuelCompensationPressure, this.finalFuelCompensationPressureReading)));
  }

  private string ReportInstrumentValue(Instrument instrument, double instrumentValue)
  {
    string empty = string.Empty;
    Conversion conversion = Converter.GlobalInstance.GetConversion(instrument.Units);
    return conversion != null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) instrumentValue, conversion, instrument.Precision), (object) conversion.OutputUnit) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) instrumentValue, instrument.Units, instrument.Precision), (object) instrument.Units);
  }

  private void AddLogLabel(string text)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void StopTest(string result)
  {
    this.intialWaitTimer.Stop();
    this.beforeDosingWaitTimer.Stop();
    this.openHcDoserWaitTimer.Stop();
    this.afterDosingWaitTimer.Stop();
    this.sampleRecordWaitTimer.Stop();
    this.AddLogLabel(result);
    this.UpdateUserInterface();
    if (this.currentState >= UserPanel.State.HcDoserWaitTimer)
    {
      if (this.Online)
      {
        this.currentState = UserPanel.State.RequestHcDoserValveStop;
        this.GoMachine();
      }
      else
      {
        this.AddLogLabel(Resources.Message_UnableToRequestStopFuelCutoffAndHCDoserValveRoutines);
        this.currentState = UserPanel.State.NotRunning;
      }
    }
    else if (this.currentState >= UserPanel.State.WaitingToRecordIntialSample)
    {
      if (this.Online)
      {
        this.currentState = UserPanel.State.RequestFuelCutoffValveStop;
        this.GoMachine();
      }
      else
      {
        this.AddLogLabel(Resources.Message_UnableToRequestStopFuelCutoffRoutine);
        this.currentState = UserPanel.State.NotRunning;
      }
    }
    else
      this.currentState = UserPanel.State.NotRunning;
  }

  private void GoMachine()
  {
    if (!this.Online)
    {
      this.StopTest(Resources.Message_TestAbortedDeviceWentOffline);
    }
    else
    {
      switch (this.currentState)
      {
        case UserPanel.State.OpenFuelCutoffValve:
          this.AddLogLabel(Resources.Message_RequestingOpenFuelCutoffValve);
          this.OpenFuelCutOffValve();
          break;
        case UserPanel.State.WaitingToRecordIntialSample:
          this.intialWaitTimer.Interval = (int) TimeSpan.FromSeconds(20.0).TotalMilliseconds;
          this.intialWaitTimer.Start();
          this.AddLogLabel(Resources.Message_WaitingFor20Seconds);
          break;
        case UserPanel.State.WaitingBeforeOpeningHcDoserValve:
          this.beforeDosingWaitTimer.Interval = (int) TimeSpan.FromSeconds(10.0).TotalMilliseconds;
          this.beforeDosingWaitTimer.Start();
          this.AddLogLabel(Resources.Message_WaitingFor10Seconds);
          break;
        case UserPanel.State.OpenHcDoserValve:
          this.AddLogLabel(Resources.Message_RequestingOpenHCDoserValve);
          this.OpenHCDoserValve();
          break;
        case UserPanel.State.HcDoserWaitTimer:
          Timer hcDoserWaitTimer = this.openHcDoserWaitTimer;
          TimeSpan timeSpan1 = TimeSpan.FromSeconds(15.0);
          int totalMilliseconds1 = (int) timeSpan1.TotalMilliseconds;
          hcDoserWaitTimer.Interval = totalMilliseconds1;
          this.openHcDoserWaitTimer.Start();
          CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
          string valveFor0For1Seconds = Resources.MessageFormat_OpenedTheHCDoserValveFor0For1Seconds;
          // ISSUE: variable of a boxed type
          __Boxed<int> local = (ValueType) 25;
          timeSpan1 = UserPanel.OpenHCDoserControlTime;
          // ISSUE: variable of a boxed type
          __Boxed<int> totalSeconds = (ValueType) (int) timeSpan1.TotalSeconds;
          this.AddLogLabel(string.Format((IFormatProvider) currentCulture1, valveFor0For1Seconds, (object) local, (object) totalSeconds));
          break;
        case UserPanel.State.CloseHcDoserValve:
          this.AddLogLabel(Resources.Message_RequestingCloseHCDoserValve);
          this.CloseHCDoserValve();
          break;
        case UserPanel.State.WaitingToRecordAfterDosingSample:
          CultureInfo currentCulture2 = CultureInfo.CurrentCulture;
          string valveFor0Minutes = Resources.MessageFormat_ClosedTheHCDoserValveFor0Minutes;
          TimeSpan timeSpan2 = UserPanel.testDuration;
          // ISSUE: variable of a boxed type
          __Boxed<int> totalMinutes = (ValueType) (int) timeSpan2.TotalMinutes;
          this.AddLogLabel(string.Format((IFormatProvider) currentCulture2, valveFor0Minutes, (object) totalMinutes));
          Timer afterDosingWaitTimer = this.afterDosingWaitTimer;
          timeSpan2 = TimeSpan.FromSeconds(45.0);
          int totalMilliseconds2 = (int) timeSpan2.TotalMilliseconds;
          afterDosingWaitTimer.Interval = totalMilliseconds2;
          this.afterDosingWaitTimer.Start();
          this.AddLogLabel(Resources.Message_WaitingFor45Seconds);
          break;
        case UserPanel.State.RequestHcDoserValveStop:
          this.AddLogLabel(Resources.Message_RequestingStopHCDoserValve);
          this.StopHCDoserValve();
          break;
        case UserPanel.State.RequestFuelCutoffValveStop:
          this.AddLogLabel(Resources.Message_RequestingStopFuelCutoffValve);
          this.StopFuelCutOffValve();
          break;
        case UserPanel.State.Stopping:
          this.AddLogLabel(Resources.Message_TestCompleted);
          this.currentState = UserPanel.State.NotRunning;
          return;
      }
      ++this.currentState;
    }
  }

  private void OpenFuelCutOffValve()
  {
    this.SetPwmRoutineStart(UserPanel.PWMFunctionIndices.FuelCutOffValve, 100, (int) UserPanel.testDuration.TotalMilliseconds);
  }

  private void OpenHCDoserValve()
  {
    this.SetPwmRoutineStart(UserPanel.PWMFunctionIndices.HCDoser, 25, (int) UserPanel.OpenHCDoserControlTime.TotalMilliseconds);
  }

  private void CloseHCDoserValve()
  {
    this.SetPwmRoutineStart(UserPanel.PWMFunctionIndices.HCDoser, 0, (int) UserPanel.testDuration.TotalMilliseconds);
  }

  private void SetPwmRoutineStart(
    UserPanel.PWMFunctionIndices pwmFunctionIndex,
    int controlValue,
    int controlTime)
  {
    Service service = this.GetService("MCM21T", "RT_SR003_PWM_Routine_by_Function_for_Production_Start_Control_Value");
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0To1, (object) SapiExtensions.NameFromRawValue(service.InputValues[0].Choices, (object) pwmFunctionIndex), (object) controlValue));
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) pwmFunctionIndex);
    service.InputValues[1].Value = (object) controlValue;
    service.InputValues[2].Value = (object) controlTime;
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnPwmRoutineServiceCompleteEvent);
    service.Execute(false);
  }

  private void OnPwmRoutineServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnPwmRoutineServiceCompleteEvent);
    if (e.Succeeded)
      this.GoMachine();
    else
      this.StopTest(e.Exception.Message);
  }

  private void StopFuelCutOffValve()
  {
    this.SetPwmRoutineStop(UserPanel.PWMFunctionIndices.FuelCutOffValve);
  }

  private void StopHCDoserValve() => this.SetPwmRoutineStop(UserPanel.PWMFunctionIndices.HCDoser);

  private void SetPwmRoutineStop(UserPanel.PWMFunctionIndices pwmFunctionIndex)
  {
    Service service = this.GetService("MCM21T", "RT_SR003_PWM_Routine_by_Function_for_Production_Stop_Function_Name");
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Resetting0, (object) SapiExtensions.NameFromRawValue(service.InputValues[0].Choices, (object) pwmFunctionIndex)));
    service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) pwmFunctionIndex);
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnPwmRoutineStopServiceCompleteEvent);
    service.Execute(false);
  }

  private void OnPwmRoutineStopServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnPwmRoutineStopServiceCompleteEvent);
    if (!e.Succeeded)
      this.AddLogLabel(e.Exception.Message);
    this.GoMachine();
  }

  private bool CanStart
  {
    get => this.Online && !this.TestRunning && !this.Busy && this.engineSpeedCheck.Checked;
  }

  private bool CanStop => this.Online && this.TestRunning;

  private bool CanReset => this.Online && !this.TestRunning;

  private bool Online => this.channel != null && this.channel.Online;

  private bool Busy
  {
    get => this.Online && this.channel.CommunicationsState != CommunicationsState.Online;
  }

  private bool TestRunning => this.currentState != UserPanel.State.NotRunning;

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.buttonStop = new Button();
    this.buttonStart = new Button();
    this.engineCheckTableLayoutPanel = new TableLayoutPanel();
    this.engineSpeedCheck = new Checkmark();
    this.engineStatusLabel = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.seekTimeListView = new SeekTimeListView();
    this.chartInstrument = new ChartInstrument();
    this.fuelCompensationPressureInstrument = new DigitalReadoutInstrument();
    this.labelNote = new ScalingLabel();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.engineCheckTableLayoutPanel).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonStop, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonStart, 0, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.engineCheckTableLayoutPanel, "engineCheckTableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.engineCheckTableLayoutPanel, 3);
    ((TableLayoutPanel) this.engineCheckTableLayoutPanel).Controls.Add((Control) this.engineSpeedCheck, 0, 0);
    ((TableLayoutPanel) this.engineCheckTableLayoutPanel).Controls.Add((Control) this.engineStatusLabel, 1, 0);
    ((Control) this.engineCheckTableLayoutPanel).Name = "engineCheckTableLayoutPanel";
    componentResourceManager.ApplyResources((object) this.engineSpeedCheck, "engineSpeedCheck");
    ((Control) this.engineSpeedCheck).Name = "engineSpeedCheck";
    this.engineStatusLabel.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.engineStatusLabel, "engineStatusLabel");
    ((Control) this.engineStatusLabel).Name = "engineStatusLabel";
    this.engineStatusLabel.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.engineStatusLabel.ShowBorder = false;
    this.engineStatusLabel.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.engineCheckTableLayoutPanel, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.chartInstrument, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.fuelCompensationPressureInstrument, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelNote, 0, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView, 2);
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "FIS Low Pressure Leak Test";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView, 2);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss:f";
    componentResourceManager.ApplyResources((object) this.chartInstrument, "chartInstrument");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.chartInstrument, 3);
    ((Collection<Qualifier>) this.chartInstrument.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure"));
    ((Control) this.chartInstrument).Name = "chartInstrument";
    this.chartInstrument.SelectedTime = new DateTime?();
    this.chartInstrument.ShowButtonPanel = false;
    this.chartInstrument.ShowEvents = false;
    this.chartInstrument.ShowLegend = false;
    componentResourceManager.ApplyResources((object) this.fuelCompensationPressureInstrument, "fuelCompensationPressureInstrument");
    this.fuelCompensationPressureInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.fuelCompensationPressureInstrument).FreezeValue = false;
    ((SingleInstrumentBase) this.fuelCompensationPressureInstrument).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS024_Fuel_Compensation_Pressure");
    ((Control) this.fuelCompensationPressureInstrument).Name = "fuelCompensationPressureInstrument";
    ((SingleInstrumentBase) this.fuelCompensationPressureInstrument).UnitAlignment = StringAlignment.Near;
    this.labelNote.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelNote, 3);
    componentResourceManager.ApplyResources((object) this.labelNote, "labelNote");
    this.labelNote.FontGroup = (string) null;
    this.labelNote.LineAlignment = StringAlignment.Center;
    ((Control) this.labelNote).Name = "labelNote";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("PanelFISLowPressureLeakTest");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.engineCheckTableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

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
    Stopping,
  }

  private enum PWMFunctionIndices
  {
    HCDoser = 10, // 0x0000000A
    FuelCutOffValve = 15, // 0x0000000F
  }
}
