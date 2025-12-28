using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Accumulated_Fuel_Mass_Sync__MY17_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ATSSet => ResourceManager.GetString("StringTable.Message_ATSSet");

	internal static string Message_ReadyToSetATSDistance => ResourceManager.GetString("StringTable.Message_ReadyToSetATSDistance");

	internal static string Message_ACMNotOnline => ResourceManager.GetString("StringTable.Message_ACMNotOnline");

	internal static string Message_ChooseAction => ResourceManager.GetString("StringTable.Message_ChooseAction");

	internal static string MessageFormat_SettingDistanceTo01 => ResourceManager.GetString("StringTable.MessageFormat_SettingDistanceTo01");

	internal static string MessageFormat_PleaseSpecifyAnATSDistanceBetween0And01 => ResourceManager.GetString("StringTable.MessageFormat_PleaseSpecifyAnATSDistanceBetween0And01");

	internal static string Message_MCMNotOnline => ResourceManager.GetString("StringTable.Message_MCMNotOnline");
}
