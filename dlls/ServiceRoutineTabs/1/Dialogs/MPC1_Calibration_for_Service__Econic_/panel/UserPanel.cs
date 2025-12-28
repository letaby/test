// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC1_Calibration_for_Service__Econic_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC1_Calibration_for_Service__Econic_.panel;

public class UserPanel : CustomPanel
{
  private Channel channel;
  private Service resultService;
  private Service staticConfigService;
  private Service resetService;
  private Service unlockService;
  private UserPanel.ProcessState state;
  private TableLayoutPanel tableLayoutPanel1;
  private SeekTimeListView seekTimeListView;
  private DigitalReadoutInstrument digitalReadoutInstrumentResult;
  private DigitalReadoutInstrument digitalReadoutInstrumentOverallStatus;
  private DigitalReadoutInstrument digitalReadoutInstrumentOnlineStatus;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrumentStaticStatus;
  private ScalingLabel scalingLabelTitle;
  private DigitalReadoutInstrument digitalReadoutInstrumentLDWFunction;
  private DigitalReadoutInstrument digitalReadoutInstrumentCameraHeight;
  private Button buttonCalibrate;

  public UserPanel() => this.InitializeComponent();

  public virtual void OnChannelsChanged() => this.SetChannel(this.GetChannel("MPC01T"));

