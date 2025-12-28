using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DDECDataPages.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_DataPagesDisabled => ResourceManager.GetString("StringTable.Message_DataPagesDisabled");

	internal static string Message_WithNoVIN => ResourceManager.GetString("StringTable.Message_WithNoVIN");

	internal static string Message_ResetFailed => ResourceManager.GetString("StringTable.Message_ResetFailed");

	internal static string Message_SucessExtraction => ResourceManager.GetString("StringTable.Message_SucessExtraction");

	internal static string Message_NotConnected => ResourceManager.GetString("StringTable.Message_NotConnected");

	internal static string Message_OperationtionCpcOffline => ResourceManager.GetString("StringTable.Message_OperationtionCpcOffline");

	internal static string Message_ResettingAllTrips => ResourceManager.GetString("StringTable.Message_ResettingAllTrips");

	internal static string Message_StartingExtraction => ResourceManager.GetString("StringTable.Message_StartingExtraction");

	internal static string Message_CommitServiceFailed => ResourceManager.GetString("StringTable.Message_CommitServiceFailed");

	internal static string Message_ClearPasswordFail => ResourceManager.GetString("StringTable.Message_ClearPasswordFail");

	internal static string Message_ReadingSupportDetails => ResourceManager.GetString("StringTable.Message_ReadingSupportDetails");

	internal static string Message_Vin => ResourceManager.GetString("StringTable.Message_Vin");

	internal static string Message_ResettingTrip => ResourceManager.GetString("StringTable.Message_ResettingTrip");

	internal static string Message_PasswordSet => ResourceManager.GetString("StringTable.Message_PasswordSet");

	internal static string Message_SettingPassword => ResourceManager.GetString("StringTable.Message_SettingPassword");

	internal static string Message_DataPagesEnabled => ResourceManager.GetString("StringTable.Message_DataPagesEnabled");

	internal static string Message_TripsReset => ResourceManager.GetString("StringTable.Message_TripsReset");

	internal static string Message_CPCReset => ResourceManager.GetString("StringTable.Message_CPCReset");

	internal static string Message_Working => ResourceManager.GetString("StringTable.Message_Working");

	internal static string Message_ClearingPassword => ResourceManager.GetString("StringTable.Message_ClearingPassword");

	internal static string Message_SetPasswordCancel => ResourceManager.GetString("StringTable.Message_SetPasswordCancel");

	internal static string Message_PasswordCleared => ResourceManager.GetString("StringTable.Message_PasswordCleared");

	internal static string Message_ExtractionFailed => ResourceManager.GetString("StringTable.Message_ExtractionFailed");

	internal static string Message_WaitingForCPCOnline => ResourceManager.GetString("StringTable.Message_WaitingForCPCOnline");

	internal static string Message_ChangesCommited => ResourceManager.GetString("StringTable.Message_ChangesCommited");

	internal static string Message_LostConnection => ResourceManager.GetString("StringTable.Message_LostConnection");

	internal static string Message_SetPasswordFail => ResourceManager.GetString("StringTable.Message_SetPasswordFail");
}
