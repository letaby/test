using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_CalibrationOfTiltSensorWasSucessful => ResourceManager.GetString("StringTable.Message_CalibrationOfTiltSensorWasSucessful");

	internal static string Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied => ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied");

	internal static string Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton => ResourceManager.GetString("StringTable.Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton");

	internal static string Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice => ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice");

	internal static string Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton => ResourceManager.GetString("StringTable.Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton");

	internal static string Message_CannotCalibrateTheTiltSensor => ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensor");

	internal static string Message_CalibrationOfTiltSensorWasNotSucessful => ResourceManager.GetString("StringTable.Message_CalibrationOfTiltSensorWasNotSucessful");
}
