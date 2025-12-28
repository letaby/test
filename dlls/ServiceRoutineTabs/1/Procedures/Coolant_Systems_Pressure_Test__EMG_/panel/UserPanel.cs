// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Coolant_Systems_Pressure_Test__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Coolant_Systems_Pressure_Test__EMG_.panel;

public class UserPanel : CustomPanel
{
  private static string ChannelName = "ECPC01T";
  private static string ThreeByTwoWayValuePositionControlStartQualifier = "RT_OTF_3by2_wayValvePositionControl_Start_3by2_Valve_ExtensionCkt_Req_1";
  private static string BatteryCircuitWaterPumpControlQualifier = "RT_OTF_ETHM_BCWaterPumpCtrl_Start";
  private static string PtcControlStartQualifier = "RT_OTF_ETHM_PtcCtrl_Start";
  private static string PtcControlStopQualifier = "RT_OTF_ETHM_PtcCtrl_Stop";
  private static string EDriveDeaerationControlStartQualifier = "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Start_e_drive_circuit_deaeration_start_resp";
  private static string EDriveDeaerationControlStopQualifier = "RT_OTF_ETHM_EDriveCircuitDeaerationCtrl_Stop_e_drive_circuit_deaeration_stop_resp";
  private static string TextStart = "Start";
  private static string TextStop = "Stop";
  private static int OffDataRawValue = 0;
  private static int OnDataRawValue = 1;
  private static int DeaerationDataRawValue = 0;
  private static int BatteryTestDelayTime = 600;
  private static int EDriveTestDelayTime = 600;
  private static int EDriveTestDelayTimeAfter = 30;
  private CoolantTestStep[] batteryCoolantTestSteps = new CoolantTestStep[4]
  {
    new CoolantTestStep(TestTypes.Battery, Resources.Message_SetBattery3x2ValveTo0, CoolantTestActions.SetBattery3X2),
    new CoolantTestStep(TestTypes.Battery, Resources.Message_SetBatteryCircuitCoolantPumpsTo60, CoolantTestActions.SetBatteryCircuitCoolantPumps),
    new CoolantTestStep(TestTypes.Battery, Resources.Message_TurnOnPTCsTo100, CoolantTestActions.TurnOnBatteryPTCs),
    new CoolantTestStep(TestTypes.Battery, Resources.Message_WaitUntilTemperatureIs38DegCOr10MinutesHasPassed, CoolantTestActions.WaitForBatteryTemperature)
  };
  private CoolantTestStep[] eDriveCoolantTestSteps = new CoolantTestStep[3]
  {
    new CoolantTestStep(TestTypes.EDrive, Resources.Message_ExecutingTheDeaerationEDrvieRoutine, CoolantTestActions.EDriveDeaeration),
    new CoolantTestStep(TestTypes.EDrive, Resources.Message_TurningOnTheCabCircuitPTCs, CoolantTestActions.TurnOnCabPTCs),
    new CoolantTestStep(TestTypes.EDrive, Resources.Message_WaitUntilTemperatureIs50DegCOr10MinutesHasPassed, CoolantTestActions.WaitForEDriveTemperature)
  };
  private CoolantTestStep currentStep = (CoolantTestStep) null;
  private int currentStepIndex = -1;
  private TestTypes currentTestType = TestTypes.Battery;
  private bool testStoppedByUser = false;
  private Channel eCpc01tChannel;
  private Service battery3X2Service;
  private Service coolantPumpService;
  private Service ptcOnService;
  private Service ptcOffService;
  private Service eDriveDeaerationService;
  private Service eDriveDeaerationStopService;
  private int testCounter;
  private Timer testTimer = (Timer) null;
  private SelectablePanel selectablePanel1;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrumentHVReady;
  private DigitalReadoutInstrument digitalReadoutInstrumentBlowerSpeed;
  private TableLayoutPanel tableLayoutPanel3;
  private System.Windows.Forms.Label labelStatusEDrive;
  private Checkmark checkmarkStatusEDrive;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private SeekTimeListView seekTimeListViewCoolantLoopTest;
  private System.Windows.Forms.Label label3;
  private System.Windows.Forms.Label label2;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private TableLayoutPanel tableLayoutPanel2;
  private System.Windows.Forms.Label labelStatusBattery;
  private Checkmark checkmarkStatusBattery;
  private DigitalReadoutInstrument digitalReadoutInstrument9;
  private DigitalReadoutInstrument digitalReadoutInstrument8;
  private DigitalReadoutInstrument digitalReadoutInstrumentEDriveCircOutTemp;
  private DigitalReadoutInstrument digitalReadoutInstrumentBatteryCircTemp;
  private Button buttonStartEDrive;
  private Button buttonStartBattery;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private DigitalReadoutInstrument digitalReadoutInstrument2;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private ScalingLabel scalingLabel1;
  private DigitalReadoutInstrument digitalReadoutInstrument4;

