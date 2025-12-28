using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Start_Reset__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_Failed0 => ResourceManager.GetString("StringTable.MessageFormat_Failed0");

	internal static string Message_CouldNotReturnControl => ResourceManager.GetString("StringTable.Message_CouldNotReturnControl");

	internal static string Message_Running => ResourceManager.GetString("StringTable.Message_Running");

	internal static string Message_Unknown => ResourceManager.GetString("StringTable.Message_Unknown");

	internal static string MessageFormat_CouldNotReturnControl0 => ResourceManager.GetString("StringTable.MessageFormat_CouldNotReturnControl0");

	internal static string Message_NotReady => ResourceManager.GetString("StringTable.Message_NotReady");

	internal static string Message_CouldNotStart => ResourceManager.GetString("StringTable.Message_CouldNotStart");

	internal static string Message_ReadyToRun => ResourceManager.GetString("StringTable.Message_ReadyToRun");

	internal static string Message_ReturningControl => ResourceManager.GetString("StringTable.Message_ReturningControl");

	internal static string Message_ResetComplete => ResourceManager.GetString("StringTable.Message_ResetComplete");

	internal static string Message_Unknown1 => ResourceManager.GetString("StringTable.Message_Unknown1");
}
