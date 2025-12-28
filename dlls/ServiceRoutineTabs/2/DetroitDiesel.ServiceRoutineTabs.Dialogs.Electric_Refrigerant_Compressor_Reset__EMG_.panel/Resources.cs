using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Electric_Refrigerant_Compressor_Reset__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageErcResetFailed => ResourceManager.GetString("StringTable.MessageErcResetFailed");

	internal static string MessageErcResetPassed => ResourceManager.GetString("StringTable.MessageErcResetPassed");

	internal static string MessageErcResetStopped => ResourceManager.GetString("StringTable.MessageErcResetStopped");

	internal static string MessageErcResetStarted => ResourceManager.GetString("StringTable.MessageErcResetStarted");

	internal static string MessageErcResetStartFailed => ResourceManager.GetString("StringTable.MessageErcResetStartFailed");
}
