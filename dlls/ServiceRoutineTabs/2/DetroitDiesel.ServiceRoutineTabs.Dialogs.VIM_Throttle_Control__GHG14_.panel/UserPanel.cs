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
using DetroitDiesel.Common;
using DetroitDiesel.Net;
using DetroitDiesel.Security;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VIM_Throttle_Control__GHG14_.panel;

public class UserPanel : CustomPanel
{
	private enum PedalState
	{
		Unknown,
		Initializing,
		InvalidPedalType,
		TemporaryPedalSettingActive,
		ValidPedalType
	}

	private class Transaction
	{
		private enum ActionType
		{
			Cleanup,
			Rollback
		}

		private Stack<Tuple<ActionType, Action>> allCleanupActions = new Stack<Tuple<ActionType, Action>>();

		public bool Commited { get; private set; }

		public bool Completed { get; private set; }

		public Exception Exception { get; private set; }

		public event EventHandler<EventArgs> TransactionCompleted;

		public void AddRollback(Action action)
		{
			allCleanupActions.Push(Tuple.Create(ActionType.Rollback, action));
		}

		public void AddCleanup(Action action)
		{
			allCleanupActions.Push(Tuple.Create(ActionType.Cleanup, action));
		}

		public void Commit()
		{
			if (!Completed)
			{
				Commited = true;
				DoCleanup(includeRollback: false);
				Completed = true;
				OnTransactionCompleted();
			}
		}

		public void Rollback()
		{
			Rollback(null);
		}

		public void Rollback(Exception exception)
		{
			if (!Completed)
			{
				Commited = false;
				Exception = exception;
				DoCleanup(includeRollback: true);
				Completed = true;
				OnTransactionCompleted();
			}
		}

		private void DoCleanup(bool includeRollback)
		{
			List<Exception> list = new List<Exception>();
			while (allCleanupActions.Count > 0)
			{
				try
				{
					Tuple<ActionType, Action> tuple = allCleanupActions.Pop();
					if (includeRollback || tuple.Item1 == ActionType.Cleanup)
					{
						tuple.Item2();
					}
				}
				catch (Exception item)
				{
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				throw new AggregateException(string.Format("Exception occurred during transaction {0}.", includeRollback ? "rollback" : "cleanup"), list);
			}
		}

		private void OnTransactionCompleted()
		{
			this.TransactionCompleted?.Invoke(this, EventArgs.Empty);
		}
	}

	private const string PedalParameterQualifier = "Accel_Pedal_Type";

	private const string EcuName = "CPC04T";

	private static string VimCompatiblePedalSettingText = Resources.Message_AnalogPedalType3;

	private static int VimCompatiblePedalSettingRawValue = 4;

	private static string ButonTextSetValidThrottle = Resources.Message_EnableVIMThrottleSupport;

	private static string ButtonTextRestoreOriginalPedal = Resources.Message_RestoreOriginalPedalType;

	private static string InitializingMessage = Resources.Message_Initializing;

	private static string OfflineMessage = string.Format(Resources.MessageFormat_The0IsNoLongerOnlinePleaseReconnectTheECUBeforeContinuing, "CPC04T");

	private static string InvalidPedalTypeMessage = string.Format(Resources.MessageFormat_TheCurrentAcceleratorPedalTypeIsNotCompatible, ButonTextSetValidThrottle);

	private static string TemporaryPedalMessage = string.Format(Resources.MessageFormat_ToEnableCompatibility, VimCompatiblePedalSettingText, "{0}", ButtonTextRestoreOriginalPedal);

	private static string ValidPedalTypeMessage = Resources.Message_TheVehicleSettingsAreCompatibleWithTheVIMNoActionIsRequiredToEnableSupportForTheVIM;

	private static string TemporaryPedalInvalidInHistoryMessage = TemporaryPedalMessage;

	private static readonly Regex pedalValueRegex = new Regex("^\\s*P,Accel_Pedal_Type,\\d{4},(?<PedalValue>\\d+)", RegexOptions.Compiled);

	private Channel channel;

	private PedalState pedalState;

	private Dictionary<PedalState, Transaction> pendingTransactions = new Dictionary<PedalState, Transaction>();

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

