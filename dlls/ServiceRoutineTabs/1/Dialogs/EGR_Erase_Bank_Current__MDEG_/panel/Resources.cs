// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Erase_Bank_Current__MDEG_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_Erase_Bank_Current__MDEG_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Label_ClickButtonToEraseBank
  {
    get => Resources.ResourceManager.GetString("StringTable.Label_ClickButtonToEraseBank");
  }

  internal static string Message_SuccessfulExecution
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SuccessfulExecution");
  }

  internal static string Message_Error
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Error");
  }

  internal static string Message_FailedExecution
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_FailedExecution");
  }
}
