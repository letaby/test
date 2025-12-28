// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Deaeration_Battery__EMG_.panel.UserPanel
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Deaeration_Battery__EMG_.panel;

public class UserPanel : CustomPanel
{
  private const string BatteryPumpCountParameterQualifier = "ethm_p_BattCirc_WtrPmpNum_u8";
  private const string DeaerationValveTimeParameterQualifier = "ethm_p_BattCirc_DeaerValveTime_u16";
  private Channel ecpcChannel;
  private int runTimeSec = 0;
  private bool running = false;
  private bool timespanAdjusted = false;
  private Parameter batteryPumpCountParameter;
  private Parameter deaerationValveTimeParameter;
  private TableLayoutPanel tableLayoutPanelMain;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponent1;
  private Button buttonStart;
  private SharedProcedureSelection sharedProcedureSelection1;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private System.Windows.Forms.Label labelInterlockWarning;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private SeekTimeListView seekTimeListView;
  private TableLayoutPanel tableLayoutPanel1;
  private Checkmark checkmarkSpStatusLabel;
  private ProgressBar progressBarProgress;
  private System.Windows.Forms.Label labelProgress;
  private Button buttonClose;
  private System.Windows.Forms.Label labelSpStatusLabel;
  private System.Windows.Forms.Label label1;
  private RunServicesButton runServicesButtonReturnControl;
  private System.Windows.Forms.Label label41;
  private TableLayoutPanel tableLayoutPanelInstruments;
  private ChartInstrument chartInstrument1;
  private TableLayoutPanel tableLayoutPanel4;
  private DigitalReadoutInstrument digitalReadoutInstrumentCharging;
  private System.Windows.Forms.Label label2;
  private System.Windows.Forms.Label label39;

  private int EstimatedRunTimeSec
  {
    get
    {
      return this.ecpcChannel != null && this.deaerationValveTimeParameter != null && this.deaerationValveTimeParameter.Value != null && (double) this.deaerationValveTimeParameter.Value > 0.0 ? (int) ((double) this.deaerationValveTimeParameter.Value * 3.0) : 840;
    }
  }

  private int BatteryWaterPumpCount
  {
    get
    {
      return this.ecpcChannel != null && this.batteryPumpCountParameter != null && this.batteryPumpCountParameter.Value != null && (double) this.batteryPumpCountParameter.Value == 2.0 ? 2 : 1;
    }
  }

  private bool EcpcOnline
  {
    get
    {
      return this.ecpcChannel != null && this.ecpcChannel.CommunicationsState == CommunicationsState.Online;
    }
  }

