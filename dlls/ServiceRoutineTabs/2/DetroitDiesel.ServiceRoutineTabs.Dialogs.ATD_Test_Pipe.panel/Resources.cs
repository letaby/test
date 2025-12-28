using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Test_Pipe.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ExecutingStop => ResourceManager.GetString("StringTable.Message_ExecutingStop");

	internal static string Message_ATDTestPipeStartFailedExecution => ResourceManager.GetString("StringTable.Message_ATDTestPipeStartFailedExecution");

	internal static string Message_ATDTestPipeStartSuccessfullyExecuted => ResourceManager.GetString("StringTable.Message_ATDTestPipeStartSuccessfullyExecuted");

	internal static string Message_ExecutingStart => ResourceManager.GetString("StringTable.Message_ExecutingStart");

	internal static string Message_ATDTestPipeStopFailedExecution => ResourceManager.GetString("StringTable.Message_ATDTestPipeStopFailedExecution");

	internal static string Message_ATDTestPipeStopSuccessfullyExecuted => ResourceManager.GetString("StringTable.Message_ATDTestPipeStopSuccessfullyExecuted");
}
