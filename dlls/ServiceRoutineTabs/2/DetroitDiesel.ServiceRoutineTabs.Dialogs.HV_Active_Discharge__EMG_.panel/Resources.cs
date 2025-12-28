using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HV_Active_Discharge__EMG_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string BlackWarning => ResourceManager.GetString("StringTable.BlackWarning");

	internal static string WarningText => ResourceManager.GetString("StringTable.WarningText");

	internal static string RedWarning => ResourceManager.GetString("StringTable.RedWarning");

	internal static string ReferenceChecklist => ResourceManager.GetString("StringTable.ReferenceChecklist");
}
