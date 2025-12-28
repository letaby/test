using System;
using System.Xml;
using System.Xml.Linq;

namespace SapiLayer1;

public sealed class ParameterValue
{
	private object value;

	private DateTime time;

	public object Value => value;

	public DateTime Time => time;

	internal ParameterValue(object ParameterValue, DateTime time)
	{
		value = ParameterValue;
		this.time = time;
	}

	public override string ToString()
	{
		return value.ToString();
	}

	internal void WriteXmlTo(DateTime startTime, Parameter parent, XmlWriter writer)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		DateTime dateTime = ((Time < startTime) ? startTime : Time);
		writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.Time].LocalName, Sapi.TimeToString(dateTime));
		writer.WriteValue(parent.FormatValue(Value));
		writer.WriteEndElement();
	}

	internal static ParameterValue FromXElement(XElement element, LogFileFormatTagCollection format, Parameter parent)
	{
		return new ParameterValue(parent.ParseValue(element.Value), Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value));
	}
}
