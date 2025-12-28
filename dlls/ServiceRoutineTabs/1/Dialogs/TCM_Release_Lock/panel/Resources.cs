// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Release_Lock.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Release_Lock.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_TheLockCannotBeReleasedBecauseTheTCMIsOffline
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheLockCannotBeReleasedBecauseTheTCMIsOffline");
    }
  }

  internal static string Message_SuccessfullyReleasedTransportSecurity
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_SuccessfullyReleasedTransportSecurity");
    }
  }

  internal static string Message_CannotReleaseTransportSecurityEitherTheTCMIsUnavailableOrTheServiceCannotBeFound
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_CannotReleaseTransportSecurityEitherTheTCMIsUnavailableOrTheServiceCannotBeFound");
    }
  }

  internal static string Message_LockIsBeingReleased
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_LockIsBeingReleased");
  }

  internal static string Message_ReleasingTransportSecurity
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ReleasingTransportSecurity");
  }

  internal static string Message_Ready
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Ready");
  }

  internal static string MessageFormat_UnableToReleaseTransportSecurityError0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_UnableToReleaseTransportSecurityError0");
    }
  }
}
