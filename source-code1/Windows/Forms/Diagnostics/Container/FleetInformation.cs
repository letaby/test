// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.FleetInformation
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using Microsoft.Win32;
using System.Collections.Generic;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public sealed class FleetInformation
{
  private const string FleetRegistryKeyLocation = "SOFTWARE\\Detroit Diesel\\Shared\\Fleet Information";
  private const string CompanyNameValueName = "Name";
  private const string AddressValueName = "Address";
  private const string CityValueName = "City";
  private const string ZipCodeValueName = "Zip";
  private const string StateValueName = "State";
  private const string TelephoneNumberValueName = "Phone";
  private const string OffsetValueName = "FTZ";
  public const int CompanyNameMaxLength = 30;
  public const int AddressMaxLength = 30;
  public const int CityMaxLength = 30;
  public const int StateMaxLength = 2;
  public const int ZipCodeMaxLength = 10;
  public const int TelephoneNumberMaxLength = 20;
  public const int DefaultOffset = -18000;
  private static Dictionary<int, FleetTimeZone> timeZones;
  private string companyName = string.Empty;
  private string address = string.Empty;
  private string state = string.Empty;
  private string city = string.Empty;
  private string zipCode = string.Empty;
  private string telephoneNumber = string.Empty;
  private int offset = -18000;

  private static void CreateTimeZoneList()
  {
    if (FleetInformation.timeZones != null)
      return;
    FleetInformation.timeZones = new Dictionary<int, FleetTimeZone>(25);
    FleetInformation.timeZones.Add(-43200, new FleetTimeZone("(GMT-12.00) Dateline", -12));
    FleetInformation.timeZones.Add(-39600, new FleetTimeZone("(GMT-11.00) Samoa", -11));
    FleetInformation.timeZones.Add(-36000, new FleetTimeZone("(GMT-10.00) Hawaii", -10));
    FleetInformation.timeZones.Add(-32400, new FleetTimeZone("(GMT-09.00) Alaska", -9));
    FleetInformation.timeZones.Add(-28800, new FleetTimeZone("(GMT-08.00) Pacific Time", -8));
    FleetInformation.timeZones.Add(-25200, new FleetTimeZone("(GMT-07.00) Mountain", -7));
    FleetInformation.timeZones.Add(-21600, new FleetTimeZone("(GMT-06.00) Central", -6));
    FleetInformation.timeZones.Add(-18000, new FleetTimeZone("(GMT-05.00) Eastern Standard Time", -5));
    FleetInformation.timeZones.Add(-14400, new FleetTimeZone("(GMT-04.00) Atlantic", -4));
    FleetInformation.timeZones.Add(-10800, new FleetTimeZone("(GMT-03.00) Eastern South America", -3));
    FleetInformation.timeZones.Add(-7200, new FleetTimeZone("(GMT-02.00) Mid Atlantic", -2));
    FleetInformation.timeZones.Add(-3600, new FleetTimeZone("(GMT-01.00) Azores", -1));
    FleetInformation.timeZones.Add(0, new FleetTimeZone("GMT", 0));
    FleetInformation.timeZones.Add(3600, new FleetTimeZone("(GMT+01.00) Western Europe", 1));
    FleetInformation.timeZones.Add(7200, new FleetTimeZone("(GMT+02.00) Eastern Europe", 2));
    FleetInformation.timeZones.Add(10800, new FleetTimeZone("(GMT+03.00) Russian", 3));
    FleetInformation.timeZones.Add(14400, new FleetTimeZone("(GMT+04.00) Arabian", 4));
    FleetInformation.timeZones.Add(18000, new FleetTimeZone("(GMT+05.00) West Asia", 5));
    FleetInformation.timeZones.Add(21600, new FleetTimeZone("(GMT+06.00) Central Asia", 6));
    FleetInformation.timeZones.Add(25200, new FleetTimeZone("(GMT+07.00) Bangkok", 7));
    FleetInformation.timeZones.Add(28800, new FleetTimeZone("(GMT+08.00) China, Perth", 8));
    FleetInformation.timeZones.Add(32400, new FleetTimeZone("(GMT+09.00) Tokyo, Adelaide", 9));
    FleetInformation.timeZones.Add(36000, new FleetTimeZone("(GMT+10.00) Sydney", 10));
    FleetInformation.timeZones.Add(39600, new FleetTimeZone("(GMT+11.00) Central Pacific", 11));
    FleetInformation.timeZones.Add(43200, new FleetTimeZone("(GMT+12.00) New Zealand", 12));
  }

  public static ICollection<FleetTimeZone> TimeZones
  {
    get
    {
      FleetInformation.CreateTimeZoneList();
      return (ICollection<FleetTimeZone>) FleetInformation.timeZones.Values;
    }
  }

  private static FleetTimeZone GetFleetTimeZoneFromOffset(int offsetInSeconds)
  {
    FleetInformation.CreateTimeZoneList();
    FleetTimeZone timeZoneFromOffset = (FleetTimeZone) null;
    FleetInformation.timeZones.TryGetValue(offsetInSeconds, out timeZoneFromOffset);
    return timeZoneFromOffset;
  }

  private FleetInformation()
  {
  }

  public string CompanyName
  {
    get => this.companyName;
    set => this.companyName = FleetInformation.NormalizeString(value, 30);
  }

  public string Address
  {
    get => this.address;
    set => this.address = FleetInformation.NormalizeString(value, 30);
  }

  public string City
  {
    get => this.city;
    set => this.city = FleetInformation.NormalizeString(value, 30);
  }

  public string State
  {
    get => this.state;
    set => this.state = FleetInformation.NormalizeString(value, 2);
  }

  public string ZipCode
  {
    get => this.zipCode;
    set => this.zipCode = FleetInformation.NormalizeString(value, 10);
  }

  public string TelephoneNumber
  {
    get => this.telephoneNumber;
    set => this.telephoneNumber = FleetInformation.NormalizeString(value, 20);
  }

  private static string NormalizeString(string value, int maxLength)
  {
    string str = string.Empty;
    if (!string.IsNullOrEmpty(value))
      str = value.Length > maxLength ? value.Substring(0, maxLength) : value;
    return str;
  }

  public FleetTimeZone TimeZone
  {
    get => FleetInformation.GetFleetTimeZoneFromOffset(this.offset);
    set => this.offset = (int) value.Offset.TotalSeconds;
  }

  public static FleetTimeZone DefaultTimeZone
  {
    get => FleetInformation.GetFleetTimeZoneFromOffset(-18000);
  }

  public static FleetInformation Load()
  {
    FleetInformation fleetInformation = new FleetInformation();
    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Detroit Diesel\\Shared\\Fleet Information", false);
    if (registryKey == null || registryKey.GetValue("Name") == null)
    {
      registryKey?.Close();
      registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Detroit Diesel\\Shared\\Fleet Information", false);
    }
    if (registryKey != null)
    {
      fleetInformation.companyName = (string) registryKey.GetValue("Name", (object) fleetInformation.companyName);
      fleetInformation.address = (string) registryKey.GetValue("Address", (object) fleetInformation.address);
      fleetInformation.city = (string) registryKey.GetValue("City", (object) fleetInformation.city);
      fleetInformation.state = (string) registryKey.GetValue("State", (object) fleetInformation.state);
      fleetInformation.zipCode = (string) registryKey.GetValue("Zip", (object) fleetInformation.zipCode);
      fleetInformation.telephoneNumber = (string) registryKey.GetValue("Phone", (object) fleetInformation.telephoneNumber);
      fleetInformation.offset = (int) registryKey.GetValue("FTZ", (object) fleetInformation.offset);
      registryKey.Close();
    }
    return fleetInformation;
  }

  public static void Save(FleetInformation fleetInformation)
  {
    RegistryKey subKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Detroit Diesel\\Shared\\Fleet Information");
    subKey.SetValue("Name", (object) fleetInformation.companyName, RegistryValueKind.String);
    subKey.SetValue("Address", (object) fleetInformation.address, RegistryValueKind.String);
    subKey.SetValue("City", (object) fleetInformation.city, RegistryValueKind.String);
    subKey.SetValue("State", (object) fleetInformation.state, RegistryValueKind.String);
    subKey.SetValue("Zip", (object) fleetInformation.zipCode, RegistryValueKind.String);
    subKey.SetValue("Phone", (object) fleetInformation.telephoneNumber, RegistryValueKind.String);
    subKey.SetValue("FTZ", (object) fleetInformation.offset, RegistryValueKind.DWord);
    subKey.Close();
  }
}
