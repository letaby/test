// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string MessageFormat_FuelTemperatureMustBeAtLeast
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_FuelTemperatureMustBeAtLeast");
    }
  }

  internal static string Message_IdleSpeedBalanceTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_IdleSpeedBalanceTest");
  }

  internal static string MessageFormat_TheConnectedMCMDoesNotSupportTheServiceRoutine0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_TheConnectedMCMDoesNotSupportTheServiceRoutine0");
    }
  }

  internal static string Message_StartingTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StartingTest");
  }

  internal static string Message_MCMIsConnectedButEngineTypeIsNotSupported
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_MCMIsConnectedButEngineTypeIsNotSupported");
    }
  }

  internal static string Message_MCMIsConnectedAndEngineTypeIsSupported
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_MCMIsConnectedAndEngineTypeIsSupported");
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

  internal static string Message_MCMIsConnectedButIsBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCMIsConnectedButIsBusy");
  }

  internal static string Message_Unknown
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Unknown");
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

  internal static string Message_TestWasNotRan
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestWasNotRan");
  }

  internal static string Message_MCMIsNotConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCMIsNotConnected");
  }

  internal static string Message_TestWasNotSuccessful
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestWasNotSuccessful");
  }

  internal static string Message_TestCompleteCloseThisWindowToContinueTroubleshooting
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TestCompleteCloseThisWindowToContinueTroubleshooting");
    }
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

  internal static string MessageFormat_Cylinder0MayBeInFaultRN
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_Cylinder0MayBeInFaultRN");
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

  internal static string MessageFormat_Cylinder0MightBeCompensatingForCylinder1RN
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_Cylinder0MightBeCompensatingForCylinder1RN");
    }
  }

  internal static string Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheTransmissionMustBeInNeutralAndTheParkingBrakeON");
    }
  }
}
