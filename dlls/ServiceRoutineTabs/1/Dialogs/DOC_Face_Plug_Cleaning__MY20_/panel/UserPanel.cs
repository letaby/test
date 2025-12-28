// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning__MY20_.panel.UserPanel
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
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning__MY20_.panel;

public class UserPanel : CustomPanel
{
  private const string CpcName = "CPC04T";
  private const string SsamName = "SSAM02T";
  private const string AcmName = "ACM301T";
  private const string DpfRegenSwitchStatusMy13Qualifier = "DT_DSL_DPF_Regen_Switch_Status";
  private const string DpfRegenSwitchStatusNGCQualifier = "RT_MSC_GetSwState_Start_Switch_033";
  private const string DefPressureQualifier = "DT_AS014_DEF_Pressure";
  private TableLayoutPanel tableLayoutPanel1;
  private BarInstrument barInstrument3;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private Button button1;
  private SharedProcedureSelection sharedProcedureSelection1;
  private Label LabelDocFacePlugUnclogging;
  private BarInstrument barInstrument2;
  private SeekTimeListView DocFacePlugUncloggingResults;
  private Checkmark checkmark1;
  private DigitalReadoutInstrument dpfRegenSwitchStatus;
  private Label commandLabel;
  private DigitalReadoutInstrument digitalReadoutInstrumentResults;
  private BarInstrument barInstrument1;
  private BarInstrument barInstrument9;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private TableLayoutPanel tableLayoutPanel2;
  private SharedProcedureCreatorComponent DocFacePlugUnclogging;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private DigitalReadoutInstrument digitalReadoutInstrument7;
  private TableLayoutPanel tableLayoutPanelVerticalInstruments;
  private BarInstrument barInstrument5;
  private BarInstrument barInstrument6;
  private BarInstrument barInstrument8;
  private BarInstrument barInstrument4;
  private BarInstrument barInstrumentDefPressure;
  private DigitalReadoutInstrument ActualDpfZone;

  public UserPanel()
  {
    this.InitializeComponent();
    this.ActualDpfZone.RepresentedStateChanged += new EventHandler(this.ActualDpfZone_RepresentedStateChanged);
    this.dpfRegenSwitchStatus.RepresentedStateChanged += new EventHandler(this.dpfRegenSwitchStatus_RepresentedStateChanged);
    this.sharedProcedureIntegrationComponent1.StartStopButton.Click += new EventHandler(this.StartStopButton_Click);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    base.OnChannelsChanged();
  }

  private void StartStopButton_Click(object sender, EventArgs e) => this.commandLabel.Text = "";

  private void UpdateConnectedEquipmentType()
  {
    SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault<EquipmentType>((Func<EquipmentType, bool>) (et =>
    {
      ElectronicsFamily family = ((EquipmentType) ref et).Family;
      return ((ElectronicsFamily) ref family).Category.Equals("Engine", StringComparison.OrdinalIgnoreCase);
    }));
    this.UpdateDEFPressureInstrument();
  }

  private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
  {
    if (!e.Category.Equals("Engine", StringComparison.OrdinalIgnoreCase))
      return;
    this.UpdateConnectedEquipmentType();
  }

  private void ActualDpfZone_RepresentedStateChanged(object sender, EventArgs e)
  {
    if (this.ActualDpfZone.RepresentedState != 1 || this.sharedProcedureIntegrationComponent1.ProceduresDropDown.SelectedProcedure.State != 2)
      return;
    this.commandLabel.Text = Resources.Message_PressAndHoldTheRegenSwitchOnTheDashForFiveSecondsToStartTheRegen;
  }

  private void dpfRegenSwitchStatus_RepresentedStateChanged(object sender, EventArgs e)
  {
    if (this.dpfRegenSwitchStatus.RepresentedState != 1 || this.sharedProcedureIntegrationComponent1.ProceduresDropDown.SelectedProcedure.State != 2)
      return;
    this.commandLabel.Text = "";
  }

