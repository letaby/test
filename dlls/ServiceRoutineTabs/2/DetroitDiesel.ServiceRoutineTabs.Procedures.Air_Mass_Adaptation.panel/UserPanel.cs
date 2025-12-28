using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

public class UserPanel : CustomPanel
{
	private const string RegenFlagInstrumentQualifier = "DT_DS014_DPF_Regen_Flag";

	private const string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";

	private const string CoolantTemperatureInstrumentQualifier = "DT_AS013_Coolant_Temperature";

	private const string OilTemperatureInstrumentQualifier = "DT_AS016_Oil_Temperature";

	private const string MaxEngineSpeedParameterQualifier = "Max_Engine_Speed";

	private const string EngSpeedLimitWhileVehStopParameterQualifier = "Eng_Speed_Limit_While_Veh_Stop";

	private const double RequiredEngineSpeedLimit = 3000.0;

	private const string ResetAdaptationDoneService = "RT_SR014_SET_EOL_Default_Values_Start(7)";

	private const string AirMassFaultUDSCode = "C78D00";

	private const string VINEcuInfo = "CO_VIN";

	private const string ESNEcuInfo = "CO_ESN";

	private const double EngineSpeedTolerance = 0.1;

	private static readonly SetupInformation MBE4000Setup = new SetupInformation("MBE 4000 series", "MBE4000", 60.0, 60.0, new EngineSpeedServicePair[2]
	{
		new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(1700)", "RT_SR015_Idle_Speed_Modification_Stop", 1700, 20),
		new EngineSpeedServicePair("RT_SR002_Engine_Torque_Demand_Substitution_Start_CAN_Torque_Demand(500,28000)", "RT_SR002_Engine_Torque_Demand_Substitution_Stop", 2270, 28)
	}, useAAMAWhenAvailable: true);

	private static readonly SetupInformation MBE900Setup = new SetupInformation("MBE 900 series", "MBE900", 40.0, 40.0, new EngineSpeedServicePair[6]
	{
		new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(1800)", "RT_SR015_Idle_Speed_Modification_Stop", 1800, 20),
		new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(2200)", "RT_SR015_Idle_Speed_Modification_Stop", 2200, 20),
		new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(2050)", "RT_SR015_Idle_Speed_Modification_Stop", 2050, 20),
		new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(2100)", "RT_SR015_Idle_Speed_Modification_Stop", 2100, 20),
		new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(1700)", "RT_SR015_Idle_Speed_Modification_Stop", 1700, 20),
		new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(1850)", "RT_SR015_Idle_Speed_Modification_Stop", 1850, 20)
	}, useAAMAWhenAvailable: false);

	private static readonly List<SetupInformation> Setups = new List<SetupInformation>(new SetupInformation[2] { MBE4000Setup, MBE900Setup });

	private ContinueMessageDialog continueManualOperationDialog = new ContinueMessageDialog(Resources.Message_AutomaticCalibrationOfAirMassAdaptationHasFailedDoYouWantToTryManuallyAdaptingTheNodes, Resources.Message_UserRequestedManualAirMassAdaptation, Resources.Message_UserDeclinedManualAirMassAdaptation);

	private ContinueMessageDialog resetDialog = new ContinueMessageDialog(Resources.Message_SettingTheFaultWillRequireAirMassAdaptationToBePerformed + "\r\n\r\n" + Resources.Message_AreYouSureYouWantToContinue, Resources.Message_UserRequestedFaultResetForAirMassAdaptation, string.Empty);

	private ContinueMessageDialog adjustDialog = new ContinueMessageDialog(Resources.Message_TheMaximumEngineSpeedLimitsMustBeTemporarilyModified, Resources.Message_UserRequestedMaximumEngineSpeedModification, Resources.Message_UserDeclinedMaximumEngineSpeedModification);

	private Channel mcm;

	private Channel cpc;

	private Instrument regenFlag;

	private Instrument engineSpeed;

	private Instrument coolantTemperature;

	private Instrument oilTemperature;

	private Parameter maxEngineSpeed;

	private Parameter engSpeedLimitWhileVehStop;

	private string lastUserNameContent = string.Empty;

	private static readonly Regex ValidUserNameCheck = new Regex("[^0-9]", RegexOptions.Compiled);

	private SetupInformation currentSetup;

	private Service currentService;

	private Timer timerToControlHoldTimes;

	private Timer timerToControlUpdate;

	private bool showUserID;

	private bool weAdjustedMaxEngineSpeed;

	private double originalMaxEngineSpeed;

	private double originalEngSpeedLimitWhileVehStop;

	private string vin;

	private string esn;

	private WarningManager warningManager;

	private DateTime timerEnds;

	private bool accessingCPCParameters = false;

	private BarInstrument BarInstrument1;

	private DigitalReadoutInstrument DigitalReadoutInstrument1;

	private BarInstrument barOilTemperature;

	private BarInstrument barCoolantTemperature;

	private DigitalReadoutInstrument driRegenFlag;

	private DigitalReadoutInstrument driFanStatusPWM06;

	private DialInstrument dialEngineSpeed;

	private BarInstrument barEGRActual;

	private BarInstrument barEGRCommanded;

	private DialInstrument dialVehicleSpeed;

	private TableLayoutPanel tableLayoutPanel1;

	private Label labelMCMMessage;

	private Label labelCPC2Message;

	private Button buttonReset;

	private Label labelFaultMessage;

	private Label labelNoFaultsMessage;

	private Label labelEngineStartedMessage;

	private Label labelRegenMessage;

	private Label labelUserIDMessage;

	private Label labelMaxEngineSpeedMessage;

	private TextBox textboxUserID;

	private Button buttonStart;

	private Button buttonCancel;

	private TableLayoutPanel tableLayoutPanel2;

	private System.Windows.Forms.Label labelWarning;

	private ParameterFile resetFaultParameters;

	private Checkmark checkMcm;

	private Checkmark checkCpc;

	private Checkmark checkFault;

	private Checkmark checkNoFaults;

	private Checkmark checkEngineStarted;

	private Checkmark checkRegen;

	private Checkmark checkUserID;

	private Checkmark checkMaxEngineSpeed;

	private Checkmark checkStartAdaptation;

	private SeekTimeListView seekTimeListView;

	private ScalingLabel labelPanelHeader;

	private bool Working => currentSetup != null && currentSetup.CurrentStep != Step.None;

	private bool MCMOnline => currentSetup != null && mcm.CommunicationsState == CommunicationsState.Online;

	private bool CPCOnline => cpc != null && cpc.CommunicationsState == CommunicationsState.Online;

	private bool Online => CPCOnline && MCMOnline;

	private bool CanStart => !Working && Online && checkMcm.Checked && checkCpc.Checked && checkFault.Checked && checkNoFaults.Checked && checkEngineStarted.Checked && checkRegen.Checked && MaxEngineSpeedReadyForStart && (!showUserID || checkUserID.Checked);

	private bool MaxEngineSpeedReadyForStart
	{
		get
		{
			if (currentSetup != null)
			{
				if (currentSetup.UseAAMA)
				{
					return true;
				}
				return HaveReadMaxEngineSpeed;
			}
			return false;
		}
	}

	private bool CanCancel => Working;

	private bool CanReset => !Working && MCMOnline && !checkFault.Checked;

	private bool HaveReadMaxEngineSpeed => maxEngineSpeed != null && maxEngineSpeed.HasBeenReadFromEcu;

	private bool HaveReadEngSpeedLimitWhileVehStop => engSpeedLimitWhileVehStop != null && engSpeedLimitWhileVehStop.HasBeenReadFromEcu;

