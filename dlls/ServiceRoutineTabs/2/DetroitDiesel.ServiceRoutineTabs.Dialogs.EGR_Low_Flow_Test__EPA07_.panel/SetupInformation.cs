using System;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Low_Flow_Test__EPA07_.panel;

public class SetupInformation
{
	public double EGRMinimumPosition { get; set; }

	public int TestIdleSpeed { get; set; }

	public int RunOffIdleSpeed { get; set; }

	public TimeSpan TestDurationThermalCondition { get; set; }

	public SetupInformation(double egrMinimumPosition, int initialIdleSpeed, int runOffIdleSpeed, int thermalWaitLength)
	{
		EGRMinimumPosition = egrMinimumPosition;
		TestIdleSpeed = initialIdleSpeed;
		RunOffIdleSpeed = runOffIdleSpeed;
		TestDurationThermalCondition = TimeSpan.FromMinutes(thermalWaitLength);
	}
}
