// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Tilt_Sensor.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_CalibrationOfTiltSensorWasSucessful
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CalibrationOfTiltSensorWasSucessful");
    }
  }

  internal static string Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensorEnsureTheVehicleIsInNEUTRALAndThatTheParkBrakeIsApplied");
    }
  }

  internal static string Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TiltSensorIsLearntToReCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton");
    }
  }

  internal static string Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensorBecauseTheDeviceIsNotConnectedConnectTheDevice");
    }
  }

  internal static string Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TiltSensorRequiresCalibrationToCalibrateEnsureThatTheVehicleIsOnALevelSurfaceAndClickTheButton");
    }
  }

  internal static string Message_CannotCalibrateTheTiltSensor
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CannotCalibrateTheTiltSensor");
  }

  internal static string Message_CalibrationOfTiltSensorWasNotSucessful
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CalibrationOfTiltSensorWasNotSucessful");
    }
  }
}
