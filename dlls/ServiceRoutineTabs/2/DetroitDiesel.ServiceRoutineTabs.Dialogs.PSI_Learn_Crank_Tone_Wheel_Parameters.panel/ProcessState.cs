namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel;

public enum ProcessState
{
	Start,
	NotRunning,
	RequestSeed,
	SendKey,
	SetShortTermAdjust,
	Running,
	Stopping,
	ReturnControl,
	ReturnControlFailed,
	WaitingOnShutdown,
	Complete
}
