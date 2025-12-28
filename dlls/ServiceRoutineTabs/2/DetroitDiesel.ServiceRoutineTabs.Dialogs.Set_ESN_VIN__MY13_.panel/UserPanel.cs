using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Adr;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Net;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_ESN_VIN__MY13_.panel;

public class UserPanel : CustomPanel
{
	private enum Stage
	{
		Idle = 0,
		SetEsn = 1,
		WaitingForSetEsn = 2,
		CommitMCMForESN = 3,
		WaitingForCommitMCMForESN = 4,
		KeyOffOnCPC = 5,
		WaitingForKeyOffOnCPC = 6,
		WaitingForBackOnlineCPC = 7,
		SetVINForMCM = 8,
		WaitingForSetMCMVIN = 9,
		SetVINForACM = 10,
		WaitingForSetACMVIN = 11,
		SetVINForTCM = 12,
		WaitingForSetTCMVIN = 13,
		SetVINForCPC = 14,
		WaitingForSetCPCVIN = 15,
		HardResetCPC = 16,
		WaitingForHardResetCPC = 17,
		TurnIgnitionOff = 18,
		WaitForIgnitionOffDisconnection = 19,
		WaitForIgnitionOnReconnection = 20,
		Finish = 21,
		Stopping = 22,
		_StartESN = 1,
		_StartVIN = 8
	}

	private enum Reason
	{
		Succeeded,
		FailedServiceExecute,
		FailedService,
		Closing,
		Disconnected
	}

	private const string McmName = "MCM21T";

	private const string AcmName = "ACM21T";

	private const string CpcName = "CPC04T";

	private const string TcmName = "TCM01T";

	private const string ESNEcuInfo = "CO_ESN";

	private const string VINEcuInfo = "CO_VIN";

	private const string EngineSerialNumberService = "DL_ID_Write_Engine_Serial_Number";

	private const string AlternateEngineSerialNumberService = "DL_ID_Engine_Serial_Number";

	private const string KeyOffOnResetService = "FN_KeyOffOnReset";

	private const string HardResetService = "FN_HardReset";

	private const string VehicleIdentificationNumberService = "DL_ID_Write_VIN_Current";

	private const string AlternateVehicleIdentificationNumberService = "DL_ID_VIN_Current";

	private const string VINServiceForCPC = "WriteVINService";

