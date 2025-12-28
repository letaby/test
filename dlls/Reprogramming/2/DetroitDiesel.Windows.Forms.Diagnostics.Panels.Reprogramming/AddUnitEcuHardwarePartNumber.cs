using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DetroitDiesel.Common;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class AddUnitEcuHardwarePartNumber
{
	private string ecuName;

	private Part ecuPartNumber;

	private string ecuPartNumberDisplayValue;

	private static Collection<string> ecuNamesAvailable;

	[Browsable(false)]
	public bool FormatPartNumber { get; set; }

	public string EcuName
	{
		get
		{
			return ecuName;
		}
		set
		{
			if (!value.Equals(ecuName, StringComparison.OrdinalIgnoreCase))
			{
				ecuName = value.ToUpperInvariant();
				EcuPartNumberDisplayValue = EcuPartNumberDisplayValue;
			}
		}
	}

	public string EcuPartNumberDisplayValue
	{
		get
		{
			return ecuPartNumberDisplayValue;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (FormatPartNumber)
				{
					ecuPartNumber = new Part(value);
					ecuPartNumberDisplayValue = PartExtensions.ToHardwarePartNumberString(ecuPartNumber, ecuName, true);
				}
				else
				{
					ecuPartNumber = null;
					ecuPartNumberDisplayValue = value;
				}
			}
			else
			{
				ecuPartNumber = null;
				ecuPartNumberDisplayValue = string.Empty;
			}
		}
	}

	[Browsable(false)]
	public Part EcuPartNumber
	{
		get
		{
			return ecuPartNumber;
		}
		set
		{
			ecuPartNumber = value;
		}
	}

	public static Collection<string> EcuNamesAvailable
	{
		get
		{
			if (ecuNamesAvailable == null)
			{
				ecuNamesAvailable = new Collection<string>();
				foreach (Ecu ecu in SapiManager.GlobalInstance.Sapi.Ecus)
				{
					if ((!ecu.IsRollCall || SapiManager.SupportsRollCallParameterization(ecu)) && !ecu.IsVirtual && (SapiExtensions.IsDataSourceDepot(ecu) || SapiExtensions.IsDataSourceEdex(ecu)))
					{
						ecuNamesAvailable.Add(ecu.Name);
					}
				}
			}
			return ecuNamesAvailable;
		}
	}

	public AddUnitEcuHardwarePartNumber()
	{
		ecuName = string.Empty;
		ecuPartNumber = null;
	}

	public AddUnitEcuHardwarePartNumber(string name, string partNumber, bool formatPartNumber)
	{
		FormatPartNumber = formatPartNumber;
		ecuName = name;
		EcuPartNumberDisplayValue = partNumber;
	}
}
