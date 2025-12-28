using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Three_By_Two_Way_Valve_Teach_In__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_LearnIncompleteRunProcedureAgain => ResourceManager.GetString("StringTable.Message_LearnIncompleteRunProcedureAgain");

	internal static string Message_LearnCompletedSuccessfully => ResourceManager.GetString("StringTable.Message_LearnCompletedSuccessfully");

	internal static string Message_BatteryCoolantSystem3by2WayValveTeachInStopped => ResourceManager.GetString("StringTable.Message_BatteryCoolantSystem3by2WayValveTeachInStopped");

	internal static string Message_BatteryCoolantSystem3by2WayValveTeachInStarted => ResourceManager.GetString("StringTable.Message_BatteryCoolantSystem3by2WayValveTeachInStarted");

	internal static string Message_EdriveCoolantSystem3by2WayValveTeachInFailedToStart => ResourceManager.GetString("StringTable.Message_EdriveCoolantSystem3by2WayValveTeachInFailedToStart");

	internal static string Message_BatteryCoolantSystem3by2WayValveTeachInFailedToStart => ResourceManager.GetString("StringTable.Message_BatteryCoolantSystem3by2WayValveTeachInFailedToStart");

	internal static string Message_ValveLearning => ResourceManager.GetString("StringTable.Message_ValveLearning");

	internal static string Message_EdriveCoolantSystem3by2WayValveTeachInStopped => ResourceManager.GetString("StringTable.Message_EdriveCoolantSystem3by2WayValveTeachInStopped");

	internal static string Message_EdriveCoolantSystem3by2WayValveTeachInStarted => ResourceManager.GetString("StringTable.Message_EdriveCoolantSystem3by2WayValveTeachInStarted");
}
