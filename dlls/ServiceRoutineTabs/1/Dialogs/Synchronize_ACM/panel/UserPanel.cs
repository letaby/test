// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Synchronize_ACM.panel.UserPanel
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Synchronize_ACM.panel;

public class UserPanel : CustomPanel
{
  private const string UnlockSharedProcedureFormat = "SP_SecurityUnlock_ACM21T_UnlockXN";
  private const string StartMarriageProcedure = "RT_Start_ECU_Marriage_Routine_Start";
  private const string MonitorMarriageProcedure = "RT_Start_ECU_Marriage_Routine_Request_Results_ECU_Marriage_Routine_Status_Byte";
  private const string StopMarriageProcedure = "RT_Start_ECU_Marriage_Routine_Stop";
  private const string DeleteFaults = "RT_SR087_Delete_Non_Erasable_FC_Start";
  private const string VinQualifier = "CO_VIN";
  private UserPanel.State currentState;
  private Timer monitorMarriageTimer = new Timer();
  private Channel mcm21t;
  private Channel acm;
  private bool hasBeenProgrammed = false;
  private TableLayoutPanel tableLayoutPanelMain;
  private DigitalReadoutInstrument digitalReadoutInstrumentAcmFault;
  private Panel panelSyncAcm;
  private TableLayoutPanel tableLayoutPanelSyncAcm;
  private SeekTimeListView seekTimeListView1;
  private Button buttonSynchronize;
  private Checkmark checkmarkSync;
  private System.Windows.Forms.Label labelSyncStatus;
  private Button buttonClose;

