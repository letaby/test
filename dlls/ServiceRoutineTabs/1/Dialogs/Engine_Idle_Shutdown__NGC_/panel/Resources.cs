// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown__NGC_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown__NGC_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_DeviceWasNotUnlockedByUser
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DeviceWasNotUnlockedByUser");
  }

  internal static string Message_AcquiringDeviceLockStatus
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_AcquiringDeviceLockStatus");
  }

  internal static string Message_DeviceWasUnlocked
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DeviceWasUnlocked");
  }

  internal static string Message_ErrorWhileUnlockingDevice
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ErrorWhileUnlockingDevice");
  }

  internal static string Message_StatusOffline
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StatusOffline");
  }

  internal static string Message_ErrorWritingParameter
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ErrorWritingParameter");
  }

  internal static string Message_ReadingParameter
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ReadingParameter");
  }

  internal static string Message_StatusOther
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StatusOther");
  }

  internal static string Message_ErrorReadingParameter
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ErrorReadingParameter");
  }

  internal static string Message_StatusActive
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StatusActive");
  }

  internal static string Message_WritingParameter
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WritingParameter");
  }

  internal static string Message_DeviceIsUnlocked
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DeviceIsUnlocked");
  }

  internal static string Message_DeviceIsLocked
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DeviceIsLocked");
  }

  internal static string Message_StatusNotActive
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StatusNotActive");
  }

  internal static string Message_CannotClose
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CannotClose");
  }
}