	private Channel Channel
	{
		get
		{
			if (ChannelLocked && !object.Equals(lockedChannel, channel))
			{
				throw new InvalidOperationException("The communication channel has been lost");
			}
			return channel;
		}
		set
		{
			if (channel != value)
			{
				if (channel != null)
				{
					channel.CommunicationsStateUpdateEvent -= Channel_CommunicationsStateUpdateEvent;
				}
				channel = value;
				if (channel != null)
				{
					channel.CommunicationsStateUpdateEvent += Channel_CommunicationsStateUpdateEvent;
				}
				OnChannelChanged();
			}
		}
	}

	private Parameter PedalParameter
	{
		get
		{
			if (Channel == null)
			{
				throw new InvalidOperationException("The Channel property cannot be null.");
			}
			return Channel.Parameters["Accel_Pedal_Type"];
		}
	}

	private bool HistoryFileContainedMismatch { get; set; }

	private bool CanClose
	{
		get
		{
			Transaction transaction = GetTransaction(CurrentPedalState);
			bool flag = transaction != null && !transaction.Completed;
			return !Online || (!flag && CurrentPedalState != PedalState.TemporaryPedalSettingActive);
		}
	}

	private bool Closing { get; set; }

	private bool DisplayProgress
	{
		get
		{
			return progressBar.Visible;
		}
		set
		{
			progressBar.Visible = value;
			progressLabel.Visible = value;
		}
	}

	private string VehicleIdentificationNumber { get; set; }

	private string EngineSerialNumber { get; set; }

	private bool ChannelIdle => channel != null && channel.CommunicationsState == CommunicationsState.Online;

	private bool Online => channel != null && channel.Online;

	private PedalState CurrentPedalState
	{
		get
		{
			return pedalState;
		}
		set
		{
			if (pedalState != value)
			{
				PedalState previousState = pedalState;
				pedalState = value;
				OnPedalStateChanged(previousState);
			}
		}
	}

	private Transaction CurrentTransaction => GetTransaction(CurrentPedalState) ?? CreateTransaction(CurrentPedalState);

	private bool ChannelLocked => lockedChannel != null;

	private Choice OriginalPedalTypeValue { get; set; }

