// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown__MY13_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown__MY13_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string MessageFormat_EngineIdleShutdownPreventionHasFailed0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_EngineIdleShutdownPreventionHasFailed0");
    }
  }

  internal static string Message_NotSupported
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_NotSupported");
  }

  internal static string Message_FailureWhileTryingToAllowEngineIdleShutdown
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_FailureWhileTryingToAllowEngineIdleShutdown");
    }
  }

  internal static string Message_RequestedEngineIdleShutdownPrevention
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_RequestedEngineIdleShutdownPrevention");
    }
  }

  internal static string Message_NotDefined
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_NotDefined");
  }

  internal static string Message_FailureWhileTryingToPreventEngineIdleShutdown
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_FailureWhileTryingToPreventEngineIdleShutdown");
    }
  }

  internal static string Message_RequestedEngineIdleShutdownAllowingRequested
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_RequestedEngineIdleShutdownAllowingRequested");
    }
  }
}
