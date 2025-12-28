using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Automatic_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_NoValue => ResourceManager.GetString("StringTable.Message_NoValue");

	internal static string Message_TheVehicleNeedsToBeInZone0PerformAParkedRegenerationAndTryAgain => ResourceManager.GetString("StringTable.Message_TheVehicleNeedsToBeInZone0PerformAParkedRegenerationAndTryAgain");

	internal static string MessageFormat_Cylinder0 => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0");

	internal static string MessageFormat_CuttingCylinders0 => ResourceManager.GetString("StringTable.MessageFormat_CuttingCylinders0");

	internal static string Message_AnErrorOccurredTheTestWillTerminateEarly => ResourceManager.GetString("StringTable.Message_AnErrorOccurredTheTestWillTerminateEarly");

	internal static string Message_CylinderCutoutTest => ResourceManager.GetString("StringTable.Message_CylinderCutoutTest");

	internal static string Message_CAUTIONToAvoidPersonalInjuryPlaceTransmissionInPARKOrNEUTRALAndApplyParkingBrake => ResourceManager.GetString("StringTable.Message_CAUTIONToAvoidPersonalInjuryPlaceTransmissionInPARKOrNEUTRALAndApplyParkingBrake");

	internal static string Message_ToProceed => ResourceManager.GetString("StringTable.Message_ToProceed");

	internal static string Message_TheDPFZoneIsZero => ResourceManager.GetString("StringTable.Message_TheDPFZoneIsZero");

	internal static string Message_Idle => ResourceManager.GetString("StringTable.Message_Idle");

	internal static string Message_And => ResourceManager.GetString("StringTable.Message_And");

	internal static string MessageFormat_ContinueWith0 => ResourceManager.GetString("StringTable.MessageFormat_ContinueWith0");

	internal static string Message_SettingRPM => ResourceManager.GetString("StringTable.Message_SettingRPM");

	internal static string Message_UserAcknowledgedCaution => ResourceManager.GetString("StringTable.Message_UserAcknowledgedCaution");

	internal static string Message_History => ResourceManager.GetString("StringTable.Message_History");

	internal static string Message_TorqueLostWhileCuttingCylinders => ResourceManager.GetString("StringTable.Message_TorqueLostWhileCuttingCylinders");

	internal static string Message_TheTestWasCancelledByTheUser => ResourceManager.GetString("StringTable.Message_TheTestWasCancelledByTheUser");

	internal static string Message_CylinderCutoutTestCompleted => ResourceManager.GetString("StringTable.Message_CylinderCutoutTestCompleted");

	internal static string Message_CancelingTest => ResourceManager.GetString("StringTable.Message_CancelingTest");
}
