// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock__NGC_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Real_time_Clock__NGC_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UnableToAcquireTheServiceRealTimeClockCannotBeSet");
    }
  }

  internal static string MessageFormat_SettingRealTimeClockTo
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_SettingRealTimeClockTo");
  }

  internal static string Message_Finished
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Finished");
  }

  internal static string MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock01PleaseCycleTheIgnitionAndTryAgain1IfTheEngineIsRunningPleaseStopTheEngineBeforeRetrying
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredWhileSettingTheRealTimeClock01PleaseCycleTheIgnitionAndTryAgain1IfTheEngineIsRunningPleaseStopTheEngineBeforeRetrying");
    }
  }

  internal static string Message_ClockResetWarning
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ClockResetWarning");
  }
}
