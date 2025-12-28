using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel;

internal class InnerTest
{
	private enum InnerTestSteps
	{
		Start = 0,
		SetDesiredClutchEngagementStart = 1,
		SetDesiredClutchEngagementRequestStatus = 2,
		ShutoffClutchValveStart = 3,
		ShutoffClutchValveResultsStatus = 4,
		StartMonitor = 5,
		EndMonitor = 6,
		ShutoffClutchValveResultsStop = 7,
		SetDesiredClutchEngagementStop = 8,
		End = 9,
		Cleanup = 7
	}

	private enum RequestStatusServiceResponses
	{
		SuccessfullyRanToCompletion,
		InProcess,
		ReturnedWithoutResult
	}

	private const float ClutchStateParameterClosed = 0f;

	private const float ClutchStateParameterOpen = 100f;

	private const string SetDesiredClutchEngagementStart = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start";

	private const string SetDesiredClutchEngagementStop = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop";

	private const string SetDesiredClutchEngagementRequestPosition = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Aktuelle_Position_Kupplung";

	private const string SetDesiredClutchEngagementRequestStatus = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status";

	private const string ShutoffClutchValveStart = "RT_052C_Kupplungsventile_abschalten_Start";

	private const string ShutoffClutchValveStop = "RT_052C_Kupplungsventile_abschalten_Stop";

	private const string ShutoffClutchValveRequestStatus = "RT_052C_Kupplungsventile_abschalten_Request_Results_Routine_Status";

	private float clutchStateParameter = 0f;

	private Channel tcm;

	private Action<string> DisplayDirections;

	private Action<TestResults, string> NotifyComplete;

	private BarInstrument barInstrumentClutchDisplacement;

	private BarInstrument barInstrumentCounterShaftSpeed;

	private TimerControl displayTimerMonitorInstrument;

	private Timer timerSetDesiredClutchEngagementRequestStatus = null;

	private Timer timerShutoffClutchValveRequestStatus = null;

	private string errorString;

	private int testNumber;

	private TestResults testResults = TestResults.NotRun;

	private BarInstrument testInstrument;

	private InnerTestSteps[] testSteps;

	private static InnerTestSteps[] ImpermeabilityClosedSteps = new InnerTestSteps[10]
	{
		InnerTestSteps.Start,
		InnerTestSteps.SetDesiredClutchEngagementStart,
		InnerTestSteps.SetDesiredClutchEngagementRequestStatus,
		InnerTestSteps.ShutoffClutchValveStart,
		InnerTestSteps.ShutoffClutchValveResultsStatus,
		InnerTestSteps.StartMonitor,
		InnerTestSteps.EndMonitor,
		InnerTestSteps.ShutoffClutchValveResultsStop,
		InnerTestSteps.SetDesiredClutchEngagementStop,
		InnerTestSteps.End
	};

	private static InnerTestSteps[] ImpermeabilityOpenSteps = new InnerTestSteps[10]
	{
		InnerTestSteps.Start,
		InnerTestSteps.SetDesiredClutchEngagementStart,
		InnerTestSteps.SetDesiredClutchEngagementRequestStatus,
		InnerTestSteps.ShutoffClutchValveStart,
		InnerTestSteps.ShutoffClutchValveResultsStatus,
		InnerTestSteps.StartMonitor,
		InnerTestSteps.EndMonitor,
		InnerTestSteps.ShutoffClutchValveResultsStop,
		InnerTestSteps.SetDesiredClutchEngagementStop,
		InnerTestSteps.End
	};

	private static InnerTestSteps[] DisengagementSteps = new InnerTestSteps[7]
	{
		InnerTestSteps.Start,
		InnerTestSteps.SetDesiredClutchEngagementStart,
		InnerTestSteps.SetDesiredClutchEngagementRequestStatus,
		InnerTestSteps.StartMonitor,
		InnerTestSteps.EndMonitor,
		InnerTestSteps.SetDesiredClutchEngagementStop,
		InnerTestSteps.End
	};

