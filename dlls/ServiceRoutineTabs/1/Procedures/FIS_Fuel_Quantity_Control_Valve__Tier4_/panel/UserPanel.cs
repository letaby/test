// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Quantity_Control_Valve__Tier4_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Quantity_Control_Valve__Tier4_.panel;

public class UserPanel : CustomPanel
{
  private const string MCMName = "MCM21T";
  private const string Fuel_Metering_Unit_FMU_Desired_Current_Qualifier = "DT_AS121_EU_Fuel_Metering_Unit_FMU_desired_current";
  private const string Quantity_Control_Valve_Current_Qualifier = "DT_AS100_Quantity_Control_Valve_Current";
  private const string Quantity_Control_Valve_Adaptation_Positive_Qualifier = "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Positive";
  private const string Quantity_Control_Valve_Adaptation_Negative_Qualifier = "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Negative";
  private const string CoolantTemperature_Qualifier = "DT_AS013_Coolant_Temperature";
  private const string RailPressure_Qualifier = "DT_AS043_Rail_Pressure";
  private const string FmuStickTestStatus_Qualifier = "DT_AS122_Fuel_Metering_Unit_Stick_Diagnosis_State";
  private const string FmuStickTestResult_Qualifier = "DT_AS123_Fuel_Metering_Unit_Diagnosis_Error_State";
  private const int TabControlSelectionFMUStickTest = 0;
  private const int TabControlSelectionFMUAdaptation = 1;
  private Channel channel;
  private Instrument instrumentCoolantTemperature;
  private Instrument instrumentFuelTemperature;
  private Instrument instrumentEngineSpeed;
  private Instrument instrumentEngineState;
  private Instrument instrumentRailPressure;
  private Instrument instrumentVehicleStatusCheck;
  private Instrument instrumentIgnitionState;
  private Instrument instrumentStarterSignalState;
  private Instrument instrumentValveSensorCurrent;
  private Instrument instrumentFmuStickTestStatus;
  private Instrument instrumentFmuStickTestResult;
  private Qualifier[] ambientQualifiers;
  private Qualifier[] fmuCurrentQualifiers;
  private Qualifier[] fmuStickTestQualifiers;
  private ToolTip tooltipFmuStickTestIgnitionAndEngineStatus;
  private ToolTip tooltipFmuStickTestFaultCodesStatus;
  private ToolTip tooltipFmuStickTestInstrumentsStatus;
  private ToolTip tooltipFmuStickTestVehicleCheckStatus;
  private string[] fmuFaultCodes;
  private WarningManager warningManager;
  private bool fmuTestRunning = false;
  private Timer fmuTestTimer;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrumentQuantity_Control_Valve_Current;
  private DigitalReadoutInstrument digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current;
  private DigitalReadoutInstrument digitalReadoutInstrumentCoolantTemperature;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;
  private TableLayoutPanel tableLayoutPanelHeader;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineLoad;
  private ChartInstrument chartInstrument1;
  private CheckBox checkBox1;
  private CheckBox checkBox2;
  private DigitalReadoutInstrument digitalReadoutInstrumentFuelMass;
  private TabControl tabControl1;
  private TabPage tabPageFmuAdaption;
  private TableLayoutPanel tableLayoutPanel2;
  private ScalingLabel scalingLabel2;
  private DigitalReadoutInstrument digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive;
  private DigitalReadoutInstrument digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative;
  private Button buttonReadFMUAdaptation;
  private Button buttonResetFMUAdaptation;
  private TabPage tabPageFmuStickTest;
  private TableLayoutPanel tableLayoutPanelFmuStickTest;
  private Button buttonFmuStickTestStop;
  private Button buttonFmuStickTestStart;
  private DigitalReadoutInstrument digitalReadoutInstrumentRailPressure;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFmuStickTestInstrumentsStatus;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFmuStickTestFaultCodesStatus;
  private Checkmark checkmarkFmuStickTestIgnitionAndEngineStatus;
  private Checkmark checkmarkInstrumentsStatus;
  private Checkmark checkmarkFmuStickTestFaultCodesStatus;
  private TableLayoutPanel tableLayoutPanel3;
  private CheckBox checkBox3;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFmuStickTestResultValueTitle;
  private ScalingLabel scalingLabelFmuStickTestResultValue;
  private Checkmark checkmarkFmuStickTestVehicleCheckStatus;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFmuStickTestVehicleCheckStatus;
  private TableLayoutPanel tableLayoutPanelFmuStickTestResults;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFmuStickTestIgnitionAndEngineStatus;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFmuStickTestStatusTitle;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelFmuStickTestResult;
  private ScalingLabel scalingLabelFmuStickTestStatus;
  private ScalingLabel scalingLabelFmuStickTestResult;
  private SeekTimeListView seekTimeListView;
  private DigitalReadoutInstrument digitalReadoutInstrumentFuelTemperature;

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningManager = new WarningManager(string.Empty, Resources.Message_FISFuelQuantityControlValve, this.seekTimeListView.RequiredUserLabelPrefix);
    this.tooltipFmuStickTestIgnitionAndEngineStatus = new ToolTip();
    this.tooltipFmuStickTestFaultCodesStatus = new ToolTip();
    this.tooltipFmuStickTestInstrumentsStatus = new ToolTip();
    this.tooltipFmuStickTestVehicleCheckStatus = new ToolTip();
    this.InitQualifiers();
    ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.fmuCurrentQualifiers);
    ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.fmuStickTestQualifiers);
    this.checkBox1.CheckStateChanged += new EventHandler(this.CheckBoxCheckStateChanged);
    this.checkBox2.CheckStateChanged += new EventHandler(this.CheckBoxCheckStateChanged);
    this.checkBox3.CheckStateChanged += new EventHandler(this.CheckBoxCheckStateChanged);
    this.buttonReadFMUAdaptation.Click += new EventHandler(this.OnReadFMUAdaptationClick);
    this.buttonResetFMUAdaptation.Click += new EventHandler(this.buttonResetFMUAdaptationClick);
    this.tabControl1.SelectedIndexChanged += new EventHandler(this.tabControl1SelectedIndexChanged);
    this.InitFmuStickTest();
  }

  private void InitQualifiers()
  {
    this.ambientQualifiers = new Qualifier[5]
    {
      new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed"),
      new Qualifier((QualifierTypes) 1, "virtual", "engineTorque"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass"),
      new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp"),
      new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp")
    };
    this.fmuCurrentQualifiers = new Qualifier[2]
    {
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS121_EU_Fuel_Metering_Unit_FMU_desired_current"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS100_Quantity_Control_Valve_Current")
    };
    this.fmuStickTestQualifiers = new Qualifier[3]
    {
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS043_Rail_Pressure"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS122_Fuel_Metering_Unit_Stick_Diagnosis_State"),
      new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS123_Fuel_Metering_Unit_Diagnosis_Error_State")
    };
  }

  private static object GetInstrumentCurrentValue(Instrument instrument)
  {
    object instrumentCurrentValue = (object) null;
    if (instrument != (Instrument) null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
      instrumentCurrentValue = instrument.InstrumentValues.Current.Value;
    return instrumentCurrentValue;
  }

  private void InitFmuStickTest()
  {
    this.buttonFmuStickTestStart.MouseClick += new MouseEventHandler(this.ButtonFmuStickTestStartMouseClick);
    this.buttonFmuStickTestStop.MouseClick += new MouseEventHandler(this.buttonFmuStickTestStopMouseClick);
    this.fmuTestTimer = new Timer();
    this.fmuTestTimer.Interval = 2000;
    this.fmuTestTimer.Tick += new EventHandler(this.FmuTestTimerTick);
    this.FmuStickTestInitFaults();
  }

  private void ExecuteService(string serviceName, ServiceCompleteEventHandler serviceCompleteEvent)
  {
    if (this.channel == null)
      return;
    Service service = this.channel.Services[serviceName];
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent += serviceCompleteEvent;
      service.Execute(false);
    }
  }

  private void FmuTestTimerTick(object sender, EventArgs e)
  {
    this.fmuTestTimer.Stop();
    if (!this.fmuTestRunning)
      return;
    this.FmuStickTestPollStatusCall();
    this.fmuTestTimer.Start();
  }

  private void ButtonFmuStickTestStartMouseClick(object sender, MouseEventArgs e)
  {
    if (!this.warningManager.RequestContinue())
      return;
    this.FmuStickTestStart();
  }

  private void FmuStickTestStart()
  {
    if (MessageBox.Show((IWin32Window) this, Resources.Message_PressTheOKButtonToBeginTheTestWhenThisDialogClosesPleaseTurnTheIgnitionKeyToCrankTheEngineTheEngineWillCrankButNotStartWhenItStopsCrankingReleaseTheKeyNN + Resources.Message_ResultsWillBeDisplayedBelow, Resources.Message_PreparingToStartTheFMUStickTest, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) != DialogResult.OK)
      return;
    this.FmuStickTestStartServiceCall();
  }

  private void FmuStickTestStartServiceCall()
  {
    this.ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Start_active_status", new ServiceCompleteEventHandler(this.FmuStickTestServiceStartCompleteEvent));
    this.FmuStickTestClearMessages();
    this.FmuStickTestDisplayMessage(Resources.Message_FMUStickTestStartRequested);
  }

  private void FmuStickTestServiceStartCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.FmuStickTestServiceStartCompleteEvent);
    if (e.Succeeded)
    {
      ServiceOutputValue outputValue = service.OutputValues[0];
      StringBuilder stringBuilder = new StringBuilder();
      if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 1))
      {
        this.fmuTestRunning = true;
        this.fmuTestTimer.Start();
        this.FmuStickTestDisplayMessage(Resources.Message_RunningFMUStickTest + Resources.Message_ContinueCrankingTheEngineUntilTheTestFinishedInstrumentSaysRPG_DIA_FMU_TESTEDOrAnErrorIsDisplayed);
      }
      else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 2))
      {
        this.fmuTestRunning = true;
        stringBuilder.Append(Resources.Message_FMUTestIsBusyPressFMUTestStopButtonToTerminateTheTest);
        this.FmuStickTestDisplayMessage(stringBuilder.ToString());
        this.fmuTestTimer.Start();
      }
      else
      {
        this.fmuTestRunning = false;
        stringBuilder.Append(Resources.Message_TestFailedToStart);
        if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 4))
          stringBuilder.Append(Resources.Message_NoIgnitionDetected + Resources.Message_StartTheTestAgainAndCrankTheEngineImmediatelyAfterSelectingOKToCloseTheDialog);
        else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 8))
          stringBuilder.Append(Resources.Message_EngineSpeedIsNotZeroOrARailPressureExists);
        else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 16 /*0x10*/))
          stringBuilder.Append(Resources.Message_TheFMUTestIsNotSupportedByThisVersionOfMCM2Software3);
        else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 32 /*0x20*/))
          stringBuilder.Append(Resources.Message_ThereIsASensorOrLeakError);
        else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 24))
          stringBuilder.Append(Resources.Message_TheFMUTestIsNotSupportedByThisVersionOfMCM2Software2);
        else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 40))
          stringBuilder.Append(Resources.Message_EngineSpeedIsNotZeroOrARailPressureExistsAndThereIsASensorOrLeakError);
        else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 48 /*0x30*/))
          stringBuilder.Append(Resources.Message_TheFMUTestIsNotSupportedByThisVersionOfMCM2Software1);
        else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 56))
          stringBuilder.Append(Resources.Message_TheFMUTestIsNotSupportedByThisVersionOfMCM2Software);
        else
          stringBuilder.Append(Resources.Message_UnknownResponseFromTheECU + outputValue.Value.ToString());
        this.FmuStickTestDisplayMessage(stringBuilder.ToString());
      }
    }
    else
      this.FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestError0, (object) e.Exception.Message));
  }

  private void FmuStickTestPollStatusCall()
  {
    this.ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Request_Results_result_status", new ServiceCompleteEventHandler(this.FmuStickTestPollStatusServiceCompleteEvent));
  }

  private void FmuStickTestPollStatusServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.FmuStickTestPollStatusServiceCompleteEvent);
    if (e.Succeeded)
    {
      ServiceOutputValue outputValue = service.OutputValues[0];
      if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 1))
      {
        this.fmuTestRunning = true;
        this.FmuStickTestDisplayMessage(Resources.Message_TheFMUStickTestHasStarted);
      }
      else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 2))
      {
        this.fmuTestRunning = true;
        this.FmuStickTestDisplayMessage(Resources.Message_TheFMUStickTestIsRunning);
      }
      else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 4))
      {
        this.fmuTestRunning = false;
        this.FmuStickTestDisplayMessage(Resources.Message_TheFMUStickTestIsComplete);
        this.FmuTestRequestResults();
      }
      else
      {
        if (outputValue.Value != (object) outputValue.Choices.GetItemFromRawValue((object) 8))
          return;
        this.fmuTestRunning = false;
        this.FmuStickTestDisplayMessage(Resources.Message_TheFMUStickTestWasStopped);
      }
    }
    else
    {
      this.fmuTestRunning = false;
      this.FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestError01, (object) e.Exception.Message));
    }
  }

  private void FmuTestRequestResults()
  {
    this.ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Request_Results_result_error_bit", new ServiceCompleteEventHandler(this.FmuTestRequestResultsResultErrorBitServiceCompleteEvent));
    this.ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Request_Results_result_fmu_value", new ServiceCompleteEventHandler(this.FmuTestRequestResultsResultFmuValueCompleteEvent));
    this.fmuTestRunning = false;
  }

  private void FmuTestRequestResultsResultErrorBitServiceCompleteEvent(
    object sender,
    ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.FmuTestRequestResultsResultErrorBitServiceCompleteEvent);
    if (e.Succeeded)
    {
      ServiceOutputValue outputValue = service.OutputValues[0];
      if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 0))
      {
        this.scalingLabelFmuStickTestResult.RepresentedState = (ValueState) 1;
        ((Control) this.scalingLabelFmuStickTestResult).Text = Resources.Message_FMUIsNOTSticking1;
        this.FmuStickTestDisplayMessage(Resources.Message_FMUIsNOTSticking);
      }
      else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) 1))
      {
        this.scalingLabelFmuStickTestResult.RepresentedState = (ValueState) 3;
        ((Control) this.scalingLabelFmuStickTestResult).Text = Resources.Message_FMUISSticking1;
        this.FmuStickTestDisplayMessage(Resources.Message_FMUISSticking);
      }
      else if (outputValue.Value == (object) outputValue.Choices.GetItemFromRawValue((object) (int) byte.MaxValue))
      {
        this.scalingLabelFmuStickTestResult.RepresentedState = (ValueState) 2;
        ((Control) this.scalingLabelFmuStickTestResult).Text = Resources.Message_FMUSignalIsNotAvailable1;
        this.FmuStickTestDisplayMessage(Resources.Message_FMUSignalIsNotAvailable);
      }
      else
      {
        this.scalingLabelFmuStickTestResult.RepresentedState = (ValueState) 2;
        ((Control) this.scalingLabelFmuStickTestResult).Text = Resources.Message_FMUValueUnknown1;
        this.FmuStickTestDisplayMessage(Resources.Message_FMUValueUnknown);
      }
    }
    else
    {
      this.scalingLabelFmuStickTestResult.RepresentedState = (ValueState) 2;
      ((Control) this.scalingLabelFmuStickTestResult).Text = string.Format(Resources.MessageFormat_FMUStickTestError03, (object) e.Exception.Message);
      this.FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestError02, (object) e.Exception.Message));
    }
  }

  private void FmuTestRequestResultsResultFmuValueCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.FmuTestRequestResultsResultFmuValueCompleteEvent);
    if (e.Succeeded)
    {
      ServiceOutputValue outputValue = service.OutputValues[0];
      if (outputValue == null || outputValue.Value == null)
        return;
      this.scalingLabelFmuStickTestResultValue.RepresentedState = (ValueState) 1;
      ((Control) this.scalingLabelFmuStickTestResultValue).Text = $"{outputValue.Value}";
      this.FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestResults0MA, (object) UserPanel.ObjectToDouble(outputValue.Value, "mA")));
    }
    else
    {
      this.scalingLabelFmuStickTestResultValue.RepresentedState = (ValueState) 2;
      this.FmuStickTestDisplayMessage(string.Format(Resources.MessageFormat_FMUStickTestError04, (object) e.Exception.Message));
    }
  }

  private void buttonFmuStickTestStopMouseClick(object sender, MouseEventArgs e)
  {
    if (!this.fmuTestRunning)
      return;
    this.ExecuteService("RT_SR071_FMU_Stick_Diagnosis_Function_Stop", new ServiceCompleteEventHandler(this.FmuStickTestStopCompleteEvent));
  }

  private void FmuStickTestStopCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.FmuStickTestStopCompleteEvent);
    this.FmuStickTestDisplayMessage(Resources.Message_TestTerminatedByTheUser);
    this.fmuTestRunning = false;
  }

  private void FmuStickTestInitFaults()
  {
    this.fmuFaultCodes = new string[24]
    {
      "350405",
      "350406",
      "35040E",
      "35041F",
      "980D03",
      "980D04",
      "980D1F",
      "9A0D03",
      "9A0D04",
      "9A0D05",
      "9A0D07",
      "9D0002",
      "9D000A",
      "9D0012",
      "A40000",
      "A40001",
      "A40003",
      "A40004",
      "A40005",
      "A40014",
      "A40015",
      "A70207",
      "AE0003",
      "AE0004"
    };
  }

  private int FmuStickTestFaultsActive(ref string faultMessage)
  {
    int num = 0;
    StringBuilder stringBuilder = new StringBuilder();
    foreach (string fmuFaultCode in this.fmuFaultCodes)
    {
      if (this.channel != null && this.channel.FaultCodes != null)
      {
        FaultCode faultCode = this.channel.FaultCodes[fmuFaultCode];
        if (faultCode != null && faultCode.FaultCodeIncidents != null)
        {
          FaultCodeIncident current = faultCode.FaultCodeIncidents.Current;
          if (current != null && current.Active == ActiveStatus.Active)
          {
            ++num;
            stringBuilder.AppendLine($"{faultCode.Text}: ({faultCode.Number}/{faultCode.Mode})");
          }
        }
      }
    }
    faultMessage = stringBuilder.ToString();
    return num;
  }

  private double EngineSpeed
  {
    get
    {
      double engineSpeed = double.NaN;
      if (UserPanel.GetInstrumentCurrentValue(this.instrumentEngineSpeed) != null)
        engineSpeed = Convert.ToDouble(this.instrumentEngineSpeed.InstrumentValues.Current.Value);
      return engineSpeed;
    }
  }

  private UserPanel.EngineState EngineStatus
  {
    get
    {
      UserPanel.EngineState engineStatus = UserPanel.EngineState.Unknown;
      object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(this.instrumentEngineState);
      if (instrumentCurrentValue != null)
        engineStatus = instrumentCurrentValue != (object) this.instrumentEngineState.Choices.GetItemFromRawValue((object) 0) ? (instrumentCurrentValue != (object) this.instrumentEngineState.Choices.GetItemFromRawValue((object) 2) ? (instrumentCurrentValue != (object) this.instrumentEngineState.Choices.GetItemFromRawValue((object) 3) ? (instrumentCurrentValue != (object) this.instrumentEngineState.Choices.GetItemFromRawValue((object) -1) ? UserPanel.EngineState.Other : UserPanel.EngineState.Unknown) : UserPanel.EngineState.Idle) : UserPanel.EngineState.StarterEngaged) : UserPanel.EngineState.Stopped;
      return engineStatus;
    }
  }

  private UserPanel.BooleanAndUnknown VehicleCheckStatus
  {
    get
    {
      UserPanel.BooleanAndUnknown vehicleCheckStatus = UserPanel.BooleanAndUnknown.Unknown;
      object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(this.instrumentVehicleStatusCheck);
      if (instrumentCurrentValue != null)
      {
        if (instrumentCurrentValue == (object) this.instrumentVehicleStatusCheck.Choices.GetItemFromRawValue((object) 0))
          vehicleCheckStatus = UserPanel.BooleanAndUnknown.False;
        else if (instrumentCurrentValue == (object) this.instrumentVehicleStatusCheck.Choices.GetItemFromRawValue((object) 1))
          vehicleCheckStatus = UserPanel.BooleanAndUnknown.True;
      }
      return vehicleCheckStatus;
    }
  }

  private static bool InstrumentInSpec(Instrument instrument, DigitalReadoutInstrument reference)
  {
    bool flag = false;
    object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(instrument);
    double result;
    if (instrumentCurrentValue != null && double.TryParse(instrumentCurrentValue.ToString(), out result) && !double.IsNaN(result) && reference.Gradient.GetState(result, false) != 3)
      flag = true;
    return flag;
  }

  private void FmuStickTestUpdateEngineStatus()
  {
    bool flag = false;
    StringBuilder stringBuilder1 = new StringBuilder();
    StringBuilder stringBuilder2 = new StringBuilder();
    if (this.Online)
    {
      if (!UserPanel.InstrumentInSpec(this.instrumentEngineSpeed, this.digitalReadoutInstrumentEngineSpeed))
      {
        stringBuilder2.Append(Resources.Message_ErrorTheEngineIsRunningStopTheEngineButKeepIgnitionSwitchOn);
        stringBuilder1.Append(Resources.Message_NotReady1);
      }
      else
      {
        stringBuilder1.Append(Resources.Message_Ignition);
        ((Control) this.labelFmuStickTestIgnitionAndEngineStatus).Text = string.Empty;
        switch (this.EngineStatus)
        {
          case UserPanel.EngineState.Unknown:
            stringBuilder2.Append(Resources.Message_ErrorTheIgnitionStateOrTheEngineSpeedAreNotKnown);
            stringBuilder1.Append(Resources.Message_Unknown);
            break;
          case UserPanel.EngineState.Stopped:
            stringBuilder1.Append(Resources.Message_Ready);
            flag = true;
            break;
          case UserPanel.EngineState.StarterEngaged:
            stringBuilder2.Append(Resources.Message_ErrorTheStarterIsEnagedTurnTheKeySoThatTheIgnitionIsOnButNotStartingTheEngine);
            stringBuilder1.Append(Resources.Message_NotReady3);
            break;
          case UserPanel.EngineState.Idle:
            stringBuilder2.Append(Resources.Message_ErrorEngineIsRunningTurnTheEngineOffButLeaveTheIginitionOn);
            stringBuilder1.Append(Resources.Message_NotReady);
            break;
          case UserPanel.EngineState.Other:
            stringBuilder2.Append(Resources.Message_ErrorTheEngineIsNotStoppedTurnTheKeySoThatTheIgnitionIsOnButNotStartingTheEngine);
            stringBuilder1.Append(Resources.Message_NotReady2);
            break;
          default:
            throw new IndexOutOfRangeException("Undefined engine state");
        }
      }
    }
    else
    {
      flag = false;
      stringBuilder1.Append(Resources.Message_Offline);
    }
    this.checkmarkFmuStickTestIgnitionAndEngineStatus.Checked = flag;
    ((Control) this.labelFmuStickTestIgnitionAndEngineStatus).Text = stringBuilder1.ToString();
    this.tooltipFmuStickTestIgnitionAndEngineStatus.SetToolTip((Control) this.labelFmuStickTestIgnitionAndEngineStatus, stringBuilder2.ToString());
    this.FmuStickTestUpdateButtons();
  }

  private void FmuStickTestUpdateVehicleCheckStatus()
  {
    StringBuilder stringBuilder1 = new StringBuilder(Resources.Message_VehicleCheck);
    StringBuilder stringBuilder2 = new StringBuilder();
    bool flag;
    if (this.Online)
    {
      ((Control) this.labelFmuStickTestVehicleCheckStatus).Text = string.Empty;
      switch (this.VehicleCheckStatus)
      {
        case UserPanel.BooleanAndUnknown.False:
        case UserPanel.BooleanAndUnknown.Unknown:
          stringBuilder1.Append(Resources.Message_NotReady4);
          stringBuilder2.Append(Resources.Message_ErrorTheTransmissionMustBeInNuetralAndTheParkingBrakeMustBeON);
          flag = false;
          break;
        case UserPanel.BooleanAndUnknown.True:
          stringBuilder1.Append(Resources.Message_Ready1);
          flag = true;
          break;
        default:
          throw new IndexOutOfRangeException("New boolean and unknown status error.");
      }
    }
    else
    {
      flag = false;
      stringBuilder1.Append(Resources.Message_Offline1);
    }
    this.checkmarkFmuStickTestVehicleCheckStatus.Checked = flag;
    ((Control) this.labelFmuStickTestVehicleCheckStatus).Text = stringBuilder1.ToString();
    this.tooltipFmuStickTestVehicleCheckStatus.SetToolTip((Control) this.labelFmuStickTestVehicleCheckStatus, stringBuilder2.ToString());
    this.FmuStickTestUpdateButtons();
  }

  private void FmuStickTestUpdateInstrumentsStatus()
  {
    bool flag = true;
    int num = 0;
    StringBuilder stringBuilder = new StringBuilder();
    string str;
    if (this.Online)
    {
      stringBuilder.AppendLine(Resources.Message_InstrumentsNotReady);
      if (!UserPanel.InstrumentInSpec(this.instrumentFuelTemperature, this.digitalReadoutInstrumentFuelTemperature))
      {
        stringBuilder.AppendLine(string.Format(Resources.MessageFormat_0IsTooLow, (object) this.instrumentFuelTemperature.Name));
        ++num;
        flag = false;
      }
      if (!UserPanel.InstrumentInSpec(this.instrumentRailPressure, this.digitalReadoutInstrumentRailPressure))
      {
        stringBuilder.AppendLine(string.Format(Resources.MessageFormat_0IsTooHigh, (object) this.instrumentRailPressure.Name));
        ++num;
        flag = false;
      }
      if (flag)
      {
        stringBuilder.Length = 0;
        str = Resources.Message_InstrumentsReady;
      }
      else
        str = string.Format(Resources.MessageFormat_0Instrument1NotReady, (object) num, num >= 2 ? (object) "s" : (object) "");
    }
    else
    {
      flag = false;
      str = Resources.Message_Offline2;
    }
    ((Control) this.labelFmuStickTestInstrumentsStatus).Text = str;
    this.checkmarkInstrumentsStatus.Checked = flag;
    this.tooltipFmuStickTestInstrumentsStatus.SetToolTip((Control) this.labelFmuStickTestInstrumentsStatus, stringBuilder.ToString());
    this.FmuStickTestUpdateButtons();
  }

  private void OnIgnitionUpdateEvent(object sender, ResultEventArgs e)
  {
    this.FmuStickTestUpdateEngineStatus();
  }

  private void OnVehicleCheckUpdateEvent(object sender, ResultEventArgs e)
  {
    this.FmuStickTestUpdateVehicleCheckStatus();
  }

  private void OnInstrumentUpdateEvent(object sender, ResultEventArgs e)
  {
    this.FmuStickTestUpdateInstrumentsStatus();
  }

  private void FmuStickTestUpdateFaultConditions()
  {
    if (this.Online)
    {
      string empty = string.Empty;
      int num = this.FmuStickTestFaultsActive(ref empty);
      this.checkmarkFmuStickTestFaultCodesStatus.Checked = num == 0;
      if (num > 0)
      {
        ((Control) this.labelFmuStickTestFaultCodesStatus).Text = string.Format(Resources.MessageFormat_0FaultCode1, (object) num, num >= 2 ? (object) "s" : (object) "");
        this.tooltipFmuStickTestFaultCodesStatus.SetToolTip((Control) this.labelFmuStickTestFaultCodesStatus, empty);
      }
      else
        ((Control) this.labelFmuStickTestFaultCodesStatus).Text = Resources.Message_NoActiveFaults;
    }
    else
    {
      ((Control) this.labelFmuStickTestFaultCodesStatus).Text = Resources.Message_Offline3;
      this.checkmarkFmuStickTestFaultCodesStatus.Checked = false;
    }
    this.FmuStickTestUpdateButtons();
  }

  private void FmuStickTestUpdateButtons()
  {
    bool flag1 = this.fmuTestRunning;
    bool flag2;
    if (!this.Online)
    {
      flag2 = false;
      flag1 = false;
    }
    else
      flag2 = this.checkmarkInstrumentsStatus.Checked && this.checkmarkFmuStickTestIgnitionAndEngineStatus.Checked && this.checkmarkFmuStickTestVehicleCheckStatus.Checked && this.checkmarkFmuStickTestFaultCodesStatus.Checked;
    this.buttonFmuStickTestStart.Enabled = flag2;
    this.buttonFmuStickTestStop.Enabled = flag1;
  }

  private void FmuStickTestDisplayMessage(string text)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
    this.AddStatusMessage(text);
  }

  private void FmuStickTestClearMessages()
  {
    ((Control) this.scalingLabelFmuStickTestResult).Text = string.Empty;
    ((Control) this.scalingLabelFmuStickTestStatus).Text = string.Empty;
    ((Control) this.scalingLabelFmuStickTestResultValue).Text = string.Empty;
    this.scalingLabelFmuStickTestStatus.RepresentedState = (ValueState) 0;
    this.scalingLabelFmuStickTestResult.RepresentedState = (ValueState) 0;
    this.scalingLabelFmuStickTestResultValue.RepresentedState = (ValueState) 0;
  }

  private void buttonResetFMUAdaptationClick(object sender, EventArgs e)
  {
    Service service = this.channel.Services["RT_SR014_SET_EOL_Default_Values_Start"];
    if (!(service != (Service) null))
      return;
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
    if (service.InputValues.Count > 0)
      service.InputValues[0].Value = (object) service.InputValues[0].Choices.GetItemFromRawValue((object) 25);
    CustomPanel.ExecuteService(service);
  }

  private void OnReadFMUAdaptationClick(object sender, EventArgs e) => this.ReadStoredData();

  private static double ObjectToDouble(object value, string units)
  {
    double num = double.NaN;
    if (value != null)
    {
      Choice choice = value as Choice;
      if (choice != (object) null)
      {
        num = Convert.ToDouble(choice.RawValue);
      }
      else
      {
        try
        {
          num = Convert.ToDouble(value);
          Conversion conversion = Converter.GlobalInstance.GetConversion(units);
          if (conversion != null)
            num = conversion.Convert(num);
        }
        catch (InvalidCastException ex)
        {
          num = double.NaN;
        }
        catch (FormatException ex)
        {
          num = double.NaN;
        }
      }
    }
    return num;
  }

  private void CheckBoxCheckStateChanged(object sender, EventArgs e)
  {
    ((Collection<Qualifier>) this.chartInstrument1.Instruments).Clear();
    if (this.checkBox1.Checked)
      ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.ambientQualifiers);
    if (this.checkBox2.Checked)
      ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.fmuCurrentQualifiers);
    if (!this.checkBox3.Checked)
      return;
    ((NotifyCollection<Qualifier>) this.chartInstrument1.Instruments).AddRange((IEnumerable) this.fmuStickTestQualifiers);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    this.SetChannel((Channel) null);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetChannel(this.GetChannel("MCM21T"));
    this.warningManager.Reset();
  }

  private static Parameter DisconnectParameter(
    ref Parameter parameter,
    ParameterUpdateEventHandler updateEventHandler)
  {
    if (parameter != null)
      parameter.ParameterUpdateEvent -= updateEventHandler;
    return (Parameter) null;
  }

  private static Parameter ConnectParameter(
    ParameterCollection parameters,
    string parameterQualifier,
    ParameterUpdateEventHandler updateEventHandler)
  {
    Parameter parameter = parameters[parameterQualifier];
    if (parameter != null)
      parameter.ParameterUpdateEvent += updateEventHandler;
    return parameter;
  }

  private static Instrument DisconnectInstrument(
    Instrument instrument,
    InstrumentUpdateEventHandler updateEventHandler)
  {
    if (instrument != (Instrument) null)
      instrument.InstrumentUpdateEvent -= updateEventHandler;
    return (Instrument) null;
  }

  private static Instrument ConnectInstrument(
    InstrumentCollection instruments,
    string qualifier,
    InstrumentUpdateEventHandler updateEventHandler)
  {
    Instrument instrument = instruments[qualifier];
    if (instrument != (Instrument) null)
      instrument.InstrumentUpdateEvent += updateEventHandler;
    return instrument;
  }

  private void SetChannel(Channel channel)
  {
    if (this.channel == channel)
      return;
    if (this.channel != null)
    {
      this.instrumentIgnitionState = UserPanel.DisconnectInstrument(this.instrumentIgnitionState, new InstrumentUpdateEventHandler(this.OnIgnitionUpdateEvent));
      this.instrumentEngineState = UserPanel.DisconnectInstrument(this.instrumentEngineState, new InstrumentUpdateEventHandler(this.OnIgnitionUpdateEvent));
      this.instrumentStarterSignalState = UserPanel.DisconnectInstrument(this.instrumentStarterSignalState, new InstrumentUpdateEventHandler(this.OnIgnitionUpdateEvent));
      this.instrumentEngineSpeed = UserPanel.DisconnectInstrument(this.instrumentEngineSpeed, new InstrumentUpdateEventHandler(this.OnIgnitionUpdateEvent));
      this.instrumentVehicleStatusCheck = UserPanel.DisconnectInstrument(this.instrumentVehicleStatusCheck, new InstrumentUpdateEventHandler(this.OnVehicleCheckUpdateEvent));
      this.instrumentCoolantTemperature = UserPanel.DisconnectInstrument(this.instrumentCoolantTemperature, new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent));
      this.instrumentFuelTemperature = UserPanel.DisconnectInstrument(this.instrumentFuelTemperature, new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent));
      this.instrumentRailPressure = UserPanel.DisconnectInstrument(this.instrumentRailPressure, new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent));
      this.instrumentValveSensorCurrent = UserPanel.DisconnectInstrument(this.instrumentValveSensorCurrent, new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent));
      this.instrumentFmuStickTestStatus = UserPanel.DisconnectInstrument(this.instrumentFmuStickTestStatus, new InstrumentUpdateEventHandler(this.OnFmuStickTestStatusUpdateEvent));
      this.instrumentFmuStickTestResult = UserPanel.DisconnectInstrument(this.instrumentFmuStickTestResult, new InstrumentUpdateEventHandler(this.OnFmuStickTestResultUpdateEvent));
      this.channel.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.FaultCodesFaultCodesUpdateEvent);
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    }
    this.channel = channel;
    if (this.channel != null)
    {
      this.instrumentIgnitionState = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_DS003_CPC2_CAN_Ignition_Status", new InstrumentUpdateEventHandler(this.OnIgnitionUpdateEvent));
      this.instrumentStarterSignalState = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_DS003_MCM_wired_Starter_Signal_Status", new InstrumentUpdateEventHandler(this.OnIgnitionUpdateEvent));
      this.instrumentEngineState = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_AS023_Engine_State", new InstrumentUpdateEventHandler(this.OnIgnitionUpdateEvent));
      this.instrumentEngineSpeed = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_AS010_Engine_Speed", new InstrumentUpdateEventHandler(this.OnIgnitionUpdateEvent));
      this.instrumentVehicleStatusCheck = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_DS019_Vehicle_Check_Status", new InstrumentUpdateEventHandler(this.OnVehicleCheckUpdateEvent));
      this.instrumentCoolantTemperature = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_AS013_Coolant_Temperature", new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent));
      this.instrumentFuelTemperature = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_AS014_Fuel_Temperature", new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent));
      this.instrumentRailPressure = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_AS043_Rail_Pressure", new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent));
      this.instrumentValveSensorCurrent = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_AS100_Quantity_Control_Valve_Current", new InstrumentUpdateEventHandler(this.OnInstrumentUpdateEvent));
      this.instrumentFmuStickTestStatus = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_AS122_Fuel_Metering_Unit_Stick_Diagnosis_State", new InstrumentUpdateEventHandler(this.OnFmuStickTestStatusUpdateEvent));
      this.instrumentFmuStickTestResult = UserPanel.ConnectInstrument(this.channel.Instruments, "DT_AS123_Fuel_Metering_Unit_Diagnosis_Error_State", new InstrumentUpdateEventHandler(this.OnFmuStickTestResultUpdateEvent));
      this.channel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.FaultCodesFaultCodesUpdateEvent);
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.FmuStickTestClearMessages();
      this.ReadStoredData();
      this.UpdateUserInterface();
    }
  }

  private void OnFmuStickTestStatusUpdateEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnFmuStickTestStatusUpdateEvent);
    if (!e.Succeeded)
      return;
    string str = string.Empty;
    object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(this.instrumentFmuStickTestStatus);
    if (instrumentCurrentValue != null && this.instrumentFmuStickTestStatus.Choices != null && this.instrumentFmuStickTestStatus.Choices.Count >= 4)
    {
      str = Resources.Message_Test;
      if (instrumentCurrentValue == (object) this.instrumentFmuStickTestStatus.Choices.GetItemFromRawValue((object) 1))
      {
        str += Resources.Message_Started;
        this.scalingLabelFmuStickTestStatus.RepresentedState = (ValueState) 0;
      }
      else if (instrumentCurrentValue == (object) this.instrumentFmuStickTestStatus.Choices.GetItemFromRawValue((object) 2))
      {
        str += Resources.Message_Running;
        this.scalingLabelFmuStickTestStatus.RepresentedState = (ValueState) 0;
      }
      else if (instrumentCurrentValue == (object) this.instrumentFmuStickTestStatus.Choices.GetItemFromRawValue((object) 4))
      {
        str += Resources.Message_Complete;
        this.scalingLabelFmuStickTestStatus.RepresentedState = (ValueState) 1;
      }
      else if (instrumentCurrentValue == (object) this.instrumentFmuStickTestStatus.Choices.GetItemFromRawValue((object) 8))
      {
        str += Resources.Message_Stopped;
        this.scalingLabelFmuStickTestStatus.RepresentedState = (ValueState) 0;
      }
    }
    ((Control) this.scalingLabelFmuStickTestStatus).Text = str;
  }

  private void OnFmuStickTestResultUpdateEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnFmuStickTestResultUpdateEvent);
    if (!e.Succeeded)
      return;
    string empty = string.Empty;
    object instrumentCurrentValue = UserPanel.GetInstrumentCurrentValue(this.instrumentFmuStickTestResult);
    if (instrumentCurrentValue != null && this.instrumentFmuStickTestResult.Choices != null && this.instrumentFmuStickTestResult.Choices.Count >= 3)
    {
      if (instrumentCurrentValue == (object) this.instrumentFmuStickTestResult.Choices.GetItemFromRawValue((object) 0))
      {
        empty += Resources.Message_Passed;
        this.scalingLabelFmuStickTestResult.RepresentedState = (ValueState) 1;
      }
      else if (instrumentCurrentValue == (object) this.instrumentFmuStickTestResult.Choices.GetItemFromRawValue((object) 1))
      {
        empty += Resources.Message_Failed;
        this.scalingLabelFmuStickTestResult.RepresentedState = (ValueState) 3;
      }
      else if (instrumentCurrentValue == (object) this.instrumentFmuStickTestResult.Choices.GetItemFromRawValue((object) (int) byte.MaxValue))
      {
        empty += Resources.Message_Unknown1;
        this.scalingLabelFmuStickTestResult.RepresentedState = (ValueState) 2;
      }
    }
    ((Control) this.scalingLabelFmuStickTestResult).Text = empty;
  }

  private void FaultCodesFaultCodesUpdateEvent(object sender, ResultEventArgs e)
  {
    if (this.tabControl1.SelectedIndex != 0)
      return;
    this.FmuStickTestUpdateFaultConditions();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void UpdateUserInterface()
  {
    if (this.tabControl1.SelectedIndex == 0)
      this.UpdateFmuStickTestInterface();
    else
      this.UpdateFmuAdaptationInterface();
  }

  private void UpdateFmuStickTestInterface()
  {
    this.FmuStickTestUpdateEngineStatus();
    this.FmuStickTestUpdateFaultConditions();
    this.FmuStickTestUpdateInstrumentsStatus();
  }

  private void UpdateFmuAdaptationInterface()
  {
    this.buttonReadFMUAdaptation.Enabled = this.Online;
    this.buttonResetFMUAdaptation.Enabled = this.Online;
  }

  private void tabControl1SelectedIndexChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private bool Online
  {
    get => this.channel != null && this.channel.CommunicationsState == CommunicationsState.Online;
  }

  private void ReadStoredData()
  {
    this.ReadEcuInfo(this.channel.EcuInfos["DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Positive"]);
    this.ReadEcuInfo(this.channel.EcuInfos["DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Negative"]);
  }

  private void ReadParameter(Parameter parameter)
  {
    if (parameter == null || !parameter.Channel.Online)
      return;
    string groupQualifier = parameter.GroupQualifier;
    parameter.Channel.Parameters.ReadGroup(groupQualifier, false, false);
  }

  private void ReadEcuInfo(EcuInfo ecuInfo)
  {
    if (ecuInfo == null || !ecuInfo.Channel.Online)
      return;
    ecuInfo.Channel.EcuInfos[ecuInfo.Qualifier].Read(false);
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    if (this.channel != null)
    {
      Service service = sender as Service;
      if (service != (Service) null)
        service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
      else
        this.channel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    }
    this.ReadStoredData();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.checkBox1 = new CheckBox();
    this.checkBox2 = new CheckBox();
    this.checkBox3 = new CheckBox();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.scalingLabel2 = new ScalingLabel();
    this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative = new DigitalReadoutInstrument();
    this.buttonReadFMUAdaptation = new Button();
    this.buttonResetFMUAdaptation = new Button();
    this.tableLayoutPanelFmuStickTestResults = new TableLayoutPanel();
    this.labelFmuStickTestResultValueTitle = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelFmuStickTestResultValue = new ScalingLabel();
    this.labelFmuStickTestStatusTitle = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelFmuStickTestResult = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelFmuStickTestStatus = new ScalingLabel();
    this.scalingLabelFmuStickTestResult = new ScalingLabel();
    this.tableLayoutPanelFmuStickTest = new TableLayoutPanel();
    this.seekTimeListView = new SeekTimeListView();
    this.checkmarkFmuStickTestFaultCodesStatus = new Checkmark();
    this.labelFmuStickTestFaultCodesStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelFmuStickTestInstrumentsStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.checkmarkInstrumentsStatus = new Checkmark();
    this.checkmarkFmuStickTestIgnitionAndEngineStatus = new Checkmark();
    this.digitalReadoutInstrumentRailPressure = new DigitalReadoutInstrument();
    this.checkmarkFmuStickTestVehicleCheckStatus = new Checkmark();
    this.labelFmuStickTestVehicleCheckStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonFmuStickTestStart = new Button();
    this.buttonFmuStickTestStop = new Button();
    this.labelFmuStickTestIgnitionAndEngineStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.tableLayoutPanelHeader = new TableLayoutPanel();
    this.digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEngineLoad = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCoolantTemperature = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFuelTemperature = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentFuelMass = new DigitalReadoutInstrument();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentQuantity_Control_Valve_Current = new DigitalReadoutInstrument();
    this.tabControl1 = new TabControl();
    this.tabPageFmuStickTest = new TabPage();
    this.tabPageFmuAdaption = new TabPage();
    this.chartInstrument1 = new ChartInstrument();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanelFmuStickTestResults).SuspendLayout();
    ((Control) this.tableLayoutPanelFmuStickTest).SuspendLayout();
    ((Control) this.tableLayoutPanelHeader).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    this.tabControl1.SuspendLayout();
    this.tabPageFmuStickTest.SuspendLayout();
    this.tabPageFmuAdaption.SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.checkBox1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.checkBox2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.checkBox3, 2, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.checkBox1, "checkBox1");
    this.checkBox1.Name = "checkBox1";
    this.checkBox1.UseCompatibleTextRendering = true;
    this.checkBox1.UseVisualStyleBackColor = true;
    this.checkBox2.Checked = true;
    this.checkBox2.CheckState = CheckState.Checked;
    componentResourceManager.ApplyResources((object) this.checkBox2, "checkBox2");
    this.checkBox2.Name = "checkBox2";
    this.checkBox2.UseCompatibleTextRendering = true;
    this.checkBox2.UseVisualStyleBackColor = true;
    this.checkBox3.Checked = true;
    this.checkBox3.CheckState = CheckState.Checked;
    componentResourceManager.ApplyResources((object) this.checkBox3, "checkBox3");
    this.checkBox3.Name = "checkBox3";
    this.checkBox3.UseCompatibleTextRendering = true;
    this.checkBox3.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.scalingLabel2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonReadFMUAdaptation, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonResetFMUAdaptation, 1, 3);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    this.scalingLabel2.Alignment = StringAlignment.Center;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.scalingLabel2, 2);
    componentResourceManager.ApplyResources((object) this.scalingLabel2, "scalingLabel2");
    this.scalingLabel2.FontGroup = "TestTitle";
    this.scalingLabel2.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel2).Name = "scalingLabel2";
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive, "digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive");
    this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Positive");
    ((Control) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive).Name = "digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Positive).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative, "digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative");
    this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_Quantity_Control_Valve_Adaptation_Negative");
    ((Control) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative).Name = "digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantity_Control_Valve_Adaptation_Negative).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonReadFMUAdaptation, "buttonReadFMUAdaptation");
    this.buttonReadFMUAdaptation.Name = "buttonReadFMUAdaptation";
    this.buttonReadFMUAdaptation.UseCompatibleTextRendering = true;
    this.buttonReadFMUAdaptation.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonResetFMUAdaptation, "buttonResetFMUAdaptation");
    this.buttonResetFMUAdaptation.Name = "buttonResetFMUAdaptation";
    this.buttonResetFMUAdaptation.UseCompatibleTextRendering = true;
    this.buttonResetFMUAdaptation.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFmuStickTestResults, "tableLayoutPanelFmuStickTestResults");
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).SetColumnSpan((Control) this.tableLayoutPanelFmuStickTestResults, 4);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTestResults).Controls.Add((Control) this.labelFmuStickTestResultValueTitle, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTestResults).Controls.Add((Control) this.scalingLabelFmuStickTestResultValue, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTestResults).Controls.Add((Control) this.labelFmuStickTestStatusTitle, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTestResults).Controls.Add((Control) this.labelFmuStickTestResult, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTestResults).Controls.Add((Control) this.scalingLabelFmuStickTestStatus, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTestResults).Controls.Add((Control) this.scalingLabelFmuStickTestResult, 0, 3);
    ((Control) this.tableLayoutPanelFmuStickTestResults).Name = "tableLayoutPanelFmuStickTestResults";
    this.labelFmuStickTestResultValueTitle.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFmuStickTestResultValueTitle, "labelFmuStickTestResultValueTitle");
    ((Control) this.labelFmuStickTestResultValueTitle).Name = "labelFmuStickTestResultValueTitle";
    this.labelFmuStickTestResultValueTitle.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelFmuStickTestResultValue.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelFmuStickTestResultValue, "scalingLabelFmuStickTestResultValue");
    this.scalingLabelFmuStickTestResultValue.FontGroup = "TestResults";
    this.scalingLabelFmuStickTestResultValue.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelFmuStickTestResultValue).Name = "scalingLabelFmuStickTestResultValue";
    this.labelFmuStickTestStatusTitle.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFmuStickTestStatusTitle, "labelFmuStickTestStatusTitle");
    ((Control) this.labelFmuStickTestStatusTitle).Name = "labelFmuStickTestStatusTitle";
    this.labelFmuStickTestStatusTitle.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelFmuStickTestResult.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFmuStickTestResult, "labelFmuStickTestResult");
    ((Control) this.labelFmuStickTestResult).Name = "labelFmuStickTestResult";
    this.labelFmuStickTestResult.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelFmuStickTestStatus.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelFmuStickTestStatus, "scalingLabelFmuStickTestStatus");
    this.scalingLabelFmuStickTestStatus.FontGroup = "TestResults";
    this.scalingLabelFmuStickTestStatus.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelFmuStickTestStatus).Name = "scalingLabelFmuStickTestStatus";
    this.scalingLabelFmuStickTestResult.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelFmuStickTestResult, "scalingLabelFmuStickTestResult");
    this.scalingLabelFmuStickTestResult.FontGroup = "TestResults";
    this.scalingLabelFmuStickTestResult.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelFmuStickTestResult).Name = "scalingLabelFmuStickTestResult";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelFmuStickTest, "tableLayoutPanelFmuStickTest");
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.seekTimeListView, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.checkmarkFmuStickTestFaultCodesStatus, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.labelFmuStickTestFaultCodesStatus, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.labelFmuStickTestInstrumentsStatus, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.checkmarkInstrumentsStatus, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.checkmarkFmuStickTestIgnitionAndEngineStatus, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.digitalReadoutInstrumentRailPressure, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.checkmarkFmuStickTestVehicleCheckStatus, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.labelFmuStickTestVehicleCheckStatus, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.buttonFmuStickTestStart, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.buttonFmuStickTestStop, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.tableLayoutPanelFmuStickTestResults, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).Controls.Add((Control) this.labelFmuStickTestIgnitionAndEngineStatus, 1, 1);
    ((Control) this.tableLayoutPanelFmuStickTest).Name = "tableLayoutPanelFmuStickTest";
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).SetColumnSpan((Control) this.seekTimeListView, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Fis Fuel Quanity Control Valve";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    componentResourceManager.ApplyResources((object) this.checkmarkFmuStickTestFaultCodesStatus, "checkmarkFmuStickTestFaultCodesStatus");
    ((Control) this.checkmarkFmuStickTestFaultCodesStatus).Name = "checkmarkFmuStickTestFaultCodesStatus";
    this.labelFmuStickTestFaultCodesStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFmuStickTestFaultCodesStatus, "labelFmuStickTestFaultCodesStatus");
    ((Control) this.labelFmuStickTestFaultCodesStatus).Name = "labelFmuStickTestFaultCodesStatus";
    this.labelFmuStickTestFaultCodesStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelFmuStickTestFaultCodesStatus.ShowBorder = false;
    this.labelFmuStickTestFaultCodesStatus.UseSystemColors = true;
    this.labelFmuStickTestInstrumentsStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFmuStickTestInstrumentsStatus, "labelFmuStickTestInstrumentsStatus");
    ((Control) this.labelFmuStickTestInstrumentsStatus).Name = "labelFmuStickTestInstrumentsStatus";
    this.labelFmuStickTestInstrumentsStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelFmuStickTestInstrumentsStatus.ShowBorder = false;
    this.labelFmuStickTestInstrumentsStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.checkmarkInstrumentsStatus, "checkmarkInstrumentsStatus");
    ((Control) this.checkmarkInstrumentsStatus).Name = "checkmarkInstrumentsStatus";
    componentResourceManager.ApplyResources((object) this.checkmarkFmuStickTestIgnitionAndEngineStatus, "checkmarkFmuStickTestIgnitionAndEngineStatus");
    ((Control) this.checkmarkFmuStickTestIgnitionAndEngineStatus).Name = "checkmarkFmuStickTestIgnitionAndEngineStatus";
    ((TableLayoutPanel) this.tableLayoutPanelFmuStickTest).SetColumnSpan((Control) this.digitalReadoutInstrumentRailPressure, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentRailPressure, "digitalReadoutInstrumentRailPressure");
    this.digitalReadoutInstrumentRailPressure.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRailPressure).FreezeValue = false;
    this.digitalReadoutInstrumentRailPressure.Gradient.Initialize((ValueState) 1, 1, "bar");
    this.digitalReadoutInstrumentRailPressure.Gradient.Modify(1, 10.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRailPressure).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS043_Rail_Pressure");
    ((Control) this.digitalReadoutInstrumentRailPressure).Name = "digitalReadoutInstrumentRailPressure";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentRailPressure).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.checkmarkFmuStickTestVehicleCheckStatus, "checkmarkFmuStickTestVehicleCheckStatus");
    ((Control) this.checkmarkFmuStickTestVehicleCheckStatus).Name = "checkmarkFmuStickTestVehicleCheckStatus";
    this.labelFmuStickTestVehicleCheckStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFmuStickTestVehicleCheckStatus, "labelFmuStickTestVehicleCheckStatus");
    ((Control) this.labelFmuStickTestVehicleCheckStatus).Name = "labelFmuStickTestVehicleCheckStatus";
    this.labelFmuStickTestVehicleCheckStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelFmuStickTestVehicleCheckStatus.ShowBorder = false;
    this.labelFmuStickTestVehicleCheckStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.buttonFmuStickTestStart, "buttonFmuStickTestStart");
    this.buttonFmuStickTestStart.Name = "buttonFmuStickTestStart";
    this.buttonFmuStickTestStart.UseCompatibleTextRendering = true;
    this.buttonFmuStickTestStart.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonFmuStickTestStop, "buttonFmuStickTestStop");
    this.buttonFmuStickTestStop.Name = "buttonFmuStickTestStop";
    this.buttonFmuStickTestStop.UseCompatibleTextRendering = true;
    this.buttonFmuStickTestStop.UseVisualStyleBackColor = true;
    this.labelFmuStickTestIgnitionAndEngineStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelFmuStickTestIgnitionAndEngineStatus, "labelFmuStickTestIgnitionAndEngineStatus");
    ((Control) this.labelFmuStickTestIgnitionAndEngineStatus).Name = "labelFmuStickTestIgnitionAndEngineStatus";
    this.labelFmuStickTestIgnitionAndEngineStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelFmuStickTestIgnitionAndEngineStatus.ShowBorder = false;
    this.labelFmuStickTestIgnitionAndEngineStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelHeader, "tableLayoutPanelHeader");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanelHeader, 4);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentEngineSpeed, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentEngineLoad, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentCoolantTemperature, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentFuelTemperature, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentFuelMass, 2, 0);
    ((Control) this.tableLayoutPanelHeader).Name = "tableLayoutPanelHeader";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
    this.digitalReadoutInstrumentEngineSpeed.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState) 0, 1);
    this.digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 0.1, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineSpeed");
    ((Control) this.digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineLoad, "digitalReadoutInstrumentEngineLoad");
    this.digitalReadoutInstrumentEngineLoad.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineLoad).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineLoad).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "engineTorque");
    ((Control) this.digitalReadoutInstrumentEngineLoad).Name = "digitalReadoutInstrumentEngineLoad";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineLoad).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCoolantTemperature, "digitalReadoutInstrumentCoolantTemperature");
    this.digitalReadoutInstrumentCoolantTemperature.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).FreezeValue = false;
    this.digitalReadoutInstrumentCoolantTemperature.Gradient.Initialize((ValueState) 2, 1, "°C");
    this.digitalReadoutInstrumentCoolantTemperature.Gradient.Modify(1, 20.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp");
    ((Control) this.digitalReadoutInstrumentCoolantTemperature).Name = "digitalReadoutInstrumentCoolantTemperature";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCoolantTemperature).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFuelTemperature, "digitalReadoutInstrumentFuelTemperature");
    this.digitalReadoutInstrumentFuelTemperature.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelTemperature).FreezeValue = false;
    this.digitalReadoutInstrumentFuelTemperature.Gradient.Initialize((ValueState) 3, 1, "°C");
    this.digitalReadoutInstrumentFuelTemperature.Gradient.Modify(1, 20.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelTemperature).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp");
    ((Control) this.digitalReadoutInstrumentFuelTemperature).Name = "digitalReadoutInstrumentFuelTemperature";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelTemperature).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFuelMass, "digitalReadoutInstrumentFuelMass");
    this.digitalReadoutInstrumentFuelMass.FontGroup = "StatusBar";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelMass).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelMass).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS087_Actual_Fuel_Mass");
    ((Control) this.digitalReadoutInstrumentFuelMass).Name = "digitalReadoutInstrumentFuelMass";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuelMass).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentQuantity_Control_Valve_Current, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tabControl1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.chartInstrument1, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 2, 4);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current, "digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current");
    this.digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS121_EU_Fuel_Metering_Unit_FMU_desired_current");
    ((Control) this.digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current).Name = "digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentFuel_Metering_Unit_FMU_desired_current).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentQuantity_Control_Valve_Current, "digitalReadoutInstrumentQuantity_Control_Valve_Current");
    this.digitalReadoutInstrumentQuantity_Control_Valve_Current.FontGroup = "Body";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantity_Control_Valve_Current).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantity_Control_Valve_Current).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS100_Quantity_Control_Valve_Current");
    ((Control) this.digitalReadoutInstrumentQuantity_Control_Valve_Current).Name = "digitalReadoutInstrumentQuantity_Control_Valve_Current";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.digitalReadoutInstrumentQuantity_Control_Valve_Current, 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentQuantity_Control_Valve_Current).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tabControl1, "tabControl1");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tabControl1, 2);
    this.tabControl1.Controls.Add((Control) this.tabPageFmuStickTest);
    this.tabControl1.Controls.Add((Control) this.tabPageFmuAdaption);
    this.tabControl1.Name = "tabControl1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tabControl1, 4);
    this.tabControl1.SelectedIndex = 0;
    this.tabPageFmuStickTest.Controls.Add((Control) this.tableLayoutPanelFmuStickTest);
    componentResourceManager.ApplyResources((object) this.tabPageFmuStickTest, "tabPageFmuStickTest");
    this.tabPageFmuStickTest.Name = "tabPageFmuStickTest";
    this.tabPageFmuStickTest.UseVisualStyleBackColor = true;
    this.tabPageFmuAdaption.Controls.Add((Control) this.tableLayoutPanel2);
    componentResourceManager.ApplyResources((object) this.tabPageFmuAdaption, "tabPageFmuAdaption");
    this.tabPageFmuAdaption.Name = "tabPageFmuAdaption";
    this.tabPageFmuAdaption.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.chartInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.chartInstrument1, "chartInstrument1");
    ((Control) this.chartInstrument1).Name = "chartInstrument1";
    this.chartInstrument1.SelectedTime = new DateTime?();
    this.chartInstrument1.ShowButtonPanel = false;
    this.chartInstrument1.ShowEvents = false;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_FISFuelQuantityControlValve");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanelFmuStickTestResults).ResumeLayout(false);
    ((Control) this.tableLayoutPanelFmuStickTest).ResumeLayout(false);
    ((Control) this.tableLayoutPanelFmuStickTest).PerformLayout();
    ((Control) this.tableLayoutPanelHeader).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    this.tabControl1.ResumeLayout(false);
    this.tabPageFmuStickTest.ResumeLayout(false);
    this.tabPageFmuAdaption.ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum EngineState
  {
    Unknown = -1, // 0xFFFFFFFF
    Stopped = 0,
    StarterEngaged = 1,
    Idle = 2,
    Other = 3,
  }

  private enum StarterState
  {
    Unknown = -1, // 0xFFFFFFFF
    Disabled = 0,
    Enabled = 1,
  }

  private enum BooleanAndUnknown
  {
    False,
    True,
    Unknown,
  }
}
