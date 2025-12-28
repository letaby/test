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
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Security.Cryptography;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Radar_Alignment_HSV__NGC_.panel;

public class UserPanel : CustomPanel
{
	private enum LevelState
	{
		Unknown,
		Initializing,
		InvalidLevelType,
		TemporaryLevelSettingActive,
		AlignmentWithTemporaryLevel,
		ValidLevelType
	}

	private class Transaction
	{
		private enum ActionType
		{
			Cleanup,
			Rollback
		}

		private Stack<Tuple<ActionType, Action>> allCleanupActions = new Stack<Tuple<ActionType, Action>>();

		public bool Committed { get; private set; }

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
				Committed = true;
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
				Committed = false;
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

	private LevelState levelState;

	private Dictionary<LevelState, Transaction> pendingTransactions = new Dictionary<LevelState, Transaction>();

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

	private Channel Rdf02tChannel
	{
		get
		{
			if (Rdf02tChannelLocked && !object.Equals(lockedRdf02tChannel, rdf02tChannel))
			{
				throw new InvalidOperationException("The communication RDF02T channel has been lost");
			}
			return rdf02tChannel;
		}
		set
		{
			if (rdf02tChannel != value)
			{
				warningManager.Reset();
				if (rdf02tChannel != null)
				{
					rdf02tChannel.CommunicationsStateUpdateEvent -= Rdf02tChannel_CommunicationsStateUpdateEvent;
					rdf02tChannel.Parameters.ParametersReadCompleteEvent -= Rdf02t_ParametersReadCompleteEvent;
				}
				rdf02tChannel = value;
				if (rdf02tChannel != null)
				{
					rdf02tChannel.CommunicationsStateUpdateEvent += Rdf02tChannel_CommunicationsStateUpdateEvent;
					rdf02tChannel.Parameters.ParametersReadCompleteEvent += Rdf02t_ParametersReadCompleteEvent;
				}
				OnRdf02tChannelChanged();
			}
		}
	}

	private Parameter LevelParameter
	{
		get
		{
			if (Rdf02tChannel == null)
			{
				throw new InvalidOperationException("The RDF02T Channel property cannot be null.");
			}
			return Rdf02tChannel.Parameters["Chassis_Leveling_System_Available_On_Axle"];
		}
	}

	private bool HistoryFileContainedMismatch { get; set; }

	private bool CanClose
	{
		get
		{
			Transaction transaction = GetTransaction(CurrentLevelState);
			bool flag = transaction != null && !transaction.Completed;
			return !sharedProcedureSelection1.AnyProcedureInProgress && (!Rdf02tOnline || (!flag && CurrentLevelState != LevelState.TemporaryLevelSettingActive && CurrentLevelState != LevelState.AlignmentWithTemporaryLevel));
		}
	}

	private string VehicleIdentificationNumber { get; set; }

	private string EngineSerialNumber { get; set; }

	private bool Rdf02tChannelIdle => rdf02tChannel != null && rdf02tChannel.CommunicationsState == CommunicationsState.Online;

	private bool Rdf02tOnline => rdf02tChannel != null && rdf02tChannel.Online;

	private bool Xmc02tOnline => xmc02tChannel != null && xmc02tChannel.Online;

	private LevelState CurrentLevelState
	{
		get
		{
			return levelState;
		}
		set
		{
			if (levelState != value)
			{
				LevelState previousState = levelState;
				levelState = value;
				OnLevelStateChanged(previousState);
			}
		}
	}

	private Transaction CurrentTransaction => GetTransaction(CurrentLevelState) ?? CreateTransaction(CurrentLevelState);

	private bool Rdf02tChannelLocked => lockedRdf02tChannel != null;

	private Choice OriginalLevelTypeValue { get; set; }

	private Choice RadarAlignmentLevelTypeValue
	{
		get
		{
			if (radarAlignmentLevelTypeValue == null)
			{
				radarAlignmentLevelTypeValue = LevelParameter.Choices.GetItemFromRawValue(RadarAlignmentLevelSettingRawValue);
			}
			return radarAlignmentLevelTypeValue;
		}
	}

	public UserPanel()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		InitializeComponent();
		warningManager = new WarningManager(Resources.WarningManagerMessage, Resources.WarningManagerJobName, outputLogView.RequiredUserLabelPrefix);
	}

