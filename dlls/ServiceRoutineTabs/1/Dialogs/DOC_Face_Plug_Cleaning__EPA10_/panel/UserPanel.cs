// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning__EPA10_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Face_Plug_Cleaning__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private TableLayoutPanel tableLayoutPanel1;
  private BarInstrument barInstrument3;
  private BarInstrument barInstrument4;
  private BarInstrument barInstrument5;
  private BarInstrument barInstrument6;
  private BarInstrument barInstrument7;
  private BarInstrument barInstrument8;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private BarInstrument barInstrument9;
  private BarInstrument barInstrument1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private SharedProcedureCreatorComponent DocFacePlugUncloggingEPA10;
  private Button button1;
  private SharedProcedureSelection sharedProcedureSelection1;
  private Label LabelDocFacePlugUnclogging;
  private DialInstrument dialInstrument1;
  private BarInstrument barInstrument2;
  private SeekTimeListView DocFacePlugUncloggingResults;
  private Checkmark checkmark1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument dpfRegenSwitchStatus;
  private Label commandLabel;
  private DigitalReadoutInstrument digitalReadoutInstrumentResults;
  private DigitalReadoutInstrument ActualDpfZone;

  public UserPanel()
  {
    this.InitializeComponent();
    this.ActualDpfZone.RepresentedStateChanged += new EventHandler(this.ActualDpfZone_RepresentedStateChanged);
    this.dpfRegenSwitchStatus.RepresentedStateChanged += new EventHandler(this.dpfRegenSwitchStatus_RepresentedStateChanged);
    this.sharedProcedureIntegrationComponent1.StartStopButton.Click += new EventHandler(this.StartStopButton_Click);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
  }

  private void StartStopButton_Click(object sender, EventArgs e) => this.commandLabel.Text = "";

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

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.dialInstrument1 = new DialInstrument();
    this.barInstrument9 = new BarInstrument();
    this.barInstrument1 = new BarInstrument();
    this.barInstrument2 = new BarInstrument();
    this.barInstrument3 = new BarInstrument();
    this.barInstrument4 = new BarInstrument();
    this.barInstrument5 = new BarInstrument();
    this.barInstrument6 = new BarInstrument();
    this.barInstrument7 = new BarInstrument();
    this.barInstrument8 = new BarInstrument();
    this.button1 = new Button();
    this.LabelDocFacePlugUnclogging = new Label();
    this.DocFacePlugUncloggingResults = new SeekTimeListView();
    this.ActualDpfZone = new DigitalReadoutInstrument();
    this.dpfRegenSwitchStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentResults = new DigitalReadoutInstrument();
    this.checkmark1 = new Checkmark();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.commandLabel = new Label();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    this.DocFacePlugUncloggingEPA10 = new SharedProcedureCreatorComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dialInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument9, 7, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument1, 8, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument2, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument3, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument4, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument5, 5, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument6, 6, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument7, 7, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrument8, 8, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.button1, 8, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.LabelDocFacePlugUnclogging, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DocFacePlugUncloggingResults, 3, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.ActualDpfZone, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dpfRegenSwitchStatus, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 5, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 5, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 5, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentResults, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmark1, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection1, 2, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.commandLabel, 1, 7);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.dialInstrument1.AngleRange = 135.0;
    this.dialInstrument1.AngleStart = -180.0;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.dialInstrument1, 4);
    componentResourceManager.ApplyResources((object) this.dialInstrument1, "dialInstrument1");
    this.dialInstrument1.FontGroup = "engineSpeed";
    ((SingleInstrumentBase) this.dialInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.dialInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.dialInstrument1).Name = "dialInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.dialInstrument1, 3);
    ((SingleInstrumentBase) this.dialInstrument1).UnitAlignment = StringAlignment.Near;
    this.barInstrument9.BarOrientation = (BarControl.ControlOrientation) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument9, "barInstrument9");
    this.barInstrument9.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS033_Throttle_Valve_Commanded_Value");
    ((Control) this.barInstrument9).Name = "barInstrument9";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.barInstrument9, 3);
    ((SingleInstrumentBase) this.barInstrument9).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument9).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument9).UnitAlignment = StringAlignment.Near;
    this.barInstrument1.BarOrientation = (BarControl.ControlOrientation) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument1, "barInstrument1");
    this.barInstrument1.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS034_Throttle_Valve_Actual_Position");
    ((Control) this.barInstrument1).Name = "barInstrument1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.barInstrument1, 3);
    ((SingleInstrumentBase) this.barInstrument1).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument1).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument1).UnitAlignment = StringAlignment.Near;
    this.barInstrument2.BarOrientation = (BarControl.ControlOrientation) 1;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.barInstrument2, "barInstrument2");
    this.barInstrument2.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS005_DOC_Inlet_Pressure");
    ((Control) this.barInstrument2).Name = "barInstrument2";
    ((SingleInstrumentBase) this.barInstrument2).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument2).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument2).UnitAlignment = StringAlignment.Near;
    this.barInstrument3.BarOrientation = (BarControl.ControlOrientation) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument3, "barInstrument3");
    this.barInstrument3.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS006_DPF_Outlet_Pressure");
    ((Control) this.barInstrument3).Name = "barInstrument3";
    ((SingleInstrumentBase) this.barInstrument3).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument3).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument3).UnitAlignment = StringAlignment.Near;
    this.barInstrument4.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument4.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument4, "barInstrument4");
    this.barInstrument4.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS007_DOC_Inlet_Temperature");
    ((Control) this.barInstrument4).Name = "barInstrument4";
    ((SingleInstrumentBase) this.barInstrument4).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument4).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument4).UnitAlignment = StringAlignment.Near;
    this.barInstrument5.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument5.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument5, "barInstrument5");
    this.barInstrument5.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument5).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS008_DOC_Outlet_Temperature");
    ((Control) this.barInstrument5).Name = "barInstrument5";
    ((SingleInstrumentBase) this.barInstrument5).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument5).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument5).UnitAlignment = StringAlignment.Near;
    this.barInstrument6.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument6.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument6, "barInstrument6");
    this.barInstrument6.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS009_DPF_Outlet_Temperature");
    ((Control) this.barInstrument6).Name = "barInstrument6";
    ((SingleInstrumentBase) this.barInstrument6).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument6).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument6).UnitAlignment = StringAlignment.Near;
    this.barInstrument7.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument7.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument7, "barInstrument7");
    this.barInstrument7.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS018_SCR_Inlet_Temperature");
    ((Control) this.barInstrument7).Name = "barInstrument7";
    ((SingleInstrumentBase) this.barInstrument7).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument7).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument7).UnitAlignment = StringAlignment.Near;
    this.barInstrument8.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barInstrument8.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barInstrument8, "barInstrument8");
    this.barInstrument8.FontGroup = "Temp";
    ((SingleInstrumentBase) this.barInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.barInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS019_SCR_Outlet_Temperature");
    ((Control) this.barInstrument8).Name = "barInstrument8";
    ((SingleInstrumentBase) this.barInstrument8).TitleOrientation = (Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barInstrument8).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barInstrument8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.button1, "button1");
    this.button1.Name = "button1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.button1, 2);
    this.button1.UseCompatibleTextRendering = true;
    this.button1.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.LabelDocFacePlugUnclogging, 6);
    componentResourceManager.ApplyResources((object) this.LabelDocFacePlugUnclogging, "LabelDocFacePlugUnclogging");
    this.LabelDocFacePlugUnclogging.Name = "LabelDocFacePlugUnclogging";
    this.LabelDocFacePlugUnclogging.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.DocFacePlugUncloggingResults, 6);
    componentResourceManager.ApplyResources((object) this.DocFacePlugUncloggingResults, "DocFacePlugUncloggingResults");
    this.DocFacePlugUncloggingResults.FilterUserLabels = true;
    ((Control) this.DocFacePlugUncloggingResults).Name = "DocFacePlugUncloggingResults";
    this.DocFacePlugUncloggingResults.RequiredUserLabelPrefix = "DOC Face Plug Cleaning EPA10";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.DocFacePlugUncloggingResults, 3);
    this.DocFacePlugUncloggingResults.SelectedTime = new DateTime?();
    this.DocFacePlugUncloggingResults.ShowChannelLabels = false;
    this.DocFacePlugUncloggingResults.ShowCommunicationsState = false;
    this.DocFacePlugUncloggingResults.ShowControlPanel = false;
    this.DocFacePlugUncloggingResults.ShowDeviceColumn = false;
    this.DocFacePlugUncloggingResults.TimeFormat = "HH:mm:ss";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.ActualDpfZone, 3);
    componentResourceManager.ApplyResources((object) this.ActualDpfZone, "ActualDpfZone");
    this.ActualDpfZone.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.ActualDpfZone).FreezeValue = false;
    this.ActualDpfZone.Gradient.Initialize((ValueState) 2, 2);
    this.ActualDpfZone.Gradient.Modify(1, 2.0, (ValueState) 1);
    this.ActualDpfZone.Gradient.Modify(2, 3.0, (ValueState) 2);
    ((SingleInstrumentBase) this.ActualDpfZone).Instrument = new Qualifier((QualifierTypes) 1, "ACM02T", "DT_AS065_Actual_DPF_zone");
    ((Control) this.ActualDpfZone).Name = "ActualDpfZone";
    ((SingleInstrumentBase) this.ActualDpfZone).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.dpfRegenSwitchStatus, 3);
    componentResourceManager.ApplyResources((object) this.dpfRegenSwitchStatus, "dpfRegenSwitchStatus");
    this.dpfRegenSwitchStatus.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.dpfRegenSwitchStatus).FreezeValue = false;
    this.dpfRegenSwitchStatus.Gradient.Initialize((ValueState) 0, 4);
    this.dpfRegenSwitchStatus.Gradient.Modify(1, 0.0, (ValueState) 2);
    this.dpfRegenSwitchStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.dpfRegenSwitchStatus.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.dpfRegenSwitchStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.dpfRegenSwitchStatus).Instrument = new Qualifier((QualifierTypes) 1, "CPC02T", "DT_DSL_DPF_Regen_Switch_Status");
    ((Control) this.dpfRegenSwitchStatus).Name = "dpfRegenSwitchStatus";
    ((SingleInstrumentBase) this.dpfRegenSwitchStatus).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument3, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS069_Jake_Brake_1_PWM13");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_AS094_Actual_Torque_Load");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "DigitalReadOut";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    this.digitalReadoutInstrument1.Gradient.Initialize((ValueState) 3, 4);
    this.digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrument1.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentResults, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentResults, "digitalReadoutInstrumentResults");
    this.digitalReadoutInstrumentResults.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).Instrument = new Qualifier((QualifierTypes) 0, (string) null, (string) null);
    ((Control) this.digitalReadoutInstrumentResults).Name = "digitalReadoutInstrumentResults";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResults).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.checkmark1, 2);
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_DocFacePlugUncloggingEPA10"
    });
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.commandLabel, 6);
    componentResourceManager.ApplyResources((object) this.commandLabel, "commandLabel");
    this.commandLabel.Name = "commandLabel";
    this.commandLabel.UseCompatibleTextRendering = true;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.LabelDocFacePlugUnclogging;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.button1;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    this.DocFacePlugUncloggingEPA10.Suspend();
    this.DocFacePlugUncloggingEPA10.MonitorCall = new ServiceCall("ACM02T", "RT_DOC_Face_Plug_Unclogging_Request_Results_Status");
    this.DocFacePlugUncloggingEPA10.MonitorGradient.Initialize((ValueState) 0, 6);
    this.DocFacePlugUncloggingEPA10.MonitorGradient.Modify(1, 0.0, (ValueState) 3);
    this.DocFacePlugUncloggingEPA10.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.DocFacePlugUncloggingEPA10.MonitorGradient.Modify(3, 2.0, (ValueState) 1);
    this.DocFacePlugUncloggingEPA10.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.DocFacePlugUncloggingEPA10.MonitorGradient.Modify(5, 4.0, (ValueState) 3);
    this.DocFacePlugUncloggingEPA10.MonitorGradient.Modify(6, 5.0, (ValueState) 3);
    componentResourceManager.ApplyResources((object) this.DocFacePlugUncloggingEPA10, "DocFacePlugUncloggingEPA10");
    this.DocFacePlugUncloggingEPA10.Qualifier = "SP_DocFacePlugUncloggingEPA10";
    this.DocFacePlugUncloggingEPA10.StartCall = new ServiceCall("ACM02T", "RT_DOC_Face_Plug_Unclogging_Start");
    dataItemCondition1.Gradient.Initialize((ValueState) 0, 4);
    dataItemCondition1.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition1.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS019_Vehicle_Check_Status");
    dataItemCondition2.Gradient.Initialize((ValueState) 3, 1, "rpm");
    dataItemCondition2.Gradient.Modify(1, 550.0, (ValueState) 1);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    this.DocFacePlugUncloggingEPA10.StartConditions.Add(dataItemCondition1);
    this.DocFacePlugUncloggingEPA10.StartConditions.Add(dataItemCondition2);
    this.DocFacePlugUncloggingEPA10.StopCall = new ServiceCall("ACM02T", "RT_DOC_Face_Plug_Unclogging_Stop");
    this.DocFacePlugUncloggingEPA10.Resume();
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_DOCFacePlugCleaning");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
