using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Gear_Split_Range_Activation.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_CanNotExitUntilControlIsReturnedToTheVehicle => ResourceManager.GetString("StringTable.Message_CanNotExitUntilControlIsReturnedToTheVehicle");
}
