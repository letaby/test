using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN__EMG_.panel;

public class UserPanel : CustomPanel
{
	private enum Stage
	{
		Idle = 0,
		SetVINForBMS01T = 1,
		WaitingForSetBMS01TVIN = 2,
		SetVINForBMS201T = 3,
		WaitingForSetBMS201TVIN = 4,
		SetVINForBMS301T = 5,
		WaitingForSetBMS301TVIN = 6,
		SetVINForBMS401T = 7,
		WaitingForSetBMS401TVIN = 8,
		SetVINForBMS501T = 9,
		WaitingForSetBMS501TVIN = 10,
		SetVINForBMS601T = 11,
		WaitingForSetBMS601TVIN = 12,
		SetVINForBMS701T = 13,
		WaitingForSetBMS701TVIN = 14,
		SetVINForBMS801T = 15,
		WaitingForSetBMS801TVIN = 16,
		SetVINForBMS901T = 17,
		WaitingForSetBMS901TVIN = 18,
		SetVINForPTI101T = 19,
		WaitingForSetPTI101TVIN = 20,
		SetVINForPTI201T = 21,
		WaitingForSetPTI201TVIN = 22,
		SetVINForPTI301T = 23,
		WaitingForSetPTI301TVIN = 24,
		SetVINForDCL101T = 25,
		WaitingForSetDCL101TVIN = 26,
		SetVINForEAPU03T = 27,
		WaitingForSetEAPU03TVIN = 28,
		SetVINForDCB01T = 29,
		WaitingForSetDCB01TVIN = 30,
		SetVINForDCB02T = 31,
		WaitingForSetDCB02TVIN = 32,
		SetVINForTCM = 33,
		WaitingForSetTCMVIN = 34,
		SetVINForCPC = 35,
		WaitingForSetCPCVIN = 36,
		HardResetCPC = 37,
		WaitingForHardResetCPC = 38,
		Finish = 39,
		Stopping = 40,
		_StartVIN = 1
	}

	private enum Reason
	{
		Succeeded,
		FailedServiceExecute,
		FailedService,
		Closing
	}

	private const string J1939Name = "J1939-255";

	private const string ECPC01TName = "ECPC01T";

	private const string ETCM01TName = "ETCM01T";

	private const string BMS01TName = "BMS01T";

	private const string BMS201TName = "BMS201T";

	private const string BMS301TName = "BMS301T";

	private const string BMS401TName = "BMS401T";

	private const string BMS501TName = "BMS501T";

	private const string BMS601TName = "BMS601T";

	private const string BMS701TName = "BMS701T";

	private const string BMS801TName = "BMS801T";

	private const string BMS901TName = "BMS901T";

	private const string PTI101TName = "PTI101T";

	private const string PTI201TName = "PTI201T";

	private const string PTI301TName = "PTI301T";

	private const string DCL101TName = "DCL101T";

	private const string EAPU03TName = "EAPU03T";

	private const string DCB01TName = "DCB01T";

	private const string DCB02TName = "DCB02T";

	private const string VINEcuInfo = "CO_VIN";

	private const string HardResetService = "FN_HardReset";

	private const string VehicleIdentificationNumberService = "DL_ID_VIN_Current";

	private Precondition vehicleChargingPrecondition;

