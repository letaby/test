// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Mechanical_Compression_Test.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Mechanical_Compression_Test.panel;

public class UserPanel : CustomPanel
{
  private double maxObservedEngineSpeed = 0.0;
  private bool showTestModeMessage = false;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentCompressionTestRoutine;
  private SharedProcedureSelection sharedProcedureSelectionCompressionTestRoutine;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentCompressionTestRoutine;
  private Button buttonCompressionTestRoutineStartStop;
  private System.Windows.Forms.Label labelCompressionTestRoutineStatus;
  private Checkmark checkmarkCompressionTestRoutine;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanelWholePanel;
  private Button buttonClose;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutBatteryVoltage;
  private SeekTimeListView seekTimeListViewLog;
  private System.Windows.Forms.Label labelTestInstructions;
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
    if (this.sharedProcedureSelectionCompressionTestRoutine.AnyProcedureInProgress)
    {
      this.LogText(Resources.Message_StopCompressionTest);
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

  private void sharedProcedureCreatorComponentCompressionTestRoutine_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.maxObservedEngineSpeed = 0.0;
    this.showTestModeMessage = true;
    this.LogText(Resources.Message_CompressionTestStarted);
    this.labelTestInstructions.Text = Resources.Message_PleaseTurnTheIgnition;
    this.buttonClose.Enabled = false;
  }

  private void sharedProcedureCreatorComponentCompressionTestRoutine_MonitorServiceComplete(
    object sender,
    MonitorServiceResultEventArgs e)
  {
    if (!((ResultEventArgs) e).Succeeded)
      return;
    if (this.showTestModeMessage)
    {
      this.LogText(Resources.Message_EngineIsRunningInCompressionTestMode);
      this.showTestModeMessage = false;
    }
    InstrumentCollection instruments = e.Service.Channel.Instruments;
    Qualifier instrument = ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument;
    string name = ((Qualifier) ref instrument).Name;
    double num = Convert.ToDouble(instruments[name].InstrumentValues.Current.Value.ToString());
    if (num > this.maxObservedEngineSpeed)
      this.maxObservedEngineSpeed = num;
  }