	private Choice VimCompatiblePedalTypeValue
	{
		get
		{
			if (vimCompatiblePedalTypeValue == null)
			{
				vimCompatiblePedalTypeValue = PedalParameter.Choices.GetItemFromRawValue(VimCompatiblePedalSettingRawValue);
			}
			return vimCompatiblePedalTypeValue;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	private Transaction CreateTransaction(PedalState state)
	{
		Transaction transaction = new Transaction();
		transaction.TransactionCompleted += delegate(object sender, EventArgs args)
		{
			Transaction transaction2 = sender as Transaction;
			if (!transaction2.Commited)
			{
				if (transaction2.Exception != null)
				{
					if (transaction2.Exception is CaesarException || transaction2.Exception is InvalidOperationException)
					{
						LogMessage(Resources.MessageFormat_OperationFailedError0, transaction2.Exception.Message);
					}
					else
					{
						if (!(transaction2.Exception is OperationCanceledException))
						{
							throw transaction2.Exception;
						}
						LogMessage(transaction2.Exception.Message);
					}
					if (state != PedalState.Unknown && state != PedalState.Initializing)
					{
						((Control)this).BeginInvoke((Delegate)(Action)delegate
						{
							InitializeVehicleDataAsync();
						});
					}
				}
				if (state == PedalState.Initializing)
				{
					CurrentPedalState = PedalState.Unknown;
				}
			}
		};
		pendingTransactions.Add(state, transaction);
		return transaction;
	}

	private Transaction GetTransaction(PedalState state)
	{
		return pendingTransactions.ContainsKey(state) ? pendingTransactions[state] : null;
	}

	private void CloseTransaction(PedalState state)
	{
		if (!pendingTransactions.ContainsKey(state))
		{
			return;
		}
		if (!pendingTransactions[state].Completed)
		{
			try
			{
				pendingTransactions[state].Rollback();
			}
			catch (AggregateException)
			{
			}
		}
		pendingTransactions.Remove(state);
	}

	private void OnPedalStateChanged(PedalState previousState)
	{
		CloseTransaction(previousState);
		switch (CurrentPedalState)
		{
		case PedalState.InvalidPedalType:
			if (previousState == PedalState.TemporaryPedalSettingActive)
			{
				LogMessage(Resources.MessageFormat_AccelPedalTypeParameterRestoredBackTo0, OriginalPedalTypeValue);
			}
			else
			{
				LogMessage(Resources.MessageFormat_AccelPedalTypeParameterValue0IsNotSupportedByTheVIM, OriginalPedalTypeValue);
			}
			break;
		case PedalState.Initializing:
			if (previousState == PedalState.TemporaryPedalSettingActive && !Online)
			{
				LogMessage(Resources.MessageFormat_UnableToRestoreTheParameterAccelPedalTypeBackTo1Because0IsDisconnected, "CPC04T", OriginalPedalTypeValue);
			}
			break;
		case PedalState.TemporaryPedalSettingActive:
			if (previousState == PedalState.InvalidPedalType)
			{
				LogMessage(Resources.MessageFormat_AccelPedalTypeParameterChangedFrom0To1, OriginalPedalTypeValue, VimCompatiblePedalSettingText);
			}
			else
			{
				LogMessage(Resources.MessageFormat_AccelPedalTypeParameterWasPreviouslyChangedFrom0To1, OriginalPedalTypeValue, VimCompatiblePedalSettingText);
			}
			break;
		case PedalState.ValidPedalType:
			LogMessage(Resources.Message_AccelPedalTypeParameterSupportedByVIM);
			break;
		case PedalState.Unknown:
			if (previousState == PedalState.TemporaryPedalSettingActive && !Online)
			{
				LogMessage(Resources.MessageFormat_UnableToRestoreTheParameterAccelPedalTypeBackTo1Because0IsDisconnected1, "CPC04T", OriginalPedalTypeValue);
			}
			OriginalPedalTypeValue = null;
			HistoryFileContainedMismatch = false;
			EngineSerialNumber = string.Empty;
			VehicleIdentificationNumber = string.Empty;
			break;
		}
		UpdateUserInterface();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += ParentForm_FormClosing;
		UpdateUserInterface();
		((UserControl)this).OnLoad(e);
	}

	public override void OnChannelsChanged()
	{
		if (!Closing)
		{
			Channel = ((CustomPanel)this).GetChannel("CPC04T");
		}
		((CustomPanel)this).OnChannelsChanged();
	}

	private void OnChannelChanged()
	{
		if (!Closing)
		{
			CurrentPedalState = PedalState.Unknown;
			InitializeVehicleDataAsync();
		}
	}

	private Action LockChannel()
	{
		Channel originalTarget = lockedChannel;
		lockedChannel = Channel;
		return delegate
		{
			lockedChannel = originalTarget;
		};
	}

	private void InitializeVehicleDataAsync()
	{
		CurrentPedalState = PedalState.Initializing;
		if (ChannelIdle)
		{
			try
			{
				CurrentTransaction.AddCleanup(EnableInitializingProgressDisplay());
				CurrentTransaction.AddCleanup(LockChannel());
				HistoryFileContainedMismatch = false;
				EngineSerialNumber = SapiManager.GetEngineSerialNumber(Channel);
				VehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(Channel);
				InitializePedalTypeAsync();
				return;
			}
			catch (Exception exception)
			{
				CurrentTransaction.Rollback(exception);
				return;
			}
		}
		CurrentTransaction.Rollback();
	}

	private void Channel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		if (e.CommunicationsState == CommunicationsState.Online && CurrentPedalState == PedalState.Unknown)
		{
			InitializeVehicleDataAsync();
		}
		else if (e.CommunicationsState == CommunicationsState.Disconnecting)
		{
			CurrentPedalState = PedalState.Unknown;
		}
	}

	private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (!CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			Closing = true;
			Channel = null;
			((ContainerControl)this).ParentForm.FormClosing -= ParentForm_FormClosing;
		}
	}

	private void LogMessage(string message)
	{
		((CustomPanel)this).LabelLog(outputLogView.RequiredUserLabelPrefix, message);
	}

	private void LogMessage(string message, params object[] args)
	{
		((CustomPanel)this).LabelLog(outputLogView.RequiredUserLabelPrefix, string.Format(CultureInfo.InvariantCulture, message, args));
	}

	private void setResetButton_Click(object sender, EventArgs e)
	{
		switch (CurrentPedalState)
		{
		case PedalState.InvalidPedalType:
			EnableVimPedalSupportAsync();
			break;
		case PedalState.TemporaryPedalSettingActive:
			RestoreOriginalPedalTypeAsync();
			break;
		}
	}

