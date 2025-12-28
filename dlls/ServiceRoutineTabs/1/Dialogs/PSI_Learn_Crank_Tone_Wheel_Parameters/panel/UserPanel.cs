// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel;

public class UserPanel : CustomPanel
{
  private const int TestTimeoutSeconds = 60;
  private const double StabilizationFactor = 0.86;
  private const int TestCompletedSeconds = 6;
  private const string SeedMessage = "2701";
  private const string KeyMessage = "2702BBBB";
  private const string ShortTermAdjustMessage = "2F0519030001";
  private const string ShortTermAdjustResponse = "6F051903";
  private const string ReturnControlMessage = "2F051900";
  private const string ReturnControlResponse = "6F051900";
  private Channel mt88;
  private Channel mt88Uds;
  private bool working = false;
  private bool ecuUnlocked = false;
  private bool ecuSeeded = false;
  private bool pedalWasPressed = false;
  private bool aborted = false;
  private ProcessState state = ProcessState.NotRunning;
  private double maxRpm = 0.0;
  private int secondsRunning = 0;
  private int secondsAtMaxRpm = 0;
  private Timer checkStateTimer;
  private WarningManager warningManager;
  private TableLayoutPanel tableLayoutPanelMain;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineCoolant;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkingBreak;
  private DigitalReadoutInstrument digitalReadoutInstrumentBatteryVoltage;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;
  private Button buttonStartRoutine;
  private Button buttonStopRoutine;
  private DigitalReadoutInstrument digitalReadoutInstrumentAccelerator;
  private TableLayoutPanel tableLayoutPanelClose;
  private System.Windows.Forms.Label labelInstructions;
  private Button buttonClose;
  private Checkmark checkmarkState;
  private TableLayoutPanel tableLayoutPanelButtons;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransmissionGear;
  private System.Windows.Forms.Label labelInstructionsBig;
  private TimerControl timerControlEngineShutoff;
  private SeekTimeListView seekTimeListView;

