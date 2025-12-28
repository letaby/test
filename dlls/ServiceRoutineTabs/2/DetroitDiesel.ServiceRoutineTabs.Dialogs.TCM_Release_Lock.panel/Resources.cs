using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Release_Lock.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TheLockCannotBeReleasedBecauseTheTCMIsOffline => ResourceManager.GetString("StringTable.Message_TheLockCannotBeReleasedBecauseTheTCMIsOffline");

	internal static string Message_SuccessfullyReleasedTransportSecurity => ResourceManager.GetString("StringTable.Message_SuccessfullyReleasedTransportSecurity");

	internal static string Message_CannotReleaseTransportSecurityEitherTheTCMIsUnavailableOrTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_CannotReleaseTransportSecurityEitherTheTCMIsUnavailableOrTheServiceCannotBeFound");

	internal static string Message_LockIsBeingReleased => ResourceManager.GetString("StringTable.Message_LockIsBeingReleased");

	internal static string Message_ReleasingTransportSecurity => ResourceManager.GetString("StringTable.Message_ReleasingTransportSecurity");

	internal static string Message_Ready => ResourceManager.GetString("StringTable.Message_Ready");

	internal static string MessageFormat_UnableToReleaseTransportSecurityError0 => ResourceManager.GetString("StringTable.MessageFormat_UnableToReleaseTransportSecurityError0");
}
