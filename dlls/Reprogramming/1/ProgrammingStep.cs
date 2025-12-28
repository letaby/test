// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.ProgrammingStep
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public enum ProgrammingStep
{
  [Description("")] None,
  [Description("Connect to device")] Connect,
  [Description("Connect to device via CAESAR")] ConnectCaesar,
  [Description("Connect to device via MVCI")] ConnectMvci,
  [Description("Write Engine Serial Number")] WriteEngineSerialNumber,
  [Description("Write Vehicle Identification Number")] WriteVehicleIdentificationNumber,
  [Description("Read existing settings")] ReadExistingSettings,
  [Description("Load existing settings")] LoadExistingSettings,
  [Description("Load settings from server")] LoadServerSettings,
  [Description("Unlock VeDoc protection for Firmware Flash")] UnlockVeDocFirmware,
  [Description("Unlock VeDoc protection for Dataset Flash")] UnlockVeDocDataSet,
  [Description("Flash Boot Firmware")] FlashBootLoader,
  [Description("Flash Firmware")] FlashFirmware,
  [Description("Flash Dataset")] FlashDataSet,
  [Description("Flash Control List")] FlashControlList,
  [Description("Load Dataset specific settings from server")] LoadDataSetSettings,
  [Description("Load preset settings from server")] LoadPresetSettings,
  [Description("Unlock")] UnlockBackdoor,
  [Description("Unlock and clear")] UnlockBackdoorAndClearPasswords,
  [Description("Reset device to default settings")] ResetToDefault,
  [Description("Execute version specific initialization")] ExecuteVersionSpecificInitialization,
  [Description("Write settings")] WriteSettings,
  [Description("Commit settings to permanent memory")] CommitToNonvolatile,
  [Description("Reconnect to device")] Reconnect,
  [Description("Post-programming actions")] ExecutePostProgrammingActions,
  [Description("Verify settings")] VerifySettings,
  [Description("Update usage count")] UpdateUsageCount,
}
