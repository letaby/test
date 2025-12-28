using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Coolant_Valve_Control__MY20_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_TheValveFailedToClose0 => ResourceManager.GetString("StringTable.MessageFormat_TheValveFailedToClose0");

	internal static string MessageFormat_TheValveFailedToOpen0 => ResourceManager.GetString("StringTable.MessageFormat_TheValveFailedToOpen0");

	internal static string Message_DisconnectionDetectedWhileProcedureRunningDEFCoolantValveMayStillBeOpen => ResourceManager.GetString("StringTable.Message_DisconnectionDetectedWhileProcedureRunningDEFCoolantValveMayStillBeOpen");

	internal static string Message_TheDEFCoolantValveHasBeenSetToClosed => ResourceManager.GetString("StringTable.Message_TheDEFCoolantValveHasBeenSetToClosed");

	internal static string MessageFormat_TheDEFCoolantValveHasBeenSetToOpenFor0Seconds => ResourceManager.GetString("StringTable.MessageFormat_TheDEFCoolantValveHasBeenSetToOpenFor0Seconds");
}
