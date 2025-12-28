// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_3G_Sundown.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.CTP_3G_Sundown.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_WritingParameters
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WritingParameters");
  }

  internal static string Message_ErrorWritingParameters
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ErrorWritingParameters");
  }

  internal static string Message_CTPBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CTPBusy");
  }

  internal static string Message_Ready
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Ready");
  }

  internal static string MessageFormat_CTPBusy0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_CTPBusy0");
  }
}
