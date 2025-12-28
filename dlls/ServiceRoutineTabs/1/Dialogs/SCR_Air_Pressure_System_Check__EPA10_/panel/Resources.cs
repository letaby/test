// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Air_Pressure_System_Check__EPA10_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Air_Pressure_System_Check__EPA10_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_EngineIsNotRunningCheckCanStart
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineIsNotRunningCheckCanStart");
    }
  }

  internal static string Message_EngineSpeedCannotBeDetectedCheckCannotStart
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetectedCheckCannotStart");
    }
  }

  internal static string Message_EngineIsRunningCheckCannotStart0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EngineIsRunningCheckCannotStart0");
    }
  }
}
