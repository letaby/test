using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SapiLayer1;

public sealed class Dump
{
	private IList<byte> data;

	public IList<byte> Data => data;

	public Dump(string source)
	{
		if (source != null)
		{
			if (source.Length % 2 == 1)
			{
				source = "0" + source;
			}
			int num = source.Length / 2;
			List<byte> list = new List<byte>();
			for (int i = 0; i < num; i++)
			{
				string value = source.Substring(i * 2, 2);
				list.Add(Convert.ToByte(value, 16));
			}
			data = list.AsReadOnly();
		}
	}

	public Dump(IEnumerable<byte> data)
	{
		if (data != null)
		{
			this.data = new List<byte>(data).AsReadOnly();
		}
	}

	public override string ToString()
	{
		if (data != null)
		{
			return data.ToHexString();
		}
		return string.Empty;
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj != null)
		{
			return string.Equals(ToString(), obj.ToString());
		}
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("The GetData method is deprecated, please use the Data property instead.")]
	public byte[] GetData()
	{
		return data.ToArray();
	}

	public static bool MaskedMatch(byte[] content, Dump pattern, Dump mask)
	{
		int num = mask?.data.Count ?? pattern.data.Count;
		if (content.Length < num || num == 0)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (mask != null)
			{
				if ((mask.data[i] & content[i]) != pattern.data[i])
				{
					return false;
				}
			}
			else if (content[i] != pattern.data[i])
			{
				return false;
			}
		}
		return true;
	}
}
