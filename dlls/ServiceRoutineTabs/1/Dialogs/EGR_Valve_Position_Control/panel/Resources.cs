// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Position_Control.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Valve_Position_Control.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_CannotStartAsDeviceIsNotOnline
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotStartAsDeviceIsNotOnline");
    }
  }

  internal static string Message_TestCanStart
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestCanStart");
  }

  internal static string Message_RequestEndEGRManipulation
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_RequestEndEGRManipulation");
  }

  internal static string Message_UnableToRequestEndEGRManipulation
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UnableToRequestEndEGRManipulation");
    }
  }

  internal static string Message_CannotStartAsDeviceIsBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CannotStartAsDeviceIsBusy");
  }

  internal static string Message_SetPosition
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SetPosition");
  }

  internal static string Message_TestIsInProgress
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestIsInProgress");
  }

  internal static string Message_CannotStartAsPositionNotValid
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CannotStartAsPositionNotValid");
  }

  internal static string Message_TestFailed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestFailed");
  }

  internal static string Message_CannotStartAsEngineIsRunningStopEngine
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotStartAsEngineIsRunningStopEngine");
    }
  }

  internal static string Message_EngineStateIncorrect
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineStateIncorrect");
  }

  internal static string Message_ECUIsBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ECUIsBusy");
  }

  internal static string Message_TestStarted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestStarted");
  }

  internal static string Message_TestCompleteDueToUserRequest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestCompleteDueToUserRequest");
  }

  internal static string Message_TestComplete
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestComplete");
  }

  internal static string Message_ECUOfflineTestAborted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ECUOfflineTestAborted");
  }
}