	private bool CanChangeUserID => !Working;

	private bool IsUserIDValid => showUserID && textboxUserID != null && !string.IsNullOrEmpty(textboxUserID.Text) && textboxUserID.TextLength == textboxUserID.MaxLength;

	public override bool CanProvideHtml => !Working && seekTimeListView.Text.Count() > 0 && Online;

	public UserPanel()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		InitializeComponent();
		warningManager = new WarningManager(string.Empty, Resources.Message_AirMassAdaptation, seekTimeListView.RequiredUserLabelPrefix);
		buttonStart.Click += OnStartClick;
		buttonCancel.Click += OnCancelClick;
		buttonReset.Click += OnResetClick;
		((Control)(object)labelMaxEngineSpeedMessage).Paint += OnPaint;
		timerToControlUpdate = new Timer();
		timerToControlUpdate.Interval = 500;
		timerToControlUpdate.Enabled = false;
		timerToControlUpdate.Tick += OnParameterReadTimer;
		InitializeUserID();
	}

	private void InitializeUserID()
	{
		if (ApplicationInformation.ProductName.Contains("Freightliner"))
		{
			showUserID = true;
		}
		if (showUserID)
		{
			((Control)(object)checkUserID).Visible = true;
			((Control)(object)labelUserIDMessage).Visible = true;
			textboxUserID.Visible = true;
			textboxUserID.Text = lastUserNameContent;
			textboxUserID.TextChanged += OnUserNameChanged;
			textboxUserID.KeyDown += OnUserNameKeyDown;
		}
		else
		{
			((Control)(object)checkUserID).Visible = false;
			((Control)(object)labelUserIDMessage).Visible = false;
			textboxUserID.Visible = false;
		}
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM"));
		SetCPC2(((CustomPanel)this).GetChannel("CPC2"));
		UpdateUserInterface();
	}

	private void SetMCM(Channel mcm)
	{
		if (this.mcm == mcm)
		{
			return;
		}
		warningManager.Reset();
		StopAdaptation(Result.Disconnected);
		if (this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent -= OnMCMChannelStateUpdate;
			this.mcm.FaultCodes.FaultCodesUpdateEvent -= OnFaultCodesUpdateEvent;
		}
		if (regenFlag != null)
		{
			regenFlag.InstrumentUpdateEvent -= OnRegenFlagUpdate;
			regenFlag = null;
		}
		if (engineSpeed != null)
		{
			engineSpeed.InstrumentUpdateEvent -= OnEngineSpeedUpdate;
			engineSpeed = null;
		}
		this.mcm = mcm;
		if (this.mcm != null)
		{
			this.mcm.CommunicationsStateUpdateEvent += OnMCMChannelStateUpdate;
			this.mcm.FaultCodes.FaultCodesUpdateEvent += OnFaultCodesUpdateEvent;
			regenFlag = this.mcm.Instruments["DT_DS014_DPF_Regen_Flag"];
			if (regenFlag != null)
			{
				regenFlag.InstrumentUpdateEvent += OnRegenFlagUpdate;
			}
			engineSpeed = this.mcm.Instruments["DT_AS010_Engine_Speed"];
			if (engineSpeed != null)
			{
				engineSpeed.InstrumentUpdateEvent += OnEngineSpeedUpdate;
			}
		}
		UpdateCurrentSetup();
	}

	private void SetCPC2(Channel cpc)
	{
		if (this.cpc == cpc)
		{
			return;
		}
		warningManager.Reset();
		StopAdaptation(Result.Disconnected);
		if (this.cpc != null)
		{
			this.cpc.CommunicationsStateUpdateEvent -= OnCPCChannelStateUpdate;
		}
		if (maxEngineSpeed != null)
		{
			maxEngineSpeed.ParameterUpdateEvent -= OnMaxEngineSpeedUpdate;
			maxEngineSpeed = null;
		}
		if (engSpeedLimitWhileVehStop != null)
		{
			engSpeedLimitWhileVehStop.ParameterUpdateEvent -= OnEngSpeedLimitWhileVehStopUpdate;
			engSpeedLimitWhileVehStop = null;
		}
		this.cpc = cpc;
		if (this.cpc != null)
		{
			this.cpc.CommunicationsStateUpdateEvent += OnCPCChannelStateUpdate;
			maxEngineSpeed = this.cpc.Parameters["Max_Engine_Speed"];
			if (maxEngineSpeed != null)
			{
				maxEngineSpeed.ParameterUpdateEvent += OnMaxEngineSpeedUpdate;
			}
			engSpeedLimitWhileVehStop = this.cpc.Parameters["Eng_Speed_Limit_While_Veh_Stop"];
			if (engSpeedLimitWhileVehStop != null)
			{
				engSpeedLimitWhileVehStop.ParameterUpdateEvent += OnEngSpeedLimitWhileVehStopUpdate;
			}
		}
	}

	private void UpdateCurrentSetup()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		SetupInformation setupInformation = null;
		if (mcm != null)
		{
			IEnumerable<EquipmentType> enumerable = EquipmentType.ConnectedEquipmentTypes("Engine");
			if (CollectionExtensions.Exactly<EquipmentType>(enumerable, 1))
			{
				EquipmentType val = enumerable.First();
				string name = ((EquipmentType)(ref val)).Name;
				if (currentSetup == null || name != currentSetup.EngineType)
				{
					foreach (SetupInformation setup in Setups)
					{
						if (name == setup.EngineType)
						{
							setupInformation = setup;
							setupInformation.UseAAMA = setupInformation.UseAAMAWhenAvailable && mcm.Services[setupInformation.AAMAStartService] != null;
							break;
						}
					}
				}
				else
				{
					setupInformation = currentSetup;
				}
			}
		}
		if (setupInformation != currentSetup)
		{
			currentSetup = setupInformation;
		}
	}

	private void UpdateUserInterface()
	{
		UpdateMCMCheck();
		UpdateCPCCheck();
		UpdateFaultCheck();
		UpdateNoFaultsCheck();
		UpdateEngineStartedCheck();
		UpdateRegenCheck();
		UpdateMaxEngineSpeedCheck();
		UpdateUserIDCheck();
		UpdateButtons();
		UpdateWarningMessage();
	}

	private void OnMCMChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateCurrentSetup();
		UpdateUserInterface();
	}

	private void OnCPCChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnFaultCodesUpdateEvent(object sender, ResultEventArgs e)
	{
		if (Working && (currentSetup.CurrentStep == Step.ManualOperation || currentSetup.CurrentStep == Step.StopEngineSpeed) && !IsFaultActive("C78D00"))
		{
			PerformCurrentStep();
		}
		UpdateUserInterface();
	}

	private void OnEngineSpeedUpdate(object sender, ResultEventArgs e)
	{
		if (Working && currentSetup.CurrentStep == Step.WaitEngineSpeed)
		{
			PerformCurrentStep();
		}
		UpdateUserInterface();
	}

	private void OnRegenFlagUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnCoolantOilUpdate(object sender, ResultEventArgs e)
	{
		PerformCurrentStep();
	}

	private void OnMaxEngineSpeedUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnEngSpeedLimitWhileVehStopUpdate(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		ClearCurrentService();
		bool flag = CheckCompleteResult(e, Resources.Message_ServiceExecuted, Resources.Message_ServiceError);
		if (flag)
		{
			Service service = (Service)sender;
			switch (currentSetup.CurrentStep)
			{
			case Step.StartAAMA:
				if (service.OutputValues.Count > 0 && service.OutputValues[0].Type == typeof(Choice))
				{
					ReportResult(string.Format(Resources.MessageFormat_ResultOfStartRoutine0, service.OutputValues[0].Value));
					Choice choice = service.OutputValues[0].Value as Choice;
					if (Convert.ToInt32(choice.RawValue) == 1)
					{
						currentSetup.GotoNextStep();
						PerformCurrentStep();
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
				break;
			case Step.RequestAAMAResults:
				if (service.OutputValues.Count > 0 && service.OutputValues[0].Type == typeof(Choice))
				{
					ReportResult(string.Format(Resources.MessageFormat_ResultOfRequestResultsRoutine0, service.OutputValues[0].Value));
					Choice choice = service.OutputValues[0].Value as Choice;
					switch (Convert.ToInt32(choice.RawValue))
					{
					case 2:
						StartTimer(new TimeSpan(0, 0, 2));
						break;
					case 5:
						currentSetup.GotoStep(Step.AdaptionComplete);
						PerformCurrentStep();
						break;
					case 4:
						currentSetup.GotoNextStep();
						PerformCurrentStep();
						break;
					default:
						flag = false;
						break;
					}
				}
				else
				{
					flag = false;
				}
				break;
			default:
				PerformCurrentStep();
				break;
			}
		}
		if (!flag)
		{
			StopAdaptation(Result.Failed);
		}
	}

	private void OnStartClick(object sender, EventArgs e)
	{
		StartAdaptation();
	}

	private void OnCancelClick(object sender, EventArgs e)
	{
		StopAdaptation(Result.Canceled);
	}

	private void OnResetClick(object sender, EventArgs e)
	{
		ResetFault();
	}

	private void OnTimerToControlHoldTimes(object sender, EventArgs e)
	{
		if (DateTime.Now >= timerEnds)
		{
			StopTimer();
			PerformCurrentStep();
		}
	}

	private void OnUserNameChanged(object sender, EventArgs e)
	{
		if (ValidUserNameCheck.IsMatch(textboxUserID.Text))
		{
			textboxUserID.Text = lastUserNameContent;
			return;
		}
		lastUserNameContent = textboxUserID.Text;
		UpdateUserInterface();
	}

	private void OnUserNameKeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.Back:
		case Keys.End:
		case Keys.Home:
		case Keys.Left:
		case Keys.Up:
		case Keys.Right:
		case Keys.Down:
		case Keys.Delete:
		case Keys.D0:
		case Keys.D1:
		case Keys.D2:
		case Keys.D3:
		case Keys.D4:
		case Keys.D5:
		case Keys.D6:
		case Keys.D7:
		case Keys.D8:
		case Keys.D9:
		case Keys.NumPad0:
		case Keys.NumPad1:
		case Keys.NumPad2:
		case Keys.NumPad3:
		case Keys.NumPad4:
		case Keys.NumPad5:
		case Keys.NumPad6:
		case Keys.NumPad7:
		case Keys.NumPad8:
		case Keys.NumPad9:
			return;
		}
		e.SuppressKeyPress = true;
	}

	private void UpdateMCMCheck()
	{
		bool flag = false;
		string text = Resources.Message_MCMIsNotConnected;
		if (mcm != null)
		{
			if (!MCMOnline)
			{
				text = ((currentSetup != null || mcm.CommunicationsState != CommunicationsState.Online) ? Resources.Message_MCMIsBusy : Resources.Message_MCMIsConnectedButEngineIsNotSupported);
			}
			else
			{
				text = Resources.Message_MCMIsConnectedAndEngineIsSupported;
				flag = true;
			}
		}
		((Control)(object)labelMCMMessage).Text = text;
		checkMcm.Checked = flag;
	}

	private void UpdateCPCCheck()
	{
		bool flag = false;
		string text = Resources.Message_CPC2IsNotConnected;
		if (cpc != null)
		{
			if (CPCOnline)
			{
				text = Resources.Message_CPC2IsConnected;
				flag = true;
			}
			else
			{
				text = Resources.Message_CPC2IsBusy;
			}
		}
		((Control)(object)labelCPC2Message).Text = text;
		checkCpc.Checked = flag;
	}

	private bool IsFaultActive(string udsCode)
	{
		bool result = false;
		if (mcm != null)
		{
			FaultCode faultCode = mcm.FaultCodes[udsCode];
			if (faultCode != null)
			{
				FaultCodeIncident current = faultCode.FaultCodeIncidents.Current;
				if (current != null && current.Active == ActiveStatus.Active)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private void UpdateFaultCheck()
	{
		bool flag = false;
		string text = Resources.Message_CannotDetectIfAirMassAdaptationIsRequired;
		if (mcm != null)
		{
			text = Resources.Message_AirMassAdaptationIsNotRequired;
			if (IsFaultActive("C78D00"))
			{
				text = Resources.Message_AirMassAdaptationIsRequired;
				flag = true;
			}
		}
		((Control)(object)labelFaultMessage).Text = text;
		checkFault.Checked = flag;
	}

	private void UpdateNoFaultsCheck()
	{
		bool flag = false;
		string text = Resources.Message_CannotDetectIfAdditionalFaultsNeedAddressing;
		if (mcm != null)
		{
			Collection<FaultCode> collection = new Collection<FaultCode>();
			mcm.FaultCodes.CopyCurrent(collection);
			if (collection.Count > 1 || (collection.Count == 1 && !IsFaultActive("C78D00")))
			{
				text = Resources.Message_AdditionalFaultsDetectedPleaseAddressTheseBeforePerformingAirMassAdaptation;
			}
			else
			{
				text = Resources.Message_NoOtherFaultsDetected;
				flag = true;
			}
		}
		((Control)(object)labelNoFaultsMessage).Text = text;
		checkNoFaults.Checked = flag;
	}

	private void UpdateEngineStartedCheck()
	{
		bool flag = false;
		string text = Resources.Message_CannotDetectIfEngineIsStarted;
		if (engineSpeed != null && engineSpeed.InstrumentValues.Current != null)
		{
			double num = Convert.ToDouble(engineSpeed.InstrumentValues.Current.Value);
			if (!double.IsNaN(num))
			{
				if (num > 0.0)
				{
					text = Resources.Message_EngineIsStarted;
					flag = true;
				}
				else
				{
					text = Resources.Message_EngineHasNotBeenStartedPleaseStartTheEngine;
				}
			}
		}
		((Control)(object)labelEngineStartedMessage).Text = text;
		checkEngineStarted.Checked = flag;
	}

	private void UpdateRegenCheck()
	{
		bool flag = false;
		string text = Resources.Message_CannotDetectIfDPFRegenerationIsInProgress;
		if (regenFlag != null && regenFlag.InstrumentValues.Current != null)
		{
			double num = Convert.ToDouble(((Choice)regenFlag.InstrumentValues.Current.Value).RawValue);
			if (!double.IsNaN(num))
			{
				if (num == 0.0)
				{
					text = Resources.Message_NotPerformingDPFRegeneration;
					flag = true;
				}
				else
				{
					text = Resources.Message_DPFRegenerationInProgressCannotPerformAirMassAdaptation;
				}
			}
		}
		((Control)(object)labelRegenMessage).Text = text;
		checkRegen.Checked = flag;
	}

	private void UpdateMaxEngineSpeedCheck()
	{
		bool flag = false;
		string text = Resources.Message_CannotDetectMaximumEngineSpeedLimits;
		if (currentSetup != null && currentSetup.UseAAMA)
		{
			flag = true;
			text = Resources.Message_EngineSpeedLimitHandledAutomaticallyByAAMATest;
		}
		else if (maxEngineSpeed != null && engSpeedLimitWhileVehStop != null)
		{
			if (maxEngineSpeed.HasBeenReadFromEcu && engSpeedLimitWhileVehStop.HasBeenReadFromEcu)
			{
				double num = 0.0;
				if (maxEngineSpeed.Value != null)
				{
					num = Convert.ToDouble(maxEngineSpeed.OriginalValue);
				}
				if ((double.IsNaN(num) || num >= 3000.0) && engSpeedLimitWhileVehStop.Value != null)
				{
					num = Convert.ToDouble(engSpeedLimitWhileVehStop.OriginalValue);
				}
				if (!double.IsNaN(num))
				{
					if (num < 3000.0)
					{
						text = string.Format(Resources.MessageFormat_MaximumEngineSpeedLimitsAreBelow0RpmAndWillNeedAdjustment, 3000.0);
						flag = false;
					}
					else if (weAdjustedMaxEngineSpeed)
					{
						text = string.Format(Resources.MessageFormat_MaximumEngineSpeedLimitsHaveBeenTemporarilyAdjustedTo0Rpm, 3000.0);
						flag = false;
					}
					else
					{
						text = string.Format(Resources.MessageFormat_MaximumEngineSpeedLimitsAreAtOrAbove0Rpm, 3000.0);
						flag = true;
					}
				}
			}
			else
			{
				text = Resources.Message_MaximumEngineSpeedLimitsHaveNotBeenRead;
			}
		}
		((Control)(object)labelMaxEngineSpeedMessage).Text = text;
		checkMaxEngineSpeed.Checked = flag;
	}

	private void UpdateUserIDCheck()
	{
		if (showUserID)
		{
			checkUserID.Checked = IsUserIDValid;
			textboxUserID.Enabled = CanChangeUserID;
		}
	}

	private void UpdateButtons()
	{
		checkStartAdaptation.Checked = CanStart;
		buttonStart.Enabled = CanStart;
		buttonCancel.Enabled = CanCancel;
		buttonReset.Enabled = CanReset;
	}

	private void ResetFault()
	{
		if (CanReset && ShowMessageBox(resetDialog))
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				resetFaultParameters.SetParameters(mcm);
				mcm.Parameters.Write(synchronous: true);
			}
			catch (CaesarException ex)
			{
				ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredClearingTheCurrentAdaptation0, ex.Message));
			}
			Cursor.Current = Cursors.Default;
		}
	}

	private void UpdateWarningMessage()
	{
		bool visible = false;
		if (cpc != null && HasUnsentChanges(cpc))
		{
			visible = true;
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

	private void StartAdaptation()
	{
		if (CanStart && warningManager.RequestContinue())
		{
			currentSetup.GotoStep(Step.StartAdaptation);
			PerformCurrentStep();
		}
	}

	private string GetEcuInfoValue(string qualifier)
	{
		string text = string.Empty;
		if (mcm != null)
		{
			EcuInfo ecuInfo = mcm.EcuInfos[qualifier];
			if (ecuInfo != null)
			{
				if (string.IsNullOrEmpty(ecuInfo.Value))
				{
					try
					{
						ecuInfo.Read(synchronous: true);
					}
					catch (CaesarException ex)
					{
						ReportResult(string.Format(Resources.MessageFormat_ErrorRetrieving01, ecuInfo.Name, ex.Message));
					}
				}
				text = ecuInfo.Value;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = Resources.Message_Unknown;
			}
		}
		return text;
	}

	private void PerformCurrentStep()
	{
		((CustomPanel)this).AddStatusMessage(string.Format(Resources.MessageFormat_PerformingStep0, currentSetup.CurrentStep.ToString()));
		switch (currentSetup.CurrentStep)
		{
		case Step.StartAdaptation:
			if (showUserID)
			{
				Label(string.Format(Resources.MessageFormat_AirMassAdaptationStartedUser0, textboxUserID.Text));
			}
			else
			{
				Label(Resources.Message_AirMassAdaptationStarted);
			}
			vin = GetEcuInfoValue("CO_VIN");
			esn = GetEcuInfoValue("CO_ESN");
			currentSetup.GotoNextStep();
			PerformCurrentStep();
			break;
		case Step.SetMaxEngineSpeedLimit:
		{
			ReportResult(Resources.Message_CheckingMaximumEngineSpeedLimit);
			bool flag2 = false;
			originalMaxEngineSpeed = Convert.ToDouble(maxEngineSpeed.OriginalValue);
			if (originalMaxEngineSpeed < 3000.0)
			{
				ReportResult(string.Format(Resources.MessageFormat_0Is1AndRequiresTemporaryAdjustment1, maxEngineSpeed.Name, originalMaxEngineSpeed));
				flag2 = true;
			}
			originalEngSpeedLimitWhileVehStop = Convert.ToDouble(engSpeedLimitWhileVehStop.OriginalValue);
			if (originalEngSpeedLimitWhileVehStop < 3000.0)
			{
				ReportResult(string.Format(Resources.MessageFormat_0Is1AndRequiresTemporaryAdjustment, engSpeedLimitWhileVehStop.Name, originalEngSpeedLimitWhileVehStop));
				flag2 = true;
			}
			if (flag2)
			{
				if (ShowMessageBox(adjustDialog))
				{
					ReportResult(adjustDialog.LabelYes);
					weAdjustedMaxEngineSpeed = true;
					maxEngineSpeed.Value = 3000.0;
					engSpeedLimitWhileVehStop.Value = 3000.0;
					if (WriteCPCParameters(new Parameter[2] { maxEngineSpeed, engSpeedLimitWhileVehStop }))
					{
						currentSetup.GotoNextStep();
						PerformCurrentStep();
					}
					else
					{
						StopAdaptation(Result.Failed);
					}
				}
				else
				{
					ReportResult(adjustDialog.LabelNo);
					StopAdaptation(Result.Canceled);
				}
			}
			else
			{
				ReportResult(Resources.Message_LimitsValid);
				currentSetup.GotoNextStep();
				PerformCurrentStep();
			}
			break;
		}
		case Step.ShutOffFans:
		{
			ReportResult(Resources.Message_ShuttingOffFans);
			bool flag = false;
			try
			{
				ExecuteService(currentSetup.Fan1OffService, synchronous: true);
			}
			catch (CaesarException ex)
			{
				flag = true;
				Label(string.Format(Resources.MessageFormat_ShuttingOffFan1Failed0, ex.Message));
			}
			try
			{
				ExecuteService(currentSetup.Fan2OffService, synchronous: true);
			}
			catch (CaesarException ex)
			{
				flag = true;
				Label(string.Format(Resources.MessageFormat_ShuttingOffFan2Failed0, ex.Message));
			}
			if (flag && DialogResult.No == MessageBox.Show(Resources.Message_TheMCMDoesNotAppearToHaveControlOfTheEngine + "\r\n\r\n" + Resources.Message_AreYouSureYouWishToProceed, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
			{
				StopAdaptation(Result.Canceled);
				break;
			}
			currentSetup.GotoNextStep();
			PerformCurrentStep();
			break;
		}
		case Step.CloseEGRValve:
			ReportResult(Resources.Message_ShuttingOffEGRValve);
			try
			{
				ExecuteService(currentSetup.CloseEGRService, synchronous: true);
			}
			catch (CaesarException ex)
			{
				Label(string.Format(Resources.MessageFormat_ShuttingOffEGRValveFailed0, ex.Message));
			}
			currentSetup.GotoNextStep();
			PerformCurrentStep();
			break;
		case Step.BeginCheckForOperatingConditions:
			coolantTemperature = mcm.Instruments["DT_AS013_Coolant_Temperature"];
			oilTemperature = mcm.Instruments["DT_AS016_Oil_Temperature"];
			if (coolantTemperature != null && oilTemperature != null)
			{
				coolantTemperature.InstrumentUpdateEvent += OnCoolantOilUpdate;
				oilTemperature.InstrumentUpdateEvent += OnCoolantOilUpdate;
				currentSetup.GotoNextStep();
				ReportResult(string.Format(Resources.MessageFormat_WaitingForOperatingCondition01CAnd23C, coolantTemperature.Name, currentSetup.CoolantTemperatureThreshold, oilTemperature.Name, currentSetup.OilTemperatureThreshold));
				PerformCurrentStep();
			}
			else
			{
				coolantTemperature = null;
				oilTemperature = null;
				Label(Resources.Message_FailedToObtainCoolantAndOilTemperaturesAbortingAdaptation);
				StopAdaptation(Result.Failed);
			}
			break;
		case Step.WaitForOperatingConditions:
		{
			if (coolantTemperature.InstrumentValues.Current == null)
			{
				break;
			}
			double num = Convert.ToDouble(coolantTemperature.InstrumentValues.Current.Value);
			if (!double.IsNaN(num) && num >= currentSetup.CoolantTemperatureThreshold && oilTemperature.InstrumentValues.Current != null)
			{
				double num2 = Convert.ToDouble(oilTemperature.InstrumentValues.Current.Value);
				if (!double.IsNaN(num2) && num2 >= currentSetup.OilTemperatureThreshold)
				{
					Label(string.Format(Resources.MessageFormat_ConditionMet01CAnd23C, coolantTemperature.Name, currentSetup.CoolantTemperatureThreshold, oilTemperature.Name, currentSetup.OilTemperatureThreshold));
					ClearOilAndCoolantInstruments();
					currentSetup.GotoNextStep();
					PerformCurrentStep();
				}
			}
			break;
		}
		case Step.DriveEngineSpeed:
			ReportResult(string.Format(Resources.MessageFormat_IncreasingEngineSpeedTo0AndHoldingFor1Seconds, currentSetup.CurrentSpeedPoint.TargetSpeed, currentSetup.CurrentSpeedPoint.HoldTimeSeconds));
			currentSetup.GotoNextStep();
			if (currentSetup.CurrentSpeedPoint != null)
			{
				ExecuteService(currentSetup.CurrentSpeedPoint.StartModificationService, synchronous: false);
			}
			else
			{
				PerformCurrentStep();
			}
			break;
		case Step.WaitEngineSpeed:
			if (currentSetup.CurrentSpeedPoint == null || (engineSpeed.InstrumentValues.Current != null && engineSpeed.InstrumentValues.Current.Value != null && WithinTolerance(Convert.ToDouble(engineSpeed.InstrumentValues.Current.Value), currentSetup.CurrentSpeedPoint.TargetSpeed, 0.1)))
			{
				currentSetup.GotoNextStep();
				PerformCurrentStep();
			}
			break;
		case Step.HoldEngineSpeed:
			currentSetup.GotoNextStep();
			if (currentSetup.CurrentSpeedPoint != null)
			{
				StartTimer(currentSetup.CurrentSpeedPoint.HoldTimeSpan);
			}
			else
			{
				PerformCurrentStep();
			}
			break;
		case Step.StopEngineSpeed:
		{
			EngineSpeedServicePair currentSpeedPoint = currentSetup.CurrentSpeedPoint;
			currentSetup.GotoNextStep();
			if (currentSpeedPoint != null)
			{
				if (!IsFaultActive("C78D00"))
				{
					currentSetup.GotoStep(Step.AdaptionComplete);
				}
				ExecuteService(currentSpeedPoint.StopModificationService, synchronous: false);
			}
			else
			{
				PerformCurrentStep();
			}
			break;
		}
		case Step.StartAAMA:
			ReportResult(string.Format(Resources.Message_CallingAAMAStartRoutine));
			ExecuteService(currentSetup.AAMAStartService, synchronous: false);
			break;
		case Step.RequestAAMAResults:
			ReportResult(string.Format(Resources.Message_CallingAAMARequestResultsRoutine));
			ExecuteService(currentSetup.AAMARequestResultsService, synchronous: false);
			break;
		case Step.ManualOperation:
			if (IsFaultActive("C78D00") && ShowMessageBox(continueManualOperationDialog))
			{
				ReportResult(Resources.Message_PleaseManuallyApplyChangesToTheEngineSpeedInOrderToCompleteTheAdaptation + "\r\n" + Resources.Message_DisconnectFromTheDevicesCompleteTheAdaptationOrPressCancelToFinish);
				break;
			}
			currentSetup.GotoNextStep();
			PerformCurrentStep();
			break;
		case Step.AdaptionComplete:
			currentSetup.GotoNextStep();
			PerformCurrentStep();
			break;
		case Step.OpenEGRValve:
			ReportResult(Resources.Message_ResettingEGRValve);
			currentSetup.GotoNextStep();
			ExecuteService(currentSetup.OpenEGRService, synchronous: false);
			break;
		case Step.TurnOnFans:
			ReportResult(Resources.Message_TurningFansOn);
			try
			{
				ExecuteService(currentSetup.Fan1OnService, synchronous: true);
			}
			catch (CaesarException ex)
			{
				ReportResult(string.Format(Resources.MessageFormat_TurningOnFan1Failed0, ex.Message));
			}
			try
			{
				ExecuteService(currentSetup.Fan2OnService, synchronous: true);
			}
			catch (CaesarException ex)
			{
				ReportResult(string.Format(Resources.MessageFormat_TurningOnFan2Failed0, ex.Message));
			}
			currentSetup.GotoNextStep();
			PerformCurrentStep();
			break;
		case Step.ResetMaxEngineSpeedLimit:
			currentSetup.GotoNextStep();
			ResetMaxEngineSpeed();
			PerformCurrentStep();
			break;
		case Step.CommitToPermanentMemory:
		{
			ReportResult(Resources.Message_FinalizingAdaptation);
			((CustomPanel)this).CommitToPermanentMemory("MCM");
			Result result = Result.Incomplete;
			if (!currentSetup.UseAAMA)
			{
				result = ((!IsFaultActive("C78D00")) ? Result.Success : Result.Failed);
			}
			else
			{
				result = Result.Success;
				ReportResult(Resources.Message_AAMAProcedureOutcomeDeterminedByMCM);
			}
			StopAdaptation(result);
			break;
		}
		case Step.ClearFaults:
			currentSetup.GotoNextStep();
			ClearFaults();
			break;
		}
		UpdateUserInterface();
	}

	private static bool WithinTolerance(double value, int target, double tolerance)
	{
		if (!double.IsNaN(value) && value >= (double)target * (1.0 - tolerance))
		{
			return true;
		}
		return false;
	}

	private void ClearFaults()
	{
		if (mcm != null && mcm.Online)
		{
			mcm.FaultCodes.Reset(synchronous: false);
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

	private void StopAdaptation(Result result)
	{
		StopTimer();
		if (!Working || currentSetup.CurrentStep == Step.None || currentSetup.CurrentStep == Step.ClearFaults)
		{
			return;
		}
		Step currentStep = currentSetup.CurrentStep;
		currentSetup.GotoStep(Step.ClearFaults);
		switch (currentStep)
		{
		case Step.WaitForOperatingConditions:
			ClearOilAndCoolantInstruments();
			break;
		default:
			ClearCurrentService();
			if (!currentSetup.UseAAMA)
			{
				foreach (EngineSpeedServicePair engineSpeedPoint in currentSetup.EngineSpeedPoints)
				{
					try
					{
						ExecuteService(engineSpeedPoint.StopModificationService, synchronous: true);
					}
					catch (CaesarException)
					{
					}
				}
			}
			else if (currentStep == Step.StartAAMA || currentStep == Step.RequestAAMAResults)
			{
				try
				{
					ExecuteService(currentSetup.AAMAStopService, synchronous: true);
				}
				catch (CaesarException)
				{
				}
			}
			try
			{
				ExecuteService(currentSetup.OpenEGRService, synchronous: true);
			}
			catch (CaesarException)
			{
			}
			try
			{
				ExecuteService(currentSetup.Fan1OnService, synchronous: true);
			}
			catch (CaesarException)
			{
			}
			try
			{
				ExecuteService(currentSetup.Fan2OnService, synchronous: true);
			}
			catch (CaesarException)
			{
			}
			if (!currentSetup.UseAAMA)
			{
				ResetMaxEngineSpeed();
			}
			break;
		case Step.CommitToPermanentMemory:
			break;
		}
		if (currentStep != Step.None && currentStep != Step.ClearFaults)
		{
			Label(string.Format(Resources.MessageFormat_AirMassAdaptationProcedureFinishedResult0, result));
		}
		PerformCurrentStep();
	}

	private void ExecuteService(string serviceCall, bool synchronous)
	{
		if (mcm == null || !mcm.Online || !(currentService == null) || string.IsNullOrEmpty(serviceCall))
		{
			return;
		}
		Service service = mcm.Services[serviceCall];
		if (service == null)
		{
			StopAdaptation(Result.Failed);
			return;
		}
		if (!serviceCall.Contains("()") && service.InputValues.Count > 0)
		{
			service.InputValues.ParseValues(serviceCall);
		}
		if (!synchronous)
		{
			currentService = service;
			currentService.ServiceCompleteEvent += OnServiceComplete;
		}
		service.Execute(synchronous);
	}

	private void StartTimer(TimeSpan duration)
	{
		if (timerToControlHoldTimes == null)
		{
			timerToControlHoldTimes = new Timer();
			timerToControlHoldTimes.Interval = 500;
			timerToControlHoldTimes.Tick += OnTimerToControlHoldTimes;
		}
		else
		{
			StopTimer();
		}
		timerEnds = DateTime.Now + duration;
		timerToControlHoldTimes.Start();
	}

	private void StopTimer()
	{
		if (timerToControlHoldTimes != null)
		{
			timerEnds = DateTime.MinValue;
			timerToControlHoldTimes.Stop();
		}
	}

	private void ClearCurrentService()
	{
		if (currentService != null)
		{
			currentService.ServiceCompleteEvent -= OnServiceComplete;
			currentService = null;
		}
	}

	private void ClearOilAndCoolantInstruments()
	{
		if (coolantTemperature != null)
		{
			coolantTemperature.InstrumentUpdateEvent -= OnCoolantOilUpdate;
			coolantTemperature = null;
		}
		if (oilTemperature != null)
		{
			oilTemperature.InstrumentUpdateEvent -= OnCoolantOilUpdate;
			oilTemperature = null;
		}
	}

	private void Label(string label)
	{
		LogEvent(label);
		ReportResult(label);
	}

	private void ReportResult(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, text);
		((CustomPanel)this).AddStatusMessage(text);
	}

	public override string ToHtml()
	{
		StringBuilder stringBuilder = new StringBuilder();
		XmlWriter xmlWriter = PrintHelper.CreateWriter(stringBuilder);
		xmlWriter.WriteStartElement("div");
		xmlWriter.WriteAttributeString("class", "standard");
		xmlWriter.WriteStartElement("p");
		xmlWriter.WriteAttributeString("class", "standard");
		if (showUserID)
		{
			xmlWriter.WriteElementString("b", Resources.Message_User);
			xmlWriter.WriteString(textboxUserID.Text);
			xmlWriter.WriteElementString("br", string.Empty);
		}
		xmlWriter.WriteElementString("b", Resources.Message_VIN);
		xmlWriter.WriteString(vin);
		xmlWriter.WriteElementString("br", string.Empty);
		xmlWriter.WriteElementString("b", Resources.Message_ESN);
		xmlWriter.WriteString(esn);
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteStartElement("p");
		xmlWriter.WriteAttributeString("class", "standard");
		xmlWriter.WriteElementString("b", Resources.Message_Report);
		xmlWriter.WriteElementString("br", string.Empty);
		string[] array = seekTimeListView.Text.Split(new string[2] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			xmlWriter.WriteStartElement("div");
			xmlWriter.WriteStartAttribute("style");
			xmlWriter.WriteString("padding-left:");
			int num = 0;
			string text2 = text;
			foreach (char c in text2)
			{
				if (c != ' ')
				{
					break;
				}
				num++;
			}
			xmlWriter.WriteValue(num * 10);
			xmlWriter.WriteString("px");
			xmlWriter.WriteEndAttribute();
			xmlWriter.WriteString(text);
			xmlWriter.WriteFullEndElement();
		}
		xmlWriter.WriteFullEndElement();
		xmlWriter.WriteFullEndElement();
		xmlWriter.Close();
		return stringBuilder.ToString();
	}

	private void ReadMaxEngineSpeed()
	{
		if (!HaveReadMaxEngineSpeed)
		{
			if (CPCOnline && !accessingCPCParameters)
			{
				accessingCPCParameters = true;
				cpc.Parameters.ParametersReadCompleteEvent += OnParametersReadComplete;
				cpc.Parameters.ReadGroup(maxEngineSpeed.GroupQualifier, fromCache: true, synchronous: false);
			}
			((Control)(object)labelMaxEngineSpeedMessage).Invalidate();
		}
	}

	private bool WriteCPCParameters(Parameter[] parameters)
	{
		bool result = false;
		if (!accessingCPCParameters && (bool)cpc.Extension.Invoke("Unlock", null))
		{
			Cursor.Current = Cursors.WaitCursor;
			accessingCPCParameters = true;
			try
			{
				cpc.Parameters.Write(synchronous: true);
				foreach (Parameter parameter in parameters)
				{
					if (parameter.Exception != null)
					{
						Label(Resources.Message_Modifying + parameter.Name + Resources.Message_ResultedInAnError + parameter.Exception.Message);
					}
				}
				result = true;
			}
			catch (CaesarException ex)
			{
				Label(string.Format(Resources.MessageFormat_ModifyingMaximumEngineSpeedLimitsFailed0, ex.Message));
				result = false;
			}
			accessingCPCParameters = false;
			Cursor.Current = Cursors.Default;
		}
		return result;
	}

	private void ResetMaxEngineSpeed()
	{
		if (weAdjustedMaxEngineSpeed)
		{
			if (maxEngineSpeed != null && engSpeedLimitWhileVehStop != null)
			{
				ReportResult(Resources.Message_ResettingMaximumEngineSpeedLimits);
				maxEngineSpeed.Value = originalMaxEngineSpeed;
				engSpeedLimitWhileVehStop.Value = originalEngSpeedLimitWhileVehStop;
				WriteCPCParameters(new Parameter[2] { maxEngineSpeed, engSpeedLimitWhileVehStop });
			}
			else
			{
				Label(Resources.Message_UnableToResetMaxmimumEngineSpeedLimitsCPC2NotConnected);
				MessageBox.Show(Resources.Message_ItHasNotBeenPossibleToResetTheMaximumEngineSpeedLimitsAsTheCPC2IsNoLongerConnected, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
			weAdjustedMaxEngineSpeed = false;
		}
	}

	private void OnPaint(object sender, PaintEventArgs e)
	{
		if (!HaveReadMaxEngineSpeed)
		{
			timerToControlUpdate.Enabled = true;
		}
	}

	private void OnParameterReadTimer(object sender, EventArgs e)
	{
		timerToControlUpdate.Enabled = false;
		ReadMaxEngineSpeed();
	}

	private void OnParametersReadComplete(object sender, ResultEventArgs e)
	{
		cpc.Parameters.ParametersReadCompleteEvent -= OnParametersReadComplete;
		accessingCPCParameters = false;
	}

	private void LogEvent(string eventText)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, eventText.Trim());
	}

	private bool ShowMessageBox(ContinueMessageDialog continueMessage)
	{
		bool result = false;
		if (DialogResult.Yes == MessageBox.Show(continueMessage.Message, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
		{
			if (!string.IsNullOrEmpty(continueMessage.LabelYes))
			{
				LogEvent(continueMessage.LabelYes);
			}
			result = true;
		}
		else if (!string.IsNullOrEmpty(continueMessage.LabelNo))
		{
			LogEvent(continueMessage.LabelNo);
		}
		return result;
	}

	private void InitializeComponent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1077: Unknown result type (might be due to invalid IL or missing references)
		//IL_1277: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		tableLayoutPanel2 = new TableLayoutPanel();
		tableLayoutPanel1 = new TableLayoutPanel();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		seekTimeListView = new SeekTimeListView();
		labelCPC2Message = new Label();
		labelPanelHeader = new ScalingLabel();
		labelMCMMessage = new Label();
		buttonReset = new Button();
		labelFaultMessage = new Label();
		labelNoFaultsMessage = new Label();
		labelEngineStartedMessage = new Label();
		labelRegenMessage = new Label();
		labelUserIDMessage = new Label();
		labelMaxEngineSpeedMessage = new Label();
		textboxUserID = new TextBox();
		buttonStart = new Button();
		buttonCancel = new Button();
		dialVehicleSpeed = new DialInstrument();
		dialEngineSpeed = new DialInstrument();
		barEGRCommanded = new BarInstrument();
		driFanStatusPWM06 = new DigitalReadoutInstrument();
		barEGRActual = new BarInstrument();
		driRegenFlag = new DigitalReadoutInstrument();
		barCoolantTemperature = new BarInstrument();
		barOilTemperature = new BarInstrument();
		DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
		BarInstrument1 = new BarInstrument();
		labelWarning = new System.Windows.Forms.Label();
		checkMcm = new Checkmark();
		checkCpc = new Checkmark();
		checkFault = new Checkmark();
		checkNoFaults = new Checkmark();
		checkEngineStarted = new Checkmark();
		checkRegen = new Checkmark();
		checkUserID = new Checkmark();
		checkMaxEngineSpeed = new Checkmark();
		checkStartAdaptation = new Checkmark();
		resetFaultParameters = new ParameterFile(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView, 4, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelCPC2Message, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelPanelHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelMCMMessage, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonReset, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelFaultMessage, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelNoFaultsMessage, 1, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelEngineStartedMessage, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelRegenMessage, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelUserIDMessage, 1, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)labelMaxEngineSpeedMessage, 1, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textboxUserID, 3, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonStart, 1, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonCancel, 2, 10);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 12);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelWarning, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkMcm, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkCpc, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkFault, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkNoFaults, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkEngineStarted, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkRegen, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkUserID, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkMaxEngineSpeed, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkStartAdaptation, 0, 10);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "Air Mass Adaption";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetRowSpan((Control)(object)seekTimeListView, 8);
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		seekTimeListView.TimeFormat = "HH:mm:ss.fff";
		labelCPC2Message.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelCPC2Message, "labelCPC2Message");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelCPC2Message, 2);
		((Control)(object)labelCPC2Message).Name = "labelCPC2Message";
		labelCPC2Message.Orientation = (TextOrientation)1;
		labelCPC2Message.ShowBorder = false;
		labelCPC2Message.UseSystemColors = true;
		labelPanelHeader.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelPanelHeader, 5);
		componentResourceManager.ApplyResources(labelPanelHeader, "labelPanelHeader");
		labelPanelHeader.FontGroup = null;
		labelPanelHeader.LineAlignment = StringAlignment.Center;
		((Control)(object)labelPanelHeader).Name = "labelPanelHeader";
		labelMCMMessage.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelMCMMessage, "labelMCMMessage");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelMCMMessage, 2);
		((Control)(object)labelMCMMessage).Name = "labelMCMMessage";
		labelMCMMessage.Orientation = (TextOrientation)1;
		labelMCMMessage.ShowBorder = false;
		labelMCMMessage.UseSystemColors = true;
		componentResourceManager.ApplyResources(buttonReset, "buttonReset");
		buttonReset.Name = "buttonReset";
		buttonReset.UseCompatibleTextRendering = true;
		buttonReset.UseVisualStyleBackColor = true;
		labelFaultMessage.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelFaultMessage, "labelFaultMessage");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelFaultMessage, 2);
		((Control)(object)labelFaultMessage).Name = "labelFaultMessage";
		labelFaultMessage.Orientation = (TextOrientation)1;
		labelFaultMessage.ShowBorder = false;
		labelFaultMessage.UseSystemColors = true;
		labelNoFaultsMessage.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelNoFaultsMessage, "labelNoFaultsMessage");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelNoFaultsMessage, 2);
		((Control)(object)labelNoFaultsMessage).Name = "labelNoFaultsMessage";
		labelNoFaultsMessage.Orientation = (TextOrientation)1;
		labelNoFaultsMessage.ShowBorder = false;
		labelNoFaultsMessage.UseSystemColors = true;
		labelEngineStartedMessage.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelEngineStartedMessage, "labelEngineStartedMessage");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelEngineStartedMessage, 2);
		((Control)(object)labelEngineStartedMessage).Name = "labelEngineStartedMessage";
		labelEngineStartedMessage.Orientation = (TextOrientation)1;
		labelEngineStartedMessage.ShowBorder = false;
		labelEngineStartedMessage.UseSystemColors = true;
		labelRegenMessage.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelRegenMessage, "labelRegenMessage");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelRegenMessage, 2);
		((Control)(object)labelRegenMessage).Name = "labelRegenMessage";
		labelRegenMessage.Orientation = (TextOrientation)1;
		labelRegenMessage.ShowBorder = false;
		labelRegenMessage.UseSystemColors = true;
		labelUserIDMessage.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelUserIDMessage, "labelUserIDMessage");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelUserIDMessage, 2);
		((Control)(object)labelUserIDMessage).Name = "labelUserIDMessage";
		labelUserIDMessage.Orientation = (TextOrientation)1;
		labelUserIDMessage.ShowBorder = false;
		labelUserIDMessage.UseSystemColors = true;
		labelMaxEngineSpeedMessage.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelMaxEngineSpeedMessage, "labelMaxEngineSpeedMessage");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)labelMaxEngineSpeedMessage, 2);
		((Control)(object)labelMaxEngineSpeedMessage).Name = "labelMaxEngineSpeedMessage";
		labelMaxEngineSpeedMessage.Orientation = (TextOrientation)1;
		labelMaxEngineSpeedMessage.ShowBorder = false;
		labelMaxEngineSpeedMessage.UseSystemColors = true;
		componentResourceManager.ApplyResources(textboxUserID, "textboxUserID");
		textboxUserID.Name = "textboxUserID";
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonCancel, "buttonCancel");
		buttonCancel.Name = "buttonCancel";
		buttonCancel.UseCompatibleTextRendering = true;
		buttonCancel.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 5);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)dialVehicleSpeed, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)dialEngineSpeed, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)barEGRCommanded, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)driFanStatusPWM06, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)barEGRActual, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)driRegenFlag, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)barCoolantTemperature, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)barOilTemperature, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)DigitalReadoutInstrument1, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)BarInstrument1, 2, 3);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		dialVehicleSpeed.AngleRange = 135.0;
		dialVehicleSpeed.AngleStart = 180.0;
		componentResourceManager.ApplyResources(dialVehicleSpeed, "dialVehicleSpeed");
		dialVehicleSpeed.FontGroup = null;
		((SingleInstrumentBase)dialVehicleSpeed).FreezeValue = false;
		((SingleInstrumentBase)dialVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "vehicleSpeed");
		((Control)(object)dialVehicleSpeed).Name = "dialVehicleSpeed";
		((TableLayoutPanel)(object)tableLayoutPanel2).SetRowSpan((Control)(object)dialVehicleSpeed, 2);
		((SingleInstrumentBase)dialVehicleSpeed).UnitAlignment = StringAlignment.Near;
		dialEngineSpeed.AngleRange = 135.0;
		dialEngineSpeed.AngleStart = 180.0;
		componentResourceManager.ApplyResources(dialEngineSpeed, "dialEngineSpeed");
		dialEngineSpeed.FontGroup = null;
		((SingleInstrumentBase)dialEngineSpeed).FreezeValue = false;
		((SingleInstrumentBase)dialEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "virtual", "engineSpeed");
		((Control)(object)dialEngineSpeed).Name = "dialEngineSpeed";
		((TableLayoutPanel)(object)tableLayoutPanel2).SetRowSpan((Control)(object)dialEngineSpeed, 2);
		((SingleInstrumentBase)dialEngineSpeed).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barEGRCommanded, "barEGRCommanded");
		barEGRCommanded.FontGroup = null;
		((SingleInstrumentBase)barEGRCommanded).FreezeValue = false;
		((SingleInstrumentBase)barEGRCommanded).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS031_EGR_Commanded_Governor_Value");
		((Control)(object)barEGRCommanded).Name = "barEGRCommanded";
		((AxisSingleInstrumentBase)barEGRCommanded).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)barEGRCommanded).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(driFanStatusPWM06, "driFanStatusPWM06");
		driFanStatusPWM06.FontGroup = null;
		((SingleInstrumentBase)driFanStatusPWM06).FreezeValue = false;
		((SingleInstrumentBase)driFanStatusPWM06).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS068_Fan_PWM06");
		((Control)(object)driFanStatusPWM06).Name = "driFanStatusPWM06";
		((SingleInstrumentBase)driFanStatusPWM06).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barEGRActual, "barEGRActual");
		barEGRActual.FontGroup = null;
		((SingleInstrumentBase)barEGRActual).FreezeValue = false;
		((SingleInstrumentBase)barEGRActual).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS032_EGR_Actual_Valve_Position");
		((Control)(object)barEGRActual).Name = "barEGRActual";
		((AxisSingleInstrumentBase)barEGRActual).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
		((SingleInstrumentBase)barEGRActual).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(driRegenFlag, "driRegenFlag");
		driRegenFlag.FontGroup = null;
		((SingleInstrumentBase)driRegenFlag).FreezeValue = false;
		((SingleInstrumentBase)driRegenFlag).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_DS014_DPF_Regen_Flag");
		((Control)(object)driRegenFlag).Name = "driRegenFlag";
		((SingleInstrumentBase)driRegenFlag).UnitAlignment = StringAlignment.Near;
		barCoolantTemperature.BarOrientation = (ControlOrientation)1;
		barCoolantTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barCoolantTemperature, "barCoolantTemperature");
		barCoolantTemperature.FontGroup = null;
		((SingleInstrumentBase)barCoolantTemperature).FreezeValue = false;
		((SingleInstrumentBase)barCoolantTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "coolantTemp");
		((Control)(object)barCoolantTemperature).Name = "barCoolantTemperature";
		((AxisSingleInstrumentBase)barCoolantTemperature).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetRowSpan((Control)(object)barCoolantTemperature, 2);
		((SingleInstrumentBase)barCoolantTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barCoolantTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barCoolantTemperature).UnitAlignment = StringAlignment.Near;
		barOilTemperature.BarOrientation = (ControlOrientation)1;
		barOilTemperature.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barOilTemperature, "barOilTemperature");
		barOilTemperature.FontGroup = null;
		((SingleInstrumentBase)barOilTemperature).FreezeValue = false;
		((SingleInstrumentBase)barOilTemperature).Instrument = new Qualifier((QualifierTypes)1, "virtual", "oilTemp");
		((Control)(object)barOilTemperature).Name = "barOilTemperature";
		((AxisSingleInstrumentBase)barOilTemperature).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
		((TableLayoutPanel)(object)tableLayoutPanel2).SetRowSpan((Control)(object)barOilTemperature, 2);
		((SingleInstrumentBase)barOilTemperature).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barOilTemperature).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barOilTemperature).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)DigitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
		DigitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)DigitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
		((SingleInstrumentBase)DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)(object)BarInstrument1, 2);
		componentResourceManager.ApplyResources(BarInstrument1, "BarInstrument1");
		BarInstrument1.FontGroup = null;
		((SingleInstrumentBase)BarInstrument1).FreezeValue = false;
		((SingleInstrumentBase)BarInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM", "DT_AS054_Differential_Pressure_Compressor_In");
		((Control)(object)BarInstrument1).Name = "BarInstrument1";
		((SingleInstrumentBase)BarInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelWarning, "labelWarning");
		labelWarning.BackColor = SystemColors.Control;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelWarning, 5);
		labelWarning.ForeColor = Color.Red;
		labelWarning.Name = "labelWarning";
		labelWarning.UseCompatibleTextRendering = true;
		labelWarning.UseMnemonic = false;
		componentResourceManager.ApplyResources(checkMcm, "checkMcm");
		((Control)(object)checkMcm).Name = "checkMcm";
		componentResourceManager.ApplyResources(checkCpc, "checkCpc");
		((Control)(object)checkCpc).Name = "checkCpc";
		componentResourceManager.ApplyResources(checkFault, "checkFault");
		((Control)(object)checkFault).Name = "checkFault";
		componentResourceManager.ApplyResources(checkNoFaults, "checkNoFaults");
		((Control)(object)checkNoFaults).Name = "checkNoFaults";
		componentResourceManager.ApplyResources(checkEngineStarted, "checkEngineStarted");
		((Control)(object)checkEngineStarted).Name = "checkEngineStarted";
		componentResourceManager.ApplyResources(checkRegen, "checkRegen");
		((Control)(object)checkRegen).Name = "checkRegen";
		componentResourceManager.ApplyResources(checkUserID, "checkUserID");
		((Control)(object)checkUserID).Name = "checkUserID";
		componentResourceManager.ApplyResources(checkMaxEngineSpeed, "checkMaxEngineSpeed");
		((Control)(object)checkMaxEngineSpeed).Name = "checkMaxEngineSpeed";
		componentResourceManager.ApplyResources(checkStartAdaptation, "checkStartAdaptation");
		((Control)(object)checkStartAdaptation).Name = "checkStartAdaptation";
		resetFaultParameters.FileContents = componentResourceManager.GetString("resetFaultParameters.FileContents");
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_AirMassAdaptation");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
