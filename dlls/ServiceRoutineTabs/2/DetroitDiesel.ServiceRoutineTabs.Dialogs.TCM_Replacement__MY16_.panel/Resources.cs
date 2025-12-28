using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Replacement__MY16_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TheTransmissionTypeCanBeSet => ResourceManager.GetString("StringTable.Message_TheTransmissionTypeCanBeSet");

	internal static string Message_TheConfigurationIsSetToTheSelectedValues => ResourceManager.GetString("StringTable.Message_TheConfigurationIsSetToTheSelectedValues");

	internal static string Message_TheTransmissionTypeCannotBeSetBecauseTheTCMIsOffline => ResourceManager.GetString("StringTable.Message_TheTransmissionTypeCannotBeSetBecauseTheTCMIsOffline");

	internal static string Message_CannotSetTransmissionTypeNoTransmissionTypeWasSelected => ResourceManager.GetString("StringTable.Message_CannotSetTransmissionTypeNoTransmissionTypeWasSelected");

	internal static string Message_SettingTransmissionType => ResourceManager.GetString("StringTable.Message_SettingTransmissionType");

	internal static string Message_SuccessfullyReleasedTransportSecurity => ResourceManager.GetString("StringTable.Message_SuccessfullyReleasedTransportSecurity");

	internal static string Message_CannotSetConstantMeshRangeGroupNoConstantMeshRangeGroupWasSelected => ResourceManager.GetString("StringTable.Message_CannotSetConstantMeshRangeGroupNoConstantMeshRangeGroupWasSelected");

	internal static string Message_IgnitionIsOffPleaseWait => ResourceManager.GetString("StringTable.Message_IgnitionIsOffPleaseWait");

	internal static string Message_CannotReleaseTransportSecurityEitherTheTCMIsUnavailableOrTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_CannotReleaseTransportSecurityEitherTheTCMIsUnavailableOrTheServiceCannotBeFound");

	internal static string MessageFormat_UnableToSetTheClutchActuatorError0 => ResourceManager.GetString("StringTable.MessageFormat_UnableToSetTheClutchActuatorError0");

	internal static string Message_SettingClutchActuatorType => ResourceManager.GetString("StringTable.Message_SettingClutchActuatorType");

	internal static string Message_PleaseTurnIgnitionOnAndWait => ResourceManager.GetString("StringTable.Message_PleaseTurnIgnitionOnAndWait");

	internal static string Message_CannotSetClutchActuatorTypeEitherTheTCMIsUnavailableOrTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_CannotSetClutchActuatorTypeEitherTheTCMIsUnavailableOrTheServiceCannotBeFound");

	internal static string Message_ReleasingTransportSecurity => ResourceManager.GetString("StringTable.Message_ReleasingTransportSecurity");

	internal static string Message_PleaseTurnIgnitionOffAndWait => ResourceManager.GetString("StringTable.Message_PleaseTurnIgnitionOffAndWait");

	internal static string MessageFormat_UnableToReleaseTransportSecurityError => ResourceManager.GetString("StringTable.MessageFormat_UnableToReleaseTransportSecurityError");

	internal static string Message_TransmissionIsBeingSet => ResourceManager.GetString("StringTable.Message_TransmissionIsBeingSet");

	internal static string Message_ByExecutingThisRoutineTheTransmissionSLearnedValuesWillBeResetUntilTheseValuesAreRelearnedShiftQualityMayNotBeOptimalDoYouWishToContinue => ResourceManager.GetString("StringTable.Message_ByExecutingThisRoutineTheTransmissionSLearnedValuesWillBeResetUntilTheseValuesAreRelearnedShiftQualityMayNotBeOptimalDoYouWishToContinue");

	internal static string Message_SuccessfullySetTheClutchActuator => ResourceManager.GetString("StringTable.Message_SuccessfullySetTheClutchActuator");

	internal static string MessageFormat_SuccessfullySetTheTransmissionTypeTo0 => ResourceManager.GetString("StringTable.MessageFormat_SuccessfullySetTheTransmissionTypeTo0");

	internal static string MessageFormat_UnableToSetTheTransmissionTypeError0 => ResourceManager.GetString("StringTable.MessageFormat_UnableToSetTheTransmissionTypeError0");

	internal static string Message_CannotSetTransmissionTypeEitherTheTCMIsUnavailableOrTheServiceCannotBeFound => ResourceManager.GetString("StringTable.Message_CannotSetTransmissionTypeEitherTheTCMIsUnavailableOrTheServiceCannotBeFound");

	internal static string Message_AutoLearn => ResourceManager.GetString("StringTable.Message_AutoLearn");
}
