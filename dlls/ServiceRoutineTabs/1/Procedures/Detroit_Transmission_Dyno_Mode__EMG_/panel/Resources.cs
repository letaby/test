// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__EMG_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Transmission_Dyno_Mode__EMG_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_TheDynamometerTestModeCannotBeStartedUntilTheIgnitionIsOn
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheDynamometerTestModeCannotBeStartedUntilTheIgnitionIsOn");
    }
  }

  internal static string Message_EndingTheTestTheService
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EndingTheTestTheService");
  }

  internal static string Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheEngineAndTheDynamometerTestModeAreRunningPressTheStopButtonToExit");
    }
  }

  internal static string Message_TheStartButtonHasBeenPressed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheStartButtonHasBeenPressed");
  }

  internal static string Message_ReadyToStartTheDynamometerTestMode
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ReadyToStartTheDynamometerTestMode");
    }
  }

  internal static string Value_FirstRearDriveAxleActive
  {
    get => Resources.ResourceManager.GetString("StringTable.Value_FirstRearDriveAxleActive");
  }

  internal static string Message_TheCPCIsOffline
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheCPCIsOffline");
  }

  internal static string MessageFormat_ErrorStartingDynamometerTestMode0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_ErrorStartingDynamometerTestMode0");
    }
  }

  internal static string Value_SecondRearDriveAxleActive
  {
    get => Resources.ResourceManager.GetString("StringTable.Value_SecondRearDriveAxleActive");
  }

  internal static string Message_DynamometerTestModeIsRunning
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DynamometerTestModeIsRunning");
  }

  internal static string Value_TwoRearDriveAxles
  {
    get => Resources.ResourceManager.GetString("StringTable.Value_TwoRearDriveAxles");
  }

  internal static string Message_TheIgnitionHasBeenTurnedOffWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheIgnitionHasBeenTurnedOffWhileTheDynamometerTestModeIsRunningYouWillNeedToStopAndRestartTheDynamometerTestMode");
    }
  }

  internal static string Message_DynamometerTestModeHasStopped
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DynamometerTestModeHasStopped");
  }

  internal static string Value_None
  {
    get => Resources.ResourceManager.GetString("StringTable.Value_None");
  }

  internal static string Message_IsNotAvailable
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_IsNotAvailable");
  }
}
