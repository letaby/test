// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Clutch_Overload_Monitoring.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.TCM_Clutch_Overload_Monitoring.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_CompletedSuccessfully
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CompletedSuccessfully");
  }

  internal static string Message_Failed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Failed");
  }
}
