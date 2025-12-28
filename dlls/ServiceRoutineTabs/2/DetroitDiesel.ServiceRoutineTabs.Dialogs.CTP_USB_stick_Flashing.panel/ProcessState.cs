namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_USB_stick_Flashing.panel;

public enum ProcessState
{
	NotRunning,
	StartHardReset1,
	WaitingforResetToFinish1,
	SetMaxFunctional,
	ReprogrammingFlashSeed,
	StartReprogramming,
	MonitorReprogramming,
	StartHardReset2,
	WaitingforResetToFinish2,
	Complete
}
