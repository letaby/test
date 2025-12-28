using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Adr;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Nox_Sensor_Drift_Verification__MY20_.panel;

public class UserPanel : CustomPanel
{
	private enum CpcVersion
	{
		Invalid = 0,
		CPC3 = 3,
		CPC5 = 5
	}

	private enum Reason
	{
		TestCompleted,
		FailedServiceExecute,
		Closing,
		Disconnected,
		Canceled,
		DewpointSensorsNotOn,
		NOxSensorDataInvalid,
		TestConditionsNotMet
	}

	private class NOxSensorDriftTest
	{
		private enum TestStep
		{
			Stopped,
			StartTest,
			SetDEFValve,
			StartDPFRegen,
			WaitForRegenToComplete,
			CalculateAndDisplayResults,
			EndTest,
			ResetUserInterface
		}

		public enum TestActivityStatus
		{
			TestInactive,
			TestActive,
			TestShuttingDown
		}

		private UserPanel ParentUserPanel;

		private string ErrorMessage = string.Empty;

		private TestStep CurrentTestStep = TestStep.Stopped;

		public string TestStepMessage = string.Empty;

		public Timer dosingQuantityCheckTimer;

		private DateTime LastSampleTime;

		private int DpfRegenCount = 0;

		private TestActivityStatus testActiveStatus = TestActivityStatus.TestInactive;

		public TestActivityStatus TestActiveStatus
		{
			get
			{
				return testActiveStatus;
			}
			private set
			{
				testActiveStatus = value;
			}
		}

		public NOxSensorDriftTest(UserPanel userPanel)
		{
			ParentUserPanel = userPanel;
			dosingQuantityCheckTimer = new Timer();
			dosingQuantityCheckTimer.Interval = (int)TimeSpan.FromMinutes(4.0).TotalMilliseconds;
		}

		private NOxSensorDriftTest()
		{
		}

		private InstrumentValue GetRecentInstrumentValue(InstrumentValueCollection instrumentValues, DateTime sampleTime)
		{
			InstrumentValue instrumentValue = null;
			InstrumentValue instrumentValue2 = instrumentValues.First();
			if (instrumentValue2 != null && instrumentValue2.FirstSampleTime <= sampleTime)
			{
				instrumentValue = instrumentValues.GetCurrentAtTime(sampleTime);
				if (instrumentValue == null)
				{
					instrumentValue = instrumentValues.Current;
				}
			}
			return instrumentValue;
		}