  private void SetChannel(Channel mpcChannel)
  {
    if (this.channel != null)
    {
      this.channel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.Advance);
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.channel_CommunicationsStateUpdateEvent);
      this.resultService = (Service) null;
      this.staticConfigService = (Service) null;
      this.resetService = (Service) null;
      this.unlockService = (Service) null;
    }
    this.channel = mpcChannel;
    if (this.channel != null)
    {
      this.channel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.Advance);
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.channel_CommunicationsStateUpdateEvent);
      if (!this.channel.Parameters.HaveBeenReadFromEcu)
        this.channel.Parameters.Read(false);
      ServiceCollection services = this.channel.Services;
      Qualifier instrument = ((SingleInstrumentBase) this.digitalReadoutInstrumentResult).Instrument;
      string name = ((Qualifier) ref instrument).Name;
      this.resultService = services[name];
      if (this.resultService != (Service) null)
        this.resultService.Execute(false);
      this.staticConfigService = this.channel.Services["DL_Static_Camera_Calibration_Data"];
      this.resetService = this.channel.Services["FN_HardReset"];
      this.unlockService = this.channel.Services["DJ_SecurityAccess_Config_Dev"];
    }
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.buttonCalibrate.Enabled = this.HaveReadParameters && !this.Working;
  }

  private void GoMachine()
  {
    ++this.state;
    switch (this.state)
    {
      case UserPanel.ProcessState.Unlock:
        this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, Resources.Message_UnlockingDevice);
        this.unlockService.Execute(false);
        break;
      case UserPanel.ProcessState.ConfigStatic:
        this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, Resources.Message_ConfiguringStaticCalibration);
        this.staticConfigService.InputValues[0].Value = (object) 0.0f;
        this.staticConfigService.InputValues[1].Value = (object) 0.0f;
        this.staticConfigService.InputValues[2].Value = (object) 0.0f;
        this.staticConfigService.Execute(false);
        break;
      case UserPanel.ProcessState.Reset:
        this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, Resources.Message_ResettingDevice);
        this.resetService.Execute(false);
        break;
      case UserPanel.ProcessState.RequestResult:
        this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, Resources.Message_VerifyingResult);
        this.resultService.Execute(false);
        break;
      case UserPanel.ProcessState.Complete:
        this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, Resources.Message_Complete);
        this.UpdateUserInterface();
        break;
    }
  }

  private void Advance(object sender, ResultEventArgs e)
  {
    if (!this.Working)
      return;
    if (e.Succeeded)
    {
      if (this.Online)
        this.GoMachine();
      else
        this.Abort(Resources.Message_EcuDisconnectedBeforeCompletion);
    }
    else
      this.Abort(e.Exception.Message);
  }

  private void Abort(string reason)
  {
    this.state = UserPanel.ProcessState.Complete;
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, reason);
    if (this.Online)
      this.resultService.Execute(false);
    else
      this.UpdateUserInterface();
  }

  private void buttonCalibrate_Click(object sender, EventArgs e)
  {
    this.state = UserPanel.ProcessState.NotRunning;
    this.GoMachine();
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.Working)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.SetChannel((Channel) null);
  }

  private void channel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  public bool Working
  {
    get
    {
      return this.state != UserPanel.ProcessState.NotRunning && this.state != UserPanel.ProcessState.Complete;
    }
  }

  public bool Online => this.channel != null && this.channel.Online;

  public bool HaveReadParameters
  {
    get => this.channel != null && this.channel.Parameters.HaveBeenReadFromEcu;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrumentOverallStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentOnlineStatus = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentResult = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentStaticStatus = new DigitalReadoutInstrument();
    this.scalingLabelTitle = new ScalingLabel();
    this.digitalReadoutInstrumentLDWFunction = new DigitalReadoutInstrument();
    this.seekTimeListView = new SeekTimeListView();
    this.buttonCalibrate = new Button();
    this.digitalReadoutInstrumentCameraHeight = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentOverallStatus, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentOnlineStatus, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentResult, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentStaticStatus, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.scalingLabelTitle, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentLDWFunction, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonCalibrate, 3, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentCameraHeight, 1, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentOverallStatus, "digitalReadoutInstrumentOverallStatus");
    this.digitalReadoutInstrumentOverallStatus.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOverallStatus).FreezeValue = false;
    this.digitalReadoutInstrumentOverallStatus.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentOverallStatus.Gradient.Modify(1, 0.0, (ValueState) 4);
    this.digitalReadoutInstrumentOverallStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentOverallStatus.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentOverallStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOverallStatus).Instrument = new Qualifier((QualifierTypes) 1, "MPC01T", "DT_disc01_Camera_Calibration_Overall_Camera_Calibration_Status");
    ((Control) this.digitalReadoutInstrumentOverallStatus).Name = "digitalReadoutInstrumentOverallStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOverallStatus).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentOnlineStatus, "digitalReadoutInstrumentOnlineStatus");
    this.digitalReadoutInstrumentOnlineStatus.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOnlineStatus).FreezeValue = false;
    this.digitalReadoutInstrumentOnlineStatus.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentOnlineStatus.Gradient.Modify(1, 0.0, (ValueState) 4);
    this.digitalReadoutInstrumentOnlineStatus.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentOnlineStatus.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentOnlineStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOnlineStatus).Instrument = new Qualifier((QualifierTypes) 1, "MPC01T", "DT_disc01_Camera_Calibration_Online_Camera_Calibration_Status");
    ((Control) this.digitalReadoutInstrumentOnlineStatus).Name = "digitalReadoutInstrumentOnlineStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentOnlineStatus).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "MPC01T", "00FBED");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentResult, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentResult, "digitalReadoutInstrumentResult");
    this.digitalReadoutInstrumentResult.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResult).FreezeValue = false;
    this.digitalReadoutInstrumentResult.Gradient.Initialize((ValueState) 0, 10);
    this.digitalReadoutInstrumentResult.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentResult.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentResult.Gradient.Modify(3, 2.0, (ValueState) 4);
    this.digitalReadoutInstrumentResult.Gradient.Modify(4, 3.0, (ValueState) 4);
    this.digitalReadoutInstrumentResult.Gradient.Modify(5, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentResult.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentResult.Gradient.Modify(7, 6.0, (ValueState) 3);
    this.digitalReadoutInstrumentResult.Gradient.Modify(8, 7.0, (ValueState) 3);
    this.digitalReadoutInstrumentResult.Gradient.Modify(9, 8.0, (ValueState) 3);
    this.digitalReadoutInstrumentResult.Gradient.Modify(10, 15.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResult).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "MPC01T", "RT_End_of_Line_Calibration_RequestResults_Static_Camera_Calibration_Result");
    ((Control) this.digitalReadoutInstrumentResult).Name = "digitalReadoutInstrumentResult";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentResult).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentStaticStatus, "digitalReadoutInstrumentStaticStatus");
    this.digitalReadoutInstrumentStaticStatus.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentStaticStatus).FreezeValue = false;
    this.digitalReadoutInstrumentStaticStatus.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentStaticStatus.Gradient.Modify(1, 0.0, (ValueState) 4);
    this.digitalReadoutInstrumentStaticStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentStaticStatus.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentStaticStatus.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentStaticStatus).Instrument = new Qualifier((QualifierTypes) 1, "MPC01T", "DT_disc01_Camera_Calibration_Static_Camera_Calibration_Status");
    ((Control) this.digitalReadoutInstrumentStaticStatus).Name = "digitalReadoutInstrumentStaticStatus";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentStaticStatus).UnitAlignment = StringAlignment.Near;
    this.scalingLabelTitle.Alignment = StringAlignment.Center;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.scalingLabelTitle, 4);
    componentResourceManager.ApplyResources((object) this.scalingLabelTitle, "scalingLabelTitle");
    this.scalingLabelTitle.FontGroup = (string) null;
    this.scalingLabelTitle.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelTitle).Name = "scalingLabelTitle";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentLDWFunction, "digitalReadoutInstrumentLDWFunction");
    this.digitalReadoutInstrumentLDWFunction.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentLDWFunction).FreezeValue = false;
    this.digitalReadoutInstrumentLDWFunction.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentLDWFunction.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentLDWFunction.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentLDWFunction.Gradient.Modify(3, 2.0, (ValueState) 2);
    this.digitalReadoutInstrumentLDWFunction.Gradient.Modify(4, 3.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentLDWFunction).Instrument = new Qualifier((QualifierTypes) 1, "MPC01T", "DT_disc02_LDW_Function_Data_LDW_Function_State");
    ((Control) this.digitalReadoutInstrumentLDWFunction).Name = "digitalReadoutInstrumentLDWFunction";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentLDWFunction).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "MPC1Calibration";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView, 3);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    componentResourceManager.ApplyResources((object) this.buttonCalibrate, "buttonCalibrate");
    this.buttonCalibrate.Name = "buttonCalibrate";
    this.buttonCalibrate.UseCompatibleTextRendering = true;
    this.buttonCalibrate.UseVisualStyleBackColor = true;
    this.buttonCalibrate.Click += new EventHandler(this.buttonCalibrate_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentCameraHeight, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCameraHeight, "digitalReadoutInstrumentCameraHeight");
    this.digitalReadoutInstrumentCameraHeight.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCameraHeight).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCameraHeight).Instrument = new Qualifier((QualifierTypes) 4, "MPC01T", "camera_height");
    ((Control) this.digitalReadoutInstrumentCameraHeight).Name = "digitalReadoutInstrumentCameraHeight";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCameraHeight).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_MPC1_Camera_Alignment");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum ProcessState
  {
    NotRunning,
    Unlock,
    ConfigStatic,
    Reset,
    RequestResult,
    Complete,
  }
}