  public UserPanel() => this.InitializeComponent();

  private bool IsEM2Vehicle
  {
    get
    {
      return SapiManager.GlobalInstance != null && SapiManager.GlobalInstance.ConnectedEquipment.Any<EquipmentType>((Func<EquipmentType, bool>) (et =>
      {
        ElectronicsFamily family = ((EquipmentType) ref et).Family;
        return ((ElectronicsFamily) ref family).Name.ToUpper().Contains("EMOBILITY");
      })) && SapiManager.GlobalInstance.ConnectedEquipment.Any<EquipmentType>((Func<EquipmentType, bool>) (et =>
      {
        ElectronicsFamily family = ((EquipmentType) ref et).Family;
        return ((ElectronicsFamily) ref family).Name.ToUpper().Contains("EM2");
      }));
    }
  }

  private bool IsReadyBattery
  {
    get
    {
      return this.digitalReadoutInstrumentParkBrake.RepresentedState == 1 && this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && this.digitalReadoutInstrumentHVReady.RepresentedState == 1;
    }
  }

  private bool IsReadyEDrive
  {
    get
    {
      return this.IsEM2Vehicle ? this.digitalReadoutInstrumentParkBrake.RepresentedState == 1 && this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && this.digitalReadoutInstrumentHVReady.RepresentedState == 1 : this.digitalReadoutInstrumentParkBrake.RepresentedState == 1 && this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && this.digitalReadoutInstrumentHVReady.RepresentedState == 1 && this.digitalReadoutInstrumentBlowerSpeed.RepresentedState == 1;
    }
  }

  private bool Ecpc01TOnline
  {
    get
    {
      return this.eCpc01tChannel != null && (this.eCpc01tChannel.CommunicationsState == CommunicationsState.Online || this.eCpc01tChannel.CommunicationsState == CommunicationsState.ExecuteService);
    }
  }

  public virtual void OnChannelsChanged()
  {
    this.SetChannel(this.GetChannel(UserPanel.ChannelName));
  }

  private void SetChannel(Channel newChannel)
  {
    if (newChannel == this.eCpc01tChannel)
      return;
    this.eCpc01tChannel = newChannel;
    this.ResetToBeginning();
  }

  private void PerformNextStep()
  {
    if (this.currentTestType == TestTypes.Battery)
    {
      if (this.batteryCoolantTestSteps.Length > this.currentStepIndex)
        ++this.currentStepIndex;
      else
        this.currentStepIndex = -1;
      this.PerformBatteryTestStep();
    }
    else
    {
      if (this.eDriveCoolantTestSteps.Length > this.currentStepIndex)
        ++this.currentStepIndex;
      else
        this.currentStepIndex = -1;
      this.PerformEDriveTestStep();
    }
  }

  private void PerformBatteryTestStep()
  {
    this.currentStep = this.batteryCoolantTestSteps[this.currentStepIndex];
    this.currentTestType = this.currentStep.TestType;
    this.UpdateMessage(this.currentStep.DisplayText);
    switch (this.currentStep.Action)
    {
      case CoolantTestActions.SetBattery3X2:
        this.SetBattery3X2();
        break;
      case CoolantTestActions.SetBatteryCircuitCoolantPumps:
        this.SetBatteryCoolantPumps();
        break;
      case CoolantTestActions.TurnOnBatteryPTCs:
        this.TurnOnPTCs(true);
        break;
      case CoolantTestActions.WaitForBatteryTemperature:
        this.BatteryTestWait();
        break;
    }
  }

