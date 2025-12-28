using System;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

internal sealed class EngineSpeedServicePair
{
	public readonly string StartModificationService;

	public readonly string StopModificationService;

	public readonly int TargetSpeed;

	public readonly int HoldTimeSeconds;

	public readonly TimeSpan HoldTimeSpan;

	public EngineSpeedServicePair(string start, string stop, int targetSpeed, int holdTimeSeconds)
	{
		StartModificationService = start;
		StopModificationService = stop;
		TargetSpeed = targetSpeed;
		HoldTimeSeconds = holdTimeSeconds;
		HoldTimeSpan = new TimeSpan(0, 0, holdTimeSeconds);
	}
}
