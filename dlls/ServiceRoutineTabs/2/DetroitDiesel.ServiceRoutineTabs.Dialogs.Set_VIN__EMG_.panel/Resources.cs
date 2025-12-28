using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_VIN__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_The01HasNotBeenChangedAndHasAValueOf2 => ResourceManager.GetString("StringTable.MessageFormat_The01HasNotBeenChangedAndHasAValueOf2");

	internal static string Message_Skipping0As0NotConnected => ResourceManager.GetString("StringTable.Message_Skipping0As0NotConnected");

	internal static string Message_ExecutingServices => ResourceManager.GetString("StringTable.Message_ExecutingServices");

	internal static string Message_Executing => ResourceManager.GetString("StringTable.Message_Executing");

	internal static string Message_ServiceError => ResourceManager.GetString("StringTable.Message_ServiceError");

	internal static string Message_ServiceExecuted => ResourceManager.GetString("StringTable.Message_ServiceExecuted");

	internal static string MessageFormat_Setting0VINTo1 => ResourceManager.GetString("StringTable.MessageFormat_Setting0VINTo1");

	internal static string Message_FailedToObtainService => ResourceManager.GetString("StringTable.Message_FailedToObtainService");

	internal static string MessageFormat_FailedToReadECUInfoCannotVerifyThe01Error2 => ResourceManager.GetString("StringTable.MessageFormat_FailedToReadECUInfoCannotVerifyThe01Error2");

	internal static string Message_TheProcedureFailedToComplete => ResourceManager.GetString("StringTable.Message_TheProcedureFailedToComplete");

	internal static string Message_FailedToExecuteService => ResourceManager.GetString("StringTable.Message_FailedToExecuteService");

	internal static string MessageFormat_The01HasSuccessfullyBeenSetTo2 => ResourceManager.GetString("StringTable.MessageFormat_The01HasSuccessfullyBeenSetTo2");

	internal static string Message_Resetting0Faults => ResourceManager.GetString("StringTable.Message_Resetting0Faults");

	internal static string Message_SkippingCPCResetAsTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_SkippingCPCResetAsTheServiceCannotBeFound");

	internal static string Message_VerifyingResults => ResourceManager.GetString("StringTable.Message_VerifyingResults");

	internal static string MessageFormat_01CannotBeVerified => ResourceManager.GetString("StringTable.MessageFormat_01CannotBeVerified");

	internal static string Message_Skipping0ServiceNotFound => ResourceManager.GetString("StringTable.Message_Skipping0ServiceNotFound");

	internal static string Message_CommittingChangesToCPCViaHardReset => ResourceManager.GetString("StringTable.Message_CommittingChangesToCPCViaHardReset");

	internal static string MessageFormat_FailedToReadECUInfoThe0IsUnavailableError1 => ResourceManager.GetString("StringTable.MessageFormat_FailedToReadECUInfoThe0IsUnavailableError1");

	internal static string MessageFormat_TheVINCannotBeSet0 => ResourceManager.GetString("StringTable.MessageFormat_TheVINCannotBeSet0");
}
