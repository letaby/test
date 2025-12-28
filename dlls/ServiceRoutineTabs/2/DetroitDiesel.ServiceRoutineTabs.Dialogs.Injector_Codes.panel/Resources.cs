using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Injector_Codes.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_FailedToSaveTheRPGLeakValuesToTheECU => ResourceManager.GetString("StringTable.Message_FailedToSaveTheRPGLeakValuesToTheECU");

	internal static string Message_FinishedSendingInjectorCodes => ResourceManager.GetString("StringTable.Message_FinishedSendingInjectorCodes");

	internal static string Message_ResetAllPIRData => ResourceManager.GetString("StringTable.Message_ResetAllPIRData");

	internal static string Message_ReadingPIRValues1 => ResourceManager.GetString("StringTable.Message_ReadingPIRValues1");

	internal static string Message_ReadingInjectorCodes => ResourceManager.GetString("StringTable.Message_ReadingInjectorCodes");

	internal static string MessageFormat_SuccessfullyResetPIRDataForCylinder0 => ResourceManager.GetString("StringTable.MessageFormat_SuccessfullyResetPIRDataForCylinder0");

	internal static string MessageFormat_Cylinder0InjectorCodeCouldNotBeReadServiceFailedToExecute => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0InjectorCodeCouldNotBeReadServiceFailedToExecute");

	internal static string Message_SuccessfullyResetPIRDataForAllCylinders => ResourceManager.GetString("StringTable.Message_SuccessfullyResetPIRDataForAllCylinders");

	internal static string MessageFormat_PIRForCylinder0Read1 => ResourceManager.GetString("StringTable.MessageFormat_PIRForCylinder0Read1");

	internal static string Message_FailedToResetRPGLeakValues => ResourceManager.GetString("StringTable.Message_FailedToResetRPGLeakValues");

	internal static string Message_FailedToResetPIRDataForAllCylindersServiceFailedToExecute => ResourceManager.GetString("StringTable.Message_FailedToResetPIRDataForAllCylindersServiceFailedToExecute");

	internal static string MessageFormat_Cylinder01 => ResourceManager.GetString("StringTable.MessageFormat_Cylinder01");

	internal static string MessageFormat_SendingCylinder0InjectorCodeAs1 => ResourceManager.GetString("StringTable.MessageFormat_SendingCylinder0InjectorCodeAs1");

	internal static string MessageFormat_Cylinder0InjectorCodeIs1 => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0InjectorCodeIs1");

	internal static string MessageFormat_Cylinder0InjectorCodeCouldNotBeSentServiceWasUnavailable => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0InjectorCodeCouldNotBeSentServiceWasUnavailable");

	internal static string Message_PIRParametersRead1 => ResourceManager.GetString("StringTable.Message_PIRParametersRead1");

	internal static string Message_FinishedResettingPIRData => ResourceManager.GetString("StringTable.Message_FinishedResettingPIRData");

	internal static string Message_FinishedReadingInjectorCodes => ResourceManager.GetString("StringTable.Message_FinishedReadingInjectorCodes");

	internal static string MessageFormat_PIRForCylinder0CouldNotBeRead => ResourceManager.GetString("StringTable.MessageFormat_PIRForCylinder0CouldNotBeRead");

	internal static string MessageFormat_Cylinder0InjectorCodeCouldNotBeRead1 => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0InjectorCodeCouldNotBeRead1");

	internal static string Message_WritingInjectorCodes => ResourceManager.GetString("StringTable.Message_WritingInjectorCodes");

	internal static string Message_RPGLeakValuesHaveBeenReset => ResourceManager.GetString("StringTable.Message_RPGLeakValuesHaveBeenReset");

	internal static string MessageFormat_Cylinder0FailedToResetPIRDataServiceFailedToExecute => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0FailedToResetPIRDataServiceFailedToExecute");

	internal static string Message_ReadingPIRValues => ResourceManager.GetString("StringTable.Message_ReadingPIRValues");

	internal static string MessageFormat_Cylinder0InjectorCodeHasAnInvalidValueOf1 => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0InjectorCodeHasAnInvalidValueOf1");

	internal static string Message_PIRParametersRead => ResourceManager.GetString("StringTable.Message_PIRParametersRead");

	internal static string MessageFormat_AnErrorOccurredWhileReadingParameters0 => ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredWhileReadingParameters0");

	internal static string Message_RPGLeakValuesHaveBeenSavedToTheECU => ResourceManager.GetString("StringTable.Message_RPGLeakValuesHaveBeenSavedToTheECU");

	internal static string MessageFormat_Cylinder0InjectorCodeCouldNotBeSentServiceFailedToExecute => ResourceManager.GetString("StringTable.MessageFormat_Cylinder0InjectorCodeCouldNotBeSentServiceFailedToExecute");
}