	private static readonly Regex ValidESNCharacters = new Regex("(\\w+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

	private bool setVIN = false;

	private Channel ecpc01t;

	private Channel etcm01t;

	private Channel bms01t;

	private Channel bms201t;

	private Channel bms301t;

	private Channel bms401t;

	private Channel bms501t;

	private Channel bms601t;

	private Channel bms701t;

	private Channel bms801t;

	private Channel bms901t;

	private Channel pti101t;

	private Channel pti201t;

	private Channel pti301t;

	private Channel dcl101t;

	private Channel eapu03t;

	private Channel dcb01t;

	private Channel dcb02t;

	private Stage currentStage = Stage.Idle;

	private Service currentService;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonClose;

	private System.Windows.Forms.Label labelVIN;

	private Button buttonSetVIN;

	private TextBox textBoxVIN;

	private System.Windows.Forms.Label labelChargingStatus;

	private TextBox textBoxOutput;

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

	private bool Online => (ecpc01t != null && ecpc01t.CommunicationsState == CommunicationsState.Online) || (etcm01t != null && etcm01t.CommunicationsState == CommunicationsState.Online) || (bms01t != null && bms01t.CommunicationsState == CommunicationsState.Online) || (bms201t != null && bms201t.CommunicationsState == CommunicationsState.Online) || (bms301t != null && bms301t.CommunicationsState == CommunicationsState.Online) || (bms401t != null && bms401t.CommunicationsState == CommunicationsState.Online) || (bms501t != null && bms501t.CommunicationsState == CommunicationsState.Online) || (bms601t != null && bms601t.CommunicationsState == CommunicationsState.Online) || (bms701t != null && bms701t.CommunicationsState == CommunicationsState.Online) || (bms801t != null && bms801t.CommunicationsState == CommunicationsState.Online) || (bms901t != null && bms901t.CommunicationsState == CommunicationsState.Online) || (pti101t != null && pti101t.CommunicationsState == CommunicationsState.Online) || (pti201t != null && pti201t.CommunicationsState == CommunicationsState.Online) || (pti301t != null && pti301t.CommunicationsState == CommunicationsState.Online) || (dcl101t != null && dcl101t.CommunicationsState == CommunicationsState.Online) || (eapu03t != null && eapu03t.CommunicationsState == CommunicationsState.Online) || (dcb01t != null && dcb01t.CommunicationsState == CommunicationsState.Online) || (dcb02t != null && dcb02t.CommunicationsState == CommunicationsState.Online);

	private bool IsValidLicense => LicenseManager.GlobalInstance.AccessLevel >= 2;

	private bool CanClose => !Working;

	private bool CanSetVIN => !Working && Online && IsValidLicense;

	private bool IsValidVIN => CanSetVIN && IsVINValid(textBoxVIN.Text);

	public UserPanel()
	{
		InitializeComponent();
		textBoxVIN.TextChanged += OnTextChanged;
		buttonSetVIN.Click += OnSetVINClick;
		vehicleChargingPrecondition = PreconditionManager.GlobalInstance.Preconditions.FirstOrDefault((Precondition p) => (int)p.PreconditionType == 1);
		vehicleChargingPrecondition.StateChanged += vehicleChargingPrecondition_StateChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		UpdateUserInterface();
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
	}

	public override void OnChannelsChanged()
	{
		SetECPC01T(((CustomPanel)this).GetChannel("ECPC01T"));
		SetETCM01T(((CustomPanel)this).GetChannel("ETCM01T"));
		SetBMS01T(((CustomPanel)this).GetChannel("BMS01T"));
		SetBMS201T(((CustomPanel)this).GetChannel("BMS201T"));
		SetBMS301T(((CustomPanel)this).GetChannel("BMS301T"));
		SetBMS401T(((CustomPanel)this).GetChannel("BMS401T"));
		SetBMS501T(((CustomPanel)this).GetChannel("BMS501T"));
		SetBMS601T(((CustomPanel)this).GetChannel("BMS601T"));
		SetBMS701T(((CustomPanel)this).GetChannel("BMS701T"));
		SetBMS801T(((CustomPanel)this).GetChannel("BMS801T"));
		SetBMS901T(((CustomPanel)this).GetChannel("BMS901T"));
		SetPTI101T(((CustomPanel)this).GetChannel("PTI101T"));
		SetPTI201T(((CustomPanel)this).GetChannel("PTI201T"));
		SetPTI301T(((CustomPanel)this).GetChannel("PTI301T"));
		SetDCL101T(((CustomPanel)this).GetChannel("DCL101T"));
		SetEAPU03T(((CustomPanel)this).GetChannel("EAPU03T"));
		SetDCB01T(((CustomPanel)this).GetChannel("DCB01T"));
		SetDCB02T(((CustomPanel)this).GetChannel("DCB02T"));
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
		UpdateUserInterface();
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			vehicleChargingPrecondition.StateChanged -= vehicleChargingPrecondition_StateChanged;
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			StopWork(Reason.Closing);
			SetECPC01T(null);
			SetETCM01T(null);
			SetBMS01T(null);
			SetBMS201T(null);
			SetBMS301T(null);
			SetBMS401T(null);
			SetBMS501T(null);
			SetBMS601T(null);
			SetBMS701T(null);
			SetBMS801T(null);
			SetBMS901T(null);
			SetPTI101T(null);
			SetPTI201T(null);
			SetPTI301T(null);
			SetDCL101T(null);
			SetEAPU03T(null);
			SetDCB01T(null);
			SetDCB02T(null);
		}
	}

