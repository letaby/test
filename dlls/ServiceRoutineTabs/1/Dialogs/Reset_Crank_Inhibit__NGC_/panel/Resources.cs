// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Reset_Crank_Inhibit__NGC_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Reset_Crank_Inhibit__NGC_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_DisablingCrankInhibit
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DisablingCrankInhibit");
  }

  internal static string Message_FlashingOverTheAirIsCurrentlyInProgress
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_FlashingOverTheAirIsCurrentlyInProgress");
    }
  }

  internal static string Message_CrankingIsNotInhibited
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CrankingIsNotInhibited");
  }

  internal static string Message_UnableToDisableCrankInhibit
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_UnableToDisableCrankInhibit");
  }

  internal static string Message_EcusMustBeConnectedToDisableCrankInhibit
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_EcusMustBeConnectedToDisableCrankInhibit");
    }
  }

  internal static string Message_CrankInhibitWasDisabled
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CrankInhibitWasDisabled");
  }

  internal static string Message_Ready
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Ready");
  }
}
