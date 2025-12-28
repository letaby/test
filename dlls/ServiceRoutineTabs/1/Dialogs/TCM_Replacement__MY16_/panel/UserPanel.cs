// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Replacement__MY16_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Adr;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Replacement__MY16_.panel;

public class UserPanel : CustomPanel
{
  private const string SetClutchTypeQualifier = "RT_0530_Kupplungsaktuatortyp_setzen_Service_Start_aktueller_Kupplungsaktuatortyp";
  private const string SetTransmissionTypeV9 = "RT_0411_Getriebetyp_und_merkmale_setzen_Getriebe_Wegwerte_loeschen_Service_Start";
  private const string SetTransmissionTypeV11 = "RT_0412_Set_transmission_type_and_features_Clear_transmission_learned_values_Service_Start";
  private const string ReleaseTransportSecurityQualifier = "DJ_Release_transport_security_for_TCM";
  private const int CPCA_SAE1 = 0;
  private Channel tcm;
  private Service SetClutchActuator;
  private Service transTypeService;
  private Service ReleaseTransportSecurity;
  private bool waitingForIgnitionOff;
  private bool waitingForIgnitionOn;
  private bool? wasAutoConnecting;
  private List<Channel> channelsToWorkWith = new List<Channel>();
  private List<Channel> channelsToWaitForReconnect = new List<Channel>();
  private List<Ecu> manualConnectEcus = new List<Ecu>();
  private bool isServiceRunning = false;
  private static string warningMessage = Resources.Message_ByExecutingThisRoutineTheTransmissionSLearnedValuesWillBeResetUntilTheseValuesAreRelearnedShiftQualityMayNotBeOptimalDoYouWishToContinue;
  private WarningManager warningMgr;
  private List<string> transNotSetFaultCodes = new List<string>()
  {
    "52F3EE",
    "25F3EE",
    "23F3EE"
  };
  private Button btnSetTransmissionType;
  private Checkmark checkmarkTcmOnline;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelTcmStatus;
  private TableLayoutPanel tableMain;
  private SeekTimeListView seekTimeListViewOutput;
  private TableLayoutPanel tableTransControls;
  private DigitalReadoutInstrument digitalReadoutInstrumentTransmissionType;
  private ComboBox comboConstantMeshRangeGroup;
  private System.Windows.Forms.Label labelTransType;
  private System.Windows.Forms.Label labelConstantMeshRangeGroup;
  private DigitalReadoutInstrument digitalReadoutInstrumentConstantMeshRangeGroup;
  private ComboBox comboTransType;

  private static void SetSelectedItem(DataItem item, ComboBox combo)
  {
    if (item == null)
      return;
    Choice dataItemChoice = item.Value as Choice;
    if (dataItemChoice != (object) null)
    {
      Choice choice = combo.Items.OfType<Choice>().FirstOrDefault<Choice>((Func<Choice, bool>) (c => (int) Convert.ToByte(c.RawValue) == (int) Convert.ToByte(dataItemChoice.RawValue)));
      if (choice != (object) null)
        combo.SelectedItem = (object) choice;
    }
  }

  private static bool ConfigurationMatches(DataItem item, ComboBox combo)
  {
    if (item == null || combo.SelectedItem == null)
      return false;
    Choice choice = item.Value as Choice;
    Choice selectedItem = combo.SelectedItem as Choice;
    return choice != (object) null && selectedItem != (object) null && (int) Convert.ToByte(choice.RawValue) == (int) Convert.ToByte(selectedItem.RawValue);
  }

