using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_NoFault => ResourceManager.GetString("StringTable.Message_NoFault");

	internal static string FuelFilterCalculationIs0Active0 => ResourceManager.GetString("StringTable.FuelFilterCalculationIs0Active0");

	internal static string MessageFormat_ValuesReset => ResourceManager.GetString("StringTable.MessageFormat_ValuesReset");

	internal static string Message_Not => ResourceManager.GetString("StringTable.Message_Not");
}