  private void PerformEDriveTestStep()
  {
    this.currentStep = this.eDriveCoolantTestSteps[this.currentStepIndex];
    this.currentTestType = this.currentStep.TestType;
    this.UpdateMessage(this.currentStep.DisplayText);
    switch (this.currentStep.Action)
    {
      case CoolantTestActions.EDriveDeaeration:
        this.ExecuteEDriveDeaeration();
        break;
      case CoolantTestActions.TurnOnCabPTCs:
        this.TurnOnPTCs(false);
        break;
      case CoolantTestActions.WaitForEDriveTemperature:
        this.EDriveTestWait();
        break;
    }
  }

  private void ResetToBeginning()
  {
    this.currentStepIndex = -1;
    this.testStoppedByUser = false;
    this.buttonStartBattery.Text = UserPanel.TextStart;
    this.buttonStartEDrive.Text = UserPanel.TextStart;
    this.UpdateUI();
  }

  private void UpdateMessage(string message)
  {
    if (this.currentStep == null || this.currentTestType == TestTypes.Battery)
    {
      this.labelStatusBattery.Text = message;
      this.LabelLog(this.seekTimeListViewCoolantLoopTest.RequiredUserLabelPrefix, string.Format(Resources.Message_TestMessageFormat, (object) TestTypes.Battery, (object) message));
    }
    if (this.currentStep != null && this.currentTestType != TestTypes.EDrive)
      return;
    this.labelStatusEDrive.Text = message;
    this.LabelLog(this.seekTimeListViewCoolantLoopTest.RequiredUserLabelPrefix, string.Format(Resources.Message_TestMessageFormat, (object) TestTypes.EDrive, (object) message));
  }

  private void UpdateUI()
  {
    bool flag1 = this.Ecpc01TOnline && this.IsReadyBattery;
    bool flag2 = this.Ecpc01TOnline && this.IsReadyEDrive;
    this.checkmarkStatusBattery.Checked = flag1;
    this.checkmarkStatusEDrive.Checked = flag2;
    this.buttonStartBattery.Enabled = flag1;
    this.buttonStartEDrive.Enabled = flag2;
    this.labelStatusBattery.Text = flag1 ? Resources.Message_ReadyToStart : Resources.Message_UnableToStartBattery;
    this.labelStatusEDrive.Text = flag2 ? Resources.Message_ReadyToStart : Resources.Message_UnableToStartEDrive;
    if (this.IsEM2Vehicle)
    {
      ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[2].SizeType = SizeType.Absolute;
      ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[2].Height = 0.0f;
      ((TableLayoutPanel) this.tableLayoutPanel1).Controls["digitalReadoutInstrumentBlowerSpeed"].Hide();
    }
    else
    {
      ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[2].SizeType = SizeType.Absolute;
      ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[2].Height = 40f;
      ((TableLayoutPanel) this.tableLayoutPanel1).Controls["digitalReadoutInstrumentBlowerSpeed"].Show();
    }
  }

