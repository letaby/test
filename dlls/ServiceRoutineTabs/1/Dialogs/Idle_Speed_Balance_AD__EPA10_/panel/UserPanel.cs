// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD__EPA10_.panel.UserPanel
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD__EPA10_.panel;

public class UserPanel : CustomPanel
{
  private const string StartServiceName = "RT_SR066_Idle_Speed_Balance_Test_Start";
  private const string EngineStateInstrumentQualifier = "DT_ASL004_Engine_State";
  private const int MaxCylinders = 6;
  private const double minCylinderTolerance = -99.5;
  private const double maxCylinderTolerance = 99.5;
  private static readonly List<SetupInformation> Setups = new List<SetupInformation>((IEnumerable<SetupInformation>) new SetupInformation[3]
  {
    new SetupInformation("DD15", "DD15"),
    new SetupInformation("DD16", "DD16"),
    new SetupInformation("DD13", "DD13")
  });
  private string TestSuccessfulMessage = Resources.Message_TheTestCompletedSuccessfully;
  private string ErrorMessage = Resources.Message_ErrorsOccurredDuringTheTest;
  private string ServiceNotSupportedMessage = Resources.MessageFormat_TheConnectedMCM2DoesNotSupportTheServiceRoutine0;
  private string EcuNotConnectedMessage = Resources.Message_MCM2IsNotConnected;
  private string EcuNotMbeMessage = Resources.Message_MCM2IsConnectedButEngineTypeIsNotSupported;
  private string EcuReadyMessage = Resources.Message_MCM2IsConnectedAndEngineTypeIsSupported;
  private string EcuBusyMessage = Resources.Message_MCM2IsConnectedButIsBusy;
  private string EngineAtIdleMessage = Resources.Message_EngineIsAtIdle;
  private string EngineStoppedMessage = Resources.Message_EngineIsStoppedStartTheEngineToProceed;
  private string EngineStateNotIdleMessage = Resources.Message_TheEngineIsNotAtIdle;
  private string EngineStateNotDetectedMessage = Resources.Message_CannotDetectIfEngineIsStarted;
  private string VehicleStatusCheckOkMessage = Resources.Message_VehicleStatusIsOK;
  private string VehicleStatusCheckNotOkMessage = Resources.Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON;
  private readonly Qualifier qualifier1 = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_1");
  private readonly Qualifier qualifier2 = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_2");
  private readonly Qualifier qualifier3 = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_3");
  private readonly Qualifier qualifier4 = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_4");
  private readonly Qualifier qualifier5 = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_5");
  private readonly Qualifier qualifier6 = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_6");
  private Dictionary<DataItem, ScalingLabel> labelMap = new Dictionary<DataItem, ScalingLabel>();
  private int[] firingOrder = new int[6]{ 3, 5, 4, 1, 0, 2 };
  private WarningManager warningManager;
  private bool adrReturnValue = false;
  private string adrReturnText = "test was not ran";
  private Channel mcm;
  private Instrument engineState;
  private bool success = true;
  private int cylinder;
  private SetupInformation connectedEcuType;
  private UserPanel.IdleStates idlingState = UserPanel.IdleStates.NotDetected;
  private bool testRunning;
  private bool testStarted;
  private TableLayoutPanel tableLayoutPanel1;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label engineIdlingStatus;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label mcmConnectionStatus;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label temperatureStatus;
  private TableLayoutPanel tableLayoutPanel3;
  private DigitalReadoutInstrument vehicleCheckInstrument;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label vehicleStatus;
  private BarInstrument fuelTemperatureInstrument;
  private BarInstrument coolantTemperatureInstrument;
  private Button buttonExecute;
  private Button buttonClose;
  private Checkmark connectionCheck;
  private Checkmark testReadyCheck;
  private TableLayoutPanel tableLayoutPanel2;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label cylinderLabel6;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label cylinderLabel3;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label cylinderLabel4;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label cylinderLabel5;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label cylinderLabel2;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label cylinderLabel1;
  private ScalingLabel scalingLabel1;
  private ScalingLabel scalingLabel2;
  private ScalingLabel scalingLabel3;
  private ScalingLabel scalingLabel4;
  private ScalingLabel scalingLabel5;
  private ScalingLabel scalingLabel6;
  private Checkmark temperatureCheck;
  private Checkmark vehicleStatusCheck;
  private Checkmark idlingCheck;
  private System.Windows.Forms.Label testStatusLabel;
  private SeekTimeListView seekTimeListView;

