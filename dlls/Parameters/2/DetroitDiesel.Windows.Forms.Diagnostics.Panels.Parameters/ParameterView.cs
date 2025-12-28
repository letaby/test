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

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public sealed class ParameterView : UserControl, ISupportActivation, ISearchable, ISupportEdit, IProvideHtml, IContextHelp, IRefreshable, IFilterable, ISupportExpandCollapseAll
{
	private class Error
	{
		private List<string> messages = new List<string>();

		private bool fatal;

		public bool Fatal => fatal;

		public bool HasMessages => messages.Count > 0;

		public string DisplayMessage
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string message in messages)
				{
					stringBuilder.AppendLine(message);
				}
				return stringBuilder.ToString();
			}
		}

		public Error()
		{
		}

		public Error(string message)
		{
			messages.Add(message);
			fatal = true;
		}

		public void AddMessage(string message, bool fatal)
		{
			messages.Add(message);
			this.fatal |= fatal;
		}

		public void ImportErrors(Error error)
		{
			messages.AddRange(error.messages);
			fatal |= error.Fatal;
		}
	}

	[Flags]
	private enum ErrorStatus
	{
		noError = 0,
		warning = 1,
		error = 2
	}

	private ChannelBaseCollection activeChannels;

	private MenuProxy menuProxy = MenuProxy.GlobalInstance;

	private string importExportPath = Directories.Parameters;

	private Queue<Channel> channelsToWrite = new Queue<Channel>();

	private List<KeyValuePair<ErrorStatus, string>> errorMessages = new List<KeyValuePair<ErrorStatus, string>>();

	private bool shutdownRequested;

	private List<Precondition> preconditions;

	private int inModalDialog;

	private Queue<Channel> channelsToAutoSave = new Queue<Channel>();

	private readonly List<Channel> parametersBeingRead = new List<Channel>();

	private Dictionary<Channel, Error> readErrors = new Dictionary<Channel, Error>();

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

	public bool CanSearch => ((PanelBase)panelHost).CanSearch;

	public bool CanUndo => ((PanelBase)panelHost).CanUndo;

	public bool CanCopy => ((PanelBase)panelHost).CanCopy;

	public bool CanDelete => ((PanelBase)panelHost).CanDelete;

	public bool CanPaste => ((PanelBase)panelHost).CanPaste;

	public bool CanSelectAll => ((PanelBase)panelHost).CanSelectAll;

	public bool CanCut => ((PanelBase)panelHost).CanCut;

	public bool CanProvideHtml => ((PanelBase)panelHost).CanProvideHtml;

	public string StyleSheet => ((PanelBase)panelHost).StyleSheet;

	public bool Active => ((PanelBase)panelHost).Active;

	public Link ContextLink => helpChain.ContextLink;

	public Link AlternateContextLink => helpChain.AlternateContextLink;

	public bool CanRefreshView
	{
		get
		{
			if (activeChannels != null)
			{
				foreach (Channel activeChannel in activeChannels)
				{
					if (activeChannel.CommunicationsState == CommunicationsState.Online)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public IEnumerable<FilterTypes> Filters => ((PanelBase)panelHost).Filters;

	public int NumberOfItemsFiltered => ((PanelBase)panelHost).NumberOfItemsFiltered;

	public int TotalNumberOfFilterableItems => ((PanelBase)panelHost).TotalNumberOfFilterableItems;

	public bool CanExpandAllItems
	{
		get
		{
			if (panelHost == null)
			{
				return false;
			}
			return ((PanelBase)panelHost).CanExpandAllItems;
		}
	}

	public bool CanCollapseAllItems
	{
		get
		{
			if (panelHost == null)
			{
				return false;
			}
			return ((PanelBase)panelHost).CanCollapseAllItems;
		}
	}

	public event EventHandler ContextLinkChanged
	{
		add
		{
			helpChain.ContextLinkChanged += value;
		}
		remove
		{
			helpChain.ContextLinkChanged -= value;
		}
	}

	public event EventHandler FilteredContentChanged;

	public ParameterView()
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		menuProxy.ParameterView = this;
		InitializeComponent();
		preconditions = PreconditionManager.GlobalInstance.Preconditions.Where((Precondition p) => (int)p.PreconditionType == 1 || (int)p.PreconditionType == 3).ToList();
		UpdateParameterWritePreconditionMonitoring();
		SapiManager.GlobalInstance.ActiveChannelsChanged += OnActiveChannelsChanged;
		((PanelBase)panelHost).FilteredContentChanged += OnFilteredContentChanged;
		ServerDataManager.GlobalInstance.ProhibitedChannelsUpdated += ServerDataManager_ProhibitedChannelsUpdated;
		helpChain = new ContextHelpChain((object)panelHost, LinkSupport.GetViewLink((PanelIdentifier)6));
		informationLinkButton.Context = (IContextHelp)(object)helpChain;
		informationLinkButton.UseAlternateContext = true;
		Application.EnterThreadModal += Application_EnterThreadModal;
		Application.LeaveThreadModal += Application_LeaveThreadModal;
	}

	private void Application_EnterThreadModal(object sender, EventArgs e)
	{
		inModalDialog++;
	}

	private void Application_LeaveThreadModal(object sender, EventArgs e)
	{
		inModalDialog--;
		if (inModalDialog != 0 || channelsToAutoSave.Count <= 0 || base.IsDisposed || !base.IsHandleCreated)
		{
			return;
		}
		BeginInvoke((Action)delegate
		{
			while (channelsToAutoSave.Count > 0 && inModalDialog == 0)
			{
				AutoSaveForServerUpload(channelsToAutoSave.Dequeue());
			}
		});
	}

	private void ServerDataManager_ProhibitedChannelsUpdated(object sender, EventArgs e)
	{
		if (base.Visible)
		{
			UpdateProhibitWarning();
		}
	}

	private void Precondition_StateChanged(object sender, EventArgs e)
	{
		if (base.Visible)
		{
			UpdateStatus();
			UpdateProhibitWarningForPreconditions();
		}
	}

	private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
	{
		AddChannel(sender as Channel);
	}

	private void OnDisconnectCompleteEvent(object sender, EventArgs e)
	{
		RemoveChannel(sender as Channel);
	}

	private void AddChannel(Channel channel)
	{
		if (channel != null && SapiManager.GlobalInstance.Online)
		{
			channel.Parameters.ParametersReadCompleteEvent += ParametersReadCompleteEvent;
			channel.Parameters.ParameterUpdateEvent += ParameterUpdateEvent;
			channel.Parameters.ParametersWriteCompleteEvent += OnParameterCollectionWriteCompleteEvent;
			channel.CommunicationsStateUpdateEvent += OnChannelCommunicationsStateUpdateEvent;
			channel.InitCompleteEvent += OnChannelInitCompleteEvent;
			if (channel.Online && channel.CommunicationsState != CommunicationsState.OnlineButNotInitialized && channel.CommunicationsState != CommunicationsState.ReadEcuInfo)
			{
				InitializeComplete(channel);
			}
		}
	}

	private void OnChannelInitCompleteEvent(object sender, EventArgs e)
	{
		if (sender is Channel channel)
		{
			InitializeComplete(channel);
		}
	}

	private void InitializeComplete(Channel channel)
	{
		if (base.Visible && channel.Online && !parametersBeingRead.Contains(channel) && channel.CommunicationsState != CommunicationsState.ReadParameters && channel.Parameters.Any())
		{
			parametersBeingRead.Add(channel);
			channel.Parameters.Read(synchronous: false);
		}
	}

	private void RemoveChannel(Channel channel)
	{
		if (channel != null)
		{
			channel.Parameters.ParametersReadCompleteEvent -= ParametersReadCompleteEvent;
			channel.Parameters.ParametersWriteCompleteEvent -= OnParameterCollectionWriteCompleteEvent;
			channel.Parameters.ParameterUpdateEvent -= ParameterUpdateEvent;
			channel.CommunicationsStateUpdateEvent -= OnChannelCommunicationsStateUpdateEvent;
			channel.InitCompleteEvent -= OnChannelInitCompleteEvent;
			readErrors.Remove(channel);
			if (!base.IsDisposed && !shutdownRequested)
			{
				CheckForUnsentChanges(channel, ParameterType.Parameter);
				CheckForUnsentChanges(channel, ParameterType.Accumulator);
			}
			ClearStatus();
		}
	}

	private void UpdateActiveChannels()
	{
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent -= OnConnectCompleteEvent;
			activeChannels.DisconnectCompleteEvent -= OnDisconnectCompleteEvent;
		}
		activeChannels = SapiManager.GlobalInstance.ActiveChannels;
		if (activeChannels == null)
		{
			return;
		}
		activeChannels.ConnectCompleteEvent += OnConnectCompleteEvent;
		activeChannels.DisconnectCompleteEvent += OnDisconnectCompleteEvent;
		foreach (Channel activeChannel in activeChannels)
		{
			AddChannel(activeChannel);
		}
	}

	private void ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		UpdateStatus();
		ParameterCollection parameters = sender as ParameterCollection;
		Error readError = new Error();
		if (e.Succeeded)
		{
			List<Tuple<ParameterGroup, Exception>> list = new List<Tuple<ParameterGroup, Exception>>();
			foreach (ParameterGroup parameterGroup in parameters.Channel.ParameterGroups)
			{
				IEnumerable<IGrouping<Exception, Parameter>> source = from p in parameterGroup.Parameters
					group p by p.Exception;
				if (source.Count() == 1 && source.First().Key != null)
				{
					list.Add(Tuple.Create(parameterGroup, source.First().Key));
				}
			}
			bool fatal = list.Count > 0 && list.Count == parameters.Channel.ParameterGroups.Count;
			list.ForEach(delegate(Tuple<ParameterGroup, Exception> domainException)
			{
				readError.AddMessage(string.Concat(parameters.Channel, ".", domainException.Item1.Qualifier, ": ", domainException.Item2.Message), fatal);
			});
			if (!fatal)
			{
				ServerDataManager.GlobalInstance.AutoSaveSettings(parameters.Channel, (AutoSaveDestination)1, "ECUREAD");
				AutoSaveForServerUpload(parameters.Channel);
			}
		}
		else
		{
			readError.AddMessage(string.Concat(parameters.Channel, ": ", e.Exception.Message), fatal: true);
		}
		readErrors[parameters.Channel] = readError;
		Error allReadErrors = new Error();
		readErrors.Where((KeyValuePair<Channel, Error> entry) => entry.Key.Online && entry.Value.HasMessages).ToList().ForEach(delegate(KeyValuePair<Channel, Error> entry)
		{
			allReadErrors.ImportErrors(entry.Value);
		});
		EndRead(allReadErrors);
		if (parameters != null)
		{
			parametersBeingRead.Remove(parameters.Channel);
		}
	}

	private void AutoSaveForServerUpload(Channel channel)
	{
		if (inModalDialog > 0)
		{
			channelsToAutoSave.Enqueue(channel);
		}
		else
		{
			ServerDataManager.GlobalInstance.AutoSaveSettings(channel, (AutoSaveDestination)0, "ECUUPDATE");
		}
	}

	private void ParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		if (!e.Succeeded && sender is Parameter parameter)
		{
			StatusLog.Add(new StatusMessage(e.Exception.Message, (StatusMessageType)1, (object)(parameter.Channel.Ecu.Name + "." + parameter.GroupQualifier + "." + parameter.Qualifier)));
		}
		UpdateStatus();
	}

	private void OnSendClick(object sender, EventArgs e)
	{
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		Form form = new ConfirmSendForm();
		if (form.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		foreach (Channel activeChannel in activeChannels)
		{
			if (!activeChannel.Online)
			{
				continue;
			}
			bool flag = true;
			PasswordManager passwordManager = menuProxy.GetPasswordManager(activeChannel);
			if (passwordManager != null)
			{
				try
				{
					bool[] array = passwordManager.AcquireRelevantListStatus(progressBar);
					bool flag2 = false;
					bool[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (array2[i])
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						PasswordEntryDialog val = new PasswordEntryDialog(activeChannel, array, passwordManager);
						if (((Form)(object)val).ShowDialog() != DialogResult.OK)
						{
							flag = false;
							if (activeChannel.Online)
							{
								ReportWarning(Resources.ProvidePassword, string.Empty);
							}
						}
					}
				}
				catch (CaesarException ex)
				{
					if (ex.NegativeResponseCode != 17)
					{
						string text = ((ex.NegativeResponseCode != 127) ? string.Format(CultureInfo.CurrentCulture, Resources.MessageFormatGatherPasswordOtherError, ex.Message) : string.Format(CultureInfo.CurrentCulture, Resources.MessageFormatGatherPasswordVehicleMoving, activeChannel.Ecu.Name));
						ControlHelpers.ShowMessageBox((Control)this, text, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						flag = false;
					}
					else
					{
						StatusLog.Add(new StatusMessage("Password protection services were indicated as supported for " + activeChannel.Ecu.Name + " but the ECU reported NRC $11 'service not supported'. Continuing without password protection.", (StatusMessageType)2, (object)this));
					}
				}
			}
			if (flag && !ServerDataManager.GlobalInstance.ProhibitedChannels.Contains(activeChannel) && activeChannel.Parameters.Any((Parameter p) => ParameterChanged(p)))
			{
				channelsToWrite.Enqueue(activeChannel);
			}
		}
		if (!SendNextWrite())
		{
			UpdateStatus();
		}
	}

	private bool SendNextWrite()
	{
		if (channelsToWrite.Count > 0)
		{
			Channel channel = channelsToWrite.Dequeue();
			if (channel.Online)
			{
				channel.Parameters.Write(synchronous: false);
				UpdateStatus();
				return true;
			}
			return SendNextWrite();
		}
		return false;
	}

	private void OnParameterCollectionWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		UpdateStatus();
		ParameterCollection parameterCollection = sender as ParameterCollection;
		if (!e.Succeeded)
		{
			string message = string.Format(CultureInfo.CurrentCulture, Resources.ParameterView_Format_ErrorsOccurred_NoValidation, parameterCollection.Channel.Ecu.Name, e.Exception.Message);
			EndWrite(ErrorStatus.error, message);
			return;
		}
		ErrorStatus errorStatus = ErrorStatus.noError;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.ParameterView_Format_DeviceReportedTheFollowingErrors, parameterCollection.Channel.Ecu.Name);
		stringBuilder2.AppendFormat(CultureInfo.CurrentCulture, Resources.ParameterView_Format_DeviceReportedTheFollowingWarnings, parameterCollection.Channel.Ecu.Name);
		foreach (Parameter item in parameterCollection)
		{
			if (item.Marked && item.Exception != null)
			{
				if (item.Exception is CaesarException ex && ex.ErrorNumber == 6602)
				{
					errorStatus |= ErrorStatus.warning;
					stringBuilder2.AppendFormat(CultureInfo.CurrentCulture, Resources.ParameterView_Format_WarningOrError, item.Name, item.Exception.Message);
				}
				else
				{
					errorStatus |= ErrorStatus.error;
					stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.ParameterView_Format_WarningOrError, item.Name, item.Exception.Message);
				}
			}
		}
		StringBuilder stringBuilder3 = new StringBuilder(null);
		if ((errorStatus & ErrorStatus.error) != ErrorStatus.noError)
		{
			stringBuilder3.Append(stringBuilder.ToString());
		}
		if ((errorStatus & ErrorStatus.warning) != ErrorStatus.noError)
		{
			if ((errorStatus & ErrorStatus.error) != ErrorStatus.noError)
			{
				stringBuilder3.Append(Environment.NewLine);
				stringBuilder3.Append(Environment.NewLine);
			}
			stringBuilder3.Append(stringBuilder2.ToString());
		}
		EndWrite(errorStatus, stringBuilder3.ToString());
		ServerDataManager.GlobalInstance.AutoSaveSettings(parameterCollection.Channel, (AutoSaveDestination)1, "ECUWRITE");
		AutoSaveForServerUpload(parameterCollection.Channel);
	}

	protected override void OnLoad(EventArgs e)
	{
		base.ParentForm.FormClosing += ParentForm_FormClosing;
		base.OnLoad(e);
	}

	private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.Cancel || base.IsDisposed)
		{
			return;
		}
		bool flag = false;
		if (activeChannels == null)
		{
			return;
		}
		foreach (Channel activeChannel in activeChannels)
		{
			if (HasUnsentChanges(activeChannel, ParameterType.Parameter) || HasUnsentChanges(activeChannel, ParameterType.Accumulator))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			string messageFormat_ShutdownRequestedWithUnsavedChanges = Resources.MessageFormat_ShutdownRequestedWithUnsavedChanges;
			DialogResult dialogResult = ControlHelpers.ShowMessageBox((Control)this, messageFormat_ShutdownRequestedWithUnsavedChanges, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
			if (dialogResult != DialogResult.Yes)
			{
				e.Cancel = true;
			}
			else
			{
				shutdownRequested = true;
			}
		}
	}

	private void OnActiveChannelsChanged(object sender, EventArgs e)
	{
		UpdateActiveChannels();
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		UpdateParameterWritePreconditionMonitoring();
		if (base.Visible)
		{
			if (activeChannels != null)
			{
				foreach (Channel activeChannel in activeChannels)
				{
					if (activeChannel.Online && !activeChannel.Parameters.HaveBeenReadFromEcu && !parametersBeingRead.Contains(activeChannel) && activeChannel.CommunicationsState != CommunicationsState.ReadParameters && activeChannel.CommunicationsState != CommunicationsState.ReadEcuInfo && activeChannel.Parameters.Count > 0)
					{
						parametersBeingRead.Add(activeChannel);
						activeChannel.Parameters.Read(synchronous: false);
					}
				}
				UpdateStatus();
				UpdateProhibitWarning();
			}
			UpdateProhibitWarningForPreconditions();
		}
		else
		{
			string[] names = Enum.GetNames(typeof(ProhibitReason));
			foreach (string text in names)
			{
				WarningsPanel.GlobalInstance.Remove(text.ToString());
			}
			preconditions.ForEach(delegate(Precondition pc)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				WarningsPanel.GlobalInstance.Remove(((object)pc.PreconditionType/*cast due to .constrained prefix*/).ToString());
			});
		}
		base.OnVisibleChanged(e);
	}

	private unsafe static void UpdateProhibitWarningFoReason(ProhibitReason reason, string messageFormat)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<Channel> source = ServerDataManager.GlobalInstance.ProhibitedChannels.Where((Channel c) => ServerDataManager.GlobalInstance.GetProhibitReason(c) == reason);
		if (source.Any())
		{
			EventHandler eventHandler = (ApplicationInformation.CanReprogramEdexUnits ? new EventHandler(warningPanel_Click) : null);
			WarningsPanel.GlobalInstance.Add(((object)(*(ProhibitReason*)(&reason))/*cast due to .constrained prefix*/).ToString(), MessageBoxIcon.Asterisk, (string)null, string.Format(CultureInfo.CurrentCulture, messageFormat, string.Join(", ", source.Select((Channel c) => c.Ecu.Name).ToArray())), eventHandler);
		}
		else
		{
			WarningsPanel.GlobalInstance.Remove(((object)(*(ProhibitReason*)(&reason))/*cast due to .constrained prefix*/).ToString());
		}
	}

	private static void UpdateProhibitWarning()
	{
		UpdateProhibitWarningFoReason((ProhibitReason)2, Resources.MessageFormat_MismatchLastServicedData);
		UpdateProhibitWarningFoReason((ProhibitReason)3, ApplicationInformation.CanReprogramEdexUnits ? Resources.MessageFormat_MissingLastServicedData : Resources.MessageFormat_ToolDoesntSupportLastServicedData);
		UpdateProhibitWarningFoReason((ProhibitReason)4, ApplicationInformation.CanReprogramEdexUnits ? Resources.MessageFormat_MissingIncompatibilityTable : Resources.MessageFormat_ToolDoesntSupportIncompatibilityTableUpdate);
		UpdateProhibitWarningFoReason((ProhibitReason)5, Resources.MessageFormat_ToolDoesntSupportParameterEditing);
		UpdateProhibitWarningFoReason((ProhibitReason)6, Resources.MessageFormat_NoVcp);
	}

	private void UpdateProhibitWarningForPreconditions()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		foreach (Precondition precondition in preconditions)
		{
			if ((int)precondition.State == 2)
			{
				string text = ((precondition.DiagnosticPanelName != null) ? ActionsMenuProxy.GlobalInstance.GetDialogLocalizedDisplayName(precondition.DiagnosticPanelName, (string)null, false) : null);
				if (text != null)
				{
					WarningsPanel.GlobalInstance.Add(((object)precondition.PreconditionType/*cast due to .constrained prefix*/).ToString(), MessageBoxIcon.Asterisk, (string)null, string.Format(CultureInfo.CurrentCulture, Resources.ParameterView_FormatPreconditionWithDialog, precondition.Text, text), (EventHandler)delegate
					{
						ActionsMenuProxy.GlobalInstance.ShowDialog(precondition.DiagnosticPanelName, (string)null, (object)null, false);
					});
				}
				else
				{
					WarningsPanel.GlobalInstance.Add(((object)precondition.PreconditionType/*cast due to .constrained prefix*/).ToString(), MessageBoxIcon.Asterisk, (string)null, string.Format(CultureInfo.CurrentCulture, Resources.ParameterView_FormatPrecondition, precondition.Text), (EventHandler)null);
				}
			}
			else
			{
				WarningsPanel.GlobalInstance.Remove(((object)precondition.PreconditionType/*cast due to .constrained prefix*/).ToString());
			}
		}
	}

	private static void warningPanel_Click(object sender, EventArgs e)
	{
		((IMenuProxy)MenuProxy.GlobalInstance).ContainerApplication.SelectPlace((PanelIdentifier)7, (string)null);
	}

	internal static ParameterFileFormat GetFileFormat(string fileName)
	{
		ParameterFileFormat result = ParameterFileFormat.ParFile;
		string text = fileName.ToUpper(CultureInfo.CurrentCulture);
		if (text.EndsWith(".VER", StringComparison.OrdinalIgnoreCase))
		{
			result = ParameterFileFormat.VerFile;
		}
		return result;
	}

	private static Channel GetConnectedChannel(string ecuName)
	{
		Channel result = null;
		foreach (Channel channel in SapiManager.GlobalInstance.Sapi.Channels)
		{
			if (channel.Ecu.Name == ecuName)
			{
				result = channel;
				break;
			}
		}
		return result;
	}

	internal bool OnParametersHistoryImportClick(object sender, EventArgs e)
	{
		//IL_003d: Expected O, but got Unknown
		bool result = false;
		OpenHistoryForm openHistoryForm = new OpenHistoryForm();
		if (openHistoryForm.ShowDialog() == DialogResult.OK)
		{
			Cursor.Current = Cursors.WaitCursor;
			StartRead(openHistoryForm.EntryName);
			byte[] array = null;
			try
			{
				array = FileEncryptionProvider.ReadEncryptedFile(openHistoryForm.FileName, true);
			}
			catch (InvalidChecksumException ex)
			{
				InvalidChecksumException ex2 = ex;
				EndRead(new Error(((Exception)(object)ex2).Message));
			}
			Error error = null;
			if (array != null)
			{
				using (MemoryStream stream = new MemoryStream(array))
				{
					using StreamReader stream2 = new StreamReader(stream);
					error = InternalImport(stream2, ParameterFileFormat.VerFile, encrypted: true);
				}
				result = !error.Fatal;
				EndRead(error);
				Cursor.Current = Cursors.Default;
			}
		}
		return result;
	}

	internal string ShowFileImportDialog(ParameterType type)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.InitialDirectory = importExportPath;
		if (type == ParameterType.Parameter)
		{
			openFileDialog.Filter = Resources.J2286OpenFilesFilter;
		}
		else
		{
			openFileDialog.Filter = Resources.J2286AccumulatorFilesFilter;
		}
		openFileDialog.FilterIndex = 1;
		openFileDialog.RestoreDirectory = false;
		if (openFileDialog.ShowDialog(this) == DialogResult.OK)
		{
			return openFileDialog.FileName;
		}
		return null;
	}

	internal bool OnParametersImportClick(string path)
	{
		bool result = false;
		if (path != null)
		{
			Cursor.Current = Cursors.WaitCursor;
			importExportPath = path;
			StartRead(path);
			Error error = null;
			using (StreamReader stream = new StreamReader(path))
			{
				ParameterFileFormat fileFormat = GetFileFormat(path);
				error = InternalImport(stream, fileFormat, encrypted: false);
			}
			result = !error.Fatal;
			EndRead(error);
			Cursor.Current = Cursors.Default;
		}
		return result;
	}

	private Error InternalImport(StreamReader stream, ParameterFileFormat format, bool encrypted)
	{
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		Channel channel = null;
		Collection<string> unknownList = new Collection<string>();
		Error error = new Error();
		try
		{
			channel = ImportToChannel(stream, format, encrypted, unknownList);
		}
		catch (DataException ex)
		{
			error.AddMessage(ex.Message, fatal: true);
		}
		catch (InvalidOperationException ex2)
		{
			error.AddMessage(ex2.Message, fatal: true);
		}
		catch (IOException ex3)
		{
			error.AddMessage(ex3.Message, fatal: true);
		}
		catch (FormatException ex4)
		{
			error.AddMessage(ex4.Message, fatal: true);
		}
		if (!error.Fatal && channel != null)
		{
			error.ImportErrors(GetParameterErrors(channel));
			error.ImportErrors(GetUnknownParameters(unknownList));
			if (channel.Parameters != null)
			{
				if (SapiExtensions.IsDataSourceEdex(channel.Ecu) && !SapiManager.GlobalInstance.UseNameValuePairsToParameterize)
				{
					string softwarePartNumber = SapiManager.GetSoftwarePartNumber(channel);
					EdexIncompatibilityFlashwareItem val = ServerDataManager.GlobalInstance.EdexIncompatibilityTable.FlashwareItems.FirstOrDefault((EdexIncompatibilityFlashwareItem fi) => PartExtensions.IsEqual(fi.PartNumber, softwarePartNumber));
					foreach (IGrouping<string, Parameter> item in from p in channel.Parameters
						where ParameterChanged(p)
						group p by p.GroupQualifier)
					{
						Parameter parameter = item.First();
						ParameterGroupDataItem val2 = new ParameterGroupDataItem(parameter, new Qualifier((QualifierTypes)128, channel.Ecu.Name, parameter.GroupQualifier), ServerDataManager.GlobalInstance.GetFactoryAggregatePart(channel, parameter.GroupQualifier), ServerDataManager.GlobalInstance.GetEngineeringCorrectionFactorParameters(channel, parameter.GroupQualifier));
						foreach (Parameter item2 in item)
						{
							bool flag = false;
							ParameterDataItem val3 = new ParameterDataItem(item2, new Qualifier((QualifierTypes)4, channel.Ecu.Name, item2.Qualifier), val2);
							CodingChoice codingChoice = val3.ValueAsCodingChoice;
							if (codingChoice != null)
							{
								if (val != null && val.ProhibitedParameters.Any((Part p) => PartExtensions.IsEqual(p, codingChoice.Part.ToString())))
								{
									error.AddMessage(string.Format(CultureInfo.CurrentCulture, Resources.ReportWarning_ParameterBlacklistFailure, codingChoice.Part.ToString(), item2.Qualifier), fatal: false);
								}
								else
								{
									flag = true;
								}
							}
							else
							{
								error.AddMessage(string.Format(CultureInfo.CurrentCulture, Resources.ReportWarning_ParameterNoPartNumberFailure, item2.Value, item2.Qualifier), fatal: false);
							}
							if (!flag && item2.OriginalValue != null)
							{
								item2.Value = item2.OriginalValue;
							}
						}
					}
				}
				if (!channel.Parameters.Any((Parameter p) => ParameterChanged(p)))
				{
					error.AddMessage(string.Format(CultureInfo.CurrentCulture, Resources.ReportWarningNoParametersChanged, channel.Ecu.Name), fatal: false);
				}
			}
		}
		return error;
	}

	private Channel ImportToChannel(StreamReader stream, ParameterFileFormat format, bool encrypted, Collection<string> unknownList)
	{
		Channel targetChannel = null;
		TargetEcuDetails targetEcuDetails = ParameterCollection.GetTargetEcuDetails(stream, format);
		if (targetEcuDetails == null || string.IsNullOrEmpty(targetEcuDetails.Ecu))
		{
			throw new InvalidOperationException(Resources.ReportError_NotAValidFile);
		}
		targetChannel = GetConnectedChannel(targetEcuDetails.Ecu);
		if (targetChannel != null)
		{
			if (targetChannel.DiagnosisVariant.IsBase)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_CannotImportParametersToBaseVariant, targetEcuDetails.Ecu));
			}
			if (ServerDataManager.GlobalInstance.ProhibitedChannels.Any((Channel c) => c.Ecu.Name == targetChannel.Ecu.Name))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ChannelLockedForEditing, targetEcuDetails.Ecu));
			}
			if (targetEcuDetails.DiagnosisVariant.Length > 0 && !string.Equals(targetEcuDetails.DiagnosisVariant, targetChannel.DiagnosisVariant.Name, StringComparison.OrdinalIgnoreCase))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, Resources.VariantDoesNotMatchFormat, targetEcuDetails.DiagnosisVariant, targetChannel.DiagnosisVariant.Name);
				if (encrypted)
				{
					stringBuilder.Append(Environment.NewLine);
					stringBuilder.Append(Environment.NewLine);
					stringBuilder.Append(Resources.ReviewParameters);
				}
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(Resources.ContinueImporting);
				DialogResult dialogResult = MessageBox.Show(stringBuilder.ToString(), ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, ControlHelpers.IsRightToLeft((Control)this) ? (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) : ((MessageBoxOptions)0));
				if (dialogResult == DialogResult.No)
				{
					targetChannel = null;
				}
			}
			if (targetChannel != null)
			{
				if (encrypted && targetChannel.Online)
				{
					foreach (Parameter parameter in targetChannel.Parameters)
					{
						if (parameter.DefaultValue != null && parameter.WriteAccess <= SapiManager.GlobalInstance.Sapi.HardwareAccess)
						{
							parameter.Value = parameter.DefaultValue;
						}
					}
				}
				targetChannel.Parameters.Load(stream, format, unknownList, !encrypted);
			}
		}
		else
		{
			if (SapiManager.GlobalInstance.Sapi.Channels.Where((Channel x) => x.ConnectionResource != null).Any())
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.FormatCanNotImport, targetEcuDetails.Ecu));
			}
			if (targetEcuDetails.DiagnosisVariant.Length > 0)
			{
				targetChannel = SapiManager.GlobalInstance.Sapi.Channels.ConnectOffline(stream, format, unknownList);
			}
			else
			{
				Ecu ecu = SapiManager.GlobalInstance.Sapi.Ecus[targetEcuDetails.Ecu];
				if (ecu == null)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.FormatDeviceDoesNotExist, targetEcuDetails.Ecu));
				}
				DiagnosisVariant targetVariant = ecu.DiagnosisVariants[targetEcuDetails.AssumedDiagnosisVariant];
				VariantSelect variantSelect = new VariantSelect(ecu, targetVariant);
				variantSelect.ShowDialog();
				if (variantSelect.DiagnosisVariant != null)
				{
					targetChannel = SapiManager.GlobalInstance.Sapi.Channels.ConnectOffline(variantSelect.DiagnosisVariant);
					targetChannel.Parameters.Load(stream, format, unknownList, !encrypted);
				}
			}
		}
		return targetChannel;
	}

	private static Error GetParameterErrors(Channel targetChannel)
	{
		Error error = new Error();
		foreach (Parameter parameter in targetChannel.Parameters)
		{
			if (parameter.Exception != null)
			{
				error.AddMessage(string.Format(CultureInfo.CurrentCulture, Resources.ParameterView_ErrorMessageFormat, parameter.GroupName, parameter.Name, parameter.Exception.Message), fatal: false);
			}
		}
		return error;
	}

	private static Error GetUnknownParameters(IEnumerable<string> unknownList)
	{
		Error error = new Error();
		foreach (string unknown in unknownList)
		{
			error.AddMessage(string.Format(CultureInfo.CurrentCulture, Resources.ParameterView_ErrorMessageFormat_UnknownParameter, unknown), fatal: false);
		}
		return error;
	}

	internal void OnParametersExportClick(Channel channel, ParameterType type)
	{
		ParameterExportForm parameterExportForm = new ParameterExportForm(channel, type);
		parameterExportForm.ExportPath = importExportPath;
		parameterExportForm.ShowDialog(this);
		importExportPath = parameterExportForm.ExportPath;
	}

	private void OnChannelCommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		UpdateStatus();
	}

	private static bool HasUnsentChanges(Channel c, ParameterType type)
	{
		bool result = false;
		foreach (Parameter parameter in c.Parameters)
		{
			if (!object.Equals(parameter.Value, parameter.OriginalValue) && !object.Equals(parameter.Value, parameter.LastPersistedValue) && ((type == ParameterType.Accumulator && !parameter.Persistable) || (type == ParameterType.Parameter && parameter.Persistable)))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private bool AskUserToSaveChanges(Channel c, ParameterType type)
	{
		string text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ChangesToExport, type, c.Ecu.Name);
		DialogResult dialogResult = ControlHelpers.ShowMessageBox((Control)this, text, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
		return dialogResult == DialogResult.Yes;
	}

	private void CheckForUnsentChanges(Channel c, ParameterType type)
	{
		if (HasUnsentChanges(c, type) && AskUserToSaveChanges(c, type))
		{
			ParameterExportForm parameterExportForm = new ParameterExportForm(c, type);
			parameterExportForm.ShowDialog();
		}
	}

	private void UpdateStatus()
	{
		dirty = true;
		Invalidate();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (!performedInitialUpdateActiveChannels)
		{
			performedInitialUpdateActiveChannels = true;
			UpdateActiveChannels();
			UpdateProhibitWarning();
		}
		if (dirty)
		{
			dirty = false;
			UpdateStatusDeferred();
		}
		base.OnPaint(e);
	}

	private void UpdateStatusDeferred()
	{
		int num = 0;
		int num2 = 0;
		if (activeChannels != null)
		{
			foreach (Channel activeChannel in activeChannels)
			{
				CommunicationsState communicationsState = activeChannel.CommunicationsState;
				if (communicationsState != CommunicationsState.Online)
				{
					if (communicationsState != CommunicationsState.ReadParameters)
					{
						if (communicationsState != CommunicationsState.WriteParameters)
						{
							continue;
						}
						StartWrite();
					}
					else
					{
						StartRead(string.Empty);
					}
					num2 = ((num2 <= 0) ? ((int)activeChannel.Parameters.Progress) : ((num2 + (int)activeChannel.Parameters.Progress) / 2));
				}
				else
				{
					num += activeChannel.Parameters.Count((Parameter p) => ParameterChanged(p));
				}
			}
		}
		progressBar.Value = num2;
		buttonSend.Enabled = num > 0 && channelsToWrite.Count == 0 && preconditions.All((Precondition pc) => (int)pc.State != 2);
		menuProxy.Notify(num);
	}

	private static bool ParameterChanged(Parameter parameter)
	{
		return !object.Equals(parameter.Value, parameter.OriginalValue);
	}

	private void StartWrite()
	{
		if (!writeStarted)
		{
			ReportStatus(Resources.readwrite, Resources.WritingParameters, string.Empty, showBox: false);
			writeStarted = true;
		}
	}

	private void EndWrite(ErrorStatus status, string message)
	{
		if (writeStarted)
		{
			if (status != ErrorStatus.noError)
			{
				errorMessages.Add(new KeyValuePair<ErrorStatus, string>(status, message));
			}
			writeStarted = false;
			if (!SendNextWrite())
			{
				ReportWriteResult();
			}
		}
	}

	private void ReportWriteResult()
	{
		if (errorMessages.Count == 0)
		{
			ReportSuccess(Resources.ParametersWritten);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		ErrorStatus errorStatus = ErrorStatus.noError;
		foreach (KeyValuePair<ErrorStatus, string> errorMessage in errorMessages)
		{
			errorStatus |= errorMessage.Key;
			stringBuilder.Append(errorMessage.Value).AppendLine();
		}
		switch (errorStatus)
		{
		case ErrorStatus.warning | ErrorStatus.error:
			ReportError(Resources.ParameterView_OneOrMoreErrorsAndWarningsDuringWrite, stringBuilder.ToString());
			break;
		case ErrorStatus.error:
			ReportError(Resources.ParameterView_OneOrMoreErrorsDuringWrite, stringBuilder.ToString());
			break;
		case ErrorStatus.warning:
			ReportWarning(Resources.ParameterView_OneOrMoreWarningsDuringWrite, stringBuilder.ToString());
			break;
		}
		errorMessages.Clear();
	}

	private void StartRead(string source)
	{
		if (!readStarted)
		{
			importFile = source;
			if (string.IsNullOrEmpty(importFile))
			{
				ReportStatus(Resources.readwrite, Resources.ReadingParameters, string.Empty, showBox: false);
			}
			else
			{
				ReportStatus(Resources.readwrite, Resources.ImportingParameters, string.Empty, showBox: false);
			}
			readStarted = true;
		}
	}

	private void EndRead(Error error)
	{
		if (!readStarted)
		{
			return;
		}
		if (string.IsNullOrEmpty(importFile))
		{
			if (error == null || !error.HasMessages)
			{
				ReportSuccess(Resources.ReportSuccess_ParametersSuccessfullyRead);
			}
			else
			{
				ReportStatus(error.Fatal ? Resources.error : Resources.warning, Resources.ReportError_OneOrMoreErrorsOccurredReading, error.DisplayMessage, showBox: false);
			}
		}
		else if (error == null || !error.HasMessages)
		{
			ReportSuccess(string.Format(CultureInfo.CurrentCulture, Resources.ReportSuccess_ParametersSuccessfullyImported, importFile));
		}
		else
		{
			ReportError(string.Format(CultureInfo.CurrentCulture, Resources.ReportError_OneOrMoreErrorsOccurredImporting, importFile), error.DisplayMessage);
		}
		readStarted = false;
		importFile = string.Empty;
	}

	private void ReportSuccess(string title)
	{
		ReportStatus(Resources.done, title, string.Empty, showBox: false);
	}

	private void ReportError(string title, string error)
	{
		ReportStatus(Resources.error, title, error, showBox: true);
	}

	private void ReportWarning(string title, string error)
	{
		ReportStatus(Resources.outofrange, title, error, showBox: false);
	}

	private void ClearStatus()
	{
		ReportStatus(null, string.Empty, string.Empty, showBox: false);
	}

	private void ReportStatus(Image image, string title, string additional, bool showBox)
	{
		pictureBoxStatus.Image = image;
		string empty = string.Empty;
		if (string.IsNullOrEmpty(additional))
		{
			labelStatus.Text = title;
			empty = title;
		}
		else
		{
			labelStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.ParameterView_Format_StatusLabel, title);
			empty = string.Format(CultureInfo.CurrentCulture, Resources.ParameterView_Format_FullStatus, title, additional);
		}
		if (showBox)
		{
			StatusMessageBox.Report(base.Parent, empty, (StatusMessageType)2);
		}
		toolTipStatus.SetToolTip(labelStatus, empty);
		toolTipStatus.SetToolTip(pictureBoxStatus, empty);
	}

	private void UpdateParameterWritePreconditionMonitoring()
	{
		if (base.Visible)
		{
			preconditions.ForEach(delegate(Precondition pc)
			{
				pc.StateChanged += Precondition_StateChanged;
			});
		}
		else
		{
			preconditions.ForEach(delegate(Precondition pc)
			{
				pc.StateChanged -= Precondition_StateChanged;
			});
		}
	}

	public bool Search(string searchText, bool caseSensitive, FindMode direction)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ((PanelBase)panelHost).Search(searchText, caseSensitive, direction);
	}

	public void Undo()
	{
		((PanelBase)panelHost).Undo();
	}

	public void Copy()
	{
		((PanelBase)panelHost).Copy();
	}

	public void Cut()
	{
		((PanelBase)panelHost).Cut();
	}

	public void Delete()
	{
		((PanelBase)panelHost).Delete();
	}

	public void Paste()
	{
		((PanelBase)panelHost).Paste();
	}

	public void SelectAll()
	{
		((PanelBase)panelHost).SelectAll();
	}

	public string ToHtml()
	{
		return ((PanelBase)panelHost).ToHtml();
	}

	public void Activate()
	{
		((PanelBase)panelHost).Activate();
	}

	public bool Deactivate()
	{
		return ((PanelBase)panelHost).Deactivate();
	}

	public bool SelectLocation(string namespaceLocation)
	{
		return ((PanelBase)panelHost).SelectLocation(namespaceLocation);
	}

	public void RefreshView()
	{
		if (activeChannels == null)
		{
			return;
		}
		foreach (Channel activeChannel in activeChannels)
		{
			if (activeChannel.CommunicationsState == CommunicationsState.Online)
			{
				activeChannel.Parameters.ReadAll(synchronous: false);
			}
		}
	}

	private void OnFilteredContentChanged(object sender, EventArgs e)
	{
		if (this.FilteredContentChanged != null)
		{
			this.FilteredContentChanged(this, e);
		}
	}

	public void OnDispose(bool disposing)
	{
		if (disposing && panelHost != null)
		{
			((PanelBase)panelHost).FilteredContentChanged -= OnFilteredContentChanged;
		}
	}

	public void ExpandAllItems()
	{
		if (panelHost != null)
		{
			((PanelBase)panelHost).ExpandAllItems();
		}
	}

	public void CollapseAllItems()
	{
		if (panelHost != null)
		{
			((PanelBase)panelHost).CollapseAllItems();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.ParameterView));
		this.informationLinkButton = new ContextLinkButton();
		this.labelStatus = new System.Windows.Forms.Label();
		this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
		this.progressBar = new System.Windows.Forms.ProgressBar();
		this.buttonSend = new System.Windows.Forms.Button();
		this.panelHost = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.ParameterPanels();
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.toolTipStatus = new System.Windows.Forms.ToolTip(this.components);
		System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
		panel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxStatus).BeginInit();
		this.tableLayoutPanel.SuspendLayout();
		base.SuspendLayout();
		panel.Controls.Add((System.Windows.Forms.Control)(object)this.informationLinkButton);
		panel.Controls.Add(this.labelStatus);
		panel.Controls.Add(this.pictureBoxStatus);
		panel.Controls.Add(this.progressBar);
		panel.Controls.Add(this.buttonSend);
		resources.ApplyResources(panel, "panelControl");
		panel.Name = "panelControl";
		resources.ApplyResources(this.informationLinkButton, "informationLinkButton");
		this.informationLinkButton.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
		this.informationLinkButton.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.informationLinkButton.ImageSource = (ButtonImageSource)1;
		((System.Windows.Forms.Control)(object)this.informationLinkButton).Name = "informationLinkButton";
		this.informationLinkButton.ShowImage = true;
		resources.ApplyResources(this.labelStatus, "labelStatus");
		this.labelStatus.AutoEllipsis = true;
		this.labelStatus.Name = "labelStatus";
		this.labelStatus.UseMnemonic = false;
		resources.ApplyResources(this.pictureBoxStatus, "pictureBoxStatus");
		this.pictureBoxStatus.Name = "pictureBoxStatus";
		this.pictureBoxStatus.TabStop = false;
		resources.ApplyResources(this.progressBar, "progressBar");
		this.progressBar.Name = "progressBar";
		resources.ApplyResources(this.buttonSend, "buttonSend");
		this.buttonSend.Name = "buttonSend";
		this.buttonSend.UseVisualStyleBackColor = true;
		this.buttonSend.Click += new System.EventHandler(OnSendClick);
		resources.ApplyResources(this.panelHost, "panelHost");
		((System.Windows.Forms.Control)(object)this.panelHost).Name = "panelHost";
		resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
		this.tableLayoutPanel.Controls.Add(panel, 0, 1);
		this.tableLayoutPanel.Controls.Add((System.Windows.Forms.Control)(object)this.panelHost, 0, 0);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		this.toolTipStatus.AutoPopDelay = 0;
		this.toolTipStatus.InitialDelay = 500;
		this.toolTipStatus.ReshowDelay = 100;
		this.toolTipStatus.ShowAlways = true;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel);
		base.Name = "ParameterView";
		panel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pictureBoxStatus).EndInit();
		this.tableLayoutPanel.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
