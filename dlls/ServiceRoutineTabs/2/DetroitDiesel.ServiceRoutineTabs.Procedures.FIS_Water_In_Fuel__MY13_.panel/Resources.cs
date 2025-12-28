using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Water_In_Fuel__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_WaterInFuelRegistersHaveBeenReset => ResourceManager.GetString("StringTable.Message_WaterInFuelRegistersHaveBeenReset");

	internal static string Message_ErrorCouldNotFindTheServiceRoutineRT_SR0AB_Reset_Water_In_Fuel_Values_Start0 => ResourceManager.GetString("StringTable.Message_ErrorCouldNotFindTheServiceRoutineRT_SR0AB_Reset_Water_In_Fuel_Values_Start0");
}
