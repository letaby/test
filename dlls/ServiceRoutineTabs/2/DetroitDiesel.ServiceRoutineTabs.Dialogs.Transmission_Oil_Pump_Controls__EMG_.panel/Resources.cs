using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Oil_Pump_Controls__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_OilPump2_Stopped => ResourceManager.GetString("StringTable.Message_OilPump2_Stopped");

	internal static string Message_OilPump2_Started => ResourceManager.GetString("StringTable.Message_OilPump2_Started");

	internal static string Message_OilPump1_FailedToStart => ResourceManager.GetString("StringTable.Message_OilPump1_FailedToStart");

	internal static string Message_OilPump1_Stopped => ResourceManager.GetString("StringTable.Message_OilPump1_Stopped");

	internal static string Message_OilPump1_Started => ResourceManager.GetString("StringTable.Message_OilPump1_Started");

	internal static string Message_OilPump2_FailedToStart => ResourceManager.GetString("StringTable.Message_OilPump2_FailedToStart");
}