  private bool RequiresUnlock => this.digitalReadoutInstrumentAcmFault.RepresentedState != 1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.monitorMarriageTimer.Interval = 1000;
    this.monitorMarriageTimer.Tick += new EventHandler(this.marriageTimer_Tick);
    this.currentState = UserPanel.State.Unknown;
    base.OnChannelsChanged();
    string acmVin = this.acm != null ? this.acm.EcuInfos["CO_VIN"].Value : string.Empty;
    string mcmVin = this.acm != null ? this.mcm21t.EcuInfos["CO_VIN"].Value : string.Empty;
    DateTime dateTime = DateTime.Now;
    dateTime = dateTime.Date;
    string today = dateTime.ToString("MM/dd/yyyy");
    this.hasBeenProgrammed = ((IEnumerable<string>) File.ReadAllLines(Directories.StationLogFile)).Any<string>((Func<string, bool>) (line => line.Contains("Replace") && ((line.Contains("ACM21T") || line.Contains("ACM301T")) && line.Contains(acmVin) || line.Contains("MCM21T") && line.Contains(mcmVin)) && line.Contains(today)));
    this.UpdateUI();
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.currentState != UserPanel.State.Unknown && this.currentState != UserPanel.State.Done;
    if (e.Cancel)
      return;
    this.monitorMarriageTimer.Tick -= new EventHandler(this.marriageTimer_Tick);
    this.monitorMarriageTimer.Dispose();
  }

  private void UpdateUI()
  {
    this.buttonSynchronize.Enabled = this.checkmarkSync.Checked = this.ReadyToStart();
  }

  private bool ReadyToStart()
  {
    if (this.currentState != UserPanel.State.Unknown && this.currentState != UserPanel.State.Done)
    {
      this.labelSyncStatus.Text = Resources.Message_Running;
      return false;
    }
    if (this.mcm21t == null)
    {
      this.labelSyncStatus.Text = Resources.Message_MCMOffline;
      return false;
    }
    if (this.acm == null)
    {
      this.labelSyncStatus.Text = Resources.Message_ACMOffline;
      return false;
    }
    if (this.mcm21t.EcuInfos["CO_VIN"].Value != this.acm.EcuInfos["CO_VIN"].Value)
    {
      this.labelSyncStatus.Text = Resources.Message_VINsNotSynchronized;
      return false;
    }
    if (!this.hasBeenProgrammed)
    {
      this.labelSyncStatus.Text = Resources.Message_UseDiagnosticLinkToProgramTheDevice;
      return false;
    }
    this.labelSyncStatus.Text = Resources.Message_Ready;
    return true;
  }

  private bool SetChannelMcm(Channel channel)
  {
    if (this.mcm21t != channel)
    {
      this.currentState = UserPanel.State.Unknown;
      this.mcm21t = channel;
    }
    return this.mcm21t != null;
  }

  private bool SetChannelAcm(Channel channel)
  {
    if (this.acm != channel)
    {
      this.currentState = UserPanel.State.Unknown;
      this.acm = channel;
      if (this.acm != null)
        ((SingleInstrumentBase) this.digitalReadoutInstrumentAcmFault).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, this.acm.Ecu.Name, "ED000D");
    }
    return this.acm != null;
  }

  private bool RunService(
    Channel channel,
    string serviceQualifier,
    ServiceCompleteEventHandler serviceCompleteEventHandler)
  {
    if (channel != null && channel.Online)
    {
      Service service = channel.Services[serviceQualifier];
      if (service != (Service) null)
      {
        if (serviceCompleteEventHandler != null)
          service.ServiceCompleteEvent += serviceCompleteEventHandler;
        service.Execute(false);
        return true;
      }
    }
    this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ServiceCouldNotBeStarted01, (object) channel.Ecu.Name, (object) serviceQualifier));
    return false;
  }

  public virtual void OnChannelsChanged()
  {
    this.SetChannelMcm(this.GetChannel("MCM21T", (CustomPanel.ChannelLookupOptions) 1));
    if (!this.SetChannelAcm(this.GetChannel("ACM21T", (CustomPanel.ChannelLookupOptions) 1)))
      this.SetChannelAcm(this.GetChannel("ACM301T", (CustomPanel.ChannelLookupOptions) 1));
    this.UpdateUI();
  }

  private void buttonSynchronize_Click(object sender, EventArgs e)
  {
    this.UpdateUI();
    if (this.RequiresUnlock)
      this.SetState(UserPanel.State.Starting);
    else
      this.SetState(UserPanel.State.StartMarriage);
  }

  private void GoMachine()
  {
    switch (this.currentState)
    {
      case UserPanel.State.Starting:
        this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_Unlocking0, (object) this.acm.Ecu.Name));
        this.currentState = UserPanel.State.ServerUnlock;
        this.PerformServerUnlock();
        break;
      case UserPanel.State.StartMarriage:
        this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_StartingMarriage0, (object) this.acm.Ecu.Name));
        this.StartMarriage();
        break;
      case UserPanel.State.Marrying:
        this.MonitorMarriage();
        break;
      case UserPanel.State.StopMarriage:
        this.StopMarriage();
        break;
      case UserPanel.State.ReadyToClearFaults:
        this.UpdateStatus(Resources.Message_ClearingCodes);
        this.PerformFaultCodeClear();
        break;
      case UserPanel.State.FaultsCleared:
        this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0CodesCleared, (object) this.acm.Ecu.Name));
        this.SetState(UserPanel.State.Success);
        break;
      case UserPanel.State.Success:
        this.UpdateStatus(Resources.Message_TurnIgnitionOffThenTurnIgnitionOn);
        if (this.RequiresUnlock)
          this.UpdateStatus(Resources.Message_OnceThisIsDoneFaultCodeShouldBeCleared);
        this.SetState(UserPanel.State.Done);
        break;
    }
    this.UpdateUI();
  }

  private void UpdateStatus(string message)
  {
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, message);
  }

  private void SetState(UserPanel.State newState)
  {
    if (newState == this.currentState)
      return;
    this.currentState = newState;
    this.GoMachine();
  }

  private void PerformServerUnlock()
  {
    SharedProcedureBase availableProcedure = SharedProcedureBase.AvailableProcedures["SP_SecurityUnlock_ACM21T_UnlockXN"];
    if (availableProcedure != null)
    {
      if (availableProcedure.CanStart)
      {
        this.SetState(UserPanel.State.WaitingForServerConnection);
        availableProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StartComplete);
        availableProcedure.Start();
        return;
      }
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ReferencedSharedProcedureWasFoundButItCouldNotBeStarted0, (object) availableProcedure.Name));
    }
    else
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ReferencedSharedProcedureWasNotFound0, (object) "SP_SecurityUnlock_ACM21T_UnlockXN"));
    this.SetState(UserPanel.State.Done);
  }

  private void unlockSharedProcedure_StartComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StartComplete);
    if (((ResultEventArgs) e).Succeeded && e.Result == 1)
    {
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0UnlockViaTheServerWasInitiatedUsingProcedure1, (object) this.acm.Ecu.Name, (object) sharedProcedureBase.Name));
      sharedProcedureBase.StopComplete += new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StopComplete);
    }
    else
    {
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ReferencedSharedProcedure0FailedAtStart1, (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty));
      this.SetState(UserPanel.State.Done);
    }
  }

  private void unlockSharedProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    SharedProcedureBase sharedProcedureBase = sender as SharedProcedureBase;
    sharedProcedureBase.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.unlockSharedProcedure_StopComplete);
    if (!((ResultEventArgs) e).Succeeded || e.Result == 0)
    {
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ReferencedSharedProcedure0Failed1, (object) sharedProcedureBase.Name, ((ResultEventArgs) e).Exception != null ? (object) ((ResultEventArgs) e).Exception.Message : (object) string.Empty));
      this.SetState(UserPanel.State.Done);
    }
    else
    {
      this.UpdateStatus(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0WasUnlockedViaTheServerUsingProcedure1, (object) this.acm.Ecu.Name, (object) sharedProcedureBase.Name));
      this.SetState(UserPanel.State.StartMarriage);
    }
  }

  private void StartMarriage()
  {
    if (this.RunService(this.acm, "RT_Start_ECU_Marriage_Routine_Start", new ServiceCompleteEventHandler(this.StartMarriageServiceComplete)))
      return;
    this.SetState(UserPanel.State.Done);
  }

  private void StartMarriageServiceComplete(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.StartMarriageServiceComplete);
    if (e.Succeeded)
    {
      this.SetState(UserPanel.State.Marrying);
    }
    else
    {
      this.UpdateStatus(Resources.Message_MarriageCouldNotBeStarted);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void MonitorMarriage()
  {
    if (this.RunService(this.acm, "RT_Start_ECU_Marriage_Routine_Request_Results_ECU_Marriage_Routine_Status_Byte", new ServiceCompleteEventHandler(this.MonitorMarriageServiceComplete)))
      return;
    this.monitorMarriageTimer.Enabled = false;
    this.SetState(UserPanel.State.Done);
  }

  private void MonitorMarriageServiceComplete(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.MonitorMarriageServiceComplete);
    Choice choice;
    if (e.Succeeded && service.OutputValues != null && service.OutputValues[0] != null && (choice = (Choice) service.OutputValues[0].Value) != (object) null)
    {
      switch (choice.Index)
      {
        case 1:
          this.SetState(UserPanel.State.StopMarriage);
          break;
        case 2:
          this.monitorMarriageTimer.Enabled = true;
          break;
        default:
          this.UpdateStatus(Resources.Message_MarriageFailed);
          this.SetState(UserPanel.State.Done);
          break;
      }
    }
    else
    {
      this.UpdateStatus(Resources.Message_MarriageCouldNotBeMonitored);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void marriageTimer_Tick(object sender, EventArgs e)
  {
    this.monitorMarriageTimer.Enabled = false;
    this.MonitorMarriage();
  }

  private void StopMarriage()
  {
    if (this.RunService(this.acm, "RT_Start_ECU_Marriage_Routine_Stop", new ServiceCompleteEventHandler(this.StopMarriageServiceComplete)))
      return;
    this.SetState(UserPanel.State.Done);
  }

  private void StopMarriageServiceComplete(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.StopMarriageServiceComplete);
    if (e.Succeeded)
    {
      if (this.RequiresUnlock)
        this.SetState(UserPanel.State.ReadyToClearFaults);
      else
        this.SetState(UserPanel.State.Success);
    }
    else
    {
      this.UpdateStatus(Resources.Message_MarriageCouldNotBeStopped);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void PerformFaultCodeClear()
  {
    if (this.RunService(this.acm, "RT_SR087_Delete_Non_Erasable_FC_Start", new ServiceCompleteEventHandler(this.DeleteNonErasableFCServiceComplete)))
      this.SetState(UserPanel.State.WaitingForFaultsToClear);
    else
      this.SetState(UserPanel.State.Done);
  }

  private void DeleteNonErasableFCServiceComplete(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.DeleteNonErasableFCServiceComplete);
    if (e.Succeeded)
    {
      this.SetState(UserPanel.State.FaultsCleared);
    }
    else
    {
      this.UpdateStatus(Resources.Message_FaultsCouldNotBeCleared);
      this.SetState(UserPanel.State.Done);
    }
  }

  private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.panelSyncAcm = new Panel();
    this.tableLayoutPanelSyncAcm = new TableLayoutPanel();
    this.digitalReadoutInstrumentAcmFault = new DigitalReadoutInstrument();
    this.checkmarkSync = new Checkmark();
    this.labelSyncStatus = new System.Windows.Forms.Label();
    this.buttonSynchronize = new Button();
    this.seekTimeListView1 = new SeekTimeListView();
    this.buttonClose = new Button();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    this.panelSyncAcm.SuspendLayout();
    ((Control) this.tableLayoutPanelSyncAcm).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.panelSyncAcm, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.seekTimeListView1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClose, 1, 3);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    this.panelSyncAcm.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.panelSyncAcm, 2);
    this.panelSyncAcm.Controls.Add((Control) this.tableLayoutPanelSyncAcm);
    componentResourceManager.ApplyResources((object) this.panelSyncAcm, "panelSyncAcm");
    this.panelSyncAcm.Name = "panelSyncAcm";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelSyncAcm, "tableLayoutPanelSyncAcm");
    ((TableLayoutPanel) this.tableLayoutPanelSyncAcm).Controls.Add((Control) this.digitalReadoutInstrumentAcmFault, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelSyncAcm).Controls.Add((Control) this.checkmarkSync, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelSyncAcm).Controls.Add((Control) this.labelSyncStatus, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelSyncAcm).Controls.Add((Control) this.buttonSynchronize, 3, 1);
    ((Control) this.tableLayoutPanelSyncAcm).Name = "tableLayoutPanelSyncAcm";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentAcmFault, "digitalReadoutInstrumentAcmFault");
    ((TableLayoutPanel) this.tableLayoutPanelSyncAcm).SetColumnSpan((Control) this.digitalReadoutInstrumentAcmFault, 4);
    this.digitalReadoutInstrumentAcmFault.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAcmFault).FreezeValue = false;
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText15"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText16"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText17"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText18"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText19"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText20"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText21"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText22"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText23"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText24"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText25"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText26"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText27"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText28"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText29"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText30"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText31"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText32"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText33"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText34"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText35"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText36"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText37"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText38"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText39"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText40"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText41"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText42"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText43"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText44"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText45"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText46"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText47"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText48"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText49"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText50"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText51"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText52"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText53"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText54"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText55"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText56"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText57"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText58"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText59"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText60"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText61"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText62"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText63"));
    this.digitalReadoutInstrumentAcmFault.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText64"));
    this.digitalReadoutInstrumentAcmFault.Gradient.Initialize((ValueState) 2, 64 /*0x40*/);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(3, 4.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(4, 5.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(5, 8.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(6, 9.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(7, 12.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(8, 13.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(9, 32.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(10, 33.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(11, 36.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(12, 37.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(13, 40.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(14, 41.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(15, 44.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(16 /*0x10*/, 45.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(17, 128.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(18, 129.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(19, 132.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(20, 133.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(21, 136.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(22, 137.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(23, 140.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(24, 141.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(25, 160.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(26, 161.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(27, 164.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(28, 165.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(29, 168.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(30, 169.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(31 /*0x1F*/, 172.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(32 /*0x20*/, 173.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(33, 256.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(34, 257.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(35, 260.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(36, 261.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(37, 264.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(38, 265.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(39, 268.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(40, 269.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(41, 288.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(42, 289.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(43, 292.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(44, 293.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(45, 296.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(46, 297.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(47, 300.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(48 /*0x30*/, 301.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(49, 384.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(50, 385.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(51, 388.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(52, 389.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(53, 392.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(54, 393.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(55, 396.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(56, 397.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(57, 416.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(58, 417.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(59, 420.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(60, 421.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(61, 424.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(62, 425.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(63 /*0x3F*/, 428.0, (ValueState) 2);
    this.digitalReadoutInstrumentAcmFault.Gradient.Modify(64 /*0x40*/, 429.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAcmFault).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "ACM21T", "ED000D");
    ((Control) this.digitalReadoutInstrumentAcmFault).Name = "digitalReadoutInstrumentAcmFault";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAcmFault).ShowUnits = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAcmFault).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentAcmFault).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentAcmFault.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.checkmarkSync, "checkmarkSync");
    ((Control) this.checkmarkSync).Name = "checkmarkSync";
    componentResourceManager.ApplyResources((object) this.labelSyncStatus, "labelSyncStatus");
    ((TableLayoutPanel) this.tableLayoutPanelSyncAcm).SetColumnSpan((Control) this.labelSyncStatus, 2);
    this.labelSyncStatus.Name = "labelSyncStatus";
    this.labelSyncStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonSynchronize, "buttonSynchronize");
    this.buttonSynchronize.Name = "buttonSynchronize";
    this.buttonSynchronize.UseCompatibleTextRendering = true;
    this.buttonSynchronize.UseVisualStyleBackColor = true;
    this.buttonSynchronize.Click += new EventHandler(this.buttonSynchronize_Click);
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.seekTimeListView1, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "Synchronize ACM";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    this.panelSyncAcm.ResumeLayout(false);
    ((Control) this.tableLayoutPanelSyncAcm).ResumeLayout(false);
    ((Control) this.tableLayoutPanelSyncAcm).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum State
  {
    Unknown,
    Starting,
    ServerUnlock,
    WaitingForServerConnection,
    StartMarriage,
    Marrying,
    StopMarriage,
    ReadyToClearFaults,
    WaitingForFaultsToClear,
    FaultsCleared,
    Success,
    Done,
  }
}
