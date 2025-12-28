// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Mechanical_Compression_Test.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Mechanical_Compression_Test.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_PleaseTurnTheIgnition
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_PleaseTurnTheIgnition");
  }

  internal static string Message_CompressionTestStopped
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CompressionTestStopped");
  }

  internal static string Message_CompressionTestStarted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CompressionTestStarted");
  }

  internal static string Message_TheMCM21TDoesNotSupportTheServiceRoutine
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheMCM21TDoesNotSupportTheServiceRoutine");
    }
  }

  internal static string Message_EngineIsRunningInCompressionTestMode
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineIsRunningInCompressionTestMode");
    }
  }

  internal static string Message_EngineSpeedUnits
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineSpeedUnits");
  }

  internal static string Message_TheMCM21TIsOfflineCannotExecuteService
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheMCM21TIsOfflineCannotExecuteService");
    }
  }

  internal static string Message_MaxObservedEngineSpeed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MaxObservedEngineSpeed");
  }

  internal static string Message_StopCompressionTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StopCompressionTest");
  }

  internal static string Message_Success
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Success");
  }

  internal static string Message_TheTestWasCancelledByTheUser
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheTestWasCancelledByTheUser");
  }

  internal static string Message_Failed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Failed");
  }
}
