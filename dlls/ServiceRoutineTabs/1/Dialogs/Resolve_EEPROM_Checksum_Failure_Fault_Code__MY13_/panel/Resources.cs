// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Resolve_EEPROM_Checksum_Failure_Fault_Code__MY13_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Resolve_EEPROM_Checksum_Failure_Fault_Code__MY13_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_FailedToReadExistingParameters
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_FailedToReadExistingParameters");
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

  internal static string Message_TheCPC04TWasDisconnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheCPC04TWasDisconnected");
  }

  internal static string Message_TheCPC04TIsNotOnlineSoFaultCodesCouldNotBeRead
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheCPC04TIsNotOnlineSoFaultCodesCouldNotBeRead");
    }
  }

  internal static string Message_AssigningParameters
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_AssigningParameters");
  }

  internal static string Message_PerformingEEPROMReset
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_PerformingEEPROMReset");
  }

  internal static string Message_FailedToObtainService
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_FailedToObtainService");
  }

  internal static string Message_ReadingDefaultParametersAfterReset
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ReadingDefaultParametersAfterReset");
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

  internal static string Message_PreparingForEEPROMReset
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_PreparingForEEPROMReset");
  }

  internal static string Message_FailedToExecuteService
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_FailedToExecuteService");
  }

  internal static string Message_UpdateSeed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_UpdateSeed");
  }

  internal static string Message_ReadingParameters
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ReadingParameters");
  }

  internal static string Message_FailedToUnlock
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_FailedToUnlock");
  }

  internal static string MessageFormat_Writing0Parameters
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_Writing0Parameters");
  }

  internal static string Message_NoParametersAreChangedFromDefaultUseProgramDeviceToRestoreServerConfiguration
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_NoParametersAreChangedFromDefaultUseProgramDeviceToRestoreServerConfiguration");
    }
  }

  internal static string MessageFormat_PreparingToWriteBack0Parameters
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_PreparingToWriteBack0Parameters");
    }
  }

  internal static string Message_TheEEPROMChecksumFailureFaultCodeIsPresentAndMustBeCleared
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheEEPROMChecksumFailureFaultCodeIsPresentAndMustBeCleared");
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

  internal static string Message_TheEEPROMChecksumFailureFaultCodeIsNotPresentNoActionIsNecessary
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheEEPROMChecksumFailureFaultCodeIsNotPresentNoActionIsNecessary");
    }
  }
}
