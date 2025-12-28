// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.High_Voltage_Measurement__EMG_.panel.UserPanel
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
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.High_Voltage_Measurement__EMG_.panel;

public class UserPanel : CustomPanel
{
  private const string NumberofStringsQualifier = "ptconf_p_Veh_BatNumOfStrings_u8";
  private static int MaxBatteryCount = 9;
  private Channel ecpc01tChannel = (Channel) null;
  private Parameter numberofStringsParameter = (Parameter) null;
  private int previousBatteryCount = 0;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentVoltages;
  private TableLayoutPanel tableLayoutPanelMain;
  private TableLayoutPanel tableLayoutPanelContent;
  private DigitalReadoutInstrument digitalReadoutInstrumentBMS1;
  private DigitalReadoutInstrument digitalReadoutInstrumentBMS2;
  private DigitalReadoutInstrument digitalReadoutInstrumentBMS3;
  private DigitalReadoutInstrument digitalReadoutInstrumentBMS4;
  private DigitalReadoutInstrument digitalReadoutInstrumentBMS5;
  private DigitalReadoutInstrument digitalReadoutInstrumentBMS6;
  private DigitalReadoutInstrument digitalReadoutInstrumentBMS7;
  private DigitalReadoutInstrument digitalReadoutInstrumentBMS8;
  private DigitalReadoutInstrument digitalReadoutInstrumentBMS9;
  private DigitalReadoutInstrument digitalReadoutInstrumentPTIInverter1;
  private DigitalReadoutInstrument digitalReadoutInstrumentPTIInverter2;
  private DigitalReadoutInstrument digitalReadoutInstrumentPTIInverter3;
  private DigitalReadoutInstrument digitalReadoutInstrumentDCLConverter;
  private DigitalReadoutInstrument digitalReadoutInstrumentPTC3;
  private DigitalReadoutInstrument digitalReadoutInstrumentPTC1;
  private DigitalReadoutInstrument digitalReadoutInstrumentPTC2;
  private DigitalReadoutInstrument digitalReadoutInstrumentEAC;
  private DigitalReadoutInstrument digitalReadoutInstrumentERC;
  private TableLayoutPanel tableLayoutPanelStatusComponentVoltages;
  private SharedProcedureSelection sharedProcedureSelectionMeasurement;
  private Button buttonStartStopHVMeasurement;
  private System.Windows.Forms.Label labelVoltagesRoutine;
  private Checkmark checkmarkVoltagesRoutine;
  private TableLayoutPanel tableLayoutPanelTop;
  private PictureBox pictureBoxWarningIcon;
  private WebBrowser webBrowserWarning;
  private DigitalReadoutInstrument digitalReadoutInstrumentPTC4;
  private System.Windows.Forms.Label label1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentVoltages;

  private bool EcpcOnline
  {
    get
    {
      return this.ecpc01tChannel != null && (this.ecpc01tChannel.CommunicationsState == CommunicationsState.Online || this.ecpc01tChannel.CommunicationsState == CommunicationsState.LogFilePlayback);
    }
  }

