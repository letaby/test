using System;
using System.Globalization;
using System.Linq;

namespace SapiLayer1;

public sealed class Part
{
	public const int PartNumberLength = 11;

	private readonly string number;

	private readonly object version;

	private bool freightlinerHardwareHasAPrefix;

	private bool isFreightlinerHardware;

	public string Number => number;

	public object Version => version;

	public bool FreightlinerHardwareHasAPrefix => freightlinerHardwareHasAPrefix;

	public bool IsFreightlinerHardware => isFreightlinerHardware;

	public Part(string number)
	{
		DetermineFormatting(number);
		this.number = Strip(number);
		if (this.number.Length > 11)
		{
			string value = this.number.Substring(11);
			try
			{
				version = Convert.ToUInt32(value, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(number, "Error parsing the part version - not a supported format of part version read");
			}
			this.number = this.number.Substring(0, 11);
		}
	}

	public Part(string number, int version)
	{
		DetermineFormatting(number);
		this.number = Strip(number);
		this.version = Convert.ToUInt32(version, CultureInfo.InvariantCulture);
	}

	public Part(string number, object version)
	{
		DetermineFormatting(number);
		this.number = Strip(number);
		try
		{
			this.version = Convert.ToUInt32(version, CultureInfo.InvariantCulture);
		}
		catch (FormatException)
		{
			Sapi.GetSapi().RaiseDebugInfoEvent(number, "Error parsing the part version - not a supported format of part version read");
		}
	}

	private void DetermineFormatting(string number)
	{
		if (number.Count((char c) => c == '-') == 2)
		{
			isFreightlinerHardware = true;
			if (number.StartsWith("A", StringComparison.OrdinalIgnoreCase))
			{
				freightlinerHardwareHasAPrefix = true;
			}
		}
	}

	public override string ToString()
	{
		if (version != null)
		{
			uint num = (uint)version;
			return string.Format(CultureInfo.InvariantCulture, "{0}-{1,2:000}", number, num);
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}", number);
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj != null)
		{
			Part part = (Part)obj;
			if (part.Version != null)
			{
				return string.Equals(ToString(), obj.ToString());
			}
			return string.Equals(Number, part.Number);
		}
		return false;
	}

	private static string Strip(string input)
	{
		string text = input.Replace(" ", string.Empty);
		text = text.Replace("A", string.Empty);
		text = text.Replace("_", string.Empty);
		text = text.Replace("-", string.Empty);
		text = text.Replace("ZGS", string.Empty);
		return string.Format(CultureInfo.InvariantCulture, "A{0}", text);
	}
}
