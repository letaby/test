using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				ResourceManager resourceManager = new ResourceManager("DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.Properties.Resources", typeof(Resources).Assembly);
				resourceMan = resourceManager;
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static Bitmap add
	{
		get
		{
			object obj = ResourceManager.GetObject("add", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string CaptionError => ResourceManager.GetString("CaptionError", resourceCulture);

	internal static string CaptionStationLog => ResourceManager.GetString("CaptionStationLog", resourceCulture);

	internal static string ChargeReferenceTypeLabelText_CANUMBER => ResourceManager.GetString("ChargeReferenceTypeLabelText_CANUMBER", resourceCulture);

	internal static string ChargeReferenceTypeLabelText_DDCREQUEST => ResourceManager.GetString("ChargeReferenceTypeLabelText_DDCREQUEST", resourceCulture);

	internal static string ChargeReferenceTypeLabelText_DESCRIPTION => ResourceManager.GetString("ChargeReferenceTypeLabelText_DESCRIPTION", resourceCulture);

	internal static string ChargeReferenceTypeLabelText_PONUMBER => ResourceManager.GetString("ChargeReferenceTypeLabelText_PONUMBER", resourceCulture);

	internal static string ChargeReferenceTypeLabelText_RONUMBER => ResourceManager.GetString("ChargeReferenceTypeLabelText_RONUMBER", resourceCulture);

	internal static string CommandTools => ResourceManager.GetString("CommandTools", resourceCulture);

	internal static string CommandViewStationLog => ResourceManager.GetString("CommandViewStationLog", resourceCulture);

	internal static string ConnectedUnit_DownloadedUnitData => ResourceManager.GetString("ConnectedUnit_DownloadedUnitData", resourceCulture);

	internal static string ConnectedUnit_EngineSerialNumberInconsistency => ResourceManager.GetString("ConnectedUnit_EngineSerialNumberInconsistency", resourceCulture);

	internal static string ConnectedUnit_EngineSerialNumberInconsistencyWithVehicleIdentification => ResourceManager.GetString("ConnectedUnit_EngineSerialNumberInconsistencyWithVehicleIdentification", resourceCulture);

	internal static string ConnectedUnit_EnterIdentity => ResourceManager.GetString("ConnectedUnit_EnterIdentity", resourceCulture);

	internal static string ConnectedUnit_EnterIdentityDescription => ResourceManager.GetString("ConnectedUnit_EnterIdentityDescription", resourceCulture);

	internal static string ConnectedUnit_FormatUnitDataDownloadedOn => ResourceManager.GetString("ConnectedUnit_FormatUnitDataDownloadedOn", resourceCulture);

	internal static string ConnectedUnit_FormatUnitDataDownloadedToday => ResourceManager.GetString("ConnectedUnit_FormatUnitDataDownloadedToday", resourceCulture);

	internal static string ConnectedUnit_FormatUnitDataDownloadedYesterday => ResourceManager.GetString("ConnectedUnit_FormatUnitDataDownloadedYesterday", resourceCulture);

	internal static string ConnectedUnit_FormatUnitDataUpdatedOn => ResourceManager.GetString("ConnectedUnit_FormatUnitDataUpdatedOn", resourceCulture);

	internal static string ConnectedUnit_FormatUnitDataUpdatedToday => ResourceManager.GetString("ConnectedUnit_FormatUnitDataUpdatedToday", resourceCulture);

	internal static string ConnectedUnit_FormatUnitDataUpdatedYesterday => ResourceManager.GetString("ConnectedUnit_FormatUnitDataUpdatedYesterday", resourceCulture);

	internal static string ConnectedUnit_FormatVehicleIdentificationAndEngineSerialNumberInconsistency => ResourceManager.GetString("ConnectedUnit_FormatVehicleIdentificationAndEngineSerialNumberInconsistency", resourceCulture);

	internal static string ConnectedUnit_FormatVehicleIdentificationInconsistency => ResourceManager.GetString("ConnectedUnit_FormatVehicleIdentificationInconsistency", resourceCulture);

	internal static string ConnectedUnit_NoDownload => ResourceManager.GetString("ConnectedUnit_NoDownload", resourceCulture);

	internal static string ConnectedUnit_NotEnoughDevicesForCorrectIdentification => ResourceManager.GetString("ConnectedUnit_NotEnoughDevicesForCorrectIdentification", resourceCulture);

	internal static string ConnectedUnit_OtherConnectedIdentityDescription => ResourceManager.GetString("ConnectedUnit_OtherConnectedIdentityDescription", resourceCulture);

	internal static string ConnectedUnit_PrimaryConnectedIdentityDescription => ResourceManager.GetString("ConnectedUnit_PrimaryConnectedIdentityDescription", resourceCulture);

	internal static string ConnectedUnit_ServerReportedErrors => ResourceManager.GetString("ConnectedUnit_ServerReportedErrors", resourceCulture);

	internal static string ConnectedUnit_UnitStatusPending => ResourceManager.GetString("ConnectedUnit_UnitStatusPending", resourceCulture);

	internal static string ConnectedUnit_UploadDataWarning => ResourceManager.GetString("ConnectedUnit_UploadDataWarning", resourceCulture);

	internal static string ConnectedUnit_UploadUnitData => ResourceManager.GetString("ConnectedUnit_UploadUnitData", resourceCulture);

	internal static string ConnectedUnit_VehicleIdentificationInconsitencyNotEnoughDevicesForCorrectIdentification => ResourceManager.GetString("ConnectedUnit_VehicleIdentificationInconsitencyNotEnoughDevicesForCorrectIdentification", resourceCulture);

	internal static string ConnectedUnit_VehicleIdentificationInvalid => ResourceManager.GetString("ConnectedUnit_VehicleIdentificationInvalid", resourceCulture);

	internal static string ConnectedUnit_VerifyUnitIdentity => ResourceManager.GetString("ConnectedUnit_VerifyUnitIdentity", resourceCulture);

	internal static string ConnectedUnit_VinIsNeededToContinueDataDownload => ResourceManager.GetString("ConnectedUnit_VinIsNeededToContinueDataDownload", resourceCulture);

	internal static Bitmap connecting
	{
		get
		{
			object obj = ResourceManager.GetObject("connecting", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string CycleIgnition_FormatWaitingForRunoff => ResourceManager.GetString("CycleIgnition_FormatWaitingForRunoff", resourceCulture);

	internal static string CycleIgnition_Title => ResourceManager.GetString("CycleIgnition_Title", resourceCulture);

	internal static string CycleIgnition_TurnIgnitionOff => ResourceManager.GetString("CycleIgnition_TurnIgnitionOff", resourceCulture);

	internal static string CycleIgnition_TurnIgnitionOn => ResourceManager.GetString("CycleIgnition_TurnIgnitionOn", resourceCulture);

	internal static string CycleIgnition_WaitingForDisconnection => ResourceManager.GetString("CycleIgnition_WaitingForDisconnection", resourceCulture);

	internal static string CycleIgnition_WaitingForReconnection => ResourceManager.GetString("CycleIgnition_WaitingForReconnection", resourceCulture);

	internal static string DuplicatePowertrainEcusNotAllowed => ResourceManager.GetString("DuplicatePowertrainEcusNotAllowed", resourceCulture);

	internal static Bitmap error
	{
		get
		{
			object obj = ResourceManager.GetObject("error", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap flash
	{
		get
		{
			object obj = ResourceManager.GetObject("flash", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string Format_ProrgrammingOperationNewDevice => ResourceManager.GetString("Format_ProrgrammingOperationNewDevice", resourceCulture);

	internal static string Format_ProrgrammingOperationSameDevice => ResourceManager.GetString("Format_ProrgrammingOperationSameDevice", resourceCulture);

	internal static string GatherServerDataPage_ChassisStatus => ResourceManager.GetString("GatherServerDataPage_ChassisStatus", resourceCulture);

	internal static string GatherServerDataPage_DataNotApplicable => ResourceManager.GetString("GatherServerDataPage_DataNotApplicable", resourceCulture);

	internal static string GatherServerDataPage_DataOK => ResourceManager.GetString("GatherServerDataPage_DataOK", resourceCulture);

	internal static string GatherServerDataPage_Description => ResourceManager.GetString("GatherServerDataPage_Description", resourceCulture);

	internal static string GatherServerDataPage_Device => ResourceManager.GetString("GatherServerDataPage_Device", resourceCulture);

	internal static string GatherServerDataPage_Devices_Separator => ResourceManager.GetString("GatherServerDataPage_Devices_Separator", resourceCulture);

	internal static string GatherServerDataPage_DownloadUnitsSection => ResourceManager.GetString("GatherServerDataPage_DownloadUnitsSection", resourceCulture);

	internal static string GatherServerDataPage_FileMissing => ResourceManager.GetString("GatherServerDataPage_FileMissing", resourceCulture);

	internal static string GatherServerDataPage_Filename => ResourceManager.GetString("GatherServerDataPage_Filename", resourceCulture);

	internal static string GatherServerDataPage_FileNameNotAvailable => ResourceManager.GetString("GatherServerDataPage_FileNameNotAvailable", resourceCulture);

	internal static string GatherServerDataPage_FlashKeyNotFound => ResourceManager.GetString("GatherServerDataPage_FlashKeyNotFound", resourceCulture);

	internal static string GatherServerDataPage_Format_FlashKeyMismatch => ResourceManager.GetString("GatherServerDataPage_Format_FlashKeyMismatch", resourceCulture);

	internal static string GatherServerDataPage_Format_MissingServerDataForDevices => ResourceManager.GetString("GatherServerDataPage_Format_MissingServerDataForDevices", resourceCulture);

	internal static string GatherServerDataPage_Format_Warnings => ResourceManager.GetString("GatherServerDataPage_Format_Warnings", resourceCulture);

	internal static string GatherServerDataPage_FormatDeviceErrors => ResourceManager.GetString("GatherServerDataPage_FormatDeviceErrors", resourceCulture);

	internal static string GatherServerDataPage_FormatMissingDevices => ResourceManager.GetString("GatherServerDataPage_FormatMissingDevices", resourceCulture);

	internal static string GatherServerDataPage_FormatUploadFor => ResourceManager.GetString("GatherServerDataPage_FormatUploadFor", resourceCulture);

	internal static string GatherServerDataPage_FormatUploadPending => ResourceManager.GetString("GatherServerDataPage_FormatUploadPending", resourceCulture);

	internal static string GatherServerDataPage_ForSoftwareVersion => ResourceManager.GetString("GatherServerDataPage_ForSoftwareVersion", resourceCulture);

	internal static string GatherServerDataPage_Identifier => ResourceManager.GetString("GatherServerDataPage_Identifier", resourceCulture);

	internal static string GatherServerDataPage_Is64BitCompatible => ResourceManager.GetString("GatherServerDataPage_Is64BitCompatible", resourceCulture);

	internal static string GatherServerDataPage_Message_AllWarning => ResourceManager.GetString("GatherServerDataPage_Message_AllWarning", resourceCulture);

	internal static string GatherServerDataPage_Message_UnitWarning => ResourceManager.GetString("GatherServerDataPage_Message_UnitWarning", resourceCulture);

	internal static string GatherServerDataPage_MessageRefreshSoftware => ResourceManager.GetString("GatherServerDataPage_MessageRefreshSoftware", resourceCulture);

	internal static string GatherServerDataPage_MVCIServerMustBeEnabledToLoadFile => ResourceManager.GetString("GatherServerDataPage_MVCIServerMustBeEnabledToLoadFile", resourceCulture);

	internal static string GatherServerDataPage_No => ResourceManager.GetString("GatherServerDataPage_No", resourceCulture);

	internal static string GatherServerDataPage_NoInformation => ResourceManager.GetString("GatherServerDataPage_NoInformation", resourceCulture);

	internal static string GatherServerDataPage_PartNumber => ResourceManager.GetString("GatherServerDataPage_PartNumber", resourceCulture);

	internal static string GatherServerDataPage_PendingRequestsSection => ResourceManager.GetString("GatherServerDataPage_PendingRequestsSection", resourceCulture);

	internal static string GatherServerDataPage_PowertrainStatus => ResourceManager.GetString("GatherServerDataPage_PowertrainStatus", resourceCulture);

	internal static string GatherServerDataPage_RemoteFileDownloadPending => ResourceManager.GetString("GatherServerDataPage_RemoteFileDownloadPending", resourceCulture);

	internal static string GatherServerDataPage_RemoteFileStatus => ResourceManager.GetString("GatherServerDataPage_RemoteFileStatus", resourceCulture);

	internal static string GatherServerDataPage_ServerStatusSeparator => ResourceManager.GetString("GatherServerDataPage_ServerStatusSeparator", resourceCulture);

	internal static string GatherServerDataPage_Status_AutomaticOperationComplete => ResourceManager.GetString("GatherServerDataPage_Status_AutomaticOperationComplete", resourceCulture);

	internal static string GatherServerDataPage_Status_AutomaticOperationRequired => ResourceManager.GetString("GatherServerDataPage_Status_AutomaticOperationRequired", resourceCulture);

	internal static string GatherServerDataPage_StatusFormat_NoServerSettings => ResourceManager.GetString("GatherServerDataPage_StatusFormat_NoServerSettings", resourceCulture);

	internal static string GatherServerDataPage_TabConnectedUnit => ResourceManager.GetString("GatherServerDataPage_TabConnectedUnit", resourceCulture);

	internal static string GatherServerDataPage_TabDataset => ResourceManager.GetString("GatherServerDataPage_TabDataset", resourceCulture);

	internal static string GatherServerDataPage_TabDiagnosticDescriptions => ResourceManager.GetString("GatherServerDataPage_TabDiagnosticDescriptions", resourceCulture);

	internal static string GatherServerDataPage_TabSoftware => ResourceManager.GetString("GatherServerDataPage_TabSoftware", resourceCulture);

	internal static string GatherServerDataPage_TabUnit => ResourceManager.GetString("GatherServerDataPage_TabUnit", resourceCulture);

	internal static string GatherServerDataPage_TabUnitManagement => ResourceManager.GetString("GatherServerDataPage_TabUnitManagement", resourceCulture);

	internal static string GatherServerDataPage_UnitNotFound => ResourceManager.GetString("GatherServerDataPage_UnitNotFound", resourceCulture);

	internal static string GatherServerDataPage_UnitStatusExpired => ResourceManager.GetString("GatherServerDataPage_UnitStatusExpired", resourceCulture);

	internal static string GatherServerDataPage_Unknown => ResourceManager.GetString("GatherServerDataPage_Unknown", resourceCulture);

	internal static string GatherServerDataPage_UploadUnitsSection => ResourceManager.GetString("GatherServerDataPage_UploadUnitsSection", resourceCulture);

	internal static string GatherServerDataPage_Version => ResourceManager.GetString("GatherServerDataPage_Version", resourceCulture);

	internal static string GatherServerDataPage_Warnings_Separator => ResourceManager.GetString("GatherServerDataPage_Warnings_Separator", resourceCulture);

	internal static string GatherServerDataPage_Yes => ResourceManager.GetString("GatherServerDataPage_Yes", resourceCulture);

	internal static string GatherServerDataPageUnitHeader_Error => ResourceManager.GetString("GatherServerDataPageUnitHeader_Error", resourceCulture);

	internal static string GatherServerDataPageUnitHeader_Warning => ResourceManager.GetString("GatherServerDataPageUnitHeader_Warning", resourceCulture);

	internal static string GatherServerDataPageUploadData => ResourceManager.GetString("GatherServerDataPageUploadData", resourceCulture);

	internal static string HardwarePartNumberIsRequired => ResourceManager.GetString("HardwarePartNumberIsRequired", resourceCulture);

	internal static string Header_GatherServerData => ResourceManager.GetString("Header_GatherServerData", resourceCulture);

	internal static string Header_ProgramDevice => ResourceManager.GetString("Header_ProgramDevice", resourceCulture);

	internal static string Header_SelectOperation => ResourceManager.GetString("Header_SelectOperation", resourceCulture);

	internal static string HelpToolTip_AddEditEcuHardwarePartNumbers_Overview => ResourceManager.GetString("HelpToolTip_AddEditEcuHardwarePartNumbers_Overview", resourceCulture);

	internal static string MenuProxy_FormatRemoteFileDownloadWarning => ResourceManager.GetString("MenuProxy_FormatRemoteFileDownloadWarning", resourceCulture);

	internal static string MessageFormat_CouldNotSetParameterError => ResourceManager.GetString("MessageFormat_CouldNotSetParameterError", resourceCulture);

	internal static string MessageFormat_CouldNotSetParameterGroupError => ResourceManager.GetString("MessageFormat_CouldNotSetParameterGroupError", resourceCulture);

	internal static string MessageFormat_ResourceCannotBeDetermined => ResourceManager.GetString("MessageFormat_ResourceCannotBeDetermined", resourceCulture);

	internal static string MessageFormatReplaceTo0Executing => ResourceManager.GetString("MessageFormatReplaceTo0Executing", resourceCulture);

	internal static string MessageFormatUpgradeFrom0To1Executing => ResourceManager.GetString("MessageFormatUpgradeFrom0To1Executing", resourceCulture);

	internal static string MessageMustEnterAValidProductIdentificationNumber => ResourceManager.GetString("MessageMustEnterAValidProductIdentificationNumber", resourceCulture);

	internal static string MessageMustEnterAValidVehicleIdentificationNumber => ResourceManager.GetString("MessageMustEnterAValidVehicleIdentificationNumber", resourceCulture);

	internal static string MessageMustEnterAValidVehicleNumberOrEngineSerialNumber => ResourceManager.GetString("MessageMustEnterAValidVehicleNumberOrEngineSerialNumber", resourceCulture);

	internal static string Messsage_NoConnectionResources => ResourceManager.GetString("Messsage_NoConnectionResources", resourceCulture);

	internal static string MustSelectAnEcu => ResourceManager.GetString("MustSelectAnEcu", resourceCulture);

	internal static Bitmap ok
	{
		get
		{
			object obj = ResourceManager.GetObject("ok", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string PanelDescription => ResourceManager.GetString("PanelDescription", resourceCulture);

	internal static Bitmap PanelImage
	{
		get
		{
			object obj = ResourceManager.GetObject("PanelImage", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string PanelTitle => ResourceManager.GetString("PanelTitle", resourceCulture);

	internal static string PartNumberErrorDialog_CodingFileDate => ResourceManager.GetString("PartNumberErrorDialog_CodingFileDate", resourceCulture);

	internal static string PartNumberErrorDialog_CodingFileName => ResourceManager.GetString("PartNumberErrorDialog_CodingFileName", resourceCulture);

	internal static string PartNumberErrorDialog_CodingFileVersion => ResourceManager.GetString("PartNumberErrorDialog_CodingFileVersion", resourceCulture);

	internal static string PartNumberErrorDialog_DiagnosticVariant => ResourceManager.GetString("PartNumberErrorDialog_DiagnosticVariant", resourceCulture);

	internal static string PartNumberErrorDialog_Domain => ResourceManager.GetString("PartNumberErrorDialog_Domain", resourceCulture);

	internal static string PartNumberErrorDialog_Error => ResourceManager.GetString("PartNumberErrorDialog_Error", resourceCulture);

	internal static string PartNumberErrorDialog_FormatDefaultStringLength => ResourceManager.GetString("PartNumberErrorDialog_FormatDefaultStringLength", resourceCulture);

	internal static string PartNumberErrorDialog_FormatDomainLength => ResourceManager.GetString("PartNumberErrorDialog_FormatDomainLength", resourceCulture);

	internal static string PartNumberErrorDialog_FormatDomainNotExist => ResourceManager.GetString("PartNumberErrorDialog_FormatDomainNotExist", resourceCulture);

	internal static string PartNumberErrorDialog_FormatFragmentBeforeDefaultString => ResourceManager.GetString("PartNumberErrorDialog_FormatFragmentBeforeDefaultString", resourceCulture);

	internal static string PartNumberErrorDialog_FormatFragmentNotExist => ResourceManager.GetString("PartNumberErrorDialog_FormatFragmentNotExist", resourceCulture);

	internal static string PartNumberErrorDialog_FormatVariantNotInPermittedList => ResourceManager.GetString("PartNumberErrorDialog_FormatVariantNotInPermittedList", resourceCulture);

	internal static string PartNumberErrorDialog_Fragment => ResourceManager.GetString("PartNumberErrorDialog_Fragment", resourceCulture);

	internal static string PartNumberErrorDialog_Meaning => ResourceManager.GetString("PartNumberErrorDialog_Meaning", resourceCulture);

	internal static string PartNumberErrorDialog_NotApplicable => ResourceManager.GetString("PartNumberErrorDialog_NotApplicable", resourceCulture);

	internal static string PartNumberErrorDialog_Notes => ResourceManager.GetString("PartNumberErrorDialog_Notes", resourceCulture);

	internal static string PartNumberErrorDialog_PartNumber => ResourceManager.GetString("PartNumberErrorDialog_PartNumber", resourceCulture);

	internal static string PartNumberErrorDialog_PrefixText => ResourceManager.GetString("PartNumberErrorDialog_PrefixText", resourceCulture);

	internal static string PartNumberErrorDialog_SoftwareVersion => ResourceManager.GetString("PartNumberErrorDialog_SoftwareVersion", resourceCulture);

	internal static string PartNumberErrorDialog_Title => ResourceManager.GetString("PartNumberErrorDialog_Title", resourceCulture);

	internal static string ProgramDeviceManager_Complete_ChannelUnexpectedlyDisconnected => ResourceManager.GetString("ProgramDeviceManager_Complete_ChannelUnexpectedlyDisconnected", resourceCulture);

	internal static string ProgramDeviceManager_Complete_ConnectionResourceForTargetPlatformNotFound => ResourceManager.GetString("ProgramDeviceManager_Complete_ConnectionResourceForTargetPlatformNotFound", resourceCulture);

	internal static string ProgramDeviceManager_Complete_ConnectionResourceIsNotValid => ResourceManager.GetString("ProgramDeviceManager_Complete_ConnectionResourceIsNotValid", resourceCulture);

	internal static string ProgramDeviceManager_Complete_ConnectionResourceNotAvailable => ResourceManager.GetString("ProgramDeviceManager_Complete_ConnectionResourceNotAvailable", resourceCulture);

	internal static string ProgramDeviceManager_Complete_CouldNotLocateFlashKey => ResourceManager.GetString("ProgramDeviceManager_Complete_CouldNotLocateFlashKey", resourceCulture);

	internal static string ProgramDeviceManager_Complete_CycleIgnitionFailed => ResourceManager.GetString("ProgramDeviceManager_Complete_CycleIgnitionFailed", resourceCulture);

	internal static string ProgramDeviceManager_Complete_DataImplausibleReconnectedInBootMode => ResourceManager.GetString("ProgramDeviceManager_Complete_DataImplausibleReconnectedInBootMode", resourceCulture);

	internal static string ProgramDeviceManager_Complete_EcuSoftwareVersionTooNew => ResourceManager.GetString("ProgramDeviceManager_Complete_EcuSoftwareVersionTooNew", resourceCulture);

	internal static string ProgramDeviceManager_Complete_FirmwareIncompatibleWithHardware => ResourceManager.GetString("ProgramDeviceManager_Complete_FirmwareIncompatibleWithHardware", resourceCulture);

	internal static string ProgramDeviceManager_Complete_FlashAreaCollectionNotAvailable => ResourceManager.GetString("ProgramDeviceManager_Complete_FlashAreaCollectionNotAvailable", resourceCulture);

	internal static string ProgramDeviceManager_Complete_FlashBlock_not_found_for_connected_hardware => ResourceManager.GetString("ProgramDeviceManager_Complete_FlashBlock_not_found_for_connected_hardware", resourceCulture);

	internal static string ProgramDeviceManager_Complete_TargetDiagnosisPlatformNotAvailable => ResourceManager.GetString("ProgramDeviceManager_Complete_TargetDiagnosisPlatformNotAvailable", resourceCulture);

	internal static string ProgramDeviceManager_Complete_UnexpectedSequenceInConnectComplete => ResourceManager.GetString("ProgramDeviceManager_Complete_UnexpectedSequenceInConnectComplete", resourceCulture);

	internal static string ProgramDeviceManager_FormatProcessing => ResourceManager.GetString("ProgramDeviceManager_FormatProcessing", resourceCulture);

	internal static string ProgramDeviceManager_MessagFormatConnectToCAN => ResourceManager.GetString("ProgramDeviceManager_MessagFormatConnectToCAN", resourceCulture);

	internal static string ProgramDeviceManager_MessagFormatConnectToEthernet => ResourceManager.GetString("ProgramDeviceManager_MessagFormatConnectToEthernet", resourceCulture);

	internal static string ProgramDeviceManager_Status_OK => ResourceManager.GetString("ProgramDeviceManager_Status_OK", resourceCulture);

	internal static string ProgramDeviceManager_Step_Committing => ResourceManager.GetString("ProgramDeviceManager_Step_Committing", resourceCulture);

	internal static string ProgramDeviceManager_Step_Complete => ResourceManager.GetString("ProgramDeviceManager_Step_Complete", resourceCulture);

	internal static string ProgramDeviceManager_Step_Connecting => ResourceManager.GetString("ProgramDeviceManager_Step_Connecting", resourceCulture);

	internal static string ProgramDeviceManager_Step_FormatPercentComplete => ResourceManager.GetString("ProgramDeviceManager_Step_FormatPercentComplete", resourceCulture);

	internal static string ProgramDeviceManager_Step_FormatPercentCompleteMultiple => ResourceManager.GetString("ProgramDeviceManager_Step_FormatPercentCompleteMultiple", resourceCulture);

	internal static string ProgramDeviceManager_Step_LoadingParameters => ResourceManager.GetString("ProgramDeviceManager_Step_LoadingParameters", resourceCulture);

	internal static string ProgramDeviceManager_Step_NAAllParametersResetToDefault => ResourceManager.GetString("ProgramDeviceManager_Step_NAAllParametersResetToDefault", resourceCulture);

	internal static string ProgramDeviceManager_Step_NAUsingExistingBootFirmware => ResourceManager.GetString("ProgramDeviceManager_Step_NAUsingExistingBootFirmware", resourceCulture);

	internal static string ProgramDeviceManager_Step_NAUsingExistingControlListFirmware => ResourceManager.GetString("ProgramDeviceManager_Step_NAUsingExistingControlListFirmware", resourceCulture);

	internal static string ProgramDeviceManager_Step_NAUsingExistingDataset => ResourceManager.GetString("ProgramDeviceManager_Step_NAUsingExistingDataset", resourceCulture);

	internal static string ProgramDeviceManager_Step_NAUsingExistingFirmware => ResourceManager.GetString("ProgramDeviceManager_Step_NAUsingExistingFirmware", resourceCulture);

	internal static string ProgramDeviceManager_Step_NotApplicable => ResourceManager.GetString("ProgramDeviceManager_Step_NotApplicable", resourceCulture);

	internal static string ProgramDeviceManager_Step_Reading => ResourceManager.GetString("ProgramDeviceManager_Step_Reading", resourceCulture);

	internal static string ProgramDeviceManager_Step_Reconnecting => ResourceManager.GetString("ProgramDeviceManager_Step_Reconnecting", resourceCulture);

	internal static string ProgramDeviceManager_Step_Resetting => ResourceManager.GetString("ProgramDeviceManager_Step_Resetting", resourceCulture);

	internal static string ProgramDeviceManager_Step_Starting => ResourceManager.GetString("ProgramDeviceManager_Step_Starting", resourceCulture);

	internal static string ProgramDeviceManager_Step_Starting_Count_Format => ResourceManager.GetString("ProgramDeviceManager_Step_Starting_Count_Format", resourceCulture);

	internal static string ProgramDeviceManager_Step_UsingSettingsFromPreviouslyFailedUpgrade => ResourceManager.GetString("ProgramDeviceManager_Step_UsingSettingsFromPreviouslyFailedUpgrade", resourceCulture);

	internal static string ProgramDeviceManager_Step_WaitingForOnlineStatus => ResourceManager.GetString("ProgramDeviceManager_Step_WaitingForOnlineStatus", resourceCulture);

	internal static string ProgramDeviceManager_Step_Writing => ResourceManager.GetString("ProgramDeviceManager_Step_Writing", resourceCulture);

	internal static string ProgramDevicePage_CheckDependentDeviceFeaturesAfterProgramming => ResourceManager.GetString("ProgramDevicePage_CheckDependentDeviceFeaturesAfterProgramming", resourceCulture);

	internal static string ProgramDevicePage_Compat_CompatibleSetFormat => ResourceManager.GetString("ProgramDevicePage_Compat_CompatibleSetFormat", resourceCulture);

	internal static string ProgramDevicePage_Compat_TargetDevice => ResourceManager.GetString("ProgramDevicePage_Compat_TargetDevice", resourceCulture);

	internal static string ProgramDevicePage_Configuration => ResourceManager.GetString("ProgramDevicePage_Configuration", resourceCulture);

	internal static string ProgramDevicePage_ECUSerialNumber => ResourceManager.GetString("ProgramDevicePage_ECUSerialNumber", resourceCulture);

	internal static string ProgramDevicePage_Error => ResourceManager.GetString("ProgramDevicePage_Error", resourceCulture);

	internal static string ProgramDevicePage_Failed => ResourceManager.GetString("ProgramDevicePage_Failed", resourceCulture);

	internal static string ProgramDevicePage_FormatIncompatibleHardware => ResourceManager.GetString("ProgramDevicePage_FormatIncompatibleHardware", resourceCulture);

	internal static string ProgramDevicePage_FormatPreconditionNotMet => ResourceManager.GetString("ProgramDevicePage_FormatPreconditionNotMet", resourceCulture);

	internal static string ProgramDevicePage_Hardware => ResourceManager.GetString("ProgramDevicePage_Hardware", resourceCulture);

	internal static string ProgramDevicePage_Identification => ResourceManager.GetString("ProgramDevicePage_Identification", resourceCulture);

	internal static string ProgramDevicePage_IncompatibleHardwareDialogTitle => ResourceManager.GetString("ProgramDevicePage_IncompatibleHardwareDialogTitle", resourceCulture);

	internal static string ProgramDevicePage_OfflineDeviceCompatibility_Information => ResourceManager.GetString("ProgramDevicePage_OfflineDeviceCompatibility_Information", resourceCulture);

	internal static string ProgramDevicePage_OperationInformation => ResourceManager.GetString("ProgramDevicePage_OperationInformation", resourceCulture);

	internal static string ProgramDevicePage_OutputName_Name => ResourceManager.GetString("ProgramDevicePage_OutputName_Name", resourceCulture);

	internal static string ProgramDevicePage_OutputName_Step => ResourceManager.GetString("ProgramDevicePage_OutputName_Step", resourceCulture);

	internal static string ProgramDevicePage_OutputValue_ActualData => ResourceManager.GetString("ProgramDevicePage_OutputValue_ActualData", resourceCulture);

	internal static string ProgramDevicePage_OutputValue_Result => ResourceManager.GetString("ProgramDevicePage_OutputValue_Result", resourceCulture);

	internal static string ProgramDevicePage_ParameterWarnings => ResourceManager.GetString("ProgramDevicePage_ParameterWarnings", resourceCulture);

	internal static string ProgramDevicePage_PleaseWait => ResourceManager.GetString("ProgramDevicePage_PleaseWait", resourceCulture);

	internal static string ProgramDevicePage_Processing => ResourceManager.GetString("ProgramDevicePage_Processing", resourceCulture);

	internal static string ProgramDevicePage_ProgrammingComplete => ResourceManager.GetString("ProgramDevicePage_ProgrammingComplete", resourceCulture);

	internal static string ProgramDevicePage_Ready => ResourceManager.GetString("ProgramDevicePage_Ready", resourceCulture);

	internal static string ProgramDevicePage_Success => ResourceManager.GetString("ProgramDevicePage_Success", resourceCulture);

	internal static string ProgramDevicePage_ViewingLogFile => ResourceManager.GetString("ProgramDevicePage_ViewingLogFile", resourceCulture);

	internal static string ProgramDevicePageItem_NoDataToProgram => ResourceManager.GetString("ProgramDevicePageItem_NoDataToProgram", resourceCulture);

	internal static string ProgramDevicePageItemName_Device => ResourceManager.GetString("ProgramDevicePageItemName_Device", resourceCulture);

	internal static string ProgramDevicePageOutputInfoLabel_IfThisIsCorrectClickTheStartButton => ResourceManager.GetString("ProgramDevicePageOutputInfoLabel_IfThisIsCorrectClickTheStartButton", resourceCulture);

	internal static string ProgramDevicePageOutputInfoLabel_Programming => ResourceManager.GetString("ProgramDevicePageOutputInfoLabel_Programming", resourceCulture);

	internal static string ProgramDevicePageOutputInfoLabel_ProgrammingFailedUnableToContinue => ResourceManager.GetString("ProgramDevicePageOutputInfoLabel_ProgrammingFailedUnableToContinue", resourceCulture);

	internal static string ProgramDevicePageOutputInfoLabel_TheDeviceWasSuccessfullyProgrammed => ResourceManager.GetString("ProgramDevicePageOutputInfoLabel_TheDeviceWasSuccessfullyProgrammed", resourceCulture);

	internal static string ProgramDevicePageOutputInfoLabel_TheDeviceWasSuccessfullyProgrammedButMayHaveConfigurationErrors => ResourceManager.GetString("ProgramDevicePageOutputInfoLabel_TheDeviceWasSuccessfullyProgrammedButMayHaveConfigurationErrors", resourceCulture);

	internal static string ProgramDevicePageOutputInfoLabel_TheProgrammingOperationFailedToRetryClickTheStartButton => ResourceManager.GetString("ProgramDevicePageOutputInfoLabel_TheProgrammingOperationFailedToRetryClickTheStartButton", resourceCulture);

	internal static string ProgramDevicePageTooltipFormat_NoParameterErrors => ResourceManager.GetString("ProgramDevicePageTooltipFormat_NoParameterErrors", resourceCulture);

	internal static string ProgramDevicePageTooltipFormat_ParameterErrors => ResourceManager.GetString("ProgramDevicePageTooltipFormat_ParameterErrors", resourceCulture);

	internal static string ProgrammingData_FormatCouldNotLocateFile => ResourceManager.GetString("ProgrammingData_FormatCouldNotLocateFile", resourceCulture);

	internal static string ProgrammingData_FormatCouldNotLocateFirmwareInfo => ResourceManager.GetString("ProgrammingData_FormatCouldNotLocateFirmwareInfo", resourceCulture);

	internal static string ProgrammingDataItem_AutomaticOperation => ResourceManager.GetString("ProgrammingDataItem_AutomaticOperation", resourceCulture);

	internal static string ProgrammingDataItem_BootSoftware => ResourceManager.GetString("ProgrammingDataItem_BootSoftware", resourceCulture);

	internal static string ProgrammingDataItem_Dataset => ResourceManager.GetString("ProgrammingDataItem_Dataset", resourceCulture);

	internal static string ProgrammingDataItem_DataSet_PartNumberFormat => ResourceManager.GetString("ProgrammingDataItem_DataSet_PartNumberFormat", resourceCulture);

	internal static string ProgrammingDataItem_Device => ResourceManager.GetString("ProgrammingDataItem_Device", resourceCulture);

	internal static string ProgrammingDataItem_EngineSerialNumber => ResourceManager.GetString("ProgrammingDataItem_EngineSerialNumber", resourceCulture);

	internal static string ProgrammingDataItem_HardwarePartNumber => ResourceManager.GetString("ProgrammingDataItem_HardwarePartNumber", resourceCulture);

	internal static string ProgrammingDataItem_HardwareRevision => ResourceManager.GetString("ProgrammingDataItem_HardwareRevision", resourceCulture);

	internal static string ProgrammingDataItem_LateBoundConfigurationHexFile => ResourceManager.GetString("ProgrammingDataItem_LateBoundConfigurationHexFile", resourceCulture);

	internal static string ProgrammingDataItem_Operation => ResourceManager.GetString("ProgrammingDataItem_Operation", resourceCulture);

	internal static string ProgrammingDataItem_Settings => ResourceManager.GetString("ProgrammingDataItem_Settings", resourceCulture);

	internal static string ProgrammingDataItem_Software => ResourceManager.GetString("ProgrammingDataItem_Software", resourceCulture);

	internal static string ProgrammingDataItem_Unit => ResourceManager.GetString("ProgrammingDataItem_Unit", resourceCulture);

	internal static string ProgrammingDataItem_VehicleIdentificationNumber => ResourceManager.GetString("ProgrammingDataItem_VehicleIdentificationNumber", resourceCulture);

	internal static string ProgrammingDataItemWarning_NoExistingSettingsResetToDefault => ResourceManager.GetString("ProgrammingDataItemWarning_NoExistingSettingsResetToDefault", resourceCulture);

	internal static string ProgrammingDataItemWarning_ResetToDefaultNotRecommended => ResourceManager.GetString("ProgrammingDataItemWarning_ResetToDefaultNotRecommended", resourceCulture);

	internal static string ProgrammingOperation_ChangeDataset => ResourceManager.GetString("ProgrammingOperation_ChangeDataset", resourceCulture);

	internal static string ProgrammingOperation_ReplaceDeviceSettingsWithServerConfiguration => ResourceManager.GetString("ProgrammingOperation_ReplaceDeviceSettingsWithServerConfiguration", resourceCulture);

	internal static string ProgrammingOperation_UpdateDeviceSoftware => ResourceManager.GetString("ProgrammingOperation_UpdateDeviceSoftware", resourceCulture);

	internal static string ProgrammingOperation_UpdateDeviceSoftwareAndChangeDataset => ResourceManager.GetString("ProgrammingOperation_UpdateDeviceSoftwareAndChangeDataset", resourceCulture);

	internal static Bitmap readwrite
	{
		get
		{
			object obj = ResourceManager.GetObject("readwrite", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap remove
	{
		get
		{
			object obj = ResourceManager.GetObject("remove", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string ReprogrammingView_NoTitle => ResourceManager.GetString("ReprogrammingView_NoTitle", resourceCulture);

	internal static string ReprogrammingViewFormat_MissingCBFWarning => ResourceManager.GetString("ReprogrammingViewFormat_MissingCBFWarning", resourceCulture);

	internal static string SelectDataSetPanel_Message_ThisIsEquivalentToTheCurrentDatasetForTheDevice => ResourceManager.GetString("SelectDataSetPanel_Message_ThisIsEquivalentToTheCurrentDatasetForTheDevice", resourceCulture);

	internal static string SelectDataSetPanel_Message_ThisIsTheCurrentDatasetForTheDevice => ResourceManager.GetString("SelectDataSetPanel_Message_ThisIsTheCurrentDatasetForTheDevice", resourceCulture);

	internal static string SelectDataSetSubPanel_SelectDataSet => ResourceManager.GetString("SelectDataSetSubPanel_SelectDataSet", resourceCulture);

	internal static string SelectDataSetSubPanelItem_CannotChangeDataSetForDeviceWithDifferentIdentity => ResourceManager.GetString("SelectDataSetSubPanelItem_CannotChangeDataSetForDeviceWithDifferentIdentity", resourceCulture);

	internal static string SelectDataSetSubPanelItem_NoDataForThisUnit => ResourceManager.GetString("SelectDataSetSubPanelItem_NoDataForThisUnit", resourceCulture);

	internal static string SelectDataSetSubPanelItem_NoOptionsForThisUnitSoftware => ResourceManager.GetString("SelectDataSetSubPanelItem_NoOptionsForThisUnitSoftware", resourceCulture);

	internal static string SelectFirmwareSubPanel_SelectFirmware => ResourceManager.GetString("SelectFirmwareSubPanel_SelectFirmware", resourceCulture);

	internal static string SelectFirmwareSubPanelItem_CannotUpdateSoftwareForDeviceWithDifferentIdentity => ResourceManager.GetString("SelectFirmwareSubPanelItem_CannotUpdateSoftwareForDeviceWithDifferentIdentity", resourceCulture);

	internal static string SelectFirmwareSubPanelItem_MustHaveUnitFileToUpdateSoftwareForThisDevice => ResourceManager.GetString("SelectFirmwareSubPanelItem_MustHaveUnitFileToUpdateSoftwareForThisDevice", resourceCulture);

	internal static string SelectFirmwareSubPanelItem_NoSoftwareUpdatesAvailableForSelectedDevice => ResourceManager.GetString("SelectFirmwareSubPanelItem_NoSoftwareUpdatesAvailableForSelectedDevice", resourceCulture);

	internal static string SelectFirmwareSubPanelItem_UnitFileDoesNotContainInformationForThisDevice => ResourceManager.GetString("SelectFirmwareSubPanelItem_UnitFileDoesNotContainInformationForThisDevice", resourceCulture);

	internal static string SelectFirmwareSubPanelToolTip_AdditionalReprogrammingIsRequiredToUseThisSoftware => ResourceManager.GetString("SelectFirmwareSubPanelToolTip_AdditionalReprogrammingIsRequiredToUseThisSoftware", resourceCulture);

	internal static string SelectOperationPage_Chec => ResourceManager.GetString("SelectOperationPage_Chec", resourceCulture);

	internal static string SelectOperationPage_ClickOK => ResourceManager.GetString("SelectOperationPage_ClickOK", resourceCulture);

	internal static string SelectOperationPage_Compat_CompatibleSetFormat => ResourceManager.GetString("SelectOperationPage_Compat_CompatibleSetFormat", resourceCulture);

	internal static string SelectOperationPage_Compat_TargetDevice => ResourceManager.GetString("SelectOperationPage_Compat_TargetDevice", resourceCulture);

	internal static string SelectOperationPage_CurrentConnectedPowertrain => ResourceManager.GetString("SelectOperationPage_CurrentConnectedPowertrain", resourceCulture);

	internal static string SelectOperationPage_CurrentSoftwareNotFoundForHardware => ResourceManager.GetString("SelectOperationPage_CurrentSoftwareNotFoundForHardware", resourceCulture);

	internal static string SelectOperationPage_DeviceWarning_FormatNoContentForDataSource => ResourceManager.GetString("SelectOperationPage_DeviceWarning_FormatNoContentForDataSource", resourceCulture);

	internal static string SelectOperationPage_DeviceWarning_NoOptionalServerData => ResourceManager.GetString("SelectOperationPage_DeviceWarning_NoOptionalServerData", resourceCulture);

	internal static string SelectOperationPage_DeviceWarning_NoServerData => ResourceManager.GetString("SelectOperationPage_DeviceWarning_NoServerData", resourceCulture);

	internal static string SelectOperationPage_DeviceWarning_NotProgrammable => ResourceManager.GetString("SelectOperationPage_DeviceWarning_NotProgrammable", resourceCulture);

	internal static string SelectOperationPage_DifferentESNOrVINMessage => ResourceManager.GetString("SelectOperationPage_DifferentESNOrVINMessage", resourceCulture);

	internal static string SelectOperationPage_EdexUpgradeMessage => ResourceManager.GetString("SelectOperationPage_EdexUpgradeMessage", resourceCulture);

	internal static string SelectOperationPage_ErrorNoOperation => ResourceManager.GetString("SelectOperationPage_ErrorNoOperation", resourceCulture);

	internal static string SelectOperationPage_ErrorSelectedChannelNull => ResourceManager.GetString("SelectOperationPage_ErrorSelectedChannelNull", resourceCulture);

	internal static string SelectOperationPage_ErrorSelectedUnitNull => ResourceManager.GetString("SelectOperationPage_ErrorSelectedUnitNull", resourceCulture);

	internal static string SelectOperationPage_FormatChangeESN => ResourceManager.GetString("SelectOperationPage_FormatChangeESN", resourceCulture);

	internal static string SelectOperationPage_FormatChangeVIN => ResourceManager.GetString("SelectOperationPage_FormatChangeVIN", resourceCulture);

	internal static string SelectOperationPage_FormatDatasetFileMissing => ResourceManager.GetString("SelectOperationPage_FormatDatasetFileMissing", resourceCulture);

	internal static string SelectOperationPage_FormatDatasetMeaningCannotBeProgrammed => ResourceManager.GetString("SelectOperationPage_FormatDatasetMeaningCannotBeProgrammed", resourceCulture);

	internal static string SelectOperationPage_FormatErrorSelectedDataSourceUnknown => ResourceManager.GetString("SelectOperationPage_FormatErrorSelectedDataSourceUnknown", resourceCulture);

	internal static string SelectOperationPage_FormatFirmwareFileMissing => ResourceManager.GetString("SelectOperationPage_FormatFirmwareFileMissing", resourceCulture);

	internal static string SelectOperationPage_FormatFirmwareFileNotAvailableDueToServerStatus => ResourceManager.GetString("SelectOperationPage_FormatFirmwareFileNotAvailableDueToServerStatus", resourceCulture);

	internal static string SelectOperationPage_FormatFirmwareFileStillToBeDownloaded => ResourceManager.GetString("SelectOperationPage_FormatFirmwareFileStillToBeDownloaded", resourceCulture);

	internal static string SelectOperationPage_FormatMeaningCannotBeProgrammed => ResourceManager.GetString("SelectOperationPage_FormatMeaningCannotBeProgrammed", resourceCulture);

	internal static string SelectOperationPage_Message_ChangingTheDataSetWillRequireTheDataForThisDeviceToNeedToBeRedownloadedFromTheServerIfYourIntentIsToSubsequentlyUpgradeTheSoftwareInThisDevicePleaseUpgradeTheSoftwareFirst => ResourceManager.GetString("SelectOperationPage_Message_ChangingTheDataSetWillRequireTheDataForThisDeviceToNeedToBeRedownloadedFromTheServerIfYourIntentIsToSubsequentlyUpgradeTheSoftwareInThisDevicePleaseUpgradeTheSoftwareFirst", resourceCulture);

	internal static string SelectOperationPage_NewestConnectedPowertrain => ResourceManager.GetString("SelectOperationPage_NewestConnectedPowertrain", resourceCulture);

	internal static string SelectOperationPage_NoCompatibleSoftwareForHardware => ResourceManager.GetString("SelectOperationPage_NoCompatibleSoftwareForHardware", resourceCulture);

	internal static string SelectOperationPage_SelectDevice => ResourceManager.GetString("SelectOperationPage_SelectDevice", resourceCulture);

	internal static string SelectOperationPage_SelectOperation => ResourceManager.GetString("SelectOperationPage_SelectOperation", resourceCulture);

	internal static string SelectOperationPage_SoftwareIsNotCompatible_Information => ResourceManager.GetString("SelectOperationPage_SoftwareIsNotCompatible_Information", resourceCulture);

	internal static string SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_Stop => ResourceManager.GetString("SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_Stop", resourceCulture);

	internal static string SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_Warning => ResourceManager.GetString("SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_Warning", resourceCulture);

	internal static string SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_WarningWithHint => ResourceManager.GetString("SelectOperationPageFormat_ChassisSoftwareIsNotCompatible_WarningWithHint", resourceCulture);

	internal static string SelectOperationPageFormat_ConfigurationDataNotConsistent => ResourceManager.GetString("SelectOperationPageFormat_ConfigurationDataNotConsistent", resourceCulture);

	internal static string SelectOperationPageFormat_ConnectedDeviceNotAMatch => ResourceManager.GetString("SelectOperationPageFormat_ConnectedDeviceNotAMatch", resourceCulture);

	internal static string SelectOperationPageFormat_ConnectedHardwareNotAMatch => ResourceManager.GetString("SelectOperationPageFormat_ConnectedHardwareNotAMatch", resourceCulture);

	internal static string SelectOperationPageFormat_DevicesIncluded => ResourceManager.GetString("SelectOperationPageFormat_DevicesIncluded", resourceCulture);

	internal static string SelectOperationPageFormat_HardwarePartNumberInvalid => ResourceManager.GetString("SelectOperationPageFormat_HardwarePartNumberInvalid", resourceCulture);

	internal static string SelectOperationPageFormat_NotProgrammableForFamily => ResourceManager.GetString("SelectOperationPageFormat_NotProgrammableForFamily", resourceCulture);

	internal static string SelectOperationPageFormat_OEMLatestDataNotAvailable => ResourceManager.GetString("SelectOperationPageFormat_OEMLatestDataNotAvailable", resourceCulture);

	internal static string SelectOperationPageFormat_PhysicalCANWillBeNeeded => ResourceManager.GetString("SelectOperationPageFormat_PhysicalCANWillBeNeeded", resourceCulture);

	internal static string SelectOperationPageFormat_PhysicalEthernetWillBeNeeded => ResourceManager.GetString("SelectOperationPageFormat_PhysicalEthernetWillBeNeeded", resourceCulture);

	internal static string SelectOperationPageFormat_PowertrainSoftwareIsNotCompatible_Stop => ResourceManager.GetString("SelectOperationPageFormat_PowertrainSoftwareIsNotCompatible_Stop", resourceCulture);

	internal static string SelectOperationPageFormat_PowertrainSoftwareIsNotCompatible_Warning => ResourceManager.GetString("SelectOperationPageFormat_PowertrainSoftwareIsNotCompatible_Warning", resourceCulture);

	internal static string SelectOperationPageFormat_RequiredDeviceNotConnected => ResourceManager.GetString("SelectOperationPageFormat_RequiredDeviceNotConnected", resourceCulture);

	internal static string SelectOperationPageFormat_SettingsForChecConnectedDevices => ResourceManager.GetString("SelectOperationPageFormat_SettingsForChecConnectedDevices", resourceCulture);

	internal static string SelectOperationPageFormat_Unit => ResourceManager.GetString("SelectOperationPageFormat_Unit", resourceCulture);

	internal static string SelectOperationPageFormat_UnitInfoNotAvailable => ResourceManager.GetString("SelectOperationPageFormat_UnitInfoNotAvailable", resourceCulture);

	internal static string SelectOperationPageItem_ManualConnection => ResourceManager.GetString("SelectOperationPageItem_ManualConnection", resourceCulture);

	internal static string SelectOperationPageItem_NoDevicesAreConnected => ResourceManager.GetString("SelectOperationPageItem_NoDevicesAreConnected", resourceCulture);

	internal static string SelectSettingsSubPanel_SelectConfiguration => ResourceManager.GetString("SelectSettingsSubPanel_SelectConfiguration", resourceCulture);

	internal static string SelectSettingsSubPanel_SelectSettings => ResourceManager.GetString("SelectSettingsSubPanel_SelectSettings", resourceCulture);

	internal static string SelectUnitSubPanel_SelectUnit => ResourceManager.GetString("SelectUnitSubPanel_SelectUnit", resourceCulture);

	internal static string SelectUnitSubPanelItem_CouldNotReadHardwarePartNumber => ResourceManager.GetString("SelectUnitSubPanelItem_CouldNotReadHardwarePartNumber", resourceCulture);

	internal static string SelectUnitSubPanelItem_FactoryDescription => ResourceManager.GetString("SelectUnitSubPanelItem_FactoryDescription", resourceCulture);

	internal static string SelectUnitSubPanelItem_HistoryDescription => ResourceManager.GetString("SelectUnitSubPanelItem_HistoryDescription", resourceCulture);

	internal static string SelectUnitSubPanelItem_LatestDescription => ResourceManager.GetString("SelectUnitSubPanelItem_LatestDescription", resourceCulture);

	internal static string SelectUnitSubPanelItem_MaxProgrammingUsage => ResourceManager.GetString("SelectUnitSubPanelItem_MaxProgrammingUsage", resourceCulture);

	internal static string SelectUnitSubPanelItem_MVCIServerRequiredButNotAvailable => ResourceManager.GetString("SelectUnitSubPanelItem_MVCIServerRequiredButNotAvailable", resourceCulture);

	internal static string SelectUnitSubPanelItem_MVCIServerRequiredButNotEnabled => ResourceManager.GetString("SelectUnitSubPanelItem_MVCIServerRequiredButNotEnabled", resourceCulture);

	internal static string SelectUnitSubPanelItem_NoUnitDataForConnectedHardware => ResourceManager.GetString("SelectUnitSubPanelItem_NoUnitDataForConnectedHardware", resourceCulture);

	internal static string SelectUnitSubPanelItem_NoUnitDataIsAvailable => ResourceManager.GetString("SelectUnitSubPanelItem_NoUnitDataIsAvailable", resourceCulture);

	internal static string SelectUnitSubPanelItem_OverTheAirDescription => ResourceManager.GetString("SelectUnitSubPanelItem_OverTheAirDescription", resourceCulture);

	internal static string SelectUnitSubPanelItem_ResetConfigurationToDefaultNotRecommended => ResourceManager.GetString("SelectUnitSubPanelItem_ResetConfigurationToDefaultNotRecommended", resourceCulture);

	internal static string SelectUnitSubPanelItem_VehiclePlantDescription => ResourceManager.GetString("SelectUnitSubPanelItem_VehiclePlantDescription", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_BootFirmwareFlashKeyRequiresDownload => ResourceManager.GetString("SelectUnitSubPanelItemFormat_BootFirmwareFlashKeyRequiresDownload", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_DatasetFlashKeyRequiresDownload => ResourceManager.GetString("SelectUnitSubPanelItemFormat_DatasetFlashKeyRequiresDownload", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_FactoryCannotBeProgrammed => ResourceManager.GetString("SelectUnitSubPanelItemFormat_FactoryCannotBeProgrammed", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_FirmwareFlashKeyRequiresDownload => ResourceManager.GetString("SelectUnitSubPanelItemFormat_FirmwareFlashKeyRequiresDownload", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_HardwarePartNumberMismatch => ResourceManager.GetString("SelectUnitSubPanelItemFormat_HardwarePartNumberMismatch", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_HardwarePartNumberNoMatchesAvailable => ResourceManager.GetString("SelectUnitSubPanelItemFormat_HardwarePartNumberNoMatchesAvailable", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_MissingBootFirmwareFlashKey => ResourceManager.GetString("SelectUnitSubPanelItemFormat_MissingBootFirmwareFlashKey", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_MissingCodingParameter => ResourceManager.GetString("SelectUnitSubPanelItemFormat_MissingCodingParameter", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_MissingDatasetFlashKey => ResourceManager.GetString("SelectUnitSubPanelItemFormat_MissingDatasetFlashKey", resourceCulture);

	internal static string SelectUnitSubPanelItemFormat_MissingFirmwareFlashKey => ResourceManager.GetString("SelectUnitSubPanelItemFormat_MissingFirmwareFlashKey", resourceCulture);

	internal static string SelectUnitSubPanelItemName_Defaults => ResourceManager.GetString("SelectUnitSubPanelItemName_Defaults", resourceCulture);

	internal static string SelectUnitSubPanelItemStatus_Default => ResourceManager.GetString("SelectUnitSubPanelItemStatus_Default", resourceCulture);

	internal static string Unit_FormatRemoteDownloadNeededStatus => ResourceManager.GetString("Unit_FormatRemoteDownloadNeededStatus", resourceCulture);

	internal static string Unit_FormatRemoteInProgressStatus => ResourceManager.GetString("Unit_FormatRemoteInProgressStatus", resourceCulture);

	internal static string UnitPartNumberViewDialog_DefaultString => ResourceManager.GetString("UnitPartNumberViewDialog_DefaultString", resourceCulture);

	internal static string UnitPartNumberViewDialog_DownloadData => ResourceManager.GetString("UnitPartNumberViewDialog_DownloadData", resourceCulture);

	internal static string UnitPartNumberViewDialog_FormatCodingFileNotFound => ResourceManager.GetString("UnitPartNumberViewDialog_FormatCodingFileNotFound", resourceCulture);

	internal static string UnitPartNumberViewDialog_FormatNoSettings => ResourceManager.GetString("UnitPartNumberViewDialog_FormatNoSettings", resourceCulture);

	internal static string UnitPartNumberViewDialog_FromParent => ResourceManager.GetString("UnitPartNumberViewDialog_FromParent", resourceCulture);

	internal static string UnitPartNumberViewDialog_Title => ResourceManager.GetString("UnitPartNumberViewDialog_Title", resourceCulture);

	internal static string UnitPartNumberViewDialog_UploadData => ResourceManager.GetString("UnitPartNumberViewDialog_UploadData", resourceCulture);

	internal static Bitmap warning
	{
		get
		{
			object obj = ResourceManager.GetObject("warning", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleClickHereToStartTheOperationForTheDevice1 => ResourceManager.GetString("WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleClickHereToStartTheOperationForTheDevice1", resourceCulture);

	internal static string WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleClickHereToStartTheOperationForTheDevice1Step2Of3 => ResourceManager.GetString("WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleClickHereToStartTheOperationForTheDevice1Step2Of3", resourceCulture);

	internal static string WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleConnectTheDevice1ToProceed => ResourceManager.GetString("WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleConnectTheDevice1ToProceed", resourceCulture);

	internal static string WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleConnectTheDevice1ToProceedStep2Of3 => ResourceManager.GetString("WarningPanelMessageFormat_TheAutomaticReprogrammingOperation0IsRequiredForTheConnectedVehicleConnectTheDevice1ToProceedStep2Of3", resourceCulture);

	internal static string WarningUserYesNoQuestion_VinIsNeededToContinueDataDownload => ResourceManager.GetString("WarningUserYesNoQuestion_VinIsNeededToContinueDataDownload", resourceCulture);

	internal static string Wizard_ButtonAddRequest => ResourceManager.GetString("Wizard_ButtonAddRequest", resourceCulture);

	internal static string Wizard_ButtonBack => ResourceManager.GetString("Wizard_ButtonBack", resourceCulture);

	internal static string Wizard_ButtonConnect => ResourceManager.GetString("Wizard_ButtonConnect", resourceCulture);

	internal static string Wizard_ButtonDownloadData => ResourceManager.GetString("Wizard_ButtonDownloadData", resourceCulture);

	internal static string Wizard_ButtonFinish => ResourceManager.GetString("Wizard_ButtonFinish", resourceCulture);

	internal static string Wizard_ButtonNext => ResourceManager.GetString("Wizard_ButtonNext", resourceCulture);

	internal static string Wizard_ButtonRefresh => ResourceManager.GetString("Wizard_ButtonRefresh", resourceCulture);

	internal static string Wizard_ButtonRefreshAllSoftware => ResourceManager.GetString("Wizard_ButtonRefreshAllSoftware", resourceCulture);

	internal static string Wizard_ButtonRefreshData => ResourceManager.GetString("Wizard_ButtonRefreshData", resourceCulture);

	internal static string Wizard_ButtonRemove => ResourceManager.GetString("Wizard_ButtonRemove", resourceCulture);

	internal static string Wizard_ButtonRemoveData => ResourceManager.GetString("Wizard_ButtonRemoveData", resourceCulture);

	internal static string Wizard_ButtonStart => ResourceManager.GetString("Wizard_ButtonStart", resourceCulture);

	internal static string Wizard_ButtonUploadData => ResourceManager.GetString("Wizard_ButtonUploadData", resourceCulture);

	internal static string Wizard_ButtonView => ResourceManager.GetString("Wizard_ButtonView", resourceCulture);

	internal Resources()
	{
	}
}
