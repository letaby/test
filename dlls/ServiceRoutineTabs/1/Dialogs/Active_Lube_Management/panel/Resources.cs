// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Active_Lube_Management.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Active_Lube_Management.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_EngineIsNotRunning
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineIsNotRunning");
  }

  internal static string Message_CurrentAlmValues
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CurrentAlmValues");
  }

  internal static string Message_SSAM02TIsNotConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SSAM02TIsNotConnected");
  }

  internal static string Message_EngineSpeedCannotBeDetected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetected");
  }

  internal static string Message_SettingDefaultValues
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SettingDefaultValues");
  }

  internal static string Message_SSAM02TIsBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SSAM02TIsBusy");
  }

  internal static string Message_ThisSSAMDoesNotHaveTheNeededParameters
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ThisSSAMDoesNotHaveTheNeededParameters");
    }
  }

  internal static string Message_EngineIsRunning
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineIsRunning");
  }

  internal static string Message_DisconnectionDetected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DisconnectionDetected");
  }

  internal static string Message_SSAM02TIsConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SSAM02TIsConnected");
  }

  internal static string Message_InitailParameterValues
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_InitailParameterValues");
  }

  internal static string Message_ParametersSetToDefaultValues
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ParametersSetToDefaultValues");
  }

  internal static string Message_Finished
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Finished");
  }

  internal static string Message_RemainInTestMode
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_RemainInTestMode");
  }

  internal static string Message_ResettingParameters
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ResettingParameters");
  }

  internal static string Message_EnteringTestMode
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EnteringTestMode");
  }

  internal static string Message_ParametersReset
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ParametersReset");
  }

  internal static string Message_ActiveLubeManagementNotEnabled
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ActiveLubeManagementNotEnabled");
    }
  }
}
