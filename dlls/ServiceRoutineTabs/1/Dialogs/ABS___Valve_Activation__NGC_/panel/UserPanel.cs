// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ABS___Valve_Activation__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ABS___Valve_Activation__NGC_.panel;

public class UserPanel : CustomPanel
{
  private const string ChannelName = "ABS02T";
  private Channel abs;
  private Timer timer;
  private TableLayoutPanel tableLayoutPanelMain;
  private TableLayoutPanel tableLayoutPanelInterlocks;
  private System.Windows.Forms.Label labelInterlockWarning;
  private System.Windows.Forms.Label labelOr;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private TableLayoutPanel tableLayoutPanelControls;
  private RunServicesButton runServicesButtonHbpvF;
  private RunServicesButton runServicesButtonHbpvE;
  private RunServicesButton runServicesButtonHbpvD;
  private RunServicesButton runServicesButtonHbpvC;
  private RunServicesButton runServicesButtonHoldTrailer;
  private RunServicesButton runServicesButtonHbpvB;
  private RunServicesButton runServicesButtonHbpvA;
  private RunServicesButton runServicesButtonAvsvA;
  private RunServicesButton runServicesButtonAvsvB;
  private TableLayoutPanel tableLayoutPanelRightColumn;
  private System.Windows.Forms.Label labelStatus;
  private System.Windows.Forms.Label labelMonitors;
  private System.Windows.Forms.Label labelControls;
  private System.Windows.Forms.Label labelInterlock;
  private TableLayoutPanel tableLayoutPanelHeading;
  private System.Windows.Forms.Label labelTitle;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private Button buttonClose;
  private SelectablePanel selectablePanel;

