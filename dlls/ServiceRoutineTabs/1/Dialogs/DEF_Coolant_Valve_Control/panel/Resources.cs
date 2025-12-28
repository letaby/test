// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Coolant_Valve_Control.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DEF_Coolant_Valve_Control.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string MessageFormat_TheValveFailedToOpen0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_TheValveFailedToOpen0");
  }

  internal static string MessageFormat_TheValveFailedToClose0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_TheValveFailedToClose0");
  }

  internal static string Message_DisconnectionDetectedWhileProcedureRunningDEFCoolantValveMayStillBeOpen
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_DisconnectionDetectedWhileProcedureRunningDEFCoolantValveMayStillBeOpen");
    }
  }

  internal static string Message_TheDEFCoolantValveHasBeenSetToClosed
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheDEFCoolantValveHasBeenSetToClosed");
    }
  }

  internal static string MessageFormat_TheDEFCoolantValveHasBeenSetToOpenFor0Seconds
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_TheDEFCoolantValveHasBeenSetToOpenFor0Seconds");
    }
  }
}
