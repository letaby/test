using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Inverter_Resolver_Learn__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string BlackWarning => ResourceManager.GetString("StringTable.BlackWarning");

	internal static string WarningText => ResourceManager.GetString("StringTable.WarningText");

	internal static string RedWarning => ResourceManager.GetString("StringTable.RedWarning");

	internal static string Message_Resolver_2_Learn_Routine_Stopped => ResourceManager.GetString("StringTable.Message_Resolver_2_Learn_Routine_Stopped");

	internal static string Message_Resolver_2_Learn_Routine_Started => ResourceManager.GetString("StringTable.Message_Resolver_2_Learn_Routine_Started");

	internal static string Message_Resolver_3_Learn_Routine_FailedToStart => ResourceManager.GetString("StringTable.Message_Resolver_3_Learn_Routine_FailedToStart");

	internal static string Message_Resolver_2_Learn_Routine_FailedToStart => ResourceManager.GetString("StringTable.Message_Resolver_2_Learn_Routine_FailedToStart");

	internal static string Message_Resolver_1_Learn_Routine_Stopped => ResourceManager.GetString("StringTable.Message_Resolver_1_Learn_Routine_Stopped");

	internal static string Message_Resolver_1_Learn_Routine_Started => ResourceManager.GetString("StringTable.Message_Resolver_1_Learn_Routine_Started");

	internal static string Message_Resolver_1_Learn_Routine_FailedToStart => ResourceManager.GetString("StringTable.Message_Resolver_1_Learn_Routine_FailedToStart");

	internal static string Message_Resolver_3_Learn_Routine_Stopped => ResourceManager.GetString("StringTable.Message_Resolver_3_Learn_Routine_Stopped");

	internal static string Message_Resolver_3_Learn_Routine_Started => ResourceManager.GetString("StringTable.Message_Resolver_3_Learn_Routine_Started");

	internal static string WarningText_Inverter_3 => ResourceManager.GetString("StringTable.WarningText_Inverter_3");
}
