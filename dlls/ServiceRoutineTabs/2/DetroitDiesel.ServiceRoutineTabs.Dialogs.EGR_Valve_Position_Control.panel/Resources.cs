using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Position_Control.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_CannotStartAsDeviceIsNotOnline => ResourceManager.GetString("StringTable.Message_CannotStartAsDeviceIsNotOnline");

	internal static string Message_TestCanStart => ResourceManager.GetString("StringTable.Message_TestCanStart");

	internal static string Message_RequestEndEGRManipulation => ResourceManager.GetString("StringTable.Message_RequestEndEGRManipulation");

	internal static string Message_UnableToRequestEndEGRManipulation => ResourceManager.GetString("StringTable.Message_UnableToRequestEndEGRManipulation");

	internal static string Message_CannotStartAsDeviceIsBusy => ResourceManager.GetString("StringTable.Message_CannotStartAsDeviceIsBusy");

	internal static string Message_SetPosition => ResourceManager.GetString("StringTable.Message_SetPosition");

	internal static string Message_TestIsInProgress => ResourceManager.GetString("StringTable.Message_TestIsInProgress");

	internal static string Message_CannotStartAsPositionNotValid => ResourceManager.GetString("StringTable.Message_CannotStartAsPositionNotValid");

	internal static string Message_TestFailed => ResourceManager.GetString("StringTable.Message_TestFailed");

	internal static string Message_CannotStartAsEngineIsRunningStopEngine => ResourceManager.GetString("StringTable.Message_CannotStartAsEngineIsRunningStopEngine");

	internal static string Message_EngineStateIncorrect => ResourceManager.GetString("StringTable.Message_EngineStateIncorrect");

	internal static string Message_ECUIsBusy => ResourceManager.GetString("StringTable.Message_ECUIsBusy");

	internal static string Message_TestStarted => ResourceManager.GetString("StringTable.Message_TestStarted");

	internal static string Message_TestCompleteDueToUserRequest => ResourceManager.GetString("StringTable.Message_TestCompleteDueToUserRequest");

	internal static string Message_TestComplete => ResourceManager.GetString("StringTable.Message_TestComplete");

	internal static string Message_ECUOfflineTestAborted => ResourceManager.GetString("StringTable.Message_ECUOfflineTestAborted");
}
