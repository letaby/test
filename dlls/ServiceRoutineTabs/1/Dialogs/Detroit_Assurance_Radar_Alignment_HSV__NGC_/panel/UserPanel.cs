// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment_HSV__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment_HSV__NGC_.panel;

public class UserPanel : CustomPanel
{
  private const string verticalPosition = "VertPos";
  private const string LevelParameterQualifier = "Chassis_Leveling_System_Available_On_Axle";
  private const string RadarEcuName = "RDF02T";
  private const string XmcEcuName = "XMC02T";
  private static int RadarAlignmentLevelSettingRawValue = 0;
  private static readonly Regex levelValueRegex = new Regex("^\\s*P,Chassis_Leveling_System_Available_On_Axle,\\d{4},(?<LevelValue>\\d+)", RegexOptions.Compiled);
  private WarningManager warningManager;
  private bool closing = false;
  private Channel rdf02tChannel;
  private Channel xmc02tChannel;
  private UserPanel.LevelState levelState;
  private Dictionary<UserPanel.LevelState, UserPanel.Transaction> pendingTransactions = new Dictionary<UserPanel.LevelState, UserPanel.Transaction>();
  private Channel lockedRdf02tChannel;
  private Choice radarAlignmentLevelTypeValue;
  private System.Windows.Forms.Label labelStatus;
  private TableLayoutPanel tableLayoutPanel1;
  private BarInstrument barInstrumentProcedureProgress;
  private SeekTimeListView outputLogView;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DigitalReadoutInstrument digitalReadoutInstrumentVertPos;
  private System.Windows.Forms.Label currentStatusLabel;
  private Button buttonStartStop;
  private SharedProcedureSelection sharedProcedureSelection1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;
  private DialInstrument dialInstrumentVehicleSpeed;
  private Checkmark checkmark1;

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningManager = new WarningManager(Resources.WarningManagerMessage, Resources.WarningManagerJobName, this.outputLogView.RequiredUserLabelPrefix);
  }

  private Channel Rdf02tChannel
  {
    get
    {
      if (this.Rdf02tChannelLocked && !object.Equals((object) this.lockedRdf02tChannel, (object) this.rdf02tChannel))
        throw new InvalidOperationException("The communication RDF02T channel has been lost");
      return this.rdf02tChannel;
    }
    set
    {
      if (this.rdf02tChannel == value)
        return;
      this.warningManager.Reset();
      if (this.rdf02tChannel != null)
      {
        this.rdf02tChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.Rdf02tChannel_CommunicationsStateUpdateEvent);
        this.rdf02tChannel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Rdf02t_ParametersReadCompleteEvent);
      }
      this.rdf02tChannel = value;
      if (this.rdf02tChannel != null)
      {
        this.rdf02tChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.Rdf02tChannel_CommunicationsStateUpdateEvent);
        this.rdf02tChannel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Rdf02t_ParametersReadCompleteEvent);
      }
      this.OnRdf02tChannelChanged();
    }
  }

  private Parameter LevelParameter
  {
    get
    {
      return this.Rdf02tChannel != null ? this.Rdf02tChannel.Parameters["Chassis_Leveling_System_Available_On_Axle"] : throw new InvalidOperationException("The RDF02T Channel property cannot be null.");
    }
  }

  private bool HistoryFileContainedMismatch { get; set; }

  private bool CanClose
  {
    get
    {
      UserPanel.Transaction transaction = this.GetTransaction(this.CurrentLevelState);
      bool flag = transaction != null && !transaction.Completed;
      return !this.sharedProcedureSelection1.AnyProcedureInProgress && (!this.Rdf02tOnline || !flag && this.CurrentLevelState != UserPanel.LevelState.TemporaryLevelSettingActive && this.CurrentLevelState != UserPanel.LevelState.AlignmentWithTemporaryLevel);
    }
  }

  private string VehicleIdentificationNumber { get; set; }

  private string EngineSerialNumber { get; set; }

  private bool Rdf02tChannelIdle
  {
    get
    {
      return this.rdf02tChannel != null && this.rdf02tChannel.CommunicationsState == CommunicationsState.Online;
    }
  }

  private bool Rdf02tOnline => this.rdf02tChannel != null && this.rdf02tChannel.Online;

  private bool Xmc02tOnline => this.xmc02tChannel != null && this.xmc02tChannel.Online;

  private UserPanel.LevelState CurrentLevelState
  {
    get => this.levelState;
    set
    {
      if (this.levelState == value)
        return;
      UserPanel.LevelState levelState = this.levelState;
      this.levelState = value;
      this.OnLevelStateChanged(levelState);
    }
  }

  private UserPanel.Transaction CreateTransaction(UserPanel.LevelState state)
  {
    UserPanel.Transaction transaction1 = new UserPanel.Transaction();
    transaction1.TransactionCompleted += (EventHandler<EventArgs>) ((sender, args) =>
    {
      UserPanel.Transaction transaction2 = sender as UserPanel.Transaction;
      if (transaction2.Committed)
        return;
      if (transaction2.Exception != null)
      {
        if (transaction2.Exception is CaesarException || transaction2.Exception is InvalidOperationException)
        {
          this.LogMessage(Resources.MessageFormat_OperationFailedError0, (object) transaction2.Exception.Message);
        }
        else
        {
          if (!(transaction2.Exception is OperationCanceledException))
            throw transaction2.Exception;
          this.LogMessage(transaction2.Exception.Message, (LabelStyle) 0);
        }
        if (state != UserPanel.LevelState.Unknown && state != UserPanel.LevelState.Initializing)
          ((Control) this).BeginInvoke((Delegate) (() => this.InitializeVehicleDataAsync()));
      }
      if (state == UserPanel.LevelState.Initializing)
        this.CurrentLevelState = UserPanel.LevelState.Unknown;
    });
    this.pendingTransactions.Add(state, transaction1);
    return transaction1;
  }

  private UserPanel.Transaction GetTransaction(UserPanel.LevelState state)
  {
    return this.pendingTransactions.ContainsKey(state) ? this.pendingTransactions[state] : (UserPanel.Transaction) null;
  }

  private void CloseTransaction(UserPanel.LevelState state)
  {
    if (!this.pendingTransactions.ContainsKey(state))
      return;
    if (!this.pendingTransactions[state].Completed)
    {
      try
      {
        this.pendingTransactions[state].Rollback();
      }
      catch (AggregateException ex)
      {
      }
    }
    this.pendingTransactions.Remove(state);
  }

  private UserPanel.Transaction CurrentTransaction
  {
    get
    {
      return this.GetTransaction(this.CurrentLevelState) ?? this.CreateTransaction(this.CurrentLevelState);
    }
  }

  private void OnLevelStateChanged(UserPanel.LevelState previousState)
  {
    this.CloseTransaction(previousState);
    switch (this.CurrentLevelState)
    {
      case UserPanel.LevelState.Unknown:
        if ((previousState == UserPanel.LevelState.TemporaryLevelSettingActive || previousState == UserPanel.LevelState.AlignmentWithTemporaryLevel) && !this.Rdf02tOnline)
          this.LogMessage(Resources.MessageFormat_UnableToRestoreTheParameter0BackTo1Because2IsDisconnected, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue, (object) "RDF02T");
        this.OriginalLevelTypeValue = (Choice) null;
        this.HistoryFileContainedMismatch = false;
        this.EngineSerialNumber = string.Empty;
        this.VehicleIdentificationNumber = string.Empty;
        break;
      case UserPanel.LevelState.Initializing:
        if (previousState == UserPanel.LevelState.TemporaryLevelSettingActive && !this.Rdf02tOnline)
        {
          this.LogMessage(Resources.MessageFormat_UnableToRestoreTheParameter0BackTo1Because2IsDisconnected, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue, (object) "RDF02T");
          break;
        }
        break;
      case UserPanel.LevelState.InvalidLevelType:
        if (previousState == UserPanel.LevelState.TemporaryLevelSettingActive)
        {
          this.LogMessage(Resources.MessageFormat_0ParameterRestoredBackTo1, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue);
          break;
        }
        this.LogMessage(Resources.MessageFormat_0ParameterValue1MustBeChangedTo2, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue, (object) this.RadarAlignmentLevelTypeValue);
        break;
      case UserPanel.LevelState.TemporaryLevelSettingActive:
      case UserPanel.LevelState.AlignmentWithTemporaryLevel:
        if (previousState == UserPanel.LevelState.InvalidLevelType)
        {
          this.LogMessage(Resources.MessageFormat_0ParameterChangedFrom1To2, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue, (object) this.RadarAlignmentLevelTypeValue);
          break;
        }
        this.LogMessage(Resources.MessageFormat_0ParameterWasPreviouslyChangedFrom1To2, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue, (object) this.RadarAlignmentLevelTypeValue);
        break;
      case UserPanel.LevelState.ValidLevelType:
        this.LogMessage(Resources.MessageFormat_0Parameter1AllowsAlignment, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue);
        break;
    }
    this.UpdateUserInterface();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.ParentForm_FormClosing);
    if (this.sharedProcedureSelection1.SelectedProcedure != null)
      this.sharedProcedureSelection1.SelectedProcedure.EnabledChanged += new EventHandler(this.OnSharedProcedureEnabledChanged);
    base.OnChannelsChanged();
    this.ReadParameters();
    this.UpdateUserInterface();
  }

  public virtual void OnChannelsChanged()
  {
    if (!this.closing)
    {
      this.Rdf02tChannel = this.GetChannel("RDF02T");
      this.xmc02tChannel = this.GetChannel("XMC02T");
    }
    base.OnChannelsChanged();
  }

  private void OnRdf02tChannelChanged()
  {
    if (this.closing)
      return;
    this.CurrentLevelState = UserPanel.LevelState.Unknown;
    this.InitializeVehicleDataAsync();
  }

  private bool Rdf02tChannelLocked => this.lockedRdf02tChannel != null;

  private Action LockRdf02tChannel()
  {
    Channel originalTarget = this.lockedRdf02tChannel;
    this.lockedRdf02tChannel = this.Rdf02tChannel;
    return (Action) (() => this.lockedRdf02tChannel = originalTarget);
  }

  private void InitializeVehicleDataAsync()
  {
    this.CurrentLevelState = UserPanel.LevelState.Initializing;
    if (this.Rdf02tChannelIdle)
    {
      try
      {
        this.CurrentTransaction.AddCleanup(this.LockRdf02tChannel());
        this.HistoryFileContainedMismatch = false;
        this.EngineSerialNumber = SapiManager.GetEngineSerialNumber(this.Rdf02tChannel);
        this.VehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(this.Rdf02tChannel);
        if (this.sharedProcedureSelection1.AnyProcedureInProgress)
        {
          this.sharedProcedureSelection1.SelectedProcedure.StopComplete += new EventHandler<PassFailResultEventArgs>(this.OnProcedureStopDuringInitialize);
          this.sharedProcedureSelection1.StopSelectedProcedure();
        }
        else
          this.InitializeLevelTypeAsync();
      }
      catch (Exception ex)
      {
        this.CurrentTransaction.Rollback(ex);
      }
    }
    else
      this.CurrentTransaction.Rollback();
  }

  private void OnProcedureStopDuringInitialize(object sender, PassFailResultEventArgs e)
  {
    this.sharedProcedureSelection1.SelectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.OnProcedureStopDuringInitialize);
    if (this.Rdf02tChannelIdle)
    {
      try
      {
        this.InitializeLevelTypeAsync();
      }
      catch (Exception ex)
      {
        this.CurrentTransaction.Rollback(ex);
      }
    }
    else
      this.CurrentTransaction.Rollback();
  }

  private void Rdf02tChannel_CommunicationsStateUpdateEvent(
    object sender,
    CommunicationsStateEventArgs e)
  {
    if (e.CommunicationsState == CommunicationsState.Online && this.CurrentLevelState == UserPanel.LevelState.Unknown)
    {
      this.InitializeVehicleDataAsync();
    }
    else
    {
      if (e.CommunicationsState != CommunicationsState.Disconnecting)
        return;
      this.CurrentLevelState = UserPanel.LevelState.Unknown;
    }
  }

  private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.closing = true;
    this.Rdf02tChannel = (Channel) null;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.ParentForm_FormClosing);
    if (this.sharedProcedureSelection1.SelectedProcedure != null)
      this.sharedProcedureSelection1.SelectedProcedure.EnabledChanged -= new EventHandler(this.OnSharedProcedureEnabledChanged);
  }

  private void LogMessage(string message, LabelStyle style = 0)
  {
    this.LabelLog(this.outputLogView.RequiredUserLabelPrefix, message, style);
  }

  private void LogMessage(string message, params object[] args)
  {
    this.LabelLog(this.outputLogView.RequiredUserLabelPrefix, string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));
  }

  private void buttonStartStop_Click(object sender, EventArgs e)
  {
    switch (this.CurrentLevelState)
    {
      case UserPanel.LevelState.InvalidLevelType:
        this.EnableRadarAlignmentLevelSupportAsync();
        break;
      case UserPanel.LevelState.TemporaryLevelSettingActive:
        this.RestoreOriginalLevelTypeAsync();
        break;
      case UserPanel.LevelState.AlignmentWithTemporaryLevel:
        this.RunSharedProcedure();
        break;
      case UserPanel.LevelState.ValidLevelType:
        this.RunSharedProcedure();
        break;
    }
  }

  private void RunSharedProcedure()
  {
    if (this.CurrentLevelState == UserPanel.LevelState.AlignmentWithTemporaryLevel)
    {
      try
      {
        if (this.sharedProcedureSelection1.CanStartSelectedProcedure)
        {
          this.CurrentTransaction.AddCleanup(this.SharedProcedureCleanup());
          this.sharedProcedureSelection1.StartSelectedProcedure();
        }
        else if (this.sharedProcedureSelection1.CanStopSelectedProcedure)
        {
          this.buttonStartStop.Text = Resources.Message_Start;
          this.buttonStartStop.Enabled = false;
          this.sharedProcedureSelection1.StopSelectedProcedure();
        }
        else
          this.CurrentTransaction.Rollback();
      }
      catch (Exception ex)
      {
        this.CurrentTransaction.Rollback(ex);
      }
    }
    else
    {
      if (this.CurrentLevelState != UserPanel.LevelState.ValidLevelType)
        return;
      if (this.sharedProcedureSelection1.CanStartSelectedProcedure)
      {
        if (this.warningManager.RequestContinue())
        {
          this.buttonStartStop.Text = Resources.Message_Stop;
          this.buttonStartStop.Enabled = false;
          this.sharedProcedureSelection1.SelectedProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.OnProcedureStart);
          this.sharedProcedureSelection1.StartSelectedProcedure();
        }
      }
      else if (this.sharedProcedureSelection1.CanStopSelectedProcedure)
      {
        this.buttonStartStop.Text = Resources.Message_Start;
        this.buttonStartStop.Enabled = false;
        this.sharedProcedureSelection1.SelectedProcedure.StopComplete += new EventHandler<PassFailResultEventArgs>(this.OnProcedureStop);
        this.sharedProcedureSelection1.StopSelectedProcedure();
      }
    }
  }

  private Action SharedProcedureCleanup()
  {
    string originalText = this.buttonStartStop.Text;
    bool originalEnabled = this.buttonStartStop.Enabled;
    this.buttonStartStop.Text = Resources.Message_Stop;
    this.buttonStartStop.Enabled = false;
    this.sharedProcedureSelection1.SelectedProcedure.StartComplete += new EventHandler<PassFailResultEventArgs>(this.OnProcedureStart);
    this.sharedProcedureSelection1.SelectedProcedure.StopComplete += new EventHandler<PassFailResultEventArgs>(this.OnProcedureStop);
    return (Action) (() =>
    {
      this.buttonStartStop.Text = originalText;
      this.buttonStartStop.Enabled = originalEnabled;
      this.sharedProcedureSelection1.SelectedProcedure.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.OnProcedureStart);
      this.sharedProcedureSelection1.SelectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.OnProcedureStop);
    });
  }

  private void EnableRadarAlignmentLevelSupportAsync()
  {
    if (this.CurrentLevelState == UserPanel.LevelState.TemporaryLevelSettingActive || !this.sharedProcedureSelection1.CanStartSelectedProcedure)
      return;
    if (this.warningManager.RequestContinue())
    {
      try
      {
        this.PrepareForLevelWriteOperation();
        this.SaveSnapshotOfParameter();
        this.WriteLevelParameterAsync(this.RadarAlignmentLevelTypeValue);
      }
      catch (Exception ex)
      {
        this.CurrentTransaction.Rollback(ex);
      }
    }
  }

  private void RestoreOriginalLevelTypeAsync()
  {
    if (this.CurrentLevelState != UserPanel.LevelState.TemporaryLevelSettingActive && this.CurrentLevelState != UserPanel.LevelState.AlignmentWithTemporaryLevel)
      return;
    try
    {
      this.PrepareForLevelWriteOperation();
      this.WriteLevelParameterAsync(this.OriginalLevelTypeValue);
    }
    catch (Exception ex)
    {
      this.CurrentTransaction.Rollback(ex);
    }
  }

  private void PrepareForLevelWriteOperation()
  {
    this.CurrentTransaction.AddCleanup(this.EnableWaitCursor());
    this.CurrentTransaction.AddCleanup(this.DisableActionableControls());
    this.CurrentTransaction.AddCleanup(this.LockRdf02tChannel());
  }

  private void WriteLevelParameterAsync(Choice value)
  {
    this.CurrentTransaction.AddCleanup(this.LockRdf02tChannel());
    object obj = this.LevelParameter.Value;
    this.LevelParameter.Value = (object) value;
    this.Rdf02tWriteUnlock();
    this.Rdf02tChannel.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.LevelParameter_WriteCompleteEvent);
    this.Rdf02tChannel.Parameters.Write(false);
  }

  private void LevelParameter_WriteCompleteEvent(object sender, ResultEventArgs e)
  {
    ParameterCollection parameterCollection = sender as ParameterCollection;
    parameterCollection.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.LevelParameter_WriteCompleteEvent);
    if (e.Succeeded)
    {
      this.CurrentTransaction.Commit();
      if (parameterCollection["Chassis_Leveling_System_Available_On_Axle"].Value == (object) this.RadarAlignmentLevelTypeValue)
      {
        this.CurrentLevelState = UserPanel.LevelState.AlignmentWithTemporaryLevel;
        this.RunSharedProcedure();
      }
      else
        this.CurrentLevelState = UserPanel.LevelState.InvalidLevelType;
    }
    else
      this.CurrentTransaction.Rollback(e.Exception);
  }

  private void SaveSnapshotOfParameter()
  {
    if (this.HistoryFileContainedMismatch)
      return;
    ServerDataManager.SaveSettings(this.Rdf02tChannel.Parameters, Directories.DrumrollHistoryData, Utility.GetSettingsFileName(this.Rdf02tChannel, "ECUREAD", DateTime.Now, (FileNameInformation.SettingsFileFormat) 2), ParameterFileFormat.VerFile);
  }

  private void UpdateUserInterface()
  {
    switch (this.CurrentLevelState)
    {
      case UserPanel.LevelState.Unknown:
        this.currentStatusLabel.Text = this.Rdf02tOnline ? Resources.Message_Initializing : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_The0IsNoLongerOnlinePleaseReconnectTheECUBeforeContinuing, (object) "RDF02T");
        this.buttonStartStop.Visible = false;
        break;
      case UserPanel.LevelState.Initializing:
        this.currentStatusLabel.Text = Resources.Message_Initializing;
        this.buttonStartStop.Visible = false;
        break;
      case UserPanel.LevelState.InvalidLevelType:
        if (!this.Xmc02tOnline)
          this.currentStatusLabel.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_XMC);
        else
          this.currentStatusLabel.Text = $"{string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_When0Selected1WillBe2, (object) Resources.Message_Start, (object) this.LevelParameter.Name, (object) this.RadarAlignmentLevelTypeValue)}\n{string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_AfterSelectingStartDoNotDisconnectUntilComplete, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue)}";
        this.buttonStartStop.Text = Resources.Message_Start;
        this.buttonStartStop.Visible = true;
        this.buttonStartStop.Enabled = this.sharedProcedureSelection1.CanStartSelectedProcedure;
        break;
      case UserPanel.LevelState.TemporaryLevelSettingActive:
        this.currentStatusLabel.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ResetLevelParameterToCorrectState, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue, (object) Resources.Message_Restore);
        this.buttonStartStop.Text = Resources.Message_Restore;
        this.buttonStartStop.Visible = true;
        break;
      case UserPanel.LevelState.AlignmentWithTemporaryLevel:
        this.currentStatusLabel.Text = $"{Resources.Message_Drive}\n\n{string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_AfterSelectingStartDoNotDisconnectUntilComplete, (object) this.LevelParameter.Name, (object) this.OriginalLevelTypeValue)}";
        break;
      case UserPanel.LevelState.ValidLevelType:
        this.currentStatusLabel.Text = $"{Resources.Message_NoParameterChangesAreNeededToAlignRadar}\n\n{Resources.Message_Drive}";
        this.buttonStartStop.Visible = true;
        break;
    }
  }

  private Choice OriginalLevelTypeValue { get; set; }

  private Choice RadarAlignmentLevelTypeValue
  {
    get
    {
      if (this.radarAlignmentLevelTypeValue == (object) null)
        this.radarAlignmentLevelTypeValue = this.LevelParameter.Choices.GetItemFromRawValue((object) UserPanel.RadarAlignmentLevelSettingRawValue);
      return this.radarAlignmentLevelTypeValue;
    }
  }

  private void InitializeLevelTypeAsync()
  {
    if (this.Rdf02tChannelIdle)
    {
      this.CurrentTransaction.AddCleanup(this.LockRdf02tChannel());
      this.Rdf02tChannel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.LevelType_ReadCompleteEvent);
      this.Rdf02tChannel.Parameters.ReadGroup(this.LevelParameter.GroupQualifier, false, false);
    }
    else
      this.CurrentTransaction.Rollback();
  }

  private void LevelType_ReadCompleteEvent(object sender, ResultEventArgs args)
  {
    ParameterCollection parameterCollection = sender as ParameterCollection;
    parameterCollection.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.LevelType_ReadCompleteEvent);
    try
    {
      if (!args.Succeeded || args.Exception != null)
      {
        this.CurrentTransaction.Rollback(args.Exception);
      }
      else
      {
        this.OriginalLevelTypeValue = parameterCollection["Chassis_Leveling_System_Available_On_Axle"].Value as Choice;
        UserPanel.LevelState levelState;
        if (this.OriginalLevelTypeValue == (object) this.RadarAlignmentLevelTypeValue)
        {
          Choice levelTypeFromHistory = this.FindLevelTypeFromHistory();
          if (levelTypeFromHistory != (object) null)
          {
            this.LogMessage(Resources.MessageFormat_Parameter01LocatedInHistory, (object) this.LevelParameter.Name, (object) levelTypeFromHistory);
            this.OriginalLevelTypeValue = levelTypeFromHistory;
            this.HistoryFileContainedMismatch = true;
            levelState = UserPanel.LevelState.TemporaryLevelSettingActive;
          }
          else
            levelState = UserPanel.LevelState.ValidLevelType;
        }
        else
          levelState = UserPanel.LevelState.InvalidLevelType;
        this.CurrentTransaction.Commit();
        this.CurrentLevelState = levelState;
      }
    }
    catch (Exception ex)
    {
      this.CurrentTransaction.Rollback(ex);
    }
  }

  private Choice ReadLevelType(string filename)
  {
    Choice choice = (Choice) null;
    try
    {
      byte[] buffer = FileEncryptionProvider.ReadEncryptedFile(filename, true);
      if (buffer != null)
      {
        using (StreamReader streamReader = new StreamReader((Stream) new MemoryStream(buffer), Encoding.UTF8))
        {
          while (!streamReader.EndOfStream && choice == (object) null)
          {
            Match match = UserPanel.levelValueRegex.Match(streamReader.ReadLine());
            int result;
            if (match.Success && int.TryParse(match.Groups["LevelValue"].Value, out result))
              choice = this.LevelParameter.Choices.GetItemFromRawValue((object) result);
          }
        }
      }
    }
    catch (InvalidChecksumException ex)
    {
    }
    return choice;
  }

  private Choice FindLevelTypeFromHistory()
  {
    IEnumerable<string> strings = Directory.EnumerateFiles(Directories.DrumrollHistoryData).Select(filePath =>
    {
      var levelTypeFromHistory = new
      {
        filePath = filePath,
        fileInfo = FileNameInformation.FromName(FileEncryptionProvider.DecryptFileName(Path.GetFileName(filePath)), (FileNameInformation.FileType) 2)
      };
      return levelTypeFromHistory;
    }).Where(_param1 => _param1.fileInfo.Valid && UserPanel.MatchesEcu(_param1.fileInfo) && this.MatchesCurrentVehicle(_param1.fileInfo)).OrderByDescending(_param0 => Utility.TimeFromString(_param0.fileInfo.Timestamp)).Select(_param0 => _param0.filePath);
    Choice levelTypeFromHistory1 = (Choice) null;
    foreach (string filename in strings)
    {
      Choice choice = this.ReadLevelType(filename);
      if (choice != (object) null && choice != (object) this.RadarAlignmentLevelTypeValue)
        levelTypeFromHistory1 = choice;
    }
    return levelTypeFromHistory1;
  }

  private static bool MatchesEcu(FileNameInformation value)
  {
    return value != null && string.Equals(value.Device, "RDF02T", StringComparison.OrdinalIgnoreCase);
  }

  private bool MatchesCurrentVehicle(FileNameInformation value)
  {
    bool flag = false;
    if (value != null)
      flag = string.Equals(value.VehicleIdentity, this.VehicleIdentificationNumber, StringComparison.OrdinalIgnoreCase);
    return flag;
  }

  private Action EnableWaitCursor()
  {
    Cursor originalCursor = ((Control) this).Cursor;
    bool originalWaitSetting = ((Control) this).UseWaitCursor;
    ((Control) this).UseWaitCursor = true;
    ((Control) this).Cursor = Cursors.WaitCursor;
    return (Action) (() =>
    {
      ((Control) this).UseWaitCursor = originalWaitSetting;
      ((Control) this).Cursor = originalCursor;
    });
  }

  private Action DisableActionableControls()
  {
    bool buttonStartStopSetting = this.buttonStartStop.Enabled;
    this.buttonStartStop.Enabled = false;
    return (Action) (() => this.buttonStartStop.Enabled = buttonStartStopSetting);
  }

  private void Rdf02tWriteUnlock() => this.LockRdf02tChannel()();

  private void ReadParameters()
  {
    if (this.Rdf02tChannel == null || this.Rdf02tChannel.CommunicationsState != CommunicationsState.Online || this.Rdf02tChannel.Parameters["VertPos"] == null || this.Rdf02tChannel.Parameters["VertPos"].HasBeenReadFromEcu)
      return;
    this.Rdf02tChannel.Parameters.ReadGroup(this.Rdf02tChannel.Parameters["VertPos"].GroupQualifier, true, false);
  }

  private void Rdf02t_ParametersReadCompleteEvent(object sender, ResultEventArgs result)
  {
    this.ReadParameters();
  }

  private void OnProcedureStart(object sender, PassFailResultEventArgs e)
  {
    this.buttonStartStop.Enabled = true;
    if (!((ResultEventArgs) e).Succeeded)
      return;
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.ModifyState(1, (ValueState) 3);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.ModifyState(3, (ValueState) 1);
    ((Control) this.dialInstrumentVehicleSpeed).Invalidate();
  }

  private void OnProcedureStop(object sender, PassFailResultEventArgs e)
  {
    this.sharedProcedureSelection1.SelectedProcedure.StopComplete -= new EventHandler<PassFailResultEventArgs>(this.OnProcedureStop);
    this.sharedProcedureSelection1.SelectedProcedure.StartComplete -= new EventHandler<PassFailResultEventArgs>(this.OnProcedureStart);
    if (((ResultEventArgs) e).Succeeded)
    {
      if (e.Result == 1)
        this.LogMessage(Resources.Message_CalibrationComplete, (LabelStyle) 1);
    }
    else
      this.LogMessage(Resources.MessageFormat_CalibrationError0, (object) ((ResultEventArgs) e).Exception.Message);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.ModifyState(1, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.ModifyState(3, (ValueState) 3);
    ((Control) this.dialInstrumentVehicleSpeed).Invalidate();
    if (this.CurrentLevelState == UserPanel.LevelState.AlignmentWithTemporaryLevel)
    {
      this.CurrentTransaction.Commit();
      this.CurrentLevelState = UserPanel.LevelState.TemporaryLevelSettingActive;
      this.RestoreOriginalLevelTypeAsync();
    }
    else
    {
      if (this.CurrentLevelState != UserPanel.LevelState.ValidLevelType)
        return;
      this.buttonStartStop.Text = Resources.Message_Start;
      this.buttonStartStop.Enabled = true;
    }
  }

  private void OnSharedProcedureEnabledChanged(object sender, EventArgs e)
  {
    if (this.CurrentLevelState != UserPanel.LevelState.InvalidLevelType)
      return;
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.labelStatus = new System.Windows.Forms.Label();
    this.checkmark1 = new Checkmark();
    this.barInstrumentProcedureProgress = new BarInstrument();
    this.buttonStartStop = new Button();
    this.outputLogView = new SeekTimeListView();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentVertPos = new DigitalReadoutInstrument();
    this.currentStatusLabel = new System.Windows.Forms.Label();
    this.sharedProcedureSelection1 = new SharedProcedureSelection();
    this.dialInstrumentVehicleSpeed = new DialInstrument();
    this.sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmark1, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.barInstrumentProcedureProgress, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStartStop, 4, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.outputLogView, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentVertPos, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.currentStatusLabel, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.sharedProcedureSelection1, 3, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dialInstrumentVehicleSpeed, 0, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelStatus, 2);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.barInstrumentProcedureProgress, 2);
    componentResourceManager.ApplyResources((object) this.barInstrumentProcedureProgress, "barInstrumentProcedureProgress");
    this.barInstrumentProcedureProgress.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.Initialize((ValueState) 0, 2, "%");
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.Modify(1, 0.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).Gradient.Modify(2, 101.0, (ValueState) 0);
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).Instrument = new Qualifier((QualifierTypes) 1, "RDF02T", "DT_Service_Justage_Progress_service_justage_progress");
    ((Control) this.barInstrumentProcedureProgress).Name = "barInstrumentProcedureProgress";
    ((AxisSingleInstrumentBase) this.barInstrumentProcedureProgress).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).ShowValueReadout = false;
    ((SingleInstrumentBase) this.barInstrumentProcedureProgress).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonStartStop, "buttonStartStop");
    this.buttonStartStop.Name = "buttonStartStop";
    this.buttonStartStop.UseCompatibleTextRendering = true;
    this.buttonStartStop.UseVisualStyleBackColor = true;
    this.buttonStartStop.Click += new EventHandler(this.buttonStartStop_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.outputLogView, 3);
    componentResourceManager.ApplyResources((object) this.outputLogView, "outputLogView");
    this.outputLogView.FilterUserLabels = true;
    ((Control) this.outputLogView).Name = "outputLogView";
    this.outputLogView.RequiredUserLabelPrefix = "Detroit Assurance Driving Radar Alignment NGC";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.outputLogView, 4);
    this.outputLogView.SelectedTime = new DateTime?();
    this.outputLogView.ShowChannelLabels = false;
    this.outputLogView.ShowCommunicationsState = false;
    this.outputLogView.ShowControlPanel = false;
    this.outputLogView.ShowDeviceColumn = false;
    this.outputLogView.TimeFormat = "HH:mm:ss.f";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ParkingBrake");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    this.digitalReadoutInstrument2.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument2.Gradient.Modify(3, 8.0, (ValueState) 1);
    this.digitalReadoutInstrument2.Gradient.Modify(4, 9.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "RDF02T", "0FFFE9");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument3, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    this.digitalReadoutInstrument3.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrument3.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument3.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.digitalReadoutInstrument3.Gradient.Modify(3, 8.0, (ValueState) 1);
    this.digitalReadoutInstrument3.Gradient.Modify(4, 9.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "RDF02T", "0FFFF3");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentVertPos, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVertPos, "digitalReadoutInstrumentVertPos");
    this.digitalReadoutInstrumentVertPos.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVertPos).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVertPos).Instrument = new Qualifier((QualifierTypes) 4, "RDF02T", "VertPos");
    ((Control) this.digitalReadoutInstrumentVertPos).Name = "digitalReadoutInstrumentVertPos";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVertPos).UnitAlignment = StringAlignment.Near;
    this.currentStatusLabel.AutoEllipsis = true;
    componentResourceManager.ApplyResources((object) this.currentStatusLabel, "currentStatusLabel");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.currentStatusLabel, 3);
    this.currentStatusLabel.Name = "currentStatusLabel";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.currentStatusLabel, 2);
    this.currentStatusLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelection1, "sharedProcedureSelection1");
    ((Control) this.sharedProcedureSelection1).Name = "sharedProcedureSelection1";
    this.sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "SP_DrivingRadarAlignment_NGC_HSV"
    });
    ((Control) this.sharedProcedureSelection1).Tag = (object) "";
    this.dialInstrumentVehicleSpeed.AngleRange = 270.0;
    this.dialInstrumentVehicleSpeed.AngleStart = 135.0;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.dialInstrumentVehicleSpeed, 2);
    componentResourceManager.ApplyResources((object) this.dialInstrumentVehicleSpeed, "dialInstrumentVehicleSpeed");
    this.dialInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Initialize((ValueState) 0, 3, "km/h");
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(1, -1.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(2, 1.0, (ValueState) 3);
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).Gradient.Modify(3, 40.0, (ValueState) 3);
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.dialInstrumentVehicleSpeed).Name = "dialInstrumentVehicleSpeed";
    ((AxisSingleInstrumentBase) this.dialInstrumentVehicleSpeed).PreferredAxisRange = new AxisRange(0.0, 90.0, "mph");
    ((SingleInstrumentBase) this.dialInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.sharedProcedureIntegrationComponent1.ProceduresDropDown = this.sharedProcedureSelection1;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = this.labelStatus;
    this.sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = this.checkmark1;
    this.sharedProcedureIntegrationComponent1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponent1.StartStopButton = (Button) null;
    this.sharedProcedureIntegrationComponent1.StopAllButton = (Button) null;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Radar_Alignment");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum LevelState
  {
    Unknown,
    Initializing,
    InvalidLevelType,
    TemporaryLevelSettingActive,
    AlignmentWithTemporaryLevel,
    ValidLevelType,
  }

  private class Transaction
  {
    private Stack<Tuple<UserPanel.Transaction.ActionType, Action>> allCleanupActions = new Stack<Tuple<UserPanel.Transaction.ActionType, Action>>();

    public bool Committed { get; private set; }

    public bool Completed { get; private set; }

    public Exception Exception { get; private set; }

    public void AddRollback(Action action)
    {
      this.allCleanupActions.Push(Tuple.Create<UserPanel.Transaction.ActionType, Action>(UserPanel.Transaction.ActionType.Rollback, action));
    }

    public void AddCleanup(Action action)
    {
      this.allCleanupActions.Push(Tuple.Create<UserPanel.Transaction.ActionType, Action>(UserPanel.Transaction.ActionType.Cleanup, action));
    }

    public void Commit()
    {
      if (this.Completed)
        return;
      this.Committed = true;
      this.DoCleanup(false);
      this.Completed = true;
      this.OnTransactionCompleted();
    }

    public void Rollback() => this.Rollback((Exception) null);

    public void Rollback(Exception exception)
    {
      if (this.Completed)
        return;
      this.Committed = false;
      this.Exception = exception;
      this.DoCleanup(true);
      this.Completed = true;
      this.OnTransactionCompleted();
    }

    private void DoCleanup(bool includeRollback)
    {
      List<Exception> innerExceptions = new List<Exception>();
      while (this.allCleanupActions.Count > 0)
      {
        try
        {
          Tuple<UserPanel.Transaction.ActionType, Action> tuple = this.allCleanupActions.Pop();
          if (includeRollback || tuple.Item1 == UserPanel.Transaction.ActionType.Cleanup)
            tuple.Item2();
        }
        catch (Exception ex)
        {
          innerExceptions.Add(ex);
        }
      }
      if (innerExceptions.Count > 0)
        throw new AggregateException($"Exception occurred during transaction {(includeRollback ? (object) "rollback" : (object) "cleanup")}.", (IEnumerable<Exception>) innerExceptions);
    }

    private void OnTransactionCompleted()
    {
      EventHandler<EventArgs> transactionCompleted = this.TransactionCompleted;
      if (transactionCompleted == null)
        return;
      transactionCompleted((object) this, EventArgs.Empty);
    }

    public event EventHandler<EventArgs> TransactionCompleted;

    private enum ActionType
    {
      Cleanup,
      Rollback,
    }
  }
}