  private Service SetTransType
  {
    get => this.transTypeService;
    set
    {
      this.transTypeService = value;
      if (this.SetTransType != (Service) null)
      {
        this.comboTransType.DataSource = (object) this.SetTransType.InputValues[0].Choices;
        this.comboConstantMeshRangeGroup.DataSource = (object) this.SetTransType.InputValues[1].Choices;
        UserPanel.SetSelectedItem(((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionType).DataItem, this.comboTransType);
        UserPanel.SetSelectedItem(((SingleInstrumentBase) this.digitalReadoutInstrumentConstantMeshRangeGroup).DataItem, this.comboConstantMeshRangeGroup);
      }
      else
      {
        this.comboTransType.DataSource = (object) null;
        this.comboConstantMeshRangeGroup.DataSource = (object) null;
      }
    }
  }

  private bool IsConstantMeshRangeGroupAutoLearn
  {
    get
    {
      return this.SetTransType != (Service) null && this.SetTransType.Qualifier == "RT_0412_Set_transmission_type_and_features_Clear_transmission_learned_values_Service_Start";
    }
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.this_ParentFormClosing);
    this.UpdateUserInterface();
    this.warningMgr = new WarningManager(UserPanel.warningMessage, (string) null, this.seekTimeListViewOutput.RequiredUserLabelPrefix);
    ConnectionManager.GlobalInstance.ConnectionChanged += new EventHandler<IgnitionStatusEventArgs>(this.ConnectionManager_ConnectionChanged);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetTcm(this.GetChannel("TCM01T", (CustomPanel.ChannelLookupOptions) 3));
  }

  private void SetTcm(Channel tcm)
  {
    if (this.tcm == tcm)
      return;
    if (this.tcm != null)
    {
      this.tcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.SetClutchActuator = (Service) null;
      this.ReleaseTransportSecurity = (Service) null;
      this.SetTransType = (Service) null;
    }
    this.tcm = tcm;
    if (this.tcm != null)
    {
      this.tcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.SetClutchActuator = this.tcm.Services["RT_0530_Kupplungsaktuatortyp_setzen_Service_Start_aktueller_Kupplungsaktuatortyp"];
      this.ReleaseTransportSecurity = this.tcm.Services["DJ_Release_transport_security_for_TCM"];
      Service service = this.tcm.Services["RT_0412_Set_transmission_type_and_features_Clear_transmission_learned_values_Service_Start"];
      if ((object) service == null)
        service = this.tcm.Services["RT_0411_Getriebetyp_und_merkmale_setzen_Getriebe_Wegwerte_loeschen_Service_Start"];
      this.SetTransType = service;
    }
    this.UpdateUserInterface();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.CheckIgnitionStatus();
  }

  private void CheckIgnitionStatus()
  {
    if (this.waitingForIgnitionOff)
    {
      if (this.tcm == null || this.tcm.CommunicationsState == CommunicationsState.Offline || ConnectionManager.GlobalInstance.IgnitionStatus == 1)
      {
        this.waitingForIgnitionOff = false;
        this.checkmarkTcmOnline.Checked = true;
        ((Control) this.labelTcmStatus).Text = Resources.Message_IgnitionIsOffPleaseWait;
        this.waitingForIgnitionOn = true;
        SapiManager.GlobalInstance.Sapi.Channels.StartAutoConnect();
        ((Control) this).OnEnabledChanged(new EventArgs());
      }
    }
    else if (this.waitingForIgnitionOn && this.tcm != null && this.tcm.CommunicationsState == CommunicationsState.Online && ConnectionManager.GlobalInstance.IgnitionStatus == 0)
    {
      this.RestoreAutoConnectState();
      ((Control) this).OnEnabledChanged(new EventArgs());
    }
    this.UpdateUserInterface();
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
    this.waitingForIgnitionOff = true;
    this.UpdateUserInterface();
  }

  private void RestoreAutoConnectState()
  {
    this.waitingForIgnitionOn = false;
    foreach (Ecu manualConnectEcu in this.manualConnectEcus)
      manualConnectEcu.MarkedForAutoConnect = false;
    this.manualConnectEcus.Clear();
    if (this.wasAutoConnecting.HasValue)
    {
      Cursor.Current = Cursors.WaitCursor;
      SapiManager.GlobalInstance.Sapi.Channels.StopAutoConnect();
      if (this.wasAutoConnecting.Value)
        SapiManager.GlobalInstance.Sapi.Channels.StartAutoConnect(1);
      this.wasAutoConnecting = new bool?();
      Cursor.Current = Cursors.Default;
    }
    this.UpdateUserInterface();
  }

