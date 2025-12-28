using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Active_Lube_Management.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_EngineIsNotRunning => ResourceManager.GetString("StringTable.Message_EngineIsNotRunning");

	internal static string Message_CurrentAlmValues => ResourceManager.GetString("StringTable.Message_CurrentAlmValues");

	internal static string Message_SSAM02TIsNotConnected => ResourceManager.GetString("StringTable.Message_SSAM02TIsNotConnected");

	internal static string Message_EngineSpeedCannotBeDetected => ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetected");

	internal static string Message_SettingDefaultValues => ResourceManager.GetString("StringTable.Message_SettingDefaultValues");

	internal static string Message_SSAM02TIsBusy => ResourceManager.GetString("StringTable.Message_SSAM02TIsBusy");

	internal static string Message_ThisSSAMDoesNotHaveTheNeededParameters => ResourceManager.GetString("StringTable.Message_ThisSSAMDoesNotHaveTheNeededParameters");

	internal static string Message_EngineIsRunning => ResourceManager.GetString("StringTable.Message_EngineIsRunning");

	internal static string Message_DisconnectionDetected => ResourceManager.GetString("StringTable.Message_DisconnectionDetected");

	internal static string Message_SSAM02TIsConnected => ResourceManager.GetString("StringTable.Message_SSAM02TIsConnected");

	internal static string Message_InitailParameterValues => ResourceManager.GetString("StringTable.Message_InitailParameterValues");

	internal static string Message_ParametersSetToDefaultValues => ResourceManager.GetString("StringTable.Message_ParametersSetToDefaultValues");

	internal static string Message_Finished => ResourceManager.GetString("StringTable.Message_Finished");

	internal static string Message_RemainInTestMode => ResourceManager.GetString("StringTable.Message_RemainInTestMode");

	internal static string Message_ResettingParameters => ResourceManager.GetString("StringTable.Message_ResettingParameters");

	internal static string Message_EnteringTestMode => ResourceManager.GetString("StringTable.Message_EnteringTestMode");

	internal static string Message_ParametersReset => ResourceManager.GetString("StringTable.Message_ParametersReset");

	internal static string Message_ActiveLubeManagementNotEnabled => ResourceManager.GetString("StringTable.Message_ActiveLubeManagementNotEnabled");
}
