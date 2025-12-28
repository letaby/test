// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD__MY13_.panel.SetupInformation
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD__MY13_.panel;

internal sealed class SetupInformation
{
  private int[] firingOrder6Cyl = new int[6]
  {
    1,
    5,
    3,
    6,
    2,
    4
  };
  private int[] firingOrder4Cyl = new int[4]{ 1, 3, 4, 2 };
  public readonly string Name;
  public readonly string NiceName;
  public readonly int CylinderCount;
  private readonly List<int> FiringOrder;
  public readonly string ThermalManagementModeStartServiceName;
  public readonly string ThermalManagementModeStopServiceName;

  public int GetPreviousCylinder(int cylinder)
  {
    int num = this.FiringOrder.IndexOf(cylinder);
    return this.FiringOrder[num > 0 ? num - 1 : this.FiringOrder.Count - 1];
  }

  public SetupInformation(
    string targetEquipmentName,
    string targetEquipmentID,
    int cylinderCount,
    string thermalManagementModeStartServiceName,
    string thermalManagementModeStopServiceName)
  {
    this.NiceName = targetEquipmentName;
    this.Name = targetEquipmentID;
    this.CylinderCount = cylinderCount;
    this.FiringOrder = (cylinderCount == 6 ? (IEnumerable<int>) this.firingOrder6Cyl : (IEnumerable<int>) this.firingOrder4Cyl).ToList<int>();
    this.ThermalManagementModeStartServiceName = thermalManagementModeStartServiceName;
    this.ThermalManagementModeStopServiceName = thermalManagementModeStopServiceName;
  }
}
