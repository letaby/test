// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel.InnerTest
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Transmission_Clutch_Control__MY13_.panel;

internal class InnerTest
{
  private const float ClutchStateParameterClosed = 0.0f;
  private const float ClutchStateParameterOpen = 100f;
  private const string SetDesiredClutchEngagementStart = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start";
  private const string SetDesiredClutchEngagementStop = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop";
  private const string SetDesiredClutchEngagementRequestPosition = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Aktuelle_Position_Kupplung";
  private const string SetDesiredClutchEngagementRequestStatus = "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status";
  private const string ShutoffClutchValveStart = "RT_052C_Kupplungsventile_abschalten_Start";
  private const string ShutoffClutchValveStop = "RT_052C_Kupplungsventile_abschalten_Stop";
  private const string ShutoffClutchValveRequestStatus = "RT_052C_Kupplungsventile_abschalten_Request_Results_Routine_Status";
  private float clutchStateParameter = 0.0f;
  private Channel tcm;
  private Action<string> DisplayDirections;
  private Action<TestResults, string> NotifyComplete;
  private BarInstrument barInstrumentClutchDisplacement;
  private BarInstrument barInstrumentCounterShaftSpeed;
  private TimerControl displayTimerMonitorInstrument;
  private Timer timerSetDesiredClutchEngagementRequestStatus = (Timer) null;
  private Timer timerShutoffClutchValveRequestStatus = (Timer) null;
  private string errorString;
  private int testNumber;
  private TestResults testResults = TestResults.NotRun;
  private BarInstrument testInstrument;
  private InnerTest.InnerTestSteps[] testSteps;
  private static InnerTest.InnerTestSteps[] ImpermeabilityClosedSteps = new InnerTest.InnerTestSteps[10]
  {
    InnerTest.InnerTestSteps.Start,
    InnerTest.InnerTestSteps.SetDesiredClutchEngagementStart,
    InnerTest.InnerTestSteps.SetDesiredClutchEngagementRequestStatus,
    InnerTest.InnerTestSteps.ShutoffClutchValveStart,
    InnerTest.InnerTestSteps.ShutoffClutchValveResultsStatus,
    InnerTest.InnerTestSteps.StartMonitor,
    InnerTest.InnerTestSteps.EndMonitor,
    InnerTest.InnerTestSteps.ShutoffClutchValveResultsStop,
    InnerTest.InnerTestSteps.SetDesiredClutchEngagementStop,
    InnerTest.InnerTestSteps.End
  };
  private static InnerTest.InnerTestSteps[] ImpermeabilityOpenSteps = new InnerTest.InnerTestSteps[10]
  {
    InnerTest.InnerTestSteps.Start,
    InnerTest.InnerTestSteps.SetDesiredClutchEngagementStart,
    InnerTest.InnerTestSteps.SetDesiredClutchEngagementRequestStatus,
    InnerTest.InnerTestSteps.ShutoffClutchValveStart,
    InnerTest.InnerTestSteps.ShutoffClutchValveResultsStatus,
    InnerTest.InnerTestSteps.StartMonitor,
    InnerTest.InnerTestSteps.EndMonitor,
    InnerTest.InnerTestSteps.ShutoffClutchValveResultsStop,
    InnerTest.InnerTestSteps.SetDesiredClutchEngagementStop,
    InnerTest.InnerTestSteps.End
  };
  private static InnerTest.InnerTestSteps[] DisengagementSteps = new InnerTest.InnerTestSteps[7]
  {
    InnerTest.InnerTestSteps.Start,
    InnerTest.InnerTestSteps.SetDesiredClutchEngagementStart,
    InnerTest.InnerTestSteps.SetDesiredClutchEngagementRequestStatus,
    InnerTest.InnerTestSteps.StartMonitor,
    InnerTest.InnerTestSteps.EndMonitor,
    InnerTest.InnerTestSteps.SetDesiredClutchEngagementStop,
    InnerTest.InnerTestSteps.End
  };
  private InnerTest.InnerTestSteps index;

  public InnerTest(
    Action<string> displayDirections,
    BarInstrument clutchDisplacement,
    BarInstrument counterShaftSpeed,
    BarInstrument engineSpeed,
    TimerControl timer,
    Action<TestResults, string> notifyComplete)
  {
    this.DisplayDirections = displayDirections;
    this.barInstrumentClutchDisplacement = clutchDisplacement;
    this.barInstrumentCounterShaftSpeed = counterShaftSpeed;
    this.displayTimerMonitorInstrument = timer;
    this.NotifyComplete = notifyComplete;
    this.timerSetDesiredClutchEngagementRequestStatus = new Timer();
    this.timerSetDesiredClutchEngagementRequestStatus.Interval = (int) TimeSpan.FromSeconds(1.0).TotalMilliseconds;
    this.timerShutoffClutchValveRequestStatus = new Timer();
    this.timerShutoffClutchValveRequestStatus.Interval = (int) TimeSpan.FromSeconds(1.0).TotalMilliseconds;
  }

