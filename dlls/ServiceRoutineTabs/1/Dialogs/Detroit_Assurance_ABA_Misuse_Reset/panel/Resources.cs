// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_ABA_Misuse_Reset.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_ABA_Misuse_Reset.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string MessageFormat_TheFaultHasBeenReset0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_TheFaultHasBeenReset0");
  }

  internal static string MessageFormat_UnableToResetError0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_UnableToResetError0");
  }

  internal static string Message_TheFaultCannotBeResetBecauseTheVRDUIsOffline
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheFaultCannotBeResetBecauseTheVRDUIsOffline");
    }
  }

  internal static string Message_Ready
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Ready");
  }

  internal static string Message_ResettingTheFault
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ResettingTheFault");
  }
}