		public bool TheSensorsWereReady(Channel channel, string dewPointInletEnabledQualifier, string dewPointOutletEnabledQualifier, DateTime referenceTime)
		{
			DateTime dateTime = referenceTime - TimeSpan.FromMinutes(8.0);
			if (channel != null)
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Checking_DewPoint_Sensors_At, dateTime));
				Instrument instrument = channel.Instruments[dewPointInletEnabledQualifier];
				Instrument instrument2 = channel.Instruments[dewPointOutletEnabledQualifier];
				if (instrument != null && instrument2 != null)
				{
					InstrumentValueCollection instrumentValues = instrument.InstrumentValues;
					InstrumentValueCollection instrumentValues2 = instrument2.InstrumentValues;
					if (instrumentValues != null && instrumentValues2 != null)
					{
						InstrumentValue recentInstrumentValue = GetRecentInstrumentValue(instrumentValues, dateTime);
						InstrumentValue recentInstrumentValue2 = GetRecentInstrumentValue(instrumentValues2, dateTime);
						if (recentInstrumentValue != null && recentInstrumentValue2 != null)
						{
							if (recentInstrumentValue.Value != null && recentInstrumentValue2.Value != null)
							{
								double num = Convert.ToDouble(recentInstrumentValue.Value);
								double num2 = Convert.ToDouble(recentInstrumentValue2.Value);
								ReportResult((num > 0.0) ? Resources.Message_DewPoint_Inlet_Sensor_On : Resources.Message_DewPoint_Inlet_Sensor_Off);
								ReportResult((num2 > 0.0) ? Resources.Message_DewPoint_Outlet_Sensor_On : Resources.Message_DewPoint_Outlet_Sensor_Off);
								return num > 0.0 && num2 > 0.0;
							}
						}
						else
						{
							ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_The_Instrument_Had_Value, dewPointInletEnabledQualifier, (recentInstrumentValue != null) ? recentInstrumentValue.Value : Resources.Text_Null, dateTime));
							ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_The_Instrument_Had_Value, dewPointOutletEnabledQualifier, (recentInstrumentValue2 != null) ? recentInstrumentValue2.Value : Resources.Text_Null, dateTime));
						}
					}
				}
			}
			return false;
		}

		public double InstrumentValueAverage(Channel channel, string instrumentQualifier, DateTime firstSampleTime, DateTime lastSampleTime)
		{
			double num = 0.0;
			int num2 = 0;
			double result = double.NaN;
			if (channel != null)
			{
				Instrument instrument = channel.Instruments[instrumentQualifier];
				if (instrument != null)
				{
					InstrumentValueCollection instrumentValues = instrument.InstrumentValues;
					if (instrumentValues != null)
					{
						InstrumentValue recentInstrumentValue = GetRecentInstrumentValue(instrumentValues, firstSampleTime);
						InstrumentValue recentInstrumentValue2 = GetRecentInstrumentValue(instrumentValues, lastSampleTime);
						if (recentInstrumentValue != null && recentInstrumentValue2 != null)
						{
							for (int i = recentInstrumentValue.ItemIndex; i <= recentInstrumentValue2.ItemIndex; i++)
							{
								num += Convert.ToDouble(instrumentValues[i].Value, CultureInfo.InvariantCulture) * (double)instrumentValues[i].ItemSampleCount;
								num2 += instrumentValues[i].ItemSampleCount;
							}
							if (num2 > 0)
							{
								result = num / (double)num2;
							}
						}
					}
				}
			}
			return result;
		}

		private void CalculateAndDisplayResults(DateTime lastSampleTime)
		{
			DateTime firstSampleTime = lastSampleTime - TimeSpan.FromMinutes(3.0);
			double num = InstrumentValueAverage(ParentUserPanel.acm, "DT_AS111_NOx_raw_concentration", firstSampleTime, lastSampleTime);
			double num2 = InstrumentValueAverage(ParentUserPanel.acm, "DT_AS114_NOx_out_concentration", firstSampleTime, lastSampleTime);
			if (!double.IsNaN(num) && !double.IsNaN(num2))
			{
				if (num != 0.0 && num2 != 0.0)
				{
					double num3 = Math.Abs(num - num2);
					bool flag = num3 < 50.0;
					((Control)(object)ParentUserPanel.scalingLabelInstrumentDifference).Text = string.Format(CultureInfo.CurrentCulture, "{0:f3}", num3);
					ParentUserPanel.DisplayResultStatus((ValueState)(flag ? 1 : 3));
					ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Results_Average_Sensor_Values, num, num2));
					ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Results_Sensor_Value_Difference, num3));
					ReportResult(string.Format(CultureInfo.CurrentCulture, flag ? Resources.Message_Results_NOx_Sensor_Values_Are_Consistent : Resources.Message_Results_NOx_Sensor_Values_Are_Not_Consistent));
					ReportResult(string.Format(CultureInfo.CurrentCulture, flag ? Resources.Message_Test_Result_Passed : Resources.Message_Test_Result_Failed));
					((Control)(object)ParentUserPanel.scalingLabelTestResult).Text = (flag ? Resources.Message_Test_Result_Passed : Resources.Message_Test_Result_Failed);
					ParentUserPanel.scalingLabelTestResult.RepresentedState = (ValueState)(flag ? 1 : 3);
					ParentUserPanel.adrReturnValue = flag;
					UserPanel parentUserPanel = ParentUserPanel;
					parentUserPanel.adrReturnMessage = parentUserPanel.adrReturnMessage + string.Format(CultureInfo.CurrentCulture, flag ? Resources.Message_Results_NOx_Sensor_Values_Are_Consistent : Resources.Message_Results_NOx_Sensor_Values_Are_Not_Consistent) + Environment.NewLine;
					ParentUserPanel.UpdateTestReadyStatus();
					AdvanceTestStep();
					TestMain();
				}
				else
				{
					ReportResult(Resources.Error_NOxSensorAverageZero);
					ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Results_Average_Sensor_Values, num, num2));
					EndTest(Reason.NOxSensorDataInvalid);
				}
			}
			else
			{
				ReportResult(string.Format(CultureInfo.CurrentCulture, Resources.Message_Results_Average_Sensor_Values, num, num2));
				EndTest(Reason.NOxSensorDataInvalid);
			}
		}

		private void DPFRegenEnded()
		{
			LastSampleTime = DateTime.Now - TimeSpan.FromSeconds(5.0);
			if (TheSensorsWereReady(ParentUserPanel.acm, "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet", LastSampleTime))
			{
				ReportResult(Resources.Message_The_Sensors_Were_Ready_To_be_Compared);
				AdvanceTestStep();
				TestMain();
				return;
			}
			ReportResult(Resources.Error_The_DPF_Regen_Ended_Before_The_Sensors_Were_Ready);
			if (DpfRegenCount < 1)
			{
				ReportResult(Resources.MessageRunningRegenAgain);
				DpfRegenCount++;
				ReportResult(Resources.Message_Restarting_The_DPF_Regen);
				CurrentTestStep = TestStep.StartDPFRegen;
				StartDPFRegen();
			}
			else
			{
				EndTest(Reason.DewpointSensorsNotOn);
			}
		}

		private void DPFRegenStarted()
		{
			CurrentTestStep = TestStep.WaitForRegenToComplete;
			TestMain();
		}

		public void StartTest()
		{
			DpfRegenCount = 0;
			CurrentTestStep = TestStep.StartTest;
			TestMain();
		}

		private void ReportResult(string text)
		{
			TestStepMessage = text;
			((CustomPanel)ParentUserPanel).LabelLog(ParentUserPanel.seekTimeListViewLog.RequiredUserLabelPrefix, text);
		}

		private static object GetInstrumentCurrentValue(Instrument instrument)
		{
			object result = null;
			if (instrument != null && instrument.InstrumentValues != null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
			{
				result = instrument.InstrumentValues.Current.Value;
			}
			return result;
		}

		public void digitalReadoutInstrumentDPFRegenState_DataChanged(object sender, EventArgs e)
		{
			DPFStatusChanged();
		}

		private void DPFStatusChanged()
		{
			if (TestActiveStatus != TestActivityStatus.TestActive)
			{
				return;
			}
			DataItem dataItem = ((SingleInstrumentBase)ParentUserPanel.digitalReadoutInstrumentDPFRegenState).DataItem;
			if (dataItem != null)
			{
				Instrument instrument = ParentUserPanel.acm.Instruments["DT_AS064_DPF_Regen_State"];
				object instrumentCurrentValue = GetInstrumentCurrentValue(instrument);
				if (CurrentTestStep == TestStep.WaitForRegenToComplete && (instrumentCurrentValue == dataItem.Choices.GetItemFromRawValue(0) || instrumentCurrentValue == dataItem.Choices.GetItemFromRawValue(2)))
				{
					DPFRegenEnded();
				}
				else if (CurrentTestStep < TestStep.WaitForRegenToComplete && (instrumentCurrentValue == dataItem.Choices.GetItemFromRawValue(4) || instrumentCurrentValue == dataItem.Choices.GetItemFromRawValue(8) || instrumentCurrentValue == dataItem.Choices.GetItemFromRawValue(16) || instrumentCurrentValue == dataItem.Choices.GetItemFromRawValue(32)))
				{
					DPFRegenStarted();
				}
			}
		}

		internal void NOxDewpointSensor_RepresentedStateChanged(object sender, EventArgs e)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Invalid comparison between Unknown and I4
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Invalid comparison between Unknown and I4
			if (TestActiveStatus == TestActivityStatus.TestActive)
			{
				bool flag = (int)ParentUserPanel.digitalReadoutInstrumentNOxDewpointInlet.RepresentedState == 1;
				ReportResult(string.Format(flag ? Resources.Message_NOx_Inlet_Sensor_Is_Ready : Resources.Message_NOx_Inlet_Sensor_Is_Not_Ready));
				flag = (int)ParentUserPanel.digitalReadoutInstrumentNOxDewpointOutlet.RepresentedState == 1;
				ReportResult(string.Format(flag ? Resources.Message_NOx_Outlet_Sensor_Is_Ready : Resources.Message_NOx_Outlet_Sensor_Is_Not_Ready));
				ReportResult(Resources.Message_Waiting_For_Regen_To_Complete);
			}
		}

		private void SetDEFValve()
		{
			if (ParentUserPanel.acm != null && ParentUserPanel.acm.Online)
			{
				ErrorMessage = Resources.Error_Failed_to_set_the_DEF_Valve;
				Service service = ((CustomPanel)ParentUserPanel).GetService("ACM301T", "RT_SCR_Dosing_Quantity_Check_Start_Status");
				if (service != null)
				{
					service.InputValues["Desired_Dosing_Quantity"].Value = 0.2;
					service.InputValues["Operation_Time"].Value = 255;
					service.InputValues["Deal_with_interrupted_communication"].Value = service.InputValues["Deal_with_interrupted_communication"].Choices.GetItemFromRawValue(0);
					service.ServiceCompleteEvent += defValveService_ServiceCompleteEvent;
					service.Execute(synchronous: false);
				}
				else
				{
					ReportResult(ErrorMessage);
					EndTest(Reason.FailedServiceExecute);
				}
			}
		}

		private void defValveService_ServiceCompleteEvent(object sender, ResultEventArgs e)
		{
			Service service = sender as Service;
			if (service != null)
			{
				service.ServiceCompleteEvent -= defValveService_ServiceCompleteEvent;
			}
			if (e.Succeeded)
			{
				dosingQuantityCheckTimer.Tick += dosingQuantityCheckTimer_Tick;
				dosingQuantityCheckTimer.Start();
			}
			else
			{
				ReportResult(ErrorMessage);
				EndTest(Reason.FailedServiceExecute);
			}
		}

		private void dosingQuantityCheckTimer_Tick(object sender, EventArgs e)
		{
			StopDosingQuantityCheckTimer();
			ResetDEFValve();
			SetDEFValve();
		}

		public void StopDosingQuantityCheckTimer()
		{
			dosingQuantityCheckTimer.Stop();
			dosingQuantityCheckTimer.Tick -= dosingQuantityCheckTimer_Tick;
		}

		private void StartDPFRegen()
		{
			if (ParentUserPanel.cpc != null && ParentUserPanel.cpc.Online)
			{
				ErrorMessage = Resources.Error_Failed_to_start_DPF_Regen;
				string name = ParentUserPanel.cpc.Ecu.Name;
				string text = string.Empty;
				switch (ParentUserPanel.ConnectedCpcVersion)
				{
				case CpcVersion.CPC3:
					text = "RT_RC0400_DPF_High_Idle_regeneration_Start";
					break;
				case CpcVersion.CPC5:
					text = "RT_DPF_High_Idle_regeneration_Start";
					break;
				}
				Service service = ((CustomPanel)ParentUserPanel).GetService(name, text, (ChannelLookupOptions)5);
				if (service != null)
				{
					service.ServiceCompleteEvent += StartDPFRegen_ServiceCompleteEvent;
					service.Execute(synchronous: false);
				}
				else
				{
					ReportResult(ErrorMessage);
					EndTest(Reason.FailedServiceExecute);
				}
			}
		}

		private void StartDPFRegen_ServiceCompleteEvent(object sender, ResultEventArgs e)
		{
			Service service = sender as Service;
			if (service != null)
			{
				service.ServiceCompleteEvent -= StartDPFRegen_ServiceCompleteEvent;
			}
			if (e.Succeeded && CurrentTestStep == TestStep.StartDPFRegen)
			{
				AdvanceTestStep();
				TestMain();
			}
			else
			{
				ReportResult(ErrorMessage);
				EndTest(Reason.FailedServiceExecute);
			}
		}

		private void ResetDEFValve()
		{
			if (ParentUserPanel.acm == null || !ParentUserPanel.acm.Online)
			{
				return;
			}
			Service service = ((CustomPanel)ParentUserPanel).GetService("ACM301T", "RT_SCR_Dosing_Quantity_Check_Stop_status");
			if (!(service != null))
			{
				return;
			}
			try
			{
				service.Execute(synchronous: true);
			}
			catch (CaesarException ex)
			{
				ReportResult(Resources.Error_Failed_To_Reset_The_DEF_Valve);
				if (ex != null && !string.IsNullOrEmpty(ex.Message))
				{
					ReportResult(string.Format(Resources.Error_Message_Is, ex.Message));
				}
			}
		}

		private void ResetDPFRegen()
		{
			if (ParentUserPanel.cpc == null || !ParentUserPanel.cpc.Online)
			{
				return;
			}
			string text = string.Empty;
			string name = ParentUserPanel.cpc.Ecu.Name;
			switch (ParentUserPanel.ConnectedCpcVersion)
			{
			case CpcVersion.CPC3:
				text = "RT_RC0400_DPF_High_Idle_regeneration_Stop";
				break;
			case CpcVersion.CPC5:
				text = "RT_DPF_High_Idle_regeneration_Stop";
				break;
			}
			Service service = ((CustomPanel)ParentUserPanel).GetService(name, text);
			if (service != null)
			{
				try
				{
					service.Execute(synchronous: true);
					return;
				}
				catch (CaesarException ex)
				{
					ReportResult(Resources.Error_Failed_To_Stop_the_DPF_Regeneration);
					if (ex != null && !string.IsNullOrEmpty(ex.Message))
					{
						ReportResult(string.Format(Resources.Error_Message_Is, ex.Message));
					}
					return;
				}
			}
			ReportResult(Resources.Error_Failed_To_Stop_the_DPF_Regeneration);
			ReportResult(string.Format(Resources.Error_DPF_Regen_Stop_Service_Not_Found, text, name));
		}

		private void ResetTestStep()
		{
			ReportResult(Resources.Message_End_Of_Test);
			TestStepMessage = string.Empty;
			TestActiveStatus = TestActivityStatus.TestInactive;
			ParentUserPanel.UpdateTestReadyStatus();
			CurrentTestStep = TestStep.Stopped;
		}

		private static string GetReasonString(Reason reason)
		{
			return reason switch
			{
				Reason.Canceled => Resources.Message_Canceled, 
				Reason.Closing => Resources.Error_Closing, 
				Reason.DewpointSensorsNotOn => Resources.Error_DewpointSensorsNotOn, 
				Reason.Disconnected => Resources.Error_Disconnected, 
				Reason.FailedServiceExecute => Resources.Error_FailedServiceExecute, 
				Reason.NOxSensorDataInvalid => Resources.Error_NOxSensorReadingsInvalid, 
				Reason.TestCompleted => Resources.Message_Test_Complete, 
				Reason.TestConditionsNotMet => Resources.Error_Test_Not_Ready_To_Be_Run, 
				_ => null, 
			};
		}

		public void EndTest(Reason reason)
		{
			TestActiveStatus = TestActivityStatus.TestShuttingDown;
			StopDosingQuantityCheckTimer();
			ReportResult(Resources.Message_ResettingTheDEFValve);
			ResetDEFValve();
			ReportResult(Resources.Message_Stopping_the_DPF_Regen);
			ResetDPFRegen();
			ReportResult(GetReasonString(reason));
			UserPanel parentUserPanel = ParentUserPanel;
			parentUserPanel.adrReturnMessage = parentUserPanel.adrReturnMessage + GetReasonString(reason) + Environment.NewLine;
			if (reason != Reason.TestCompleted)
			{
				((Control)(object)ParentUserPanel.scalingLabelTestResult).Text = Resources.Message_Test_Result_Test_Aborted;
				ParentUserPanel.scalingLabelTestResult.RepresentedState = (ValueState)2;
			}
			CurrentTestStep = TestStep.ResetUserInterface;
			TestMain();
		}

		private void AdvanceTestStep()
		{
			if (CurrentTestStep != TestStep.Stopped)
			{
				CurrentTestStep++;
			}
			else
			{
				TestActiveStatus = TestActivityStatus.TestInactive;
			}
		}

		private void TestMain()
		{
			switch (CurrentTestStep)
			{
			case TestStep.Stopped:
				break;
			case TestStep.StartTest:
				ReportResult(Resources.Message_Starting_NOx_Sensor_Calibration_Drift_Test);
				TestActiveStatus = TestActivityStatus.TestActive;
				AdvanceTestStep();
				TestMain();
				break;
			case TestStep.SetDEFValve:
				ReportResult(Resources.Message_Setting_the_DEF_valve);
				SetDEFValve();
				AdvanceTestStep();
				TestMain();
				break;
			case TestStep.StartDPFRegen:
				ReportResult(Resources.Message_Starting_DPF_Regen);
				StartDPFRegen();
				break;
			case TestStep.WaitForRegenToComplete:
				ReportResult(Resources.Message_Waiting_For_Regen_To_Complete);
				break;
			case TestStep.CalculateAndDisplayResults:
				CalculateAndDisplayResults(LastSampleTime);
				break;
			case TestStep.EndTest:
				EndTest(Reason.TestCompleted);
				break;
			case TestStep.ResetUserInterface:
				ResetTestStep();
				break;
			}
		}
	}

	private const string MCMName = "MCM21T";

	private const string ACMName = "ACM301T";

	private Channel acm = null;

	private Channel mcm = null;

	private Channel cpc = null;

	private WarningManager warningManager;

	private NOxSensorDriftTest NOxSensorTest;

	private Timer ignitionTimer;

	private TableLayoutPanel tableLayoutPanelPreReqs;

	private TableLayoutPanel tableLayoutPanelTestControls;

	private TableLayoutPanel tableLayoutPanelMain;

	private SeekTimeListView seekTimeListViewLog;

	private TableLayoutPanel tableLayoutPanelInstruments;

	private BarInstrument barInstrumentNOxRaw;

	private BarInstrument barInstrumentNOxOut;

	private BarInstrument barInstrument1;

	private BarInstrument barInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineSpeed;

	private Button buttonStart;

	private Checkmark checkmarkStatus;

	private System.Windows.Forms.Label labelStatus;

	private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;

	private TableLayoutPanel tableLayoutPanelDifferenceInstrument;

	private Label labelInstrumentDifferenceTitle;

	private ScalingLabel scalingLabelInstrumentDifference;

	private BarInstrument barInstrument7;

	private BarInstrument barInstrument8;

	private BarInstrument barInstrument9;

	private DigitalReadoutInstrument digitalReadoutInstrumentNOxDewpointInlet;

	private DigitalReadoutInstrument digitalReadoutInstrumentNeutralSwitch;

	private Button buttonStop;

	private Button buttonClose;

	private DigitalReadoutInstrument digitalReadoutInstrumentNOxDewpointOutlet;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrumentVehicleCheckStatus;

	private ScalingLabel scalingLabelTestResult;

	private DigitalReadoutInstrument digitalReadoutInstrumentDPFRegenState;

	private TableLayoutPanel tableLayoutPanelNoxHours;

	private DigitalReadoutInstrument digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime;

	private DigitalReadoutInstrument digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime;

	private BarInstrument barInstrument4;

	private TableLayoutPanel tableLayoutPanelThermometers;

	internal bool adrReturnValue { get; set; }

	internal string adrReturnMessage { get; set; }

	private CpcVersion ConnectedCpcVersion { get; set; }

	public bool CanClose => NOxSensorTest.TestActiveStatus != NOxSensorDriftTest.TestActivityStatus.TestActive;

	private bool TestPreconditionsMet => (int)digitalReadoutInstrumentEngineSpeed.RepresentedState == 1 && (int)digitalReadoutInstrumentVehicleCheckStatus.RepresentedState == 1;

	public bool DewpointSensorsReady => (int)digitalReadoutInstrumentNOxDewpointInlet.RepresentedState == 1 && (int)digitalReadoutInstrumentNOxDewpointOutlet.RepresentedState == 1;

	public static bool IsMediumDuty { get; set; }

	private bool AreAllChannelsOnline => IsChannelOnline(cpc) && IsChannelOnline(mcm) && IsChannelOnline(acm);

	public UserPanel()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		NOxSensorTest = new NOxSensorDriftTest(this);
		InitializeComponent();
		ignitionTimer = new Timer();
		ignitionTimer.Interval = 10000;
		ignitionTimer.Tick += OnTimerTick;
		ConnectionManager.GlobalInstance.PropertyChanged += GlobalInstance_PropertyChanged;
		warningManager = new WarningManager(Resources.WarningManagerMessage, Resources.Message_NOx_Sensor_Test, seekTimeListViewLog.RequiredUserLabelPrefix);
		StartIgnitionTimer();
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing)
			{
				NOxSensorTest.StopDosingQuantityCheckTimer();
				NOxSensorTest.dosingQuantityCheckTimer.Dispose();
				ignitionTimer.Dispose();
				ignitionTimer = null;
			}
		}
		finally
		{
			((CustomPanel)this).Dispose(disposing);
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		digitalReadoutInstrumentNOxDewpointInlet.RepresentedStateChanged += NOxSensorTest.NOxDewpointSensor_RepresentedStateChanged;
		digitalReadoutInstrumentNOxDewpointOutlet.RepresentedStateChanged += NOxSensorTest.NOxDewpointSensor_RepresentedStateChanged;
		((SingleInstrumentBase)digitalReadoutInstrumentDPFRegenState).DataChanged += NOxSensorTest.digitalReadoutInstrumentDPFRegenState_DataChanged;
		UpdateChannels();
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			ConnectionManager.GlobalInstance.PropertyChanged -= GlobalInstance_PropertyChanged;
			if (ignitionTimer != null)
			{
				ignitionTimer.Stop();
				ignitionTimer.Tick -= OnTimerTick;
			}
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			digitalReadoutInstrumentNOxDewpointInlet.RepresentedStateChanged -= NOxSensorTest.NOxDewpointSensor_RepresentedStateChanged;
			digitalReadoutInstrumentNOxDewpointOutlet.RepresentedStateChanged -= NOxSensorTest.NOxDewpointSensor_RepresentedStateChanged;
			((SingleInstrumentBase)digitalReadoutInstrumentDPFRegenState).DataChanged -= NOxSensorTest.digitalReadoutInstrumentDPFRegenState_DataChanged;
			CleanUpChannels();
			((Control)this).Tag = new object[2] { adrReturnValue, adrReturnMessage };
		}
	}

	private void UpdateTestReadyStatus()
	{
		if (NOxSensorTest.TestActiveStatus == NOxSensorDriftTest.TestActivityStatus.TestActive)
		{
			if (TestPreconditionsMet)
			{
				labelStatus.Text = NOxSensorTest.TestStepMessage;
				checkmarkStatus.Checked = true;
			}
			else
			{
				StopWork(Reason.TestConditionsNotMet);
			}
		}
		else if (NOxSensorTest.TestActiveStatus == NOxSensorDriftTest.TestActivityStatus.TestInactive)
		{
			checkmarkStatus.Checked = TestPreconditionsMet;
			if (TestPreconditionsMet)
			{
				labelStatus.Text = Resources.Message_Test_Ready_To_Be_Run;
			}
			else
			{
				labelStatus.Text = Resources.Error_Test_Not_Ready_To_Be_Run;
			}
		}
	}

	private void UpdateUserInterface()
	{
		if (NOxSensorTest != null)
		{
			UpdateTestReadyStatus();
			buttonStart.Enabled = NOxSensorTest.TestActiveStatus == NOxSensorDriftTest.TestActivityStatus.TestInactive && TestPreconditionsMet && AreAllChannelsOnline;
			buttonStop.Enabled = NOxSensorTest.TestActiveStatus == NOxSensorDriftTest.TestActivityStatus.TestActive;
			buttonClose.Enabled = CanClose;
		}
	}

	private void ClearResults()
	{
		((Control)(object)scalingLabelInstrumentDifference).Text = string.Empty;
		scalingLabelInstrumentDifference.RepresentedState = (ValueState)0;
		((Control)(object)scalingLabelTestResult).Text = string.Empty;
		scalingLabelTestResult.RepresentedState = (ValueState)0;
	}

	private bool UpdateChannels()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		flag = SetCPC(((CustomPanel)this).GetChannel("CPC302T", (ChannelLookupOptions)5));
		flag2 = SetMCM(GetActiveChannel("MCM21T"));
		flag3 = SetACM(GetActiveChannel("ACM301T"));
		return flag || flag2 || flag3;
	}

	private void CleanUpChannels()
	{
		SetCPC(null);
		SetMCM(null);
		SetACM(null);
	}

	private void StopWork(Reason reason)
	{
		NOxSensorTest.EndTest(reason);
	}

	private void buttonStart_Click(object sender, EventArgs e)
	{
		if (warningManager.RequestContinue())
		{
			ClearResults();
			adrReturnValue = false;
			adrReturnMessage = string.Empty;
			NOxSensorTest.StartTest();
		}
	}

	private void buttonStop_Click(object sender, EventArgs e)
	{
		NOxSensorTest.EndTest(Reason.Canceled);
	}

	private void DisplayResultStatus(ValueState state)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		scalingLabelInstrumentDifference.RepresentedState = state;
		((Control)(object)scalingLabelInstrumentDifference).Refresh();
	}

	private void OnTimerTick(object sender, EventArgs e)
	{
		ignitionTimer.Stop();
		((Control)(object)digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).Visible = digitalReadoutInstrumentNOxDewpointInlet != null && ((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointInlet).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointInlet).DataItem.ValueAsDouble(((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointInlet).DataItem.Value) == 0.0;
		((Control)(object)digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).Visible = digitalReadoutInstrumentNOxDewpointOutlet != null && ((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointOutlet).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointOutlet).DataItem.ValueAsDouble(((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointOutlet).DataItem.Value) == 0.0;
	}

	private void StartIgnitionTimer()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (ignitionTimer != null)
		{
			if ((int)ConnectionManager.GlobalInstance.IgnitionStatus == 0 && acm != null)
			{
				ignitionTimer.Start();
			}
			else
			{
				ignitionTimer.Stop();
			}
		}
	}

	private void GlobalInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		StartIgnitionTimer();
	}

	public override void OnChannelsChanged()
	{
		if (UpdateChannels())
		{
			UpdateConnectedEquipmentType();
			ClearResults();
			UpdateUserInterface();
		}
	}

	private void TestPrerequisite_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private static bool IsMediumDutyEquipment(EquipmentType equipment)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (equipment != EquipmentType.Empty)
		{
			switch (((EquipmentType)(ref equipment)).Name.ToUpperInvariant())
			{
			case "DD5":
			case "DD8":
			case "MDEG 4-CYLINDER TIER4":
			case "MDEG 6-CYLINDER TIER4":
				return true;
			}
		}
		return false;
	}

	private static void UpdateConnectedEquipmentType()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EquipmentType equipment = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault(delegate(EquipmentType et)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			ElectronicsFamily family = ((EquipmentType)(ref et)).Family;
			return ((ElectronicsFamily)(ref family)).Category.Equals("Engine", StringComparison.OrdinalIgnoreCase);
		});
		IsMediumDuty = IsMediumDutyEquipment(equipment);
	}

	private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
	{
		if (e.Category.Equals("Engine", StringComparison.OrdinalIgnoreCase))
		{
			UpdateConnectedEquipmentType();
		}
	}

	private static Channel GetActiveChannel(string channelName)
	{
		return SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel ac) => ac.Ecu.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
	}

	private bool SetCPC(Channel cpc)
	{
		if (this.cpc != cpc)
		{
			warningManager.Reset();
			if (NOxSensorTest.TestActiveStatus == NOxSensorDriftTest.TestActivityStatus.TestActive)
			{
				StopWork(Reason.Disconnected);
			}
			if (this.cpc != null)
			{
				this.cpc.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			ConnectedCpcVersion = CpcVersion.Invalid;
			this.cpc = cpc;
			if (this.cpc != null)
			{
				if (string.Equals(this.cpc.Ecu.Name, "CPC302T", StringComparison.OrdinalIgnoreCase))
				{
					ConnectedCpcVersion = CpcVersion.CPC3;
				}
				else if (string.Equals(this.cpc.Ecu.Name, "CPC501T", StringComparison.OrdinalIgnoreCase) || string.Equals(this.cpc.Ecu.Name, "CPC502T", StringComparison.OrdinalIgnoreCase))
				{
					ConnectedCpcVersion = CpcVersion.CPC5;
				}
				this.cpc.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
			return true;
		}
		return false;
	}

	private bool SetMCM(Channel mcm)
	{
		if (this.mcm != mcm)
		{
			warningManager.Reset();
			if (NOxSensorTest.TestActiveStatus == NOxSensorDriftTest.TestActivityStatus.TestActive)
			{
				StopWork(Reason.Disconnected);
			}
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			this.mcm = mcm;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
			return true;
		}
		return false;
	}

	private bool SetACM(Channel acm)
	{
		if (this.acm != acm)
		{
			warningManager.Reset();
			if (NOxSensorTest.TestActiveStatus == NOxSensorDriftTest.TestActivityStatus.TestActive)
			{
				StopWork(Reason.Disconnected);
			}
			if (this.acm != null)
			{
				this.acm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			this.acm = acm;
			if (this.acm != null)
			{
				this.acm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
				StartIgnitionTimer();
			}
			return true;
		}
		return false;
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	public static bool IsChannelOnline(Channel channel)
	{
		return channel != null && channel.CommunicationsState == CommunicationsState.Online;
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
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
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
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0902: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1038: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1174: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1236: Unknown result type (might be due to invalid IL or missing references)
		//IL_132b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1559: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelPreReqs = new TableLayoutPanel();
		digitalReadoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		digitalReadoutInstrumentDPFRegenState = new DigitalReadoutInstrument();
		digitalReadoutInstrumentNeutralSwitch = new DigitalReadoutInstrument();
		digitalReadoutInstrumentVehicleCheckStatus = new DigitalReadoutInstrument();
		digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
		digitalReadoutInstrumentNOxDewpointInlet = new DigitalReadoutInstrument();
		checkmarkStatus = new Checkmark();
		barInstrumentNOxRaw = new BarInstrument();
		barInstrumentNOxOut = new BarInstrument();
		labelStatus = new System.Windows.Forms.Label();
		seekTimeListViewLog = new SeekTimeListView();
		tableLayoutPanelTestControls = new TableLayoutPanel();
		buttonStart = new Button();
		buttonStop = new Button();
		barInstrument1 = new BarInstrument();
		barInstrument2 = new BarInstrument();
		tableLayoutPanelMain = new TableLayoutPanel();
		tableLayoutPanelInstruments = new TableLayoutPanel();
		tableLayoutPanelNoxHours = new TableLayoutPanel();
		digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime = new DigitalReadoutInstrument();
		digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime = new DigitalReadoutInstrument();
		tableLayoutPanelThermometers = new TableLayoutPanel();
		barInstrument4 = new BarInstrument();
		barInstrument7 = new BarInstrument();
		barInstrument8 = new BarInstrument();
		barInstrument9 = new BarInstrument();
		digitalReadoutInstrumentNOxDewpointOutlet = new DigitalReadoutInstrument();
		tableLayoutPanelDifferenceInstrument = new TableLayoutPanel();
		labelInstrumentDifferenceTitle = new Label();
		scalingLabelInstrumentDifference = new ScalingLabel();
		tableLayoutPanel1 = new TableLayoutPanel();
		buttonClose = new Button();
		scalingLabelTestResult = new ScalingLabel();
		((Control)(object)tableLayoutPanelPreReqs).SuspendLayout();
		((Control)(object)tableLayoutPanelTestControls).SuspendLayout();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelInstruments).SuspendLayout();
		((Control)(object)tableLayoutPanelNoxHours).SuspendLayout();
		((Control)(object)tableLayoutPanelThermometers).SuspendLayout();
		((Control)(object)tableLayoutPanelDifferenceInstrument).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelPreReqs, "tableLayoutPanelPreReqs");
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).SetColumnSpan((Control)(object)tableLayoutPanelPreReqs, 4);
		((TableLayoutPanel)(object)tableLayoutPanelPreReqs).Controls.Add((Control)(object)digitalReadoutInstrumentEngineSpeed, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelPreReqs).Controls.Add((Control)(object)digitalReadoutInstrumentDPFRegenState, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanelPreReqs).Controls.Add((Control)(object)digitalReadoutInstrumentNeutralSwitch, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelPreReqs).Controls.Add((Control)(object)digitalReadoutInstrumentVehicleCheckStatus, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelPreReqs).Controls.Add((Control)(object)digitalReadoutInstrumentParkingBrake, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelPreReqs).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanelPreReqs).Name = "tableLayoutPanelPreReqs";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineSpeed, "digitalReadoutInstrumentEngineSpeed");
		digitalReadoutInstrumentEngineSpeed.FontGroup = "prereqs";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).FreezeValue = false;
		digitalReadoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)digitalReadoutInstrumentEngineSpeed).Name = "digitalReadoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentEngineSpeed.RepresentedStateChanged += TestPrerequisite_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentDPFRegenState, "digitalReadoutInstrumentDPFRegenState");
		digitalReadoutInstrumentDPFRegenState.FontGroup = "";
		((SingleInstrumentBase)digitalReadoutInstrumentDPFRegenState).FreezeValue = false;
		digitalReadoutInstrumentDPFRegenState.Gradient.Initialize((ValueState)0, 6);
		digitalReadoutInstrumentDPFRegenState.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrumentDPFRegenState.Gradient.Modify(2, 2.0, (ValueState)1);
		digitalReadoutInstrumentDPFRegenState.Gradient.Modify(3, 4.0, (ValueState)2);
		digitalReadoutInstrumentDPFRegenState.Gradient.Modify(4, 8.0, (ValueState)2);
		digitalReadoutInstrumentDPFRegenState.Gradient.Modify(5, 16.0, (ValueState)2);
		digitalReadoutInstrumentDPFRegenState.Gradient.Modify(6, 32.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentDPFRegenState).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS064_DPF_Regen_State");
		((Control)(object)digitalReadoutInstrumentDPFRegenState).Name = "digitalReadoutInstrumentDPFRegenState";
		((SingleInstrumentBase)digitalReadoutInstrumentDPFRegenState).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentNeutralSwitch, "digitalReadoutInstrumentNeutralSwitch");
		digitalReadoutInstrumentNeutralSwitch.FontGroup = "prereqs";
		((SingleInstrumentBase)digitalReadoutInstrumentNeutralSwitch).FreezeValue = false;
		digitalReadoutInstrumentNeutralSwitch.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentNeutralSwitch.Gradient.Modify(1, 1.0, (ValueState)1);
		digitalReadoutInstrumentNeutralSwitch.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentNeutralSwitch).Instrument = new Qualifier((QualifierTypes)1, "virtual", "NeutralSwitch");
		((Control)(object)digitalReadoutInstrumentNeutralSwitch).Name = "digitalReadoutInstrumentNeutralSwitch";
		((SingleInstrumentBase)digitalReadoutInstrumentNeutralSwitch).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentVehicleCheckStatus, "digitalReadoutInstrumentVehicleCheckStatus");
		digitalReadoutInstrumentVehicleCheckStatus.FontGroup = "prereqs";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheckStatus).FreezeValue = false;
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
		((Control)(object)digitalReadoutInstrumentVehicleCheckStatus).Name = "digitalReadoutInstrumentVehicleCheckStatus";
		((SingleInstrumentBase)digitalReadoutInstrumentVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrumentVehicleCheckStatus.RepresentedStateChanged += TestPrerequisite_RepresentedStateChanged;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
		digitalReadoutInstrumentParkingBrake.FontGroup = "prereqs";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).FreezeValue = false;
		digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 1.0, (ValueState)1);
		digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 2.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
		((SingleInstrumentBase)digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentNOxDewpointInlet, "digitalReadoutInstrumentNOxDewpointInlet");
		digitalReadoutInstrumentNOxDewpointInlet.FontGroup = "small";
		((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointInlet).FreezeValue = false;
		digitalReadoutInstrumentNOxDewpointInlet.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentNOxDewpointInlet.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentNOxDewpointInlet.Gradient.Initialize((ValueState)0, 1);
		digitalReadoutInstrumentNOxDewpointInlet.Gradient.Modify(1, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointInlet).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS105_NOx_Sensor_Dewpoint_enabled_Inlet");
		((Control)(object)digitalReadoutInstrumentNOxDewpointInlet).Name = "digitalReadoutInstrumentNOxDewpointInlet";
		((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointInlet).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointInlet).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(barInstrumentNOxRaw, "barInstrumentNOxRaw");
		barInstrumentNOxRaw.FontGroup = "ShortBar";
		((SingleInstrumentBase)barInstrumentNOxRaw).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentNOxRaw).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS111_NOx_raw_concentration");
		((Control)(object)barInstrumentNOxRaw).Name = "barInstrumentNOxRaw";
		((AxisSingleInstrumentBase)barInstrumentNOxRaw).PreferredAxisRange = new AxisRange(0.0, 500.0, (string)null);
		((SingleInstrumentBase)barInstrumentNOxRaw).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barInstrumentNOxOut, "barInstrumentNOxOut");
		barInstrumentNOxOut.FontGroup = "ShortBar";
		((SingleInstrumentBase)barInstrumentNOxOut).FreezeValue = false;
		((SingleInstrumentBase)barInstrumentNOxOut).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS114_NOx_out_concentration");
		((Control)(object)barInstrumentNOxOut).Name = "barInstrumentNOxOut";
		((AxisSingleInstrumentBase)barInstrumentNOxOut).PreferredAxisRange = new AxisRange(0.0, 500.0, (string)null);
		((SingleInstrumentBase)barInstrumentNOxOut).UnitAlignment = StringAlignment.Near;
		labelStatus.BackColor = SystemColors.Control;
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(seekTimeListViewLog, "seekTimeListViewLog");
		seekTimeListViewLog.FilterUserLabels = true;
		((Control)(object)seekTimeListViewLog).Name = "seekTimeListViewLog";
		seekTimeListViewLog.RequiredUserLabelPrefix = "NoxSensorDrift";
		seekTimeListViewLog.SelectedTime = null;
		seekTimeListViewLog.ShowChannelLabels = false;
		seekTimeListViewLog.ShowCommunicationsState = false;
		seekTimeListViewLog.ShowControlPanel = false;
		seekTimeListViewLog.ShowDeviceColumn = false;
		seekTimeListViewLog.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		componentResourceManager.ApplyResources(tableLayoutPanelTestControls, "tableLayoutPanelTestControls");
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add((Control)(object)checkmarkStatus, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(labelStatus, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(buttonStart, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add((Control)(object)tableLayoutPanelPreReqs, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTestControls).Controls.Add(buttonStop, 2, 1);
		((Control)(object)tableLayoutPanelTestControls).Name = "tableLayoutPanelTestControls";
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetRowSpan((Control)(object)tableLayoutPanelTestControls, 3);
		componentResourceManager.ApplyResources(buttonStart, "buttonStart");
		buttonStart.Name = "buttonStart";
		buttonStart.UseCompatibleTextRendering = true;
		buttonStart.UseVisualStyleBackColor = true;
		buttonStart.Click += buttonStart_Click;
		componentResourceManager.ApplyResources(buttonStop, "buttonStop");
		buttonStop.Name = "buttonStop";
		buttonStop.UseCompatibleTextRendering = true;
		buttonStop.UseVisualStyleBackColor = true;
		buttonStop.Click += buttonStop_Click;
		componentResourceManager.ApplyResources(barInstrument1, "barInstrument1");
		barInstrument1.FontGroup = "ShortBar";
		((SingleInstrumentBase)barInstrument1).FreezeValue = false;
		((SingleInstrumentBase)barInstrument1).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS036_SCR_Inlet_NOx_Sensor");
		((Control)(object)barInstrument1).Name = "barInstrument1";
		((AxisSingleInstrumentBase)barInstrument1).PreferredAxisRange = new AxisRange(0.0, 500.0, (string)null);
		((SingleInstrumentBase)barInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(barInstrument2, "barInstrument2");
		barInstrument2.FontGroup = "horizontalBar";
		((SingleInstrumentBase)barInstrument2).FreezeValue = false;
		((SingleInstrumentBase)barInstrument2).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS035_SCR_Outlet_NOx_Sensor");
		((Control)(object)barInstrument2).Name = "barInstrument2";
		((AxisSingleInstrumentBase)barInstrument2).PreferredAxisRange = new AxisRange(0.0, 500.0, (string)null);
		((SingleInstrumentBase)barInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)seekTimeListViewLog, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelInstruments, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelTestControls, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanel1, 0, 5);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(tableLayoutPanelInstruments, "tableLayoutPanelInstruments");
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)tableLayoutPanelNoxHours, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)tableLayoutPanelThermometers, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)barInstrument2, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)barInstrument1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentNOxDewpointOutlet, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)barInstrumentNOxRaw, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)digitalReadoutInstrumentNOxDewpointInlet, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)barInstrumentNOxOut, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).Controls.Add((Control)(object)tableLayoutPanelDifferenceInstrument, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanelInstruments).Name = "tableLayoutPanelInstruments";
		componentResourceManager.ApplyResources(tableLayoutPanelNoxHours, "tableLayoutPanelNoxHours");
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetColumnSpan((Control)(object)tableLayoutPanelNoxHours, 2);
		((TableLayoutPanel)(object)tableLayoutPanelNoxHours).Controls.Add((Control)(object)digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelNoxHours).Controls.Add((Control)(object)digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime, 0, 0);
		((Control)(object)tableLayoutPanelNoxHours).Name = "tableLayoutPanelNoxHours";
		componentResourceManager.ApplyResources(digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime, "digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime");
		digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime.FontGroup = "small";
		((SingleInstrumentBase)digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).Instrument = new Qualifier((QualifierTypes)4, "ACM301T", "e2p_nox_out_dia_sens_runtime");
		((Control)(object)digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).Name = "digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime";
		((SingleInstrumentBase)digitalReadoutInstrument_e2p_nox_out_dia_sens_runtime).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime, "digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime");
		digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime.FontGroup = "small";
		((SingleInstrumentBase)digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).Instrument = new Qualifier((QualifierTypes)4, "ACM301T", "e2p_nox_raw_dia_sens_runtime");
		((Control)(object)digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).Name = "digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime";
		((SingleInstrumentBase)digitalReadoutInstrument_e2p_nox_raw_dia_sens_runtime).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelThermometers, "tableLayoutPanelThermometers");
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)barInstrument4, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)barInstrument7, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)barInstrument8, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelThermometers).Controls.Add((Control)(object)barInstrument9, 2, 0);
		((Control)(object)tableLayoutPanelThermometers).Name = "tableLayoutPanelThermometers";
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetRowSpan((Control)(object)tableLayoutPanelThermometers, 5);
		barInstrument4.BarOrientation = (ControlOrientation)1;
		barInstrument4.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument4, "barInstrument4");
		barInstrument4.FontGroup = "ThermometerBar";
		((SingleInstrumentBase)barInstrument4).FreezeValue = false;
		((SingleInstrumentBase)barInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS019_SCR_Outlet_Temperature");
		((Control)(object)barInstrument4).Name = "barInstrument4";
		((SingleInstrumentBase)barInstrument4).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument4).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument4).UnitAlignment = StringAlignment.Near;
		barInstrument7.BarOrientation = (ControlOrientation)1;
		barInstrument7.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument7, "barInstrument7");
		barInstrument7.FontGroup = "ThermometerBar";
		((SingleInstrumentBase)barInstrument7).FreezeValue = false;
		((SingleInstrumentBase)barInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS007_DOC_Inlet_Temperature");
		((Control)(object)barInstrument7).Name = "barInstrument7";
		((SingleInstrumentBase)barInstrument7).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument7).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument7).UnitAlignment = StringAlignment.Near;
		barInstrument8.BarOrientation = (ControlOrientation)1;
		barInstrument8.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument8, "barInstrument8");
		barInstrument8.FontGroup = "ThermometerBar";
		((SingleInstrumentBase)barInstrument8).FreezeValue = false;
		((SingleInstrumentBase)barInstrument8).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS008_DOC_Outlet_Temperature");
		((Control)(object)barInstrument8).Name = "barInstrument8";
		((AxisSingleInstrumentBase)barInstrument8).PreferredAxisRange = new AxisRange(-250.0, 1000.0, (string)null);
		((SingleInstrumentBase)barInstrument8).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument8).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument8).UnitAlignment = StringAlignment.Near;
		barInstrument9.BarOrientation = (ControlOrientation)1;
		barInstrument9.BarStyle = (ControlStyle)1;
		componentResourceManager.ApplyResources(barInstrument9, "barInstrument9");
		barInstrument9.FontGroup = "ThermometerBar";
		((SingleInstrumentBase)barInstrument9).FreezeValue = false;
		((SingleInstrumentBase)barInstrument9).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS009_DPF_Outlet_Temperature");
		((Control)(object)barInstrument9).Name = "barInstrument9";
		((SingleInstrumentBase)barInstrument9).TitleOrientation = (TextOrientation)0;
		((SingleInstrumentBase)barInstrument9).TitlePosition = (LabelPosition)3;
		((SingleInstrumentBase)barInstrument9).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentNOxDewpointOutlet, "digitalReadoutInstrumentNOxDewpointOutlet");
		digitalReadoutInstrumentNOxDewpointOutlet.FontGroup = "small";
		((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointOutlet).FreezeValue = false;
		digitalReadoutInstrumentNOxDewpointOutlet.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentNOxDewpointOutlet.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentNOxDewpointOutlet.Gradient.Initialize((ValueState)0, 1);
		digitalReadoutInstrumentNOxDewpointOutlet.Gradient.Modify(1, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointOutlet).Instrument = new Qualifier((QualifierTypes)1, "ACM301T", "DT_AS106_NOx_Sensor_Dewpoint_enabled_Outlet");
		((Control)(object)digitalReadoutInstrumentNOxDewpointOutlet).Name = "digitalReadoutInstrumentNOxDewpointOutlet";
		((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointOutlet).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentNOxDewpointOutlet).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanelDifferenceInstrument, "tableLayoutPanelDifferenceInstrument");
		((TableLayoutPanel)(object)tableLayoutPanelDifferenceInstrument).Controls.Add((Control)(object)labelInstrumentDifferenceTitle, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelDifferenceInstrument).Controls.Add((Control)(object)scalingLabelInstrumentDifference, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelDifferenceInstrument).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanelDifferenceInstrument).Name = "tableLayoutPanelDifferenceInstrument";
		((TableLayoutPanel)(object)tableLayoutPanelInstruments).SetRowSpan((Control)(object)tableLayoutPanelDifferenceInstrument, 2);
		labelInstrumentDifferenceTitle.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelInstrumentDifferenceTitle, "labelInstrumentDifferenceTitle");
		((Control)(object)labelInstrumentDifferenceTitle).Name = "labelInstrumentDifferenceTitle";
		labelInstrumentDifferenceTitle.Orientation = (TextOrientation)1;
		scalingLabelInstrumentDifference.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelInstrumentDifference, "scalingLabelInstrumentDifference");
		scalingLabelInstrumentDifference.FontGroup = null;
		scalingLabelInstrumentDifference.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelInstrumentDifference).Name = "scalingLabelInstrumentDifference";
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)scalingLabelTestResult, 0, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		scalingLabelTestResult.Alignment = StringAlignment.Center;
		componentResourceManager.ApplyResources(scalingLabelTestResult, "scalingLabelTestResult");
		scalingLabelTestResult.FontGroup = null;
		scalingLabelTestResult.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelTestResult).Name = "scalingLabelTestResult";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_NOx_Sensor_Verification");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelPreReqs).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelTestControls).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInstruments).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelNoxHours).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelThermometers).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelDifferenceInstrument).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