	private InnerTestSteps index;

	public bool InnerTestIsRunning => InnerTestSteps.Start < index && index < InnerTestSteps.End;

	public InnerTest(Action<string> displayDirections, BarInstrument clutchDisplacement, BarInstrument counterShaftSpeed, BarInstrument engineSpeed, TimerControl timer, Action<TestResults, string> notifyComplete)
	{
		DisplayDirections = displayDirections;
		barInstrumentClutchDisplacement = clutchDisplacement;
		barInstrumentCounterShaftSpeed = counterShaftSpeed;
		displayTimerMonitorInstrument = timer;
		NotifyComplete = notifyComplete;
		timerSetDesiredClutchEngagementRequestStatus = new Timer();
		timerSetDesiredClutchEngagementRequestStatus.Interval = (int)TimeSpan.FromSeconds(1.0).TotalMilliseconds;
		timerShutoffClutchValveRequestStatus = new Timer();
		timerShutoffClutchValveRequestStatus.Interval = (int)TimeSpan.FromSeconds(1.0).TotalMilliseconds;
	}

	public void Dispose(bool disposing)
	{
		if (disposing && displayTimerMonitorInstrument != null)
		{
			((Component)(object)displayTimerMonitorInstrument).Dispose();
			displayTimerMonitorInstrument = null;
		}
	}

	public void InnerTestStartTest(int testNumber, Channel tcm)
	{
		this.tcm = tcm;
		SetupInnerTest(testNumber);
		AdvanceInnerTest();
	}

	public void StopInnerTest()
	{
		testResults = TestResults.StopTest;
		errorString = string.Format(CultureInfo.CurrentCulture, Resources.TestNStopped, testNumber);
		if (InnerTestIsRunning)
		{
			AdvanceInnerTest();
		}
		else
		{
			NotifyComplete(testResults, errorString);
		}
	}

	private void SetupInnerTest(int testNumber)
	{
		index = InnerTestSteps.Start;
		testResults = TestResults.NotRun;
		this.testNumber = testNumber;
		switch (this.testNumber)
		{
		case 1:
			testInstrument = barInstrumentClutchDisplacement;
			testSteps = ImpermeabilityClosedSteps;
			clutchStateParameter = 0f;
			displayTimerMonitorInstrument.Duration = TimeSpan.FromMinutes(1.0);
			break;
		case 2:
			testSteps = ImpermeabilityOpenSteps;
			clutchStateParameter = 100f;
			displayTimerMonitorInstrument.Duration = TimeSpan.FromMinutes(1.0);
			break;
		case 3:
			testSteps = DisengagementSteps;
			clutchStateParameter = 100f;
			testInstrument = barInstrumentCounterShaftSpeed;
			displayTimerMonitorInstrument.Duration = TimeSpan.FromSeconds(10.0);
			break;
		}
	}

	private InnerTestSteps GetNextInnerTestStep()
	{
		InnerTestSteps innerTestSteps = testSteps[(int)index];
		if (innerTestSteps < InnerTestSteps.End)
		{
			index++;
		}
		return innerTestSteps;
	}