  public UserPanel()
  {
    this.InitializeComponent();
    this.timer = new Timer();
    this.timer.Interval = 2500;
    this.timer.Tick += new EventHandler(this.timer_Tick);
    this.UpdatePreconditionState();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.ParentForm_FormClosing);
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentIoInterlock_RepresentedStateChanged);
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentIoInterlock_RepresentedStateChanged);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.ParentForm_FormClosing);
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrumentIoInterlock_RepresentedStateChanged);
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged -= new EventHandler(this.digitalReadoutInstrumentIoInterlock_RepresentedStateChanged);
  }

  public virtual void OnChannelsChanged() => this.SetAbsChannel("ABS02T");

  private void SetAbsChannel(string ecuName)
  {
    if (this.abs != null)
      this.abs.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
    this.abs = this.GetChannel(ecuName);
    if (this.abs == null)
      return;
    this.abs.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
  }

  private void Services_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (e.Succeeded)
    {
      this.ResetError();
    }
    else
    {
      this.labelStatus.Text = e.Exception != null ? e.Exception.Message : Resources.ErrorUnknown;
      this.timer.Start();
    }
  }

  private void ResetError()
  {
    this.timer.Stop();
    this.labelStatus.Text = string.Empty;
  }

  private void timer_Tick(object sender, EventArgs e) => this.ResetError();

  private IEnumerable<Control> GetAllControls(Control source)
  {
    yield return source;
    foreach (Control child in source.Controls.OfType<Control>().SelectMany<Control, Control>((Func<Control, IEnumerable<Control>>) (c => this.GetAllControls(c))))
      yield return child;
  }

  private void UpdatePreconditionState()
  {
    bool flag1 = this.digitalReadoutInstrumentParkBrake.RepresentedState == 1 || this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1;
    bool flag2 = this.abs != null && this.abs.Online;
    foreach (Control control in this.GetAllControls((Control) this).OfType<RunServicesButton>())
      control.Enabled = flag1 && flag2;
    this.labelInterlockWarning.Visible = !flag1;
  }

  private void digitalReadoutInstrumentIoInterlock_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdatePreconditionState();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.labelStatus = new System.Windows.Forms.Label();
    this.tableLayoutPanelInterlocks = new TableLayoutPanel();
    this.labelInterlock = new System.Windows.Forms.Label();
    this.labelInterlockWarning = new System.Windows.Forms.Label();
    this.labelOr = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.tableLayoutPanelControls = new TableLayoutPanel();
    this.labelControls = new System.Windows.Forms.Label();
    this.runServicesButtonHbpvF = new RunServicesButton();
    this.runServicesButtonHbpvE = new RunServicesButton();
    this.runServicesButtonHbpvD = new RunServicesButton();
    this.runServicesButtonHbpvC = new RunServicesButton();
    this.runServicesButtonHoldTrailer = new RunServicesButton();
    this.runServicesButtonHbpvB = new RunServicesButton();
    this.runServicesButtonHbpvA = new RunServicesButton();
    this.runServicesButtonAvsvA = new RunServicesButton();
    this.runServicesButtonAvsvB = new RunServicesButton();
    this.tableLayoutPanelHeading = new TableLayoutPanel();
    this.labelTitle = new System.Windows.Forms.Label();
    this.tableLayoutPanelRightColumn = new TableLayoutPanel();
    this.labelMonitors = new System.Windows.Forms.Label();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.buttonClose = new Button();
    this.selectablePanel = new SelectablePanel();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelInterlocks).SuspendLayout();
    ((Control) this.tableLayoutPanelControls).SuspendLayout();
    ((ISupportInitialize) this.runServicesButtonHbpvF).BeginInit();
    ((ISupportInitialize) this.runServicesButtonHbpvE).BeginInit();
    ((ISupportInitialize) this.runServicesButtonHbpvD).BeginInit();
    ((ISupportInitialize) this.runServicesButtonHbpvC).BeginInit();
    ((ISupportInitialize) this.runServicesButtonHoldTrailer).BeginInit();
    ((ISupportInitialize) this.runServicesButtonHbpvB).BeginInit();
    ((ISupportInitialize) this.runServicesButtonHbpvA).BeginInit();
    ((ISupportInitialize) this.runServicesButtonAvsvA).BeginInit();
    ((ISupportInitialize) this.runServicesButtonAvsvB).BeginInit();
    ((Control) this.tableLayoutPanelHeading).SuspendLayout();
    ((Control) this.tableLayoutPanelRightColumn).SuspendLayout();
    ((Control) this.selectablePanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.labelStatus, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelInterlocks, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelControls, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelHeading, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelRightColumn, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClose, 2, 2);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.labelStatus, 2);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.ForeColor = Color.Red;
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInterlocks, "tableLayoutPanelInterlocks");
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.labelInterlock, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.labelInterlockWarning, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.labelOr, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 3);
    ((Control) this.tableLayoutPanelInterlocks).Name = "tableLayoutPanelInterlocks";
    componentResourceManager.ApplyResources((object) this.labelInterlock, "labelInterlock");
    this.labelInterlock.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanelInterlocks).SetColumnSpan((Control) this.labelInterlock, 2);
    this.labelInterlock.Name = "labelInterlock";
    this.labelInterlock.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelInterlockWarning, "labelInterlockWarning");
    this.labelInterlockWarning.ForeColor = Color.Red;
    this.labelInterlockWarning.Name = "labelInterlockWarning";
    this.labelInterlockWarning.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelOr, "labelOr");
    this.labelOr.Name = "labelOr";
    this.labelOr.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 0, 6);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(6, 5.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 1, 2, "mph");
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 5.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 6.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "J1939-0", "DT_84");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelControls, "tableLayoutPanelControls");
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.labelControls, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonHbpvF, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonHbpvE, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonHbpvD, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonHbpvC, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonHoldTrailer, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonHbpvB, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonHbpvA, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonAvsvA, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelControls).Controls.Add((Control) this.runServicesButtonAvsvB, 0, 2);
    ((Control) this.tableLayoutPanelControls).Name = "tableLayoutPanelControls";
    componentResourceManager.ApplyResources((object) this.labelControls, "labelControls");
    this.labelControls.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanelControls).SetColumnSpan((Control) this.labelControls, 2);
    this.labelControls.Name = "labelControls";
    this.labelControls.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.runServicesButtonHbpvF, "runServicesButtonHbpvF");
    ((Control) this.runServicesButtonHbpvF).Name = "runServicesButtonHbpvF";
    this.runServicesButtonHbpvF.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_F_StartRoutine_Start", (IEnumerable<string>) new string[1]
    {
      "Timing=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonHbpvE, "runServicesButtonHbpvE");
    ((Control) this.runServicesButtonHbpvE).Name = "runServicesButtonHbpvE";
    this.runServicesButtonHbpvE.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_E_StartRoutine_Start", (IEnumerable<string>) new string[1]
    {
      "Timing=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonHbpvD, "runServicesButtonHbpvD");
    ((Control) this.runServicesButtonHbpvD).Name = "runServicesButtonHbpvD";
    this.runServicesButtonHbpvD.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_D_StartRoutine_Start", (IEnumerable<string>) new string[1]
    {
      "Timing=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonHbpvC, "runServicesButtonHbpvC");
    ((Control) this.runServicesButtonHbpvC).Name = "runServicesButtonHbpvC";
    this.runServicesButtonHbpvC.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_C_StartRoutine_Start", (IEnumerable<string>) new string[1]
    {
      "Timing=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonHoldTrailer, "runServicesButtonHoldTrailer");
    ((Control) this.runServicesButtonHoldTrailer).Name = "runServicesButtonHoldTrailer";
    this.runServicesButtonHoldTrailer.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Trailer_Control_Pressure_StartRoutine_Start", (IEnumerable<string>) new string[1]
    {
      "Timing=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonHbpvB, "runServicesButtonHbpvB");
    ((Control) this.runServicesButtonHbpvB).Name = "runServicesButtonHbpvB";
    this.runServicesButtonHbpvB.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_B_StartRoutine_Start", (IEnumerable<string>) new string[1]
    {
      "Timing=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonHbpvA, "runServicesButtonHbpvA");
    ((Control) this.runServicesButtonHbpvA).Name = "runServicesButtonHbpvA";
    this.runServicesButtonHbpvA.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_Hold_Brake_Pressure_at_ABS_valve_A_StartRoutine_Start", (IEnumerable<string>) new string[1]
    {
      "Timing=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonAvsvA, "runServicesButtonAvsvA");
    ((Control) this.runServicesButtonAvsvA).Name = "runServicesButtonAvsvA";
    this.runServicesButtonAvsvA.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_3_2_Solenoid_valve_A_actuate_StartRoutine_Start", (IEnumerable<string>) new string[1]
    {
      "Timing=2000"
    }));
    componentResourceManager.ApplyResources((object) this.runServicesButtonAvsvB, "runServicesButtonAvsvB");
    ((Control) this.runServicesButtonAvsvB).Name = "runServicesButtonAvsvB";
    this.runServicesButtonAvsvB.ServiceCalls.Add(new ServiceCall("ABS02T", "RT_3_2_Solenoid_valve_B_actuate_StartRoutine_Start", (IEnumerable<string>) new string[1]
    {
      "Timing=2000"
    }));
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelHeading, "tableLayoutPanelHeading");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelHeading, 3);
    ((TableLayoutPanel) this.tableLayoutPanelHeading).Controls.Add((Control) this.labelTitle, 0, 0);
    ((Control) this.tableLayoutPanelHeading).Name = "tableLayoutPanelHeading";
    componentResourceManager.ApplyResources((object) this.labelTitle, "labelTitle");
    this.labelTitle.Name = "labelTitle";
    this.labelTitle.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelRightColumn, "tableLayoutPanelRightColumn");
    ((TableLayoutPanel) this.tableLayoutPanelRightColumn).Controls.Add((Control) this.labelMonitors, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelRightColumn).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelRightColumn).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 2);
    ((Control) this.tableLayoutPanelRightColumn).Name = "tableLayoutPanelRightColumn";
    componentResourceManager.ApplyResources((object) this.labelMonitors, "labelMonitors");
    this.labelMonitors.BorderStyle = BorderStyle.FixedSingle;
    this.labelMonitors.Name = "labelMonitors";
    this.labelMonitors.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "IoMonitor";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_PressCrcut1_Stat_EAPU");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "IoMonitor";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_APC_Diagnostic_Displayables_DDAPC_PressCrcut2_Stat_EAPU");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.selectablePanel, "selectablePanel");
    ((Control) this.selectablePanel).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this.selectablePanel).Name = "selectablePanel";
    ((Panel) this.selectablePanel).TabStop = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Vehicle_ABSActivationValve");
    ((Control) this).Controls.Add((Control) this.selectablePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanelInterlocks).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInterlocks).PerformLayout();
    ((Control) this.tableLayoutPanelControls).ResumeLayout(false);
    ((Control) this.tableLayoutPanelControls).PerformLayout();
    ((ISupportInitialize) this.runServicesButtonHbpvF).EndInit();
    ((ISupportInitialize) this.runServicesButtonHbpvE).EndInit();
    ((ISupportInitialize) this.runServicesButtonHbpvD).EndInit();
    ((ISupportInitialize) this.runServicesButtonHbpvC).EndInit();
    ((ISupportInitialize) this.runServicesButtonHoldTrailer).EndInit();
    ((ISupportInitialize) this.runServicesButtonHbpvB).EndInit();
    ((ISupportInitialize) this.runServicesButtonHbpvA).EndInit();
    ((ISupportInitialize) this.runServicesButtonAvsvA).EndInit();
    ((ISupportInitialize) this.runServicesButtonAvsvB).EndInit();
    ((Control) this.tableLayoutPanelHeading).ResumeLayout(false);
    ((Control) this.tableLayoutPanelHeading).PerformLayout();
    ((Control) this.tableLayoutPanelRightColumn).ResumeLayout(false);
    ((Control) this.tableLayoutPanelRightColumn).PerformLayout();
    ((Control) this.selectablePanel).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
