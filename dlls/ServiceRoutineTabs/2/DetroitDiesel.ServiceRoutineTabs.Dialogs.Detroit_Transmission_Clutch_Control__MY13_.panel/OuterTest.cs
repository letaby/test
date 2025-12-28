using System;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel;

internal class OuterTest
{
	private enum OuterTestState
	{
		Stopped,
		Starting,
		RunFirstInnerTest,
		RunSecondInnerTest,
		StartEngine,
		RunThirdInnerTest,
		End
	}

	private Channel tcm;

	private Action<string> LogText;

	private Action<string> DisplayDirections;

	private Action<int, TestResults, string> UpdateTestResults;

	private BarInstrument barInstrumentClutchDisplacement;

	private BarInstrument barInstrumentEngineSpeed;

	private BarInstrument barInstrumentCounterShaftSpeed;

	private TestResults testResults = TestResults.Success;

	private InnerTest innerTest;

	private StartEngineStep startTheEngine;

	private int testNumber = 1;

	private OuterTestState outerTestState = OuterTestState.Stopped;

	public bool TestStopped => outerTestState == OuterTestState.Stopped;

	public bool TestEnded => outerTestState == OuterTestState.End;

	public bool OuterTestIsRunning => !TestStopped && !TestEnded;

	public string TestNumber => testNumber.ToString(CultureInfo.CurrentCulture);

	public OuterTest(Action<string> logText, Action<string> displayDirections, Action<int, TestResults, string> updateTestResults, BarInstrument clutchDisplacement, BarInstrument counterShaftSpeed, BarInstrument engineSpeed, TimerControl timer)
	{
		LogText = logText;
		DisplayDirections = displayDirections;
		UpdateTestResults = updateTestResults;
		barInstrumentClutchDisplacement = clutchDisplacement;
		barInstrumentEngineSpeed = engineSpeed;
		barInstrumentCounterShaftSpeed = counterShaftSpeed;
		innerTest = new InnerTest(displayDirections, clutchDisplacement, counterShaftSpeed, engineSpeed, timer, InnerTestComplete);
		startTheEngine = new StartEngineStep(engineSpeed, displayDirections, StopEngineComplete);
		SetupTestConditions();
	}