  private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureSelection1.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.this_ParentFormClosing);
    string messageNotAvailable = Resources.Message_NotAvailable;
    bool flag = false;
    if (this.sharedProcedureSelection1.SelectedProcedure != null)
    {
      flag = this.sharedProcedureSelection1.SelectedProcedure.Result == 1;
      if (((SingleInstrumentBase) this.digitalReadoutInstrumentResults).DataItem != null && ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).DataItem.Value != null)
        messageNotAvailable = ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).DataItem.Value.ToString();
    }
    ((Control) this).Tag = (object) new object[2]
    {
      (object) flag,
      (object) messageNotAvailable
    };
  }

  public virtual void OnChannelsChanged()
  {
    this.UpdateConnectedEquipmentType();
    this.UpdateDpfRegenSwitchInstrument();
  }

  private void UpdateDpfRegenSwitchInstrument()
  {
    if (this.GetChannel("SSAM02T") != null)
      ((SingleInstrumentBase) this.dpfRegenSwitchStatus).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "RT_MSC_GetSwState_Start_Switch_033");
    else
      ((SingleInstrumentBase) this.dpfRegenSwitchStatus).Instrument = new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_DPF_Regen_Switch_Status");
    ((Control) this.dpfRegenSwitchStatus).Refresh();
  }

  private void UpdateDEFPressureInstrument()
  {
    ((SingleInstrumentBase) this.barInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS014_DEF_Pressure");
    ((Control) this.barInstrumentDefPressure).Refresh();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.checkmark1 = new Checkmark();
    this.LabelDocFacePlugUnclogging = new Label();
    this.commandLabel = new Label();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.DocFacePlugUncloggingResults = new SeekTimeListView();
    this.digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
    this.dpfRegenSwitchStatus = new DigitalReadoutInstrument();
    this.ActualDpfZone = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.barInstrument2 = new BarInstrument();
    this.barInstrument1 = new BarInstrument();
    this.barInstrument9 = new BarInstrument();
    this.barInstrument3 = new BarInstrument();
    this.button1 = new Button();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    this.DocFacePlugUnclogging = new SharedProcedureCreatorComponent(this.components);
    this.tableLayoutPanelVerticalInstruments = new TableLayoutPanel();
    this.barInstrumentDefPressure = new BarInstrument();
    this.barInstrument4 = new BarInstrument();
    this.barInstrument8 = new BarInstrument();
    this.barInstrument6 = new BarInstrument();
    this.barInstrument5 = new BarInstrument();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanelVerticalInstruments).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument6, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument7, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DocFacePlugUncloggingResults, 2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentResults, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dpfRegenSwitchStatus, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.ActualDpfZone, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument5, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument2, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument1, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument9, 6, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument3, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.button1, 7, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelVerticalInstruments, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 7);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.sharedProcedureSelection1, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmark1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.LabelDocFacePlugUnclogging, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.commandLabel, 1, 1);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_DocFacePlugUnclogging"
    });
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    ((TableLayoutPanel) this.tableLayoutPanel2).SetRowSpan((Control) this.checkmark1, 2);
    componentResourceManager.ApplyResources((object) this.LabelDocFacePlugUnclogging, "LabelDocFacePlugUnclogging");
    this.LabelDocFacePlugUnclogging.Name = "LabelDocFacePlugUnclogging";
    this.LabelDocFacePlugUnclogging.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.commandLabel, "commandLabel");
    this.commandLabel.Name = "commandLabel";
    this.commandLabel.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument6, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS143_ADS_Pump_Speed");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument7, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument7, "digitalReadoutInstrument7");
    this.digitalReadoutInstrument7.FontGroup = "engineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 3, 4);
    this.digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS094_Actual_Torque_Load");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.DocFacePlugUncloggingResults, 6);
    componentResourceManager.ApplyResources((object) this.DocFacePlugUncloggingResults, "DocFacePlugUncloggingResults");
    this.DocFacePlugUncloggingResults.FilterUserLabels = true;
    ((Control) this.DocFacePlugUncloggingResults).Name = "DocFacePlugUncloggingResults";
    this.DocFacePlugUncloggingResults.RequiredUserLabelPrefix = "DOC Face Plug Cleaning";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.DocFacePlugUncloggingResults, 3);
    this.DocFacePlugUncloggingResults.SelectedTime = new DateTime?();
    this.DocFacePlugUncloggingResults.ShowChannelLabels = false;
    this.DocFacePlugUncloggingResults.ShowCommunicationsState = false;
    this.DocFacePlugUncloggingResults.ShowControlPanel = false;
    this.DocFacePlugUncloggingResults.ShowDeviceColumn = false;
    this.DocFacePlugUncloggingResults.TimeFormat = "HH:mm:ss";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentResults, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentResults, "digitalReadoutInstrumentResults");
    this.digitalReadoutInstrumentResults.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ACM301T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status");
    ((Control) this.digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.dpfRegenSwitchStatus, 2);
    componentResourceManager.ApplyResources((object) this.dpfRegenSwitchStatus, "dpfRegenSwitchStatus");
    this.dpfRegenSwitchStatus.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.dpfRegenSwitchStatus).FreezeValue = false;
    this.dpfRegenSwitchStatus.Gradient.Initialize((ValueState) 0, 4);
    this.dpfRegenSwitchStatus.Gradient.Modify(1, 0.0, (ValueState) 2);
    this.dpfRegenSwitchStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.dpfRegenSwitchStatus.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.dpfRegenSwitchStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.dpfRegenSwitchStatus).Instrument = new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_DPF_Regen_Switch_Status");
    ((Control) this.dpfRegenSwitchStatus).Name = "dpfRegenSwitchStatus";
    ((SingleInstrumentBase) this.dpfRegenSwitchStatus).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.ActualDpfZone, 2);
    componentResourceManager.ApplyResources((object) this.ActualDpfZone, "ActualDpfZone");
    this.ActualDpfZone.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.ActualDpfZone).FreezeValue = false;
    this.ActualDpfZone.Gradient.Initialize((ValueState) 2, 2);
    this.ActualDpfZone.Gradient.Modify(1, 2.0, (ValueState) 1);
    this.ActualDpfZone.Gradient.Modify(2, 3.0, (ValueState) 2);
    ((SingleInstrumentBase) this.ActualDpfZone).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS065_Actual_DPF_zone");
    ((Control) this.ActualDpfZone).Name = "ActualDpfZone";
    ((SingleInstrumentBase) this.ActualDpfZone).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument3, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS069_Jake_Brake_1_PWM13");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument4, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS036_SCR_Inlet_NOx_Sensor");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument5, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS035_SCR_Outlet_NOx_Sensor");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument2, "barInstrument2");
    this.barInstrument2.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS005_DOC_Inlet_Pressure");
    ((Control) this.barInstrument2).Name = "barInstrument2";
    ((SingleInstrumentBase) this.barInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument1, "barInstrument1");
    this.barInstrument1.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS034_Throttle_Valve_Actual_Position");
    ((Control) this.barInstrument1).Name = "barInstrument1";
    ((SingleInstrumentBase) this.barInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument9, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument9, "barInstrument9");
    this.barInstrument9.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS033_Throttle_Valve_Commanded_Value");
    ((Control) this.barInstrument9).Name = "barInstrument9";
    ((SingleInstrumentBase) this.barInstrument9).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument3, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument3, "barInstrument3");
    this.barInstrument3.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS006_DPF_Outlet_Pressure");
    ((Control) this.barInstrument3).Name = "barInstrument3";
    ((SingleInstrumentBase) this.barInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.button1, "button1");
    this.button1.Name = "button1";
    this.button1.UseCompatibleTextRendering = true;
    this.button1.UseVisualStyleBackColor = true;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.LabelDocFacePlugUnclogging;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.button1;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    this.DocFacePlugUnclogging.Suspend();
    this.DocFacePlugUnclogging.MonitorCall = new ServiceCall("ACM301T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status");
    this.DocFacePlugUnclogging.MonitorGradient.Initialize((ValueState) 0, 6);
    this.DocFacePlugUnclogging.MonitorGradient.Modify(1, 0.0, (ValueState) 3);
    this.DocFacePlugUnclogging.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.DocFacePlugUnclogging.MonitorGradient.Modify(3, 2.0, (ValueState) 1);
    this.DocFacePlugUnclogging.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.DocFacePlugUnclogging.MonitorGradient.Modify(5, 4.0, (ValueState) 3);
    this.DocFacePlugUnclogging.MonitorGradient.Modify(6, 5.0, (ValueState) 3);
    componentResourceManager.ApplyResources((object) this.DocFacePlugUnclogging, "DocFacePlugUnclogging");
    this.DocFacePlugUnclogging.Qualifier = "SP_DocFacePlugUnclogging";
    this.DocFacePlugUnclogging.StartCall = new ServiceCall("ACM301T", "RT_DOC_Face_Plug_Unclogging_Start");
    dataItemCondition1.Gradient.Initialize((ValueState) 0, 4);
    dataItemCondition1.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition1.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
    dataItemCondition2.Gradient.Initialize((ValueState) 3, 1, "rpm");
    dataItemCondition2.Gradient.Modify(1, 550.0, (ValueState) 1);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    this.DocFacePlugUnclogging.StartConditions.Add(dataItemCondition1);
    this.DocFacePlugUnclogging.StartConditions.Add(dataItemCondition2);
    this.DocFacePlugUnclogging.StopCall = new ServiceCall("ACM301T", "RT_DOC_Face_Plug_Unclogging_Stop");
    this.DocFacePlugUnclogging.Resume();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelVerticalInstruments, "tableLayoutPanelVerticalInstruments");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelVerticalInstruments, 6);
    ((TableLayoutPanel) this.tableLayoutPanelVerticalInstruments).Controls.Add((Control) this.barInstrument5, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanelVerticalInstruments).Controls.Add((Control) this.barInstrument6, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelVerticalInstruments).Controls.Add((Control) this.barInstrument8, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelVerticalInstruments).Controls.Add((Control) this.barInstrument4, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelVerticalInstruments).Controls.Add((Control) this.barInstrumentDefPressure, 0, 0);
    ((Control) this.tableLayoutPanelVerticalInstruments).Name = "tableLayoutPanelVerticalInstruments";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanelVerticalInstruments, 3);
    this.barInstrumentDefPressure.BarOrientation = (BarControl.ControlOrientation) 1;
    componentResourceManager.ApplyResources((object) this.barInstrumentDefPressure, "barInstrumentDefPressure");
    this.barInstrumentDefPressure.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrumentDefPressure).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrumentDefPressure).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS014_DEF_Pressure");
    ((Control) this.barInstrumentDefPressure).Name = "barInstrumentDefPressure";
    ((SingleInstrumentBase) this.barInstrumentDefPressure).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrumentDefPressure).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrumentDefPressure).UnitAlignment = StringAlignment.Near;
    this.barInstrument4.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument4.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument4, "barInstrument4");
    this.barInstrument4.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS007_DOC_Inlet_Temperature");
    ((Control) this.barInstrument4).Name = "barInstrument4";
    ((SingleInstrumentBase) this.barInstrument4).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument4).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument4).UnitAlignment = StringAlignment.Near;
    this.barInstrument8.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument8.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument8, "barInstrument8");
    this.barInstrument8.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS019_SCR_Outlet_Temperature");
    ((Control) this.barInstrument8).Name = "barInstrument8";
    ((SingleInstrumentBase) this.barInstrument8).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument8).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument8).UnitAlignment = StringAlignment.Near;
    this.barInstrument6.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument6.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument6, "barInstrument6");
    this.barInstrument6.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS009_DPF_Outlet_Temperature");
    ((Control) this.barInstrument6).Name = "barInstrument6";
    ((SingleInstrumentBase) this.barInstrument6).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument6).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument6).UnitAlignment = StringAlignment.Near;
    this.barInstrument5.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument5.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument5, "barInstrument5");
    this.barInstrument5.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "ACM301T", "DT_AS008_DOC_Outlet_Temperature");
    ((Control) this.barInstrument5).Name = "barInstrument5";
    ((SingleInstrumentBase) this.barInstrument5).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument5).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_DOCFacePlugCleaning");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanelVerticalInstruments).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
