// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance__EPA10_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance__EPA10_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_MCM2IsConnectedButEngineTypeIsNotSupported
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_MCM2IsConnectedButEngineTypeIsNotSupported");
    }
  }

  internal static string Message_StartingTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StartingTest");
  }

  internal static string MessageFormat_TheConnectedMCM2DoesNotSupportTheServiceRoutine0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_TheConnectedMCM2DoesNotSupportTheServiceRoutine0");
    }
  }

  internal static string Message_Cylinder
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Cylinder");
  }

  internal static string Message_EngineIsAtIdle
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineIsAtIdle");
  }

  internal static string MessageFormat_CoolantTemperatureMustBeAtLeast0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_CoolantTemperatureMustBeAtLeast0");
    }
  }

  internal static string Message_FuelAndCoolantTemperaturesAreInRange
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_FuelAndCoolantTemperaturesAreInRange");
    }
  }

  internal static string Message_CannotDetectIfEngineIsStarted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CannotDetectIfEngineIsStarted");
  }

  internal static string Message_MCM2IsNotConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCM2IsNotConnected");
  }

  internal static string Message_TestWasNotSuccessful
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestWasNotSuccessful");
  }

  internal static string Message_TheEngineIsNotAtIdle
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheEngineIsNotAtIdle");
  }

  internal static string Message_TestWasSuccessful
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestWasSuccessful");
  }

  internal static string Message_ErrorsOccurredDuringTheTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ErrorsOccurredDuringTheTest");
  }

  internal static string Message_WhileTestingCylinder
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WhileTestingCylinder");
  }

  internal static string Message_MCM2IsConnectedAndEngineTypeIsSupported
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_MCM2IsConnectedAndEngineTypeIsSupported");
    }
  }

  internal static string Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON0");
    }
  }

  internal static string Message_MCM2IsConnectedButIsBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCM2IsConnectedButIsBusy");
  }

  internal static string MessageFormat_FuelTemperatureMustBeAtLeast0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_FuelTemperatureMustBeAtLeast0");
    }
  }

  internal static string Message_VehicleStatusIsOK
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_VehicleStatusIsOK");
  }

  internal static string Message_EngineIsStoppedStartTheEngineToProceed
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineIsStoppedStartTheEngineToProceed");
    }
  }

  internal static string Message_TheTestCompletedSuccessfully
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheTestCompletedSuccessfully");
  }

  internal static string Message_Cylinder1
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Cylinder1");
  }

  internal static string Message_IdleSpeedBalanceTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_IdleSpeedBalanceTest");
  }
}
