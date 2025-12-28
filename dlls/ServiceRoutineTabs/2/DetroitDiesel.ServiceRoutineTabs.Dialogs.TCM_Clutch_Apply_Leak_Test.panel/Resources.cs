using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clutch_Apply_Leak_Test.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_IntialClutchPostionObserved0 => ResourceManager.GetString("StringTable.MessageFormat_IntialClutchPostionObserved0");

	internal static string Message_TestCanStart => ResourceManager.GetString("StringTable.Message_TestCanStart");

	internal static string Message_WaitingToRecordSampleClutchPositionValue => ResourceManager.GetString("StringTable.Message_WaitingToRecordSampleClutchPositionValue");

	internal static string Message_CannotStartTheTestAsTheDeviceIsBusy => ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheDeviceIsBusy");

	internal static string Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionIsInNeutral => ResourceManager.GetString("StringTable.Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionIsInNeutral");

	internal static string Message_RequestingDesiredValueRequirementClutchStopService => ResourceManager.GetString("StringTable.Message_RequestingDesiredValueRequirementClutchStopService");

	internal static string Message_ErrorExecutingDesiredValueRequirementClutchRequestService => ResourceManager.GetString("StringTable.Message_ErrorExecutingDesiredValueRequirementClutchRequestService");

	internal static string Message_CannotStartTheTestAsAirSupplyPressureIsBeGreaterThan90Psi => ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsAirSupplyPressureIsBeGreaterThan90Psi");

	internal static string Message_ErrorExecutingDesiredValueRequirementClutchStartService => ResourceManager.GetString("StringTable.Message_ErrorExecutingDesiredValueRequirementClutchStartService");

	internal static string Message_TestInProgress => ResourceManager.GetString("StringTable.Message_TestInProgress");

	internal static string Message_LeakFoundTestFailed => ResourceManager.GetString("StringTable.Message_LeakFoundTestFailed");

	internal static string Message_NoLeakFoundTestPassed => ResourceManager.GetString("StringTable.Message_NoLeakFoundTestPassed");

	internal static string Message_CannotStartTheTestAsTheEngineIsRunningStopTheEngine => ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheEngineIsRunningStopTheEngine");

	internal static string Message_RequestingDesiredValueRequirementClutchStartService => ResourceManager.GetString("StringTable.Message_RequestingDesiredValueRequirementClutchStartService");

	internal static string Message_ErrorStoppingDesiredValueRequirementClutchStopService => ResourceManager.GetString("StringTable.Message_ErrorStoppingDesiredValueRequirementClutchStopService");

	internal static string Message_StartingClutchApplyLeakTest => ResourceManager.GetString("StringTable.Message_StartingClutchApplyLeakTest");

	internal static string MessageFormat_FinalClutchPostionObserved0 => ResourceManager.GetString("StringTable.MessageFormat_FinalClutchPostionObserved0");

	internal static string Message_CannotStartTheTestAsTheDeviceIsNotOnline => ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheDeviceIsNotOnline");

	internal static string Message_CompletedClutchApplyLeakTest => ResourceManager.GetString("StringTable.Message_CompletedClutchApplyLeakTest");
}