  public UserPanel()
  {
    this.InitializeComponent();
    this.warningManager = new WarningManager(string.Empty, "idle speed balance test", this.seekTimeListView.RequiredUserLabelPrefix);
    this.scalingLabel1.AddScalingString("-100");
    this.scalingLabel2.AddScalingString("-100");
    this.scalingLabel3.AddScalingString("-100");
    this.scalingLabel4.AddScalingString("-100");
    this.scalingLabel5.AddScalingString("-100");
    this.scalingLabel6.AddScalingString("-100");
    this.buttonExecute.Click += new EventHandler(this.OnButtonClick);
    this.fuelTemperatureInstrument.RepresentedStateChanged += new EventHandler(this.OnTemperatureStateChanged);
    this.coolantTemperatureInstrument.RepresentedStateChanged += new EventHandler(this.OnTemperatureStateChanged);
    this.vehicleCheckInstrument.RepresentedStateChanged += new EventHandler(this.OnVehicleCheckStateChanged);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.adrReturnValue,
      (object) this.adrReturnText
    };
  }

  public void OnButtonClick(object sender, EventArgs e) => this.StartTest();

  private bool EcuReady
  {
    get => this.EcuCorrectType && this.mcm.CommunicationsState == CommunicationsState.Online;
  }

  private bool EcuCorrectType => this.IsConnected && this.connectedEcuType != null;

  private bool IsConnected => this.mcm != null && this.mcm.Online;

  private bool ValidTestCondition
  {
    get
    {
      return this.EcuReady && this.IsEngineIdling && this.TemperaturesAreInRange && this.VehicleCheckStatusOk;
    }
  }

  public bool IsEngineIdling
  {
    get => this.idlingState == UserPanel.IdleStates.Idling;
    set
    {
      this.idlingState = value ? UserPanel.IdleStates.Idling : UserPanel.IdleStates.NotIdling;
      this.UpdateTestReadiness();
    }
  }

  private bool TemperaturesAreInRange
  {
    get
    {
      return this.fuelTemperatureInstrument.RepresentedState == 1 && this.coolantTemperatureInstrument.RepresentedState == 1;
    }
  }

  private bool VehicleCheckStatusOk => this.vehicleCheckInstrument.RepresentedState == 1;

  protected virtual void Dispose(bool disposing)
  {
    this.SetMCM((Channel) null);
    this.DisconnectDataItems();
    base.Dispose(disposing);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM02T"));
    this.ConnectDataItems();
    this.UpdateTestReadiness();
  }

  private void OnDataItemUpdate(object sender, ResultEventArgs e)
  {
    if (!(sender is DataItem dataItem) || !this.labelMap.ContainsKey(dataItem))
      return;
    ScalingLabel label = this.labelMap[dataItem];
    if (label != null)
      this.UpdateInstrumentValue(label, dataItem);
  }

  private void SetMCM(Channel mcm)
  {
    if (this.mcm == mcm)
      return;
    this.warningManager.Reset();
    this.testRunning = false;
    if (this.mcm != null)
    {
      this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      if (this.engineState != (Instrument) null)
        this.engineState.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnEngineStateUpdate);
      this.idlingState = UserPanel.IdleStates.NotDetected;
    }
    this.mcm = mcm;
    if (this.mcm != null)
    {
      this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.engineState = this.mcm.Instruments["DT_ASL004_Engine_State"];
      if (this.engineState != (Instrument) null)
      {
        this.engineState.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnEngineStateUpdate);
        this.UpdateEngineState();
      }
    }
    this.UpdateConnectedEcuType();
  }

  private void DisconnectDataItems()
  {
    foreach (DataItem key in this.labelMap.Keys)
      key.UpdateEvent -= new EventHandler<ResultEventArgs>(this.OnDataItemUpdate);
    this.labelMap.Clear();
  }

  private void ConnectDataItems()
  {
    this.DisconnectDataItems();
    DataItem key1 = DataItem.Create(this.qualifier1, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
    DataItem key2 = DataItem.Create(this.qualifier2, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
    DataItem key3 = DataItem.Create(this.qualifier3, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
    DataItem key4 = DataItem.Create(this.qualifier4, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
    DataItem key5 = DataItem.Create(this.qualifier5, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
    DataItem key6 = DataItem.Create(this.qualifier6, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels);
    if (key1 != null && key2 != null && key3 != null && key4 != null && key5 != null && key6 != null)
    {
      this.labelMap.Add(key1, this.scalingLabel1);
      this.labelMap.Add(key2, this.scalingLabel2);
      this.labelMap.Add(key3, this.scalingLabel3);
      this.labelMap.Add(key4, this.scalingLabel4);
      this.labelMap.Add(key5, this.scalingLabel5);
      this.labelMap.Add(key6, this.scalingLabel6);
      foreach (DataItem key7 in this.labelMap.Keys)
      {
        key7.UpdateEvent += new EventHandler<ResultEventArgs>(this.OnDataItemUpdate);
        this.UpdateInstrumentValue(this.labelMap[key7], key7);
      }
    }
    else
    {
      ((Control) this.scalingLabel1).Text = string.Empty;
      ((Control) this.scalingLabel2).Text = string.Empty;
      ((Control) this.scalingLabel3).Text = string.Empty;
      ((Control) this.scalingLabel4).Text = string.Empty;
      ((Control) this.scalingLabel5).Text = string.Empty;
      ((Control) this.scalingLabel6).Text = string.Empty;
    }
  }

  private void UpdateTestReadiness()
  {
    this.UpdateEcuReadyCheck();
    this.UpdateEngineIdleCheck();
    this.UpdateTemperatureCheck();
    this.UpdateVehicleCheckStatus();
    this.buttonExecute.Enabled = !this.testRunning && this.ValidTestCondition;
    this.testReadyCheck.Checked = this.ValidTestCondition;
  }

  private string GetOkMinimumString(Gradient gradient)
  {
    foreach (GradientCell cell in (IEnumerable<GradientCell>) gradient.Cells)
    {
      if (((GradientCell) ref cell).State == 1)
      {
        Conversion conversion = Converter.GlobalInstance.GetConversion(gradient.Units);
        return conversion == null ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}{1}", (object) Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) ((GradientCell) ref cell).LowerBoundary, gradient.Units, -1), (object) gradient.Units) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}{1}", (object) Converter.ConvertToString((IFormatProvider) CultureInfo.CurrentCulture, (object) ((GradientCell) ref cell).LowerBoundary, conversion, -1), (object) conversion.OutputUnit);
      }
    }
    return Resources.Message_Unknown;
  }

  private void UpdateTemperatureCheck()
  {
    this.temperatureCheck.CheckState = this.TemperaturesAreInRange ? CheckState.Checked : CheckState.Unchecked;
    if (this.TemperaturesAreInRange)
    {
      ((Control) this.temperatureStatus).Text = Resources.Message_FuelAndCoolantTemperaturesAreInRange;
    }
    else
    {
      string empty = string.Empty;
      if (this.fuelTemperatureInstrument.RepresentedState != 1)
        empty += string.Format(Resources.MessageFormat_FuelTemperatureMustBeAtLeast0, (object) this.GetOkMinimumString(((AxisSingleInstrumentBase) this.fuelTemperatureInstrument).Gradient));
      if (this.coolantTemperatureInstrument.RepresentedState != 1)
        empty += string.Format(Resources.MessageFormat_CoolantTemperatureMustBeAtLeast0, (object) this.GetOkMinimumString(((AxisSingleInstrumentBase) this.coolantTemperatureInstrument).Gradient));
      ((Control) this.temperatureStatus).Text = empty;
    }
  }

  private void UpdateEcuReadyCheck()
  {
    this.connectionCheck.CheckState = this.IsConnected ? CheckState.Checked : CheckState.Unchecked;
    if (this.IsConnected)
    {
      if (this.EcuCorrectType)
      {
        if (this.EcuReady)
          ((Control) this.mcmConnectionStatus).Text = this.EcuReadyMessage;
        else
          ((Control) this.mcmConnectionStatus).Text = this.EcuBusyMessage;
      }
      else
        ((Control) this.mcmConnectionStatus).Text = this.EcuNotMbeMessage;
    }
    else
      ((Control) this.mcmConnectionStatus).Text = this.EcuNotConnectedMessage;
  }

  private void UpdateEngineIdleCheck()
  {
    this.idlingCheck.CheckState = this.IsEngineIdling ? CheckState.Checked : CheckState.Unchecked;
    switch (this.idlingState)
    {
      case UserPanel.IdleStates.Idling:
        ((Control) this.engineIdlingStatus).Text = this.EngineAtIdleMessage;
        break;
      case UserPanel.IdleStates.NotIdling:
        ((Control) this.engineIdlingStatus).Text = this.EngineStateNotIdleMessage;
        break;
      case UserPanel.IdleStates.Stopped:
        ((Control) this.engineIdlingStatus).Text = this.EngineStoppedMessage;
        break;
      case UserPanel.IdleStates.NotDetected:
        ((Control) this.engineIdlingStatus).Text = this.EngineStateNotDetectedMessage;
        break;
      default:
        throw new ArgumentOutOfRangeException("Unknown idle state.");
    }
  }

  private void UpdateVehicleCheckStatus()
  {
    this.vehicleStatusCheck.CheckState = this.VehicleCheckStatusOk ? CheckState.Checked : CheckState.Unchecked;
    ((Control) this.vehicleStatus).Text = this.VehicleCheckStatusOk ? this.VehicleStatusCheckOkMessage : this.VehicleStatusCheckNotOkMessage;
  }

  private void UpdateConnectedEcuType()
  {
    if (this.mcm == null)
      return;
    IEnumerable<EquipmentType> source = EquipmentType.ConnectedEquipmentTypes("Engine");
    if (CollectionExtensions.Exactly<EquipmentType>(source, 1))
    {
      EquipmentType equipmentType = source.First<EquipmentType>();
      string name = ((EquipmentType) ref equipmentType).Name;
      foreach (SetupInformation setup in UserPanel.Setups)
      {
        if (setup.Name == name)
        {
          if (setup != this.connectedEcuType)
          {
            this.connectedEcuType = setup;
            this.UpdateTestReadiness();
            break;
          }
          break;
        }
      }
    }
  }

  private void StartTest()
  {
    if (!this.warningManager.RequestContinue())
      return;
    this.testRunning = true;
    if (!this.ValidTestCondition)
    {
      this.UpdateTestReadiness();
    }
    else
    {
      this.AppendDisplayMessage(Resources.Message_StartingTest);
      this.success = this.testStarted = true;
      this.cylinder = 0;
      this.testStatusLabel.Text = string.Empty;
      this.Advance();
    }
  }

  private void Advance()
  {
    Service service = this.mcm.Services["RT_SR066_Idle_Speed_Balance_Test_Start"];
    if (service != (Service) null)
    {
      if (++this.cylinder <= 6 && this.testRunning)
      {
        service.InputValues[0].Value = (object) service.InputValues[0].Choices[this.cylinder - 1];
        if (service.InputValues.Count > 1)
          service.InputValues[1].Value = (object) service.InputValues[1].Choices.GetItemFromRawValue((object) 1);
        service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnServiceComplete);
        service.Execute(false);
      }
      else
      {
        this.testRunning = false;
        this.AppendDisplayMessage(this.success ? this.TestSuccessfulMessage : this.ErrorMessage);
        this.UpdateTestReadiness();
      }
    }
    else
      this.AppendDisplayMessage(string.Format(this.ServiceNotSupportedMessage, (object) "RT_SR066_Idle_Speed_Balance_Test_Start"));
  }

  private void AppendDisplayMessage(string txt)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, txt);
    this.AddStatusMessage(txt);
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateConnectedEcuType();
    this.UpdateTestReadiness();
  }

  private void OnEngineStateUpdate(object sender, ResultEventArgs e) => this.UpdateEngineState();

  private void OnTemperatureStateChanged(object sender, EventArgs e) => this.UpdateTestReadiness();

  private void OnVehicleCheckStateChanged(object sender, EventArgs e) => this.UpdateTestReadiness();

  private void UpdateEngineState()
  {
    this.idlingState = UserPanel.IdleStates.NotIdling;
    if (this.engineState != (Instrument) null && this.engineState.InstrumentValues.Current != null && this.engineState.Choices != null)
    {
      object obj = this.engineState.InstrumentValues.Current.Value;
      if (obj == (object) this.engineState.Choices.GetItemFromRawValue((object) 0))
        this.idlingState = UserPanel.IdleStates.Stopped;
      else if (obj == (object) this.engineState.Choices.GetItemFromRawValue((object) 3))
        this.idlingState = UserPanel.IdleStates.Idling;
      else if (obj == (object) this.engineState.Choices.GetItemFromRawValue((object) -1))
        this.idlingState = UserPanel.IdleStates.NotDetected;
    }
    this.UpdateTestReadiness();
  }

  private void OnServiceComplete(object sender, ResultEventArgs e)
  {
    ((Service) sender).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnServiceComplete);
    if (!e.Succeeded)
    {
      this.success = false;
      if (e.Exception != null && e.Exception.Message != null)
        this.AppendDisplayMessage(e.Exception.Message + Resources.Message_WhileTestingCylinder + (object) this.cylinder);
      else
        this.AppendDisplayMessage(Resources.Message_Cylinder + (object) this.cylinder + Resources.Message_TestWasNotSuccessful);
      this.testRunning = false;
    }
    else
      this.AppendDisplayMessage(Resources.Message_Cylinder1 + (object) this.cylinder + Resources.Message_TestWasSuccessful);
    this.Advance();
  }

  private void UpdateInstrumentValue(ScalingLabel scalingLabel, DataItem dataItem)
  {
    string str = string.Empty;
    if (dataItem != null && dataItem.Value != null)
      str = dataItem.ValueAsString(dataItem.Value);
    ((Control) scalingLabel).Text = str;
    this.HighlightValues();
  }

  private void HighlightValues()
  {
    List<KeyValuePair<DataItem, ScalingLabel>> keyValuePairList = new List<KeyValuePair<DataItem, ScalingLabel>>((IEnumerable<KeyValuePair<DataItem, ScalingLabel>>) this.labelMap);
    List<int> source1 = new List<int>();
    List<Tuple<int, int>> source2 = new List<Tuple<int, int>>();
    string str = string.Empty;
    for (int index1 = 0; index1 < keyValuePairList.Count; ++index1)
    {
      ValueState valueState = (ValueState) 0;
      KeyValuePair<DataItem, ScalingLabel> keyValuePair = keyValuePairList[index1];
      DataItem key1 = keyValuePair.Key;
      keyValuePair = keyValuePairList[index1];
      object obj1 = keyValuePair.Key.Value;
      double num1 = key1.ValueAsDouble(obj1);
      if (num1 > 99.5 || num1 < -99.5)
      {
        int index2 = this.firingOrder[index1];
        keyValuePair = keyValuePairList[index2];
        DataItem key2 = keyValuePair.Key;
        keyValuePair = keyValuePairList[index2];
        object obj2 = keyValuePair.Key.Value;
        double num2 = key2.ValueAsDouble(obj2);
        if (num2 > 99.5 || num2 < -99.5)
        {
          valueState = (ValueState) 2;
          source2.Add(new Tuple<int, int>(index1 + 1, index2 + 1));
        }
        else
        {
          valueState = (ValueState) 3;
          source1.Add(index1 + 1);
        }
      }
      keyValuePair = keyValuePairList[index1];
      keyValuePair.Value.RepresentedState = valueState;
    }
    if (!this.testStarted)
      return;
    if (this.success)
    {
      this.adrReturnValue = source1.Count == 0;
      if (source1.Any<int>())
        str = string.Join(", <br />", source1.Select<int, string>((Func<int, string>) (i => string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_Cylinder0MayBeInFaultRN, (object) i))));
      if (source2.Any<Tuple<int, int>>())
        str += string.Join(", <br />", source2.Select<Tuple<int, int>, string>((Func<Tuple<int, int>, string>) (i => string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MessageFormat_Cylinder0MightBeCompensatingForCylinder1RN, (object) i.Item1, (object) i.Item2))));
      this.adrReturnText = "<br />" + str;
    }
    else
      this.adrReturnText = this.ErrorMessage;
    this.testStatusLabel.Text = Resources.Message_TestCompleteCloseThisWindowToContinueTroubleshooting;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.connectionCheck = new Checkmark();
    this.vehicleCheckInstrument = new DigitalReadoutInstrument();
    this.engineIdlingStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.testReadyCheck = new Checkmark();
    this.mcmConnectionStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.cylinderLabel6 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.cylinderLabel3 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.cylinderLabel4 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.cylinderLabel5 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.cylinderLabel2 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.cylinderLabel1 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabel1 = new ScalingLabel();
    this.scalingLabel2 = new ScalingLabel();
    this.scalingLabel3 = new ScalingLabel();
    this.scalingLabel4 = new ScalingLabel();
    this.scalingLabel5 = new ScalingLabel();
    this.scalingLabel6 = new ScalingLabel();
    this.temperatureCheck = new Checkmark();
    this.temperatureStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.vehicleStatusCheck = new Checkmark();
    this.vehicleStatus = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonExecute = new Button();
    this.idlingCheck = new Checkmark();
    this.seekTimeListView = new SeekTimeListView();
    this.buttonClose = new Button();
    this.fuelTemperatureInstrument = new BarInstrument();
    this.coolantTemperatureInstrument = new BarInstrument();
    this.testStatusLabel = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.connectionCheck, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.vehicleCheckInstrument, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.engineIdlingStatus, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.testReadyCheck, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.mcmConnectionStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.temperatureCheck, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.temperatureStatus, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.vehicleStatusCheck, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.vehicleStatus, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonExecute, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.idlingCheck, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 7, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 4, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.testStatusLabel, 0, 8);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.connectionCheck, "connectionCheck");
    ((Control) this.connectionCheck).Name = "connectionCheck";
    this.vehicleCheckInstrument.FontGroup = "idleSpeedBalancePreconditionsEPA10";
    ((SingleInstrumentBase) this.vehicleCheckInstrument).FreezeValue = false;
    this.vehicleCheckInstrument.Gradient.Initialize((ValueState) 0, 3);
    this.vehicleCheckInstrument.Gradient.Modify(1, 0.0, (ValueState) 2);
    this.vehicleCheckInstrument.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.vehicleCheckInstrument.Gradient.Modify(3, 2.0, (ValueState) 0);
    componentResourceManager.ApplyResources((object) this.vehicleCheckInstrument, "vehicleCheckInstrument");
    ((SingleInstrumentBase) this.vehicleCheckInstrument).Instrument = new Qualifier((QualifierTypes) 1, "MCM02T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.vehicleCheckInstrument).Name = "vehicleCheckInstrument";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.vehicleCheckInstrument, 4);
    ((SingleInstrumentBase) this.vehicleCheckInstrument).UnitAlignment = StringAlignment.Near;
    this.engineIdlingStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.engineIdlingStatus, "engineIdlingStatus");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.engineIdlingStatus, 2);
    ((Control) this.engineIdlingStatus).Name = "engineIdlingStatus";
    this.engineIdlingStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.engineIdlingStatus.ShowBorder = false;
    this.engineIdlingStatus.UseSystemColors = true;
    ((Control) this.testReadyCheck).BackColor = Color.Transparent;
    this.testReadyCheck.CheckedImage = (Image) componentResourceManager.GetObject("testReadyCheck.CheckedImage");
    componentResourceManager.ApplyResources((object) this.testReadyCheck, "testReadyCheck");
    ((Control) this.testReadyCheck).Name = "testReadyCheck";
    this.testReadyCheck.UncheckedImage = (Image) componentResourceManager.GetObject("testReadyCheck.UncheckedImage");
    this.mcmConnectionStatus.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.mcmConnectionStatus, "mcmConnectionStatus");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.mcmConnectionStatus, 2);
    ((Control) this.mcmConnectionStatus).Name = "mcmConnectionStatus";
    this.mcmConnectionStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.mcmConnectionStatus.ShowBorder = false;
    this.mcmConnectionStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 8);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinderLabel6, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinderLabel3, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinderLabel4, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinderLabel5, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinderLabel2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.cylinderLabel1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.scalingLabel1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.scalingLabel2, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.scalingLabel3, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.scalingLabel4, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.scalingLabel5, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.scalingLabel6, 2, 3);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    this.cylinderLabel6.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.cylinderLabel6, "cylinderLabel6");
    ((Control) this.cylinderLabel6).Name = "cylinderLabel6";
    this.cylinderLabel6.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.cylinderLabel3.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.cylinderLabel3, "cylinderLabel3");
    ((Control) this.cylinderLabel3).Name = "cylinderLabel3";
    this.cylinderLabel3.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.cylinderLabel4.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.cylinderLabel4, "cylinderLabel4");
    ((Control) this.cylinderLabel4).Name = "cylinderLabel4";
    this.cylinderLabel4.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.cylinderLabel5.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.cylinderLabel5, "cylinderLabel5");
    ((Control) this.cylinderLabel5).Name = "cylinderLabel5";
    this.cylinderLabel5.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.cylinderLabel2.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.cylinderLabel2, "cylinderLabel2");
    ((Control) this.cylinderLabel2).Name = "cylinderLabel2";
    this.cylinderLabel2.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.cylinderLabel1.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.cylinderLabel1, "cylinderLabel1");
    ((Control) this.cylinderLabel1).Name = "cylinderLabel1";
    this.cylinderLabel1.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabel1.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabel1, "scalingLabel1");
    this.scalingLabel1.FontGroup = (string) null;
    this.scalingLabel1.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel1).Name = "scalingLabel1";
    this.scalingLabel2.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabel2, "scalingLabel2");
    this.scalingLabel2.FontGroup = (string) null;
    this.scalingLabel2.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel2).Name = "scalingLabel2";
    this.scalingLabel3.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabel3, "scalingLabel3");
    this.scalingLabel3.FontGroup = (string) null;
    this.scalingLabel3.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel3).Name = "scalingLabel3";
    this.scalingLabel4.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabel4, "scalingLabel4");
    this.scalingLabel4.FontGroup = (string) null;
    this.scalingLabel4.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel4).Name = "scalingLabel4";
    this.scalingLabel5.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabel5, "scalingLabel5");
    this.scalingLabel5.FontGroup = (string) null;
    this.scalingLabel5.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel5).Name = "scalingLabel5";
    this.scalingLabel6.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabel6, "scalingLabel6");
    this.scalingLabel6.FontGroup = (string) null;
    this.scalingLabel6.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel6).Name = "scalingLabel6";
    componentResourceManager.ApplyResources((object) this.temperatureCheck, "temperatureCheck");
    ((Control) this.temperatureCheck).Name = "temperatureCheck";
    this.temperatureStatus.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.temperatureStatus, 2);
    componentResourceManager.ApplyResources((object) this.temperatureStatus, "temperatureStatus");
    ((Control) this.temperatureStatus).Name = "temperatureStatus";
    this.temperatureStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.temperatureStatus.ShowBorder = false;
    this.temperatureStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.vehicleStatusCheck, "vehicleStatusCheck");
    ((Control) this.vehicleStatusCheck).Name = "vehicleStatusCheck";
    this.vehicleStatus.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.vehicleStatus, 2);
    componentResourceManager.ApplyResources((object) this.vehicleStatus, "vehicleStatus");
    ((Control) this.vehicleStatus).Name = "vehicleStatus";
    this.vehicleStatus.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.vehicleStatus.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.buttonExecute, "buttonExecute");
    this.buttonExecute.Name = "buttonExecute";
    this.buttonExecute.UseCompatibleTextRendering = true;
    this.buttonExecute.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.idlingCheck, "idlingCheck");
    ((Control) this.idlingCheck).Name = "idlingCheck";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView, 2);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Idle Speed Balance";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.seekTimeListView, 2);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 4);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.fuelTemperatureInstrument, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.coolantTemperatureInstrument, 0, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel3, 6);
    this.fuelTemperatureInstrument.BarOrientation = (BarControl.ControlOrientation) 1;
    this.fuelTemperatureInstrument.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.fuelTemperatureInstrument, "fuelTemperatureInstrument");
    this.fuelTemperatureInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.fuelTemperatureInstrument).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.fuelTemperatureInstrument).Gradient.Initialize((ValueState) 2, 1, "°C");
    ((AxisSingleInstrumentBase) this.fuelTemperatureInstrument).Gradient.Modify(1, 10.0, (ValueState) 1);
    ((SingleInstrumentBase) this.fuelTemperatureInstrument).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "fuelTemp");
    ((Control) this.fuelTemperatureInstrument).Name = "fuelTemperatureInstrument";
    ((SingleInstrumentBase) this.fuelTemperatureInstrument).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.fuelTemperatureInstrument).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.fuelTemperatureInstrument).UnitAlignment = StringAlignment.Near;
    this.coolantTemperatureInstrument.BarOrientation = (BarControl.ControlOrientation) 1;
    this.coolantTemperatureInstrument.BarStyle = (BarControl.ControlStyle) 1;
    componentResourceManager.ApplyResources((object) this.coolantTemperatureInstrument, "coolantTemperatureInstrument");
    this.coolantTemperatureInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.coolantTemperatureInstrument).FreezeValue = false;
    ((AxisSingleInstrumentBase) this.coolantTemperatureInstrument).Gradient.Initialize((ValueState) 2, 1, "°C");
    ((AxisSingleInstrumentBase) this.coolantTemperatureInstrument).Gradient.Modify(1, 70.0, (ValueState) 1);
    ((SingleInstrumentBase) this.coolantTemperatureInstrument).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "coolantTemp");
    ((Control) this.coolantTemperatureInstrument).Name = "coolantTemperatureInstrument";
    ((SingleInstrumentBase) this.coolantTemperatureInstrument).TitleOrientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 0;
    ((SingleInstrumentBase) this.coolantTemperatureInstrument).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.coolantTemperatureInstrument).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.testStatusLabel, 7);
    componentResourceManager.ApplyResources((object) this.testStatusLabel, "testStatusLabel");
    this.testStatusLabel.Name = "testStatusLabel";
    this.testStatusLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_IdleSpeedBalance");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum IdleStates
  {
    Idling,
    NotIdling,
    Stopped,
    NotDetected,
  }
}
