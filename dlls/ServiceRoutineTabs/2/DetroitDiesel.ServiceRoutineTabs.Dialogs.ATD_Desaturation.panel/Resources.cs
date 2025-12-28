using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Desaturation.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ProcedureStillRunningCannotCloseDialog => ResourceManager.GetString("StringTable.Message_ProcedureStillRunningCannotCloseDialog");

	internal static string Message_HighTemperatureWarningDialogClosed => ResourceManager.GetString("StringTable.Message_HighTemperatureWarningDialogClosed");

	internal static string Message_InstructionText => ResourceManager.GetString("StringTable.Message_InstructionText");

	internal static string Message_ATDDesaturation => ResourceManager.GetString("StringTable.Message_ATDDesaturation");

	internal static string Message_InstructionsAcknowledged => ResourceManager.GetString("StringTable.Message_InstructionsAcknowledged");

	internal static string Message_TemperaturesAreTooHighCannotCloseDialog => ResourceManager.GetString("StringTable.Message_TemperaturesAreTooHighCannotCloseDialog");

	internal static string Message_WarningTitle => ResourceManager.GetString("StringTable.Message_WarningTitle");

	internal static string Message_WarningText => ResourceManager.GetString("StringTable.Message_WarningText");

	internal static string Message_InstructionTitle => ResourceManager.GetString("StringTable.Message_InstructionTitle");
}
