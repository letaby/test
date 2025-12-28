// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__MY20_.panel.Log
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__MY20_.panel;

internal static class Log
{
  public static void AddEvent(string eventText)
  {
    if (SapiManager.GlobalInstance == null || SapiManager.GlobalInstance.Sapi == null || string.IsNullOrEmpty(eventText))
      return;
    SapiManager.GlobalInstance.Sapi.LogFiles.LabelLog(eventText.Trim());
  }
}
