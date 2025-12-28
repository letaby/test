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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__MY20_.panel;

public class UserPanel : CustomPanel
{
	private class ValidationInformation
	{
		public readonly Regex Regex;

		public ValidationInformation(Regex regex)
		{
			Regex = regex;
		}
	}

	private enum Stage
	{
		Idle = 0,
		GetConfirmation = 1,
		ResetValues = 2,
		WaitingForReset = 3,
		CommitChanges = 4,
		WaitingForCommit = 5,
		Finish = 6,
		Stopping = 7,
		_Start = 1
	}

	private enum Reason
	{
		Success,
		FailedWrite,
		FailedCommit,
		Closing,
		Disconnected,
		Canceled
	}

	private static readonly ValidationInformation HeavyDutySerialNumberValidation = new ValidationInformation(new Regex("[A-Za-z0-9]{3,}", RegexOptions.Compiled));

	private static readonly ValidationInformation MediumDutySerialNumberValidation = new ValidationInformation(new Regex("[A-Za-z0-9]{3,}", RegexOptions.Compiled));

	private Dictionary<string, ValidationInformation> serialNumberValidations = new Dictionary<string, ValidationInformation>();

	private int MaxLengthForRefText = 29;

	private Channel acm;

	private Channel mcm;

	private static readonly IList<string> acmScrAccumulatorQualifiers = new List<string>(new string[5] { "Time_Above_SCR_Inlet_Temp_1_Hour", "Time_Above_SCR_Inlet_Temp_1_Min", "Time_Above_SCR_Inlet_Temp_1_Sec", "Time_Above_SCR_Inlet_Temp_2", "Time_Above_SCR_Outlet_Temp" }).AsReadOnly();

	private List<ParameterDataItem> accumulators = new List<ParameterDataItem>();

	private string targetVIN;

	private string targetESN;

	private static readonly Qualifier ATDTypeParameter = new Qualifier((QualifierTypes)4, "ACM301T", "ATD_Hardware_Type");

	private ParameterDataItem atdType;

	private bool adrReturnValue = false;

	private static readonly Regex AnySerialNumberValidation = new Regex("[R\\d]", RegexOptions.Compiled);

	private static readonly Regex MdegFullScopeSerialNumberValidation = new Regex("[A-Z0-9]", RegexOptions.Compiled);

	private Stage currentStage = Stage.Idle;

	private ScalingLabel titleLabel;

	private TableLayoutPanel tableLayoutSerialNumber;

	private System.Windows.Forms.Label labelSerialNumberHeader;

	private TextBox textBoxSerialNumber1;

	private System.Windows.Forms.Label labelProgress;

	private TextBox textBoxProgress;

	private Button buttonPerformAction;

	private System.Windows.Forms.Label labelSNErrorMessage1;

	private System.Windows.Forms.Label labelWarning;

	private Button buttonClose;

	private FlowLayoutPanel flowLayoutPanel1;

	private System.Windows.Forms.Label labelLicenseMessage;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private TableLayoutPanel tableLayoutPanel;

