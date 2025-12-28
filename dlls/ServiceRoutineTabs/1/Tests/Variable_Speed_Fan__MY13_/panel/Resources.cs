// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Tests.Variable_Speed_Fan__MY13_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Tests.Variable_Speed_Fan__MY13_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_TheFanCanBeStarted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheFanCanBeStarted");
  }

  internal static string Message_StoppingFan
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StoppingFan");
  }

  internal static string Message_TheFanHasBeenStopped
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheFanHasBeenStopped");
  }

  internal static string MessageFormat_TheFanIsProgrammedToRunFor0Seconds
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_TheFanIsProgrammedToRunFor0Seconds");
    }
  }

  internal static string MessageFormat_ErrorReadingTheParametersTheFanTypeMayBeIncorrectError0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_ErrorReadingTheParametersTheFanTypeMayBeIncorrectError0");
    }
  }

  internal static string Message_StartingFan
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StartingFan");
  }

  internal static string Message_TheFanCannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheFanCannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL");
    }
  }

  internal static string MessageFormat_AnErrorOccurredStoppingTheFan
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredStoppingTheFan");
    }
  }

  internal static string Message_TheFanIsNotAVariableSpeedType
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheFanIsNotAVariableSpeedType");
  }

  internal static string MessageFormat_AnErrorOccurredStartingTheFan0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredStartingTheFan0");
    }
  }

  internal static string Message_TheFanCannotBeRunBecauseTheMCMIsOffline
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheFanCannotBeRunBecauseTheMCMIsOffline");
    }
  }

  internal static string Message_TheFanCannotStartUntilTheEngineIsRunning
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheFanCannotStartUntilTheEngineIsRunning");
    }
  }
}
