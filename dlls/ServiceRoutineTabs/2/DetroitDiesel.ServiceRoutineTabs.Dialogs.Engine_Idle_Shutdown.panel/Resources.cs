using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_EngineIdleShutdownPreventionHasFailed0 => ResourceManager.GetString("StringTable.MessageFormat_EngineIdleShutdownPreventionHasFailed0");

	internal static string Message_NotSupported => ResourceManager.GetString("StringTable.Message_NotSupported");

	internal static string Message_FailureWhileTryingToAllowEngineIdleShutdown => ResourceManager.GetString("StringTable.Message_FailureWhileTryingToAllowEngineIdleShutdown");

	internal static string Message_RequestedEngineIdleShutdownPrevention => ResourceManager.GetString("StringTable.Message_RequestedEngineIdleShutdownPrevention");

	internal static string Message_NotDefined => ResourceManager.GetString("StringTable.Message_NotDefined");

	internal static string Message_FailureWhileTryingToPreventEngineIdleShutdown => ResourceManager.GetString("StringTable.Message_FailureWhileTryingToPreventEngineIdleShutdown");

	internal static string Message_RequestedEngineIdleShutdownAllowingRequested => ResourceManager.GetString("StringTable.Message_RequestedEngineIdleShutdownAllowingRequested");
}
