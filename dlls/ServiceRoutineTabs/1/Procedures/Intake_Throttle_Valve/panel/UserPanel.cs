// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Intake_Throttle_Valve.panel.UserPanel
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
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Intake_Throttle_Valve.panel;

public class UserPanel : CustomPanel
{
  private const string ITVStartRoutine = "RT_SR068_Control_IAT_Start_status";
  private const string ITVStopRoutine = "RT_SR068_Control_IAT_Stop";
  private const string EngineSpeedInstrumentName = "DT_AS010_Engine_Speed";
  private const int MinCommandedValue = 0;
  private const int MaxCommandedValue = 100;
  private const int PositionStep = 10;
  private static readonly Regex ValidCommandedValueCharacters = new Regex("[0123456789]", RegexOptions.Compiled);
  private string ErrorServiceNotFound = Resources.Message_TheMCMConnectedIsNotSupported0;
  private Channel mcm = (Channel) null;
  private Instrument engineSpeedInstrument = (Instrument) null;
  private bool manipulating = false;
  private DigitalReadoutInstrument vehicleCheckInstrument;
  private DigitalReadoutInstrument itvActualInstrument;
  private ScalingLabel titleLabel;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label positionHeader;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanel2;
  private TextBox textboxResults;
  private Button buttonStart;
  private Button buttonStop;
  private Button buttonCommandedValueOpen;
  private Button buttonDecrement;
  private TextBox textboxCommandedValue;
  private Button buttonIncrement;
  private Button buttonCommandedValueClose;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelCylindersHeader;

  public UserPanel()
  {
    this.InitializeComponent();
    this.buttonStart.Click += new EventHandler(this.OnStartClick);
    this.buttonStop.Click += new EventHandler(this.OnStopClick);
    this.textboxCommandedValue.KeyPress += new KeyPressEventHandler(this.OnCommandedValueKeyPress);
    this.textboxCommandedValue.TextChanged += new EventHandler(this.OnCommandedValueChanged);
    this.buttonIncrement.Click += new EventHandler(this.OnIncrementClick);
    this.buttonDecrement.Click += new EventHandler(this.OnDecrementClick);
    this.buttonCommandedValueClose.Click += new EventHandler(this.OnCommandedValueCloseClick);
    this.buttonCommandedValueOpen.Click += new EventHandler(this.OnCommandedValueOpenClick);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM"));
    this.UpdateUserInterface();
  }

  private bool SetMCM(Channel mcm)
  {
    bool flag = false;
    if (this.mcm != mcm)
    {
      flag = true;
      if (this.mcm != null)
      {
        this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        if (this.engineSpeedInstrument != (Instrument) null)
        {
          this.engineSpeedInstrument.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
          this.engineSpeedInstrument = (Instrument) null;
        }
        this.manipulating = false;
      }
      if (this.mcm == null && mcm != null)
        this.ClearResults();
      this.mcm = mcm;
      if (this.mcm != null)
      {
        this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.engineSpeedInstrument = this.GetInstrument("MCM", "DT_AS010_Engine_Speed");
        if (this.engineSpeedInstrument != (Instrument) null)
          this.engineSpeedInstrument.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
      }
    }
    return flag;
  }

  private void UpdateUserInterface()
  {
    this.buttonStart.Enabled = this.CanStart;
    this.buttonStop.Enabled = this.CanStop;
    this.UpdateCommandedValueUI();
  }

  private void UpdateCommandedValueUI()
  {
    bool canManipulate = this.CanManipulate;
    this.buttonIncrement.Enabled = this.CanIncrement;
    this.buttonDecrement.Enabled = this.CanDecrement;
    this.buttonCommandedValueClose.Enabled = canManipulate && this.CommandedValue != 100;
    this.buttonCommandedValueOpen.Enabled = canManipulate && this.CommandedValue != 0;
    this.textboxCommandedValue.Enabled = canManipulate;
  }

  private void CommandValue()
  {
    if (!this.CanManipulate || !this.ValidCommandedValue)
      return;
    Service service = this.GetService("MCM", "RT_SR068_Control_IAT_Start_status");
    if (service != (Service) null)
    {
      service.InputValues[0].Value = (object) this.CommandedValue;
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      this.ReportResult($"{Resources.Message_ManipulatingIntakeThrottleValveTo}{(object) this.CommandedValue}% : ", false);
      service.Execute(false);
    }
    else
      this.ReportResult(this.ErrorServiceNotFound, true);
  }