	private AtsType AtsType
	{
		get
		{
			if (acm != null)
			{
				if (atdType == null)
				{
					return AtsType.OneBox;
				}
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

	public bool MdegFullScope
	{
		get
		{
			string currentEngineSerialNumber = SapiManager.GlobalInstance.CurrentEngineSerialNumber;
			bool flag = !string.IsNullOrEmpty(currentEngineSerialNumber) && currentEngineSerialNumber.StartsWith("934912C");
			return !flag;
		}
	}

	public bool Online => IsChannelOnline(acm) && IsChannelOnline(mcm);

	public bool Working => currentStage != Stage.Idle;

	public bool CanClose => !Working;

	public bool CanResetAccumulators
	{
		get
		{
			if (!Working && Online && ValidSerialNumberProvided && accumulators.Count == acmScrAccumulatorQualifiers.Count)
			{
				bool flag = true;
				foreach (string acmScrAccumulatorQualifier in acmScrAccumulatorQualifiers)
				{
					flag &= acm.Parameters[acmScrAccumulatorQualifier] != null;
				}
				return flag;
			}
			return false;
		}
	}

	public bool ValidSerialNumberProvided
	{
		get
		{
			string errorText;
			return ValidateSerialNumber(textBoxSerialNumber1.Text, out errorText);
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
		textBoxSerialNumber1.TextChanged += OnSerialNumberChanged;
		textBoxSerialNumber1.KeyPress += OnSerialNumberKeyPress;
		buttonPerformAction.Click += OnPerformAction;
	}

	public override void OnChannelsChanged()
	{
		UpdateChannels();
		UpdateUserInterface();
	}

	private void UpdateChannels()
	{
		if (SetACM(((CustomPanel)this).GetChannel("ACM301T")) && SetMCM2(((CustomPanel)this).GetChannel("MCM21T")))
		{
			UpdateWarningMessage();
			textBoxSerialNumber1.Text = string.Empty;
		}
	}

	private void CleanUpChannels()
	{
		SetACM(null);
		SetMCM2(null);
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
			if ((!acmScrAccumulatorQualifiers.Contains(parameter.Name) || channel.Ecu.Name != "ACM301T") && !object.Equals(parameter.Value, parameter.OriginalValue))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private bool SetACM(Channel acm)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (this.acm != acm)
		{
			StopWork(Reason.Disconnected);
			if (this.acm != null)
			{
				this.acm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
				accumulators.Clear();
				atdType = null;
				targetVIN = string.Empty;
				targetESN = string.Empty;
			}
			this.acm = acm;
			result = true;
			if (this.acm != null)
			{
				ref ParameterDataItem reference = ref atdType;
				DataItem obj = DataItem.Create(ATDTypeParameter, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
				reference = (ParameterDataItem)(object)((obj is ParameterDataItem) ? obj : null);
				this.acm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				ReadAccumulators(synchronous: false);
			}
		}
		return result;
	}

	private void ReadAccumulators(bool synchronous)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (acm == null)
		{
			return;
		}
		ParameterCollection parameters = acm.Parameters;
		Qualifier aTDTypeParameter = ATDTypeParameter;
		if (parameters[((Qualifier)(ref aTDTypeParameter)).Name] != null)
		{
			ParameterCollection parameters2 = acm.Parameters;
			ParameterCollection parameters3 = acm.Parameters;
			aTDTypeParameter = ATDTypeParameter;
			parameters2.ReadGroup(parameters3[((Qualifier)(ref aTDTypeParameter)).Name].GroupQualifier, fromCache: true, synchronous);
		}
		foreach (string acmScrAccumulatorQualifier in acmScrAccumulatorQualifiers)
		{
			DataItem obj = DataItem.Create(new Qualifier((QualifierTypes)4, "ACM301T", acmScrAccumulatorQualifier), (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
			ParameterDataItem val = (ParameterDataItem)(object)((obj is ParameterDataItem) ? obj : null);
			if (val != null)
			{
				accumulators.Add(val);
			}
		}
		if (acm.Parameters[acmScrAccumulatorQualifiers[0]] != null)
		{
			acm.Parameters.ReadGroup(acm.Parameters[acmScrAccumulatorQualifiers[0]].GroupQualifier, fromCache: true, synchronous);
		}
	}

	private bool SetMCM2(Channel mcm)
	{
		bool result = false;
		if (this.mcm != mcm)
		{
			StopWork(Reason.Disconnected);
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
				targetVIN = string.Empty;
				targetESN = string.Empty;
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

	private void OnSerialNumberChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnSerialNumberKeyPress(object sender, KeyPressEventArgs e)
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
		ValidationInformation validationInformation = GetValidationInformation();
		if (validationInformation == null)
		{
			errorText = Resources.Message_UnsupportedEquipment;
		}
		else
		{
			result = validationInformation.Regex.IsMatch(text);
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
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		ValidationInformation value = null;
		EquipmentType val = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Category == "Engine";
		});
		if (val != EquipmentType.Empty)
		{
			serialNumberValidations.TryGetValue(((EquipmentType)(ref val)).Name, out value);
		}
		return value;
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
		((Control)(object)tableLayoutSerialNumber).Enabled = flag;
		textBoxSerialNumber1.ReadOnly = !flag;
		buttonPerformAction.Enabled = CanResetAccumulators && flag;
		buttonClose.Enabled = CanClose;
		ValidateSCRSerialBox(textBoxSerialNumber1, labelSNErrorMessage1);
		labelSerialNumberHeader.Text = Resources.Message_PleaseProvideTheSerialNumberForTheNewSCRUnit;
	}

	private void ValidateSCRSerialBox(TextBox box, System.Windows.Forms.Label errorMessage)
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
			targetESN = SapiManager.GetEngineSerialNumber(mcm);
			targetVIN = SapiManager.GetVehicleIdentificationNumber(mcm);
			string text = textBoxSerialNumber1.Text;
			if (ConfirmationDialog.Show(targetESN, targetVIN, text))
			{
				ClearOutput();
				Report(Resources.Message_SCRAccumulatorsResetStarted);
				Report(Resources.Message_VIN + targetVIN);
				Report(Resources.Message_ESN + targetESN);
				Report(Resources.Message_SerialNumber + text);
				CurrentStage = Stage.ResetValues;
				PerformCurrentStage();
			}
			else
			{
				StopWork(Reason.Canceled);
			}
			break;
		}
		case Stage.ResetValues:
			Report(Resources.Message_WritingNewValue);
			if (ResetAccumulators())
			{
				CurrentStage = Stage.WaitingForReset;
			}
			else
			{
				StopWork(Reason.FailedWrite);
			}
			break;
		case Stage.WaitingForReset:
			CurrentStage = Stage.CommitChanges;
			PerformCurrentStage();
			break;
		case Stage.CommitChanges:
			CurrentStage = Stage.WaitingForCommit;
			CommitToPermanentMemory();
			break;
		case Stage.WaitingForCommit:
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

	private void CommitToPermanentMemory()
	{
		if (acm.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService"))
		{
			Report(Resources.Message_CommittingChanges);
			acm.Services.ServiceCompleteEvent += OnCommitCompleteEvent;
			acm.Services.Execute(acm.Ecu.Properties["CommitToPermanentMemoryService"], synchronous: false);
		}
		else
		{
			Report(Resources.Message_NoCommitServiceAvailable);
			StopWork(Reason.FailedCommit);
		}
	}

	private void OnCommitCompleteEvent(object sender, ResultEventArgs e)
	{
		acm.Services.ServiceCompleteEvent -= OnCommitCompleteEvent;
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
		if (CurrentStage != Stage.Stopping && CurrentStage != Stage.Idle)
		{
			Stage stage = CurrentStage;
			CurrentStage = Stage.Stopping;
			if (reason == Reason.Success)
			{
				AddStationLogEntry(textBoxSerialNumber1.Text);
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
					textBoxSerialNumber1.Text = string.Empty;
					break;
				case Reason.FailedWrite:
					Report(Resources.Message_FailedToWriteTheAccumulators);
					break;
				case Reason.FailedCommit:
					Report(Resources.Message_FailedToCommitTheChangesToTheACMYouMayNeedToRepeatThisProcedure);
					break;
				case Reason.Canceled:
					Report(Resources.Message_TheUserCanceledTheOperation);
					break;
				}
			}
			CurrentStage = Stage.Idle;
		}
		UpdateWarningMessage();
	}

	private void AddStationLogEntry(string serialNumber)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(serialNumber);
		stringBuilder.Append(";");
		switch (AtsType)
		{
		case AtsType.OneBox:
			stringBuilder.Append("a");
			break;
		case AtsType.TwoBox:
			stringBuilder.Append("b");
			break;
		}
		if (stringBuilder.Length > MaxLengthForRefText)
		{
			throw new InvalidOperationException("String too long for server event.");
		}
		ServerDataManager.UpdateEventsFile(acm, "SCRReset", targetESN, targetVIN, "OK", "DESCRIPTION", stringBuilder.ToString());
	}

	private bool ResetAccumulators()
	{
		bool result = false;
		if (accumulators.Count == acmScrAccumulatorQualifiers.Count)
		{
			Cursor.Current = Cursors.WaitCursor;
			foreach (ParameterDataItem accumulator in accumulators)
			{
				((DataItem)accumulator).WriteValue((object)0);
			}
			acm.Parameters.ParametersWriteCompleteEvent += OnParametersWriteComplete;
			acm.Parameters.Write(synchronous: false);
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
		ParameterCollection parameterCollection = (ParameterCollection)sender;
		parameterCollection.ParametersWriteCompleteEvent -= OnParametersWriteComplete;
		bool flag = false;
		if (e.Succeeded)
		{
			foreach (string acmScrAccumulatorQualifier in acmScrAccumulatorQualifiers)
			{
				Parameter parameter = parameterCollection[acmScrAccumulatorQualifier];
				flag |= !CheckParameterWriteStatus(parameter);
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

	private bool CheckParameterWriteStatus(Parameter parameter)
	{
		if (parameter.Exception != null)
		{
			if (!(parameter.Exception is CaesarException ex) || ex.ErrorNumber != 6602)
			{
				Report(Resources.Message_WhileResettingTheAccumulatorsTheFollowingErrorWasReported + "\r\n" + parameter.Name + ": " + parameter.Exception.Message);
				return false;
			}
			Report(Resources.Message_WhileResettingTheAccumulatorsTheFollowingWarningWasReported + "\r\n" + parameter.Name + ": " + ex.Message);
		}
		return true;
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel = new TableLayoutPanel();
		flowLayoutPanel1 = new FlowLayoutPanel();
		labelLicenseMessage = new System.Windows.Forms.Label();
		labelWarning = new System.Windows.Forms.Label();
		titleLabel = new ScalingLabel();
		tableLayoutSerialNumber = new TableLayoutPanel();
		labelSerialNumberHeader = new System.Windows.Forms.Label();
		textBoxSerialNumber1 = new TextBox();
		labelSNErrorMessage1 = new System.Windows.Forms.Label();
		buttonPerformAction = new Button();
		labelProgress = new System.Windows.Forms.Label();
		textBoxProgress = new TextBox();
		buttonClose = new Button();
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel).SuspendLayout();
		flowLayoutPanel1.SuspendLayout();
		((Control)(object)tableLayoutSerialNumber).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(flowLayoutPanel1, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)titleLabel, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)tableLayoutSerialNumber, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonPerformAction, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(labelProgress, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(textBoxProgress, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add(buttonClose, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)tableLayoutPanel1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrument5, 1, 2);
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
		componentResourceManager.ApplyResources(tableLayoutSerialNumber, "tableLayoutSerialNumber");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)tableLayoutSerialNumber, 2);
		((TableLayoutPanel)(object)tableLayoutSerialNumber).Controls.Add(labelSerialNumberHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutSerialNumber).Controls.Add(textBoxSerialNumber1, 1, 1);
		((TableLayoutPanel)(object)tableLayoutSerialNumber).Controls.Add(labelSNErrorMessage1, 2, 1);
		((Control)(object)tableLayoutSerialNumber).Name = "tableLayoutSerialNumber";
		componentResourceManager.ApplyResources(labelSerialNumberHeader, "labelSerialNumberHeader");
		labelSerialNumberHeader.BackColor = SystemColors.ControlDark;
		((TableLayoutPanel)(object)tableLayoutSerialNumber).SetColumnSpan((Control)labelSerialNumberHeader, 3);
		labelSerialNumberHeader.Name = "labelSerialNumberHeader";
		labelSerialNumberHeader.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxSerialNumber1, "textBoxSerialNumber1");
		textBoxSerialNumber1.Name = "textBoxSerialNumber1";
		componentResourceManager.ApplyResources(labelSNErrorMessage1, "labelSNErrorMessage1");
		labelSNErrorMessage1.ForeColor = Color.Red;
		labelSNErrorMessage1.Name = "labelSNErrorMessage1";
		labelSNErrorMessage1.UseCompatibleTextRendering = true;
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
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel).SetColumnSpan((Control)(object)tableLayoutPanel1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 2, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)4, "ACM301T", "Time_Above_SCR_Inlet_Temp_1_Hour");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)4, "ACM301T", "Time_Above_SCR_Inlet_Temp_1_Min");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)4, "ACM301T", "Time_Above_SCR_Inlet_Temp_1_Sec");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)4, "ACM301T", "Time_Above_SCR_Inlet_Temp_2");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)4, "ACM301T", "Time_Above_SCR_Outlet_Temp");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SCRReplacement");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel).PerformLayout();
		flowLayoutPanel1.ResumeLayout(performLayout: false);
		flowLayoutPanel1.PerformLayout();
		((Control)(object)tableLayoutSerialNumber).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutSerialNumber).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
