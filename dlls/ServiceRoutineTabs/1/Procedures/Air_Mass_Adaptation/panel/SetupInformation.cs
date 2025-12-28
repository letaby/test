// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel.SetupInformation
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

internal sealed class SetupInformation
{
  public readonly string Fan1OffService = "RT_SR005_SW_Routine_Start_SW_Operation(3,1,1000000)";
  public readonly string Fan1OnService = "RT_SR005_SW_Routine_Stop(3)";
  public readonly string Fan2OffService = "RT_SR003_PWM_Routine_Start_PWM_Value(6,100,1000000)";
  public readonly string Fan2OnService = "RT_SR003_PWM_Routine_Stop(6)";
  public readonly string CloseEGRService = "RT_SR003_PWM_Routine_Start_PWM_Value(1,5,1000000)";
  public readonly string OpenEGRService = "RT_SR003_PWM_Routine_Stop(1)";
  public readonly string AAMAStartService = "RT_SR07E_Automated_Air_Mass_Adaption_Start_status";
  public readonly string AAMAStopService = "RT_SR07E_Automated_Air_Mass_Adaption_Stop";
  public readonly string AAMARequestResultsService = "RT_SR07E_Automated_Air_Mass_Adaption_Request_Results_Results_Status";
  public readonly string DisplayName;
  public readonly string EngineType;
  public readonly double CoolantTemperatureThreshold;
  public readonly double OilTemperatureThreshold;
  public readonly bool UseAAMAWhenAvailable;
  private bool useAAMA;
  private static Step[] AMASteps = new Step[17]
  {
    Step.StartAdaptation,
    Step.SetMaxEngineSpeedLimit,
    Step.ShutOffFans,
    Step.CloseEGRValve,
    Step.BeginCheckForOperatingConditions,
    Step.WaitForOperatingConditions,
    Step.DriveEngineSpeed,
    Step.WaitEngineSpeed,
    Step.HoldEngineSpeed,
    Step.StopEngineSpeed,
    Step.ManualOperation,
    Step.AdaptionComplete,
    Step.OpenEGRValve,
    Step.TurnOnFans,
    Step.ResetMaxEngineSpeedLimit,
    Step.CommitToPermanentMemory,
    Step.ClearFaults
  };
  private static Step[] AAMASteps = new Step[11]
  {
    Step.StartAdaptation,
    Step.ShutOffFans,
    Step.BeginCheckForOperatingConditions,
    Step.WaitForOperatingConditions,
    Step.StartAAMA,
    Step.RequestAAMAResults,
    Step.ManualOperation,
    Step.AdaptionComplete,
    Step.TurnOnFans,
    Step.CommitToPermanentMemory,
    Step.ClearFaults
  };
  private Step[] steps;
  private int currentStepIndex = -1;
  private int currentSpeedPoint = 0;
  public readonly IList<EngineSpeedServicePair> EngineSpeedPoints;

  public int CurrentSpeedPointIndex => this.currentSpeedPoint;

  public EngineSpeedServicePair CurrentSpeedPoint
  {
    get
    {
      return this.currentSpeedPoint >= 0 && this.currentSpeedPoint < this.EngineSpeedPoints.Count ? this.EngineSpeedPoints[this.currentSpeedPoint] : (EngineSpeedServicePair) null;
    }
  }

  public bool UseAAMA
  {
    get => this.useAAMA;
    set
    {
      this.useAAMA = value;
      this.steps = this.useAAMA ? SetupInformation.AAMASteps : SetupInformation.AMASteps;
      this.currentSpeedPoint = 0;
      this.currentStepIndex = -1;
    }
  }

  public SetupInformation(
    string displayName,
    string engineType,
    double coolantTemperatureThreshold,
    double oilTemperatureThreshold,
    EngineSpeedServicePair[] speedPoints,
    bool useAAMAWhenAvailable)
  {
    this.DisplayName = displayName;
    this.EngineType = engineType;
    this.CoolantTemperatureThreshold = coolantTemperatureThreshold;
    this.OilTemperatureThreshold = oilTemperatureThreshold;
    this.EngineSpeedPoints = (IList<EngineSpeedServicePair>) new List<EngineSpeedServicePair>((IEnumerable<EngineSpeedServicePair>) speedPoints);
    this.UseAAMAWhenAvailable = useAAMAWhenAvailable;
  }

  public void GotoNextStep()
  {
    if (this.CurrentStep == Step.None)
      return;
    if (this.CurrentStep == Step.StopEngineSpeed)
    {
      ++this.currentSpeedPoint;
      if (this.currentSpeedPoint < this.EngineSpeedPoints.Count)
      {
        this.GotoStep(Step.DriveEngineSpeed);
      }
      else
      {
        ++this.currentStepIndex;
        this.currentSpeedPoint = 0;
      }
    }
    else
      ++this.currentStepIndex;
  }

  public void GotoStep(Step skipTo)
  {
    if (skipTo == Step.StartAdaptation)
      this.currentSpeedPoint = 0;
    this.currentStepIndex = -1;
    for (int index = 0; index < this.steps.Length; ++index)
    {
      if (this.steps[index] == skipTo)
      {
        this.currentStepIndex = index;
        break;
      }
    }
  }

  public Step CurrentStep
  {
    get
    {
      return this.steps == null || this.currentStepIndex == -1 || this.currentStepIndex >= this.steps.Length ? Step.None : this.steps[this.currentStepIndex];
    }
  }
}
