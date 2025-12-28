using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DetroitDiesel.DataHub.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resource
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
				resourceMan = new ResourceManager("DetroitDiesel.DataHub.Properties.Resource", typeof(Resource).Assembly);
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

	internal static string DataPageRequest_ChangePassword => ResourceManager.GetString("DataPageRequest_ChangePassword", resourceCulture);

	internal static string DataPageRequest_Clear => ResourceManager.GetString("DataPageRequest_Clear", resourceCulture);

	internal static string DataPageRequest_Extraction => ResourceManager.GetString("DataPageRequest_Extraction", resourceCulture);

	internal static string DataPageRequest_Password => ResourceManager.GetString("DataPageRequest_Password", resourceCulture);

	internal static string DataPageType_Alert => ResourceManager.GetString("DataPageType_Alert", resourceCulture);

	internal static string DataPageType_AuditTrail => ResourceManager.GetString("DataPageType_AuditTrail", resourceCulture);

	internal static string DataPageType_BundledTrip => ResourceManager.GetString("DataPageType_BundledTrip", resourceCulture);

	internal static string DataPageType_Configuration => ResourceManager.GetString("DataPageType_Configuration", resourceCulture);

	internal static string DataPageType_DetailedAlert => ResourceManager.GetString("DataPageType_DetailedAlert", resourceCulture);

	internal static string DataPageType_DisplayScreen => ResourceManager.GetString("DataPageType_DisplayScreen", resourceCulture);

	internal static string DataPageType_DriverTables => ResourceManager.GetString("DataPageType_DriverTables", resourceCulture);

	internal static string DataPageType_DriverTripMonthly => ResourceManager.GetString("DataPageType_DriverTripMonthly", resourceCulture);

	internal static string DataPageType_DriverTripTablesMonthly => ResourceManager.GetString("DataPageType_DriverTripTablesMonthly", resourceCulture);

	internal static string DataPageType_EngineUsage => ResourceManager.GetString("DataPageType_EngineUsage", resourceCulture);

	internal static string DataPageType_EngineUsageHeader => ResourceManager.GetString("DataPageType_EngineUsageHeader", resourceCulture);

	internal static string DataPageType_Header => ResourceManager.GetString("DataPageType_Header", resourceCulture);

	internal static string DataPageType_IncidentAutomaticHeader => ResourceManager.GetString("DataPageType_IncidentAutomaticHeader", resourceCulture);

	internal static string DataPageType_IncidentDataAutomatic => ResourceManager.GetString("DataPageType_IncidentDataAutomatic", resourceCulture);

	internal static string DataPageType_IncidentDataDriver => ResourceManager.GetString("DataPageType_IncidentDataDriver", resourceCulture);

	internal static string DataPageType_IncidentHardBreak => ResourceManager.GetString("DataPageType_IncidentHardBreak", resourceCulture);

	internal static string DataPageType_IncidentHardBreakHeader => ResourceManager.GetString("DataPageType_IncidentHardBreakHeader", resourceCulture);

	internal static string DataPageType_Permanent => ResourceManager.GetString("DataPageType_Permanent", resourceCulture);

	internal static string DataPageType_Profile => ResourceManager.GetString("DataPageType_Profile", resourceCulture);

	internal static string DataPageType_RequestTypes => ResourceManager.GetString("DataPageType_RequestTypes", resourceCulture);

	internal static string DataPageType_SelfDiagnostic => ResourceManager.GetString("DataPageType_SelfDiagnostic", resourceCulture);

	internal static string DataPageType_ServiceInterval => ResourceManager.GetString("DataPageType_ServiceInterval", resourceCulture);

	internal static string DataPageType_SnapshotBuffer => ResourceManager.GetString("DataPageType_SnapshotBuffer", resourceCulture);

	internal static string DataPageType_StateActivity => ResourceManager.GetString("DataPageType_StateActivity", resourceCulture);

	internal static string DataPageType_StateLineCrossing => ResourceManager.GetString("DataPageType_StateLineCrossing", resourceCulture);

	internal static string DataPageType_Supported => ResourceManager.GetString("DataPageType_Supported", resourceCulture);

	internal static string DataPageType_SupportRequestPage0 => ResourceManager.GetString("DataPageType_SupportRequestPage0", resourceCulture);

	internal static string DataPageType_TotalSupportedPages => ResourceManager.GetString("DataPageType_TotalSupportedPages", resourceCulture);

	internal static string DataPageType_TrendAnalysis => ResourceManager.GetString("DataPageType_TrendAnalysis", resourceCulture);

	internal static string DataPageType_Trip => ResourceManager.GetString("DataPageType_Trip", resourceCulture);

	internal static string DataPageType_TripMonthly => ResourceManager.GetString("DataPageType_TripMonthly", resourceCulture);

	internal static string DataPageType_TripTables => ResourceManager.GetString("DataPageType_TripTables", resourceCulture);

	internal static string DataPageType_TripTablesMonthly => ResourceManager.GetString("DataPageType_TripTablesMonthly", resourceCulture);

	internal static string DataPageType_VirtualPage => ResourceManager.GetString("DataPageType_VirtualPage", resourceCulture);

	internal static string ChangeDataPagesPassword_PasswordMustContains => ResourceManager.GetString("ChangeDataPagesPassword_PasswordMustContains", resourceCulture);

	internal static string ChangeDataPagesPassword_PasswordNotSame => ResourceManager.GetString("ChangeDataPagesPassword_PasswordNotSame", resourceCulture);

	internal Resource()
	{
	}
}
