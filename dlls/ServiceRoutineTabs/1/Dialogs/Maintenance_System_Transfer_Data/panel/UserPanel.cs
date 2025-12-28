// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data.panel;

public class UserPanel : CustomPanel
{
  private SeekTimeListView seekTimeListView1;
  private Button button1;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentTransferData;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private SharedProcedureSelection sharedProcedureSelection1;
  private System.Windows.Forms.Label label1;
  private Checkmark checkmark1;
  private DigitalReadoutInstrument digitalReadoutRequestResults;
  private TableLayoutPanel tableLayoutPanel1;
  private TableLayoutPanel tableLayoutPanel2;
  private Button button2;

  private bool testStoppedByUser { get; set; }

  public UserPanel() => this.InitializeComponent();

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.ParentFormClosing -= new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
  }

  private void button1_Click(object sender, EventArgs e)
  {
    this.testStoppedByUser = this.sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress;
  }

  private void digitalReadoutRequestResults_RepresentedStateChanged(object sender, EventArgs e)
  {
    if (((SingleInstrumentBase) this.digitalReadoutRequestResults).DataItem == null)
      return;
    this.LogText(((SingleInstrumentBase) this.digitalReadoutRequestResults).DataItem.ValueAsString(((SingleInstrumentBase) this.digitalReadoutRequestResults).DataItem.Value));
  }

  private void LogText(string text)
  {
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void sharedProcedureCreatorComponentTransferData_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.LogText(Resources.Message_Started);
  }

  private void sharedProcedureCreatorComponentTransferData_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      if (this.testStoppedByUser)
        this.LogText(Resources.Message_Stopped);
      else if (((SingleInstrumentBase) this.digitalReadoutRequestResults).DataItem.Choices != null && ((SingleInstrumentBase) this.digitalReadoutRequestResults).DataItem.Value == (object) ((SingleInstrumentBase) this.digitalReadoutRequestResults).DataItem.Choices.GetItemFromRawValue((object) 0))
        this.LogText(Resources.Message_CompleteSuccess);
      else
        this.LogText(Resources.Message_Error);
    }
    else
    {
      this.LogText(Resources.Message_Error);
      this.LogText(((ResultEventArgs) e).Exception.Message);
    }
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.seekTimeListView1 = new SeekTimeListView();
    this.button1 = new Button();
    this.sharedProcedureCreatorComponentTransferData = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.label1 = new System.Windows.Forms.Label();
    this.checkmark1 = new Checkmark();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.button2 = new Button();
    this.digitalReadoutRequestResults = new DigitalReadoutInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "MaintenanceSystemTransfer";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.seekTimeListView1.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.button1, "button1");
    this.button1.Name = "button1";
    this.button1.UseCompatibleTextRendering = true;
    this.button1.UseVisualStyleBackColor = true;
    this.button1.Click += new EventHandler(this.button1_Click);
    this.sharedProcedureCreatorComponentTransferData.Suspend();
    this.sharedProcedureCreatorComponentTransferData.MonitorCall = new ServiceCall("MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory");
    this.sharedProcedureCreatorComponentTransferData.MonitorGradient.Initialize((ValueState) 0, 7);
    this.sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(1, 0.0, (ValueState) 1);
    this.sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(3, 2.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(5, 4.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(6, 5.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(7, 6.0, (ValueState) 3);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentTransferData, "sharedProcedureCreatorComponentTransferData");
    this.sharedProcedureCreatorComponentTransferData.Qualifier = "SP_TransferDataFromMirrorMemory";
    this.sharedProcedureCreatorComponentTransferData.StartCall = new ServiceCall("MS01T", "RT_Transfer_data_from_the_mirror_memory_Start");
    this.sharedProcedureCreatorComponentTransferData.StopCall = new ServiceCall("MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory");
    this.sharedProcedureCreatorComponentTransferData.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentTransferData_StartServiceComplete);
    this.sharedProcedureCreatorComponentTransferData.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentTransferData_StopServiceComplete);
    this.sharedProcedureCreatorComponentTransferData.Resume();
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.label1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.button1;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_TransferDataFromMirrorMemory"
    });
    ((Control) this.sharedProcedureSelection1).TabStop = false;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.button1, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmark1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection1, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.button2, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.button2.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.button2, "button2");
    this.button2.Name = "button2";
    this.button2.UseCompatibleTextRendering = true;
    this.button2.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.digitalReadoutRequestResults, "digitalReadoutRequestResults");
    this.digitalReadoutRequestResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutRequestResults).FreezeValue = false;
    this.digitalReadoutRequestResults.Gradient.Initialize((ValueState) 0, 7);
    this.digitalReadoutRequestResults.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutRequestResults.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutRequestResults.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutRequestResults.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutRequestResults.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutRequestResults.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.digitalReadoutRequestResults.Gradient.Modify(7, 6.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutRequestResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory");
    ((Control) this.digitalReadoutRequestResults).Name = "digitalReadoutRequestResults";
    ((SingleInstrumentBase) this.digitalReadoutRequestResults).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutRequestResults.RepresentedStateChanged += new EventHandler(this.digitalReadoutRequestResults_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.seekTimeListView1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.tableLayoutPanel1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.digitalReadoutRequestResults, 0, 1);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel2);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
