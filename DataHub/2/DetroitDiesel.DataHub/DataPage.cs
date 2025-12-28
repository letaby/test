using System;
using System.Globalization;
using System.IO;
using System.Linq;
using DetroitDiesel.Common.Status;
using DetroitDiesel.DataHub.Properties;

namespace DetroitDiesel.DataHub;

public class DataPage
{
	private readonly byte[] rawData;

	public bool Valid { get; private set; }

	public DataPageType PageType { get; private set; }

	public int DataLength => rawData.Length;

	internal byte GetByteValue(int offset)
	{
		if (Valid && offset < rawData.Length)
		{
			return rawData[offset];
		}
		return 0;
	}

	public DataPage(byte[] data)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		if (data.Length >= 5 && data[0] == 98 && data[1] == 62)
		{
			PageType = (DataPageType)data[2];
			data[1] = data[2];
			data[2] = 0;
			int num = ((PageType == DataPageType.SupportRequestPage0) ? (data.Length - 1) : ((data[4] << 8) + data[3] + 4));
			if (num > 0)
			{
				rawData = new byte[num];
				Array.Copy(data.Skip(1).ToArray(), rawData, num);
				Valid = true;
			}
			else
			{
				StatusLog.Add(new StatusMessage(string.Format(CultureInfo.InvariantCulture, "DataPage Response invalid, could not create page."), (StatusMessageType)2, (object)this), true);
			}
		}
	}

	public void Encode(BinaryWriter writer)
	{
		writer.Write(rawData);
	}

	public static string PageTypeDescription(DataPageType type)
	{
		return type switch
		{
			DataPageType.Alert => Resource.DataPageType_Alert, 
			DataPageType.AuditTrail => Resource.DataPageType_AuditTrail, 
			DataPageType.BundledTrip => Resource.DataPageType_BundledTrip, 
			DataPageType.Configuration => Resource.DataPageType_Configuration, 
			DataPageType.DetailedAlert => Resource.DataPageType_DetailedAlert, 
			DataPageType.DisplayScreen => Resource.DataPageType_DisplayScreen, 
			DataPageType.DriverTables => Resource.DataPageType_DriverTables, 
			DataPageType.DriverTripMonthly => Resource.DataPageType_DriverTripMonthly, 
			DataPageType.DriverTripTablesMonthly => Resource.DataPageType_DriverTripTablesMonthly, 
			DataPageType.EngineUsage => Resource.DataPageType_EngineUsage, 
			DataPageType.EngineUsageHeader => Resource.DataPageType_EngineUsageHeader, 
			DataPageType.Header => Resource.DataPageType_Header, 
			DataPageType.IncidentAutomaticHeader => Resource.DataPageType_IncidentAutomaticHeader, 
			DataPageType.IncidentDataAutomatic => Resource.DataPageType_IncidentDataAutomatic, 
			DataPageType.IncidentDataDriver => Resource.DataPageType_IncidentDataDriver, 
			DataPageType.IncidentDataHardBreak => Resource.DataPageType_IncidentHardBreak, 
			DataPageType.IncidentHardBrakeHeader => Resource.DataPageType_IncidentHardBreakHeader, 
			DataPageType.Permanent => Resource.DataPageType_Permanent, 
			DataPageType.Profile => Resource.DataPageType_Profile, 
			DataPageType.RequestTypes => Resource.DataPageType_RequestTypes, 
			DataPageType.SelfDiagnostic => Resource.DataPageType_SelfDiagnostic, 
			DataPageType.ServiceInterval => Resource.DataPageType_ServiceInterval, 
			DataPageType.SnapshotBuffer => Resource.DataPageType_SnapshotBuffer, 
			DataPageType.StateActivity => Resource.DataPageType_StateActivity, 
			DataPageType.StateLineCrossing => Resource.DataPageType_StateLineCrossing, 
			DataPageType.SupportRequestPage0 => Resource.DataPageType_SupportRequestPage0, 
			DataPageType.Supported => Resource.DataPageType_Supported, 
			DataPageType.TotalSupportedPages => Resource.DataPageType_TotalSupportedPages, 
			DataPageType.TrendAnalysis => Resource.DataPageType_TrendAnalysis, 
			DataPageType.Trip => Resource.DataPageType_Trip, 
			DataPageType.TripDataMonthly => Resource.DataPageType_TripMonthly, 
			DataPageType.TripTables => Resource.DataPageType_TripTables, 
			DataPageType.TripTablesMonthly => Resource.DataPageType_TripTablesMonthly, 
			DataPageType.VirtualPage => Resource.DataPageType_VirtualPage, 
			_ => string.Empty, 
		};
	}
}
