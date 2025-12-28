// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel.OuterTest
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel;

internal class OuterTest
{
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
  private OuterTest.OuterTestState outerTestState = OuterTest.OuterTestState.Stopped;

  public bool TestStopped => this.outerTestState == OuterTest.OuterTestState.Stopped;

  public bool TestEnded => this.outerTestState == OuterTest.OuterTestState.End;

  public bool OuterTestIsRunning => !this.TestStopped && !this.TestEnded;

  public string TestNumber
  {
    get => this.testNumber.ToString((IFormatProvider) CultureInfo.CurrentCulture);
  }

  public OuterTest(
    Action<string> logText,
    Action<string> displayDirections,
    Action<int, TestResults, string> updateTestResults,
    BarInstrument clutchDisplacement,
    BarInstrument counterShaftSpeed,
    BarInstrument engineSpeed,
    TimerControl timer)
  {
    this.LogText = logText;
    this.DisplayDirections = displayDirections;
    this.UpdateTestResults = updateTestResults;
    this.barInstrumentClutchDisplacement = clutchDisplacement;
    this.barInstrumentEngineSpeed = engineSpeed;
    this.barInstrumentCounterShaftSpeed = counterShaftSpeed;
    this.innerTest = new InnerTest(displayDirections, clutchDisplacement, counterShaftSpeed, engineSpeed, timer, new Action<TestResults, string>(this.InnerTestComplete));
    this.startTheEngine = new StartEngineStep(engineSpeed, displayDirections, new Action<TestResults>(this.StopEngineComplete));
    this.SetupTestConditions();
  }

