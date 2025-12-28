using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using McdAbstraction;

namespace SapiLayer1;

internal static class Extensions
{
	internal static bool CompareNoCase(this string source, string compare)
	{
		return string.Equals(source, compare, StringComparison.OrdinalIgnoreCase);
	}

	internal static decimal ToDecimal(this float source)
	{
		return Convert.ToDecimal(source, CultureInfo.InvariantCulture);
	}

	internal static object ToBoxed(this decimal? source, Type destinationType)
	{
		if (source.HasValue && destinationType != null)
		{
			return Convert.ChangeType(source.Value, destinationType, CultureInfo.InvariantCulture);
		}
		return null;
	}

	internal static string RemoveArguments(this string source)
	{
		int num = source.IndexOf('(');
		if (num != -1)
		{
			source = source.Substring(0, num);
		}
		return source;
	}

	internal static void CopyAllTo(this Stream stream, Stream outputStream)
	{
		long position = (outputStream.Position = 0L);
		stream.Position = position;
		stream.CopyTo(outputStream, 1048576);
		outputStream.Flush();
	}

	internal static void CopyChildrenTo(this XmlReader xmlReader, XmlWriter xmlWriter, string containerNodeName, string itemNodeName)
	{
		if (xmlReader.ReadToFollowing(containerNodeName) && xmlReader.ReadToDescendant(itemNodeName))
		{
			do
			{
				xmlWriter.WriteNode(xmlReader, defattr: false);
			}
			while (xmlReader.ReadToNextSibling(itemNodeName));
		}
	}

	internal static string Reduce(this string source)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in source)
		{
			if (char.IsLetter(c))
			{
				stringBuilder.Append(char.ToUpperInvariant(c));
			}
		}
		return stringBuilder.ToString();
	}

	internal static CultureInfo Neutralize(this CultureInfo culture)
	{
		if (!culture.IsNeutralCulture)
		{
			return culture.Parent;
		}
		return culture;
	}

	internal static string CreateQualifierFromName(this string name)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in name)
		{
			if (char.IsLetterOrDigit(c))
			{
				stringBuilder.Append(c);
			}
			else
			{
				stringBuilder.Append("_");
			}
		}
		return stringBuilder.ToString();
	}

	internal static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		HashSet<TKey> knownKeys = new HashSet<TKey>();
		foreach (TSource item in source)
		{
			if (knownKeys.Add(keySelector(item)))
			{
				yield return item;
			}
		}
	}

	public static XElement ToXElement(this XmlNode node)
	{
		XDocument xDocument = new XDocument();
		using (XmlWriter w = xDocument.CreateWriter())
		{
			node.WriteTo(w);
		}
		return xDocument.Root;
	}

	public static void WriteElementString(this XmlWriter writer, XName name, string value)
	{
		writer.WriteStartElement(name.LocalName);
		writer.WriteValue(value);
		writer.WriteEndElement();
	}

	internal static string ToNumberString(this Enum value)
	{
		return Convert.ToUInt32(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
	}

	internal static string ToHexString(this IList<byte> bytes)
	{
		int num = bytes.Count * 2;
		char[] array = new char[num];
		int num2 = 0;
		int num3 = 0;
		while (num3 < num)
		{
			byte b = bytes[num2++];
			array[num3++] = "0123456789ABCDEF"[b / 16];
			array[num3++] = "0123456789ABCDEF"[b % 16];
		}
		return new string(array);
	}

	internal static T GetValue<T>(this StringDictionary dictionary, string propertyName, T defaultIfNotSet)
	{
		if (dictionary.ContainsKey(propertyName))
		{
			return (T)Convert.ChangeType(dictionary[propertyName], typeof(T), CultureInfo.InvariantCulture);
		}
		return defaultIfNotSet;
	}

	internal static T[] GetCurrentArraySet<T>(this EcuInfo ecuInfo)
	{
		if (ecuInfo != null && ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null && ecuInfo.EcuInfoValues.Current.Value.GetType().IsArray)
		{
			return (ecuInfo.EcuInfoValues.Current.Value as Array).Cast<T>().ToArray();
		}
		return new T[0];
	}

	internal static object GetValue(this McdValue value, Type type, ChoiceCollection choices)
	{
		if (value != null)
		{
			if (value.Value != null && value.Value.GetType() == typeof(byte[]))
			{
				return new Dump((byte[])value.Value);
			}
			if (type == typeof(Choice) && value.CodedValue != null)
			{
				return choices.GetItemFromRawValue(value.CodedValue);
			}
			return value.Value;
		}
		return null;
	}

	internal static IEnumerable<McdDBRequestParameter> GetComParameters(this McdDBLocation location)
	{
		if (location == null)
		{
			yield break;
		}
		McdDBControlPrimitive mcdDBControlPrimitive = location.DBControlPrimitives.FirstOrDefault((McdDBControlPrimitive p) => p.Qualifier == "ProtocolParameterSet");
		if (mcdDBControlPrimitive == null)
		{
			yield break;
		}
		foreach (McdDBRequestParameter allRequestParameter in mcdDBControlPrimitive.AllRequestParameters)
		{
			yield return allRequestParameter;
		}
	}

	internal static bool AreCodingStringsEquivalent(this Dump mask, byte[] codingString1, byte[] codingString2)
	{
		for (int i = 0; i < mask.Data.Count; i++)
		{
			byte b = mask.Data[i];
			if ((byte)(codingString1[i] & b) != (byte)(codingString2[i] & b))
			{
				return false;
			}
		}
		return true;
	}

	internal static Dump CreateCodingStringMask(this IEnumerable<Tuple<int, int>> parameters, int length, bool includeExclude)
	{
		List<bool> list = Enumerable.Repeat(!includeExclude, length * 8).ToList();
		foreach (Tuple<int, int> parameter in parameters)
		{
			for (int i = parameter.Item1; i < parameter.Item1 + parameter.Item2 && i < list.Count; i++)
			{
				list[i] = includeExclude;
			}
		}
		byte[] array = new byte[length];
		for (int j = 0; j < length; j++)
		{
			for (int k = 0; k < 8; k++)
			{
				if (list[j * 8 + k])
				{
					array[j] |= (byte)(1 << k);
				}
			}
		}
		return new Dump(array);
	}

	internal static bool TryParseSourceAddress(this string address, out int sourceAddress)
	{
		bool flag = address.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
		return int.TryParse(address.Substring(flag ? 2 : 0), flag ? NumberStyles.HexNumber : NumberStyles.None, CultureInfo.InvariantCulture, out sourceAddress);
	}

	internal static bool AllSiblingsAreStructures(this IEnumerable<McdDBResponseParameter> responseParameterSet)
	{
		IEnumerable<McdDBResponseParameter> source = responseParameterSet.Where((McdDBResponseParameter rpc) => !rpc.IsConst && !rpc.IsMatchingRequestParameter && !rpc.IsReserved);
		if (source.Count() > 1)
		{
			return source.All((McdDBResponseParameter rpc) => rpc.IsStructure);
		}
		return false;
	}

	internal static bool ByteOrderMatchesSystem(this ByteOrder? byteOrder)
	{
		ByteOrder? byteOrder2 = byteOrder;
		ByteOrder byteOrder3 = ByteOrder.LowHigh;
		if (byteOrder2.GetValueOrDefault() != byteOrder3 || !byteOrder2.HasValue || !BitConverter.IsLittleEndian)
		{
			if (byteOrder == ByteOrder.HighLow)
			{
				return !BitConverter.IsLittleEndian;
			}
			return false;
		}
		return true;
	}
}