  private bool mt88Online => this.mt88Uds != null && this.mt88 != null;

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningManager = new WarningManager(string.Empty, Resources.Message_PSILearnCrankToneWheelParameters, this.seekTimeListView.RequiredUserLabelPrefix);
    this.checkStateTimer = new Timer();
    this.checkStateTimer.Tick += new EventHandler(this.checkState_Tick);
    this.checkStateTimer.Interval = 1000;
    this.checkStateTimer.Enabled = false;
    this.buttonClose.Enabled = true;
    this.labelInstructionsBig.Text = this.labelInstructions.Text = Resources.Message_TheProcedureCanNotStart;
    this.checkmarkState.CheckState = CheckState.Unchecked;
    this.UpdateUI();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  protected virtual void Dispose(bool disposing)
  {
    if (this.checkStateTimer != null)
    {
      this.checkStateTimer.Stop();
      this.checkStateTimer.Dispose();
      this.checkStateTimer = (Timer) null;
    }
    base.Dispose(disposing);
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.state != ProcessState.Complete && this.state != ProcessState.NotRunning && this.state != ProcessState.WaitingOnShutdown)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.state = ProcessState.NotRunning;
    if (this.mt88Online)
      this.mt88Uds.Disconnect();
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void LogMessage(string message)
  {
    if (this.labelInstructions.Text != message)
      this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, message);
    this.labelInstructionsBig.Text = this.labelInstructions.Text = message;
  }

  public virtual void OnChannelsChanged()
  {
    Channel channel1 = this.GetChannel("MT88ECU", (CustomPanel.ChannelLookupOptions) 1);
    Channel channel2 = this.GetChannel("UDS-0", (CustomPanel.ChannelLookupOptions) 1);
    if (this.mt88 != channel1)
    {
      if (this.mt88 != null)
        this.state = ProcessState.NotRunning;
      this.mt88 = channel1;
      if (this.mt88 != null && this.mt88Uds == null && channel2 == null)
      {
        this.ConnectToGenericUds(0U);
        this.ecuUnlocked = false;
        this.ecuSeeded = false;
        this.state = ProcessState.NotRunning;
        this.warningManager.Reset();
      }
    }
    if (this.mt88Uds != channel2)
    {
      if (this.mt88Uds != null)
      {
        this.mt88Uds.ByteMessageCompleteEvent -= new ByteMessageCompleteEventHandler(this.OnByteMessageComplete);
        if (channel2 != null)
          this.state = ProcessState.NotRunning;
        else if (this.state == ProcessState.WaitingOnShutdown)
        {
          this.timerControlEngineShutoff.Reset();
          this.timerControlEngineShutoff.Start();
        }
      }
      this.mt88Uds = channel2;
      if (this.mt88Uds != null)
        this.mt88Uds.ByteMessageCompleteEvent += new ByteMessageCompleteEventHandler(this.OnByteMessageComplete);
    }
    this.UpdateUI();
  }

  public void ConnectToGenericUds(uint sourceAddress)
  {
    Sapi sapi = Sapi.GetSapi();
    DiagnosisProtocol diagnosisProtocol = sapi.DiagnosisProtocols["UDS"];
    ConnectionResource resource = (ConnectionResource) null;
    int num1;
    if (sapi != null)
    {
      ConnectionResourceCollection connectionResources = diagnosisProtocol.GetConnectionResources((byte) sourceAddress);
      Func<ConnectionResource, bool> predicate = (Func<ConnectionResource, bool>) (cr => cr.Type == "CANHS" && cr.PortIndex == 1);
      num1 = (resource = connectionResources.FirstOrDefault<ConnectionResource>(predicate)) == null ? 1 : 0;
    }
    else
      num1 = 1;
    if (num1 != 0)
      return;
    uint num2 = (uint) (416940273 + ((int) sourceAddress << 8));
    uint num3 = 417001728U + sourceAddress;
    resource.Ecu.EcuInfoComParameters[(object) "CP_IDENTIFIER_LENGTH"] = (object) 29;
    resource.Ecu.EcuInfoComParameters[(object) "CP_REQUEST_CANIDENTIFIER"] = (object) num2;
    resource.Ecu.EcuInfoComParameters[(object) "CP_RESPONSE_CANIDENTIFIER"] = (object) num3;
    sapi.Channels.Connect(resource, false);
  }

  private void OnByteMessageComplete(object sender, ResultEventArgs e)
  {
    ByteMessage byteMessage = sender as ByteMessage;
    if (e.Succeeded && byteMessage != null)
    {
      switch (this.state)
      {
        case ProcessState.RequestSeed:
          this.ecuSeeded = true;
          this.GoMachine();
          break;
        case ProcessState.SendKey:
          this.ecuUnlocked = true;
          this.GoMachine();
          break;
        case ProcessState.SetShortTermAdjust:
          if (byteMessage.Response.ToString().Equals("6F051903", StringComparison.OrdinalIgnoreCase))
          {
            this.state = ProcessState.Running;
            this.checkStateTimer.Enabled = true;
            this.checkStateTimer.Start();
            break;
          }
          this.LogMessage(Resources.Message_SetShortTermAdjustRequestFailed);
          this.checkmarkState.CheckState = CheckState.Unchecked;
          this.state = ProcessState.NotRunning;
          this.checkStateTimer.Enabled = false;
          this.checkStateTimer.Stop();
          break;
        case ProcessState.ReturnControl:
          if (!byteMessage.Response.ToString().Equals("6F051900", StringComparison.OrdinalIgnoreCase))
            this.state = ProcessState.ReturnControlFailed;
          this.GoMachine();
          break;
      }
    }
    else
      this.state = ProcessState.NotRunning;
    this.UpdateUI();
  }

  private void EvaluateResults()
  {
    if (this.aborted)
    {
      this.LogMessage(Resources.Message_AcceleratorReleasedBeforeProcedureHadCompleted);
      this.checkmarkState.CheckState = CheckState.Indeterminate;
    }
    else if (this.secondsRunning > 60)
    {
      this.LogMessage(string.Format(Resources.MessageFormat_RoutineTimeoutProcessExceeded0Seconds, (object) 60));
      this.checkmarkState.CheckState = CheckState.Unchecked;
      this.state = ProcessState.Complete;
    }
    else if (this.state == ProcessState.WaitingOnShutdown)
    {
      if (this.secondsAtMaxRpm > 6)
        this.LogMessage(Resources.Message_RoutineCompletedAutomaticallyReleaseAcceleratorTurnTheIgnitionOffFor15SecondsToFinalizeValues);
      else
        this.LogMessage(Resources.MessageFormat_RoutineTerminatedManuallyTurnTheIgnitionOffFor15SecondsToFinalizeValues);
      this.checkmarkState.CheckState = CheckState.Checked;
    }
    else
    {
      this.LogMessage(Resources.Message_ErrorCouldNotDisableShortTermAdjustment);
      this.checkmarkState.CheckState = CheckState.Unchecked;
    }
  }

  private void GoMachine()
  {
    switch (this.state)
    {
      case ProcessState.Start:
        this.checkmarkState.CheckState = CheckState.Checked;
        this.state = ProcessState.RequestSeed;
        if (!this.ecuSeeded)
        {
          this.SendMessage("2701");
          break;
        }
        this.GoMachine();
        break;
      case ProcessState.RequestSeed:
        this.state = ProcessState.SendKey;
        if (!this.ecuUnlocked)
        {
          this.SendMessage("2702BBBB");
          break;
        }
        this.GoMachine();
        break;
      case ProcessState.SendKey:
        this.state = ProcessState.SetShortTermAdjust;
        this.secondsRunning = 0;
        this.secondsAtMaxRpm = 0;
        this.SendMessage("2F0519030001");
        break;
      case ProcessState.Stopping:
        this.checkStateTimer.Enabled = false;
        this.checkStateTimer.Stop();
        this.state = ProcessState.ReturnControl;
        this.SendMessage("2F051900");
        break;
      case ProcessState.ReturnControl:
        this.state = ProcessState.WaitingOnShutdown;
        this.EvaluateResults();
        break;
      case ProcessState.ReturnControlFailed:
        this.EvaluateResults();
        this.state = ProcessState.Complete;
        break;
    }
    this.UpdateUI();
  }

  private void SendMessage(string message)
  {
    if (this.mt88Uds != null && this.mt88Uds.CommunicationsState == CommunicationsState.Online)
      this.mt88Uds.SendByteMessage(new Dump(message), false);
    else
      this.state = ProcessState.NotRunning;
  }

  private void UpdateUI()
  {
    this.buttonStopRoutine.Enabled = this.mt88Online && this.state == ProcessState.Running;
    this.buttonClose.Enabled = this.state == ProcessState.Complete || this.state == ProcessState.NotRunning;
    this.EnableStartButton();
    switch (this.state)
    {
      case ProcessState.RequestSeed:
      case ProcessState.SendKey:
      case ProcessState.SetShortTermAdjust:
        this.LogMessage(Resources.Message_StartingProcedure);
        break;
      case ProcessState.Running:
        this.LogMessage(Resources.Message_PressAndHoldAccelerator);
        break;
      case ProcessState.Stopping:
        this.LogMessage(Resources.Message_StoppingProcedure);
        break;
    }
  }

  private void buttonStartRoutine_Click(object sender, EventArgs e)
  {
    if (!this.warningManager.RequestContinue())
      return;
    this.state = ProcessState.Start;
    this.pedalWasPressed = false;
    this.aborted = false;
    this.GoMachine();
  }

  private void buttonStopRoutine_Click(object sender, EventArgs e)
  {
    this.state = ProcessState.Stopping;
    this.GoMachine();
  }

  private void checkState_Tick(object sender, EventArgs e)
  {
    ++this.secondsRunning;
    double result1 = 0.0;
    if (((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).DataItem != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).DataItem.Value != null)
      double.TryParse(((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).DataItem.Value.ToString(), out result1);
    double result2 = 0.0;
    if (((SingleInstrumentBase) this.digitalReadoutInstrumentAccelerator).DataItem != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentAccelerator).DataItem.Value != null)
      double.TryParse(((SingleInstrumentBase) this.digitalReadoutInstrumentAccelerator).DataItem.Value.ToString(), out result2);
    if (result2 > 95.0)
    {
      if (result1 > 500.0 && result1 >= this.maxRpm * 0.86)
      {
        if (result1 > this.maxRpm)
          this.maxRpm = result1;
        ++this.secondsAtMaxRpm;
      }
      else
        this.secondsAtMaxRpm = 0;
      if (!this.pedalWasPressed)
      {
        this.pedalWasPressed = true;
        this.LogMessage(Resources.Message_ContinueToHoldAccelerator);
      }
    }
    if (this.state == ProcessState.Running && this.secondsAtMaxRpm <= 6 && this.secondsRunning <= 60)
      return;
    this.state = ProcessState.Stopping;
    this.GoMachine();
  }

  private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
    if (!this.InstrumentErrors(true) && this.mt88Online)
    {
      this.labelInstructionsBig.Text = this.labelInstructions.Text = Resources.Message_TheProcedureCanStart;
      this.checkmarkState.CheckState = CheckState.Checked;
    }
    else
    {
      if (this.mt88Online)
        return;
      this.checkmarkState.CheckState = CheckState.Unchecked;
      this.labelInstructionsBig.Text = this.labelInstructions.Text = Resources.Message_TheProcedureCanNotStart;
    }
  }

  private void EnableStartButton()
  {
    this.buttonStartRoutine.Enabled = this.mt88Online && !this.working && (this.state == ProcessState.Complete || this.state == ProcessState.NotRunning) && !this.InstrumentErrors(false);
  }

  private bool CheckInstrumentForErrors(
    DigitalReadoutInstrument digitalReadoutInstrument,
    string errorMessage,
    bool displayMessage)
  {
    if (this.state != ProcessState.WaitingOnShutdown && displayMessage && digitalReadoutInstrument.RepresentedState != 1)
    {
      this.labelInstructionsBig.Text = this.labelInstructions.Text = errorMessage;
      this.checkmarkState.CheckState = CheckState.Unchecked;
    }
    return digitalReadoutInstrument.RepresentedState != 1;
  }

  private bool InstrumentErrors(bool displayMessage)
  {
    return this.CheckInstrumentForErrors(this.digitalReadoutInstrumentEngineCoolant, Resources.Message_EngineCoolantTemperatureIsLow, displayMessage) || this.CheckInstrumentForErrors(this.digitalReadoutInstrumentEngineSpeed, Resources.Message_EngineSpeedMustBeAbove200RPM, displayMessage) || this.CheckInstrumentForErrors(this.digitalReadoutInstrumentBatteryVoltage, Resources.Message_BatteryVoltageMustBeBetween1116Volts, displayMessage) || this.CheckInstrumentForErrors(this.digitalReadoutInstrumentTransmissionGear, Resources.Message_TransmissionMustBeInParkOrNeutral, displayMessage) || this.CheckInstrumentForErrors(this.digitalReadoutInstrumentParkingBreak, Resources.Message_ParkingBrakeMustBeOn, displayMessage);
  }

  private void timerControlEngineShutoff_TimerCountdownCompleted(object sender, EventArgs e)
  {
    this.timerControlEngineShutoff.Stop();
    this.LogMessage(Resources.Message_RoutineComplete);
  }

  private void timerControlEngineShutoff_TimerDisplayUpdated(object sender, EventArgs e)
  {
    Channel channel1 = this.GetChannel("MT88ECU", (CustomPanel.ChannelLookupOptions) 1);
    Channel channel2 = this.GetChannel("UDS-0", (CustomPanel.ChannelLookupOptions) 1);
    if (channel1 != null && channel2 != null && this.state == ProcessState.WaitingOnShutdown)
    {
      this.LogMessage(Resources.Message_IgnitionTurnedOnBeforeLearnComplete);
      this.state = ProcessState.NotRunning;
      this.timerControlEngineShutoff.Stop();
    }
    else
      this.LogMessage(string.Format(Resources.MessageFormat_KeepTheIgnitionOffFor0Seconds, (object) this.timerControlEngineShutoff.RemainingSeconds));
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.digitalReadoutInstrumentEngineCoolant = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentAccelerator = new DigitalReadoutInstrument();
    this.tableLayoutPanelClose = new TableLayoutPanel();
    this.labelInstructions = new System.Windows.Forms.Label();
    this.buttonClose = new Button();
    this.checkmarkState = new Checkmark();
    this.digitalReadoutInstrumentBatteryVoltage = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkingBreak = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentTransmissionGear = new DigitalReadoutInstrument();
    this.tableLayoutPanelButtons = new TableLayoutPanel();
    this.buttonStopRoutine = new Button();
    this.buttonStartRoutine = new Button();
    this.seekTimeListView = new SeekTimeListView();
    this.timerControlEngineShutoff = new TimerControl();
    this.labelInstructionsBig = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelClose).SuspendLayout();
    ((Control) this.tableLayoutPanelButtons).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentEngineCoolant, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentAccelerator, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelClose, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentBatteryVoltage, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentParkingBreak, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.digitalReadoutInstrumentTransmissionGear, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelButtons, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelInstructionsBig, 0, 0);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineCoolant, "digitalReadoutInstrumentEngineCoolant");
    this.digitalReadoutInstrumentEngineCoolant.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineCoolant).FreezeValue = false;
    this.digitalReadoutInstrumentEngineCoolant.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrumentEngineCoolant.Gradient.Modify(1, 60.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineCoolant).Instrument = new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_110");
    ((Control) this.digitalReadoutInstrumentEngineCoolant).Name = "digitalReadoutInstrumentEngineCoolant";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineCoolant).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentEngineCoolant.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 200.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_190");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAccelerator, "digitalReadoutInstrumentAccelerator");
    this.digitalReadoutInstrumentAccelerator.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAccelerator).FreezeValue = false;
    this.digitalReadoutInstrumentAccelerator.Gradient.Initialize((ValueState) 3, 4);
    this.digitalReadoutInstrumentAccelerator.Gradient.Modify(1, 0.0, (ValueState) 2);
    this.digitalReadoutInstrumentAccelerator.Gradient.Modify(2, 90.0, (ValueState) 1);
    this.digitalReadoutInstrumentAccelerator.Gradient.Modify(3, 110.0, (ValueState) 3);
    this.digitalReadoutInstrumentAccelerator.Gradient.Modify(4, double.NaN, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAccelerator).Instrument = new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_91");
    ((Control) this.digitalReadoutInstrumentAccelerator).Name = "digitalReadoutInstrumentAccelerator";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAccelerator).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelClose, "tableLayoutPanelClose");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelClose, 4);
    ((TableLayoutPanel) this.tableLayoutPanelClose).Controls.Add((Control) this.labelInstructions, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelClose).Controls.Add((Control) this.buttonClose, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelClose).Controls.Add((Control) this.checkmarkState, 0, 0);
    ((Control) this.tableLayoutPanelClose).Name = "tableLayoutPanelClose";
    componentResourceManager.ApplyResources((object) this.labelInstructions, "labelInstructions");
    this.labelInstructions.ForeColor = SystemColors.ControlText;
    this.labelInstructions.Name = "labelInstructions";
    this.labelInstructions.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.checkmarkState.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkState, "checkmarkState");
    ((Control) this.checkmarkState).Name = "checkmarkState";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBatteryVoltage, "digitalReadoutInstrumentBatteryVoltage");
    this.digitalReadoutInstrumentBatteryVoltage.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryVoltage).FreezeValue = false;
    this.digitalReadoutInstrumentBatteryVoltage.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentBatteryVoltage.Gradient.Modify(1, 11.0, (ValueState) 1);
    this.digitalReadoutInstrumentBatteryVoltage.Gradient.Modify(2, 16.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryVoltage).Instrument = new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_168");
    ((Control) this.digitalReadoutInstrumentBatteryVoltage).Name = "digitalReadoutInstrumentBatteryVoltage";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryVoltage).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentBatteryVoltage.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkingBreak, "digitalReadoutInstrumentParkingBreak");
    this.digitalReadoutInstrumentParkingBreak.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBreak).FreezeValue = false;
    this.digitalReadoutInstrumentParkingBreak.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentParkingBreak.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkingBreak.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBreak).Instrument = new Qualifier((QualifierTypes) 1, "MT88ECU", "DT_70");
    ((Control) this.digitalReadoutInstrumentParkingBreak).Name = "digitalReadoutInstrumentParkingBreak";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBreak).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentParkingBreak.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransmissionGear, "digitalReadoutInstrumentTransmissionGear");
    this.digitalReadoutInstrumentTransmissionGear.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionGear).FreezeValue = false;
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Initialize((ValueState) 3, 12);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(7, 6.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(8, 7.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(9, 8.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(10, 9.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(11, 10.0, (ValueState) 3);
    this.digitalReadoutInstrumentTransmissionGear.Gradient.Modify(12, "Parameter specific ($FB)", (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionGear).Instrument = new Qualifier((QualifierTypes) 1, "J1939-3", "DT_523");
    ((Control) this.digitalReadoutInstrumentTransmissionGear).Name = "digitalReadoutInstrumentTransmissionGear";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionGear).ShowUnits = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionGear).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionGear).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentTransmissionGear.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelButtons, "tableLayoutPanelButtons");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelButtons, 2);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.buttonStopRoutine, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.buttonStartRoutine, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.seekTimeListView, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.timerControlEngineShutoff, 0, 1);
    ((Control) this.tableLayoutPanelButtons).Name = "tableLayoutPanelButtons";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.tableLayoutPanelButtons, 3);
    componentResourceManager.ApplyResources((object) this.buttonStopRoutine, "buttonStopRoutine");
    this.buttonStopRoutine.Name = "buttonStopRoutine";
    this.buttonStopRoutine.UseCompatibleTextRendering = true;
    this.buttonStopRoutine.UseVisualStyleBackColor = true;
    this.buttonStopRoutine.Click += new EventHandler(this.buttonStopRoutine_Click);
    componentResourceManager.ApplyResources((object) this.buttonStartRoutine, "buttonStartRoutine");
    this.buttonStartRoutine.Name = "buttonStartRoutine";
    this.buttonStartRoutine.UseCompatibleTextRendering = true;
    this.buttonStartRoutine.UseVisualStyleBackColor = true;
    this.buttonStartRoutine.Click += new EventHandler(this.buttonStartRoutine_Click);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).SetColumnSpan((Control) this.seekTimeListView, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "PSI Learn Crank Tone Wheel Parameters";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.timerControlEngineShutoff.Duration = TimeSpan.Parse("00:00:15");
    this.timerControlEngineShutoff.FontGroup = (string) null;
    componentResourceManager.ApplyResources((object) this.timerControlEngineShutoff, "timerControlEngineShutoff");
    ((Control) this.timerControlEngineShutoff).Name = "timerControlEngineShutoff";
    this.timerControlEngineShutoff.TimerCountdownCompletedDisplayMessage = (string) null;
    this.timerControlEngineShutoff.TimerCountdownCompleted += new EventHandler(this.timerControlEngineShutoff_TimerCountdownCompleted);
    this.timerControlEngineShutoff.TimerDisplayUpdated += new EventHandler(this.timerControlEngineShutoff_TimerDisplayUpdated);
    componentResourceManager.ApplyResources((object) this.labelInstructionsBig, "labelInstructionsBig");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.labelInstructionsBig, 4);
    this.labelInstructionsBig.ForeColor = Color.Red;
    this.labelInstructionsBig.Name = "labelInstructionsBig";
    this.labelInstructionsBig.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_PSI_Learn_Crank_Tone_Wheel_Parameters");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanelClose).ResumeLayout(false);
    ((Control) this.tableLayoutPanelButtons).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