	private void AdvanceInnerTest()
	{
		InnerTestSteps nextInnerTestStep = GetNextInnerTestStep();
		if (nextInnerTestStep < InnerTestSteps.EndMonitor && (testResults == TestResults.Error || testResults == TestResults.StopTest))
		{
			while (nextInnerTestStep != InnerTestSteps.ShutoffClutchValveResultsStop)
			{
				nextInnerTestStep = GetNextInnerTestStep();
			}
		}
		if (nextInnerTestStep != InnerTestSteps.EndMonitor && nextInnerTestStep < InnerTestSteps.End && !tcm.Online)
		{
			while (nextInnerTestStep != InnerTestSteps.End)
			{
				nextInnerTestStep = GetNextInnerTestStep();
			}
		}
		switch (nextInnerTestStep)
		{
		case InnerTestSteps.Start:
			testResults = TestResults.Running;
			errorString = string.Empty;
			AdvanceInnerTest();
			break;
		case InnerTestSteps.SetDesiredClutchEngagementStart:
			SetDesiredClutchEngagementStartExecute();
			break;
		case InnerTestSteps.SetDesiredClutchEngagementRequestStatus:
			ExecuteService(tcm, "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status", setDesiredClutchEngagementRequestStatus_ServiceCompleteEvent);
			break;
		case InnerTestSteps.ShutoffClutchValveStart:
			ExecuteService(tcm, "RT_052C_Kupplungsventile_abschalten_Start", serviceShutoffClutchValveStart_ServiceCompleteEvent);
			break;
		case InnerTestSteps.ShutoffClutchValveResultsStatus:
			ExecuteService(tcm, "RT_052C_Kupplungsventile_abschalten_Request_Results_Routine_Status", serviceShutoffClutchValveRequestStatus_ServiceCompleteEvent);
			break;
		case InnerTestSteps.StartMonitor:
			SetupMonitorInstrumentState();
			break;
		case InnerTestSteps.EndMonitor:
			BreakdownMonitorInstrument();
			AdvanceInnerTest();
			break;
		case InnerTestSteps.ShutoffClutchValveResultsStop:
			ExecuteService(tcm, "RT_052C_Kupplungsventile_abschalten_Stop", shutoffClutchValveStop_ServiceCompleteEvent);
			break;
		case InnerTestSteps.SetDesiredClutchEngagementStop:
			ExecuteService(tcm, "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop", setDesiredClutchEngagementStop_ServiceCompleteEvent);
			break;
		case InnerTestSteps.End:
			NotifyComplete(testResults, errorString);
			break;
		}
	}

	private static void ExecuteService(Channel tcm, string serviceQualifier, ServiceCompleteEventHandler serviceCompleteEvent)
	{
		Service service = tcm.Services[serviceQualifier];
		service.ServiceCompleteEvent += serviceCompleteEvent;
		service.Execute(synchronous: false);
	}

	private void SetDesiredClutchEngagementStartExecute()
	{
		Service service = tcm.Services["RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start"];
		service.ServiceCompleteEvent += setDesiredClutchEngagementStart_ServiceCompleteEvent;
		service.InputValues[0].Value = clutchStateParameter;
		service.Execute(synchronous: false);
	}

