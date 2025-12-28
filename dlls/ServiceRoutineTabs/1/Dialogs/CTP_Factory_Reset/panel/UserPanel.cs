// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Factory_Reset.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Factory_Reset.panel;

public class UserPanel : CustomPanel
{
  private const string CTP01T = "CTP01T";
  private const string ResetServiceQualifier = "ResetCalibrationsToDefaultService";
  private Channel ctp;
  private TableLayoutPanel tableLayoutPanel1;
  private SeekTimeListView seekTimeListView;
  private Button buttonClose;
  private Button buttonFactoryReset;
  private Checkmark checkmarkReady;
  private System.Windows.Forms.Label labelStatus;

  public UserPanel() => this.InitializeComponent();

  private bool CanClose => this.ctp == null || !this.Busy;

  public bool CanStart
  {
    get
    {
      return this.ctp != null && !this.Busy && this.ctp.Ecu.Properties.ContainsKey("ResetCalibrationsToDefaultService");
    }
  }

  public bool Busy { get; private set; }

  public bool RoutineComplete { get; private set; }

  public bool Result { get; private set; }

  public string ResultMessage { get; private set; }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.Result = false;
    this.ResultMessage = Resources.Message_None;
    this.UpdateChannels();
    this.UpdateUserInterface();
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = !this.CanClose;
    if (!this.CanClose)
      return;
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.Result,
      (object) this.ResultMessage
    };
    this.buttonClose.DialogResult = this.Result ? DialogResult.Yes : DialogResult.No;
    if (((ContainerControl) this).ParentForm != null)
      ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
  }

  public virtual void OnChannelsChanged() => this.UpdateChannels();

  private void UpdateChannels()
  {
    Channel channel = this.GetChannel("CTP01T", (CustomPanel.ChannelLookupOptions) 5);
    if (this.ctp == channel)
      return;
    this.ctp = channel;
    this.ResetResults();
  }

  private void LogMessage(string message)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, message);
    this.AddStatusMessage(message);
  }

  private void ResetResults()
  {
    this.RoutineComplete = false;
    this.Result = false;
    this.ResultMessage = Resources.Message_None;
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.buttonClose.Enabled = this.CanClose;
    this.buttonFactoryReset.Enabled = this.CanStart;
    if (this.ctp == null)
    {
      this.labelStatus.Text = Resources.Message_CTPIsOffline;
      this.checkmarkReady.Checked = false;
    }
    else if (this.Busy)
    {
      this.checkmarkReady.CheckState = CheckState.Indeterminate;
      this.labelStatus.Text = Resources.Message_PerformingFactoryResetRoutine;
    }
    else if (this.RoutineComplete)
    {
      this.checkmarkReady.Checked = this.Result;
      this.labelStatus.Text = this.ResultMessage;
    }
    else
    {
      this.checkmarkReady.Checked = this.CanStart;
      if (this.CanStart)
        this.labelStatus.Text = Resources.Message_Ready;
      else
        this.labelStatus.Text = Resources.Message_ErrorCannotStartRoutine;
    }
  }

  private void ExecuteReloadService()
  {
    this.ctp.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.ReloadOriginalSupplier_ServiceCompleteEvent);
    this.ctp.Services.Execute(this.ctp.Ecu.Properties["ResetCalibrationsToDefaultService"], false);
    this.LogMessage(Resources.Message_ReloadSupplierConfigurationServiceExecuted);
    this.UpdateUserInterface();
  }

  private void ReloadOriginalSupplier_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.ctp.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.ReloadOriginalSupplier_ServiceCompleteEvent);
    this.Result = e.Succeeded;
    if (e.Succeeded)
    {
      this.LogMessage(Resources.Message_ReloadSupplierConfigurationExecutedSuccessfully);
      this.ResultMessage = Resources.Message_FactoryResetRoutineCompletedSuccessfully;
    }
    else
    {
      this.LogMessage(Resources.Message_ReloadSupplierConfigurationFailed);
      this.ResultMessage = Resources.Message_FactoryResetRoutineFAILED;
    }
    Application.DoEvents();
    this.LogMessage(Resources.Message_Finished);
    this.RoutineComplete = true;
    this.Busy = false;
    this.UpdateUserInterface();
  }

  private void buttonFactoryReset_Click(object sender, EventArgs e)
  {
    this.Busy = true;
    this.LogMessage(Resources.Message_UserRequestedCTPFactoryReset);
    this.ResetResults();
    this.ExecuteReloadService();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.seekTimeListView = new SeekTimeListView();
    this.buttonClose = new Button();
    this.buttonFactoryReset = new Button();
    this.checkmarkReady = new Checkmark();
    this.labelStatus = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonFactoryReset, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkReady, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 1, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView, 5);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "CTPFactoryReset";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonFactoryReset, "buttonFactoryReset");
    this.buttonFactoryReset.Name = "buttonFactoryReset";
    this.buttonFactoryReset.UseCompatibleTextRendering = true;
    this.buttonFactoryReset.UseVisualStyleBackColor = true;
    this.buttonFactoryReset.Click += new EventHandler(this.buttonFactoryReset_Click);
    componentResourceManager.ApplyResources((object) this.checkmarkReady, "checkmarkReady");
    ((Control) this.checkmarkReady).Name = "checkmarkReady";
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
