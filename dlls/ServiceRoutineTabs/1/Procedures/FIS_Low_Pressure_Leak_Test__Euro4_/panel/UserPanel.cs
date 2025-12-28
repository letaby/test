// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__Euro4_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__Euro4_.panel;

public class UserPanel : CustomPanel
{
  private const string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";
  private const string LppoPressureInstrumentQualifier = "DT_AS124_Low_Pressure_Pump_Outlet_Pressure";
  private const double ThresholdLeakValue = 0.03447;
  private Channel channel;
  private Instrument engineSpeed;
  private Instrument lppoPressure;
  private UserPanel.State currentState = UserPanel.State.NotRunning;
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
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label engineStatusLabel;
  private ChartInstrument chartInstrument;

  public UserPanel()
  {
    this.InitializeComponent();
    this.airPressureStabilizeTimer = new Timer();
    this.airPressureStabilizeTimer.Tick += new EventHandler(this.AirPressureStabilizeTimer_Tick);
    this.monitorLppoPresureTimer = new Timer();
    this.monitorLppoPresureTimer.Tick += new EventHandler(this.MonitorLppoPresureTimer_Tick);
    this.progressBarTimer = new Timer();
    this.progressBarTimer.Tick += new EventHandler(this.ProgressBarTimer_Tick);
    this.buttonStart.Click += new EventHandler(this.OnStartButton);
    this.buttonStop.Click += new EventHandler(this.OnStopButton);
    this.UpdateUserInterface();
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMcm(this.GetChannel("MCM"));
    this.UpdateUserInterface();
  }