	private Transaction CreateTransaction(LevelState state)
	{
		Transaction transaction = new Transaction();
		transaction.TransactionCompleted += delegate(object sender, EventArgs args)
		{
			Transaction transaction2 = sender as Transaction;
			if (!transaction2.Committed)
			{
				if (transaction2.Exception != null)
				{
					if (transaction2.Exception is CaesarException || transaction2.Exception is InvalidOperationException)
					{
						LogMessage(Resources.MessageFormat_OperationFailedError0, new object[1] { transaction2.Exception.Message });
					}
					else
					{
						if (!(transaction2.Exception is OperationCanceledException))
						{
							throw transaction2.Exception;
						}
						LogMessage(transaction2.Exception.Message, (LabelStyle)0);
					}
					if (state != LevelState.Unknown && state != LevelState.Initializing)
					{
						((Control)this).BeginInvoke((Delegate)(Action)delegate
						{
							InitializeVehicleDataAsync();
						});
					}
				}
				if (state == LevelState.Initializing)
				{
					CurrentLevelState = LevelState.Unknown;
				}
			}
		};
		pendingTransactions.Add(state, transaction);
		return transaction;
	}

	private Transaction GetTransaction(LevelState state)
	{
		return pendingTransactions.ContainsKey(state) ? pendingTransactions[state] : null;
	}

	private void CloseTransaction(LevelState state)
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

	private void OnLevelStateChanged(LevelState previousState)
	{
		CloseTransaction(previousState);
		switch (CurrentLevelState)
		{
		case LevelState.InvalidLevelType:
			if (previousState == LevelState.TemporaryLevelSettingActive)
			{
				LogMessage(Resources.MessageFormat_0ParameterRestoredBackTo1, LevelParameter.Name, OriginalLevelTypeValue);
			}
			else
			{
				LogMessage(Resources.MessageFormat_0ParameterValue1MustBeChangedTo2, LevelParameter.Name, OriginalLevelTypeValue, RadarAlignmentLevelTypeValue);
			}
			break;
		case LevelState.Initializing:
			if (previousState == LevelState.TemporaryLevelSettingActive && !Rdf02tOnline)
			{
				LogMessage(Resources.MessageFormat_UnableToRestoreTheParameter0BackTo1Because2IsDisconnected, LevelParameter.Name, OriginalLevelTypeValue, "RDF02T");
			}
			break;
		case LevelState.TemporaryLevelSettingActive:
		case LevelState.AlignmentWithTemporaryLevel:
			if (previousState == LevelState.InvalidLevelType)
			{
				LogMessage(Resources.MessageFormat_0ParameterChangedFrom1To2, LevelParameter.Name, OriginalLevelTypeValue, RadarAlignmentLevelTypeValue);
			}
			else
			{
				LogMessage(Resources.MessageFormat_0ParameterWasPreviouslyChangedFrom1To2, LevelParameter.Name, OriginalLevelTypeValue, RadarAlignmentLevelTypeValue);
			}
			break;
		case LevelState.ValidLevelType:
			LogMessage(Resources.MessageFormat_0Parameter1AllowsAlignment, LevelParameter.Name, OriginalLevelTypeValue);
			break;
		case LevelState.Unknown:
			if ((previousState == LevelState.TemporaryLevelSettingActive || previousState == LevelState.AlignmentWithTemporaryLevel) && !Rdf02tOnline)
			{
				LogMessage(Resources.MessageFormat_UnableToRestoreTheParameter0BackTo1Because2IsDisconnected, LevelParameter.Name, OriginalLevelTypeValue, "RDF02T");
			}
			OriginalLevelTypeValue = null;
			HistoryFileContainedMismatch = false;
			EngineSerialNumber = string.Empty;
			VehicleIdentificationNumber = string.Empty;
			break;
		}
		UpdateUserInterface();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += ParentForm_FormClosing;
		if (sharedProcedureSelection1.SelectedProcedure != null)
		{
			sharedProcedureSelection1.SelectedProcedure.EnabledChanged += OnSharedProcedureEnabledChanged;
		}
		((CustomPanel)this).OnChannelsChanged();
		ReadParameters();
		UpdateUserInterface();
	}

	public override void OnChannelsChanged()
	{
		if (!closing)
		{
			Rdf02tChannel = ((CustomPanel)this).GetChannel("RDF02T");
			xmc02tChannel = ((CustomPanel)this).GetChannel("XMC02T");
		}
		((CustomPanel)this).OnChannelsChanged();
	}

