// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;
using SapiLayer1;
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

#nullable disable
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
  }, true);
  private static readonly SetupInformation MBE900Setup = new SetupInformation("MBE 900 series", "MBE900", 40.0, 40.0, new EngineSpeedServicePair[6]
  {
    new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(1800)", "RT_SR015_Idle_Speed_Modification_Stop", 1800, 20),
    new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(2200)", "RT_SR015_Idle_Speed_Modification_Stop", 2200, 20),
    new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(2050)", "RT_SR015_Idle_Speed_Modification_Stop", 2050, 20),
    new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(2100)", "RT_SR015_Idle_Speed_Modification_Stop", 2100, 20),
    new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(1700)", "RT_SR015_Idle_Speed_Modification_Stop", 1700, 20),
    new EngineSpeedServicePair("RT_SR015_Idle_Speed_Modification_Start(1850)", "RT_SR015_Idle_Speed_Modification_Stop", 1850, 20)
  }, false);
  private static readonly List<SetupInformation> Setups = new List<SetupInformation>((IEnumerable<SetupInformation>) new SetupInformation[2]
  {
    UserPanel.MBE4000Setup,
    UserPanel.MBE900Setup
  });
  private ContinueMessageDialog continueManualOperationDialog = new ContinueMessageDialog(Resources.Message_AutomaticCalibrationOfAirMassAdaptationHasFailedDoYouWantToTryManuallyAdaptingTheNodes, Resources.Message_UserRequestedManualAirMassAdaptation, Resources.Message_UserDeclinedManualAirMassAdaptation);
  private ContinueMessageDialog resetDialog = new ContinueMessageDialog($"{Resources.Message_SettingTheFaultWillRequireAirMassAdaptationToBePerformed}\r\n\r\n{Resources.Message_AreYouSureYouWantToContinue}", Resources.Message_UserRequestedFaultResetForAirMassAdaptation, string.Empty);
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
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelMCMMessage;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelCPC2Message;
  private Button buttonReset;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFaultMessage;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelNoFaultsMessage;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelEngineStartedMessage;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelRegenMessage;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelUserIDMessage;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelMaxEngineSpeedMessage;
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

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningManager = new WarningManager(string.Empty, Resources.Message_AirMassAdaptation, this.seekTimeListView.RequiredUserLabelPrefix);
    this.buttonStart.Click += new EventHandler(this.OnStartClick);
    this.buttonCancel.Click += new EventHandler(this.OnCancelClick);
    this.buttonReset.Click += new EventHandler(this.OnResetClick);
    ((Control) this.labelMaxEngineSpeedMessage).Paint += new PaintEventHandler(this.OnPaint);
    this.timerToControlUpdate = new Timer();
    this.timerToControlUpdate.Interval = 500;
    this.timerToControlUpdate.Enabled = false;
    this.timerToControlUpdate.Tick += new EventHandler(this.OnParameterReadTimer);
    this.InitializeUserID();
  }

  private void InitializeUserID()
  {
    if (ApplicationInformation.ProductName.Contains("Freightliner"))
      this.showUserID = true;
    if (this.showUserID)
    {
      ((Control) this.checkUserID).Visible = true;
      ((Control) this.labelUserIDMessage).Visible = true;
      this.textboxUserID.Visible = true;
      this.textboxUserID.Text = this.lastUserNameContent;
      this.textboxUserID.TextChanged += new EventHandler(this.OnUserNameChanged);
      this.textboxUserID.KeyDown += new KeyEventHandler(this.OnUserNameKeyDown);
    }
    else
    {
      ((Control) this.checkUserID).Visible = false;
      ((Control) this.labelUserIDMessage).Visible = false;
      this.textboxUserID.Visible = false;
    }
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM"));
    this.SetCPC2(this.GetChannel("CPC2"));
    this.UpdateUserInterface();
  }

  private void SetMCM(Channel mcm)
  {
    if (this.mcm == mcm)
      return;
    this.warningManager.Reset();
    this.StopAdaptation(Result.Disconnected);
    if (this.mcm != null)
    {
      this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnMCMChannelStateUpdate);
      this.mcm.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.OnFaultCodesUpdateEvent);
    }
    if (this.regenFlag != (Instrument) null)
    {
      this.regenFlag.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnRegenFlagUpdate);
      this.regenFlag = (Instrument) null;
    }
    if (this.engineSpeed != (Instrument) null)
    {
      this.engineSpeed.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
      this.engineSpeed = (Instrument) null;
    }
    this.mcm = mcm;
    if (this.mcm != null)
    {
      this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnMCMChannelStateUpdate);
      this.mcm.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.OnFaultCodesUpdateEvent);
      this.regenFlag = this.mcm.Instruments["DT_DS014_DPF_Regen_Flag"];
      if (this.regenFlag != (Instrument) null)
        this.regenFlag.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnRegenFlagUpdate);
      this.engineSpeed = this.mcm.Instruments["DT_AS010_Engine_Speed"];
      if (this.engineSpeed != (Instrument) null)
        this.engineSpeed.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnEngineSpeedUpdate);
    }
    this.UpdateCurrentSetup();
  }

  private void SetCPC2(Channel cpc)
  {
    if (this.cpc == cpc)
      return;
    this.warningManager.Reset();
    this.StopAdaptation(Result.Disconnected);
    if (this.cpc != null)
      this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCPCChannelStateUpdate);
    if (this.maxEngineSpeed != null)
    {
      this.maxEngineSpeed.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.OnMaxEngineSpeedUpdate);
      this.maxEngineSpeed = (Parameter) null;
    }
    if (this.engSpeedLimitWhileVehStop != null)
    {
      this.engSpeedLimitWhileVehStop.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.OnEngSpeedLimitWhileVehStopUpdate);
      this.engSpeedLimitWhileVehStop = (Parameter) null;
    }
    this.cpc = cpc;
    if (this.cpc != null)
    {
      this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCPCChannelStateUpdate);
      this.maxEngineSpeed = this.cpc.Parameters["Max_Engine_Speed"];
      if (this.maxEngineSpeed != null)
        this.maxEngineSpeed.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.OnMaxEngineSpeedUpdate);
      this.engSpeedLimitWhileVehStop = this.cpc.Parameters["Eng_Speed_Limit_While_Veh_Stop"];
      if (this.engSpeedLimitWhileVehStop != null)
        this.engSpeedLimitWhileVehStop.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.OnEngSpeedLimitWhileVehStopUpdate);
    }
  }

  private bool Working => this.currentSetup != null && this.currentSetup.CurrentStep != Step.None;

  private bool MCMOnline
  {
    get => this.currentSetup != null && this.mcm.CommunicationsState == CommunicationsState.Online;
  }

  private bool CPCOnline
  {
    get => this.cpc != null && this.cpc.CommunicationsState == CommunicationsState.Online;
  }

  private bool Online => this.CPCOnline && this.MCMOnline;

  private bool CanStart
  {
    get
    {
      return !this.Working && this.Online && this.checkMcm.Checked && this.checkCpc.Checked && this.checkFault.Checked && this.checkNoFaults.Checked && this.checkEngineStarted.Checked && this.checkRegen.Checked && this.MaxEngineSpeedReadyForStart && (!this.showUserID || this.checkUserID.Checked);
    }
  }

  private bool MaxEngineSpeedReadyForStart
  {
    get
    {
      if (this.currentSetup == null)
        return false;
      return this.currentSetup.UseAAMA || this.HaveReadMaxEngineSpeed;
    }
  }

  private bool CanCancel => this.Working;

  private bool CanReset => !this.Working && this.MCMOnline && !this.checkFault.Checked;

  private bool HaveReadMaxEngineSpeed
  {
    get => this.maxEngineSpeed != null && this.maxEngineSpeed.HasBeenReadFromEcu;
  }

  private bool HaveReadEngSpeedLimitWhileVehStop
  {
    get
    {
      return this.engSpeedLimitWhileVehStop != null && this.engSpeedLimitWhileVehStop.HasBeenReadFromEcu;
    }
  }

  private bool CanChangeUserID => !this.Working;

  private bool IsUserIDValid
  {
    get
    {
      return this.showUserID && this.textboxUserID != null && !string.IsNullOrEmpty(this.textboxUserID.Text) && this.textboxUserID.TextLength == this.textboxUserID.MaxLength;
    }
  }

  private void UpdateCurrentSetup()
  {
    SetupInformation setupInformation = (SetupInformation) null;
    if (this.mcm != null)
    {
      IEnumerable<EquipmentType> source = EquipmentType.ConnectedEquipmentTypes("Engine");
      if (CollectionExtensions.Exactly<EquipmentType>(source, 1))
      {
        EquipmentType equipmentType = source.First<EquipmentType>();
        string name = ((EquipmentType) ref equipmentType).Name;
        if (this.currentSetup == null || name != this.currentSetup.EngineType)
        {
          foreach (SetupInformation setup in UserPanel.Setups)
          {
            if (name == setup.EngineType)
            {
              setupInformation = setup;
              setupInformation.UseAAMA = setupInformation.UseAAMAWhenAvailable && this.mcm.Services[setupInformation.AAMAStartService] != (Service) null;
              break;
            }
          }
        }
        else
          setupInformation = this.currentSetup;
      }
    }
    if (setupInformation == this.currentSetup)
      return;
    this.currentSetup = setupInformation;
  }

  private void UpdateUserInterface()
  {
    this.UpdateMCMCheck();
    this.UpdateCPCCheck();
    this.UpdateFaultCheck();
    this.UpdateNoFaultsCheck();
    this.UpdateEngineStartedCheck();
    this.UpdateRegenCheck();
    this.UpdateMaxEngineSpeedCheck();
    this.UpdateUserIDCheck();
    this.UpdateButtons();
    this.UpdateWarningMessage();
  }

  private void OnMCMChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateCurrentSetup();
    this.UpdateUserInterface();
  }

  private void OnCPCChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnFaultCodesUpdateEvent(object sender, ResultEventArgs e)
  {
    if (this.Working && (this.currentSetup.CurrentStep == Step.ManualOperation || this.currentSetup.CurrentStep == Step.StopEngineSpeed) && !this.IsFaultActive("C78D00"))
      this.PerformCurrentStep();
    this.UpdateUserInterface();
  }

  private void OnEngineSpeedUpdate(object sender, ResultEventArgs e)
  {
    if (this.Working && this.currentSetup.CurrentStep == Step.WaitEngineSpeed)
      this.PerformCurrentStep();
    this.UpdateUserInterface();
  }

  private void OnRegenFlagUpdate(object sender, ResultEventArgs e) => this.UpdateUserInterface();

  private void OnCoolantOilUpdate(object sender, ResultEventArgs e) => this.PerformCurrentStep();

  private void OnMaxEngineSpeedUpdate(object sender, ResultEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnEngSpeedLimitWhileVehStopUpdate(object sender, ResultEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    this.ClearCurrentService();
    bool flag = this.CheckCompleteResult(e, Resources.Message_ServiceExecuted, Resources.Message_ServiceError);
    if (flag)
    {
      Service service = (Service) sender;
      switch (this.currentSetup.CurrentStep)
      {
        case Step.StartAAMA:
          if (service.OutputValues.Count > 0 && service.OutputValues[0].Type == typeof (Choice))
          {
            this.ReportResult(string.Format(Resources.MessageFormat_ResultOfStartRoutine0, service.OutputValues[0].Value));
            if (Convert.ToInt32((service.OutputValues[0].Value as Choice).RawValue) == 1)
            {
              this.currentSetup.GotoNextStep();
              this.PerformCurrentStep();
              break;
            }
            flag = false;
            break;
          }
          flag = false;
          break;
        case Step.RequestAAMAResults:
          if (service.OutputValues.Count > 0 && service.OutputValues[0].Type == typeof (Choice))
          {
            this.ReportResult(string.Format(Resources.MessageFormat_ResultOfRequestResultsRoutine0, service.OutputValues[0].Value));
            switch (Convert.ToInt32((service.OutputValues[0].Value as Choice).RawValue))
            {
              case 2:
                this.StartTimer(new TimeSpan(0, 0, 2));
                break;
              case 4:
                this.currentSetup.GotoNextStep();
                this.PerformCurrentStep();
                break;
              case 5:
                this.currentSetup.GotoStep(Step.AdaptionComplete);
                this.PerformCurrentStep();
                break;
              default:
                flag = false;
                break;
            }
          }
          else
          {
            flag = false;
            break;
          }
          break;
        default:
          this.PerformCurrentStep();
          break;
      }
    }
    if (flag)
      return;
    this.StopAdaptation(Result.Failed);
  }

  private void OnStartClick(object sender, EventArgs e) => this.StartAdaptation();

  private void OnCancelClick(object sender, EventArgs e) => this.StopAdaptation(Result.Canceled);

  private void OnResetClick(object sender, EventArgs e) => this.ResetFault();

  private void OnTimerToControlHoldTimes(object sender, EventArgs e)
  {
    if (!(DateTime.Now >= this.timerEnds))
      return;
    this.StopTimer();
    this.PerformCurrentStep();
  }

  private void OnUserNameChanged(object sender, EventArgs e)
  {
    if (UserPanel.ValidUserNameCheck.IsMatch(this.textboxUserID.Text))
    {
      this.textboxUserID.Text = this.lastUserNameContent;
    }
    else
    {
      this.lastUserNameContent = this.textboxUserID.Text;
      this.UpdateUserInterface();
    }
  }

  private void OnUserNameKeyDown(object sender, KeyEventArgs e)
  {
    switch (e.KeyCode)
    {
      case Keys.Back:
        break;
      case Keys.End:
        break;
      case Keys.Home:
        break;
      case Keys.Left:
        break;
      case Keys.Up:
        break;
      case Keys.Right:
        break;
      case Keys.Down:
        break;
      case Keys.Delete:
        break;
      case Keys.D0:
        break;
      case Keys.D1:
        break;
      case Keys.D2:
        break;
      case Keys.D3:
        break;
      case Keys.D4:
        break;
      case Keys.D5:
        break;
      case Keys.D6:
        break;
      case Keys.D7:
        break;
      case Keys.D8:
        break;
      case Keys.D9:
        break;
      case Keys.NumPad0:
        break;
      case Keys.NumPad1:
        break;
      case Keys.NumPad2:
        break;
      case Keys.NumPad3:
        break;
      case Keys.NumPad4:
        break;
      case Keys.NumPad5:
        break;
      case Keys.NumPad6:
        break;
      case Keys.NumPad7:
        break;
      case Keys.NumPad8:
        break;
      case Keys.NumPad9:
        break;
      default:
        e.SuppressKeyPress = true;
        break;
    }
  }

  private void UpdateMCMCheck()
  {
    bool flag = false;
    string str = Resources.Message_MCMIsNotConnected;
    if (this.mcm != null)
    {
      if (this.MCMOnline)
      {
        str = Resources.Message_MCMIsConnectedAndEngineIsSupported;
        flag = true;
      }
      else
        str = this.currentSetup != null || this.mcm.CommunicationsState != CommunicationsState.Online ? Resources.Message_MCMIsBusy : Resources.Message_MCMIsConnectedButEngineIsNotSupported;
    }
    ((Control) this.labelMCMMessage).Text = str;
    this.checkMcm.Checked = flag;
  }

  private void UpdateCPCCheck()
  {
    bool flag = false;
    string str = Resources.Message_CPC2IsNotConnected;
    if (this.cpc != null)
    {
      if (this.CPCOnline)
      {
        str = Resources.Message_CPC2IsConnected;
        flag = true;
      }
      else
        str = Resources.Message_CPC2IsBusy;
    }
    ((Control) this.labelCPC2Message).Text = str;
    this.checkCpc.Checked = flag;
  }

  private bool IsFaultActive(string udsCode)
  {
    bool flag = false;
    if (this.mcm != null)
    {
      FaultCode faultCode = this.mcm.FaultCodes[udsCode];
      if (faultCode != null)
      {
        FaultCodeIncident current = faultCode.FaultCodeIncidents.Current;
        if (current != null && current.Active == ActiveStatus.Active)
          flag = true;
      }
    }
    return flag;
  }

  private void UpdateFaultCheck()
  {
    bool flag = false;
    string str = Resources.Message_CannotDetectIfAirMassAdaptationIsRequired;
    if (this.mcm != null)
    {
      str = Resources.Message_AirMassAdaptationIsNotRequired;
      if (this.IsFaultActive("C78D00"))
      {
        str = Resources.Message_AirMassAdaptationIsRequired;
        flag = true;
      }
    }
    ((Control) this.labelFaultMessage).Text = str;
    this.checkFault.Checked = flag;
  }

  private void UpdateNoFaultsCheck()
  {
    bool flag = false;
    string str = Resources.Message_CannotDetectIfAdditionalFaultsNeedAddressing;
    if (this.mcm != null)
    {
      Collection<FaultCode> output = new Collection<FaultCode>();
      this.mcm.FaultCodes.CopyCurrent(output);
      if (output.Count > 1 || output.Count == 1 && !this.IsFaultActive("C78D00"))
      {
        str = Resources.Message_AdditionalFaultsDetectedPleaseAddressTheseBeforePerformingAirMassAdaptation;
      }
      else
      {
        str = Resources.Message_NoOtherFaultsDetected;
        flag = true;
      }
    }
    ((Control) this.labelNoFaultsMessage).Text = str;
    this.checkNoFaults.Checked = flag;
  }

  private void UpdateEngineStartedCheck()
  {
    bool flag = false;
    string str = Resources.Message_CannotDetectIfEngineIsStarted;
    if (this.engineSpeed != (Instrument) null && this.engineSpeed.InstrumentValues.Current != null)
    {
      double d = Convert.ToDouble(this.engineSpeed.InstrumentValues.Current.Value);
      if (!double.IsNaN(d))
      {
        if (d > 0.0)
        {
          str = Resources.Message_EngineIsStarted;
          flag = true;
        }
        else
          str = Resources.Message_EngineHasNotBeenStartedPleaseStartTheEngine;
      }
    }
    ((Control) this.labelEngineStartedMessage).Text = str;
    this.checkEngineStarted.Checked = flag;
  }

  private void UpdateRegenCheck()
  {
    bool flag = false;
    string str = Resources.Message_CannotDetectIfDPFRegenerationIsInProgress;
    if (this.regenFlag != (Instrument) null && this.regenFlag.InstrumentValues.Current != null)
    {
      double d = Convert.ToDouble(((Choice) this.regenFlag.InstrumentValues.Current.Value).RawValue);
      if (!double.IsNaN(d))
      {
        if (d == 0.0)
        {
          str = Resources.Message_NotPerformingDPFRegeneration;
          flag = true;
        }
        else
          str = Resources.Message_DPFRegenerationInProgressCannotPerformAirMassAdaptation;
      }
    }
    ((Control) this.labelRegenMessage).Text = str;
    this.checkRegen.Checked = flag;
  }

  private void UpdateMaxEngineSpeedCheck()
  {
    bool flag = false;
    string str = Resources.Message_CannotDetectMaximumEngineSpeedLimits;
    if (this.currentSetup != null && this.currentSetup.UseAAMA)
    {
      flag = true;
      str = Resources.Message_EngineSpeedLimitHandledAutomaticallyByAAMATest;
    }
    else if (this.maxEngineSpeed != null && this.engSpeedLimitWhileVehStop != null)
    {
      if (this.maxEngineSpeed.HasBeenReadFromEcu && this.engSpeedLimitWhileVehStop.HasBeenReadFromEcu)
      {
        double d = 0.0;
        if (this.maxEngineSpeed.Value != null)
          d = Convert.ToDouble(this.maxEngineSpeed.OriginalValue);
        if ((double.IsNaN(d) || d >= 3000.0) && this.engSpeedLimitWhileVehStop.Value != null)
          d = Convert.ToDouble(this.engSpeedLimitWhileVehStop.OriginalValue);
        if (!double.IsNaN(d))
        {
          if (d < 3000.0)
          {
            str = string.Format(Resources.MessageFormat_MaximumEngineSpeedLimitsAreBelow0RpmAndWillNeedAdjustment, (object) 3000.0);
            flag = false;
          }
          else if (this.weAdjustedMaxEngineSpeed)
          {
            str = string.Format(Resources.MessageFormat_MaximumEngineSpeedLimitsHaveBeenTemporarilyAdjustedTo0Rpm, (object) 3000.0);
            flag = false;
          }
          else
          {
            str = string.Format(Resources.MessageFormat_MaximumEngineSpeedLimitsAreAtOrAbove0Rpm, (object) 3000.0);
            flag = true;
          }
        }
      }
      else
        str = Resources.Message_MaximumEngineSpeedLimitsHaveNotBeenRead;
    }
    ((Control) this.labelMaxEngineSpeedMessage).Text = str;
    this.checkMaxEngineSpeed.Checked = flag;
  }

  private void UpdateUserIDCheck()
  {
    if (!this.showUserID)
      return;
    this.checkUserID.Checked = this.IsUserIDValid;
    this.textboxUserID.Enabled = this.CanChangeUserID;
  }

  private void UpdateButtons()
  {
    this.checkStartAdaptation.Checked = this.CanStart;
    this.buttonStart.Enabled = this.CanStart;
    this.buttonCancel.Enabled = this.CanCancel;
    this.buttonReset.Enabled = this.CanReset;
  }

  private void ResetFault()
  {
    if (!this.CanReset || !this.ShowMessageBox(this.resetDialog))
      return;
    Cursor.Current = Cursors.WaitCursor;
    try
    {
      this.resetFaultParameters.SetParameters(this.mcm);
      this.mcm.Parameters.Write(true);
    }
    catch (CaesarException ex)
    {
      this.ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredClearingTheCurrentAdaptation0, (object) ex.Message));
    }
    Cursor.Current = Cursors.Default;
  }

  private void UpdateWarningMessage()
  {
    bool flag = false;
    if (this.cpc != null && UserPanel.HasUnsentChanges(this.cpc))
      flag = true;
    this.labelWarning.Visible = flag;
  }

  private static bool HasUnsentChanges(Channel channel)
  {
    bool flag = false;
    foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) channel.Parameters)
    {
      if (!object.Equals(parameter.Value, parameter.OriginalValue))
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  private void StartAdaptation()
  {
    if (!this.CanStart || !this.warningManager.RequestContinue())
      return;
    this.currentSetup.GotoStep(Step.StartAdaptation);
    this.PerformCurrentStep();
  }

  private string GetEcuInfoValue(string qualifier)
  {
    string ecuInfoValue = string.Empty;
    if (this.mcm != null)
    {
      EcuInfo ecuInfo = this.mcm.EcuInfos[qualifier];
      if (ecuInfo != null)
      {
        if (string.IsNullOrEmpty(ecuInfo.Value))
        {
          try
          {
            ecuInfo.Read(true);
          }
          catch (CaesarException ex)
          {
            this.ReportResult(string.Format(Resources.MessageFormat_ErrorRetrieving01, (object) ecuInfo.Name, (object) ex.Message));
          }
        }
        ecuInfoValue = ecuInfo.Value;
      }
      if (string.IsNullOrEmpty(ecuInfoValue))
        ecuInfoValue = Resources.Message_Unknown;
    }
    return ecuInfoValue;
  }

  private void PerformCurrentStep()
  {
    this.AddStatusMessage(string.Format(Resources.MessageFormat_PerformingStep0, (object) this.currentSetup.CurrentStep.ToString()));
    switch (this.currentSetup.CurrentStep)
    {
      case Step.StartAdaptation:
        if (this.showUserID)
          this.Label(string.Format(Resources.MessageFormat_AirMassAdaptationStartedUser0, (object) this.textboxUserID.Text));
        else
          this.Label(Resources.Message_AirMassAdaptationStarted);
        this.vin = this.GetEcuInfoValue("CO_VIN");
        this.esn = this.GetEcuInfoValue("CO_ESN");
        this.currentSetup.GotoNextStep();
        this.PerformCurrentStep();
        break;
      case Step.SetMaxEngineSpeedLimit:
        this.ReportResult(Resources.Message_CheckingMaximumEngineSpeedLimit);
        bool flag1 = false;
        this.originalMaxEngineSpeed = Convert.ToDouble(this.maxEngineSpeed.OriginalValue);
        if (this.originalMaxEngineSpeed < 3000.0)
        {
          this.ReportResult(string.Format(Resources.MessageFormat_0Is1AndRequiresTemporaryAdjustment1, (object) this.maxEngineSpeed.Name, (object) this.originalMaxEngineSpeed));
          flag1 = true;
        }
        this.originalEngSpeedLimitWhileVehStop = Convert.ToDouble(this.engSpeedLimitWhileVehStop.OriginalValue);
        if (this.originalEngSpeedLimitWhileVehStop < 3000.0)
        {
          this.ReportResult(string.Format(Resources.MessageFormat_0Is1AndRequiresTemporaryAdjustment, (object) this.engSpeedLimitWhileVehStop.Name, (object) this.originalEngSpeedLimitWhileVehStop));
          flag1 = true;
        }
        if (flag1)
        {
          if (this.ShowMessageBox(this.adjustDialog))
          {
            this.ReportResult(this.adjustDialog.LabelYes);
            this.weAdjustedMaxEngineSpeed = true;
            this.maxEngineSpeed.Value = (object) 3000.0;
            this.engSpeedLimitWhileVehStop.Value = (object) 3000.0;
            if (this.WriteCPCParameters(new Parameter[2]
            {
              this.maxEngineSpeed,
              this.engSpeedLimitWhileVehStop
            }))
            {
              this.currentSetup.GotoNextStep();
              this.PerformCurrentStep();
              break;
            }
            this.StopAdaptation(Result.Failed);
            break;
          }
          this.ReportResult(this.adjustDialog.LabelNo);
          this.StopAdaptation(Result.Canceled);
          break;
        }
        this.ReportResult(Resources.Message_LimitsValid);
        this.currentSetup.GotoNextStep();
        this.PerformCurrentStep();
        break;
      case Step.ShutOffFans:
        this.ReportResult(Resources.Message_ShuttingOffFans);
        bool flag2 = false;
        try
        {
          this.ExecuteService(this.currentSetup.Fan1OffService, true);
        }
        catch (CaesarException ex)
        {
          flag2 = true;
          this.Label(string.Format(Resources.MessageFormat_ShuttingOffFan1Failed0, (object) ex.Message));
        }
        try
        {
          this.ExecuteService(this.currentSetup.Fan2OffService, true);
        }
        catch (CaesarException ex)
        {
          flag2 = true;
          this.Label(string.Format(Resources.MessageFormat_ShuttingOffFan2Failed0, (object) ex.Message));
        }
        if (flag2 && DialogResult.No == MessageBox.Show($"{Resources.Message_TheMCMDoesNotAppearToHaveControlOfTheEngine}\r\n\r\n{Resources.Message_AreYouSureYouWishToProceed}", ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
        {
          this.StopAdaptation(Result.Canceled);
          break;
        }
        this.currentSetup.GotoNextStep();
        this.PerformCurrentStep();
        break;
      case Step.CloseEGRValve:
        this.ReportResult(Resources.Message_ShuttingOffEGRValve);
        try
        {
          this.ExecuteService(this.currentSetup.CloseEGRService, true);
        }
        catch (CaesarException ex)
        {
          this.Label(string.Format(Resources.MessageFormat_ShuttingOffEGRValveFailed0, (object) ex.Message));
        }
        this.currentSetup.GotoNextStep();
        this.PerformCurrentStep();
        break;
      case Step.BeginCheckForOperatingConditions:
        this.coolantTemperature = this.mcm.Instruments["DT_AS013_Coolant_Temperature"];
        this.oilTemperature = this.mcm.Instruments["DT_AS016_Oil_Temperature"];
        if (this.coolantTemperature != (Instrument) null && this.oilTemperature != (Instrument) null)
        {
          this.coolantTemperature.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnCoolantOilUpdate);
          this.oilTemperature.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnCoolantOilUpdate);
          this.currentSetup.GotoNextStep();
          this.ReportResult(string.Format(Resources.MessageFormat_WaitingForOperatingCondition01CAnd23C, (object) this.coolantTemperature.Name, (object) this.currentSetup.CoolantTemperatureThreshold, (object) this.oilTemperature.Name, (object) this.currentSetup.OilTemperatureThreshold));
          this.PerformCurrentStep();
          break;
        }
        this.coolantTemperature = (Instrument) null;
        this.oilTemperature = (Instrument) null;
        this.Label(Resources.Message_FailedToObtainCoolantAndOilTemperaturesAbortingAdaptation);
        this.StopAdaptation(Result.Failed);
        break;
      case Step.WaitForOperatingConditions:
        if (this.coolantTemperature.InstrumentValues.Current != null)
        {
          double d1 = Convert.ToDouble(this.coolantTemperature.InstrumentValues.Current.Value);
          if (!double.IsNaN(d1) && d1 >= this.currentSetup.CoolantTemperatureThreshold && this.oilTemperature.InstrumentValues.Current != null)
          {
            double d2 = Convert.ToDouble(this.oilTemperature.InstrumentValues.Current.Value);
            if (!double.IsNaN(d2) && d2 >= this.currentSetup.OilTemperatureThreshold)
            {
              this.Label(string.Format(Resources.MessageFormat_ConditionMet01CAnd23C, (object) this.coolantTemperature.Name, (object) this.currentSetup.CoolantTemperatureThreshold, (object) this.oilTemperature.Name, (object) this.currentSetup.OilTemperatureThreshold));
              this.ClearOilAndCoolantInstruments();
              this.currentSetup.GotoNextStep();
              this.PerformCurrentStep();
            }
          }
          break;
        }
        break;
      case Step.DriveEngineSpeed:
        this.ReportResult(string.Format(Resources.MessageFormat_IncreasingEngineSpeedTo0AndHoldingFor1Seconds, (object) this.currentSetup.CurrentSpeedPoint.TargetSpeed, (object) this.currentSetup.CurrentSpeedPoint.HoldTimeSeconds));
        this.currentSetup.GotoNextStep();
        if (this.currentSetup.CurrentSpeedPoint != null)
        {
          this.ExecuteService(this.currentSetup.CurrentSpeedPoint.StartModificationService, false);
          break;
        }
        this.PerformCurrentStep();
        break;
      case Step.WaitEngineSpeed:
        if (this.currentSetup.CurrentSpeedPoint == null || this.engineSpeed.InstrumentValues.Current != null && this.engineSpeed.InstrumentValues.Current.Value != null && UserPanel.WithinTolerance(Convert.ToDouble(this.engineSpeed.InstrumentValues.Current.Value), this.currentSetup.CurrentSpeedPoint.TargetSpeed, 0.1))
        {
          this.currentSetup.GotoNextStep();
          this.PerformCurrentStep();
          break;
        }
        break;
      case Step.HoldEngineSpeed:
        this.currentSetup.GotoNextStep();
        if (this.currentSetup.CurrentSpeedPoint != null)
        {
          this.StartTimer(this.currentSetup.CurrentSpeedPoint.HoldTimeSpan);
          break;
        }
        this.PerformCurrentStep();
        break;
      case Step.StopEngineSpeed:
        EngineSpeedServicePair currentSpeedPoint = this.currentSetup.CurrentSpeedPoint;
        this.currentSetup.GotoNextStep();
        if (currentSpeedPoint != null)
        {
          if (!this.IsFaultActive("C78D00"))
            this.currentSetup.GotoStep(Step.AdaptionComplete);
          this.ExecuteService(currentSpeedPoint.StopModificationService, false);
          break;
        }
        this.PerformCurrentStep();
        break;
      case Step.StartAAMA:
        this.ReportResult(string.Format(Resources.Message_CallingAAMAStartRoutine));
        this.ExecuteService(this.currentSetup.AAMAStartService, false);
        break;
      case Step.RequestAAMAResults:
        this.ReportResult(string.Format(Resources.Message_CallingAAMARequestResultsRoutine));
        this.ExecuteService(this.currentSetup.AAMARequestResultsService, false);
        break;
      case Step.ManualOperation:
        if (this.IsFaultActive("C78D00") && this.ShowMessageBox(this.continueManualOperationDialog))
        {
          this.ReportResult($"{Resources.Message_PleaseManuallyApplyChangesToTheEngineSpeedInOrderToCompleteTheAdaptation}\r\n{Resources.Message_DisconnectFromTheDevicesCompleteTheAdaptationOrPressCancelToFinish}");
          break;
        }
        this.currentSetup.GotoNextStep();
        this.PerformCurrentStep();
        break;
      case Step.AdaptionComplete:
        this.currentSetup.GotoNextStep();
        this.PerformCurrentStep();
        break;
      case Step.OpenEGRValve:
        this.ReportResult(Resources.Message_ResettingEGRValve);
        this.currentSetup.GotoNextStep();
        this.ExecuteService(this.currentSetup.OpenEGRService, false);
        break;
      case Step.TurnOnFans:
        this.ReportResult(Resources.Message_TurningFansOn);
        try
        {
          this.ExecuteService(this.currentSetup.Fan1OnService, true);
        }
        catch (CaesarException ex)
        {
          this.ReportResult(string.Format(Resources.MessageFormat_TurningOnFan1Failed0, (object) ex.Message));
        }
        try
        {
          this.ExecuteService(this.currentSetup.Fan2OnService, true);
        }
        catch (CaesarException ex)
        {
          this.ReportResult(string.Format(Resources.MessageFormat_TurningOnFan2Failed0, (object) ex.Message));
        }
        this.currentSetup.GotoNextStep();
        this.PerformCurrentStep();
        break;
      case Step.ResetMaxEngineSpeedLimit:
        this.currentSetup.GotoNextStep();
        this.ResetMaxEngineSpeed();
        this.PerformCurrentStep();
        break;
      case Step.CommitToPermanentMemory:
        this.ReportResult(Resources.Message_FinalizingAdaptation);
        this.CommitToPermanentMemory("MCM");
        Result result;
        if (!this.currentSetup.UseAAMA)
        {
          result = !this.IsFaultActive("C78D00") ? Result.Success : Result.Failed;
        }
        else
        {
          result = Result.Success;
          this.ReportResult(Resources.Message_AAMAProcedureOutcomeDeterminedByMCM);
        }
        this.StopAdaptation(result);
        break;
      case Step.ClearFaults:
        this.currentSetup.GotoNextStep();
        this.ClearFaults();
        break;
    }
    this.UpdateUserInterface();
  }

  private static bool WithinTolerance(double value, int target, double tolerance)
  {
    return !double.IsNaN(value) && value >= (double) target * (1.0 - tolerance);
  }

  private void ClearFaults()
  {
    if (this.mcm == null || !this.mcm.Online)
      return;
    this.mcm.FaultCodes.Reset(false);
  }

  private bool CheckCompleteResult(ResultEventArgs e, string successText, string errorText)
  {
    bool flag = false;
    StringBuilder stringBuilder = new StringBuilder("    ");
    if (e.Succeeded)
    {
      flag = true;
      stringBuilder.Append(successText);
      if (e.Exception != null)
        stringBuilder.AppendFormat(" ({0})", (object) e.Exception.Message);
    }
    else
    {
      stringBuilder.Append(errorText);
      if (e.Exception != null)
        stringBuilder.AppendFormat(": {0}", (object) e.Exception.Message);
      else
        stringBuilder.Append(": Unknown");
    }
    this.ReportResult(stringBuilder.ToString());
    return flag;
  }

  private void StopAdaptation(Result result)
  {
    this.StopTimer();
    if (!this.Working || this.currentSetup.CurrentStep == Step.None || this.currentSetup.CurrentStep == Step.ClearFaults)
      return;
    Step currentStep = this.currentSetup.CurrentStep;
    this.currentSetup.GotoStep(Step.ClearFaults);
    switch (currentStep)
    {
      case Step.WaitForOperatingConditions:
        this.ClearOilAndCoolantInstruments();
        goto case Step.CommitToPermanentMemory;
      case Step.CommitToPermanentMemory:
        if (currentStep != Step.None && currentStep != Step.ClearFaults)
          this.Label(string.Format(Resources.MessageFormat_AirMassAdaptationProcedureFinishedResult0, (object) result));
        this.PerformCurrentStep();
        break;
      default:
        this.ClearCurrentService();
        if (!this.currentSetup.UseAAMA)
        {
          foreach (EngineSpeedServicePair engineSpeedPoint in (IEnumerable<EngineSpeedServicePair>) this.currentSetup.EngineSpeedPoints)
          {
            try
            {
              this.ExecuteService(engineSpeedPoint.StopModificationService, true);
            }
            catch (CaesarException ex)
            {
            }
          }
        }
        else if (currentStep == Step.StartAAMA || currentStep == Step.RequestAAMAResults)
        {
          try
          {
            this.ExecuteService(this.currentSetup.AAMAStopService, true);
          }
          catch (CaesarException ex)
          {
          }
        }
        try
        {
          this.ExecuteService(this.currentSetup.OpenEGRService, true);
        }
        catch (CaesarException ex)
        {
        }
        try
        {
          this.ExecuteService(this.currentSetup.Fan1OnService, true);
        }
        catch (CaesarException ex)
        {
        }
        try
        {
          this.ExecuteService(this.currentSetup.Fan2OnService, true);
        }
        catch (CaesarException ex)
        {
        }
        if (!this.currentSetup.UseAAMA)
          this.ResetMaxEngineSpeed();
        goto case Step.CommitToPermanentMemory;
    }
  }

  private void ExecuteService(string serviceCall, bool synchronous)
  {
    if (this.mcm == null || !this.mcm.Online || !(this.currentService == (Service) null) || string.IsNullOrEmpty(serviceCall))
      return;
    Service service = this.mcm.Services[serviceCall];
    if (service == (Service) null)
    {
      this.StopAdaptation(Result.Failed);
    }
    else
    {
      if (!serviceCall.Contains("()") && service.InputValues.Count > 0)
        service.InputValues.ParseValues(serviceCall);
      if (!synchronous)
      {
        this.currentService = service;
        this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
      }
      service.Execute(synchronous);
    }
  }

  private void StartTimer(TimeSpan duration)
  {
    if (this.timerToControlHoldTimes == null)
    {
      this.timerToControlHoldTimes = new Timer();
      this.timerToControlHoldTimes.Interval = 500;
      this.timerToControlHoldTimes.Tick += new EventHandler(this.OnTimerToControlHoldTimes);
    }
    else
      this.StopTimer();
    this.timerEnds = DateTime.Now + duration;
    this.timerToControlHoldTimes.Start();
  }

  private void StopTimer()
  {
    if (this.timerToControlHoldTimes == null)
      return;
    this.timerEnds = DateTime.MinValue;
    this.timerToControlHoldTimes.Stop();
  }

  private void ClearCurrentService()
  {
    if (!(this.currentService != (Service) null))
      return;
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    this.currentService = (Service) null;
  }

  private void ClearOilAndCoolantInstruments()
  {
    if (this.coolantTemperature != (Instrument) null)
    {
      this.coolantTemperature.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnCoolantOilUpdate);
      this.coolantTemperature = (Instrument) null;
    }
    if (!(this.oilTemperature != (Instrument) null))
      return;
    this.oilTemperature.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnCoolantOilUpdate);
    this.oilTemperature = (Instrument) null;
  }

  private void Label(string label)
  {
    this.LogEvent(label);
    this.ReportResult(label);
  }

  private void ReportResult(string text)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
    this.AddStatusMessage(text);
  }

  public virtual bool CanProvideHtml
  {
    get => !this.Working && this.seekTimeListView.Text.Count<char>() > 0 && this.Online;
  }

  public virtual string ToHtml()
  {
    StringBuilder stringBuilder = new StringBuilder();
    XmlWriter writer = PrintHelper.CreateWriter(stringBuilder);
    writer.WriteStartElement("div");
    writer.WriteAttributeString("class", "standard");
    writer.WriteStartElement("p");
    writer.WriteAttributeString("class", "standard");
    if (this.showUserID)
    {
      writer.WriteElementString("b", Resources.Message_User);
      writer.WriteString(this.textboxUserID.Text);
      writer.WriteElementString("br", string.Empty);
    }
    writer.WriteElementString("b", Resources.Message_VIN);
    writer.WriteString(this.vin);
    writer.WriteElementString("br", string.Empty);
    writer.WriteElementString("b", Resources.Message_ESN);
    writer.WriteString(this.esn);
    writer.WriteFullEndElement();
    writer.WriteStartElement("p");
    writer.WriteAttributeString("class", "standard");
    writer.WriteElementString("b", Resources.Message_Report);
    writer.WriteElementString("br", string.Empty);
    string text1 = this.seekTimeListView.Text;
    string[] separator = new string[2]{ "\n", "\r\n" };
    foreach (string text2 in text1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
    {
      writer.WriteStartElement("div");
      writer.WriteStartAttribute("style");
      writer.WriteString("padding-left:");
      int num = 0;
      string str = text2;
      for (int index = 0; index < str.Length && str[index] == ' '; ++index)
        ++num;
      writer.WriteValue(num * 10);
      writer.WriteString("px");
      writer.WriteEndAttribute();
      writer.WriteString(text2);
      writer.WriteFullEndElement();
    }
    writer.WriteFullEndElement();
    writer.WriteFullEndElement();
    writer.Close();
    return stringBuilder.ToString();
  }

  private void ReadMaxEngineSpeed()
  {
    if (this.HaveReadMaxEngineSpeed)
      return;
    if (this.CPCOnline && !this.accessingCPCParameters)
    {
      this.accessingCPCParameters = true;
      this.cpc.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.OnParametersReadComplete);
      this.cpc.Parameters.ReadGroup(this.maxEngineSpeed.GroupQualifier, true, false);
    }
    ((Control) this.labelMaxEngineSpeedMessage).Invalidate();
  }

  private bool WriteCPCParameters(Parameter[] parameters)
  {
    bool flag = false;
    if (!this.accessingCPCParameters && (bool) this.cpc.Extension.Invoke("Unlock", (object[]) null))
    {
      Cursor.Current = Cursors.WaitCursor;
      this.accessingCPCParameters = true;
      try
      {
        this.cpc.Parameters.Write(true);
        foreach (Parameter parameter in parameters)
        {
          if (parameter.Exception != null)
            this.Label(Resources.Message_Modifying + parameter.Name + Resources.Message_ResultedInAnError + parameter.Exception.Message);
        }
        flag = true;
      }
      catch (CaesarException ex)
      {
        this.Label(string.Format(Resources.MessageFormat_ModifyingMaximumEngineSpeedLimitsFailed0, (object) ex.Message));
        flag = false;
      }
      this.accessingCPCParameters = false;
      Cursor.Current = Cursors.Default;
    }
    return flag;
  }

  private void ResetMaxEngineSpeed()
  {
    if (!this.weAdjustedMaxEngineSpeed)
      return;
    if (this.maxEngineSpeed != null && this.engSpeedLimitWhileVehStop != null)
    {
      this.ReportResult(Resources.Message_ResettingMaximumEngineSpeedLimits);
      this.maxEngineSpeed.Value = (object) this.originalMaxEngineSpeed;
      this.engSpeedLimitWhileVehStop.Value = (object) this.originalEngSpeedLimitWhileVehStop;
      this.WriteCPCParameters(new Parameter[2]
      {
        this.maxEngineSpeed,
        this.engSpeedLimitWhileVehStop
      });
    }
    else
    {
      this.Label(Resources.Message_UnableToResetMaxmimumEngineSpeedLimitsCPC2NotConnected);
      int num = (int) MessageBox.Show(Resources.Message_ItHasNotBeenPossibleToResetTheMaximumEngineSpeedLimitsAsTheCPC2IsNoLongerConnected, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }
    this.weAdjustedMaxEngineSpeed = false;
  }

  private void OnPaint(object sender, PaintEventArgs e)
  {
    if (this.HaveReadMaxEngineSpeed)
      return;
    this.timerToControlUpdate.Enabled = true;
  }

  private void OnParameterReadTimer(object sender, EventArgs e)
  {
    this.timerToControlUpdate.Enabled = false;
    this.ReadMaxEngineSpeed();
  }

  private void OnParametersReadComplete(object sender, ResultEventArgs e)
  {
    this.cpc.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.OnParametersReadComplete);
    this.accessingCPCParameters = false;
  }

  private void LogEvent(string eventText)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, eventText.Trim());
  }

  private bool ShowMessageBox(ContinueMessageDialog continueMessage)
  {
    bool flag = false;
    if (DialogResult.Yes == MessageBox.Show(continueMessage.Message, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
    {
      if (!string.IsNullOrEmpty(continueMessage.LabelYes))
        this.LogEvent(continueMessage.LabelYes);
      flag = true;
    }
    else if (!string.IsNullOrEmpty(continueMessage.LabelNo))
      this.LogEvent(continueMessage.LabelNo);
    return flag;
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.seekTimeListView = new SeekTimeListView();
    this.labelCPC2Message = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelPanelHeader = new ScalingLabel();
    this.labelMCMMessage = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonReset = new Button();
    this.labelFaultMessage = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelNoFaultsMessage = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelEngineStartedMessage = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelRegenMessage = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelUserIDMessage = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelMaxEngineSpeedMessage = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.textboxUserID = new TextBox();
    this.buttonStart = new Button();
    this.buttonCancel = new Button();
    this.dialVehicleSpeed = new DialInstrument();
    this.dialEngineSpeed = new DialInstrument();
    this.barEGRCommanded = new BarInstrument();
    this.driFanStatusPWM06 = new DigitalReadoutInstrument();
    this.barEGRActual = new BarInstrument();
    this.driRegenFlag = new DigitalReadoutInstrument();
    this.barCoolantTemperature = new BarInstrument();
    this.barOilTemperature = new BarInstrument();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.BarInstrument1 = new BarInstrument();
    this.labelWarning = new System.Windows.Forms.Label();
    this.checkMcm = new Checkmark();
    this.checkCpc = new Checkmark();
    this.checkFault = new Checkmark();
    this.checkNoFaults = new Checkmark();
    this.checkEngineStarted = new Checkmark();
    this.checkRegen = new Checkmark();
    this.checkUserID = new Checkmark();
    this.checkMaxEngineSpeed = new Checkmark();
    this.checkStartAdaptation = new Checkmark();
    this.resetFaultParameters = new ParameterFile(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelCPC2Message, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelPanelHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelMCMMessage, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonReset, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelFaultMessage, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelNoFaultsMessage, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelEngineStartedMessage, 1, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelRegenMessage, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelUserIDMessage, 1, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelMaxEngineSpeedMessage, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textboxUserID, 3, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonStart, 1, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonCancel, 2, 10);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 12);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelWarning, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkMcm, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkCpc, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkFault, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkNoFaults, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkEngineStarted, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkRegen, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkUserID, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkMaxEngineSpeed, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkStartAdaptation, 0, 10);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Air Mass Adaption";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView, 8);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    this.labelCPC2Message.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelCPC2Message, "labelCPC2Message");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelCPC2Message, 2);
    ((Control) this.labelCPC2Message).Name = "labelCPC2Message";
    this.labelCPC2Message.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelCPC2Message.ShowBorder = false;
    this.labelCPC2Message.UseSystemColors = true;
    this.labelPanelHeader.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelPanelHeader, 5);
    componentResourceManager.ApplyResources((object) this.labelPanelHeader, "labelPanelHeader");
    this.labelPanelHeader.FontGroup = (string) null;
    this.labelPanelHeader.LineAlignment = StringAlignment.Center;
    ((Control) this.labelPanelHeader).Name = "labelPanelHeader";
    this.labelMCMMessage.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelMCMMessage, "labelMCMMessage");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelMCMMessage, 2);
    ((Control) this.labelMCMMessage).Name = "labelMCMMessage";
    this.labelMCMMessage.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelMCMMessage.ShowBorder = false;
    this.labelMCMMessage.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.buttonReset, "buttonReset");
    this.buttonReset.Name = "buttonReset";
    this.buttonReset.UseCompatibleTextRendering = true;
    this.buttonReset.UseVisualStyleBackColor = true;
    this.labelFaultMessage.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFaultMessage, "labelFaultMessage");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelFaultMessage, 2);
    ((Control) this.labelFaultMessage).Name = "labelFaultMessage";
    this.labelFaultMessage.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelFaultMessage.ShowBorder = false;
    this.labelFaultMessage.UseSystemColors = true;
    this.labelNoFaultsMessage.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelNoFaultsMessage, "labelNoFaultsMessage");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelNoFaultsMessage, 2);
    ((Control) this.labelNoFaultsMessage).Name = "labelNoFaultsMessage";
    this.labelNoFaultsMessage.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelNoFaultsMessage.ShowBorder = false;
    this.labelNoFaultsMessage.UseSystemColors = true;
    this.labelEngineStartedMessage.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelEngineStartedMessage, "labelEngineStartedMessage");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelEngineStartedMessage, 2);
    ((Control) this.labelEngineStartedMessage).Name = "labelEngineStartedMessage";
    this.labelEngineStartedMessage.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelEngineStartedMessage.ShowBorder = false;
    this.labelEngineStartedMessage.UseSystemColors = true;
    this.labelRegenMessage.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelRegenMessage, "labelRegenMessage");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelRegenMessage, 2);
    ((Control) this.labelRegenMessage).Name = "labelRegenMessage";
    this.labelRegenMessage.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelRegenMessage.ShowBorder = false;
    this.labelRegenMessage.UseSystemColors = true;
    this.labelUserIDMessage.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelUserIDMessage, "labelUserIDMessage");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelUserIDMessage, 2);
    ((Control) this.labelUserIDMessage).Name = "labelUserIDMessage";
    this.labelUserIDMessage.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelUserIDMessage.ShowBorder = false;
    this.labelUserIDMessage.UseSystemColors = true;
    this.labelMaxEngineSpeedMessage.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelMaxEngineSpeedMessage, "labelMaxEngineSpeedMessage");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelMaxEngineSpeedMessage, 2);
    ((Control) this.labelMaxEngineSpeedMessage).Name = "labelMaxEngineSpeedMessage";
    this.labelMaxEngineSpeedMessage.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelMaxEngineSpeedMessage.ShowBorder = false;
    this.labelMaxEngineSpeedMessage.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.textboxUserID, "textboxUserID");
    this.textboxUserID.Name = "textboxUserID";
    componentResourceManager.ApplyResources((object) this.buttonStart, "buttonStart");
    this.buttonStart.Name = "buttonStart";
    this.buttonStart.UseCompatibleTextRendering = true;
    this.buttonStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.buttonCancel.Name = "buttonCancel";
    this.buttonCancel.UseCompatibleTextRendering = true;
    this.buttonCancel.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 5);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.dialVehicleSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.dialEngineSpeed, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.barEGRCommanded, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.driFanStatusPWM06, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.barEGRActual, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.driRegenFlag, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.barCoolantTemperature, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.barOilTemperature, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.DigitalReadoutInstrument1, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.BarInstrument1, 2, 3);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    this.dialVehicleSpeed.AngleRange = 135.0;
    this.dialVehicleSpeed.AngleStart = 180.0;
    componentResourceManager.ApplyResources((object) this.dialVehicleSpeed, "dialVehicleSpeed");
    this.dialVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.dialVehicleSpeed).FreezeValue = false;
    ((SingleInstrumentBase) this.dialVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.dialVehicleSpeed).Name = "dialVehicleSpeed";
    ((TableLayoutPanel) this.tableLayoutPanel2).SetRowSpan((Control) this.dialVehicleSpeed, 2);
    ((SingleInstrumentBase) this.dialVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.dialEngineSpeed.AngleRange = 135.0;
    this.dialEngineSpeed.AngleStart = 180.0;
    componentResourceManager.ApplyResources((object) this.dialEngineSpeed, "dialEngineSpeed");
    this.dialEngineSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.dialEngineSpeed).FreezeValue = false;
    ((SingleInstrumentBase) this.dialEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.dialEngineSpeed).Name = "dialEngineSpeed";
    ((TableLayoutPanel) this.tableLayoutPanel2).SetRowSpan((Control) this.dialEngineSpeed, 2);
    ((SingleInstrumentBase) this.dialEngineSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barEGRCommanded, "barEGRCommanded");
    this.barEGRCommanded.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barEGRCommanded).FreezeValue = false;
    ((SingleInstrumentBase) this.barEGRCommanded).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS031_EGR_Commanded_Governor_Value");
    ((Control) this.barEGRCommanded).Name = "barEGRCommanded";
    ((AxisSingleInstrumentBase) this.barEGRCommanded).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.barEGRCommanded).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.driFanStatusPWM06, "driFanStatusPWM06");
    this.driFanStatusPWM06.FontGroup = (string) null;
    ((SingleInstrumentBase) this.driFanStatusPWM06).FreezeValue = false;
    ((SingleInstrumentBase) this.driFanStatusPWM06).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS068_Fan_PWM06");
    ((Control) this.driFanStatusPWM06).Name = "driFanStatusPWM06";
    ((SingleInstrumentBase) this.driFanStatusPWM06).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.barEGRActual, "barEGRActual");
    this.barEGRActual.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barEGRActual).FreezeValue = false;
    ((SingleInstrumentBase) this.barEGRActual).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS032_EGR_Actual_Valve_Position");
    ((Control) this.barEGRActual).Name = "barEGRActual";
    ((AxisSingleInstrumentBase) this.barEGRActual).PreferredAxisRange = new AxisRange(0.0, 100.0, "");
    ((SingleInstrumentBase) this.barEGRActual).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.driRegenFlag, "driRegenFlag");
    this.driRegenFlag.FontGroup = (string) null;
    ((SingleInstrumentBase) this.driRegenFlag).FreezeValue = false;
    ((SingleInstrumentBase) this.driRegenFlag).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_DS014_DPF_Regen_Flag");
    ((Control) this.driRegenFlag).Name = "driRegenFlag";
    ((SingleInstrumentBase) this.driRegenFlag).UnitAlignment = StringAlignment.Near;
    this.barCoolantTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barCoolantTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barCoolantTemperature, "barCoolantTemperature");
    this.barCoolantTemperature.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barCoolantTemperature).FreezeValue = false;
    ((SingleInstrumentBase) this.barCoolantTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp");
    ((Control) this.barCoolantTemperature).Name = "barCoolantTemperature";
    ((AxisSingleInstrumentBase) this.barCoolantTemperature).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetRowSpan((Control) this.barCoolantTemperature, 2);
    ((SingleInstrumentBase) this.barCoolantTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barCoolantTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barCoolantTemperature).UnitAlignment = StringAlignment.Near;
    this.barOilTemperature.BarOrientation = (BarControl.ControlOrientation) 1;
    this.barOilTemperature.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.barOilTemperature, "barOilTemperature");
    this.barOilTemperature.FontGroup = (string) null;
    ((SingleInstrumentBase) this.barOilTemperature).FreezeValue = false;
    ((SingleInstrumentBase) this.barOilTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "oilTemp");
    ((Control) this.barOilTemperature).Name = "barOilTemperature";
    ((AxisSingleInstrumentBase) this.barOilTemperature).PreferredAxisRange = new AxisRange(-40.0, 200.0, "");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetRowSpan((Control) this.barOilTemperature, 2);
    ((SingleInstrumentBase) this.barOilTemperature).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.barOilTemperature).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.barOilTemperature).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.DigitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.BarInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.BarInstrument1, "BarInstrument1");
    this.BarInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.BarInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.BarInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS054_Differential_Pressure_Compressor_In");
    ((Control) this.BarInstrument1).Name = "BarInstrument1";
    ((SingleInstrumentBase) this.BarInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelWarning, "labelWarning");
    this.labelWarning.BackColor = SystemColors.Control;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelWarning, 5);
    this.labelWarning.ForeColor = Color.Red;
    this.labelWarning.Name = "labelWarning";
    this.labelWarning.UseCompatibleTextRendering = true;
    this.labelWarning.UseMnemonic = false;
    componentResourceManager.ApplyResources((object) this.checkMcm, "checkMcm");
    ((Control) this.checkMcm).Name = "checkMcm";
    componentResourceManager.ApplyResources((object) this.checkCpc, "checkCpc");
    ((Control) this.checkCpc).Name = "checkCpc";
    componentResourceManager.ApplyResources((object) this.checkFault, "checkFault");
    ((Control) this.checkFault).Name = "checkFault";
    componentResourceManager.ApplyResources((object) this.checkNoFaults, "checkNoFaults");
    ((Control) this.checkNoFaults).Name = "checkNoFaults";
    componentResourceManager.ApplyResources((object) this.checkEngineStarted, "checkEngineStarted");
    ((Control) this.checkEngineStarted).Name = "checkEngineStarted";
    componentResourceManager.ApplyResources((object) this.checkRegen, "checkRegen");
    ((Control) this.checkRegen).Name = "checkRegen";
    componentResourceManager.ApplyResources((object) this.checkUserID, "checkUserID");
    ((Control) this.checkUserID).Name = "checkUserID";
    componentResourceManager.ApplyResources((object) this.checkMaxEngineSpeed, "checkMaxEngineSpeed");
    ((Control) this.checkMaxEngineSpeed).Name = "checkMaxEngineSpeed";
    componentResourceManager.ApplyResources((object) this.checkStartAdaptation, "checkStartAdaptation");
    ((Control) this.checkStartAdaptation).Name = "checkStartAdaptation";
    this.resetFaultParameters.FileContents = componentResourceManager.GetString("resetFaultParameters.FileContents");
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_AirMassAdaptation");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
