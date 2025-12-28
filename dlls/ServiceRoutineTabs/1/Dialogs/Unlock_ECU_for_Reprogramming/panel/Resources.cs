// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_ECU_for_Reprogramming.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Unlock_ECU_for_Reprogramming.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_ErrorUnlocking
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ErrorUnlocking");
  }

  internal static string Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ErrorConvertingPleaseTypeInADecimalNumberInThe0255Range");
    }
  }

  internal static string Message_ReadingLockConfigurationWait
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ReadingLockConfigurationWait");
  }

  internal static string Message_UnlockingWait0
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_UnlockingWait0");
  }

  internal static string Message_Unknown
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Unknown");
  }

  internal static string Message_PleaseTypeInVeDocSKey
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_PleaseTypeInVeDocSKey");
  }

  internal static string Message_FailureReadingVeDocInputs
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_FailureReadingVeDocInputs");
  }

  internal static string Message_Done
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Done");
  }

  internal static string Message_Closing
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Closing");
  }

  internal static string Message_ReadingInputsWait
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ReadingInputsWait");
  }

  internal static string Message_InitializingWait
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_InitializingWait");
  }

  internal static string Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_InputTooLargePleaseTypeInADecimalNumberInThe0255Range");
    }
  }
}