  private void SetBattery3X2()
  {
    if (this.eCpc01tChannel == null)
      return;
    this.battery3X2Service = this.eCpc01tChannel.Services[UserPanel.ThreeByTwoWayValuePositionControlStartQualifier];
    if (this.battery3X2Service != (Service) null)
    {
      this.battery3X2Service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.battery3X2Service_ServiceCompleteEvent);
      this.battery3X2Service.InputValues[0].Value = (object) this.battery3X2Service.Choices.GetItemFromRawValue((object) UserPanel.OffDataRawValue);
      this.battery3X2Service.InputValues[1].Value = (object) 0;
      this.battery3X2Service.InputValues[2].Value = (object) this.battery3X2Service.Choices.GetItemFromRawValue((object) UserPanel.OnDataRawValue);
      this.battery3X2Service.InputValues[3].Value = (object) 0;
      this.battery3X2Service.InputValues[4].Value = (object) this.battery3X2Service.Choices.GetItemFromRawValue((object) UserPanel.OffDataRawValue);
      this.battery3X2Service.InputValues[5].Value = (object) 0;
      this.battery3X2Service.Execute(false);
    }
    else
      this.UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {UserPanel.ThreeByTwoWayValuePositionControlStartQualifier}");
  }

  private void battery3X2Service_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.battery3X2Service))
      return;
    this.battery3X2Service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.battery3X2Service_ServiceCompleteEvent);
    if (e.Succeeded)
      this.PerformNextStep();
    else
      this.UpdateMessage(Resources.Message_FailedSettingBattery3x2ValveTo0);
  }

  private void SetBatteryCoolantPumps()
  {
    if (this.eCpc01tChannel == null)
      return;
    this.coolantPumpService = this.eCpc01tChannel.Services[UserPanel.BatteryCircuitWaterPumpControlQualifier];
    if (this.coolantPumpService != (Service) null)
    {
      this.coolantPumpService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.coolantPumpsService_ServiceCompleteEvent);
      this.coolantPumpService.InputValues[0].Value = (object) 60;
      this.coolantPumpService.InputValues[1].Value = (object) 60;
      this.coolantPumpService.Execute(false);
    }
    else
      this.UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {UserPanel.BatteryCircuitWaterPumpControlQualifier}");
  }

  private void coolantPumpsService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.coolantPumpService))
      return;
    this.coolantPumpService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.coolantPumpsService_ServiceCompleteEvent);
    if (e.Succeeded)
      this.PerformNextStep();
    else
      this.UpdateMessage(Resources.Message_FailedSettingBatteryCircuitCoolantPumpsTo60);
  }

  private void TurnOnPTCs(bool turnOnBatteryPTCs)
  {
    if (this.eCpc01tChannel == null)
      return;
    this.ptcOnService = this.eCpc01tChannel.Services[UserPanel.PtcControlStartQualifier];
    if (this.ptcOnService != (Service) null)
    {
      this.ptcOnService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.ptcService_ServiceCompleteEvent);
      this.ptcOnService.InputValues[0].Value = (object) this.ptcOnService.Choices.GetItemFromRawValue((object) (turnOnBatteryPTCs ? UserPanel.OnDataRawValue : UserPanel.OffDataRawValue));
      this.ptcOnService.InputValues[1].Value = (object) this.ptcOnService.Choices.GetItemFromRawValue((object) (turnOnBatteryPTCs ? UserPanel.OnDataRawValue : UserPanel.OffDataRawValue));
      this.ptcOnService.InputValues[2].Value = (object) this.ptcOnService.Choices.GetItemFromRawValue((object) UserPanel.OffDataRawValue);
      this.ptcOnService.InputValues[3].Value = (object) this.ptcOnService.Choices.GetItemFromRawValue((object) (turnOnBatteryPTCs ? UserPanel.OffDataRawValue : UserPanel.OnDataRawValue));
      this.ptcOnService.InputValues[4].Value = (object) this.ptcOnService.Choices.GetItemFromRawValue((object) (turnOnBatteryPTCs ? UserPanel.OffDataRawValue : UserPanel.OnDataRawValue));
      this.ptcOnService.Execute(false);
    }
    else
      this.UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {UserPanel.PtcControlStartQualifier}");
  }

  private void ptcService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.ptcOnService))
      return;
    this.ptcOnService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.ptcService_ServiceCompleteEvent);
    if (e.Succeeded)
      this.PerformNextStep();
    else
      this.UpdateMessage(Resources.Message_FailedTuringOnThePTCs);
  }

  private void TurnOffPTCs()
  {
    if (this.eCpc01tChannel == null)
      return;
    this.UpdateMessage(Resources.Message_TurningOffPTCs);
    this.ptcOffService = this.eCpc01tChannel.Services[UserPanel.PtcControlStopQualifier];
    if (this.ptcOffService != (Service) null)
    {
      this.ptcOffService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.ptcOffService_ServiceCompleteEvent);
      this.ptcOffService.InputValues[0].Value = (object) this.ptcOffService.Choices.GetItemFromRawValue((object) UserPanel.OffDataRawValue);
      this.ptcOffService.InputValues[1].Value = (object) this.ptcOffService.Choices.GetItemFromRawValue((object) UserPanel.OffDataRawValue);
      this.ptcOffService.InputValues[2].Value = (object) this.ptcOffService.Choices.GetItemFromRawValue((object) UserPanel.OffDataRawValue);
      this.ptcOffService.InputValues[3].Value = (object) this.ptcOffService.Choices.GetItemFromRawValue((object) UserPanel.OffDataRawValue);
      this.ptcOffService.InputValues[4].Value = (object) this.ptcOffService.Choices.GetItemFromRawValue((object) UserPanel.OffDataRawValue);
      this.ptcOffService.Execute(false);
    }
    else
      this.UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {UserPanel.PtcControlStopQualifier}");
  }

  private void ptcOffService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.ptcOffService))
      return;
    this.ptcOffService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.ptcService_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.UpdateMessage(Resources.Message_PTCsAreOff);
      if (this.currentTestType == TestTypes.Battery && !this.testStoppedByUser)
        this.UpdateMessage(Resources.Message_BatteryCoolantTestHasFinished);
    }
    else
      this.UpdateMessage(Resources.Message_FailedTurningOffThePTCs);
    if (this.currentTestType == TestTypes.Battery)
      this.ResetToBeginning();
  }

  private void ExecuteEDriveDeaeration()
  {
    if (this.eCpc01tChannel == null)
      return;
    this.eDriveDeaerationService = this.eCpc01tChannel.Services[UserPanel.EDriveDeaerationControlStartQualifier];
    if (this.eDriveDeaerationService != (Service) null)
    {
      this.eDriveDeaerationService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.eDriveDeaerationService_ServiceCompleteEvent);
      this.eDriveDeaerationService.InputValues[0].Value = (object) this.eDriveDeaerationService.Choices[UserPanel.DeaerationDataRawValue];
      this.eDriveDeaerationService.Execute(false);
    }
    else
      this.UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {UserPanel.EDriveDeaerationControlStartQualifier}");
  }

  private void eDriveDeaerationService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.eDriveDeaerationService))
      return;
    this.eDriveDeaerationService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.eDriveDeaerationService_ServiceCompleteEvent);
    if (e.Succeeded)
      this.PerformNextStep();
    else
      this.UpdateMessage(Resources.Message_EDriveDeaearationFailed);
  }

  private void ExecuteEDriveDeaerationStop()
  {
    if (this.eCpc01tChannel == null || this.eCpc01tChannel == null)
      return;
    this.eDriveDeaerationStopService = this.eCpc01tChannel.Services[UserPanel.EDriveDeaerationControlStopQualifier];
    if (this.eDriveDeaerationStopService != (Service) null)
    {
      this.eDriveDeaerationStopService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.eDriveDeaerationStopService_ServiceCompleteEvent);
      this.eDriveDeaerationStopService.InputValues[0].Value = (object) this.eDriveDeaerationStopService.Choices[UserPanel.DeaerationDataRawValue];
      this.eDriveDeaerationStopService.Execute(false);
    }
    else
      this.UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {UserPanel.EDriveDeaerationControlStopQualifier}");
  }

  private void eDriveDeaerationStopService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!(sender as Service == this.eDriveDeaerationStopService))
      return;
    this.eDriveDeaerationStopService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.eDriveDeaerationStopService_ServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.UpdateMessage(Resources.Message_EDriveDeaerationStopped);
      if (!this.testStoppedByUser)
        this.UpdateMessage(Resources.Message_EDriveCoolantTestHasFinished);
    }
    else
      this.UpdateMessage(Resources.Message_EDriveDeaearationStopFailed);
    this.ResetToBeginning();
  }

  private void BatteryTestWait()
  {
    if (this.IsBatteryTemperatureReady())
    {
      this.BatteryTestFinished();
    }
    else
    {
      ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).DataChanged += new EventHandler(this.digitalReadoutInstrumentBatteryCircTemp_DataChanged);
      this.StartTestCounter(UserPanel.BatteryTestDelayTime);
    }
  }

  private void digitalReadoutInstrumentBatteryCircTemp_DataChanged(object sender, EventArgs e)
  {
    if (!this.IsBatteryTemperatureReady())
      return;
    this.BatteryTestFinished();
  }

  private bool IsBatteryTemperatureReady()
  {
    double num = ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).Unit.Contains("C") ? 38.0 : 100.4;
    if (((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).DataItem == null)
      return false;
    object obj = ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).DataItem.Value;
    if (obj == null || ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).DataItem.ValueAsDouble(obj) < num)
      return false;
    this.UpdateMessage(string.Format(Resources.Message_TemperatureIs01, (object) ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).DataItem.ValueAsDouble(obj), (object) ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).Unit));
    return true;
  }

  private void BatteryTestFinished()
  {
    if (this.testTimer != null)
      this.testTimer.Stop();
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).DataChanged -= new EventHandler(this.digitalReadoutInstrumentBatteryCircTemp_DataChanged);
    this.TurnOffPTCs();
  }

  private void EDriveTestWait()
  {
    if (this.IsEDriveTemperatureReady())
    {
      this.EDriveTestFinished();
    }
    else
    {
      ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).DataChanged += new EventHandler(this.digitalReadoutInstrumentEDriveCircOutTemp_DataChanged);
      this.StartTestCounter(UserPanel.EDriveTestDelayTime);
    }
  }

  private void digitalReadoutInstrumentEDriveCircOutTemp_DataChanged(object sender, EventArgs e)
  {
    if (!this.IsEDriveTemperatureReady())
      return;
    this.EDriveTestFinished();
  }

  private bool IsEDriveTemperatureReady()
  {
    double num = ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).Unit.Contains("C") ? 50.0 : 122.0;
    if (((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).DataItem == null)
      return false;
    object obj = ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).DataItem.Value;
    if (obj == null || ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).DataItem.ValueAsDouble(obj) < num)
      return false;
    this.UpdateMessage(string.Format(Resources.Message_TemperatureIs01, (object) ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).DataItem.ValueAsDouble(obj), (object) ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).Unit));
    return true;
  }

  private void EDriveTestFinished()
  {
    if (this.testTimer != null)
      this.testTimer.Stop();
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).DataChanged -= new EventHandler(this.digitalReadoutInstrumentEDriveCircOutTemp_DataChanged);
    this.TurnOffPTCs();
    if (this.testStoppedByUser)
      this.ExecuteEDriveDeaerationStop();
    else
      this.StartTestCounter(UserPanel.EDriveTestDelayTimeAfter);
  }

  private void StartTestCounter(int delayTime)
  {
    this.testCounter = delayTime;
    this.testTimer = new Timer();
    this.testTimer.Interval = 1000;
    if (delayTime != UserPanel.EDriveTestDelayTimeAfter)
      this.testTimer.Tick += new EventHandler(this.Test_Tick);
    else
      this.testTimer.Tick += new EventHandler(this.TestCleanUp_Tick);
    this.testTimer.Start();
  }

  private void Test_Tick(object sender, EventArgs e)
  {
    --this.testCounter;
    if (this.currentStep.TestType == TestTypes.Battery)
    {
      if (this.testCounter <= 0)
        this.BatteryTestFinished();
      else
        this.labelStatusBattery.Text = string.Format(Resources.Message_WaitingUntilTemperatureReaches38DegCOrFor0Seconds, (object) this.testCounter);
    }
    else if (this.testCounter <= 0)
      this.EDriveTestFinished();
    else
      this.labelStatusEDrive.Text = string.Format(Resources.Message_WaitingUntilTemperatureReaches50DegCOrFor0Seconds, (object) this.testCounter);
  }

  private void TestCleanUp_Tick(object sender, EventArgs e)
  {
    --this.testCounter;
    if (this.currentStep.TestType != TestTypes.EDrive)
      return;
    if (this.testCounter <= 0)
    {
      if (this.testTimer != null)
        this.testTimer.Stop();
      this.ExecuteEDriveDeaerationStop();
    }
    else
      this.labelStatusEDrive.Text = string.Format(Resources.Message_EDriveWaitingFor0Seconds, (object) this.testCounter);
  }

  private void SetButtons()
  {
    this.buttonStartEDrive.Enabled = this.currentTestType == TestTypes.EDrive;
    ((Control) this.checkmarkStatusEDrive).Enabled = this.currentTestType == TestTypes.EDrive;
    ((Control) this.checkmarkStatusEDrive).Enabled = this.currentTestType == TestTypes.EDrive;
    this.buttonStartBattery.Enabled = this.currentTestType == TestTypes.Battery;
    ((Control) this.checkmarkStatusBattery).Enabled = this.currentTestType == TestTypes.Battery;
    this.labelStatusBattery.Enabled = this.currentTestType == TestTypes.Battery;
  }

  private void buttonStartBattery_Click(object sender, EventArgs e)
  {
    if (this.buttonStartBattery.Text == UserPanel.TextStart)
    {
      this.buttonStartBattery.Text = UserPanel.TextStop;
      this.currentTestType = TestTypes.Battery;
      this.SetButtons();
      this.PerformNextStep();
    }
    else
    {
      this.testStoppedByUser = true;
      this.UpdateMessage(Resources.Message_TestStopped);
      this.BatteryTestFinished();
    }
  }

  private void buttonStartEDrive_Click(object sender, EventArgs e)
  {
    if (this.buttonStartEDrive.Text == UserPanel.TextStart)
    {
      this.buttonStartEDrive.Text = UserPanel.TextStop;
      this.currentTestType = TestTypes.EDrive;
      this.SetButtons();
      this.PerformNextStep();
    }
    else
    {
      this.testStoppedByUser = true;
      this.UpdateMessage(Resources.Message_TestStopped);
      this.EDriveTestFinished();
    }
  }

  private void digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentParkBrake_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentHVReady_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentBlowerSpeed_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.selectablePanel1 = new SelectablePanel();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentEDriveCircOutTemp = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBatteryCircTemp = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument9 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument8 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentHVReady = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentBlowerSpeed = new DigitalReadoutInstrument();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.buttonStartEDrive = new Button();
    this.labelStatusEDrive = new System.Windows.Forms.Label();
    this.checkmarkStatusEDrive = new Checkmark();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.seekTimeListViewCoolantLoopTest = new SeekTimeListView();
    this.label3 = new System.Windows.Forms.Label();
    this.label2 = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.buttonStartBattery = new Button();
    this.labelStatusBattery = new System.Windows.Forms.Label();
    this.checkmarkStatusBattery = new Checkmark();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.scalingLabel1 = new ScalingLabel();
    ((Control) this.selectablePanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    ((Control) this.selectablePanel1).Controls.Add((Control) this.tableLayoutPanel1);
    componentResourceManager.ApplyResources((object) this.selectablePanel1, "selectablePanel1");
    ((Control) this.selectablePanel1).Name = "selectablePanel1";
    ((Panel) this.selectablePanel1).TabStop = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument2, 3, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 3, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentEDriveCircOutTemp, 3, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentBatteryCircTemp, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument9, 3, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument8, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentHVReady, 3, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument6, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentBlowerSpeed, 3, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListViewCoolantLoopTest, 0, 11);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label3, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.scalingLabel1, 3, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument2, "digitalReadoutInstrument2");
    this.digitalReadoutInstrument2.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS243_PwmOutput05ReqDutyCycle_PwmOutput05ReqDutyCycle");
    ((Control) this.digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_OTF_ETHM_PtcCtrl_Request_Results_OTF_ETHM_Cabin_PTC2_High_Voltage");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentEDriveCircOutTemp, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEDriveCircOutTemp, "digitalReadoutInstrumentEDriveCircOutTemp");
    this.digitalReadoutInstrumentEDriveCircOutTemp.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS253_EDrvCircOutTemp_EDrvCircOutTemp");
    ((Control) this.digitalReadoutInstrumentEDriveCircOutTemp).Name = "digitalReadoutInstrumentEDriveCircOutTemp";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEDriveCircOutTemp).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentBatteryCircTemp, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBatteryCircTemp, "digitalReadoutInstrumentBatteryCircTemp");
    this.digitalReadoutInstrumentBatteryCircTemp.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS202_Batt_Circ_Temp_Batt_Circ_Temp");
    ((Control) this.digitalReadoutInstrumentBatteryCircTemp).Name = "digitalReadoutInstrumentBatteryCircTemp";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBatteryCircTemp).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument9, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument9, "digitalReadoutInstrument9");
    this.digitalReadoutInstrument9.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS082_LIN2_PTC_Cab1_DutyCycle_LIN2_PTC_Cab1_DutyCycle");
    ((Control) this.digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument8, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument8, "digitalReadoutInstrument8");
    this.digitalReadoutInstrument8.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS081_LIN1_PTC_Batt2_DutyCycle_LIN1_PTC_Batt2_DutyCycle");
    ((Control) this.digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentHVReady, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentHVReady, "digitalReadoutInstrumentHVReady");
    this.digitalReadoutInstrumentHVReady.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).FreezeValue = false;
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentHVReady.Gradient.Initialize((ValueState) 3, 4);
    this.digitalReadoutInstrumentHVReady.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentHVReady.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentHVReady.Gradient.Modify(3, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentHVReady.Gradient.Modify(4, 3.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS008_HV_Ready");
    ((Control) this.digitalReadoutInstrumentHVReady).Name = "digitalReadoutInstrumentHVReady";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHVReady).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentHVReady.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentHVReady_RepresentedStateChanged);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument6, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS007_AmbientAirTemperature_AmbientAirTemperature");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentBlowerSpeed, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentBlowerSpeed, "digitalReadoutInstrumentBlowerSpeed");
    this.digitalReadoutInstrumentBlowerSpeed.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBlowerSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentBlowerSpeed.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrumentBlowerSpeed.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBlowerSpeed).Instrument = new Qualifier((QualifierTypes) 1, "HVAC_F01T", "DT_Blower_Speed_feedback_from_blower_Blower_Speed_feedback_from_blower");
    ((Control) this.digitalReadoutInstrumentBlowerSpeed).Name = "digitalReadoutInstrumentBlowerSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBlowerSpeed).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBlowerSpeed).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBlowerSpeed).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentBlowerSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentBlowerSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentBlowerSpeed_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.buttonStartEDrive, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.labelStatusEDrive, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.checkmarkStatusEDrive, 0, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.buttonStartEDrive, "buttonStartEDrive");
    this.buttonStartEDrive.Name = "buttonStartEDrive";
    this.buttonStartEDrive.UseCompatibleTextRendering = true;
    this.buttonStartEDrive.UseVisualStyleBackColor = true;
    this.buttonStartEDrive.Click += new EventHandler(this.buttonStartEDrive_Click);
    componentResourceManager.ApplyResources((object) this.labelStatusEDrive, "labelStatusEDrive");
    this.labelStatusEDrive.Name = "labelStatusEDrive";
    this.labelStatusEDrive.UseCompatibleTextRendering = true;
    this.checkmarkStatusEDrive.CheckState = CheckState.Checked;
    componentResourceManager.ApplyResources((object) this.checkmarkStatusEDrive, "checkmarkStatusEDrive");
    ((Control) this.checkmarkStatusEDrive).Name = "checkmarkStatusEDrive";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentVehicleSpeed, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 1, 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListViewCoolantLoopTest, 5);
    componentResourceManager.ApplyResources((object) this.seekTimeListViewCoolantLoopTest, "seekTimeListViewCoolantLoopTest");
    this.seekTimeListViewCoolantLoopTest.FilterUserLabels = true;
    ((Control) this.seekTimeListViewCoolantLoopTest).Name = "seekTimeListViewCoolantLoopTest";
    this.seekTimeListViewCoolantLoopTest.RequiredUserLabelPrefix = "CoolantLoopTest";
    this.seekTimeListViewCoolantLoopTest.SelectedTime = new DateTime?();
    this.seekTimeListViewCoolantLoopTest.ShowChannelLabels = false;
    this.seekTimeListViewCoolantLoopTest.ShowCommunicationsState = false;
    this.seekTimeListViewCoolantLoopTest.ShowControlPanel = false;
    this.seekTimeListViewCoolantLoopTest.ShowDeviceColumn = false;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label3, 2);
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label2, 2);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentParkBrake, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentParkBrake_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonStartBattery, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelStatusBattery, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmarkStatusBattery, 0, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.buttonStartBattery, "buttonStartBattery");
    this.buttonStartBattery.Name = "buttonStartBattery";
    this.buttonStartBattery.UseCompatibleTextRendering = true;
    this.buttonStartBattery.UseVisualStyleBackColor = true;
    this.buttonStartBattery.Click += new EventHandler(this.buttonStartBattery_Click);
    componentResourceManager.ApplyResources((object) this.labelStatusBattery, "labelStatusBattery");
    this.labelStatusBattery.Name = "labelStatusBattery";
    this.labelStatusBattery.UseCompatibleTextRendering = true;
    this.checkmarkStatusBattery.CheckState = CheckState.Checked;
    componentResourceManager.ApplyResources((object) this.checkmarkStatusBattery, "checkmarkStatusBattery");
    ((Control) this.checkmarkStatusBattery).Name = "checkmarkStatusBattery";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument4, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "CoolantTest";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS080_LIN1_PTC_Batt1_DutyCycle_LIN1_PTC_Batt1_DutyCycle");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).TitleLengthPercentOfControl = 60;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).TitlePosition = (LabelPosition) 3;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).TitleWordWrap = true;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    this.scalingLabel1.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.scalingLabel1, 2);
    componentResourceManager.ApplyResources((object) this.scalingLabel1, "scalingLabel1");
    this.scalingLabel1.FontGroup = (string) null;
    this.scalingLabel1.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabel1).Name = "scalingLabel1";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Coolant_Systems_Pressure_Test");
    ((Control) this).Controls.Add((Control) this.selectablePanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.selectablePanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
