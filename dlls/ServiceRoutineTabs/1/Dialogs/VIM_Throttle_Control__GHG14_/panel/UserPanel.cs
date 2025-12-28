// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.VIM_Throttle_Control__GHG14_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms;
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VIM_Throttle_Control__GHG14_.panel;

public class UserPanel : CustomPanel
{
  private const string PedalParameterQualifier = "Accel_Pedal_Type";
  private const string EcuName = "CPC04T";
  private static string VimCompatiblePedalSettingText = Resources.Message_AnalogPedalType3;
  private static int VimCompatiblePedalSettingRawValue = 4;
  private static string ButonTextSetValidThrottle = Resources.Message_EnableVIMThrottleSupport;
  private static string ButtonTextRestoreOriginalPedal = Resources.Message_RestoreOriginalPedalType;
  private static string InitializingMessage = Resources.Message_Initializing;
  private static string OfflineMessage = string.Format(Resources.MessageFormat_The0IsNoLongerOnlinePleaseReconnectTheECUBeforeContinuing, (object) "CPC04T");
  private static string InvalidPedalTypeMessage = string.Format(Resources.MessageFormat_TheCurrentAcceleratorPedalTypeIsNotCompatible, (object) UserPanel.ButonTextSetValidThrottle);
  private static string TemporaryPedalMessage = string.Format(Resources.MessageFormat_ToEnableCompatibility, (object) UserPanel.VimCompatiblePedalSettingText, (object) "{0}", (object) UserPanel.ButtonTextRestoreOriginalPedal);
  private static string ValidPedalTypeMessage = Resources.Message_TheVehicleSettingsAreCompatibleWithTheVIMNoActionIsRequiredToEnableSupportForTheVIM;
  private static string TemporaryPedalInvalidInHistoryMessage = UserPanel.TemporaryPedalMessage;
  private static readonly Regex pedalValueRegex = new Regex("^\\s*P,Accel_Pedal_Type,\\d{4},(?<PedalValue>\\d+)", RegexOptions.Compiled);
  private Channel channel;
  private UserPanel.PedalState pedalState;
  private Dictionary<UserPanel.PedalState, UserPanel.Transaction> pendingTransactions = new Dictionary<UserPanel.PedalState, UserPanel.Transaction>();
  private Channel lockedChannel;
  private Choice vimCompatiblePedalTypeValue;
  private TableLayoutPanel tableLayoutPanel;
  private SeekTimeListView outputLogView;
  private BarInstrument pedalPositionInstrument;
  private System.Windows.Forms.Label titleLabel;
  private Button buttonClose;
  private DigitalReadoutInstrument pedalTypeInstrument;
  private System.Windows.Forms.Label closeWarningLabel;
  private System.Windows.Forms.Label currentStatusLabel;
  private Button setResetButton;
  private TableLayoutPanel actionsPanel;
  private ProgressBar progressBar;
  private System.Windows.Forms.Label progressLabel;
  private DialInstrument engineSpeedInstrument;

  public UserPanel() => this.InitializeComponent();