  private int BatteryCount
  {
    get
    {
      int result = 9;
      if (this.ecpc01tChannel != null && this.numberofStringsParameter != null && this.numberofStringsParameter.HasBeenReadFromEcu && this.numberofStringsParameter.Value != null)
        int.TryParse(this.numberofStringsParameter.Value.ToString(), out result);
      if (result <= 3)
        result = 4;
      if (result > UserPanel.MaxBatteryCount)
        result = UserPanel.MaxBatteryCount;
      return result;
    }
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.previousBatteryCount = this.BatteryCount;
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.webBrowserWarning.DocumentText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<html><style>{0}</style><body><span class='scaled bold red'>{1}</span><span class='scaled bold'>{2}</span><br><span class='scaled'>{3}</span><span class='scaled bold'>{4}</span></body><span class='scaled'>).</span></html>", (object) ("html { height:100%; display: table; } " + "body { margin: 0px; padding: 0px; display: table-cell; vertical-align: middle; } " + ".scaled { font-size: calc(0.33vw + 12.0vh); font-family: Segoe UI; padding: 0px; margin: 0px; }  " + ".bold { font-weight: bold; }" + ".red { color: red; }"), (object) Resources.RedWarning, (object) Resources.BlackWarning, (object) Resources.WarningText, (object) Resources.ReferenceChecklist);
  }

  private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureSelectionMeasurement.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.this_ParentFormClosing);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetECPC(this.GetChannel("ECPC01T", (CustomPanel.ChannelLookupOptions) 3));
    this.UpdateUI();
  }

  private void SetECPC(Channel ecpc01t)
  {
    if (this.ecpc01tChannel == ecpc01t)
      return;
    if (this.ecpc01tChannel != null)
    {
      this.ecpc01tChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.numberofStringsParameter = (Parameter) null;
    }
    this.ecpc01tChannel = ecpc01t;
    if (this.ecpc01tChannel != null)
    {
      this.ecpc01tChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.numberofStringsParameter = this.ecpc01tChannel.Parameters["ptconf_p_Veh_BatNumOfStrings_u8"];
      if (this.EcpcOnline)
        this.ReadInitialParameters();
    }
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    if (this.EcpcOnline)
      this.ReadInitialParameters();
    this.UpdateUI();
  }

  private void ReadInitialParameters()
  {
    if (this.EcpcOnline && this.ecpc01tChannel.CommunicationsState == CommunicationsState.Online && this.ecpc01tChannel.Parameters != null && this.numberofStringsParameter != null && !this.numberofStringsParameter.HasBeenReadFromEcu)
    {
      this.ecpc01tChannel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
      this.ecpc01tChannel.Parameters.ReadGroup(this.numberofStringsParameter.GroupQualifier, false, false);
    }
    this.UpdateUI();
  }

  private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.ecpc01tChannel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
    this.UpdateUI();
  }

  private void UpdateUI()
  {
    if (this.BatteryCount == this.previousBatteryCount)
      return;
    ((Control) this.tableLayoutPanelContent).SuspendLayout();
    if (this.digitalReadoutInstrumentPTIInverter3 != null)
      ((Control) this.digitalReadoutInstrumentPTIInverter3).Visible = this.BatteryCount >= 7;
    for (int index = 0; index < UserPanel.MaxBatteryCount; ++index)
    {
      DigitalReadoutInstrument control = (DigitalReadoutInstrument) ((TableLayoutPanel) this.tableLayoutPanelContent).Controls[$"digitalReadoutInstrumentBMS{index + 1}"];
      if (control != null)
        ((Control) control).Visible = index < this.BatteryCount;
    }
    ((Control) this.tableLayoutPanelContent).ResumeLayout();
    this.previousBatteryCount = this.BatteryCount;
  }

  private void sharedProcedureCreatorComponentVoltages_MonitorServiceComplete(
    object sender,
    MonitorServiceResultEventArgs e)
  {
    e.Service.CombinedService.Execute(false);
  }

  private void sharedProcedureCreatorComponentVoltages_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.digitalReadoutInstrumentBMS1.ShowScalingValue = true;
    this.digitalReadoutInstrumentBMS2.ShowScalingValue = true;
    this.digitalReadoutInstrumentBMS3.ShowScalingValue = true;
    this.digitalReadoutInstrumentBMS4.ShowScalingValue = true;
    this.digitalReadoutInstrumentBMS5.ShowScalingValue = true;
    this.digitalReadoutInstrumentBMS6.ShowScalingValue = true;
    this.digitalReadoutInstrumentBMS7.ShowScalingValue = true;
    this.digitalReadoutInstrumentBMS8.ShowScalingValue = true;
    this.digitalReadoutInstrumentBMS9.ShowScalingValue = true;
    this.digitalReadoutInstrumentPTIInverter1.ShowScalingValue = true;
    this.digitalReadoutInstrumentPTIInverter2.ShowScalingValue = true;
    this.digitalReadoutInstrumentPTIInverter3.ShowScalingValue = true;
    this.digitalReadoutInstrumentDCLConverter.ShowScalingValue = true;
    this.digitalReadoutInstrumentPTC1.ShowScalingValue = true;
    this.digitalReadoutInstrumentPTC2.ShowScalingValue = true;
    this.digitalReadoutInstrumentPTC3.ShowScalingValue = true;
    this.digitalReadoutInstrumentPTC4.ShowScalingValue = true;
    this.digitalReadoutInstrumentEAC.ShowScalingValue = true;
    this.digitalReadoutInstrumentERC.ShowScalingValue = true;
  }

  private void sharedProcedureCreatorComponentVoltages_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.digitalReadoutInstrumentBMS1.ShowScalingValue = false;
    this.digitalReadoutInstrumentBMS2.ShowScalingValue = false;
    this.digitalReadoutInstrumentBMS3.ShowScalingValue = false;
    this.digitalReadoutInstrumentBMS4.ShowScalingValue = false;
    this.digitalReadoutInstrumentBMS5.ShowScalingValue = false;
    this.digitalReadoutInstrumentBMS6.ShowScalingValue = false;
    this.digitalReadoutInstrumentBMS7.ShowScalingValue = false;
    this.digitalReadoutInstrumentBMS8.ShowScalingValue = false;
    this.digitalReadoutInstrumentBMS9.ShowScalingValue = false;
    this.digitalReadoutInstrumentPTIInverter1.ShowScalingValue = false;
    this.digitalReadoutInstrumentPTIInverter2.ShowScalingValue = false;
    this.digitalReadoutInstrumentPTIInverter3.ShowScalingValue = false;
    this.digitalReadoutInstrumentDCLConverter.ShowScalingValue = false;
    this.digitalReadoutInstrumentPTC1.ShowScalingValue = false;
    this.digitalReadoutInstrumentPTC2.ShowScalingValue = false;
    this.digitalReadoutInstrumentPTC3.ShowScalingValue = false;
    this.digitalReadoutInstrumentPTC4.ShowScalingValue = false;
    this.digitalReadoutInstrumentEAC.ShowScalingValue = false;
    this.digitalReadoutInstrumentERC.ShowScalingValue = false;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.sharedProcedureCreatorComponentVoltages = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentVoltages = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureSelectionMeasurement = new SharedProcedureSelection();
    this.labelVoltagesRoutine = new System.Windows.Forms.Label();
    this.checkmarkVoltagesRoutine = new Checkmark();
    this.buttonStartStopHVMeasurement = new Button();
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.tableLayoutPanelContent = new TableLayoutPanel();
    this.digitalReadoutInstrumentPTC4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBMS1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBMS2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBMS3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBMS4 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBMS5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBMS6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBMS7 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBMS8 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBMS9 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentPTIInverter1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentPTIInverter2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentPTIInverter3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentDCLConverter = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentPTC3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentPTC1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentPTC2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEAC = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentERC = new DigitalReadoutInstrument();
    this.tableLayoutPanelStatusComponentVoltages = new TableLayoutPanel();
    this.label1 = new System.Windows.Forms.Label();
    this.tableLayoutPanelTop = new TableLayoutPanel();
    this.pictureBoxWarningIcon = new PictureBox();
    this.webBrowserWarning = new WebBrowser();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelContent).SuspendLayout();
    ((Control) this.tableLayoutPanelStatusComponentVoltages).SuspendLayout();
    ((Control) this.tableLayoutPanelTop).SuspendLayout();
    ((ISupportInitialize) this.pictureBoxWarningIcon).BeginInit();
    ((Control) this).SuspendLayout();
    this.sharedProcedureCreatorComponentVoltages.Suspend();
    this.sharedProcedureCreatorComponentVoltages.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Request_Results_VoltageReadoutStat");
    this.sharedProcedureCreatorComponentVoltages.MonitorGradient.Initialize((ValueState) 0, 6);
    this.sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(1, 0.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(3, 2.0, (ValueState) 1);
    this.sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(5, 4.0, (ValueState) 0);
    this.sharedProcedureCreatorComponentVoltages.MonitorGradient.Modify(6, 15.0, (ValueState) 0);
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentVoltages, "sharedProcedureCreatorComponentVoltages");
    this.sharedProcedureCreatorComponentVoltages.Qualifier = "SP_OTF_Readout";
    this.sharedProcedureCreatorComponentVoltages.StartCall = new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Start");
    this.sharedProcedureCreatorComponentVoltages.StopCall = new ServiceCall("ECPC01T", "RT_OTF_HV_Readout_Stop");
    this.sharedProcedureCreatorComponentVoltages.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentVoltages_StartServiceComplete);
    this.sharedProcedureCreatorComponentVoltages.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentVoltages_StopServiceComplete);
    this.sharedProcedureCreatorComponentVoltages.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponentVoltages_MonitorServiceComplete);
    this.sharedProcedureCreatorComponentVoltages.Resume();
    this.sharedProcedureIntegrationComponentVoltages.ProceduresDropDown = this.sharedProcedureSelectionMeasurement;
    this.sharedProcedureIntegrationComponentVoltages.ProcedureStatusMessageTarget = this.labelVoltagesRoutine;
    this.sharedProcedureIntegrationComponentVoltages.ProcedureStatusStateTarget = this.checkmarkVoltagesRoutine;
    this.sharedProcedureIntegrationComponentVoltages.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentVoltages.StartStopButton = this.buttonStartStopHVMeasurement;
    this.sharedProcedureIntegrationComponentVoltages.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionMeasurement, "sharedProcedureSelectionMeasurement");
    ((Control) this.sharedProcedureSelectionMeasurement).Name = "sharedProcedureSelectionMeasurement";
    this.sharedProcedureSelectionMeasurement.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_OTF_Readout"
    });
    componentResourceManager.ApplyResources((object) this.labelVoltagesRoutine, "labelVoltagesRoutine");
    this.labelVoltagesRoutine.Name = "labelVoltagesRoutine";
    this.labelVoltagesRoutine.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkVoltagesRoutine, "checkmarkVoltagesRoutine");
    ((Control) this.checkmarkVoltagesRoutine).Name = "checkmarkVoltagesRoutine";
    componentResourceManager.ApplyResources((object) this.buttonStartStopHVMeasurement, "buttonStartStopHVMeasurement");
    this.buttonStartStopHVMeasurement.Name = "buttonStartStopHVMeasurement";
    this.buttonStartStopHVMeasurement.UseCompatibleTextRendering = true;
    this.buttonStartStopHVMeasurement.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelContent, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelStatusComponentVoltages, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelTop, 0, 0);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    ((Control) this.tableLayoutPanelContent).BackColor = SystemColors.Window;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelContent, "tableLayoutPanelContent");
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentPTC4, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentBMS1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentBMS2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentBMS3, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentBMS4, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentBMS5, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentBMS6, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentBMS7, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentBMS8, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentBMS9, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentPTIInverter1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentPTIInverter2, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentPTIInverter3, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentDCLConverter, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentPTC3, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentPTC1, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentPTC2, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentEAC, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanelContent).Controls.Add((Control) this.digitalReadoutInstrumentERC, 1, 9);
    ((Control) this.tableLayoutPanelContent).Name = "tableLayoutPanelContent";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentPTC4, "digitalReadoutInstrumentPTC4");
    this.digitalReadoutInstrumentPTC4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC4).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_PtcCab2");
    ((Control) this.digitalReadoutInstrumentPTC4).Name = "digitalReadoutInstrumentPTC4";
    this.digitalReadoutInstrumentPTC4.ShowBorder = false;
    this.digitalReadoutInstrumentPTC4.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC4).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC4).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC4).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBMS1, "digitalReadoutInstrumentBMS1");
    this.digitalReadoutInstrumentBMS1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS1).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS01");
    ((Control) this.digitalReadoutInstrumentBMS1).Name = "digitalReadoutInstrumentBMS1";
    this.digitalReadoutInstrumentBMS1.ShowBorder = false;
    this.digitalReadoutInstrumentBMS1.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS1).TitleLengthPercentOfControl = 36;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS1).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS1).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBMS2, "digitalReadoutInstrumentBMS2");
    this.digitalReadoutInstrumentBMS2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS2).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS02");
    ((Control) this.digitalReadoutInstrumentBMS2).Name = "digitalReadoutInstrumentBMS2";
    this.digitalReadoutInstrumentBMS2.ShowBorder = false;
    this.digitalReadoutInstrumentBMS2.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS2).TitleLengthPercentOfControl = 36;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS2).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS2).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBMS3, "digitalReadoutInstrumentBMS3");
    this.digitalReadoutInstrumentBMS3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS3).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS03");
    ((Control) this.digitalReadoutInstrumentBMS3).Name = "digitalReadoutInstrumentBMS3";
    this.digitalReadoutInstrumentBMS3.ShowBorder = false;
    this.digitalReadoutInstrumentBMS3.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS3).TitleLengthPercentOfControl = 36;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS3).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS3).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBMS4, "digitalReadoutInstrumentBMS4");
    this.digitalReadoutInstrumentBMS4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS4).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS04");
    ((Control) this.digitalReadoutInstrumentBMS4).Name = "digitalReadoutInstrumentBMS4";
    this.digitalReadoutInstrumentBMS4.ShowBorder = false;
    this.digitalReadoutInstrumentBMS4.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS4).TitleLengthPercentOfControl = 36;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS4).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS4).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBMS5, "digitalReadoutInstrumentBMS5");
    this.digitalReadoutInstrumentBMS5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS5).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS5).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS05");
    ((Control) this.digitalReadoutInstrumentBMS5).Name = "digitalReadoutInstrumentBMS5";
    this.digitalReadoutInstrumentBMS5.ShowBorder = false;
    this.digitalReadoutInstrumentBMS5.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS5).TitleLengthPercentOfControl = 36;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS5).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS5).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBMS6, "digitalReadoutInstrumentBMS6");
    this.digitalReadoutInstrumentBMS6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS6).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS6).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS06");
    ((Control) this.digitalReadoutInstrumentBMS6).Name = "digitalReadoutInstrumentBMS6";
    this.digitalReadoutInstrumentBMS6.ShowBorder = false;
    this.digitalReadoutInstrumentBMS6.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS6).TitleLengthPercentOfControl = 36;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS6).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS6).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBMS7, "digitalReadoutInstrumentBMS7");
    this.digitalReadoutInstrumentBMS7.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS7).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS7).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS07");
    ((Control) this.digitalReadoutInstrumentBMS7).Name = "digitalReadoutInstrumentBMS7";
    this.digitalReadoutInstrumentBMS7.ShowBorder = false;
    this.digitalReadoutInstrumentBMS7.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS7).TitleLengthPercentOfControl = 36;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS7).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS7).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS7).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBMS8, "digitalReadoutInstrumentBMS8");
    this.digitalReadoutInstrumentBMS8.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS8).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS8).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS08");
    ((Control) this.digitalReadoutInstrumentBMS8).Name = "digitalReadoutInstrumentBMS8";
    this.digitalReadoutInstrumentBMS8.ShowBorder = false;
    this.digitalReadoutInstrumentBMS8.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS8).TitleLengthPercentOfControl = 36;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS8).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS8).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS8).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBMS9, "digitalReadoutInstrumentBMS9");
    this.digitalReadoutInstrumentBMS9.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS9).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS9).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvBatLinkVoltageBMS09");
    ((Control) this.digitalReadoutInstrumentBMS9).Name = "digitalReadoutInstrumentBMS9";
    this.digitalReadoutInstrumentBMS9.ShowBorder = false;
    this.digitalReadoutInstrumentBMS9.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS9).TitleLengthPercentOfControl = 36;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS9).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS9).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBMS9).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentPTIInverter1, "digitalReadoutInstrumentPTIInverter1");
    this.digitalReadoutInstrumentPTIInverter1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter1).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti1ActDcVolt");
    ((Control) this.digitalReadoutInstrumentPTIInverter1).Name = "digitalReadoutInstrumentPTIInverter1";
    this.digitalReadoutInstrumentPTIInverter1.ShowBorder = false;
    this.digitalReadoutInstrumentPTIInverter1.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter1).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter1).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter1).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentPTIInverter2, "digitalReadoutInstrumentPTIInverter2");
    this.digitalReadoutInstrumentPTIInverter2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter2).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti2ActDcVolt");
    ((Control) this.digitalReadoutInstrumentPTIInverter2).Name = "digitalReadoutInstrumentPTIInverter2";
    this.digitalReadoutInstrumentPTIInverter2.ShowBorder = false;
    this.digitalReadoutInstrumentPTIInverter2.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter2).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter2).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter2).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentPTIInverter3, "digitalReadoutInstrumentPTIInverter3");
    this.digitalReadoutInstrumentPTIInverter3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter3).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_Pti3ActDcVolt_3");
    ((Control) this.digitalReadoutInstrumentPTIInverter3).Name = "digitalReadoutInstrumentPTIInverter3";
    this.digitalReadoutInstrumentPTIInverter3.ShowBorder = false;
    this.digitalReadoutInstrumentPTIInverter3.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter3).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter3).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter3).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTIInverter3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentDCLConverter, "digitalReadoutInstrumentDCLConverter");
    this.digitalReadoutInstrumentDCLConverter.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCLConverter).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCLConverter).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HVDCLinkVoltCvalDCL");
    ((Control) this.digitalReadoutInstrumentDCLConverter).Name = "digitalReadoutInstrumentDCLConverter";
    this.digitalReadoutInstrumentDCLConverter.ShowBorder = false;
    this.digitalReadoutInstrumentDCLConverter.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCLConverter).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCLConverter).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCLConverter).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentDCLConverter).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentPTC3, "digitalReadoutInstrumentPTC3");
    this.digitalReadoutInstrumentPTC3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC3).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_PtcCab1");
    ((Control) this.digitalReadoutInstrumentPTC3).Name = "digitalReadoutInstrumentPTC3";
    this.digitalReadoutInstrumentPTC3.ShowBorder = false;
    this.digitalReadoutInstrumentPTC3.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC3).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC3).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC3).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentPTC1, "digitalReadoutInstrumentPTC1");
    this.digitalReadoutInstrumentPTC1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC1).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvPtcBatt1HvVoltage");
    ((Control) this.digitalReadoutInstrumentPTC1).Name = "digitalReadoutInstrumentPTC1";
    this.digitalReadoutInstrumentPTC1.ShowBorder = false;
    this.digitalReadoutInstrumentPTC1.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC1).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC1).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC1).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentPTC2, "digitalReadoutInstrumentPTC2");
    this.digitalReadoutInstrumentPTC2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC2).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HvPtcBatt2HvVoltage");
    ((Control) this.digitalReadoutInstrumentPTC2).Name = "digitalReadoutInstrumentPTC2";
    this.digitalReadoutInstrumentPTC2.ShowBorder = false;
    this.digitalReadoutInstrumentPTC2.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC2).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC2).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC2).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentPTC2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEAC, "digitalReadoutInstrumentEAC");
    this.digitalReadoutInstrumentEAC.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEAC).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEAC).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_HVDCLinkVoltCvalEComp");
    ((Control) this.digitalReadoutInstrumentEAC).Name = "digitalReadoutInstrumentEAC";
    this.digitalReadoutInstrumentEAC.ShowBorder = false;
    this.digitalReadoutInstrumentEAC.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEAC).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEAC).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEAC).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEAC).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentERC, "digitalReadoutInstrumentERC");
    this.digitalReadoutInstrumentERC.FontGroup = "";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentERC).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentERC).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_HV_Readout_Request_Results_ErcHvVolt");
    ((Control) this.digitalReadoutInstrumentERC).Name = "digitalReadoutInstrumentERC";
    this.digitalReadoutInstrumentERC.ShowBorder = false;
    this.digitalReadoutInstrumentERC.ShowScalingValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentERC).TitleLengthPercentOfControl = 40;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentERC).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentERC).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentERC).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelStatusComponentVoltages, "tableLayoutPanelStatusComponentVoltages");
    ((Control) this.tableLayoutPanelStatusComponentVoltages).BackColor = SystemColors.Control;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelStatusComponentVoltages, 2);
    ((TableLayoutPanel) this.tableLayoutPanelStatusComponentVoltages).Controls.Add((Control) this.sharedProcedureSelectionMeasurement, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelStatusComponentVoltages).Controls.Add((Control) this.buttonStartStopHVMeasurement, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanelStatusComponentVoltages).Controls.Add((Control) this.labelVoltagesRoutine, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelStatusComponentVoltages).Controls.Add((Control) this.checkmarkVoltagesRoutine, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelStatusComponentVoltages).Controls.Add((Control) this.label1, 3, 0);
    ((Control) this.tableLayoutPanelStatusComponentVoltages).Name = "tableLayoutPanelStatusComponentVoltages";
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTop, "tableLayoutPanelTop");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelTop, 2);
    ((TableLayoutPanel) this.tableLayoutPanelTop).Controls.Add((Control) this.pictureBoxWarningIcon, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTop).Controls.Add((Control) this.webBrowserWarning, 1, 0);
    ((Control) this.tableLayoutPanelTop).Name = "tableLayoutPanelTop";
    this.pictureBoxWarningIcon.BackColor = Color.White;
    componentResourceManager.ApplyResources((object) this.pictureBoxWarningIcon, "pictureBoxWarningIcon");
    this.pictureBoxWarningIcon.Name = "pictureBoxWarningIcon";
    this.pictureBoxWarningIcon.TabStop = false;
    componentResourceManager.ApplyResources((object) this.webBrowserWarning, "webBrowserWarning");
    this.webBrowserWarning.Name = "webBrowserWarning";
    this.webBrowserWarning.Url = new Uri("about: blank", UriKind.Absolute);
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_High_Voltage_Measurement");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanelContent).ResumeLayout(false);
    ((Control) this.tableLayoutPanelStatusComponentVoltages).ResumeLayout(false);
    ((Control) this.tableLayoutPanelStatusComponentVoltages).PerformLayout();
    ((Control) this.tableLayoutPanelTop).ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxWarningIcon).EndInit();
    ((Control) this).ResumeLayout(false);
  }
}