  private void SetMcm(Channel mcmChannel)
  {
    if (this.channel == mcmChannel)
      return;
    if (this.channel != null)
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    if (this.engineSpeed != (Instrument) null)
    {
      this.engineSpeed.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
      this.engineSpeed = (Instrument) null;
    }
    if (this.lppoPressure != (Instrument) null)
      this.lppoPressure.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.LppoPressure_InstrumentUpdateEvent);
    this.channel = mcmChannel;
    if (this.channel != null)
    {
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.engineSpeed = this.channel.Instruments["DT_AS010_Engine_Speed"];
      this.lppoPressure = this.channel.Instruments["DT_AS124_Low_Pressure_Pump_Outlet_Pressure"];
      if (this.engineSpeed != (Instrument) null)
        this.engineSpeed.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
      if (this.lppoPressure != (Instrument) null)
        this.lppoPressure.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.LppoPressure_InstrumentUpdateEvent);
    }
  }

  private void OnStartButton(object sender, EventArgs e)
  {
    this.currentState = UserPanel.State.WaitingLetAirPressureStabilize;
    this.GoMachine();
  }

  private void OnStopButton(object sender, EventArgs e)
  {
    this.StopTest(Resources.Message_TestAbortedUserCanceledTheTest);
  }

  private void SampleFuelPressure()
  {
    double instrumentValue = this.GetInstrumentValue(this.lppoPressure);
    if (!double.IsNaN(instrumentValue))
    {
      if (this.initialLppoPressureReading - instrumentValue <= 0.03447)
        return;
      this.finalLppoPressureReading = instrumentValue;
      this.ReportLppoPressureResult();
      this.StopTest(Resources.Message_TestFailedLeakWasDetected);
    }
    else
      this.StopTest(Resources.Message_TestFailedPressureCouldNotBeRead);
  }

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
      {
        if (this.currentState != UserPanel.State.NotRunning)
          this.StopTest(Resources.Message_TestFailedEnigneIsRunning);
        str = Resources.Message_EngineIsRunningTestCannotStart;
      }
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

  private void ReportLppoPressureResult()
  {
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TheInitialLPPOPressureObservedWas0, (object) this.ReportInstrumentValue(this.lppoPressure, this.initialLppoPressureReading)));
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_TheFinalLPPOPressureObservedWas0, (object) this.ReportInstrumentValue(this.lppoPressure, this.finalLppoPressureReading)));
  }

  private string ReportInstrumentValue(Instrument instrument, double instrumentValue)
  {
    Conversion conversion = Converter.GlobalInstance.GetConversion(instrument.Units);
    return conversion == null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) instrumentValue, instrument.Units, instrument.Precision), (object) instrument.Units) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) instrumentValue, conversion, instrument.Precision), (object) conversion.OutputUnit);
  }

  private void AddLogLabel(string text)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void StopTest(string result)
  {
    this.airPressureStabilizeTimer.Stop();
    this.monitorLppoPresureTimer.Stop();
    this.progressBarTimer.Stop();
    this.progressBar.Value = 0;
    this.AddLogLabel(Resources.Message_TestCompleted);
    this.AddLogLabel(result);
    this.currentState = UserPanel.State.NotRunning;
    this.UpdateUserInterface();
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
        case UserPanel.State.WaitingLetAirPressureStabilize:
          this.progressBar.Minimum = 0;
          this.progressBar.Maximum = 300;
          this.progressBar.Value = 0;
          this.progressBarTimer.Interval = (int) TimeSpan.FromSeconds(1.0).TotalMilliseconds;
          this.airPressureStabilizeTimer.Interval = (int) TimeSpan.FromMinutes(5.0).TotalMilliseconds;
          this.airPressureStabilizeTimer.Start();
          this.progressBarTimer.Start();
          this.AddLogLabel(Resources.Message_WaitingFiveMinutesToLetTheAirPressureStabilize);
          break;
        case UserPanel.State.StartMonitorLppoPresure:
          this.progressBar.Minimum = 0;
          this.progressBar.Maximum = 600;
          this.progressBar.Value = 0;
          this.progressBarTimer.Interval = (int) TimeSpan.FromSeconds(1.0).TotalMilliseconds;
          this.monitorLppoPresureTimer.Interval = (int) TimeSpan.FromMinutes(10.0).TotalMilliseconds;
          this.monitorLppoPresureTimer.Start();
          this.progressBarTimer.Start();
          this.initialLppoPressureReading = this.GetInstrumentValue(this.lppoPressure);
          this.AddLogLabel(Resources.Message_WaitingTenMinutesWhileThePressureIsMonitoredForADropOfMoreThan5Psi);
          break;
      }
      this.UpdateUserInterface();
    }
  }

  private bool CanStart
  {
    get => this.Online && !this.TestRunning && !this.Busy && this.engineSpeedCheck.Checked;
  }

  private bool CanStop => this.Online && this.TestRunning;

  private bool Online => this.channel != null && this.channel.Online;

  private bool Busy
  {
    get => this.Online && this.channel.CommunicationsState != CommunicationsState.Online;
  }

  private bool TestRunning => this.currentState != UserPanel.State.NotRunning;

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnEngineSpeedUpdate(object sender, ResultEventArgs e) => this.UpdateUserInterface();

  private void AirPressureStabilizeTimer_Tick(object sender, EventArgs e)
  {
    this.airPressureStabilizeTimer.Stop();
    this.currentState = UserPanel.State.StartMonitorLppoPresure;
    this.GoMachine();
  }

  private void MonitorLppoPresureTimer_Tick(object sender, EventArgs e)
  {
    this.monitorLppoPresureTimer.Stop();
    this.StopTest(Resources.Message_TestPassedNoLeakDetected);
  }

  private void LppoPressure_InstrumentUpdateEvent(object sender, ResultEventArgs e)
  {
    if (this.currentState != UserPanel.State.StartMonitorLppoPresure)
      return;
    this.SampleFuelPressure();
  }

  private void ProgressBarTimer_Tick(object sender, EventArgs e) => ++this.progressBar.Value;

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.engineCheckTableLayoutPanel = new TableLayoutPanel();
    this.engineSpeedCheck = new Checkmark();
    this.engineStatusLabel = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.seekTimeListView = new SeekTimeListView();
    this.chartInstrument = new ChartInstrument();
    this.fuelCompensationPressureInstrument = new DigitalReadoutInstrument();
    this.progressBar = new ProgressBar();
    this.buttonStop = new Button();
    this.buttonStart = new Button();
    ((Control) this.engineCheckTableLayoutPanel).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.engineCheckTableLayoutPanel, "engineCheckTableLayoutPanel");
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
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.chartInstrument, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.fuelCompensationPressureInstrument, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.progressBar, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.engineCheckTableLayoutPanel, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStop, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStart, 0, 3);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "FIS Low Pressure Leak Test";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss:f";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.chartInstrument, 3);
    componentResourceManager.ApplyResources((object) this.chartInstrument, "chartInstrument");
    ((Collection<Qualifier>) this.chartInstrument.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure"));
    ((Control) this.chartInstrument).Name = "chartInstrument";
    this.chartInstrument.SelectedTime = new DateTime?();
    this.chartInstrument.ShowButtonPanel = false;
    this.chartInstrument.ShowEvents = false;
    this.chartInstrument.ShowLegend = false;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.fuelCompensationPressureInstrument, 2);
    componentResourceManager.ApplyResources((object) this.fuelCompensationPressureInstrument, "fuelCompensationPressureInstrument");
    this.fuelCompensationPressureInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.fuelCompensationPressureInstrument).FreezeValue = false;
    ((SingleInstrumentBase) this.fuelCompensationPressureInstrument).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS124_Low_Pressure_Pump_Outlet_Pressure");
    ((Control) this.fuelCompensationPressureInstrument).Name = "fuelCompensationPressureInstrument";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.fuelCompensationPressureInstrument, 2);
    ((SingleInstrumentBase) this.fuelCompensationPressureInstrument).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.progressBar, "progressBar");
    this.progressBar.Name = "progressBar";
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("PanelFISLowPressureLeakTest");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.engineCheckTableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum State
  {
    NotRunning,
    WaitingLetAirPressureStabilize,
    StartMonitorLppoPresure,
  }
}
