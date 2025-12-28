using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

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
				ResourceManager resourceManager = new ResourceManager("DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources", typeof(Resources).Assembly);
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

	internal static Bitmap about
	{
		get
		{
			object obj = ResourceManager.GetObject("about", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap add_user_event
	{
		get
		{
			object obj = ResourceManager.GetObject("add_user_event", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string AddCbfForm_AutoconnectEcuDissconnectContinue => ResourceManager.GetString("AddCbfForm_AutoconnectEcuDissconnectContinue", resourceCulture);

	internal static string AddCbfForm_Continue => ResourceManager.GetString("AddCbfForm_Continue", resourceCulture);

	internal static string AddCbfForm_CouldNotSelectRelatedEcu => ResourceManager.GetString("AddCbfForm_CouldNotSelectRelatedEcu", resourceCulture);

	internal static string AddCbfForm_FormatAddNewCbf => ResourceManager.GetString("AddCbfForm_FormatAddNewCbf", resourceCulture);

	internal static string AddCbfForm_FormatChannelClosedForEcu => ResourceManager.GetString("AddCbfForm_FormatChannelClosedForEcu", resourceCulture);

	internal static string AddCbfForm_FormatLogFileClosedForEcu => ResourceManager.GetString("AddCbfForm_FormatLogFileClosedForEcu", resourceCulture);

	internal static string AddCbfForm_FormatRemoveExistingCbf => ResourceManager.GetString("AddCbfForm_FormatRemoveExistingCbf", resourceCulture);

	internal static string AddCbfForm_FormatSelectedRelatedEcu => ResourceManager.GetString("AddCbfForm_FormatSelectedRelatedEcu", resourceCulture);

	internal static string AddCbfForm_NewFile => ResourceManager.GetString("AddCbfForm_NewFile", resourceCulture);

	internal static string AddCbfForm_ResumingAutoconnect => ResourceManager.GetString("AddCbfForm_ResumingAutoconnect", resourceCulture);

	internal static string AddCbfForm_SuspendingAutoconnect => ResourceManager.GetString("AddCbfForm_SuspendingAutoconnect", resourceCulture);

	internal static string AddCbfForm_UserCanceled => ResourceManager.GetString("AddCbfForm_UserCanceled", resourceCulture);

	internal static string AdrAdditionSaveLocationDialogMessage => ResourceManager.GetString("AdrAdditionSaveLocationDialogMessage", resourceCulture);

	internal static Bitmap autoconnect_off
	{
		get
		{
			object obj = ResourceManager.GetObject("autoconnect_off", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap autoconnect_on
	{
		get
		{
			object obj = ResourceManager.GetObject("autoconnect_on", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap autoconnect_pause
	{
		get
		{
			object obj = ResourceManager.GetObject("autoconnect_pause", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap back
	{
		get
		{
			object obj = ResourceManager.GetObject("back", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string BusMonitor_OpenFileDialogFilter => ResourceManager.GetString("BusMonitor_OpenFileDialogFilter", resourceCulture);

	internal static string BusMonitor_OpenFileDialogTitle => ResourceManager.GetString("BusMonitor_OpenFileDialogTitle", resourceCulture);

	internal static string BusMonitor_SaveFileDialogFilterCAN => ResourceManager.GetString("BusMonitor_SaveFileDialogFilterCAN", resourceCulture);

	internal static string BusMonitor_SaveFileDialogFilterEthernet => ResourceManager.GetString("BusMonitor_SaveFileDialogFilterEthernet", resourceCulture);

	internal static string BusMonitor_SaveFileDialogTitle => ResourceManager.GetString("BusMonitor_SaveFileDialogTitle", resourceCulture);

	internal static string BusMonitorForm_CannotLocateInterfaceNoConfiguredSetting => ResourceManager.GetString("BusMonitorForm_CannotLocateInterfaceNoConfiguredSetting", resourceCulture);

	internal static string BusMonitorForm_CannotLocateInterfaceWithConfiguredSetting => ResourceManager.GetString("BusMonitorForm_CannotLocateInterfaceWithConfiguredSetting", resourceCulture);

	internal static string BusMonitorForm_ConnectionResource => ResourceManager.GetString("BusMonitorForm_ConnectionResource", resourceCulture);

	internal static string BusMonitorForm_ConnectionResourceWithBaud => ResourceManager.GetString("BusMonitorForm_ConnectionResourceWithBaud", resourceCulture);

	internal static string BusMonitorForm_ContentClearPrompt => ResourceManager.GetString("BusMonitorForm_ContentClearPrompt", resourceCulture);

	internal static string BusMonitorForm_MonitorAll => ResourceManager.GetString("BusMonitorForm_MonitorAll", resourceCulture);

	internal static string BusMonitorForm_MonitorAllKnown => ResourceManager.GetString("BusMonitorForm_MonitorAllKnown", resourceCulture);

	internal static string BusMonitorForm_MonitorException => ResourceManager.GetString("BusMonitorForm_MonitorException", resourceCulture);

	internal static string BusMonitorForm_Paused => ResourceManager.GetString("BusMonitorForm_Paused", resourceCulture);

	internal static string BusMonitorForm_RunningNoFrames => ResourceManager.GetString("BusMonitorForm_RunningNoFrames", resourceCulture);

	internal static string BusMonitorForm_SelectedResourceToolTip => ResourceManager.GetString("BusMonitorForm_SelectedResourceToolTip", resourceCulture);

	internal static string BusMonitorForm_Stopped => ResourceManager.GetString("BusMonitorForm_Stopped", resourceCulture);

	internal static string BusMonitorForm_Title => ResourceManager.GetString("BusMonitorForm_Title", resourceCulture);

	internal static string BusMonitorStatisticsForm_BurstsTotal => ResourceManager.GetString("BusMonitorStatisticsForm_BurstsTotal", resourceCulture);

	internal static string BusMonitorStatisticsForm_BurstTime => ResourceManager.GetString("BusMonitorStatisticsForm_BurstTime", resourceCulture);

	internal static string BusMonitorStatisticsForm_Busload => ResourceManager.GetString("BusMonitorStatisticsForm_Busload", resourceCulture);

	internal static string BusMonitorStatisticsForm_ChipState => ResourceManager.GetString("BusMonitorStatisticsForm_ChipState", resourceCulture);

	internal static string BusMonitorStatisticsForm_ChipStateActive => ResourceManager.GetString("BusMonitorStatisticsForm_ChipStateActive", resourceCulture);

	internal static string BusMonitorStatisticsForm_ChipStateBusoff => ResourceManager.GetString("BusMonitorStatisticsForm_ChipStateBusoff", resourceCulture);

	internal static string BusMonitorStatisticsForm_ChipStatePassive => ResourceManager.GetString("BusMonitorStatisticsForm_ChipStatePassive", resourceCulture);

	internal static string BusMonitorStatisticsForm_ChipStateWarning => ResourceManager.GetString("BusMonitorStatisticsForm_ChipStateWarning", resourceCulture);

	internal static string BusMonitorStatisticsForm_DataFramesPerSecond => ResourceManager.GetString("BusMonitorStatisticsForm_DataFramesPerSecond", resourceCulture);

	internal static string BusMonitorStatisticsForm_DataFramesTotal => ResourceManager.GetString("BusMonitorStatisticsForm_DataFramesTotal", resourceCulture);

	internal static string BusMonitorStatisticsForm_ErrorFramesPerSecond => ResourceManager.GetString("BusMonitorStatisticsForm_ErrorFramesPerSecond", resourceCulture);

	internal static string BusMonitorStatisticsForm_ErrorFramesTotal => ResourceManager.GetString("BusMonitorStatisticsForm_ErrorFramesTotal", resourceCulture);

	internal static string BusMonitorStatisticsForm_FilteredDataFramesPerSecond => ResourceManager.GetString("BusMonitorStatisticsForm_FilteredDataFramesPerSecond", resourceCulture);

	internal static string BusMonitorStatisticsForm_FilteredDataFramesTotal => ResourceManager.GetString("BusMonitorStatisticsForm_FilteredDataFramesTotal", resourceCulture);

	internal static string BusMonitorStatisticsForm_FramesPerBurst => ResourceManager.GetString("BusMonitorStatisticsForm_FramesPerBurst", resourceCulture);

	internal static string BusMonitorStatisticsForm_IdentifierDataFramesPerSecond => ResourceManager.GetString("BusMonitorStatisticsForm_IdentifierDataFramesPerSecond", resourceCulture);

	internal static string BusMonitorStatisticsForm_IdentifierDataFramesTotal => ResourceManager.GetString("BusMonitorStatisticsForm_IdentifierDataFramesTotal", resourceCulture);

	internal static string BusMonitorStatisticsForm_NotAvailable => ResourceManager.GetString("BusMonitorStatisticsForm_NotAvailable", resourceCulture);

	internal static string BusMonitorStatisticsForm_ReceiveErrorCount => ResourceManager.GetString("BusMonitorStatisticsForm_ReceiveErrorCount", resourceCulture);

	internal static string BusMonitorStatisticsForm_TransmitErrorCount => ResourceManager.GetString("BusMonitorStatisticsForm_TransmitErrorCount", resourceCulture);

	internal static UnmanagedMemoryStream button_click => ResourceManager.GetStream("button_click", resourceCulture);

	internal static string CaptionFileAccessError => ResourceManager.GetString("CaptionFileAccessError", resourceCulture);

	internal static string CaptionFormatPrintLogInfo => ResourceManager.GetString("CaptionFormatPrintLogInfo", resourceCulture);

	internal static Bitmap caution
	{
		get
		{
			object obj = ResourceManager.GetObject("caution", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string CautionHTML => ResourceManager.GetString("CautionHTML", resourceCulture);

	internal static Bitmap cd_pause
	{
		get
		{
			object obj = ResourceManager.GetObject("cd_pause", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap clock_refresh
	{
		get
		{
			object obj = ResourceManager.GetObject("clock_refresh", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap collapseall
	{
		get
		{
			object obj = ResourceManager.GetObject("collapseall", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string CommandCloseConnections => ResourceManager.GetString("CommandCloseConnections", resourceCulture);

	internal static string CommandCloseLog => ResourceManager.GetString("CommandCloseLog", resourceCulture);

	internal static string CommandLineArgInvalidFileType => ResourceManager.GetString("CommandLineArgInvalidFileType", resourceCulture);

	internal static string CommandPause => ResourceManager.GetString("CommandPause", resourceCulture);

	internal static string CommandPlay => ResourceManager.GetString("CommandPlay", resourceCulture);

	internal static string ComponentGroup_ConfiguredTranslators => ResourceManager.GetString("ComponentGroup_ConfiguredTranslators", resourceCulture);

	internal static string ComponentGroup_SupportedDevices => ResourceManager.GetString("ComponentGroup_SupportedDevices", resourceCulture);

	internal static string ComponentInfo_ApplicationType => ResourceManager.GetString("ComponentInfo_ApplicationType", resourceCulture);

	internal static string ComponentInfo_Caesar => ResourceManager.GetString("ComponentInfo_Caesar", resourceCulture);

	internal static string ComponentInfo_DoIP_PDU_API => ResourceManager.GetString("ComponentInfo_DoIP_PDU-API", resourceCulture);

	internal static string ComponentInfo_Globals => ResourceManager.GetString("ComponentInfo_Globals", resourceCulture);

	internal static string ComponentInfo_MVCI => ResourceManager.GetString("ComponentInfo_MVCI", resourceCulture);

	internal static string ComponentInfo_NotAvailable => ResourceManager.GetString("ComponentInfo_NotAvailable", resourceCulture);

	internal static string ComponentInfo_NotEnabled => ResourceManager.GetString("ComponentInfo_NotEnabled", resourceCulture);

	internal static string ComponentInfo_NotFound => ResourceManager.GetString("ComponentInfo_NotFound", resourceCulture);

	internal static string ComponentInfo_RP1210 => ResourceManager.GetString("ComponentInfo_RP1210", resourceCulture);

	internal static string ComponentInfo_SAPI => ResourceManager.GetString("ComponentInfo_SAPI", resourceCulture);

	internal static string ComponentInfo_Sid => ResourceManager.GetString("ComponentInfo_Sid", resourceCulture);

	internal static string ConnectionDialog_All => ResourceManager.GetString("ConnectionDialog_All", resourceCulture);

	internal static string ConnectionDialog_RemovePreviousDescriptionFileFailed => ResourceManager.GetString("ConnectionDialog_RemovePreviousDescriptionFileFailed", resourceCulture);

	internal static string ConnectionDialog_SelectDescriptionFileFilter => ResourceManager.GetString("ConnectionDialog_SelectDescriptionFileFilter", resourceCulture);

	internal static string ConnectionDialog_SelectDescriptionFileTitle => ResourceManager.GetString("ConnectionDialog_SelectDescriptionFileTitle", resourceCulture);

	internal static string ConnectionDialog_UseDetectedVariant => ResourceManager.GetString("ConnectionDialog_UseDetectedVariant", resourceCulture);

	internal static string ConnectionDialog_WrongDescriptionFileTarget => ResourceManager.GetString("ConnectionDialog_WrongDescriptionFileTarget", resourceCulture);

	internal static string ConnectionDialog_WrongDescriptionFileTypeTarget => ResourceManager.GetString("ConnectionDialog_WrongDescriptionFileTypeTarget", resourceCulture);

	internal static string ConnectionDialog_WrongDescriptionFileTypeTargetNoMvciFile => ResourceManager.GetString("ConnectionDialog_WrongDescriptionFileTypeTargetNoMvciFile", resourceCulture);

	internal static string ConnectionOptionsPanel_CheckRollCallSupport_Unsupported_Feature => ResourceManager.GetString("ConnectionOptionsPanel_CheckRollCallSupport_Unsupported_Feature", resourceCulture);

	internal static string ConnectionOptionsPanel_CheckRollCallSupport_Untested_Support_Feature => ResourceManager.GetString("ConnectionOptionsPanel_CheckRollCallSupport_Untested_Support_Feature", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_0 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_0", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_1 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_1", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_11 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_11", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_127 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_127", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_23 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_23", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_230 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_230", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_232 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_232", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_3 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_3", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_33 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_33", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_37 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_37", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_42 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_42", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_49 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_49", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_61 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_61", resourceCulture);

	internal static string ConnectionOptionsPanel_IdentifierUDS_71 => ResourceManager.GetString("ConnectionOptionsPanel_IdentifierUDS_71", resourceCulture);

	internal static string ConnectionOptionsPanel_RollCallSupport_NoSelectedTranslator => ResourceManager.GetString("ConnectionOptionsPanel_RollCallSupport_NoSelectedTranslator", resourceCulture);

	internal static string ConnectionRequiredNow => ResourceManager.GetString("ConnectionRequiredNow", resourceCulture);

	internal static string ConnectionRequiredWithin1Day => ResourceManager.GetString("ConnectionRequiredWithin1Day", resourceCulture);

	internal static Bitmap copy
	{
		get
		{
			object obj = ResourceManager.GetObject("copy", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap cut
	{
		get
		{
			object obj = ResourceManager.GetObject("cut", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string DialogsOptionasPanel_WarningAutoBaudRateUnavailable => ResourceManager.GetString("DialogsOptionasPanel_WarningAutoBaudRateUnavailable", resourceCulture);

	internal static string DialogsOptionasPanel_WarningRollCallDisabled => ResourceManager.GetString("DialogsOptionasPanel_WarningRollCallDisabled", resourceCulture);

	internal static string DialogsOptionsPanel_WarningChangeLanguage => ResourceManager.GetString("DialogsOptionsPanel_WarningChangeLanguage", resourceCulture);

	internal static string DialogsOptionsPanel_WarningCloseConnections => ResourceManager.GetString("DialogsOptionsPanel_WarningCloseConnections", resourceCulture);

	internal static string DialogsOptionsPanel_WarningCloseLogFile => ResourceManager.GetString("DialogsOptionsPanel_WarningCloseLogFile", resourceCulture);

	internal static string DialogsOptionsPanel_WarningMinimumRequiredRam => ResourceManager.GetString("DialogsOptionsPanel_WarningMinimumRequiredRam", resourceCulture);

	internal static string DialogsOptionsPanel_WarningProcessor => ResourceManager.GetString("DialogsOptionsPanel_WarningProcessor", resourceCulture);

	internal static string DialogsOptionsPanel_WarningSystemModel => ResourceManager.GetString("DialogsOptionsPanel_WarningSystemModel", resourceCulture);

	internal static string DialogsOptionsPanel_WarningTroubleshootingUnavailable => ResourceManager.GetString("DialogsOptionsPanel_WarningTroubleshootingUnavailable", resourceCulture);

	internal static Bitmap disk_blue
	{
		get
		{
			object obj = ResourceManager.GetObject("disk_blue", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap document_plain
	{
		get
		{
			object obj = ResourceManager.GetObject("document_plain", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string DurationFormat_HoursMinutes => ResourceManager.GetString("DurationFormat_HoursMinutes", resourceCulture);

	internal static string DurationFormat_Minutes => ResourceManager.GetString("DurationFormat_Minutes", resourceCulture);

	internal static string DurationFormat_Seconds => ResourceManager.GetString("DurationFormat_Seconds", resourceCulture);

	internal static string EcuStatus_ConnectionsHeading => ResourceManager.GetString("EcuStatus_ConnectionsHeading", resourceCulture);

	internal static string EcuStatus_Format_ActiveFaults => ResourceManager.GetString("EcuStatus_Format_ActiveFaults", resourceCulture);

	internal static string EcuStatus_Format_Connecting => ResourceManager.GetString("EcuStatus_Format_Connecting", resourceCulture);

	internal static string EcuStatus_Format_OtherState => ResourceManager.GetString("EcuStatus_Format_OtherState", resourceCulture);

	internal static string EcuStatus_Format_OtherStatusWithPercentage => ResourceManager.GetString("EcuStatus_Format_OtherStatusWithPercentage", resourceCulture);

	internal static string EcuStatus_LoggedConnectionsHeading => ResourceManager.GetString("EcuStatus_LoggedConnectionsHeading", resourceCulture);

	internal static string EcuStatus_LoggedConnectionsPausedHeading => ResourceManager.GetString("EcuStatus_LoggedConnectionsPausedHeading", resourceCulture);

	internal static string EcuStatus_Separator => ResourceManager.GetString("EcuStatus_Separator", resourceCulture);

	internal static string EcuStatusFormat_Offline => ResourceManager.GetString("EcuStatusFormat_Offline", resourceCulture);

	internal static string EcuStatusItem_DescriptionToolTipGenericFormat => ResourceManager.GetString("EcuStatusItem_DescriptionToolTipGenericFormat", resourceCulture);

	internal static string EcuStatusMenu_ClearAllConnectionErrors => ResourceManager.GetString("EcuStatusMenu_ClearAllConnectionErrors", resourceCulture);

	internal static string EcuStatusMenu_ClearConnectionError => ResourceManager.GetString("EcuStatusMenu_ClearConnectionError", resourceCulture);

	internal static string EcuStatusMenu_Close => ResourceManager.GetString("EcuStatusMenu_Close", resourceCulture);

	internal static string EcuStatusMenu_Connect => ResourceManager.GetString("EcuStatusMenu_Connect", resourceCulture);

	internal static string EcuStatusMenu_Disconnect => ResourceManager.GetString("EcuStatusMenu_Disconnect", resourceCulture);

	internal static string EcuStatusMenu_Reconnect => ResourceManager.GetString("EcuStatusMenu_Reconnect", resourceCulture);

	internal static string EcuStatusMenu_RetryAutomaticConnection => ResourceManager.GetString("EcuStatusMenu_RetryAutomaticConnection", resourceCulture);

	internal static string EcuStatusView_CertificateManagementServerNotInstalled => ResourceManager.GetString("EcuStatusView_CertificateManagementServerNotInstalled", resourceCulture);

	internal static string EcuStatusView_CertificateManagementServerNotResponding => ResourceManager.GetString("EcuStatusView_CertificateManagementServerNotResponding", resourceCulture);

	internal static string EcuStatusView_CertificateManagementServerNotRunning => ResourceManager.GetString("EcuStatusView_CertificateManagementServerNotRunning", resourceCulture);

	internal static string EcuStatusView_CertificatesNotAvailable => ResourceManager.GetString("EcuStatusView_CertificatesNotAvailable", resourceCulture);

	internal static string EcuStatusView_Compat_CompatibleOptions => ResourceManager.GetString("EcuStatusView_Compat_CompatibleOptions", resourceCulture);

	internal static string EcuStatusView_Compat_TargetDevice => ResourceManager.GetString("EcuStatusView_Compat_TargetDevice", resourceCulture);

	internal static string EcuStatusView_ConnectedDevicesCompatible => ResourceManager.GetString("EcuStatusView_ConnectedDevicesCompatible", resourceCulture);

	internal static string EcuStatusView_FormatFirewallConfigurationIncorrect => ResourceManager.GetString("EcuStatusView_FormatFirewallConfigurationIncorrect", resourceCulture);

	internal static string EcuStatusView_NoDepotReprogramming => ResourceManager.GetString("EcuStatusView_NoDepotReprogramming", resourceCulture);

	internal static string EcuStatusView_NoEdexReprogramming => ResourceManager.GetString("EcuStatusView_NoEdexReprogramming", resourceCulture);

	internal static string EcuStatusView_NoUnitData => ResourceManager.GetString("EcuStatusView_NoUnitData", resourceCulture);

	internal static string EcuStatusView_Reading => ResourceManager.GetString("EcuStatusView_Reading", resourceCulture);

	internal static string EcuStatusView_TargetNotInCompat => ResourceManager.GetString("EcuStatusView_TargetNotInCompat", resourceCulture);

	internal static string EcuStatusView_UnitDataSoftwareNotInCompat => ResourceManager.GetString("EcuStatusView_UnitDataSoftwareNotInCompat", resourceCulture);

	internal static string EcuStatusView_UnitDataSoftwareSetNotInCompat => ResourceManager.GetString("EcuStatusView_UnitDataSoftwareSetNotInCompat", resourceCulture);

	internal static string EcuStatusView_Writing => ResourceManager.GetString("EcuStatusView_Writing", resourceCulture);

	internal static string EcuStatusView_WrongCertificateManagementServerReference => ResourceManager.GetString("EcuStatusView_WrongCertificateManagementServerReference", resourceCulture);

	internal static Bitmap environment_view
	{
		get
		{
			object obj = ResourceManager.GetObject("environment_view", resourceCulture);
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

	internal static string FilterDialog_SelectAll => ResourceManager.GetString("FilterDialog_SelectAll", resourceCulture);

	internal static string FilterDialog_SelectNone => ResourceManager.GetString("FilterDialog_SelectNone", resourceCulture);

	internal static string FilterDialog_Title => ResourceManager.GetString("FilterDialog_Title", resourceCulture);

	internal static string FilterDialog_UserText => ResourceManager.GetString("FilterDialog_UserText", resourceCulture);

	internal static Bitmap find_next
	{
		get
		{
			object obj = ResourceManager.GetObject("find_next", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap find_previous
	{
		get
		{
			object obj = ResourceManager.GetObject("find_previous", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap FleetInfoHeaderImage
	{
		get
		{
			object obj = ResourceManager.GetObject("FleetInfoHeaderImage", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap folder_out
	{
		get
		{
			object obj = ResourceManager.GetObject("folder_out", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string Format_BluetoothPerformanceWarning => ResourceManager.GetString("Format_BluetoothPerformanceWarning", resourceCulture);

	internal static string Format_FilterButtonCount => ResourceManager.GetString("Format_FilterButtonCount", resourceCulture);

	internal static string Format_InvalidVariantWarning => ResourceManager.GetString("Format_InvalidVariantWarning", resourceCulture);

	internal static string Format_NewVersionAvailable => ResourceManager.GetString("Format_NewVersionAvailable", resourceCulture);

	internal static string Format_NewVersionAvailableService => ResourceManager.GetString("Format_NewVersionAvailableService", resourceCulture);

	internal static string Format_PendingVariantWarning => ResourceManager.GetString("Format_PendingVariantWarning", resourceCulture);

	internal static string Format_ProhibitedAdapterDriverVersionWarning => ResourceManager.GetString("Format_ProhibitedAdapterDriverVersionWarning", resourceCulture);

	internal static string Format_ProhibitedAdapterWarning => ResourceManager.GetString("Format_ProhibitedAdapterWarning", resourceCulture);

	internal static string Format_RequiresEthernetWarning => ResourceManager.GetString("Format_RequiresEthernetWarning", resourceCulture);

	internal static string FormatActiveFaultStatusPlural => ResourceManager.GetString("FormatActiveFaultStatusPlural", resourceCulture);

	internal static string FormatActiveFaultStatusSingular => ResourceManager.GetString("FormatActiveFaultStatusSingular", resourceCulture);

	internal static string FormatAutoConnectFailed => ResourceManager.GetString("FormatAutoConnectFailed", resourceCulture);

	internal static string FormatBaseVariantWarning => ResourceManager.GetString("FormatBaseVariantWarning", resourceCulture);

	internal static string FormatBaseVariantWarning_Engineering => ResourceManager.GetString("FormatBaseVariantWarning_Engineering", resourceCulture);

	internal static string FormatConnectFailed => ResourceManager.GetString("FormatConnectFailed", resourceCulture);

	internal static string FormatConnectionRequiredWithnXDays => ResourceManager.GetString("FormatConnectionRequiredWithnXDays", resourceCulture);

	internal static string FormatConnectionStatusPlural => ResourceManager.GetString("FormatConnectionStatusPlural", resourceCulture);

	internal static string FormatConnectionStatusSingular => ResourceManager.GetString("FormatConnectionStatusSingular", resourceCulture);

	internal static string FormatConnectProgressWithCompStatus => ResourceManager.GetString("FormatConnectProgressWithCompStatus", resourceCulture);

	internal static string FormatConnectProgressWithoutCompStatus => ResourceManager.GetString("FormatConnectProgressWithoutCompStatus", resourceCulture);

	internal static string FormatDaysLeftUntilSubscriptionExpires => ResourceManager.GetString("FormatDaysLeftUntilSubscriptionExpires", resourceCulture);

	internal static string FormatDisconnectingEcu => ResourceManager.GetString("FormatDisconnectingEcu", resourceCulture);

	internal static string FormatFilteredAmountBanner => ResourceManager.GetString("FormatFilteredAmountBanner", resourceCulture);

	internal static string FormatLastServerUpdateTimeandTimeZone => ResourceManager.GetString("FormatLastServerUpdateTimeandTimeZone", resourceCulture);

	internal static string FormatLastUpdateCheckWasDaysAgoCheckForUpdates => ResourceManager.GetString("FormatLastUpdateCheckWasDaysAgoCheckForUpdates", resourceCulture);

	internal static string FormatLegacySidRegistryKeyDetected => ResourceManager.GetString("FormatLegacySidRegistryKeyDetected", resourceCulture);

	internal static string FormatLegacySidRegistryKeyNotRemoved => ResourceManager.GetString("FormatLegacySidRegistryKeyNotRemoved", resourceCulture);

	internal static string FormatLogSchemaMessage => ResourceManager.GetString("FormatLogSchemaMessage", resourceCulture);

	internal static string FormatLogTimeStatus => ResourceManager.GetString("FormatLogTimeStatus", resourceCulture);

	internal static string FormatMaxMatchStrings => ResourceManager.GetString("FormatMaxMatchStrings", resourceCulture);

	internal static string FormatPercentage => ResourceManager.GetString("FormatPercentage", resourceCulture);

	internal static string FormatProductUpdateRequired => ResourceManager.GetString("FormatProductUpdateRequired", resourceCulture);

	internal static string FormatProgressStatusCombine => ResourceManager.GetString("FormatProgressStatusCombine", resourceCulture);

	internal static string FormatRegistrationInLossOfFunctionalityMode => ResourceManager.GetString("FormatRegistrationInLossOfFunctionalityMode", resourceCulture);

	internal static string FormatRegistrationSetToExpire => ResourceManager.GetString("FormatRegistrationSetToExpire", resourceCulture);

	internal static string FormatRegistrationSetToExpireFixedDate => ResourceManager.GetString("FormatRegistrationSetToExpireFixedDate", resourceCulture);

	internal static string FormatRegistrationSetToExpireFixedDateInternal => ResourceManager.GetString("FormatRegistrationSetToExpireFixedDateInternal", resourceCulture);

	internal static string FormatServerProvidedFileMissing => ResourceManager.GetString("FormatServerProvidedFileMissing", resourceCulture);

	internal static string FormatServerRegistrationDialog => ResourceManager.GetString("FormatServerRegistrationDialog", resourceCulture);

	internal static string FormatServerRegistrationWarning => ResourceManager.GetString("FormatServerRegistrationWarning", resourceCulture);

	internal static string FormatStatusCombine => ResourceManager.GetString("FormatStatusCombine", resourceCulture);

	internal static string FormatTitleOfflineNoLog => ResourceManager.GetString("FormatTitleOfflineNoLog", resourceCulture);

	internal static string FormatTitleOfflineWithLogFile => ResourceManager.GetString("FormatTitleOfflineWithLogFile", resourceCulture);

	internal static string FormatToBeCompatibleWith => ResourceManager.GetString("FormatToBeCompatibleWith", resourceCulture);

	internal static string FormatUserEvent => ResourceManager.GetString("FormatUserEvent", resourceCulture);

	internal static Bitmap forward
	{
		get
		{
			object obj = ResourceManager.GetObject("forward", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap funnel
	{
		get
		{
			object obj = ResourceManager.GetObject("funnel", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap funnel_enabled
	{
		get
		{
			object obj = ResourceManager.GetObject("funnel_enabled", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap gear
	{
		get
		{
			object obj = ResourceManager.GetObject("gear", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap Gray1
	{
		get
		{
			object obj = ResourceManager.GetObject("Gray1", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap Green2
	{
		get
		{
			object obj = ResourceManager.GetObject("Green2", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap headphones
	{
		get
		{
			object obj = ResourceManager.GetObject("headphones", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string HistoryBack => ResourceManager.GetString("HistoryBack", resourceCulture);

	internal static string HistoryForward => ResourceManager.GetString("HistoryForward", resourceCulture);

	internal static string InfoDaysTilConnectionExpiration => ResourceManager.GetString("InfoDaysTilConnectionExpiration", resourceCulture);

	internal static string LabelUserEvent => ResourceManager.GetString("LabelUserEvent", resourceCulture);

	internal static string LastUpdateCheckInformation_KeyName => ResourceManager.GetString("LastUpdateCheckInformation_KeyName", resourceCulture);

	internal static string LastUpdateInformation_Groupname => ResourceManager.GetString("LastUpdateInformation_Groupname", resourceCulture);

	internal static Bitmap log_fast_forward
	{
		get
		{
			object obj = ResourceManager.GetObject("log_fast_forward", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap log_pause
	{
		get
		{
			object obj = ResourceManager.GetObject("log_pause", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap log_play
	{
		get
		{
			object obj = ResourceManager.GetObject("log_play", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap log_rewind
	{
		get
		{
			object obj = ResourceManager.GetObject("log_rewind", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap log_seek_end
	{
		get
		{
			object obj = ResourceManager.GetObject("log_seek_end", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap log_seek_start
	{
		get
		{
			object obj = ResourceManager.GetObject("log_seek_start", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap log_stop
	{
		get
		{
			object obj = ResourceManager.GetObject("log_stop", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string LogFileFolderDialogDescription => ResourceManager.GetString("LogFileFolderDialogDescription", resourceCulture);

	internal static string LogFileInformation_AdvancedChannelOptions => ResourceManager.GetString("LogFileInformation_AdvancedChannelOptions", resourceCulture);

	internal static string LogFileInformation_ConnectionResource => ResourceManager.GetString("LogFileInformation_ConnectionResource", resourceCulture);

	internal static string LogFileInformation_DontAutoExecuteConfiguredServices => ResourceManager.GetString("LogFileInformation_DontAutoExecuteConfiguredServices", resourceCulture);

	internal static string LogFileInformation_DontAutoStartStop => ResourceManager.GetString("LogFileInformation_DontAutoStartStop", resourceCulture);

	internal static string LogFileInformation_DontCyclicallyRead => ResourceManager.GetString("LogFileInformation_DontCyclicallyRead", resourceCulture);

	internal static string LogFileInformation_FixedVariant => ResourceManager.GetString("LogFileInformation_FixedVariant", resourceCulture);

	internal static string LogFileInformation_Group_ActiveFaults => ResourceManager.GetString("LogFileInformation_Group_ActiveFaults", resourceCulture);

	internal static string LogFileInformation_Group_InactiveFaults => ResourceManager.GetString("LogFileInformation_Group_InactiveFaults", resourceCulture);

	internal static string LogFileInformation_Group_LogFileInformation => ResourceManager.GetString("LogFileInformation_Group_LogFileInformation", resourceCulture);

	internal static string LogFileInformation_Group_SystemInformation => ResourceManager.GetString("LogFileInformation_Group_SystemInformation", resourceCulture);

	internal static string LogFileInformation_GroupFormat_Session => ResourceManager.GetString("LogFileInformation_GroupFormat_Session", resourceCulture);

	internal static string LogFileInformation_Item_CBFVersion => ResourceManager.GetString("LogFileInformation_Item_CBFVersion", resourceCulture);

	internal static string LogFileInformation_Item_ConnectionTime => ResourceManager.GetString("LogFileInformation_Item_ConnectionTime", resourceCulture);

	internal static string LogFileInformation_Item_DataMiningProcessTag => ResourceManager.GetString("LogFileInformation_Item_DataMiningProcessTag", resourceCulture);

	internal static string LogFileInformation_Item_DiagnosticVariant => ResourceManager.GetString("LogFileInformation_Item_DiagnosticVariant", resourceCulture);

	internal static string LogFileInformation_Item_Duration => ResourceManager.GetString("LogFileInformation_Item_Duration", resourceCulture);

	internal static string LogFileInformation_Item_EndTime => ResourceManager.GetString("LogFileInformation_Item_EndTime", resourceCulture);

	internal static string LogFileInformation_Item_FileName => ResourceManager.GetString("LogFileInformation_Item_FileName", resourceCulture);

	internal static string LogFileInformation_Item_FileNotFound => ResourceManager.GetString("LogFileInformation_Item_FileNotFound", resourceCulture);

	internal static string LogFileInformation_Item_Location => ResourceManager.GetString("LogFileInformation_Item_Location", resourceCulture);

	internal static string LogFileInformation_Item_SMRVersion => ResourceManager.GetString("LogFileInformation_Item_SMRVersion", resourceCulture);

	internal static string LogFileInformation_Item_StartTime => ResourceManager.GetString("LogFileInformation_Item_StartTime", resourceCulture);

	internal static string LogFileNameUnknown => ResourceManager.GetString("LogFileNameUnknown", resourceCulture);

	internal static Bitmap LogFiles
	{
		get
		{
			object obj = ResourceManager.GetObject("LogFiles", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string Message_Deauthorizing => ResourceManager.GetString("Message_Deauthorizing", resourceCulture);

	internal static string Message_DeepSearch => ResourceManager.GetString("Message_DeepSearch", resourceCulture);

	internal static string Message_MarkedLogForUpload => ResourceManager.GetString("Message_MarkedLogForUpload", resourceCulture);

	internal static string Message_NetworkDebuggerCompleteIssuesFound => ResourceManager.GetString("Message_NetworkDebuggerCompleteIssuesFound", resourceCulture);

	internal static string Message_NetworkDebuggerCompleteNoIssuesFound => ResourceManager.GetString("Message_NetworkDebuggerCompleteNoIssuesFound", resourceCulture);

	internal static string Message_NetworkDebuggerConnectTo => ResourceManager.GetString("Message_NetworkDebuggerConnectTo", resourceCulture);

	internal static string Message_NetworkDebuggerDone => ResourceManager.GetString("Message_NetworkDebuggerDone", resourceCulture);

	internal static string Message_NetworkDebuggerFailure => ResourceManager.GetString("Message_NetworkDebuggerFailure", resourceCulture);

	internal static string Message_NetworkDebuggerInProgress => ResourceManager.GetString("Message_NetworkDebuggerInProgress", resourceCulture);

	internal static string Message_NetworkDebuggerMessageBox => ResourceManager.GetString("Message_NetworkDebuggerMessageBox", resourceCulture);

	internal static string Message_NetworkDebuggerOK => ResourceManager.GetString("Message_NetworkDebuggerOK", resourceCulture);

	internal static string Message_NetworkDebuggerPing => ResourceManager.GetString("Message_NetworkDebuggerPing", resourceCulture);

	internal static string Message_NetworkDebuggerSettingNotDefault => ResourceManager.GetString("Message_NetworkDebuggerSettingNotDefault", resourceCulture);

	internal static string Message_NetworkDebuggerVerifySetting => ResourceManager.GetString("Message_NetworkDebuggerVerifySetting", resourceCulture);

	internal static string Message_NoProtocol => ResourceManager.GetString("Message_NoProtocol", resourceCulture);

	internal static string Message_RollCallDisabled => ResourceManager.GetString("Message_RollCallDisabled", resourceCulture);

	internal static string Message_ServerUploadNeeded => ResourceManager.GetString("Message_ServerUploadNeeded", resourceCulture);

	internal static string Message_SIDConfigureUnavailable => ResourceManager.GetString("Message_SIDConfigureUnavailable", resourceCulture);

	internal static string MessageActiveFaultStatusNone => ResourceManager.GetString("MessageActiveFaultStatusNone", resourceCulture);

	internal static string MessageAddingLicenseKeyMayRequireDrumrollReinitialize => ResourceManager.GetString("MessageAddingLicenseKeyMayRequireDrumrollReinitialize", resourceCulture);

	internal static string MessageCanNotOpenActiveLog => ResourceManager.GetString("MessageCanNotOpenActiveLog", resourceCulture);

	internal static string MessageClickHereForMoreInformation => ResourceManager.GetString("MessageClickHereForMoreInformation", resourceCulture);

	internal static string MessageConnectionsAndLogFilesClosedBeforeChangesApplied => ResourceManager.GetString("MessageConnectionsAndLogFilesClosedBeforeChangesApplied", resourceCulture);

	internal static string MessageConnectionStatusNone => ResourceManager.GetString("MessageConnectionStatusNone", resourceCulture);

	internal static string MessageDeviceBeingFlashedCannotBeTerminatedPleaseWait => ResourceManager.GetString("MessageDeviceBeingFlashedCannotBeTerminatedPleaseWait", resourceCulture);

	internal static string MessageDialog_CautionDialog_Bullet0Line0 => ResourceManager.GetString("MessageDialog_CautionDialog_Bullet0Line0", resourceCulture);

	internal static string MessageDialog_CautionDialog_Bullet0Line1 => ResourceManager.GetString("MessageDialog_CautionDialog_Bullet0Line1", resourceCulture);

	internal static string MessageDialog_CautionDialog_Bullet1Line0 => ResourceManager.GetString("MessageDialog_CautionDialog_Bullet1Line0", resourceCulture);

	internal static string MessageDialog_CautionDialog_Bullet2Line0 => ResourceManager.GetString("MessageDialog_CautionDialog_Bullet2Line0", resourceCulture);

	internal static string MessageDialog_CautionDialog_Footer => ResourceManager.GetString("MessageDialog_CautionDialog_Footer", resourceCulture);

	internal static string MessageDialog_CautionDialog_Header => ResourceManager.GetString("MessageDialog_CautionDialog_Header", resourceCulture);

	internal static string MessageDialog_CautionDialog_Title => ResourceManager.GetString("MessageDialog_CautionDialog_Title", resourceCulture);

	internal static string MessageFeedBackReportSubmitInformation => ResourceManager.GetString("MessageFeedBackReportSubmitInformation", resourceCulture);

	internal static string MessageFormat_BackTo0 => ResourceManager.GetString("MessageFormat_BackTo0", resourceCulture);

	internal static string MessageFormat_DeepSearchProgress => ResourceManager.GetString("MessageFormat_DeepSearchProgress", resourceCulture);

	internal static string MessageFormat_FowardTo0 => ResourceManager.GetString("MessageFormat_FowardTo0", resourceCulture);

	internal static string MessageFormat_MismatchRunningADEVersion => ResourceManager.GetString("MessageFormat_MismatchRunningADEVersion", resourceCulture);

	internal static string MessageFormat_NativeISONotRecommended => ResourceManager.GetString("MessageFormat_NativeISONotRecommended", resourceCulture);

	internal static string MessageFormat_NoConnectionHardwareWarning => ResourceManager.GetString("MessageFormat_NoConnectionHardwareWarning", resourceCulture);

	internal static string MessageFormat_PathToLong => ResourceManager.GetString("MessageFormat_PathToLong", resourceCulture);

	internal static string MessageFormat_ProductUpdateRequired => ResourceManager.GetString("MessageFormat_ProductUpdateRequired", resourceCulture);

	internal static string MessageFormat_ProductUpdateRequiredContent => ResourceManager.GetString("MessageFormat_ProductUpdateRequiredContent", resourceCulture);

	internal static string MessageFormat_ProgrammingEventsLeft => ResourceManager.GetString("MessageFormat_ProgrammingEventsLeft", resourceCulture);

	internal static string MessageFormat_RequiresInternetExplorer11OrLater => ResourceManager.GetString("MessageFormat_RequiresInternetExplorer11OrLater", resourceCulture);

	internal static string MessageFormat_RequiresNETFramework4OrLaterPleaseUpgradeYourInstallationOfNETFrameworkAndThenRestart0 => ResourceManager.GetString("MessageFormat_RequiresNETFramework4OrLaterPleaseUpgradeYourInstallationOfNETFrameworkAndThenRestart0", resourceCulture);

	internal static string MessageFormat_RequiresUpgradetoLatestRelease => ResourceManager.GetString("MessageFormat_RequiresUpgradetoLatestRelease", resourceCulture);

	internal static string MessageFormat_ResourceCannotBeDetermined => ResourceManager.GetString("MessageFormat_ResourceCannotBeDetermined", resourceCulture);

	internal static string MessageFormat_ResourceCannotBeDeterminedViaEcuPresent => ResourceManager.GetString("MessageFormat_ResourceCannotBeDeterminedViaEcuPresent", resourceCulture);

	internal static string MessageFormat_SidNotFound => ResourceManager.GetString("MessageFormat_SidNotFound", resourceCulture);

	internal static string MessageFormat_SidRollback => ResourceManager.GetString("MessageFormat_SidRollback", resourceCulture);

	internal static string MessageFormat_UploadSpecifiedFile => ResourceManager.GetString("MessageFormat_UploadSpecifiedFile", resourceCulture);

	internal static string MessageFormat_WarningProcessor => ResourceManager.GetString("MessageFormat_WarningProcessor", resourceCulture);

	internal static string MessageFormat_WarningSystemModel => ResourceManager.GetString("MessageFormat_WarningSystemModel", resourceCulture);

	internal static string MessageIncompatibleDepotHardwareNoReprogrammingAccess => ResourceManager.GetString("MessageIncompatibleDepotHardwareNoReprogrammingAccess", resourceCulture);

	internal static string MessageIncompatibleDepotHardwareReprogrammingAccess => ResourceManager.GetString("MessageIncompatibleDepotHardwareReprogrammingAccess", resourceCulture);

	internal static string MessageIncompatibleDepotSoftwareNoReprogrammingAccess => ResourceManager.GetString("MessageIncompatibleDepotSoftwareNoReprogrammingAccess", resourceCulture);

	internal static string MessageIncompatibleDepotSoftwareReprogrammingAccess => ResourceManager.GetString("MessageIncompatibleDepotSoftwareReprogrammingAccess", resourceCulture);

	internal static string MessageIncompatibleEdexSoftwareNoReprogrammingAccess => ResourceManager.GetString("MessageIncompatibleEdexSoftwareNoReprogrammingAccess", resourceCulture);

	internal static string MessageIncompatibleEdexSoftwareReprogrammingAccess => ResourceManager.GetString("MessageIncompatibleEdexSoftwareReprogrammingAccess", resourceCulture);

	internal static string MessageIncompatibleSoftwareNoReprogrammingAccess => ResourceManager.GetString("MessageIncompatibleSoftwareNoReprogrammingAccess", resourceCulture);

	internal static string MessageIncompatibleSoftwareReprogrammingAccess => ResourceManager.GetString("MessageIncompatibleSoftwareReprogrammingAccess", resourceCulture);

	internal static string MessageInvalidPort => ResourceManager.GetString("MessageInvalidPort", resourceCulture);

	internal static string MessageLicenseKeyInvalid => ResourceManager.GetString("MessageLicenseKeyInvalid", resourceCulture);

	internal static string MessageMoving => ResourceManager.GetString("MessageMoving", resourceCulture);

	internal static string MessageNoLogFilesFound => ResourceManager.GetString("MessageNoLogFilesFound", resourceCulture);

	internal static string MessageNoMatchesFound => ResourceManager.GetString("MessageNoMatchesFound", resourceCulture);

	internal static string MessageOrphanedDrumrollInstanceExist => ResourceManager.GetString("MessageOrphanedDrumrollInstanceExist", resourceCulture);

	internal static string MessagePanelProblemDuringStartup => ResourceManager.GetString("MessagePanelProblemDuringStartup", resourceCulture);

	internal static string MessageRemovingLicenseKeyMayRequireDrumrollReinitialize => ResourceManager.GetString("MessageRemovingLicenseKeyMayRequireDrumrollReinitialize", resourceCulture);

	internal static string MessageSearching => ResourceManager.GetString("MessageSearching", resourceCulture);

	internal static string Messsage_NoConnectionResources => ResourceManager.GetString("Messsage_NoConnectionResources", resourceCulture);

	internal static string MetadataTypeFaultCode => ResourceManager.GetString("MetadataTypeFaultCode", resourceCulture);

	internal static string MetadataTypeIdentification => ResourceManager.GetString("MetadataTypeIdentification", resourceCulture);

	internal static string MetadataTypeLabel => ResourceManager.GetString("MetadataTypeLabel", resourceCulture);

	internal static Bitmap nav_plain_red
	{
		get
		{
			object obj = ResourceManager.GetObject("nav_plain_red", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap navigate_down
	{
		get
		{
			object obj = ResourceManager.GetObject("navigate_down", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap navigate_up
	{
		get
		{
			object obj = ResourceManager.GetObject("navigate_up", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string NoProgrammingEventsLeft => ResourceManager.GetString("NoProgrammingEventsLeft", resourceCulture);

	internal static Bitmap open_log
	{
		get
		{
			object obj = ResourceManager.GetObject("open_log", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap option_warnings
	{
		get
		{
			object obj = ResourceManager.GetObject("option_warnings", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap paste
	{
		get
		{
			object obj = ResourceManager.GetObject("paste", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap print
	{
		get
		{
			object obj = ResourceManager.GetObject("print", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string PrintFaultsDialog_Fault => ResourceManager.GetString("PrintFaultsDialog_Fault", resourceCulture);

	internal static string PrintFaultsDialog_None => ResourceManager.GetString("PrintFaultsDialog_None", resourceCulture);

	internal static string PrintFaultsDialog_Status => ResourceManager.GetString("PrintFaultsDialog_Status", resourceCulture);

	internal static string Program_FormatInconsistentInstallation => ResourceManager.GetString("Program_FormatInconsistentInstallation", resourceCulture);

	internal static Bitmap Red2
	{
		get
		{
			object obj = ResourceManager.GetObject("Red2", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap refresh
	{
		get
		{
			object obj = ResourceManager.GetObject("refresh", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string RemainingProgrammingEvents => ResourceManager.GetString("RemainingProgrammingEvents", resourceCulture);

	internal static Bitmap retry_autoconnect
	{
		get
		{
			object obj = ResourceManager.GetObject("retry_autoconnect", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap row
	{
		get
		{
			object obj = ResourceManager.GetObject("row", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string SearchTypePrefix_Deep => ResourceManager.GetString("SearchTypePrefix_Deep", resourceCulture);

	internal static string SearchTypePrefix_Regex => ResourceManager.GetString("SearchTypePrefix_Regex", resourceCulture);

	internal static Bitmap seek_time
	{
		get
		{
			object obj = ResourceManager.GetObject("seek_time", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string ServerConnectionHasExpired => ResourceManager.GetString("ServerConnectionHasExpired", resourceCulture);

	internal static string ServerOptionsPanel_Deauthorize => ResourceManager.GetString("ServerOptionsPanel_Deauthorize", resourceCulture);

	internal static string ServerRegistrationHasExpired => ResourceManager.GetString("ServerRegistrationHasExpired", resourceCulture);

	internal static string StartupStatus_Format_LoadingView => ResourceManager.GetString("StartupStatus_Format_LoadingView", resourceCulture);

	internal static string StatusPreparingCommunications => ResourceManager.GetString("StatusPreparingCommunications", resourceCulture);

	internal static string StatusPreparingDiagnosticData => ResourceManager.GetString("StatusPreparingDiagnosticData", resourceCulture);

	internal static Bitmap stop
	{
		get
		{
			object obj = ResourceManager.GetObject("stop", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap SummaryFiles
	{
		get
		{
			object obj = ResourceManager.GetObject("SummaryFiles", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap summaryfiles_noupload
	{
		get
		{
			object obj = ResourceManager.GetObject("summaryfiles_noupload", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string TabbedView_FormatESN => ResourceManager.GetString("TabbedView_FormatESN", resourceCulture);

	internal static string TabbedView_FormatVIN => ResourceManager.GetString("TabbedView_FormatVIN", resourceCulture);

	internal static string TabbedView_ToolTip_ESN => ResourceManager.GetString("TabbedView_ToolTip_ESN", resourceCulture);

	internal static string TabbedView_ToolTip_VIN => ResourceManager.GetString("TabbedView_ToolTip_VIN", resourceCulture);

	internal static string Tier4PinError_UserText => ResourceManager.GetString("Tier4PinError_UserText", resourceCulture);

	internal static string ToolLicenseExpirationDate => ResourceManager.GetString("ToolLicenseExpirationDate", resourceCulture);

	internal static string ToolLicenseInfo_Groupname => ResourceManager.GetString("ToolLicenseInfo_Groupname", resourceCulture);

	internal static string Tooltip_FilterCurrentViewDoesNotSupportFiltering => ResourceManager.GetString("Tooltip_FilterCurrentViewDoesNotSupportFiltering", resourceCulture);

	internal static string Tooltip_FilterShowingAllContent => ResourceManager.GetString("Tooltip_FilterShowingAllContent", resourceCulture);

	internal static string Tooltip_FilterShowingOnlyEssentialContent => ResourceManager.GetString("Tooltip_FilterShowingOnlyEssentialContent", resourceCulture);

	internal static string Tooltip_Format_FilterCountDescription => ResourceManager.GetString("Tooltip_Format_FilterCountDescription", resourceCulture);

	internal static string Tooltip_Format_ShowingSelectedContent => ResourceManager.GetString("Tooltip_Format_ShowingSelectedContent", resourceCulture);

	internal static string Tooltip_UnitsSystem_English => ResourceManager.GetString("Tooltip_UnitsSystem_English", resourceCulture);

	internal static string TooltipAutoConnectIsDisabledForDevice => ResourceManager.GetString("TooltipAutoConnectIsDisabledForDevice", resourceCulture);

	internal static string TooltipAutoConnectIsEnabledForDevice => ResourceManager.GetString("TooltipAutoConnectIsEnabledForDevice", resourceCulture);

	internal static string TooltipAutoConnectSuspendedForDevice => ResourceManager.GetString("TooltipAutoConnectSuspendedForDevice", resourceCulture);

	internal static string TooltipAutoConnectWillBeDisabledForDevice => ResourceManager.GetString("TooltipAutoConnectWillBeDisabledForDevice", resourceCulture);

	internal static string TooltipAutoConnectWillBeEnabledForDevice => ResourceManager.GetString("TooltipAutoConnectWillBeEnabledForDevice", resourceCulture);

	internal static string TooltipAutoConnectWillBeResumedForDevice => ResourceManager.GetString("TooltipAutoConnectWillBeResumedForDevice", resourceCulture);

	internal static string TooltipPause => ResourceManager.GetString("TooltipPause", resourceCulture);

	internal static string TooltipPlay => ResourceManager.GetString("TooltipPlay", resourceCulture);

	internal static string Tootip_UnitsSystem_Metric => ResourceManager.GetString("Tootip_UnitsSystem_Metric", resourceCulture);

	internal static string TroubleshootingGuides => ResourceManager.GetString("TroubleshootingGuides", resourceCulture);

	internal static Bitmap undo
	{
		get
		{
			object obj = ResourceManager.GetObject("undo", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap units_system
	{
		get
		{
			object obj = ResourceManager.GetObject("units_system", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string Unlimited => ResourceManager.GetString("Unlimited", resourceCulture);

	internal static Bitmap upload_log
	{
		get
		{
			object obj = ResourceManager.GetObject("upload_log", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string UseSystemLanguage => ResourceManager.GetString("UseSystemLanguage", resourceCulture);

	internal static Bitmap ViewAll
	{
		get
		{
			object obj = ResourceManager.GetObject("ViewAll", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap warning
	{
		get
		{
			object obj = ResourceManager.GetObject("warning", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static Bitmap Yellow2
	{
		get
		{
			object obj = ResourceManager.GetObject("Yellow2", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal Resources()
	{
	}
}