  private void sharedProcedureCreatorComponentCompressionTestRoutine_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (!((ResultEventArgs) e).Succeeded)
      return;
    string text = $"{Resources.Message_MaxObservedEngineSpeed} {this.maxObservedEngineSpeed.ToString()} {Resources.Message_EngineSpeedUnits}";
    this.LogText(Resources.Message_CompressionTestStopped);
    this.LogText(text);
    this.LogText(this.maxObservedEngineSpeed >= 150.0 ? Resources.Message_Success : Resources.Message_Failed);
    this.labelTestInstructions.Text = string.Empty;
    this.buttonClose.Enabled = true;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    DataItemCondition dataItemCondition3 = new DataItemCondition();
    DataItemCondition dataItemCondition4 = new DataItemCondition();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.digitalReadoutBatteryVoltage = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.checkmarkCompressionTestRoutine = new Checkmark();
    this.buttonCompressionTestRoutineStartStop = new Button();
    this.buttonClose = new Button();
    this.labelCompressionTestRoutineStatus = new System.Windows.Forms.Label();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.seekTimeListViewLog = new SeekTimeListView();
    this.labelTestInstructions = new System.Windows.Forms.Label();
    this.sharedProcedureSelectionCompressionTestRoutine = new SharedProcedureSelection();
    this.sharedProcedureCreatorComponentCompressionTestRoutine = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentCompressionTestRoutine = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutBatteryVoltage, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanel1, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.seekTimeListViewLog, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.labelTestInstructions, 0, 2);
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    componentResourceManager.ApplyResources((object) this.digitalReadoutBatteryVoltage, "digitalReadoutBatteryVoltage");
    this.digitalReadoutBatteryVoltage.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutBatteryVoltage).FreezeValue = false;
    this.digitalReadoutBatteryVoltage.Gradient.Initialize((ValueState) 3, 1, "V");
    this.digitalReadoutBatteryVoltage.Gradient.Modify(1, 12.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutBatteryVoltage).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS021_Battery_Voltage");
    ((Control) this.digitalReadoutBatteryVoltage).Name = "digitalReadoutBatteryVoltage";
    ((Control) this.digitalReadoutBatteryVoltage).TabStop = false;
    ((SingleInstrumentBase) this.digitalReadoutBatteryVoltage).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
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
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.tableLayoutPanel1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkCompressionTestRoutine, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonCompressionTestRoutineStartStop, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelCompressionTestRoutineStatus, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.checkmarkCompressionTestRoutine, "checkmarkCompressionTestRoutine");
    ((Control) this.checkmarkCompressionTestRoutine).Name = "checkmarkCompressionTestRoutine";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.checkmarkCompressionTestRoutine, 2);
    componentResourceManager.ApplyResources((object) this.buttonCompressionTestRoutineStartStop, "buttonCompressionTestRoutineStartStop");
    this.buttonCompressionTestRoutineStartStop.Name = "buttonCompressionTestRoutineStartStop";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.buttonCompressionTestRoutineStartStop, 2);
    this.buttonCompressionTestRoutineStartStop.UseCompatibleTextRendering = true;
    this.buttonCompressionTestRoutineStartStop.UseVisualStyleBackColor = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.buttonClose, 2);
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelCompressionTestRoutineStatus, "labelCompressionTestRoutineStatus");
    this.labelCompressionTestRoutineStatus.Name = "labelCompressionTestRoutineStatus";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.labelCompressionTestRoutineStatus, 2);
    this.labelCompressionTestRoutineStatus.UseCompatibleTextRendering = true;
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
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.seekTimeListViewLog, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListViewLog, "seekTimeListViewLog");
    this.seekTimeListViewLog.FilterUserLabels = true;
    ((Control) this.seekTimeListViewLog).Name = "seekTimeListViewLog";
    this.seekTimeListViewLog.RequiredUserLabelPrefix = "Compression Test Routine";
    this.seekTimeListViewLog.SelectedTime = new DateTime?();
    this.seekTimeListViewLog.ShowChannelLabels = false;
    this.seekTimeListViewLog.ShowCommunicationsState = false;
    this.seekTimeListViewLog.ShowControlPanel = false;
    this.seekTimeListViewLog.ShowDeviceColumn = false;
    this.seekTimeListViewLog.TimeFormat = "HH:mm:ss";
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.labelTestInstructions, 2);
    componentResourceManager.ApplyResources((object) this.labelTestInstructions, "labelTestInstructions");
    this.labelTestInstructions.ForeColor = Color.Red;
    this.labelTestInstructions.Name = "labelTestInstructions";
    this.labelTestInstructions.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionCompressionTestRoutine, "sharedProcedureSelectionCompressionTestRoutine");
    ((Control) this.sharedProcedureSelectionCompressionTestRoutine).Name = "sharedProcedureSelectionCompressionTestRoutine";
    this.sharedProcedureSelectionCompressionTestRoutine.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_Compression_Test"
    });
    this.sharedProcedureCreatorComponentCompressionTestRoutine.Suspend();
    this.sharedProcedureCreatorComponentCompressionTestRoutine.MonitorCall = new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status");
    this.sharedProcedureCreatorComponentCompressionTestRoutine.MonitorGradient.Initialize((ValueState) 0, 2);
    this.sharedProcedureCreatorComponentCompressionTestRoutine.MonitorGradient.Modify(1, 0.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentCompressionTestRoutine.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentCompressionTestRoutine, "sharedProcedureCreatorComponentCompressionTestRoutine");
    this.sharedProcedureCreatorComponentCompressionTestRoutine.Qualifier = "SP_Compression_Test";
    this.sharedProcedureCreatorComponentCompressionTestRoutine.StartCall = new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Start_Status");
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
    dataItemCondition4.Gradient.Initialize((ValueState) 3, 1, "V");
    dataItemCondition4.Gradient.Modify(1, 12.0, (ValueState) 1);
    dataItemCondition4.Qualifier = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS021_Battery_Voltage");
    this.sharedProcedureCreatorComponentCompressionTestRoutine.StartConditions.Add(dataItemCondition1);
    this.sharedProcedureCreatorComponentCompressionTestRoutine.StartConditions.Add(dataItemCondition2);
    this.sharedProcedureCreatorComponentCompressionTestRoutine.StartConditions.Add(dataItemCondition3);
    this.sharedProcedureCreatorComponentCompressionTestRoutine.StartConditions.Add(dataItemCondition4);
    this.sharedProcedureCreatorComponentCompressionTestRoutine.StopCall = new ServiceCall("MCM21T", "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0");
    this.sharedProcedureCreatorComponentCompressionTestRoutine.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentCompressionTestRoutine_StartServiceComplete);
    this.sharedProcedureCreatorComponentCompressionTestRoutine.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentCompressionTestRoutine_StopServiceComplete);
    this.sharedProcedureCreatorComponentCompressionTestRoutine.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponentCompressionTestRoutine_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentCompressionTestRoutine.Resume();
    this.sharedProcedureIntegrationComponentCompressionTestRoutine.ProceduresDropDown = this.sharedProcedureSelectionCompressionTestRoutine;
    this.sharedProcedureIntegrationComponentCompressionTestRoutine.ProcedureStatusMessageTarget = this.labelCompressionTestRoutineStatus;
    this.sharedProcedureIntegrationComponentCompressionTestRoutine.ProcedureStatusStateTarget = this.checkmarkCompressionTestRoutine;
    this.sharedProcedureIntegrationComponentCompressionTestRoutine.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentCompressionTestRoutine.StartStopButton = this.buttonCompressionTestRoutineStartStop;
    this.sharedProcedureIntegrationComponentCompressionTestRoutine.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Mechanical_Compression_Test");
    ((Control) this).Controls.Add((Control) this.sharedProcedureSelectionCompressionTestRoutine);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
