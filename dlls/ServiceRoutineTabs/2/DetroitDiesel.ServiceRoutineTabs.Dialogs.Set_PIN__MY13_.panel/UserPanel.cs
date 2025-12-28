using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_PIN__MY13_.panel;

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
		UnlockMCMParameterWrite = 9,
		ExecuteMCMVINService = 10,
		WaitingForSetMCMVIN = 11,
		SetVINForACM = 12,
		UnlockACMParameterWrite = 13,
		ExecuteACMVINService = 14,
		WaitingForSetACMVIN = 15,
		SetVINForCPC = 16,
		WaitingForSetCPCVIN = 17,
		ReadParameters = 18,
		WaitingForReadParameters = 19,
		ConnectToServer = 20,
		WaitingForConnectToServer = 21,
		Finish = 22,
		Stopping = 23,
		_StartESN = 1,
		_StartVIN = 8
	}

	private enum Reason
	{
		Succeeded,
		FailedServiceExecute,
		FailedService,
		FailedParameterRead,
		Closing,
		Disconnected
	}

	private const string McmName = "MCM21T";

	private const string AcmName = "ACM21T";

	private const string CpcName = "CPC04T";

	private const string ESNEcuInfo = "CO_ESN";

	private const string VINEcuInfo = "CO_VIN";

	private const string EngineSerialNumberService = "DL_ID_Write_Engine_Serial_Number";

	private const string KeyOffOnResetService = "FN_KeyOffOnReset";

	private const string VehicleIdentificationNumberService = "DL_ID_Write_VIN_Current";

	private const string VINServiceForCPC = "DL_ID_VIN_Current";

	private static readonly Regex ValidESNCharacters = new Regex("[\\da-zA-Z]");

	private static readonly Regex ValidESNRegex = new Regex("\\A[\\da-zA-Z]{14}\\z");

	private static readonly Regex VinNameRegex = new Regex("\\bVIN\\b");

	private static readonly Regex ValidPinCharacters = new Regex("[\\da-zA-Z-[iIoOqQ]]");

	private Channel mcm;

	private bool setESN = false;

	private bool setPIN = false;

	private EcuInfo ecuInfoESN = null;

	private Channel cpc;

	private Channel acm;

	private bool haveUpdatedESN = false;

	private bool alreadyAskedUser = false;

	private Stage currentStage = Stage.Idle;

	private bool parametersHaveBeenRead = false;

	private bool PINHasBeenSynchronized = false;

	private bool UploadToServerAlso = false;

	private List<Channel> parametersBeingRead = new List<Channel>();

	private Service currentService;

	private Channel executeAsynchronousServicesChannel;

	private string currentServiceList;

	private TableLayoutPanel tableLayoutPanel1;

	private System.Windows.Forms.Label labelPIN;

	private TextBox textBoxESN;

	private Button buttonSetESN;

	private TextBox textBoxOutput;

	private Button buttonSetPIN;

	private TextBox textBoxPIN;

	private Button buttonReadParameters;

	private Button buttonConnectToServer;

	private System.Windows.Forms.Label labelReadParameters;

	private System.Windows.Forms.Label labelUploadToServer;

	private Button buttonClose;

	private System.Windows.Forms.Label labelReadParametersMessage;

	private System.Windows.Forms.Label labelUploadToServerMessage;

	private System.Windows.Forms.Label labelESN;

	public string PinIdentifier
	{
		get
		{
			return textBoxPIN.Text;
		}
		set
		{
			textBoxPIN.Text = value;
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

	private bool Online => (cpc != null && cpc.CommunicationsState == CommunicationsState.Online) || (mcm != null && mcm.CommunicationsState == CommunicationsState.Online) || (acm != null && acm.CommunicationsState == CommunicationsState.Online);

	private bool IsValidLicense => LicenseManager.GlobalInstance.AccessLevel >= 2;

	private bool CanClose => !Working;

	private bool CanSetESN => !Working && mcm != null && mcm.CommunicationsState == CommunicationsState.Online && mcm.Services["DL_ID_Write_Engine_Serial_Number"] != null && IsValidLicense;

	private bool IsValidEsn => CanSetESN && IsSerialNumberValid(textBoxESN.Text);

	private bool CanEditEsn => CanSetESN && ecuInfoESN != null && ecuInfoESN.Value != null;

	private bool CanSetPIN => !Working && Online && IsValidLicense;

	private bool IsValidPIN => CanSetPIN && VehicleIdentification.IsValidPin(textBoxPIN.Text);

	public UserPanel()
	{
		InitializeComponent();
		textBoxESN.TextChanged += OnTextChanged;
		textBoxESN.KeyPress += textBoxESN_KeyPress;
		textBoxPIN.TextChanged += OnTextChanged;
		textBoxPIN.KeyPress += textBoxPIN_KeyPress;
		buttonSetESN.Click += buttonSetESN_Click;
		buttonSetPIN.Click += buttonSetPIN_Click;
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += ParentForm_FormClosing;
		((UserControl)this).OnLoad(e);
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM21T"));
		SetCPC(((CustomPanel)this).GetChannel("CPC04T"));
		SetACM(((CustomPanel)this).GetChannel("ACM21T", (ChannelLookupOptions)5));
		if (string.IsNullOrEmpty(PinIdentifier))
		{
			PinIdentifier = CollectionExtensions.FirstOrDefault<string>(from channel in SapiManager.GlobalInstance.Sapi.Channels
				let id = SapiManager.GetVehicleIdentificationNumber(channel)
				where VehicleIdentification.IsValidPin(id)
				select id, PinIdentifier);
		}
		UpdateUserInterface();
	}

	private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= ParentForm_FormClosing;
			StopWork(Reason.Closing);
			SetMCM(null);
			SetCPC(null);
			SetACM(null);
		}
	}

	private bool SetMCM(Channel mcm)
	{
		bool result = false;
		if (this.mcm != mcm)
		{
			result = true;
			if (CurrentStage != Stage.WaitingForBackOnlineCPC)
			{
				StopWork(Reason.Disconnected);
				ClearOutput();
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
				PinIdentifier = SapiManager.GetVehicleIdentificationNumber(this.cpc);
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

	private void textBoxESN_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (!char.IsControl(e.KeyChar) && !ValidESNCharacters.IsMatch(e.KeyChar.ToString()))
		{
			e.Handled = true;
		}
	}

	private void textBoxPIN_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (!char.IsControl(e.KeyChar) && !ValidPinCharacters.IsMatch(e.KeyChar.ToString()))
		{
			e.Handled = true;
		}
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
			CheckPINandESN();
		}
		else
		{
			textBoxESN.Text = string.Empty;
			if (ecuInfoESN != null)
			{
				ecuInfoESN.Read(synchronous: false);
			}
		}
	}

	private void CheckPINandESN()
	{
		if (IsValidEsn && !IsValidPIN && !alreadyAskedUser && haveUpdatedESN)
		{
			alreadyAskedUser = true;
			DialogResult dialogResult = MessageBox.Show(Resources.Message_UseESNAsPIN, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dialogResult == DialogResult.Yes)
			{
				textBoxPIN.Text = textBoxESN.Text;
			}
		}
	}

	private void UpdateUserInterface()
	{
		UpdateESN(forceUpdate: false);
		buttonReadParameters.Enabled = PINHasBeenSynchronized;
		buttonConnectToServer.Enabled = parametersHaveBeenRead;
		buttonClose.Enabled = CanClose;
		buttonSetESN.Enabled = IsValidEsn;
		buttonSetPIN.Enabled = IsValidPIN;
		textBoxESN.ReadOnly = !CanEditEsn;
		textBoxPIN.ReadOnly = !CanSetPIN;
		if (textBoxESN.ReadOnly)
		{
			textBoxESN.BackColor = SystemColors.Control;
		}
		else
		{
			textBoxESN.BackColor = (IsValidEsn ? Color.PaleGreen : Color.LightPink);
		}
		if (textBoxPIN.ReadOnly)
		{
			textBoxPIN.BackColor = SystemColors.Control;
			return;
		}
		textBoxPIN.BackColor = (IsValidPIN ? Color.PaleGreen : Color.LightPink);
		CheckPINandESN();
	}

	private static bool IsSerialNumberValid(string text)
	{
		return text != null && ValidESNRegex.IsMatch(text);
	}

	private void ClearOutput()
	{
		textBoxOutput.Text = string.Empty;
	}

	private void ReportResult(string text)
	{
		textBoxOutput.Text = textBoxOutput.Text + text + "\r\n";
		textBoxOutput.SelectionStart = textBoxOutput.TextLength;
		textBoxOutput.SelectionLength = 0;
		textBoxOutput.ScrollToCaret();
	}

	private void buttonSetESN_Click(object sender, EventArgs e)
	{
		if (IsValidEsn)
		{
			setESN = true;
			setPIN = false;
			StartWork();
		}
	}

	private void buttonSetPIN_Click(object sender, EventArgs e)
	{
		if (IsValidPIN)
		{
			setESN = false;
			setPIN = true;
			StartWork();
		}
	}

	private void StartWork()
	{
		if (setESN)
		{
			CurrentStage = Stage.SetEsn;
		}
		else if (setPIN)
		{
			CurrentStage = Stage.SetVINForMCM;
		}
		PerformCurrentStage();
	}

	private void PerformCurrentStage()
	{
		switch (CurrentStage)
		{
		case Stage.Idle:
			break;
		case Stage.SetEsn:
		{
			ClearOutput();
			CurrentStage = Stage.WaitingForSetEsn;
			Service service3 = mcm.Services["DL_ID_Write_Engine_Serial_Number"];
			if (service3 == null)
			{
				StopWork(Reason.FailedServiceExecute);
				break;
			}
			string text = textBoxESN.Text;
			ReportResult(Resources.Message_SettingESNTo + text + ".");
			service3.InputValues[0].Value = text;
			ExecuteAsynchronousService(service3);
			break;
		}
		case Stage.WaitingForSetEsn:
			CurrentStage = Stage.CommitMCMForESN;
			PerformCurrentStage();
			break;
		case Stage.CommitMCMForESN:
			if (mcm != null && !SapiManager.GetBootModeStatus(mcm))
			{
				string dereferencedServiceList = mcm.Services.GetDereferencedServiceList("CommitToPermanentMemoryService");
				CurrentStage = Stage.WaitingForCommitMCMForESN;
				ReportResult(Resources.Message_WritingChangesToPermanentMemory);
				ExecuteAsynchronousServices(dereferencedServiceList, mcm);
			}
			else
			{
				ReportResult(Resources.Message_SkippingMCM21TSetPINProcessAsTheMCM21TIsNotConnectedOrInBootMode);
				CurrentStage = Stage.KeyOffOnCPC;
				PerformCurrentStage();
			}
			break;
		case Stage.WaitingForCommitMCMForESN:
			CurrentStage = Stage.KeyOffOnCPC;
			PerformCurrentStage();
			break;
		case Stage.KeyOffOnCPC:
		{
			if (cpc == null)
			{
				ReportResult(Resources.Message_SkippingCPC04TCommitProcessAsTheCPC04TIsNotConnected);
				CurrentStage = Stage.Finish;
				PerformCurrentStage();
				break;
			}
			Service service2 = cpc.Services["FN_KeyOffOnReset"];
			if (service2 == null)
			{
				CurrentStage = Stage.Finish;
				ReportResult(Resources.Message_SkippingCPC04TResetAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForKeyOffOnCPC;
				ReportResult(Resources.Message_SynchronizingESNToCPC04TViaKeyOffOnReset);
				ExecuteAsynchronousService(service2);
			}
			break;
		}
		case Stage.WaitingForKeyOffOnCPC:
			CurrentStage = Stage.WaitingForBackOnlineCPC;
			ReportResult(Resources.Message_WaitingForTheCPC04TToComeBackOnline);
			break;
		case Stage.WaitingForBackOnlineCPC:
			CurrentStage = Stage.Finish;
			PerformCurrentStage();
			break;
		case Stage.SetVINForMCM:
			ClearOutput();
			if (mcm == null)
			{
				ReportResult(Resources.Message_SkippingMCM21TSetPINProcessAsTheMCM21TIsNotConnectedOrInBootMode);
				CurrentStage = Stage.SetVINForACM;
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.UnlockMCMParameterWrite;
				PerformCurrentStage();
			}
			break;
		case Stage.UnlockMCMParameterWrite:
		{
			string dereferencedServiceList3 = mcm.Services.GetDereferencedServiceList("ParameterWriteInitializeService");
			if (mcm != null && !SapiManager.GetBootModeStatus(mcm))
			{
				CurrentStage = Stage.ExecuteMCMVINService;
				ReportResult(string.Format(Resources.Message_Unlocking0ParameterWriteService, mcm.Ecu.Name));
				ExecuteAsynchronousServices(dereferencedServiceList3, mcm);
			}
			else
			{
				ReportResult(Resources.Message_SkippingMCM21TSetPINProcessAsTheMCM21TIsNotConnectedOrInBootMode);
				CurrentStage = Stage.SetVINForACM;
				PerformCurrentStage();
			}
			break;
		}
		case Stage.ExecuteMCMVINService:
		{
			Service service5 = mcm.Services["DL_ID_Write_VIN_Current"];
			if (service5 == null)
			{
				CurrentStage = Stage.SetVINForACM;
				ReportResult(Resources.Message_SkippingSettingOfMCM21TPINAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetMCMVIN;
				ReportResult(string.Format(Resources.Message_Setting1PINTo + PinIdentifier + ".", mcm.Ecu.Name));
				service5.InputValues[0].Value = PinIdentifier;
				ExecuteAsynchronousService(service5);
			}
			break;
		}
		case Stage.WaitingForSetMCMVIN:
			CurrentStage = Stage.SetVINForACM;
			PerformCurrentStage();
			break;
		case Stage.SetVINForACM:
			if (acm == null)
			{
				ReportResult(string.Format(Resources.Message_SkippingACMSetPINProcessAsTheACMIsNotConnectedOrInBootMode, (acm == null) ? "ACM" : acm.Ecu.Name));
				CurrentStage = Stage.SetVINForCPC;
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.UnlockACMParameterWrite;
				PerformCurrentStage();
			}
			break;
		case Stage.UnlockACMParameterWrite:
		{
			string dereferencedServiceList2 = acm.Services.GetDereferencedServiceList("ParameterWriteInitializeService");
			if (acm != null && !SapiManager.GetBootModeStatus(acm))
			{
				CurrentStage = Stage.ExecuteACMVINService;
				ReportResult(string.Format(Resources.Message_Unlocking0ParameterWriteService, acm.Ecu.Name));
				ExecuteAsynchronousServices(dereferencedServiceList2, acm);
			}
			else
			{
				ReportResult(string.Format(Resources.Message_SkippingACMSetPINProcessAsTheACMIsNotConnectedOrInBootMode, (acm == null) ? "ACM21T" : acm.Ecu.Name));
				CurrentStage = Stage.SetVINForCPC;
				PerformCurrentStage();
			}
			break;
		}
		case Stage.ExecuteACMVINService:
		{
			CurrentStage = Stage.WaitingForSetACMVIN;
			Service service = acm.Services["DL_ID_Write_VIN_Current"];
			if (service == null)
			{
				CurrentStage = Stage.SetVINForCPC;
				ReportResult(string.Format(Resources.Message_SkippingSettingOfACMPINAsTheServiceCannotBeFound, (acm == null) ? "ACM" : acm.Ecu.Name));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetACMVIN;
				ReportResult(string.Format(Resources.Message_Setting1PINTo + PinIdentifier + ".", acm.Ecu.Name));
				service.InputValues[0].Value = PinIdentifier;
				ExecuteAsynchronousService(service);
			}
			break;
		}
		case Stage.WaitingForSetACMVIN:
			CurrentStage = Stage.SetVINForCPC;
			PerformCurrentStage();
			break;
		case Stage.SetVINForCPC:
		{
			CurrentStage = Stage.WaitingForSetCPCVIN;
			if (cpc == null)
			{
				ReportResult(string.Format(Resources.Message_SkippingCPC04TSetPINProcessAsTheCPC04TIsNotConnectedOrInBootMode, (cpc == null) ? "CPC04T" : cpc.Ecu.Name));
				PerformCurrentStage();
				break;
			}
			Service service4 = cpc.Services["DL_ID_VIN_Current"];
			if (service4 == null)
			{
				ReportResult(Resources.Message_SkippingSettingOfCPC04TPINAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				ReportResult(string.Format(Resources.Message_Setting1PINTo + PinIdentifier + ".", cpc.Ecu.Name));
				service4.InputValues[0].Value = PinIdentifier;
				ExecuteAsynchronousService(service4);
			}
			break;
		}
		case Stage.WaitingForSetCPCVIN:
			CurrentStage = Stage.Finish;
			PINHasBeenSynchronized = VerifyResults(mcm, "MCM21T", "CO_VIN", "PIN", PinIdentifier);
			PINHasBeenSynchronized &= VerifyResults(cpc, "CPC04T", "CO_VIN", "PIN", PinIdentifier);
			PINHasBeenSynchronized &= VerifyResults(acm, "ACM21T", "CO_VIN", "PIN", PinIdentifier);
			if (PINHasBeenSynchronized)
			{
				DialogResult dialogResult = MessageBox.Show(Resources.Message_ReadParametersAndUpload, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (dialogResult == DialogResult.Yes)
				{
					CurrentStage = Stage.ReadParameters;
					UploadToServerAlso = true;
				}
			}
			PerformCurrentStage();
			break;
		case Stage.ReadParameters:
			CurrentStage = Stage.WaitingForReadParameters;
			parametersBeingRead.Add(acm);
			acm.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
			acm.Parameters.Read(synchronous: false);
			parametersBeingRead.Add(cpc);
			cpc.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
			cpc.Parameters.Read(synchronous: false);
			parametersBeingRead.Add(mcm);
			mcm.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
			mcm.Parameters.Read(synchronous: false);
			break;
		case Stage.WaitingForReadParameters:
			ReportResult(Resources.Message_ParametersReadSuccessfully);
			parametersHaveBeenRead = true;
			CurrentStage = (UploadToServerAlso ? Stage.ConnectToServer : Stage.Finish);
			PerformCurrentStage();
			break;
		case Stage.ConnectToServer:
			if (parametersHaveBeenRead)
			{
				CurrentStage = Stage.WaitingForConnectToServer;
				ReportResult(Resources.Message_ConnectToServer);
				ServerClient.GlobalInstance.Complete += ServerClient_ServerClientCompleteEvent;
				ServerClient.GlobalInstance.Go((Collection<UnitInformation>)null, (Collection<UnitInformation>)null);
			}
			break;
		case Stage.WaitingForConnectToServer:
			CurrentStage = Stage.Finish;
			PerformCurrentStage();
			break;
		case Stage.Finish:
			if (mcm != null)
			{
				mcm.FaultCodes.Reset(synchronous: false);
			}
			if (acm != null)
			{
				acm.FaultCodes.Reset(synchronous: false);
			}
			StopWork(Reason.Succeeded);
			break;
		case Stage.Stopping:
			break;
		default:
			throw new InvalidOperationException("Unknown stage.");
		}
	}

	private void ServerClient_ServerClientCompleteEvent(object sender, ClientConnectionCompleteEventArgs e)
	{
		PerformCurrentStage();
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
			if (setESN)
			{
				VerifyResults(mcm, "MCM21T", "CO_ESN", "ESN", textBoxESN.Text);
				VerifyResults(cpc, "CPC04T", "CO_ESN", "ESN", textBoxESN.Text);
			}
		}
		else
		{
			ReportResult(Resources.Message_TheProcedureFailedToComplete);
			switch (reason)
			{
			case Reason.Disconnected:
				ReportResult(Resources.Message_TheMCM21TWasDisconnected);
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

	private bool VerifyResults(Channel channel, string ecuName, string function, string functionName, string textBoxText)
	{
		bool flag = false;
		string text = ((channel == null) ? ecuName : channel.Ecu.Name);
		if (channel == null || channel.EcuInfos[function] == null)
		{
			ReportResult(Resources.Message_The + text + " " + functionName + Resources.Message_CannotBeVerified);
		}
		else
		{
			EcuInfo ecuInfo = channel.EcuInfos[function];
			try
			{
				ecuInfo.Read(synchronous: true);
			}
			catch (CaesarException ex)
			{
				ReportResult(Resources.Message_FailedToReadECUInfoCannotVerifyThe + text + " " + functionName + Resources.Message_Error + ex.Message);
			}
			catch (InvalidOperationException ex2)
			{
				ReportResult(Resources.Message_FailedToReadECUInfoThe + text + Resources.Message_IsUnavailableError + ex2.Message);
			}
			flag = string.Compare(ecuInfo.Value, textBoxText, ignoreCase: true) == 0;
			if (flag)
			{
				ReportResult(Resources.Message_The1 + text + " " + functionName + Resources.Message_HasSuccessfullyBeenSetTo + textBoxText + ".");
			}
			else
			{
				ReportResult(Resources.Message_The2 + text + " " + functionName + Resources.Message_HasNotBeenChangedAndHasAValueOf + ecuInfo.Value + ".");
			}
		}
		return flag;
	}

	private void ExecuteAsynchronousService(Service service)
	{
		currentService = service;
		currentService.ServiceCompleteEvent += OnServiceComplete;
		ReportResult(Resources.Message_Executing + VinNameRegex.Replace(service.Name, "PIN") + "…");
		currentService.Execute(synchronous: false);
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		ClearCurrentService();
		if (CheckCompleteResult(e, "Service executed", "Service error"))
		{
			PerformCurrentStage();
		}
		else
		{
			StopWork(Reason.FailedService);
		}
	}

	private void ExecuteAsynchronousServices(string serviceList, Channel ch)
	{
		bool flag = false;
		executeAsynchronousServicesChannel = ch;
		if (!string.IsNullOrEmpty(currentServiceList))
		{
			throw new InvalidOperationException("Must wait for current service to finish before continuing.");
		}
		if (string.IsNullOrEmpty(serviceList))
		{
			flag = false;
		}
		else
		{
			currentServiceList = serviceList;
			currentServiceList = currentServiceList.TrimEnd(';');
			List<string> source = currentServiceList.Split(";".ToCharArray()).ToList();
			ReportResult(Resources.Message_ExecutingServices);
			int num = executeAsynchronousServicesChannel.Services.Execute(currentServiceList, synchronous: false);
			if (num != 0 && num == source.Count())
			{
				flag = true;
			}
		}
		if (!flag)
		{
			if (CurrentStage == Stage.ExecuteMCMVINService)
			{
				ReportResult(string.Format(Resources.Message_SkippingSettingOf0PINAsTheServiceCannotBeFound, executeAsynchronousServicesChannel.Ecu.Name));
				CurrentStage = Stage.SetVINForACM;
			}
			else if (CurrentStage == Stage.ExecuteACMVINService)
			{
				ReportResult(string.Format(Resources.Message_SkippingSettingOf0PINAsTheServiceCannotBeFound, executeAsynchronousServicesChannel.Ecu.Name));
				CurrentStage = Stage.SetVINForCPC;
			}
			else if (CurrentStage == Stage.CommitMCMForESN)
			{
				ReportResult(Resources.Message_SkippingMCM21TCommitProcessAsTheServiceCouldNotBeFound);
				CurrentStage = Stage.KeyOffOnCPC;
			}
		}
		currentServiceList = null;
		executeAsynchronousServicesChannel = null;
		PerformCurrentStage();
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
	}

	private void buttonConnectToServer_Click(object sender, EventArgs e)
	{
		CurrentStage = Stage.ConnectToServer;
		PerformCurrentStage();
	}

	private void buttonReadParameters_Click(object sender, EventArgs e)
	{
		CurrentStage = Stage.ReadParameters;
		PerformCurrentStage();
	}

	private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		ParameterCollection parameterCollection = sender as ParameterCollection;
		parameterCollection.Channel.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersReadCompleteEvent;
		if (parameterCollection != null)
		{
			parametersBeingRead.Remove(parameterCollection.Channel);
		}
		if (e.Succeeded)
		{
			ServerDataManager.GlobalInstance.AutoSaveSettings(parameterCollection.Channel, (AutoSaveDestination)1, "ECUREAD");
			ServerDataManager.GlobalInstance.AutoSaveSettings(parameterCollection.Channel, (AutoSaveDestination)0, "ECUUPDATE");
			if (parametersBeingRead.Count == 0)
			{
				PerformCurrentStage();
			}
		}
		else
		{
			StopWork(Reason.FailedParameterRead);
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		labelPIN = new System.Windows.Forms.Label();
		labelESN = new System.Windows.Forms.Label();
		textBoxESN = new TextBox();
		textBoxOutput = new TextBox();
		textBoxPIN = new TextBox();
		buttonSetPIN = new Button();
		buttonSetESN = new Button();
		buttonReadParameters = new Button();
		buttonConnectToServer = new Button();
		labelReadParameters = new System.Windows.Forms.Label();
		labelUploadToServer = new System.Windows.Forms.Label();
		buttonClose = new Button();
		labelReadParametersMessage = new System.Windows.Forms.Label();
		labelUploadToServerMessage = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelPIN, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelESN, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxESN, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxOutput, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxPIN, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSetPIN, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSetESN, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonReadParameters, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonConnectToServer, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelReadParameters, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelUploadToServer, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelReadParametersMessage, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelUploadToServerMessage, 1, 3);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(labelPIN, "labelPIN");
		labelPIN.Name = "labelPIN";
		labelPIN.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelESN, "labelESN");
		labelESN.Name = "labelESN";
		labelESN.UseCompatibleTextRendering = true;
		textBoxESN.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textBoxESN, "textBoxESN");
		textBoxESN.Name = "textBoxESN";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)textBoxOutput, 4);
		componentResourceManager.ApplyResources(textBoxOutput, "textBoxOutput");
		textBoxOutput.Name = "textBoxOutput";
		textBoxOutput.ReadOnly = true;
		textBoxPIN.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textBoxPIN, "textBoxPIN");
		textBoxPIN.Name = "textBoxPIN";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)buttonSetPIN, 2);
		componentResourceManager.ApplyResources(buttonSetPIN, "buttonSetPIN");
		buttonSetPIN.Name = "buttonSetPIN";
		buttonSetPIN.UseCompatibleTextRendering = true;
		buttonSetPIN.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)buttonSetESN, 2);
		componentResourceManager.ApplyResources(buttonSetESN, "buttonSetESN");
		buttonSetESN.Name = "buttonSetESN";
		buttonSetESN.UseCompatibleTextRendering = true;
		buttonSetESN.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)buttonReadParameters, 2);
		componentResourceManager.ApplyResources(buttonReadParameters, "buttonReadParameters");
		buttonReadParameters.Name = "buttonReadParameters";
		buttonReadParameters.UseCompatibleTextRendering = true;
		buttonReadParameters.UseVisualStyleBackColor = true;
		buttonReadParameters.Click += buttonReadParameters_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)buttonConnectToServer, 2);
		componentResourceManager.ApplyResources(buttonConnectToServer, "buttonConnectToServer");
		buttonConnectToServer.Name = "buttonConnectToServer";
		buttonConnectToServer.UseCompatibleTextRendering = true;
		buttonConnectToServer.UseVisualStyleBackColor = true;
		buttonConnectToServer.Click += buttonConnectToServer_Click;
		componentResourceManager.ApplyResources(labelReadParameters, "labelReadParameters");
		labelReadParameters.Name = "labelReadParameters";
		labelReadParameters.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelUploadToServer, "labelUploadToServer");
		labelUploadToServer.Name = "labelUploadToServer";
		labelUploadToServer.UseCompatibleTextRendering = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelReadParametersMessage, "labelReadParametersMessage");
		labelReadParametersMessage.Name = "labelReadParametersMessage";
		labelReadParametersMessage.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelUploadToServerMessage, "labelUploadToServerMessage");
		labelUploadToServerMessage.Name = "labelUploadToServerMessage";
		labelUploadToServerMessage.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Set_Engine_Serial_Number_Product_Identification_Number");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
