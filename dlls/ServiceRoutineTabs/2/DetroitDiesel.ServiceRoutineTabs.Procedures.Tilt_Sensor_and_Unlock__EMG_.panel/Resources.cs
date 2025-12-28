using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor_and_Unlock__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageFormat_UnableToReleaseTransportSecurity0 => ResourceManager.GetString("StringTable.MessageFormat_UnableToReleaseTransportSecurity0");

	internal static string Message_SuccessfullyReleasedTransportSecurity => ResourceManager.GetString("StringTable.Message_SuccessfullyReleasedTransportSecurity");

	internal static string Message_CannotReleaseTransportSecurityTheVehicleIsCharging => ResourceManager.GetString("StringTable.Message_CannotReleaseTransportSecurityTheVehicleIsCharging");

	internal static string Message_CalibrationOfTiltSensorWasSucessful => ResourceManager.GetString("StringTable.Message_CalibrationOfTiltSensorWasSucessful");

	internal static string Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied => ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied");

	internal static string Message_CannotReleaseTransportSecurityEitherTheTCMIsOfflineOrTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_CannotReleaseTransportSecurityEitherTheTCMIsOfflineOrTheServiceCannotBeFound");

	internal static string Message_ReleasingTransportSecurity => ResourceManager.GetString("StringTable.Message_ReleasingTransportSecurity");

	internal static string Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton => ResourceManager.GetString("StringTable.Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton");

	internal static string Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice => ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice");

	internal static string Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton => ResourceManager.GetString("StringTable.Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton");

	internal static string Message_CannotCalibrateTheTiltSensor => ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensor");

	internal static string Message_CannotCalibrateTheTiltSensorTheVehicleIsCharging => ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensorTheVehicleIsCharging");

	internal static string Message_CannotReleaseTransportSecurity => ResourceManager.GetString("StringTable.Message_CannotReleaseTransportSecurity");

	internal static string Message_Ready => ResourceManager.GetString("StringTable.Message_Ready");

	internal static string Message_Error => ResourceManager.GetString("StringTable.Message_Error");

	internal static string Message_CalibrationOfTiltSensorWasNotSucessful => ResourceManager.GetString("StringTable.Message_CalibrationOfTiltSensorWasNotSucessful");
}
