using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ResultFailed => ResourceManager.GetString("StringTable.Message_ResultFailed");

	internal static string Message_TheTestIsReadyToBeStarted => ResourceManager.GetString("StringTable.Message_TheTestIsReadyToBeStarted");

	internal static string Print_Test_History => ResourceManager.GetString("StringTable.Print_Test_History");

	internal static string Message_TestIsRunning => ResourceManager.GetString("StringTable.Message_TestIsRunning");

	internal static string Message_TheTestCannotBeRunUntilAnMCMIsConnected => ResourceManager.GetString("StringTable.Message_TheTestCannotBeRunUntilAnMCMIsConnected");

	internal static string Message_PleaseTurnTheIgnitionKey => ResourceManager.GetString("StringTable.Message_PleaseTurnTheIgnitionKey");

	internal static string Message_TheTestFailedToRun => ResourceManager.GetString("StringTable.Message_TheTestFailedToRun");

	internal static string Message_StartTheTestWithTheEngineOffAndTheIgnitionOn => ResourceManager.GetString("StringTable.Message_StartTheTestWithTheEngineOffAndTheIgnitionOn");

	internal static string Print_Header => ResourceManager.GetString("StringTable.Print_Header");

	internal static string Message_CompressionTest => ResourceManager.GetString("StringTable.Message_CompressionTest");

	internal static string Message_ParkingBrakeOnTransInNeutral => ResourceManager.GetString("StringTable.Message_ParkingBrakeOnTransInNeutral");

	internal static string Message_TheMCMDoesNotSupportTheServiceRoutine => ResourceManager.GetString("StringTable.Message_TheMCMDoesNotSupportTheServiceRoutine");

	internal static string Message_TheMCMIsOfflineCannotExecuteService => ResourceManager.GetString("StringTable.Message_TheMCMIsOfflineCannotExecuteService");

	internal static string Message_CannotRunTestWithEngineRunning => ResourceManager.GetString("StringTable.Message_CannotRunTestWithEngineRunning");

	internal static string Message_AutomaticCompressionTest => ResourceManager.GetString("StringTable.Message_AutomaticCompressionTest");

	internal static string Message_Error => ResourceManager.GetString("StringTable.Message_Error");

	internal static string Message_Result => ResourceManager.GetString("StringTable.Message_Result");

	internal static string Message_The_Test_Has_Not_Started => ResourceManager.GetString("StringTable.Message_The_Test_Has_Not_Started");

	internal static string Print_Results_String => ResourceManager.GetString("StringTable.Print_Results_String");

	internal static string Message_TheTestCompletedSuccessfully => ResourceManager.GetString("StringTable.Message_TheTestCompletedSuccessfully");

	internal static string Report_Title => ResourceManager.GetString("StringTable.Report_Title");

	internal static string Message_ResultPassed => ResourceManager.GetString("StringTable.Message_ResultPassed");

	internal static string MessageFormat_ErrorOperatorError => ResourceManager.GetString("StringTable.MessageFormat_ErrorOperatorError");

	internal static string Message_TheTestWasCancelledByTheUser => ResourceManager.GetString("StringTable.Message_TheTestWasCancelledByTheUser");
}
