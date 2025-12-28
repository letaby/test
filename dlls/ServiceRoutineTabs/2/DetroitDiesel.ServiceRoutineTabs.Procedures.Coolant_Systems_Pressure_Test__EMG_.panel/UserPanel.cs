using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

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

	private CoolantTestStep currentStep = null;

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

	private Timer testTimer = null;

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

	private bool IsEM2Vehicle
	{
		get
		{
			if (SapiManager.GlobalInstance != null && SapiManager.GlobalInstance.ConnectedEquipment.Any(delegate(EquipmentType et)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
				return ((ElectronicsFamily)(ref family)).Name.ToUpper().Contains("EMOBILITY");
			}) && SapiManager.GlobalInstance.ConnectedEquipment.Any(delegate(EquipmentType et)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
				return ((ElectronicsFamily)(ref family)).Name.ToUpper().Contains("EM2");
			}))
			{
				return true;
			}
			return false;
		}
	}

	private bool IsReadyBattery => (int)digitalReadoutInstrumentParkBrake.RepresentedState == 1 && (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && (int)digitalReadoutInstrumentHVReady.RepresentedState == 1;

	private bool IsReadyEDrive
	{
		get
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Invalid comparison between Unknown and I4
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Invalid comparison between Unknown and I4
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Invalid comparison between Unknown and I4
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Invalid comparison between Unknown and I4
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Invalid comparison between Unknown and I4
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Invalid comparison between Unknown and I4
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Invalid comparison between Unknown and I4
			if (IsEM2Vehicle)
			{
				return (int)digitalReadoutInstrumentParkBrake.RepresentedState == 1 && (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && (int)digitalReadoutInstrumentHVReady.RepresentedState == 1;
			}
			return (int)digitalReadoutInstrumentParkBrake.RepresentedState == 1 && (int)digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 && (int)digitalReadoutInstrumentHVReady.RepresentedState == 1 && (int)digitalReadoutInstrumentBlowerSpeed.RepresentedState == 1;
		}
	}

	private bool Ecpc01TOnline => eCpc01tChannel != null && (eCpc01tChannel.CommunicationsState == CommunicationsState.Online || eCpc01tChannel.CommunicationsState == CommunicationsState.ExecuteService);

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetChannel(((CustomPanel)this).GetChannel(ChannelName));
	}

	private void SetChannel(Channel newChannel)
	{
		if (newChannel != eCpc01tChannel)
		{
			eCpc01tChannel = newChannel;
			ResetToBeginning();
		}
	}

	private void PerformNextStep()
	{
		if (currentTestType == TestTypes.Battery)
		{
			if (batteryCoolantTestSteps.Length > currentStepIndex)
			{
				currentStepIndex++;
			}
			else
			{
				currentStepIndex = -1;
			}
			PerformBatteryTestStep();
		}
		else
		{
			if (eDriveCoolantTestSteps.Length > currentStepIndex)
			{
				currentStepIndex++;
			}
			else
			{
				currentStepIndex = -1;
			}
			PerformEDriveTestStep();
		}
	}

	private void PerformBatteryTestStep()
	{
		currentStep = batteryCoolantTestSteps[currentStepIndex];
		currentTestType = currentStep.TestType;
		UpdateMessage(currentStep.DisplayText);
		switch (currentStep.Action)
		{
		case CoolantTestActions.SetBattery3X2:
			SetBattery3X2();
			break;
		case CoolantTestActions.SetBatteryCircuitCoolantPumps:
			SetBatteryCoolantPumps();
			break;
		case CoolantTestActions.TurnOnBatteryPTCs:
			TurnOnPTCs(turnOnBatteryPTCs: true);
			break;
		case CoolantTestActions.WaitForBatteryTemperature:
			BatteryTestWait();
			break;
		}
	}

	private void PerformEDriveTestStep()
	{
		currentStep = eDriveCoolantTestSteps[currentStepIndex];
		currentTestType = currentStep.TestType;
		UpdateMessage(currentStep.DisplayText);
		switch (currentStep.Action)
		{
		case CoolantTestActions.EDriveDeaeration:
			ExecuteEDriveDeaeration();
			break;
		case CoolantTestActions.TurnOnCabPTCs:
			TurnOnPTCs(turnOnBatteryPTCs: false);
			break;
		case CoolantTestActions.WaitForEDriveTemperature:
			EDriveTestWait();
			break;
		}
	}

	private void ResetToBeginning()
	{
		currentStepIndex = -1;
		testStoppedByUser = false;
		buttonStartBattery.Text = TextStart;
		buttonStartEDrive.Text = TextStart;
		UpdateUI();
	}

	private void UpdateMessage(string message)
	{
		if (currentStep == null || currentTestType == TestTypes.Battery)
		{
			labelStatusBattery.Text = message;
			((CustomPanel)this).LabelLog(seekTimeListViewCoolantLoopTest.RequiredUserLabelPrefix, string.Format(Resources.Message_TestMessageFormat, TestTypes.Battery, message));
		}
		if (currentStep == null || currentTestType == TestTypes.EDrive)
		{
			labelStatusEDrive.Text = message;
			((CustomPanel)this).LabelLog(seekTimeListViewCoolantLoopTest.RequiredUserLabelPrefix, string.Format(Resources.Message_TestMessageFormat, TestTypes.EDrive, message));
		}
	}

	private void UpdateUI()
	{
		bool flag = Ecpc01TOnline && IsReadyBattery;
		bool flag2 = Ecpc01TOnline && IsReadyEDrive;
		checkmarkStatusBattery.Checked = flag;
		checkmarkStatusEDrive.Checked = flag2;
		buttonStartBattery.Enabled = flag;
		buttonStartEDrive.Enabled = flag2;
		labelStatusBattery.Text = (flag ? Resources.Message_ReadyToStart : Resources.Message_UnableToStartBattery);
		labelStatusEDrive.Text = (flag2 ? Resources.Message_ReadyToStart : Resources.Message_UnableToStartEDrive);
		if (IsEM2Vehicle)
		{
			((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[2].SizeType = SizeType.Absolute;
			((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[2].Height = 0f;
			((TableLayoutPanel)(object)tableLayoutPanel1).Controls["digitalReadoutInstrumentBlowerSpeed"].Hide();
		}
		else
		{
			((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[2].SizeType = SizeType.Absolute;
			((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[2].Height = 40f;
			((TableLayoutPanel)(object)tableLayoutPanel1).Controls["digitalReadoutInstrumentBlowerSpeed"].Show();
		}
	}

	private void SetBattery3X2()
	{
		if (eCpc01tChannel != null)
		{
			battery3X2Service = eCpc01tChannel.Services[ThreeByTwoWayValuePositionControlStartQualifier];
			if (battery3X2Service != null)
			{
				battery3X2Service.ServiceCompleteEvent += battery3X2Service_ServiceCompleteEvent;
				battery3X2Service.InputValues[0].Value = battery3X2Service.Choices.GetItemFromRawValue(OffDataRawValue);
				battery3X2Service.InputValues[1].Value = 0;
				battery3X2Service.InputValues[2].Value = battery3X2Service.Choices.GetItemFromRawValue(OnDataRawValue);
				battery3X2Service.InputValues[3].Value = 0;
				battery3X2Service.InputValues[4].Value = battery3X2Service.Choices.GetItemFromRawValue(OffDataRawValue);
				battery3X2Service.InputValues[5].Value = 0;
				battery3X2Service.Execute(synchronous: false);
			}
			else
			{
				UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {ThreeByTwoWayValuePositionControlStartQualifier}");
			}
		}
	}

	private void battery3X2Service_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender as Service == battery3X2Service)
		{
			battery3X2Service.ServiceCompleteEvent -= battery3X2Service_ServiceCompleteEvent;
			if (e.Succeeded)
			{
				PerformNextStep();
			}
			else
			{
				UpdateMessage(Resources.Message_FailedSettingBattery3x2ValveTo0);
			}
		}
	}

	private void SetBatteryCoolantPumps()
	{
		if (eCpc01tChannel != null)
		{
			coolantPumpService = eCpc01tChannel.Services[BatteryCircuitWaterPumpControlQualifier];
			if (coolantPumpService != null)
			{
				coolantPumpService.ServiceCompleteEvent += coolantPumpsService_ServiceCompleteEvent;
				coolantPumpService.InputValues[0].Value = 60;
				coolantPumpService.InputValues[1].Value = 60;
				coolantPumpService.Execute(synchronous: false);
			}
			else
			{
				UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {BatteryCircuitWaterPumpControlQualifier}");
			}
		}
	}

	private void coolantPumpsService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender as Service == coolantPumpService)
		{
			coolantPumpService.ServiceCompleteEvent -= coolantPumpsService_ServiceCompleteEvent;
			if (e.Succeeded)
			{
				PerformNextStep();
			}
			else
			{
				UpdateMessage(Resources.Message_FailedSettingBatteryCircuitCoolantPumpsTo60);
			}
		}
	}

	private void TurnOnPTCs(bool turnOnBatteryPTCs)
	{
		if (eCpc01tChannel != null)
		{
			ptcOnService = eCpc01tChannel.Services[PtcControlStartQualifier];
			if (ptcOnService != null)
			{
				ptcOnService.ServiceCompleteEvent += ptcService_ServiceCompleteEvent;
				ptcOnService.InputValues[0].Value = ptcOnService.Choices.GetItemFromRawValue(turnOnBatteryPTCs ? OnDataRawValue : OffDataRawValue);
				ptcOnService.InputValues[1].Value = ptcOnService.Choices.GetItemFromRawValue(turnOnBatteryPTCs ? OnDataRawValue : OffDataRawValue);
				ptcOnService.InputValues[2].Value = ptcOnService.Choices.GetItemFromRawValue(OffDataRawValue);
				ptcOnService.InputValues[3].Value = ptcOnService.Choices.GetItemFromRawValue(turnOnBatteryPTCs ? OffDataRawValue : OnDataRawValue);
				ptcOnService.InputValues[4].Value = ptcOnService.Choices.GetItemFromRawValue(turnOnBatteryPTCs ? OffDataRawValue : OnDataRawValue);
				ptcOnService.Execute(synchronous: false);
			}
			else
			{
				UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {PtcControlStartQualifier}");
			}
		}
	}

	private void ptcService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender as Service == ptcOnService)
		{
			ptcOnService.ServiceCompleteEvent -= ptcService_ServiceCompleteEvent;
			if (e.Succeeded)
			{
				PerformNextStep();
			}
			else
			{
				UpdateMessage(Resources.Message_FailedTuringOnThePTCs);
			}
		}
	}

	private void TurnOffPTCs()
	{
		if (eCpc01tChannel != null)
		{
			UpdateMessage(Resources.Message_TurningOffPTCs);
			ptcOffService = eCpc01tChannel.Services[PtcControlStopQualifier];
			if (ptcOffService != null)
			{
				ptcOffService.ServiceCompleteEvent += ptcOffService_ServiceCompleteEvent;
				ptcOffService.InputValues[0].Value = ptcOffService.Choices.GetItemFromRawValue(OffDataRawValue);
				ptcOffService.InputValues[1].Value = ptcOffService.Choices.GetItemFromRawValue(OffDataRawValue);
				ptcOffService.InputValues[2].Value = ptcOffService.Choices.GetItemFromRawValue(OffDataRawValue);
				ptcOffService.InputValues[3].Value = ptcOffService.Choices.GetItemFromRawValue(OffDataRawValue);
				ptcOffService.InputValues[4].Value = ptcOffService.Choices.GetItemFromRawValue(OffDataRawValue);
				ptcOffService.Execute(synchronous: false);
			}
			else
			{
				UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {PtcControlStopQualifier}");
			}
		}
	}

	private void ptcOffService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (!(sender as Service == ptcOffService))
		{
			return;
		}
		ptcOffService.ServiceCompleteEvent -= ptcService_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			UpdateMessage(Resources.Message_PTCsAreOff);
			if (currentTestType == TestTypes.Battery && !testStoppedByUser)
			{
				UpdateMessage(Resources.Message_BatteryCoolantTestHasFinished);
			}
		}
		else
		{
			UpdateMessage(Resources.Message_FailedTurningOffThePTCs);
		}
		if (currentTestType == TestTypes.Battery)
		{
			ResetToBeginning();
		}
	}

	private void ExecuteEDriveDeaeration()
	{
		if (eCpc01tChannel != null)
		{
			eDriveDeaerationService = eCpc01tChannel.Services[EDriveDeaerationControlStartQualifier];
			if (eDriveDeaerationService != null)
			{
				eDriveDeaerationService.ServiceCompleteEvent += eDriveDeaerationService_ServiceCompleteEvent;
				eDriveDeaerationService.InputValues[0].Value = eDriveDeaerationService.Choices[DeaerationDataRawValue];
				eDriveDeaerationService.Execute(synchronous: false);
			}
			else
			{
				UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {EDriveDeaerationControlStartQualifier}");
			}
		}
	}

	private void eDriveDeaerationService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (sender as Service == eDriveDeaerationService)
		{
			eDriveDeaerationService.ServiceCompleteEvent -= eDriveDeaerationService_ServiceCompleteEvent;
			if (e.Succeeded)
			{
				PerformNextStep();
			}
			else
			{
				UpdateMessage(Resources.Message_EDriveDeaearationFailed);
			}
		}
	}

	private void ExecuteEDriveDeaerationStop()
	{
		if (eCpc01tChannel != null && eCpc01tChannel != null)
		{
			eDriveDeaerationStopService = eCpc01tChannel.Services[EDriveDeaerationControlStopQualifier];
			if (eDriveDeaerationStopService != null)
			{
				eDriveDeaerationStopService.ServiceCompleteEvent += eDriveDeaerationStopService_ServiceCompleteEvent;
				eDriveDeaerationStopService.InputValues[0].Value = eDriveDeaerationStopService.Choices[DeaerationDataRawValue];
				eDriveDeaerationStopService.Execute(synchronous: false);
			}
			else
			{
				UpdateMessage($"{Resources.Message_UnableToPerformTestMissingService} {EDriveDeaerationControlStopQualifier}");
			}
		}
	}

	private void eDriveDeaerationStopService_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		if (!(sender as Service == eDriveDeaerationStopService))
		{
			return;
		}
		eDriveDeaerationStopService.ServiceCompleteEvent -= eDriveDeaerationStopService_ServiceCompleteEvent;
		if (e.Succeeded)
		{
			UpdateMessage(Resources.Message_EDriveDeaerationStopped);
			if (!testStoppedByUser)
			{
				UpdateMessage(Resources.Message_EDriveCoolantTestHasFinished);
			}
		}
		else
		{
			UpdateMessage(Resources.Message_EDriveDeaearationStopFailed);
		}
		ResetToBeginning();
	}

	private void BatteryTestWait()
	{
		if (IsBatteryTemperatureReady())
		{
			BatteryTestFinished();
			return;
		}
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).DataChanged += digitalReadoutInstrumentBatteryCircTemp_DataChanged;
		StartTestCounter(BatteryTestDelayTime);
	}

	private void digitalReadoutInstrumentBatteryCircTemp_DataChanged(object sender, EventArgs e)
	{
		if (IsBatteryTemperatureReady())
		{
			BatteryTestFinished();
		}
	}

	private bool IsBatteryTemperatureReady()
	{
		double num = (((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).Unit.Contains("C") ? 38.0 : 100.4);
		if (((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).DataItem != null)
		{
			object value = ((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).DataItem.Value;
			if (value != null && ((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).DataItem.ValueAsDouble(value) >= num)
			{
				UpdateMessage(string.Format(Resources.Message_TemperatureIs01, ((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).DataItem.ValueAsDouble(value), ((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).Unit));
				return true;
			}
			return false;
		}
		return false;
	}

	private void BatteryTestFinished()
	{
		if (testTimer != null)
		{
			testTimer.Stop();
		}
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).DataChanged -= digitalReadoutInstrumentBatteryCircTemp_DataChanged;
		TurnOffPTCs();
	}

	private void EDriveTestWait()
	{
		if (IsEDriveTemperatureReady())
		{
			EDriveTestFinished();
			return;
		}
		((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).DataChanged += digitalReadoutInstrumentEDriveCircOutTemp_DataChanged;
		StartTestCounter(EDriveTestDelayTime);
	}

	private void digitalReadoutInstrumentEDriveCircOutTemp_DataChanged(object sender, EventArgs e)
	{
		if (IsEDriveTemperatureReady())
		{
			EDriveTestFinished();
		}
	}

	private bool IsEDriveTemperatureReady()
	{
		double num = (((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).Unit.Contains("C") ? 50 : 122);
		if (((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).DataItem != null)
		{
			object value = ((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).DataItem.Value;
			if (value != null && ((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).DataItem.ValueAsDouble(value) >= num)
			{
				UpdateMessage(string.Format(Resources.Message_TemperatureIs01, ((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).DataItem.ValueAsDouble(value), ((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).Unit));
				return true;
			}
			return false;
		}
		return false;
	}

	private void EDriveTestFinished()
	{
		if (testTimer != null)
		{
			testTimer.Stop();
		}
		((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).DataChanged -= digitalReadoutInstrumentEDriveCircOutTemp_DataChanged;
		TurnOffPTCs();
		if (testStoppedByUser)
		{
			ExecuteEDriveDeaerationStop();
		}
		else
		{
			StartTestCounter(EDriveTestDelayTimeAfter);
		}
	}

	private void StartTestCounter(int delayTime)
	{
		testCounter = delayTime;
		testTimer = new Timer();
		testTimer.Interval = 1000;
		if (delayTime != EDriveTestDelayTimeAfter)
		{
			testTimer.Tick += Test_Tick;
		}
		else
		{
			testTimer.Tick += TestCleanUp_Tick;
		}
		testTimer.Start();
	}

	private void Test_Tick(object sender, EventArgs e)
	{
		testCounter--;
		if (currentStep.TestType == TestTypes.Battery)
		{
			if (testCounter <= 0)
			{
				BatteryTestFinished();
			}
			else
			{
				labelStatusBattery.Text = string.Format(Resources.Message_WaitingUntilTemperatureReaches38DegCOrFor0Seconds, testCounter);
			}
		}
		else if (testCounter <= 0)
		{
			EDriveTestFinished();
		}
		else
		{
			labelStatusEDrive.Text = string.Format(Resources.Message_WaitingUntilTemperatureReaches50DegCOrFor0Seconds, testCounter);
		}
	}

	private void TestCleanUp_Tick(object sender, EventArgs e)
	{
		testCounter--;
		if (currentStep.TestType != TestTypes.EDrive)
		{
			return;
		}
		if (testCounter <= 0)
		{
			if (testTimer != null)
			{
				testTimer.Stop();
			}
			ExecuteEDriveDeaerationStop();
		}
		else
		{
			labelStatusEDrive.Text = string.Format(Resources.Message_EDriveWaitingFor0Seconds, testCounter);
		}
	}

	private void SetButtons()
	{
		buttonStartEDrive.Enabled = currentTestType == TestTypes.EDrive;
		((Control)(object)checkmarkStatusEDrive).Enabled = currentTestType == TestTypes.EDrive;
		((Control)(object)checkmarkStatusEDrive).Enabled = currentTestType == TestTypes.EDrive;
		buttonStartBattery.Enabled = currentTestType == TestTypes.Battery;
		((Control)(object)checkmarkStatusBattery).Enabled = currentTestType == TestTypes.Battery;
		labelStatusBattery.Enabled = currentTestType == TestTypes.Battery;
	}

	private void buttonStartBattery_Click(object sender, EventArgs e)
	{
		if (buttonStartBattery.Text == TextStart)
		{
			buttonStartBattery.Text = TextStop;
			currentTestType = TestTypes.Battery;
			SetButtons();
			PerformNextStep();
		}
		else
		{
			testStoppedByUser = true;
			UpdateMessage(Resources.Message_TestStopped);
			BatteryTestFinished();
		}
	}

	private void buttonStartEDrive_Click(object sender, EventArgs e)
	{
		if (buttonStartEDrive.Text == TextStart)
		{
			buttonStartEDrive.Text = TextStop;
			currentTestType = TestTypes.EDrive;
			SetButtons();
			PerformNextStep();
		}
		else
		{
			testStoppedByUser = true;
			UpdateMessage(Resources.Message_TestStopped);
			EDriveTestFinished();
		}
	}

	private void digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void digitalReadoutInstrumentParkBrake_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void digitalReadoutInstrumentHVReady_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void digitalReadoutInstrumentBlowerSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_119e: Unknown result type (might be due to invalid IL or missing references)
		//IL_125f: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		selectablePanel1 = new SelectablePanel();
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEDriveCircOutTemp = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBatteryCircTemp = new DigitalReadoutInstrument();
		digitalReadoutInstrument9 = new DigitalReadoutInstrument();
		digitalReadoutInstrument8 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentHVReady = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentBlowerSpeed = new DigitalReadoutInstrument();
		tableLayoutPanel3 = new TableLayoutPanel();
		buttonStartEDrive = new Button();
		labelStatusEDrive = new System.Windows.Forms.Label();
		checkmarkStatusEDrive = new Checkmark();
		digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
		seekTimeListViewCoolantLoopTest = new SeekTimeListView();
		label3 = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		buttonStartBattery = new Button();
		labelStatusBattery = new System.Windows.Forms.Label();
		checkmarkStatusBattery = new Checkmark();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		scalingLabel1 = new ScalingLabel();
		((Control)(object)selectablePanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)(object)selectablePanel1).Controls.Add((Control)(object)tableLayoutPanel1);
		componentResourceManager.ApplyResources(selectablePanel1, "selectablePanel1");
		((Control)(object)selectablePanel1).Name = "selectablePanel1";
		((Panel)(object)selectablePanel1).TabStop = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 3, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentEDriveCircOutTemp, 3, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentBatteryCircTemp, 0, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument9, 3, 7);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument8, 0, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentHVReady, 3, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument6, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentBlowerSpeed, 3, 9);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleSpeed, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListViewCoolantLoopTest, 0, 11);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label3, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentParkBrake, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 8);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)scalingLabel1, 3, 2);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS243_PwmOutput05ReqDutyCycle_PwmOutput05ReqDutyCycle");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument2).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument2).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_ETHM_PtcCtrl_Request_Results_OTF_ETHM_Cabin_PTC2_High_Voltage");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument1).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument1).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentEDriveCircOutTemp, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEDriveCircOutTemp, "digitalReadoutInstrumentEDriveCircOutTemp");
		digitalReadoutInstrumentEDriveCircOutTemp.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS253_EDrvCircOutTemp_EDrvCircOutTemp");
		((Control)(object)digitalReadoutInstrumentEDriveCircOutTemp).Name = "digitalReadoutInstrumentEDriveCircOutTemp";
		((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentEDriveCircOutTemp).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentBatteryCircTemp, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBatteryCircTemp, "digitalReadoutInstrumentBatteryCircTemp");
		digitalReadoutInstrumentBatteryCircTemp.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS202_Batt_Circ_Temp_Batt_Circ_Temp");
		((Control)(object)digitalReadoutInstrumentBatteryCircTemp).Name = "digitalReadoutInstrumentBatteryCircTemp";
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBatteryCircTemp).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument9, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument9, "digitalReadoutInstrument9");
		digitalReadoutInstrument9.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrument9).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS082_LIN2_PTC_Cab1_DutyCycle_LIN2_PTC_Cab1_DutyCycle");
		((Control)(object)digitalReadoutInstrument9).Name = "digitalReadoutInstrument9";
		((SingleInstrumentBase)digitalReadoutInstrument9).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument9).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument9).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument9).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument8, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument8, "digitalReadoutInstrument8");
		digitalReadoutInstrument8.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrument8).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument8).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS081_LIN1_PTC_Batt2_DutyCycle_LIN1_PTC_Batt2_DutyCycle");
		((Control)(object)digitalReadoutInstrument8).Name = "digitalReadoutInstrument8";
		((SingleInstrumentBase)digitalReadoutInstrument8).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument8).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument8).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument8).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentHVReady, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentHVReady, "digitalReadoutInstrumentHVReady");
		digitalReadoutInstrumentHVReady.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).FreezeValue = false;
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentHVReady.Gradient.Initialize((ValueState)3, 4);
		digitalReadoutInstrumentHVReady.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentHVReady.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentHVReady.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrumentHVReady.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS008_HV_Ready");
		((Control)(object)digitalReadoutInstrumentHVReady).Name = "digitalReadoutInstrumentHVReady";
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentHVReady.RepresentedStateChanged += digitalReadoutInstrumentHVReady_RepresentedStateChanged;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument6, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS007_AmbientAirTemperature_AmbientAirTemperature");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument6).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument6).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentBlowerSpeed, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentBlowerSpeed, "digitalReadoutInstrumentBlowerSpeed");
		digitalReadoutInstrumentBlowerSpeed.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrumentBlowerSpeed).FreezeValue = false;
		digitalReadoutInstrumentBlowerSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrumentBlowerSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentBlowerSpeed).Instrument = new Qualifier((QualifierTypes)1, "HVAC_F01T", "DT_Blower_Speed_feedback_from_blower_Blower_Speed_feedback_from_blower");
		((Control)(object)digitalReadoutInstrumentBlowerSpeed).Name = "digitalReadoutInstrumentBlowerSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentBlowerSpeed).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrumentBlowerSpeed).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentBlowerSpeed).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentBlowerSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentBlowerSpeed.RepresentedStateChanged += digitalReadoutInstrumentBlowerSpeed_RepresentedStateChanged;
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(buttonStartEDrive, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(labelStatusEDrive, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)checkmarkStatusEDrive, 0, 0);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		componentResourceManager.ApplyResources(buttonStartEDrive, "buttonStartEDrive");
		buttonStartEDrive.Name = "buttonStartEDrive";
		buttonStartEDrive.UseCompatibleTextRendering = true;
		buttonStartEDrive.UseVisualStyleBackColor = true;
		buttonStartEDrive.Click += buttonStartEDrive_Click;
		componentResourceManager.ApplyResources(labelStatusEDrive, "labelStatusEDrive");
		labelStatusEDrive.Name = "labelStatusEDrive";
		labelStatusEDrive.UseCompatibleTextRendering = true;
		checkmarkStatusEDrive.CheckState = CheckState.Checked;
		componentResourceManager.ApplyResources(checkmarkStatusEDrive, "checkmarkStatusEDrive");
		((Control)(object)checkmarkStatusEDrive).Name = "checkmarkStatusEDrive";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentVehicleSpeed, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
		digitalReadoutInstrumentVehicleSpeed.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
		digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		((Control)(object)digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListViewCoolantLoopTest, 5);
		componentResourceManager.ApplyResources(seekTimeListViewCoolantLoopTest, "seekTimeListViewCoolantLoopTest");
		seekTimeListViewCoolantLoopTest.FilterUserLabels = true;
		((Control)(object)seekTimeListViewCoolantLoopTest).Name = "seekTimeListViewCoolantLoopTest";
		seekTimeListViewCoolantLoopTest.RequiredUserLabelPrefix = "CoolantLoopTest";
		seekTimeListViewCoolantLoopTest.SelectedTime = null;
		seekTimeListViewCoolantLoopTest.ShowChannelLabels = false;
		seekTimeListViewCoolantLoopTest.ShowCommunicationsState = false;
		seekTimeListViewCoolantLoopTest.ShowControlPanel = false;
		seekTimeListViewCoolantLoopTest.ShowDeviceColumn = false;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label3, 2);
		componentResourceManager.ApplyResources(label3, "label3");
		label3.Name = "label3";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label2, 2);
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentParkBrake, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
		digitalReadoutInstrumentParkBrake.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).FreezeValue = false;
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
		((Control)(object)digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentParkBrake.RepresentedStateChanged += digitalReadoutInstrumentParkBrake_RepresentedStateChanged;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonStartBattery, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(labelStatusBattery, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmarkStatusBattery, 0, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(buttonStartBattery, "buttonStartBattery");
		buttonStartBattery.Name = "buttonStartBattery";
		buttonStartBattery.UseCompatibleTextRendering = true;
		buttonStartBattery.UseVisualStyleBackColor = true;
		buttonStartBattery.Click += buttonStartBattery_Click;
		componentResourceManager.ApplyResources(labelStatusBattery, "labelStatusBattery");
		labelStatusBattery.Name = "labelStatusBattery";
		labelStatusBattery.UseCompatibleTextRendering = true;
		checkmarkStatusBattery.CheckState = CheckState.Checked;
		componentResourceManager.ApplyResources(checkmarkStatusBattery, "checkmarkStatusBattery");
		((Control)(object)checkmarkStatusBattery).Name = "checkmarkStatusBattery";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument4, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "CoolantTest";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS080_LIN1_PTC_Batt1_DutyCycle_LIN1_PTC_Batt1_DutyCycle");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).TitleLengthPercentOfControl = 60;
		((SingleInstrumentBase)digitalReadoutInstrument4).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)digitalReadoutInstrument4).TitleWordWrap = true;
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		scalingLabel1.Alignment = StringAlignment.Far;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)scalingLabel1, 2);
		componentResourceManager.ApplyResources(scalingLabel1, "scalingLabel1");
		scalingLabel1.FontGroup = null;
		scalingLabel1.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabel1).Name = "scalingLabel1";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Coolant_Systems_Pressure_Test");
		((Control)this).Controls.Add((Control)(object)selectablePanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)selectablePanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
