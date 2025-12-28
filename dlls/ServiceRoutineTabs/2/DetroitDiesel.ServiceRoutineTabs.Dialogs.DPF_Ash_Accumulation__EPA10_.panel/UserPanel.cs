using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation__EPA10_.panel;

public class UserPanel : CustomPanel
{
	private class ValidationInformation
	{
		public readonly Regex Regex;

		public readonly string ErrorMessage;

		public ValidationInformation(Regex regex, string errorMessage)
		{
			Regex = regex;
			ErrorMessage = errorMessage;
		}
	}

	private enum Stage
	{
		Idle = 0,
		GetConfirmation = 1,
		GetValue = 2,
		WaitingForCPC2Read = 3,
		WriteValue = 4,
		WaitingForWrite = 5,
		WaitingToConfirmChange = 6,
		ResetCPC2 = 7,
		WaitingForCPC2Reset = 8,
		CommitChangesToCPC2 = 9,
		WaitingForCPC2Commit = 10,
		Finish = 11,
		Stopping = 12,
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

	private const string ResetRatioService = "RT_DPF_Ash_Volume_Reset_Start";

	private const string ReadRatioService = "RT_DPF_Ash_Volume_Read_Request_Results_Status";

	private const string ACMAshVolumeRatioUpdateStart = "RT_Ash_Volume_Ratio_Update_Start";

	private const string QualifierOdometer = "CO_Odometer";

	private const string QualifierEngineHours = "DT_AS045_Engine_Operating_Hours";

	private static readonly ValidationInformation HeavyDutySerialNumberValidation = new ValidationInformation(new Regex("(R124\\d{11}|124R\\d{11}|124\\d{11})", RegexOptions.Compiled), Resources.Message_SerialNumberShouldBeOfTheFormat124Xxxxxxxxxxx);

	private Dictionary<string, ValidationInformation> serialNumberValidations = new Dictionary<string, ValidationInformation>();

	private Channel acm;

	private Channel cpc2;

	private static readonly Qualifier ATDTypeParameter = new Qualifier((QualifierTypes)4, "ACM02T", "ATD_Hardware_Type");

	private ParameterDataItem atdType;

	private static readonly Qualifier AshRatioInstrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS109_Ash_Filter_Full_Volume");

	private InstrumentDataItem ashRatioInstrument;

	private string targetVIN;

	private bool adrReturnValue = false;

	private static readonly Regex AnySerialNumberValidation = new Regex("[R\\d]", RegexOptions.Compiled);

	private Stage currentStage = Stage.Idle;

	private object oldValue;

	private string valueToWrite;

	private string dpfSN1;

	private string dpfSN2;

	private string instrumentRatioAtStart;

	private DateTime timeAtStart;

	private Timer verificationTimeoutTimer = new Timer();

	private static readonly TimeSpan VerificationTimeoutPeriod = new TimeSpan(0, 0, 10);

	private Service currentService;

	private Service lastRunService;

	private ScalingLabel titleLabel;

	private DigitalReadoutInstrument cpcReadout;

	private DigitalReadoutInstrument acmReadout;

	private System.Windows.Forms.Label labelCPC2;

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

	private TextBox textBoxDPFSerialNumber2;

	private System.Windows.Forms.Label labelSNErrorMessage2;

	private TableLayoutPanel tableLayoutPanel;

	private AtsType AtsType
	{
		get
		{
			if (acm != null && atdType != null)
			{
				Choice choice = atdType.OriginalValue as Choice;
				if (choice != null)
				{
					return Convert.ToByte(choice.RawValue) switch
					{
						0 => AtsType.OneBox, 
						1 => AtsType.TwoBox, 
						_ => AtsType.Unknown, 
					};
				}
			}
			return AtsType.Unknown;
		}
	}

	public bool Online => IsChannelOnline(cpc2) && IsChannelOnline(acm);

	public bool Working => currentStage != Stage.Idle;

	public bool CanClose => !Working;

	public bool CanSetAshRatio
	{
		get
		{
			bool result = false;
			if (!Working && Online)
			{
				result = ((!radioACMReplace.Checked) ? (cpc2.Services["RT_DPF_Ash_Volume_Reset_Start"] != null && ValidSerialNumberProvided) : (cpc2.Services["RT_DPF_Ash_Volume_Read_Request_Results_Status"] != null));
				result = result && ashRatioInstrument != null && acm.Services["RT_Ash_Volume_Ratio_Update_Start"] != null;
			}
			return result;
		}
	}

