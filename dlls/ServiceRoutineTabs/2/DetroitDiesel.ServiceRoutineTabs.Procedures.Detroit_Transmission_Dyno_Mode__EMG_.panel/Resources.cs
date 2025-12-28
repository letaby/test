using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TheDynamometerTestModeCannotBeStartedUntilTheIgnitionIsOn => ResourceManager.GetString("StringTable.Message_TheDynamometerTestModeCannotBeStartedUntilTheIgnitionIsOn");

	internal static string Message_EndingTheTestTheService => ResourceManager.GetString("StringTable.Message_EndingTheTestTheService");

	internal static string Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit => ResourceManager.GetString("StringTable.Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit");

	internal static string Message_TheStartButtonHasBeenPressed => ResourceManager.GetString("StringTable.Message_TheStartButtonHasBeenPressed");

	internal static string Message_ReadyToStartTheDynamometerTestMode => ResourceManager.GetString("StringTable.Message_ReadyToStartTheDynamometerTestMode");

	internal static string Value_FirstRearDriveAxleActive => ResourceManager.GetString("StringTable.Value_FirstRearDriveAxleActive");

	internal static string Message_TheCPCIsOffline => ResourceManager.GetString("StringTable.Message_TheCPCIsOffline");

	internal static string MessageFormat_ErrorStartingDynamometerTestMode0 => ResourceManager.GetString("StringTable.MessageFormat_ErrorStartingDynamometerTestMode0");

	internal static string Value_SecondRearDriveAxleActive => ResourceManager.GetString("StringTable.Value_SecondRearDriveAxleActive");

	internal static string Message_DynamometerTestModeIsRunning => ResourceManager.GetString("StringTable.Message_DynamometerTestModeIsRunning");

	internal static string Value_TwoRearDriveAxles => ResourceManager.GetString("StringTable.Value_TwoRearDriveAxles");

	internal static string Message_TheIgnitionHasBeenTurnedOffWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode => ResourceManager.GetString("StringTable.Message_TheIgnitionHasBeenTurnedOffWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode");

	internal static string Message_DynamometerTestModeHasStopped => ResourceManager.GetString("StringTable.Message_DynamometerTestModeHasStopped");

	internal static string Value_None => ResourceManager.GetString("StringTable.Value_None");

	internal static string Message_IsNotAvailable => ResourceManager.GetString("StringTable.Message_IsNotAvailable");
}