	private void EnableVimPedalSupportAsync()
	{
		if (CurrentPedalState != PedalState.TemporaryPedalSettingActive)
		{
			try
			{
				PrepareForPedalWriteOperation();
				SaveSnapshotOfParameter();
				WritePedalParameterAsync(VimCompatiblePedalTypeValue);
			}
			catch (Exception exception)
			{
				CurrentTransaction.Rollback(exception);
			}
		}
	}

	private void RestoreOriginalPedalTypeAsync()
	{
		if (CurrentPedalState == PedalState.TemporaryPedalSettingActive)
		{
			try
			{
				PrepareForPedalWriteOperation();
				WritePedalParameterAsync(OriginalPedalTypeValue);
			}
			catch (Exception exception)
			{
				CurrentTransaction.Rollback(exception);
			}
		}
	}

	private void PrepareForPedalWriteOperation()
	{
		CurrentTransaction.AddCleanup(EnableWriteProgressDisplay());
		CurrentTransaction.AddCleanup(EnableWaitCursor());
		CurrentTransaction.AddCleanup(DisableActionableControls());
		CurrentTransaction.AddCleanup(LockChannel());
	}

	private void WritePedalParameterAsync(Choice value)
	{
		CurrentTransaction.AddCleanup(LockChannel());
		object value2 = PedalParameter.Value;
		PedalParameter.Value = value;
		if (!WriteUnlock())
		{
			PedalParameter.Value = value2;
			throw new OperationCanceledException();
		}
		Channel.Parameters.ParametersWriteCompleteEvent += PedalParameter_WriteCompleteEvent;
		Channel.Parameters.Write(synchronous: false);
	}

	private void PedalParameter_WriteCompleteEvent(object sender, ResultEventArgs e)
	{
		ParameterCollection parameterCollection = sender as ParameterCollection;
		parameterCollection.ParametersWriteCompleteEvent -= PedalParameter_WriteCompleteEvent;
		if (e.Succeeded)
		{
			CurrentTransaction.Commit();
			CurrentPedalState = ((parameterCollection["Accel_Pedal_Type"].Value == VimCompatiblePedalTypeValue) ? PedalState.TemporaryPedalSettingActive : PedalState.InvalidPedalType);
		}
		else
		{
			CurrentTransaction.Rollback(e.Exception);
		}
	}

	private void SaveSnapshotOfParameter()
	{
		if (!HistoryFileContainedMismatch)
		{
			string settingsFileName = Utility.GetSettingsFileName(Channel, "ECUREAD", DateTime.Now, (SettingsFileFormat)2);
			ServerDataManager.SaveSettings(Channel.Parameters, Directories.DrumrollHistoryData, settingsFileName, ParameterFileFormat.VerFile);
		}
	}

	private void UpdateUserInterface()
	{
		switch (CurrentPedalState)
		{
		case PedalState.InvalidPedalType:
			currentStatusLabel.Text = InvalidPedalTypeMessage;
			setResetButton.Text = ButonTextSetValidThrottle;
			setResetButton.Visible = true;
			break;
		case PedalState.TemporaryPedalSettingActive:
			currentStatusLabel.Text = string.Format(HistoryFileContainedMismatch ? TemporaryPedalInvalidInHistoryMessage : TemporaryPedalMessage, OriginalPedalTypeValue);
			setResetButton.Text = ButtonTextRestoreOriginalPedal;
			setResetButton.Visible = true;
			break;
		case PedalState.ValidPedalType:
			currentStatusLabel.Text = ValidPedalTypeMessage;
			setResetButton.Visible = false;
			break;
		case PedalState.Initializing:
			currentStatusLabel.Text = InitializingMessage;
			setResetButton.Visible = false;
			break;
		case PedalState.Unknown:
			DisplayProgress = false;
			currentStatusLabel.Text = (Online ? InitializingMessage : OfflineMessage);
			setResetButton.Visible = false;
			break;
		}
		buttonClose.Enabled = CanClose;
		closeWarningLabel.Visible = !CanClose;
	}