	private void OnRdf02tChannelChanged()
	{
		if (!closing)
		{
			CurrentLevelState = LevelState.Unknown;
			InitializeVehicleDataAsync();
		}
	}

	private Action LockRdf02tChannel()
	{
		Channel originalTarget = lockedRdf02tChannel;
		lockedRdf02tChannel = Rdf02tChannel;
		return delegate
		{
			lockedRdf02tChannel = originalTarget;
		};
	}

	private void InitializeVehicleDataAsync()
	{
		CurrentLevelState = LevelState.Initializing;
		if (Rdf02tChannelIdle)
		{
			try
			{
				CurrentTransaction.AddCleanup(LockRdf02tChannel());
				HistoryFileContainedMismatch = false;
				EngineSerialNumber = SapiManager.GetEngineSerialNumber(Rdf02tChannel);
				VehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(Rdf02tChannel);
				if (sharedProcedureSelection1.AnyProcedureInProgress)
				{
					sharedProcedureSelection1.SelectedProcedure.StopComplete += OnProcedureStopDuringInitialize;
					sharedProcedureSelection1.StopSelectedProcedure();
				}
				else
				{
					InitializeLevelTypeAsync();
				}
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

	private void OnProcedureStopDuringInitialize(object sender, PassFailResultEventArgs e)
	{
		sharedProcedureSelection1.SelectedProcedure.StopComplete -= OnProcedureStopDuringInitialize;
		if (Rdf02tChannelIdle)
		{
			try
			{
				InitializeLevelTypeAsync();
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

	private void Rdf02tChannel_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		if (e.CommunicationsState == CommunicationsState.Online && CurrentLevelState == LevelState.Unknown)
		{
			InitializeVehicleDataAsync();
		}
		else if (e.CommunicationsState == CommunicationsState.Disconnecting)
		{
			CurrentLevelState = LevelState.Unknown;
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
			closing = true;
			Rdf02tChannel = null;
			((ContainerControl)this).ParentForm.FormClosing -= ParentForm_FormClosing;
			if (sharedProcedureSelection1.SelectedProcedure != null)
			{
				sharedProcedureSelection1.SelectedProcedure.EnabledChanged -= OnSharedProcedureEnabledChanged;
			}
		}
	}

	private void LogMessage(string message, LabelStyle style = (LabelStyle)0)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		((CustomPanel)this).LabelLog(outputLogView.RequiredUserLabelPrefix, message, style);
	}

	private void LogMessage(string message, params object[] args)
	{
		((CustomPanel)this).LabelLog(outputLogView.RequiredUserLabelPrefix, string.Format(CultureInfo.InvariantCulture, message, args));
	}

	private void buttonStartStop_Click(object sender, EventArgs e)
	{
		switch (CurrentLevelState)
		{
		case LevelState.InvalidLevelType:
			EnableRadarAlignmentLevelSupportAsync();
			break;
		case LevelState.ValidLevelType:
			RunSharedProcedure();
			break;
		case LevelState.TemporaryLevelSettingActive:
			RestoreOriginalLevelTypeAsync();
			break;
		case LevelState.AlignmentWithTemporaryLevel:
			RunSharedProcedure();
			break;
		}
	}

	private void RunSharedProcedure()
	{
		if (CurrentLevelState == LevelState.AlignmentWithTemporaryLevel)
		{
			try
			{
				if (sharedProcedureSelection1.CanStartSelectedProcedure)
				{
					CurrentTransaction.AddCleanup(SharedProcedureCleanup());
					sharedProcedureSelection1.StartSelectedProcedure();
				}
				else if (sharedProcedureSelection1.CanStopSelectedProcedure)
				{
					buttonStartStop.Text = Resources.Message_Start;
					buttonStartStop.Enabled = false;
					sharedProcedureSelection1.StopSelectedProcedure();
				}
				else
				{
					CurrentTransaction.Rollback();
				}
				return;
			}
			catch (Exception exception)
			{
				CurrentTransaction.Rollback(exception);
				return;
			}
		}
		if (CurrentLevelState != LevelState.ValidLevelType)
		{
			return;
		}
		if (sharedProcedureSelection1.CanStartSelectedProcedure)
		{
			if (warningManager.RequestContinue())
			{
				buttonStartStop.Text = Resources.Message_Stop;
				buttonStartStop.Enabled = false;
				sharedProcedureSelection1.SelectedProcedure.StartComplete += OnProcedureStart;
				sharedProcedureSelection1.StartSelectedProcedure();
			}
		}
		else if (sharedProcedureSelection1.CanStopSelectedProcedure)
		{
			buttonStartStop.Text = Resources.Message_Start;
			buttonStartStop.Enabled = false;
			sharedProcedureSelection1.SelectedProcedure.StopComplete += OnProcedureStop;
			sharedProcedureSelection1.StopSelectedProcedure();
		}
	}

	private Action SharedProcedureCleanup()
	{
		string originalText = buttonStartStop.Text;
		bool originalEnabled = buttonStartStop.Enabled;
		buttonStartStop.Text = Resources.Message_Stop;
		buttonStartStop.Enabled = false;
		sharedProcedureSelection1.SelectedProcedure.StartComplete += OnProcedureStart;
		sharedProcedureSelection1.SelectedProcedure.StopComplete += OnProcedureStop;
		return delegate
		{
			buttonStartStop.Text = originalText;
			buttonStartStop.Enabled = originalEnabled;
			sharedProcedureSelection1.SelectedProcedure.StartComplete -= OnProcedureStart;
			sharedProcedureSelection1.SelectedProcedure.StopComplete -= OnProcedureStop;
		};
	}

	private void EnableRadarAlignmentLevelSupportAsync()
	{
		if (CurrentLevelState != LevelState.TemporaryLevelSettingActive && sharedProcedureSelection1.CanStartSelectedProcedure && warningManager.RequestContinue())
		{
			try
			{
				PrepareForLevelWriteOperation();
				SaveSnapshotOfParameter();
				WriteLevelParameterAsync(RadarAlignmentLevelTypeValue);
			}
			catch (Exception exception)
			{
				CurrentTransaction.Rollback(exception);
			}
		}
	}

	private void RestoreOriginalLevelTypeAsync()
	{
		if (CurrentLevelState == LevelState.TemporaryLevelSettingActive || CurrentLevelState == LevelState.AlignmentWithTemporaryLevel)
		{
			try
			{
				PrepareForLevelWriteOperation();
				WriteLevelParameterAsync(OriginalLevelTypeValue);
			}
			catch (Exception exception)
			{
				CurrentTransaction.Rollback(exception);
			}
		}
	}

	private void PrepareForLevelWriteOperation()
	{
		CurrentTransaction.AddCleanup(EnableWaitCursor());
		CurrentTransaction.AddCleanup(DisableActionableControls());
		CurrentTransaction.AddCleanup(LockRdf02tChannel());
	}

	private void WriteLevelParameterAsync(Choice value)
	{
		CurrentTransaction.AddCleanup(LockRdf02tChannel());
		object value2 = LevelParameter.Value;
		LevelParameter.Value = value;
		Rdf02tWriteUnlock();
		Rdf02tChannel.Parameters.ParametersWriteCompleteEvent += LevelParameter_WriteCompleteEvent;
		Rdf02tChannel.Parameters.Write(synchronous: false);
	}

	private void LevelParameter_WriteCompleteEvent(object sender, ResultEventArgs e)
	{
		ParameterCollection parameterCollection = sender as ParameterCollection;
		parameterCollection.ParametersWriteCompleteEvent -= LevelParameter_WriteCompleteEvent;
		if (e.Succeeded)
		{
			CurrentTransaction.Commit();
			if (parameterCollection["Chassis_Leveling_System_Available_On_Axle"].Value == RadarAlignmentLevelTypeValue)
			{
				CurrentLevelState = LevelState.AlignmentWithTemporaryLevel;
				RunSharedProcedure();
			}
			else
			{
				CurrentLevelState = LevelState.InvalidLevelType;
			}
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
			string settingsFileName = Utility.GetSettingsFileName(Rdf02tChannel, "ECUREAD", DateTime.Now, (SettingsFileFormat)2);
			ServerDataManager.SaveSettings(Rdf02tChannel.Parameters, Directories.DrumrollHistoryData, settingsFileName, ParameterFileFormat.VerFile);
		}
	}

	private void UpdateUserInterface()
	{
		switch (CurrentLevelState)
		{
		case LevelState.InvalidLevelType:
			if (!Xmc02tOnline)
			{
				currentStatusLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.Message_XMC);
			}
			else
			{
				currentStatusLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_When0Selected1WillBe2, Resources.Message_Start, LevelParameter.Name, RadarAlignmentLevelTypeValue) + "\n" + string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_AfterSelectingStartDoNotDisconnectUntilComplete, LevelParameter.Name, OriginalLevelTypeValue);
			}
			buttonStartStop.Text = Resources.Message_Start;
			buttonStartStop.Visible = true;
			buttonStartStop.Enabled = sharedProcedureSelection1.CanStartSelectedProcedure;
			break;
		case LevelState.TemporaryLevelSettingActive:
			currentStatusLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_ResetLevelParameterToCorrectState, LevelParameter.Name, OriginalLevelTypeValue, Resources.Message_Restore);
			buttonStartStop.Text = Resources.Message_Restore;
			buttonStartStop.Visible = true;
			break;
		case LevelState.AlignmentWithTemporaryLevel:
			currentStatusLabel.Text = Resources.Message_Drive + "\n\n" + string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_AfterSelectingStartDoNotDisconnectUntilComplete, LevelParameter.Name, OriginalLevelTypeValue);
			break;
		case LevelState.ValidLevelType:
			currentStatusLabel.Text = Resources.Message_NoParameterChangesAreNeededToAlignRadar + "\n\n" + Resources.Message_Drive;
			buttonStartStop.Visible = true;
			break;
		case LevelState.Initializing:
			currentStatusLabel.Text = Resources.Message_Initializing;
			buttonStartStop.Visible = false;
			break;
		case LevelState.Unknown:
			currentStatusLabel.Text = (Rdf02tOnline ? Resources.Message_Initializing : string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_The0IsNoLongerOnlinePleaseReconnectTheECUBeforeContinuing, "RDF02T"));
			buttonStartStop.Visible = false;
			break;
		}
	}

	private void InitializeLevelTypeAsync()
	{
		if (Rdf02tChannelIdle)
		{
			CurrentTransaction.AddCleanup(LockRdf02tChannel());
			Rdf02tChannel.Parameters.ParametersReadCompleteEvent += LevelType_ReadCompleteEvent;
			Rdf02tChannel.Parameters.ReadGroup(LevelParameter.GroupQualifier, fromCache: false, synchronous: false);
		}
		else
		{
			CurrentTransaction.Rollback();
		}
	}

	private void LevelType_ReadCompleteEvent(object sender, ResultEventArgs args)
	{
		ParameterCollection parameterCollection = sender as ParameterCollection;
		parameterCollection.ParametersReadCompleteEvent -= LevelType_ReadCompleteEvent;
		LevelState levelState = LevelState.Unknown;
		try
		{
			if (!args.Succeeded || args.Exception != null)
			{
				CurrentTransaction.Rollback(args.Exception);
				return;
			}
			OriginalLevelTypeValue = parameterCollection["Chassis_Leveling_System_Available_On_Axle"].Value as Choice;
			if (OriginalLevelTypeValue == RadarAlignmentLevelTypeValue)
			{
				Choice choice = FindLevelTypeFromHistory();
				if (choice != null)
				{
					LogMessage(Resources.MessageFormat_Parameter01LocatedInHistory, LevelParameter.Name, choice);
					OriginalLevelTypeValue = choice;
					HistoryFileContainedMismatch = true;
					levelState = LevelState.TemporaryLevelSettingActive;
				}
				else
				{
					levelState = LevelState.ValidLevelType;
				}
			}
			else
			{
				levelState = LevelState.InvalidLevelType;
			}
			CurrentTransaction.Commit();
			CurrentLevelState = levelState;
		}
		catch (Exception exception)
		{
			CurrentTransaction.Rollback(exception);
		}
	}

	private Choice ReadLevelType(string filename)
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
					Match match = levelValueRegex.Match(streamReader.ReadLine());
					if (match.Success && int.TryParse(match.Groups["LevelValue"].Value, out var result))
					{
						choice = LevelParameter.Choices.GetItemFromRawValue(result);
					}
				}
			}
		}
		catch (InvalidChecksumException)
		{
		}
		return choice;
	}

	private Choice FindLevelTypeFromHistory()
	{
		IEnumerable<string> enumerable = from filePath in Directory.EnumerateFiles(Directories.DrumrollHistoryData)
			let fileInfo = FileNameInformation.FromName(FileEncryptionProvider.DecryptFileName(Path.GetFileName(filePath)), (FileType)2)
			where fileInfo.Valid && MatchesEcu(fileInfo) && MatchesCurrentVehicle(fileInfo)
			orderby Utility.TimeFromString(fileInfo.Timestamp) descending
			select filePath;
		Choice result = null;
		foreach (string item in enumerable)
		{
			Choice choice = ReadLevelType(item);
			if (choice != null && choice != RadarAlignmentLevelTypeValue)
			{
				result = choice;
			}
		}
		return result;
	}

	private static bool MatchesEcu(FileNameInformation value)
	{
		return value != null && string.Equals(value.Device, "RDF02T", StringComparison.OrdinalIgnoreCase);
	}

	private bool MatchesCurrentVehicle(FileNameInformation value)
	{
		bool result = false;
		if (value != null)
		{
			result = string.Equals(value.VehicleIdentity, VehicleIdentificationNumber, StringComparison.OrdinalIgnoreCase);
		}
		return result;
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
		bool buttonStartStopSetting = buttonStartStop.Enabled;
		buttonStartStop.Enabled = false;
		return delegate
		{
			buttonStartStop.Enabled = buttonStartStopSetting;
		};
	}

	private void Rdf02tWriteUnlock()
	{
		Action action = LockRdf02tChannel();
		action();
	}

	private void ReadParameters()
	{
		if (Rdf02tChannel != null && Rdf02tChannel.CommunicationsState == CommunicationsState.Online && Rdf02tChannel.Parameters["VertPos"] != null && !Rdf02tChannel.Parameters["VertPos"].HasBeenReadFromEcu)
		{
			Rdf02tChannel.Parameters.ReadGroup(Rdf02tChannel.Parameters["VertPos"].GroupQualifier, fromCache: true, synchronous: false);
		}
	}

	private void Rdf02t_ParametersReadCompleteEvent(object sender, ResultEventArgs result)
	{
		ReadParameters();
	}

	private void OnProcedureStart(object sender, PassFailResultEventArgs e)
	{
		buttonStartStop.Enabled = true;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.ModifyState(1, (ValueState)3);
			((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.ModifyState(3, (ValueState)1);
			((Control)(object)dialInstrumentVehicleSpeed).Invalidate();
		}
	}

	private void OnProcedureStop(object sender, PassFailResultEventArgs e)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Invalid comparison between Unknown and I4
		sharedProcedureSelection1.SelectedProcedure.StopComplete -= OnProcedureStop;
		sharedProcedureSelection1.SelectedProcedure.StartComplete -= OnProcedureStart;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			if ((int)e.Result == 1)
			{
				LogMessage(Resources.Message_CalibrationComplete, (LabelStyle)1);
			}
		}
		else
		{
			LogMessage(Resources.MessageFormat_CalibrationError0, new object[1] { ((ResultEventArgs)(object)e).Exception.Message });
		}
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.ModifyState(1, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.ModifyState(3, (ValueState)3);
		((Control)(object)dialInstrumentVehicleSpeed).Invalidate();
		if (CurrentLevelState == LevelState.AlignmentWithTemporaryLevel)
		{
			CurrentTransaction.Commit();
			CurrentLevelState = LevelState.TemporaryLevelSettingActive;
			RestoreOriginalLevelTypeAsync();
		}
		else if (CurrentLevelState == LevelState.ValidLevelType)
		{
			buttonStartStop.Text = Resources.Message_Start;
			buttonStartStop.Enabled = true;
		}
	}

	private void OnSharedProcedureEnabledChanged(object sender, EventArgs e)
	{
		if (CurrentLevelState == LevelState.InvalidLevelType)
		{
			UpdateUserInterface();
		}
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_088d: Expected O, but got Unknown
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_09be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		labelStatus = new System.Windows.Forms.Label();
		checkmark1 = new Checkmark();
		barInstrumentProcedureProgress = new BarInstrument();
		buttonStartStop = new Button();
		outputLogView = new SeekTimeListView();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVertPos = new DigitalReadoutInstrument();
		currentStatusLabel = new System.Windows.Forms.Label();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		dialInstrumentVehicleSpeed = new DialInstrument();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmark1, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)barInstrumentProcedureProgress, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStartStop, 4, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)outputLogView, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentVertPos, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(currentStatusLabel, 2, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelection1, 3, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)dialInstrumentVehicleSpeed, 0, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelStatus, 2);
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)barInstrumentProcedureProgress, 2);
		componentResourceManager.ApplyResources(barInstrumentProcedureProgress, "barInstrumentProcedureProgress");
		barInstrumentProcedureProgress.FontGroup = null;
		((SingleInstrumentBase)barInstrumentProcedureProgress).FreezeValue = false;
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.Initialize((ValueState)0, 2, "%");
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.Modify(1, 0.0, (ValueState)1);
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).Gradient.Modify(2, 101.0, (ValueState)0);
		((SingleInstrumentBase)barInstrumentProcedureProgress).Instrument = new Qualifier((QualifierTypes)1, "RDF02T", "DT_Service_Justage_Progress_service_justage_progress");
		((Control)(object)barInstrumentProcedureProgress).Name = "barInstrumentProcedureProgress";
		((AxisSingleInstrumentBase)barInstrumentProcedureProgress).PreferredAxisRange = new AxisRange(0.0, 100.0, "%");
		((SingleInstrumentBase)barInstrumentProcedureProgress).ShowValueReadout = false;
		((SingleInstrumentBase)barInstrumentProcedureProgress).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonStartStop, "buttonStartStop");
		buttonStartStop.Name = "buttonStartStop";
		buttonStartStop.UseCompatibleTextRendering = true;
		buttonStartStop.UseVisualStyleBackColor = true;
		buttonStartStop.Click += buttonStartStop_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)outputLogView, 3);
		componentResourceManager.ApplyResources(outputLogView, "outputLogView");
		outputLogView.FilterUserLabels = true;
		((Control)(object)outputLogView).Name = "outputLogView";
		outputLogView.RequiredUserLabelPrefix = "Detroit Assurance Driving Radar Alignment NGC";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)outputLogView, 4);
		outputLogView.SelectedTime = null;
		outputLogView.ShowChannelLabels = false;
		outputLogView.ShowCommunicationsState = false;
		outputLogView.ShowControlPanel = false;
		outputLogView.ShowDeviceColumn = false;
		outputLogView.TimeFormat = "HH:mm:ss.f";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument2.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument2.Gradient.Modify(3, 8.0, (ValueState)1);
		digitalReadoutInstrument2.Gradient.Modify(4, 9.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)32, "RDF02T", "0FFFE9");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument3, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		digitalReadoutInstrument3.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument3.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument3.Gradient.Modify(2, 1.0, (ValueState)3);
		digitalReadoutInstrument3.Gradient.Modify(3, 8.0, (ValueState)1);
		digitalReadoutInstrument3.Gradient.Modify(4, 9.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)32, "RDF02T", "0FFFF3");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentVertPos, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVertPos, "digitalReadoutInstrumentVertPos");
		digitalReadoutInstrumentVertPos.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentVertPos).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentVertPos).Instrument = new Qualifier((QualifierTypes)4, "RDF02T", "VertPos");
		((Control)(object)digitalReadoutInstrumentVertPos).Name = "digitalReadoutInstrumentVertPos";
		((SingleInstrumentBase)digitalReadoutInstrumentVertPos).UnitAlignment = StringAlignment.Near;
		currentStatusLabel.AutoEllipsis = true;
		componentResourceManager.ApplyResources(currentStatusLabel, "currentStatusLabel");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)currentStatusLabel, 3);
		currentStatusLabel.Name = "currentStatusLabel";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)currentStatusLabel, 2);
		currentStatusLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_DrivingRadarAlignment_NGC_HSV" });
		((Control)(object)sharedProcedureSelection1).Tag = "";
		dialInstrumentVehicleSpeed.AngleRange = 270.0;
		dialInstrumentVehicleSpeed.AngleStart = 135.0;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)dialInstrumentVehicleSpeed, 2);
		componentResourceManager.ApplyResources(dialInstrumentVehicleSpeed, "dialInstrumentVehicleSpeed");
		dialInstrumentVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).FreezeValue = false;
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Initialize((ValueState)0, 3, "km/h");
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(1, -1.0, (ValueState)1);
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(2, 1.0, (ValueState)3);
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).Gradient.Modify(3, 40.0, (ValueState)3);
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)dialInstrumentVehicleSpeed).Name = "dialInstrumentVehicleSpeed";
		((AxisSingleInstrumentBase)dialInstrumentVehicleSpeed).PreferredAxisRange = new AxisRange(0.0, 90.0, "mph");
		((SingleInstrumentBase)dialInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = null;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Radar_Alignment");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
