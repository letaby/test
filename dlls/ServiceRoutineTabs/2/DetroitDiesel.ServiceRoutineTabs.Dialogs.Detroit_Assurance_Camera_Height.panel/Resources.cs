using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Camera_Height.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_Ready2a => ResourceManager.GetString("StringTable.Message_Ready2a");

	internal static string Message_ClearingFault => ResourceManager.GetString("StringTable.Message_ClearingFault");

	internal static string Message_FailedToWriteParameters => ResourceManager.GetString("StringTable.Message_FailedToWriteParameters");

	internal static string Message_WritingParameter => ResourceManager.GetString("StringTable.Message_WritingParameter");

	internal static string Message_DefaultSettingNotFound => ResourceManager.GetString("StringTable.Message_DefaultSettingNotFound");

	internal static string Message_Ready => ResourceManager.GetString("StringTable.Message_Ready");

	internal static string Message_WaitingForFaultToGoInactive => ResourceManager.GetString("StringTable.Message_WaitingForFaultToGoInactive");

	internal static string Message_Ready1 => ResourceManager.GetString("StringTable.Message_Ready1");
}
