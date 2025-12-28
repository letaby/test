// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Three_By_Two_Way_Valve_Teach_In__EMG_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Three_By_Two_Way_Valve_Teach_In__EMG_.panel;

public class UserPanel : CustomPanel
{
  private Channel eCpcChannel;
  private bool running = false;
  private TableLayoutPanel tableLayoutPanel1;
  private SeekTimeListView seekTimeListView1;
  private TableLayoutPanel tableLayoutPanelCoolantStart;
  private Button buttonStartCoolant;
  private System.Windows.Forms.Label label3;
  private System.Windows.Forms.Label label2;
  private TableLayoutPanel tableLayoutPanelBatteryStart;
  private Button buttonStartBattery;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentBattery;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentBattery;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentCoolant;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentCoolant;
  private SharedProcedureSelection sharedProcedureSelectionBattery;
  private SharedProcedureSelection sharedProcedureSelectionCoolant;
  private TableLayoutPanel tableLayoutPanelText;
  private TableLayoutPanel tableLayoutPanelText2;
  private System.Windows.Forms.Label label6;
  private TableLayoutPanel tableLayoutPanelBatteryMessages;
  private System.Windows.Forms.Label labelStatusBattery;
  private Checkmark checkmarkStatusBattery;
  private TableLayoutPanel tableLayoutPanelCoolantMessage;
  private System.Windows.Forms.Label labelStatusCoolant;
  private Checkmark checkmarkStatusCoolant;
  private DigitalReadoutInstrument digitalReadoutInstrumentBatteryResults;
  private DigitalReadoutInstrument digitalReadoutInstrumentCoolantResults;
  private TableLayoutPanel tableLayoutPanel4;
  private DigitalReadoutInstrument digitalReadoutInstrumentCharging;
  private System.Windows.Forms.Label label1;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private System.Windows.Forms.Label labelInterlockWarning;
  private System.Windows.Forms.Label label39;
  private Button buttonClose;
  private System.Windows.Forms.Label label5;

  private bool CanStart
  {
    get
    {
      return this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && this.digitalReadoutInstrumentParkBrake.RepresentedState == 1 && this.digitalReadoutInstrumentCharging.RepresentedState == 1;
    }
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.running = false;
    this.UpdateUI();
  }

  public UserPanel() => this.InitializeComponent();

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.running;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  public virtual void OnChannelsChanged() => this.SetECPC01TChannel("ECPC01T");

  private void SetECPC01TChannel(string ecuName)
  {
    if (this.eCpcChannel != this.GetChannel(ecuName))
    {
      this.running = false;
      this.eCpcChannel = this.GetChannel(ecuName);
    }
    this.UpdateUI();
  }

  private void AddLogLabel(string text)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void UpdateUI()
  {
    this.labelInterlockWarning.Visible = !this.CanStart;
    this.buttonClose.Enabled = !this.running;
  }

