using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VRDU_Snapshot.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_ExtractingDiagnosticLinkQualifiers => ResourceManager.GetString("StringTable.Message_ExtractingDiagnosticLinkQualifiers");

	internal static string Message_StartingVRDUExtraction => ResourceManager.GetString("StringTable.Message_StartingVRDUExtraction");

	internal static string Message_VRDUNotReady => ResourceManager.GetString("StringTable.Message_VRDUNotReady");

	internal static string Message_ProcessingComplete => ResourceManager.GetString("StringTable.Message_ProcessingComplete");

	internal static string MessageFormat_WritingExcelCompatableXmlDataFile0 => ResourceManager.GetString("StringTable.MessageFormat_WritingExcelCompatableXmlDataFile0");

	internal static string Message_CouldNotRefreshEcuInfo => ResourceManager.GetString("StringTable.Message_CouldNotRefreshEcuInfo");

	internal static string Message_CouldNotUnlockVRDU => ResourceManager.GetString("StringTable.Message_CouldNotUnlockVRDU");

	internal static string Message_RefreshingData => ResourceManager.GetString("StringTable.Message_RefreshingData");

	internal static string Message_HoldOnThisWillTakeAbout60Seconds => ResourceManager.GetString("StringTable.Message_HoldOnThisWillTakeAbout60Seconds");

	internal static string MessageFormat_WritingDataFile => ResourceManager.GetString("StringTable.MessageFormat_WritingDataFile");

	internal static string Message_ExtractingVRDUQualifiers => ResourceManager.GetString("StringTable.Message_ExtractingVRDUQualifiers");

	internal static string Message_Working => ResourceManager.GetString("StringTable.Message_Working");

	internal static string Message_DataRefreshed => ResourceManager.GetString("StringTable.Message_DataRefreshed");

	internal static string Message_UnlockingVRDU => ResourceManager.GetString("StringTable.Message_UnlockingVRDU");

	internal static string Message_VRDUUnlocked => ResourceManager.GetString("StringTable.Message_VRDUUnlocked");

	internal static string Message_ExtractingABAData => ResourceManager.GetString("StringTable.Message_ExtractingABAData");
}
