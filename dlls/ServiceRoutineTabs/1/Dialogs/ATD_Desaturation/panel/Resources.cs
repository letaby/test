// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Desaturation.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Desaturation.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_ProcedureStillRunningCannotCloseDialog
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ProcedureStillRunningCannotCloseDialog");
    }
  }

  internal static string Message_HighTemperatureWarningDialogClosed
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_HighTemperatureWarningDialogClosed");
    }
  }

  internal static string Message_InstructionText
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_InstructionText");
  }

  internal static string Message_ATDDesaturation
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ATDDesaturation");
  }

  internal static string Message_InstructionsAcknowledged
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_InstructionsAcknowledged");
  }

  internal static string Message_TemperaturesAreTooHighCannotCloseDialog
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TemperaturesAreTooHighCannotCloseDialog");
    }
  }

  internal static string Message_WarningTitle
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WarningTitle");
  }

  internal static string Message_WarningText
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WarningText");
  }

  internal static string Message_InstructionTitle
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_InstructionTitle");
  }
}
