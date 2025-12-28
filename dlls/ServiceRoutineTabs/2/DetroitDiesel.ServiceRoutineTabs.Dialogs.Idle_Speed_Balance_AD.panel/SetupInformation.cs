namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Idle_Speed_Balance_AD.panel;

internal sealed class SetupInformation
{
	public readonly string Name;

	public readonly string NiceName;

	public readonly bool UseFuelTemperature;

	public SetupInformation(string targetEquipmentName, string targetEquipmentID, bool useFuelTemperature)
	{
		NiceName = targetEquipmentName;
		Name = targetEquipmentID;
		UseFuelTemperature = useFuelTemperature;
	}
}
