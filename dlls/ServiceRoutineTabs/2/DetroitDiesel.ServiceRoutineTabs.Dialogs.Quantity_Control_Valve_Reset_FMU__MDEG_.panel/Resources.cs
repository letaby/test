using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Quantity_Control_Valve_Reset_FMU__MDEG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_SuccessfulExecution => ResourceManager.GetString("StringTable.Message_SuccessfulExecution");

	internal static string Message_Error => ResourceManager.GetString("StringTable.Message_Error");

	internal static string Label_ClickButtonToResetFMU => ResourceManager.GetString("StringTable.Label_ClickButtonToResetFMU");

	internal static string Message_FailedExecution => ResourceManager.GetString("StringTable.Message_FailedExecution");
}
