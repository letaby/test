// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Right_Calibration__NGC_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Right_Calibration__NGC_.panel;

public class UserPanel : CustomPanel
{
  private Channel channel;
  private Service calibrateRequestResultsProgressService;
  private TableLayoutPanel tableLayoutPanelWholePanel;
  private TableLayoutPanel tableLayoutPanelButtons;
  private ProgressBar progressBarResultsProgress;
  private SeekTimeListView seekTimeListViewLog;
  private TextBox textBoxInstructions;
  private DigitalReadoutInstrument digitalReadoutInstrumentRequestResultsStatus;
  private TableLayoutPanel tableLayoutPanelInstruments;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;
  private Checkmark checkmarkStatus;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponent;
  private System.Windows.Forms.Label labelStatus;
  private Button buttonStartStop;
  private SharedProcedureSelection sharedProcedureSelection;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent;
  private System.Windows.Forms.Label labelTitle;

  public UserPanel() => this.InitializeComponent();

  public virtual void OnChannelsChanged() => this.SetChannel(this.GetChannel("SRRR01T"));

  private void SetChannel(Channel mpcChannel)
  {
    if (this.channel != null)
      this.calibrateRequestResultsProgressService = (Service) null;
    this.channel = mpcChannel;
    if (this.channel == null)
      return;
    this.calibrateRequestResultsProgressService = this.channel.Services["RT_Service_Alignment_Azimuth_Request_Results_Routine_Progress"];
  }

