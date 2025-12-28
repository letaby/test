namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Idle_Speed_Balance__EPA10_.panel;

internal sealed class SetupInformation
{
	public readonly string Name;

	public readonly string NiceName;

	public SetupInformation(string targetEquipmentName, string targetEquipmentID)
	{
		NiceName = targetEquipmentName;
		Name = targetEquipmentID;
	}
}
