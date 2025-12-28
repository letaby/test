// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__EPA10_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private const string IdleSpeedModificationStartQualifier = "RT_SR015_Idle_Speed_Modification_Start";
  private const string IdleSpeedModificationStopQualifier = "RT_SR015_Idle_Speed_Modification_Stop";
  private const string ChargeAirCoolerOutTemperatureInstrumentQualifier = "DT_AS060_Charge_Air_Cooler_Outlet_Temperature";
  private const string CoolantTemperatureInstrumentQualifier = "DT_AS013_Coolant_Temperature";
  private const int TestIdleSpeed = 1200;
  private const int RunOffIdleSpeed = 1800;
  private static readonly TimeSpan testDurationThermalCondition = TimeSpan.FromMinutes(6.0);
  private static readonly TimeSpan maxTestDurationEGRInRange = TimeSpan.FromMinutes(30.0);
  private static readonly TimeSpan testDurationRunOffPeriod = TimeSpan.FromSeconds(30.0);
  private Channel channel;
  private UserPanel.State currentState = UserPanel.State.NotRunning;
  private UserPanel.WaitState currentWaitState = UserPanel.WaitState.NotWaiting;
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

  public UserPanel()
  {
    this.warningManager = new WarningManager(UserPanel.warningFormat, Resources.Message_EGRLowFlowTest);
    this.InitializeComponent();
    this.timer = new Timer();
    this.buttonStart.Click += new EventHandler(this.OnStartButton);
    this.buttonStop.Click += new EventHandler(this.OnStopButton);
    this.timer.Tick += new EventHandler(this.OnTimerTick);
    this.digitalReadoutVehicleCheckStatus.RepresentedStateChanged += new EventHandler(this.OnPreconditionStateChanged);
    this.digitalReadoutEngineSpeed.RepresentedStateChanged += new EventHandler(this.OnPreconditionStateChanged);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    this.UpdateUserInterface();
  }

  public virtual void OnChannelsChanged()
  {
    this.warningManager.Reset();
    this.SetChannel(this.GetChannel("MCM02T"));
  }

  private void SetChannel(Channel channel)
  {
    if (this.channel == channel)
      return;
    if (this.channel != null)
    {
      this.channel.Instruments["DT_AS060_Charge_Air_Cooler_Outlet_Temperature"].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnTemperaturePreconditionChanged);
      this.channel.Instruments["DT_AS013_Coolant_Temperature"].InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnTemperaturePreconditionChanged);
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    this.channel = channel;
    if (this.channel != null)
    {
      this.channel.Instruments["DT_AS060_Charge_Air_Cooler_Outlet_Temperature"].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnTemperaturePreconditionChanged);
      this.channel.Instruments["DT_AS013_Coolant_Temperature"].InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnTemperaturePreconditionChanged);
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    this.UpdateUserInterface();
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.TestRunning)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.buttonStart.Click -= new EventHandler(this.OnStartButton);
    this.buttonStop.Click -= new EventHandler(this.OnStopButton);
    this.timer.Tick -= new EventHandler(this.OnTimerTick);
    this.digitalReadoutVehicleCheckStatus.RepresentedStateChanged -= new EventHandler(this.OnPreconditionStateChanged);
    this.digitalReadoutEngineSpeed.RepresentedStateChanged -= new EventHandler(this.OnPreconditionStateChanged);
    this.SetChannel((Channel) null);
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.adrReturnValue,
      (object) this.textBoxOutput.Text
    };
  }

  private void OnStartButton(object sender, EventArgs e)
  {
    if (!this.warningManager.RequestContinue())
      return;
    this.startTime = DateTime.Now;
    this.currentState = UserPanel.State.Timer;
    this.currentWaitState = UserPanel.WaitState.NotWaiting;
    this.startTimeEGRInRange = DateTime.MinValue;
    this.startTimeRunOff = DateTime.MinValue;
    this.GoMachine();
  }

  private void OnStopButton(object sender, EventArgs e)
  {
    this.StopTest(UserPanel.Result.UserCanceled);
  }

  private void OnTimerTick(object sender, EventArgs e)
  {
    if (this.Online)
    {
      if (!this.TemperaturesOk)
      {
        this.StopTest(UserPanel.Result.FailTemperaturesOutOfRange);
      }
      else
      {
        switch (this.currentWaitState)
        {
          case UserPanel.WaitState.WaitingToInitiateRampUp:
            this.ManipulateIdleSpeed(1200);
            this.currentWaitState = UserPanel.WaitState.WaitingForRampUp;
            break;
          case UserPanel.WaitState.WaitingForRampUp:
            if (this.EngineAtTestSpeed)
            {
              this.currentWaitState = UserPanel.WaitState.WaitingForThermalCondition;
              this.Output(Resources.Message_EngineIsAtSpeedWaitingToGetPastThermalCondition);
              break;
            }
            break;
          case UserPanel.WaitState.WaitingForThermalCondition:
            if (DateTime.Now >= this.startTime + UserPanel.testDurationThermalCondition)
            {
              this.currentWaitState = UserPanel.WaitState.WaitingForEGRInRange;
              this.startTimeEGRInRange = DateTime.Now;
              this.Output(Resources.Message_ThermalConditionWaitCompleteWaitingForEGRToBeInRange);
              break;
            }
            break;
          case UserPanel.WaitState.WaitingForEGRInRange:
            if (this.EGRValveInTestRange)
            {
              this.Output(Resources.Message_EGRValveInRange);
              this.currentWaitState = UserPanel.WaitState.WaitingToInitiateRunOffRampUp;
              break;
            }
            if (DateTime.Now >= this.startTimeEGRInRange + UserPanel.maxTestDurationEGRInRange)
              this.StopTest(UserPanel.Result.FailEGRNotInRangeForPeriod);
            break;
          case UserPanel.WaitState.WaitingToInitiateRunOffRampUp:
            this.ManipulateIdleSpeed(1800);
            this.currentWaitState = UserPanel.WaitState.WaitingForRunOffRampUp;
            break;
          case UserPanel.WaitState.WaitingForRunOffRampUp:
            if (this.EngineAtRunOffSpeed)
            {
              this.currentWaitState = UserPanel.WaitState.WaitingForRunOffPeriod;
              this.startTimeRunOff = DateTime.Now;
              this.Output(Resources.Message_EngineIsAtRunoffSpeedWaitingForRunoffPeriod);
              break;
            }
            break;
          case UserPanel.WaitState.WaitingForRunOffPeriod:
            if (this.EGRValveInTestRange)
            {
              if (DateTime.Now >= this.startTimeRunOff + UserPanel.testDurationRunOffPeriod)
              {
                this.StopTest(this.TestPassed ? UserPanel.Result.CompletePass : UserPanel.Result.CompleteFail);
                break;
              }
              break;
            }
            this.Output(Resources.Message_EGRValveDroppedOutOfRangeWaitingForItToBeInRangeAgain);
            this.currentWaitState = UserPanel.WaitState.WaitingForEGRInRange;
            break;
        }
      }
      this.UpdateUserInterface();
    }
    else
      this.StopTest(UserPanel.Result.EcuOffline);
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnPreconditionStateChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void OnTemperaturePreconditionChanged(object sender, ResultEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.buttonStart.Enabled = this.CanStart;
    this.buttonStop.Enabled = this.CanStop;
    this.checkmarkCanStart.CheckState = this.CanStart || this.TestRunning ? CheckState.Checked : CheckState.Unchecked;
    string str = Resources.Message_TestCanStart0;
    if (!this.buttonStart.Enabled)
    {
      if (this.TestRunning)
      {
        switch (this.currentWaitState)
        {
          case UserPanel.WaitState.NotWaiting:
          case UserPanel.WaitState.WaitingToInitiateRampUp:
          case UserPanel.WaitState.WaitingToInitiateRunOffRampUp:
            str = Resources.Message_ServiceRoutinesInProgress;
            break;
          case UserPanel.WaitState.WaitingForRampUp:
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForEngineSpeedToReach0Rpm1, (object) 1200);
            break;
          case UserPanel.WaitState.WaitingForThermalCondition:
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ThermalConditionWait0SecondsRemaining, (object) (int) (this.startTime + UserPanel.testDurationThermalCondition - DateTime.Now).TotalSeconds);
            break;
          case UserPanel.WaitState.WaitingForEGRInRange:
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForEGRValveToReachRange0SecondsRemaining, (object) (int) (this.startTimeEGRInRange + UserPanel.maxTestDurationEGRInRange - DateTime.Now).TotalSeconds);
            break;
          case UserPanel.WaitState.WaitingForRunOffRampUp:
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForEngineSpeedToReach0Rpm, (object) 1800);
            break;
          case UserPanel.WaitState.WaitingForRunOffPeriod:
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_WaitingForRunoff0SecondsRemaining, (object) (int) (this.startTimeRunOff + UserPanel.testDurationRunOffPeriod - DateTime.Now).TotalSeconds);
            break;
        }
      }
      else if (this.Busy)
        str = Resources.Message_CannotStartTheTestAsTheDeviceIsBusy;
      else if (!this.Online)
        str = Resources.Message_CannotStartTheTestAsTheDeviceIsNotOnline;
      else if (!this.EngineRunning)
        str = Resources.Message_CannotStartTheTestAsTheEngineIsNotRunningStartTheEngine;
      else if (!this.VehicleCheckStatusOk)
        str = Resources.Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionInNeutral;
      else if (!this.TemperaturesOk)
        str = Resources.Message_CannotStartTheTestAsTheRequiredTemperaturesAreNotInRange;
    }
    this.labelCanStart.Text = str;
  }

  private void Output(string text)
  {
    TextBox textBoxOutput = this.textBoxOutput;
    textBoxOutput.Text = textBoxOutput.Text + text + Environment.NewLine;
    this.textBoxOutput.SelectionLength = 0;
    this.textBoxOutput.SelectionStart = this.textBoxOutput.Text.Length;
    this.textBoxOutput.ScrollToCaret();
  }

  private void StopTest(UserPanel.Result result)
  {
    this.timer.Stop();
    switch (result)
    {
      case UserPanel.Result.CompletePass:
        this.Output(Resources.Message_TESTCOMPLETEPASSED);
        this.Output(Resources.Message_CloseThisWindowToContinueTroubleshooting1);
        this.adrReturnValue = true;
        break;
      case UserPanel.Result.CompleteFail:
        this.Output(Resources.Message_TESTCOMPLETEFAILED);
        this.Output(Resources.Message_CloseThisWindowToContinueTroubleshooting);
        break;
      case UserPanel.Result.FailTemperaturesOutOfRange:
        this.Output(Resources.Message_TESTFAILEDTemperaturesFellOutRangeCorrectAndRestartTheTest);
        break;
      case UserPanel.Result.FailEGRNotInRangeForPeriod:
        this.Output(Resources.Message_TESTFAILEDEGRWasNotInRangeForTheRequiredPeriodCorrectAndRestartTheTest);
        break;
      case UserPanel.Result.UserCanceled:
        this.Output(Resources.Message_TESTABORTEDUserCanceledTheTest);
        break;
      case UserPanel.Result.ServiceFailure:
        this.Output(Resources.Message_TESTFAILEDServicesFailedToExecute);
        break;
      case UserPanel.Result.EcuOffline:
        this.Output(Resources.Message_TESTFAILEDDeviceWentOffline);
        break;
    }
    this.ReportInstrumentResults();
    this.UpdateUserInterface();
    this.currentWaitState = UserPanel.WaitState.NotWaiting;
    if (this.currentState <= UserPanel.State.RequestIdleModificationStop)
    {
      if (this.Online)
      {
        this.currentState = UserPanel.State.RequestIdleModificationStop;
        this.GoMachine();
      }
      else
      {
        this.Output(Resources.Message_UnableToRequestEndOfManipulation);
        this.currentState = UserPanel.State.NotRunning;
      }
    }
    else
      this.currentState = UserPanel.State.NotRunning;
  }

  private void ReportInstrumentResults()
  {
    this.GetInstrumentItemForDisplay((SingleInstrumentBase) this.barInstrumentEGRCommandedValue);
    this.GetInstrumentItemForDisplay((SingleInstrumentBase) this.barInstrumentEGRDeltaPressure);
    this.GetInstrumentItemForDisplay((SingleInstrumentBase) this.barInstrumentCoolantTemp);
    this.GetInstrumentItemForDisplay((SingleInstrumentBase) this.barInstrumentChargeAirCoolerOutTemp);
  }

  private void GoMachine()
  {
    if (!this.Online)
    {
      this.StopTest(UserPanel.Result.EcuOffline);
    }
    else
    {
      switch (this.currentState)
      {
        case UserPanel.State.Timer:
          this.currentWaitState = UserPanel.WaitState.WaitingToInitiateRampUp;
          this.timer.Interval = 1000;
          this.timer.Start();
          this.Output(Resources.Message_WaitingForTimer);
          this.UpdateUserInterface();
          break;
        case UserPanel.State.RequestIdleModificationStop:
          this.Output(Resources.Message_RequestEndIdleSpeedManipulation);
          this.channel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnIdleSpeedStopServiceComplete);
          this.channel.Services["RT_SR015_Idle_Speed_Modification_Stop"].Execute(false);
          break;
        case UserPanel.State.Stopping:
          this.Output(Resources.Message_TestSequenceEnded);
          this.currentState = UserPanel.State.NotRunning;
          return;
      }
      ++this.currentState;
    }
  }

  private void ManipulateIdleSpeed(int speed)
  {
    this.Output(Resources.Message_ManipulateEngineSpeedTo + (object) speed + Resources.Message_Rpm);
    this.channel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnIdleSpeedStartServiceComplete);
    Service service = this.channel.Services["RT_SR015_Idle_Speed_Modification_Start"];
    service.InputValues[0].Value = (object) speed;
    service.Execute(false);
  }

  private double GetInstrumentValue(SingleInstrumentBase control)
  {
    double instrumentValue = double.NaN;
    Qualifier instrument1 = control.Instrument;
    Channel channel = this.GetChannel(((Qualifier) ref instrument1).Ecu);
    if (channel != null)
    {
      InstrumentCollection instruments = channel.Instruments;
      Qualifier instrument2 = control.Instrument;
      string name = ((Qualifier) ref instrument2).Name;
      Instrument instrument3 = instruments[name];
      if (instrument3 != (Instrument) null && instrument3.InstrumentValues.Current != null && instrument3.InstrumentValues.Current.Value != null)
        instrumentValue = Convert.ToDouble(instrument3.InstrumentValues.Current.Value);
    }
    return instrumentValue;
  }

  private void GetInstrumentItemForDisplay(SingleInstrumentBase control)
  {
    if (!(DataItem.Create(control.Instrument, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels) is InstrumentDataItem instrumentDataItem))
      return;
    this.Output(string.Format(Resources.MessageFormat_TheObservedValueOf0Was12, (object) ((DataItem) instrumentDataItem).Name, (object) Math.Round(((DataItem) instrumentDataItem).ValueAsDouble(((DataItem) instrumentDataItem).Value), ((DataItem) instrumentDataItem).Precision).ToString(), (object) ((DataItem) instrumentDataItem).Units));
  }

  private void OnIdleSpeedStartServiceComplete(object sender, ResultEventArgs e)
  {
    this.channel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnIdleSpeedStartServiceComplete);
    if (e.Succeeded)
      return;
    this.Output(e.Exception.Message);
    this.StopTest(UserPanel.Result.ServiceFailure);
  }

  private void OnIdleSpeedStopServiceComplete(object sender, ResultEventArgs e)
  {
    this.channel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnIdleSpeedStopServiceComplete);
    if (!e.Succeeded)
      this.Output(e.Exception.Message);
    this.GoMachine();
  }

  private bool CanStart
  {
    get
    {
      return this.Online && !this.TestRunning && !this.Busy && this.VehicleCheckStatusOk && this.EngineRunning && this.TemperaturesOk;
    }
  }

  private bool CanStop => this.Online && this.TestRunning;

  private bool TestRunning => this.currentState != UserPanel.State.NotRunning;

  private bool Online => this.channel != null && this.channel.Online;

  private bool Busy
  {
    get => this.Online && this.channel.CommunicationsState != CommunicationsState.Online;
  }

  private bool EngineRunning => this.digitalReadoutEngineSpeed.RepresentedState != 3;

  private bool VehicleCheckStatusOk => this.digitalReadoutVehicleCheckStatus.RepresentedState != 3;

  private bool TemperaturesOk
  {
    get
    {
      return this.GetInstrumentValue((SingleInstrumentBase) this.barInstrumentChargeAirCoolerOutTemp) >= 16.0 && this.GetInstrumentValue((SingleInstrumentBase) this.barInstrumentCoolantTemp) >= 80.0;
    }
  }

  private bool TestPassed
  {
    get
    {
      double instrumentValue1 = this.GetInstrumentValue((SingleInstrumentBase) this.barInstrumentEGRCommandedValue);
      double instrumentValue2 = this.GetInstrumentValue((SingleInstrumentBase) this.barInstrumentEGRDeltaPressure);
      return instrumentValue1 >= 20.0 && instrumentValue1 <= 40.0 && instrumentValue2 >= 50.0 && instrumentValue2 <= 150.0;
    }
  }

  private bool EGRValveInTestRange
  {
    get
    {
      return this.GetInstrumentValue((SingleInstrumentBase) this.barInstrumentEGRCommandedValue) >= 20.0;
    }
  }

  private bool EngineAtTestSpeed => this.digitalReadoutEngineSpeed.RepresentedState == 2;

  private bool EngineAtRunOffSpeed => this.digitalReadoutEngineSpeed.RepresentedState == 1;

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.panelButtons = new Panel();
    this.barInstrumentCoolantTemp = new BarInstrument();
    this.textBoxOutput = new TextBox();
    this.barInstrumentEGRDeltaPressure = new BarInstrument();
    this.barInstrumentEGRCommandedValue = new BarInstrument();
    this.barInstrumentChargeAirCoolerOutTemp = new BarInstrument();
    this.digitalReadoutFault = new DigitalReadoutInstrument();
    this.digitalReadoutVehicleCheckStatus = new DigitalReadoutInstrument();
    this.digitalReadoutEngineSpeed = new DigitalReadoutInstrument();
    this.labelCanStart = new System.Windows.Forms.Label();
    this.checkmarkCanStart = new Checkmark();
    this.buttonStart = new Button();
    this.buttonStop = new Button();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    this.panelButtons.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.barInstrumentCoolantTemp, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.textBoxOutput, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.barInstrumentEGRDeltaPressure, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.barInstrumentEGRCommandedValue, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.barInstrumentChargeAirCoolerOutTemp, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutFault, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutVehicleCheckStatus, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutEngineSpeed, 0, 2);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    this.barInstrumentCoolantTemp.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentCoolantTemp.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentCoolantTemp, "barInstrumentCoolantTemp");
    this.barInstrumentCoolantTemp.FontGroup = "lowflow_temps";
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentCoolantTemp).Gradient.Initialize((ValueState) 3, 1, "°C");
    ((AxisSingleInstrumentBase) this.barInstrumentCoolantTemp).Gradient.Modify(1, 80.0, (ValueState) 1);
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS013_Coolant_Temperature");
    ((Control) this.barInstrumentCoolantTemp).Name = "barInstrumentCoolantTemp";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.barInstrumentCoolantTemp, 7);
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentCoolantTemp).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.textBoxOutput, 3);
    componentResourceManager.ApplyResources((object) this.textBoxOutput, "textBoxOutput");
    this.textBoxOutput.Name = "textBoxOutput";
    this.textBoxOutput.ReadOnly = true;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.textBoxOutput, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.barInstrumentEGRDeltaPressure, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentEGRDeltaPressure, "barInstrumentEGRDeltaPressure");
    this.barInstrumentEGRDeltaPressure.FontGroup = "lowflow_pressures";
    ((SingleInstrumentBase) this.barInstrumentEGRDeltaPressure).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentEGRDeltaPressure).Gradient.Initialize((ValueState) 3, 2, "mbar");
    ((AxisSingleInstrumentBase) this.barInstrumentEGRDeltaPressure).Gradient.Modify(1, 80.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.barInstrumentEGRDeltaPressure).Gradient.Modify(2, 151.0, (ValueState) 3);
    ((SingleInstrumentBase) this.barInstrumentEGRDeltaPressure).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS025_EGR_Delta_Pressure");
    ((Control) this.barInstrumentEGRDeltaPressure).Name = "barInstrumentEGRDeltaPressure";
    ((SingleInstrumentBase) this.barInstrumentEGRDeltaPressure).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.barInstrumentEGRCommandedValue, 3);
    componentResourceManager.ApplyResources((object) this.barInstrumentEGRCommandedValue, "barInstrumentEGRCommandedValue");
    this.barInstrumentEGRCommandedValue.FontGroup = "lowflow_pressures";
    ((SingleInstrumentBase) this.barInstrumentEGRCommandedValue).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentEGRCommandedValue).Gradient.Initialize((ValueState) 3, 2);
    ((AxisSingleInstrumentBase) this.barInstrumentEGRCommandedValue).Gradient.Modify(1, 20.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.barInstrumentEGRCommandedValue).Gradient.Modify(2, 41.0, (ValueState) 2);
    ((SingleInstrumentBase) this.barInstrumentEGRCommandedValue).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS031_EGR_Commanded_Governor_Value");
    ((Control) this.barInstrumentEGRCommandedValue).Name = "barInstrumentEGRCommandedValue";
    ((SingleInstrumentBase) this.barInstrumentEGRCommandedValue).UnitAlignment = StringAlignment.Near;
    this.barInstrumentChargeAirCoolerOutTemp.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrumentChargeAirCoolerOutTemp.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentChargeAirCoolerOutTemp, "barInstrumentChargeAirCoolerOutTemp");
    this.barInstrumentChargeAirCoolerOutTemp.FontGroup = "lowflow_temps";
    ((SingleInstrumentBase) this.barInstrumentChargeAirCoolerOutTemp).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentChargeAirCoolerOutTemp).Gradient.Initialize((ValueState) 3, 1, "°C");
    ((AxisSingleInstrumentBase) this.barInstrumentChargeAirCoolerOutTemp).Gradient.Modify(1, 16.0, (ValueState) 1);
    ((SingleInstrumentBase) this.barInstrumentChargeAirCoolerOutTemp).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS060_Charge_Air_Cooler_Outlet_Temperature");
    ((Control) this.barInstrumentChargeAirCoolerOutTemp).Name = "barInstrumentChargeAirCoolerOutTemp";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.barInstrumentChargeAirCoolerOutTemp, 7);
    ((SingleInstrumentBase) this.barInstrumentChargeAirCoolerOutTemp).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentChargeAirCoolerOutTemp).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentChargeAirCoolerOutTemp).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.digitalReadoutFault, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutFault, "digitalReadoutFault");
    this.digitalReadoutFault.FontGroup = "lowflow_digitals";
    ((SingleInstrumentBase) this.digitalReadoutFault).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutFault).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MCM02T", "630A12");
    ((Control) this.digitalReadoutFault).Name = "digitalReadoutFault";
    ((SingleInstrumentBase) this.digitalReadoutFault).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.digitalReadoutVehicleCheckStatus, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutVehicleCheckStatus, "digitalReadoutVehicleCheckStatus");
    this.digitalReadoutVehicleCheckStatus.FontGroup = "lowflow_digitals";
    ((SingleInstrumentBase) this.digitalReadoutVehicleCheckStatus).FreezeValue = false;
    this.digitalReadoutVehicleCheckStatus.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutVehicleCheckStatus).Name = "digitalReadoutVehicleCheckStatus";
    ((SingleInstrumentBase) this.digitalReadoutVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.digitalReadoutEngineSpeed, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutEngineSpeed, "digitalReadoutEngineSpeed");
    this.digitalReadoutEngineSpeed.FontGroup = "lowflow_digitals";
    ((SingleInstrumentBase) this.digitalReadoutEngineSpeed).FreezeValue = false;
    this.digitalReadoutEngineSpeed.Gradient.Initialize((ValueState) 3, 3);
    this.digitalReadoutEngineSpeed.Gradient.Modify(1, 150.0, (ValueState) 0);
    this.digitalReadoutEngineSpeed.Gradient.Modify(2, 1150.0, (ValueState) 2);
    this.digitalReadoutEngineSpeed.Gradient.Modify(3, 1750.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS010_Engine_Speed");
    ((Control) this.digitalReadoutEngineSpeed).Name = "digitalReadoutEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutEngineSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.panelButtons, "panelButtons");
    this.panelButtons.Controls.Add((Control) this.labelCanStart);
    this.panelButtons.Controls.Add((Control) this.checkmarkCanStart);
    this.panelButtons.Controls.Add((Control) this.buttonStart);
    this.panelButtons.Controls.Add((Control) this.buttonStop);
    this.panelButtons.Name = "panelButtons";
    componentResourceManager.ApplyResources((object) this.labelCanStart, "labelCanStart");
    this.labelCanStart.Name = "labelCanStart";
    this.labelCanStart.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkCanStart, "checkmarkCanStart");
    ((Control) this.checkmarkCanStart).Name = "checkmarkCanStart";
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Controls.Add((Control) this.panelButtons);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    this.panelButtons.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum State
  {
    NotRunning,
    Timer,
    RequestIdleModificationStop,
    Stopping,
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
    WaitingForRunOffPeriod,
  }

  private enum Result
  {
    CompletePass,
    CompleteFail,
    FailTemperaturesOutOfRange,
    FailEGRNotInRangeForPeriod,
    UserCanceled,
    ServiceFailure,
    EcuOffline,
  }
}
