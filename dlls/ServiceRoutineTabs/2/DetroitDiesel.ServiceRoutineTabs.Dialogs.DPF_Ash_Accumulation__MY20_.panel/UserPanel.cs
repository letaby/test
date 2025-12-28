using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__MY20_.panel;

public class UserPanel : CustomPanel
{
	private class ValidationInformation
	{
		public readonly AtsType OneBoxType;

		public readonly Regex Regex;

		public ValidationInformation(AtsType oneBoxType, Regex regex)
		{
			OneBoxType = oneBoxType;
			Regex = regex;
		}
	}

	private enum Stage
	{
		Idle = 0,
		GetConfirmation = 1,
		GetValue = 2,
		WaitingForMCM21TRead = 3,
		WriteValue = 4,
		WaitingForWrite = 5,
		WaitingToConfirmChange = 6,
		WaitingForMCM21TInputToUpdate = 7,
		ResetMCM21T = 8,
		WaitingForMCM21TReset = 9,
		CommitChangesToMCM21T = 10,
		WaitingForMCM21TCommit = 11,
		Finish = 12,
		Stopping = 13,
		_Start = 1
	}

	private enum Reason
	{
		Success,
		FailedServiceExecute,
		FailedService,
		FailedWrite,
		FailedCommit,
		Closing,
		Disconnected,
		Canceled
	}

	private const string DpfAshVolumeSet = "RT_SR08B_DPF_ash_volume_ratio_update_Start";

	private const string DpfAshVolumeRead = "RT_SR08B_DPF_ash_volume_ratio_update_Request_Results_E2P_DPF_ASH_VOL_ACM";

	private const string ACMAshVolumeRatioUpdateStart = "RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction";

	private const string QualifierOdometer = "CO_Odometer";

	private const string QualifierEngineHours = "DT_AS045_Engine_Operating_Hours";

	private static readonly ValidationInformation HeavyDutySerialNumberValidation = new ValidationInformation(AtsType.TwoBox, new Regex("[A-Za-z0-9]{3,}", RegexOptions.Compiled));

	private static readonly ValidationInformation MediumDutySerialNumberValidation = new ValidationInformation(AtsType.OneBoxOneFilter, new Regex("[A-Za-z0-9]{3,}", RegexOptions.Compiled));

	private Dictionary<string, ValidationInformation> serialNumberValidations = new Dictionary<string, ValidationInformation>();

	private ValidationInformation connectedValidationInformation;

	private Channel acm;

	private Channel mcm;

	private static readonly Qualifier ATDTypeParameter = new Qualifier((QualifierTypes)4, "ACM301T", "ATD_Hardware_Type");

	private ParameterDataItem atdType;

	private static readonly Qualifier AshRatioInstrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS109_Ash_Filter_Full_Volume");

	private InstrumentDataItem ashRatioInstrument;

	private string targetVIN;

	private bool adrReturnValue = false;

	private static readonly Regex AnySerialNumberValidation = new Regex("[R\\d]", RegexOptions.Compiled);

	private static readonly Regex MdegFullScopeSerialNumberValidation = new Regex("[A-Z0-9]", RegexOptions.Compiled);

	private Stage currentStage = Stage.Idle;

	private object oldValue;

	private string valueToWrite;

	private string dpfSN1;

	private string instrumentRatioAtStart;

	private DateTime timeAtStart;

	private Timer verificationTimeoutTimer = new Timer();

	private static readonly TimeSpan VerificationTimeoutPeriod = new TimeSpan(0, 0, 10);

	private Service currentService;

	private Service lastRunService;

	private ScalingLabel titleLabel;

	private DigitalReadoutInstrument cpcReadout;

	private DigitalReadoutInstrument acmReadout;

	private System.Windows.Forms.Label labelMCM;

	private System.Windows.Forms.Label labelACM;

	private System.Windows.Forms.Label labelTaskQuestion;

	private RadioButton radioCleanRemanFilter;

	private RadioButton radioNewFilter;

	private RadioButton radioACMReplace;

	private TableLayoutPanel tableLayoutDPFSerialNumber;

	private TextBox textBoxDPFSerialNumber1;

	private System.Windows.Forms.Label labelProgress;

	private TextBox textBoxProgress;

	private Button buttonPerformAction;

	private System.Windows.Forms.Label labelSNErrorMessage1;

	private System.Windows.Forms.Label labelWarning;

	private Button buttonClose;

	private FlowLayoutPanel flowLayoutPanel1;

	private System.Windows.Forms.Label labelLicenseMessage;

	private System.Windows.Forms.Label labelDPFSerialNumberHeader;

	private TableLayoutPanel tableLayoutPanel;

