using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Factory_Reset.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_CTPIsOffline => ResourceManager.GetString("StringTable.Message_CTPIsOffline");

	internal static string Message_ReloadSupplierConfigurationServiceExecuted => ResourceManager.GetString("StringTable.Message_ReloadSupplierConfigurationServiceExecuted");

	internal static string Message_PerformingFactoryResetRoutine => ResourceManager.GetString("StringTable.Message_PerformingFactoryResetRoutine");

	internal static string Message_FactoryResetRoutineCompletedSuccessfully => ResourceManager.GetString("StringTable.Message_FactoryResetRoutineCompletedSuccessfully");

	internal static string Message_None => ResourceManager.GetString("StringTable.Message_None");

	internal static string Message_Finished => ResourceManager.GetString("StringTable.Message_Finished");

	internal static string Message_ReloadSupplierConfigurationExecutedSuccessfully => ResourceManager.GetString("StringTable.Message_ReloadSupplierConfigurationExecutedSuccessfully");

	internal static string Message_Ready => ResourceManager.GetString("StringTable.Message_Ready");

	internal static string Message_FactoryResetRoutineFAILED => ResourceManager.GetString("StringTable.Message_FactoryResetRoutineFAILED");

	internal static string Message_UserRequestedCTPFactoryReset => ResourceManager.GetString("StringTable.Message_UserRequestedCTPFactoryReset");

	internal static string Message_ErrorCannotStartRoutine => ResourceManager.GetString("StringTable.Message_ErrorCannotStartRoutine");

	internal static string Message_ReloadSupplierConfigurationFailed => ResourceManager.GetString("StringTable.Message_ReloadSupplierConfigurationFailed");
}
