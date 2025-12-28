// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.DataHub.ExtractionManager
// Assembly: DataHub, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 89346980-C6E7-48B1-88DD-CF29796E810E
// Assembly location: C:\Users\petra\Downloads\Архив (2)\DataHub.dll

using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.DataHub.Properties;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Settings;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.DataHub;

public class ExtractionManager
{
  private static ExtractionManager globalInstance;
  private Channel channel;
  private XtrFile currentXtrFile;
  private DataPageRequest currentRequest;
  private List<DataPageType> supportedPages;
  private readonly Queue<DataPageType> pagesToClear;
  private int extractedPageCount;
  private bool performExtraction;
  private bool performSetPassword;
  private bool performClearPassword;
  private bool performClear;
  private bool ecuUnlocked;
  private string newDataPagesPassword;
  private bool? autoExtractDataPagesAtConnection;
  private ChannelBaseCollection logFileAllChannels;

  public static ExtractionManager GlobalInstance
  {
    get
    {
      return ExtractionManager.globalInstance ?? (ExtractionManager.globalInstance = new ExtractionManager());
    }
  }

  public bool PasswordRequired { get; private set; }

  public bool DataPagesEnabled { get; private set; }

  public bool SupportDetailsRead { get; private set; }

  public event EventHandler<ExtractionProgressEventArgs> ExtractionProgressEvent;

  public event EventHandler<ExtractionCompleteEventArgs> ExtractionCompleteEvent;

  public event EventHandler<ResultEventArgs> ReadSupportDetailsCompleteEvent;

  public event EventHandler<ResultEventArgs> UnlockDataPagesCompleteEvent;

  public event EventHandler<ResultEventArgs> ClearDataPagesCompleteEvent;

  public event EventHandler<ChangeDataPagePasswordRequestEventArgs> SetDataPagesPasswordCompleteEvent;

  public event EventHandler<ResultEventArgs> ClearDataPagesPasswordCompleteEvent;

  public static bool Busy { get; private set; }

  private ExtractionManager()
  {
    this.supportedPages = new List<DataPageType>();
    this.pagesToClear = new Queue<DataPageType>();
    this.SupportDetailsRead = false;
  }

  public void Init()
  {
    this.SupportDetailsRead = false;
    this.performClear = false;
    this.performExtraction = false;
    this.performSetPassword = false;
    this.performClearPassword = false;
    this.supportedPages = new List<DataPageType>();
    this.channel = (Channel) null;
    this.currentRequest = (DataPageRequest) null;
    this.currentXtrFile = (XtrFile) null;
    this.extractedPageCount = 0;
    this.newDataPagesPassword = string.Empty;
    SapiManager.GlobalInstance.LogFileChannelsChanged += new EventHandler(this.SapiManager_LogFileChannelsChanged);
    SapiManager.GlobalInstance.ChannelInitializingEvent += new EventHandler<ChannelInitializingEventArgs>(this.SapiManager_ChannelInitializingEvent);
  }

  public bool AutoExtractDataPagesAtConnection
  {
    get
    {
      if (!this.autoExtractDataPagesAtConnection.HasValue)
        this.autoExtractDataPagesAtConnection = new bool?(SettingsManager.GlobalInstance.GetValue<bool>(nameof (AutoExtractDataPagesAtConnection), "FleetInformation", ApplicationInformation.Branding.AutoExtractDataPagesAtConnectionDefault));
      return this.autoExtractDataPagesAtConnection.Value;
    }
    set
    {
      this.autoExtractDataPagesAtConnection = new bool?(value);
      SettingsManager.GlobalInstance.SetValue<bool>(nameof (AutoExtractDataPagesAtConnection), "FleetInformation", this.autoExtractDataPagesAtConnection.Value, false);
    }
  }

  private void ExtractionManager_ExtractionCompleteEvent(
    object sender,
    ExtractionCompleteEventArgs extractionCompleteEventArgs)
  {
    this.ExtractionCompleteEvent -= new EventHandler<ExtractionCompleteEventArgs>(this.ExtractionManager_ExtractionCompleteEvent);
    if (!extractionCompleteEventArgs.Succeeded || extractionCompleteEventArgs.XtrFile == null)
      return;
    this.SaveXtrFiles(extractionCompleteEventArgs.XtrFile);
  }

