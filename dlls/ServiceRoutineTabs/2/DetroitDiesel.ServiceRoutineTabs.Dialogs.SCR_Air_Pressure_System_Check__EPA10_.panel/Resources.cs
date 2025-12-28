using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Air_Pressure_System_Check__EPA10_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_EngineIsNotRunningCheckCanStart => ResourceManager.GetString("StringTable.Message_EngineIsNotRunningCheckCanStart");

	internal static string Message_EngineSpeedCannotBeDetectedCheckCannotStart => ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetectedCheckCannotStart");

	internal static string Message_EngineIsRunningCheckCannotStart0 => ResourceManager.GetString("StringTable.Message_EngineIsRunningCheckCannotStart0");
}
