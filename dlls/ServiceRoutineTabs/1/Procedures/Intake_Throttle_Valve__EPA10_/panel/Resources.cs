// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Intake_Throttle_Valve__EPA10_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Intake_Throttle_Valve__EPA10_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_TheMCM2ConnectedIsNotSupported
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheMCM2ConnectedIsNotSupported");
    }
  }

  internal static string Message_ManipulatingIntakeThrottleValveTo
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ManipulatingIntakeThrottleValveTo");
    }
  }

  internal static string Message_StoppingIntakeThrottleValveManipulation
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_StoppingIntakeThrottleValveManipulation");
    }
  }

  internal static string Message_EngineStartedWhileIntakeThrottleValveManipulationInProgressStoppingNow
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineStartedWhileIntakeThrottleValveManipulationInProgressStoppingNow");
    }
  }

  internal static string MessageFormat_Done0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_Done0");
  }

  internal static string MessageFormat_Error0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_Error0");
  }

  internal static string Message_Done
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Done");
  }
}