	private static readonly Regex ValidESNCharacters = new Regex("(\\w+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

	private Channel mcm;

	private bool setESN = false;

	private bool setVIN = false;

	private EcuInfo ecuInfoESN = null;

	private Channel cpc;

	private Channel acm;

	private Channel tcm;

	private bool haveUpdatedESN = false;

	private Stage currentStage = Stage.Idle;

	private Service currentService;

	private string currentServiceString;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonClose;

	private System.Windows.Forms.Label labelVIN;

	private TextBox textBoxESN;

	private Button buttonSetESN;

	private Button buttonSetVIN;

	private TextBox textBoxVIN;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private TextBox textBoxOutput;

	private System.Windows.Forms.Label labelESN;

	public string Vehicle
	{
		get
		{
			return textBoxVIN.Text;
		}
		set
		{
			textBoxVIN.Text = value;
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

	private bool Working => currentStage != Stage.Idle;

	private bool Online => (cpc != null && cpc.CommunicationsState == CommunicationsState.Online) || (mcm != null && mcm.CommunicationsState == CommunicationsState.Online) || (acm != null && acm.CommunicationsState == CommunicationsState.Online) || (tcm != null && tcm.CommunicationsState == CommunicationsState.Online);

	private bool IsValidLicense => LicenseManager.GlobalInstance.AccessLevel >= 2;

	private bool CanClose => !Working;

	private bool CanSetESN => !Working && mcm != null && mcm.CommunicationsState == CommunicationsState.Online && (mcm.Services["DL_ID_Write_Engine_Serial_Number"] != null || mcm.Services["DL_ID_Engine_Serial_Number"] != null) && IsValidLicense;

	private bool IsValidEsn => CanSetESN && IsSerialNumberValid(textBoxESN.Text);

	private bool CanEditEsn => CanSetESN && ecuInfoESN != null && ecuInfoESN.Value != null;

	private bool CanSetVIN => !Working && Online && IsValidLicense;

	private bool IsValidVIN => CanSetVIN && IsVINValid(textBoxVIN.Text);

	public UserPanel()
	{
		InitializeComponent();
		textBoxESN.TextChanged += OnTextChanged;
		textBoxVIN.TextChanged += OnTextChanged;
		buttonSetESN.Click += OnSetESNClick;
		buttonSetVIN.Click += OnSetVINClick;
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		ConnectionManager.GlobalInstance.PropertyChanged += ConnectionManager_PropertyChanged;
		((UserControl)this).OnLoad(e);
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM21T", (ChannelLookupOptions)5));
		SetCPC(((CustomPanel)this).GetChannel("CPC04T", (ChannelLookupOptions)5));
		SetACM(((CustomPanel)this).GetChannel("ACM21T", (ChannelLookupOptions)5));
		SetTCM(((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)5));
		if (CurrentStage == Stage.WaitForIgnitionOnReconnection || CurrentStage == Stage.WaitForIgnitionOffDisconnection)
		{
			PerformCurrentStage();
		}
		else
		{
			foreach (Channel channel in SapiManager.GlobalInstance.Sapi.Channels)
			{
				string vehicleIdentificationNumber = SapiManager.GetVehicleIdentificationNumber(channel);
				if (!string.IsNullOrEmpty(vehicleIdentificationNumber) && Utility.ValidateVehicleIdentificationNumber(vehicleIdentificationNumber))
				{
					Vehicle = vehicleIdentificationNumber;
				}
				if (Vehicle.Length > 0)
				{
					break;
				}
			}
		}
		UpdateUserInterface();
	}

	private void ConnectionManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if ((e.PropertyName == "IgnitionStatus" && CurrentStage == Stage.WaitForIgnitionOnReconnection) || CurrentStage == Stage.WaitForIgnitionOffDisconnection)
		{
			PerformCurrentStage();
		}
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			ConnectionManager.GlobalInstance.PropertyChanged -= ConnectionManager_PropertyChanged;
			StopWork(Reason.Closing);
			SetMCM(null);
			SetCPC(null);
			SetACM(null);
			SetTCM(null);
		}
	}

	private bool SetMCM(Channel mcm)
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (this.mcm != mcm)
		{
			result = true;
			if (CurrentStage != Stage.WaitingForBackOnlineCPC && CurrentStage != Stage.WaitForIgnitionOffDisconnection && CurrentStage != Stage.WaitForIgnitionOnReconnection)
			{
				StopWork(Reason.Disconnected);
			}
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				if (ecuInfoESN != null)
				{
					ecuInfoESN.EcuInfoUpdateEvent -= OnESNUpdate;
					ecuInfoESN = null;
				}
			}
			this.mcm = mcm;
			haveUpdatedESN = false;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				ecuInfoESN = this.mcm.EcuInfos["CO_ESN"];
				if (ecuInfoESN != null)
				{
					ecuInfoESN.EcuInfoUpdateEvent += OnESNUpdate;
				}
				((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)8, this.mcm.Ecu.Name, "CO_EquipmentType");
			}
		}
		return result;
	}

	private bool SetCPC(Channel cpc)
	{
		bool result = false;
		if (this.cpc != cpc)
		{
			if (this.cpc != null)
			{
				this.cpc.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			this.cpc = cpc;
			if (this.cpc != null)
			{
				this.cpc.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetACM(Channel acm)
	{
		bool result = false;
		if (this.acm != acm)
		{
			if (this.acm != null)
			{
				this.acm.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			this.acm = acm;
			if (this.acm != null)
			{
				this.acm.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetTCM(Channel tcm)
	{
		bool result = false;
		if (this.tcm != tcm)
		{
			result = true;
			this.tcm = tcm;
		}
		return result;
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		if (sender == cpc && e.CommunicationsState == CommunicationsState.Online && CurrentStage == Stage.WaitingForBackOnlineCPC)
		{
			PerformCurrentStage();
		}
		UpdateUserInterface();
	}

	private void OnTextChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnESNUpdate(object sender, ResultEventArgs e)
	{
		UpdateESN(forceUpdate: true);
	}

	private void UpdateESN(bool forceUpdate)
	{
		if (!forceUpdate && (textBoxESN.TextLength != 0 || haveUpdatedESN))
		{
			return;
		}
		if (ecuInfoESN != null && ecuInfoESN.Value != null)
		{
			textBoxESN.Text = ecuInfoESN.Value;
			textBoxESN.SelectAll();
			haveUpdatedESN = true;
			return;
		}
		textBoxESN.Text = string.Empty;
		if (ecuInfoESN != null)
		{
			ecuInfoESN.Read(synchronous: false);
		}
	}

	private void UpdateUserInterface()
	{
		UpdateESN(forceUpdate: false);
		((Control)(object)digitalReadoutInstrument1).Visible = mcm != null;
		buttonClose.Enabled = CanClose;
		buttonSetESN.Enabled = IsValidEsn;
		buttonSetVIN.Enabled = IsValidVIN;
		textBoxESN.ReadOnly = !CanEditEsn;
		textBoxVIN.ReadOnly = !CanSetVIN;
		if (textBoxESN.ReadOnly)
		{
			textBoxESN.BackColor = SystemColors.Control;
		}
		else if (IsSerialNumberValid(textBoxESN.Text))
		{
			textBoxESN.BackColor = Color.PaleGreen;
		}
		else
		{
			textBoxESN.BackColor = Color.LightPink;
		}
		if (textBoxVIN.ReadOnly)
		{
			textBoxVIN.BackColor = SystemColors.Control;
		}
		else if (IsVINValid(textBoxVIN.Text))
		{
			textBoxVIN.BackColor = Color.PaleGreen;
		}
		else
		{
			textBoxVIN.BackColor = Color.LightPink;
		}
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

	public bool IsVINValid(string text)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(text) && Utility.ValidateVehicleIdentificationNumber(text))
		{
			result = true;
		}
		return result;
	}

	public bool IsSerialNumberValid(string text)
	{
		bool result = false;
		int num = ((GetEngineTypeName() == "S60") ? 10 : 14);
		if (!string.IsNullOrEmpty(text) && text.Length == num && ValidESNCharacters.IsMatch(text))
		{
			result = true;
		}
		return result;
	}

	private void ReportResult(string text)
	{
		((CustomPanel)this).LabelLog("SetESNVIN", text);
		textBoxOutput.AppendText(text + Environment.NewLine);
	}

	private void OnSetESNClick(object sender, EventArgs e)
	{
		if (IsValidEsn)
		{
			setESN = true;
			setVIN = false;
			StartWork();
		}
	}

	private void OnSetVINClick(object sender, EventArgs e)
	{
		if (IsValidVIN)
		{
			setESN = false;
			setVIN = true;
			StartWork();
		}
	}

	private void StartWork()
	{
		if (setESN)
		{
			CurrentStage = Stage.SetEsn;
		}
		else if (setVIN)
		{
			CurrentStage = Stage.SetVINForMCM;
		}
		PerformCurrentStage();
	}

	private void PerformCurrentStage()
	{
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Invalid comparison between Unknown and I4
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Invalid comparison between Unknown and I4
		switch (CurrentStage)
		{
		case Stage.Idle:
			break;
		case Stage.SetEsn:
		{
			CurrentStage = Stage.WaitingForSetEsn;
			Service service4 = mcm.Services["DL_ID_Write_Engine_Serial_Number"] ?? mcm.Services["DL_ID_Engine_Serial_Number"];
			if (service4 == null)
			{
				StopWork(Reason.FailedServiceExecute);
				break;
			}
			string text2 = textBoxESN.Text;
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_SettingESNTo0, text2));
			service4.InputValues[0].Value = text2;
			ExecuteAsynchronousService(service4);
			break;
		}
		case Stage.WaitingForSetEsn:
			CurrentStage = Stage.CommitMCMForESN;
			PerformCurrentStage();
			break;
		case Stage.CommitMCMForESN:
			if (mcm.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService"))
			{
				CurrentStage = Stage.WaitingForCommitMCMForESN;
				ReportResult(Resources.Message_WritingChangesToPermanentMemory);
				ExecuteAsynchronousService(mcm.Ecu.Properties["CommitToPermanentMemoryService"]);
				break;
			}
			if (mcm.Ecu.Name != "MR201T")
			{
				ReportResult(Resources.Message_SkippingMCMCommitProcessAsTheServiceCouldNotBeFound);
			}
			CurrentStage = Stage.KeyOffOnCPC;
			PerformCurrentStage();
			break;
		case Stage.WaitingForCommitMCMForESN:
			CurrentStage = Stage.KeyOffOnCPC;
			PerformCurrentStage();
			break;
		case Stage.KeyOffOnCPC:
			if (cpc == null)
			{
				ReportResult(Resources.Message_SkippingCPCCommitProcessAsTheCPCIsNotConnected);
				CurrentStage = Stage.TurnIgnitionOff;
				PerformCurrentStage();
			}
			else if (cpc.Ecu.Name != "CPC302T" && cpc.Ecu.Name != "CPC501T" && cpc.Ecu.Name != "CPC502T")
			{
				Service service3 = cpc.Services["FN_KeyOffOnReset"];
				if (service3 == null)
				{
					CurrentStage = Stage.TurnIgnitionOff;
					ReportResult(Resources.Message_SkippingCPCResetAsTheServiceCannotBeFound);
					PerformCurrentStage();
				}
				else
				{
					CurrentStage = Stage.WaitingForKeyOffOnCPC;
					ReportResult(Resources.Message_SynchronizingESNToCPCViaKeyOffOnReset);
					ExecuteAsynchronousService(service3);
				}
			}
			else
			{
				CurrentStage = Stage.TurnIgnitionOff;
				PerformCurrentStage();
			}
			break;
		case Stage.WaitingForKeyOffOnCPC:
			CurrentStage = Stage.WaitingForBackOnlineCPC;
			ReportResult(Resources.Message_WaitingForTheCPCToComeBackOnline);
			break;
		case Stage.WaitingForBackOnlineCPC:
			CurrentStage = Stage.TurnIgnitionOff;
			PerformCurrentStage();
			break;
		case Stage.SetVINForMCM:
		{
			if (mcm == null)
			{
				ReportResult(Resources.Message_SkippingMCMSetVINProcessAsTheMCMIsNotConnected);
				CurrentStage = Stage.SetVINForACM;
				PerformCurrentStage();
				break;
			}
			CurrentStage = Stage.WaitingForSetMCMVIN;
			Service service = mcm.Services["DL_ID_Write_VIN_Current"] ?? mcm.Services["DL_ID_VIN_Current"];
			if (service == null)
			{
				CurrentStage = Stage.SetVINForACM;
				ReportResult(Resources.Message_SkippingSettingOfMCMVINAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetMCMVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_SettingVINTo0, text));
				service.InputValues[0].Value = text;
				ExecuteAsynchronousService(service);
			}
			break;
		}
		case Stage.WaitingForSetMCMVIN:
			CurrentStage = Stage.SetVINForACM;
			PerformCurrentStage();
			break;
		case Stage.SetVINForACM:
		{
			if (acm == null)
			{
				ReportResult(Resources.Message_SkippingACMSetVINProcessAsTheACMIsNotConnected);
				CurrentStage = Stage.SetVINForTCM;
				PerformCurrentStage();
				break;
			}
			Service service2 = acm.Services["DL_ID_Write_VIN_Current"];
			if (service2 == null)
			{
				CurrentStage = Stage.SetVINForTCM;
				ReportResult(Resources.Message_SkippingSettingOfACMVINAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetACMVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_SettingVINTo0, text));
				service2.InputValues[0].Value = text;
				ExecuteAsynchronousService(service2);
			}
			break;
		}
		case Stage.WaitingForSetACMVIN:
			CurrentStage = Stage.SetVINForTCM;
			PerformCurrentStage();
			break;
		case Stage.SetVINForTCM:
		{
			if (tcm == null)
			{
				ReportResult(Resources.Message_SkippingTCMSetVINProcessAsTheTCMIsNotConnected);
				CurrentStage = Stage.SetVINForCPC;
				PerformCurrentStage();
				break;
			}
			Service service6 = tcm.Services["DL_ID_VIN_Current"];
			if (service6 == null)
			{
				CurrentStage = Stage.SetVINForCPC;
				ReportResult(Resources.Message_SkippingSettingOfTCMVINAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetTCMVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_SettingVINTo0, text));
				service6.InputValues[0].Value = text;
				ExecuteAsynchronousService(service6);
			}
			break;
		}
		case Stage.WaitingForSetTCMVIN:
			CurrentStage = Stage.SetVINForCPC;
			PerformCurrentStage();
			break;
		case Stage.SetVINForCPC:
		{
			if (cpc == null)
			{
				ReportResult(Resources.Message_SkippingCPCSetVINProcessAsTheCPCIsNotConnected);
				CurrentStage = Stage.TurnIgnitionOff;
				PerformCurrentStage();
				break;
			}
			Service service5 = cpc.Services[cpc.Ecu.Properties["WriteVINService"]];
			if (service5 == null)
			{
				CurrentStage = Stage.TurnIgnitionOff;
				ReportResult(Resources.Message_SkippingSettingOfCPCVINAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetCPCVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_SettingVINTo0, text));
				service5.InputValues[0].Value = text;
				ExecuteAsynchronousService(service5);
			}
			break;
		}
		case Stage.WaitingForSetCPCVIN:
			CurrentStage = ((cpc.Ecu.Name == "CPC302T" || cpc.Ecu.Name == "CPC501T" || cpc.Ecu.Name == "CPC502T") ? Stage.HardResetCPC : Stage.TurnIgnitionOff);
			PerformCurrentStage();
			break;
		case Stage.HardResetCPC:
		{
			Service service7 = cpc.Services["FN_HardReset"];
			if (service7 == null)
			{
				CurrentStage = Stage.TurnIgnitionOff;
				ReportResult(Resources.Message_SkippingCPCResetAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForHardResetCPC;
				ReportResult(Resources.Message_CommittingChangesToCPCViaHardReset);
				ExecuteAsynchronousService(service7);
			}
			break;
		}
		case Stage.WaitingForHardResetCPC:
			CurrentStage = Stage.TurnIgnitionOff;
			PerformCurrentStage();
			break;
		case Stage.TurnIgnitionOff:
			if (mcm != null && mcm.Ecu.Name == "MR201T")
			{
				CurrentStage = Stage.WaitForIgnitionOffDisconnection;
				ReportResult(Resources.Message_TurnTheIgnitionOffToFinalizeChanges);
			}
			else
			{
				CurrentStage = Stage.Finish;
				PerformCurrentStage();
			}
			break;
		case Stage.WaitForIgnitionOffDisconnection:
			if ((int)ConnectionManager.GlobalInstance.IgnitionStatus == 1 && ((CustomPanel)this).GetChannel("MR201T") == null)
			{
				CurrentStage = Stage.WaitForIgnitionOnReconnection;
				ReportResult(Resources.Message_TurnTheIgnitionOnToVerifyTheChanges);
			}
			break;
		case Stage.WaitForIgnitionOnReconnection:
			if (((CustomPanel)this).GetChannel("MR201T") != null && (int)ConnectionManager.GlobalInstance.IgnitionStatus == 0)
			{
				CurrentStage = Stage.Finish;
				PerformCurrentStage();
			}
			break;
		case Stage.Finish:
		{
			if (mcm != null)
			{
				ReportResult(Resources.Message_ResettingMCMFaults);
				mcm.FaultCodes.Reset(synchronous: false);
			}
			if (acm != null)
			{
				ReportResult(Resources.Message_ResettingACMFaults);
				acm.FaultCodes.Reset(synchronous: false);
			}
			if (cpc != null)
			{
				ReportResult(Resources.Message_ResettingCPCFaults);
				cpc.FaultCodes.Reset(synchronous: false);
			}
			Channel channel = ((CustomPanel)this).GetChannel("J1939-255");
			if (channel != null)
			{
				ReportResult(Resources.Message_ResettingJ1939Faults);
				channel.FaultCodes.Reset(synchronous: false);
			}
			StopWork(Reason.Succeeded);
			break;
		}
		case Stage.Stopping:
			break;
		default:
			throw new InvalidOperationException("Unknown stage.");
		}
	}

	private void StopWork(Reason reason)
	{
		if (CurrentStage == Stage.Stopping || CurrentStage == Stage.Idle)
		{
			return;
		}
		Stage stage = CurrentStage;
		CurrentStage = Stage.Stopping;
		if (reason == Reason.Succeeded)
		{
			if (stage != Stage.Finish)
			{
				throw new InvalidOperationException();
			}
			ReportResult(Resources.Message_VerifyingResults);
			if (setESN)
			{
				VerifyResults(mcm, "MCM", "CO_ESN", "ESN", textBoxESN.Text);
				if (cpc != null && cpc.Ecu.Name != "CPC302T" && cpc.Ecu.Name != "CPC501T" && cpc.Ecu.Name != "CPC502T")
				{
					VerifyResults(cpc, "CPC", "CO_ESN", "ESN", textBoxESN.Text);
				}
			}
			else if (setVIN)
			{
				VerifyResults(mcm, "MCM", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(cpc, "CPC", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(acm, "ACM", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(tcm, "TCM", "CO_VIN", "VIN", textBoxVIN.Text);
			}
		}
		else
		{
			ReportResult(Resources.Message_TheProcedureFailedToComplete);
			switch (reason)
			{
			case Reason.Disconnected:
				ReportResult(Resources.Message_TheMCMWasDisconnected);
				break;
			case Reason.FailedService:
				ReportResult(Resources.Message_FailedToExecuteService);
				break;
			case Reason.FailedServiceExecute:
				ReportResult(Resources.Message_FailedToObtainService);
				break;
			}
		}
		ClearCurrentService();
		CurrentStage = Stage.Idle;
	}

	private void VerifyResults(Channel channel, string channelName, string function, string functionName, string textBoxText)
	{
		if (channel == null || channel.EcuInfos[function] == null)
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_01CannotBeVerified, channelName, functionName));
			return;
		}
		EcuInfo ecuInfo = channel.EcuInfos[function];
		try
		{
			ecuInfo.Read(synchronous: true);
		}
		catch (CaesarException ex)
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_FailedToReadECUInfoCannotVerifyThe01Error2, channelName, functionName, ex.Message));
		}
		catch (InvalidOperationException ex2)
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_FailedToReadECUInfoThe0IsUnavailableError1, channelName, ex2.Message));
		}
		if (string.Compare(ecuInfo.Value, textBoxText, ignoreCase: true) == 0)
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_The01HasSuccessfullyBeenSetTo2, channelName, functionName, textBoxText));
		}
		else
		{
			ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_The01HasNotBeenChangedAndHasAValueOf2, channelName, functionName, ecuInfo.Value));
		}
	}

	private void ExecuteAsynchronousService(Service service)
	{
		if (currentService != null)
		{
			throw new InvalidOperationException("Must wait for current service to finish before continuing.");
		}
		if (service == null)
		{
			StopWork(Reason.FailedServiceExecute);
			return;
		}
		currentService = service;
		currentService.ServiceCompleteEvent += OnServiceComplete;
		ReportResult(Resources.Message_Executing + service.Name + "…");
		currentService.Execute(synchronous: false);
	}

	private void ExecuteAsynchronousService(string serviceString)
	{
		if (!string.IsNullOrEmpty(currentServiceString))
		{
			throw new InvalidOperationException("Must wait for current service to finish before continuing.");
		}
		if (string.IsNullOrEmpty(serviceString))
		{
			StopWork(Reason.FailedServiceExecute);
			return;
		}
		currentServiceString = serviceString;
		mcm.Services.ServiceCompleteEvent += OnServiceComplete;
		ReportResult(Resources.Message_ExecutingServices);
		mcm.Services.Execute(serviceString, synchronous: false);
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		ClearCurrentService();
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
				stringBuilder.Append(": Unknown");
			}
		}
		ReportResult(stringBuilder.ToString());
		return result;
	}

	private void ClearCurrentService()
	{
		if (currentService != null)
		{
			currentService.ServiceCompleteEvent -= OnServiceComplete;
			currentService = null;
		}
		if (!string.IsNullOrEmpty(currentServiceString))
		{
			mcm.Services.ServiceCompleteEvent -= OnServiceComplete;
			currentServiceString = null;
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		labelVIN = new System.Windows.Forms.Label();
		labelESN = new System.Windows.Forms.Label();
		textBoxESN = new TextBox();
		textBoxVIN = new TextBox();
		buttonSetVIN = new Button();
		buttonSetESN = new Button();
		buttonClose = new Button();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		textBoxOutput = new TextBox();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelVIN, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelESN, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxESN, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxVIN, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSetVIN, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSetESN, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxOutput, 0, 4);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(labelVIN, "labelVIN");
		labelVIN.Name = "labelVIN";
		labelVIN.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelESN, "labelESN");
		labelESN.Name = "labelESN";
		labelESN.UseCompatibleTextRendering = true;
		textBoxESN.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textBoxESN, "textBoxESN");
		textBoxESN.Name = "textBoxESN";
		textBoxVIN.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textBoxVIN, "textBoxVIN");
		textBoxVIN.Name = "textBoxVIN";
		componentResourceManager.ApplyResources(buttonSetVIN, "buttonSetVIN");
		buttonSetVIN.Name = "buttonSetVIN";
		buttonSetVIN.UseCompatibleTextRendering = true;
		buttonSetVIN.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonSetESN, "buttonSetESN");
		buttonSetESN.Name = "buttonSetESN";
		buttonSetESN.UseCompatibleTextRendering = true;
		buttonSetESN.UseVisualStyleBackColor = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)8, "MCM21T", "CO_EquipmentType");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).TitleLengthPercentOfControl = 48;
		((SingleInstrumentBase)digitalReadoutInstrument1).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)textBoxOutput, 3);
		componentResourceManager.ApplyResources(textBoxOutput, "textBoxOutput");
		textBoxOutput.Name = "textBoxOutput";
		textBoxOutput.ReadOnly = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SetESN");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
