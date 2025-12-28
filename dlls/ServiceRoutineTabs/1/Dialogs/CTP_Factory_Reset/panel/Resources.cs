// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Factory_Reset.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Factory_Reset.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_CTPIsOffline
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CTPIsOffline");
  }

  internal static string Message_ReloadSupplierConfigurationServiceExecuted
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ReloadSupplierConfigurationServiceExecuted");
    }
  }

  internal static string Message_PerformingFactoryResetRoutine
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_PerformingFactoryResetRoutine");
  }

  internal static string Message_FactoryResetRoutineCompletedSuccessfully
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_FactoryResetRoutineCompletedSuccessfully");
    }
  }

  internal static string Message_None
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_None");
  }

  internal static string Message_Finished
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Finished");
  }

  internal static string Message_ReloadSupplierConfigurationExecutedSuccessfully
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ReloadSupplierConfigurationExecutedSuccessfully");
    }
  }

  internal static string Message_Ready
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Ready");
  }

  internal static string Message_FactoryResetRoutineFAILED
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_FactoryResetRoutineFAILED");
  }

  internal static string Message_UserRequestedCTPFactoryReset
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_UserRequestedCTPFactoryReset");
  }

  internal static string Message_ErrorCannotStartRoutine
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ErrorCannotStartRoutine");
  }

  internal static string Message_ReloadSupplierConfigurationFailed
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ReloadSupplierConfigurationFailed");
    }
  }
}
