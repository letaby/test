using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Check_VIN_Synchronization.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_WaitingForDevicesToReconnect => ResourceManager.GetString("StringTable.Message_WaitingForDevicesToReconnect");

	internal static string Message_ConnectAtLeastTwoDevicesToDetermineVINSynchronization => ResourceManager.GetString("StringTable.Message_ConnectAtLeastTwoDevicesToDetermineVINSynchronization");

	internal static string Message_ProcessingPleaseWait => ResourceManager.GetString("StringTable.Message_ProcessingPleaseWait");

	internal static string Message_UnknownStage => ResourceManager.GetString("StringTable.Message_UnknownStage");

	internal static string Message_TheVINsInThisVehicleAreSynchronizedNoActionIsNecessaryIfTheVINIsIncorrectYouWillNeedToReprogramUsingServerData => ResourceManager.GetString("StringTable.Message_TheVINsInThisVehicleAreSynchronizedNoActionIsNecessaryIfTheVINIsIncorrectYouWillNeedToReprogramUsingServerData");

	internal static string MessageFormat_FailedToWriteVINFor0 => ResourceManager.GetString("StringTable.MessageFormat_FailedToWriteVINFor0");

	internal static string Message_PleaseTurnTheIgnitionOffAndWait => ResourceManager.GetString("StringTable.Message_PleaseTurnTheIgnitionOffAndWait");

	internal static string Message_WaitingForRemainingDevicesToShutdownPleaseWait => ResourceManager.GetString("StringTable.Message_WaitingForRemainingDevicesToShutdownPleaseWait");

	internal static string Message_ItWasNotNecessaryToChangeAnyVINs => ResourceManager.GetString("StringTable.Message_ItWasNotNecessaryToChangeAnyVINs");

	internal static string MessageFormat_TheVINsInThisVehicleAreNotSynchronizedClickStartToCopyTheVINFrom0ToTheOtherDevices => ResourceManager.GetString("StringTable.MessageFormat_TheVINsInThisVehicleAreNotSynchronizedClickStartToCopyTheVINFrom0ToTheOtherDevices");

	internal static string Message_TheOperationWasAborted => ResourceManager.GetString("StringTable.Message_TheOperationWasAborted");

	internal static string Message_WaitingForDevicesToDisconnect => ResourceManager.GetString("StringTable.Message_WaitingForDevicesToDisconnect");

	internal static string Message_WaitingForRemainingDevicesToComeOnlinePleaseWait => ResourceManager.GetString("StringTable.Message_WaitingForRemainingDevicesToComeOnlinePleaseWait");

	internal static string MessageFormat_SuccessfullyWroteVINFor0 => ResourceManager.GetString("StringTable.MessageFormat_SuccessfullyWroteVINFor0");

	internal static string Message_TheProcedureFailedToComplete => ResourceManager.GetString("StringTable.Message_TheProcedureFailedToComplete");

	internal static string Message_Complete => ResourceManager.GetString("StringTable.Message_Complete");

	internal static string Message_ThereAreNoDevicesOnline => ResourceManager.GetString("StringTable.Message_ThereAreNoDevicesOnline");

	internal static string Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseTheVINMasterDeviceDoesNotHaveAVIN => ResourceManager.GetString("StringTable.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseTheVINMasterDeviceDoesNotHaveAVIN");

	internal static string Message_ADeviceWasDisconnected => ResourceManager.GetString("StringTable.Message_ADeviceWasDisconnected");

	internal static string Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseMultipleVINMasterDevicesAreDefinedAndConnected => ResourceManager.GetString("StringTable.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseMultipleVINMasterDevicesAreDefinedAndConnected");

	internal static string Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseNoVINMasterDeviceIsDefinedOrConnected => ResourceManager.GetString("StringTable.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseNoVINMasterDeviceIsDefinedOrConnected");

	internal static string Message_PleaseTurnTheIgnitionOnAndWait => ResourceManager.GetString("StringTable.Message_PleaseTurnTheIgnitionOnAndWait");

	internal static string Message_Start => ResourceManager.GetString("StringTable.Message_Start");

	internal static string Message_Retry => ResourceManager.GetString("StringTable.Message_Retry");

	internal static string MessageFormat_UpdatingVINFor0 => ResourceManager.GetString("StringTable.MessageFormat_UpdatingVINFor0");

	internal static string Message_NoVINWasFoundInTheVINMasterDevice => ResourceManager.GetString("StringTable.Message_NoVINWasFoundInTheVINMasterDevice");
}
