using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Intake_Throttle_Valve.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ManipulatingIntakeThrottleValveTo => ResourceManager.GetString("StringTable.Message_ManipulatingIntakeThrottleValveTo");

	internal static string Message_EngineStartedWhileIntakeThrottleValveManipulationInProgressStoppingNow => ResourceManager.GetString("StringTable.Message_EngineStartedWhileIntakeThrottleValveManipulationInProgressStoppingNow");

	internal static string Message_StoppingIntakeThrottleValveManipulation => ResourceManager.GetString("StringTable.Message_StoppingIntakeThrottleValveManipulation");

	internal static string Message_Done => ResourceManager.GetString("StringTable.Message_Done");

	internal static string MessageFormat_Done0 => ResourceManager.GetString("StringTable.MessageFormat_Done0");

	internal static string MessageFormat_Error0 => ResourceManager.GetString("StringTable.MessageFormat_Error0");

	internal static string Message_TheMCMConnectedIsNotSupported0 => ResourceManager.GetString("StringTable.Message_TheMCMConnectedIsNotSupported0");
}
