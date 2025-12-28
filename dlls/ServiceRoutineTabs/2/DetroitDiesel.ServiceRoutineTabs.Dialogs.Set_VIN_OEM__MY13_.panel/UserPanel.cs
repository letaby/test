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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN_OEM__MY13_.panel;

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
		ExecuteMCMVINService = 9,
		WaitingForSetMCMVIN = 10,
		SetVINForACM = 11,
		ExecuteACMVINService = 12,
		WaitingForSetACMVIN = 13,
		SetVINForCPC = 14,
		WaitingForSetCPCVIN = 15,
		ReadParameters = 16,
		WaitingForReadParameters = 17,
		ConnectToServer = 18,
		WaitingForConnectToServer = 19,
		Finish = 20,
		Stopping = 21,
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

	private const string CpcName = "CPC04T";

	private const string AcmName = "ACM21T";

	private const string ESNEcuInfo = "CO_ESN";

	private const string VINEcuInfo = "CO_VIN";

	private const string EngineSerialNumberService = "DL_ID_Write_Engine_Serial_Number";

	private const string KeyOffOnResetService = "FN_KeyOffOnReset";

	private const string VehicleIdentificationNumberService = "DL_ID_Write_VIN_Current";

	private const string VINServiceForCPC = "DL_ID_VIN_Current";

	private const string ParameterWriteInitializeServiceName = "ParameterWriteInitializeService";

	private static readonly Regex ValidESNCharacters = new Regex("[\\da-zA-Z]");

	private static readonly Regex ValidESNRegex = new Regex("\\A[\\da-zA-Z]{14}\\z");

	private static readonly Regex VinNameRegex = new Regex("\\bVIN\\b");

	private static readonly Regex ValidVinCharacters = new Regex("[\\da-zA-Z-[iIoOqQ]]");

	private Channel mcm;

	private EcuInfo ecuInfoESN = null;

	private Channel cpc;

	private Channel acm;

	private bool haveUpdatedESN = false;

	private Stage currentStage = Stage.Idle;

	private bool parametersHaveBeenRead = false;

	private List<Channel> parametersBeingRead = new List<Channel>();

	private Service currentService;

	private TableLayoutPanel tableLayoutPanel1;

	private System.Windows.Forms.Label labelVIN;

	private TextBox textBoxESN;

	private Button buttonSynchronize;

	private TextBox textBoxVIN;

	private Button buttonClose;

	private SeekTimeListView seekTimeListView;

	private System.Windows.Forms.Label labelESN;

	public string VinIdentifier
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

	private bool Online => (cpc != null && cpc.CommunicationsState == CommunicationsState.Online) || (mcm != null && mcm.CommunicationsState == CommunicationsState.Online) || (acm != null && acm.CommunicationsState == CommunicationsState.Online);

	private bool IsValidLicense => LicenseManager.GlobalInstance.AccessLevel >= 2;

	private bool CanClose => !Working;

	private bool CanSetESN => !Working && mcm != null && mcm.CommunicationsState == CommunicationsState.Online && mcm.Services["DL_ID_Write_Engine_Serial_Number"] != null && IsValidLicense;

	private bool IsValidEsn => CanSetESN && IsSerialNumberValid(textBoxESN.Text);

	private bool CanEditEsn => CanSetESN && ecuInfoESN != null && ecuInfoESN.Value != null;

	private bool CanSetVIN => !Working && Online && IsValidLicense;

	private bool IsValidVIN => CanSetVIN && VehicleIdentification.IsValidVin(textBoxVIN.Text);

	public UserPanel()
	{
		InitializeComponent();
		textBoxESN.TextChanged += OnTextChanged;
		textBoxESN.KeyPress += textBoxESN_KeyPress;
		textBoxVIN.TextChanged += OnTextChanged;
		textBoxVIN.KeyPress += textBoxVIN_KeyPress;
		buttonSynchronize.Click += buttonSynchronize_Click;
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
		SetACM(((CustomPanel)this).GetChannel("ACM21T"));
		if (string.IsNullOrEmpty(VinIdentifier))
		{
			VinIdentifier = CollectionExtensions.FirstOrDefault<string>(from channel in SapiManager.GlobalInstance.Sapi.Channels
				let id = SapiManager.GetVehicleIdentificationNumber(channel)
				where VehicleIdentification.IsValidVin(id)
				select id, VinIdentifier);
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
				VinIdentifier = SapiManager.GetVehicleIdentificationNumber(this.cpc);
			}
		}
		return result;
	}

	private bool SetACM(Channel acm)
	{
		bool result = false;
		if (this.acm != acm)
		{
			result = true;
			this.acm = acm;
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

	private void textBoxVIN_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (!char.IsControl(e.KeyChar) && !ValidVinCharacters.IsMatch(e.KeyChar.ToString()))
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
		buttonClose.Enabled = CanClose;
		buttonSynchronize.Enabled = IsValidEsn && IsValidVIN && !Working;
		textBoxESN.ReadOnly = !CanEditEsn;
		textBoxVIN.ReadOnly = !CanSetVIN;
		if (textBoxESN.ReadOnly)
		{
			textBoxESN.BackColor = SystemColors.Control;
		}
		else
		{
			textBoxESN.BackColor = (IsValidEsn ? Color.PaleGreen : Color.LightPink);
		}
		if (textBoxVIN.ReadOnly)
		{
			textBoxVIN.BackColor = SystemColors.Control;
		}
		else
		{
			textBoxVIN.BackColor = (IsValidVIN ? Color.PaleGreen : Color.LightPink);
		}
	}

	private static bool IsSerialNumberValid(string text)
	{
		return text != null && ValidESNRegex.IsMatch(text);
	}

	private void ReportResult(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
	}

	private void buttonSynchronize_Click(object sender, EventArgs e)
	{
		if (IsValidEsn && IsValidVIN)
		{
			StartWork();
		}
	}

	private void StartWork()
	{
		CurrentStage = Stage.SetEsn;
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
			CurrentStage = Stage.WaitingForSetEsn;
			Service service = mcm.Services["DL_ID_Write_Engine_Serial_Number"];
			if (service == null)
			{
				StopWork(Reason.FailedServiceExecute);
				break;
			}
			string text = textBoxESN.Text;
			ReportResult(Resources.Message_SettingESNTo + text + ".");
			service.InputValues[0].Value = text;
			ExecuteAsynchronousService(service);
			break;
		}
		case Stage.WaitingForSetEsn:
			CurrentStage = Stage.CommitMCMForESN;
			PerformCurrentStage();
			break;
		case Stage.CommitMCMForESN:
			if (mcm.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService") && mcm.Services[mcm.Ecu.Properties["CommitToPermanentMemoryService"]] != null)
			{
				CurrentStage = Stage.WaitingForCommitMCMForESN;
				ReportResult(Resources.Message_WritingChangesToPermanentMemory);
				ExecuteAsynchronousService(mcm.Services[mcm.Ecu.Properties["CommitToPermanentMemoryService"]]);
			}
			else
			{
				ReportResult(Resources.Message_SkippingMCM21TCommitProcessAsTheServiceCouldNotBeFound);
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
			Service service3 = cpc.Services["FN_KeyOffOnReset"];
			if (service3 == null)
			{
				CurrentStage = Stage.Finish;
				ReportResult(Resources.Message_SkippingCPC04TResetAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForKeyOffOnCPC;
				ReportResult(Resources.Message_SynchronizingESNToCPC04TViaKeyOffOnReset);
				ExecuteAsynchronousService(service3);
			}
			break;
		}
		case Stage.WaitingForKeyOffOnCPC:
			CurrentStage = Stage.WaitingForBackOnlineCPC;
			ReportResult(Resources.Message_WaitingForTheCPC04TToComeBackOnline);
			break;
		case Stage.WaitingForBackOnlineCPC:
			CurrentStage = Stage.SetVINForMCM;
			PerformCurrentStage();
			break;
		case Stage.SetVINForMCM:
			if (mcm == null)
			{
				ReportResult(Resources.Message_SkippingMCM21TSetVINProcessAsTheMCM21TIsNotConnected);
				CurrentStage = Stage.SetVINForACM;
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.ExecuteMCMVINService;
				PerformCurrentStage();
			}
			break;
		case Stage.ExecuteMCMVINService:
		{
			CurrentStage = Stage.WaitingForSetMCMVIN;
			Service service4 = mcm.Services["DL_ID_Write_VIN_Current"];
			if (service4 == null)
			{
				CurrentStage = Stage.SetVINForACM;
				ReportResult(Resources.Message_SkippingSettingOfMCM21TVINAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetMCMVIN;
				ReportResult(Resources.Message_SettingVINTo + VinIdentifier + ".");
				service4.InputValues[0].Value = VinIdentifier;
				ExecuteAsynchronousService(service4);
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
				ReportResult(Resources.Message_SkippingACM21TSetVINProcessAsTheACM21TIsNotConnected);
				CurrentStage = Stage.SetVINForCPC;
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.ExecuteACMVINService;
				PerformCurrentStage();
			}
			break;
		case Stage.ExecuteACMVINService:
		{
			CurrentStage = Stage.WaitingForSetACMVIN;
			Service service2 = acm.Services["DL_ID_Write_VIN_Current"];
			if (service2 == null)
			{
				CurrentStage = Stage.SetVINForCPC;
				ReportResult(Resources.Message_SkippingSettingOfACM21TVINAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetACMVIN;
				ReportResult(Resources.Message_SettingVINTo1 + VinIdentifier + ".");
				service2.InputValues[0].Value = VinIdentifier;
				ExecuteAsynchronousService(service2);
			}
			break;
		}
		case Stage.WaitingForSetACMVIN:
			CurrentStage = Stage.SetVINForCPC;
			PerformCurrentStage();
			break;
		case Stage.SetVINForCPC:
		{
			if (cpc == null)
			{
				ReportResult(Resources.Message_SkippingCPC04TSetVINProcessAsTheCPC04TIsNotConnected);
				CurrentStage = Stage.Finish;
				PerformCurrentStage();
				break;
			}
			Service service5 = cpc.Services["DL_ID_VIN_Current"];
			if (service5 == null)
			{
				CurrentStage = Stage.Finish;
				ReportResult(Resources.Message_SkippingSettingOfCPC04TVINAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetCPCVIN;
				ReportResult(Resources.Message_SettingVINTo2 + VinIdentifier + ".");
				service5.InputValues[0].Value = VinIdentifier;
				ExecuteAsynchronousService(service5);
			}
			break;
		}
		case Stage.WaitingForSetCPCVIN:
			CurrentStage = Stage.ReadParameters;
			PerformCurrentStage();
			break;
		case Stage.ReadParameters:
			CurrentStage = Stage.WaitingForReadParameters;
			if (acm != null)
			{
				parametersBeingRead.Add(acm);
				acm.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
				acm.Parameters.Read(synchronous: false);
			}
			if (cpc != null)
			{
				parametersBeingRead.Add(cpc);
				cpc.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
				cpc.Parameters.Read(synchronous: false);
			}
			if (mcm != null)
			{
				parametersBeingRead.Add(mcm);
				mcm.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
				mcm.Parameters.Read(synchronous: false);
			}
			break;
		case Stage.WaitingForReadParameters:
			ReportResult(Resources.Message_ParametersReadSuccessfully);
			parametersHaveBeenRead = true;
			CurrentStage = Stage.ConnectToServer;
			PerformCurrentStage();
			break;
		case Stage.ConnectToServer:
			if (parametersHaveBeenRead)
			{
				CurrentStage = Stage.WaitingForConnectToServer;
				ReportResult(Resources.Message_ConnectToServer);
				ServerClient.GlobalInstance.Complete += ServerClient_ServerClientCompleteEvent;
				Collection<UnitInformation> collection = new Collection<UnitInformation>();
				ServerDataManager.GlobalInstance.GetUploadUnits(collection, (UploadType)0);
				ServerClient.GlobalInstance.Go((Collection<UnitInformation>)null, collection);
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
			VerifyResults(mcm, "MCM21T", "CO_ESN", "ESN", textBoxESN.Text);
			VerifyResults(cpc, "CPC04T", "CO_ESN", "ESN", textBoxESN.Text);
			VerifyResults(acm, "ACM21T", "CO_ESN", "ESN", textBoxESN.Text);
			VerifyResults(mcm, "MCM21T", "CO_VIN", "VIN", textBoxVIN.Text);
			VerifyResults(cpc, "CPC04T", "CO_VIN", "VIN", textBoxVIN.Text);
			VerifyResults(acm, "ACM21T", "CO_VIN", "VIN", textBoxVIN.Text);
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

	private bool VerifyResults(Channel channel, string channelName, string function, string functionName, string textBoxText)
	{
		bool flag = false;
		if (channel == null || channel.EcuInfos[function] == null)
		{
			ReportResult(Resources.Message_The + channelName + " " + functionName + Resources.Message_CannotBeVerified);
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
				ReportResult(Resources.Message_FailedToReadECUInfoCannotVerifyThe + channelName + " " + functionName + Resources.Message_Error + ex.Message);
			}
			catch (InvalidOperationException ex2)
			{
				ReportResult(Resources.Message_FailedToReadECUInfoThe + channelName + Resources.Message_IsUnavailableError + ex2.Message);
			}
			flag = string.Compare(ecuInfo.Value, textBoxText, ignoreCase: true) == 0;
			if (flag)
			{
				ReportResult(Resources.Message_The1 + channelName + " " + functionName + Resources.Message_HasSuccessfullyBeenSetTo + textBoxText + ".");
			}
			else
			{
				ReportResult(Resources.Message_The2 + channelName + " " + functionName + Resources.Message_HasNotBeenChangedAndHasAValueOf + ecuInfo.Value + ".");
			}
		}
		return flag;
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
		ReportResult(Resources.Message_Executing + VinNameRegex.Replace(service.Name, "VIN") + "…");
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
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		labelVIN = new System.Windows.Forms.Label();
		labelESN = new System.Windows.Forms.Label();
		textBoxESN = new TextBox();
		textBoxVIN = new TextBox();
		buttonSynchronize = new Button();
		buttonClose = new Button();
		seekTimeListView = new SeekTimeListView();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelVIN, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelESN, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxESN, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxVIN, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSynchronize, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 0, 2);
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
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)buttonSynchronize, 2);
		componentResourceManager.ApplyResources(buttonSynchronize, "buttonSynchronize");
		buttonSynchronize.Name = "buttonSynchronize";
		buttonSynchronize.UseCompatibleTextRendering = true;
		buttonSynchronize.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView, 4);
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "SetEsnVinOemMy13";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Set_Engine_Serial_Number_Product_Identification_Number");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
