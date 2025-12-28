using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_3G_Sundown.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_WritingParameters => ResourceManager.GetString("StringTable.Message_WritingParameters");

	internal static string Message_ErrorWritingParameters => ResourceManager.GetString("StringTable.Message_ErrorWritingParameters");

	internal static string Message_CTPBusy => ResourceManager.GetString("StringTable.Message_CTPBusy");

	internal static string Message_Ready => ResourceManager.GetString("StringTable.Message_Ready");

	internal static string MessageFormat_CTPBusy0 => ResourceManager.GetString("StringTable.MessageFormat_CTPBusy0");
}
