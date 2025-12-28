using System.ComponentModel;

namespace DetroitDiesel.FaultCodeTabs.General.Virtual_Technician_Data.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_NoConnectedVIN1 => ResourceManager.GetString("StringTable.Message_NoConnectedVIN1");

	internal static string Message_NoConnectedVIN => ResourceManager.GetString("StringTable.Message_NoConnectedVIN");

	internal static string Message_ConnectedVIN => ResourceManager.GetString("StringTable.Message_ConnectedVIN");

	internal static string Message_HtmlBodyDivH1YouAreNotConnectedToTheInternetPleaseClickTheRefreshButtonAfterConnectingToInternetH1DivBodyHtml => ResourceManager.GetString("StringTable.Message_HtmlBodyDivH1YouAreNotConnectedToTheInternetPleaseClickTheRefreshButtonAfterConnectingToInternetH1DivBodyHtml");

	internal static string Message_ConnectedVIN1 => ResourceManager.GetString("StringTable.Message_ConnectedVIN1");
}
