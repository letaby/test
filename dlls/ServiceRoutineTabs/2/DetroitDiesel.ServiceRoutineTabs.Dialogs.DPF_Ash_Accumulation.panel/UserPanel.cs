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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DPF_Ash_Accumulation.panel;

public class UserPanel : CustomPanel
{
	private enum SupportedAccumulatorManualChange
	{
		None,
		Distance
	}

	private enum Stage
	{
		Idle = 0,
		GetConfirmation = 1,
		GetValue = 2,
		WaitingForCPC2Read = 3,
		WriteValue = 4,
		ResetCPC2 = 5,
		WaitingForCPC2Reset = 6,
		CommitChangesToCPC2 = 7,
		WaitingForCPC2Commit = 8,
		ResetDPFMaxima = 9,
		WaitingForDPFMaximaReset = 10,
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

	private static class Log
	{
		public static void AddEvent(string eventText)
		{
			if (SapiManager.GlobalInstance != null && SapiManager.GlobalInstance.Sapi != null && !string.IsNullOrEmpty(eventText))
			{
				SapiManager.GlobalInstance.Sapi.LogFiles.LabelLog(eventText.Trim());
			}
		}
	}

	private static class ConfirmationDialog
	{
		private static string FormatString = Resources.Message_HereIsASummaryOfTheInformationThatYouHaveProvidedAndHasBeenCollectedByTheToolRN + "\r\n" + Resources.Message_AllInformationWillBeRecordedAndAnyFalseInformationCouldVoidWarrantyRN + "\r\n" + Resources.Message_PleaseReviewTheInformationAndConfirmThatItIsCorrectAndYouWouldLikeRN + "\r\n" + Resources.Message_ToProceedWithTheRequestedChangeToTheDPFAshAccumulationDistanceRN + "\r\n\r\n" + Resources.MessageFormat_VIN1RN + "\r\n" + Resources.MessageFormat_ESN0RN + "\r\nDPF SN: {2}\r\n" + Resources.MessageFormat_NewAshMileage3RN + "\r\n\r\n" + Resources.Message_IsThisInformationCorrectAndDoYouWantToContinue;

		private static string LogEntryForConfirmation = Resources.Message_DPFAshAccumulationChangeRequested + "DPFSN:{0}," + Resources.MessageFormat_Mileage1;

