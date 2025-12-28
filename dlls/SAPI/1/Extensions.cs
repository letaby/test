// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Extensions
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

internal static class Extensions
{
  internal static bool CompareNoCase(this string source, string compare)
  {
    return string.Equals(source, compare, StringComparison.OrdinalIgnoreCase);
  }

  internal static Decimal ToDecimal(this float source)
  {
    return Convert.ToDecimal((object) source, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  internal static object ToBoxed(this Decimal? source, Type destinationType)
  {
    return source.HasValue && destinationType != (Type) null ? Convert.ChangeType((object) source.Value, destinationType, (IFormatProvider) CultureInfo.InvariantCulture) : (object) null;
  }

  internal static string RemoveArguments(this string source)
  {
    int length = source.IndexOf('(');
    if (length != -1)
      source = source.Substring(0, length);
    return source;
  }

  internal static void CopyAllTo(this Stream stream, Stream outputStream)
  {
    stream.Position = outputStream.Position = 0L;
    stream.CopyTo(outputStream, 1048576 /*0x100000*/);
    outputStream.Flush();
  }

  internal static void CopyChildrenTo(
    this XmlReader xmlReader,
    XmlWriter xmlWriter,
    string containerNodeName,
    string itemNodeName)
  {
    if (!xmlReader.ReadToFollowing(containerNodeName) || !xmlReader.ReadToDescendant(itemNodeName))
      return;
    do
    {
      xmlWriter.WriteNode(xmlReader, false);
    }
    while (xmlReader.ReadToNextSibling(itemNodeName));
  }

  internal static string Reduce(this string source)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (char c in source)
    {
      if (char.IsLetter(c))
        stringBuilder.Append(char.ToUpperInvariant(c));
    }
    return stringBuilder.ToString();
  }

  internal static CultureInfo Neutralize(this CultureInfo culture)
  {
    return !culture.IsNeutralCulture ? culture.Parent : culture;
  }

  internal static string CreateQualifierFromName(this string name)
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < name.Length; ++index)
    {
      char c = name[index];
      if (char.IsLetterOrDigit(c))
        stringBuilder.Append(c);
      else
        stringBuilder.Append("_");
    }
    return stringBuilder.ToString();
  }

  internal static IEnumerable<TSource> DistinctBy<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector)
  {
    HashSet<TKey> knownKeys = new HashSet<TKey>();
    foreach (TSource source1 in source)
    {
      if (knownKeys.Add(keySelector(source1)))
        yield return source1;
    }
  }

  public static XElement ToXElement(this XmlNode node)
  {
    XDocument xdocument = new XDocument();
    using (XmlWriter writer = xdocument.CreateWriter())
      node.WriteTo(writer);
    return xdocument.Root;
  }

  public static void WriteElementString(this XmlWriter writer, XName name, string value)
  {
    writer.WriteStartElement(name.LocalName);
    writer.WriteValue(value);
    writer.WriteEndElement();
  }

  internal static string ToNumberString(this Enum value)
  {
    return Convert.ToUInt32((object) value, (IFormatProvider) CultureInfo.InvariantCulture).ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  internal static string ToHexString(this IList<byte> bytes)
  {
    int length = bytes.Count * 2;
    char[] chArray1 = new char[length];
    int num1 = 0;
    int num2 = 0;
    while (num2 < length)
    {
      byte num3 = bytes[num1++];
      char[] chArray2 = chArray1;
      int index1 = num2;
      int num4 = index1 + 1;
      int num5 = (int) "0123456789ABCDEF"[(int) num3 / 16 /*0x10*/];
      chArray2[index1] = (char) num5;
      char[] chArray3 = chArray1;
      int index2 = num4;
      num2 = index2 + 1;
      int num6 = (int) "0123456789ABCDEF"[(int) num3 % 16 /*0x10*/];
      chArray3[index2] = (char) num6;
    }
    return new string(chArray1);
  }

  internal static T GetValue<T>(
    this StringDictionary dictionary,
    string propertyName,
    T defaultIfNotSet)
  {
    return dictionary.ContainsKey(propertyName) ? (T) Convert.ChangeType((object) dictionary[propertyName], typeof (T), (IFormatProvider) CultureInfo.InvariantCulture) : defaultIfNotSet;
  }

  internal static T[] GetCurrentArraySet<T>(this EcuInfo ecuInfo)
  {
    return ecuInfo != null && ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null && ecuInfo.EcuInfoValues.Current.Value.GetType().IsArray ? (ecuInfo.EcuInfoValues.Current.Value as Array).Cast<T>().ToArray<T>() : new T[0];
  }

  internal static object GetValue(this McdValue value, Type type, ChoiceCollection choices)
  {
    if (value == null)
      return (object) null;
    if (value.Value != null && value.Value.GetType() == typeof (byte[]))
      return (object) new Dump((IEnumerable<byte>) (byte[]) value.Value);
    return type == typeof (Choice) && value.CodedValue != null ? (object) choices.GetItemFromRawValue(value.CodedValue) : value.Value;
  }

  internal static IEnumerable<McdDBRequestParameter> GetComParameters(this McdDBLocation location)
  {
    if (location != null)
    {
      McdDBControlPrimitive controlPrimitive = location.DBControlPrimitives.FirstOrDefault<McdDBControlPrimitive>((Func<McdDBControlPrimitive, bool>) (p => p.Qualifier == "ProtocolParameterSet"));
      if (controlPrimitive != null)
      {
        foreach (McdDBRequestParameter requestParameter in controlPrimitive.AllRequestParameters)
          yield return requestParameter;
      }
    }
  }

  internal static bool AreCodingStringsEquivalent(
    this Dump mask,
    byte[] codingString1,
    byte[] codingString2)
  {
    for (int index = 0; index < mask.Data.Count; ++index)
    {
      byte num = mask.Data[index];
      if ((int) (byte) ((uint) codingString1[index] & (uint) num) != (int) (byte) ((uint) codingString2[index] & (uint) num))
        return false;
    }
    return true;
  }

  internal static Dump CreateCodingStringMask(
    this IEnumerable<Tuple<int, int>> parameters,
    int length,
    bool includeExclude)
  {
    List<bool> list = Enumerable.Repeat<bool>(!includeExclude, length * 8).ToList<bool>();
    using (IEnumerator<Tuple<int, int>> enumerator = parameters.GetEnumerator())
    {
label_5:
      while (enumerator.MoveNext())
      {
        Tuple<int, int> current = enumerator.Current;
        int index = current.Item1;
        while (true)
        {
          if (index < current.Item1 + current.Item2 && index < list.Count)
          {
            list[index] = includeExclude;
            ++index;
          }
          else
            goto label_5;
        }
      }
    }
    byte[] data = new byte[length];
    for (int index1 = 0; index1 < length; ++index1)
    {
      for (int index2 = 0; index2 < 8; ++index2)
      {
        if (list[index1 * 8 + index2])
          data[index1] |= (byte) (1 << index2);
      }
    }
    return new Dump((IEnumerable<byte>) data);
  }

  internal static bool TryParseSourceAddress(this string address, out int sourceAddress)
  {
    bool flag = address.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
    return int.TryParse(address.Substring(flag ? 2 : 0), flag ? NumberStyles.HexNumber : NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out sourceAddress);
  }

  internal static bool AllSiblingsAreStructures(
    this IEnumerable<McdDBResponseParameter> responseParameterSet)
  {
    IEnumerable<McdDBResponseParameter> source = responseParameterSet.Where<McdDBResponseParameter>((Func<McdDBResponseParameter, bool>) (rpc => !rpc.IsConst && !rpc.IsMatchingRequestParameter && !rpc.IsReserved));
    return source.Count<McdDBResponseParameter>() > 1 && source.All<McdDBResponseParameter>((Func<McdDBResponseParameter, bool>) (rpc => rpc.IsStructure));
  }

  internal static bool ByteOrderMatchesSystem(this ByteOrder? byteOrder)
  {
    ByteOrder? nullable1 = byteOrder;
    ByteOrder byteOrder1 = ByteOrder.LowHigh;
    if ((nullable1.GetValueOrDefault() == byteOrder1 ? (nullable1.HasValue ? 1 : 0) : 0) != 0 && BitConverter.IsLittleEndian)
      return true;
    ByteOrder? nullable2 = byteOrder;
    ByteOrder byteOrder2 = ByteOrder.HighLow;
    return (nullable2.GetValueOrDefault() == byteOrder2 ? (nullable2.HasValue ? 1 : 0) : 0) != 0 && !BitConverter.IsLittleEndian;
  }
}
