// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual_.panel;

public class UserPanel : CustomPanel
{
  private const string CylinderCutStartStopRoutine = "RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder";
  private const string AllCylindersOnRoutine = "RT_SR004_Engine_Cylinder_Cut_Off_Stop";
  private const string IdleSpeedSetRoutine = "RT_SR015_Idle_Speed_Modification_Start";
  private const string IdleSpeedResetRoutine = "RT_SR015_Idle_Speed_Modification_Stop";
  private const int CylinderCount = 6;
  private const int MinIdleSpeed = 600;
  private const int MaxIdleSpeed = 1000;
  private const int IdleSpeedStep = 100;
  private Button[] onButtons;
  private Button[] offButtons;
  private static readonly Regex ValidIdleSpeedCharacters = new Regex("[0123456789]", RegexOptions.Compiled);
  private Channel mcm = (Channel) null;
  private ChartInstrument chartInstrument;
  private DigitalReadoutInstrument DigitalReadoutInstrument3;
  private DigitalReadoutInstrument DigitalReadoutInstrument2;
  private DigitalReadoutInstrument DigitalReadoutInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrument4;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanel2;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label2;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label1;
  private Button allOnButton;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label8;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label3;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label4;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label5;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label6;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label7;
  private Button cylinder1On;
  private Button cylinder2On;
  private Button cylinder3On;
  private Button cylinder4On;
  private Button cylinder5On;
  private Button cylinder6On;
  private Button cylinder1Off;
  private Button cylinder2Off;
  private Button cylinder3Off;
  private Button cylinder4Off;
  private Button cylinder5Off;
  private Button cylinder6Off;
  private Button decButton;
  private Button incButton;
  private TextBox idleSpeedText;
  private TextBox textBoxResults;
  private Button buttonIdleSpeedApply;
  private Button buttonIdleSpeedReset;

  public UserPanel()
  {
    WarningManager.SetJobName(Resources.Message_ThisChange);
    this.InitializeComponent();
    this.onButtons = new Button[6]
    {
      this.cylinder1On,
      this.cylinder2On,
      this.cylinder3On,
      this.cylinder4On,
      this.cylinder5On,
      this.cylinder6On
    };
    this.offButtons = new Button[6]
    {
      this.cylinder1Off,
      this.cylinder2Off,
      this.cylinder3Off,
      this.cylinder4Off,
      this.cylinder5Off,
      this.cylinder6Off
    };
    this.cylinder1On.Click += new EventHandler(this.OnCylinderOnClick);
    this.cylinder2On.Click += new EventHandler(this.OnCylinderOnClick);
    this.cylinder3On.Click += new EventHandler(this.OnCylinderOnClick);
    this.cylinder4On.Click += new EventHandler(this.OnCylinderOnClick);
    this.cylinder5On.Click += new EventHandler(this.OnCylinderOnClick);
    this.cylinder6On.Click += new EventHandler(this.OnCylinderOnClick);
    this.cylinder1Off.Click += new EventHandler(this.OnCylinderOffClick);
    this.cylinder2Off.Click += new EventHandler(this.OnCylinderOffClick);
    this.cylinder3Off.Click += new EventHandler(this.OnCylinderOffClick);
    this.cylinder4Off.Click += new EventHandler(this.OnCylinderOffClick);
    this.cylinder5Off.Click += new EventHandler(this.OnCylinderOffClick);
    this.cylinder6Off.Click += new EventHandler(this.OnCylinderOffClick);
    this.allOnButton.Click += new EventHandler(this.OnAllCylindersOnClick);
    this.idleSpeedText.KeyPress += new KeyPressEventHandler(this.OnIdleSpeedKeyPress);
    this.idleSpeedText.TextChanged += new EventHandler(this.OnIdleSpeedChanged);
    this.incButton.Click += new EventHandler(this.OnIncrementClick);
    this.decButton.Click += new EventHandler(this.OnDecrementClick);
    this.buttonIdleSpeedApply.Click += new EventHandler(this.OnApplyClick);
    this.buttonIdleSpeedReset.Click += new EventHandler(this.OnResetClick);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM"));
    this.UpdateUserInterface();
  }

  private void SetMCM(Channel mcm)
  {
    if (this.mcm == mcm)
      return;
    WarningManager.Reset();
    if (this.mcm != null)
      this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    if (this.mcm == null && mcm != null)
      this.ClearResults();
    this.mcm = mcm;
    if (this.mcm != null)
      this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
  }

  private void UpdateUserInterface()
  {
    bool turnCylinderOnOrOff = this.CanTurnCylinderOnOrOff;
    foreach (Control onButton in this.onButtons)
      onButton.Enabled = turnCylinderOnOrOff;
    foreach (Control offButton in this.offButtons)
      offButton.Enabled = turnCylinderOnOrOff;
    this.allOnButton.Enabled = this.CanTurnAllCylindersOn;
    this.UpdateIdleSpeedUI();
  }

