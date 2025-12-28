// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.ParameterView
// Assembly: Parameters, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 266306EF-5E5A-4E97-A95E-0BCBE6FD3F76
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Parameters.dll

using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Reflection;
using DetroitDiesel.Security;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public sealed class ParameterView : 
  UserControl,
  ISupportActivation,
  ISearchable,
  ISupportEdit,
  IProvideHtml,
  IContextHelp,
  IRefreshable,
  IFilterable,
  ISupportExpandCollapseAll
{
  private ChannelBaseCollection activeChannels;
  private MenuProxy menuProxy = MenuProxy.GlobalInstance;
  private string importExportPath = Directories.Parameters;
  private Queue<Channel> channelsToWrite = new Queue<Channel>();
  private List<KeyValuePair<ParameterView.ErrorStatus, string>> errorMessages = new List<KeyValuePair<ParameterView.ErrorStatus, string>>();
  private bool shutdownRequested;
  private List<Precondition> preconditions;
  private int inModalDialog;
  private Queue<Channel> channelsToAutoSave = new Queue<Channel>();
  private readonly List<Channel> parametersBeingRead = new List<Channel>();
  private Dictionary<Channel, ParameterView.Error> readErrors = new Dictionary<Channel, ParameterView.Error>();
  private bool performedInitialUpdateActiveChannels;
  private bool dirty;
  private bool writeStarted;
  private string importFile = string.Empty;
  private bool readStarted;
  private readonly ContextHelpChain helpChain;
  private IContainer components;
  private Button buttonSend;
  private ProgressBar progressBar;
  private PictureBox pictureBoxStatus;
  private System.Windows.Forms.Label labelStatus;
  private ToolTip toolTipStatus;
  private TableLayoutPanel tableLayoutPanel;
  private ParameterPanels panelHost;
  private ContextLinkButton informationLinkButton;

  public ParameterView()
  {
    this.menuProxy.ParameterView = this;
    this.InitializeComponent();
    this.preconditions = PreconditionManager.GlobalInstance.Preconditions.Where<Precondition>((System.Func<Precondition, bool>) (p => p.PreconditionType == 1 || p.PreconditionType == 3)).ToList<Precondition>();
    this.UpdateParameterWritePreconditionMonitoring();
    SapiManager.GlobalInstance.ActiveChannelsChanged += new EventHandler(this.OnActiveChannelsChanged);
    this.panelHost.FilteredContentChanged += new EventHandler(this.OnFilteredContentChanged);
    ServerDataManager.GlobalInstance.ProhibitedChannelsUpdated += new EventHandler(this.ServerDataManager_ProhibitedChannelsUpdated);
    this.helpChain = new ContextHelpChain((object) this.panelHost, LinkSupport.GetViewLink((PanelIdentifier) 6));
    this.informationLinkButton.Context = (IContextHelp) this.helpChain;
    this.informationLinkButton.UseAlternateContext = true;
    Application.EnterThreadModal += new EventHandler(this.Application_EnterThreadModal);
    Application.LeaveThreadModal += new EventHandler(this.Application_LeaveThreadModal);
  }

  private void Application_EnterThreadModal(object sender, EventArgs e) => ++this.inModalDialog;

  private void Application_LeaveThreadModal(object sender, EventArgs e)
  {
    --this.inModalDialog;
    if (this.inModalDialog != 0 || this.channelsToAutoSave.Count <= 0 || this.IsDisposed || !this.IsHandleCreated)
      return;
    this.BeginInvoke((Delegate) (() =>
    {
      while (this.channelsToAutoSave.Count > 0 && this.inModalDialog == 0)
        this.AutoSaveForServerUpload(this.channelsToAutoSave.Dequeue());
    }));
  }

  private void ServerDataManager_ProhibitedChannelsUpdated(object sender, EventArgs e)
  {
    if (!this.Visible)
      return;
    ParameterView.UpdateProhibitWarning();
  }

  private void Precondition_StateChanged(object sender, EventArgs e)
  {
    if (!this.Visible)
      return;
    this.UpdateStatus();
    this.UpdateProhibitWarningForPreconditions();
  }

  private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
  {
    this.AddChannel(sender as Channel);
  }

  private void OnDisconnectCompleteEvent(object sender, EventArgs e)
  {
    this.RemoveChannel(sender as Channel);
  }

  private void AddChannel(Channel channel)
  {
    if (channel == null || !SapiManager.GlobalInstance.Online)
      return;
    channel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.ParametersReadCompleteEvent);
    channel.Parameters.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.ParameterUpdateEvent);
    channel.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.OnParameterCollectionWriteCompleteEvent);
    channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelCommunicationsStateUpdateEvent);
    channel.InitCompleteEvent += new InitCompleteEventHandler(this.OnChannelInitCompleteEvent);
    if (!channel.Online || channel.CommunicationsState == CommunicationsState.OnlineButNotInitialized || channel.CommunicationsState == CommunicationsState.ReadEcuInfo)
      return;
    this.InitializeComplete(channel);
  }

  private void OnChannelInitCompleteEvent(object sender, EventArgs e)
  {
    if (!(sender is Channel channel))
      return;
    this.InitializeComplete(channel);
  }

  private void InitializeComplete(Channel channel)
  {
    if (!this.Visible || !channel.Online || this.parametersBeingRead.Contains(channel) || channel.CommunicationsState == CommunicationsState.ReadParameters || !channel.Parameters.Any<Parameter>())
      return;
    this.parametersBeingRead.Add(channel);
    channel.Parameters.Read(false);
  }

  private void RemoveChannel(Channel channel)
  {
    if (channel == null)
      return;
    channel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.ParametersReadCompleteEvent);
    channel.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.OnParameterCollectionWriteCompleteEvent);
    channel.Parameters.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.ParameterUpdateEvent);
    channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelCommunicationsStateUpdateEvent);
    channel.InitCompleteEvent -= new InitCompleteEventHandler(this.OnChannelInitCompleteEvent);
    this.readErrors.Remove(channel);
    if (!this.IsDisposed && !this.shutdownRequested)
    {
      this.CheckForUnsentChanges(channel, ParameterType.Parameter);
      this.CheckForUnsentChanges(channel, ParameterType.Accumulator);
    }
    this.ClearStatus();
  }

  private void UpdateActiveChannels()
  {
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent -= new ConnectCompleteEventHandler(this.OnConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
    }
    this.activeChannels = SapiManager.GlobalInstance.ActiveChannels;
    if (this.activeChannels == null)
      return;
    this.activeChannels.ConnectCompleteEvent += new ConnectCompleteEventHandler(this.OnConnectCompleteEvent);
    this.activeChannels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
    foreach (Channel activeChannel in this.activeChannels)
      this.AddChannel(activeChannel);
  }

  private void ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.UpdateStatus();
    ParameterCollection parameters = sender as ParameterCollection;
    ParameterView.Error readError = new ParameterView.Error();
    if (e.Succeeded)
    {
      List<Tuple<ParameterGroup, Exception>> tupleList = new List<Tuple<ParameterGroup, Exception>>();
      foreach (ParameterGroup parameterGroup in (ReadOnlyCollection<ParameterGroup>) parameters.Channel.ParameterGroups)
      {
        IEnumerable<IGrouping<Exception, Parameter>> source = parameterGroup.Parameters.GroupBy<Parameter, Exception>((System.Func<Parameter, Exception>) (p => p.Exception));
        if (source.Count<IGrouping<Exception, Parameter>>() == 1 && source.First<IGrouping<Exception, Parameter>>().Key != null)
          tupleList.Add(Tuple.Create<ParameterGroup, Exception>(parameterGroup, source.First<IGrouping<Exception, Parameter>>().Key));
      }
      bool fatal = tupleList.Count > 0 && tupleList.Count == parameters.Channel.ParameterGroups.Count;
      tupleList.ForEach((Action<Tuple<ParameterGroup, Exception>>) (domainException => readError.AddMessage($"{(object) parameters.Channel}.{domainException.Item1.Qualifier}: {domainException.Item2.Message}", (fatal ? 1 : 0) != 0)));
      if (!fatal)
      {
        ServerDataManager.GlobalInstance.AutoSaveSettings(parameters.Channel, (ServerDataManager.AutoSaveDestination) 1, "ECUREAD");
        this.AutoSaveForServerUpload(parameters.Channel);
      }
    }
    else
      readError.AddMessage($"{(object) parameters.Channel}: {e.Exception.Message}", true);
    this.readErrors[parameters.Channel] = readError;
    ParameterView.Error allReadErrors = new ParameterView.Error();
    this.readErrors.Where<KeyValuePair<Channel, ParameterView.Error>>((System.Func<KeyValuePair<Channel, ParameterView.Error>, bool>) (entry => entry.Key.Online && entry.Value.HasMessages)).ToList<KeyValuePair<Channel, ParameterView.Error>>().ForEach((Action<KeyValuePair<Channel, ParameterView.Error>>) (entry => allReadErrors.ImportErrors(entry.Value)));
    this.EndRead(allReadErrors);
    if (parameters == null)
      return;
    this.parametersBeingRead.Remove(parameters.Channel);
  }

  private void AutoSaveForServerUpload(Channel channel)
  {
    if (this.inModalDialog > 0)
      this.channelsToAutoSave.Enqueue(channel);
    else
      ServerDataManager.GlobalInstance.AutoSaveSettings(channel, (ServerDataManager.AutoSaveDestination) 0, "ECUUPDATE");
  }

  private void ParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    if (!e.Succeeded && sender is Parameter parameter)
      StatusLog.Add(new StatusMessage(e.Exception.Message, (StatusMessageType) 1, (object) $"{parameter.Channel.Ecu.Name}.{parameter.GroupQualifier}.{parameter.Qualifier}"));
    this.UpdateStatus();
  }

  private void OnSendClick(object sender, EventArgs e)
  {
    if (new ConfirmSendForm().ShowDialog() != DialogResult.OK)
      return;
    foreach (Channel activeChannel in this.activeChannels)
    {
      if (activeChannel.Online)
      {
        bool flag1 = true;
        PasswordManager passwordManager = this.menuProxy.GetPasswordManager(activeChannel);
        if (passwordManager != null)
        {
          try
          {
            bool[] flagArray = passwordManager.AcquireRelevantListStatus(this.progressBar);
            bool flag2 = false;
            foreach (bool flag3 in flagArray)
            {
              if (flag3)
              {
                flag2 = true;
                break;
              }
            }
            if (flag2)
            {
              if (((Form) new PasswordEntryDialog(activeChannel, flagArray, passwordManager)).ShowDialog() != DialogResult.OK)
              {
                flag1 = false;
                if (activeChannel.Online)
                  this.ReportWarning(Resources.ProvidePassword, string.Empty);
              }
            }
          }
          catch (CaesarException ex)
          {
            if (ex.NegativeResponseCode != 17)
            {
              int num = (int) ControlHelpers.ShowMessageBox((Control) this, ex.NegativeResponseCode != (int) sbyte.MaxValue ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormatGatherPasswordOtherError, (object) ex.Message) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormatGatherPasswordVehicleMoving, (object) activeChannel.Ecu.Name), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
              flag1 = false;
            }
            else
              StatusLog.Add(new StatusMessage($"Password protection services were indicated as supported for {activeChannel.Ecu.Name} but the ECU reported NRC $11 'service not supported'. Continuing without password protection.", (StatusMessageType) 2, (object) this));
          }
        }
        if (flag1 && !ServerDataManager.GlobalInstance.ProhibitedChannels.Contains<Channel>(activeChannel) && activeChannel.Parameters.Any<Parameter>((System.Func<Parameter, bool>) (p => ParameterView.ParameterChanged(p))))
          this.channelsToWrite.Enqueue(activeChannel);
      }
    }
    if (this.SendNextWrite())
      return;
    this.UpdateStatus();
  }

  private bool SendNextWrite()
  {
    if (this.channelsToWrite.Count <= 0)
      return false;
    Channel channel = this.channelsToWrite.Dequeue();
    if (!channel.Online)
      return this.SendNextWrite();
    channel.Parameters.Write(false);
    this.UpdateStatus();
    return true;
  }

  private void OnParameterCollectionWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    this.UpdateStatus();
    ParameterCollection parameterCollection = sender as ParameterCollection;
    if (!e.Succeeded)
    {
      this.EndWrite(ParameterView.ErrorStatus.error, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_Format_ErrorsOccurred_NoValidation, (object) parameterCollection.Channel.Ecu.Name, (object) e.Exception.Message));
    }
    else
    {
      ParameterView.ErrorStatus status = ParameterView.ErrorStatus.noError;
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_Format_DeviceReportedTheFollowingErrors, (object) parameterCollection.Channel.Ecu.Name);
      stringBuilder2.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_Format_DeviceReportedTheFollowingWarnings, (object) parameterCollection.Channel.Ecu.Name);
      foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) parameterCollection)
      {
        if (parameter.Marked && parameter.Exception != null)
        {
          if (parameter.Exception is CaesarException exception && exception.ErrorNumber == 6602L)
          {
            status |= ParameterView.ErrorStatus.warning;
            stringBuilder2.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_Format_WarningOrError, (object) parameter.Name, (object) parameter.Exception.Message);
          }
          else
          {
            status |= ParameterView.ErrorStatus.error;
            stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_Format_WarningOrError, (object) parameter.Name, (object) parameter.Exception.Message);
          }
        }
      }
      StringBuilder stringBuilder3 = new StringBuilder((string) null);
      if ((status & ParameterView.ErrorStatus.error) != ParameterView.ErrorStatus.noError)
        stringBuilder3.Append(stringBuilder1.ToString());
      if ((status & ParameterView.ErrorStatus.warning) != ParameterView.ErrorStatus.noError)
      {
        if ((status & ParameterView.ErrorStatus.error) != ParameterView.ErrorStatus.noError)
        {
          stringBuilder3.Append(Environment.NewLine);
          stringBuilder3.Append(Environment.NewLine);
        }
        stringBuilder3.Append(stringBuilder2.ToString());
      }
      this.EndWrite(status, stringBuilder3.ToString());
      ServerDataManager.GlobalInstance.AutoSaveSettings(parameterCollection.Channel, (ServerDataManager.AutoSaveDestination) 1, "ECUWRITE");
      this.AutoSaveForServerUpload(parameterCollection.Channel);
    }
  }

  protected override void OnLoad(EventArgs e)
  {
    this.ParentForm.FormClosing += new FormClosingEventHandler(this.ParentForm_FormClosing);
    base.OnLoad(e);
  }

  private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel || this.IsDisposed)
      return;
    bool flag = false;
    if (this.activeChannels == null)
      return;
    foreach (Channel activeChannel in this.activeChannels)
    {
      if (ParameterView.HasUnsentChanges(activeChannel, ParameterType.Parameter) || ParameterView.HasUnsentChanges(activeChannel, ParameterType.Accumulator))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    if (ControlHelpers.ShowMessageBox((Control) this, Resources.MessageFormat_ShutdownRequestedWithUnsavedChanges, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
      e.Cancel = true;
    else
      this.shutdownRequested = true;
  }

  private void OnActiveChannelsChanged(object sender, EventArgs e) => this.UpdateActiveChannels();

  protected override void OnVisibleChanged(EventArgs e)
  {
    this.UpdateParameterWritePreconditionMonitoring();
    if (this.Visible)
    {
      if (this.activeChannels != null)
      {
        foreach (Channel activeChannel in this.activeChannels)
        {
          if (activeChannel.Online && !activeChannel.Parameters.HaveBeenReadFromEcu && !this.parametersBeingRead.Contains(activeChannel) && activeChannel.CommunicationsState != CommunicationsState.ReadParameters && activeChannel.CommunicationsState != CommunicationsState.ReadEcuInfo && activeChannel.Parameters.Count > 0)
          {
            this.parametersBeingRead.Add(activeChannel);
            activeChannel.Parameters.Read(false);
          }
        }
        this.UpdateStatus();
        ParameterView.UpdateProhibitWarning();
      }
      this.UpdateProhibitWarningForPreconditions();
    }
    else
    {
      foreach (object name in Enum.GetNames(typeof (ServerDataManager.ProhibitReason)))
        WarningsPanel.GlobalInstance.Remove(name.ToString());
      this.preconditions.ForEach((Action<Precondition>) (pc => WarningsPanel.GlobalInstance.Remove(pc.PreconditionType.ToString())));
    }
    base.OnVisibleChanged(e);
  }

  private static void UpdateProhibitWarningFoReason(
    ServerDataManager.ProhibitReason reason,
    string messageFormat)
  {
    IEnumerable<Channel> source = ServerDataManager.GlobalInstance.ProhibitedChannels.Where<Channel>((System.Func<Channel, bool>) (c => ServerDataManager.GlobalInstance.GetProhibitReason(c) == reason));
    if (source.Any<Channel>())
    {
      EventHandler eventHandler = ApplicationInformation.CanReprogramEdexUnits ? new EventHandler(ParameterView.warningPanel_Click) : (EventHandler) null;
      WarningsPanel.GlobalInstance.Add(reason.ToString(), MessageBoxIcon.Asterisk, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, messageFormat, (object) string.Join(", ", source.Select<Channel, string>((System.Func<Channel, string>) (c => c.Ecu.Name)).ToArray<string>())), eventHandler);
    }
    else
      WarningsPanel.GlobalInstance.Remove(reason.ToString());
  }

  private static void UpdateProhibitWarning()
  {
    ParameterView.UpdateProhibitWarningFoReason((ServerDataManager.ProhibitReason) 2, Resources.MessageFormat_MismatchLastServicedData);
    ParameterView.UpdateProhibitWarningFoReason((ServerDataManager.ProhibitReason) 3, ApplicationInformation.CanReprogramEdexUnits ? Resources.MessageFormat_MissingLastServicedData : Resources.MessageFormat_ToolDoesntSupportLastServicedData);
    ParameterView.UpdateProhibitWarningFoReason((ServerDataManager.ProhibitReason) 4, ApplicationInformation.CanReprogramEdexUnits ? Resources.MessageFormat_MissingIncompatibilityTable : Resources.MessageFormat_ToolDoesntSupportIncompatibilityTableUpdate);
    ParameterView.UpdateProhibitWarningFoReason((ServerDataManager.ProhibitReason) 5, Resources.MessageFormat_ToolDoesntSupportParameterEditing);
    ParameterView.UpdateProhibitWarningFoReason((ServerDataManager.ProhibitReason) 6, Resources.MessageFormat_NoVcp);
  }

  private void UpdateProhibitWarningForPreconditions()
  {
    foreach (Precondition precondition1 in this.preconditions)
    {
      Precondition precondition = precondition1;
      if (precondition.State == 2)
      {
        string localizedDisplayName = precondition.DiagnosticPanelName != null ? ActionsMenuProxy.GlobalInstance.GetDialogLocalizedDisplayName(precondition.DiagnosticPanelName, (string) null, false) : (string) null;
        if (localizedDisplayName != null)
          WarningsPanel.GlobalInstance.Add(precondition.PreconditionType.ToString(), MessageBoxIcon.Asterisk, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_FormatPreconditionWithDialog, (object) precondition.Text, (object) localizedDisplayName), (EventHandler) ((sender, e) => ActionsMenuProxy.GlobalInstance.ShowDialog(precondition.DiagnosticPanelName, (string) null, (object) null, false)));
        else
          WarningsPanel.GlobalInstance.Add(precondition.PreconditionType.ToString(), MessageBoxIcon.Asterisk, (string) null, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_FormatPrecondition, (object) precondition.Text), (EventHandler) null);
      }
      else
        WarningsPanel.GlobalInstance.Remove(precondition.PreconditionType.ToString());
    }
  }

  private static void warningPanel_Click(object sender, EventArgs e)
  {
    ((IMenuProxy) MenuProxy.GlobalInstance).ContainerApplication.SelectPlace((PanelIdentifier) 7, (string) null);
  }

  internal static ParameterFileFormat GetFileFormat(string fileName)
  {
    ParameterFileFormat fileFormat = ParameterFileFormat.ParFile;
    if (fileName.ToUpper(CultureInfo.CurrentCulture).EndsWith(".VER", StringComparison.OrdinalIgnoreCase))
      fileFormat = ParameterFileFormat.VerFile;
    return fileFormat;
  }

  private static Channel GetConnectedChannel(string ecuName)
  {
    Channel connectedChannel = (Channel) null;
    foreach (Channel channel in (ChannelBaseCollection) SapiManager.GlobalInstance.Sapi.Channels)
    {
      if (channel.Ecu.Name == ecuName)
      {
        connectedChannel = channel;
        break;
      }
    }
    return connectedChannel;
  }

  internal bool OnParametersHistoryImportClick(object sender, EventArgs e)
  {
    bool flag = false;
    OpenHistoryForm openHistoryForm = new OpenHistoryForm();
    if (openHistoryForm.ShowDialog() == DialogResult.OK)
    {
      Cursor.Current = Cursors.WaitCursor;
      this.StartRead(openHistoryForm.EntryName);
      byte[] buffer = (byte[]) null;
      try
      {
        buffer = FileEncryptionProvider.ReadEncryptedFile(openHistoryForm.FileName, true);
      }
      catch (InvalidChecksumException ex)
      {
        this.EndRead(new ParameterView.Error(((Exception) ex).Message));
      }
      ParameterView.Error error = (ParameterView.Error) null;
      if (buffer != null)
      {
        using (MemoryStream memoryStream = new MemoryStream(buffer))
        {
          using (StreamReader stream = new StreamReader((Stream) memoryStream))
            error = this.InternalImport(stream, ParameterFileFormat.VerFile, true);
        }
        flag = !error.Fatal;
        this.EndRead(error);
        Cursor.Current = Cursors.Default;
      }
    }
    return flag;
  }

  internal string ShowFileImportDialog(ParameterType type)
  {
    OpenFileDialog openFileDialog = new OpenFileDialog();
    openFileDialog.InitialDirectory = this.importExportPath;
    if (type == ParameterType.Parameter)
      openFileDialog.Filter = Resources.J2286OpenFilesFilter;
    else
      openFileDialog.Filter = Resources.J2286AccumulatorFilesFilter;
    openFileDialog.FilterIndex = 1;
    openFileDialog.RestoreDirectory = false;
    return openFileDialog.ShowDialog((IWin32Window) this) == DialogResult.OK ? openFileDialog.FileName : (string) null;
  }

  internal bool OnParametersImportClick(string path)
  {
    bool flag = false;
    if (path != null)
    {
      Cursor.Current = Cursors.WaitCursor;
      this.importExportPath = path;
      this.StartRead(path);
      ParameterView.Error error = (ParameterView.Error) null;
      using (StreamReader stream = new StreamReader(path))
      {
        ParameterFileFormat fileFormat = ParameterView.GetFileFormat(path);
        error = this.InternalImport(stream, fileFormat, false);
      }
      flag = !error.Fatal;
      this.EndRead(error);
      Cursor.Current = Cursors.Default;
    }
    return flag;
  }

  private ParameterView.Error InternalImport(
    StreamReader stream,
    ParameterFileFormat format,
    bool encrypted)
  {
    Channel targetChannel = (Channel) null;
    Collection<string> unknownList = new Collection<string>();
    ParameterView.Error error = new ParameterView.Error();
    try
    {
      targetChannel = this.ImportToChannel(stream, format, encrypted, unknownList);
    }
    catch (DataException ex)
    {
      error.AddMessage(ex.Message, true);
    }
    catch (InvalidOperationException ex)
    {
      error.AddMessage(ex.Message, true);
    }
    catch (IOException ex)
    {
      error.AddMessage(ex.Message, true);
    }
    catch (FormatException ex)
    {
      error.AddMessage(ex.Message, true);
    }
    if (!error.Fatal && targetChannel != null)
    {
      error.ImportErrors(ParameterView.GetParameterErrors(targetChannel));
      error.ImportErrors(ParameterView.GetUnknownParameters((IEnumerable<string>) unknownList));
      if (targetChannel.Parameters != null)
      {
        if (SapiExtensions.IsDataSourceEdex(targetChannel.Ecu) && !SapiManager.GlobalInstance.UseNameValuePairsToParameterize)
        {
          string softwarePartNumber = SapiManager.GetSoftwarePartNumber(targetChannel);
          EdexIncompatibilityFlashwareItem incompatibilityFlashwareItem = ServerDataManager.GlobalInstance.EdexIncompatibilityTable.FlashwareItems.FirstOrDefault<EdexIncompatibilityFlashwareItem>((System.Func<EdexIncompatibilityFlashwareItem, bool>) (fi => PartExtensions.IsEqual(fi.PartNumber, softwarePartNumber)));
          foreach (IGrouping<string, Parameter> source in targetChannel.Parameters.Where<Parameter>((System.Func<Parameter, bool>) (p => ParameterView.ParameterChanged(p))).GroupBy<Parameter, string>((System.Func<Parameter, string>) (p => p.GroupQualifier)))
          {
            Parameter parameter1 = source.First<Parameter>();
            ParameterGroupDataItem parameterGroupDataItem = new ParameterGroupDataItem(parameter1, new Qualifier((QualifierTypes) 128 /*0x80*/, targetChannel.Ecu.Name, parameter1.GroupQualifier), ServerDataManager.GlobalInstance.GetFactoryAggregatePart(targetChannel, parameter1.GroupQualifier), ServerDataManager.GlobalInstance.GetEngineeringCorrectionFactorParameters(targetChannel, parameter1.GroupQualifier));
            foreach (Parameter parameter2 in (IEnumerable<Parameter>) source)
            {
              bool flag = false;
              CodingChoice codingChoice = new ParameterDataItem(parameter2, new Qualifier((QualifierTypes) 4, targetChannel.Ecu.Name, parameter2.Qualifier), parameterGroupDataItem).ValueAsCodingChoice;
              if (codingChoice != null)
              {
                if (incompatibilityFlashwareItem != null && incompatibilityFlashwareItem.ProhibitedParameters.Any<Part>((System.Func<Part, bool>) (p => PartExtensions.IsEqual(p, codingChoice.Part.ToString()))))
                  error.AddMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReportWarning_ParameterBlacklistFailure, (object) codingChoice.Part.ToString(), (object) parameter2.Qualifier), false);
                else
                  flag = true;
              }
              else
                error.AddMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReportWarning_ParameterNoPartNumberFailure, parameter2.Value, (object) parameter2.Qualifier), false);
              if (!flag && parameter2.OriginalValue != null)
                parameter2.Value = parameter2.OriginalValue;
            }
          }
        }
        if (!targetChannel.Parameters.Any<Parameter>((System.Func<Parameter, bool>) (p => ParameterView.ParameterChanged(p))))
          error.AddMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReportWarningNoParametersChanged, (object) targetChannel.Ecu.Name), false);
      }
    }
    return error;
  }

  private Channel ImportToChannel(
    StreamReader stream,
    ParameterFileFormat format,
    bool encrypted,
    Collection<string> unknownList)
  {
    Channel targetChannel = (Channel) null;
    TargetEcuDetails targetEcuDetails = ParameterCollection.GetTargetEcuDetails(stream, format);
    targetChannel = targetEcuDetails != null && !string.IsNullOrEmpty(targetEcuDetails.Ecu) ? ParameterView.GetConnectedChannel(targetEcuDetails.Ecu) : throw new InvalidOperationException(Resources.ReportError_NotAValidFile);
    if (targetChannel != null)
    {
      if (targetChannel.DiagnosisVariant.IsBase)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotImportParametersToBaseVariant, (object) targetEcuDetails.Ecu));
      if (ServerDataManager.GlobalInstance.ProhibitedChannels.Any<Channel>((System.Func<Channel, bool>) (c => c.Ecu.Name == targetChannel.Ecu.Name)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ChannelLockedForEditing, (object) targetEcuDetails.Ecu));
      if (targetEcuDetails.DiagnosisVariant.Length > 0 && !string.Equals(targetEcuDetails.DiagnosisVariant, targetChannel.DiagnosisVariant.Name, StringComparison.OrdinalIgnoreCase))
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, Resources.VariantDoesNotMatchFormat, (object) targetEcuDetails.DiagnosisVariant, (object) targetChannel.DiagnosisVariant.Name);
        if (encrypted)
        {
          stringBuilder.Append(Environment.NewLine);
          stringBuilder.Append(Environment.NewLine);
          stringBuilder.Append(Resources.ReviewParameters);
        }
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append(Resources.ContinueImporting);
        if (MessageBox.Show(stringBuilder.ToString(), ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, ControlHelpers.IsRightToLeft((Control) this) ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : (MessageBoxOptions) 0) == DialogResult.No)
          targetChannel = (Channel) null;
      }
      if (targetChannel != null)
      {
        if (encrypted && targetChannel.Online)
        {
          foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) targetChannel.Parameters)
          {
            if (parameter.DefaultValue != null && parameter.WriteAccess <= SapiManager.GlobalInstance.Sapi.HardwareAccess)
              parameter.Value = parameter.DefaultValue;
          }
        }
        targetChannel.Parameters.Load(stream, format, unknownList, !encrypted);
      }
    }
    else
    {
      if (SapiManager.GlobalInstance.Sapi.Channels.Where<Channel>((System.Func<Channel, bool>) (x => x.ConnectionResource != null)).Any<Channel>())
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.FormatCanNotImport, (object) targetEcuDetails.Ecu));
      if (targetEcuDetails.DiagnosisVariant.Length > 0)
      {
        targetChannel = SapiManager.GlobalInstance.Sapi.Channels.ConnectOffline(stream, format, unknownList);
      }
      else
      {
        Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[targetEcuDetails.Ecu];
        if (ecu == null)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.FormatDeviceDoesNotExist, (object) targetEcuDetails.Ecu));
        DiagnosisVariant diagnosisVariant = ecu.DiagnosisVariants[targetEcuDetails.AssumedDiagnosisVariant];
        VariantSelect variantSelect = new VariantSelect(ecu, diagnosisVariant);
        int num = (int) variantSelect.ShowDialog();
        if (variantSelect.DiagnosisVariant != null)
        {
          targetChannel = SapiManager.GlobalInstance.Sapi.Channels.ConnectOffline(variantSelect.DiagnosisVariant);
          targetChannel.Parameters.Load(stream, format, unknownList, !encrypted);
        }
      }
    }
    return targetChannel;
  }

  private static ParameterView.Error GetParameterErrors(Channel targetChannel)
  {
    ParameterView.Error parameterErrors = new ParameterView.Error();
    foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) targetChannel.Parameters)
    {
      if (parameter.Exception != null)
        parameterErrors.AddMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_ErrorMessageFormat, (object) parameter.GroupName, (object) parameter.Name, (object) parameter.Exception.Message), false);
    }
    return parameterErrors;
  }

  private static ParameterView.Error GetUnknownParameters(IEnumerable<string> unknownList)
  {
    ParameterView.Error unknownParameters = new ParameterView.Error();
    foreach (string unknown in unknownList)
      unknownParameters.AddMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_ErrorMessageFormat_UnknownParameter, (object) unknown), false);
    return unknownParameters;
  }

  internal void OnParametersExportClick(Channel channel, ParameterType type)
  {
    ParameterExportForm parameterExportForm = new ParameterExportForm(channel, type);
    parameterExportForm.ExportPath = this.importExportPath;
    int num = (int) parameterExportForm.ShowDialog((IWin32Window) this);
    this.importExportPath = parameterExportForm.ExportPath;
  }

  private void OnChannelCommunicationsStateUpdateEvent(
    object sender,
    CommunicationsStateEventArgs e)
  {
    this.UpdateStatus();
  }

  private static bool HasUnsentChanges(Channel c, ParameterType type)
  {
    bool flag = false;
    foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) c.Parameters)
    {
      if (!object.Equals(parameter.Value, parameter.OriginalValue) && !object.Equals(parameter.Value, parameter.LastPersistedValue) && (type == ParameterType.Accumulator && !parameter.Persistable || type == ParameterType.Parameter && parameter.Persistable))
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  private bool AskUserToSaveChanges(Channel c, ParameterType type)
  {
    return ControlHelpers.ShowMessageBox((Control) this, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ChangesToExport, (object) type, (object) c.Ecu.Name), MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
  }

  private void CheckForUnsentChanges(Channel c, ParameterType type)
  {
    if (!ParameterView.HasUnsentChanges(c, type) || !this.AskUserToSaveChanges(c, type))
      return;
    int num = (int) new ParameterExportForm(c, type).ShowDialog();
  }

  private void UpdateStatus()
  {
    this.dirty = true;
    this.Invalidate();
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    if (!this.performedInitialUpdateActiveChannels)
    {
      this.performedInitialUpdateActiveChannels = true;
      this.UpdateActiveChannels();
      ParameterView.UpdateProhibitWarning();
    }
    if (this.dirty)
    {
      this.dirty = false;
      this.UpdateStatusDeferred();
    }
    base.OnPaint(e);
  }

  private void UpdateStatusDeferred()
  {
    int notificationCount = 0;
    int num = 0;
    if (this.activeChannels != null)
    {
      foreach (Channel activeChannel in this.activeChannels)
      {
        switch (activeChannel.CommunicationsState)
        {
          case CommunicationsState.Online:
            notificationCount += activeChannel.Parameters.Count<Parameter>((System.Func<Parameter, bool>) (p => ParameterView.ParameterChanged(p)));
            continue;
          case CommunicationsState.ReadParameters:
            this.StartRead(string.Empty);
            break;
          case CommunicationsState.WriteParameters:
            this.StartWrite();
            break;
          default:
            continue;
        }
        num = num <= 0 ? (int) activeChannel.Parameters.Progress : (num + (int) activeChannel.Parameters.Progress) / 2;
      }
    }
    this.progressBar.Value = num;
    this.buttonSend.Enabled = notificationCount > 0 && this.channelsToWrite.Count == 0 && this.preconditions.All<Precondition>((System.Func<Precondition, bool>) (pc => pc.State != 2));
    this.menuProxy.Notify(notificationCount);
  }

  private static bool ParameterChanged(Parameter parameter)
  {
    return !object.Equals(parameter.Value, parameter.OriginalValue);
  }

  private void StartWrite()
  {
    if (this.writeStarted)
      return;
    this.ReportStatus((Image) Resources.readwrite, Resources.WritingParameters, string.Empty, false);
    this.writeStarted = true;
  }

  private void EndWrite(ParameterView.ErrorStatus status, string message)
  {
    if (!this.writeStarted)
      return;
    if (status != ParameterView.ErrorStatus.noError)
      this.errorMessages.Add(new KeyValuePair<ParameterView.ErrorStatus, string>(status, message));
    this.writeStarted = false;
    if (this.SendNextWrite())
      return;
    this.ReportWriteResult();
  }

  private void ReportWriteResult()
  {
    if (this.errorMessages.Count == 0)
    {
      this.ReportSuccess(Resources.ParametersWritten);
    }
    else
    {
      StringBuilder stringBuilder = new StringBuilder();
      ParameterView.ErrorStatus errorStatus = ParameterView.ErrorStatus.noError;
      foreach (KeyValuePair<ParameterView.ErrorStatus, string> errorMessage in this.errorMessages)
      {
        errorStatus |= errorMessage.Key;
        stringBuilder.Append(errorMessage.Value).AppendLine();
      }
      switch (errorStatus)
      {
        case ParameterView.ErrorStatus.warning:
          this.ReportWarning(Resources.ParameterView_OneOrMoreWarningsDuringWrite, stringBuilder.ToString());
          break;
        case ParameterView.ErrorStatus.error:
          this.ReportError(Resources.ParameterView_OneOrMoreErrorsDuringWrite, stringBuilder.ToString());
          break;
        case ParameterView.ErrorStatus.warning | ParameterView.ErrorStatus.error:
          this.ReportError(Resources.ParameterView_OneOrMoreErrorsAndWarningsDuringWrite, stringBuilder.ToString());
          break;
      }
      this.errorMessages.Clear();
    }
  }

  private void StartRead(string source)
  {
    if (this.readStarted)
      return;
    this.importFile = source;
    if (string.IsNullOrEmpty(this.importFile))
      this.ReportStatus((Image) Resources.readwrite, Resources.ReadingParameters, string.Empty, false);
    else
      this.ReportStatus((Image) Resources.readwrite, Resources.ImportingParameters, string.Empty, false);
    this.readStarted = true;
  }

  private void EndRead(ParameterView.Error error)
  {
    if (!this.readStarted)
      return;
    if (string.IsNullOrEmpty(this.importFile))
    {
      if (error == null || !error.HasMessages)
        this.ReportSuccess(Resources.ReportSuccess_ParametersSuccessfullyRead);
      else
        this.ReportStatus(error.Fatal ? (Image) Resources.error : (Image) Resources.warning, Resources.ReportError_OneOrMoreErrorsOccurredReading, error.DisplayMessage, false);
    }
    else if (error == null || !error.HasMessages)
      this.ReportSuccess(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReportSuccess_ParametersSuccessfullyImported, (object) this.importFile));
    else
      this.ReportError(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReportError_OneOrMoreErrorsOccurredImporting, (object) this.importFile), error.DisplayMessage);
    this.readStarted = false;
    this.importFile = string.Empty;
  }

  private void ReportSuccess(string title)
  {
    this.ReportStatus((Image) Resources.done, title, string.Empty, false);
  }

  private void ReportError(string title, string error)
  {
    this.ReportStatus((Image) Resources.error, title, error, true);
  }

  private void ReportWarning(string title, string error)
  {
    this.ReportStatus((Image) Resources.outofrange, title, error, false);
  }

  private void ClearStatus() => this.ReportStatus((Image) null, string.Empty, string.Empty, false);

  private void ReportStatus(Image image, string title, string additional, bool showBox)
  {
    this.pictureBoxStatus.Image = image;
    string empty = string.Empty;
    string caption;
    if (string.IsNullOrEmpty(additional))
    {
      this.labelStatus.Text = title;
      caption = title;
    }
    else
    {
      this.labelStatus.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_Format_StatusLabel, (object) title);
      caption = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterView_Format_FullStatus, (object) title, (object) additional);
    }
    if (showBox)
      StatusMessageBox.Report(this.Parent, caption, (StatusMessageType) 2);
    this.toolTipStatus.SetToolTip((Control) this.labelStatus, caption);
    this.toolTipStatus.SetToolTip((Control) this.pictureBoxStatus, caption);
  }

  private void UpdateParameterWritePreconditionMonitoring()
  {
    if (this.Visible)
      this.preconditions.ForEach((Action<Precondition>) (pc => pc.StateChanged += new EventHandler(this.Precondition_StateChanged)));
    else
      this.preconditions.ForEach((Action<Precondition>) (pc => pc.StateChanged -= new EventHandler(this.Precondition_StateChanged)));
  }

  public bool Search(string searchText, bool caseSensitive, FindMode direction)
  {
    return this.panelHost.Search(searchText, caseSensitive, direction);
  }

  public bool CanSearch => this.panelHost.CanSearch;

  public bool CanUndo => this.panelHost.CanUndo;

  public void Undo() => this.panelHost.Undo();

  public bool CanCopy => this.panelHost.CanCopy;

  public void Copy() => this.panelHost.Copy();

  public bool CanDelete => this.panelHost.CanDelete;

  public bool CanPaste => this.panelHost.CanPaste;

  public void Cut() => this.panelHost.Cut();

  public bool CanSelectAll => this.panelHost.CanSelectAll;

  public void Delete() => this.panelHost.Delete();

  public bool CanCut => this.panelHost.CanCut;

  public void Paste() => this.panelHost.Paste();

  public void SelectAll() => this.panelHost.SelectAll();

  public bool CanProvideHtml => this.panelHost.CanProvideHtml;

  public string ToHtml() => this.panelHost.ToHtml();

  public string StyleSheet => this.panelHost.StyleSheet;

  public bool Active => this.panelHost.Active;

  public void Activate() => this.panelHost.Activate();

  public bool Deactivate() => this.panelHost.Deactivate();

  public bool SelectLocation(string namespaceLocation)
  {
    return this.panelHost.SelectLocation(namespaceLocation);
  }

  public Link ContextLink => this.helpChain.ContextLink;

  public Link AlternateContextLink => this.helpChain.AlternateContextLink;

  public event EventHandler ContextLinkChanged
  {
    add => this.helpChain.ContextLinkChanged += value;
    remove => this.helpChain.ContextLinkChanged -= value;
  }

  public bool CanRefreshView
  {
    get
    {
      if (this.activeChannels != null)
      {
        foreach (Channel activeChannel in this.activeChannels)
        {
          if (activeChannel.CommunicationsState == CommunicationsState.Online)
            return true;
        }
      }
      return false;
    }
  }

  public void RefreshView()
  {
    if (this.activeChannels == null)
      return;
    foreach (Channel activeChannel in this.activeChannels)
    {
      if (activeChannel.CommunicationsState == CommunicationsState.Online)
        activeChannel.Parameters.ReadAll(false);
    }
  }

  public IEnumerable<FilterTypes> Filters => this.panelHost.Filters;

  public int NumberOfItemsFiltered => this.panelHost.NumberOfItemsFiltered;

  public int TotalNumberOfFilterableItems => this.panelHost.TotalNumberOfFilterableItems;

  public event EventHandler FilteredContentChanged;

  private void OnFilteredContentChanged(object sender, EventArgs e)
  {
    if (this.FilteredContentChanged == null)
      return;
    this.FilteredContentChanged((object) this, e);
  }

  public void OnDispose(bool disposing)
  {
    if (!disposing || this.panelHost == null)
      return;
    this.panelHost.FilteredContentChanged -= new EventHandler(this.OnFilteredContentChanged);
  }

  public bool CanExpandAllItems => this.panelHost != null && this.panelHost.CanExpandAllItems;

  public bool CanCollapseAllItems => this.panelHost != null && this.panelHost.CanCollapseAllItems;

  public void ExpandAllItems()
  {
    if (this.panelHost == null)
      return;
    this.panelHost.ExpandAllItems();
  }

  public void CollapseAllItems()
  {
    if (this.panelHost == null)
      return;
    this.panelHost.CollapseAllItems();
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ParameterView));
    this.informationLinkButton = new ContextLinkButton();
    this.labelStatus = new System.Windows.Forms.Label();
    this.pictureBoxStatus = new PictureBox();
    this.progressBar = new ProgressBar();
    this.buttonSend = new Button();
    this.panelHost = new ParameterPanels();
    this.tableLayoutPanel = new TableLayoutPanel();
    this.toolTipStatus = new ToolTip(this.components);
    Panel panel = new Panel();
    panel.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxStatus).BeginInit();
    this.tableLayoutPanel.SuspendLayout();
    this.SuspendLayout();
    panel.Controls.Add((Control) this.informationLinkButton);
    panel.Controls.Add((Control) this.labelStatus);
    panel.Controls.Add((Control) this.pictureBoxStatus);
    panel.Controls.Add((Control) this.progressBar);
    panel.Controls.Add((Control) this.buttonSend);
    componentResourceManager.ApplyResources((object) panel, "panelControl");
    panel.Name = "panelControl";
    componentResourceManager.ApplyResources((object) this.informationLinkButton, "informationLinkButton");
    this.informationLinkButton.FlatStyle = FlatStyle.Standard;
    this.informationLinkButton.ImageAlign = ContentAlignment.MiddleCenter;
    this.informationLinkButton.ImageSource = (ContextLinkButton.ButtonImageSource) 1;
    ((Control) this.informationLinkButton).Name = "informationLinkButton";
    this.informationLinkButton.ShowImage = true;
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.AutoEllipsis = true;
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseMnemonic = false;
    componentResourceManager.ApplyResources((object) this.pictureBoxStatus, "pictureBoxStatus");
    this.pictureBoxStatus.Name = "pictureBoxStatus";
    this.pictureBoxStatus.TabStop = false;
    componentResourceManager.ApplyResources((object) this.progressBar, "progressBar");
    this.progressBar.Name = "progressBar";
    componentResourceManager.ApplyResources((object) this.buttonSend, "buttonSend");
    this.buttonSend.Name = "buttonSend";
    this.buttonSend.UseVisualStyleBackColor = true;
    this.buttonSend.Click += new EventHandler(this.OnSendClick);
    componentResourceManager.ApplyResources((object) this.panelHost, "panelHost");
    ((Control) this.panelHost).Name = "panelHost";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel, "tableLayoutPanel");
    this.tableLayoutPanel.Controls.Add((Control) panel, 0, 1);
    this.tableLayoutPanel.Controls.Add((Control) this.panelHost, 0, 0);
    this.tableLayoutPanel.Name = "tableLayoutPanel";
    this.toolTipStatus.AutoPopDelay = 0;
    this.toolTipStatus.InitialDelay = 500;
    this.toolTipStatus.ReshowDelay = 100;
    this.toolTipStatus.ShowAlways = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.tableLayoutPanel);
    this.Name = nameof (ParameterView);
    panel.ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxStatus).EndInit();
    this.tableLayoutPanel.ResumeLayout(false);
    this.ResumeLayout(false);
  }

  private class Error
  {
    private List<string> messages = new List<string>();
    private bool fatal;

    public bool Fatal => this.fatal;

    public bool HasMessages => this.messages.Count > 0;

    public string DisplayMessage
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string message in this.messages)
          stringBuilder.AppendLine(message);
        return stringBuilder.ToString();
      }
    }

    public Error()
    {
    }

    public Error(string message)
    {
      this.messages.Add(message);
      this.fatal = true;
    }

    public void AddMessage(string message, bool fatal)
    {
      this.messages.Add(message);
      this.fatal |= fatal;
    }

    public void ImportErrors(ParameterView.Error error)
    {
      this.messages.AddRange((IEnumerable<string>) error.messages);
      this.fatal |= error.Fatal;
    }
  }

  [Flags]
  private enum ErrorStatus
  {
    noError = 0,
    warning = 1,
    error = 2,
  }
}
