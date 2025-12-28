using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_CycleTheIgnitionToCompleteTheProcess => ResourceManager.GetString("StringTable.Message_CycleTheIgnitionToCompleteTheProcess");

	internal static string Message_ResetEcu => ResourceManager.GetString("StringTable.Message_ResetEcu");

	internal static string Message_FailedToReadUnlockInputDetails => ResourceManager.GetString("StringTable.Message_FailedToReadUnlockInputDetails");

	internal static string Message_FailedToEnableXCPCCP => ResourceManager.GetString("StringTable.Message_FailedToEnableXCPCCP");

	internal static string Message_VeDocUnlockHasFailedCheckInputsAndTryAgain => ResourceManager.GetString("StringTable.Message_VeDocUnlockHasFailedCheckInputsAndTryAgain");

	internal static string Message_SwitchToServerUnlock => ResourceManager.GetString("StringTable.Message_SwitchToServerUnlock");

	internal static string Message_EcuIsReseting => ResourceManager.GetString("StringTable.Message_EcuIsReseting");

	internal static string Message_XCPCCPHasBeenDisabled => ResourceManager.GetString("StringTable.Message_XCPCCPHasBeenDisabled");

	internal static string Message_XCPCCPHasBeenEnabled => ResourceManager.GetString("StringTable.Message_XCPCCPHasBeenEnabled");

	internal static string Message_XCPCCPActivated => ResourceManager.GetString("StringTable.Message_XCPCCPActivated");

	internal static string Message_EnablingXCPCCP => ResourceManager.GetString("StringTable.Message_EnablingXCPCCP");

	internal static string Message_VeDocUnlockRequiredForActivationButToolIsNotPermittedToPerformAction => ResourceManager.GetString("StringTable.Message_VeDocUnlockRequiredForActivationButToolIsNotPermittedToPerformAction");

	internal static string Message_VeDocUnlockRequiredForActivation => ResourceManager.GetString("StringTable.Message_VeDocUnlockRequiredForActivation");

	internal static string Message_DisableXCPCCP => ResourceManager.GetString("StringTable.Message_DisableXCPCCP");

	internal static string Message_CommitServiceFailed => ResourceManager.GetString("StringTable.Message_CommitServiceFailed");

	internal static string Message_OperationCompleted => ResourceManager.GetString("StringTable.Message_OperationCompleted");

	internal static string Message_SwitchToManualUnlock => ResourceManager.GetString("StringTable.Message_SwitchToManualUnlock");

	internal static string Message_NullValue => ResourceManager.GetString("StringTable.Message_NullValue");

	internal static string Message_ServiceNotSupportedByCurrentlyConnectSoftware => ResourceManager.GetString("StringTable.Message_ServiceNotSupportedByCurrentlyConnectSoftware");

	internal static string Message_ReadingUnlockInputDetails => ResourceManager.GetString("StringTable.Message_ReadingUnlockInputDetails");

	internal static string Message_UnlockInputDetailsReadSuccessfully => ResourceManager.GetString("StringTable.Message_UnlockInputDetailsReadSuccessfully");

	internal static string Message_AttemptingToUnlockController => ResourceManager.GetString("StringTable.Message_AttemptingToUnlockController");

	internal static string Message_FailedToDisableXCPCCP => ResourceManager.GetString("StringTable.Message_FailedToDisableXCPCCP");

	internal static string Message_NotSupportedOrRequired => ResourceManager.GetString("StringTable.Message_NotSupportedOrRequired");

	internal static string Message_ChangesCommited => ResourceManager.GetString("StringTable.Message_ChangesCommited");

	internal static string Message_EcuCouldNotBeReset => ResourceManager.GetString("StringTable.Message_EcuCouldNotBeReset");

	internal static string Message_UnlockSuccessful => ResourceManager.GetString("StringTable.Message_UnlockSuccessful");

	internal static string Message_CommitingChanges => ResourceManager.GetString("StringTable.Message_CommitingChanges");
}
