using System.Collections.Generic;
using Microsoft.Win32;

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

	public static ICollection<FleetTimeZone> TimeZones
	{
		get
		{
			CreateTimeZoneList();
			return timeZones.Values;
		}
	}

	public string CompanyName
	{
		get
		{
			return companyName;
		}
		set
		{
			companyName = NormalizeString(value, 30);
		}
	}

	public string Address
	{
		get
		{
			return address;
		}
		set
		{
			address = NormalizeString(value, 30);
		}
	}

	public string City
	{
		get
		{
			return city;
		}
		set
		{
			city = NormalizeString(value, 30);
		}
	}

	public string State
	{
		get
		{
			return state;
		}
		set
		{
			state = NormalizeString(value, 2);
		}
	}

	public string ZipCode
	{
		get
		{
			return zipCode;
		}
		set
		{
			zipCode = NormalizeString(value, 10);
		}
	}

	public string TelephoneNumber
	{
		get
		{
			return telephoneNumber;
		}
		set
		{
			telephoneNumber = NormalizeString(value, 20);
		}
	}

	public FleetTimeZone TimeZone
	{
		get
		{
			return GetFleetTimeZoneFromOffset(offset);
		}
		set
		{
			offset = (int)value.Offset.TotalSeconds;
		}
	}

	public static FleetTimeZone DefaultTimeZone => GetFleetTimeZoneFromOffset(-18000);

	private static void CreateTimeZoneList()
	{
		if (timeZones == null)
		{
			timeZones = new Dictionary<int, FleetTimeZone>(25);
			timeZones.Add(-43200, new FleetTimeZone("(GMT-12.00) Dateline", -12));
			timeZones.Add(-39600, new FleetTimeZone("(GMT-11.00) Samoa", -11));
			timeZones.Add(-36000, new FleetTimeZone("(GMT-10.00) Hawaii", -10));
			timeZones.Add(-32400, new FleetTimeZone("(GMT-09.00) Alaska", -9));
			timeZones.Add(-28800, new FleetTimeZone("(GMT-08.00) Pacific Time", -8));
			timeZones.Add(-25200, new FleetTimeZone("(GMT-07.00) Mountain", -7));
			timeZones.Add(-21600, new FleetTimeZone("(GMT-06.00) Central", -6));
			timeZones.Add(-18000, new FleetTimeZone("(GMT-05.00) Eastern Standard Time", -5));
			timeZones.Add(-14400, new FleetTimeZone("(GMT-04.00) Atlantic", -4));
			timeZones.Add(-10800, new FleetTimeZone("(GMT-03.00) Eastern South America", -3));
			timeZones.Add(-7200, new FleetTimeZone("(GMT-02.00) Mid Atlantic", -2));
			timeZones.Add(-3600, new FleetTimeZone("(GMT-01.00) Azores", -1));
			timeZones.Add(0, new FleetTimeZone("GMT", 0));
			timeZones.Add(3600, new FleetTimeZone("(GMT+01.00) Western Europe", 1));
			timeZones.Add(7200, new FleetTimeZone("(GMT+02.00) Eastern Europe", 2));
			timeZones.Add(10800, new FleetTimeZone("(GMT+03.00) Russian", 3));
			timeZones.Add(14400, new FleetTimeZone("(GMT+04.00) Arabian", 4));
			timeZones.Add(18000, new FleetTimeZone("(GMT+05.00) West Asia", 5));
			timeZones.Add(21600, new FleetTimeZone("(GMT+06.00) Central Asia", 6));
			timeZones.Add(25200, new FleetTimeZone("(GMT+07.00) Bangkok", 7));
			timeZones.Add(28800, new FleetTimeZone("(GMT+08.00) China, Perth", 8));
			timeZones.Add(32400, new FleetTimeZone("(GMT+09.00) Tokyo, Adelaide", 9));
			timeZones.Add(36000, new FleetTimeZone("(GMT+10.00) Sydney", 10));
			timeZones.Add(39600, new FleetTimeZone("(GMT+11.00) Central Pacific", 11));
			timeZones.Add(43200, new FleetTimeZone("(GMT+12.00) New Zealand", 12));
		}
	}

	private static FleetTimeZone GetFleetTimeZoneFromOffset(int offsetInSeconds)
	{
		CreateTimeZoneList();
		FleetTimeZone value = null;
		timeZones.TryGetValue(offsetInSeconds, out value);
		return value;
	}

	private FleetInformation()
	{
	}

	private static string NormalizeString(string value, int maxLength)
	{
		string result = string.Empty;
		if (!string.IsNullOrEmpty(value))
		{
			result = ((value.Length > maxLength) ? value.Substring(0, maxLength) : value);
		}
		return result;
	}

	public static FleetInformation Load()
	{
		FleetInformation fleetInformation = new FleetInformation();
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Detroit Diesel\\Shared\\Fleet Information", writable: false);
		if (registryKey == null || registryKey.GetValue("Name") == null)
		{
			registryKey?.Close();
			registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Detroit Diesel\\Shared\\Fleet Information", writable: false);
		}
		if (registryKey != null)
		{
			fleetInformation.companyName = (string)registryKey.GetValue("Name", fleetInformation.companyName);
			fleetInformation.address = (string)registryKey.GetValue("Address", fleetInformation.address);
			fleetInformation.city = (string)registryKey.GetValue("City", fleetInformation.city);
			fleetInformation.state = (string)registryKey.GetValue("State", fleetInformation.state);
			fleetInformation.zipCode = (string)registryKey.GetValue("Zip", fleetInformation.zipCode);
			fleetInformation.telephoneNumber = (string)registryKey.GetValue("Phone", fleetInformation.telephoneNumber);
			fleetInformation.offset = (int)registryKey.GetValue("FTZ", fleetInformation.offset);
			registryKey.Close();
		}
		return fleetInformation;
	}

	public static void Save(FleetInformation fleetInformation)
	{
		RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Detroit Diesel\\Shared\\Fleet Information");
		registryKey.SetValue("Name", fleetInformation.companyName, RegistryValueKind.String);
		registryKey.SetValue("Address", fleetInformation.address, RegistryValueKind.String);
		registryKey.SetValue("City", fleetInformation.city, RegistryValueKind.String);
		registryKey.SetValue("State", fleetInformation.state, RegistryValueKind.String);
		registryKey.SetValue("Zip", fleetInformation.zipCode, RegistryValueKind.String);
		registryKey.SetValue("Phone", fleetInformation.telephoneNumber, RegistryValueKind.String);
		registryKey.SetValue("FTZ", fleetInformation.offset, RegistryValueKind.DWord);
		registryKey.Close();
	}
}