	private bool SetECPC01T(Channel cpc)
	{
		bool result = false;
		if (ecpc01t != cpc)
		{
			if (ecpc01t != null)
			{
				ecpc01t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			ecpc01t = cpc;
			if (ecpc01t != null)
			{
				ecpc01t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetETCM01T(Channel tcm)
	{
		bool result = false;
		if (etcm01t != tcm)
		{
			if (etcm01t != null)
			{
				etcm01t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			etcm01t = tcm;
			if (etcm01t != null)
			{
				etcm01t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetBMS01T(Channel bms)
	{
		bool result = false;
		if (bms01t != bms)
		{
			if (bms01t != null)
			{
				bms01t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			bms01t = bms;
			if (bms01t != null)
			{
				bms01t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetBMS201T(Channel bms)
	{
		bool result = false;
		if (bms201t != bms)
		{
			if (bms201t != null)
			{
				bms201t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			bms201t = bms;
			if (bms201t != null)
			{
				bms201t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetBMS301T(Channel bms)
	{
		bool result = false;
		if (bms301t != bms)
		{
			if (bms301t != null)
			{
				bms301t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			bms301t = bms;
			if (bms301t != null)
			{
				bms301t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetBMS401T(Channel bms)
	{
		bool result = false;
		if (bms401t != bms)
		{
			if (bms401t != null)
			{
				bms401t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			bms401t = bms;
			if (bms401t != null)
			{
				bms401t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetBMS501T(Channel bms)
	{
		bool result = false;
		if (bms501t != bms)
		{
			if (bms501t != null)
			{
				bms501t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			bms501t = bms;
			if (bms501t != null)
			{
				bms501t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetBMS601T(Channel bms)
	{
		bool result = false;
		if (bms601t != bms)
		{
			if (bms601t != null)
			{
				bms601t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			bms601t = bms;
			if (bms601t != null)
			{
				bms601t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetBMS701T(Channel bms)
	{
		bool result = false;
		if (bms701t != bms)
		{
			if (bms701t != null)
			{
				bms701t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			bms701t = bms;
			if (bms701t != null)
			{
				bms701t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetBMS801T(Channel bms)
	{
		bool result = false;
		if (bms801t != bms)
		{
			if (bms801t != null)
			{
				bms801t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			bms801t = bms;
			if (bms801t != null)
			{
				bms801t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetBMS901T(Channel bms)
	{
		bool result = false;
		if (bms901t != bms)
		{
			if (bms901t != null)
			{
				bms901t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			bms901t = bms;
			if (bms901t != null)
			{
				bms901t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetPTI101T(Channel pti)
	{
		bool result = false;
		if (pti101t != pti)
		{
			if (pti101t != null)
			{
				pti101t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			pti101t = pti;
			if (pti101t != null)
			{
				pti101t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetPTI201T(Channel pti)
	{
		bool result = false;
		if (pti201t != pti)
		{
			if (pti201t != null)
			{
				pti201t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			pti201t = pti;
			if (pti201t != null)
			{
				pti201t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetPTI301T(Channel pti)
	{
		bool result = false;
		if (pti301t != pti)
		{
			if (pti301t != null)
			{
				pti301t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			pti301t = pti;
			if (pti301t != null)
			{
				pti301t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetDCL101T(Channel dcl)
	{
		bool result = false;
		if (dcl101t != dcl)
		{
			if (dcl101t != null)
			{
				dcl101t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			dcl101t = dcl;
			if (dcl101t != null)
			{
				dcl101t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetEAPU03T(Channel eapu)
	{
		bool result = false;
		if (eapu03t != eapu)
		{
			if (eapu03t != null)
			{
				eapu03t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			eapu03t = eapu;
			if (eapu03t != null)
			{
				eapu03t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetDCB01T(Channel dcb)
	{
		bool result = false;
		if (dcb01t != dcb)
		{
			if (dcb01t != null)
			{
				dcb01t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			dcb01t = dcb;
			if (dcb01t != null)
			{
				dcb01t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private bool SetDCB02T(Channel dcb)
	{
		bool result = false;
		if (dcb02t != dcb)
		{
			if (dcb02t != null)
			{
				dcb02t.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			result = true;
			dcb02t = dcb;
			if (dcb02t != null)
			{
				dcb02t.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			}
		}
		return result;
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnTextChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		buttonClose.Enabled = CanClose;
		buttonSetVIN.Enabled = IsValidVIN && (int)vehicleChargingPrecondition.State != 2;
		labelChargingStatus.Text = (((int)vehicleChargingPrecondition.State == 2) ? string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_TheVINCannotBeSet0, vehicleChargingPrecondition.Text) : string.Empty);
		textBoxVIN.ReadOnly = !CanSetVIN;
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

	public bool IsVINValid(string text)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(text) && Utility.ValidateVehicleIdentificationNumber(text))
		{
			result = true;
		}
		return result;
	}

	private void ReportResult(string text)
	{
		((CustomPanel)this).LabelLog("SetVIN", text);
		textBoxOutput.AppendText(text + Environment.NewLine);
	}

	private void OnSetVINClick(object sender, EventArgs e)
	{
		if (IsValidVIN)
		{
			setVIN = true;
			StartWork();
		}
	}

	private void StartWork()
	{
		if (setVIN)
		{
			CurrentStage = Stage.SetVINForBMS01T;
		}
		PerformCurrentStage();
	}

	private void PerformCurrentStage()
	{
		switch (CurrentStage)
		{
		case Stage.Idle:
			break;
		case Stage.SetVINForBMS01T:
		{
			if (bms01t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "BMS01T"));
				CurrentStage = Stage.SetVINForBMS201T;
				PerformCurrentStage();
				break;
			}
			Service service14 = bms01t.Services["DL_ID_VIN_Current"];
			if (service14 == null)
			{
				CurrentStage = Stage.SetVINForBMS201T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "BMS01T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetBMS01TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "BMS01T", text));
				service14.InputValues[0].Value = text;
				ExecuteAsynchronousService(service14);
			}
			break;
		}
		case Stage.WaitingForSetBMS01TVIN:
			CurrentStage = Stage.SetVINForBMS201T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForBMS201T:
		{
			if (bms201t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "BMS201T"));
				CurrentStage = Stage.SetVINForBMS301T;
				PerformCurrentStage();
				break;
			}
			Service service7 = bms201t.Services["DL_ID_VIN_Current"];
			if (service7 == null)
			{
				CurrentStage = Stage.SetVINForBMS301T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "BMS201T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetBMS201TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "BMS201T", text));
				service7.InputValues[0].Value = text;
				ExecuteAsynchronousService(service7);
			}
			break;
		}
		case Stage.WaitingForSetBMS201TVIN:
			CurrentStage = Stage.SetVINForBMS301T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForBMS301T:
		{
			if (bms301t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "BMS301T"));
				CurrentStage = Stage.SetVINForBMS401T;
				PerformCurrentStage();
				break;
			}
			Service service16 = bms301t.Services["DL_ID_VIN_Current"];
			if (service16 == null)
			{
				CurrentStage = Stage.SetVINForBMS401T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "BMS301T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetBMS301TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "BMS301T", text));
				service16.InputValues[0].Value = text;
				ExecuteAsynchronousService(service16);
			}
			break;
		}
		case Stage.WaitingForSetBMS301TVIN:
			CurrentStage = Stage.SetVINForBMS401T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForBMS401T:
		{
			if (bms401t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "BMS401T"));
				CurrentStage = Stage.SetVINForBMS501T;
				PerformCurrentStage();
				break;
			}
			Service service2 = bms401t.Services["DL_ID_VIN_Current"];
			if (service2 == null)
			{
				CurrentStage = Stage.SetVINForBMS501T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "BMS401T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetBMS401TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "BMS401T", text));
				service2.InputValues[0].Value = text;
				ExecuteAsynchronousService(service2);
			}
			break;
		}
		case Stage.WaitingForSetBMS401TVIN:
			CurrentStage = Stage.SetVINForBMS501T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForBMS501T:
		{
			if (bms501t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "BMS501T"));
				CurrentStage = Stage.SetVINForBMS601T;
				PerformCurrentStage();
				break;
			}
			Service service10 = bms501t.Services["DL_ID_VIN_Current"];
			if (service10 == null)
			{
				CurrentStage = Stage.SetVINForBMS601T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "BMS501T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetBMS501TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "BMS501T", text));
				service10.InputValues[0].Value = text;
				ExecuteAsynchronousService(service10);
			}
			break;
		}
		case Stage.WaitingForSetBMS501TVIN:
			CurrentStage = Stage.SetVINForBMS601T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForBMS601T:
		{
			if (bms601t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "BMS601T"));
				CurrentStage = Stage.SetVINForBMS701T;
				PerformCurrentStage();
				break;
			}
			Service service19 = bms601t.Services["DL_ID_VIN_Current"];
			if (service19 == null)
			{
				CurrentStage = Stage.SetVINForBMS701T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "BMS601T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetBMS601TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "BMS601T", text));
				service19.InputValues[0].Value = text;
				ExecuteAsynchronousService(service19);
			}
			break;
		}
		case Stage.WaitingForSetBMS601TVIN:
			CurrentStage = Stage.SetVINForBMS701T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForBMS701T:
		{
			if (bms701t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "BMS701T"));
				CurrentStage = Stage.SetVINForBMS801T;
				PerformCurrentStage();
				break;
			}
			Service service11 = bms701t.Services["DL_ID_VIN_Current"];
			if (service11 == null)
			{
				CurrentStage = Stage.SetVINForBMS801T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "BMS701T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetBMS701TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "BMS701T", text));
				service11.InputValues[0].Value = text;
				ExecuteAsynchronousService(service11);
			}
			break;
		}
		case Stage.WaitingForSetBMS701TVIN:
			CurrentStage = Stage.SetVINForBMS801T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForBMS801T:
		{
			if (bms801t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "BMS801T"));
				CurrentStage = Stage.SetVINForBMS901T;
				PerformCurrentStage();
				break;
			}
			Service service5 = bms801t.Services["DL_ID_VIN_Current"];
			if (service5 == null)
			{
				CurrentStage = Stage.SetVINForBMS901T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "BMS801T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetBMS801TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "BMS801T", text));
				service5.InputValues[0].Value = text;
				ExecuteAsynchronousService(service5);
			}
			break;
		}
		case Stage.WaitingForSetBMS801TVIN:
			CurrentStage = Stage.SetVINForBMS901T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForBMS901T:
		{
			if (bms901t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "BMS901T"));
				CurrentStage = Stage.SetVINForPTI101T;
				PerformCurrentStage();
				break;
			}
			Service service17 = bms901t.Services["DL_ID_VIN_Current"];
			if (service17 == null)
			{
				CurrentStage = Stage.SetVINForPTI101T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "BMS901T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetBMS901TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "BMS901T", text));
				service17.InputValues[0].Value = text;
				ExecuteAsynchronousService(service17);
			}
			break;
		}
		case Stage.WaitingForSetBMS901TVIN:
			CurrentStage = Stage.SetVINForPTI101T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForPTI101T:
		{
			if (pti101t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "PTI101T"));
				CurrentStage = Stage.SetVINForPTI201T;
				PerformCurrentStage();
				break;
			}
			Service service13 = pti101t.Services["DL_ID_VIN_Current"];
			if (service13 == null)
			{
				CurrentStage = Stage.SetVINForPTI201T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "PTI101T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetPTI101TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "PTI101T", text));
				service13.InputValues[0].Value = text;
				ExecuteAsynchronousService(service13);
			}
			break;
		}
		case Stage.WaitingForSetPTI101TVIN:
			CurrentStage = Stage.SetVINForPTI201T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForPTI201T:
		{
			if (pti201t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "PTI201T"));
				CurrentStage = Stage.SetVINForPTI301T;
				PerformCurrentStage();
				break;
			}
			Service service8 = pti201t.Services["DL_ID_VIN_Current"];
			if (service8 == null)
			{
				CurrentStage = Stage.SetVINForPTI301T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "PTI201T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetPTI201TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "PTI201T", text));
				service8.InputValues[0].Value = text;
				ExecuteAsynchronousService(service8);
			}
			break;
		}
		case Stage.WaitingForSetPTI201TVIN:
			CurrentStage = Stage.SetVINForPTI301T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForPTI301T:
		{
			if (pti301t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "PTI301T"));
				CurrentStage = Stage.SetVINForDCL101T;
				PerformCurrentStage();
				break;
			}
			Service service4 = pti301t.Services["DL_ID_VIN_Current"];
			if (service4 == null)
			{
				CurrentStage = Stage.SetVINForDCL101T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "PTI301T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetPTI301TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "PTI301T", text));
				service4.InputValues[0].Value = text;
				ExecuteAsynchronousService(service4);
			}
			break;
		}
		case Stage.WaitingForSetPTI301TVIN:
			CurrentStage = Stage.SetVINForDCL101T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForDCL101T:
		{
			if (dcl101t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "DCL101T"));
				CurrentStage = Stage.SetVINForEAPU03T;
				PerformCurrentStage();
				break;
			}
			Service service18 = dcl101t.Services["DL_ID_VIN_Current"];
			if (service18 == null)
			{
				CurrentStage = Stage.SetVINForEAPU03T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "DCL101T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetDCL101TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "DCL101T", text));
				service18.InputValues[0].Value = text;
				ExecuteAsynchronousService(service18);
			}
			break;
		}
		case Stage.WaitingForSetDCL101TVIN:
			CurrentStage = Stage.SetVINForEAPU03T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForEAPU03T:
		{
			if (eapu03t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "EAPU03T"));
				CurrentStage = Stage.SetVINForDCB01T;
				PerformCurrentStage();
				break;
			}
			Service service15 = eapu03t.Services["DL_ID_VIN_Current"];
			if (service15 == null)
			{
				CurrentStage = Stage.SetVINForDCB01T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "EAPU03T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetEAPU03TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "EAPU03T", text));
				service15.InputValues[0].Value = text;
				ExecuteAsynchronousService(service15);
			}
			break;
		}
		case Stage.WaitingForSetEAPU03TVIN:
			CurrentStage = Stage.SetVINForDCB01T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForDCB01T:
		{
			if (dcb01t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "DCB01T"));
				CurrentStage = Stage.SetVINForDCB02T;
				PerformCurrentStage();
				break;
			}
			Service service12 = dcb01t.Services["DL_ID_VIN_Current"];
			if (service12 == null)
			{
				CurrentStage = Stage.SetVINForDCB02T;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "DCB01T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetDCB01TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "DCB01T", text));
				service12.InputValues[0].Value = text;
				ExecuteAsynchronousService(service12);
			}
			break;
		}
		case Stage.WaitingForSetDCB01TVIN:
			CurrentStage = Stage.SetVINForDCB02T;
			PerformCurrentStage();
			break;
		case Stage.SetVINForDCB02T:
		{
			if (dcb02t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "DCB02T"));
				CurrentStage = Stage.SetVINForTCM;
				PerformCurrentStage();
				break;
			}
			Service service9 = dcb02t.Services["DL_ID_VIN_Current"];
			if (service9 == null)
			{
				CurrentStage = Stage.SetVINForTCM;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "DCB02T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetDCB02TVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "DCB02T", text));
				service9.InputValues[0].Value = text;
				ExecuteAsynchronousService(service9);
			}
			break;
		}
		case Stage.WaitingForSetDCB02TVIN:
			CurrentStage = Stage.SetVINForTCM;
			PerformCurrentStage();
			break;
		case Stage.SetVINForTCM:
		{
			if (etcm01t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "ETCM01T"));
				CurrentStage = Stage.SetVINForCPC;
				PerformCurrentStage();
				break;
			}
			Service service6 = etcm01t.Services["DL_ID_VIN_Current"];
			if (service6 == null)
			{
				CurrentStage = Stage.SetVINForCPC;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "ETCM01T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetTCMVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "ETCM01T", text));
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
			if (ecpc01t == null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0As0NotConnected, "ECPC01T"));
				CurrentStage = Stage.Finish;
				PerformCurrentStage();
				break;
			}
			Service service3 = ecpc01t.Services["DL_ID_VIN_Current"];
			if (service3 == null)
			{
				CurrentStage = Stage.Finish;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Skipping0ServiceNotFound, "ECPC01T"));
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForSetCPCVIN;
				string text = textBoxVIN.Text;
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_Setting0VINTo1, "ETCM01T", text));
				service3.InputValues[0].Value = text;
				ExecuteAsynchronousService(service3);
			}
			break;
		}
		case Stage.WaitingForSetCPCVIN:
			CurrentStage = Stage.HardResetCPC;
			PerformCurrentStage();
			break;
		case Stage.HardResetCPC:
		{
			Service service = ecpc01t.Services["FN_HardReset"];
			if (service == null)
			{
				CurrentStage = Stage.Finish;
				ReportResult(Resources.Message_SkippingCPCResetAsTheServiceCannotBeFound);
				PerformCurrentStage();
			}
			else
			{
				CurrentStage = Stage.WaitingForHardResetCPC;
				ReportResult(Resources.Message_CommittingChangesToCPCViaHardReset);
				ExecuteAsynchronousService(service);
			}
			break;
		}
		case Stage.WaitingForHardResetCPC:
			CurrentStage = Stage.Finish;
			PerformCurrentStage();
			break;
		case Stage.Finish:
		{
			if (ecpc01t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "ECPC01T"));
				ecpc01t.FaultCodes.Reset(synchronous: false);
			}
			if (etcm01t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "ETCM01T"));
				etcm01t.FaultCodes.Reset(synchronous: false);
			}
			if (bms01t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "BMS01T"));
				bms01t.FaultCodes.Reset(synchronous: false);
			}
			if (bms201t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "BMS201T"));
				bms201t.FaultCodes.Reset(synchronous: false);
			}
			if (bms301t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "BMS301T"));
				bms301t.FaultCodes.Reset(synchronous: false);
			}
			if (bms401t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "BMS401T"));
				bms401t.FaultCodes.Reset(synchronous: false);
			}
			if (bms501t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "BMS501T"));
				bms501t.FaultCodes.Reset(synchronous: false);
			}
			if (bms601t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "BMS601T"));
				bms601t.FaultCodes.Reset(synchronous: false);
			}
			if (bms701t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "BMS701T"));
				bms701t.FaultCodes.Reset(synchronous: false);
			}
			if (bms801t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "BMS801T"));
				bms801t.FaultCodes.Reset(synchronous: false);
			}
			if (bms901t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "BMS901T"));
				bms901t.FaultCodes.Reset(synchronous: false);
			}
			if (pti101t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "PTI101T"));
				pti101t.FaultCodes.Reset(synchronous: false);
			}
			if (pti201t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "PTI201T"));
				pti201t.FaultCodes.Reset(synchronous: false);
			}
			if (pti301t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "PTI301T"));
				pti301t.FaultCodes.Reset(synchronous: false);
			}
			if (dcl101t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "DCL101T"));
				dcl101t.FaultCodes.Reset(synchronous: false);
			}
			if (eapu03t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "EAPU03T"));
				eapu03t.FaultCodes.Reset(synchronous: false);
			}
			if (dcb01t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "DCB01T"));
				dcb01t.FaultCodes.Reset(synchronous: false);
			}
			if (dcb02t != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "DCB02T"));
				dcb02t.FaultCodes.Reset(synchronous: false);
			}
			Channel channel = ((CustomPanel)this).GetChannel("J1939-255");
			if (channel != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Resetting0Faults, "J1939-255"));
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
			if (setVIN)
			{
				VerifyResults(ecpc01t, "ECPC01T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(etcm01t, "ETCM01T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(bms01t, "BMS01T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(bms201t, "BMS201T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(bms301t, "BMS301T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(bms401t, "BMS401T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(bms501t, "BMS501T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(bms601t, "BMS601T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(bms701t, "BMS701T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(bms801t, "BMS801T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(bms901t, "BMS901T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(pti101t, "PTI101T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(pti201t, "PTI201T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(pti301t, "PTI301T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(dcl101t, "DCL101T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(eapu03t, "EAPU03T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(dcb01t, "DCB01T", "CO_VIN", "VIN", textBoxVIN.Text);
				VerifyResults(dcb02t, "DCB02T", "CO_VIN", "VIN", textBoxVIN.Text);
			}
		}
		else
		{
			ReportResult(Resources.Message_TheProcedureFailedToComplete);
			switch (reason)
			{
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

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		ClearCurrentService();
		if (CheckCompleteResult(e, Resources.Message_ServiceExecuted, Resources.Message_ServiceError))
		{
			PerformCurrentStage();
			return;
		}
		ReportResult(Resources.Message_FailedToExecuteService);
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

	private void vehicleChargingPrecondition_StateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		labelVIN = new System.Windows.Forms.Label();
		textBoxVIN = new TextBox();
		buttonSetVIN = new Button();
		buttonClose = new Button();
		textBoxOutput = new TextBox();
		labelChargingStatus = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelVIN, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxVIN, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSetVIN, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBoxOutput, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelChargingStatus, 0, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(labelVIN, "labelVIN");
		labelVIN.Name = "labelVIN";
		labelVIN.UseCompatibleTextRendering = true;
		textBoxVIN.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textBoxVIN, "textBoxVIN");
		textBoxVIN.Name = "textBoxVIN";
		componentResourceManager.ApplyResources(buttonSetVIN, "buttonSetVIN");
		buttonSetVIN.Name = "buttonSetVIN";
		buttonSetVIN.UseCompatibleTextRendering = true;
		buttonSetVIN.UseVisualStyleBackColor = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)textBoxOutput, 3);
		componentResourceManager.ApplyResources(textBoxOutput, "textBoxOutput");
		textBoxOutput.Name = "textBoxOutput";
		textBoxOutput.ReadOnly = true;
		componentResourceManager.ApplyResources(labelChargingStatus, "labelChargingStatus");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelChargingStatus, 3);
		labelChargingStatus.ForeColor = Color.Red;
		labelChargingStatus.Name = "labelChargingStatus";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SetESN");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
