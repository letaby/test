// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Hysteresis_Measurement.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Hysteresis_Measurement.panel;

public class UserPanel : CustomPanel
{
  private const string RequestADCPosEGRStatus = "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_is05_adc_pos_egr";
  private string is05AdcPosEGR = string.Empty;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentHysteresisRoutine;
  private SharedProcedureSelection sharedProcedureSelectionHysteresisRoutine;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentVCPTestRoutine;
  private Button buttonHysteresisRoutineStartStop;
  private System.Windows.Forms.Label labelHysteresisRoutineStatus;
  private Checkmark checkmarkHysteresisRoutine;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanelWholePanel;
  private SeekTimeListView seekTimeListViewLog;
  private Button buttonClose;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
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
    if (this.sharedProcedureSelectionHysteresisRoutine.AnyProcedureInProgress)
    {
      this.LogText(Resources.Message_StopHysteresisTest);
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

  private void sharedProcedureCreatorComponentHysteresisRoutine_MonitorServiceComplete(
    object sender,
    MonitorServiceResultEventArgs e)
  {
    Service service = e.Service.Channel.Services["RT_SR0CB_EGR_hysteresis_measurement_Request_Results_is05_adc_pos_egr"];
    if (!(service != (Service) null))
      return;
    try
    {
      service.Execute(true);
    }
    catch (CaesarException ex)
    {
      ((MonitorResultEventArgs) e).Action = (MonitorAction) 1;
      return;
    }
    if (!service.OutputValues[0].Value.ToString().Equals(this.is05AdcPosEGR))
    {
      this.is05AdcPosEGR = service.OutputValues[0].Value.ToString();
      this.LogText(this.is05AdcPosEGR);
    }
  }

  private void sharedProcedureCreatorComponentHysteresisRoutine_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.is05AdcPosEGR = string.Empty;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    DataItemCondition dataItemCondition3 = new DataItemCondition();
    this.checkmarkHysteresisRoutine = new Checkmark();
    this.buttonHysteresisRoutineStartStop = new Button();
    this.buttonClose = new Button();
    this.labelHysteresisRoutineStatus = new System.Windows.Forms.Label();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.seekTimeListViewLog = new SeekTimeListView();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.sharedProcedureSelectionHysteresisRoutine = new SharedProcedureSelection();
    this.sharedProcedureCreatorComponentHysteresisRoutine = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentVCPTestRoutine = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.tableLayoutPanel1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkHysteresisRoutine, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonHysteresisRoutineStartStop, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelHysteresisRoutineStatus, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.checkmarkHysteresisRoutine, "checkmarkHysteresisRoutine");
    ((Control) this.checkmarkHysteresisRoutine).Name = "checkmarkHysteresisRoutine";
    componentResourceManager.ApplyResources((object) this.buttonHysteresisRoutineStartStop, "buttonHysteresisRoutineStartStop");
    this.buttonHysteresisRoutineStartStop.Name = "buttonHysteresisRoutineStartStop";
    this.buttonHysteresisRoutineStartStop.UseCompatibleTextRendering = true;
    this.buttonHysteresisRoutineStartStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelHysteresisRoutineStatus, "labelHysteresisRoutineStatus");
    this.labelHysteresisRoutineStatus.Name = "labelHysteresisRoutineStatus";
    this.labelHysteresisRoutineStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument6, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument5, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.seekTimeListViewLog, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument4, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanel1, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 1);
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_am_egr_current_valve");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_is05_adc_pos_egr");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).SetColumnSpan((Control) this.seekTimeListViewLog, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListViewLog, "seekTimeListViewLog");
    ((Control) this.seekTimeListViewLog).Name = "seekTimeListViewLog";
    this.seekTimeListViewLog.RequiredUserLabelPrefix = "";
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
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS023_Engine_State");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS032_EGR_Actual_Valve_Position");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrument2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrument2.Gradient.Modify(1, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(2, 2.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ignitionStatus");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS031_EGR_Commanded_Governor_Value");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((Control) this.digitalReadoutInstrument3).TabStop = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionHysteresisRoutine, "sharedProcedureSelectionHysteresisRoutine");
    ((Control) this.sharedProcedureSelectionHysteresisRoutine).Name = "sharedProcedureSelectionHysteresisRoutine";
    this.sharedProcedureSelectionHysteresisRoutine.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_EGR_Hysteresis"
    });
    this.sharedProcedureCreatorComponentHysteresisRoutine.Suspend();
    this.sharedProcedureCreatorComponentHysteresisRoutine.MonitorCall = new ServiceCall("MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Request_Results_am_egr_current_valve");
    this.sharedProcedureCreatorComponentHysteresisRoutine.MonitorGradient.Initialize((ValueState) 0, 2);
    this.sharedProcedureCreatorComponentHysteresisRoutine.MonitorGradient.Modify(1, 0.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentHysteresisRoutine.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentHysteresisRoutine, "sharedProcedureCreatorComponentHysteresisRoutine");
    this.sharedProcedureCreatorComponentHysteresisRoutine.Qualifier = "SP_EGR_Hysteresis";
    this.sharedProcedureCreatorComponentHysteresisRoutine.StartCall = new ServiceCall("MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Start");
    dataItemCondition1.Gradient.Initialize((ValueState) 3, 2);
    dataItemCondition1.Gradient.Modify(1, 1.0, (ValueState) 1);
    dataItemCondition1.Gradient.Modify(2, 2.0, (ValueState) 3);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "ignitionStatus");
    dataItemCondition2.Gradient.Initialize((ValueState) 1, 1);
    dataItemCondition2.Gradient.Modify(1, 1.0, (ValueState) 3);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    dataItemCondition3.Gradient.Initialize((ValueState) 1, 1);
    dataItemCondition3.Gradient.Modify(1, 1.0, (ValueState) 3);
    dataItemCondition3.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    this.sharedProcedureCreatorComponentHysteresisRoutine.StartConditions.Add(dataItemCondition1);
    this.sharedProcedureCreatorComponentHysteresisRoutine.StartConditions.Add(dataItemCondition2);
    this.sharedProcedureCreatorComponentHysteresisRoutine.StartConditions.Add(dataItemCondition3);
    this.sharedProcedureCreatorComponentHysteresisRoutine.StopCall = new ServiceCall("MCM21T", "RT_SR0CB_EGR_hysteresis_measurement_Stop");
    this.sharedProcedureCreatorComponentHysteresisRoutine.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentHysteresisRoutine_StartServiceComplete);
    this.sharedProcedureCreatorComponentHysteresisRoutine.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponentHysteresisRoutine_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentHysteresisRoutine.Resume();
    this.sharedProcedureIntegrationComponentVCPTestRoutine.ProceduresDropDown = this.sharedProcedureSelectionHysteresisRoutine;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.ProcedureStatusMessageTarget = this.labelHysteresisRoutineStatus;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.ProcedureStatusStateTarget = this.checkmarkHysteresisRoutine;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.StartStopButton = this.buttonHysteresisRoutineStartStop;
    this.sharedProcedureIntegrationComponentVCPTestRoutine.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.sharedProcedureSelectionHysteresisRoutine);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
