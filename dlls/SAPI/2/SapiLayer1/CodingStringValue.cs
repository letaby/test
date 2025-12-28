using System;
using System.Xml;
using System.Xml.Linq;

namespace SapiLayer1;

public sealed class CodingStringValue
{
	private Dump value;

	private DateTime time;

	public Dump Value => value;

	public DateTime Time => time;

	internal CodingStringValue(Dump codingStringValue, DateTime time)
	{
		value = codingStringValue;
		this.time = time;
	}

	public override string ToString()
	{
		return value.ToString();
	}

	internal void WriteXmlTo(DateTime startTime, XmlWriter writer)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		DateTime dateTime = ((Time < startTime) ? startTime : Time);
		writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.Time].LocalName, Sapi.TimeToString(dateTime));
		writer.WriteValue(Value.ToString());
		writer.WriteEndElement();
	}

	internal static CodingStringValue FromXElement(XElement element, LogFileFormatTagCollection format)
	{
		return new CodingStringValue(new Dump(element.Value), Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value));
	}
}
