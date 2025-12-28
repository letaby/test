namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Coolant_Systems_Pressure_Test__EMG_.panel;

public class CoolantTestStep
{
	public TestTypes TestType { get; set; }

	public string DisplayText { get; set; }

	public CoolantTestActions Action { get; set; }

	public CoolantTestStep(TestTypes testType, string displayText, CoolantTestActions action)
	{
		TestType = testType;
		DisplayText = displayText;
		Action = action;
	}
}
