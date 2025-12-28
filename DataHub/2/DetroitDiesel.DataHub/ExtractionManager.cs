using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Common.Status;
using DetroitDiesel.DataHub.Properties;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Settings;
using SapiLayer1;

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

	public static ExtractionManager GlobalInstance => globalInstance ?? (globalInstance = new ExtractionManager());

	public bool PasswordRequired { get; private set; }

	public bool DataPagesEnabled { get; private set; }

	public bool SupportDetailsRead { get; private set; }

	public static bool Busy { get; private set; }

	public bool AutoExtractDataPagesAtConnection
	{
		get
		{
			if (!autoExtractDataPagesAtConnection.HasValue)
			{
				autoExtractDataPagesAtConnection = SettingsManager.GlobalInstance.GetValue<bool>("AutoExtractDataPagesAtConnection", "FleetInformation", ApplicationInformation.Branding.AutoExtractDataPagesAtConnectionDefault);
			}
			return autoExtractDataPagesAtConnection.Value;
		}
		set
		{
			autoExtractDataPagesAtConnection = value;
			SettingsManager.GlobalInstance.SetValue<bool>("AutoExtractDataPagesAtConnection", "FleetInformation", autoExtractDataPagesAtConnection.Value, false);
		}
	}

	public event EventHandler<ExtractionProgressEventArgs> ExtractionProgressEvent;

	public event EventHandler<ExtractionCompleteEventArgs> ExtractionCompleteEvent;

	public event EventHandler<ResultEventArgs> ReadSupportDetailsCompleteEvent;

	public event EventHandler<ResultEventArgs> UnlockDataPagesCompleteEvent;

	public event EventHandler<ResultEventArgs> ClearDataPagesCompleteEvent;

	public event EventHandler<ChangeDataPagePasswordRequestEventArgs> SetDataPagesPasswordCompleteEvent;

	public event EventHandler<ResultEventArgs> ClearDataPagesPasswordCompleteEvent;

	private ExtractionManager()
	{
		supportedPages = new List<DataPageType>();
		pagesToClear = new Queue<DataPageType>();
		SupportDetailsRead = false;
	}

	public void Init()
	{
		SupportDetailsRead = false;
		performClear = false;
		performExtraction = false;
		performSetPassword = false;
		performClearPassword = false;
		supportedPages = new List<DataPageType>();
		channel = null;
		currentRequest = null;
		currentXtrFile = null;
		extractedPageCount = 0;
		newDataPagesPassword = string.Empty;
		SapiManager.GlobalInstance.LogFileChannelsChanged += SapiManager_LogFileChannelsChanged;
		SapiManager.GlobalInstance.ChannelInitializingEvent += SapiManager_ChannelInitializingEvent;
	}

	private void ExtractionManager_ExtractionCompleteEvent(object sender, ExtractionCompleteEventArgs extractionCompleteEventArgs)
	{
		ExtractionCompleteEvent -= ExtractionManager_ExtractionCompleteEvent;
		if (extractionCompleteEventArgs.Succeeded && extractionCompleteEventArgs.XtrFile != null)
		{
			SaveXtrFiles(extractionCompleteEventArgs.XtrFile);
		}
	}

	private void SaveXtrFiles(XtrFile xtrFile)
	{
		string text = CreateXtrFileName(channel, DateTime.Now);
		xtrFile.Save(Path.Combine(Directories.DrumrollUploadData, FileEncryptionProvider.EncryptFileName(text)), encrypt: true);
		xtrFile.Save(Path.Combine(Directories.DataPageExtractions, text), encrypt: false);
	}

	private static string CreateXtrFileName(Channel channel, DateTime timestamp)
	{
		string text = Sapi.TimeToString(timestamp);
		string text2 = SapiManager.RemoveInvalidChars(SapiManager.GetEngineSerialNumber(channel), "Unknown");
		string text3 = SapiManager.RemoveInvalidChars(SapiManager.GetVehicleIdentificationNumber(channel), "Unknown");
		return string.Format(CultureInfo.CurrentCulture, "{0}_{1}_{2}_{3}.xtr", text, text2, text3, channel.Ecu.Name);
	}

	private void SapiManager_LogFileChannelsChanged(object sender, EventArgs e)
	{
		if (logFileAllChannels != null)
		{
			logFileAllChannels.DisconnectCompleteEvent -= logFileAllChannels_DisconnectCompleteEvent;
		}
		logFileAllChannels = SapiManager.GlobalInstance.LogFileAllChannels;
		if (logFileAllChannels == null)
		{
			return;
		}
		logFileAllChannels.DisconnectCompleteEvent += logFileAllChannels_DisconnectCompleteEvent;
		foreach (Channel logFileAllChannel in logFileAllChannels)
		{
			if (SapiManager.SupportsDdecDataPages(logFileAllChannel))
			{
				RegisterCommunicationStatusOverrides(logFileAllChannel);
			}
		}
	}

	private void logFileAllChannels_DisconnectCompleteEvent(object sender, EventArgs e)
	{
		Channel channel = sender as Channel;
		if (SapiManager.SupportsDdecDataPages(channel))
		{
			ByteMessageCommunicationStatusOverride.Unregister(channel);
		}
	}

	private void SapiManager_ChannelInitializingEvent(object sender, ChannelInitializingEventArgs e)
	{
		if (AutoExtractDataPagesAtConnection && SapiManager.SupportsDdecDataPages(e.Channel) && !e.Channel.DiagnosisVariant.IsBase && !e.Channel.DiagnosisVariant.IsBoot)
		{
			e.Channel.InitCompleteEvent += channel_InitCompleteEvent;
			e.Channel.CommunicationsStateUpdateEvent += Channel_CommunicationsStateUpdateEvent;
		}
	}

	private void channel_InitCompleteEvent(object sender, EventArgs e)
	{
		if (sender is Channel channel && !ServerDataManager.GlobalInstance.Programming && !SapiManager.GlobalInstance.Troubleshooting)
		{
			channel.InitCompleteEvent -= channel_InitCompleteEvent;
			if (AutoExtractDataPagesAtConnection)
			{
				performExtraction = true;
				SetChannel(channel);
			}
		}
	}

	private void Channel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		if (sender == channel && e.CommunicationsState == CommunicationsState.Disconnecting)
		{
			ByteMessageCommunicationStatusOverride.Unregister(channel);
			channel = null;
		}
	}

	public void SetChannel(Channel channel)
	{
		supportedPages.Clear();
		pagesToClear.Clear();
		SupportDetailsRead = false;
		ecuUnlocked = false;
		this.channel = channel;
		RegisterCommunicationStatusOverrides(channel);
		GetDataPageSupportDetails();
	}

	private static void RegisterCommunicationStatusOverrides(Channel channel)
	{
		foreach (DataPageType value in Enum.GetValues(typeof(DataPageType)))
		{
			ByteMessageCommunicationStatusOverride.Register(channel, string.Format(CultureInfo.InvariantCulture, "{0}{1:X2}", "223E", (int)value), string.Format(CultureInfo.InvariantCulture, "Reading {0}", DataPage.PageTypeDescription(value)));
		}
		ByteMessageCommunicationStatusOverride.Register(channel, "31013E", Resource.DataPageRequest_Clear);
		ByteMessageCommunicationStatusOverride.Register(channel, "31013E00", Resource.DataPageRequest_Password);
		ByteMessageCommunicationStatusOverride.Register(channel, "2E3E060D0006100000000000", Resource.DataPageRequest_ChangePassword);
	}

	private void GetDataPageSupportDetails()
	{
		Busy = true;
		DataPageRequest dataPageRequest = new DataPageRequest(channel);
		dataPageRequest.DataPageRequestCompleteEvent += pageZeroPageRequest_DataPageRequestCompleteEvent;
		dataPageRequest.RequestDataPageRead(DataPageType.SupportRequestPage0);
	}

	private void pageZeroPageRequest_DataPageRequestCompleteEvent(object sender, DataPageRequestEventArgs e)
	{
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		if (sender is DataPageRequest dataPageRequest)
		{
			dataPageRequest.DataPageRequestCompleteEvent -= pageZeroPageRequest_DataPageRequestCompleteEvent;
		}
		if (e.Succeeded && e.RequestType == DataPageRequestType.Page0)
		{
			DataPage page = e.Page;
			if (page.DataLength >= 5)
			{
				switch (page.GetByteValue(2))
				{
				case 0:
					PasswordRequired = false;
					DataPagesEnabled = false;
					SupportDetailsRead = true;
					break;
				case 1:
					PasswordRequired = false;
					DataPagesEnabled = true;
					break;
				case 17:
					PasswordRequired = true;
					DataPagesEnabled = true;
					break;
				default:
					DataPagesEnabled = false;
					SupportDetailsRead = false;
					break;
				}
				if (DataPagesEnabled && page.DataLength >= 10 && !SupportDetailsRead)
				{
					int byteValue = page.GetByteValue(8);
					for (int i = 9; i < 9 + byteValue; i++)
					{
						supportedPages.Add((DataPageType)page.GetByteValue(i));
					}
					SupportDetailsRead = true;
				}
			}
			else
			{
				StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "DataPage Response invalid, could not read support details."), (StatusMessageType)2, (object)this), true);
			}
		}
		if (this.ReadSupportDetailsCompleteEvent != null)
		{
			if (SupportDetailsRead)
			{
				this.ReadSupportDetailsCompleteEvent(this, new ResultEventArgs(null));
			}
			else
			{
				this.ReadSupportDetailsCompleteEvent(this, new ResultEventArgs(new InvalidDataException("Response data invalid")));
			}
		}
		if (performExtraction && SupportDetailsRead && DataPagesEnabled)
		{
			DoExtraction(string.Empty);
		}
		else
		{
			Busy = false;
		}
	}

	public void DoExtraction(string password)
	{
		Busy = true;
		if (SupportDetailsRead && DataPagesEnabled)
		{
			performExtraction = true;
			if (PasswordRequired && !ecuUnlocked)
			{
				UnlockDataPages(password);
				return;
			}
			extractedPageCount = 0;
			currentXtrFile = new XtrFile(channel.Ecu.Name, SapiManager.GetSoftwareVersion(channel));
			ExtractionCompleteEvent += ExtractionManager_ExtractionCompleteEvent;
			GetNextPage();
		}
		Busy = false;
	}

	private void GetNextPage()
	{
		if (extractedPageCount < supportedPages.Count)
		{
			if (channel != null)
			{
				currentRequest = new DataPageRequest(channel);
				currentRequest.DataPageRequestCompleteEvent += currentRequest_DataPageRequestCompleteEvent;
				currentRequest.RequestDataPageRead(supportedPages[extractedPageCount]);
			}
			else
			{
				Complete(success: false);
			}
		}
		else
		{
			Complete(success: true);
		}
	}

	private void currentRequest_DataPageRequestCompleteEvent(object sender, DataPageRequestEventArgs dataPageRequestEventArgs)
	{
		currentRequest.DataPageRequestCompleteEvent -= currentRequest_DataPageRequestCompleteEvent;
		extractedPageCount++;
		if (currentXtrFile != null && dataPageRequestEventArgs.Succeeded && dataPageRequestEventArgs.Page != null && dataPageRequestEventArgs.Page.Valid)
		{
			currentXtrFile.AddDataPage(dataPageRequestEventArgs.Page);
			UpdateProgress((double)extractedPageCount / (double)supportedPages.Count * 100.0);
			GetNextPage();
		}
		else
		{
			Busy = false;
			Complete(success: false);
		}
	}

	public void SetDataPagesPassword()
	{
		performSetPassword = true;
		if (SupportDetailsRead)
		{
			Busy = true;
			PasswordFleetDataPagesChangePasswordDialog passwordFleetDataPagesChangePasswordDialog = new PasswordFleetDataPagesChangePasswordDialog(PasswordRequired && !ecuUnlocked);
			if (passwordFleetDataPagesChangePasswordDialog.ShowDialog() == DialogResult.OK)
			{
				newDataPagesPassword = passwordFleetDataPagesChangePasswordDialog.NewPassword;
				if (passwordFleetDataPagesChangePasswordDialog.OldPassword.Length > 0)
				{
					UnlockDataPages(passwordFleetDataPagesChangePasswordDialog.OldPassword);
				}
				else
				{
					DoPasswordSet();
				}
			}
			else
			{
				performSetPassword = false;
				Busy = false;
				this.SetDataPagesPasswordCompleteEvent(this, new ChangeDataPagePasswordRequestEventArgs(ChangePasswordResult.Cancel));
			}
		}
		else
		{
			Complete(success: false);
		}
	}

	public void ClearDataPagesPassword()
	{
		Busy = true;
		performClearPassword = true;
		if (SupportDetailsRead && PasswordRequired && !ecuUnlocked)
		{
			UnlockDataPages(string.Empty);
		}
		else
		{
			DoPasswordClear();
		}
	}

	public void DoPasswordSet()
	{
		DataPageRequest dataPageRequest = new DataPageRequest(channel);
		dataPageRequest.DataPageRequestCompleteEvent += setDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent;
		dataPageRequest.RequestPasswordChange(newDataPagesPassword);
	}

	public void DoPasswordClear()
	{
		DataPageRequest dataPageRequest = new DataPageRequest(channel);
		dataPageRequest.DataPageRequestCompleteEvent += clearDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent;
		dataPageRequest.RequestPasswordChange("");
	}

	public void ResetTripData()
	{
		performClear = true;
		pagesToClear.Enqueue(DataPageType.Trip);
		pagesToClear.Enqueue(DataPageType.TripTables);
		ResetNextQueuedDataPage();
	}

	public void ResetAllData()
	{
		pagesToClear.Enqueue(DataPageType.Trip);
		pagesToClear.Enqueue(DataPageType.TripTables);
		pagesToClear.Enqueue(DataPageType.TripDataMonthly);
		pagesToClear.Enqueue(DataPageType.Profile);
		ResetNextQueuedDataPage();
	}

	private void ResetNextQueuedDataPage()
	{
		Busy = true;
		if (pagesToClear != null && pagesToClear.Count > 0)
		{
			performClear = true;
			if (SupportDetailsRead && PasswordRequired && !ecuUnlocked)
			{
				UnlockDataPages(string.Empty);
				return;
			}
			DataPageType dataPageType = pagesToClear.Dequeue();
			if (supportedPages.Contains(dataPageType))
			{
				DataPageRequest dataPageRequest = new DataPageRequest(channel);
				dataPageRequest.DataPageRequestCompleteEvent += resetDataPageRequest_RequestDataPageClearCompleteEvent;
				dataPageRequest.RequestDataPageClear(dataPageType);
			}
			else if (pagesToClear.Count > 0)
			{
				ResetNextQueuedDataPage();
			}
		}
		else
		{
			Complete(success: true);
		}
	}

	private void setDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent(object sender, DataPageRequestEventArgs e)
	{
		if (sender is DataPageRequest dataPageRequest)
		{
			dataPageRequest.DataPageRequestCompleteEvent -= setDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent;
		}
		if (e.Succeeded)
		{
			PasswordRequired = true;
			ecuUnlocked = false;
			newDataPagesPassword = string.Empty;
		}
		Complete(e.Succeeded);
	}

	private void clearDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent(object sender, DataPageRequestEventArgs e)
	{
		if (sender is DataPageRequest dataPageRequest)
		{
			dataPageRequest.DataPageRequestCompleteEvent -= clearDataPagePasswordRequest_RequestChangeDataPagePasswordCompleteEvent;
		}
		if (e.Succeeded)
		{
			PasswordRequired = false;
			ecuUnlocked = false;
			newDataPagesPassword = string.Empty;
		}
		Complete(e.Succeeded);
	}

	private void resetDataPageRequest_RequestDataPageClearCompleteEvent(object sender, DataPageRequestEventArgs e)
	{
		if (sender is DataPageRequest dataPageRequest)
		{
			dataPageRequest.DataPageRequestCompleteEvent -= resetDataPageRequest_RequestDataPageClearCompleteEvent;
		}
		ResetNextQueuedDataPage();
	}

	private void UnlockDataPages(string password)
	{
		if (channel == null || !SupportDetailsRead)
		{
			return;
		}
		if (string.IsNullOrEmpty(password))
		{
			PasswordFleetDataPagesDialog passwordFleetDataPagesDialog = new PasswordFleetDataPagesDialog();
			if (passwordFleetDataPagesDialog.ShowDialog() != DialogResult.OK)
			{
				ecuUnlocked = false;
				Busy = false;
				Complete(success: false);
				return;
			}
			password = passwordFleetDataPagesDialog.Password;
		}
		DataPageRequest dataPageRequest = new DataPageRequest(channel);
		dataPageRequest.DataPageRequestCompleteEvent += unlockDataPageRequest_DataPageRequestCompleteEvent;
		dataPageRequest.RequestDataPageUnlock(password);
	}

	private void unlockDataPageRequest_DataPageRequestCompleteEvent(object sender, DataPageRequestEventArgs e)
	{
		if (sender is DataPageRequest dataPageRequest)
		{
			dataPageRequest.DataPageRequestCompleteEvent -= unlockDataPageRequest_DataPageRequestCompleteEvent;
		}
		ecuUnlocked = e.Succeeded;
		if (this.UnlockDataPagesCompleteEvent != null)
		{
			if (e.Succeeded)
			{
				this.UnlockDataPagesCompleteEvent(this, new ResultEventArgs(null));
			}
			else
			{
				this.UnlockDataPagesCompleteEvent(this, new ResultEventArgs(new SecurityException("Failed to unlock")));
			}
		}
		if (performExtraction)
		{
			DoExtraction(string.Empty);
		}
		else if (performSetPassword)
		{
			DoPasswordSet();
		}
		else if (performClearPassword)
		{
			DoPasswordClear();
		}
		else if (performClear)
		{
			ResetNextQueuedDataPage();
		}
		else if (!e.Succeeded)
		{
			Busy = false;
		}
	}

	private void UpdateProgress(double percentComplete)
	{
		if (this.ExtractionProgressEvent != null)
		{
			this.ExtractionProgressEvent(this, new ExtractionProgressEventArgs(channel, percentComplete));
		}
	}

	private void Complete(bool success)
	{
		Busy = false;
		if (performExtraction)
		{
			if (!success)
			{
				currentXtrFile = null;
			}
			performExtraction = false;
			if (this.ExtractionCompleteEvent != null)
			{
				this.ExtractionCompleteEvent(this, new ExtractionCompleteEventArgs(success, channel, currentXtrFile));
			}
		}
		if (performSetPassword)
		{
			performSetPassword = false;
			if (this.SetDataPagesPasswordCompleteEvent != null)
			{
				if (success)
				{
					this.SetDataPagesPasswordCompleteEvent(this, new ChangeDataPagePasswordRequestEventArgs(ChangePasswordResult.Success));
				}
				else
				{
					this.SetDataPagesPasswordCompleteEvent(this, new ChangeDataPagePasswordRequestEventArgs(ChangePasswordResult.Fail));
				}
			}
		}
		if (performClearPassword)
		{
			performClearPassword = false;
			if (this.ClearDataPagesPasswordCompleteEvent != null)
			{
				if (success)
				{
					this.ClearDataPagesPasswordCompleteEvent(this, new ResultEventArgs(null));
				}
				else
				{
					this.ClearDataPagesPasswordCompleteEvent(this, new ResultEventArgs(new OperationCanceledException("Failed to clear DataPages password")));
				}
			}
		}
		if (!performClear)
		{
			return;
		}
		performClear = false;
		if (this.ClearDataPagesCompleteEvent != null)
		{
			if (success)
			{
				this.ClearDataPagesCompleteEvent(this, new ResultEventArgs(null));
			}
			else
			{
				this.ClearDataPagesCompleteEvent(this, new ResultEventArgs(new OperationCanceledException("Failed to clear requested pages")));
			}
		}
	}
}
