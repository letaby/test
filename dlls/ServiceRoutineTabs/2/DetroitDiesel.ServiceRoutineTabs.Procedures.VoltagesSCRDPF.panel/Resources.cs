using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.VoltagesSCRDPF.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_StartedAcquiringSensorVoltageSignals => ResourceManager.GetString("StringTable.Message_StartedAcquiringSensorVoltageSignals");

	internal static string Message_StoppedAcquiringSensorVoltageSignals => ResourceManager.GetString("StringTable.Message_StoppedAcquiringSensorVoltageSignals");
}
