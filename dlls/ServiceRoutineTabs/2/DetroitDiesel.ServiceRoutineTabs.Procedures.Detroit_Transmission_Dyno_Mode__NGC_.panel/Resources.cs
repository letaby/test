using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__NGC_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_AutomaticShifting => ResourceManager.GetString("StringTable.Message_AutomaticShifting");

	internal static string Message_EndingTheTestTheService => ResourceManager.GetString("StringTable.Message_EndingTheTestTheService");

	internal static string Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit => ResourceManager.GetString("StringTable.Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit");

	internal static string Message_TheStartButtonHasBeenPressed => ResourceManager.GetString("StringTable.Message_TheStartButtonHasBeenPressed");

	internal static string Message_PleaseStartTheEngine => ResourceManager.GetString("StringTable.Message_PleaseStartTheEngine");

	internal static string Message_TheDynamometerTestModeCannotBeStartedUntilTheEngineIsOffAndTheIgnitionIsOn => ResourceManager.GetString("StringTable.Message_TheDynamometerTestModeCannotBeStartedUntilTheEngineIsOffAndTheIgnitionIsOn");

	internal static string Message_ReadyToStartTheDynamometerTestMode => ResourceManager.GetString("StringTable.Message_ReadyToStartTheDynamometerTestMode");

	internal static string Message_ManualShifting => ResourceManager.GetString("StringTable.Message_ManualShifting");

	internal static string Message_StoppingTheDynoTestModeBecauseTheEngineStopped => ResourceManager.GetString("StringTable.Message_StoppingTheDynoTestModeBecauseTheEngineStopped");

	internal static string Message_WaitingForTheEngineToBeStarted => ResourceManager.GetString("StringTable.Message_WaitingForTheEngineToBeStarted");

	internal static string Message_TheCPCIsOffline => ResourceManager.GetString("StringTable.Message_TheCPCIsOffline");

	internal static string MessageFormat_ErrorStartingDynamometerTestMode0 => ResourceManager.GetString("StringTable.MessageFormat_ErrorStartingDynamometerTestMode0");

	internal static string Message_DynamometerTestModeIsRunning => ResourceManager.GetString("StringTable.Message_DynamometerTestModeIsRunning");

	internal static string Message_WaitingForYouToStartTheEngine => ResourceManager.GetString("StringTable.Message_WaitingForYouToStartTheEngine");

	internal static string Message_DynamometerTestModeHasStopped => ResourceManager.GetString("StringTable.Message_DynamometerTestModeHasStopped");

	internal static string Message_TheEngineHasBeenStoppedWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode => ResourceManager.GetString("StringTable.Message_TheEngineHasBeenStoppedWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode");

	internal static string Message_TheEngineHasBeenStartedAndTheDynamometerTestModeHasStarted => ResourceManager.GetString("StringTable.Message_TheEngineHasBeenStartedAndTheDynamometerTestModeHasStarted");

	internal static string Message_IsNotAvailable => ResourceManager.GetString("StringTable.Message_IsNotAvailable");
}
