// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_ABA_Misuse_Reset.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_ABA_Misuse_Reset.panel;

public class UserPanel : CustomPanel
{
  private Channel vrdu;
  private Checkmark checkmarkTcmOnline;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelTcmStatus;
  private TableLayoutPanel tableMain;
  private SeekTimeListView seekTimeListViewOutput;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private RunServicesButton runServicesButton;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private TableLayoutPanel tableTransControls;

  public UserPanel()
  {
    this.InitializeComponent();
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
    this.UpdateUserInterface();
  }

  public virtual void OnChannelsChanged() => this.SetTcm(this.GetChannel("VRDU01T"));

  private void SetTcm(Channel vrdu)
  {
    if (this.vrdu == vrdu)
      return;
    this.vrdu = vrdu;
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.checkmarkTcmOnline.Checked = this.Online;
    if (((RunSharedProcedureButtonBase) this.runServicesButton).InProgress)
      ((Control) this.labelTcmStatus).Text = Resources.Message_ResettingTheFault;
    else if (!this.Online)
      ((Control) this.labelTcmStatus).Text = Resources.Message_TheFaultCannotBeResetBecauseTheVRDUIsOffline;
    else
      ((Control) this.labelTcmStatus).Text = Resources.Message_Ready;
  }

  private bool Online => this.vrdu != null && this.vrdu.Online;

  private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!((RunSharedProcedureButtonBase) this.runServicesButton).InProgress || e.CloseReason != CloseReason.UserClosing)
      return;
    e.Cancel = true;
  }

  private void AddLogLabel(string text)
  {
    this.LabelLog(this.seekTimeListViewOutput.RequiredUserLabelPrefix, text);
  }

  private void runServicesButton_Complete(object sender, PassFailResultEventArgs e)
  {
    if (((ResultEventArgs) e).Succeeded)
      this.AddLogLabel(string.Format(Resources.MessageFormat_TheFaultHasBeenReset0, ((SingleInstrumentBase) this.digitalReadoutInstrument1).DataItem.Value));
    else
      this.AddLogLabel(string.Format(Resources.MessageFormat_UnableToResetError0, (object) ((ResultEventArgs) e).Exception.Message));
    this.UpdateUserInterface();
  }

  private void runServicesButton_Starting(object sender, CancelEventArgs e)
  {
    this.AddLogLabel(Resources.Message_ResettingTheFault);
    this.UpdateUserInterface();
  }

  private void runServicesButton_ButtonEnabledChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableMain = new TableLayoutPanel();
    this.tableTransControls = new TableLayoutPanel();
    this.runServicesButton = new RunServicesButton();
    this.labelTcmStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.checkmarkTcmOnline = new Checkmark();
    this.seekTimeListViewOutput = new SeekTimeListView();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    ((Control) this.tableTransControls).SuspendLayout();
    ((ISupportInitialize) this.runServicesButton).BeginInit();
    ((Control) this.tableMain).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableTransControls, "tableTransControls");
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.tableTransControls, 3);
    ((TableLayoutPanel) this.tableTransControls).Controls.Add((Control) this.runServicesButton, 1, 0);
    ((Control) this.tableTransControls).Name = "tableTransControls";
    componentResourceManager.ApplyResources((object) this.runServicesButton, "runServicesButton");
    ((Control) this.runServicesButton).Name = "runServicesButton";
    this.runServicesButton.ServiceCalls.Add(new ServiceCall("VRDU01T", "DJ_SecurityAccess_Routine"));
    this.runServicesButton.ServiceCalls.Add(new ServiceCall("VRDU01T", "RT_Delete_permanent_errors_Start"));
    this.runServicesButton.ServiceCalls.Add(new ServiceCall("VRDU01T", "RT_Delete_permanent_errors_Request_Results_Delete_Results"));
    this.runServicesButton.Complete += new EventHandler<PassFailResultEventArgs>(this.runServicesButton_Complete);
    ((RunSharedProcedureButtonBase) this.runServicesButton).Starting += new EventHandler<CancelEventArgs>(this.runServicesButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServicesButton).ButtonEnabledChanged += new EventHandler(this.runServicesButton_ButtonEnabledChanged);
    componentResourceManager.ApplyResources((object) this.tableMain, "tableMain");
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.labelTcmStatus, 1, 1);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.checkmarkTcmOnline, 0, 1);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.seekTimeListViewOutput, 0, 0);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.tableTransControls, 0, 3);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.digitalReadoutInstrument1, 2, 2);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 2);
    ((Control) this.tableMain).Name = "tableMain";
    this.labelTcmStatus.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.labelTcmStatus, 2);
    componentResourceManager.ApplyResources((object) this.labelTcmStatus, "labelTcmStatus");
    ((Control) this.labelTcmStatus).Name = "labelTcmStatus";
    this.labelTcmStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    componentResourceManager.ApplyResources((object) this.checkmarkTcmOnline, "checkmarkTcmOnline");
    ((Control) this.checkmarkTcmOnline).Name = "checkmarkTcmOnline";
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.seekTimeListViewOutput, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListViewOutput, "seekTimeListViewOutput");
    this.seekTimeListViewOutput.FilterUserLabels = true;
    ((Control) this.seekTimeListViewOutput).Name = "seekTimeListViewOutput";
    this.seekTimeListViewOutput.RequiredUserLabelPrefix = "vrduABAMisuseReset";
    this.seekTimeListViewOutput.SelectedTime = new DateTime?();
    this.seekTimeListViewOutput.ShowChannelLabels = false;
    this.seekTimeListViewOutput.ShowCommunicationsState = false;
    this.seekTimeListViewOutput.ShowControlPanel = false;
    this.seekTimeListViewOutput.ShowDeviceColumn = false;
    this.seekTimeListViewOutput.TimeFormat = "HH:mm:ss.f";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "VRDU01T", "RT_Delete_permanent_errors_Request_Results_Delete_Results");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "VRDU01T", "02FBFF");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableTransControls).ResumeLayout(false);
    ((ISupportInitialize) this.runServicesButton).EndInit();
    ((Control) this.tableMain).ResumeLayout(false);
    ((Control) this.tableMain).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
