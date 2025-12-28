// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clutch_Apply_Leak_Test.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Clutch_Apply_Leak_Test.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string MessageFormat_IntialClutchPostionObserved0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_IntialClutchPostionObserved0");
    }
  }

  internal static string Message_TestCanStart
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestCanStart");
  }

  internal static string Message_WaitingToRecordSampleClutchPositionValue
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_WaitingToRecordSampleClutchPositionValue");
    }
  }

  internal static string Message_CannotStartTheTestAsTheDeviceIsBusy
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheDeviceIsBusy");
    }
  }

  internal static string Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionIsInNeutral
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TestCannotStartEnsureParkBrakeIsOnAndTransmissionIsInNeutral");
    }
  }

  internal static string Message_RequestingDesiredValueRequirementClutchStopService
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_RequestingDesiredValueRequirementClutchStopService");
    }
  }

  internal static string Message_ErrorExecutingDesiredValueRequirementClutchRequestService
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ErrorExecutingDesiredValueRequirementClutchRequestService");
    }
  }

  internal static string Message_CannotStartTheTestAsAirSupplyPressureIsBeGreaterThan90Psi
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsAirSupplyPressureIsBeGreaterThan90Psi");
    }
  }

  internal static string Message_ErrorExecutingDesiredValueRequirementClutchStartService
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ErrorExecutingDesiredValueRequirementClutchStartService");
    }
  }

  internal static string Message_TestInProgress
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestInProgress");
  }

  internal static string Message_LeakFoundTestFailed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_LeakFoundTestFailed");
  }

  internal static string Message_NoLeakFoundTestPassed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_NoLeakFoundTestPassed");
  }

  internal static string Message_CannotStartTheTestAsTheEngineIsRunningStopTheEngine
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheEngineIsRunningStopTheEngine");
    }
  }

  internal static string Message_RequestingDesiredValueRequirementClutchStartService
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_RequestingDesiredValueRequirementClutchStartService");
    }
  }

  internal static string Message_ErrorStoppingDesiredValueRequirementClutchStopService
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ErrorStoppingDesiredValueRequirementClutchStopService");
    }
  }

  internal static string Message_StartingClutchApplyLeakTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StartingClutchApplyLeakTest");
  }

  internal static string MessageFormat_FinalClutchPostionObserved0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_FinalClutchPostionObserved0");
    }
  }

  internal static string Message_CannotStartTheTestAsTheDeviceIsNotOnline
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotStartTheTestAsTheDeviceIsNotOnline");
    }
  }

  internal static string Message_CompletedClutchApplyLeakTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CompletedClutchApplyLeakTest");
  }
}
