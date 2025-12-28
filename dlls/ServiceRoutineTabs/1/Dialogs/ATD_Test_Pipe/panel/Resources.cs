// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Test_Pipe.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Test_Pipe.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_ExecutingStop
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ExecutingStop");
  }

  internal static string Message_ATDTestPipeStartFailedExecution
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ATDTestPipeStartFailedExecution");
    }
  }

  internal static string Message_ATDTestPipeStartSuccessfullyExecuted
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ATDTestPipeStartSuccessfullyExecuted");
    }
  }

  internal static string Message_ExecutingStart
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ExecutingStart");
  }

  internal static string Message_ATDTestPipeStopFailedExecution
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ATDTestPipeStopFailedExecution");
    }
  }

  internal static string Message_ATDTestPipeStopSuccessfullyExecuted
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ATDTestPipeStopSuccessfullyExecuted");
    }
  }
}
