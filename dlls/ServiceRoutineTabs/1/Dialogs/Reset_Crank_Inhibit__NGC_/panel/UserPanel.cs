// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Reset_Crank_Inhibit__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Reset_Crank_Inhibit__NGC_.panel;

public class UserPanel : CustomPanel
{
  private Channel ssam;
  private Channel ctp;
  private TableLayoutPanel tableMain;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelResetStatus;
  private Checkmark checkmarkEcusOnline;
  private SeekTimeListView seekTimeListViewOutput;
  private TableLayoutPanel tableTransControls;
  private RunServicesButton runServicesButton;
  private DigitalReadoutInstrument digitalReadoutInstrumentFlashInProgress;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrumentCrankInhibit;

  public UserPanel()
  {
    this.InitializeComponent();
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
    this.UpdateUserInterface();
  }

  public virtual void OnChannelsChanged()
  {
    this.SetSSAM(this.GetChannel("SSAM02T"));
    this.SetCTP(this.GetChannel("CTP01T"));
  }

  private void SetSSAM(Channel ssam)
  {
    if (this.ssam == ssam)
      return;
    this.ssam = ssam;
    this.UpdateUserInterface();
  }

  private void SetCTP(Channel ctp)
  {
    if (this.ctp == ctp)
      return;
    this.ctp = ctp;
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    this.checkmarkEcusOnline.Checked = this.Online;
    ((Control) this.runServicesButton).Enabled = this.CanReset;
    if (((RunSharedProcedureButtonBase) this.runServicesButton).InProgress)
      ((Control) this.labelResetStatus).Text = Resources.Message_DisablingCrankInhibit;
    else if (!this.Online)
      ((Control) this.labelResetStatus).Text = Resources.Message_EcusMustBeConnectedToDisableCrankInhibit;
    else if (this.digitalReadoutInstrumentCrankInhibit.RepresentedState == 3)
      ((Control) this.labelResetStatus).Text = Resources.Message_CrankingIsNotInhibited;
    else if (this.digitalReadoutInstrumentFlashInProgress.RepresentedState == 3)
      ((Control) this.labelResetStatus).Text = Resources.Message_FlashingOverTheAirIsCurrentlyInProgress;
    else
      ((Control) this.labelResetStatus).Text = Resources.Message_Ready;
  }

  private bool CanReset
  {
    get
    {
      return this.digitalReadoutInstrumentCrankInhibit.RepresentedState != 3 && this.digitalReadoutInstrumentFlashInProgress.RepresentedState != 3 && this.Online;
    }
  }

