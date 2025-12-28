namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

internal enum Step
{
	None,
	StartAdaptation,
	SetMaxEngineSpeedLimit,
	ShutOffFans,
	CloseEGRValve,
	BeginCheckForOperatingConditions,
	WaitForOperatingConditions,
	DriveEngineSpeed,
	WaitEngineSpeed,
	HoldEngineSpeed,
	StopEngineSpeed,
	StartAAMA,
	RequestAAMAResults,
	ManualOperation,
	AdaptionComplete,
	OpenEGRValve,
	TurnOnFans,
	ResetMaxEngineSpeedLimit,
	CommitToPermanentMemory,
	ClearFaults
}