	private AtsType AtsType
	{
		get
		{
			if (acm != null && connectedValidationInformation != null)
			{
				if (atdType == null)
				{
					return connectedValidationInformation.OneBoxType;
				}
				Choice choice = atdType.OriginalValue as Choice;
				if (choice != null)
				{
					return Convert.ToByte(choice.RawValue) switch
					{
						0 => connectedValidationInformation.OneBoxType, 
						1 => AtsType.TwoBox, 
						_ => AtsType.Unknown, 
					};
				}
			}
			return AtsType.Unknown;
		}
	}

	public bool Online => IsChannelOnline(mcm) && IsChannelOnline(acm);

	public bool Working => currentStage != Stage.Idle;

	public bool CanClose => !Working;

	public bool MdegFullScope
	{
		get
		{
			string currentEngineSerialNumber = SapiManager.GlobalInstance.CurrentEngineSerialNumber;
			bool flag = !string.IsNullOrEmpty(currentEngineSerialNumber) && currentEngineSerialNumber.StartsWith("934912C");
			return !flag;
		}
	}

	public bool CanSetAshRatio
	{
		get
		{
			bool result = false;
			if (!Working && Online)
			{
				result = ((!radioACMReplace.Checked) ? (mcm.Services["RT_SR08B_DPF_ash_volume_ratio_update_Start"] != null && ValidSerialNumberProvided) : (mcm.Services["RT_SR08B_DPF_ash_volume_ratio_update_Request_Results_E2P_DPF_ASH_VOL_ACM"] != null));
				result = result && ashRatioInstrument != null && acm.Services["RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction"] != null;
			}
			return result;
		}
	}

	public bool ValidSerialNumberProvided
	{
		get
		{
			switch (AtsType)
			{
			case AtsType.OneBoxOneFilter:
			case AtsType.TwoBox:
			{
				string errorText;
				return ValidateSerialNumber(textBoxDPFSerialNumber1.Text, out errorText);
			}
			default:
				return false;
			}
		}
	}

	private bool IsLicenseValid => LicenseManager.GlobalInstance.AccessLevel >= 1;

	private Stage CurrentStage
	{
		get
		{
			return currentStage;
		}
		set
		{
			if (currentStage != value)
			{
				currentStage = value;
				UpdateUserInterface();
				Application.DoEvents();
			}
		}
	}

	public UserPanel()
	{
		InitializeComponent();
		serialNumberValidations.Add("DD13", HeavyDutySerialNumberValidation);
		serialNumberValidations.Add("DD15", HeavyDutySerialNumberValidation);
		serialNumberValidations.Add("DD16", HeavyDutySerialNumberValidation);
		serialNumberValidations.Add("DD5", MediumDutySerialNumberValidation);
		serialNumberValidations.Add("DD8", MediumDutySerialNumberValidation);
		radioCleanRemanFilter.Checked = true;
		radioCleanRemanFilter.CheckedChanged += OnReasonChanged;
		radioNewFilter.CheckedChanged += OnReasonChanged;
		radioACMReplace.CheckedChanged += OnReasonChanged;
		textBoxDPFSerialNumber1.TextChanged += OnDPFSerialNumberChanged;
		textBoxDPFSerialNumber1.KeyPress += OnDPFSerialNumberKeyPress;
		buttonPerformAction.Click += OnPerformAction;
		verificationTimeoutTimer.Interval = (int)(VerificationTimeoutPeriod.TotalMilliseconds / 2.0);
		Timer timer = verificationTimeoutTimer;
		EventHandler value = delegate
		{
			if (CurrentStage == Stage.WaitingToConfirmChange || CurrentStage == Stage.WaitingForMCM21TInputToUpdate)
			{
				PerformCurrentStage();
			}
		};
		timer.Tick += value;
		verificationTimeoutTimer.Enabled = false;
		SapiManager.GlobalInstance.EquipmentTypeChanged += GlobalInstance_EquipmentTypeChanged;
	}

	public override void OnChannelsChanged()
	{
		UpdateChannels();
		UpdateConnectedEquipmentType();
		UpdateUserInterface();
	}

	private void UpdateChannels()
	{
		if (SetMCM(((CustomPanel)this).GetChannel("MCM21T")) | SetACM(((CustomPanel)this).GetChannel("ACM301T")))
		{
			UpdateWarningMessage();
			textBoxDPFSerialNumber1.Text = string.Empty;
		}
	}

	private void CleanUpChannels()
	{
		SetMCM(null);
		SetACM(null);
		UpdateWarningMessage();
	}

	private void UpdateWarningMessage()
	{
		bool visible = false;
		if (IsLicenseValid)
		{
			if (acm != null && HasUnsentChanges(acm))
			{
				visible = true;
			}
			labelLicenseMessage.Visible = false;
		}
		else
		{
			labelLicenseMessage.Visible = true;
		}
		labelWarning.Visible = visible;
	}