  private bool CanStart
  {
    get
    {
      return (this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 || this.digitalReadoutInstrumentParkBrake.RepresentedState == 1) && this.digitalReadoutInstrumentCharging.RepresentedState == 1;
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

  public UserPanel()
  {
    this.InitializeComponent();
    this.UpdateUI();
  }

  public virtual void OnChannelsChanged() => this.SetECPC01TChannel("ECPC01T");

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.running;
    if (e.Cancel)
      return;
    if (this.ecpcChannel != null)
    {
      this.ecpcChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.ecpcChannel_CommunicationsStateUpdateEvent);
      this.ecpcChannel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
    }
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void AddLogLabel(string text)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void UpdateUI()
  {
    if (this.BatteryWaterPumpCount == 1 && ((Collection<Qualifier>) this.chartInstrument1.Instruments).Count == 3)
      ((Collection<Qualifier>) this.chartInstrument1.Instruments).Remove(new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br2"));
    else if (this.BatteryWaterPumpCount == 2 && ((Collection<Qualifier>) this.chartInstrument1.Instruments).Count == 2)
      ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br2"));
    this.checkmarkSpStatusLabel.Checked = this.CanStart;
    ((Control) this.runServicesButtonReturnControl).Enabled = !this.running && this.CanStart && this.EcpcOnline;
    this.buttonStart.Enabled = this.CanStart && this.EcpcOnline;
    this.labelInterlockWarning.Visible = !this.CanStart;
    if (!this.CanStart)
      this.labelSpStatusLabel.Text = Resources.Message_TheParkBrakeMustBeSetOrTheVehicleSpeedMustBe0;
    else if (this.labelSpStatusLabel.Text == Resources.Message_TheParkBrakeMustBeSetOrTheVehicleSpeedMustBe0)
      this.labelSpStatusLabel.Text = Resources.Message_Ready;
    if (!this.timespanAdjusted && this.ecpcChannel != null && this.deaerationValveTimeParameter != null)
    {
      this.chartInstrument1.SpanOfView = new TimeSpan(0, 0, this.EstimatedRunTimeSec + 30);
      this.timespanAdjusted = true;
    }
    this.progressBarProgress.Visible = this.labelProgress.Visible = this.running;
    this.buttonClose.Enabled = !this.running;
  }

  private void SetECPC01TChannel(string ecuName)
  {
    Channel channel = this.GetChannel(ecuName, (CustomPanel.ChannelLookupOptions) 3);
    if (this.ecpcChannel != channel)
    {
      this.deaerationValveTimeParameter = this.batteryPumpCountParameter = (Parameter) null;
      if (this.ecpcChannel != null)
      {
        if (this.batteryPumpCountParameter != null)
        {
          this.batteryPumpCountParameter.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.batteryPumpCountParameter_ParameterUpdateEvent);
          this.batteryPumpCountParameter = (Parameter) null;
        }
        if (this.deaerationValveTimeParameter != null)
        {
          this.deaerationValveTimeParameter.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.deaerationValveTimeParameter_ParameterUpdateEvent);
          this.deaerationValveTimeParameter = (Parameter) null;
        }
        this.ecpcChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.ecpcChannel_CommunicationsStateUpdateEvent);
        this.ecpcChannel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
        this.running = false;
      }
      this.ecpcChannel = channel;
      if (this.ecpcChannel != null)
      {
        this.batteryPumpCountParameter = this.ecpcChannel.Parameters["ethm_p_BattCirc_WtrPmpNum_u8"];
        if (this.batteryPumpCountParameter != null)
          this.batteryPumpCountParameter.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.batteryPumpCountParameter_ParameterUpdateEvent);
        this.deaerationValveTimeParameter = this.ecpcChannel.Parameters["ethm_p_BattCirc_DeaerValveTime_u16"];
        if (this.deaerationValveTimeParameter != null)
          this.deaerationValveTimeParameter.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.deaerationValveTimeParameter_ParameterUpdateEvent);
        this.ecpcChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.ecpcChannel_CommunicationsStateUpdateEvent);
        this.ecpcChannel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
        this.ReadParameters();
        this.running = false;
      }
    }
    this.UpdateUI();
  }

  private void batteryPumpCountParameter_ParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateUI();
  }

  private void deaerationValveTimeParameter_ParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateUI();
  }

  private void ReadParameters()
  {
    if (this.EcpcOnline && this.batteryPumpCountParameter != null && !this.batteryPumpCountParameter.HasBeenReadFromEcu)
      this.ecpcChannel.Parameters.ReadGroup(this.batteryPumpCountParameter.GroupQualifier, false, false);
    if (this.EcpcOnline && this.deaerationValveTimeParameter != null && !this.deaerationValveTimeParameter.HasBeenReadFromEcu)
      this.ecpcChannel.Parameters.ReadGroup(this.deaerationValveTimeParameter.GroupQualifier, false, false);
    this.UpdateUI();
  }

  private void ecpcChannel_CommunicationsStateUpdateEvent(
    object sender,
    CommunicationsStateEventArgs e)
  {
    this.ReadParameters();
  }

  private void sharedProcedureCreatorComponent1_MonitorServiceComplete(
    object sender,
    MonitorServiceResultEventArgs e)
  {
    this.runTimeSec += this.sharedProcedureCreatorComponent1.MonitorInterval / 1000;
    int num = this.runTimeSec * 100 / this.EstimatedRunTimeSec;
    if (this.progressBarProgress.Value != num)
      this.AddLogLabel(num.ToString() + Resources.Message_Complete);
    this.progressBarProgress.Value = num > this.progressBarProgress.Maximum ? this.progressBarProgress.Maximum : num;
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponent1_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.runTimeSec = 0;
    this.progressBarProgress.Value = 0;
    this.running = ((ResultEventArgs) e).Succeeded;
    if (this.running)
    {
      this.AddLogLabel(Resources.Message_DeAirationStarted);
      this.AddLogLabel(Resources.Message_EstimatedRuntime + (object) (this.EstimatedRunTimeSec / 60) + Resources.Message_Min);
    }
    else
    {
      this.AddLogLabel(Resources.Message_DeAirationFailedToStart);
      this.AddLogLabel(((ResultEventArgs) e).Exception.Message);
    }
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponent1_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.running = false;
    this.AddLogLabel(Resources.Message_DeAirationStopped);
    this.UpdateUI();
  }

  private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentParkBrake_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentCharging_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.label41 = new System.Windows.Forms.Label();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.runServicesButtonReturnControl = new RunServicesButton();
    this.label1 = new System.Windows.Forms.Label();
    this.checkmarkSpStatusLabel = new Checkmark();
    this.labelSpStatusLabel = new System.Windows.Forms.Label();
    this.progressBarProgress = new ProgressBar();
    this.labelProgress = new System.Windows.Forms.Label();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.buttonStart = new Button();
    this.buttonClose = new Button();
    this.tableLayoutPanelInstruments = new TableLayoutPanel();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.digitalReadoutInstrumentCharging = new DigitalReadoutInstrument();
    this.label2 = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.labelInterlockWarning = new System.Windows.Forms.Label();
    this.label39 = new System.Windows.Forms.Label();
    this.chartInstrument1 = new ChartInstrument();
    this.seekTimeListView = new SeekTimeListView();
    this.sharedProcedureCreatorComponent1 = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((ISupportInitialize) this.runServicesButtonReturnControl).BeginInit();
    ((Control) this.tableLayoutPanelInstruments).SuspendLayout();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.label41, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanel1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelInstruments, 0, 1);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.label41, "label41");
    this.label41.Name = "label41";
    this.label41.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServicesButtonReturnControl, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkSpStatusLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelSpStatusLabel, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.progressBarProgress, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelProgress, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStart, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 4, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.runServicesButtonReturnControl, 2);
    componentResourceManager.ApplyResources((object) this.runServicesButtonReturnControl, "runServicesButtonReturnControl");
    ((Control) this.runServicesButtonReturnControl).Name = "runServicesButtonReturnControl";
    this.runServicesButtonReturnControl.ServiceCalls.Add(new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery1_Circuit_Res", (IEnumerable<string>) new string[2]
    {
      "WaterPump_Speed_Battery1_Circuit_Req=0",
      "WaterPump_Speed_Battery2_Circuit_Req=0"
    }));
    this.runServicesButtonReturnControl.ServiceCalls.Add(new ServiceCall("ECPC01T", "RT_OTF_ETHM_BCWaterPumpCtrl_Stop_WaterPump_Speed_Battery2_Circuit_Res", (IEnumerable<string>) new string[2]
    {
      "WaterPump_Speed_Battery1_Circuit_Req=0",
      "WaterPump_Speed_Battery2_Circuit_Req=0"
    }));
    this.runServicesButtonReturnControl.ServiceCalls.Add(new ServiceCall("ECPC01T", "IOC_IOC_ETHM_Shutoff_ValveControl_Return_Control"));
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkSpStatusLabel, "checkmarkSpStatusLabel");
    ((Control) this.checkmarkSpStatusLabel).Name = "checkmarkSpStatusLabel";
    componentResourceManager.ApplyResources((object) this.labelSpStatusLabel, "labelSpStatusLabel");
    this.labelSpStatusLabel.Name = "labelSpStatusLabel";
    this.labelSpStatusLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.progressBarProgress, "progressBarProgress");
    this.progressBarProgress.Name = "progressBarProgress";
    componentResourceManager.ApplyResources((object) this.labelProgress, "labelProgress");
    this.labelProgress.Name = "labelProgress";
    this.labelProgress.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "ETHM_BatteryCircuitDeaerationCtrl"
    });
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.tableLayoutPanel4, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.chartInstrument1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.seekTimeListView, 1, 1);
    ((Control) this.tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentCharging, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.label2, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelInterlockWarning, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.label39, 0, 2);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).SetRowSpan((Control) this.tableLayoutPanel4, 2);
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
    this.digitalReadoutInstrumentCharging.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentCharging_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
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
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged);
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
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentParkBrake_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.labelInterlockWarning, "labelInterlockWarning");
    this.labelInterlockWarning.ForeColor = Color.Red;
    this.labelInterlockWarning.Name = "labelInterlockWarning";
    this.labelInterlockWarning.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label39, "label39");
    this.label39.Name = "label39";
    this.label39.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.chartInstrument1, "chartInstrument1");
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br1"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_Current_value_percentage_water_pump_Br2"));
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS236_BattCircValveActPos_BattCircValveActPos"));
    ((Control) this.chartInstrument1).Name = "chartInstrument1";
    this.chartInstrument1.SelectedTime = new DateTime?();
    this.chartInstrument1.ShowButtonPanel = false;
    this.chartInstrument1.ShowEvents = false;
    this.chartInstrument1.ShowLabels = false;
    this.chartInstrument1.SpanOfView = TimeSpan.Parse("00:13:00");
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "De-AirationBattery";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.sharedProcedureCreatorComponent1.Suspend();
    this.sharedProcedureCreatorComponent1.AllowStopAlways = true;
    this.sharedProcedureCreatorComponent1.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Request_Results_ETHM_Battery_Circuit_Deaeration");
    this.sharedProcedureCreatorComponent1.MonitorGradient.Initialize((ValueState) 3, 6);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(1, 0.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(3, 2.0, (ValueState) 1);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(5, 4.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent1.MonitorGradient.Modify(6, (double) byte.MaxValue, (ValueState) 3);
    this.sharedProcedureCreatorComponent1.MonitorInterval = 5000;
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponent1, "sharedProcedureCreatorComponent1");
    this.sharedProcedureCreatorComponent1.Qualifier = "ETHM_BatteryCircuitDeaerationCtrl";
    this.sharedProcedureCreatorComponent1.StartCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Start_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>) new string[1]
    {
      "Battery_Circuit_Deaeration_contro_start=1"
    });
    this.sharedProcedureCreatorComponent1.StopCall = new ServiceCall("ECPC01T", "RT_OTF_ETHM_BatteryCircuitDeaerationCtrl_Stop_ETHM_Battery_Circuit_Deaeration", (IEnumerable<string>) new string[1]
    {
      "Battery_Circuit_Deaeration_control_start=0"
    });
    this.sharedProcedureCreatorComponent1.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent1_StartServiceComplete);
    this.sharedProcedureCreatorComponent1.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent1_StopServiceComplete);
    this.sharedProcedureCreatorComponent1.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponent1_MonitorServiceComplete);
    this.sharedProcedureCreatorComponent1.Resume();
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.labelSpStatusLabel;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmarkSpStatusLabel;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = this.buttonStart;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_De-Aeration_Battery");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((ISupportInitialize) this.runServicesButtonReturnControl).EndInit();
    ((Control) this.tableLayoutPanelInstruments).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