  public void Dispose(bool disposing)
  {
    if (!disposing || this.displayTimerMonitorInstrument == null)
      return;
    ((Component) this.displayTimerMonitorInstrument).Dispose();
    this.displayTimerMonitorInstrument = (TimerControl) null;
  }

  public void InnerTestStartTest(int testNumber, Channel tcm)
  {
    this.tcm = tcm;
    this.SetupInnerTest(testNumber);
    this.AdvanceInnerTest();
  }

  public void StopInnerTest()
  {
    this.testResults = TestResults.StopTest;
    this.errorString = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TestNStopped, (object) this.testNumber);
    if (this.InnerTestIsRunning)
      this.AdvanceInnerTest();
    else
      this.NotifyComplete(this.testResults, this.errorString);
  }

  public bool InnerTestIsRunning
  {
    get => InnerTest.InnerTestSteps.Start < this.index && this.index < InnerTest.InnerTestSteps.End;
  }

  private void SetupInnerTest(int testNumber)
  {
    this.index = InnerTest.InnerTestSteps.Start;
    this.testResults = TestResults.NotRun;
    this.testNumber = testNumber;
    switch (this.testNumber)
    {
      case 1:
        this.testInstrument = this.barInstrumentClutchDisplacement;
        this.testSteps = InnerTest.ImpermeabilityClosedSteps;
        this.clutchStateParameter = 0.0f;
        this.displayTimerMonitorInstrument.Duration = TimeSpan.FromMinutes(1.0);
        break;
      case 2:
        this.testSteps = InnerTest.ImpermeabilityOpenSteps;
        this.clutchStateParameter = 100f;
        this.displayTimerMonitorInstrument.Duration = TimeSpan.FromMinutes(1.0);
        break;
      case 3:
        this.testSteps = InnerTest.DisengagementSteps;
        this.clutchStateParameter = 100f;
        this.testInstrument = this.barInstrumentCounterShaftSpeed;
        this.displayTimerMonitorInstrument.Duration = TimeSpan.FromSeconds(10.0);
        break;
    }
  }

  private InnerTest.InnerTestSteps GetNextInnerTestStep()
  {
    InnerTest.InnerTestSteps testStep = this.testSteps[(int) this.index];
    if (testStep < InnerTest.InnerTestSteps.End)
      ++this.index;
    return testStep;
  }

  private void AdvanceInnerTest()
  {
    InnerTest.InnerTestSteps nextInnerTestStep = this.GetNextInnerTestStep();
    if (nextInnerTestStep < InnerTest.InnerTestSteps.EndMonitor && (this.testResults == TestResults.Error || this.testResults == TestResults.StopTest))
    {
      while (nextInnerTestStep != InnerTest.InnerTestSteps.ShutoffClutchValveResultsStop)
        nextInnerTestStep = this.GetNextInnerTestStep();
    }
    if (nextInnerTestStep != InnerTest.InnerTestSteps.EndMonitor && nextInnerTestStep < InnerTest.InnerTestSteps.End && !this.tcm.Online)
    {
      while (nextInnerTestStep != InnerTest.InnerTestSteps.End)
        nextInnerTestStep = this.GetNextInnerTestStep();
    }
    switch (nextInnerTestStep)
    {
      case InnerTest.InnerTestSteps.Start:
        this.testResults = TestResults.Running;
        this.errorString = string.Empty;
        this.AdvanceInnerTest();
        break;
      case InnerTest.InnerTestSteps.SetDesiredClutchEngagementStart:
        this.SetDesiredClutchEngagementStartExecute();
        break;
      case InnerTest.InnerTestSteps.SetDesiredClutchEngagementRequestStatus:
        InnerTest.ExecuteService(this.tcm, "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status", new ServiceCompleteEventHandler(this.setDesiredClutchEngagementRequestStatus_ServiceCompleteEvent));
        break;
      case InnerTest.InnerTestSteps.ShutoffClutchValveStart:
        InnerTest.ExecuteService(this.tcm, "RT_052C_Kupplungsventile_abschalten_Start", new ServiceCompleteEventHandler(this.serviceShutoffClutchValveStart_ServiceCompleteEvent));
        break;
      case InnerTest.InnerTestSteps.ShutoffClutchValveResultsStatus:
        InnerTest.ExecuteService(this.tcm, "RT_052C_Kupplungsventile_abschalten_Request_Results_Routine_Status", new ServiceCompleteEventHandler(this.serviceShutoffClutchValveRequestStatus_ServiceCompleteEvent));
        break;
      case InnerTest.InnerTestSteps.StartMonitor:
        this.SetupMonitorInstrumentState();
        break;
      case InnerTest.InnerTestSteps.EndMonitor:
        this.BreakdownMonitorInstrument();
        this.AdvanceInnerTest();
        break;
      case InnerTest.InnerTestSteps.ShutoffClutchValveResultsStop:
        InnerTest.ExecuteService(this.tcm, "RT_052C_Kupplungsventile_abschalten_Stop", new ServiceCompleteEventHandler(this.shutoffClutchValveStop_ServiceCompleteEvent));
        break;
      case InnerTest.InnerTestSteps.SetDesiredClutchEngagementStop:
        InnerTest.ExecuteService(this.tcm, "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Stop", new ServiceCompleteEventHandler(this.setDesiredClutchEngagementStop_ServiceCompleteEvent));
        break;
      case InnerTest.InnerTestSteps.End:
        this.NotifyComplete(this.testResults, this.errorString);
        break;
    }
  }

  private static void ExecuteService(
    Channel tcm,
    string serviceQualifier,
    ServiceCompleteEventHandler serviceCompleteEvent)
  {
    Service service = tcm.Services[serviceQualifier];
    service.ServiceCompleteEvent += serviceCompleteEvent;
    service.Execute(false);
  }

  private void SetDesiredClutchEngagementStartExecute()
  {
    Service service = this.tcm.Services["RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Start"];
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.setDesiredClutchEngagementStart_ServiceCompleteEvent);
    service.InputValues[0].Value = (object) this.clutchStateParameter;
    service.Execute(false);
  }

  private void setDesiredClutchEngagementStart_ServiceCompleteEvent(
    object sender,
    ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.setDesiredClutchEngagementStart_ServiceCompleteEvent);
    this.testResults = e.Succeeded ? TestResults.Running : TestResults.Error;
    if (!e.Succeeded && e.Exception != null && string.IsNullOrEmpty(this.errorString))
      this.errorString = e.Exception.Message;
    this.AdvanceInnerTest();
  }

  private void setDesiredClutchEngagementRequestStatus_ServiceCompleteEvent(
    object sender,
    ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.setDesiredClutchEngagementRequestStatus_ServiceCompleteEvent);
    this.HandleRequestResults(service, e, this.timerSetDesiredClutchEngagementRequestStatus, new EventHandler(this.timerSetDesiredClutchEngagementRequestStatus_Tick));
  }

  private void setDesiredClutchEngagementStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.setDesiredClutchEngagementStop_ServiceCompleteEvent);
    if (!e.Succeeded && (this.testResults == TestResults.NotRun || this.testResults == TestResults.Running))
    {
      this.testResults = TestResults.Error;
      if (e.Exception != null && string.IsNullOrEmpty(this.errorString))
        this.errorString = e.Exception.Message;
    }
    this.AdvanceInnerTest();
  }

  private void serviceShutoffClutchValveStart_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.serviceShutoffClutchValveStart_ServiceCompleteEvent);
    this.testResults = e.Succeeded ? TestResults.Running : TestResults.Error;
    if (!e.Succeeded && e.Exception != null)
      this.errorString = e.Exception.Message;
    this.AdvanceInnerTest();
  }

  private void serviceShutoffClutchValveRequestStatus_ServiceCompleteEvent(
    object sender,
    ResultEventArgs e)
  {
    Service service = sender as Service;
    service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.serviceShutoffClutchValveRequestStatus_ServiceCompleteEvent);
    this.HandleRequestResults(service, e, this.timerShutoffClutchValveRequestStatus, new EventHandler(this.timerShutoffClutchValveRequestStatus_Tick));
  }

  private void HandleRequestResults(
    Service service,
    ResultEventArgs e,
    Timer timer,
    EventHandler timerEvent)
  {
    bool flag = false;
    object obj = service.OutputValues[0].Value;
    if (obj != null && this.testResults != TestResults.StopTest)
    {
      if (obj == (object) service.Choices.GetItemFromRawValue((object) InnerTest.RequestStatusServiceResponses.SuccessfullyRanToCompletion))
        this.testResults = TestResults.Running;
      else if (obj == (object) service.Choices.GetItemFromRawValue((object) InnerTest.RequestStatusServiceResponses.InProcess))
      {
        flag = true;
        timer.Tick += timerEvent;
        timer.Start();
      }
      else
      {
        this.testResults = TestResults.Error;
        if (e.Exception != null)
          this.errorString = e.Exception.Message;
      }
    }
    if (flag)
      return;
    this.AdvanceInnerTest();
  }

  private void timerShutoffClutchValveRequestStatus_Tick(object sender, EventArgs e)
  {
    Timer timer = sender as Timer;
    timer.Tick -= new EventHandler(this.timerShutoffClutchValveRequestStatus_Tick);
    timer.Stop();
    if (this.tcm.CommunicationsState == CommunicationsState.Online)
    {
      InnerTest.ExecuteService(this.tcm, "RT_052C_Kupplungsventile_abschalten_Request_Results_Routine_Status", new ServiceCompleteEventHandler(this.serviceShutoffClutchValveRequestStatus_ServiceCompleteEvent));
    }
    else
    {
      timer.Tick += new EventHandler(this.timerShutoffClutchValveRequestStatus_Tick);
      timer.Start();
    }
  }

  private void timerSetDesiredClutchEngagementRequestStatus_Tick(object sender, EventArgs e)
  {
    Timer timer = sender as Timer;
    timer.Tick -= new EventHandler(this.timerSetDesiredClutchEngagementRequestStatus_Tick);
    timer.Stop();
    if (this.tcm.CommunicationsState == CommunicationsState.Online)
    {
      InnerTest.ExecuteService(this.tcm, "RT_0521_Sollwertvorgabe_Kupplung_in_Prozent_Request_Results_Routine_Status", new ServiceCompleteEventHandler(this.setDesiredClutchEngagementRequestStatus_ServiceCompleteEvent));
    }
    else
    {
      timer.Tick += new EventHandler(this.timerSetDesiredClutchEngagementRequestStatus_Tick);
      timer.Start();
    }
  }

  private void shutoffClutchValveStop_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.shutoffClutchValveStop_ServiceCompleteEvent);
    if (!e.Succeeded && (this.testResults == TestResults.NotRun || this.testResults == TestResults.Running))
    {
      this.testResults = TestResults.Error;
      if (e.Exception != null && string.IsNullOrEmpty(this.errorString))
        this.errorString = e.Exception.Message;
    }
    this.AdvanceInnerTest();
  }

  private void SetupMonitorInstrumentState()
  {
    if (this.testInstrument.RepresentedState == 3)
    {
      this.testResults = TestResults.Fail;
      this.AdvanceInnerTest();
    }
    else
    {
      this.testInstrument.RepresentedStateChanged += new EventHandler(this.testInstrument_RepresentedStateChanged);
      this.displayTimerMonitorInstrument.TimerCountdownCompleted += new EventHandler(this.displayTimerMonitorInstrument_TimerCountdownCompleted);
      this.displayTimerMonitorInstrument.Start();
    }
  }

  private void testInstrument_RepresentedStateChanged(object sender, EventArgs e)
  {
    if (this.testInstrument.RepresentedState != 3)
      return;
    this.testResults = TestResults.Fail;
    this.AdvanceInnerTest();
  }

  private void displayTimerMonitorInstrument_TimerCountdownCompleted(object sender, EventArgs e)
  {
    this.testResults = TestResults.Success;
    this.AdvanceInnerTest();
  }

  private void BreakdownMonitorInstrument()
  {
    this.displayTimerMonitorInstrument.Stop();
    this.displayTimerMonitorInstrument.TimerCountdownCompleted -= new EventHandler(this.displayTimerMonitorInstrument_TimerCountdownCompleted);
    this.testInstrument.RepresentedStateChanged -= new EventHandler(this.testInstrument_RepresentedStateChanged);
  }

  private enum InnerTestSteps
  {
    Start = 0,
    SetDesiredClutchEngagementStart = 1,
    SetDesiredClutchEngagementRequestStatus = 2,
    ShutoffClutchValveStart = 3,
    ShutoffClutchValveResultsStatus = 4,
    StartMonitor = 5,
    EndMonitor = 6,
    Cleanup = 7,
    ShutoffClutchValveResultsStop = 7,
    SetDesiredClutchEngagementStop = 8,
    End = 9,
  }

  private enum RequestStatusServiceResponses
  {
    SuccessfullyRanToCompletion,
    InProcess,
    ReturnedWithoutResult,
  }
}