	private void InitializePedalTypeAsync()
	{
		if (ChannelIdle)
		{
			CurrentTransaction.AddCleanup(LockChannel());
			Channel.Parameters.ParametersReadCompleteEvent += PedalType_ReadCompleteEvent;
			Channel.Parameters.ReadGroup(PedalParameter.GroupQualifier, fromCache: false, synchronous: false);
		}
		else
		{
			CurrentTransaction.Rollback();
		}
	}

	private void PedalType_ReadCompleteEvent(object sender, ResultEventArgs args)
	{
		ParameterCollection parameterCollection = sender as ParameterCollection;
		parameterCollection.ParametersReadCompleteEvent -= PedalType_ReadCompleteEvent;
		PedalState pedalState = PedalState.Unknown;
		try
		{
			if (!args.Succeeded || args.Exception != null)
			{
				CurrentTransaction.Rollback(args.Exception);
				return;
			}
			OriginalPedalTypeValue = parameterCollection["Accel_Pedal_Type"].Value as Choice;
			if (OriginalPedalTypeValue == VimCompatiblePedalTypeValue)
			{
				Choice choice = FindPedalTypeFromHistory();
				if (choice != null)
				{
					LogMessage(Resources.MessageFormat_ParameterAccelPedalType0LocatedInHistory, choice);
					OriginalPedalTypeValue = choice;
					HistoryFileContainedMismatch = true;
					pedalState = PedalState.TemporaryPedalSettingActive;
				}
				else
				{
					pedalState = PedalState.ValidPedalType;
				}
			}
			else
			{
				pedalState = PedalState.InvalidPedalType;
			}
			CurrentTransaction.Commit();
			CurrentPedalState = pedalState;
		}
		catch (Exception exception)
		{
			CurrentTransaction.Rollback(exception);
		}
	}

	private Choice ReadPedalType(string filename)
	{
		Choice choice = null;
		try
		{
			byte[] array = FileEncryptionProvider.ReadEncryptedFile(filename, true);
			if (array != null)
			{
				using StreamReader streamReader = new StreamReader(new MemoryStream(array), Encoding.UTF8);
				while (!streamReader.EndOfStream && choice == null)
				{
					Match match = pedalValueRegex.Match(streamReader.ReadLine());
					if (match.Success && int.TryParse(match.Groups["PedalValue"].Value, out var result))
					{
						choice = PedalParameter.Choices.GetItemFromRawValue(result);
					}
				}
			}
		}
		catch (InvalidChecksumException)
		{
		}
		return choice;
	}

	private Choice FindPedalTypeFromHistory()
	{
		IEnumerable<string> source = from filePath in Directory.EnumerateFiles(Directories.DrumrollHistoryData)
			let fileInfo = FileNameInformation.FromName(FileEncryptionProvider.DecryptFileName(Path.GetFileName(filePath)), (FileType)2)
			where fileInfo.Valid && MatchesEcu(fileInfo) && MatchesCurrentVehicle(fileInfo)
			orderby Utility.TimeFromString(fileInfo.Timestamp) descending
			select filePath;
		return (from file in source
			let pedalValue = ReadPedalType(file)
			where pedalValue != null && pedalValue != VimCompatiblePedalTypeValue
			select pedalValue).FirstOrDefault();
	}

	private static bool MatchesEcu(FileNameInformation value)
	{
		return value != null && string.Equals(value.Device, "CPC04T", StringComparison.OrdinalIgnoreCase);
	}

	private bool MatchesCurrentVehicle(FileNameInformation value)
	{
		return value != null && string.Equals(value.EngineSerialNumber, EngineSerialNumber, StringComparison.OrdinalIgnoreCase) && string.Equals(value.VehicleIdentity, VehicleIdentificationNumber, StringComparison.OrdinalIgnoreCase);
	}

	private Action EnableWriteProgressDisplay()
	{
		bool originalDisplayValue = DisplayProgress;
		string originalMessage = progressLabel.Text;
		progressLabel.Text = Resources.Message_WritingParameter;
		DisplayProgress = true;
		return delegate
		{
			DisplayProgress = originalDisplayValue;
			progressLabel.Text = originalMessage;
		};
	}

	private Action EnableInitializingProgressDisplay()
	{
		bool originalDisplayValue = DisplayProgress;
		string originalMessage = progressLabel.Text;
		progressLabel.Text = string.Empty;
		DisplayProgress = true;
		return delegate
		{
			DisplayProgress = originalDisplayValue;
			progressLabel.Text = originalMessage;
		};
	}

