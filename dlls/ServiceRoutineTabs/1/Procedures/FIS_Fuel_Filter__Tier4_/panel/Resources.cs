// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter__Tier4_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.FIS_Fuel_Filter__Tier4_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_NoFault
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_NoFault");
  }

  internal static string FuelFilterCalculationIs0Active0
  {
    get => Resources.ResourceManager.GetString("StringTable.FuelFilterCalculationIs0Active0");
  }

  internal static string MessageFormat_ValuesReset
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_ValuesReset");
  }

  internal static string Message_Not
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Not");
  }
}