  private void StopManipulation()
  {
    if (!this.CanStop)
      return;
    Service service = this.GetService("MCM", "RT_SR068_Control_IAT_Stop");
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      this.ReportResult(Resources.Message_StoppingIntakeThrottleValveManipulation, false);
      service.Execute(false);
      this.manipulating = false;
      this.UpdateUserInterface();
    }
    else
      this.ReportResult(this.ErrorServiceNotFound, true);
  }

  private void ClearResults() => this.textboxResults.Text = string.Empty;

  private void ReportResult(string text, bool withNewLine)
  {
    this.textboxResults.Text = this.textboxResults.Text + text + (withNewLine ? "\r\n" : string.Empty);
    this.textboxResults.SelectionStart = this.textboxResults.TextLength;
    this.textboxResults.SelectionLength = 0;
    this.textboxResults.ScrollToCaret();
    this.AddStatusMessage(text);
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnEngineSpeedUpdate(object sender, ResultEventArgs e)
  {
    if (this.EngineSpeed != 0)
    {
      if (!this.manipulating)
        return;
      this.ReportResult(Resources.Message_EngineStartedWhileIntakeThrottleValveManipulationInProgressStoppingNow, true);
      this.StopManipulation();
    }
    else
      this.UpdateUserInterface();
  }

  private void OnServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    string empty = string.Empty;
    this.ReportResult(!e.Succeeded ? string.Format(Resources.MessageFormat_Error0, (object) e.Exception.Message) : (e.Exception == null ? Resources.Message_Done : string.Format(Resources.MessageFormat_Done0, (object) e.Exception.Message)), true);
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
  }

  private void OnDecrementClick(object sender, EventArgs e) => this.CommandedValue -= 10;

  private void OnIncrementClick(object sender, EventArgs e) => this.CommandedValue += 10;

  private void OnCommandedValueKeyPress(object sender, KeyPressEventArgs e)
  {
    if (!UserPanel.ValidCommandedValueCharacters.IsMatch(e.KeyChar.ToString()) && e.KeyChar != '\b')
      e.Handled = true;
    if (e.KeyChar != '\r' || !this.ValidCommandedValue)
      return;
    this.CommandValue();
  }

  private void OnCommandedValueChanged(object sender, EventArgs e) => this.UpdateCommandedValueUI();

  private void OnStartClick(object sender, EventArgs e)
  {
    this.manipulating = true;
    this.UpdateUserInterface();
    this.CommandValue();
  }

  private void OnStopClick(object sender, EventArgs e) => this.StopManipulation();

  private void OnCommandedValueOpenClick(object sender, EventArgs e) => this.CommandedValue = 0;

  private void OnCommandedValueCloseClick(object sender, EventArgs e) => this.CommandedValue = 100;

  private bool Online
  {
    get => this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online;
  }

  private bool ValidCommandedValue
  {
    get
    {
      bool validCommandedValue = false;
      int commandedValue = this.CommandedValue;
      if (commandedValue >= 0 && commandedValue <= 100)
        validCommandedValue = true;
      return validCommandedValue;
    }
  }

  private int CommandedValue
  {
    get
    {
      int result = -1;
      string text = this.textboxCommandedValue.Text;
      if (UserPanel.ValidCommandedValueCharacters.IsMatch(text))
        int.TryParse(text, out result);
      return result;
    }
    set
    {
      if (value < 0)
        value = 0;
      else if (value > 100)
        value = 100;
      if (value == this.CommandedValue)
        return;
      this.textboxCommandedValue.Text = value.ToString();
      this.UpdateCommandedValueUI();
      this.CommandValue();
    }
  }

  private bool CanDecrement
  {
    get
    {
      int commandedValue = this.CommandedValue;
      return this.CanManipulate && (commandedValue > 0 || commandedValue == -1);
    }
  }

  private bool CanIncrement
  {
    get
    {
      int commandedValue = this.CommandedValue;
      return this.CanManipulate && (commandedValue == -1 || commandedValue < 100);
    }
  }

  private bool CanStart
  {
    get => this.Online && this.ValidCommandedValue && this.ValidEngineSpeed && !this.manipulating;
  }

  private bool ValidEngineSpeed => this.Online && this.EngineSpeed == 0;

  private int EngineSpeed
  {
    get
    {
      int result;
      return this.engineSpeedInstrument != (Instrument) null && this.engineSpeedInstrument.InstrumentValues.Current != null && int.TryParse(this.engineSpeedInstrument.InstrumentValues.Current.Value.ToString(), out result) ? result : -1;
    }
  }

  private bool CanStop => this.Online && this.manipulating;

  private bool CanManipulate => this.Online && this.manipulating;

  private bool ServicesExist
  {
    get
    {
      return this.mcm != null && this.mcm.Services["RT_SR068_Control_IAT_Start_status"] != (Service) null && this.mcm.Services["RT_SR068_Control_IAT_Stop"] != (Service) null;
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.positionHeader = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelCylindersHeader = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonStart = new Button();
    this.buttonStop = new Button();
    this.buttonCommandedValueOpen = new Button();
    this.buttonDecrement = new Button();
    this.textboxCommandedValue = new TextBox();
    this.buttonIncrement = new Button();
    this.buttonCommandedValueClose = new Button();
    this.textboxResults = new TextBox();
    this.titleLabel = new ScalingLabel();
    this.itvActualInstrument = new DigitalReadoutInstrument();
    this.vehicleCheckInstrument = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.positionHeader, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelCylindersHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonStart, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonStop, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonCommandedValueOpen, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonDecrement, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textboxCommandedValue, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonIncrement, 5, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonCommandedValueClose, 6, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textboxResults, 0, 2);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    this.positionHeader.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.positionHeader, 6);
    componentResourceManager.ApplyResources((object) this.positionHeader, "positionHeader");
    ((Control) this.positionHeader).Name = "positionHeader";
    this.positionHeader.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelCylindersHeader.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelCylindersHeader, "labelCylindersHeader");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.labelCylindersHeader, 2);
    ((Control) this.labelCylindersHeader).Name = "labelCylindersHeader";
    this.labelCylindersHeader.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStop, "buttonStop");
    this.buttonStop.Name = "buttonStop";
    this.buttonStop.UseCompatibleTextRendering = true;
    this.buttonStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonCommandedValueOpen, "buttonCommandedValueOpen");
    this.buttonCommandedValueOpen.Name = "buttonCommandedValueOpen";
    this.buttonCommandedValueOpen.UseCompatibleTextRendering = true;
    this.buttonCommandedValueOpen.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonDecrement, "buttonDecrement");
    this.buttonDecrement.Name = "buttonDecrement";
    this.buttonDecrement.UseCompatibleTextRendering = true;
    this.buttonDecrement.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.textboxCommandedValue, "textboxCommandedValue");
    this.textboxCommandedValue.Name = "textboxCommandedValue";
    this.textboxCommandedValue.ShortcutsEnabled = false;
    componentResourceManager.ApplyResources((object) this.buttonIncrement, "buttonIncrement");
    this.buttonIncrement.Name = "buttonIncrement";
    this.buttonIncrement.UseCompatibleTextRendering = true;
    this.buttonIncrement.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonCommandedValueClose, "buttonCommandedValueClose");
    this.buttonCommandedValueClose.Name = "buttonCommandedValueClose";
    this.buttonCommandedValueClose.UseCompatibleTextRendering = true;
    this.buttonCommandedValueClose.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.textboxResults, 8);
    componentResourceManager.ApplyResources((object) this.textboxResults, "textboxResults");
    this.textboxResults.Name = "textboxResults";
    this.textboxResults.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.titleLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.itvActualInstrument, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.vehicleCheckInstrument, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.titleLabel.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.titleLabel, 2);
    componentResourceManager.ApplyResources((object) this.titleLabel, "titleLabel");
    this.titleLabel.FontGroup = (string) null;
    this.titleLabel.LineAlignment = StringAlignment.Center;
    ((Control) this.titleLabel).Name = "titleLabel";
    this.titleLabel.ShowBorder = false;
    componentResourceManager.ApplyResources((object) this.itvActualInstrument, "itvActualInstrument");
    this.itvActualInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.itvActualInstrument).FreezeValue = false;
    ((SingleInstrumentBase) this.itvActualInstrument).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS034_Throttle_Valve_Actual_Position");
    ((Control) this.itvActualInstrument).Name = "itvActualInstrument";
    ((SingleInstrumentBase) this.itvActualInstrument).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.vehicleCheckInstrument, "vehicleCheckInstrument");
    this.vehicleCheckInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.vehicleCheckInstrument).FreezeValue = false;
    ((SingleInstrumentBase) this.vehicleCheckInstrument).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS010_Engine_Speed");
    ((Control) this.vehicleCheckInstrument).Name = "vehicleCheckInstrument";
    ((SingleInstrumentBase) this.vehicleCheckInstrument).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_IntakeThrottleValve");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