	private static bool HasUnsentChanges(Channel channel)
	{
		bool result = false;
		foreach (Parameter parameter in channel.Parameters)
		{
			if (!object.Equals(parameter.Value, parameter.OriginalValue))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private bool SetMCM(Channel mcm)
	{
		bool result = false;
		if (this.mcm != mcm)
		{
			StopWork(Reason.Disconnected);
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
				targetVIN = string.Empty;
			}
			this.mcm = mcm;
			result = true;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
		}
		return result;
	}

	private bool SetACM(Channel acm)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (this.acm != acm)
		{
			StopWork(Reason.Disconnected);
			if (this.acm != null)
			{
				this.acm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
				if (ashRatioInstrument != null)
				{
					((DataItem)ashRatioInstrument).UpdateEvent -= OnAshRatioUpdate;
					ashRatioInstrument = null;
				}
				targetVIN = string.Empty;
			}
			this.acm = acm;
			result = true;
			if (this.acm != null)
			{
				this.acm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				ref ParameterDataItem reference = ref atdType;
				DataItem obj = DataItem.Create(ATDTypeParameter, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
				reference = (ParameterDataItem)(object)((obj is ParameterDataItem) ? obj : null);
				ref InstrumentDataItem reference2 = ref ashRatioInstrument;
				DataItem obj2 = DataItem.Create(AshRatioInstrument, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
				reference2 = (InstrumentDataItem)(object)((obj2 is InstrumentDataItem) ? obj2 : null);
				if (ashRatioInstrument != null)
				{
					Service service = this.acm.Services["RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction"];
					if (service != null && service.InputValues[0].Units != ((DataItem)ashRatioInstrument).Units)
					{
						ashRatioInstrument = null;
						MessageBox.Show(Resources.Message_AshVolumeRatioUpdateRoutineUnitsDoNotMatchInstrumentUnitsInvalidCBF);
					}
					else
					{
						((DataItem)ashRatioInstrument).UpdateEvent += OnAshRatioUpdate;
					}
				}
				ReadAccumulators(synchronous: false);
			}
		}
		return result;
	}

	private void OnAshRatioUpdate(object sender, ResultEventArgs args)
	{
		if (CurrentStage == Stage.WaitingToConfirmChange)
		{
			PerformCurrentStage();
		}
	}

	private void ReadAccumulators(bool synchronous)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (acm != null)
		{
			ParameterCollection parameters = acm.Parameters;
			Qualifier aTDTypeParameter = ATDTypeParameter;
			if (parameters[((Qualifier)(ref aTDTypeParameter)).Name] != null)
			{
				ParameterCollection parameters2 = acm.Parameters;
				ParameterCollection parameters3 = acm.Parameters;
				aTDTypeParameter = ATDTypeParameter;
				parameters2.ReadGroup(parameters3[((Qualifier)(ref aTDTypeParameter)).Name].GroupQualifier, fromCache: false, synchronous);
			}
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		UpdateChannels();
		UpdateAccessLevels();
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			StopWork(Reason.Closing);
			CleanUpChannels();
			((Control)this).Tag = new object[2] { adrReturnValue, textBoxProgress.Text };
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnReasonChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnDPFSerialNumberChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnDPFSerialNumberKeyPress(object sender, KeyPressEventArgs e)
	{
		e.KeyChar = e.KeyChar.ToString().ToUpperInvariant()[0];
		if (e.KeyChar != '\b' && (MdegFullScope ? (!MdegFullScopeSerialNumberValidation.IsMatch(e.KeyChar.ToString())) : (!AnySerialNumberValidation.IsMatch(e.KeyChar.ToString()))))
		{
			e.Handled = true;
		}
	}

	private bool ValidateSerialNumber(string text, out string errorText)
	{
		bool result = false;
		errorText = string.Empty;
		if (connectedValidationInformation == null)
		{
			errorText = Resources.Message_UnsupportedEquipment;
		}
		else
		{
			result = connectedValidationInformation.Regex.IsMatch(text);
		}
		return result;
	}

	private void OnPerformAction(object sender, EventArgs e)
	{
		DoWork();
	}

	public bool IsChannelOnline(Channel channel)
	{
		return channel != null && channel.CommunicationsState == CommunicationsState.Online;
	}

	private void UpdateAccessLevels()
	{
		UpdateUserInterface();
		UpdateWarningMessage();
	}

	private void UpdateUserInterface()
	{
		bool flag = Online && !Working && IsLicenseValid && AtsType != AtsType.Unknown && connectedValidationInformation != null;
		RadioButton radioButton = radioCleanRemanFilter;
		bool enabled = (radioNewFilter.Enabled = flag);
		radioButton.Enabled = enabled;
		buttonClose.Enabled = CanClose;
		labelTaskQuestion.Enabled = flag;
		buttonPerformAction.Enabled = CanSetAshRatio && flag;
		if (ApplicationInformation.ProductAccessLevel == 1)
		{
			radioACMReplace.Visible = false;
		}
		else if (ApplicationInformation.ProductAccessLevel == 3 || ApplicationInformation.ProductAccessLevel == 2)
		{
			radioACMReplace.Enabled = flag;
			if (radioACMReplace.Checked)
			{
				((Control)(object)tableLayoutDPFSerialNumber).Visible = false;
			}
			else if (!((Control)(object)tableLayoutDPFSerialNumber).Visible)
			{
				((Control)(object)tableLayoutDPFSerialNumber).Visible = true;
			}
		}
		if (((Control)(object)tableLayoutDPFSerialNumber).Visible)
		{
			textBoxDPFSerialNumber1.ReadOnly = !flag;
			((Control)(object)tableLayoutDPFSerialNumber).Enabled = flag;
			ValidateDPFSerialBox(textBoxDPFSerialNumber1, labelSNErrorMessage1);
			switch (AtsType)
			{
			case AtsType.OneBoxOneFilter:
			case AtsType.TwoBox:
				labelDPFSerialNumberHeader.Text = Resources.Message_PleaseProvideTheSerialNumberForTheAftertreatmentSystemNowInstalledOnTheVehicle;
				break;
			}
		}
	}

	private void ValidateDPFSerialBox(TextBox box, System.Windows.Forms.Label errorMessage)
	{
		string errorText;
		if (box.ReadOnly)
		{
			box.BackColor = SystemColors.Control;
		}
		else if (ValidateSerialNumber(box.Text, out errorText))
		{
			errorMessage.Text = string.Empty;
			box.BackColor = Color.LightGreen;
		}
		else
		{
			errorMessage.Text = errorText;
			box.BackColor = Color.LightPink;
		}
	}

	private void DoWork()
	{
		CurrentStage = Stage.GetConfirmation;
		PerformCurrentStage();
	}

	private void PerformCurrentStage()
	{
		switch (CurrentStage)
		{
		case Stage.Idle:
		case Stage.WaitingForMCM21TInputToUpdate:
			break;
		case Stage.GetConfirmation:
		{
			targetVIN = SapiManager.GetVehicleIdentificationNumber(acm);
			string text;
			if (radioACMReplace.Checked)
			{
				dpfSN1 = string.Empty;
				text = Resources.Message_CopiedFromMCM21T;
			}
			else
			{
				dpfSN1 = textBoxDPFSerialNumber1.Text;
				text = Resources.Message_Reset;
			}
			if (ConfirmationDialog.Show(targetVIN, AtsType, dpfSN1, text))
			{
				ClearOutput();
				Report(Resources.Message_AshVolumeRatioModificationStarted);
				Report(Resources.Message_VIN + targetVIN);
				if (((Control)(object)tableLayoutDPFSerialNumber).Visible)
				{
					Report(Resources.Message_DPFSerialNumber + dpfSN1);
				}
				Report(Resources.Message_AshVolumeRatio + text);
				CurrentStage = Stage.GetValue;
				PerformCurrentStage();
			}
			else
			{
				StopWork(Reason.Canceled);
			}
			break;
		}
		case Stage.GetValue:
			if (radioACMReplace.Checked)
			{
				CurrentStage = Stage.WaitingForMCM21TRead;
				ExecuteService(mcm.Services["RT_SR08B_DPF_ash_volume_ratio_update_Request_Results_E2P_DPF_ASH_VOL_ACM"], synchronous: false);
			}
			else
			{
				CurrentStage = Stage.WriteValue;
				valueToWrite = ((DataItem)ashRatioInstrument).ValueAsString((object)0.0);
				PerformCurrentStage();
			}
			break;
		case Stage.WaitingForMCM21TRead:
			valueToWrite = ((DataItem)ashRatioInstrument).ValueAsString((object)Convert.ToDouble(lastRunService.OutputValues[0].Value));
			CurrentStage = Stage.WriteValue;
			PerformCurrentStage();
			break;
		case Stage.WriteValue:
			if (ashRatioInstrument == null)
			{
				StopWork(Reason.Disconnected);
			}
			else
			{
				instrumentRatioAtStart = ((DataItem)ashRatioInstrument).ValueAsString(((DataItem)ashRatioInstrument).Value);
			}
			if (SetACMAshRatio(valueToWrite))
			{
				CurrentStage = Stage.WaitingForWrite;
			}
			else
			{
				StopWork(Reason.FailedWrite);
			}
			break;
		case Stage.WaitingForWrite:
			CurrentStage = Stage.WaitingToConfirmChange;
			timeAtStart = DateTime.UtcNow;
			verificationTimeoutTimer.Enabled = true;
			PerformCurrentStage();
			break;
		case Stage.WaitingToConfirmChange:
			if (ashRatioInstrument == null)
			{
				StopWork(Reason.Disconnected);
			}
			else if (DateTime.UtcNow - timeAtStart >= VerificationTimeoutPeriod)
			{
				Report((((DataItem)ashRatioInstrument).ValueAsString(((DataItem)ashRatioInstrument).Value) != instrumentRatioAtStart) ? Resources.Message_RatioUpdated : Resources.Message_RatioNotUpdated);
				CurrentStage = Stage.ResetMCM21T;
				verificationTimeoutTimer.Enabled = false;
				PerformCurrentStage();
			}
			break;
		case Stage.ResetMCM21T:
		{
			if (radioACMReplace.Checked)
			{
				CurrentStage = Stage.Finish;
				PerformCurrentStage();
				break;
			}
			CurrentStage = Stage.WaitingForMCM21TReset;
			Service service = mcm.Services["RT_SR08B_DPF_ash_volume_ratio_update_Start"];
			service.InputValues[0].Value = 0;
			ExecuteService(service, synchronous: false);
			break;
		}
		case Stage.WaitingForMCM21TReset:
			CurrentStage = Stage.CommitChangesToMCM21T;
			PerformCurrentStage();
			break;
		case Stage.CommitChangesToMCM21T:
			CommitToMCM21TPermanentMemory();
			CurrentStage = Stage.WaitingForMCM21TCommit;
			break;
		case Stage.WaitingForMCM21TCommit:
			CurrentStage = Stage.Finish;
			PerformCurrentStage();
			break;
		case Stage.Finish:
			StopWork(Reason.Success);
			break;
		case Stage.Stopping:
			break;
		}
	}

	private void CommitToMCM21TPermanentMemory()
	{
		if (mcm.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService"))
		{
			Report(Resources.Message_CommittingChanges);
			mcm.Services.ServiceCompleteEvent += OnCommitCompleteEvent;
			mcm.Services.Execute(mcm.Ecu.Properties["CommitToPermanentMemoryService"], synchronous: false);
		}
		else
		{
			Report(Resources.Message_NoCommitServiceAvailable);
			StopWork(Reason.FailedCommit);
		}
	}

	private void OnCommitCompleteEvent(object sender, ResultEventArgs e)
	{
		mcm.Services.ServiceCompleteEvent -= OnCommitCompleteEvent;
		if (e.Succeeded)
		{
			PerformCurrentStage();
		}
		else
		{
			StopWork(Reason.FailedCommit);
		}
	}

	private void StopWork(Reason reason)
	{
		verificationTimeoutTimer.Enabled = false;
		if (CurrentStage != Stage.Stopping && CurrentStage != Stage.Idle)
		{
			Stage stage = CurrentStage;
			CurrentStage = Stage.Stopping;
			if (reason == Reason.Success)
			{
				AddStationLogEntry(dpfSN1);
				Report(Resources.Message_TheProcedureCompletedSuccessfully);
				adrReturnValue = true;
			}
			else
			{
				adrReturnValue = false;
				Report(Resources.Message_TheProcedureFailedToComplete);
				switch (reason)
				{
				case Reason.Disconnected:
					Report(Resources.Message_OneOrMoreDevicesDisconnected);
					textBoxDPFSerialNumber1.Text = string.Empty;
					break;
				case Reason.FailedServiceExecute:
					switch (stage)
					{
					case Stage.ResetMCM21T:
						Report(Resources.Message_FailedToObtainServiceToResetMCM21T);
						break;
					case Stage.GetValue:
						Report(Resources.Message_FailedToObtainServiceForRetrievingMCM21TValue);
						break;
					}
					break;
				case Reason.FailedService:
					if (stage == Stage.WaitingForMCM21TRead)
					{
						Report(Resources.Message_FailedToExecuteReadOfAshAccumulationDistanceFromMCM21T);
					}
					else
					{
						Report(Resources.Message_FailedToExecuteResetOfAshAccumulationDistanceInMCM21T);
					}
					break;
				case Reason.FailedWrite:
					Report(Resources.Message_FailedToWriteTheAshMileageAccumulator);
					break;
				case Reason.FailedCommit:
					Report(Resources.Message_FailedToCommitTheChangesToTheMCM21TYouMayNeedToRepeatThisProcedure);
					break;
				case Reason.Canceled:
					Report(Resources.Message_TheUserCanceledTheOperation);
					break;
				}
			}
			ClearCurrentService(saveLastRun: false);
			valueToWrite = string.Empty;
			oldValue = null;
			CurrentStage = Stage.Idle;
			ReadAccumulators(synchronous: false);
		}
		UpdateWarningMessage();
	}

	private void AddStationLogEntry(string serialNumber1)
	{
		string empty = string.Empty;
		if (radioCleanRemanFilter.Checked)
		{
			empty = Resources.Message_ReasonCleanRemanFilter;
		}
		else if (radioNewFilter.Checked)
		{
			empty = Resources.Message_ReasonNewFilter;
		}
		else
		{
			if (!radioACMReplace.Checked)
			{
				throw new InvalidOperationException();
			}
			empty = Resources.Message_ReasonACMReplacement;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["reasontext"] = empty;
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		if (oldValue != null)
		{
			dictionary2["Old_AshRatio"] = Convert.ToInt32(((DataItem)ashRatioInstrument).ValueAsDouble(oldValue)).ToString();
		}
		dictionary2["Current_AshRatio"] = Convert.ToInt32(((DataItem)ashRatioInstrument).ValueAsDouble(((DataItem)ashRatioInstrument).Value)).ToString();
		dictionary2["DPF_Serial_Number1"] = (string.IsNullOrEmpty(serialNumber1) ? Resources.Message_NA : serialNumber1);
		dictionary2["CO_Odometer"] = ReadEcuInfoData(((CustomPanel)this).GetChannel("CPC04T", (ChannelLookupOptions)5), "CO_Odometer");
		dictionary2["DT_AS045_Engine_Operating_Hours"] = ReadInstrumentValue(mcm, "DT_AS045_Engine_Operating_Hours");
		ServerDataManager.UpdateEventsFile(acm, (IDictionary<string, string>)dictionary, (IDictionary<string, string>)dictionary2, "DPFAshAccumulation", string.Empty, targetVIN, "OK", "DESCRIPTION", string.Empty);
	}

	private string ReadInstrumentValue(Channel channel, string qualifier)
	{
		string result = string.Empty;
		if (channel != null)
		{
			Instrument instrument = channel.Instruments[qualifier];
			if (instrument != null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
			{
				result = instrument.InstrumentValues.Current.Value.ToString().Trim();
			}
		}
		return result;
	}

	private string ReadEcuInfoData(Channel channel, string qualifier)
	{
		string text = string.Empty;
		if (channel != null)
		{
			EcuInfo ecuInfo = channel.EcuInfos[qualifier];
			if (ecuInfo == null)
			{
				ecuInfo = channel.EcuInfos.GetItemContaining(qualifier);
			}
			if (ecuInfo != null)
			{
				text = ecuInfo.Value.ToString().Trim();
			}
		}
		return text.Trim();
	}

	private void ExecuteService(Service service, bool synchronous)
	{
		if (service == null)
		{
			StopWork(Reason.FailedServiceExecute);
			return;
		}
		if (!synchronous)
		{
			currentService = service;
			currentService.ServiceCompleteEvent += OnServiceComplete;
		}
		Report("Executing " + currentService.Name + "...");
		service.Execute(synchronous);
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		ClearCurrentService(saveLastRun: true);
		if (CheckCompleteResult(e, Resources.Message_ServiceExecuted, Resources.Message_ServiceError))
		{
			PerformCurrentStage();
		}
		else
		{
			StopWork(Reason.FailedService);
		}
	}

	private bool CheckCompleteResult(ResultEventArgs e, string successText, string errorText)
	{
		bool result = false;
		StringBuilder stringBuilder = new StringBuilder("    ");
		if (e.Succeeded)
		{
			result = true;
			stringBuilder.Append(successText);
			if (e.Exception != null)
			{
				stringBuilder.AppendFormat(" ({0})", e.Exception.Message);
			}
		}
		else
		{
			stringBuilder.Append(errorText);
			if (e.Exception != null)
			{
				stringBuilder.AppendFormat(": {0}", e.Exception.Message);
			}
			else
			{
				stringBuilder.Append(Resources.Message_Unknown);
			}
		}
		Report(stringBuilder.ToString());
		return result;
	}

	private void ClearCurrentService(bool saveLastRun)
	{
		if (currentService != null)
		{
			currentService.ServiceCompleteEvent -= OnServiceComplete;
			if (saveLastRun)
			{
				lastRunService = currentService;
			}
			else
			{
				lastRunService = null;
			}
			currentService = null;
		}
	}

	private bool SetACMAshRatio(string value)
	{
		bool result = false;
		CurrentStage = Stage.WaitingForWrite;
		Service service = acm.Services["RT_Ash_Volume_Ratio_Update_Start_Ash_Ratio_for_dpf_volume_correction"];
		if (service != null)
		{
			oldValue = ((DataItem)ashRatioInstrument).Value;
			Cursor.Current = Cursors.WaitCursor;
			Report("Updating ash volume ratio...");
			service.InputValues[0].Value = ((DataItem)ashRatioInstrument).UnscaledValueAsDouble((object)value);
			ExecuteService(service, synchronous: false);
			result = true;
		}
		else
		{
			Report(Resources.Message_FailedToObtainACMAshDistanceAccumulator);
		}
		return result;
	}

	private void ClearOutput()
	{
		textBoxProgress.Text = string.Empty;
	}

	private void Report(string text)
	{
		if (textBoxProgress != null)
		{
			TextBox textBox = textBoxProgress;
			textBox.Text = textBox.Text + text + "\r\n";
			textBoxProgress.SelectionStart = textBoxProgress.TextLength;
			textBoxProgress.SelectionLength = 0;
			textBoxProgress.ScrollToCaret();
		}
		((CustomPanel)this).AddStatusMessage(text);
	}

	private void UpdateConnectedEquipmentType()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EquipmentType val = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Category == "Engine";
		});
		if (val != EquipmentType.Empty && !serialNumberValidations.TryGetValue(((EquipmentType)(ref val)).Name, out connectedValidationInformation))
		{
			connectedValidationInformation = null;
		}
	}

	private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		if (e.Category == "Engine")
		{
			UpdateConnectedEquipmentType();
			UpdateUserInterface();
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutDPFSerialNumber = new TableLayoutPanel();
		labelDPFSerialNumberHeader = new System.Windows.Forms.Label();
		textBoxDPFSerialNumber1 = new TextBox();
		labelSNErrorMessage1 = new System.Windows.Forms.Label();
		tableLayoutPanel = new TableLayoutPanel();
		flowLayoutPanel1 = new FlowLayoutPanel();
		labelLicenseMessage = new System.Windows.Forms.Label();
		labelWarning = new System.Windows.Forms.Label();
		titleLabel = new ScalingLabel();
		cpcReadout = new DigitalReadoutInstrument();
		acmReadout = new DigitalReadoutInstrument();
		labelMCM = new System.Windows.Forms.Label();
		labelACM = new System.Windows.Forms.Label();
		labelTaskQuestion = new System.Windows.Forms.Label();
		radioCleanRemanFilter = new RadioButton();
		radioNewFilter = new RadioButton();
		radioACMReplace = new RadioButton();
		buttonPerformAction = new Button();
		labelProgress = new System.Windows.Forms.Label();
		textBoxProgress = new TextBox();
		buttonClose = new Button();
		((Control)(object)tableLayoutDPFSerialNumber).SuspendLayout();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		flowLayoutPanel1.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutDPFSerialNumber, "tableLayoutDPFSerialNumber");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)tableLayoutDPFSerialNumber, 2);
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).Controls.Add(labelDPFSerialNumberHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).Controls.Add(textBoxDPFSerialNumber1, 1, 1);
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).Controls.Add(labelSNErrorMessage1, 2, 1);
		((Control)(object)tableLayoutDPFSerialNumber).Name = "tableLayoutDPFSerialNumber";
		componentResourceManager.ApplyResources(labelDPFSerialNumberHeader, "labelDPFSerialNumberHeader");
		labelDPFSerialNumberHeader.BackColor = SystemColors.ControlDark;
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).SetColumnSpan((Control)labelDPFSerialNumberHeader, 3);
		labelDPFSerialNumberHeader.Name = "labelDPFSerialNumberHeader";
		labelDPFSerialNumberHeader.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxDPFSerialNumber1, "textBoxDPFSerialNumber1");
		textBoxDPFSerialNumber1.Name = "textBoxDPFSerialNumber1";
		componentResourceManager.ApplyResources(labelSNErrorMessage1, "labelSNErrorMessage1");
		labelSNErrorMessage1.ForeColor = Color.Red;
		labelSNErrorMessage1.Name = "labelSNErrorMessage1";
		labelSNErrorMessage1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(flowLayoutPanel1, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)titleLabel, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)cpcReadout, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)acmReadout, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelMCM, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelACM, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelTaskQuestion, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(radioCleanRemanFilter, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(radioNewFilter, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(radioACMReplace, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)tableLayoutDPFSerialNumber, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonPerformAction, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelProgress, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(textBoxProgress, 0, 10);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonClose, 1, 11);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
		flowLayoutPanel1.Controls.Add(labelLicenseMessage);
		flowLayoutPanel1.Controls.Add(labelWarning);
		flowLayoutPanel1.Name = "flowLayoutPanel1";
		componentResourceManager.ApplyResources(labelLicenseMessage, "labelLicenseMessage");
		labelLicenseMessage.BackColor = SystemColors.Control;
		labelLicenseMessage.ForeColor = Color.Red;
		labelLicenseMessage.Name = "labelLicenseMessage";
		labelLicenseMessage.UseCompatibleTextRendering = true;
		labelLicenseMessage.UseMnemonic = false;
		componentResourceManager.ApplyResources(labelWarning, "labelWarning");
		labelWarning.BackColor = SystemColors.Control;
		labelWarning.ForeColor = Color.Red;
		labelWarning.Name = "labelWarning";
		labelWarning.UseCompatibleTextRendering = true;
		labelWarning.UseMnemonic = false;
		titleLabel.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)titleLabel, 2);
		componentResourceManager.ApplyResources(titleLabel, "titleLabel");
		titleLabel.FontGroup = null;
		titleLabel.LineAlignment = StringAlignment.Center;
		((Control)(object)titleLabel).Name = "titleLabel";
		((Control)(object)titleLabel).TabStop = false;
		componentResourceManager.ApplyResources(cpcReadout, "cpcReadout");
		cpcReadout.FontGroup = null;
		((SingleInstrumentBase)cpcReadout).FreezeValue = false;
		((SingleInstrumentBase)cpcReadout).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS237_DPF_Ash_volume_from_ACM");
		((Control)(object)cpcReadout).Name = "cpcReadout";
		((Control)(object)cpcReadout).TabStop = false;
		((SingleInstrumentBase)cpcReadout).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(acmReadout, "acmReadout");
		acmReadout.FontGroup = null;
		((SingleInstrumentBase)acmReadout).FreezeValue = false;
		((SingleInstrumentBase)acmReadout).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS109_Ash_Filter_Full_Volume");
		((Control)(object)acmReadout).Name = "acmReadout";
		((SingleInstrumentBase)acmReadout).UnitAlignment = StringAlignment.Near;
		labelMCM.AutoEllipsis = true;
		componentResourceManager.ApplyResources(labelMCM, "labelMCM");
		labelMCM.BackColor = SystemColors.ControlDark;
		labelMCM.CausesValidation = false;
		labelMCM.ForeColor = SystemColors.ControlText;
		labelMCM.Name = "labelMCM";
		labelMCM.UseCompatibleTextRendering = true;
		labelACM.AutoEllipsis = true;
		componentResourceManager.ApplyResources(labelACM, "labelACM");
		labelACM.BackColor = SystemColors.ControlDark;
		labelACM.CausesValidation = false;
		labelACM.ForeColor = SystemColors.ControlText;
		labelACM.Name = "labelACM";
		labelACM.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelTaskQuestion, "labelTaskQuestion");
		labelTaskQuestion.BackColor = SystemColors.ControlDark;
		labelTaskQuestion.CausesValidation = false;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)labelTaskQuestion, 2);
		labelTaskQuestion.Name = "labelTaskQuestion";
		labelTaskQuestion.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(radioCleanRemanFilter, "radioCleanRemanFilter");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)radioCleanRemanFilter, 2);
		radioCleanRemanFilter.Name = "radioCleanRemanFilter";
		radioCleanRemanFilter.TabStop = true;
		radioCleanRemanFilter.UseCompatibleTextRendering = true;
		radioCleanRemanFilter.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(radioNewFilter, "radioNewFilter");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)radioNewFilter, 2);
		radioNewFilter.Name = "radioNewFilter";
		radioNewFilter.TabStop = true;
		radioNewFilter.UseCompatibleTextRendering = true;
		radioNewFilter.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(radioACMReplace, "radioACMReplace");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)radioACMReplace, 2);
		radioACMReplace.Name = "radioACMReplace";
		radioACMReplace.TabStop = true;
		radioACMReplace.UseCompatibleTextRendering = true;
		radioACMReplace.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonPerformAction, "buttonPerformAction");
		buttonPerformAction.Name = "buttonPerformAction";
		buttonPerformAction.UseCompatibleTextRendering = true;
		buttonPerformAction.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelProgress, "labelProgress");
		labelProgress.BackColor = SystemColors.ControlDark;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)labelProgress, 2);
		labelProgress.Name = "labelProgress";
		labelProgress.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)textBoxProgress, 2);
		componentResourceManager.ApplyResources(textBoxProgress, "textBoxProgress");
		textBoxProgress.Name = "textBoxProgress";
		textBoxProgress.ReadOnly = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_DPFAshAccumulator_EPA10");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutDPFSerialNumber).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutDPFSerialNumber).PerformLayout();
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).PerformLayout();
		flowLayoutPanel1.ResumeLayout(performLayout: false);
		flowLayoutPanel1.PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
