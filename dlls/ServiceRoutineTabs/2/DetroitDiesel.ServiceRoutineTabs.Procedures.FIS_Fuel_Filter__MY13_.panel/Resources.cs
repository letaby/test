using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_NoFault => ResourceManager.GetString("StringTable.Message_NoFault");

	internal static string MessageFormat_ValuesReset => ResourceManager.GetString("StringTable.MessageFormat_ValuesReset");

	internal static string Message_Not => ResourceManager.GetString("StringTable.Message_Not");

	internal static string FuelFilterCalculationIs0Active => ResourceManager.GetString("StringTable.FuelFilterCalculationIs0Active");
}
