using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;

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
				ResourceManager resourceManager = new ResourceManager("DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties.Resources", typeof(Resources).Assembly);
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

	internal static string Accumulators => ResourceManager.GetString("Accumulators", resourceCulture);

	internal static Bitmap collapseall
	{
		get
		{
			object obj = ResourceManager.GetObject("collapseall", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string ColumnHeaderDevice => ResourceManager.GetString("ColumnHeaderDevice", resourceCulture);

	internal static string ColumnHeaderDiagnosticVariant => ResourceManager.GetString("ColumnHeaderDiagnosticVariant", resourceCulture);

	internal static string ColumnHeaderEquipmentSerialNumber => ResourceManager.GetString("ColumnHeaderEquipmentSerialNumber", resourceCulture);

	internal static string ColumnHeaderFormatSourceN => ResourceManager.GetString("ColumnHeaderFormatSourceN", resourceCulture);

	internal static string ColumnHeaderReason => ResourceManager.GetString("ColumnHeaderReason", resourceCulture);

	internal static string ColumnHeaderSeed => ResourceManager.GetString("ColumnHeaderSeed", resourceCulture);

	internal static string ColumnHeaderSettings => ResourceManager.GetString("ColumnHeaderSettings", resourceCulture);

	internal static string ColumnHeaderTime => ResourceManager.GetString("ColumnHeaderTime", resourceCulture);

	internal static string ColumnHeaderVIN => ResourceManager.GetString("ColumnHeaderVIN", resourceCulture);

	internal static string CompareSendForm_Format_TotalChanges => ResourceManager.GetString("CompareSendForm_Format_TotalChanges", resourceCulture);

	internal static string ConfigurePasswords => ResourceManager.GetString("ConfigurePasswords", resourceCulture);

	internal static string ConfirmParameterSendPrint => ResourceManager.GetString("ConfirmParameterSendPrint", resourceCulture);

	internal static string ContinueImporting => ResourceManager.GetString("ContinueImporting", resourceCulture);

	internal static Bitmap done
	{
		get
		{
			object obj = ResourceManager.GetObject("done", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string EcuStatusFormat_Offline => ResourceManager.GetString("EcuStatusFormat_Offline", resourceCulture);

	internal static string EcuStatusFormat_Online => ResourceManager.GetString("EcuStatusFormat_Online", resourceCulture);

	internal static Bitmap edited
	{
		get
		{
			object obj = ResourceManager.GetObject("edited", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap error
	{
		get
		{
			object obj = ResourceManager.GetObject("error", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap expandall
	{
		get
		{
			object obj = ResourceManager.GetObject("expandall", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string Export => ResourceManager.GetString("Export", resourceCulture);

	internal static string FileImport => ResourceManager.GetString("FileImport", resourceCulture);

	internal static string FormatCanNotImport => ResourceManager.GetString("FormatCanNotImport", resourceCulture);

	internal static string FormatDeviceDoesNotExist => ResourceManager.GetString("FormatDeviceDoesNotExist", resourceCulture);

	internal static string FormatMessageAborted => ResourceManager.GetString("FormatMessageAborted", resourceCulture);

	internal static string FormatMessageOtherError => ResourceManager.GetString("FormatMessageOtherError", resourceCulture);

	internal static string FormatWarningPasswordServicesMissing => ResourceManager.GetString("FormatWarningPasswordServicesMissing", resourceCulture);

	internal static Bitmap group_edit
	{
		get
		{
			object obj = ResourceManager.GetObject("group_edit", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap group_open
	{
		get
		{
			object obj = ResourceManager.GetObject("group_open", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string GroupNameIdentification => ResourceManager.GetString("GroupNameIdentification", resourceCulture);

	internal static string GroupNameInvalidData => ResourceManager.GetString("GroupNameInvalidData", resourceCulture);

	internal static string HistoryImport => ResourceManager.GetString("HistoryImport", resourceCulture);

	internal static string Identification_EcuModel => ResourceManager.GetString("Identification_EcuModel", resourceCulture);

	internal static string Identification_HardwarePartNumber => ResourceManager.GetString("Identification_HardwarePartNumber", resourceCulture);

	internal static string Identification_HardwareRevision => ResourceManager.GetString("Identification_HardwareRevision", resourceCulture);

	internal static string Identification_SoftwarePartNumber => ResourceManager.GetString("Identification_SoftwarePartNumber", resourceCulture);

	internal static string Import => ResourceManager.GetString("Import", resourceCulture);

	internal static string ImportingParameters => ResourceManager.GetString("ImportingParameters", resourceCulture);

	internal static string J2286AccumulatorFilesFilter => ResourceManager.GetString("J2286AccumulatorFilesFilter", resourceCulture);

	internal static string J2286OpenFilesFilter => ResourceManager.GetString("J2286OpenFilesFilter", resourceCulture);

	internal static string J2286SaveFilesFilter => ResourceManager.GetString("J2286SaveFilesFilter", resourceCulture);

	internal static string ListParameterComparePartHeader => ResourceManager.GetString("ListParameterComparePartHeader", resourceCulture);

	internal static string Message_ParameterValueFromParent => ResourceManager.GetString("Message_ParameterValueFromParent", resourceCulture);

	internal static string Message_ParameterValueFromParentEngineeringCorrectionFactor => ResourceManager.GetString("Message_ParameterValueFromParentEngineeringCorrectionFactor", resourceCulture);

	internal static string MessageChangesMayNotHaveTakenEffectAsTheTargetDeviceIsOffline => ResourceManager.GetString("MessageChangesMayNotHaveTakenEffectAsTheTargetDeviceIsOffline", resourceCulture);

	internal static string MessageFormat_AccumulatorServiceImportOnlineChannel => ResourceManager.GetString("MessageFormat_AccumulatorServiceImportOnlineChannel", resourceCulture);

	internal static string MessageFormat_CannotImportParametersToBaseVariant => ResourceManager.GetString("MessageFormat_CannotImportParametersToBaseVariant", resourceCulture);

	internal static string MessageFormat_ChangesToExport => ResourceManager.GetString("MessageFormat_ChangesToExport", resourceCulture);

	internal static string MessageFormat_ChannelLockedForEditing => ResourceManager.GetString("MessageFormat_ChannelLockedForEditing", resourceCulture);

	internal static string MessageFormat_MismatchLastServicedData => ResourceManager.GetString("MessageFormat_MismatchLastServicedData", resourceCulture);

	internal static string MessageFormat_MissingIncompatibilityTable => ResourceManager.GetString("MessageFormat_MissingIncompatibilityTable", resourceCulture);

	internal static string MessageFormat_MissingLastServicedData => ResourceManager.GetString("MessageFormat_MissingLastServicedData", resourceCulture);

	internal static string MessageFormat_NoVcp => ResourceManager.GetString("MessageFormat_NoVcp", resourceCulture);

	internal static string MessageFormat_ShutdownRequestedWithUnsavedChanges => ResourceManager.GetString("MessageFormat_ShutdownRequestedWithUnsavedChanges", resourceCulture);

	internal static string MessageFormat_ToolDoesntSupportIncompatibilityTableUpdate => ResourceManager.GetString("MessageFormat_ToolDoesntSupportIncompatibilityTableUpdate", resourceCulture);

	internal static string MessageFormat_ToolDoesntSupportLastServicedData => ResourceManager.GetString("MessageFormat_ToolDoesntSupportLastServicedData", resourceCulture);

	internal static string MessageFormat_ToolDoesntSupportParameterEditing => ResourceManager.GetString("MessageFormat_ToolDoesntSupportParameterEditing", resourceCulture);

	internal static string MessageFormatErrorsOccurredDuringRetrieval => ResourceManager.GetString("MessageFormatErrorsOccurredDuringRetrieval", resourceCulture);

	internal static string MessageFormatGatherPasswordOtherError => ResourceManager.GetString("MessageFormatGatherPasswordOtherError", resourceCulture);

	internal static string MessageFormatGatherPasswordVehicleMoving => ResourceManager.GetString("MessageFormatGatherPasswordVehicleMoving", resourceCulture);

	internal static string MessageFormatPasswordForListSuccessfullyChanged => ResourceManager.GetString("MessageFormatPasswordForListSuccessfullyChanged", resourceCulture);

	internal static string MessageFormatPasswordForListSuccessfullyCleared => ResourceManager.GetString("MessageFormatPasswordForListSuccessfullyCleared", resourceCulture);

	internal static string MessageFormatSelectVariantForDevice => ResourceManager.GetString("MessageFormatSelectVariantForDevice", resourceCulture);

	internal static string MessageFormatWarningCorruptFile => ResourceManager.GetString("MessageFormatWarningCorruptFile", resourceCulture);

	internal static string MessageFormatWhileConfiguringNewPassword => ResourceManager.GetString("MessageFormatWhileConfiguringNewPassword", resourceCulture);

	internal static string MessageFormatWhileSubmittingOldPassword => ResourceManager.GetString("MessageFormatWhileSubmittingOldPassword", resourceCulture);

	internal static string MessageMustEnterNewPassword => ResourceManager.GetString("MessageMustEnterNewPassword", resourceCulture);

	internal static string MessageMustProvidePreviousPassword => ResourceManager.GetString("MessageMustProvidePreviousPassword", resourceCulture);

	internal static string NotAvailable => ResourceManager.GetString("NotAvailable", resourceCulture);

	internal static string OpenHistoryForm_EntryNameFormat => ResourceManager.GetString("OpenHistoryForm_EntryNameFormat", resourceCulture);

	internal static string OpenServerDataForm_SourceDownload => ResourceManager.GetString("OpenServerDataForm_SourceDownload", resourceCulture);

	internal static string OpenServerDataForm_SourceUpload => ResourceManager.GetString("OpenServerDataForm_SourceUpload", resourceCulture);

	internal static Bitmap outofrange
	{
		get
		{
			object obj = ResourceManager.GetObject("outofrange", resourceCulture);
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

	internal static string Parameters => ResourceManager.GetString("Parameters", resourceCulture);

	internal static string ParametersWritten => ResourceManager.GetString("ParametersWritten", resourceCulture);

	internal static string ParameterView_ConnectOffline => ResourceManager.GetString("ParameterView_ConnectOffline", resourceCulture);

	internal static string ParameterView_ErrorMessageFormat => ResourceManager.GetString("ParameterView_ErrorMessageFormat", resourceCulture);

	internal static string ParameterView_ErrorMessageFormat_UnknownParameter => ResourceManager.GetString("ParameterView_ErrorMessageFormat_UnknownParameter", resourceCulture);

	internal static string ParameterView_Format_DeviceReportedTheFollowingErrors => ResourceManager.GetString("ParameterView_Format_DeviceReportedTheFollowingErrors", resourceCulture);

	internal static string ParameterView_Format_DeviceReportedTheFollowingWarnings => ResourceManager.GetString("ParameterView_Format_DeviceReportedTheFollowingWarnings", resourceCulture);

	internal static string ParameterView_Format_ErrorsOccurred_NoValidation => ResourceManager.GetString("ParameterView_Format_ErrorsOccurred_NoValidation", resourceCulture);

	internal static string ParameterView_Format_FullStatus => ResourceManager.GetString("ParameterView_Format_FullStatus", resourceCulture);

	internal static string ParameterView_Format_StatusLabel => ResourceManager.GetString("ParameterView_Format_StatusLabel", resourceCulture);

	internal static string ParameterView_Format_WarningOrError => ResourceManager.GetString("ParameterView_Format_WarningOrError", resourceCulture);

	internal static string ParameterView_FormatPrecondition => ResourceManager.GetString("ParameterView_FormatPrecondition", resourceCulture);

	internal static string ParameterView_FormatPreconditionWithDialog => ResourceManager.GetString("ParameterView_FormatPreconditionWithDialog", resourceCulture);

	internal static string ParameterView_OneOrMoreErrorsAndWarningsDuringWrite => ResourceManager.GetString("ParameterView_OneOrMoreErrorsAndWarningsDuringWrite", resourceCulture);

	internal static string ParameterView_OneOrMoreErrorsDuringWrite => ResourceManager.GetString("ParameterView_OneOrMoreErrorsDuringWrite", resourceCulture);

	internal static string ParameterView_OneOrMoreWarningsDuringWrite => ResourceManager.GetString("ParameterView_OneOrMoreWarningsDuringWrite", resourceCulture);

	internal static string ProvidePassword => ResourceManager.GetString("ProvidePassword", resourceCulture);

	internal static string ReadingParameters => ResourceManager.GetString("ReadingParameters", resourceCulture);

	internal static Bitmap readwrite
	{
		get
		{
			object obj = ResourceManager.GetObject("readwrite", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string ReportError_NotAValidFile => ResourceManager.GetString("ReportError_NotAValidFile", resourceCulture);

	internal static string ReportError_OneOrMoreErrorsOccurredImporting => ResourceManager.GetString("ReportError_OneOrMoreErrorsOccurredImporting", resourceCulture);

	internal static string ReportError_OneOrMoreErrorsOccurredReading => ResourceManager.GetString("ReportError_OneOrMoreErrorsOccurredReading", resourceCulture);

	internal static string ReportSuccess_ParametersSuccessfullyImported => ResourceManager.GetString("ReportSuccess_ParametersSuccessfullyImported", resourceCulture);

	internal static string ReportSuccess_ParametersSuccessfullyRead => ResourceManager.GetString("ReportSuccess_ParametersSuccessfullyRead", resourceCulture);

	internal static string ReportWarning_ParameterBlacklistFailure => ResourceManager.GetString("ReportWarning_ParameterBlacklistFailure", resourceCulture);

	internal static string ReportWarning_ParameterNoPartNumberFailure => ResourceManager.GetString("ReportWarning_ParameterNoPartNumberFailure", resourceCulture);

	internal static string ReportWarningNoParametersChanged => ResourceManager.GetString("ReportWarningNoParametersChanged", resourceCulture);

	internal static string ReviewParameters => ResourceManager.GetString("ReviewParameters", resourceCulture);

	internal static string SuffixFormat_HasPresets => ResourceManager.GetString("SuffixFormat_HasPresets", resourceCulture);

	internal static string UserWarningIntro => ResourceManager.GetString("UserWarningIntro", resourceCulture);

	internal static string UserWarningLogLabel => ResourceManager.GetString("UserWarningLogLabel", resourceCulture);

	internal static string UserWarningStatusLabel => ResourceManager.GetString("UserWarningStatusLabel", resourceCulture);

	internal static string UserWarningTitle => ResourceManager.GetString("UserWarningTitle", resourceCulture);

	internal static string ValueNoSource => ResourceManager.GetString("ValueNoSource", resourceCulture);

	internal static string VariantDoesNotMatchFormat => ResourceManager.GetString("VariantDoesNotMatchFormat", resourceCulture);

	internal static string VariantMethodSuffix_Assumed => ResourceManager.GetString("VariantMethodSuffix_Assumed", resourceCulture);

	internal static string VariantMethodSuffix_Preset => ResourceManager.GetString("VariantMethodSuffix_Preset", resourceCulture);

	internal static string VariantMethodSuffix_PreviousSource => ResourceManager.GetString("VariantMethodSuffix_PreviousSource", resourceCulture);

	internal static Bitmap warning
	{
		get
		{
			object obj = ResourceManager.GetObject("warning", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string WarningPasswordServicesMissingTitle => ResourceManager.GetString("WarningPasswordServicesMissingTitle", resourceCulture);

	internal static string WritingParameters => ResourceManager.GetString("WritingParameters", resourceCulture);

	internal Resources()
	{
	}
}