  private void DisplayIgnitionMessage()
  {
    if (this.waitingForIgnitionOff)
    {
      ((Control) this.labelTcmStatus).Text = Resources.Message_PleaseTurnIgnitionOffAndWait;
      this.checkmarkTcmOnline.Checked = false;
    }
    else
    {
      if (!this.waitingForIgnitionOn)
        return;
      ((Control) this.labelTcmStatus).Text = Resources.Message_PleaseTurnIgnitionOnAndWait;
      this.checkmarkTcmOnline.Checked = false;
    }
  }

  private void UpdateUserInterface()
  {
    this.checkmarkTcmOnline.Checked = this.Online;
    if (this.isServiceRunning)
    {
      ((Control) this.labelTcmStatus).Text = Resources.Message_TransmissionIsBeingSet;
      this.btnSetTransmissionType.Enabled = false;
    }
    else if (!this.Online)
    {
      ((Control) this.labelTcmStatus).Text = Resources.Message_TheTransmissionTypeCannotBeSetBecauseTheTCMIsOffline;
      this.btnSetTransmissionType.Enabled = false;
    }
    else if (this.ConfigurationMatchesSelection)
    {
      ((Control) this.labelTcmStatus).Text = Resources.Message_TheConfigurationIsSetToTheSelectedValues;
      this.btnSetTransmissionType.Enabled = true;
    }
    else
    {
      ((Control) this.labelTcmStatus).Text = Resources.Message_TheTransmissionTypeCanBeSet;
      this.btnSetTransmissionType.Enabled = true;
    }
    this.DisplayIgnitionMessage();
    if (!(this.SetTransType != (Service) null))
      return;
    this.labelConstantMeshRangeGroup.Visible = this.comboConstantMeshRangeGroup.Visible = ((Control) this.digitalReadoutInstrumentConstantMeshRangeGroup).Visible = !this.IsConstantMeshRangeGroupAutoLearn;
  }

  private bool Online => this.tcm != null && this.tcm.Online;

  private bool ConfigurationMatchesSelection
  {
    get
    {
      return UserPanel.ConfigurationMatches(((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionType).DataItem, this.comboTransType) && (this.IsConstantMeshRangeGroupAutoLearn || UserPanel.ConfigurationMatches(((SingleInstrumentBase) this.digitalReadoutInstrumentConstantMeshRangeGroup).DataItem, this.comboConstantMeshRangeGroup));
    }
  }

  private bool CheckFaultsAndWarn()
  {
    return this.Online && (this.transNotSetFaultCodes.Intersect<string>(this.tcm.FaultCodes.GetCurrentByFunction(ReadFunctions.NonPermanent | ReadFunctions.Permanent).Select<FaultCode, string>((Func<FaultCode, string>) (x => x.Code))).Count<string>() > 0 || this.warningMgr.RequestContinue());
  }

  private void btnSetTransmissionType_Click(object sender, EventArgs e)
  {
    if (!this.CheckFaultsAndWarn())
      return;
    this.isServiceRunning = true;
    this.StartSetClutchActuator();
  }

  private void StartSetClutchActuator()
  {
    if (this.Online && this.SetClutchActuator != (Service) null)
    {
      this.SetClutchActuator.InputValues[0].Value = (object) this.SetClutchActuator.InputValues[0].Choices.GetItemFromRawValue((object) 0);
      this.SetClutchActuator.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.SetClutchActuator_ServiceCompleteEvent);
      this.AddLogLabel(Resources.Message_SettingClutchActuatorType);
      this.SetClutchActuator.Execute(false);
    }
    else
    {
      this.AddLogLabel(Resources.Message_CannotSetClutchActuatorTypeEitherTheTCMIsUnavailableOrTheServiceCannotBeFound);
      this.isServiceRunning = false;
    }
    this.UpdateUserInterface();
  }

