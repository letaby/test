using System.ComponentModel;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Maintenance_System.panel;

public class Resources
{
	private static ComponentResourceManager ResourceManager => new ComponentResourceManager(typeof(UserPanel));

	internal static string Message_RemainingDrivingDistance => ResourceManager.GetString("StringTable.Message_RemainingDrivingDistance");

	internal static string MessageFormat_CannotResetAxle2System01 => ResourceManager.GetString("StringTable.MessageFormat_CannotResetAxle2System01");

	internal static string Message_Description => ResourceManager.GetString("StringTable.Message_Description");

	internal static string MessageFormat_CannotResetEngineSystem0MS01TOffline => ResourceManager.GetString("StringTable.MessageFormat_CannotResetEngineSystem0MS01TOffline");

	internal static string Message_EstimatedReplacementDate => ResourceManager.GetString("StringTable.Message_EstimatedReplacementDate");

	internal static string MessageFormat_CannotResetEngineSystem01 => ResourceManager.GetString("StringTable.MessageFormat_CannotResetEngineSystem01");

	internal static string MessageFormat_CannotResetAxle1System0MS01TOffline => ResourceManager.GetString("StringTable.MessageFormat_CannotResetAxle1System0MS01TOffline");

	internal static string Message_Starting => ResourceManager.GetString("StringTable.Message_Starting");

	internal static string Message_Unavaiable => ResourceManager.GetString("StringTable.Message_Unavaiable");

	internal static string Message_OperatingTime => ResourceManager.GetString("StringTable.Message_OperatingTime");

	internal static string Message_ReadingECUInformation => ResourceManager.GetString("StringTable.Message_ReadingECUInformation");

	internal static string Message_ReadyToReset => ResourceManager.GetString("StringTable.Message_ReadyToReset");

	internal static string Message_NotEnoughOperatingTime => ResourceManager.GetString("StringTable.Message_NotEnoughOperatingTime");

	internal static string Message_SystemActive => ResourceManager.GetString("StringTable.Message_SystemActive");

	internal static string Message_LoadLifeCycleConsumption => ResourceManager.GetString("StringTable.Message_LoadLifeCycleConsumption");

	internal static string Message_MaintenanceDateMonth => ResourceManager.GetString("StringTable.Message_MaintenanceDateMonth");

	internal static string Message_MaintenanceDateDay => ResourceManager.GetString("StringTable.Message_MaintenanceDateDay");

	internal static string Message_DrivenDistance => ResourceManager.GetString("StringTable.Message_DrivenDistance");

	internal static string MessageFormat_CannotResetTransmissionSystem0MS01TOffline => ResourceManager.GetString("StringTable.MessageFormat_CannotResetTransmissionSystem0MS01TOffline");

	internal static string MessageFormat_CannotResetAxle2System0MS01TOffline => ResourceManager.GetString("StringTable.MessageFormat_CannotResetAxle2System0MS01TOffline");

	internal static string MessageFormat_0OilSystemValues => ResourceManager.GetString("StringTable.MessageFormat_0OilSystemValues");

	internal static string Message_Value => ResourceManager.GetString("StringTable.Message_Value");

	internal static string MessageFormat_CannotResetTransmissionSystem01 => ResourceManager.GetString("StringTable.MessageFormat_CannotResetTransmissionSystem01");

	internal static string Message_RemainingOperatingTime => ResourceManager.GetString("StringTable.Message_RemainingOperatingTime");

	internal static string Message_LifeCycleConsumption => ResourceManager.GetString("StringTable.Message_LifeCycleConsumption");

	internal static string Message_MaintenanceDateYear => ResourceManager.GetString("StringTable.Message_MaintenanceDateYear");

	internal static string Message_ResetComplete => ResourceManager.GetString("StringTable.Message_ResetComplete");

	internal static string MessageFormat_CannotResetAxle1System01 => ResourceManager.GetString("StringTable.MessageFormat_CannotResetAxle1System01");

	internal static string Message_MS01TOffline => ResourceManager.GetString("StringTable.Message_MS01TOffline");
}
