using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Workshop_Mode.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_WorkshopModeIsOff => ResourceManager.GetString("StringTable.Message_WorkshopModeIsOff");

	internal static string Message_WorkshopModeOff => ResourceManager.GetString("StringTable.Message_WorkshopModeOff");

	internal static string Message_CTPWorkshopModeHasBeenTurnedOffDiagnosticLinkNeedsToBeClosedAndTheVehicleInterfaceAdaptorNeedsToBeDisconnectedFromTheDiagnosticPort => ResourceManager.GetString("StringTable.Message_CTPWorkshopModeHasBeenTurnedOffDiagnosticLinkNeedsToBeClosedAndTheVehicleInterfaceAdaptorNeedsToBeDisconnectedFromTheDiagnosticPort");

	internal static string Message_UnableToRunRoutine => ResourceManager.GetString("StringTable.Message_UnableToRunRoutine");

	internal static string Message_Ready => ResourceManager.GetString("StringTable.Message_Ready");
}
