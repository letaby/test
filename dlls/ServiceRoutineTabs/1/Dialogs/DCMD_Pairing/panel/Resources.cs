// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.DCMD_Pairing.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DCMD_Pairing.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_CannotReloadEcuInfo
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CannotReloadEcuInfo");
  }

  internal static string Message_NoKeyfobsChanged
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_NoKeyfobsChanged");
  }

  internal static string Message_TheProcedureCanStart
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TheProcedureCanStart");
  }

  internal static string Message_ValuesRead
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ValuesRead");
  }

  internal static string Message_InvalidKeyfob
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_InvalidKeyfob");
  }

  internal static string Message_PreconditionsNotMet
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_PreconditionsNotMet");
  }

  internal static string Message_DuplicateKeyfob
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DuplicateKeyfob");
  }

  internal static string Message_ValuesWritten
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ValuesWritten");
  }

  internal static string Message_ReadingValues
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ReadingValues");
  }

  internal static string MessageFormat_CanNotStart0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_CanNotStart0");
  }

  internal static string Message_ResettingUnit
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ResettingUnit");
  }

  internal static string Message_WritingValues
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WritingValues");
  }

  internal static string MessageFormat_0Failed
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_0Failed");
  }
}
