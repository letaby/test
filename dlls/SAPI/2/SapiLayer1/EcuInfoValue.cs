using System;
using System.Xml.Linq;

namespace SapiLayer1;

public sealed class EcuInfoValue
{
	private object value;

	private DateTime time;

	public object Value => value;

	public DateTime Time => time;

	internal EcuInfoValue(object ecuInfoValue, DateTime time)
	{
		value = ecuInfoValue;
		this.time = time;
	}

	public override string ToString()
	{
		return value.ToString();
	}

	internal XElement GetXElement(DateTime startTime)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		DateTime dateTime = ((Time < startTime) ? startTime : Time);
		return new XElement(currentFormat[TagName.Value], Presentation.FormatForLog(Value), new XAttribute(currentFormat[TagName.Time], Sapi.TimeToString(dateTime)));
	}

	internal static EcuInfoValue FromXElement(XElement element, LogFileFormatTagCollection format, EcuInfo parent)
	{
		return new EcuInfoValue((parent.Presentation != null) ? parent.Presentation.ParseFromLog(element.Value) : element.Value, Sapi.TimeFromString(element.Attribute(format[TagName.Time]).Value));
	}
}
