// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Electric_Refrigerant_Compressor_Reset__EMG_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Electric_Refrigerant_Compressor_Reset__EMG_.panel;

public class UserPanel : CustomPanel
{
  private bool ResetCanceled = false;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehSpeed;
  private TableLayoutPanel tableLayoutPanel2;
  private Checkmark checkmark1;
  private SeekTimeListView seekTimeListView1;
  private DigitalReadoutInstrument digitalReadoutInstrumentResetStatus;
  private SharedProcedureSelection sharedProcedureSelection1;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponent1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private Button button1;
  private System.Windows.Forms.Label labelServiceStartStatus;

  private bool Running => this.sharedProcedureSelection1.SelectedProcedure.State != 0;

  public UserPanel() => this.InitializeComponent();

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel |= this.Running;
    if (e.Cancel)
      return;
    this.ParentFormClosing -= new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
  }

  private void UpdateMessage(string message)
  {
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, message);
  }

  private void sharedProcedureCreatorComponent1_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    string empty = string.Empty;
    this.UpdateMessage(!((ResultEventArgs) e).Succeeded ? (((ResultEventArgs) e).Exception == null || string.IsNullOrEmpty(((ResultEventArgs) e).Exception.Message) ? Resources.MessageErcResetStartFailed : $"{Resources.MessageErcResetStartFailed} Error: {((ResultEventArgs) e).Exception.Message}.") : Resources.MessageErcResetStarted);
  }

  private void sharedProcedureCreatorComponent1_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    bool flag = this.digitalReadoutInstrumentResetStatus.RepresentedState == 1;
    this.UpdateMessage(this.ResetCanceled ? Resources.MessageErcResetStopped : (flag ? Resources.MessageErcResetPassed : Resources.MessageErcResetFailed));
  }

  private void button1_Click(object sender, EventArgs e) => this.ResetCanceled = this.Running;

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehSpeed = new DigitalReadoutInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.checkmark1 = new Checkmark();
    this.labelServiceStartStatus = new System.Windows.Forms.Label();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.button1 = new Button();
    this.seekTimeListView1 = new SeekTimeListView();
    this.digitalReadoutInstrumentResetStatus = new DigitalReadoutInstrument();
    this.sharedProcedureCreatorComponent1 = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentParkingBrake, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentVehSpeed, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentResetStatus, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
    this.digitalReadoutInstrumentParkingBrake.FontGroup = "MainInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentParkingBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState) 3, 4);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    ((Control) this.digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehSpeed, "digitalReadoutInstrumentVehSpeed");
    this.digitalReadoutInstrumentVehSpeed.FontGroup = "MainInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentVehSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentVehSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentVehSpeed.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentVehSpeed.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehSpeed.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    ((Control) this.digitalReadoutInstrumentVehSpeed).Name = "digitalReadoutInstrumentVehSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmark1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelServiceStartStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.sharedProcedureSelection1, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.button1, 2, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    componentResourceManager.ApplyResources((object) this.labelServiceStartStatus, "labelServiceStartStatus");
    this.labelServiceStartStatus.Name = "labelServiceStartStatus";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_ElectricRefrigerantCompressorReset"
    });
    componentResourceManager.ApplyResources((object) this.button1, "button1");
    this.button1.Name = "button1";
    this.button1.UseVisualStyleBackColor = true;
    this.button1.Click += new EventHandler(this.button1_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "ErcReset";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentResetStatus, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentResetStatus, "digitalReadoutInstrumentResetStatus");
    this.digitalReadoutInstrumentResetStatus.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResetStatus).FreezeValue = false;
    this.digitalReadoutInstrumentResetStatus.Gradient.Initialize((ValueState) 0, 6);
    this.digitalReadoutInstrumentResetStatus.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentResetStatus.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentResetStatus.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentResetStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentResetStatus.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentResetStatus.Gradient.Modify(6, 21.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResetStatus).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results_ERC_Reset_Ctrl");
    ((Control) this.digitalReadoutInstrumentResetStatus).Name = "digitalReadoutInstrumentResetStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResetStatus).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureCreatorComponent1.Suspend();
    this.sharedProcedureCreatorComponent1.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Request_Results");
    this.sharedProcedureCreatorComponent1.MonitorGradient.Initialize((ValueState) 0, 6);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(1, 0.0, (ValueState) 0);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(3, 2.0, (ValueState) 1);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(5, 4.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(6, 21.0, (ValueState) 3);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponent1, "sharedProcedureCreatorComponent1");
    this.sharedProcedureCreatorComponent1.Qualifier = "SP_ElectricRefrigerantCompressorReset";
    this.sharedProcedureCreatorComponent1.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Start");
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
    dataItemCondition1.Gradient.Initialize((ValueState) 2, 4);
    dataItemCondition1.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition1.Gradient.Modify(3, 2.0, (ValueState) 2);
    dataItemCondition1.Gradient.Modify(4, 3.0, (ValueState) 2);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    dataItemCondition2.Gradient.Initialize((ValueState) 3, 2);
    dataItemCondition2.Gradient.Modify(1, 0.0, (ValueState) 1);
    dataItemCondition2.Gradient.Modify(2, 1.0, (ValueState) 3);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    this.sharedProcedureCreatorComponent1.StartConditions.Add(dataItemCondition1);
    this.sharedProcedureCreatorComponent1.StartConditions.Add(dataItemCondition2);
    this.sharedProcedureCreatorComponent1.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_ERC_Reset_Ctrl_Stop");
    this.sharedProcedureCreatorComponent1.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent1_StartServiceComplete);
    this.sharedProcedureCreatorComponent1.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent1_StopServiceComplete);
    this.sharedProcedureCreatorComponent1.Resume();
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.labelServiceStartStatus;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.button1;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_ERC_Reset");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
