using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.GPRS_Configuration.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ConnectedCTP01TIsAllowed => ResourceManager.GetString("StringTable.Message_ConnectedCTP01TIsAllowed");

	internal static string Message_GPRSConfigurationCanNotBeRead => ResourceManager.GetString("StringTable.Message_GPRSConfigurationCanNotBeRead");

	internal static string Message_CouldNotFindGPRSWriteService => ResourceManager.GetString("StringTable.Message_CouldNotFindGPRSWriteService");

	internal static string Message_ConnectedCTP01THardwareIsNotSupported => ResourceManager.GetString("StringTable.Message_ConnectedCTP01THardwareIsNotSupported");

	internal static string Message_GPRSConfigurationWasSuccessfullyUpdated => ResourceManager.GetString("StringTable.Message_GPRSConfigurationWasSuccessfullyUpdated");

	internal static string Message_UnknownErrorWhenExecutingGPRSWrite => ResourceManager.GetString("StringTable.Message_UnknownErrorWhenExecutingGPRSWrite");

	internal static string Message_CouldNotFindResetServiceToCommitChanges => ResourceManager.GetString("StringTable.Message_CouldNotFindResetServiceToCommitChanges");

	internal static string Message_CTP01TDeviceChangedDuringProcess => ResourceManager.GetString("StringTable.Message_CTP01TDeviceChangedDuringProcess");

	internal static string Message_GPRSConfigurationIsCORRECT => ResourceManager.GetString("StringTable.Message_GPRSConfigurationIsCORRECT");

	internal static string Message_WritingGPRSConfigurationValue => ResourceManager.GetString("StringTable.Message_WritingGPRSConfigurationValue");

	internal static string Message_None => ResourceManager.GetString("StringTable.Message_None");

	internal static string Message_AntennaDiagnosticsEnableFailedToWrite => ResourceManager.GetString("StringTable.Message_AntennaDiagnosticsEnableFailedToWrite");

	internal static string Message_CannotConfirmConnectedCTP01THardwareVersion => ResourceManager.GetString("StringTable.Message_CannotConfirmConnectedCTP01THardwareVersion");

	internal static string Message_ConnectedCTP01TSoftwareIsNotSupported => ResourceManager.GetString("StringTable.Message_ConnectedCTP01TSoftwareIsNotSupported");

	internal static string Message_EnablingAntennaDiagnosticsFormat => ResourceManager.GetString("StringTable.Message_EnablingAntennaDiagnosticsFormat");

	internal static string Message_ResetToCommitChangesFailedToComplete => ResourceManager.GetString("StringTable.Message_ResetToCommitChangesFailedToComplete");

	internal static string Message_ResettingDeviceToCommitChanges => ResourceManager.GetString("StringTable.Message_ResettingDeviceToCommitChanges");

	internal static string Message_FailedToUpdateGPRSConfiguration => ResourceManager.GetString("StringTable.Message_FailedToUpdateGPRSConfiguration");

	internal static string Message_ProcessCompletedSuccessfully => ResourceManager.GetString("StringTable.Message_ProcessCompletedSuccessfully");

	internal static string Message_GPRSNotApplicableToCurrentUnit => ResourceManager.GetString("StringTable.Message_GPRSNotApplicableToCurrentUnit");

	internal static string Message_AntennaDiagnosticsHaveBeenEnabled => ResourceManager.GetString("StringTable.Message_AntennaDiagnosticsHaveBeenEnabled");

	internal static string Message_GPRSConfigurationIsNOTCORRECT => ResourceManager.GetString("StringTable.Message_GPRSConfigurationIsNOTCORRECT");

	internal static string Message_CannotConfirmConnectedCTP01TSoftwareVersion => ResourceManager.GetString("StringTable.Message_CannotConfirmConnectedCTP01TSoftwareVersion");
}