  private bool Online
  {
    get => this.ssam != null && this.ssam.Online && this.ctp != null && this.ctp.Online;
  }

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
      this.AddLogLabel(Resources.Message_CrankInhibitWasDisabled);
    else
      this.AddLogLabel(Resources.Message_UnableToDisableCrankInhibit);
    this.UpdateUserInterface();
  }

  private void runServicesButton_Starting(object sender, CancelEventArgs e)
  {
    this.AddLogLabel(Resources.Message_DisablingCrankInhibit);
    this.UpdateUserInterface();
  }

  private void runServicesButton_ButtonEnabledChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void runServicesButton_Started(object sender, PassFailResultEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrument1_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void digitalReadoutInstrument2_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableMain = new TableLayoutPanel();
    this.labelResetStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.checkmarkEcusOnline = new Checkmark();
    this.seekTimeListViewOutput = new SeekTimeListView();
    this.tableTransControls = new TableLayoutPanel();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.runServicesButton = new RunServicesButton();
    this.digitalReadoutInstrumentFlashInProgress = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCrankInhibit = new DigitalReadoutInstrument();
    ((Control) this.tableMain).SuspendLayout();
    ((Control) this.tableTransControls).SuspendLayout();
    ((ISupportInitialize) this.runServicesButton).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableMain, "tableMain");
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.labelResetStatus, 1, 1);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.checkmarkEcusOnline, 0, 1);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.seekTimeListViewOutput, 0, 0);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.tableTransControls, 0, 3);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.digitalReadoutInstrumentFlashInProgress, 2, 2);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.digitalReadoutInstrumentCrankInhibit, 0, 2);
    ((Control) this.tableMain).Name = "tableMain";
    this.labelResetStatus.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.labelResetStatus, 2);
    componentResourceManager.ApplyResources((object) this.labelResetStatus, "labelResetStatus");
    ((Control) this.labelResetStatus).Name = "labelResetStatus";
    this.labelResetStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    componentResourceManager.ApplyResources((object) this.checkmarkEcusOnline, "checkmarkEcusOnline");
    ((Control) this.checkmarkEcusOnline).Name = "checkmarkEcusOnline";
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.seekTimeListViewOutput, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListViewOutput, "seekTimeListViewOutput");
    this.seekTimeListViewOutput.FilterUserLabels = true;
    ((Control) this.seekTimeListViewOutput).Name = "seekTimeListViewOutput";
    this.seekTimeListViewOutput.RequiredUserLabelPrefix = "CrankInhibitReset";
    this.seekTimeListViewOutput.SelectedTime = new DateTime?();
    this.seekTimeListViewOutput.ShowChannelLabels = false;
    this.seekTimeListViewOutput.ShowCommunicationsState = false;
    this.seekTimeListViewOutput.ShowControlPanel = false;
    this.seekTimeListViewOutput.ShowDeviceColumn = false;
    this.seekTimeListViewOutput.TimeFormat = "HH:mm:ss.f";
    componentResourceManager.ApplyResources((object) this.tableTransControls, "tableTransControls");
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.tableTransControls, 3);
    ((TableLayoutPanel) this.tableTransControls).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 0);
    ((TableLayoutPanel) this.tableTransControls).Controls.Add((Control) this.runServicesButton, 1, 0);
    ((Control) this.tableTransControls).Name = "tableTransControls";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    this.digitalReadoutInstrument3.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrument3.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument3.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument3.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrument3.Gradient.Modify(4, 3.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "CTP01T", "RT_Reset_Crank_inhibition_Start_status");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.runServicesButton, "runServicesButton");
    ((Control) this.runServicesButton).Name = "runServicesButton";
    this.runServicesButton.ServiceCalls.Add(new ServiceCall("CTP01T", "RT_Reset_Crank_inhibition_Start_status", (IEnumerable<string>) new string[1]
    {
      "InhibitionStatus=0"
    }));
    this.runServicesButton.Complete += new EventHandler<PassFailResultEventArgs>(this.runServicesButton_Complete);
    ((RunSharedProcedureButtonBase) this.runServicesButton).Starting += new EventHandler<CancelEventArgs>(this.runServicesButton_Starting);
    ((RunSharedProcedureButtonBase) this.runServicesButton).Started += new EventHandler<PassFailResultEventArgs>(this.runServicesButton_Started);
    ((RunSharedProcedureButtonBase) this.runServicesButton).ButtonEnabledChanged += new EventHandler(this.runServicesButton_ButtonEnabledChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFlashInProgress, "digitalReadoutInstrumentFlashInProgress");
    this.digitalReadoutInstrumentFlashInProgress.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFlashInProgress).FreezeValue = false;
    this.digitalReadoutInstrumentFlashInProgress.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentFlashInProgress.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentFlashInProgress.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentFlashInProgress.Gradient.Modify(3, 2.0, (ValueState) 2);
    this.digitalReadoutInstrumentFlashInProgress.Gradient.Modify(4, 3.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFlashInProgress).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_FOA_Diagnostic_Displayables_DDFOA_FOTA_InProcess");
    ((Control) this.digitalReadoutInstrumentFlashInProgress).Name = "digitalReadoutInstrumentFlashInProgress";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFlashInProgress).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentFlashInProgress.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument1_RepresentedStateChanged);
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.digitalReadoutInstrumentCrankInhibit, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCrankInhibit, "digitalReadoutInstrumentCrankInhibit");
    this.digitalReadoutInstrumentCrankInhibit.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCrankInhibit).FreezeValue = false;
    this.digitalReadoutInstrumentCrankInhibit.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentCrankInhibit.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentCrankInhibit.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentCrankInhibit.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentCrankInhibit.Gradient.Modify(4, 3.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCrankInhibit).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_FOA_Diagnostic_Displayables_DDFOA_CrankIntrlService_Cmd");
    ((Control) this.digitalReadoutInstrumentCrankInhibit).Name = "digitalReadoutInstrumentCrankInhibit";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCrankInhibit).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentCrankInhibit.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument2_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableMain).ResumeLayout(false);
    ((Control) this.tableMain).PerformLayout();
    ((Control) this.tableTransControls).ResumeLayout(false);
    ((ISupportInitialize) this.runServicesButton).EndInit();
    ((Control) this).ResumeLayout(false);
  }
}