  public void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    if (this.innerTest != null)
    {
      this.innerTest.Dispose(disposing);
      this.innerTest = (InnerTest) null;
    }
    if (this.startTheEngine != null)
    {
      this.startTheEngine.Dispose(disposing);
      this.startTheEngine = (StartEngineStep) null;
    }
  }

  public void StartTest(Channel tcm)
  {
    this.tcm = tcm;
    this.DisplayDirections(Resources.DirectionsWaitForTests);
    this.outerTestState = OuterTest.OuterTestState.Starting;
    this.LogText(Resources.TestStateTestStarted);
    this.AdvanceOuterTest();
  }

  public void StopTest()
  {
    if (this.outerTestState == OuterTest.OuterTestState.RunFirstInnerTest || this.outerTestState == OuterTest.OuterTestState.RunSecondInnerTest || this.outerTestState == OuterTest.OuterTestState.RunThirdInnerTest)
      this.innerTest.StopInnerTest();
    else if (this.outerTestState == OuterTest.OuterTestState.StartEngine)
    {
      this.startTheEngine.Stop();
    }
    else
    {
      this.DisplayDirections(Resources.TestStateTestsStoppedByUser);
      this.LogText(Resources.TestStateTestsStoppedByUser);
    }
  }

  private void SetupTestConditions()
  {
    switch (this.outerTestState)
    {
      case OuterTest.OuterTestState.Stopped:
        ((AxisSingleInstrumentBase) this.barInstrumentEngineSpeed).Gradient = Gradient.FromString("(Ok),(1,Ok),(1,Fault)");
        ((Control) this.barInstrumentEngineSpeed).Refresh();
        ((AxisSingleInstrumentBase) this.barInstrumentClutchDisplacement).Gradient = Gradient.FromString("(Ok),(10.01,Ok),(10.01,Fault)");
        ((Control) this.barInstrumentClutchDisplacement).Refresh();
        ((AxisSingleInstrumentBase) this.barInstrumentCounterShaftSpeed).Gradient = Gradient.FromString("(Default)");
        ((Control) this.barInstrumentCounterShaftSpeed).Refresh();
        break;
      case OuterTest.OuterTestState.RunSecondInnerTest:
        ((AxisSingleInstrumentBase) this.barInstrumentClutchDisplacement).Gradient = Gradient.FromString("(Fault),(90,Fault),(90,Ok)");
        ((Control) this.barInstrumentClutchDisplacement).Refresh();
        break;
      case OuterTest.OuterTestState.StartEngine:
        ((AxisSingleInstrumentBase) this.barInstrumentClutchDisplacement).Gradient = Gradient.FromString("(Default)");
        ((Control) this.barInstrumentClutchDisplacement).Refresh();
        ((AxisSingleInstrumentBase) this.barInstrumentEngineSpeed).Gradient = Gradient.FromString("(Fault),(1,Fault),(1,Ok)");
        ((Control) this.barInstrumentEngineSpeed).Refresh();
        break;
      case OuterTest.OuterTestState.RunThirdInnerTest:
        ((AxisSingleInstrumentBase) this.barInstrumentCounterShaftSpeed).Gradient = Gradient.FromString("(Ok),(10,Ok),(10,Fault)");
        ((Control) this.barInstrumentCounterShaftSpeed).Refresh();
        break;
      case OuterTest.OuterTestState.End:
        ((AxisSingleInstrumentBase) this.barInstrumentEngineSpeed).Gradient = Gradient.FromString("(Ok),(1,Ok),(1,Fault)");
        ((Control) this.barInstrumentEngineSpeed).Refresh();
        ((AxisSingleInstrumentBase) this.barInstrumentClutchDisplacement).Gradient = Gradient.FromString("(Ok),(10.01,Ok),(10.01,Fault)");
        ((Control) this.barInstrumentClutchDisplacement).Refresh();
        ((AxisSingleInstrumentBase) this.barInstrumentCounterShaftSpeed).Gradient = Gradient.FromString("(Default)");
        ((Control) this.barInstrumentCounterShaftSpeed).Refresh();
        break;
    }
  }

  private void ReportTestResults()
  {
    string str1 = string.Empty;
    switch (this.testResults)
    {
      case TestResults.Fail:
        string str2 = string.Empty;
        switch (this.testNumber)
        {
          case 1:
            str2 = Resources.Test1FailureReason;
            break;
          case 2:
            str2 = Resources.Test2FailureReason;
            break;
          case 3:
            str2 = Resources.Test3FailureReason;
            break;
        }
        str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestNumberFailed, (object) this.testNumber, (object) str2);
        break;
      case TestResults.Success:
        str1 = this.testNumber >= 3 ? Resources.TestStateAllTestsPassed : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestNPassed, (object) this.testNumber, (object) string.Empty);
        break;
      case TestResults.StopTest:
        str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestNStopped, (object) this.testNumber, (object) string.Empty);
        break;
      case TestResults.Error:
        str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestNError, (object) this.testNumber, (object) string.Empty);
        break;
    }
    this.LogText(str1);
    this.DisplayDirections(str1);
  }

  public void InnerTestComplete(TestResults innerTestResults, string errorString)
  {
    this.testResults = innerTestResults;
    this.UpdateTestResults(this.testNumber, this.testResults, errorString);
    this.ReportTestResults();
    if (this.testResults == TestResults.Success && this.testNumber < 3)
      this.AdvanceOuterTest();
    else
      this.EndOuterTest();
  }

  public void StopEngineComplete(TestResults innerTestResults)
  {
    this.testResults = innerTestResults;
    if (this.testResults == TestResults.Success)
      this.AdvanceOuterTest();
    else
      this.EndOuterTest();
  }

  private void EndOuterTest()
  {
    this.outerTestState = OuterTest.OuterTestState.End;
    this.AdvanceOuterTest();
  }

  private void CallInnerTest(int testNumber)
  {
    this.LogText(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestStateTestNIsRunning, (object) testNumber));
    this.testNumber = testNumber;
    this.innerTest.InnerTestStartTest(testNumber, this.tcm);
  }

  private void AdvanceOuterTest()
  {
    if (this.outerTestState != OuterTest.OuterTestState.End)
    {
      if (this.outerTestState == OuterTest.OuterTestState.Stopped)
        return;
      ++this.outerTestState;
    }
    this.SetupTestConditions();
    switch (this.outerTestState)
    {
      case OuterTest.OuterTestState.Stopped:
        throw new InvalidOperationException("The Stopped step is not valid for the Outer Test.");
      case OuterTest.OuterTestState.Starting:
        throw new InvalidOperationException("The Starting step is not valid for the Outer Test.");
      case OuterTest.OuterTestState.RunFirstInnerTest:
        this.CallInnerTest(1);
        break;
      case OuterTest.OuterTestState.RunSecondInnerTest:
        this.CallInnerTest(2);
        break;
      case OuterTest.OuterTestState.StartEngine:
        this.startTheEngine.AskTheTechToStartTheEngine();
        break;
      case OuterTest.OuterTestState.RunThirdInnerTest:
        this.CallInnerTest(3);
        break;
      case OuterTest.OuterTestState.End:
        this.SetupTestConditions();
        break;
    }
  }

  private enum OuterTestState
  {
    Stopped,
    Starting,
    RunFirstInnerTest,
    RunSecondInnerTest,
    StartEngine,
    RunThirdInnerTest,
    End,
  }
}
