// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ESC_Learn_Procedure.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ESC_Learn_Procedure.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_ProcedureCanStart
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ProcedureCanStart");
  }

  internal static string Message_ErrorStoppingProcedure
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ErrorStoppingProcedure");
  }

  internal static string Message_ProcedureStopped
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ProcedureStopped");
  }

  internal static string Message_ProcedureStarted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ProcedureStarted");
  }

  internal static string Message_ESCLearnProcedureComplete
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ESCLearnProcedureComplete");
  }

  internal static string Message_StartDrivingTheTruck
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StartDrivingTheTruck");
  }

  internal static string Message_ErrorStartingProcedure
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ErrorStartingProcedure");
  }

  internal static string Message_YouMayStopTheVehicle
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_YouMayStopTheVehicle");
  }

  internal static string Message_ProcedureRunning
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ProcedureRunning");
  }

  internal static string Message_WheelBasedSpeedMustBeZero
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WheelBasedSpeedMustBeZero");
  }
}