	private void setDesiredClutchEngagementStart_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= setDesiredClutchEngagementStart_ServiceCompleteEvent;
		testResults = (e.Succeeded ? TestResults.Running : TestResults.Error);
		if (!e.Succeeded && e.Exception != null && string.IsNullOrEmpty(errorString))
		{
			errorString = e.Exception.Message;
		}
		AdvanceInnerTest();
	}

	private void setDesiredClutchEngagementRequestStatus_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= setDesiredClutchEngagementRequestStatus_ServiceCompleteEvent;
		HandleRequestResults(service, e, timerSetDesiredClutchEngagementRequestStatus, timerSetDesiredClutchEngagementRequestStatus_Tick);
	}

	private void setDesiredClutchEngagementStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= setDesiredClutchEngagementStop_ServiceCompleteEvent;
		if (!e.Succeeded && (testResults == TestResults.NotRun || testResults == TestResults.Running))
		{
			testResults = TestResults.Error;
			if (e.Exception != null && string.IsNullOrEmpty(errorString))
			{
				errorString = e.Exception.Message;
			}
		}
		AdvanceInnerTest();
	}

	private void serviceShutoffClutchValveStart_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= serviceShutoffClutchValveStart_ServiceCompleteEvent;
		testResults = (e.Succeeded ? TestResults.Running : TestResults.Error);
		if (!e.Succeeded && e.Exception != null)
		{
			errorString = e.Exception.Message;
		}
		AdvanceInnerTest();
	}

	private void serviceShutoffClutchValveRequestStatus_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= serviceShutoffClutchValveRequestStatus_ServiceCompleteEvent;
		HandleRequestResults(service, e, timerShutoffClutchValveRequestStatus, timerShutoffClutchValveRequestStatus_Tick);
	}

	private void HandleRequestResults(Service service, ResultEventArgs e, Timer timer, EventHandler timerEvent)
	{
		bool flag = false;
		object value = service.OutputValues[0].Value;
		if (value != null && testResults != TestResults.StopTest)
		{
			if (value == service.Choices.GetItemFromRawValue(RequestStatusServiceResponses.SuccessfullyRanToCompletion))
			{
				testResults = TestResults.Running;
			}
			else if (value == service.Choices.GetItemFromRawValue(RequestStatusServiceResponses.InProcess))
			{
				flag = true;
				timer.Tick += timerEvent;
				timer.Start();
			}
			else
			{
				testResults = TestResults.Error;
				if (e.Exception != null)
				{
					errorString = e.Exception.Message;
				}
			}
		}
		if (!flag)
		{
			AdvanceInnerTest();
		}
	}

	private void timerShutoffClutchValveRequestStatus_Tick(object sender, EventArgs e)
	{
		Timer timer = sender as Timer;
		timer.Tick -= timerShutoffClutchValveRequestStatus_Tick;
		timer.Stop();
		if (tcm.CommunicationsState == CommunicationsState.Online)
		{
			ExecuteService(tcm, "RT_052C_Kupplungsventile_abschalten_Request_Results_Routine_Status", serviceShutoffClutchValveRequestStatus_ServiceCompleteEvent);
			return;
		}
		timer.Tick += timerShutoffClutchValveRequestStatus_Tick;
		timer.Start();
	}

	private void timerSetDesiredClutchEngagementRequestStatus_Tick(object sender, EventArgs e)
	{
		Timer timer = sender as Timer;
		timer.Tick -= timerSetDesiredClutchEngagementRequestStatus_Tick;
		timer.Stop();
		if (tcm.CommunicationsState == CommunicationsState.Online)
		{
			ExecuteService(tcm, "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status", setDesiredClutchEngagementRequestStatus_ServiceCompleteEvent);
			return;
		}
		timer.Tick += timerSetDesiredClutchEngagementRequestStatus_Tick;
		timer.Start();
	}

	private void shutoffClutchValveStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= shutoffClutchValveStop_ServiceCompleteEvent;
		if (!e.Succeeded && (testResults == TestResults.NotRun || testResults == TestResults.Running))
		{
			testResults = TestResults.Error;
			if (e.Exception != null && string.IsNullOrEmpty(errorString))
			{
				errorString = e.Exception.Message;
			}
		}
		AdvanceInnerTest();
	}

	private void SetupMonitorInstrumentState()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)testInstrument.RepresentedState == 3)
		{
			testResults = TestResults.Fail;
			AdvanceInnerTest();
		}
		else
		{
			testInstrument.RepresentedStateChanged += testInstrument_RepresentedStateChanged;
			displayTimerMonitorInstrument.TimerCountdownCompleted += displayTimerMonitorInstrument_TimerCountdownCompleted;
			displayTimerMonitorInstrument.Start();
		}
	}

	private void testInstrument_RepresentedStateChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)testInstrument.RepresentedState == 3)
		{
			testResults = TestResults.Fail;
			AdvanceInnerTest();
		}
	}

	private void displayTimerMonitorInstrument_TimerCountdownCompleted(object sender, EventArgs e)
	{
		testResults = TestResults.Success;
		AdvanceInnerTest();
	}

	private void BreakdownMonitorInstrument()
	{
		displayTimerMonitorInstrument.Stop();
		displayTimerMonitorInstrument.TimerCountdownCompleted -= displayTimerMonitorInstrument_TimerCountdownCompleted;
		testInstrument.RepresentedStateChanged -= testInstrument_RepresentedStateChanged;
	}
}
