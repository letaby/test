using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VIM_Throttle_Control__GHG14_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_EnableVIMThrottleSupport => ResourceManager.GetString("StringTable.Message_EnableVIMThrottleSupport");

	internal static string MessageFormat_UnableToRestoreTheParameterAccelPedalTypeBackTo1Because0IsDisconnected => ResourceManager.GetString("StringTable.MessageFormat_UnableToRestoreTheParameterAccelPedalTypeBackTo1Because0IsDisconnected");

	internal static string Message_RestoreOriginalPedalType => ResourceManager.GetString("StringTable.Message_RestoreOriginalPedalType");

	internal static string MessageFormat_AccelPedalTypeParameterChangedFrom0To1 => ResourceManager.GetString("StringTable.MessageFormat_AccelPedalTypeParameterChangedFrom0To1");

	internal static string MessageFormat_AccelPedalTypeParameterValue0IsNotSupportedByTheVIM => ResourceManager.GetString("StringTable.MessageFormat_AccelPedalTypeParameterValue0IsNotSupportedByTheVIM");

	internal static string MessageFormat_FailedUnlocking0Error1 => ResourceManager.GetString("StringTable.MessageFormat_FailedUnlocking0Error1");

	internal static string MessageFormat_TheCurrentAcceleratorPedalTypeIsNotCompatible => ResourceManager.GetString("StringTable.MessageFormat_TheCurrentAcceleratorPedalTypeIsNotCompatible");

	internal static string Message_Initializing => ResourceManager.GetString("StringTable.Message_Initializing");

	internal static string MessageFormat_0Was1ByUser => ResourceManager.GetString("StringTable.MessageFormat_0Was1ByUser");

	internal static string MessageFormat_ParameterAccelPedalType0LocatedInHistory => ResourceManager.GetString("StringTable.MessageFormat_ParameterAccelPedalType0LocatedInHistory");

	internal static string MessageFormat_0IsLocked => ResourceManager.GetString("StringTable.MessageFormat_0IsLocked");

	internal static string MessageFormat_ToEnableCompatibility => ResourceManager.GetString("StringTable.MessageFormat_ToEnableCompatibility");

	internal static string MessageFormat_AccelPedalTypeParameterWasPreviouslyChangedFrom0To1 => ResourceManager.GetString("StringTable.MessageFormat_AccelPedalTypeParameterWasPreviouslyChangedFrom0To1");

	internal static string Message_AnalogPedalType3 => ResourceManager.GetString("StringTable.Message_AnalogPedalType3");

	internal static string Message_NotUnlocked => ResourceManager.GetString("StringTable.Message_NotUnlocked");

	internal static string MessageFormat_AccelPedalTypeParameterRestoredBackTo0 => ResourceManager.GetString("StringTable.MessageFormat_AccelPedalTypeParameterRestoredBackTo0");

	internal static string Message_WritingParameter => ResourceManager.GetString("StringTable.Message_WritingParameter");

	internal static string MessageFormat_OperationFailedError0 => ResourceManager.GetString("StringTable.MessageFormat_OperationFailedError0");

	internal static string MessageFormat_The0IsNoLongerOnlinePleaseReconnectTheECUBeforeContinuing => ResourceManager.GetString("StringTable.MessageFormat_The0IsNoLongerOnlinePleaseReconnectTheECUBeforeContinuing");

	internal static string MessageFormat_UnableToRestoreTheParameterAccelPedalTypeBackTo1Because0IsDisconnected1 => ResourceManager.GetString("StringTable.MessageFormat_UnableToRestoreTheParameterAccelPedalTypeBackTo1Because0IsDisconnected1");

	internal static string Message_AccelPedalTypeParameterSupportedByVIM => ResourceManager.GetString("StringTable.Message_AccelPedalTypeParameterSupportedByVIM");

	internal static string Message_TheVehicleSettingsAreCompatibleWithTheVIMNoActionIsRequiredToEnableSupportForTheVIM => ResourceManager.GetString("StringTable.Message_TheVehicleSettingsAreCompatibleWithTheVIMNoActionIsRequiredToEnableSupportForTheVIM");

	internal static string Message_Unlocked => ResourceManager.GetString("StringTable.Message_Unlocked");
}
