// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Check_VIN_Synchronization.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Adr;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Check_VIN_Synchronization.panel;

public class UserPanel : CustomPanel
{
  private const string VINMasterProperty = "VINMaster";
  private const string VINParameter = "VIN";
  private const string VINEcuInfo = "CO_VIN";
  private UserPanel.Stage currentStage = UserPanel.Stage.Idle;
  private Dictionary<string, object> parameters = new Dictionary<string, object>();
  private List<Channel> channelsToWorkWith = new List<Channel>();
  private List<Channel> channelsToWriteVINsFor = new List<Channel>();
  private List<Channel> channelsToWaitForReconnect = new List<Channel>();
  private Channel vinMasterChannel = (Channel) null;
  private bool duplicateVinMasterError = false;
  private List<Ecu> manualConnectEcus = new List<Ecu>();
  private bool? wasAutoConnecting;
  private Checkmark checkmarkFaultPresent;
  private System.Windows.Forms.Label labelFault;
  private TextBox textBoxOutput;
  private Button buttonClose;
  private ListViewEx listViewVins;
  private ColumnHeader columnEcu;
  private ColumnHeader columnVin;
  private Button buttonStart;

  public UserPanel()
  {
    this.InitializeComponent();
    this.buttonStart.Click += new EventHandler(this.OnStartClick);
    this.currentStage = UserPanel.Stage.Idle;
    ConnectionManager.GlobalInstance.ConnectionChanged += new EventHandler<IgnitionStatusEventArgs>(this.ConnectionManager_ConnectionChanged);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    base.OnChannelsChanged();
  }

  private void CheckVinMaster(Channel channel)
  {
    if (!channel.Ecu.Properties.ContainsKey("VINMaster"))
      return;
    bool result = false;
    if (bool.TryParse(channel.Ecu.Properties["VINMaster"], out result) && result)
    {
      if (this.vinMasterChannel == null)
      {
        this.vinMasterChannel = channel;
        this.duplicateVinMasterError = false;
      }
      else
      {
        this.vinMasterChannel = (Channel) null;
        this.duplicateVinMasterError = true;
      }
    }
  }

