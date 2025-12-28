using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Mechanical_Compression_Test.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_PleaseTurnTheIgnition => ResourceManager.GetString("StringTable.Message_PleaseTurnTheIgnition");

	internal static string Message_CompressionTestStopped => ResourceManager.GetString("StringTable.Message_CompressionTestStopped");

	internal static string Message_CompressionTestStarted => ResourceManager.GetString("StringTable.Message_CompressionTestStarted");

	internal static string Message_TheMCM21TDoesNotSupportTheServiceRoutine => ResourceManager.GetString("StringTable.Message_TheMCM21TDoesNotSupportTheServiceRoutine");

	internal static string Message_EngineIsRunningInCompressionTestMode => ResourceManager.GetString("StringTable.Message_EngineIsRunningInCompressionTestMode");

	internal static string Message_EngineSpeedUnits => ResourceManager.GetString("StringTable.Message_EngineSpeedUnits");

	internal static string Message_TheMCM21TIsOfflineCannotExecuteService => ResourceManager.GetString("StringTable.Message_TheMCM21TIsOfflineCannotExecuteService");

	internal static string Message_MaxObservedEngineSpeed => ResourceManager.GetString("StringTable.Message_MaxObservedEngineSpeed");

	internal static string Message_StopCompressionTest => ResourceManager.GetString("StringTable.Message_StopCompressionTest");

	internal static string Message_Success => ResourceManager.GetString("StringTable.Message_Success");

	internal static string Message_TheTestWasCancelledByTheUser => ResourceManager.GetString("StringTable.Message_TheTestWasCancelledByTheUser");

	internal static string Message_Failed => ResourceManager.GetString("StringTable.Message_Failed");
}