	public void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (innerTest != null)
			{
				innerTest.Dispose(disposing);
				innerTest = null;
			}
			if (startTheEngine != null)
			{
				startTheEngine.Dispose(disposing);
				startTheEngine = null;
			}
		}
	}

	public void StartTest(Channel tcm)
	{
		this.tcm = tcm;
		DisplayDirections(Resources.DirectionsWaitForTests);
		outerTestState = OuterTestState.Starting;
		LogText(Resources.TestStateTestStarted);
		AdvanceOuterTest();
	}

	public void StopTest()
	{
		if (outerTestState == OuterTestState.RunFirstInnerTest || outerTestState == OuterTestState.RunSecondInnerTest || outerTestState == OuterTestState.RunThirdInnerTest)
		{
			innerTest.StopInnerTest();
			return;
		}
		if (outerTestState == OuterTestState.StartEngine)
		{
			startTheEngine.Stop();
			return;
		}
		DisplayDirections(Resources.TestStateTestsStoppedByUser);
		LogText(Resources.TestStateTestsStoppedByUser);
	}

	private void SetupTestConditions()
	{
		switch (outerTestState)
		{
		case OuterTestState.Stopped:
			((AxisSingleInstrumentBase)barInstrumentEngineSpeed).Gradient = Gradient.FromString("(Ok),(1,Ok),(1,Fault)");
			((Control)(object)barInstrumentEngineSpeed).Refresh();
			((AxisSingleInstrumentBase)barInstrumentClutchDisplacement).Gradient = Gradient.FromString("(Ok),(10.01,Ok),(10.01,Fault)");
			((Control)(object)barInstrumentClutchDisplacement).Refresh();
			((AxisSingleInstrumentBase)barInstrumentCounterShaftSpeed).Gradient = Gradient.FromString("(Default)");
			((Control)(object)barInstrumentCounterShaftSpeed).Refresh();
			break;
		case OuterTestState.Starting:
			break;
		case OuterTestState.RunFirstInnerTest:
			break;
		case OuterTestState.RunSecondInnerTest:
			((AxisSingleInstrumentBase)barInstrumentClutchDisplacement).Gradient = Gradient.FromString("(Fault),(90,Fault),(90,Ok)");
			((Control)(object)barInstrumentClutchDisplacement).Refresh();
			break;
		case OuterTestState.StartEngine:
			((AxisSingleInstrumentBase)barInstrumentClutchDisplacement).Gradient = Gradient.FromString("(Default)");
			((Control)(object)barInstrumentClutchDisplacement).Refresh();
			((AxisSingleInstrumentBase)barInstrumentEngineSpeed).Gradient = Gradient.FromString("(Fault),(1,Fault),(1,Ok)");
			((Control)(object)barInstrumentEngineSpeed).Refresh();
			break;
		case OuterTestState.RunThirdInnerTest:
			((AxisSingleInstrumentBase)barInstrumentCounterShaftSpeed).Gradient = Gradient.FromString("(Ok),(10,Ok),(10,Fault)");
			((Control)(object)barInstrumentCounterShaftSpeed).Refresh();
			break;
		case OuterTestState.End:
			((AxisSingleInstrumentBase)barInstrumentEngineSpeed).Gradient = Gradient.FromString("(Ok),(1,Ok),(1,Fault)");
			((Control)(object)barInstrumentEngineSpeed).Refresh();
			((AxisSingleInstrumentBase)barInstrumentClutchDisplacement).Gradient = Gradient.FromString("(Ok),(10.01,Ok),(10.01,Fault)");
			((Control)(object)barInstrumentClutchDisplacement).Refresh();
			((AxisSingleInstrumentBase)barInstrumentCounterShaftSpeed).Gradient = Gradient.FromString("(Default)");
			((Control)(object)barInstrumentCounterShaftSpeed).Refresh();
			break;
		}
	}

	private void ReportTestResults()
	{
		string obj = string.Empty;
		switch (testResults)
		{
		case TestResults.Success:
			obj = ((testNumber >= 3) ? Resources.TestStateAllTestsPassed : string.Format(CultureInfo.CurrentCulture, Resources.TestNPassed, testNumber, string.Empty));
			break;
		case TestResults.Fail:
		{
			string arg = string.Empty;
			switch (testNumber)
			{
			case 1:
				arg = Resources.Test1FailureReason;
				break;
			case 2:
				arg = Resources.Test2FailureReason;
				break;
			case 3:
				arg = Resources.Test3FailureReason;
				break;
			}
			obj = string.Format(CultureInfo.CurrentCulture, Resources.TestNumberFailed, testNumber, arg);
			break;
		}
		case TestResults.Error:
			obj = string.Format(CultureInfo.CurrentCulture, Resources.TestNError, testNumber, string.Empty);
			break;
		case TestResults.StopTest:
			obj = string.Format(CultureInfo.CurrentCulture, Resources.TestNStopped, testNumber, string.Empty);
			break;
		}
		LogText(obj);
		DisplayDirections(obj);
	}

	public void InnerTestComplete(TestResults innerTestResults, string errorString)
	{
		testResults = innerTestResults;
		UpdateTestResults(testNumber, testResults, errorString);
		ReportTestResults();
		if (testResults == TestResults.Success && testNumber < 3)
		{
			AdvanceOuterTest();
		}
		else
		{
			EndOuterTest();
		}
	}

	public void StopEngineComplete(TestResults innerTestResults)
	{
		testResults = innerTestResults;
		if (testResults == TestResults.Success)
		{
			AdvanceOuterTest();
		}
		else
		{
			EndOuterTest();
		}
	}

	private void EndOuterTest()
	{
		outerTestState = OuterTestState.End;
		AdvanceOuterTest();
	}

	private void CallInnerTest(int testNumber)
	{
		LogText(string.Format(CultureInfo.CurrentCulture, Resources.TestStateTestNIsRunning, testNumber));
		this.testNumber = testNumber;
		innerTest.InnerTestStartTest(testNumber, tcm);
	}

	private void AdvanceOuterTest()
	{
		if (outerTestState != OuterTestState.End)
		{
			if (outerTestState == OuterTestState.Stopped)
			{
				return;
			}
			outerTestState++;
		}
		SetupTestConditions();
		switch (outerTestState)
		{
		case OuterTestState.Stopped:
			throw new InvalidOperationException("The Stopped step is not valid for the Outer Test.");
		case OuterTestState.Starting:
			throw new InvalidOperationException("The Starting step is not valid for the Outer Test.");
		case OuterTestState.RunFirstInnerTest:
			CallInnerTest(1);
			break;
		case OuterTestState.RunSecondInnerTest:
			CallInnerTest(2);
			break;
		case OuterTestState.StartEngine:
			startTheEngine.AskTheTechToStartTheEngine();
			break;
		case OuterTestState.RunThirdInnerTest:
			CallInnerTest(3);
			break;
		case OuterTestState.End:
			SetupTestConditions();
			break;
		}
	}
}
