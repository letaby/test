// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.FaultCodeTabs.General.Test_Results__EPA10_.panel.Resources
// Assembly: FaultCodeTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 35DAF471-66CA-4F8E-B39E-2FF7E69A8BE3
// Assembly location: C:\Users\petra\Downloads\Архив (2)\FaultCodeTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.FaultCodeTabs.General.Test_Results__EPA10_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_Standard
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Standard");
  }

  internal static string Message_TestResultsData
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_TestResultsData");
  }
}