  private void UpdateIdleSpeedUI()
  {
    this.idleSpeedText.Enabled = this.Online;
    this.incButton.Enabled = this.CanIncrement;
    this.decButton.Enabled = this.CanDecrement;
    this.buttonIdleSpeedApply.Enabled = this.CanApply;
    this.buttonIdleSpeedReset.Enabled = this.CanReset;
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnDecrementClick(object sender, EventArgs e) => this.IdleSpeed -= 100;

  private void OnIncrementClick(object sender, EventArgs e) => this.IdleSpeed += 100;

  private void OnIdleSpeedKeyPress(object sender, KeyPressEventArgs e)
  {
    if (UserPanel.ValidIdleSpeedCharacters.IsMatch(e.KeyChar.ToString()) || e.KeyChar == '\b')
      return;
    e.Handled = true;
  }

  private void OnIdleSpeedChanged(object sender, EventArgs e) => this.UpdateIdleSpeedUI();

  private void OnCylinderOnClick(object sender, EventArgs e)
  {
    this.SwitchCylinder(Convert.ToInt32((sender as Button).Tag), true);
  }

  private void OnCylinderOffClick(object sender, EventArgs e)
  {
    this.SwitchCylinder(Convert.ToInt32((sender as Button).Tag), false);
  }

  private void OnAllCylindersOnClick(object sender, EventArgs e) => this.SwitchAllCylindersOn();

  private void OnApplyClick(object sender, EventArgs e) => this.SetIdleSpeed();

  private void OnResetClick(object sender, EventArgs e) => this.ResetIdleSpeed();

  private void SwitchCylinder(int number, bool on)
  {
    if (!this.CanTurnCylinderOnOrOff || !WarningManager.RequestContinue())
      return;
    Service service = this.mcm.Services["RT_SR004_Engine_Cylinder_Cut_Off_Start_Cylinder"];
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) number);
      service.InputValues[1].Value = (object) service.InputValues[1].Choices.GetItemFromRawValue((object) (on ? 1 : 0));
      if (on)
        this.ReportResult(string.Format(Resources.MessageFormat_EnablingCylinder0, (object) number), false);
      else
        this.ReportResult(string.Format(Resources.MessageFormat_CuttingCylinder0, (object) number), false);
      service.Execute(false);
    }
    else
      this.ReportResult(Resources.Message_ErrorCouldNotFindService, true);
  }

  private void SwitchAllCylindersOn()
  {
    if (!this.CanTurnAllCylindersOn || !WarningManager.RequestContinue())
      return;
    Service service = this.mcm.Services["RT_SR004_Engine_Cylinder_Cut_Off_Stop"];
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      this.ReportResult(Resources.Message_EnablingAllCylinders, false);
      service.Execute(false);
    }
    else
      this.ReportResult(Resources.Message_ErrorCouldNotFindService, true);
  }

  private void SetIdleSpeed()
  {
    if (!this.CanApply || !WarningManager.RequestContinue())
      return;
    Service service = this.mcm.Services["RT_SR015_Idle_Speed_Modification_Start"];
    if (service != (Service) null)
    {
      int idleSpeed = this.IdleSpeed;
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      service.InputValues[0].Value = (object) (double) idleSpeed;
      this.ReportResult(string.Format(Resources.MessageFormat_SettingIdleSpeedTo0Rpm, (object) idleSpeed), false);
      service.Execute(false);
    }
    else
      this.ReportResult(Resources.Message_ErrorCouldNotFindService, true);
  }

  private void ResetIdleSpeed()
  {
    if (!this.CanReset || !WarningManager.RequestContinue())
      return;
    Service service = this.mcm.Services["RT_SR015_Idle_Speed_Modification_Stop"];
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
      this.ReportResult(Resources.Message_ResettingIdleSpeed, false);
      service.Execute(false);
    }
    else
      this.ReportResult(Resources.Message_ErrorCouldNotFindService, true);
  }

  private void OnServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    string empty = string.Empty;
    this.ReportResult(!e.Succeeded ? string.Format(Resources.MessageFormat_Error0, (object) e.Exception.Message) : (e.Exception == null ? Resources.Message_Done : string.Format(Resources.MessageFormat_Done0, (object) e.Exception.Message)), true);
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceCompleteEvent);
  }

  private void ClearResults()
  {
    if (this.textBoxResults == null)
      return;
    this.textBoxResults.Text = string.Empty;
  }

  private void ReportResult(string text, bool withNewLine)
  {
    this.textBoxResults.Text = this.textBoxResults.Text + text + (withNewLine ? "\r\n" : string.Empty);
    this.textBoxResults.SelectionStart = this.textBoxResults.TextLength;
    this.textBoxResults.SelectionLength = 0;
    this.textBoxResults.ScrollToCaret();
    this.AddStatusMessage(text);
  }

  private bool Online
  {
    get => this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online;
  }

  private bool ValidIdleSpeed
  {
    get
    {
      bool validIdleSpeed = false;
      int idleSpeed = this.IdleSpeed;
      if (idleSpeed >= 600 && idleSpeed <= 1000)
        validIdleSpeed = true;
      return validIdleSpeed;
    }
  }

  private int IdleSpeed
  {
    get
    {
      int result = -1;
      string text = this.idleSpeedText.Text;
      if (UserPanel.ValidIdleSpeedCharacters.IsMatch(text))
        int.TryParse(text, out result);
      return result;
    }
    set
    {
      if (value < 600)
        value = 600;
      else if (value > 1000)
        value = 1000;
      if (value == this.IdleSpeed)
        return;
      this.idleSpeedText.Text = value.ToString();
      this.UpdateIdleSpeedUI();
    }
  }

  private bool CanDecrement
  {
    get
    {
      int idleSpeed = this.IdleSpeed;
      return this.Online && (idleSpeed > 600 || idleSpeed == -1);
    }
  }

  private bool CanIncrement
  {
    get
    {
      int idleSpeed = this.IdleSpeed;
      return this.Online && (idleSpeed == -1 || idleSpeed < 1000);
    }
  }

  private bool CanApply => this.Online && this.ValidIdleSpeed;

  private bool CanReset => this.Online;

  private bool CanTurnCylinderOnOrOff => this.Online;

  private bool CanTurnAllCylindersOn => this.Online;

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.chartInstrument = new ChartInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.label8 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label2 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label1 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.allOnButton = new Button();
    this.label3 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label4 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label5 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label6 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label7 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.cylinder1On = new Button();
    this.cylinder2On = new Button();
    this.cylinder3On = new Button();
    this.cylinder4On = new Button();
    this.cylinder5On = new Button();
    this.cylinder6On = new Button();
    this.cylinder1Off = new Button();
    this.cylinder2Off = new Button();
    this.cylinder3Off = new Button();
    this.cylinder4Off = new Button();
    this.cylinder5Off = new Button();
    this.cylinder6Off = new Button();
    this.decButton = new Button();
    this.incButton = new Button();
    this.idleSpeedText = new TextBox();
    this.textBoxResults = new TextBox();
    this.buttonIdleSpeedApply = new Button();
    this.buttonIdleSpeedReset = new Button();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument4, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument1, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument2, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrument3, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.chartInstrument, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
    this.DigitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS013_Coolant_Temperature");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
    this.DigitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS010_Engine_Speed");
    ((Control) this.DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument3, "DigitalReadoutInstrument3");
    this.DigitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS003_Actual_Torque");
    ((Control) this.DigitalReadoutInstrument3).Name = "DigitalReadoutInstrument3";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.chartInstrument, 4);
    componentResourceManager.ApplyResources((object) this.chartInstrument, "chartInstrument");
    ((Collection<Qualifier>) this.chartInstrument.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM", "DT_AS003_Actual_Torque"));
    ((Control) this.chartInstrument).Name = "chartInstrument";
    this.chartInstrument.SelectedTime = new DateTime?();
    this.chartInstrument.ShowEvents = false;
    this.chartInstrument.ShowLegend = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label8, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label2, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.allOnButton, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label3, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label4, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label5, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label6, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label7, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder1On, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder2On, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder3On, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder4On, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder5On, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder6On, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder1Off, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder2Off, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder3Off, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder4Off, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder5Off, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinder6Off, 2, 7);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.decButton, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.incButton, 5, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.idleSpeedText, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textBoxResults, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonIdleSpeedApply, 6, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonIdleSpeedReset, 7, 1);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    this.label8.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label8, "label8");
    ((Control) this.label8).Name = "label8";
    this.label8.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label2.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.label2, 6);
    ((Control) this.label2).Name = "label2";
    this.label2.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label1.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.label1, 3);
    ((Control) this.label1).Name = "label1";
    this.label1.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.allOnButton, 3);
    componentResourceManager.ApplyResources((object) this.allOnButton, "allOnButton");
    this.allOnButton.Name = "allOnButton";
    this.allOnButton.UseCompatibleTextRendering = true;
    this.allOnButton.UseVisualStyleBackColor = true;
    this.label3.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    ((Control) this.label3).Name = "label3";
    this.label3.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label4.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label4, "label4");
    ((Control) this.label4).Name = "label4";
    this.label4.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label5.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    ((Control) this.label5).Name = "label5";
    this.label5.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label6.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label6, "label6");
    ((Control) this.label6).Name = "label6";
    this.label6.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label7.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label7, "label7");
    ((Control) this.label7).Name = "label7";
    this.label7.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    componentResourceManager.ApplyResources((object) this.cylinder1On, "cylinder1On");
    this.cylinder1On.Name = "cylinder1On";
    this.cylinder1On.Tag = (object) "1";
    this.cylinder1On.UseCompatibleTextRendering = true;
    this.cylinder1On.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder2On, "cylinder2On");
    this.cylinder2On.Name = "cylinder2On";
    this.cylinder2On.Tag = (object) "2";
    this.cylinder2On.UseCompatibleTextRendering = true;
    this.cylinder2On.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder3On, "cylinder3On");
    this.cylinder3On.Name = "cylinder3On";
    this.cylinder3On.Tag = (object) "3";
    this.cylinder3On.UseCompatibleTextRendering = true;
    this.cylinder3On.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder4On, "cylinder4On");
    this.cylinder4On.Name = "cylinder4On";
    this.cylinder4On.Tag = (object) "4";
    this.cylinder4On.UseCompatibleTextRendering = true;
    this.cylinder4On.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder5On, "cylinder5On");
    this.cylinder5On.Name = "cylinder5On";
    this.cylinder5On.Tag = (object) "5";
    this.cylinder5On.UseCompatibleTextRendering = true;
    this.cylinder5On.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder6On, "cylinder6On");
    this.cylinder6On.Name = "cylinder6On";
    this.cylinder6On.Tag = (object) "6";
    this.cylinder6On.UseCompatibleTextRendering = true;
    this.cylinder6On.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder1Off, "cylinder1Off");
    this.cylinder1Off.Name = "cylinder1Off";
    this.cylinder1Off.Tag = (object) "1";
    this.cylinder1Off.UseCompatibleTextRendering = true;
    this.cylinder1Off.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder2Off, "cylinder2Off");
    this.cylinder2Off.Name = "cylinder2Off";
    this.cylinder2Off.Tag = (object) "2";
    this.cylinder2Off.UseCompatibleTextRendering = true;
    this.cylinder2Off.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder3Off, "cylinder3Off");
    this.cylinder3Off.Name = "cylinder3Off";
    this.cylinder3Off.Tag = (object) "3";
    this.cylinder3Off.UseCompatibleTextRendering = true;
    this.cylinder3Off.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder4Off, "cylinder4Off");
    this.cylinder4Off.Name = "cylinder4Off";
    this.cylinder4Off.Tag = (object) "4";
    this.cylinder4Off.UseCompatibleTextRendering = true;
    this.cylinder4Off.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder5Off, "cylinder5Off");
    this.cylinder5Off.Name = "cylinder5Off";
    this.cylinder5Off.Tag = (object) "5";
    this.cylinder5Off.UseCompatibleTextRendering = true;
    this.cylinder5Off.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.cylinder6Off, "cylinder6Off");
    this.cylinder6Off.Name = "cylinder6Off";
    this.cylinder6Off.Tag = (object) "6";
    this.cylinder6Off.UseCompatibleTextRendering = true;
    this.cylinder6Off.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.decButton, "decButton");
    this.decButton.Name = "decButton";
    this.decButton.UseCompatibleTextRendering = true;
    this.decButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.incButton, "incButton");
    this.incButton.Name = "incButton";
    this.incButton.UseCompatibleTextRendering = true;
    this.incButton.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.idleSpeedText, "idleSpeedText");
    this.idleSpeedText.Name = "idleSpeedText";
    this.idleSpeedText.ShortcutsEnabled = false;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.textBoxResults, 6);
    componentResourceManager.ApplyResources((object) this.textBoxResults, "textBoxResults");
    this.textBoxResults.Name = "textBoxResults";
    this.textBoxResults.ReadOnly = true;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetRowSpan((Control) this.textBoxResults, 6);
    componentResourceManager.ApplyResources((object) this.buttonIdleSpeedApply, "buttonIdleSpeedApply");
    this.buttonIdleSpeedApply.Name = "buttonIdleSpeedApply";
    this.buttonIdleSpeedApply.UseCompatibleTextRendering = true;
    this.buttonIdleSpeedApply.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonIdleSpeedReset, "buttonIdleSpeedReset");
    this.buttonIdleSpeedReset.Name = "buttonIdleSpeedReset";
    this.buttonIdleSpeedReset.UseCompatibleTextRendering = true;
    this.buttonIdleSpeedReset.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_CylinderCutoutTestManual");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
