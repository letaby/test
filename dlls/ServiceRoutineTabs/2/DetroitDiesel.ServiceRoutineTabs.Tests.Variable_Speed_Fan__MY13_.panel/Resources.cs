using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Variable_Speed_Fan__MY13_.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_TheFanCanBeStarted => ResourceManager.GetString("StringTable.Message_TheFanCanBeStarted");

	internal static string Message_StoppingFan => ResourceManager.GetString("StringTable.Message_StoppingFan");

	internal static string Message_TheFanHasBeenStopped => ResourceManager.GetString("StringTable.Message_TheFanHasBeenStopped");

	internal static string MessageFormat_TheFanIsProgrammedToRunFor0Seconds => ResourceManager.GetString("StringTable.MessageFormat_TheFanIsProgrammedToRunFor0Seconds");

	internal static string MessageFormat_ErrorReadingTheParametersTheFanTypeMayBeIncorrectError0 => ResourceManager.GetString("StringTable.MessageFormat_ErrorReadingTheParametersTheFanTypeMayBeIncorrectError0");

	internal static string Message_StartingFan => ResourceManager.GetString("StringTable.Message_StartingFan");

	internal static string Message_TheFanCannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL => ResourceManager.GetString("StringTable.Message_TheFanCannotStartUntilTheParkingBrakeIsONAndTheTransmissionIsInNEUTRAL");

	internal static string MessageFormat_AnErrorOccurredStoppingTheFan => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredStoppingTheFan");

	internal static string Message_TheFanIsNotAVariableSpeedType => ResourceManager.GetString("StringTable.Message_TheFanIsNotAVariableSpeedType");

	internal static string MessageFormat_AnErrorOccurredStartingTheFan0 => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredStartingTheFan0");

	internal static string Message_TheFanCannotBeRunBecauseTheMCMIsOffline => ResourceManager.GetString("StringTable.Message_TheFanCannotBeRunBecauseTheMCMIsOffline");

	internal static string Message_TheFanCannotStartUntilTheEngineIsRunning => ResourceManager.GetString("StringTable.Message_TheFanCannotStartUntilTheEngineIsRunning");
}
