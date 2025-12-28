using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ESC_Learn_Procedure.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ProcedureCanStart => ResourceManager.GetString("StringTable.Message_ProcedureCanStart");

	internal static string Message_ErrorStoppingProcedure => ResourceManager.GetString("StringTable.Message_ErrorStoppingProcedure");

	internal static string Message_ProcedureStopped => ResourceManager.GetString("StringTable.Message_ProcedureStopped");

	internal static string Message_ProcedureStarted => ResourceManager.GetString("StringTable.Message_ProcedureStarted");

	internal static string Message_ESCLearnProcedureComplete => ResourceManager.GetString("StringTable.Message_ESCLearnProcedureComplete");

	internal static string Message_StartDrivingTheTruck => ResourceManager.GetString("StringTable.Message_StartDrivingTheTruck");

	internal static string Message_ErrorStartingProcedure => ResourceManager.GetString("StringTable.Message_ErrorStartingProcedure");

	internal static string Message_YouMayStopTheVehicle => ResourceManager.GetString("StringTable.Message_YouMayStopTheVehicle");

	internal static string Message_ProcedureRunning => ResourceManager.GetString("StringTable.Message_ProcedureRunning");

	internal static string Message_WheelBasedSpeedMustBeZero => ResourceManager.GetString("StringTable.Message_WheelBasedSpeedMustBeZero");
}
