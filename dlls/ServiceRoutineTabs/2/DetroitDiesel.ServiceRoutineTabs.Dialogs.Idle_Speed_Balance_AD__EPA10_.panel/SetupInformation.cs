namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD__EPA10_.panel;

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
