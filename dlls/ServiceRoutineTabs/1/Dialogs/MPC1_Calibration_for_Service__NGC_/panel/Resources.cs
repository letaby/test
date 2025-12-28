// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC1_Calibration_for_Service__NGC_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.MPC1_Calibration_for_Service__NGC_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_EcuDisconnectedBeforeCompletion
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EcuDisconnectedBeforeCompletion");
    }
  }

  internal static string Message_VerifyingResult
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_VerifyingResult");
  }

  internal static string Message_UnlockingDevice
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_UnlockingDevice");
  }

  internal static string Message_Complete
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Complete");
  }

  internal static string Message_ConfiguringStaticCalibration
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ConfiguringStaticCalibration");
  }

  internal static string Message_ResettingDevice
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ResettingDevice");
  }

  internal static string Message_SettingParameters
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SettingParameters");
  }

  internal static string Message_ConfiguringOnlineCalibration
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ConfiguringOnlineCalibration");
  }
}
