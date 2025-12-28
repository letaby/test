using System.Collections.Generic;

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

	public int CurrentSpeedPointIndex => currentSpeedPoint;

	public EngineSpeedServicePair CurrentSpeedPoint
	{
		get
		{
			if (currentSpeedPoint >= 0 && currentSpeedPoint < EngineSpeedPoints.Count)
			{
				return EngineSpeedPoints[currentSpeedPoint];
			}
			return null;
		}
	}

	public bool UseAAMA
	{
		get
		{
			return useAAMA;
		}
		set
		{
			useAAMA = value;
			steps = (useAAMA ? AAMASteps : AMASteps);
			currentSpeedPoint = 0;
			currentStepIndex = -1;
		}
	}

	public Step CurrentStep
	{
		get
		{
			if (steps == null || currentStepIndex == -1 || currentStepIndex >= steps.Length)
			{
				return Step.None;
			}
			return steps[currentStepIndex];
		}
	}

	public SetupInformation(string displayName, string engineType, double coolantTemperatureThreshold, double oilTemperatureThreshold, EngineSpeedServicePair[] speedPoints, bool useAAMAWhenAvailable)
	{
		DisplayName = displayName;
		EngineType = engineType;
		CoolantTemperatureThreshold = coolantTemperatureThreshold;
		OilTemperatureThreshold = oilTemperatureThreshold;
		EngineSpeedPoints = new List<EngineSpeedServicePair>(speedPoints);
		UseAAMAWhenAvailable = useAAMAWhenAvailable;
	}

	public void GotoNextStep()
	{
		if (CurrentStep == Step.None)
		{
			return;
		}
		if (CurrentStep == Step.StopEngineSpeed)
		{
			currentSpeedPoint++;
			if (currentSpeedPoint < EngineSpeedPoints.Count)
			{
				GotoStep(Step.DriveEngineSpeed);
				return;
			}
			currentStepIndex++;
			currentSpeedPoint = 0;
		}
		else
		{
			currentStepIndex++;
		}
	}

	public void GotoStep(Step skipTo)
	{
		if (skipTo == Step.StartAdaptation)
		{
			currentSpeedPoint = 0;
		}
		currentStepIndex = -1;
		for (int i = 0; i < steps.Length; i++)
		{
			if (steps[i] == skipTo)
			{
				currentStepIndex = i;
				break;
			}
		}
	}
}
