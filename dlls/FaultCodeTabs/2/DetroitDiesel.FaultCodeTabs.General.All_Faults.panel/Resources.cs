using System.ComponentModel;

namespace DetroitDiesel.FaultCodeTabs.General.All_Faults.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_EngineeringTroubleshooting => ResourceManager.GetString("StringTable.Message_EngineeringTroubleshooting");

	internal static string Message_TraditionalTroubleshooting => ResourceManager.GetString("StringTable.Message_TraditionalTroubleshooting");

	internal static string Message_ReferToTroubleshootingView => ResourceManager.GetString("StringTable.Message_ReferToTroubleshootingView");

	internal static string Message_ReferToTechLit => ResourceManager.GetString("StringTable.Message_ReferToTechLit");

	internal static string Message_AdvancedDiagnostics => ResourceManager.GetString("StringTable.Message_AdvancedDiagnostics");
}
