using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__Euro4_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TestAbortedUserCanceledTheTest => ResourceManager.GetString("StringTable.Message_TestAbortedUserCanceledTheTest");

	internal static string Message_TestFailedPressureCouldNotBeRead => ResourceManager.GetString("StringTable.Message_TestFailedPressureCouldNotBeRead");

	internal static string Message_TestFailedLeakWasDetected => ResourceManager.GetString("StringTable.Message_TestFailedLeakWasDetected");

	internal static string Message_EngineIsNotRunningTestCanStart => ResourceManager.GetString("StringTable.Message_EngineIsNotRunningTestCanStart");

	internal static string Message_EngineSpeedCannotBeDetected => ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetected");

	internal static string Message_TestFailedEnigneIsRunning => ResourceManager.GetString("StringTable.Message_TestFailedEnigneIsRunning");

	internal static string Message_EngineIsRunningTestCannotStart => ResourceManager.GetString("StringTable.Message_EngineIsRunningTestCannotStart");

	internal static string MessageFormat_TheFinalLPPOPressureObservedWas0 => ResourceManager.GetString("StringTable.MessageFormat_TheFinalLPPOPressureObservedWas0");

	internal static string MessageFormat_TheInitialLPPOPressureObservedWas0 => ResourceManager.GetString("StringTable.MessageFormat_TheInitialLPPOPressureObservedWas0");

	internal static string Message_TestCompleted => ResourceManager.GetString("StringTable.Message_TestCompleted");

	internal static string Message_TestAbortedDeviceWentOffline => ResourceManager.GetString("StringTable.Message_TestAbortedDeviceWentOffline");

	internal static string Message_WaitingTenMinutesWhileThePressureIsMonitoredForADropOfMoreThan5Psi => ResourceManager.GetString("StringTable.Message_WaitingTenMinutesWhileThePressureIsMonitoredForADropOfMoreThan5Psi");

	internal static string Message_WaitingFiveMinutesToLetTheAirPressureStabilize => ResourceManager.GetString("StringTable.Message_WaitingFiveMinutesToLetTheAirPressureStabilize");

	internal static string Message_TestPassedNoLeakDetected => ResourceManager.GetString("StringTable.Message_TestPassedNoLeakDetected");
}
