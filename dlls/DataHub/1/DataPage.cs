// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.DataHub.DataPage
// Assembly: DataHub, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 89346980-C6E7-48B1-88DD-CF29796E810E
// Assembly location: C:\Users\petra\Downloads\Архив (2)\DataHub.dll

using DetroitDiesel.Common.Status;
using DetroitDiesel.DataHub.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

#nullable disable
namespace DetroitDiesel.DataHub;

public class DataPage
{
  private readonly byte[] rawData;

  public bool Valid { get; private set; }

  public DataPageType PageType { get; private set; }

  public int DataLength => this.rawData.Length;

  internal byte GetByteValue(int offset)
  {
    return this.Valid && offset < this.rawData.Length ? this.rawData[offset] : (byte) 0;
  }

  public DataPage(byte[] data)
  {
    if (data.Length < 5 || data[0] != (byte) 98 || data[1] != (byte) 62)
      return;
    this.PageType = (DataPageType) data[2];
    data[1] = data[2];
    data[2] = (byte) 0;
    int length = this.PageType == DataPageType.SupportRequestPage0 ? data.Length - 1 : ((int) data[4] << 8) + (int) data[3] + 4;
    if (length > 0)
    {
      this.rawData = new byte[length];
      Array.Copy((Array) ((IEnumerable<byte>) data).Skip<byte>(1).ToArray<byte>(), (Array) this.rawData, length);
      this.Valid = true;
    }
    else
      StatusLog.Add(new StatusMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DataPage Response invalid, could not create page."), (StatusMessageType) 2, (object) this), true);
  }

  public void Encode(BinaryWriter writer) => writer.Write(this.rawData);

  public static string PageTypeDescription(DataPageType type)
  {
    switch (type)
    {
      case DataPageType.SupportRequestPage0:
        return Resource.DataPageType_SupportRequestPage0;
      case DataPageType.Trip:
        return Resource.DataPageType_Trip;
      case DataPageType.IncidentDataHardBreak:
        return Resource.DataPageType_IncidentHardBreak;
      case DataPageType.IncidentDataDriver:
        return Resource.DataPageType_IncidentDataDriver;
      case DataPageType.IncidentDataAutomatic:
        return Resource.DataPageType_IncidentDataAutomatic;
      case DataPageType.Profile:
        return Resource.DataPageType_Profile;
      case DataPageType.Configuration:
        return Resource.DataPageType_Configuration;
      case DataPageType.ServiceInterval:
        return Resource.DataPageType_ServiceInterval;
      case DataPageType.DisplayScreen:
        return Resource.DataPageType_DisplayScreen;
      case DataPageType.Alert:
        return Resource.DataPageType_Alert;
      case DataPageType.Header:
        return Resource.DataPageType_Header;
      case DataPageType.BundledTrip:
        return Resource.DataPageType_BundledTrip;
      case DataPageType.TripTables:
        return Resource.DataPageType_TripTables;
      case DataPageType.TrendAnalysis:
        return Resource.DataPageType_TrendAnalysis;
      case DataPageType.TripDataMonthly:
        return Resource.DataPageType_TripMonthly;
      case DataPageType.DetailedAlert:
        return Resource.DataPageType_DetailedAlert;
      case DataPageType.EngineUsage:
        return Resource.DataPageType_EngineUsage;
      case DataPageType.AuditTrail:
        return Resource.DataPageType_AuditTrail;
      case DataPageType.SelfDiagnostic:
        return Resource.DataPageType_SelfDiagnostic;
      case DataPageType.SnapshotBuffer:
        return Resource.DataPageType_SnapshotBuffer;
      case DataPageType.Permanent:
        return Resource.DataPageType_Permanent;
      case DataPageType.DriverTables:
        return Resource.DataPageType_DriverTables;
      case DataPageType.DriverTripMonthly:
        return Resource.DataPageType_DriverTripMonthly;
      case DataPageType.DriverTripTablesMonthly:
        return Resource.DataPageType_DriverTripTablesMonthly;
      case DataPageType.TripTablesMonthly:
        return Resource.DataPageType_TripTablesMonthly;
      case DataPageType.StateActivity:
        return Resource.DataPageType_StateActivity;
      case DataPageType.StateLineCrossing:
        return Resource.DataPageType_StateLineCrossing;
      case DataPageType.TotalSupportedPages:
        return Resource.DataPageType_TotalSupportedPages;
      case DataPageType.EngineUsageHeader:
        return Resource.DataPageType_EngineUsageHeader;
      case DataPageType.IncidentAutomaticHeader:
        return Resource.DataPageType_IncidentAutomaticHeader;
      case DataPageType.IncidentHardBrakeHeader:
        return Resource.DataPageType_IncidentHardBreakHeader;
      case DataPageType.VirtualPage:
        return Resource.DataPageType_VirtualPage;
      case DataPageType.RequestTypes:
        return Resource.DataPageType_RequestTypes;
      case DataPageType.Supported:
        return Resource.DataPageType_Supported;
      default:
        return string.Empty;
    }
  }
}