  private void LogText(string text)
  {
    this.LabelLog(this.seekTimeListViewLog.RequiredUserLabelPrefix, text);
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.sharedProcedureSelection.AnyProcedureInProgress)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.SetChannel((Channel) null);
  }

  private void sharedProcedureCreatorComponent_MonitorServiceComplete(
    object sender,
    MonitorServiceResultEventArgs e)
  {
    if (((MonitorResultEventArgs) e).Action != null && ((MonitorResultEventArgs) e).Action != 1)
      return;
    if (this.calibrateRequestResultsProgressService != (Service) null)
    {
      try
      {
        this.calibrateRequestResultsProgressService.Execute(true);
        if (!(this.calibrateRequestResultsProgressService.OutputValues[0].Value is string))
          this.progressBarResultsProgress.Value = Convert.ToInt32(this.calibrateRequestResultsProgressService.OutputValues[0].Value);
      }
      catch (CaesarException ex)
      {
      }
    }
  }

  private void sharedProcedureCreatorComponent_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    if (!(sender is SharedProcedureBase sharedProcedureBase) || sharedProcedureBase.Result != 1 || this.channel == null)
      return;
    this.LogText(Resources.Message_ResettingFaultCodes);
    this.channel.FaultCodes.Reset(false);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition = new DataItemCondition();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.tableLayoutPanelButtons = new TableLayoutPanel();
    this.checkmarkStatus = new Checkmark();
    this.labelStatus = new System.Windows.Forms.Label();
    this.buttonStartStop = new Button();
    this.sharedProcedureSelection = new SharedProcedureSelection();
    this.progressBarResultsProgress = new ProgressBar();
    this.seekTimeListViewLog = new SeekTimeListView();
    this.labelTitle = new System.Windows.Forms.Label();
    this.textBoxInstructions = new TextBox();
    this.tableLayoutPanelInstruments = new TableLayoutPanel();
    this.digitalReadoutInstrumentRequestResultsStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.sharedProcedureCreatorComponent = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponent = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanelWholePanel).SuspendLayout();
    ((Control) this.tableLayoutPanelButtons).SuspendLayout();
    ((Control) this.tableLayoutPanelInstruments).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelButtons, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.progressBarResultsProgress, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.seekTimeListViewLog, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.labelTitle, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.textBoxInstructions, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).Controls.Add((Control) this.tableLayoutPanelInstruments, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelWholePanel).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelButtons, "tableLayoutPanelButtons");
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.checkmarkStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.labelStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.buttonStartStop, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.sharedProcedureSelection, 2, 0);
    ((Control) this.tableLayoutPanelButtons).Name = "tableLayoutPanelButtons";
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonStartStop, "buttonStartStop");
    this.buttonStartStop.Name = "buttonStartStop";
    this.buttonStartStop.UseCompatibleTextRendering = true;
    this.buttonStartStop.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_SRRRCalibration"
    });
    componentResourceManager.ApplyResources((object) this.progressBarResultsProgress, "progressBarResultsProgress");
    this.progressBarResultsProgress.Name = "progressBarResultsProgress";
    componentResourceManager.ApplyResources((object) this.seekTimeListViewLog, "seekTimeListViewLog");
    this.seekTimeListViewLog.FilterUserLabels = true;
    ((Control) this.seekTimeListViewLog).Name = "seekTimeListViewLog";
    this.seekTimeListViewLog.RequiredUserLabelPrefix = "SRRR-Calibration";
    this.seekTimeListViewLog.SelectedTime = new DateTime?();
    this.seekTimeListViewLog.ShowChannelLabels = false;
    this.seekTimeListViewLog.ShowCommunicationsState = false;
    this.seekTimeListViewLog.ShowControlPanel = false;
    this.seekTimeListViewLog.ShowDeviceColumn = false;
    this.seekTimeListViewLog.TimeFormat = "MM.dd.yyyy HH:mm:ss";
    componentResourceManager.ApplyResources((object) this.labelTitle, "labelTitle");
    this.labelTitle.Name = "labelTitle";
    this.labelTitle.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxInstructions, "textBoxInstructions");
    this.textBoxInstructions.Name = "textBoxInstructions";
    this.textBoxInstructions.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentRequestResultsStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 0, 0);
    ((Control) this.tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentRequestResultsStatus, "digitalReadoutInstrumentRequestResultsStatus");
    this.digitalReadoutInstrumentRequestResultsStatus.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).FreezeValue = false;
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Initialize((ValueState) 0, 11);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(7, 6.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(8, 7.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(9, 8.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(10, 9.0, (ValueState) 0);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(11, (double) byte.MaxValue, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "SRRR01T", "RT_Service_Alignment_Azimuth_Request_Results_Routine_Status");
    ((Control) this.digitalReadoutInstrumentRequestResultsStatus).Name = "digitalReadoutInstrumentRequestResultsStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
    this.digitalReadoutInstrumentEngineSpeed.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureCreatorComponent.Suspend();
    this.sharedProcedureCreatorComponent.MonitorCall = new ServiceCall("SRRR01T", "RT_Service_Alignment_Azimuth_Request_Results_Routine_Status");
    this.sharedProcedureCreatorComponent.MonitorGradient.Initialize((ValueState) 0, 11);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(1, 0.0, (ValueState) 1);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(3, 2.0, (ValueState) 1);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(5, 4.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(6, 5.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(7, 6.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(8, 7.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(9, 8.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(10, 9.0, (ValueState) 0);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(11, (double) byte.MaxValue, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorInterval = 2500;
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponent, "sharedProcedureCreatorComponent");
    this.sharedProcedureCreatorComponent.Qualifier = "SP_SRRRCalibration";
    this.sharedProcedureCreatorComponent.StartCall = new ServiceCall("SRRR01T", "RT_Service_Alignment_Azimuth_Start");
    dataItemCondition.Gradient.Initialize((ValueState) 3, 1);
    dataItemCondition.Gradient.Modify(1, 1.0, (ValueState) 1);
    dataItemCondition.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    this.sharedProcedureCreatorComponent.StartConditions.Add(dataItemCondition);
    this.sharedProcedureCreatorComponent.StopCall = new ServiceCall("SRRR01T", "RT_Service_Alignment_Azimuth_Stop");
    this.sharedProcedureCreatorComponent.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StopServiceComplete);
    this.sharedProcedureCreatorComponent.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponent_MonitorServiceComplete);
    this.sharedProcedureCreatorComponent.Resume();
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.checkmarkStatus;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.buttonStartStop;
    this.sharedProcedureIntegrationComponent.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelWholePanel);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanelWholePanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanelWholePanel).PerformLayout();
    ((Control) this.tableLayoutPanelButtons).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInstruments).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
