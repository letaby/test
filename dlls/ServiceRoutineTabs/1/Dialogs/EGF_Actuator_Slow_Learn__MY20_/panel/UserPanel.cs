// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGF_Actuator_Slow_Learn__MY20_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGF_Actuator_Slow_Learn__MY20_.panel;

public class UserPanel : CustomPanel
{
  private TableLayoutPanel tableLayoutPanel1;
  private Checkmark engineSpeedCheck;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;
  private Button buttonStart;
  private Button buttonClose;
  private Button buttonStopAll;
  private SharedProcedureSelection sharedProcedureSelection;
  private TableLayoutPanel tableLayoutPanel2;
  private TextBox textBoxResults;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private TableLayoutPanel tableLayoutPanel3;
  private Label labelNote;
  private Label engineStatusLabel;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureIntegrationComponent.ProceduresDropDown.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.textBoxResults = new TextBox();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.engineSpeedCheck = new Checkmark();
    this.buttonStart = new Button();
    this.buttonClose = new Button();
    this.engineStatusLabel = new Label();
    this.buttonStopAll = new Button();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.labelNote = new Label();
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textBoxResults, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    this.textBoxResults.BackColor = SystemColors.Control;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.textBoxResults, 5);
    componentResourceManager.ApplyResources((object) this.textBoxResults, "textBoxResults");
    this.textBoxResults.Name = "textBoxResults";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 2, 100);
    this.digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(4, 3.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(5, 4.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(6, 5.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(7, 6.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(8, 7.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(9, 8.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(10, 9.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(11, 10.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(12, 11.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(13, 12.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(14, 13.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(15, 14.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(16 /*0x10*/, 15.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(17, 16.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(18, 17.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(19, 18.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(20, 19.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(21, 20.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(22, 21.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(23, 22.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(24, 23.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(25, 30.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(26, 31.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(27, 32.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(28, 33.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(29, 34.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(30, 35.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(31 /*0x1F*/, 36.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(32 /*0x20*/, 50.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(33, 51.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(34, 52.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(35, 53.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(36, 54.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(37, 55.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(38, 56.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(39, 60.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(40, 62.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(41, 63.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(42, 64.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(43, 65.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(44, 66.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(45, 67.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(46, 68.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(47, 69.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(48 /*0x30*/, 70.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(49, 100.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(50, 101.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(51, 103.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(52, 104.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(53, 105.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(54, 106.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(55, 107.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(56, 108.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(57, 109.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(58, 110.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(59, 111.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(60, 112.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(61, 113.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(62, 115.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(63 /*0x3F*/, 116.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(64 /*0x40*/, 117.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(65, 118.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(66, 119.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(67, 121.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(68, 122.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(69, 123.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(70, 124.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(71, 140.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(72, 141.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(73, 142.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(74, 143.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(75, 144.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(76, 145.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(77, 146.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(78, 200.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(79, 201.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(80 /*0x50*/, 202.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(81, 203.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(82, 204.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(83, 205.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(84, 206.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(85, 236.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(86, 237.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(87, 238.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(88, 239.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(89, 240.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(90, 241.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(91, 243.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(92, 244.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(93, 245.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(94, 247.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(95, 248.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(96 /*0x60*/, 249.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(97, 250.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(98, 251.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(99, 252.0, (ValueState) 2);
    this.digitalReadoutInstrument1.Gradient.Modify(100, (double) byte.MaxValue, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS050_SRA3_Status_Code");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.engineSpeedCheck, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStart, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 4, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.engineStatusLabel, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStopAll, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.engineSpeedCheck, "engineSpeedCheck");
    ((Control) this.engineSpeedCheck).Name = "engineSpeedCheck";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.buttonStart, 2);
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.engineStatusLabel, 4);
    componentResourceManager.ApplyResources((object) this.engineStatusLabel, "engineStatusLabel");
    this.engineStatusLabel.Name = "engineStatusLabel";
    this.engineStatusLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonStopAll, "buttonStopAll");
    this.buttonStopAll.Name = "buttonStopAll";
    this.buttonStopAll.UseCompatibleTextRendering = true;
    this.buttonStopAll.UseVisualStyleBackColor = true;
    ((Control) this.sharedProcedureSelection).BackColor = SystemColors.Control;
    ((Control) this.sharedProcedureSelection).ForeColor = SystemColors.ControlText;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_EGFActuatorSlowLearn_MY20"
    });
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.labelNote, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.tableLayoutPanel1, 0, 1);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    this.labelNote.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelNote, "labelNote");
    ((Control) this.labelNote).Name = "labelNote";
    this.labelNote.Orientation = (Label.TextOrientation) 1;
    this.labelNote.UseSystemColors = true;
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.engineStatusLabel;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.engineSpeedCheck;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) this.textBoxResults;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent.StopAllButton = this.buttonStopAll;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Exhaust_Gas_Flap_Actuator_Slow_Learn");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel3);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