	private Action EnableWaitCursor()
	{
		Cursor originalCursor = ((Control)(object)this).Cursor;
		bool originalWaitSetting = ((Control)this).UseWaitCursor;
		((Control)this).UseWaitCursor = true;
		((Control)(object)this).Cursor = Cursors.WaitCursor;
		return delegate
		{
			((Control)this).UseWaitCursor = originalWaitSetting;
			((Control)(object)this).Cursor = originalCursor;
		};
	}

	private Action DisableActionableControls()
	{
		bool closeButtonSetting = buttonClose.Enabled;
		bool setResetButtonSetting = setResetButton.Enabled;
		buttonClose.Enabled = false;
		setResetButton.Enabled = false;
		return delegate
		{
			buttonClose.Enabled = closeButtonSetting;
			setResetButton.Enabled = setResetButtonSetting;
		};
	}

	private bool WriteUnlock()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		bool flag = true;
		Action action = LockChannel();
		try
		{
			if (PasswordManager.HasPasswords(Channel))
			{
				PasswordManager val = PasswordManager.Create(Channel);
				if (val.Valid)
				{
					try
					{
						bool[] array = val.AcquireRelevantListStatus((ProgressBar)null);
						if (array.Any((bool x) => x))
						{
							LogMessage(Resources.MessageFormat_0IsLocked, Channel.Ecu.Name);
							flag = ((Form)new PasswordEntryDialog(Channel, array, val)).ShowDialog() == DialogResult.OK;
							LogMessage(Resources.MessageFormat_0Was1ByUser, Channel.Ecu.Name, flag ? Resources.Message_Unlocked : Resources.Message_NotUnlocked);
						}
					}
					catch (CaesarException ex)
					{
						flag = false;
						LogMessage(Resources.MessageFormat_FailedUnlocking0Error1, Channel.Ecu.Name, ex.Message);
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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		actionsPanel = new TableLayoutPanel();
		tableLayoutPanel = new TableLayoutPanel();
		setResetButton = new Button();
		progressBar = new ProgressBar();
		progressLabel = new System.Windows.Forms.Label();
		outputLogView = new SeekTimeListView();
		pedalPositionInstrument = new BarInstrument();
		engineSpeedInstrument = new DialInstrument();
		titleLabel = new System.Windows.Forms.Label();
		buttonClose = new Button();
		pedalTypeInstrument = new DigitalReadoutInstrument();
		closeWarningLabel = new System.Windows.Forms.Label();
		currentStatusLabel = new System.Windows.Forms.Label();
		((Control)(object)actionsPanel).SuspendLayout();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(actionsPanel, "actionsPanel");
		((TableLayoutPanel)(object)actionsPanel).Controls.Add(setResetButton, 0, 0);
		((TableLayoutPanel)(object)actionsPanel).Controls.Add(progressBar, 1, 1);
		((TableLayoutPanel)(object)actionsPanel).Controls.Add(progressLabel, 1, 0);
		((Control)(object)actionsPanel).Name = "actionsPanel";
		componentResourceManager.ApplyResources(setResetButton, "setResetButton");
		setResetButton.Name = "setResetButton";
		((TableLayoutPanel)(object)actionsPanel).SetRowSpan((Control)setResetButton, 2);
		setResetButton.UseCompatibleTextRendering = true;
		setResetButton.UseVisualStyleBackColor = true;
		setResetButton.Click += setResetButton_Click;
		componentResourceManager.ApplyResources(progressBar, "progressBar");
		progressBar.Name = "progressBar";
		progressBar.Style = ProgressBarStyle.Marquee;
		componentResourceManager.ApplyResources(progressLabel, "progressLabel");
		progressLabel.Name = "progressLabel";
		progressLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)outputLogView, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)pedalPositionInstrument, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)engineSpeedInstrument, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(titleLabel, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonClose, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)pedalTypeInstrument, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(closeWarningLabel, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(currentStatusLabel, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)actionsPanel, 0, 3);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)outputLogView, 3);
		componentResourceManager.ApplyResources(outputLogView, "outputLogView");
		outputLogView.FilterUserLabels = true;
		((Control)(object)outputLogView).Name = "outputLogView";
		outputLogView.RequiredUserLabelPrefix = "Vim";
		outputLogView.SelectedTime = null;
		outputLogView.ShowChannelLabels = false;
		outputLogView.ShowCommunicationsState = false;
		outputLogView.ShowControlPanel = false;
		outputLogView.ShowDeviceColumn = false;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)pedalPositionInstrument, 3);
		componentResourceManager.ApplyResources(pedalPositionInstrument, "pedalPositionInstrument");
		pedalPositionInstrument.FontGroup = null;
		((SingleInstrumentBase)pedalPositionInstrument).FreezeValue = false;
		((SingleInstrumentBase)pedalPositionInstrument).Instrument = new Qualifier((QualifierTypes)1, "CPC04T", "DT_ASL_Accelerator_Pedal_Position");
		((Control)(object)pedalPositionInstrument).Name = "pedalPositionInstrument";
		((Control)(object)pedalPositionInstrument).TabStop = false;
		((SingleInstrumentBase)pedalPositionInstrument).UnitAlignment = StringAlignment.Near;
		engineSpeedInstrument.AngleRange = 180.0;
		engineSpeedInstrument.AngleStart = -180.0;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)engineSpeedInstrument, 2);
		componentResourceManager.ApplyResources(engineSpeedInstrument, "engineSpeedInstrument");
		engineSpeedInstrument.FontGroup = null;
		((SingleInstrumentBase)engineSpeedInstrument).FreezeValue = false;
		((SingleInstrumentBase)engineSpeedInstrument).Instrument = new Qualifier((QualifierTypes)1, "CPC04T", "DT_ASL_Actual_Engine_Speed");
		((Control)(object)engineSpeedInstrument).Name = "engineSpeedInstrument";
		((TableLayoutPanel)(object)tableLayoutPanel).SetRowSpan((Control)(object)engineSpeedInstrument, 3);
		((Control)(object)engineSpeedInstrument).TabStop = false;
		((Control)(object)engineSpeedInstrument).Tag = "";
		((SingleInstrumentBase)engineSpeedInstrument).UnitAlignment = StringAlignment.Near;
		titleLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(titleLabel, "titleLabel");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)titleLabel, 2);
		titleLabel.Name = "titleLabel";
		titleLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(pedalTypeInstrument, "pedalTypeInstrument");
		pedalTypeInstrument.FontGroup = null;
		((SingleInstrumentBase)pedalTypeInstrument).FreezeValue = false;
		pedalTypeInstrument.Gradient.Initialize((ValueState)4, 8);
		pedalTypeInstrument.Gradient.Modify(1, 0.0, (ValueState)0);
		pedalTypeInstrument.Gradient.Modify(2, 1.0, (ValueState)3);
		pedalTypeInstrument.Gradient.Modify(3, 2.0, (ValueState)3);
		pedalTypeInstrument.Gradient.Modify(4, 3.0, (ValueState)3);
		pedalTypeInstrument.Gradient.Modify(5, 4.0, (ValueState)1);
		pedalTypeInstrument.Gradient.Modify(6, 5.0, (ValueState)3);
		pedalTypeInstrument.Gradient.Modify(7, 6.0, (ValueState)3);
		pedalTypeInstrument.Gradient.Modify(8, 7.0, (ValueState)3);
		((SingleInstrumentBase)pedalTypeInstrument).Instrument = new Qualifier((QualifierTypes)4, "CPC04T", "Accel_Pedal_Type");
		((Control)(object)pedalTypeInstrument).Name = "pedalTypeInstrument";
		((SingleInstrumentBase)pedalTypeInstrument).ShowUnits = false;
		((Control)(object)pedalTypeInstrument).TabStop = false;
		((SingleInstrumentBase)pedalTypeInstrument).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)pedalTypeInstrument).TitleWordWrap = true;
		((SingleInstrumentBase)pedalTypeInstrument).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(closeWarningLabel, "closeWarningLabel");
		closeWarningLabel.BackColor = SystemColors.Info;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)closeWarningLabel, 2);
		closeWarningLabel.Name = "closeWarningLabel";
		closeWarningLabel.UseCompatibleTextRendering = true;
		currentStatusLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(currentStatusLabel, "currentStatusLabel");
		currentStatusLabel.Name = "currentStatusLabel";
		currentStatusLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)actionsPanel).ResumeLayout(performLayout: false);
		((Control)(object)actionsPanel).PerformLayout();
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
