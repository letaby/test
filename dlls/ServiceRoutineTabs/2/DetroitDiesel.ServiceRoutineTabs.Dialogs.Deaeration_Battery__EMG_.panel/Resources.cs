using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Deaeration_Battery__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TheParkBrakeMustBeSetOrTheVehicleSpeedMustBe0 => ResourceManager.GetString("StringTable.Message_TheParkBrakeMustBeSetOrTheVehicleSpeedMustBe0");

	internal static string Message_EstimatedRuntime => ResourceManager.GetString("StringTable.Message_EstimatedRuntime");

	internal static string Message_Complete => ResourceManager.GetString("StringTable.Message_Complete");

	internal static string Message_DeAirationFailedToStart => ResourceManager.GetString("StringTable.Message_DeAirationFailedToStart");

	internal static string Message_Min => ResourceManager.GetString("StringTable.Message_Min");

	internal static string Message_DeAirationStarted => ResourceManager.GetString("StringTable.Message_DeAirationStarted");

	internal static string Message_DeAirationStopped => ResourceManager.GetString("StringTable.Message_DeAirationStopped");

	internal static string Message_Ready => ResourceManager.GetString("StringTable.Message_Ready");
}
