using System.Collections.Generic;
using System.Linq;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance__MY13_.panel;

internal sealed class SetupInformation
{
	private int[] firingOrder6Cyl = new int[6] { 1, 5, 3, 6, 2, 4 };

	private int[] firingOrder4Cyl = new int[4] { 1, 3, 4, 2 };

	public readonly string Name;

	public readonly string NiceName;

	public readonly int CylinderCount;

	private readonly List<int> FiringOrder;

	public readonly string ThermalManagementModeStartServiceName;

	public readonly string ThermalManagementModeStopServiceName;

	public int GetPreviousCylinder(int cylinder)
	{
		int num = FiringOrder.IndexOf(cylinder);
		int index = ((num > 0) ? (num - 1) : (FiringOrder.Count - 1));
		return FiringOrder[index];
	}

	public SetupInformation(string targetEquipmentName, string targetEquipmentID, int cylinderCount, string thermalManagementModeStartServiceName, string thermalManagementModeStopServiceName)
	{
		NiceName = targetEquipmentName;
		Name = targetEquipmentID;
		CylinderCount = cylinderCount;
		FiringOrder = ((cylinderCount == 6) ? firingOrder6Cyl : firingOrder4Cyl).ToList();
		ThermalManagementModeStartServiceName = thermalManagementModeStartServiceName;
		ThermalManagementModeStopServiceName = thermalManagementModeStopServiceName;
	}
}
