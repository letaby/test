// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD.panel.SetupInformation
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD.panel;

internal sealed class SetupInformation
{
  public readonly string Name;
  public readonly string NiceName;
  public readonly bool UseFuelTemperature;

  public SetupInformation(
    string targetEquipmentName,
    string targetEquipmentID,
    bool useFuelTemperature)
  {
    this.NiceName = targetEquipmentName;
    this.Name = targetEquipmentID;
    this.UseFuelTemperature = useFuelTemperature;
  }
}
