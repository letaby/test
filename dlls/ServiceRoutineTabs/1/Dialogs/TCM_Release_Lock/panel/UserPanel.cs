// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Release_Lock.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Release_Lock.panel;

public class UserPanel : CustomPanel
{
  private const string ReleaseTransportSecurityQualifier = "DJ_Release_transport_security_for_TCM";
  private const string AntiTheftSecurityQualifier = "DJ_SecurityAccess_AntiTheftInit";
  private const string RoutineSecurityQualifier = "DJ_SecurityAccess_RoutineIO_AS";
  private Channel tcm;
  private Service ReleaseTransportSecurity;
  private Service AntiTheftSecurity;
  private Service RoutineSecurity;
  private bool isServiceRunning = false;
  private Button btnReleaseLock;
  private Checkmark checkmarkTcmOnline;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelTcmStatus;
  private TableLayoutPanel tableMain;
  private SeekTimeListView seekTimeListViewOutput;
  private TableLayoutPanel tableTransControls;

  public UserPanel()
  {
    this.InitializeComponent();
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
    this.UpdateUserInterface();
  }

  public virtual void OnChannelsChanged()
  {
    this.SetTcm(this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 7));
  }

  private void SetTcm(Channel tcm)
  {
    if (this.tcm == tcm)
      return;
    if (this.tcm != null)
    {
      this.tcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.ReleaseTransportSecurity = (Service) null;
      this.AntiTheftSecurity = (Service) null;
      this.RoutineSecurity = (Service) null;
    }
    this.tcm = tcm;
    if (this.tcm != null)
    {
      this.tcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.ReleaseTransportSecurity = this.tcm.Services["DJ_Release_transport_security_for_TCM"];
      this.AntiTheftSecurity = this.tcm.Ecu.Name == "TCM05T" ? this.tcm.Services["DJ_SecurityAccess_AntiTheftInit"] : (Service) null;
      this.RoutineSecurity = this.tcm.Ecu.Name == "TCM05T" ? this.tcm.Services["DJ_SecurityAccess_RoutineIO_AS"] : (Service) null;
    }
    this.UpdateUserInterface();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.checkmarkTcmOnline.Checked = this.Online;
    if (this.isServiceRunning)
    {
      ((Control) this.labelTcmStatus).Text = Resources.Message_LockIsBeingReleased;
      this.btnReleaseLock.Enabled = false;
    }
    else if (!this.Online)
    {
      ((Control) this.labelTcmStatus).Text = Resources.Message_TheLockCannotBeReleasedBecauseTheTCMIsOffline;
      this.btnReleaseLock.Enabled = false;
    }
    else
    {
      ((Control) this.labelTcmStatus).Text = Resources.Message_Ready;
      this.btnReleaseLock.Enabled = true;
    }
  }

  private bool Online => this.tcm != null && this.tcm.Online;

  private void btnReleaseLock_Click(object sender, EventArgs e)
  {
    this.isServiceRunning = true;
    this.StartReleaseTransportSecurity();
  }

  private void StartReleaseTransportSecurity()
  {
    if (this.Online && this.AntiTheftSecurity != (Service) null)
    {
      this.AntiTheftSecurity.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.AntiTheftSecurity_ServiceCompleteEvent);
      this.AntiTheftSecurity.Execute(false);
    }
    else if (this.Online && this.ReleaseTransportSecurity != (Service) null)
    {
      this.ReleaseTransportSecurity.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.ReleaseTransportSecurity_ServiceCompleteEvent);
      this.AddLogLabel(Resources.Message_ReleasingTransportSecurity);
      this.ReleaseTransportSecurity.Execute(false);
    }
    else
    {
      this.AddLogLabel(Resources.Message_CannotReleaseTransportSecurityEitherTheTCMIsUnavailableOrTheServiceCannotBeFound);
      this.isServiceRunning = false;
    }
    this.UpdateUserInterface();
  }

  private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.isServiceRunning || e.CloseReason != CloseReason.UserClosing)
      return;
    e.Cancel = true;
  }

  private void AntiTheftSecurity_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.AntiTheftSecurity.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.AntiTheftSecurity_ServiceCompleteEvent);
    if (this.Online && this.ReleaseTransportSecurity != (Service) null)
    {
      this.ReleaseTransportSecurity.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.ReleaseTransportSecurity_ServiceCompleteEvent);
      this.AddLogLabel(Resources.Message_ReleasingTransportSecurity);
      this.ReleaseTransportSecurity.Execute(false);
    }
    else
    {
      this.isServiceRunning = false;
      this.UpdateUserInterface();
    }
  }

  private void ReleaseTransportSecurity_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.ReleaseTransportSecurity.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.ReleaseTransportSecurity_ServiceCompleteEvent);
    if (e.Succeeded)
      this.AddLogLabel(Resources.Message_SuccessfullyReleasedTransportSecurity);
    else
      this.AddLogLabel(string.Format(Resources.MessageFormat_UnableToReleaseTransportSecurityError0, (object) e.Exception.Message));
    if (this.Online && this.RoutineSecurity != (Service) null)
    {
      this.RoutineSecurity.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.RoutineSecurity_ServiceServiceComplete);
      this.RoutineSecurity.Execute(false);
    }
    else
    {
      this.isServiceRunning = false;
      this.UpdateUserInterface();
    }
  }

  private void RoutineSecurity_ServiceServiceComplete(object sender, ResultEventArgs e)
  {
    this.RoutineSecurity.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.RoutineSecurity_ServiceServiceComplete);
    this.isServiceRunning = false;
    this.UpdateUserInterface();
  }

  private void AddLogLabel(string text)
  {
    this.LabelLog(this.seekTimeListViewOutput.RequiredUserLabelPrefix, text);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableTransControls = new TableLayoutPanel();
    this.btnReleaseLock = new Button();
    this.tableMain = new TableLayoutPanel();
    this.labelTcmStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.checkmarkTcmOnline = new Checkmark();
    this.seekTimeListViewOutput = new SeekTimeListView();
    ((Control) this.tableTransControls).SuspendLayout();
    ((Control) this.tableMain).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableTransControls, "tableTransControls");
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.tableTransControls, 2);
    ((TableLayoutPanel) this.tableTransControls).Controls.Add((Control) this.btnReleaseLock, 1, 0);
    ((Control) this.tableTransControls).Name = "tableTransControls";
    componentResourceManager.ApplyResources((object) this.btnReleaseLock, "btnReleaseLock");
    this.btnReleaseLock.Name = "btnReleaseLock";
    this.btnReleaseLock.UseCompatibleTextRendering = true;
    this.btnReleaseLock.UseVisualStyleBackColor = true;
    this.btnReleaseLock.Click += new EventHandler(this.btnReleaseLock_Click);
    componentResourceManager.ApplyResources((object) this.tableMain, "tableMain");
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.labelTcmStatus, 1, 1);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.checkmarkTcmOnline, 0, 1);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.seekTimeListViewOutput, 0, 0);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.tableTransControls, 0, 3);
    ((Control) this.tableMain).Name = "tableMain";
    this.labelTcmStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelTcmStatus, "labelTcmStatus");
    ((Control) this.labelTcmStatus).Name = "labelTcmStatus";
    this.labelTcmStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    componentResourceManager.ApplyResources((object) this.checkmarkTcmOnline, "checkmarkTcmOnline");
    ((Control) this.checkmarkTcmOnline).Name = "checkmarkTcmOnline";
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.seekTimeListViewOutput, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListViewOutput, "seekTimeListViewOutput");
    this.seekTimeListViewOutput.FilterUserLabels = true;
    ((Control) this.seekTimeListViewOutput).Name = "seekTimeListViewOutput";
    this.seekTimeListViewOutput.RequiredUserLabelPrefix = "tcmReplacementMy13";
    this.seekTimeListViewOutput.SelectedTime = new DateTime?();
    this.seekTimeListViewOutput.ShowChannelLabels = false;
    this.seekTimeListViewOutput.ShowCommunicationsState = false;
    this.seekTimeListViewOutput.ShowControlPanel = false;
    this.seekTimeListViewOutput.ShowDeviceColumn = false;
    this.seekTimeListViewOutput.TimeFormat = "HH:mm:ss.f";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_TCM_Release_Lock");
    ((Control) this).Controls.Add((Control) this.tableMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableTransControls).ResumeLayout(false);
    ((Control) this.tableMain).ResumeLayout(false);
    ((Control) this.tableMain).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
