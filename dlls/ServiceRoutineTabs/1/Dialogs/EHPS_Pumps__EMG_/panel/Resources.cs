// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EHPS_Pumps__EMG_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EHPS_Pumps__EMG_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_EHPS201TOffline
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EHPS201TOffline");
  }

  internal static string Message_EHPS401TOffline
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EHPS401TOffline");
  }

  internal static string Message_Null
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Null");
  }

  internal static string Message_EHPS401TPumpTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EHPS401TPumpTest");
  }

  internal static string MessageFormat_ServiceStarted01
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_ServiceStarted01");
  }

  internal static string MessageFormat_ServiceCouldNotBeStarted01
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_ServiceCouldNotBeStarted01");
    }
  }

  internal static string Message_EHPS201TPumpTest
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EHPS201TPumpTest");
  }
}