  private void sharedProcedureCreatorComponentBattery_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      this.AddLogLabel(Resources.Message_BatteryCoolantSystem3by2WayValveTeachInStarted);
    }
    else
    {
      this.running = false;
      this.AddLogLabel(Resources.Message_BatteryCoolantSystem3by2WayValveTeachInFailedToStart);
      this.AddLogLabel(((ResultEventArgs) e).Exception.Message);
    }
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentBattery_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.AddLogLabel(Resources.Message_BatteryCoolantSystem3by2WayValveTeachInStopped);
    this.running = false;
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentCoolant_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      this.AddLogLabel(Resources.Message_EdriveCoolantSystem3by2WayValveTeachInStarted);
    }
    else
    {
      this.running = false;
      this.AddLogLabel(Resources.Message_EdriveCoolantSystem3by2WayValveTeachInFailedToStart);
      this.AddLogLabel(((ResultEventArgs) e).Exception.Message);
    }
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentCoolant_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.AddLogLabel(Resources.Message_EdriveCoolantSystem3by2WayValveTeachInStopped);
    this.running = false;
    this.UpdateUI();
  }

  private void buttonStart_Click(object sender, EventArgs e)
  {
    this.running = true;
    this.UpdateUI();
  }

  private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    DataItemCondition dataItemCondition3 = new DataItemCondition();
    DataItemCondition dataItemCondition4 = new DataItemCondition();
    DataItemCondition dataItemCondition5 = new DataItemCondition();
    DataItemCondition dataItemCondition6 = new DataItemCondition();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
    this.label1 = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.labelInterlockWarning = new System.Windows.Forms.Label();
    this.label39 = new System.Windows.Forms.Label();
    this.seekTimeListView1 = new SeekTimeListView();
    this.tableLayoutPanelCoolantStart = new TableLayoutPanel();
    this.buttonStartCoolant = new Button();
    this.label3 = new System.Windows.Forms.Label();
    this.label2 = new System.Windows.Forms.Label();
    this.tableLayoutPanelBatteryStart = new TableLayoutPanel();
    this.buttonStartBattery = new Button();
    this.sharedProcedureSelectionBattery = new SharedProcedureSelection();
    this.sharedProcedureSelectionCoolant = new SharedProcedureSelection();
    this.tableLayoutPanelText = new TableLayoutPanel();
    this.label5 = new System.Windows.Forms.Label();
    this.tableLayoutPanelText2 = new TableLayoutPanel();
    this.label6 = new System.Windows.Forms.Label();
    this.tableLayoutPanelBatteryMessages = new TableLayoutPanel();
    this.labelStatusBattery = new System.Windows.Forms.Label();
    this.checkmarkStatusBattery = new Checkmark();
    this.tableLayoutPanelCoolantMessage = new TableLayoutPanel();
    this.labelStatusCoolant = new System.Windows.Forms.Label();
    this.checkmarkStatusCoolant = new Checkmark();
    this.digitalReadoutInstrumentBatteryResults = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCoolantResults = new DigitalReadoutInstrument();
    this.buttonClose = new Button();
    this.sharedProcedureIntegrationComponentBattery = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentBattery = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentCoolant = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentCoolant = new SharedProcedureCreatorComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this.tableLayoutPanelCoolantStart).SuspendLayout();
    ((Control) this.tableLayoutPanelBatteryStart).SuspendLayout();
    ((Control) this.tableLayoutPanelText).SuspendLayout();
    ((Control) this.tableLayoutPanelText2).SuspendLayout();
    ((Control) this.tableLayoutPanelBatteryMessages).SuspendLayout();
    ((Control) this.tableLayoutPanelCoolantMessage).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel4, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 1, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelCoolantStart, 9, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label3, 6, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelBatteryStart, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelectionBattery, 5, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelectionCoolant, 5, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelText, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelText2, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelBatteryMessages, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelCoolantMessage, 6, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentBatteryResults, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentCoolantResults, 6, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 9, 10);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentCharging, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.label1, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelInterlockWarning, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.label39, 0, 2);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel4, 11);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCharging, "digitalReadoutInstrumentCharging");
    this.digitalReadoutInstrumentCharging.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).FreezeValue = false;
    this.digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentCharging.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentCharging.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentCharging.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentCharging.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition");
    ((Control) this.digitalReadoutInstrumentCharging).Name = "digitalReadoutInstrumentCharging";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).ShowUnits = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCharging).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentCharging.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 3, 5, "mph");
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(5, (double) int.MaxValue, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).ShowUnits = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 0, 6);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(6, 5.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.labelInterlockWarning, "labelInterlockWarning");
    this.labelInterlockWarning.ForeColor = Color.Red;
    this.labelInterlockWarning.Name = "labelInterlockWarning";
    this.labelInterlockWarning.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label39, "label39");
    this.label39.Name = "label39";
    this.label39.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 9);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "3by2WayValveTeachInEMG";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelCoolantStart, "tableLayoutPanelCoolantStart");
    ((TableLayoutPanel) this.tableLayoutPanelCoolantStart).Controls.Add((Control) this.buttonStartCoolant, 0, 0);
    ((Control) this.tableLayoutPanelCoolantStart).Name = "tableLayoutPanelCoolantStart";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanelCoolantStart, 2);
    componentResourceManager.ApplyResources((object) this.buttonStartCoolant, "buttonStartCoolant");
    this.buttonStartCoolant.Name = "buttonStartCoolant";
    this.buttonStartCoolant.UseCompatibleTextRendering = true;
    this.buttonStartCoolant.UseVisualStyleBackColor = true;
    this.buttonStartCoolant.Click += new EventHandler(this.buttonStart_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label3, 4);
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label2, 4);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelBatteryStart, "tableLayoutPanelBatteryStart");
    ((TableLayoutPanel) this.tableLayoutPanelBatteryStart).Controls.Add((Control) this.buttonStartBattery, 0, 0);
    ((Control) this.tableLayoutPanelBatteryStart).Name = "tableLayoutPanelBatteryStart";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanelBatteryStart, 2);
    componentResourceManager.ApplyResources((object) this.buttonStartBattery, "buttonStartBattery");
    this.buttonStartBattery.Name = "buttonStartBattery";
    this.buttonStartBattery.UseCompatibleTextRendering = true;
    this.buttonStartBattery.UseVisualStyleBackColor = true;
    this.buttonStartBattery.Click += new EventHandler(this.buttonStart_Click);
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionBattery, "sharedProcedureSelectionBattery");
    ((Control) this.sharedProcedureSelectionBattery).Name = "sharedProcedureSelectionBattery";
    this.sharedProcedureSelectionBattery.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_BatteryCoolant3By2WayValveTeachIn"
    });
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionCoolant, "sharedProcedureSelectionCoolant");
    ((Control) this.sharedProcedureSelectionCoolant).Name = "sharedProcedureSelectionCoolant";
    this.sharedProcedureSelectionCoolant.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_EdriveCoolant3By2WayTeachIn"
    });
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelText, "tableLayoutPanelText");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelText, 9);
    ((TableLayoutPanel) this.tableLayoutPanelText).Controls.Add((Control) this.label5, 0, 0);
    ((Control) this.tableLayoutPanelText).Name = "tableLayoutPanelText";
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelText2, "tableLayoutPanelText2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelText2, 9);
    ((TableLayoutPanel) this.tableLayoutPanelText2).Controls.Add((Control) this.label6, 0, 0);
    ((Control) this.tableLayoutPanelText2).Name = "tableLayoutPanelText2";
    componentResourceManager.ApplyResources((object) this.label6, "label6");
    this.label6.Name = "label6";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelBatteryMessages, "tableLayoutPanelBatteryMessages");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelBatteryMessages, 4);
    ((TableLayoutPanel) this.tableLayoutPanelBatteryMessages).Controls.Add((Control) this.labelStatusBattery, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBatteryMessages).Controls.Add((Control) this.checkmarkStatusBattery, 0, 0);
    ((Control) this.tableLayoutPanelBatteryMessages).Name = "tableLayoutPanelBatteryMessages";
    ((TableLayoutPanel) this.tableLayoutPanelBatteryMessages).SetColumnSpan((Control) this.labelStatusBattery, 2);
    componentResourceManager.ApplyResources((object) this.labelStatusBattery, "labelStatusBattery");
    this.labelStatusBattery.Name = "labelStatusBattery";
    this.labelStatusBattery.UseCompatibleTextRendering = true;
    this.checkmarkStatusBattery.CheckState = CheckState.Checked;
    componentResourceManager.ApplyResources((object) this.checkmarkStatusBattery, "checkmarkStatusBattery");
    ((Control) this.checkmarkStatusBattery).Name = "checkmarkStatusBattery";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelCoolantMessage, "tableLayoutPanelCoolantMessage");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelCoolantMessage, 4);
    ((TableLayoutPanel) this.tableLayoutPanelCoolantMessage).Controls.Add((Control) this.labelStatusCoolant, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelCoolantMessage).Controls.Add((Control) this.checkmarkStatusCoolant, 0, 0);
    ((Control) this.tableLayoutPanelCoolantMessage).Name = "tableLayoutPanelCoolantMessage";
    ((TableLayoutPanel) this.tableLayoutPanelCoolantMessage).SetColumnSpan((Control) this.labelStatusCoolant, 2);
    componentResourceManager.ApplyResources((object) this.labelStatusCoolant, "labelStatusCoolant");
    this.labelStatusCoolant.Name = "labelStatusCoolant";
    this.labelStatusCoolant.UseCompatibleTextRendering = true;
    this.checkmarkStatusCoolant.CheckState = CheckState.Checked;
    componentResourceManager.ApplyResources((object) this.checkmarkStatusCoolant, "checkmarkStatusCoolant");
    ((Control) this.checkmarkStatusCoolant).Name = "checkmarkStatusCoolant";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentBatteryResults, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBatteryResults, "digitalReadoutInstrumentBatteryResults");
    this.digitalReadoutInstrumentBatteryResults.FontGroup = "Testing";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryResults).FreezeValue = false;
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText15"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText16"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText17"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText18"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText19"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText20"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText21"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText22"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText23"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText24"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText25"));
    this.digitalReadoutInstrumentBatteryResults.Gradient.Initialize((ValueState) 0, 15);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(7, 6.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(8, 7.0, (ValueState) 0);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(9, 8.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(10, 9.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(11, 10.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(12, 11.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(13, 12.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(14, 13.0, (ValueState) 3);
    this.digitalReadoutInstrumentBatteryResults.Gradient.Modify(15, 14.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState");
    ((Control) this.digitalReadoutInstrumentBatteryResults).Name = "digitalReadoutInstrumentBatteryResults";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrumentBatteryResults, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryResults).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryResults).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentCoolantResults, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCoolantResults, "digitalReadoutInstrumentCoolantResults");
    this.digitalReadoutInstrumentCoolantResults.FontGroup = "Testing";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantResults).FreezeValue = false;
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText26"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText27"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText28"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText29"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText30"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText31"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText32"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText33"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText34"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText35"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText36"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText37"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText38"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText39"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText40"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText41"));
    this.digitalReadoutInstrumentCoolantResults.Gradient.Initialize((ValueState) 0, 15);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(7, 6.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(8, 7.0, (ValueState) 0);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(9, 8.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(10, 9.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(11, 10.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(12, 11.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(13, 12.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(14, 13.0, (ValueState) 3);
    this.digitalReadoutInstrumentCoolantResults.Gradient.Modify(15, 14.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1");
    ((Control) this.digitalReadoutInstrumentCoolantResults).Name = "digitalReadoutInstrumentCoolantResults";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrumentCoolantResults, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantResults).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantResults).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.sharedProcedureIntegrationComponentBattery.ProceduresDropDown = this.sharedProcedureSelectionBattery;
    this.sharedProcedureIntegrationComponentBattery.ProcedureStatusMessageTarget = this.labelStatusBattery;
    this.sharedProcedureIntegrationComponentBattery.ProcedureStatusStateTarget = this.checkmarkStatusBattery;
    this.sharedProcedureIntegrationComponentBattery.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentBattery.StartStopButton = this.buttonStartBattery;
    this.sharedProcedureIntegrationComponentBattery.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentBattery.Suspend();
    this.sharedProcedureCreatorComponentBattery.AllowStopAlways = true;
    this.sharedProcedureCreatorComponentBattery.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_BattCircValvePosCtrlState");
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Initialize((ValueState) 0, 15);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(1, 0.0, (ValueState) 1);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(3, 2.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(5, 4.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(6, 5.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(7, 6.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(8, 7.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(9, 8.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(10, 9.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(11, 10.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(12, 11.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(13, 12.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(14, 13.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentBattery.MonitorGradient.Modify(15, 14.0, (ValueState) 3);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentBattery, "sharedProcedureCreatorComponentBattery");
    this.sharedProcedureCreatorComponentBattery.Qualifier = "SP_BatteryCoolant3By2WayValveTeachIn";
    this.sharedProcedureCreatorComponentBattery.StartCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_BatteryCirc", (IEnumerable<string>) new string[3]
    {
      "3by2WayValveBatteryCircuit=1",
      "3by2WayValveExtCircuit1=0",
      "3by2WayValveExtCircuit2=0"
    });
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText42"));
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText43"));
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText44"));
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText45"));
    dataItemCondition1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText46"));
    dataItemCondition1.Gradient.Initialize((ValueState) 0, 4);
    dataItemCondition1.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition1.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    dataItemCondition2.Gradient.Initialize((ValueState) 3, 5, "mph");
    dataItemCondition2.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition2.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition2.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition2.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition2.Gradient.Modify(5, (double) int.MaxValue, (ValueState) 3);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
    dataItemCondition3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText47"));
    dataItemCondition3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText48"));
    dataItemCondition3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText49"));
    dataItemCondition3.Gradient.Initialize((ValueState) 3, 2);
    dataItemCondition3.Gradient.Modify(1, 0.0, (ValueState) 1);
    dataItemCondition3.Gradient.Modify(2, 1.0, (ValueState) 3);
    dataItemCondition3.Qualifier = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition");
    this.sharedProcedureCreatorComponentBattery.StartConditions.Add(dataItemCondition1);
    this.sharedProcedureCreatorComponentBattery.StartConditions.Add(dataItemCondition2);
    this.sharedProcedureCreatorComponentBattery.StartConditions.Add(dataItemCondition3);
    this.sharedProcedureCreatorComponentBattery.StopCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_BasicCircValvePosCtrlState", (IEnumerable<string>) new string[3]
    {
      "BasicCircValvePosCtrlState=1",
      "ExtCircValvePosCtrlState_1=0",
      "ExtCircValvePosCtrlState_2=0"
    });
    this.sharedProcedureCreatorComponentBattery.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentBattery_StartServiceComplete);
    this.sharedProcedureCreatorComponentBattery.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentBattery_StopServiceComplete);
    this.sharedProcedureCreatorComponentBattery.Resume();
    this.sharedProcedureIntegrationComponentCoolant.ProceduresDropDown = this.sharedProcedureSelectionCoolant;
    this.sharedProcedureIntegrationComponentCoolant.ProcedureStatusMessageTarget = this.labelStatusCoolant;
    this.sharedProcedureIntegrationComponentCoolant.ProcedureStatusStateTarget = this.checkmarkStatusCoolant;
    this.sharedProcedureIntegrationComponentCoolant.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentCoolant.StartStopButton = this.buttonStartCoolant;
    this.sharedProcedureIntegrationComponentCoolant.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentCoolant.Suspend();
    this.sharedProcedureCreatorComponentCoolant.AllowStopAlways = true;
    this.sharedProcedureCreatorComponentCoolant.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Request_Results_ExtCircValvePosCtrlState_1");
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Initialize((ValueState) 0, 15);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(1, 0.0, (ValueState) 1);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(3, 2.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(5, 4.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(6, 5.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(7, 6.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(8, 7.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(9, 8.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(10, 9.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(11, 10.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(12, 11.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(13, 12.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(14, 13.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentCoolant.MonitorGradient.Modify(15, 14.0, (ValueState) 3);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentCoolant, "sharedProcedureCreatorComponentCoolant");
    this.sharedProcedureCreatorComponentCoolant.Qualifier = "SP_EdriveCoolant3By2WayTeachIn";
    this.sharedProcedureCreatorComponentCoolant.StartCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Start_3by2_wayValveMinMaxPositionTeachIn_ExtensionCkt_1", (IEnumerable<string>) new string[3]
    {
      "3by2WayValveBatteryCircuit=0",
      "3by2WayValveExtCircuit1=1",
      "3by2WayValveExtCircuit2=0"
    });
    dataItemCondition4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText50"));
    dataItemCondition4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText51"));
    dataItemCondition4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText52"));
    dataItemCondition4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText53"));
    dataItemCondition4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText54"));
    dataItemCondition4.Gradient.Initialize((ValueState) 0, 4);
    dataItemCondition4.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition4.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition4.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition4.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition4.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    dataItemCondition5.Gradient.Initialize((ValueState) 3, 5, "mph");
    dataItemCondition5.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition5.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition5.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition5.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition5.Gradient.Modify(5, (double) int.MaxValue, (ValueState) 3);
    dataItemCondition5.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
    dataItemCondition6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText55"));
    dataItemCondition6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText56"));
    dataItemCondition6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText57"));
    dataItemCondition6.Gradient.Initialize((ValueState) 3, 2);
    dataItemCondition6.Gradient.Modify(1, 0.0, (ValueState) 1);
    dataItemCondition6.Gradient.Modify(2, 1.0, (ValueState) 3);
    dataItemCondition6.Qualifier = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "FakeIsChargingPrecondition");
    this.sharedProcedureCreatorComponentCoolant.StartConditions.Add(dataItemCondition4);
    this.sharedProcedureCreatorComponentCoolant.StartConditions.Add(dataItemCondition5);
    this.sharedProcedureCreatorComponentCoolant.StartConditions.Add(dataItemCondition6);
    this.sharedProcedureCreatorComponentCoolant.StopCall = new ServiceCall("ECPC01T", "RT_TI_3by2_wayValveMinMaxPositionTeachIn_Stop_ExtCircValvePosCtrlState_1", (IEnumerable<string>) new string[3]
    {
      "BasicCircValvePosCtrlState=0",
      "ExtCircValvePosCtrlState_1=1",
      "ExtCircValvePosCtrlState_2=0"
    });
    this.sharedProcedureCreatorComponentCoolant.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentCoolant_StartServiceComplete);
    this.sharedProcedureCreatorComponentCoolant.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentCoolant_StopServiceComplete);
    this.sharedProcedureCreatorComponentCoolant.Resume();
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_3by2_Way_Valve_Teach_In");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).PerformLayout();
    ((Control) this.tableLayoutPanelCoolantStart).ResumeLayout(false);
    ((Control) this.tableLayoutPanelBatteryStart).ResumeLayout(false);
    ((Control) this.tableLayoutPanelText).ResumeLayout(false);
    ((Control) this.tableLayoutPanelText).PerformLayout();
    ((Control) this.tableLayoutPanelText2).ResumeLayout(false);
    ((Control) this.tableLayoutPanelText2).PerformLayout();
    ((Control) this.tableLayoutPanelBatteryMessages).ResumeLayout(false);
    ((Control) this.tableLayoutPanelCoolantMessage).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