		public static bool Show(string esn, string vin, string dpfsn, string action)
		{
			bool flag = false;
			string empty = string.Empty;
			string empty2 = string.Empty;
			empty = ((!string.IsNullOrEmpty(dpfsn)) ? string.Format(FormatString, esn, vin, dpfsn, action) : string.Format(FormatString.Replace("DPF SN: {2}\r\n", ""), esn, vin, dpfsn, action));
			if (DialogResult.Yes == MessageBox.Show(empty, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
			{
				empty2 = ((!string.IsNullOrEmpty(dpfsn)) ? string.Format(LogEntryForConfirmation, dpfsn, action) : string.Format(LogEntryForConfirmation.Replace("DPFSN:{0},", ""), dpfsn, action));
				Log.AddEvent(empty2);
				return true;
			}
			return false;
		}
	}

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

	private const string ResetMileageService = "RT_DPF_Ash_Mileage_Reset_Start";

	private const string ReadMileageService = "RT_DPF_Ash_Mileage_Read_Request_Results_Status";

	private const string ResetMaximaService = "RT_SR014_SET_EOL_Default_Values_Start";

	private const string QualifierOdometer = "CO_Odometer";

	private const string QualifierEngineHours = "DT_AS045_Engine_Operating_Hours";

	private const double ReducedDpfAshMileageValueBy = 241402.0;

	private static readonly ValidationInformation HeavyDutySerialNumberValidation = new ValidationInformation(new Regex("S[HL]M\\d{6}|124\\d{11}", RegexOptions.Compiled), Resources.Message_SerialNumberShouldBeOfTheFormatSHMxxxxxxSLMxxxxxxOr124XxxxxxxxxxxAustralia);

	private static readonly ValidationInformation MediumDutySerialNumberValidation = new ValidationInformation(new Regex("SM[LM]\\d{6}", RegexOptions.Compiled), Resources.Message_SerialNumberShouldBeOfTheFormatSMLxxxxxxOrSMMxxxxxx);

	private Dictionary<string, ValidationInformation> serialNumberValidations = new Dictionary<string, ValidationInformation>();

	private Channel mcm;

	private Channel cpc2;

	private static readonly Qualifier AshMileageParameter = new Qualifier((QualifierTypes)4, "MCM", "e2p_dpf_ash_last_clean_dist");

	private ParameterDataItem ashMileage;

	private static readonly Qualifier DistanceTillAshFullInstrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS078_Distance_till_Ash_Full");

	private InstrumentDataItem distanceTillAshFull;

	private string targetVIN;

	private string targetESN;

	private bool adrReturnValue = false;

	private static readonly Regex AnySerialNumberValidation = new Regex("[SHLM\\d]", RegexOptions.Compiled);

	private static readonly Regex DistanceValidation = new Regex("\\d{1,6}", RegexOptions.Compiled);

	private Stage currentStage = Stage.Idle;

	private object oldDistanceValue;

	private object newDistanceValue;

	private string distanceValueToWrite;

	private string dpfSN;

	private Service currentService;

	private Service lastRunService;

	private RadioButton radioMCMReplace;

	private TableLayoutPanel tableLayoutPanel;

	private ScalingLabel titleLabel;

	private DigitalReadoutInstrument cpcReadout;

	private System.Windows.Forms.Label labelCPC2;

	private System.Windows.Forms.Label labelMCM;

	private System.Windows.Forms.Label labelTaskQuestion;

	private RadioButton radioCleanRemanFilter;

	private RadioButton radioNewFilter;

	private RadioButton radioMileageReset;

	private TableLayoutPanel tableLayoutDPFSerialNumber;

	private System.Windows.Forms.Label labelDPFSerialNumberHeader;

	private System.Windows.Forms.Label labelDPFSerialNumber;

	private TextBox textBoxDPFSerialNumber;

	private System.Windows.Forms.Label labelSNErrorMessage;

	private TableLayoutPanel tableLayoutAshDistance;

	private System.Windows.Forms.Label labelAshDistanceAndTime;

	private RadioButton radioDistanceCopiedFromCPC2;

	private RadioButton radioDistanceProvidedByTech;

	private System.Windows.Forms.Label labelTechDistanceUnits;

	private TextBox textBoxAshDistance;

	private System.Windows.Forms.Label labelTechDistance;

	private TableLayoutPanel tableLayoutPanelMCM;

	private DigitalReadoutInstrument readoutMCMDistance;

	private Button buttonClose;

	private TextBox textBoxProgress;

	private System.Windows.Forms.Label labelProgress;

	private FlowLayoutPanel flowLayoutPanel1;

	private System.Windows.Forms.Label labelWarning;

	private System.Windows.Forms.Label labelLicenseMessage;

	private Button buttonPerformAction;

	private bool Online => IsChannelOnline(cpc2) && IsChannelOnline(mcm);

	private bool Working => currentStage != Stage.Idle;

	private bool CanClose => !Working;

	private SupportedAccumulatorManualChange ShowAshAccumulators
	{
		get
		{
			SupportedAccumulatorManualChange supportedAccumulatorManualChange = SupportedAccumulatorManualChange.None;
			if (radioMCMReplace.Checked && AllowTechAccumulatorProvision)
			{
				supportedAccumulatorManualChange |= SupportedAccumulatorManualChange.Distance;
			}
			return supportedAccumulatorManualChange;
		}
	}

	private bool CanSetAshAccumulationDistance
	{
		get
		{
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			if (!Working && Online)
			{
				int num;
				if (radioMCMReplace.Checked ? ((!radioDistanceCopiedFromCPC2.Checked) ? (ValidDistanceProvided && cpc2.Services["RT_DPF_Ash_Mileage_Reset_Start"] != null && ashMileage != null) : (cpc2.Services["RT_DPF_Ash_Mileage_Read_Request_Results_Status"] != null && ashMileage != null)) : ((!radioMileageReset.Checked) ? (cpc2.Services["RT_DPF_Ash_Mileage_Reset_Start"] != null && ashMileage != null && ValidSerialNumberProvided) : (cpc2.Services["RT_DPF_Ash_Mileage_Reset_Start"] != null && ashMileage != null && ValidSerialNumberProvided && CanPerformMileageReset)))
				{
					ParameterCollection parameters = mcm.Parameters;
					Qualifier ashMileageParameter = AshMileageParameter;
					num = ((parameters[((Qualifier)(ref ashMileageParameter)).Name] != null) ? 1 : 0);
				}
				else
				{
					num = 0;
				}
				result = (byte)num != 0;
			}
			return result;
		}
	}

	private bool ValidDistanceProvided => ValidateDistance(textBoxAshDistance.Text);

	private bool ValidSerialNumberProvided
	{
		get
		{
			string errorText;
			return ValidateSerialNumber(textBoxDPFSerialNumber.Text, out errorText);
		}
	}

	private bool HaveDistanceMaxValue => ashMileage != null && distanceTillAshFull != null && ashMileage.OriginalValue != null && ((DataItem)distanceTillAshFull).Value != null;

	private bool AllowTechAccumulatorProvision
	{
		get
		{
			switch (LicenseManager.GlobalInstance.AccessLevel)
			{
			case 2:
				if (ashMileage != null && ashMileage.OriginalValue != null)
				{
					double num = ((DataItem)ashMileage).ValueAsDouble(ashMileage.OriginalValue);
					return num == 0.0;
				}
				break;
			case 3:
				return true;
			}
			return false;
		}
	}

	private bool IsLicenseValid => LicenseManager.GlobalInstance.AccessLevel >= 1;

	private bool SupportsMileageReset
	{
		get
		{
			string engineTypeName = GetEngineTypeName();
			if (!string.IsNullOrEmpty(engineTypeName))
			{
				return engineTypeName != "MBE900";
			}
			return false;
		}
	}

	private bool CanPerformMileageReset
	{
		get
		{
			if (ashMileage != null && ashMileage.OriginalValue != null)
			{
				return Math.Round(Convert.ToDouble(ashMileage.OriginalValue)) >= 241402.0;
			}
			return false;
		}
	}

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
		serialNumberValidations.Add("S60", HeavyDutySerialNumberValidation);
		serialNumberValidations.Add("MBE900", MediumDutySerialNumberValidation);
		serialNumberValidations.Add("MBE4000", HeavyDutySerialNumberValidation);
		serialNumberValidations.Add("DD13", HeavyDutySerialNumberValidation);
		serialNumberValidations.Add("DD15", HeavyDutySerialNumberValidation);
		serialNumberValidations.Add("DD15EURO4", HeavyDutySerialNumberValidation);
		radioDistanceCopiedFromCPC2.Checked = true;
		radioCleanRemanFilter.Checked = true;
		radioCleanRemanFilter.CheckedChanged += OnReasonChanged;
		radioNewFilter.CheckedChanged += OnReasonChanged;
		radioMCMReplace.CheckedChanged += OnReasonChanged;
		radioMileageReset.CheckedChanged += OnReasonChanged;
		radioDistanceCopiedFromCPC2.CheckedChanged += OnSourceChanged;
		radioDistanceProvidedByTech.CheckedChanged += OnSourceChanged;
		textBoxAshDistance.TextChanged += OnAshDistanceChanged;
		textBoxAshDistance.KeyPress += OnAshDistanceKeyPress;
		textBoxDPFSerialNumber.TextChanged += OnDPFSerialNumberChanged;
		textBoxDPFSerialNumber.KeyPress += OnDPFSerialNumberKeyPress;
		buttonPerformAction.Click += OnPerformAction;
	}

