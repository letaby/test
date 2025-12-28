// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Oil_Pump_Controls__EMG_.panel.UserPanel
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Oil_Pump_Controls__EMG_.panel;

public class UserPanel : CustomPanel
{
  private const string PtfConfPTransGroup = "VCD_PID_222_ptconf_p_Trans";
  private const string PtfConfPTransEmotNum = "ptconf_p_Trans_EmotNum_u8";
  private Channel eCpcChannel;
  private bool isEmot3Num = false;
  private bool oilPump1Started = false;
  private bool oilPump2Started = false;
  private Parameter emotNumParameter = (Parameter) null;
  private SelectablePanel selectablePanel1;
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonClose;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentOilPump1;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentOilPump1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentOilPump2;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentOilPump2;
  private Button buttonStartOilPump2;
  private System.Windows.Forms.Label labelStatusLabelOilPump2;
  private System.Windows.Forms.Label labelStatus2;
  private Checkmark checkmarkOilPump2;
  private System.Windows.Forms.Label label1;
  private SeekTimeListView seekTimeListView1;
  private TableLayoutPanel tableLayoutPanel4;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelOilPumpHeader1;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private ScalingLabel scalingLabelOilPump2;
  private SharedProcedureSelection sharedProcedureSelectionOilPump1;
  private Button buttonStartOilPump1;
  private Checkmark checkmarkOilPump1;
  private System.Windows.Forms.Label labelStatus1;
  private System.Windows.Forms.Label labelStatusLabelOilPump1;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelOilPumpHeader2;
  private SharedProcedureSelection sharedProcedureSelectionOilPump2;
  private ScalingLabel scalingLabelOilPump1;

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.oilPump1Started = false;
    this.oilPump2Started = false;
    this.UpdateUI();
  }

  public UserPanel() => this.InitializeComponent();

  public virtual void OnChannelsChanged()
  {
    this.SetECPC01TChannel(this.GetChannel("ECPC01T", (CustomPanel.ChannelLookupOptions) 3));
  }

  private bool EcpcOnline
  {
    get
    {
      return this.eCpcChannel != null && this.eCpcChannel.CommunicationsState == CommunicationsState.Online;
    }
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.oilPump1Started || this.oilPump2Started;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    if (this.eCpcChannel != null)
      this.eCpcChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
  }

  private void SetECPC01TChannel(Channel eCpc)
  {
    if (this.eCpcChannel != eCpc)
    {
      if (this.eCpcChannel != null)
        this.eCpcChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.eCpcChannel = eCpc;
      if (this.eCpcChannel != null)
      {
        this.oilPump1Started = false;
        this.oilPump2Started = false;
        this.eCpcChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.emotNumParameter = this.eCpcChannel.Parameters["ptconf_p_Trans_EmotNum_u8"];
        if (this.EcpcOnline)
          this.ReadInitialParameters();
      }
    }
    this.UpdateUI();
  }

  private void ReadInitialParameters()
  {
    if (this.eCpcChannel.CommunicationsState != CommunicationsState.Online || this.eCpcChannel.Parameters == null || this.emotNumParameter.HasBeenReadFromEcu)
      return;
    this.eCpcChannel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
    this.eCpcChannel.Parameters.ReadGroup(this.emotNumParameter.GroupQualifier, true, false);
  }

  private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.eCpcChannel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
    this.UpdateUI();
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.ReadInitialParameters();
  }

  private void AddLogLabel(string text)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void UpdateUI()
  {
    int result = 1;
    if (this.emotNumParameter != null && this.emotNumParameter.Value != null)
      result = int.TryParse(this.emotNumParameter.Value.ToString(), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : 3;
    this.isEmot3Num = result == 3;
    this.sharedProcedureIntegrationComponentOilPump2.ProceduresDropDown = this.isEmot3Num ? this.sharedProcedureSelectionOilPump2 : (SharedProcedureSelection) null;
    ((Control) this.labelOilPumpHeader2).Visible = this.isEmot3Num;
    ((Control) this.scalingLabelOilPump2).Visible = this.isEmot3Num;
    ((Control) this.checkmarkOilPump2).Visible = this.isEmot3Num;
    this.labelStatus2.Visible = this.isEmot3Num;
    this.labelStatusLabelOilPump2.Visible = this.isEmot3Num;
    this.buttonStartOilPump2.Visible = this.isEmot3Num;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[9].Height = this.isEmot3Num ? 25f : 0.0f;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[10].Height = this.isEmot3Num ? 20f : 0.0f;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[11].Height = this.isEmot3Num ? 20f : 0.0f;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[12].Height = this.isEmot3Num ? 20f : 0.0f;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[13].Height = this.isEmot3Num ? 20f : 0.0f;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[14].Height = this.isEmot3Num ? 20f : 0.0f;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[15].Height = this.isEmot3Num ? 39f : 0.0f;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[16 /*0x10*/].Height = this.isEmot3Num ? 8f : 0.0f;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[17].Height = this.isEmot3Num ? 32f : 0.0f;
    this.buttonClose.Enabled = this.isEmot3Num ? !this.oilPump1Started && !this.oilPump2Started : !this.oilPump1Started;
  }

  private void sharedProcedureCreatorComponentOilPump1_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      this.oilPump1Started = true;
      ((Control) this.scalingLabelOilPump1).Text = "0.0";
      this.AddLogLabel(Resources.Message_OilPump1_Started);
    }
    else
    {
      this.AddLogLabel(Resources.Message_OilPump1_FailedToStart);
      this.AddLogLabel(((ResultEventArgs) e).Exception.Message);
    }
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentOilPump1_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.oilPump1Started = false;
    ((Control) this.scalingLabelOilPump1).Text = "0.0";
    this.AddLogLabel(Resources.Message_OilPump1_Stopped);
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentOilPump1_MonitorServiceComplete(
    object sender,
    MonitorServiceResultEventArgs e)
  {
    if (e.Service.OutputValues.Count<ServiceOutputValue>() <= 0)
      return;
    ((Control) this.scalingLabelOilPump1).Text = e.Service.OutputValues[0].Value.ToString();
  }

  private void sharedProcedureCreatorComponentOilPump2_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
    {
      this.oilPump2Started = true;
      ((Control) this.scalingLabelOilPump2).Text = "0.0";
      this.AddLogLabel(Resources.Message_OilPump2_Started);
    }
    else
    {
      this.AddLogLabel(Resources.Message_OilPump2_FailedToStart);
      this.AddLogLabel(((ResultEventArgs) e).Exception.Message);
    }
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentOilPump2_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.oilPump2Started = false;
    ((Control) this.scalingLabelOilPump2).Text = "0.0";
    this.AddLogLabel(Resources.Message_OilPump2_Stopped);
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentOilPump2_MonitorServiceComplete(
    object sender,
    MonitorServiceResultEventArgs e)
  {
    if (e.Service.OutputValues.Count<ServiceOutputValue>() <= 0)
      return;
    ((Control) this.scalingLabelOilPump2).Text = e.Service.OutputValues[0].Value.ToString();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    DataItemCondition dataItemCondition3 = new DataItemCondition();
    DataItemCondition dataItemCondition4 = new DataItemCondition();
    this.selectablePanel1 = new SelectablePanel();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.sharedProcedureSelectionOilPump1 = new SharedProcedureSelection();
    this.buttonStartOilPump2 = new Button();
    this.buttonStartOilPump1 = new Button();
    this.label1 = new System.Windows.Forms.Label();
    this.seekTimeListView1 = new SeekTimeListView();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.labelOilPumpHeader1 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelOilPump1 = new ScalingLabel();
    this.checkmarkOilPump1 = new Checkmark();
    this.labelStatus1 = new System.Windows.Forms.Label();
    this.labelStatusLabelOilPump1 = new System.Windows.Forms.Label();
    this.labelOilPumpHeader2 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelOilPump2 = new ScalingLabel();
    this.checkmarkOilPump2 = new Checkmark();
    this.labelStatus2 = new System.Windows.Forms.Label();
    this.labelStatusLabelOilPump2 = new System.Windows.Forms.Label();
    this.sharedProcedureSelectionOilPump2 = new SharedProcedureSelection();
    this.buttonClose = new Button();
    this.sharedProcedureIntegrationComponentOilPump1 = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentOilPump1 = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentOilPump2 = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentOilPump2 = new SharedProcedureCreatorComponent(this.components);
    ((Control) this.selectablePanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this).SuspendLayout();
    ((Control) this.selectablePanel1).Controls.Add((Control) this.tableLayoutPanel1);
    componentResourceManager.ApplyResources((object) this.selectablePanel1, "selectablePanel1");
    ((Control) this.selectablePanel1).Name = "selectablePanel1";
    ((Panel) this.selectablePanel1).TabStop = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelectionOilPump1, 5, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStartOilPump2, 5, 15);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStartOilPump1, 5, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 3, 18);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel4, 0, 18);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelOilPumpHeader1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.scalingLabelOilPump1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkOilPump1, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus1, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatusLabelOilPump1, 2, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelOilPumpHeader2, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.scalingLabelOilPump2, 0, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkOilPump2, 0, 15);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus2, 1, 15);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatusLabelOilPump2, 2, 15);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelectionOilPump2, 5, 17);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 5, 20);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionOilPump1, "sharedProcedureSelectionOilPump1");
    ((Control) this.sharedProcedureSelectionOilPump1).Name = "sharedProcedureSelectionOilPump1";
    this.sharedProcedureSelectionOilPump1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "ETHM_OilPumpCtr"
    });
    componentResourceManager.ApplyResources((object) this.buttonStartOilPump2, "buttonStartOilPump2");
    this.buttonStartOilPump2.Name = "buttonStartOilPump2";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.buttonStartOilPump2, 2);
    this.buttonStartOilPump2.UseCompatibleTextRendering = true;
    this.buttonStartOilPump2.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStartOilPump1, "buttonStartOilPump1");
    this.buttonStartOilPump1.Name = "buttonStartOilPump1";
    this.buttonStartOilPump1.UseCompatibleTextRendering = true;
    this.buttonStartOilPump1.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label1, 6);
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "eTransmissionOilPumpControls";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView1, 2);
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel4, 3);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 5);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel4, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((TableLayoutPanel) this.tableLayoutPanel4).SetRowSpan((Control) this.digitalReadoutInstrumentParkBrake, 5);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
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
    ((TableLayoutPanel) this.tableLayoutPanel4).SetRowSpan((Control) this.digitalReadoutInstrumentVehicleSpeed, 5);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.labelOilPumpHeader1.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelOilPumpHeader1, 6);
    componentResourceManager.ApplyResources((object) this.labelOilPumpHeader1, "labelOilPumpHeader1");
    ((Control) this.labelOilPumpHeader1).Name = "labelOilPumpHeader1";
    this.labelOilPumpHeader1.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelOilPump1.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.scalingLabelOilPump1, 6);
    componentResourceManager.ApplyResources((object) this.scalingLabelOilPump1, "scalingLabelOilPump1");
    this.scalingLabelOilPump1.FontGroup = (string) null;
    this.scalingLabelOilPump1.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelOilPump1).Name = "scalingLabelOilPump1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.scalingLabelOilPump1, 5);
    componentResourceManager.ApplyResources((object) this.checkmarkOilPump1, "checkmarkOilPump1");
    ((Control) this.checkmarkOilPump1).Name = "checkmarkOilPump1";
    componentResourceManager.ApplyResources((object) this.labelStatus1, "labelStatus1");
    this.labelStatus1.Name = "labelStatus1";
    this.labelStatus1.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelStatusLabelOilPump1, 3);
    componentResourceManager.ApplyResources((object) this.labelStatusLabelOilPump1, "labelStatusLabelOilPump1");
    this.labelStatusLabelOilPump1.Name = "labelStatusLabelOilPump1";
    this.labelStatusLabelOilPump1.UseCompatibleTextRendering = true;
    this.labelOilPumpHeader2.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelOilPumpHeader2, 6);
    componentResourceManager.ApplyResources((object) this.labelOilPumpHeader2, "labelOilPumpHeader2");
    ((Control) this.labelOilPumpHeader2).Name = "labelOilPumpHeader2";
    this.labelOilPumpHeader2.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelOilPump2.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.scalingLabelOilPump2, 6);
    componentResourceManager.ApplyResources((object) this.scalingLabelOilPump2, "scalingLabelOilPump2");
    this.scalingLabelOilPump2.FontGroup = (string) null;
    this.scalingLabelOilPump2.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelOilPump2).Name = "scalingLabelOilPump2";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.scalingLabelOilPump2, 5);
    componentResourceManager.ApplyResources((object) this.checkmarkOilPump2, "checkmarkOilPump2");
    ((Control) this.checkmarkOilPump2).Name = "checkmarkOilPump2";
    componentResourceManager.ApplyResources((object) this.labelStatus2, "labelStatus2");
    this.labelStatus2.Name = "labelStatus2";
    this.labelStatus2.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelStatusLabelOilPump2, 3);
    componentResourceManager.ApplyResources((object) this.labelStatusLabelOilPump2, "labelStatusLabelOilPump2");
    this.labelStatusLabelOilPump2.Name = "labelStatusLabelOilPump2";
    this.labelStatusLabelOilPump2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionOilPump2, "sharedProcedureSelectionOilPump2");
    ((Control) this.sharedProcedureSelectionOilPump2).Name = "sharedProcedureSelectionOilPump2";
    this.sharedProcedureSelectionOilPump2.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "ETHM_OilPumpCtrl2"
    });
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.sharedProcedureIntegrationComponentOilPump1.ProceduresDropDown = this.sharedProcedureSelectionOilPump1;
    this.sharedProcedureIntegrationComponentOilPump1.ProcedureStatusMessageTarget = this.labelStatusLabelOilPump1;
    this.sharedProcedureIntegrationComponentOilPump1.ProcedureStatusStateTarget = this.checkmarkOilPump1;
    this.sharedProcedureIntegrationComponentOilPump1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentOilPump1.StartStopButton = this.buttonStartOilPump1;
    this.sharedProcedureIntegrationComponentOilPump1.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentOilPump1.Suspend();
    this.sharedProcedureCreatorComponentOilPump1.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump1_Control_results_resp");
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentOilPump1, "sharedProcedureCreatorComponentOilPump1");
    this.sharedProcedureCreatorComponentOilPump1.Qualifier = "ETHM_OilPumpCtr";
    this.sharedProcedureCreatorComponentOilPump1.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump1_Control_resp", (IEnumerable<string>) new string[2]
    {
      "ETHM_Oil_Pump1_Control=100",
      "ETHM_Oil_Pump2_Control=0"
    });
    dataItemCondition1.Gradient.Initialize((ValueState) 3, 5, "mph");
    dataItemCondition1.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition1.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition1.Gradient.Modify(5, (double) int.MaxValue, (ValueState) 3);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
    dataItemCondition2.Gradient.Initialize((ValueState) 0, 4);
    dataItemCondition2.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition2.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition2.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition2.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    this.sharedProcedureCreatorComponentOilPump1.StartConditions.Add(dataItemCondition1);
    this.sharedProcedureCreatorComponentOilPump1.StartConditions.Add(dataItemCondition2);
    this.sharedProcedureCreatorComponentOilPump1.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump1_Control_stop_resp", (IEnumerable<string>) new string[2]
    {
      "ETHM_Oil_Pump1_Control_stop=0",
      "ETHM_Oil_Pump2_Control_stop=0"
    });
    this.sharedProcedureCreatorComponentOilPump1.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentOilPump1_StartServiceComplete);
    this.sharedProcedureCreatorComponentOilPump1.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentOilPump1_StopServiceComplete);
    this.sharedProcedureCreatorComponentOilPump1.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponentOilPump1_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentOilPump1.Resume();
    this.sharedProcedureIntegrationComponentOilPump2.ProceduresDropDown = this.sharedProcedureSelectionOilPump2;
    this.sharedProcedureIntegrationComponentOilPump2.ProcedureStatusMessageTarget = this.labelStatusLabelOilPump2;
    this.sharedProcedureIntegrationComponentOilPump2.ProcedureStatusStateTarget = this.checkmarkOilPump2;
    this.sharedProcedureIntegrationComponentOilPump2.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentOilPump2.StartStopButton = this.buttonStartOilPump2;
    this.sharedProcedureIntegrationComponentOilPump2.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentOilPump2.Suspend();
    this.sharedProcedureCreatorComponentOilPump2.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Request_Results_ETHM_Oil_Pump2_Control_results_resp");
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentOilPump2, "sharedProcedureCreatorComponentOilPump2");
    this.sharedProcedureCreatorComponentOilPump2.Qualifier = "ETHM_OilPumpCtrl2";
    this.sharedProcedureCreatorComponentOilPump2.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Start_ETHM_Oil_Pump2_Control_resp", (IEnumerable<string>) new string[2]
    {
      "ETHM_Oil_Pump1_Control=0",
      "ETHM_Oil_Pump2_Control=100"
    });
    dataItemCondition3.Gradient.Initialize((ValueState) 3, 5, "mph");
    dataItemCondition3.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition3.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition3.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition3.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition3.Gradient.Modify(5, (double) int.MaxValue, (ValueState) 3);
    dataItemCondition3.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS043_PMC_VehStandStill_PMC_VehStandStill");
    dataItemCondition4.Gradient.Initialize((ValueState) 0, 4);
    dataItemCondition4.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition4.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition4.Gradient.Modify(3, 2.0, (ValueState) 3);
    dataItemCondition4.Gradient.Modify(4, 3.0, (ValueState) 3);
    dataItemCondition4.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    this.sharedProcedureCreatorComponentOilPump2.StartConditions.Add(dataItemCondition3);
    this.sharedProcedureCreatorComponentOilPump2.StartConditions.Add(dataItemCondition4);
    this.sharedProcedureCreatorComponentOilPump2.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_OilPumpCtrl_Stop_ETHM_Oil_Pump2_Control_stop_resp", (IEnumerable<string>) new string[2]
    {
      "ETHM_Oil_Pump1_Control_stop=0",
      "ETHM_Oil_Pump2_Control_stop=0"
    });
    this.sharedProcedureCreatorComponentOilPump2.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentOilPump2_StartServiceComplete);
    this.sharedProcedureCreatorComponentOilPump2.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentOilPump2_StopServiceComplete);
    this.sharedProcedureCreatorComponentOilPump2.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponentOilPump2_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentOilPump2.Resume();
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Transmission_Oil_Pump_Controls");
    ((Control) this).Controls.Add((Control) this.selectablePanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.selectablePanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