  public virtual void OnChannelsChanged()
  {
    foreach (Channel channel in this.channelsToWorkWith)
      channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    this.vinMasterChannel = (Channel) null;
    this.channelsToWorkWith.Clear();
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
    {
      if (channel.Parameters["VIN"] != null && channel.EcuInfos["CO_VIN"] != null)
      {
        channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        this.channelsToWorkWith.Add(channel);
        if (channel.CommunicationsState == CommunicationsState.Online)
          channel.EcuInfos["CO_VIN"].Read(false);
        this.CheckVinMaster(channel);
      }
    }
    switch (this.CurrentStage)
    {
      case UserPanel.Stage.WaitForIgnitionOffDisconnection:
        if (this.channelsToWorkWith.Count != 0 && ConnectionManager.GlobalInstance.IgnitionStatus != 1)
          break;
        this.CurrentStage = UserPanel.Stage.WaitForIgnitionOnReconnection;
        this.PerformCurrentStage();
        break;
      case UserPanel.Stage.WaitForIgnitionOnReconnection:
        int num = 0;
        foreach (Channel channel1 in this.channelsToWaitForReconnect)
        {
          foreach (Channel channel2 in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
          {
            if (channel2.Ecu.Name.Equals(channel1.Ecu.Name))
            {
              ++num;
              break;
            }
          }
        }
        if (num != this.channelsToWaitForReconnect.Count || ConnectionManager.GlobalInstance.IgnitionStatus != 0)
          break;
        this.channelsToWaitForReconnect.Clear();
        this.CurrentStage = UserPanel.Stage.Finish;
        this.PerformCurrentStage();
        break;
      default:
        if (this.Working && this.channelsToWorkWith.Count == 0)
        {
          this.StopWork(UserPanel.Reason.Disconnected);
          break;
        }
        this.UpdateUserInterface();
        break;
    }
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.StopWork(UserPanel.Reason.Closing);
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    foreach (Channel channel in this.channelsToWorkWith)
      channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    ConnectionManager.GlobalInstance.ConnectionChanged -= new EventHandler<IgnitionStatusEventArgs>(this.ConnectionManager_ConnectionChanged);
  }

  private void OnStartClick(object sender, EventArgs e)
  {
    this.ClearOutput();
    this.CurrentStage = UserPanel.Stage.WriteVins;
    this.PerformCurrentStage();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnParametersWriteComplete(object sender, ResultEventArgs e)
  {
    ParameterCollection parameterCollection = sender as ParameterCollection;
    parameterCollection.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
    this.channelsToWriteVINsFor.Remove(parameterCollection.Channel);
    if (e.Succeeded)
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_SuccessfullyWroteVINFor0, (object) parameterCollection.Channel.Ecu.Name));
    else
      this.ReportResult(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_FailedToWriteVINFor0, (object) parameterCollection.Channel.Ecu.Name));
    if (this.channelsToWriteVINsFor.Count != 0)
      return;
    this.CurrentStage = UserPanel.Stage.WaitForIgnitionOffDisconnection;
    this.PerformCurrentStage();
  }

  private bool CanClose
  {
    get => !this.Working || this.currentStage == UserPanel.Stage.WaitForIgnitionOnReconnection;
  }

  private bool Online
  {
    get
    {
      foreach (Channel channel in this.channelsToWorkWith)
      {
        if (channel.Online)
          return true;
      }
      return false;
    }
  }

  private bool FaultIsPresent
  {
    get
    {
      string masterVin = this.MasterVIN;
      if (string.IsNullOrEmpty(masterVin))
        return true;
      foreach (Channel channel in this.channelsToWorkWith)
      {
        string identificationNumber = SapiManager.GetVehicleIdentificationNumber(channel);
        if (!masterVin.Equals(identificationNumber))
          return true;
      }
      return false;
    }
  }

  private bool AnEcuIsBusy
  {
    get
    {
      foreach (Channel channel in this.channelsToWorkWith)
      {
        if (channel.Online && channel.CommunicationsState != CommunicationsState.Online)
          return true;
      }
      return false;
    }
  }

  private bool CanStart
  {
    get
    {
      return !this.Working && this.Online && this.FaultIsPresent && !this.AnEcuIsBusy && !string.IsNullOrEmpty(this.MasterVIN);
    }
  }

  private UserPanel.Stage CurrentStage
  {
    get => this.currentStage;
    set
    {
      if (this.currentStage == value)
        return;
      this.currentStage = value;
      this.UpdateUserInterface();
      Application.DoEvents();
    }
  }

  private bool Working => this.currentStage != UserPanel.Stage.Idle;

  private string MasterVIN
  {
    get
    {
      return this.vinMasterChannel != null ? SapiManager.GetVehicleIdentificationNumber(this.vinMasterChannel) : (string) null;
    }
  }

  private void ClearOutput() => this.textBoxOutput.Text = string.Empty;

  private void ReportResult(string text)
  {
    this.textBoxOutput.Text = $"{this.textBoxOutput.Text}{text}\r\n";
    this.textBoxOutput.SelectionStart = this.textBoxOutput.TextLength;
    this.textBoxOutput.SelectionLength = 0;
    this.textBoxOutput.ScrollToCaret();
  }

  private void UpdateUserInterface()
  {
    if (this.CurrentStage == UserPanel.Stage.Stopping)
      return;
    this.buttonStart.Enabled = this.CanStart;
    this.buttonClose.Enabled = this.CanClose;
    ((ListView) this.listViewVins).Items.Clear();
    if (this.Online)
    {
      foreach (Channel channel in this.channelsToWorkWith)
      {
        ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(channel.Ecu.Name);
        ((ListViewItem) listViewExGroupItem).SubItems.Add(SapiManager.GetVehicleIdentificationNumber(channel));
        if (channel == this.vinMasterChannel)
          ((ListViewItem) listViewExGroupItem).Font = new Font(((ListViewItem) listViewExGroupItem).Font.FontFamily, ((ListViewItem) listViewExGroupItem).Font.Size, FontStyle.Bold);
        ((ListView) this.listViewVins).Items.Add((ListViewItem) listViewExGroupItem);
      }
    }
    if (!this.Working)
    {
      if (this.Online)
      {
        this.checkmarkFaultPresent.Checked = !this.FaultIsPresent;
        if (this.FaultIsPresent)
        {
          if (this.vinMasterChannel != null)
          {
            if (!string.IsNullOrEmpty(this.MasterVIN))
              this.labelFault.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_TheVINsInThisVehicleAreNotSynchronizedClickStartToCopyTheVINFrom0ToTheOtherDevices, (object) this.vinMasterChannel.Ecu.Name);
            else
              this.labelFault.Text = Resources.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseTheVINMasterDeviceDoesNotHaveAVIN;
          }
          else if (this.duplicateVinMasterError)
            this.labelFault.Text = Resources.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseMultipleVINMasterDevicesAreDefinedAndConnected;
          else
            this.labelFault.Text = Resources.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseNoVINMasterDeviceIsDefinedOrConnected;
        }
        else if (this.channelsToWorkWith.Count < 2)
          this.labelFault.Text = Resources.Message_ConnectAtLeastTwoDevicesToDetermineVINSynchronization;
        else
          this.labelFault.Text = Resources.Message_TheVINsInThisVehicleAreSynchronizedNoActionIsNecessaryIfTheVINIsIncorrectYouWillNeedToReprogramUsingServerData;
      }
      else
      {
        this.checkmarkFaultPresent.Checked = false;
        this.labelFault.Text = Resources.Message_ThereAreNoDevicesOnline;
      }
    }
    else
    {
      this.checkmarkFaultPresent.Checked = false;
      switch (this.CurrentStage)
      {
        case UserPanel.Stage.WaitForIgnitionOffDisconnection:
          if (this.channelsToWorkWith.Count < this.channelsToWaitForReconnect.Count)
          {
            this.labelFault.Text = Resources.Message_WaitingForRemainingDevicesToShutdownPleaseWait;
            break;
          }
          this.labelFault.Text = Resources.Message_PleaseTurnTheIgnitionOffAndWait;
          break;
        case UserPanel.Stage.WaitForIgnitionOnReconnection:
          if (this.channelsToWorkWith.Count == 0 || ConnectionManager.GlobalInstance.IgnitionStatus == 1)
          {
            this.labelFault.Text = Resources.Message_PleaseTurnTheIgnitionOnAndWait;
            break;
          }
          this.labelFault.Text = Resources.Message_WaitingForRemainingDevicesToComeOnlinePleaseWait;
          break;
        default:
          this.labelFault.Text = Resources.Message_ProcessingPleaseWait;
          break;
      }
    }
  }

  private void WriteVins()
  {
    string masterVin = this.MasterVIN;
    if (!string.IsNullOrEmpty(masterVin))
    {
      foreach (Channel channel in this.channelsToWorkWith)
      {
        string identificationNumber = SapiManager.GetVehicleIdentificationNumber(channel);
        if (!masterVin.Equals(identificationNumber))
        {
          this.ReportResult(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_UpdatingVINFor0, (object) channel.Ecu.Name));
          channel.Parameters["VIN"].Value = (object) masterVin;
          channel.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
          this.channelsToWriteVINsFor.Add(channel);
          channel.Parameters.Write(false);
        }
      }
      if (this.channelsToWriteVINsFor.Count != 0)
        return;
      this.StopWork(UserPanel.Reason.NoVinsChanged);
    }
    else
      this.StopWork(UserPanel.Reason.NoVinMaster);
  }

  private void TurnOffAutoConnect()
  {
    this.wasAutoConnecting = new bool?(SapiManager.GlobalInstance.Sapi.Channels.AutoConnecting);
    Cursor.Current = Cursors.WaitCursor;
    SapiManager.GlobalInstance.Sapi.Channels.StopAutoConnect();
    Cursor.Current = Cursors.Default;
    foreach (Channel channel in this.channelsToWorkWith)
    {
      this.channelsToWaitForReconnect.Add(channel);
      if (!channel.Ecu.MarkedForAutoConnect)
      {
        this.manualConnectEcus.Add(channel.Ecu);
        channel.Ecu.MarkedForAutoConnect = true;
      }
    }
  }

  private void RestoreAutoConnectState()
  {
    foreach (Ecu manualConnectEcu in this.manualConnectEcus)
      manualConnectEcu.MarkedForAutoConnect = false;
    this.manualConnectEcus.Clear();
    if (!this.wasAutoConnecting.HasValue)
      return;
    Cursor.Current = Cursors.WaitCursor;
    SapiManager.GlobalInstance.Sapi.Channels.StopAutoConnect();
    if (this.wasAutoConnecting.Value)
      SapiManager.GlobalInstance.Sapi.Channels.StartAutoConnect(1);
    this.wasAutoConnecting = new bool?();
    Cursor.Current = Cursors.Default;
  }

  private void PerformCurrentStage()
  {
    if (this.Online || this.CurrentStage == UserPanel.Stage.WaitForIgnitionOffDisconnection || this.CurrentStage == UserPanel.Stage.WaitForIgnitionOnReconnection)
    {
      switch (this.CurrentStage)
      {
        case UserPanel.Stage.WriteVins:
          this.WriteVins();
          break;
        case UserPanel.Stage.WaitForIgnitionOffDisconnection:
          this.ReportResult(Resources.Message_WaitingForDevicesToDisconnect);
          this.TurnOffAutoConnect();
          break;
        case UserPanel.Stage.WaitForIgnitionOnReconnection:
          this.ReportResult(Resources.Message_WaitingForDevicesToReconnect);
          SapiManager.GlobalInstance.Sapi.Channels.StartAutoConnect();
          break;
        case UserPanel.Stage.Finish:
          foreach (Channel channel in SapiManager.GlobalInstance.ActiveChannels.Where<Channel>((Func<Channel, bool>) (x => x.Ecu.Name.StartsWith("MCM") || x.Ecu.Name.StartsWith("ACM"))))
            channel.FaultCodes.Reset(false);
          this.StopWork(UserPanel.Reason.Succeeded);
          break;
        case UserPanel.Stage.Unknown:
          this.StopWork(UserPanel.Reason.UnknownStage);
          break;
      }
    }
    else
      this.StopWork(UserPanel.Reason.Disconnected);
  }

  private void StopWork(UserPanel.Reason reason)
  {
    if (this.CurrentStage == UserPanel.Stage.Stopping || this.CurrentStage == UserPanel.Stage.Idle)
      return;
    UserPanel.Stage currentStage = this.CurrentStage;
    this.CurrentStage = UserPanel.Stage.Stopping;
    switch (reason)
    {
      case UserPanel.Reason.Succeeded:
        if (currentStage != UserPanel.Stage.Finish)
          throw new InvalidOperationException();
        this.ReportResult(Resources.Message_Complete);
        this.buttonStart.Text = Resources.Message_Start;
        break;
      case UserPanel.Reason.NoVinsChanged:
        this.ReportResult(Resources.Message_TheProcedureFailedToComplete);
        switch (reason - 1)
        {
          case UserPanel.Reason.Succeeded:
            this.ReportResult(Resources.Message_TheOperationWasAborted);
            break;
          case UserPanel.Reason.Closing:
            this.ReportResult(Resources.Message_ADeviceWasDisconnected);
            break;
          case UserPanel.Reason.Disconnected:
            this.ReportResult(Resources.Message_UnknownStage);
            break;
          case UserPanel.Reason.UnknownStage:
            this.ReportResult(Resources.Message_ItWasNotNecessaryToChangeAnyVINs);
            break;
          case UserPanel.Reason.NoVinsChanged:
            this.ReportResult(Resources.Message_NoVINWasFoundInTheVINMasterDevice);
            break;
        }
        break;
      default:
        this.buttonStart.Text = Resources.Message_Retry;
        goto case UserPanel.Reason.NoVinsChanged;
    }
    this.RestoreAutoConnectState();
    this.channelsToWaitForReconnect.Clear();
    this.channelsToWriteVINsFor.Clear();
    this.CurrentStage = UserPanel.Stage.Idle;
  }

  private void ConnectionManager_ConnectionChanged(object sender, IgnitionStatusEventArgs e)
  {
    base.OnChannelsChanged();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.checkmarkFaultPresent = new Checkmark();
    this.labelFault = new System.Windows.Forms.Label();
    this.textBoxOutput = new TextBox();
    this.buttonClose = new Button();
    this.buttonStart = new Button();
    this.listViewVins = new ListViewEx();
    this.columnEcu = new ColumnHeader();
    this.columnVin = new ColumnHeader();
    ((ISupportInitialize) this.listViewVins).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.checkmarkFaultPresent, "checkmarkFaultPresent");
    ((Control) this.checkmarkFaultPresent).Name = "checkmarkFaultPresent";
    componentResourceManager.ApplyResources((object) this.labelFault, "labelFault");
    this.labelFault.Name = "labelFault";
    this.labelFault.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxOutput, "textBoxOutput");
    this.textBoxOutput.Name = "textBoxOutput";
    this.textBoxOutput.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.Cancel;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    this.listViewVins.CanDelete = false;
    ((ListView) this.listViewVins).Columns.AddRange(new ColumnHeader[2]
    {
      this.columnEcu,
      this.columnVin
    });
    this.listViewVins.EditableColumn = -1;
    this.listViewVins.GridLines = true;
    componentResourceManager.ApplyResources((object) this.listViewVins, "listViewVins");
    ((Control) this.listViewVins).Name = "listViewVins";
    this.listViewVins.ShowGlyphs = (GlyphBehavior) 1;
    this.listViewVins.ShowItemImages = (ImageBehavior) 1;
    this.listViewVins.ShowStateImages = (ImageBehavior) 1;
    ((ListView) this.listViewVins).UseCompatibleStateImageBehavior = false;
    componentResourceManager.ApplyResources((object) this.columnEcu, "columnEcu");
    componentResourceManager.ApplyResources((object) this.columnVin, "columnVin");
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_CheckVINSynchronization");
    ((Control) this).Controls.Add((Control) this.listViewVins);
    ((Control) this).Controls.Add((Control) this.buttonStart);
    ((Control) this).Controls.Add((Control) this.buttonClose);
    ((Control) this).Controls.Add((Control) this.textBoxOutput);
    ((Control) this).Controls.Add((Control) this.labelFault);
    ((Control) this).Controls.Add((Control) this.checkmarkFaultPresent);
    ((Control) this).Name = nameof (UserPanel);
    ((ISupportInitialize) this.listViewVins).EndInit();
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }

  private enum Stage
  {
    Idle,
    WriteVins,
    WaitForIgnitionOffDisconnection,
    WaitForIgnitionOnReconnection,
    Stopping,
    Finish,
    Unknown,
  }

  private enum Reason
  {
    Succeeded,
    Closing,
    Disconnected,
    UnknownStage,
    NoVinsChanged,
    NoVinMaster,
  }
}
