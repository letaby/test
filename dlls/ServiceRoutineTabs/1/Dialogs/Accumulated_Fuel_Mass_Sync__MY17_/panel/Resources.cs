// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Accumulated_Fuel_Mass_Sync__MY17_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Accumulated_Fuel_Mass_Sync__MY17_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_ATSSet
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ATSSet");
  }

  internal static string Message_ReadyToSetATSDistance
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ReadyToSetATSDistance");
  }

  internal static string Message_ACMNotOnline
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ACMNotOnline");
  }

  internal static string Message_ChooseAction
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ChooseAction");
  }

  internal static string MessageFormat_SettingDistanceTo01
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_SettingDistanceTo01");
  }

  internal static string MessageFormat_PleaseSpecifyAnATSDistanceBetween0And01
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_PleaseSpecifyAnATSDistanceBetween0And01");
    }
  }

  internal static string Message_MCMNotOnline
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCMNotOnline");
  }
}