  private void StartSetTransmissionType()
  {
    if (this.Online && this.SetTransType != (Service) null)
    {
      this.SetTransType.InputValues[0].Value = (object) (this.comboTransType.SelectedValue as Choice);
      if (!this.IsConstantMeshRangeGroupAutoLearn)
        this.SetTransType.InputValues[1].Value = (object) (this.comboConstantMeshRangeGroup.SelectedValue as Choice);
      this.SetTransType.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.SetTransType_ServiceCompleteEvent);
      this.AddLogLabel(Resources.Message_SettingTransmissionType);
      this.SetTransType.Execute(false);
    }
    else
    {
      this.AddLogLabel(Resources.Message_CannotSetTransmissionTypeEitherTheTCMIsUnavailableOrTheServiceCannotBeFound);
      this.isServiceRunning = false;
    }
    this.UpdateUserInterface();
  }

  private void StartReleaseTransportSecurity()
  {
    if (this.Online && this.ReleaseTransportSecurity != (Service) null)
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
    if (this.isServiceRunning && e.CloseReason == CloseReason.UserClosing)
    {
      e.Cancel = true;
    }
    else
    {
      if (e.Cancel)
        return;
      ConnectionManager.GlobalInstance.ConnectionChanged -= new EventHandler<IgnitionStatusEventArgs>(this.ConnectionManager_ConnectionChanged);
    }
  }

  private void SetClutchActuator_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.SetClutchActuator.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.SetClutchActuator_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.AddLogLabel(Resources.Message_SuccessfullySetTheClutchActuator);
      this.StartSetTransmissionType();
    }
    else
    {
      this.AddLogLabel(string.Format(Resources.MessageFormat_UnableToSetTheClutchActuatorError0, (object) e.Exception.Message));
      this.isServiceRunning = false;
    }
    this.UpdateUserInterface();
  }

  private void SetTransType_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.SetTransType.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.SetTransType_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.AddLogLabel(string.Format(Resources.MessageFormat_SuccessfullySetTheTransmissionTypeTo0, (object) this.comboTransType.SelectedItem.ToString(), this.IsConstantMeshRangeGroupAutoLearn ? (object) Resources.Message_AutoLearn : (object) this.comboConstantMeshRangeGroup.SelectedItem.ToString()));
      this.StartReleaseTransportSecurity();
    }
    else
    {
      this.AddLogLabel(string.Format(Resources.MessageFormat_UnableToSetTheTransmissionTypeError0, (object) e.Exception.Message));
      this.isServiceRunning = false;
    }
    this.UpdateUserInterface();
  }

  private void ReleaseTransportSecurity_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.ReleaseTransportSecurity.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.ReleaseTransportSecurity_ServiceCompleteEvent);
    if (e.Succeeded)
      this.AddLogLabel(Resources.Message_SuccessfullyReleasedTransportSecurity);
    else
      this.AddLogLabel(string.Format(Resources.MessageFormat_UnableToReleaseTransportSecurityError, (object) e.Exception.Message));
    this.isServiceRunning = false;
    this.TurnOffAutoConnect();
    this.UpdateUserInterface();
  }

  private void AddLogLabel(string text)
  {
    this.LabelLog(this.seekTimeListViewOutput.RequiredUserLabelPrefix, text);
  }

  private void ConnectionManager_ConnectionChanged(object sender, IgnitionStatusEventArgs e)
  {
    this.CheckIgnitionStatus();
  }

  private void comboTransType_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void comboConstantMeshRangeGroup_SelectedIndexChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableTransControls = new TableLayoutPanel();
    this.btnSetTransmissionType = new Button();
    this.labelTransType = new System.Windows.Forms.Label();
    this.comboTransType = new ComboBox();
    this.labelConstantMeshRangeGroup = new System.Windows.Forms.Label();
    this.comboConstantMeshRangeGroup = new ComboBox();
    this.tableMain = new TableLayoutPanel();
    this.digitalReadoutInstrumentConstantMeshRangeGroup = new DigitalReadoutInstrument();
    this.labelTcmStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.checkmarkTcmOnline = new Checkmark();
    this.seekTimeListViewOutput = new SeekTimeListView();
    this.digitalReadoutInstrumentTransmissionType = new DigitalReadoutInstrument();
    ((Control) this.tableTransControls).SuspendLayout();
    ((Control) this.tableMain).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableTransControls, "tableTransControls");
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.tableTransControls, 2);
    ((TableLayoutPanel) this.tableTransControls).Controls.Add((Control) this.btnSetTransmissionType, 2, 0);
    ((TableLayoutPanel) this.tableTransControls).Controls.Add((Control) this.labelTransType, 0, 0);
    ((TableLayoutPanel) this.tableTransControls).Controls.Add((Control) this.comboTransType, 1, 0);
    ((TableLayoutPanel) this.tableTransControls).Controls.Add((Control) this.labelConstantMeshRangeGroup, 0, 1);
    ((TableLayoutPanel) this.tableTransControls).Controls.Add((Control) this.comboConstantMeshRangeGroup, 1, 1);
    ((Control) this.tableTransControls).Name = "tableTransControls";
    componentResourceManager.ApplyResources((object) this.btnSetTransmissionType, "btnSetTransmissionType");
    this.btnSetTransmissionType.Name = "btnSetTransmissionType";
    ((TableLayoutPanel) this.tableTransControls).SetRowSpan((Control) this.btnSetTransmissionType, 2);
    this.btnSetTransmissionType.UseCompatibleTextRendering = true;
    this.btnSetTransmissionType.UseVisualStyleBackColor = true;
    this.btnSetTransmissionType.Click += new EventHandler(this.btnSetTransmissionType_Click);
    componentResourceManager.ApplyResources((object) this.labelTransType, "labelTransType");
    this.labelTransType.Name = "labelTransType";
    this.labelTransType.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.comboTransType, "comboTransType");
    this.comboTransType.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboTransType.FormattingEnabled = true;
    this.comboTransType.Name = "comboTransType";
    this.comboTransType.SelectedIndexChanged += new EventHandler(this.comboTransType_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.labelConstantMeshRangeGroup, "labelConstantMeshRangeGroup");
    this.labelConstantMeshRangeGroup.Name = "labelConstantMeshRangeGroup";
    this.labelConstantMeshRangeGroup.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.comboConstantMeshRangeGroup, "comboConstantMeshRangeGroup");
    this.comboConstantMeshRangeGroup.DropDownStyle = ComboBoxStyle.DropDownList;
    this.comboConstantMeshRangeGroup.FormattingEnabled = true;
    this.comboConstantMeshRangeGroup.Name = "comboConstantMeshRangeGroup";
    this.comboConstantMeshRangeGroup.SelectedIndexChanged += new EventHandler(this.comboConstantMeshRangeGroup_SelectedIndexChanged);
    componentResourceManager.ApplyResources((object) this.tableMain, "tableMain");
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.digitalReadoutInstrumentConstantMeshRangeGroup, 0, 3);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.labelTcmStatus, 1, 1);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.checkmarkTcmOnline, 0, 1);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.seekTimeListViewOutput, 0, 0);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.tableTransControls, 0, 4);
    ((TableLayoutPanel) this.tableMain).Controls.Add((Control) this.digitalReadoutInstrumentTransmissionType, 0, 2);
    ((Control) this.tableMain).Name = "tableMain";
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.digitalReadoutInstrumentConstantMeshRangeGroup, 2);
    this.digitalReadoutInstrumentConstantMeshRangeGroup.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentConstantMeshRangeGroup).FreezeValue = false;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentConstantMeshRangeGroup, "digitalReadoutInstrumentConstantMeshRangeGroup");
    ((SingleInstrumentBase) this.digitalReadoutInstrumentConstantMeshRangeGroup).Instrument = new Qualifier((QualifierTypes) 8, "TCM01T", "DT_STO_Getriebe_Merkmale_Range_Klaue");
    ((Control) this.digitalReadoutInstrumentConstantMeshRangeGroup).Name = "digitalReadoutInstrumentConstantMeshRangeGroup";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentConstantMeshRangeGroup).UnitAlignment = StringAlignment.Near;
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
    ((TableLayoutPanel) this.tableMain).SetColumnSpan((Control) this.digitalReadoutInstrumentTransmissionType, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentTransmissionType, "digitalReadoutInstrumentTransmissionType");
    this.digitalReadoutInstrumentTransmissionType.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionType).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionType).Instrument = new Qualifier((QualifierTypes) 8, "TCM01T", "CO_TransType");
    ((Control) this.digitalReadoutInstrumentTransmissionType).Name = "digitalReadoutInstrumentTransmissionType";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentTransmissionType).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_TCMReplacement");
    ((Control) this).Controls.Add((Control) this.tableMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableTransControls).ResumeLayout(false);
    ((Control) this.tableMain).ResumeLayout(false);
    ((Control) this.tableMain).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
