// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Air_Pressure_System_Check__EPA10_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Air_Pressure_System_Check__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private const string EngineSpeedInstrumentQualifier = "DT_AS001_Engine_Speed";
  private Channel acm = (Channel) null;
  private Instrument engineSpeed = (Instrument) null;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private BarInstrument barInstrument1;
  private TableLayoutPanel tableLayoutPanel2;
  private System.Windows.Forms.Label labelDuration;
  private System.Windows.Forms.Label labelDurationUnits;
  private DecimalTextBox durationTextBox;
  private Button buttonClose;
  private TableLayoutPanel tableLayoutPanel3;
  private RunServiceButton runServiceButtonStart;
  private RunServiceButton runServiceButtonStop;
  private TableLayoutPanel tableLayoutPanel4;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelEngineStatus;
  private Checkmark engineSpeedCheck;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private System.Windows.Forms.Label label1;
  private BarInstrument barInstrument2;

  public UserPanel()
  {
    this.InitializeComponent();
    this.durationTextBox.ValueChanged += new EventHandler(this.OnDurationChanged);
    ((RunSharedProcedureButtonBase) this.runServiceButtonStart).ButtonEnabledChanged += new EventHandler(this.OnStartButtonEnabledChanged);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.UpdateUserInterface();
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    this.SetACM((Channel) null);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetACM(this.GetChannel("ACM02T"));
    this.UpdateUserInterface();
  }

  private void SetACM(Channel acm)
  {
    if (this.acm != acm && this.acm != null)
      this.acm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    if (this.engineSpeed != (Instrument) null)
    {
      this.engineSpeed.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
      this.engineSpeed = (Instrument) null;
    }
    this.acm = acm;
    if (this.acm == null)
      return;
    this.acm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    this.engineSpeed = this.acm.Instruments["DT_AS001_Engine_Speed"];
    if (this.engineSpeed != (Instrument) null)
      this.engineSpeed.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnEngineSpeedUpdate(object sender, ResultEventArgs e) => this.UpdateUserInterface();

  private void OnStartButtonEnabledChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private bool CanEditDuration
  {
    get
    {
      int num;
      if (!((RunSharedProcedureButtonBase) this.runServiceButtonStart).IsBusy)
      {
        ServiceCall serviceCall = this.runServiceButtonStart.ServiceCall;
        if (((ServiceCall) ref serviceCall).IsServiceAvailable)
        {
          num = this.engineSpeedCheck.Checked ? 1 : 0;
          goto label_4;
        }
      }
      num = 0;
label_4:
      return num != 0;
    }
  }

  private void UpdateUserInterface()
  {
    this.UpdateEngineStatus();
    if (!this.engineSpeedCheck.Checked)
      ((Control) this.runServiceButtonStop).Enabled = false;
    ((Control) this.runServiceButtonStart).Enabled = this.ValidateAndSetDuration() && this.engineSpeedCheck.Checked;
    ((Control) this.durationTextBox).Enabled = this.CanEditDuration;
  }

  private void UpdateEngineStatus()
  {
    bool flag = false;
    string str = Resources.Message_EngineSpeedCannotBeDetectedCheckCannotStart;
    if (this.engineSpeed != (Instrument) null && this.engineSpeed.InstrumentValues.Current != null)
    {
      double d = Convert.ToDouble(this.engineSpeed.InstrumentValues.Current.Value);
      if (!double.IsNaN(d))
      {
        if (d == 0.0)
        {
          str = Resources.Message_EngineIsNotRunningCheckCanStart;
          flag = true;
        }
        else
          str = Resources.Message_EngineIsRunningCheckCannotStart0;
      }
    }
    ((Control) this.labelEngineStatus).Text = str;
    this.engineSpeedCheck.Checked = flag;
  }

  private bool ValidateAndSetDuration()
  {
    if (!((RunSharedProcedureButtonBase) this.runServiceButtonStart).IsBusy)
    {
      double d = this.durationTextBox.Value;
      if (!double.IsInfinity(d) && !double.IsNaN(d))
      {
        ServiceCall serviceCall1 = this.runServiceButtonStart.ServiceCall;
        if (!((ServiceCall) ref serviceCall1).IsEmpty)
        {
          RunServiceButton serviceButtonStart = this.runServiceButtonStart;
          ServiceCall serviceCall2 = this.runServiceButtonStart.ServiceCall;
          string ecu = ((ServiceCall) ref serviceCall2).Ecu;
          serviceCall2 = this.runServiceButtonStart.ServiceCall;
          string qualifier = ((ServiceCall) ref serviceCall2).Qualifier;
          string[] strArray = new string[1]{ d.ToString() };
          ServiceCall serviceCall3 = new ServiceCall(ecu, qualifier, (IEnumerable<string>) strArray);
          serviceButtonStart.ServiceCall = serviceCall3;
          return true;
        }
      }
    }
    return false;
  }

  private void OnDurationChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.barInstrument1 = new BarInstrument();
    this.barInstrument2 = new BarInstrument();
    this.buttonClose = new Button();
    this.labelDuration = new System.Windows.Forms.Label();
    this.labelDurationUnits = new System.Windows.Forms.Label();
    this.durationTextBox = new DecimalTextBox();
    this.runServiceButtonStart = new RunServiceButton();
    this.runServiceButtonStop = new RunServiceButton();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.label1 = new System.Windows.Forms.Label();
    this.labelEngineStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.engineSpeedCheck = new Checkmark();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.tableLayoutPanel1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelEngineStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.engineSpeedCheck, 0, 0);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel4).SetColumnSpan((Control) this.tableLayoutPanel1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument2, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 3);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS122_Pressure_Limiting_Unit");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS003_Enable_DEF_pump");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument1, 3);
    componentResourceManager.ApplyResources((object) this.barInstrument1, "barInstrument1");
    this.barInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS016_DEF_Air_Pressure");
    ((Control) this.barInstrument1).Name = "barInstrument1";
    ((SingleInstrumentBase) this.barInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument2, 3);
    componentResourceManager.ApplyResources((object) this.barInstrument2, "barInstrument2");
    this.barInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS014_DEF_Pressure");
    ((Control) this.barInstrument2).Name = "barInstrument2";
    ((SingleInstrumentBase) this.barInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClose, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.tableLayoutPanel3, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.runServiceButtonStart, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.runServiceButtonStop, 2, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.labelDuration, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.labelDurationUnits, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.durationTextBox, 1, 1);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.labelDuration, "labelDuration");
    this.labelDuration.Name = "labelDuration";
    this.labelDuration.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelDurationUnits, "labelDurationUnits");
    this.labelDurationUnits.Name = "labelDurationUnits";
    this.labelDurationUnits.UseCompatibleTextRendering = true;
    ((Control) this.durationTextBox).Cursor = Cursors.IBeam;
    componentResourceManager.ApplyResources((object) this.durationTextBox, "durationTextBox");
    this.durationTextBox.MaximumValue = 60.0;
    this.durationTextBox.MinimumValue = 0.0;
    ((Control) this.durationTextBox).Name = "durationTextBox";
    this.durationTextBox.Precision = new int?(0);
    this.durationTextBox.Value = 60.0;
    componentResourceManager.ApplyResources((object) this.runServiceButtonStart, "runServiceButtonStart");
    ((Control) this.runServiceButtonStart).Name = "runServiceButtonStart";
    this.runServiceButtonStart.ServiceCall = new ServiceCall("ACM02T", "RT_SCR_Pressure_System_Check_Start", (IEnumerable<string>) new string[1]
    {
      "10"
    });
    componentResourceManager.ApplyResources((object) this.runServiceButtonStop, "runServiceButtonStop");
    ((Control) this.runServiceButtonStop).Name = "runServiceButtonStop";
    this.runServiceButtonStop.ServiceCall = new ServiceCall("ACM02T", "RT_SCR_Pressure_System_Check_Stop");
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_DS001_Enable_compressed_air_pressure");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label1, 3);
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    this.labelEngineStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelEngineStatus, "labelEngineStatus");
    ((Control) this.labelEngineStatus).Name = "labelEngineStatus";
    this.labelEngineStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelEngineStatus.ShowBorder = false;
    this.labelEngineStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.engineSpeedCheck, "engineSpeedCheck");
    ((Control) this.engineSpeedCheck).Name = "engineSpeedCheck";
    this.engineSpeedCheck.SizeMode = PictureBoxSizeMode.AutoSize;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_SCRAirPressureSystemCheck");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel4);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
