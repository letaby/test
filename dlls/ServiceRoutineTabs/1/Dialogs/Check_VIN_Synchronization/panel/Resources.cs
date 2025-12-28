// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Check_VIN_Synchronization.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Check_VIN_Synchronization.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_WaitingForDevicesToReconnect
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WaitingForDevicesToReconnect");
  }

  internal static string Message_ConnectAtLeastTwoDevicesToDetermineVINSynchronization
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ConnectAtLeastTwoDevicesToDetermineVINSynchronization");
    }
  }

  internal static string Message_ProcessingPleaseWait
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ProcessingPleaseWait");
  }

  internal static string Message_UnknownStage
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_UnknownStage");
  }

  internal static string Message_TheVINsInThisVehicleAreSynchronizedNoActionIsNecessaryIfTheVINIsIncorrectYouWillNeedToReprogramUsingServerData
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheVINsInThisVehicleAreSynchronizedNoActionIsNecessaryIfTheVINIsIncorrectYouWillNeedToReprogramUsingServerData");
    }
  }

  internal static string MessageFormat_FailedToWriteVINFor0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_FailedToWriteVINFor0");
  }

  internal static string Message_PleaseTurnTheIgnitionOffAndWait
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_PleaseTurnTheIgnitionOffAndWait");
    }
  }

  internal static string Message_WaitingForRemainingDevicesToShutdownPleaseWait
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_WaitingForRemainingDevicesToShutdownPleaseWait");
    }
  }

  internal static string Message_ItWasNotNecessaryToChangeAnyVINs
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ItWasNotNecessaryToChangeAnyVINs");
    }
  }

  internal static string MessageFormat_TheVINsInThisVehicleAreNotSynchronizedClickStartToCopyTheVINFrom0ToTheOtherDevices
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_TheVINsInThisVehicleAreNotSynchronizedClickStartToCopyTheVINFrom0ToTheOtherDevices");
    }
  }

  internal static string Message_TheOperationWasAborted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheOperationWasAborted");
  }

  internal static string Message_WaitingForDevicesToDisconnect
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WaitingForDevicesToDisconnect");
  }

  internal static string Message_WaitingForRemainingDevicesToComeOnlinePleaseWait
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_WaitingForRemainingDevicesToComeOnlinePleaseWait");
    }
  }

  internal static string MessageFormat_SuccessfullyWroteVINFor0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_SuccessfullyWroteVINFor0");
    }
  }

  internal static string Message_TheProcedureFailedToComplete
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheProcedureFailedToComplete");
  }

  internal static string Message_Complete
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Complete");
  }

  internal static string Message_ThereAreNoDevicesOnline
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ThereAreNoDevicesOnline");
  }

  internal static string Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseTheVINMasterDeviceDoesNotHaveAVIN
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseTheVINMasterDeviceDoesNotHaveAVIN");
    }
  }

  internal static string Message_ADeviceWasDisconnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ADeviceWasDisconnected");
  }

  internal static string Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseMultipleVINMasterDevicesAreDefinedAndConnected
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseMultipleVINMasterDevicesAreDefinedAndConnected");
    }
  }

  internal static string Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseNoVINMasterDeviceIsDefinedOrConnected
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheVINsInThisVehicleAreNotSynchronizedButTheOperationCannotProceedBecauseNoVINMasterDeviceIsDefinedOrConnected");
    }
  }

  internal static string Message_PleaseTurnTheIgnitionOnAndWait
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_PleaseTurnTheIgnitionOnAndWait");
    }
  }

  internal static string Message_Start
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Start");
  }

  internal static string Message_Retry
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Retry");
  }

  internal static string MessageFormat_UpdatingVINFor0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_UpdatingVINFor0");
  }

  internal static string Message_NoVINWasFoundInTheVINMasterDevice
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_NoVINWasFoundInTheVINMasterDevice");
    }
  }
}
