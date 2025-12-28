// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.APS_Calibration__NGC_.panel;

public class UserPanel : CustomPanel
{
  private static string ChannelName = "APS301T";
  private static string AbsName = "ABS02T";
  private static string SsamName = "SSAM02T";
  private static string VrduName = "VRDU02T";
  private static string CalibrateCenterQualifier = "RT_Calibrate_extern_and_intern_steering_angle_Start_Calibration_state";
  private static string CalibrateLeftEndstopQualifier = "RT_Calibrate_Left_Endstop_Start_Endstop_Learnconditions_Endstop_state";
  private static string CalibrateRightEndstopQualifier = "RT_Calibrate_Right_Endstop_Start_Endstop_Learnconditions_Endstop_state";
  private static string CalibrateTorsionBarTorqueStartQualifier = "RT_Calibrate_TorsionBarTorqueOffset_Start";
  private static string CalibrateTorsionBarTorqueResultQualifier = "RT_Calibrate_TorsionBarTorqueOffset_Request_Results_Tbtoffset_calibration_request_status";
  private static string TorsionBarTorqueOffsetParameterQualifier = "TorsionBarTorqueOffset";
  private static string HardResetEcuQualifier = "FN_HardReset";
  private static string SteeringAngleQualifier = "DT_Steering_Angle_Steering_Angle";
  private static string ResetCalibrationDataQualifier = "RT_Discard_calibration_data_Start";
  private static int ResetCalibrationDataRawValue = 1;
  private static double TorsionBarRangeValue = 0.5;
  private static int HardResetDelayTime = 15;
  private CalibrationStep[] calibrationSteps;
  private CalibrationStep[] steeringAngleCalibrationSteps = new CalibrationStep[15]
  {
    new CalibrationStep(Resources.Message_PressNextToBeginCalibrationProcess, string.Empty, CalibrationActions.DisplayText, true),
    new CalibrationStep(Resources.Message_ResettingCalibrationData, string.Empty, CalibrationActions.ResetCalibrationData, false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
    new CalibrationStep(Resources.Message_StartEngine, Resources.Message_ClickNextToContinueToCalibration, CalibrationActions.StartEngine, true),
    new CalibrationStep(Resources.Message_CenterSteeringWheel, Resources.Message_ClickNextToContinue, CalibrationActions.DisplayText, true),
    new CalibrationStep(Resources.Message_CenterSteeringWheel, string.Empty, CalibrationActions.ConfirmCenter, false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
    new CalibrationStep(Resources.Message_CalibratingCenter, string.Empty, CalibrationActions.CalibrateCenter, false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
    new CalibrationStep(Resources.Message_TurnTheSteeringWheelToTheLeftUntilItReachesTheEndstopAndHold, string.Empty, CalibrationActions.DisplayText, true, 0.0, 0.0, 1500.0, "(Fault),(900,Ok),(1250,Fault)"),
    new CalibrationStep(Resources.Message_CalibrateLeftEndstop, string.Empty, CalibrationActions.CalibrateLeftEndstop, false, 0.0, 0.0, 1500.0, "(Fault),(900,Ok),(1250,Fault)"),
    new CalibrationStep(Resources.Message_TurnTheSteeringWheelToTheRightUntilItReachesTheEndstopAndHold, string.Empty, CalibrationActions.DisplayText, true, 0.0, -1500.0, 0.0, "(Fault),(-1250,Ok),(-900,Fault)"),
    new CalibrationStep(Resources.Message_CalibrateRightEndstop, string.Empty, CalibrationActions.CalibrateRightEndstop, false, 0.0, -1500.0, 0.0, "(Fault),(-1250,Ok),(-900,Fault)"),
    new CalibrationStep(Resources.Message_CenterSteeringWheel, Resources.Message_ClickNextToContinue, CalibrationActions.DisplayText, true, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
    new CalibrationStep(Resources.Message_PerformingHardReset, string.Empty, CalibrationActions.HardReset, false),
    new CalibrationStep(Resources.Message_PerformingHardReset, string.Format(Resources.MessageFormat_PleaseWait0, (object) UserPanel.HardResetDelayTime), CalibrationActions.HardResetDelay, false),
    new CalibrationStep(Resources.Message_VerifyingCalibration, string.Empty, CalibrationActions.VerifyCalibration, false),
    new CalibrationStep(Resources.Message_CalibrationWasSuccessful, Resources.Message_YouMayCloseThisCalibration, CalibrationActions.Complete, false)
  };
  private CalibrationStep[] steeringAngleWithTorsionBarCalibrationSteps = new CalibrationStep[20]
  {
    new CalibrationStep(Resources.Message_PressNextToBeginCalibrationProcess, string.Empty, CalibrationActions.DisplayText, true),
    new CalibrationStep(Resources.Message_ResettingCalibrationData, string.Empty, CalibrationActions.ResetCalibrationData, false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
    new CalibrationStep(Resources.Message_StartEngine, Resources.Message_ClickNextToContinueToCalibration, CalibrationActions.StartEngine, true),
    new CalibrationStep(Resources.Message_CenterSteeringWheel, Resources.Message_ClickNextToContinue, CalibrationActions.DisplayText, true),
    new CalibrationStep(Resources.Message_CenterSteeringWheel, string.Empty, CalibrationActions.ConfirmCenter, false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
    new CalibrationStep(Resources.Message_CalibratingCenter, string.Empty, CalibrationActions.CalibrateCenter, false, 0.0, -150.0, 150.0, "(Fault),(-75,Ok),(75,Fault)"),
    new CalibrationStep(Resources.Message_RemoveHandsFromWheelPressNextToBeginTorsionBarCalibration, string.Empty, CalibrationActions.DisplayText, true),
    new CalibrationStep(Resources.Message_CalibratingDoNotTouchSteeringWheel, string.Empty, CalibrationActions.TorsionBarCalibration, false),
    new CalibrationStep(Resources.Message_PerformingHardReset, string.Empty, CalibrationActions.HardReset, false),
    new CalibrationStep(Resources.Message_PerformingHardReset, string.Format(Resources.MessageFormat_PleaseWait0, (object) UserPanel.HardResetDelayTime), CalibrationActions.HardResetDelay, false),
    new CalibrationStep(Resources.Message_VerifyingTorsionBarTorqueOffset, string.Empty, CalibrationActions.VerifyTorsionBarCalibration, false),
    new CalibrationStep(Resources.Message_SlowlyTurnTheSteeringWheelToTheLeftUntilItReachesTheEndstopAndHold, string.Empty, CalibrationActions.DisplayText, true, 0.0, 0.0, 1500.0, "(Fault),(900,Ok),(1250,Fault)"),
    new CalibrationStep(Resources.Message_CalibrateLeftEndstop, string.Empty, CalibrationActions.CalibrateLeftEndstop, false, 0.0, 0.0, 1500.0, "(Fault),(900,Ok),(1250,Fault)"),
    new CalibrationStep(Resources.Message_SlowlyTurnTheSteeringWheelToTheRightUntilItReachesTheEndstopAndHold, string.Empty, CalibrationActions.DisplayText, true, 0.0, -1500.0, 0.0, "(Fault),(-1250,Ok),(-900,Fault)"),
    new CalibrationStep(Resources.Message_CalibrateRightEndstop, string.Empty, CalibrationActions.CalibrateRightEndstop, false, 0.0, -1500.0, 0.0, "(Fault),(-1250,Ok),(-900,Fault)"),
    new CalibrationStep(Resources.Message_CenterSteeringWheel, Resources.Message_ClickNextToContinue, CalibrationActions.DisplayText, true),
    new CalibrationStep(Resources.Message_PerformingHardReset, string.Empty, CalibrationActions.HardReset, false),
    new CalibrationStep(Resources.Message_PerformingHardReset, string.Format(Resources.MessageFormat_PleaseWait0, (object) UserPanel.HardResetDelayTime), CalibrationActions.HardResetDelay, false),
    new CalibrationStep(Resources.Message_VerifyingCalibration, string.Empty, CalibrationActions.VerifyCalibration, false),
    new CalibrationStep(Resources.Message_CalibrationWasSuccessful, Resources.Message_YouMayCloseThisCalibration, CalibrationActions.Complete, false)
  };
  private readonly Tuple<string, string, string>[] VrduFaultList = new Tuple<string, string, string>[41]
  {
    new Tuple<string, string, string>("VRDU02T", "524130", "9"),
    new Tuple<string, string, string>("VRDU02T", "524231", "19"),
    new Tuple<string, string, string>("VRDU02T", "524231", "9"),
    new Tuple<string, string, string>("VRDU02T", "524128", "19"),
    new Tuple<string, string, string>("VRDU02T", "524128", "9"),
    new Tuple<string, string, string>("VRDU02T", "524037", "19"),
    new Tuple<string, string, string>("VRDU02T", "524047", "19"),
    new Tuple<string, string, string>("VRDU02T", "524047", "9"),
    new Tuple<string, string, string>("VRDU02T", "524000", "19"),
    new Tuple<string, string, string>("VRDU02T", "524000", "9"),
    new Tuple<string, string, string>("VRDU02T", "524236", "19"),
    new Tuple<string, string, string>("VRDU02T", "524236", "9"),
    new Tuple<string, string, string>("VRDU02T", "524011", "19"),
    new Tuple<string, string, string>("VRDU02T", "524011", "9"),
    new Tuple<string, string, string>("VRDU02T", "524023", "19"),
    new Tuple<string, string, string>("VRDU02T", "524023", "9"),
    new Tuple<string, string, string>("VRDU02T", "523000", "12"),
    new Tuple<string, string, string>("VRDU02T", "524127", "19"),
    new Tuple<string, string, string>("VRDU02T", "524127", "9"),
    new Tuple<string, string, string>("VRDU02T", "524049", "19"),
    new Tuple<string, string, string>("VRDU02T", "524049", "9"),
    new Tuple<string, string, string>("VRDU02T", "524230", "19"),
    new Tuple<string, string, string>("VRDU02T", "524230", "9"),
    new Tuple<string, string, string>("VRDU02T", "524042", "19"),
    new Tuple<string, string, string>("VRDU02T", "524042", "9"),
    new Tuple<string, string, string>("VRDU02T", "524033", "19"),
    new Tuple<string, string, string>("VRDU02T", "524033", "9"),
    new Tuple<string, string, string>("VRDU02T", "524071", "19"),
    new Tuple<string, string, string>("VRDU02T", "524071", "9"),
    new Tuple<string, string, string>("VRDU02T", "524134", "19"),
    new Tuple<string, string, string>("VRDU02T", "524134", "9"),
    new Tuple<string, string, string>("VRDU02T", "524133", "19"),
    new Tuple<string, string, string>("VRDU02T", "524133", "9"),
    new Tuple<string, string, string>("VRDU02T", "524019", "19"),
    new Tuple<string, string, string>("VRDU02T", "524019", "9"),
    new Tuple<string, string, string>("VRDU02T", "1231", "11"),
    new Tuple<string, string, string>("VRDU02T", "1231", "3"),
    new Tuple<string, string, string>("VRDU02T", "1231", "9"),
    new Tuple<string, string, string>("VRDU02T", "639", "11"),
    new Tuple<string, string, string>("VRDU02T", "639", "3"),
    new Tuple<string, string, string>("VRDU02T", "639", "9")
  };
  private CalibrationStep calibrationFailureStep = new CalibrationStep(Resources.Message_CalibrationFailed, string.Empty, CalibrationActions.Complete, false);
  private CalibrationStep currentStep = (CalibrationStep) null;
  private int currentStepIndex = 0;
  private bool monitorCurrentSteeringWheelAngle = false;
  private Channel channel;
  private Channel ssamChannel;
  private Channel absChannel;
  private Channel vrduChannel;
  private bool calibrationIncomplete = false;
  private bool busy = false;
  private bool calibrationInProgress = false;
  private bool engineStarted = false;
  private bool hasActiveFaults = false;
  private bool instrumentsAreInitialized = false;
  private Service resetService;
  private int resetCounter;
  private Timer resetTimer = (Timer) null;
  private Service calibrationService;
  private Service resetCalibrationService;
  private StartMonitorStopServiceSharedProcedure torsionBarCalibrationProcedure;
  private Parameter torsionBarOffset = (Parameter) null;
  private TableLayoutPanel tableLayoutPanel1;
  private DialInstrument dialInstrumentCurrentStep;
  private TableLayoutPanel tableLayoutPanelInstruments;
  private DigitalReadoutInstrument digitalReadoutInstrument3;
  private DialInstrument dialInstrumentRightEndstop;
  private DialInstrument dialInstrument2;
  private DialInstrument dialInstrumentSteeringWheelAngle;
  private DigitalReadoutInstrument digitalReadoutInstrumentLeftEndstopCalibration;
  private DigitalReadoutInstrument digitalReadoutInstrumentRightEndstopCalibration;
  private DialInstrument dialInstrumentTorsionBarTorqueOffset;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineState;
  private TableLayoutPanel tableLayoutPanelLabels;
  private ScalingLabel scalingLabelCurrentStep;
  private ScalingLabel scalingLabelCurrentStepSubText;
  private ListViewEx listViewFaultCodes;
  private ColumnHeader columnHeaderChannel;
  private ColumnHeader columnHeaderNumber;
  private ColumnHeader columnHeaderMode;
  private ColumnHeader columnHeaderStatus;
  private TableLayoutPanel tableLayoutPanel3;
  private Button buttonReset;
  private Button buttonNext;
  private SeekTimeListView seekTimeListView1;

  public bool CalibrationIncomplete
  {
    get => this.calibrationIncomplete;
    private set
    {
      this.calibrationIncomplete = value;
      this.UpdateUI();
    }
  }

  public bool Busy
  {
    get => this.busy;
    private set
    {
      this.busy = value;
      this.UpdateUI();
    }
  }

  public bool CalibrationInProgress
  {
    get => this.calibrationInProgress;
    private set
    {
      this.calibrationInProgress = value;
      this.UpdateUI();
    }
  }

  public bool EngineStarted
  {
    get => this.engineStarted;
    private set
    {
      this.engineStarted = value;
      this.UpdateUI();
    }
  }

  public bool HasActiveFaults
  {
    get => this.hasActiveFaults;
    private set
    {
      this.hasActiveFaults = value;
      this.UpdateUI();
    }
  }

  public bool InstrumentsAreInitialized
  {
    get => this.instrumentsAreInitialized;
    private set
    {
      this.instrumentsAreInitialized = value;
      this.UpdateUI();
    }
  }

  public bool SupportsTorsionBarCalibration
  {
    get
    {
      return this.channel != null && this.channel.Services[UserPanel.CalibrateTorsionBarTorqueStartQualifier] != (Service) null;
    }
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.scalingLabelCurrentStep.AutoSize = true;
    this.scalingLabelCurrentStep.MinimumSize = new Size(441, 73);
    this.UpdateUI();
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.Busy)
      e.Cancel = true;
    if (e.Cancel)
      return;
    if (this.CalibrationIncomplete)
      this.PerformResetCalibration(true);
    if (this.resetTimer != null)
      this.StopHardResetTimer();
    this.SetChannel((Channel) null, ref this.channel);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetChannel(this.GetChannel(UserPanel.ChannelName), ref this.channel);
    this.SetChannel(this.GetChannel(UserPanel.AbsName), ref this.absChannel);
    this.SetChannel(this.GetChannel(UserPanel.SsamName), ref this.ssamChannel);
    this.SetVrduChannel(this.GetChannel(UserPanel.VrduName));
    this.UpdateDisplayedFaults();
  }

  private void SetChannel(Channel newChannel, ref Channel channel)
  {
    if (newChannel == channel)
      return;
    if (channel != null)
      channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    channel = newChannel;
    if (channel == null)
    {
      if (this.currentStep.Action != CalibrationActions.HardReset)
        this.ResetToBeginning();
    }
    else
      this.ResetToBeginning();
    if (channel != null)
      channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUI();
  }

  private void ResetToBeginning()
  {
    if (this.channel != null)
    {
      this.calibrationSteps = !this.SupportsTorsionBarCalibration ? this.steeringAngleCalibrationSteps : this.steeringAngleWithTorsionBarCalibrationSteps;
      this.currentStepIndex = 0;
      this.PerformStep();
    }
    else
    {
      this.currentStep = new CalibrationStep(Resources.Message_EcuIsDisconnected, string.Empty, CalibrationActions.DisplayText, false);
      this.UpdateUI();
    }
    this.CalibrationInProgress = false;
  }

  private void PerformNextStep()
  {
    if (this.calibrationSteps.Length > this.currentStepIndex)
      ++this.currentStepIndex;
    else
      this.currentStepIndex = 0;
    this.PerformStep();
  }

  private void PerformStep()
  {
    this.currentStep = this.calibrationSteps[this.currentStepIndex];
    switch (this.currentStep.Action)
    {
      case CalibrationActions.DisplayText:
        this.UpdateUI();
        break;
      case CalibrationActions.StartEngine:
        this.CheckIfEngineIsStarted();
        break;
      case CalibrationActions.ConfirmCenter:
        this.UpdateUI();
        if (this.ConfirmSteeringWheelAngleIsValid())
        {
          this.PerformNextStep();
          break;
        }
        this.monitorCurrentSteeringWheelAngle = true;
        break;
      case CalibrationActions.CalibrateCenter:
        this.CalibrationInProgress = true;
        this.PerformCenterCalibration();
        break;
      case CalibrationActions.CalibrateLeftEndstop:
        this.PerformEndstopCalibration(true);
        break;
      case CalibrationActions.CalibrateRightEndstop:
        this.PerformEndstopCalibration(false);
        break;
      case CalibrationActions.TorsionBarCalibration:
        this.PerformTorsionBarTorqueCalibration();
        break;
      case CalibrationActions.VerifyTorsionBarCalibration:
        this.VerifyTorsionBarTorqueOffset();
        break;
      case CalibrationActions.HardReset:
        this.PerformHardReset();
        break;
      case CalibrationActions.HardResetDelay:
        this.StartHardResetCounter();
        break;
      case CalibrationActions.ResetCalibrationData:
        this.PerformResetCalibration(false);
        break;
      case CalibrationActions.VerifyCalibration:
        if (this.ConfirmEndstopCalibration())
        {
          this.PerformNextStep();
          break;
        }
        this.ReportCalibrationFailed(Resources.Message_CalibrationFailedReachedEndOfRoutineWithEndstopsNotLearned);
        break;
      case CalibrationActions.Complete:
        this.ReportCalibrationPassed();
        break;
      case CalibrationActions.Unknown:
        this.ReportCalibrationFailed(Resources.Message_CalibrationFailedForUnknownReason);
        break;
    }
  }

  private bool IsChannelConnected(Channel channel)
  {
    return channel != null && channel.CommunicationsState == CommunicationsState.Online;
  }

  private void UpdateUI()
  {
    bool logFileIsOpen = SapiManager.GlobalInstance.LogFileIsOpen;
    if (this.currentStep != null)
    {
      ((Control) this.scalingLabelCurrentStep).Text = this.currentStep.DisplayText;
      ((Control) this.scalingLabelCurrentStepSubText).Text = this.currentStep.DisplaySubText;
      bool flag = this.currentStep.ButtonEnabled && !logFileIsOpen && !this.HasActiveFaults && this.InstrumentsAreInitialized && this.IsChannelConnected(this.channel) && this.IsChannelConnected(this.vrduChannel) && this.IsChannelConnected(this.absChannel) && this.IsChannelConnected(this.ssamChannel);
      this.buttonNext.Enabled = this.currentStep.Action == CalibrationActions.StartEngine ? flag && this.EngineStarted : flag;
      string connectEcusMessage = this.GetConnectEcusMessage();
      if (!string.IsNullOrEmpty(connectEcusMessage))
        ((Control) this.scalingLabelCurrentStep).Text = connectEcusMessage;
      if (this.currentStep.DialEnabled)
      {
        ((AxisSingleInstrumentBase) this.dialInstrumentCurrentStep).CustomOffset = new double?(this.currentStep.Offset);
        ((AxisSingleInstrumentBase) this.dialInstrumentCurrentStep).CustomScalingFactor = new double?(1.0);
        ((AxisSingleInstrumentBase) this.dialInstrumentCurrentStep).PreferredAxisRange = this.currentStep.VisibleRange;
        ((AxisSingleInstrumentBase) this.dialInstrumentCurrentStep).Gradient = Gradient.FromString(this.currentStep.GradientString);
        ((Control) this.dialInstrumentCurrentStep).Enabled = true;
        ((Control) this.dialInstrumentCurrentStep).Visible = true;
        ((Control) this.dialInstrumentCurrentStep).Refresh();
      }
      else
      {
        ((Control) this.dialInstrumentCurrentStep).Visible = false;
        ((Control) this.dialInstrumentCurrentStep).Enabled = false;
        ((Control) this.dialInstrumentCurrentStep).Refresh();
      }
    }
    else
      this.buttonNext.Enabled = !logFileIsOpen && this.EngineStarted && !this.HasActiveFaults;
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).ColumnStyles[3].Width = this.SupportsTorsionBarCalibration ? 25f : 0.0f;
    this.buttonReset.Enabled = !this.Busy && !logFileIsOpen;
  }

  private string GetConnectEcusMessage()
  {
    string str = string.Empty;
    ArrayList arrayList = new ArrayList();
    if (!this.IsChannelConnected(this.channel))
      arrayList.Add((object) UserPanel.ChannelName);
    if (!this.IsChannelConnected(this.absChannel))
      arrayList.Add((object) UserPanel.AbsName);
    if (!this.IsChannelConnected(this.ssamChannel))
      arrayList.Add((object) UserPanel.SsamName);
    if (!this.IsChannelConnected(this.vrduChannel))
      arrayList.Add((object) UserPanel.VrduName);
    if (arrayList.Count == 4)
      str = string.Format(Resources.MessageFormat_Connect0123ToBeginTheCalibration, arrayList[0], arrayList[1], arrayList[2], arrayList[3]);
    if (arrayList.Count == 3)
      str = string.Format(Resources.MessageFormat_Connect012ToBeginTheCalibration, arrayList[0], arrayList[1], arrayList[2]);
    else if (arrayList.Count == 2)
      str = string.Format(Resources.MessageFormat_Connect01ToBeginTheCalibration, arrayList[0], arrayList[1]);
    else if (arrayList.Count == 1)
      str = string.Format(Resources.MessageFormat_Connect0ToBeginTheCalibration, arrayList[0]);
    return str.Trim();
  }

  private void ReportCalibrationFailed(string reason)
  {
    this.ReportCalibrationFailed(reason, string.Empty);
  }

  private void ReportCalibrationFailed(string reason, string exception)
  {
    this.PerformResetCalibration(true);
    this.AddLogLabel(string.Format(Resources.MessageFormat_CalibrationFailed0, (object) reason, (object) exception));
    this.currentStep.DisplayText = reason;
    this.currentStep.DisplaySubText = exception;
    this.currentStep.Action = CalibrationActions.ReportFailure;
    this.currentStep.DialEnabled = false;
    this.currentStep.ButtonEnabled = false;
    this.CalibrationInProgress = false;
    this.UpdateUI();
  }

  private void ReportCalibrationPassed()
  {
    this.AddLogLabel(Resources.Message_CalibrationWasSuccessful);
    this.CalibrationIncomplete = false;
    this.currentStep.DisplayText = Resources.Message_CalibrationWasSuccessful;
    this.currentStep.Action = CalibrationActions.Complete;
    this.currentStep.DialEnabled = false;
    this.currentStep.ButtonEnabled = false;
    this.CalibrationInProgress = false;
    this.UpdateUI();
  }

  private bool ConfirmEndstopCalibration()
  {
    return this.digitalReadoutInstrumentLeftEndstopCalibration.RepresentedState == 1 && this.digitalReadoutInstrumentRightEndstopCalibration.RepresentedState == 1;
  }

  private bool ConfirmSteeringWheelAngleIsValid()
  {
    return this.dialInstrumentSteeringWheelAngle.RepresentedState == 1;
  }

  private int ConvertChoiceValueObjectToRawValue(ServiceOutputValue value)
  {
    Choice choice = value.Value as Choice;
    try
    {
      return Convert.ToInt32(choice.RawValue);
    }
    catch (InvalidCastException ex)
    {
      this.ReportCalibrationFailed(ex.Message);
    }
    return -1;
  }

  private int ReadCurrentSteeringAngle()
  {
    try
    {
      return Convert.ToInt32(this.channel.Instruments[UserPanel.SteeringAngleQualifier].InstrumentValues.Current.Value);
    }
    catch (InvalidCastException ex)
    {
      this.ReportCalibrationFailed(ex.Message);
    }
    catch (NullReferenceException ex)
    {
      this.ReportCalibrationFailed(ex.Message);
    }
    return int.MinValue;
  }

  private void PerformHardReset()
  {
    this.UpdateUI();
    this.resetService = this.channel.Services[UserPanel.HardResetEcuQualifier];
    if (this.resetService != (Service) null)
    {
      this.Busy = true;
      this.resetService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.resetService_ServiceCompleteEvent);
      this.resetService.Execute(false);
    }
    else
      this.ReportCalibrationFailed(Resources.Message_UnableToPerformCalibrationMissingService);
  }

  private void resetService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.resetService))
      return;
    this.Busy = false;
    this.resetService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.resetService_ServiceCompleteEvent);
    if (e.Succeeded)
      this.PerformNextStep();
    else
      this.ReportCalibrationFailed(Resources.Message_CalibrationFailedDuringEcuReset);
  }

  private void StartHardResetCounter()
  {
    if (this.resetTimer != null)
      this.StopHardResetTimer();
    this.resetCounter = UserPanel.HardResetDelayTime;
    this.resetTimer = new Timer();
    this.resetTimer.Interval = 1000;
    this.resetTimer.Tick += new EventHandler(this.ResetTimer_Tick);
    this.resetTimer.Start();
  }

  private void ResetTimer_Tick(object sender, EventArgs e)
  {
    --this.resetCounter;
    if (this.resetCounter <= 0)
    {
      this.StopHardResetTimer();
      this.PerformNextStep();
    }
    else
      ((Control) this.scalingLabelCurrentStepSubText).Text = string.Format(Resources.MessageFormat_PleaseWait0, (object) this.resetCounter);
  }

  private void StopHardResetTimer()
  {
    if (this.resetTimer == null)
      return;
    this.resetTimer.Stop();
    this.resetTimer.Tick -= new EventHandler(this.ResetTimer_Tick);
    this.resetTimer.Dispose();
    this.resetTimer = (Timer) null;
  }

  private void PerformCenterCalibration()
  {
    this.UpdateUI();
    this.calibrationService = this.channel.Services[UserPanel.CalibrateCenterQualifier];
    if (this.calibrationService != (Service) null)
    {
      this.CalibrationIncomplete = true;
      this.Busy = true;
      this.calibrationService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.centerCalibrationService_ServiceCompleteEvent);
      this.calibrationService.Execute(false);
    }
    else
      this.ReportCalibrationFailed(Resources.Message_UnableToPerformCalibrationMissingService);
  }

  private void centerCalibrationService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.calibrationService))
      return;
    this.Busy = false;
    this.calibrationService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.centerCalibrationService_ServiceCompleteEvent);
    if (e.Succeeded && this.ConvertChoiceValueObjectToRawValue(this.calibrationService.OutputValues[0]) == 1)
      this.PerformNextStep();
    else
      this.ReportCalibrationFailed(Resources.Message_CenterCalibrationServiceFailed);
  }

  private void PerformEndstopCalibration(bool leftEndstop)
  {
    this.UpdateUI();
    this.calibrationService = this.channel.Services[leftEndstop ? UserPanel.CalibrateLeftEndstopQualifier : UserPanel.CalibrateRightEndstopQualifier];
    if (this.calibrationService != (Service) null)
    {
      this.CalibrationIncomplete = true;
      this.Busy = true;
      this.calibrationService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.endstopCalibrationService_ServiceCompleteEvent);
      this.calibrationService.Execute(false);
    }
    else
      this.ReportCalibrationFailed(Resources.Message_UnableToPerformCalibrationMissingService);
  }

  private void endstopCalibrationService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.calibrationService))
      return;
    this.Busy = false;
    this.calibrationService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.endstopCalibrationService_ServiceCompleteEvent);
    if (e.Succeeded && this.ConvertChoiceValueObjectToRawValue(this.calibrationService.OutputValues[0]) <= 1)
      this.PerformNextStep();
    else
      this.ReportCalibrationFailed(Resources.Message_EndstopCalibrationFailed);
  }

  private bool PerformResetCalibration(bool fireAndForget)
  {
    this.UpdateUI();
    this.resetCalibrationService = this.channel.Services[UserPanel.ResetCalibrationDataQualifier];
    if (!(this.resetCalibrationService != (Service) null))
      return false;
    if (!fireAndForget)
    {
      this.Busy = true;
      this.resetCalibrationService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.resetCalibrationService_ServiceCompleteEvent);
    }
    this.resetCalibrationService.InputValues[0].Value = (object) this.resetCalibrationService.InputValues[0].Choices.GetItemFromRawValue((object) UserPanel.ResetCalibrationDataRawValue);
    this.resetCalibrationService.Execute(false);
    return true;
  }

  private void resetCalibrationService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.resetCalibrationService))
      return;
    this.Busy = false;
    this.resetCalibrationService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.resetCalibrationService_ServiceCompleteEvent);
    if (e.Succeeded)
      this.PerformNextStep();
    else
      this.ReportCalibrationFailed(Resources.Message_UnableToResetCalibrationValues);
  }

  private void PerformTorsionBarTorqueCalibration()
  {
    this.UpdateUI();
    this.torsionBarCalibrationProcedure = new StartMonitorStopServiceSharedProcedure("Torsion Bar Torque Calibration", "SP_TorsionBarTorqueCalibration", new ServiceCall(UserPanel.ChannelName, UserPanel.CalibrateTorsionBarTorqueStartQualifier), new ServiceCall(UserPanel.ChannelName, UserPanel.CalibrateTorsionBarTorqueResultQualifier), new ServiceCall(UserPanel.ChannelName, UserPanel.CalibrateTorsionBarTorqueResultQualifier), 1000, Gradient.FromString("(Default),(0,Fault),(1,Fault),(2,Default),(3,Default),(4,Ok),(5,Default),(6,Default),(7,Default),(8,Fault)"), true, (IEnumerable<DataItemCondition>) null, false);
    if (this.torsionBarCalibrationProcedure == null || !((SharedProcedureBase) this.torsionBarCalibrationProcedure).CanStart)
      return;
    ((SharedProcedureBase) this.torsionBarCalibrationProcedure).StartComplete += new EventHandler<PassFailResultEventArgs>(this.torsionBarCalibrationProcedure_StartComplete);
    ((SharedProcedureBase) this.torsionBarCalibrationProcedure).StopComplete += new EventHandler<PassFailResultEventArgs>(this.torsionBarCalibrationProcedure_StopComplete);
    ((SharedProcedureBase) this.torsionBarCalibrationProcedure).Start();
  }

  private void torsionBarCalibrationProcedure_StartComplete(
    object sender,
    PassFailResultEventArgs e)
  {
    ((SharedProcedureBase) this.torsionBarCalibrationProcedure).StartComplete -= new EventHandler<PassFailResultEventArgs>(this.torsionBarCalibrationProcedure_StartComplete);
    if (((ResultEventArgs) e).Succeeded && e.Result != 0)
      return;
    this.ReportCalibrationFailed(Resources.Message_CalibrationFailedCouldNotStartTorsionBarCalibration, ((ResultEventArgs) e).Exception != null ? ((ResultEventArgs) e).Exception.Message : string.Empty);
  }

  private void torsionBarCalibrationProcedure_StopComplete(object sender, PassFailResultEventArgs e)
  {
    ((SharedProcedureBase) this.torsionBarCalibrationProcedure).StopComplete -= new EventHandler<PassFailResultEventArgs>(this.torsionBarCalibrationProcedure_StopComplete);
    if (((ResultEventArgs) e).Succeeded && e.Result == 1)
      this.PerformNextStep();
    else
      this.ReportCalibrationFailed(Resources.Message_CalibrationFailedDuringTorsionBarCalibration, ((ResultEventArgs) e).Exception != null ? ((ResultEventArgs) e).Exception.Message : string.Empty);
  }

  private void VerifyTorsionBarTorqueOffset()
  {
    this.torsionBarOffset = this.channel.Parameters[UserPanel.TorsionBarTorqueOffsetParameterQualifier];
    if (this.torsionBarOffset != null)
    {
      this.channel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.channelParameter_ReadComplete);
      this.channel.Parameters.ReadAll(false);
    }
    else
      this.ReportCalibrationFailed(Resources.Message_CalibrationFailedCouldNotFindTorsionBarOffsetParameter);
  }

  private void channelParameter_ReadComplete(object sender, ResultEventArgs e)
  {
    this.channel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.channelParameter_ReadComplete);
    double num = double.NaN;
    if (e.Succeeded)
    {
      try
      {
        num = Convert.ToDouble(this.torsionBarOffset.Value);
        if (num != double.NaN && num >= -1.0 * UserPanel.TorsionBarRangeValue && num <= UserPanel.TorsionBarRangeValue)
        {
          this.PerformNextStep();
          return;
        }
      }
      catch (FormatException ex)
      {
        this.AddLogLabel(string.Format(Resources.MessageFormat_TorsionBarOffsetInvalid0, (object) ex.Message));
      }
      catch (InvalidCastException ex)
      {
        this.AddLogLabel(string.Format(Resources.MessageFormat_TorsionBarOffsetInvalid0, (object) ex.Message));
      }
    }
    this.AddLogLabel(string.Format(Resources.MessageFormat_TorsionBarOffsetValues012, (object) e.Succeeded, this.torsionBarOffset.Value, (object) num));
    this.ReportCalibrationFailed(Resources.Message_CalibrationFailedTorsionBarOffsetFailedToReadOrOutOfRange);
  }

  private void buttonNext_Click(object sender, EventArgs e) => this.PerformNextStep();

  private void buttonReset_Click(object sender, EventArgs e) => this.ResetToBeginning();

  private void dialInstrumentSteeringWheelAngle_RepresentedStateChanged(object sender, EventArgs e)
  {
    if (!this.monitorCurrentSteeringWheelAngle || !this.ConfirmSteeringWheelAngleIsValid())
      return;
    this.monitorCurrentSteeringWheelAngle = false;
    this.PerformNextStep();
  }

  private void AddLogLabel(string text)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void SetVrduChannel(Channel newChannel)
  {
    if (this.vrduChannel != null)
    {
      this.vrduChannel.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
      this.vrduChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    this.vrduChannel = newChannel;
    if (this.vrduChannel != null)
    {
      this.vrduChannel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
      this.vrduChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    this.UpdateDisplayedFaults();
  }

  private void UpdateDisplayedFaults()
  {
    if (!this.CalibrationInProgress)
    {
      bool hasActiveFaults = this.hasActiveFaults;
      this.listViewFaultCodes.BeginUpdate();
      ((ListView) this.listViewFaultCodes).Items.Clear();
      foreach (Tuple<string, string, string> vrduFault in this.VrduFaultList)
      {
        Tuple<string, string, string> fault = vrduFault;
        if (this.vrduChannel != null)
        {
          FaultCode faultCode = this.vrduChannel.FaultCodes.FirstOrDefault<FaultCode>((Func<FaultCode, bool>) (fc1 => fc1.FaultCodeIncidents.Count > 0 && fc1.FaultCodeIncidents.Current != null && fc1.FaultCodeIncidents.Current.Active == ActiveStatus.Active && fc1.FaultCodeIncidents.Current.FaultCode.Number.Equals(fault.Item2) && fc1.FaultCodeIncidents.Current.FaultCode.Mode.Equals(fault.Item3)));
          if (faultCode != null)
          {
            ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(new string[4]
            {
              this.vrduChannel.Ecu.Name,
              faultCode.Text,
              faultCode.Number,
              faultCode.Mode
            });
            ((ListViewItem) listViewExGroupItem).ForeColor = Color.Red;
            ((ListView) this.listViewFaultCodes).Items.Add((ListViewItem) listViewExGroupItem);
          }
        }
      }
      this.listViewFaultCodes.EndUpdate();
      this.HasActiveFaults = ((ListView) this.listViewFaultCodes).Items.Count > 0;
      if (this.HasActiveFaults)
        this.currentStep = new CalibrationStep(Resources.Message_ResolveTheActiveFaultsBeforeBeginning, string.Empty, CalibrationActions.DisplayText, false);
      else if (hasActiveFaults)
        this.ResetToBeginning();
    }
    this.UpdateUI();
  }

  private void CheckIfEngineIsStarted()
  {
    this.EngineStarted = this.digitalReadoutInstrumentEngineState.RepresentedState == 1;
    if (this.EngineStarted)
      this.PerformNextStep();
    else
      this.UpdateUI();
  }

  private void FaultCodes_FaultCodesUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateDisplayedFaults();
  }

  private void digitalReadoutInstrumentEngineState_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    if (this.currentStep == null || this.currentStep.Action != CalibrationActions.StartEngine)
      return;
    this.CheckIfEngineIsStarted();
  }

  private void dialInstrumentTorsionBarTorqueOffset_DataChanged(object sender, EventArgs e)
  {
    if (!this.SupportsTorsionBarCalibration || this.InstrumentsAreInitialized || ((SingleInstrumentBase) this.dialInstrumentTorsionBarTorqueOffset).DataItem.RawValue == null)
      return;
    this.InstrumentsAreInitialized = true;
    this.ResetToBeginning();
  }

  private void dialInstrumentRightEndstop_DataChanged(object sender, EventArgs e)
  {
    if (this.SupportsTorsionBarCalibration || this.InstrumentsAreInitialized || ((SingleInstrumentBase) this.dialInstrumentRightEndstop).DataItem.RawValue == null)
      return;
    this.InstrumentsAreInitialized = true;
    this.ResetToBeginning();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanelInstruments = new TableLayoutPanel();
    this.dialInstrumentRightEndstop = new DialInstrument();
    this.dialInstrument2 = new DialInstrument();
    this.dialInstrumentSteeringWheelAngle = new DialInstrument();
    this.digitalReadoutInstrumentLeftEndstopCalibration = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentRightEndstopCalibration = new DigitalReadoutInstrument();
    this.dialInstrumentTorsionBarTorqueOffset = new DialInstrument();
    this.dialInstrumentCurrentStep = new DialInstrument();
    this.seekTimeListView1 = new SeekTimeListView();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.buttonReset = new Button();
    this.buttonNext = new Button();
    this.digitalReadoutInstrumentEngineState = new DigitalReadoutInstrument();
    this.tableLayoutPanelLabels = new TableLayoutPanel();
    this.scalingLabelCurrentStep = new ScalingLabel();
    this.scalingLabelCurrentStepSubText = new ScalingLabel();
    this.listViewFaultCodes = new ListViewEx();
    this.columnHeaderChannel = new ColumnHeader();
    this.columnHeaderNumber = new ColumnHeader();
    this.columnHeaderMode = new ColumnHeader();
    this.columnHeaderStatus = new ColumnHeader();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanelInstruments).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanelLabels).SuspendLayout();
    ((ISupportInitialize) this.listViewFaultCodes).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelInstruments, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.dialInstrumentCurrentStep, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentEngineState, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelLabels, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.listViewFaultCodes, 1, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelInstruments, 4);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.dialInstrumentRightEndstop, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.dialInstrument2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.dialInstrumentSteeringWheelAngle, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentLeftEndstopCalibration, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrument3, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.digitalReadoutInstrumentRightEndstopCalibration, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInstruments).Controls.Add((Control) this.dialInstrumentTorsionBarTorqueOffset, 3, 0);
    ((Control) this.tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
    this.dialInstrumentRightEndstop.AngleRange = 180.0;
    this.dialInstrumentRightEndstop.AngleStart = 0.0;
    componentResourceManager.ApplyResources((object) this.dialInstrumentRightEndstop, "dialInstrumentRightEndstop");
    this.dialInstrumentRightEndstop.FontGroup = "DialGroup";
    ((SingleInstrumentBase) this.dialInstrumentRightEndstop).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrumentRightEndstop).Gradient.Initialize((ValueState) 3, 2, "°");
    ((AxisSingleInstrumentBase) this.dialInstrumentRightEndstop).Gradient.Modify(1, -1100.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrumentRightEndstop).Gradient.Modify(2, -800.0, (ValueState) 3);
    ((SingleInstrumentBase) this.dialInstrumentRightEndstop).Instrument = new Qualifier((QualifierTypes) 1, "APS301T", "DT_Endstop_Right_Endstop");
    ((Control) this.dialInstrumentRightEndstop).Name = "dialInstrumentRightEndstop";
    ((AxisSingleInstrumentBase) this.dialInstrumentRightEndstop).PreferredAxisRange = new AxisRange(-1200.0, 1.0, (string) null);
    ((SingleInstrumentBase) this.dialInstrumentRightEndstop).UnitAlignment = StringAlignment.Near;
    ((SingleInstrumentBase) this.dialInstrumentRightEndstop).DataChanged += new EventHandler(this.dialInstrumentRightEndstop_DataChanged);
    this.dialInstrument2.AngleRange = 180.0;
    this.dialInstrument2.AngleStart = 0.0;
    componentResourceManager.ApplyResources((object) this.dialInstrument2, "dialInstrument2");
    this.dialInstrument2.FontGroup = "DialGroup";
    ((SingleInstrumentBase) this.dialInstrument2).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrument2).Gradient.Initialize((ValueState) 3, 2, "°");
    ((AxisSingleInstrumentBase) this.dialInstrument2).Gradient.Modify(1, 800.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrument2).Gradient.Modify(2, 1100.0, (ValueState) 3);
    ((SingleInstrumentBase) this.dialInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "APS301T", "DT_Endstop_Left_Endstop");
    ((Control) this.dialInstrument2).Name = "dialInstrument2";
    ((AxisSingleInstrumentBase) this.dialInstrument2).PreferredAxisRange = new AxisRange(-1.0, 1200.0, (string) null);
    ((SingleInstrumentBase) this.dialInstrument2).UnitAlignment = StringAlignment.Near;
    this.dialInstrumentSteeringWheelAngle.AngleRange = 180.0;
    this.dialInstrumentSteeringWheelAngle.AngleStart = 0.0;
    componentResourceManager.ApplyResources((object) this.dialInstrumentSteeringWheelAngle, "dialInstrumentSteeringWheelAngle");
    this.dialInstrumentSteeringWheelAngle.FontGroup = "DialGroup";
    ((SingleInstrumentBase) this.dialInstrumentSteeringWheelAngle).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrumentSteeringWheelAngle).Gradient.Initialize((ValueState) 3, 2);
    ((AxisSingleInstrumentBase) this.dialInstrumentSteeringWheelAngle).Gradient.Modify(1, 0.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrumentSteeringWheelAngle).Gradient.Modify(2, 0.5, (ValueState) 3);
    ((SingleInstrumentBase) this.dialInstrumentSteeringWheelAngle).Instrument = new Qualifier((QualifierTypes) 1, "ABS02T", "DT_Steering_wheel_angle_sensor_Read_Steering_wheel_angle");
    ((Control) this.dialInstrumentSteeringWheelAngle).Name = "dialInstrumentSteeringWheelAngle";
    ((AxisSingleInstrumentBase) this.dialInstrumentSteeringWheelAngle).PreferredAxisRange = new AxisRange(-1.0, 1.0, "");
    ((SingleInstrumentBase) this.dialInstrumentSteeringWheelAngle).UnitAlignment = StringAlignment.Near;
    this.dialInstrumentSteeringWheelAngle.RepresentedStateChanged += new EventHandler(this.dialInstrumentSteeringWheelAngle_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentLeftEndstopCalibration, "digitalReadoutInstrumentLeftEndstopCalibration");
    this.digitalReadoutInstrumentLeftEndstopCalibration.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentLeftEndstopCalibration).FreezeValue = false;
    this.digitalReadoutInstrumentLeftEndstopCalibration.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentLeftEndstopCalibration.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentLeftEndstopCalibration.Gradient.Modify(2, 1.0, (ValueState) 5);
    this.digitalReadoutInstrumentLeftEndstopCalibration.Gradient.Modify(3, 2.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentLeftEndstopCalibration).Instrument = new Qualifier((QualifierTypes) 1, "APS301T", "DT_Endstop_Calibration_Status_Left_Calibration_State");
    ((Control) this.digitalReadoutInstrumentLeftEndstopCalibration).Name = "digitalReadoutInstrumentLeftEndstopCalibration";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentLeftEndstopCalibration).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument3, "digitalReadoutInstrument3");
    this.digitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).FreezeValue = false;
    this.digitalReadoutInstrument3.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrument3.Gradient.Modify(1, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument3.Gradient.Modify(2, 15.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "APS301T", "DT_Steering_Angle_Calibration_Status_Calibration_Status");
    ((Control) this.digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
    ((SingleInstrumentBase) this.digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentRightEndstopCalibration, "digitalReadoutInstrumentRightEndstopCalibration");
    this.digitalReadoutInstrumentRightEndstopCalibration.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRightEndstopCalibration).FreezeValue = false;
    this.digitalReadoutInstrumentRightEndstopCalibration.Gradient.Initialize((ValueState) 0, 3);
    this.digitalReadoutInstrumentRightEndstopCalibration.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentRightEndstopCalibration.Gradient.Modify(2, 1.0, (ValueState) 5);
    this.digitalReadoutInstrumentRightEndstopCalibration.Gradient.Modify(3, 2.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRightEndstopCalibration).Instrument = new Qualifier((QualifierTypes) 1, "APS301T", "DT_Endstop_Calibration_Status_Right_Calibration_State");
    ((Control) this.digitalReadoutInstrumentRightEndstopCalibration).Name = "digitalReadoutInstrumentRightEndstopCalibration";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRightEndstopCalibration).UnitAlignment = StringAlignment.Near;
    this.dialInstrumentTorsionBarTorqueOffset.AngleRange = 180.0;
    this.dialInstrumentTorsionBarTorqueOffset.AngleStart = 0.0;
    componentResourceManager.ApplyResources((object) this.dialInstrumentTorsionBarTorqueOffset, "dialInstrumentTorsionBarTorqueOffset");
    this.dialInstrumentTorsionBarTorqueOffset.FontGroup = "DialGroup";
    ((SingleInstrumentBase) this.dialInstrumentTorsionBarTorqueOffset).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.dialInstrumentTorsionBarTorqueOffset).Gradient.Initialize((ValueState) 3, 2, "Nm");
    ((AxisSingleInstrumentBase) this.dialInstrumentTorsionBarTorqueOffset).Gradient.Modify(1, 0.0, (ValueState) 1);
    ((AxisSingleInstrumentBase) this.dialInstrumentTorsionBarTorqueOffset).Gradient.Modify(2, 0.5, (ValueState) 3);
    ((SingleInstrumentBase) this.dialInstrumentTorsionBarTorqueOffset).Instrument = new Qualifier((QualifierTypes) 4, "APS301T", "TorsionBarTorqueOffset");
    ((Control) this.dialInstrumentTorsionBarTorqueOffset).Name = "dialInstrumentTorsionBarTorqueOffset";
    ((AxisSingleInstrumentBase) this.dialInstrumentTorsionBarTorqueOffset).PreferredAxisRange = new AxisRange(-1.0, 1.0, (string) null);
    ((SingleInstrumentBase) this.dialInstrumentTorsionBarTorqueOffset).UnitAlignment = StringAlignment.Near;
    ((SingleInstrumentBase) this.dialInstrumentTorsionBarTorqueOffset).DataChanged += new EventHandler(this.dialInstrumentTorsionBarTorqueOffset_DataChanged);
    this.dialInstrumentCurrentStep.AngleRange = 180.0;
    this.dialInstrumentCurrentStep.AngleStart = 0.0;
    componentResourceManager.ApplyResources((object) this.dialInstrumentCurrentStep, "dialInstrumentCurrentStep");
    this.dialInstrumentCurrentStep.FontGroup = (string) null;
    ((SingleInstrumentBase) this.dialInstrumentCurrentStep).FreezeValue = false;
    ((SingleInstrumentBase) this.dialInstrumentCurrentStep).Instrument = new Qualifier((QualifierTypes) 1, "APS301T", "DT_Steering_Angle_Steering_Angle");
    ((Control) this.dialInstrumentCurrentStep).Name = "dialInstrumentCurrentStep";
    ((AxisSingleInstrumentBase) this.dialInstrumentCurrentStep).PreferredAxisRange = new AxisRange(-50.0, 50.0, "");
    ((SingleInstrumentBase) this.dialInstrumentCurrentStep).ShowValueReadout = false;
    ((SingleInstrumentBase) this.dialInstrumentCurrentStep).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "APSCalibration";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.buttonReset, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.buttonNext, 0, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.buttonReset, "buttonReset");
    this.buttonReset.Name = "buttonReset";
    this.buttonReset.UseCompatibleTextRendering = true;
    this.buttonReset.UseVisualStyleBackColor = true;
    this.buttonReset.Click += new EventHandler(this.buttonReset_Click);
    componentResourceManager.ApplyResources((object) this.buttonNext, "buttonNext");
    this.buttonNext.Name = "buttonNext";
    this.buttonNext.UseCompatibleTextRendering = true;
    this.buttonNext.UseVisualStyleBackColor = true;
    this.buttonNext.Click += new EventHandler(this.buttonNext_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentEngineState, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineState, "digitalReadoutInstrumentEngineState");
    this.digitalReadoutInstrumentEngineState.FontGroup = "DialGroup";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).FreezeValue = false;
    this.digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentEngineState.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentEngineState.Gradient.Initialize((ValueState) 0, 7);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(2, 1.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(4, 3.0, (ValueState) 1);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(6, 5.0, (ValueState) 0);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(7, 15.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_ESC_Diagnostic_Displayables_DDESC_EngineState");
    ((Control) this.digitalReadoutInstrumentEngineState).Name = "digitalReadoutInstrumentEngineState";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentEngineState.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentEngineState_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelLabels, "tableLayoutPanelLabels");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelLabels, 3);
    ((TableLayoutPanel) this.tableLayoutPanelLabels).Controls.Add((Control) this.scalingLabelCurrentStep, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelLabels).Controls.Add((Control) this.scalingLabelCurrentStepSubText, 0, 1);
    ((Control) this.tableLayoutPanelLabels).Name = "tableLayoutPanelLabels";
    this.scalingLabelCurrentStep.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanelLabels).SetColumnSpan((Control) this.scalingLabelCurrentStep, 2);
    componentResourceManager.ApplyResources((object) this.scalingLabelCurrentStep, "scalingLabelCurrentStep");
    this.scalingLabelCurrentStep.FontGroup = (string) null;
    this.scalingLabelCurrentStep.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelCurrentStep).Name = "scalingLabelCurrentStep";
    this.scalingLabelCurrentStepSubText.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanelLabels).SetColumnSpan((Control) this.scalingLabelCurrentStepSubText, 2);
    componentResourceManager.ApplyResources((object) this.scalingLabelCurrentStepSubText, "scalingLabelCurrentStepSubText");
    this.scalingLabelCurrentStepSubText.FontGroup = (string) null;
    this.scalingLabelCurrentStepSubText.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelCurrentStepSubText).Name = "scalingLabelCurrentStepSubText";
    this.listViewFaultCodes.CanDelete = false;
    ((ListView) this.listViewFaultCodes).Columns.AddRange(new ColumnHeader[4]
    {
      this.columnHeaderChannel,
      this.columnHeaderNumber,
      this.columnHeaderMode,
      this.columnHeaderStatus
    });
    componentResourceManager.ApplyResources((object) this.listViewFaultCodes, "listViewFaultCodes");
    this.listViewFaultCodes.EditableColumn = -1;
    ((Control) this.listViewFaultCodes).Name = "listViewFaultCodes";
    ((ListView) this.listViewFaultCodes).UseCompatibleStateImageBehavior = false;
    componentResourceManager.ApplyResources((object) this.columnHeaderChannel, "columnHeaderChannel");
    componentResourceManager.ApplyResources((object) this.columnHeaderNumber, "columnHeaderNumber");
    componentResourceManager.ApplyResources((object) this.columnHeaderMode, "columnHeaderMode");
    componentResourceManager.ApplyResources((object) this.columnHeaderStatus, "columnHeaderStatus");
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInstruments).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanelLabels).ResumeLayout(false);
    ((ISupportInitialize) this.listViewFaultCodes).EndInit();
    ((Control) this).ResumeLayout(false);
  }
}