  private Channel Channel
  {
    get
    {
      if (this.ChannelLocked && !object.Equals((object) this.lockedChannel, (object) this.channel))
        throw new InvalidOperationException("The communication channel has been lost");
      return this.channel;
    }
    set
    {
      if (this.channel == value)
        return;
      if (this.channel != null)
        this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.Channel_CommunicationsStateUpdateEvent);
      this.channel = value;
      if (this.channel != null)
        this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.Channel_CommunicationsStateUpdateEvent);
      this.OnChannelChanged();
    }
  }

  private Parameter PedalParameter
  {
    get
    {
      return this.Channel != null ? this.Channel.Parameters["Accel_Pedal_Type"] : throw new InvalidOperationException("The Channel property cannot be null.");
    }
  }

  private bool HistoryFileContainedMismatch { get; set; }

  private bool CanClose
  {
    get
    {
      UserPanel.Transaction transaction = this.GetTransaction(this.CurrentPedalState);
      bool flag = transaction != null && !transaction.Completed;
      return !this.Online || !flag && this.CurrentPedalState != UserPanel.PedalState.TemporaryPedalSettingActive;
    }
  }

  private bool Closing { get; set; }

  private bool DisplayProgress
  {
    get => this.progressBar.Visible;
    set
    {
      this.progressBar.Visible = value;
      this.progressLabel.Visible = value;
    }
  }

  private string VehicleIdentificationNumber { get; set; }

  private string EngineSerialNumber { get; set; }

  private bool ChannelIdle
  {
    get => this.channel != null && this.channel.CommunicationsState == CommunicationsState.Online;
  }

  private bool Online => this.channel != null && this.channel.Online;

  private UserPanel.PedalState CurrentPedalState
  {
    get => this.pedalState;
    set
    {
      if (this.pedalState == value)
        return;
      UserPanel.PedalState pedalState = this.pedalState;
      this.pedalState = value;
      this.OnPedalStateChanged(pedalState);
    }
  }

  private UserPanel.Transaction CreateTransaction(UserPanel.PedalState state)
  {
    UserPanel.Transaction transaction1 = new UserPanel.Transaction();
    transaction1.TransactionCompleted += (EventHandler<EventArgs>) ((sender, args) =>
    {
      UserPanel.Transaction transaction2 = sender as UserPanel.Transaction;
      if (transaction2.Commited)
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
          this.LogMessage(transaction2.Exception.Message);
        }
        if (state != UserPanel.PedalState.Unknown && state != UserPanel.PedalState.Initializing)
          ((Control) this).BeginInvoke((Delegate) (() => this.InitializeVehicleDataAsync()));
      }
      if (state == UserPanel.PedalState.Initializing)
        this.CurrentPedalState = UserPanel.PedalState.Unknown;
    });
    this.pendingTransactions.Add(state, transaction1);
    return transaction1;
  }

  private UserPanel.Transaction GetTransaction(UserPanel.PedalState state)
  {
    return this.pendingTransactions.ContainsKey(state) ? this.pendingTransactions[state] : (UserPanel.Transaction) null;
  }

  private void CloseTransaction(UserPanel.PedalState state)
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
      return this.GetTransaction(this.CurrentPedalState) ?? this.CreateTransaction(this.CurrentPedalState);
    }
  }

  private void OnPedalStateChanged(UserPanel.PedalState previousState)
  {
    this.CloseTransaction(previousState);
    switch (this.CurrentPedalState)
    {
      case UserPanel.PedalState.Unknown:
        if (previousState == UserPanel.PedalState.TemporaryPedalSettingActive && !this.Online)
          this.LogMessage(Resources.MessageFormat_UnableToRestoreTheParameterAccelPedalTypeBackTo1Because0IsDisconnected1, (object) "CPC04T", (object) this.OriginalPedalTypeValue);
        this.OriginalPedalTypeValue = (Choice) null;
        this.HistoryFileContainedMismatch = false;
        this.EngineSerialNumber = string.Empty;
        this.VehicleIdentificationNumber = string.Empty;
        break;
      case UserPanel.PedalState.Initializing:
        if (previousState == UserPanel.PedalState.TemporaryPedalSettingActive && !this.Online)
        {
          this.LogMessage(Resources.MessageFormat_UnableToRestoreTheParameterAccelPedalTypeBackTo1Because0IsDisconnected, (object) "CPC04T", (object) this.OriginalPedalTypeValue);
          break;
        }
        break;
      case UserPanel.PedalState.InvalidPedalType:
        if (previousState == UserPanel.PedalState.TemporaryPedalSettingActive)
        {
          this.LogMessage(Resources.MessageFormat_AccelPedalTypeParameterRestoredBackTo0, (object) this.OriginalPedalTypeValue);
          break;
        }
        this.LogMessage(Resources.MessageFormat_AccelPedalTypeParameterValue0IsNotSupportedByTheVIM, (object) this.OriginalPedalTypeValue);
        break;
      case UserPanel.PedalState.TemporaryPedalSettingActive:
        if (previousState == UserPanel.PedalState.InvalidPedalType)
        {
          this.LogMessage(Resources.MessageFormat_AccelPedalTypeParameterChangedFrom0To1, (object) this.OriginalPedalTypeValue, (object) UserPanel.VimCompatiblePedalSettingText);
          break;
        }
        this.LogMessage(Resources.MessageFormat_AccelPedalTypeParameterWasPreviouslyChangedFrom0To1, (object) this.OriginalPedalTypeValue, (object) UserPanel.VimCompatiblePedalSettingText);
        break;
      case UserPanel.PedalState.ValidPedalType:
        this.LogMessage(Resources.Message_AccelPedalTypeParameterSupportedByVIM);
        break;
    }
    this.UpdateUserInterface();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.ParentForm_FormClosing);
    this.UpdateUserInterface();
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public virtual void OnChannelsChanged()
  {
    if (!this.Closing)
      this.Channel = this.GetChannel("CPC04T");
    base.OnChannelsChanged();
  }

  private void OnChannelChanged()
  {
    if (this.Closing)
      return;
    this.CurrentPedalState = UserPanel.PedalState.Unknown;
    this.InitializeVehicleDataAsync();
  }

  private bool ChannelLocked => this.lockedChannel != null;

  private Action LockChannel()
  {
    Channel originalTarget = this.lockedChannel;
    this.lockedChannel = this.Channel;
    return (Action) (() => this.lockedChannel = originalTarget);
  }

  private void InitializeVehicleDataAsync()
  {
    this.CurrentPedalState = UserPanel.PedalState.Initializing;
    if (this.ChannelIdle)
    {
      try
      {
        this.CurrentTransaction.AddCleanup(this.EnableInitializingProgressDisplay());
        this.CurrentTransaction.AddCleanup(this.LockChannel());
        this.HistoryFileContainedMismatch = false;
        this.EngineSerialNumber = SapiManager.GetEngineSerialNumber(this.Channel);
        this.VehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(this.Channel);
        this.InitializePedalTypeAsync();
      }
      catch (Exception ex)
      {
        this.CurrentTransaction.Rollback(ex);
      }
    }
    else
      this.CurrentTransaction.Rollback();
  }

  private void Channel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    if (e.CommunicationsState == CommunicationsState.Online && this.CurrentPedalState == UserPanel.PedalState.Unknown)
    {
      this.InitializeVehicleDataAsync();
    }
    else
    {
      if (e.CommunicationsState != CommunicationsState.Disconnecting)
        return;
      this.CurrentPedalState = UserPanel.PedalState.Unknown;
    }
  }

  private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.Closing = true;
    this.Channel = (Channel) null;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.ParentForm_FormClosing);
  }

  private void LogMessage(string message)
  {
    this.LabelLog(this.outputLogView.RequiredUserLabelPrefix, message);
  }

  private void LogMessage(string message, params object[] args)
  {
    this.LabelLog(this.outputLogView.RequiredUserLabelPrefix, string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));
  }

  private void setResetButton_Click(object sender, EventArgs e)
  {
    switch (this.CurrentPedalState)
    {
      case UserPanel.PedalState.InvalidPedalType:
        this.EnableVimPedalSupportAsync();
        break;
      case UserPanel.PedalState.TemporaryPedalSettingActive:
        this.RestoreOriginalPedalTypeAsync();
        break;
    }
  }

  private void EnableVimPedalSupportAsync()
  {
    if (this.CurrentPedalState == UserPanel.PedalState.TemporaryPedalSettingActive)
      return;
    try
    {
      this.PrepareForPedalWriteOperation();
      this.SaveSnapshotOfParameter();
      this.WritePedalParameterAsync(this.VimCompatiblePedalTypeValue);
    }
    catch (Exception ex)
    {
      this.CurrentTransaction.Rollback(ex);
    }
  }

  private void RestoreOriginalPedalTypeAsync()
  {
    if (this.CurrentPedalState != UserPanel.PedalState.TemporaryPedalSettingActive)
      return;
    try
    {
      this.PrepareForPedalWriteOperation();
      this.WritePedalParameterAsync(this.OriginalPedalTypeValue);
    }
    catch (Exception ex)
    {
      this.CurrentTransaction.Rollback(ex);
    }
  }

  private void PrepareForPedalWriteOperation()
  {
    this.CurrentTransaction.AddCleanup(this.EnableWriteProgressDisplay());
    this.CurrentTransaction.AddCleanup(this.EnableWaitCursor());
    this.CurrentTransaction.AddCleanup(this.DisableActionableControls());
    this.CurrentTransaction.AddCleanup(this.LockChannel());
  }

  private void WritePedalParameterAsync(Choice value)
  {
    this.CurrentTransaction.AddCleanup(this.LockChannel());
    object obj = this.PedalParameter.Value;
    this.PedalParameter.Value = (object) value;
    if (!this.WriteUnlock())
    {
      this.PedalParameter.Value = obj;
      throw new OperationCanceledException();
    }
    this.Channel.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.PedalParameter_WriteCompleteEvent);
    this.Channel.Parameters.Write(false);
  }

  private void PedalParameter_WriteCompleteEvent(object sender, ResultEventArgs e)
  {
    ParameterCollection parameterCollection = sender as ParameterCollection;
    parameterCollection.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.PedalParameter_WriteCompleteEvent);
    if (e.Succeeded)
    {
      this.CurrentTransaction.Commit();
      this.CurrentPedalState = parameterCollection["Accel_Pedal_Type"].Value == (object) this.VimCompatiblePedalTypeValue ? UserPanel.PedalState.TemporaryPedalSettingActive : UserPanel.PedalState.InvalidPedalType;
    }
    else
      this.CurrentTransaction.Rollback(e.Exception);
  }

  private void SaveSnapshotOfParameter()
  {
    if (this.HistoryFileContainedMismatch)
      return;
    ServerDataManager.SaveSettings(this.Channel.Parameters, Directories.DrumrollHistoryData, Utility.GetSettingsFileName(this.Channel, "ECUREAD", DateTime.Now, (FileNameInformation.SettingsFileFormat) 2), ParameterFileFormat.VerFile);
  }

  private void UpdateUserInterface()
  {
    switch (this.CurrentPedalState)
    {
      case UserPanel.PedalState.Unknown:
        this.DisplayProgress = false;
        this.currentStatusLabel.Text = this.Online ? UserPanel.InitializingMessage : UserPanel.OfflineMessage;
        this.setResetButton.Visible = false;
        break;
      case UserPanel.PedalState.Initializing:
        this.currentStatusLabel.Text = UserPanel.InitializingMessage;
        this.setResetButton.Visible = false;
        break;
      case UserPanel.PedalState.InvalidPedalType:
        this.currentStatusLabel.Text = UserPanel.InvalidPedalTypeMessage;
        this.setResetButton.Text = UserPanel.ButonTextSetValidThrottle;
        this.setResetButton.Visible = true;
        break;
      case UserPanel.PedalState.TemporaryPedalSettingActive:
        this.currentStatusLabel.Text = string.Format(this.HistoryFileContainedMismatch ? UserPanel.TemporaryPedalInvalidInHistoryMessage : UserPanel.TemporaryPedalMessage, (object) this.OriginalPedalTypeValue);
        this.setResetButton.Text = UserPanel.ButtonTextRestoreOriginalPedal;
        this.setResetButton.Visible = true;
        break;
      case UserPanel.PedalState.ValidPedalType:
        this.currentStatusLabel.Text = UserPanel.ValidPedalTypeMessage;
        this.setResetButton.Visible = false;
        break;
    }
    this.buttonClose.Enabled = this.CanClose;
    this.closeWarningLabel.Visible = !this.CanClose;
  }

  private Choice OriginalPedalTypeValue { get; set; }

  private Choice VimCompatiblePedalTypeValue
  {
    get
    {
      if (this.vimCompatiblePedalTypeValue == (object) null)
        this.vimCompatiblePedalTypeValue = this.PedalParameter.Choices.GetItemFromRawValue((object) UserPanel.VimCompatiblePedalSettingRawValue);
      return this.vimCompatiblePedalTypeValue;
    }
  }

  private void InitializePedalTypeAsync()
  {
    if (this.ChannelIdle)
    {
      this.CurrentTransaction.AddCleanup(this.LockChannel());
      this.Channel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.PedalType_ReadCompleteEvent);
      this.Channel.Parameters.ReadGroup(this.PedalParameter.GroupQualifier, false, false);
    }
    else
      this.CurrentTransaction.Rollback();
  }

  private void PedalType_ReadCompleteEvent(object sender, ResultEventArgs args)
  {
    ParameterCollection parameterCollection = sender as ParameterCollection;
    parameterCollection.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.PedalType_ReadCompleteEvent);
    try
    {
      if (!args.Succeeded || args.Exception != null)
      {
        this.CurrentTransaction.Rollback(args.Exception);
      }
      else
      {
        this.OriginalPedalTypeValue = parameterCollection["Accel_Pedal_Type"].Value as Choice;
        UserPanel.PedalState pedalState;
        if (this.OriginalPedalTypeValue == (object) this.VimCompatiblePedalTypeValue)
        {
          Choice pedalTypeFromHistory = this.FindPedalTypeFromHistory();
          if (pedalTypeFromHistory != (object) null)
          {
            this.LogMessage(Resources.MessageFormat_ParameterAccelPedalType0LocatedInHistory, (object) pedalTypeFromHistory);
            this.OriginalPedalTypeValue = pedalTypeFromHistory;
            this.HistoryFileContainedMismatch = true;
            pedalState = UserPanel.PedalState.TemporaryPedalSettingActive;
          }
          else
            pedalState = UserPanel.PedalState.ValidPedalType;
        }
        else
          pedalState = UserPanel.PedalState.InvalidPedalType;
        this.CurrentTransaction.Commit();
        this.CurrentPedalState = pedalState;
      }
    }
    catch (Exception ex)
    {
      this.CurrentTransaction.Rollback(ex);
    }
  }

  private Choice ReadPedalType(string filename)
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
            Match match = UserPanel.pedalValueRegex.Match(streamReader.ReadLine());
            int result;
            if (match.Success && int.TryParse(match.Groups["PedalValue"].Value, out result))
              choice = this.PedalParameter.Choices.GetItemFromRawValue((object) result);
          }
        }
      }
    }
    catch (InvalidChecksumException ex)
    {
    }
    return choice;
  }

  private Choice FindPedalTypeFromHistory()
  {
    return Directory.EnumerateFiles(Directories.DrumrollHistoryData).Select(filePath =>
    {
      var pedalTypeFromHistory = new
      {
        filePath = filePath,
        fileInfo = FileNameInformation.FromName(FileEncryptionProvider.DecryptFileName(Path.GetFileName(filePath)), (FileNameInformation.FileType) 2)
      };
      return pedalTypeFromHistory;
    }).Where(_param1 => _param1.fileInfo.Valid && UserPanel.MatchesEcu(_param1.fileInfo) && this.MatchesCurrentVehicle(_param1.fileInfo)).OrderByDescending(_param0 => Utility.TimeFromString(_param0.fileInfo.Timestamp)).Select(_param0 => _param0.filePath).Select(file =>
    {
      var pedalTypeFromHistory = new
      {
        file = file,
        pedalValue = this.ReadPedalType(file)
      };
      return pedalTypeFromHistory;
    }).Where(_param1 => _param1.pedalValue != (object) null && _param1.pedalValue != (object) this.VimCompatiblePedalTypeValue).Select(_param0 => _param0.pedalValue).FirstOrDefault<Choice>();
  }

  private static bool MatchesEcu(FileNameInformation value)
  {
    return value != null && string.Equals(value.Device, "CPC04T", StringComparison.OrdinalIgnoreCase);
  }

  private bool MatchesCurrentVehicle(FileNameInformation value)
  {
    return value != null && string.Equals(value.EngineSerialNumber, this.EngineSerialNumber, StringComparison.OrdinalIgnoreCase) && string.Equals(value.VehicleIdentity, this.VehicleIdentificationNumber, StringComparison.OrdinalIgnoreCase);
  }

  private Action EnableWriteProgressDisplay()
  {
    bool originalDisplayValue = this.DisplayProgress;
    string originalMessage = this.progressLabel.Text;
    this.progressLabel.Text = Resources.Message_WritingParameter;
    this.DisplayProgress = true;
    return (Action) (() =>
    {
      this.DisplayProgress = originalDisplayValue;
      this.progressLabel.Text = originalMessage;
    });
  }

  private Action EnableInitializingProgressDisplay()
  {
    bool originalDisplayValue = this.DisplayProgress;
    string originalMessage = this.progressLabel.Text;
    this.progressLabel.Text = string.Empty;
    this.DisplayProgress = true;
    return (Action) (() =>
    {
      this.DisplayProgress = originalDisplayValue;
      this.progressLabel.Text = originalMessage;
    });
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
    bool closeButtonSetting = this.buttonClose.Enabled;
    bool setResetButtonSetting = this.setResetButton.Enabled;
    this.buttonClose.Enabled = false;
    this.setResetButton.Enabled = false;
    return (Action) (() =>
    {
      this.buttonClose.Enabled = closeButtonSetting;
      this.setResetButton.Enabled = setResetButtonSetting;
    });
  }

  private bool WriteUnlock()
  {
    bool flag = true;
    Action action = this.LockChannel();
    try
    {
      if (PasswordManager.HasPasswords(this.Channel))
      {
        PasswordManager passwordManager = PasswordManager.Create(this.Channel);
        if (passwordManager.Valid)
        {
          try
          {
            bool[] source = passwordManager.AcquireRelevantListStatus((ProgressBar) null);
            if (((IEnumerable<bool>) source).Any<bool>((Func<bool, bool>) (x => x)))
            {
              this.LogMessage(Resources.MessageFormat_0IsLocked, (object) this.Channel.Ecu.Name);
              flag = ((Form) new PasswordEntryDialog(this.Channel, source, passwordManager)).ShowDialog() == DialogResult.OK;
              this.LogMessage(Resources.MessageFormat_0Was1ByUser, (object) this.Channel.Ecu.Name, flag ? (object) Resources.Message_Unlocked : (object) Resources.Message_NotUnlocked);
            }
          }
          catch (CaesarException ex)
          {
            flag = false;
            this.LogMessage(Resources.MessageFormat_FailedUnlocking0Error1, (object) this.Channel.Ecu.Name, (object) ex.Message);
          }
        }
      }
    }
    finally
    {
      action();
    }
    return flag;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.actionsPanel = new TableLayoutPanel();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.setResetButton = new Button();
    this.progressBar = new ProgressBar();
    this.progressLabel = new System.Windows.Forms.Label();
    this.outputLogView = new SeekTimeListView();
    this.pedalPositionInstrument = new BarInstrument();
    this.engineSpeedInstrument = new DialInstrument();
    this.titleLabel = new System.Windows.Forms.Label();
    this.buttonClose = new Button();
    this.pedalTypeInstrument = new DigitalReadoutInstrument();
    this.closeWarningLabel = new System.Windows.Forms.Label();
    this.currentStatusLabel = new System.Windows.Forms.Label();
    ((Control) this.actionsPanel).SuspendLayout();
    ((Control) this.tableLayoutPanel).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.actionsPanel, "actionsPanel");
    ((TableLayoutPanel) this.actionsPanel).Controls.Add((Control) this.setResetButton, 0, 0);
    ((TableLayoutPanel) this.actionsPanel).Controls.Add((Control) this.progressBar, 1, 1);
    ((TableLayoutPanel) this.actionsPanel).Controls.Add((Control) this.progressLabel, 1, 0);
    ((Control) this.actionsPanel).Name = "actionsPanel";
    componentResourceManager.ApplyResources((object) this.setResetButton, "setResetButton");
    this.setResetButton.Name = "setResetButton";
    ((TableLayoutPanel) this.actionsPanel).SetRowSpan((Control) this.setResetButton, 2);
    this.setResetButton.UseCompatibleTextRendering = true;
    this.setResetButton.UseVisualStyleBackColor = true;
    this.setResetButton.Click += new EventHandler(this.setResetButton_Click);
    componentResourceManager.ApplyResources((object) this.progressBar, "progressBar");
    this.progressBar.Name = "progressBar";
    this.progressBar.Style = ProgressBarStyle.Marquee;
    componentResourceManager.ApplyResources((object) this.progressLabel, "progressLabel");
    this.progressLabel.Name = "progressLabel";
    this.progressLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.outputLogView, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.pedalPositionInstrument, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.engineSpeedInstrument, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.titleLabel, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.buttonClose, 2, 6);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.pedalTypeInstrument, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.closeWarningLabel, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.currentStatusLabel, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel).Controls.Add((Control) this.actionsPanel, 0, 3);
    ((Control) this.tableLayoutPanel).Name = "tableLayoutPanel";
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.outputLogView, 3);
    componentResourceManager.ApplyResources((object) this.outputLogView, "outputLogView");
    this.outputLogView.FilterUserLabels = true;
    ((Control) this.outputLogView).Name = "outputLogView";
    this.outputLogView.RequiredUserLabelPrefix = "Vim";
    this.outputLogView.SelectedTime = new DateTime?();
    this.outputLogView.ShowChannelLabels = false;
    this.outputLogView.ShowCommunicationsState = false;
    this.outputLogView.ShowControlPanel = false;
    this.outputLogView.ShowDeviceColumn = false;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.pedalPositionInstrument, 3);
    componentResourceManager.ApplyResources((object) this.pedalPositionInstrument, "pedalPositionInstrument");
    this.pedalPositionInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.pedalPositionInstrument).FreezeValue = false;
    ((SingleInstrumentBase) this.pedalPositionInstrument).Instrument = new Qualifier((QualifierTypes) 1, "CPC04T", "DT_ASL_Accelerator_Pedal_Position");
    ((Control) this.pedalPositionInstrument).Name = "pedalPositionInstrument";
    ((Control) this.pedalPositionInstrument).TabStop = false;
    ((SingleInstrumentBase) this.pedalPositionInstrument).UnitAlignment = StringAlignment.Near;
    this.engineSpeedInstrument.AngleRange = 180.0;
    this.engineSpeedInstrument.AngleStart = -180.0;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.engineSpeedInstrument, 2);
    componentResourceManager.ApplyResources((object) this.engineSpeedInstrument, "engineSpeedInstrument");
    this.engineSpeedInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.engineSpeedInstrument).FreezeValue = false;
    ((SingleInstrumentBase) this.engineSpeedInstrument).Instrument = new Qualifier((QualifierTypes) 1, "CPC04T", "DT_ASL_Actual_Engine_Speed");
    ((Control) this.engineSpeedInstrument).Name = "engineSpeedInstrument";
    ((TableLayoutPanel) this.tableLayoutPanel).SetRowSpan((Control) this.engineSpeedInstrument, 3);
    ((Control) this.engineSpeedInstrument).TabStop = false;
    ((Control) this.engineSpeedInstrument).Tag = (object) "";
    ((SingleInstrumentBase) this.engineSpeedInstrument).UnitAlignment = StringAlignment.Near;
    this.titleLabel.AutoEllipsis = true;
    componentResourceManager.ApplyResources((object) this.titleLabel, "titleLabel");
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.titleLabel, 2);
    this.titleLabel.Name = "titleLabel";
    this.titleLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.pedalTypeInstrument, "pedalTypeInstrument");
    this.pedalTypeInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.pedalTypeInstrument).FreezeValue = false;
    this.pedalTypeInstrument.Gradient.Initialize((ValueState) 4, 8);
    this.pedalTypeInstrument.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.pedalTypeInstrument.Gradient.Modify(2, 1.0, (ValueState) 3);
    this.pedalTypeInstrument.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.pedalTypeInstrument.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.pedalTypeInstrument.Gradient.Modify(5, 4.0, (ValueState) 1);
    this.pedalTypeInstrument.Gradient.Modify(6, 5.0, (ValueState) 3);
    this.pedalTypeInstrument.Gradient.Modify(7, 6.0, (ValueState) 3);
    this.pedalTypeInstrument.Gradient.Modify(8, 7.0, (ValueState) 3);
    ((SingleInstrumentBase) this.pedalTypeInstrument).Instrument = new Qualifier((QualifierTypes) 4, "CPC04T", "Accel_Pedal_Type");
    ((Control) this.pedalTypeInstrument).Name = "pedalTypeInstrument";
    ((SingleInstrumentBase) this.pedalTypeInstrument).ShowUnits = false;
    ((Control) this.pedalTypeInstrument).TabStop = false;
    ((SingleInstrumentBase) this.pedalTypeInstrument).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.pedalTypeInstrument).TitleWordWrap = true;
    ((SingleInstrumentBase) this.pedalTypeInstrument).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.closeWarningLabel, "closeWarningLabel");
    this.closeWarningLabel.BackColor = SystemColors.Info;
    ((TableLayoutPanel) this.tableLayoutPanel).SetColumnSpan((Control) this.closeWarningLabel, 2);
    this.closeWarningLabel.Name = "closeWarningLabel";
    this.closeWarningLabel.UseCompatibleTextRendering = true;
    this.currentStatusLabel.AutoEllipsis = true;
    componentResourceManager.ApplyResources((object) this.currentStatusLabel, "currentStatusLabel");
    this.currentStatusLabel.Name = "currentStatusLabel";
    this.currentStatusLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.actionsPanel).ResumeLayout(false);
    ((Control) this.actionsPanel).PerformLayout();
    ((Control) this.tableLayoutPanel).ResumeLayout(false);
    ((Control) this.tableLayoutPanel).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum PedalState
  {
    Unknown,
    Initializing,
    InvalidPedalType,
    TemporaryPedalSettingActive,
    ValidPedalType,
  }

  private class Transaction
  {
    private Stack<Tuple<UserPanel.Transaction.ActionType, Action>> allCleanupActions = new Stack<Tuple<UserPanel.Transaction.ActionType, Action>>();

    public bool Commited { get; private set; }

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
      this.Commited = true;
      this.DoCleanup(false);
      this.Completed = true;
      this.OnTransactionCompleted();
    }

    public void Rollback() => this.Rollback((Exception) null);

    public void Rollback(Exception exception)
    {
      if (this.Completed)
        return;
      this.Commited = false;
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