	public override void OnChannelsChanged()
	{
		UpdateChannels();
		UpdateUserInterface();
	}

	private void UpdateChannels()
	{
		if (SetCPC2(((CustomPanel)this).GetChannel("CPC2")) | SetMCM(((CustomPanel)this).GetChannel("MCM")))
		{
			UpdateWarningMessage();
		}
	}

	private void CleanUpChannels()
	{
		SetCPC2(null);
		SetMCM(null);
		UpdateWarningMessage();
	}

	private void UpdateWarningMessage()
	{
		bool visible = false;
		if (IsLicenseValid)
		{
			if (mcm != null && HasUnsentChanges(mcm))
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
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		foreach (Parameter parameter in channel.Parameters)
		{
			string qualifier = parameter.Qualifier;
			Qualifier ashMileageParameter = AshMileageParameter;
			if (!(qualifier != ((Qualifier)(ref ashMileageParameter)).Name))
			{
				string name = channel.Ecu.Name;
				ashMileageParameter = AshMileageParameter;
				if (!(name != ((Qualifier)(ref ashMileageParameter)).Ecu))
				{
					continue;
				}
			}
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
				targetESN = string.Empty;
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

	private bool SetMCM(Channel mcm)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (this.mcm != mcm)
		{
			StopWork(Reason.Disconnected);
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
				ashMileage = null;
				if (distanceTillAshFull != null)
				{
					((DataItem)distanceTillAshFull).UpdateEvent -= OnDistanceTillAshFullUpdate;
					distanceTillAshFull = null;
				}
				targetVIN = string.Empty;
				targetESN = string.Empty;
			}
			this.mcm = mcm;
			result = true;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				ref ParameterDataItem reference = ref ashMileage;
				DataItem obj = DataItem.Create(AshMileageParameter, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
				reference = (ParameterDataItem)(object)((obj is ParameterDataItem) ? obj : null);
				ref InstrumentDataItem reference2 = ref distanceTillAshFull;
				DataItem obj2 = DataItem.Create(DistanceTillAshFullInstrument, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
				reference2 = (InstrumentDataItem)(object)((obj2 is InstrumentDataItem) ? obj2 : null);
				if (distanceTillAshFull != null)
				{
					((DataItem)distanceTillAshFull).UpdateEvent += OnDistanceTillAshFullUpdate;
				}
				ReadAccumulators(synchronous: false);
			}
		}
		return result;
	}

	private void OnDistanceTillAshFullUpdate(object sender, ResultEventArgs e)
	{
		UpdateAccessLevels();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		SapiManager.GlobalInstance.AccessLevelsChanged += OnAccessLevelsChanged;
		UpdateChannels();
		UpdateAccessLevels();
	}

	private void OnAccessLevelsChanged(object sender, EventArgs e)
	{
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
			SapiManager.GlobalInstance.AccessLevelsChanged -= OnAccessLevelsChanged;
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

	private void OnSourceChanged(object sender, EventArgs e)
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

	private void OnAshDistanceChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnAshDistanceKeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar != '\b' && !DistanceValidation.IsMatch(e.KeyChar.ToString()))
		{
			e.Handled = true;
		}
	}

	private bool ValidateDistance(string text)
	{
		if (!string.IsNullOrEmpty(text) && HaveDistanceMaxValue && DistanceValidation.IsMatch(text) && double.TryParse(text, out var result))
		{
			double num = ((DataItem)ashMileage).ValueAsDouble(ashMileage.OriginalValue);
			num += ((DataItem)distanceTillAshFull).ValueAsDouble(((DataItem)distanceTillAshFull).Value);
			if (result <= num)
			{
				return true;
			}
		}
		return false;
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

	private bool IsChannelOnline(Channel channel)
	{
		return channel != null && channel.CommunicationsState == CommunicationsState.Online;
	}

	private void UpdateAccessLevels()
	{
		if (AllowTechAccumulatorProvision && !((Control)(object)tableLayoutAshDistance).Visible)
		{
			UpdateUserInterface();
		}
		else if (!AllowTechAccumulatorProvision && ((Control)(object)tableLayoutAshDistance).Visible)
		{
			radioDistanceCopiedFromCPC2.Checked = true;
			UpdateUserInterface();
		}
		UpdateWarningMessage();
	}

	private void UpdateAccumulatorEntryUserInterface()
	{
		labelTechDistanceUnits.Text = ((SingleInstrumentBase)readoutMCMDistance).Unit;
		textBoxAshDistance.ReadOnly = !radioDistanceProvidedByTech.Checked || !HaveDistanceMaxValue;
		SupportedAccumulatorManualChange showAshAccumulators = ShowAshAccumulators;
		((Control)(object)tableLayoutAshDistance).Visible = showAshAccumulators != SupportedAccumulatorManualChange.None;
		labelAshDistanceAndTime.Text = Resources.Message_PleaseIndicateTheNewAshAccumulationDistance;
	}

	private void UpdateUserInterface()
	{
		bool flag = Online && !Working && IsLicenseValid && GetValidationInformation() != null;
		RadioButton radioButton = radioCleanRemanFilter;
		bool enabled = (radioNewFilter.Enabled = flag);
		radioButton.Enabled = enabled;
		radioMileageReset.Enabled = flag && SupportsMileageReset && CanPerformMileageReset;
		buttonClose.Enabled = CanClose;
		labelTaskQuestion.Enabled = flag;
		buttonPerformAction.Enabled = CanSetAshAccumulationDistance && flag;
		if (ApplicationInformation.ProductAccessLevel == 1)
		{
			radioMCMReplace.Visible = false;
			((Control)(object)tableLayoutAshDistance).Visible = false;
		}
		else if (ApplicationInformation.ProductAccessLevel == 3 || ApplicationInformation.ProductAccessLevel == 2)
		{
			radioMCMReplace.Enabled = flag;
			((Control)(object)tableLayoutAshDistance).Enabled = flag;
			if (radioMCMReplace.Checked)
			{
				((Control)(object)tableLayoutDPFSerialNumber).Visible = false;
			}
			else if (!((Control)(object)tableLayoutDPFSerialNumber).Visible)
			{
				((Control)(object)tableLayoutDPFSerialNumber).Visible = true;
			}
		}
		UpdateAccumulatorEntryUserInterface();
		if (((Control)(object)tableLayoutDPFSerialNumber).Visible)
		{
			((Control)(object)tableLayoutDPFSerialNumber).Enabled = flag;
			textBoxDPFSerialNumber.ReadOnly = !flag;
			string errorText;
			if (textBoxDPFSerialNumber.ReadOnly)
			{
				textBoxDPFSerialNumber.BackColor = SystemColors.Control;
			}
			else if (ValidateSerialNumber(textBoxDPFSerialNumber.Text, out errorText))
			{
				labelSNErrorMessage.Visible = false;
				textBoxDPFSerialNumber.BackColor = Color.LightGreen;
			}
			else
			{
				labelSNErrorMessage.Text = errorText;
				labelSNErrorMessage.Visible = true;
				textBoxDPFSerialNumber.BackColor = Color.LightPink;
			}
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
			targetESN = SapiManager.GetEngineSerialNumber(mcm);
			targetVIN = SapiManager.GetVehicleIdentificationNumber(mcm);
			string text;
			if (radioMCMReplace.Checked)
			{
				dpfSN = string.Empty;
				text = ((!radioDistanceCopiedFromCPC2.Checked) ? (Resources.Message_ChangeDistanceTo + textBoxAshDistance.Text + " " + labelTechDistanceUnits.Text) : Resources.Message_SynchronizedFromCPC2);
			}
			else
			{
				text = ((!radioMileageReset.Checked) ? Resources.Message_Reset : string.Format(Resources.MessageFormat_DPFAshMileageExtendedBy01, Math.Round(((DataItem)ashMileage).ValueAsDouble((object)241402.0)).ToString(), ((DataItem)ashMileage).Units));
				dpfSN = textBoxDPFSerialNumber.Text;
			}
			if (ConfirmationDialog.Show(targetESN, targetVIN, dpfSN, text))
			{
				ClearOutput();
				Report(Resources.Message_AshAccumulatorModificationsStarted);
				Report(Resources.Message_VIN + targetVIN);
				Report(Resources.Message_ESN + targetESN);
				if (((Control)(object)tableLayoutDPFSerialNumber).Visible)
				{
					Report(Resources.Message_DPFSerialNumber + dpfSN);
				}
				Report(Resources.Message_AshAccumulators0 + text);
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
			if (radioMCMReplace.Checked)
			{
				if (radioDistanceCopiedFromCPC2.Checked)
				{
					CurrentStage = Stage.WaitingForCPC2Read;
					ExecuteService(cpc2.Services["RT_DPF_Ash_Mileage_Read_Request_Results_Status"], synchronous: false);
				}
				else
				{
					CurrentStage = Stage.WriteValue;
					distanceValueToWrite = textBoxAshDistance.Text;
					PerformCurrentStage();
				}
			}
			else
			{
				if (radioMileageReset.Checked)
				{
					distanceValueToWrite = (Math.Round(((DataItem)ashMileage).ValueAsDouble(ashMileage.OriginalValue)) - Math.Round(((DataItem)ashMileage).ValueAsDouble((object)241402.0))).ToString();
				}
				else
				{
					distanceValueToWrite = ((DataItem)ashMileage).ValueAsString((object)0.0);
				}
				CurrentStage = Stage.WriteValue;
				PerformCurrentStage();
			}
			break;
		case Stage.WaitingForCPC2Read:
			distanceValueToWrite = ((DataItem)ashMileage).ValueAsString((object)Convert.ToDouble(lastRunService.OutputValues[0].Value));
			CurrentStage = Stage.WriteValue;
			PerformCurrentStage();
			break;
		case Stage.WriteValue:
			Report(Resources.Message_WritingNewValues);
			if (SetMCMAshAccumulators(distanceValueToWrite))
			{
				CurrentStage = Stage.ResetCPC2;
			}
			else
			{
				StopWork(Reason.FailedWrite);
			}
			break;
		case Stage.ResetCPC2:
			CurrentStage = Stage.WaitingForCPC2Reset;
			ExecuteService(cpc2.Services["RT_DPF_Ash_Mileage_Reset_Start"], synchronous: false);
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
			CurrentStage = Stage.ResetDPFMaxima;
			PerformCurrentStage();
			break;
		case Stage.ResetDPFMaxima:
		{
			CurrentStage = Stage.WaitingForDPFMaximaReset;
			Service service = mcm.Services["RT_SR014_SET_EOL_Default_Values_Start"];
			if (service == null)
			{
				Report(Resources.Message_NoResetServiceAvailable);
				PerformCurrentStage();
			}
			else
			{
				Report(Resources.Message_ResettingDPFMaximumTemperatures);
				service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(6);
				ExecuteService(service, synchronous: false);
			}
			break;
		}
		case Stage.WaitingForDPFMaximaReset:
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

	private void UpdateServiceLog(char type, object oldValue, object currentValue)
	{
		string value = string.Empty;
		if (radioCleanRemanFilter.Checked)
		{
			value = Resources.Message_ReasonCleanRemanFilter;
		}
		else if (radioNewFilter.Checked)
		{
			value = Resources.Message_ReasonNewFilter;
		}
		else if (radioMCMReplace.Checked)
		{
			value = Resources.Message_ReasonMCMReplacement;
		}
		else if (radioMileageReset.Checked)
		{
			value = Resources.Message_ReasonMileageResetFromFilterMeasurement;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["reasontext"] = value;
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2["DPF_Serial_Number1"] = (string.IsNullOrEmpty(dpfSN) ? Resources.Message_NA : dpfSN);
		string text = ((type == 'd') ? "Distance" : "Time");
		if (oldValue != null)
		{
			dictionary2["Old_" + text] = Convert.ToInt32(oldValue).ToString();
		}
		dictionary2["Current_" + text] = Convert.ToInt32(currentValue).ToString();
		dictionary2["CO_Odometer"] = ReadEcuInfoData(cpc2, "CO_Odometer");
		dictionary2["DT_AS045_Engine_Operating_Hours"] = ReadInstrumentValue(mcm, "DT_AS045_Engine_Operating_Hours");
		ServerDataManager.UpdateEventsFile(mcm, (IDictionary<string, string>)dictionary, (IDictionary<string, string>)dictionary2, "DPFAshAccumulation", targetESN, targetVIN, "OK", "DESCRIPTION", string.Empty);
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

	private void StopWork(Reason reason)
	{
		if (CurrentStage != Stage.Stopping && CurrentStage != Stage.Idle)
		{
			Stage stage = CurrentStage;
			CurrentStage = Stage.Stopping;
			if (reason == Reason.Success)
			{
				UpdateServiceLog('d', oldDistanceValue, newDistanceValue);
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
				{
					Report(Resources.Message_OneOrMoreDevicesDisconnected);
					TextBox textBox = textBoxDPFSerialNumber;
					string text = (textBoxAshDistance.Text = string.Empty);
					textBox.Text = text;
					break;
				}
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
					switch (stage)
					{
					case Stage.WaitingForCPC2Read:
						Report(Resources.Message_FailedToExecuteReadOfAshAccumulationDistanceFromCPC2);
						break;
					case Stage.WaitingForCPC2Reset:
						Report(Resources.Message_FailedToExecuteResetOfAshAccumulationDistanceInCPC2);
						break;
					case Stage.WaitingForDPFMaximaReset:
						Report(Resources.Message_FailedToExecuteResetOfDPFTemperatureAccumulatorsInMCM);
						break;
					}
					break;
				case Reason.FailedWrite:
					Report(Resources.Message_FailedToWriteTheAshAccumulators);
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
			distanceValueToWrite = string.Empty;
			oldDistanceValue = null;
			newDistanceValue = null;
			CurrentStage = Stage.Idle;
			ReadAccumulators(synchronous: false);
		}
		UpdateWarningMessage();
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

	private void ReadAccumulators(bool synchronous)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (mcm != null)
		{
			ParameterCollection parameters = mcm.Parameters;
			Qualifier ashMileageParameter = AshMileageParameter;
			if (parameters[((Qualifier)(ref ashMileageParameter)).Name] != null)
			{
				ParameterCollection parameters2 = mcm.Parameters;
				ParameterCollection parameters3 = mcm.Parameters;
				ashMileageParameter = AshMileageParameter;
				parameters2.ReadGroup(parameters3[((Qualifier)(ref ashMileageParameter)).Name].GroupQualifier, fromCache: true, synchronous);
			}
		}
	}

	private bool SetMCMAshAccumulators(string valueDistance)
	{
		bool result = false;
		if (ashMileage != null)
		{
			Cursor.Current = Cursors.WaitCursor;
			oldDistanceValue = ashMileage.OriginalValue;
			((DataItem)ashMileage).WriteValue((object)valueDistance);
			newDistanceValue = ((DataItem)ashMileage).Value;
			mcm.Parameters.ParametersWriteCompleteEvent += OnParametersWriteComplete;
			mcm.Parameters.Write(synchronous: false);
			result = true;
		}
		else
		{
			Report(Resources.Message_FailedToObtainMCMAshDistanceAccumulator);
		}
		return result;
	}

	private void OnParametersWriteComplete(object sender, ResultEventArgs e)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		ParameterCollection parameterCollection = (ParameterCollection)sender;
		parameterCollection.ParametersWriteCompleteEvent -= OnParametersWriteComplete;
		bool flag = false;
		if (e.Succeeded)
		{
			Qualifier ashMileageParameter = AshMileageParameter;
			Parameter parameter = parameterCollection[((Qualifier)(ref ashMileageParameter)).Name];
			if (parameter.Exception != null)
			{
				if (parameter.Exception is CaesarException ex && ex.ErrorNumber == 6602)
				{
					Report(Resources.Message_WhileWritingTheNewAshAccumulationDistanceTheFollowingWarningWasReportedRN + "\r\n" + parameter.Name + ": " + ex.Message);
				}
				else
				{
					flag = true;
					Report(Resources.Message_WhileWritingTheNewAshAccumulationDistanceTheFollowingErrorWasReportedRN + "\r\n" + parameter.Name + ": " + parameter.Exception.Message);
				}
			}
		}
		else
		{
			Report(string.Format(Resources.MessageFormat_WhileWritingTheNewAshAccumulationDistanceTheFollowingErrorOccurred + "\r\n\r\n\"{0}\"\r\n\r\n" + Resources.ParametersHaveNotBeenVerifiedAndMayNotHaveBeenWritten, e.Exception.Message));
			flag = true;
		}
		Cursor.Current = Cursors.Default;
		if (flag)
		{
			StopWork(Reason.FailedWrite);
		}
		else
		{
			PerformCurrentStage();
		}
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
		Application.DoEvents();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd9: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		radioMCMReplace = new RadioButton();
		titleLabel = new ScalingLabel();
		cpcReadout = new DigitalReadoutInstrument();
		labelCPC2 = new System.Windows.Forms.Label();
		labelMCM = new System.Windows.Forms.Label();
		labelTaskQuestion = new System.Windows.Forms.Label();
		radioCleanRemanFilter = new RadioButton();
		radioNewFilter = new RadioButton();
		radioMileageReset = new RadioButton();
		tableLayoutDPFSerialNumber = new TableLayoutPanel();
		labelDPFSerialNumberHeader = new System.Windows.Forms.Label();
		labelDPFSerialNumber = new System.Windows.Forms.Label();
		textBoxDPFSerialNumber = new TextBox();
		labelSNErrorMessage = new System.Windows.Forms.Label();
		tableLayoutAshDistance = new TableLayoutPanel();
		labelAshDistanceAndTime = new System.Windows.Forms.Label();
		radioDistanceCopiedFromCPC2 = new RadioButton();
		radioDistanceProvidedByTech = new RadioButton();
		labelTechDistanceUnits = new System.Windows.Forms.Label();
		textBoxAshDistance = new TextBox();
		labelTechDistance = new System.Windows.Forms.Label();
		tableLayoutPanelMCM = new TableLayoutPanel();
		readoutMCMDistance = new DigitalReadoutInstrument();
		buttonClose = new Button();
		textBoxProgress = new TextBox();
		labelProgress = new System.Windows.Forms.Label();
		flowLayoutPanel1 = new FlowLayoutPanel();
		labelWarning = new System.Windows.Forms.Label();
		labelLicenseMessage = new System.Windows.Forms.Label();
		buttonPerformAction = new Button();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		((Control)(object)tableLayoutDPFSerialNumber).SuspendLayout();
		((Control)(object)tableLayoutAshDistance).SuspendLayout();
		((Control)(object)tableLayoutPanelMCM).SuspendLayout();
		flowLayoutPanel1.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(radioMCMReplace, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)titleLabel, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)cpcReadout, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelCPC2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelMCM, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelTaskQuestion, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(radioCleanRemanFilter, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(radioNewFilter, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(radioMileageReset, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)tableLayoutDPFSerialNumber, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)tableLayoutAshDistance, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)tableLayoutPanelMCM, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonClose, 1, 14);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(textBoxProgress, 0, 13);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelProgress, 0, 12);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(flowLayoutPanel1, 0, 11);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonPerformAction, 0, 10);
		((Control)(object)tableLayoutPanel).Name = "tableLayoutPanel";
		componentResourceManager.ApplyResources(radioMCMReplace, "radioMCMReplace");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)radioMCMReplace, 2);
		radioMCMReplace.Name = "radioMCMReplace";
		radioMCMReplace.TabStop = true;
		radioMCMReplace.UseCompatibleTextRendering = true;
		radioMCMReplace.UseVisualStyleBackColor = true;
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
		((SingleInstrumentBase)cpcReadout).Instrument = new Qualifier((QualifierTypes)1, "CPC2", "DT_AS056_DPF_Ash_Content_Mileage");
		((Control)(object)cpcReadout).Name = "cpcReadout";
		((Control)(object)cpcReadout).TabStop = false;
		((SingleInstrumentBase)cpcReadout).UnitAlignment = StringAlignment.Near;
		labelCPC2.AutoEllipsis = true;
		componentResourceManager.ApplyResources(labelCPC2, "labelCPC2");
		labelCPC2.BackColor = SystemColors.ControlDark;
		labelCPC2.CausesValidation = false;
		labelCPC2.ForeColor = SystemColors.ControlText;
		labelCPC2.Name = "labelCPC2";
		labelCPC2.UseCompatibleTextRendering = true;
		labelMCM.AutoEllipsis = true;
		componentResourceManager.ApplyResources(labelMCM, "labelMCM");
		labelMCM.BackColor = SystemColors.ControlDark;
		labelMCM.CausesValidation = false;
		labelMCM.ForeColor = SystemColors.ControlText;
		labelMCM.Name = "labelMCM";
		labelMCM.UseCompatibleTextRendering = true;
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
		componentResourceManager.ApplyResources(radioMileageReset, "radioMileageReset");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)radioMileageReset, 2);
		radioMileageReset.Name = "radioMileageReset";
		radioMileageReset.TabStop = true;
		radioMileageReset.UseCompatibleTextRendering = true;
		radioMileageReset.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutDPFSerialNumber, "tableLayoutDPFSerialNumber");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)tableLayoutDPFSerialNumber, 2);
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).Controls.Add(labelDPFSerialNumberHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).Controls.Add(labelDPFSerialNumber, 0, 1);
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).Controls.Add(textBoxDPFSerialNumber, 1, 1);
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).Controls.Add(labelSNErrorMessage, 2, 1);
		((Control)(object)tableLayoutDPFSerialNumber).Name = "tableLayoutDPFSerialNumber";
		componentResourceManager.ApplyResources(labelDPFSerialNumberHeader, "labelDPFSerialNumberHeader");
		labelDPFSerialNumberHeader.BackColor = SystemColors.ControlDark;
		((TableLayoutPanel)(object)tableLayoutDPFSerialNumber).SetColumnSpan((Control)labelDPFSerialNumberHeader, 3);
		labelDPFSerialNumberHeader.Name = "labelDPFSerialNumberHeader";
		labelDPFSerialNumberHeader.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelDPFSerialNumber, "labelDPFSerialNumber");
		labelDPFSerialNumber.Name = "labelDPFSerialNumber";
		labelDPFSerialNumber.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxDPFSerialNumber, "textBoxDPFSerialNumber");
		textBoxDPFSerialNumber.Name = "textBoxDPFSerialNumber";
		componentResourceManager.ApplyResources(labelSNErrorMessage, "labelSNErrorMessage");
		labelSNErrorMessage.ForeColor = Color.Red;
		labelSNErrorMessage.Name = "labelSNErrorMessage";
		labelSNErrorMessage.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutAshDistance, "tableLayoutAshDistance");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)tableLayoutAshDistance, 2);
		((TableLayoutPanel)(object)tableLayoutAshDistance).Controls.Add(labelAshDistanceAndTime, 0, 0);
		((TableLayoutPanel)(object)tableLayoutAshDistance).Controls.Add(radioDistanceCopiedFromCPC2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutAshDistance).Controls.Add(radioDistanceProvidedByTech, 0, 2);
		((TableLayoutPanel)(object)tableLayoutAshDistance).Controls.Add(labelTechDistanceUnits, 2, 3);
		((TableLayoutPanel)(object)tableLayoutAshDistance).Controls.Add(textBoxAshDistance, 1, 3);
		((TableLayoutPanel)(object)tableLayoutAshDistance).Controls.Add(labelTechDistance, 0, 3);
		((Control)(object)tableLayoutAshDistance).Name = "tableLayoutAshDistance";
		componentResourceManager.ApplyResources(labelAshDistanceAndTime, "labelAshDistanceAndTime");
		labelAshDistanceAndTime.BackColor = SystemColors.ControlDark;
		((TableLayoutPanel)(object)tableLayoutAshDistance).SetColumnSpan((Control)labelAshDistanceAndTime, 4);
		labelAshDistanceAndTime.Name = "labelAshDistanceAndTime";
		labelAshDistanceAndTime.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(radioDistanceCopiedFromCPC2, "radioDistanceCopiedFromCPC2");
		((TableLayoutPanel)(object)tableLayoutAshDistance).SetColumnSpan((Control)radioDistanceCopiedFromCPC2, 4);
		radioDistanceCopiedFromCPC2.Name = "radioDistanceCopiedFromCPC2";
		radioDistanceCopiedFromCPC2.TabStop = true;
		radioDistanceCopiedFromCPC2.UseCompatibleTextRendering = true;
		radioDistanceCopiedFromCPC2.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(radioDistanceProvidedByTech, "radioDistanceProvidedByTech");
		((TableLayoutPanel)(object)tableLayoutAshDistance).SetColumnSpan((Control)radioDistanceProvidedByTech, 4);
		radioDistanceProvidedByTech.Name = "radioDistanceProvidedByTech";
		radioDistanceProvidedByTech.TabStop = true;
		radioDistanceProvidedByTech.UseCompatibleTextRendering = true;
		radioDistanceProvidedByTech.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelTechDistanceUnits, "labelTechDistanceUnits");
		labelTechDistanceUnits.Name = "labelTechDistanceUnits";
		labelTechDistanceUnits.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxAshDistance, "textBoxAshDistance");
		textBoxAshDistance.Name = "textBoxAshDistance";
		componentResourceManager.ApplyResources(labelTechDistance, "labelTechDistance");
		labelTechDistance.Name = "labelTechDistance";
		labelTechDistance.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelMCM, "tableLayoutPanelMCM");
		((TableLayoutPanel)(object)tableLayoutPanelMCM).Controls.Add((Control)(object)readoutMCMDistance, 0, 0);
		((Control)(object)tableLayoutPanelMCM).Name = "tableLayoutPanelMCM";
		componentResourceManager.ApplyResources(readoutMCMDistance, "readoutMCMDistance");
		readoutMCMDistance.FontGroup = null;
		((SingleInstrumentBase)readoutMCMDistance).FreezeValue = false;
		((SingleInstrumentBase)readoutMCMDistance).Instrument = new Qualifier((QualifierTypes)4, "MCM", "e2p_dpf_ash_last_clean_dist");
		((Control)(object)readoutMCMDistance).Name = "readoutMCMDistance";
		((SingleInstrumentBase)readoutMCMDistance).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)textBoxProgress, 2);
		componentResourceManager.ApplyResources(textBoxProgress, "textBoxProgress");
		textBoxProgress.Name = "textBoxProgress";
		textBoxProgress.ReadOnly = true;
		componentResourceManager.ApplyResources(labelProgress, "labelProgress");
		labelProgress.BackColor = SystemColors.ControlDark;
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)labelProgress, 2);
		labelProgress.Name = "labelProgress";
		labelProgress.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)flowLayoutPanel1, 2);
		flowLayoutPanel1.Controls.Add(labelWarning);
		flowLayoutPanel1.Controls.Add(labelLicenseMessage);
		flowLayoutPanel1.Name = "flowLayoutPanel1";
		componentResourceManager.ApplyResources(labelWarning, "labelWarning");
		labelWarning.BackColor = SystemColors.Control;
		labelWarning.ForeColor = Color.Red;
		labelWarning.Name = "labelWarning";
		labelWarning.UseCompatibleTextRendering = true;
		labelWarning.UseMnemonic = false;
		componentResourceManager.ApplyResources(labelLicenseMessage, "labelLicenseMessage");
		labelLicenseMessage.BackColor = SystemColors.Control;
		labelLicenseMessage.ForeColor = Color.Red;
		labelLicenseMessage.Name = "labelLicenseMessage";
		labelLicenseMessage.UseCompatibleTextRendering = true;
		labelLicenseMessage.UseMnemonic = false;
		componentResourceManager.ApplyResources(buttonPerformAction, "buttonPerformAction");
		buttonPerformAction.Name = "buttonPerformAction";
		buttonPerformAction.UseCompatibleTextRendering = true;
		buttonPerformAction.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_DPFAshAccumulator_EPA07");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).PerformLayout();
		((Control)(object)tableLayoutDPFSerialNumber).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutDPFSerialNumber).PerformLayout();
		((Control)(object)tableLayoutAshDistance).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutAshDistance).PerformLayout();
		((Control)(object)tableLayoutPanelMCM).ResumeLayout(performLayout: false);
		flowLayoutPanel1.ResumeLayout(performLayout: false);
		flowLayoutPanel1.PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
