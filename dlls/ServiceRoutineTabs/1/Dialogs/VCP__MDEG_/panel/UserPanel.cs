// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.VCP__MDEG_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VCP__MDEG_.panel;

public class UserPanel : CustomPanel
{
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentVCPTestRoutine;
  private SharedProcedureSelection sharedProcedureSelectionVCPTestRoutine;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentVCPTestRoutine;
  private Button buttonVCPTestRoutineStartStop;
  private Label labelVCPTestRoutineStatus;
  private Checkmark checkmarkVCPTestRoutine;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanelWholePanel;
  private SeekTimeListView seekTimeListViewLog;
  private Button buttonClose;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;

  public UserPanel()
  {
    this.InitializeComponent();
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.OnParentFormClosing);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureSelectionVCPTestRoutine.AnyProcedureInProgress)
    {
      this.LogText(Resources.Message_StopVCPTest);
      e.Cancel = true;
    }
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void LogText(string text)
  {
    this.LabelLog(this.seekTimeListViewLog.RequiredUserLabelPrefix, text);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    DataItemCondition dataItemCondition3 = new DataItemCondition();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.seekTimeListViewLog = new SeekTimeListView();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.checkmarkVCPTestRoutine = new Checkmark();
    this.buttonVCPTestRoutineStartStop = new Button();
    this.buttonClose = new Button();
    this.labelVCPTestRoutineStatus = new Label();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.sharedProcedureSelectionVCPTestRoutine = new SharedProcedureSelection();
    this.sharedProcedureCreatorComponentVCPTestRoutine = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentVCPTestRoutine = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.seekTimeListViewLog, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanel1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument3, 2, 0);
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.seekTimeListViewLog, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListViewLog, "seekTimeListViewLog");
    this.seekTimeListViewLog.FilterUserLabels = true;
    ((Control) this.seekTimeListViewLog).Name = "seekTimeListViewLog";
    this.seekTimeListViewLog.RequiredUserLabelPrefix = "VCP Test Routine";
    this.seekTimeListViewLog.SelectedTime = new DateTime?();
    this.seekTimeListViewLog.ShowChannelLabels = false;
    this.seekTimeListViewLog.ShowCommunicationsState = false;
    this.seekTimeListViewLog.ShowControlPanel = false;
    this.seekTimeListViewLog.ShowDeviceColumn = false;
    this.seekTimeListViewLog.TimeFormat = "HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrument1.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 3, 4);
    this.digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.tableLayoutPanel1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkVCPTestRoutine, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelVCPTestRoutineStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonVCPTestRoutineStartStop, 2, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.checkmarkVCPTestRoutine, "checkmarkVCPTestRoutine");
    ((Control) this.checkmarkVCPTestRoutine).Name = "checkmarkVCPTestRoutine";
    componentResourceManager.ApplyResources((object) this.buttonVCPTestRoutineStartStop, "buttonVCPTestRoutineStartStop");
    this.buttonVCPTestRoutineStartStop.Name = "buttonVCPTestRoutineStartStop";
    this.buttonVCPTestRoutineStartStop.UseCompatibleTextRendering = true;
    this.buttonVCPTestRoutineStartStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelVCPTestRoutineStatus, "labelVCPTestRoutineStatus");
    this.labelVCPTestRoutineStatus.Name = "labelVCPTestRoutineStatus";
    this.labelVCPTestRoutineStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    this.digitalReadoutInstrument3.Gradient.Initialize((ValueState) 1, 5);
    this.digitalReadoutInstrument3.Gradient.Modify(1, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument3.Gradient.Modify(2, 256.0, (ValueState) 3);
    this.digitalReadoutInstrument3.Gradient.Modify(3, "signal not available", (ValueState) 3);
    this.digitalReadoutInstrument3.Gradient.Modify(4, "No signal from input source i.e sensor/actuator", (ValueState) 3);
    this.digitalReadoutInstrument3.Gradient.Modify(5, "Values are invalid", (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS012_Vehicle_Speed");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((Control) this.digitalReadoutInstrument3).TabStop = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionVCPTestRoutine, "sharedProcedureSelectionVCPTestRoutine");
    ((Control) this.sharedProcedureSelectionVCPTestRoutine).Name = "sharedProcedureSelectionVCPTestRoutine";
    this.sharedProcedureSelectionVCPTestRoutine.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_VCP_Test"
    });
    this.sharedProcedureCreatorComponentVCPTestRoutine.Suspend();
    this.sharedProcedureCreatorComponentVCPTestRoutine.MonitorCall = new ServiceCall("MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Request_Results_Routine_status");
    this.sharedProcedureCreatorComponentVCPTestRoutine.MonitorGradient.Initialize((ValueState) 0, 2);
    this.sharedProcedureCreatorComponentVCPTestRoutine.MonitorGradient.Modify(1, 0.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentVCPTestRoutine.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentVCPTestRoutine, "sharedProcedureCreatorComponentVCPTestRoutine");
    this.sharedProcedureCreatorComponentVCPTestRoutine.Qualifier = "SP_VCP_Test";
    this.sharedProcedureCreatorComponentVCPTestRoutine.StartCall = new ServiceCall("MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Start");
    dataItemCondition1.Gradient.Initialize((ValueState) 3, 4);
    dataItemCondition1.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition1.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS003_MCM_wired_Ignition_Status");
    dataItemCondition2.Gradient.Initialize((ValueState) 1, 1);
    dataItemCondition2.Gradient.Modify(1, 1.0, (ValueState) 3);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed");
    dataItemCondition3.Gradient.Initialize((ValueState) 1, 5);
    dataItemCondition3.Gradient.Modify(1, 1.0, (ValueState) 3);
    dataItemCondition3.Gradient.Modify(2, 256.0, (ValueState) 3);
    dataItemCondition3.Gradient.Modify(3, "signal not available", (ValueState) 3);
    dataItemCondition3.Gradient.Modify(4, "No signal from input source i.e sensor/actuator", (ValueState) 3);
    dataItemCondition3.Gradient.Modify(5, "Values are invalid", (ValueState) 3);
    dataItemCondition3.Qualifier = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS012_Vehicle_Speed");
    this.sharedProcedureCreatorComponentVCPTestRoutine.StartConditions.Add(dataItemCondition1);
    this.sharedProcedureCreatorComponentVCPTestRoutine.StartConditions.Add(dataItemCondition2);
    this.sharedProcedureCreatorComponentVCPTestRoutine.StartConditions.Add(dataItemCondition3);
    this.sharedProcedureCreatorComponentVCPTestRoutine.StopCall = new ServiceCall("MCM21T", "RT_SR0B0_Set_AM_VCP_PWM_Stop");
    this.sharedProcedureCreatorComponentVCPTestRoutine.Resume();
    this.sharedProcedureIntegrationComponentVCPTestRoutine.ProceduresDropDown = this.sharedProcedureSelectionVCPTestRoutine;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.ProcedureStatusMessageTarget = this.labelVCPTestRoutineStatus;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.ProcedureStatusStateTarget = this.checkmarkVCPTestRoutine;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.StartStopButton = this.buttonVCPTestRoutineStartStop;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Variable_Camshaft_Phaser_VCP_");
    ((Control) this).Controls.Add((Control) this.sharedProcedureSelectionVCPTestRoutine);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