  private void SaveXtrFiles(XtrFile xtrFile)
  {
    string xtrFileName = ExtractionManager.CreateXtrFileName(this.channel, DateTime.Now);
    xtrFile.Save(Path.Combine(Directories.DrumrollUploadData, FileEncryptionProvider.EncryptFileName(xtrFileName)), true);
    xtrFile.Save(Path.Combine(Directories.DataPageExtractions, xtrFileName), false);
  }

  private static string CreateXtrFileName(Channel channel, DateTime timestamp)
  {
    return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}_{1}_{2}_{3}.xtr", (object) Sapi.TimeToString(timestamp), (object) SapiManager.RemoveInvalidChars(SapiManager.GetEngineSerialNumber(channel), "Unknown"), (object) SapiManager.RemoveInvalidChars(SapiManager.GetVehicleIdentificationNumber(channel), "Unknown"), (object) channel.Ecu.Name);
  }

  private void SapiManager_LogFileChannelsChanged(object sender, EventArgs e)
  {
    if (this.logFileAllChannels != null)
      this.logFileAllChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(this.logFileAllChannels_DisconnectCompleteEvent);
    this.logFileAllChannels = SapiManager.GlobalInstance.LogFileAllChannels;
    if (this.logFileAllChannels == null)
      return;
    this.logFileAllChannels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(this.logFileAllChannels_DisconnectCompleteEvent);
    foreach (Channel logFileAllChannel in this.logFileAllChannels)
    {
      if (SapiManager.SupportsDdecDataPages(logFileAllChannel))
        ExtractionManager.RegisterCommunicationStatusOverrides(logFileAllChannel);
    }
  }

  private void logFileAllChannels_DisconnectCompleteEvent(object sender, EventArgs e)
  {
    Channel channel = sender as Channel;
    if (!SapiManager.SupportsDdecDataPages(channel))
      return;
    ByteMessageCommunicationStatusOverride.Unregister(channel);
  }

  private void SapiManager_ChannelInitializingEvent(object sender, ChannelInitializingEventArgs e)
  {
    if (!this.AutoExtractDataPagesAtConnection || !SapiManager.SupportsDdecDataPages(e.Channel) || e.Channel.DiagnosisVariant.IsBase || e.Channel.DiagnosisVariant.IsBoot)
      return;
    e.Channel.InitCompleteEvent += new InitCompleteEventHandler(this.channel_InitCompleteEvent);
    e.Channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.Channel_CommunicationsStateUpdateEvent);
  }

  private void channel_InitCompleteEvent(object sender, EventArgs e)
  {
    if (!(sender is Channel channel) || ServerDataManager.GlobalInstance.Programming || SapiManager.GlobalInstance.Troubleshooting)
      return;
    channel.InitCompleteEvent -= new InitCompleteEventHandler(this.channel_InitCompleteEvent);
    if (!this.AutoExtractDataPagesAtConnection)
      return;
    this.performExtraction = true;
    this.SetChannel(channel);
  }

  private void Channel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    if (sender != this.channel || e.CommunicationsState != CommunicationsState.Disconnecting)
      return;
    ByteMessageCommunicationStatusOverride.Unregister(this.channel);
    this.channel = (Channel) null;
  }

  public void SetChannel(Channel channel)
  {
    this.supportedPages.Clear();
    this.pagesToClear.Clear();
    this.SupportDetailsRead = false;
    this.ecuUnlocked = false;
    this.channel = channel;
    ExtractionManager.RegisterCommunicationStatusOverrides(channel);
    this.GetDataPageSupportDetails();
  }

  private static void RegisterCommunicationStatusOverrides(Channel channel)
  {
    foreach (DataPageType type in Enum.GetValues(typeof (DataPageType)))
      ByteMessageCommunicationStatusOverride.Register(channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1:X2}", (object) "223E", (object) (int) type), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Reading {0}", (object) DataPage.PageTypeDescription(type)));
    ByteMessageCommunicationStatusOverride.Register(channel, "31013E", Resource.DataPageRequest_Clear);
    ByteMessageCommunicationStatusOverride.Register(channel, "31013E00", Resource.DataPageRequest_Password);
    ByteMessageCommunicationStatusOverride.Register(channel, "2E3E060D0006100000000000", Resource.DataPageRequest_ChangePassword);
  }

  private void GetDataPageSupportDetails()
  {
    ExtractionManager.Busy = true;
    DataPageRequest dataPageRequest = new DataPageRequest(this.channel);
    dataPageRequest.DataPageRequestCompleteEvent += new EventHandler<DataPageRequestEventArgs>(this.pageZeroPageRequest_DataPageRequestCompleteEvent);
    dataPageRequest.RequestDataPageRead(DataPageType.SupportRequestPage0);
  }

  private void pageZeroPageRequest_DataPageRequestCompleteEvent(
    object sender,
    DataPageRequestEventArgs e)
  {
    if (sender is DataPageRequest dataPageRequest)
      dataPageRequest.DataPageRequestCompleteEvent -= new EventHandler<DataPageRequestEventArgs>(this.pageZeroPageRequest_DataPageRequestCompleteEvent);
    if (e.Succeeded && e.RequestType == DataPageRequestType.Page0)
    {
      DataPage page = e.Page;
      if (page.DataLength >= 5)
      {
        switch (page.GetByteValue(2))
        {
          case 0:
            this.PasswordRequired = false;
            this.DataPagesEnabled = false;
            this.SupportDetailsRead = true;
            break;
          case 1:
            this.PasswordRequired = false;
            this.DataPagesEnabled = true;
            break;
          case 17:
            this.PasswordRequired = true;
            this.DataPagesEnabled = true;
            break;
          default:
            this.DataPagesEnabled = false;
            this.SupportDetailsRead = false;
            break;
        }
        if (this.DataPagesEnabled && page.DataLength >= 10 && !this.SupportDetailsRead)
        {
          int byteValue = (int) page.GetByteValue(8);
          for (int offset = 9; offset < 9 + byteValue; ++offset)
            this.supportedPages.Add((DataPageType) page.GetByteValue(offset));
          this.SupportDetailsRead = true;
        }
      }
      else
        StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DataPage Response invalid, could not read support details."), (StatusMessageType) 2, (object) this), true);
    }
    if (this.ReadSupportDetailsCompleteEvent != null)
    {
      if (this.SupportDetailsRead)
        this.ReadSupportDetailsCompleteEvent((object) this, new ResultEventArgs((Exception) null));
      else
        this.ReadSupportDetailsCompleteEvent((object) this, new ResultEventArgs((Exception) new InvalidDataException("Response data invalid")));
    }
    if (this.performExtraction && this.SupportDetailsRead && this.DataPagesEnabled)
      this.DoExtraction(string.Empty);
    else
      ExtractionManager.Busy = false;
  }

  public void DoExtraction(string password)
  {
    ExtractionManager.Busy = true;
    if (this.SupportDetailsRead && this.DataPagesEnabled)
    {
      this.performExtraction = true;
      if (this.PasswordRequired && !this.ecuUnlocked)
      {
        this.UnlockDataPages(password);
        return;
      }
      this.extractedPageCount = 0;
      this.currentXtrFile = new XtrFile(this.channel.Ecu.Name, SapiManager.GetSoftwareVersion(this.channel));
      this.ExtractionCompleteEvent += new EventHandler<ExtractionCompleteEventArgs>(this.ExtractionManager_ExtractionCompleteEvent);
      this.GetNextPage();
    }
    ExtractionManager.Busy = false;
  }

  private void GetNextPage()
  {
    if (this.extractedPageCount < this.supportedPages.Count)
    {
      if (this.channel != null)
      {
        this.currentRequest = new DataPageRequest(this.channel);
        this.currentRequest.DataPageRequestCompleteEvent += new EventHandler<DataPageRequestEventArgs>(this.currentRequest_DataPageRequestCompleteEvent);
        this.currentRequest.RequestDataPageRead(this.supportedPages[this.extractedPageCount]);
      }
      else
        this.Complete(false);
    }
    else
      this.Complete(true);
  }

  private void currentRequest_DataPageRequestCompleteEvent(
    object sender,
    DataPageRequestEventArgs dataPageRequestEventArgs)
  {
    this.currentRequest.DataPageRequestCompleteEvent -= new EventHandler<DataPageRequestEventArgs>(this.currentRequest_DataPageRequestCompleteEvent);
    ++this.extractedPageCount;
    if (this.currentXtrFile != null && dataPageRequestEventArgs.Succeeded && dataPageRequestEventArgs.Page != null && dataPageRequestEventArgs.Page.Valid)
    {
      this.currentXtrFile.AddDataPage(dataPageRequestEventArgs.Page);
      this.UpdateProgress((double) this.extractedPageCount / (double) this.supportedPages.Count * 100.0);
      this.GetNextPage();
    }
    else
    {
      ExtractionManager.Busy = false;
      this.Complete(false);
    }
  }

  public void SetDataPagesPassword()
  {
    this.performSetPassword = true;
    if (this.SupportDetailsRead)
    {
      ExtractionManager.Busy = true;
      PasswordFleetDataPagesChangePasswordDialog changePasswordDialog = new PasswordFleetDataPagesChangePasswordDialog(this.PasswordRequired && !this.ecuUnlocked);
      if (changePasswordDialog.ShowDialog() == DialogResult.OK)
      {
        this.newDataPagesPassword = changePasswordDialog.NewPassword;
        if (changePasswordDialog.OldPassword.Length > 0)
          this.UnlockDataPages(changePasswordDialog.OldPassword);
        else
          this.DoPasswordSet();
      }
      else
      {
        this.performSetPassword = false;
        ExtractionManager.Busy = false;
        this.SetDataPagesPasswordCompleteEvent((object) this, new ChangeDataPagePasswordRequestEventArgs(ChangePasswordResult.Cancel));
      }
    }
    else
      this.Complete(false);
  }

  public void ClearDataPagesPassword()
  {
    ExtractionManager.Busy = true;
    this.performClearPassword = true;
    if (this.SupportDetailsRead && this.PasswordRequired && !this.ecuUnlocked)
      this.UnlockDataPages(string.Empty);
    else
      this.DoPasswordClear();
  }

  public void DoPasswordSet()
  {
    DataPageRequest dataPageRequest = new DataPageRequest(this.channel);
    dataPageRequest.DataPageRequestCompleteEvent += new EventHandler<DataPageRequestEventArgs>(this.setDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent);
    dataPageRequest.RequestPasswordChange(this.newDataPagesPassword);
  }

  public void DoPasswordClear()
  {
    DataPageRequest dataPageRequest = new DataPageRequest(this.channel);
    dataPageRequest.DataPageRequestCompleteEvent += new EventHandler<DataPageRequestEventArgs>(this.clearDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent);
    dataPageRequest.RequestPasswordChange("");
  }

  public void ResetTripData()
  {
    this.performClear = true;
    this.pagesToClear.Enqueue(DataPageType.Trip);
    this.pagesToClear.Enqueue(DataPageType.TripTables);
    this.ResetNextQueuedDataPage();
  }

  public void ResetAllData()
  {
    this.pagesToClear.Enqueue(DataPageType.Trip);
    this.pagesToClear.Enqueue(DataPageType.TripTables);
    this.pagesToClear.Enqueue(DataPageType.TripDataMonthly);
    this.pagesToClear.Enqueue(DataPageType.Profile);
    this.ResetNextQueuedDataPage();
  }

  private void ResetNextQueuedDataPage()
  {
    ExtractionManager.Busy = true;
    if (this.pagesToClear != null && this.pagesToClear.Count > 0)
    {
      this.performClear = true;
      if (this.SupportDetailsRead && this.PasswordRequired && !this.ecuUnlocked)
      {
        this.UnlockDataPages(string.Empty);
      }
      else
      {
        DataPageType pageType = this.pagesToClear.Dequeue();
        if (this.supportedPages.Contains(pageType))
        {
          DataPageRequest dataPageRequest = new DataPageRequest(this.channel);
          dataPageRequest.DataPageRequestCompleteEvent += new EventHandler<DataPageRequestEventArgs>(this.resetDataPageRequest_RequestDataPageClearCompleteEvent);
          dataPageRequest.RequestDataPageClear(pageType);
        }
        else
        {
          if (this.pagesToClear.Count <= 0)
            return;
          this.ResetNextQueuedDataPage();
        }
      }
    }
    else
      this.Complete(true);
  }

  private void setDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent(
    object sender,
    DataPageRequestEventArgs e)
  {
    if (sender is DataPageRequest dataPageRequest)
      dataPageRequest.DataPageRequestCompleteEvent -= new EventHandler<DataPageRequestEventArgs>(this.setDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent);
    if (e.Succeeded)
    {
      this.PasswordRequired = true;
      this.ecuUnlocked = false;
      this.newDataPagesPassword = string.Empty;
    }
    this.Complete(e.Succeeded);
  }

  private void clearDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent(
    object sender,
    DataPageRequestEventArgs e)
  {
    if (sender is DataPageRequest dataPageRequest)
      dataPageRequest.DataPageRequestCompleteEvent -= new EventHandler<DataPageRequestEventArgs>(this.clearDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent);
    if (e.Succeeded)
    {
      this.PasswordRequired = false;
      this.ecuUnlocked = false;
      this.newDataPagesPassword = string.Empty;
    }
    this.Complete(e.Succeeded);
  }

  private void resetDataPageRequest_RequestDataPageClearCompleteEvent(
    object sender,
    DataPageRequestEventArgs e)
  {
    if (sender is DataPageRequest dataPageRequest)
      dataPageRequest.DataPageRequestCompleteEvent -= new EventHandler<DataPageRequestEventArgs>(this.resetDataPageRequest_RequestDataPageClearCompleteEvent);
    this.ResetNextQueuedDataPage();
  }

  private void UnlockDataPages(string password)
  {
    if (this.channel == null || !this.SupportDetailsRead)
      return;
    if (string.IsNullOrEmpty(password))
    {
      PasswordFleetDataPagesDialog fleetDataPagesDialog = new PasswordFleetDataPagesDialog();
      if (fleetDataPagesDialog.ShowDialog() != DialogResult.OK)
      {
        this.ecuUnlocked = false;
        ExtractionManager.Busy = false;
        this.Complete(false);
        return;
      }
      password = fleetDataPagesDialog.Password;
    }
    DataPageRequest dataPageRequest = new DataPageRequest(this.channel);
    dataPageRequest.DataPageRequestCompleteEvent += new EventHandler<DataPageRequestEventArgs>(this.unlockDataPageRequest_DataPageRequestCompleteEvent);
    dataPageRequest.RequestDataPageUnlock(password);
  }

  private void unlockDataPageRequest_DataPageRequestCompleteEvent(
    object sender,
    DataPageRequestEventArgs e)
  {
    if (sender is DataPageRequest dataPageRequest)
      dataPageRequest.DataPageRequestCompleteEvent -= new EventHandler<DataPageRequestEventArgs>(this.unlockDataPageRequest_DataPageRequestCompleteEvent);
    this.ecuUnlocked = e.Succeeded;
    if (this.UnlockDataPagesCompleteEvent != null)
    {
      if (e.Succeeded)
        this.UnlockDataPagesCompleteEvent((object) this, new ResultEventArgs((Exception) null));
      else
        this.UnlockDataPagesCompleteEvent((object) this, new ResultEventArgs((Exception) new SecurityException("Failed to unlock")));
    }
    if (this.performExtraction)
      this.DoExtraction(string.Empty);
    else if (this.performSetPassword)
      this.DoPasswordSet();
    else if (this.performClearPassword)
      this.DoPasswordClear();
    else if (this.performClear)
    {
      this.ResetNextQueuedDataPage();
    }
    else
    {
      if (e.Succeeded)
        return;
      ExtractionManager.Busy = false;
    }
  }

  private void UpdateProgress(double percentComplete)
  {
    if (this.ExtractionProgressEvent == null)
      return;
    this.ExtractionProgressEvent((object) this, new ExtractionProgressEventArgs(this.channel, percentComplete));
  }

  private void Complete(bool success)
  {
    ExtractionManager.Busy = false;
    if (this.performExtraction)
    {
      if (!success)
        this.currentXtrFile = (XtrFile) null;
      this.performExtraction = false;
      if (this.ExtractionCompleteEvent != null)
        this.ExtractionCompleteEvent((object) this, new ExtractionCompleteEventArgs(success, this.channel, this.currentXtrFile));
    }
    if (this.performSetPassword)
    {
      this.performSetPassword = false;
      if (this.SetDataPagesPasswordCompleteEvent != null)
      {
        if (success)
          this.SetDataPagesPasswordCompleteEvent((object) this, new ChangeDataPagePasswordRequestEventArgs(ChangePasswordResult.Success));
        else
          this.SetDataPagesPasswordCompleteEvent((object) this, new ChangeDataPagePasswordRequestEventArgs(ChangePasswordResult.Fail));
      }
    }
    if (this.performClearPassword)
    {
      this.performClearPassword = false;
      if (this.ClearDataPagesPasswordCompleteEvent != null)
      {
        if (success)
          this.ClearDataPagesPasswordCompleteEvent((object) this, new ResultEventArgs((Exception) null));
        else
          this.ClearDataPagesPasswordCompleteEvent((object) this, new ResultEventArgs((Exception) new OperationCanceledException("Failed to clear DataPages password")));
      }
    }
    if (!this.performClear)
      return;
    this.performClear = false;
    if (this.ClearDataPagesCompleteEvent == null)
      return;
    if (success)
      this.ClearDataPagesCompleteEvent((object) this, new ResultEventArgs((Exception) null));
    else
      this.ClearDataPagesCompleteEvent((object) this, new ResultEventArgs((Exception) new OperationCanceledException("Failed to clear requested pages")));
  }
}
