// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Workshop_Mode.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_Workshop_Mode.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_WorkshopModeIsOff
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WorkshopModeIsOff");
  }

  internal static string Message_WorkshopModeOff
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WorkshopModeOff");
  }

  internal static string Message_CTPWorkshopModeHasBeenTurnedOffDiagnosticLinkNeedsToBeClosedAndTheVehicleInterfaceAdaptorNeedsToBeDisconnectedFromTheDiagnosticPort
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CTPWorkshopModeHasBeenTurnedOffDiagnosticLinkNeedsToBeClosedAndTheVehicleInterfaceAdaptorNeedsToBeDisconnectedFromTheDiagnosticPort");
    }
  }

  internal static string Message_UnableToRunRoutine
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_UnableToRunRoutine");
  }

  internal static string Message_Ready
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Ready");
  }
}
