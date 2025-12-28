using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Clutch_Learn_Values_Reset.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string MessageNewClutchFacingWearActualValue => ResourceManager.GetString("StringTable.MessageNewClutchFacingWearActualValue");

	internal static string MessageTheProcedureStarted => ResourceManager.GetString("StringTable.MessageTheProcedureStarted");

	internal static string ConditionTcmOnline => ResourceManager.GetString("StringTable.ConditionTcmOnline");

	internal static string MessageTheProcedureIsRunning => ResourceManager.GetString("StringTable.MessageTheProcedureIsRunning");

	internal static string MessageNewClutchFacingRemainingThickness => ResourceManager.GetString("StringTable.MessageNewClutchFacingRemainingThickness");

	internal static string MessageRunTheTCMLearnProcedurePart5 => ResourceManager.GetString("StringTable.MessageRunTheTCMLearnProcedurePart5");

	internal static string MessageRunTheTCMLearnProcedurePart4 => ResourceManager.GetString("StringTable.MessageRunTheTCMLearnProcedurePart4");

	internal static string MessageRunTheTCMLearnProcedurePart3 => ResourceManager.GetString("StringTable.MessageRunTheTCMLearnProcedurePart3");

	internal static string MessageRunTheTCMLearnProcedurePart2 => ResourceManager.GetString("StringTable.MessageRunTheTCMLearnProcedurePart2");

	internal static string MessageRunTheTCMLearnProcedurePart1 => ResourceManager.GetString("StringTable.MessageRunTheTCMLearnProcedurePart1");

	internal static string ConditionNoLearningProcess => ResourceManager.GetString("StringTable.ConditionNoLearningProcess");

	internal static string ConditionEngineStopped => ResourceManager.GetString("StringTable.ConditionEngineStopped");

	internal static string ConditionClutchReplacedChecked => ResourceManager.GetString("StringTable.ConditionClutchReplacedChecked");

	internal static string MessageTheProcedureRanSuccessfully => ResourceManager.GetString("StringTable.MessageTheProcedureRanSuccessfully");

	internal static string MessageTheProcedureCannotBeRunAgain => ResourceManager.GetString("StringTable.MessageTheProcedureCannotBeRunAgain");

	internal static string MessageOffline => ResourceManager.GetString("StringTable.MessageOffline");

	internal static string MessageTheProcedureFailed => ResourceManager.GetString("StringTable.MessageTheProcedureFailed");

	internal static string ConditionTransmissionInNeutral => ResourceManager.GetString("StringTable.ConditionTransmissionInNeutral");

	internal static string MessageErrorReturned => ResourceManager.GetString("StringTable.MessageErrorReturned");

	internal static string MessageValueNotAvailable => ResourceManager.GetString("StringTable.MessageValueNotAvailable");

	internal static string CaptionRunTheTCMLearnProcedure => ResourceManager.GetString("StringTable.CaptionRunTheTCMLearnProcedure");

	internal static string MessageTheProcedureCanStart => ResourceManager.GetString("StringTable.MessageTheProcedureCanStart");

	internal static string MessageTheProcedureCannotStart => ResourceManager.GetString("StringTable.MessageTheProcedureCannotStart");

	internal static string MessageClutchReplacedChecked => ResourceManager.GetString("StringTable.MessageClutchReplacedChecked");

	internal static string ConditionNoShifting => ResourceManager.GetString("StringTable.ConditionNoShifting");

	internal static string MessageTheProcedureHasAlreadyBeenRun => ResourceManager.GetString("StringTable.MessageTheProcedureHasAlreadyBeenRun");

	internal static string ConditionParkingBrakeSet => ResourceManager.GetString("StringTable.ConditionParkingBrakeSet");

	internal static string ConditionVehicleStandstill => ResourceManager.GetString("StringTable.ConditionVehicleStandstill");

	internal static string MessageTcmOnline => ResourceManager.GetString("StringTable.MessageTcmOnline");

	internal static string MessageNewClutchMinimumValue => ResourceManager.GetString("StringTable.MessageNewClutchMinimumValue");

	internal static string MessageNewClutchMaximumValue => ResourceManager.GetString("StringTable.MessageNewClutchMaximumValue");
}
