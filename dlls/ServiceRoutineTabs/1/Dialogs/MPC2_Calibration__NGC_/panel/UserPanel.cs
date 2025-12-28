// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC2_Calibration__NGC_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC2_Calibration__NGC_.panel;

public class UserPanel : CustomPanel
{
  private const string cameraHeight = "camera_height";
  private Channel channel;
  private Service calibrateRequestResultsProgressService;
  private string calibrateCommitServiceList;
  private WarningManager warningManager;
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
  private Button buttonHiddenStart;
  private DigitalReadoutInstrument digitalReadoutInstrumentCameraHeight;
  private System.Windows.Forms.Label labelTitle;

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningManager = new WarningManager(Resources.WarningManagerMessage, Resources.WarningManagerJobName, this.seekTimeListViewLog.RequiredUserLabelPrefix);
  }

  public virtual void OnChannelsChanged() => this.SetChannel(this.GetChannel("MPC02T"));

  private void SetChannel(Channel mpcChannel)
  {
    if (this.channel == mpcChannel)
      return;
    this.warningManager.Reset();
    if (this.channel != null)
    {
      this.calibrateRequestResultsProgressService = (Service) null;
      this.calibrateCommitServiceList = (string) null;
    }
    this.channel = mpcChannel;
    if (this.channel != null)
    {
      this.calibrateRequestResultsProgressService = this.channel.Services["RT_iOCAL_Routine_Control_Request_Results_Progress"];
      this.calibrateCommitServiceList = this.channel.Services.GetDereferencedServiceList("CommitToPermanentMemoryService");
      if (this.channel.CommunicationsState == CommunicationsState.Online && this.channel.Parameters["camera_height"] != null && !this.channel.Parameters["camera_height"].HasBeenReadFromEcu)
        this.channel.Parameters.ReadGroup(this.channel.Parameters["camera_height"].GroupQualifier, true, false);
    }
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
    if (((MonitorResultEventArgs) e).Action != 0)
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
    if (!(sender is SharedProcedureBase sharedProcedureBase))
      return;
    if (sharedProcedureBase.Result == 1)
    {
      if (this.channel != null && this.calibrateCommitServiceList != null)
      {
        this.LogText(Resources.Message_ResettingFaultCodes);
        this.channel.FaultCodes.Reset(false);
        this.LogText(Resources.Message_CommittingCalibration);
        this.channel.Services.Execute(this.calibrateCommitServiceList, false);
        this.LogText(Resources.Messagre_CalibrationComplete);
      }
      else
        this.LogText(Resources.Message_CalibrationNotComplete);
    }
    else
      this.LogText(Resources.Message_CalibrationNotComplete);
  }

  private void buttonStartStop_Click(object sender, EventArgs e)
  {
    if (this.sharedProcedureSelection.SelectedProcedure.CanStart)
    {
      if (!this.warningManager.RequestContinue())
        return;
      this.sharedProcedureIntegrationComponent.StartStopButton.PerformClick();
    }
    else
    {
      this.sharedProcedureIntegrationComponent.StartStopButton.PerformClick();
      this.LogText(Resources.Message_Cancelled);
    }
  }

  private void buttonHiddenStart_EnabledChanged(object sender, EventArgs e)
  {
    this.buttonStartStop.Enabled = this.buttonHiddenStart.Enabled;
  }

  private void buttonHiddenStart_TextChanged(object sender, EventArgs e)
  {
    string a = (string) null;
    if (sender is Button)
      a = (sender as Button).Text;
    if (string.Equals(a, "&Start", StringComparison.OrdinalIgnoreCase))
      this.buttonStartStop.Text = Resources.Button_StartTitle;
    else
      this.buttonStartStop.Text = this.buttonHiddenStart.Text;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition = new DataItemCondition();
    this.tableLayoutPanelWholePanel = new TableLayoutPanel();
    this.tableLayoutPanelButtons = new TableLayoutPanel();
    this.buttonHiddenStart = new Button();
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
    this.digitalReadoutInstrumentCameraHeight = new DigitalReadoutInstrument();
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
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.buttonHiddenStart, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.checkmarkStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.labelStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.buttonStartStop, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanelButtons).Controls.Add((Control) this.sharedProcedureSelection, 3, 0);
    ((Control) this.tableLayoutPanelButtons).Name = "tableLayoutPanelButtons";
    componentResourceManager.ApplyResources((object) this.buttonHiddenStart, "buttonHiddenStart");
    this.buttonHiddenStart.Name = "buttonHiddenStart";
    this.buttonHiddenStart.UseCompatibleTextRendering = true;
    this.buttonHiddenStart.UseVisualStyleBackColor = true;
    this.buttonHiddenStart.EnabledChanged += new EventHandler(this.buttonHiddenStart_EnabledChanged);
    this.buttonHiddenStart.TextChanged += new EventHandler(this.buttonHiddenStart_TextChanged);
    componentResourceManager.ApplyResources((object) this.checkmarkStatus, "checkmarkStatus");
    ((Control) this.checkmarkStatus).Name = "checkmarkStatus";
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonStartStop, "buttonStartStop");
    this.buttonStartStop.Name = "buttonStartStop";
    this.buttonStartStop.UseCompatibleTextRendering = true;
    this.buttonStartStop.UseVisualStyleBackColor = true;
    this.buttonStartStop.Click += new EventHandler(this.buttonStartStop_Click);
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection, "sharedProcedureSelection");
    ((Control) this.sharedProcedureSelection).Name = "sharedProcedureSelection";
    this.sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_MPC2Calibration"
    });
    componentResourceManager.ApplyResources((object) this.progressBarResultsProgress, "progressBarResultsProgress");
    this.progressBarResultsProgress.Name = "progressBarResultsProgress";
    componentResourceManager.ApplyResources((object) this.seekTimeListViewLog, "seekTimeListViewLog");
    this.seekTimeListViewLog.FilterUserLabels = true;
    ((Control) this.seekTimeListViewLog).Name = "seekTimeListViewLog";
    this.seekTimeListViewLog.RequiredUserLabelPrefix = "MPC2-Calibration";
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
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentRequestResultsStatus, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentCameraHeight, 0, 0);
    ((Control) this.tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentRequestResultsStatus, "digitalReadoutInstrumentRequestResultsStatus");
    this.digitalReadoutInstrumentRequestResultsStatus.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).FreezeValue = false;
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Initialize((ValueState) 0, 8);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(7, 6.0, (ValueState) 3);
    this.digitalReadoutInstrumentRequestResultsStatus.Gradient.Modify(8, (double) byte.MaxValue, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "MPC02T", "RT_iOCAL_Routine_Control_Request_Results_Status");
    ((Control) this.digitalReadoutInstrumentRequestResultsStatus).Name = "digitalReadoutInstrumentRequestResultsStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRequestResultsStatus).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCameraHeight, "digitalReadoutInstrumentCameraHeight");
    this.digitalReadoutInstrumentCameraHeight.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCameraHeight).FreezeValue = false;
    this.digitalReadoutInstrumentCameraHeight.Gradient.Initialize((ValueState) 3, 1);
    this.digitalReadoutInstrumentCameraHeight.Gradient.Modify(1, 1.01, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCameraHeight).Instrument = new Qualifier((QualifierTypes) 4, "MPC02T", "camera_height");
    ((Control) this.digitalReadoutInstrumentCameraHeight).Name = "digitalReadoutInstrumentCameraHeight";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCameraHeight).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureCreatorComponent.Suspend();
    this.sharedProcedureCreatorComponent.MonitorCall = new ServiceCall("MPC02T", "RT_iOCAL_Routine_Control_Request_Results_Status");
    this.sharedProcedureCreatorComponent.MonitorGradient.Initialize((ValueState) 0, 8);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(1, 0.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(2, 1.0, (ValueState) 0);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(3, 2.0, (ValueState) 1);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(4, 3.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(5, 4.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(6, 5.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(7, 6.0, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorGradient.Modify(8, (double) byte.MaxValue, (ValueState) 3);
    this.sharedProcedureCreatorComponent.MonitorInterval = 2500;
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponent, "sharedProcedureCreatorComponent");
    this.sharedProcedureCreatorComponent.Qualifier = "SP_MPC2Calibration";
    this.sharedProcedureCreatorComponent.StartCall = new ServiceCall("MPC02T", "RT_iOCAL_Routine_Control_Start");
    dataItemCondition.Gradient.Initialize((ValueState) 3, 1);
    dataItemCondition.Gradient.Modify(1, 1.0, (ValueState) 1);
    dataItemCondition.Qualifier = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    this.sharedProcedureCreatorComponent.StartConditions.Add(dataItemCondition);
    this.sharedProcedureCreatorComponent.StopCall = new ServiceCall("MPC02T", "RT_iOCAL_Routine_Control_Stop");
    this.sharedProcedureCreatorComponent.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponent_StopServiceComplete);
    this.sharedProcedureCreatorComponent.MonitorServiceComplete += new EventHandler<MonitorServiceResultEventArgs>(this.sharedProcedureCreatorComponent_MonitorServiceComplete);
    this.sharedProcedureCreatorComponent.Resume();
    this.sharedProcedureIntegrationComponent.ProceduresDropDown = this.sharedProcedureSelection;
    this.sharedProcedureIntegrationComponent.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent.ProcedureStatusStateTarget = this.checkmarkStatus;
    this.sharedProcedureIntegrationComponent.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent.StartStopButton = this.buttonHiddenStart;
    this.sharedProcedureIntegrationComponent.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_MPC2_Calibration");
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