	public bool ValidSerialNumberProvided
	{
		get
		{
			string errorText;
			return AtsType switch
			{
				AtsType.OneBox => ValidateSerialNumber(textBoxDPFSerialNumber1.Text, out errorText) && ValidateSerialNumber(textBoxDPFSerialNumber2.Text, out errorText), 
				AtsType.TwoBox => ValidateSerialNumber(textBoxDPFSerialNumber1.Text, out errorText), 
				_ => false, 
			};
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
		radioCleanRemanFilter.Checked = true;
		radioCleanRemanFilter.CheckedChanged += OnReasonChanged;
		radioNewFilter.CheckedChanged += OnReasonChanged;
		radioACMReplace.CheckedChanged += OnReasonChanged;
		textBoxDPFSerialNumber1.TextChanged += OnDPFSerialNumberChanged;
		textBoxDPFSerialNumber1.KeyPress += OnDPFSerialNumberKeyPress;
		textBoxDPFSerialNumber2.TextChanged += OnDPFSerialNumberChanged;
		textBoxDPFSerialNumber2.KeyPress += OnDPFSerialNumberKeyPress;
		buttonPerformAction.Click += OnPerformAction;
		verificationTimeoutTimer.Interval = (int)(VerificationTimeoutPeriod.TotalMilliseconds / 2.0);
		Timer timer = verificationTimeoutTimer;
		EventHandler value = delegate
		{
			if (CurrentStage == Stage.WaitingToConfirmChange)
			{
				PerformCurrentStage();
			}
		};
		timer.Tick += value;
		verificationTimeoutTimer.Enabled = false;
	}

	public override void OnChannelsChanged()
	{
		UpdateChannels();
		UpdateUserInterface();
	}

	private void UpdateChannels()
	{
		if (SetCPC2(((CustomPanel)this).GetChannel("CPC02T")) | SetACM(((CustomPanel)this).GetChannel("ACM02T")))
		{
			UpdateWarningMessage();
			textBoxDPFSerialNumber1.Text = string.Empty;
			textBoxDPFSerialNumber2.Text = string.Empty;
		}
	}

	private void CleanUpChannels()
	{
		SetCPC2(null);
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

	private bool SetCPC2(Channel cpc2)
	{
		bool result = false;
		if (this.cpc2 != cpc2)
		{
			StopWork(Reason.Disconnected);
			if (this.cpc2 != null)
			{
				this.cpc2.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
				targetVIN = string.Empty;
			}
			this.cpc2 = cpc2;
			result = true;
			if (this.cpc2 != null)
			{
				this.cpc2.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
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
					Service service = this.acm.Services["RT_Ash_Volume_Ratio_Update_Start"];
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
		if (e.KeyChar != '\b' && !AnySerialNumberValidation.IsMatch(e.KeyChar.ToString()))
		{
			e.Handled = true;
		}
	}

	private bool ValidateSerialNumber(string text, out string errorText)
	{
		bool result = false;
		errorText = string.Empty;
		ValidationInformation validationInformation = GetValidationInformation();
		if (validationInformation == null)
		{
			errorText = Resources.Message_UnsupportedEquipment;
		}
		else if (validationInformation.Regex.IsMatch(text))
		{
			result = true;
		}
		else
		{
			errorText = validationInformation.ErrorMessage;
			result = false;
		}
		return result;
	}

	private string GetEngineTypeName()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<EquipmentType> enumerable = EquipmentType.ConnectedEquipmentTypes("Engine");
		if (CollectionExtensions.Exactly<EquipmentType>(enumerable, 1))
		{
			EquipmentType val = enumerable.First();
			return ((EquipmentType)(ref val)).Name;
		}
		return null;
	}

	private ValidationInformation GetValidationInformation()
	{
		string engineTypeName = GetEngineTypeName();
		if (string.IsNullOrEmpty(engineTypeName))
		{
			return null;
		}
		if (serialNumberValidations.TryGetValue(engineTypeName, out var value))
		{
			return value;
		}
		return null;
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
		bool flag = Online && !Working && IsLicenseValid && AtsType != AtsType.Unknown && GetValidationInformation() != null;
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
			textBoxDPFSerialNumber2.ReadOnly = !flag;
			((Control)(object)tableLayoutDPFSerialNumber).Enabled = flag;
			ValidateDPFSerialBox(textBoxDPFSerialNumber1, labelSNErrorMessage1);
			switch (AtsType)
			{
			case AtsType.OneBox:
			{
				TextBox textBox2 = textBoxDPFSerialNumber2;
				enabled = (labelSNErrorMessage2.Visible = true);
				textBox2.Visible = enabled;
				ValidateDPFSerialBox(textBoxDPFSerialNumber2, labelSNErrorMessage2);
				labelDPFSerialNumberHeader.Text = Resources.Message_PleaseProvideTheTwoSerialNumbersForTheAftertreatmentSystemNowInstalledOnTheVehicle;
				break;
			}
			case AtsType.TwoBox:
			{
				TextBox textBox = textBoxDPFSerialNumber2;
				enabled = (labelSNErrorMessage2.Visible = false);
				textBox.Visible = enabled;
				labelDPFSerialNumberHeader.Text = Resources.Message_PleaseProvideTheSerialNumberForTheAftertreatmentSystemNowInstalledOnTheVehicle;
				break;
			}
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
			break;
		case Stage.GetConfirmation:
		{
			targetVIN = SapiManager.GetVehicleIdentificationNumber(acm);
			string text;
			if (radioACMReplace.Checked)
			{
				dpfSN1 = (dpfSN2 = string.Empty);
				text = Resources.Message_CopiedFromCPC2;
			}
			else
			{
				dpfSN1 = textBoxDPFSerialNumber1.Text;
				dpfSN2 = textBoxDPFSerialNumber2.Text;
				text = Resources.Message_Reset;
			}
			if (ConfirmationDialog.Show(targetVIN, AtsType, dpfSN1, dpfSN2, text))
			{
				ClearOutput();
				Report(Resources.Message_AshVolumeRatioModificationStarted);
				Report(Resources.Message_VIN + targetVIN);
				if (((Control)(object)tableLayoutDPFSerialNumber).Visible)
				{
					Report(Resources.Message_DPFSerialNumbers + dpfSN1 + ((AtsType == AtsType.OneBox) ? (", " + dpfSN2) : string.Empty));
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
				CurrentStage = Stage.WaitingForCPC2Read;
				ExecuteService(cpc2.Services["RT_DPF_Ash_Volume_Read_Request_Results_Status"], synchronous: false);
			}
			else
			{
				CurrentStage = Stage.WriteValue;
				valueToWrite = ((DataItem)ashRatioInstrument).ValueAsString((object)0.0);
				PerformCurrentStage();
			}
			break;
		case Stage.WaitingForCPC2Read:
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
			else if (((DataItem)ashRatioInstrument).ValueAsString(((DataItem)ashRatioInstrument).Value) != instrumentRatioAtStart)
			{
				Report(Resources.Message_RatioUpdated);
				CurrentStage = Stage.ResetCPC2;
				verificationTimeoutTimer.Enabled = false;
				PerformCurrentStage();
			}
			else if (DateTime.UtcNow - timeAtStart >= VerificationTimeoutPeriod)
			{
				Report(Resources.Message_RatioNotUpdated);
				CurrentStage = Stage.ResetCPC2;
				verificationTimeoutTimer.Enabled = false;
				PerformCurrentStage();
			}
			break;
		case Stage.ResetCPC2:
			if (radioACMReplace.Checked)
			{
				CurrentStage = Stage.Finish;
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForCPC2Reset;
				ExecuteService(cpc2.Services["RT_DPF_Ash_Volume_Reset_Start"], synchronous: false);
			}
			break;
		case Stage.WaitingForCPC2Reset:
			CurrentStage = Stage.CommitChangesToCPC2;
			PerformCurrentStage();
			break;
		case Stage.CommitChangesToCPC2:
			CommitToCPC2PermanentMemory();
			CurrentStage = Stage.WaitingForCPC2Commit;
			break;
		case Stage.WaitingForCPC2Commit:
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

	private void CommitToCPC2PermanentMemory()
	{
		if (cpc2.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService"))
		{
			Report(Resources.Message_CommittingChanges);
			cpc2.Services.ServiceCompleteEvent += OnCommitCompleteEvent;
			cpc2.Services.Execute(cpc2.Ecu.Properties["CommitToPermanentMemoryService"], synchronous: false);
		}
		else
		{
			Report(Resources.Message_NoCommitServiceAvailable);
			StopWork(Reason.FailedCommit);
		}
	}

	private void OnCommitCompleteEvent(object sender, ResultEventArgs e)
	{
		cpc2.Services.ServiceCompleteEvent -= OnCommitCompleteEvent;
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
				AddStationLogEntry(dpfSN1, dpfSN2);
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
					textBoxDPFSerialNumber2.Text = string.Empty;
					break;
				case Reason.FailedServiceExecute:
					switch (stage)
					{
					case Stage.ResetCPC2:
						Report(Resources.Message_FailedToObtainServiceToResetCPC2);
						break;
					case Stage.GetValue:
						Report(Resources.Message_FailedToObtainServiceForRetrievingCPC2Value);
						break;
					}
					break;
				case Reason.FailedService:
					if (stage == Stage.WaitingForCPC2Read)
					{
						Report(Resources.Message_FailedToExecuteReadOfAshAccumulationDistanceFromCPC2);
					}
					else
					{
						Report(Resources.Message_FailedToExecuteResetOfAshAccumulationDistanceInCPC2);
					}
					break;
				case Reason.FailedWrite:
					Report(Resources.Message_FailedToWriteTheAshMileageAccumulator);
					break;
				case Reason.FailedCommit:
					Report(Resources.Message_FailedToCommitTheChangesToTheCPC2YouMayNeedToRepeatThisProcedure);
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

	private void AddStationLogEntry(string serialNumber1, string serialNumber2)
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
		dictionary2["DPF_Serial_Number2"] = (string.IsNullOrEmpty(serialNumber2) ? Resources.Message_NA : serialNumber2);
		dictionary2["CO_Odometer"] = ReadEcuInfoData(cpc2, "CO_Odometer");
		dictionary2["DT_AS045_Engine_Operating_Hours"] = ReadInstrumentValue(((CustomPanel)this).GetChannel("MCM02T"), "DT_AS045_Engine_Operating_Hours");
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
		Report(Resources.Message_Executing + currentService.Name + "...");
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
		Service service = acm.Services["RT_Ash_Volume_Ratio_Update_Start"];
		if (service != null)
		{
			oldValue = ((DataItem)ashRatioInstrument).Value;
			Cursor.Current = Cursors.WaitCursor;
			Report(Resources.Message_UpdatingAshVolumeRatio);
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

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutDPFSerialNumber = new TableLayoutPanel();
		labelDPFSerialNumberHeader = new System.Windows.Forms.Label();
		textBoxDPFSerialNumber1 = new TextBox();
		labelSNErrorMessage1 = new System.Windows.Forms.Label();
		textBoxDPFSerialNumber2 = new TextBox();
		labelSNErrorMessage2 = new System.Windows.Forms.Label();
		tableLayoutPanel = new TableLayoutPanel();
		flowLayoutPanel1 = new FlowLayoutPanel();
		labelLicenseMessage = new System.Windows.Forms.Label();
		labelWarning = new System.Windows.Forms.Label();
		titleLabel = new ScalingLabel();
		cpcReadout = new DigitalReadoutInstrument();
		acmReadout = new DigitalReadoutInstrument();
		labelCPC2 = new System.Windows.Forms.Label();
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
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).Controls.Add(textBoxDPFSerialNumber2, 1, 2);
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).Controls.Add(labelSNErrorMessage2, 2, 2);
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
		componentResourceManager.ApplyResources(textBoxDPFSerialNumber2, "textBoxDPFSerialNumber2");
		textBoxDPFSerialNumber2.Name = "textBoxDPFSerialNumber2";
		componentResourceManager.ApplyResources(labelSNErrorMessage2, "labelSNErrorMessage2");
		labelSNErrorMessage2.ForeColor = Color.Red;
		labelSNErrorMessage2.Name = "labelSNErrorMessage2";
		labelSNErrorMessage2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(flowLayoutPanel1, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)titleLabel, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)cpcReadout, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)acmReadout, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelCPC2, 0, 1);
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
		((SingleInstrumentBase)cpcReadout).Instrument = new Qualifier((QualifierTypes)1, "CPC02T", "DT_AS052_DPF_Ash_Volume");
		((Control)(object)cpcReadout).Name = "cpcReadout";
		((Control)(object)cpcReadout).TabStop = false;
		((SingleInstrumentBase)cpcReadout).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(acmReadout, "acmReadout");
		acmReadout.FontGroup = null;
		((SingleInstrumentBase)acmReadout).FreezeValue = false;
		((SingleInstrumentBase)acmReadout).Instrument = new Qualifier((QualifierTypes)1, "ACM02T", "DT_AS109_Ash_Filter_Full_Volume");
		((Control)(object)acmReadout).Name = "acmReadout";
		((SingleInstrumentBase)acmReadout).UnitAlignment = StringAlignment.Near;
		labelCPC2.AutoEllipsis = true;
		componentResourceManager.ApplyResources(labelCPC2, "labelCPC2");
		labelCPC2.BackColor = SystemColors.ControlDark;
		labelCPC2.CausesValidation = false;
		labelCPC2.ForeColor = SystemColors.ControlText;
		labelCPC2.Name = "labelCPC2";
		labelCPC2.UseCompatibleTextRendering = true;
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
