// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__Euro4_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Low_Pressure_Leak_Test__Euro4_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_TestAbortedUserCanceledTheTest
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TestAbortedUserCanceledTheTest");
    }
  }

  internal static string Message_TestFailedPressureCouldNotBeRead
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TestFailedPressureCouldNotBeRead");
    }
  }

  internal static string Message_TestFailedLeakWasDetected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestFailedLeakWasDetected");
  }

  internal static string Message_EngineIsNotRunningTestCanStart
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineIsNotRunningTestCanStart");
    }
  }

  internal static string Message_EngineSpeedCannotBeDetected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetected");
  }

  internal static string Message_TestFailedEnigneIsRunning
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestFailedEnigneIsRunning");
  }

  internal static string Message_EngineIsRunningTestCannotStart
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineIsRunningTestCannotStart");
    }
  }

  internal static string MessageFormat_TheFinalLPPOPressureObservedWas0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_TheFinalLPPOPressureObservedWas0");
    }
  }

  internal static string MessageFormat_TheInitialLPPOPressureObservedWas0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_TheInitialLPPOPressureObservedWas0");
    }
  }

  internal static string Message_TestCompleted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestCompleted");
  }

  internal static string Message_TestAbortedDeviceWentOffline
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestAbortedDeviceWentOffline");
  }

  internal static string Message_WaitingTenMinutesWhileThePressureIsMonitoredForADropOfMoreThan5Psi
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_WaitingTenMinutesWhileThePressureIsMonitoredForADropOfMoreThan5Psi");
    }
  }

  internal static string Message_WaitingFiveMinutesToLetTheAirPressureStabilize
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_WaitingFiveMinutesToLetTheAirPressureStabilize");
    }
  }

  internal static string Message_TestPassedNoLeakDetected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestPassedNoLeakDetected");
  }
}
